using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace xasset.editor
{
    public static class BuildScript
    {
        public static Action<BuildTask> postprocessBuildBundles { get; set; }
        public static Action<BuildTask> preprocessBuildBundles { get; set; }

        public static void BuildBundles(BuildTask task)
        {
            preprocessBuildBundles?.Invoke(task);
            task.Run();
            postprocessBuildBundles?.Invoke(task);
        }

        public static void BuildBundles()
        {
            BuildBundles(new BuildTask());
        }

        public static string GetTimeForNow()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private static string GetBuildTargetName(BuildTarget target)
        {
            var targetName = $"/{PlayerSettings.productName}-v{PlayerSettings.bundleVersion}-{GetTimeForNow()}";
            switch (target)
            {
                case BuildTarget.Android:
                    return targetName + ".apk";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return targetName + ".exe";
                case BuildTarget.StandaloneOSX:
                    return targetName + ".app";
                default:
                    return targetName;
            }
        }

        public static void BuildPlayer(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = $"Players/{Settings.GetPlatformName()}";
            }

            var levels = new List<string>();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    levels.Add(scene.path);
                }
            }

            if (levels.Count == 0)
            {
                Debug.Log("Nothing to build.");
                return;
            }

            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var buildTargetName = GetBuildTargetName(buildTarget);
            if (buildTargetName == null)
            {
                return;
            }

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = levels.ToArray(),
                locationPathName = path + buildTargetName,
                target = buildTarget,
                options = EditorUserBuildSettings.development
                    ? BuildOptions.Development
                    : BuildOptions.None
            };
            BuildPipeline.BuildPlayer(buildPlayerOptions);
            UnityEditor.EditorUtility.OpenWithDefaultApp(path);
        }

        public static void CopyToStreamingAssets()
        {
            var settings = Settings.GetDefaultSettings();
            var destinationDir = Settings.BuildPlayerDataPath;
            if (Directory.Exists(destinationDir))
            {
                Directory.Delete(destinationDir, true);
            }

            Directory.CreateDirectory(destinationDir);
            var versions = BuildVersions.Load(Settings.GetBuildPath(Versions.Filename));
            var bundles = settings.GetBundlesInBuild(versions);
            foreach (var bundle in bundles)
            {
                Copy(bundle.nameWithAppendHash, destinationDir);
            }

            foreach (var build in versions.data)
            {
                Copy(build.file, destinationDir);
            }

            versions.streamingAssets = bundles.ConvertAll(o => o.nameWithAppendHash);
            versions.offlineMode = settings.scriptPlayMode != ScriptPlayMode.Increment;
            File.WriteAllText($"{destinationDir}/{Versions.Filename}", JsonUtility.ToJson(versions));
        }

        private static void Copy(string filename, string destinationDir)
        {
            var from = Settings.GetBuildPath(filename);
            if (File.Exists(from))
            {
                var dest = $"{destinationDir}/{filename}";
                File.Copy(from, dest, true);
            }
            else
            {
                Debug.LogErrorFormat("File not found: {0}", from);
            }
        }

        public static void ClearBuildFromSelection()
        {
            var filtered = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            var assetPaths = new List<string>();
            foreach (var o in filtered)
            {
                var assetPath = AssetDatabase.GetAssetPath(o);
                if (string.IsNullOrEmpty(assetPath))
                {
                    continue;
                }

                assetPaths.Add(assetPath);
            }

            var bundles = new List<string>();
            var versions = BuildVersions.Load(Settings.GetBuildPath(Versions.Filename));
            foreach (var version in versions.data)
            {
                var manifest = Manifest.LoadFromFile(Settings.GetBuildPath(version.file));
                foreach (var assetPath in assetPaths)
                {
                    var bundle = manifest.GetBundle(assetPath);
                    if (bundle != null)
                    {
                        bundles.Add(bundle.nameWithAppendHash);
                    }
                }
            }

            foreach (var bundle in bundles)
            {
                var file = Settings.GetBuildPath(bundle);
                if (!File.Exists(file))
                {
                    continue;
                }

                File.Delete(file);
                Debug.LogFormat("Delete:{0}", file);
            }
        }

        public static void ClearHistory()
        {
            var usedFiles = new List<string>
            {
                Settings.GetPlatformName(),
                Settings.GetPlatformName() + ".manifest",
                Versions.Filename
            };

            var versions = BuildVersions.Load(Settings.GetBuildPath(Versions.Filename));
            foreach (var version in versions.data)
            {
                usedFiles.Add(version.file);
                usedFiles.Add(version.name + ".bin");
                var manifest = Manifest.LoadFromFile(Settings.GetBuildPath(version.file));
                foreach (var bundle in manifest.bundles)
                {
                    usedFiles.Add(bundle.name);
                    usedFiles.Add($"{bundle.name}.manifest");
                    usedFiles.Add(bundle.nameWithAppendHash);
                }
            }

            var files = Directory.GetFiles(Settings.PlatformBuildPath);
            foreach (var file in files)
            {
                var name = Path.GetFileName(file);
                if (usedFiles.Contains(name))
                {
                    continue;
                }

                File.Delete(file);
                Debug.LogFormat("Delete {0}", file);
            }
        }

        public static void ClearBuild()
        {
            if (!UnityEditor.EditorUtility.DisplayDialog("提示", "清理构建数据将无法正常增量打包，确认清理？", "确定"))
            {
                return;
            }

            var buildPath = Settings.PlatformBuildPath;
            Directory.Delete(buildPath, true);
        }
    }
}