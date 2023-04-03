using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SingularityGroup.HotReload.Editor.Cli;
using SingularityGroup.HotReload.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    internal class ServerDownloader : IProgress<float> {
        public float Progress {get; private set;}
        public bool Started {get; private set;}

        class Config {
            public Dictionary<string, string> customServerExecutables;
        }
        
        public string GetExecutablePath(ICliController cliController) {
            var targetDir = CliUtils.GetExecutableTargetDir();
            var targetPath = Path.Combine(targetDir, cliController.BinaryFileName);
            return targetPath;
        }
        
        public bool IsDownloaded(ICliController cliController) {
            return File.Exists(GetExecutablePath(cliController));
        }
        
        public bool CheckIfDownloaded(ICliController cliController) {
            if(IsDownloaded(cliController)) {
                Started = true;
                Progress = 1f;
                return true;
            } else if(TryUseUserDefinedBinaryPath(cliController, GetExecutablePath(cliController))) {
                Started = true;
                Progress = 1f;
                return true;
            } else {
                Started = false;
                Progress = 0f;
                return false;
            }
        }
        
        public async Task EnsureDownloaded(ICliController cliController, CancellationToken cancellationToken) {
            var targetDir = CliUtils.GetExecutableTargetDir();
            var targetPath = Path.Combine(targetDir, cliController.BinaryFileName);
            Started = true;
            if(File.Exists(targetPath)) {
                Progress = 1f;
                return;
            }
            Progress = 0f;
            await ThreadUtility.SwitchToThreadPool(cancellationToken);

            Directory.CreateDirectory(targetDir);
            if(TryUseUserDefinedBinaryPath(cliController, targetPath)) {
                Progress = 1f;
                return;
            }

            var tmpPath = CliUtils.GetTempDownloadFilePath("Server.tmp");
            var attempt = 0;
            bool sucess = false;
            while(!sucess) {
                try {
                    if(File.Exists(targetPath)) {
                        Progress = 1f;
                        return;
                    }
                    var result = await DownloadUtility.DownloadFile(GetDownloadUrl(cliController), tmpPath, this, cancellationToken);
                    sucess = result.statusCode == HttpStatusCode.OK;
                } catch {
                    //ignored
                }
                if(!sucess) {
                    await Task.Delay(ExponentialBackoff.GetTimeout(attempt), cancellationToken).ConfigureAwait(false);
                }
                attempt++;
            }
            
            const int ERROR_ALREADY_EXISTS = 0xB7;
            try {
                File.Move(tmpPath, targetPath);
            } catch(IOException ex) when((ex.HResult & 0x0000FFFF) == ERROR_ALREADY_EXISTS) {
                //another downloader came first
                try {
                    File.Delete(tmpPath); 
                } catch {
                    //ignored 
                }
            }
            Progress = 1f;
        }

        static bool TryUseUserDefinedBinaryPath(ICliController cliController, string targetPath) {
            if (!File.Exists(PackageConst.ConfigFileName)) {
                return false;
            } 
            
            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(PackageConst.ConfigFileName));
            var customExecutables = config?.customServerExecutables;
            if (customExecutables == null) {
                return false;
            }

            string customBinaryPath;
            if(!customExecutables.TryGetValue(cliController.PlatformName, out customBinaryPath)) {
                return false;
            }
            
            if (!File.Exists(customBinaryPath)) {
                Log.Warning($"unable to find server binary for platform '{cliController.PlatformName}' at '{customBinaryPath}'. " +
                            $"Will proceed with downloading the binary (default behavior)");
                return false;
            } 
            
            try {
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                File.Copy(customBinaryPath, targetPath);
                return true;
            } catch(IOException ex) {
                Log.Warning("encountered exception when copying server binary in the specified custom executable path '{0}':\n{1}", customBinaryPath, ex);
                return false;
            }
        }

        static string GetDownloadUrl(ICliController cliController) {
            const string version = PackageConst.ServerVersion;
            var key = $"{DownloadUtility.GetPackagePrefix(version)}/server/{cliController.PlatformName}/{cliController.BinaryFileName}";
            return DownloadUtility.GetDownloadUrl(key);
        }

        void IProgress<float>.Report(float value) {
            Progress = value;
        }
        
        public void PromptForDownload() {
            if(EditorUtility.DisplayDialog("Installing platform specific components",
                InstallDescription,
                "Install",
#if UNITY_2019_4_OR_NEWER
                DialogOptOutDecisionType.ForThisMachine, 
#endif
                HotReloadPrefs.DontShowPromptForDownloadKey)) {
                EnsureDownloaded(HotReloadCli.controller, CancellationToken.None).Forget();
            }
        }
        
        public const string InstallDescription = "For Hot Reload to work, additional components specific to your operating system have to be installed";
    }
    
    class DownloadResult {
        public readonly HttpStatusCode statusCode;
        public readonly string error;
        public DownloadResult(HttpStatusCode statusCode, string error) {
            this.statusCode = statusCode;
            this.error = error;
        }
    }
}
