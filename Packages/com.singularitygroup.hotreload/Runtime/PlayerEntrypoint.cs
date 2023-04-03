#if UNITY_ANDROID && !UNITY_EDITOR
#define MOBILE_ANDROID
#endif
#if UNITY_IOS && !UNITY_EDITOR
#define MOBILE_IOS
#endif
#if MOBILE_ANDROID || MOBILE_IOS
#define MOBILE
#endif

using System;
using System.Threading.Tasks;
#if MOBILE_ANDROID
// not able to use File apis for reading from StreamingAssets
using UnityEngine.Networking;
#endif
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.IO;

namespace SingularityGroup.HotReload {
    // entrypoint for Unity Player builds. Not necessary in Unity Editor.
    internal static class PlayerEntrypoint {
        /// Set when behaviour is created, when you access this instance through the singleton,
        /// you can assume that this field is not null.
        /// <remarks>
        /// In Player code you can assume this is set.<br/>
        /// When in Editor this is usually null.
        /// </remarks>
        static BuildInfo buildInfo { get; set; }

        /// In Player code you can assume this is set (not null)
        public static BuildInfo PlayerBuildInfo => buildInfo;

        #if ENABLE_MONO
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        #endif
        private static void InitOnAppLoad() {
            AppCallbackListener.Init(); // any platform might be using this
            UnityHelper.Init();
            bool onlyBuildSettingsNotSupported;
            if (!IsPlayerWithHotReload(out onlyBuildSettingsNotSupported)) {
                if (onlyBuildSettingsNotSupported) {
                    // standalone/desktop builds not supported yet
                    #if UNITY_ANDROID
                    Log.Warning("Hot Reload is not available in this build because one or more build settings were not supported.");
                    #endif
                }
                return;
            }

            // beginning a UWR only works on main thread
            var jsonTask = ReadTextFromStreamingAssets(BuildInfo.GetStoredPath());
            
            Task.Run(async () => {
                try {
                    // Read build info on a worker thread (instead of blocking Unity Main thread)
                    var json = await jsonTask;
                    buildInfo = BuildInfo.FromJson(json);
                } catch (IOException ex) {
                    // ignored in editor (no build info makes sense)
                    if (IsPlayer()) {
                        Log.Exception(ex);
                    } else {
                        buildInfo = null;
                    }
                }
                try {
                    // switch back to main thread and init singleton
                    await ThreadUtility.SwitchToMainThread();
                    // Check if a deeplink launched the app
                    DeepLinkHandler.SetupDeepLinkHandling((serverInfo) => {
                        if (Prompts.IsShowingRetryDialog) {
                            // hide it to indicate to user that we are handling it
                            Prompts.HideRetryDialog();
                        }
                        TryConnect(serverInfo).Forget();
                    });

                    // Check if a deeplink launched the app
                    if (DeepLinkHandler.TryHandleLaunchDeepLink()) {
                        // we're trying to connect to server from the deeplink
                        return;
                    }

                    if (IsPlayer()) { // use if (true) to test in editor
                        // ReSharper disable once PossibleNullReferenceException
                        if (buildInfo.BuildMachineServer == null) {
                            Prompts.ShowRetryDialog(null);
                        } else {
                            // try reach server running on the build machine.
                            await TryConnect(buildInfo.BuildMachineServer);
                        }
                    }
                } catch (Exception ex) {
                    Log.Exception(ex);
                }
            });
        }

        public static async Task TryConnect(PatchServerInfo serverInfo) {
            // try reach server running on the build machine.
            var handshake = PlayerCodePatcher.UpdateHost(serverInfo);
            await Task.WhenAny(handshake, Task.Delay(TimeSpan.FromSeconds(40)));
            await ThreadUtility.SwitchToMainThread();
            var handshakeResults = await handshake;
            var handshakeOk = handshakeResults.HasFlag(ServerHandshake.Result.Verified);
            if (!handshakeOk) {
                Log.Debug("ShowRetryPrompt because handshake result is {0}", handshakeResults);
                Prompts.ShowRetryDialog(serverInfo, handshakeResults);
                // cancel trying to connect. They can use the retry button
                PlayerCodePatcher.UpdateHost(null).Forget();
            }

            Log.Info($"Server is healthy after first handshake? {handshakeOk}");
        }

        /// on Android, streaming assets are inside apk zip, which can only be read using unity web request
        private static Task<string> ReadTextFromStreamingAssets(string path) {
            return Task.Run(() => {
                #if MOBILE_ANDROID
                var relativePath = path.Replace(Application.streamingAssetsPath, "").TrimStart('/', '\\');
                AndroidJNI.AttachCurrentThread();
                try {
                    var contents = UnityAndroidHelpers.ReadTextFromApkAssets(relativePath);
                    if (string.IsNullOrEmpty(contents)) {
                        throw new IOException($"ReadTextFromStreamingAssets failed to read (or the asset file is actually empty) {relativePath}");
                    }

                    return contents;
                } finally {
                    AndroidJNI.DetachCurrentThread();
                }
                #else
                return File.ReadAllText(path);
                #endif
            });
        }

        public static bool IsPlayer() => !Application.isEditor;

        public static bool IsPlayerWithHotReload() {
            bool _;
            return IsPlayerWithHotReload(out _);
        }

        public static bool IsPlayerWithHotReload(out bool onlyBuildSettingsNotSupported) {
            if (IsPlayer()) {
                // When a build setting is not supported, Hot Reload prefab is not included in the build.
                if (!RuntimeSupportsHotReload || !HotReloadSettingsObject.I.IncludeInBuild) {
                    onlyBuildSettingsNotSupported = false;
                    return false;
                }
                onlyBuildSettingsNotSupported = !HotReloadSettingsObject.I.PromptsPrefab;
                return !onlyBuildSettingsNotSupported;
            } else {
                // In the Unity Editor (Playmode), Hot Reload takes a different route
                onlyBuildSettingsNotSupported = false;
                return false;
            }
        }
        
        public static bool RuntimeSupportsHotReload {
            get {
                #if ENABLE_MONO
                // We try to support any platform that uses the mono scripting backend.
                // This includes Playmode in the Unity Editor, Android, Standalone MacOS/Windows/Linux, ...
                // Only development builds are supported - we exclude some CodePatcher things from release builds.
                return Debug.isDebugBuild;
                #else
                return false;
                #endif
            }
        }
    }
}