using System;
using System.IO;
using UnityEditor;

namespace SingularityGroup.HotReload.Editor {
    internal static class HotReloadBuildHelper {
        /// <summary>
        /// Should HotReload runtime be included in the current build?
        /// </summary>
        public static bool IncludeInThisBuild() {
            //On device functionality is currently not good enough for production
            return false;
        }

        /// <summary>
        /// Get scripting backend for the current platform.
        /// </summary>
        /// <returns>Scripting backend</returns>
        public static ScriptingImplementation GetCurrentScriptingBackend() {
#pragma warning disable CS0618
            return PlayerSettings.GetScriptingBackend(EditorUserBuildSettings
                .selectedBuildTargetGroup);
#pragma warning restore CS0618
        }

        public static void SetCurrentScriptingBackend(ScriptingImplementation to) {
#pragma warning disable CS0618
            // only set it if default is not correct (avoid changing ProjectSettings when not needed)
            if (GetCurrentScriptingBackend() != to) {
                PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, to);
            }
#pragma warning restore CS0618
        }

        /// Is the current build target supported?
        /// main thread only
        public static bool IsBuildTargetSupported() {
            var buildTarget = EditorUserBuildSettings.selectedBuildTargetGroup;  
            return Array.IndexOf(unsupportedBuildTargets, buildTarget) == -1;
        }
        
        /// Are all the settings supported?
        /// main thread only
        static bool IsAllBuildSettingsSupported() {
            if (!IsBuildTargetSupported()) {
                return false;
            }

            // need way to give it settings object, dont want to give serializedobject
            var options = HotReloadSettingsEditor.LoadSettingsOrDefault();
            var so = new SerializedObject(options);
            
            // check all projeect options
            foreach (var option in HotReloadOnDeviceTab.allOptions) {
                var projectOption = option as ProjectOptionBase;
                if (projectOption != null) {
                    // if option is required, build can't use hot reload
                    if (projectOption.IsRequiredForBuild() && !projectOption.GetValue(so)) {
                        return false;
                    }
                }
            }

            return GetCurrentScriptingBackend() == ScriptingImplementation.Mono2x
                   && EditorUserBuildSettings.development;
        }

        /// <summary>
        /// Some platforms are not supported because they don't have Mono scripting backend.
        /// </summary>
        /// <remarks>
        /// Only list the platforms that definately don't have Mono scripting.
        /// </remarks>
        private static readonly BuildTargetGroup[] unsupportedBuildTargets = new BuildTargetGroup[] {
            BuildTargetGroup.iOS, // mono support was removed many years ago
            BuildTargetGroup.WebGL, // has never had mono
            BuildTargetGroup.Standalone, // Note: We have it on our roadmap to support standalone: SG-29499
        };
        
        public static bool IsMonoSupported(BuildTargetGroup buildTarget) {
            // "When a platform can support both backends, Mono is the default. For more information, see Scripting restrictions."
            // Unity docs https://docs.unity3d.com/Manual/Mono.html (2019.4/2020.3/2021.3)
#pragma warning disable CS0618 // obsolete since 2023
            var defaultScripting = PlayerSettings.GetDefaultScriptingBackend(buildTarget);
#pragma warning restore CS0618
            if (defaultScripting == ScriptingImplementation.Mono2x) {
                return Array.IndexOf(unsupportedBuildTargets, buildTarget) == -1;
            }
            // default scripting was not Mono, so the platform doesn't support Mono at all.
            return false;
        }
        
        // Adapted from https://answers.unity.com/questions/984854/is-it-possible-to-excluding-streamingassets-depend.html
        public static string GetStreamingAssetsBuiltPath(BuildTarget target, string pathToBuiltProject) {
            string streamingAssetsPath = null;

            switch (target) {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneLinux64:
                #if !UNITY_2019_2_OR_NEWER
                // "StandaloneLinux has been removed in 2019.2"
                case BuildTarget.StandaloneLinux:
                // "StandaloneLinuxUniversal has been removed in 2019.2"
                case BuildTarget.StandaloneLinuxUniversal:
                #endif
                {
                    // windows and linux use "_Data" folder
                    // ReSharper disable once AssignNullToNotNullAttribute
                    string root = Path.Combine(Path.GetDirectoryName(pathToBuiltProject),
                        Path.GetFileNameWithoutExtension(pathToBuiltProject) + "_Data");
                    streamingAssetsPath = Path.Combine(root, "StreamingAssets");
                    break;
                }
                // StandaloneOSXIntel64 and StandaloneOSXIntel "has been removed in 2017.3"
                case BuildTarget.StandaloneOSX: {
                    var appContents = Path.Combine(pathToBuiltProject, "Contents");
                    streamingAssetsPath = Path.Combine(appContents, "Resources", "Data", "StreamingAssets");
                    break;
                }
                case BuildTarget.Android: {
                    streamingAssetsPath = Path.Combine(pathToBuiltProject, "src", "main", "assets");
                    break;
                }
            }

            return streamingAssetsPath;
        }
    }
}