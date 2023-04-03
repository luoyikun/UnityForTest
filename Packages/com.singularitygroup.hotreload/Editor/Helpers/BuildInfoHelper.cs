using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    static class BuildInfoHelper {

        public static Task<BuildInfo> GenerateBuildInfoAsync() {
            return Task.FromResult(GenerateBuildInfoMainThread(EditorUserBuildSettings.activeBuildTarget));
        }

        public static BuildInfo GenerateBuildInfoMainThread() => GenerateBuildInfoMainThread(EditorUserBuildSettings.activeBuildTarget);

        public static BuildInfo GenerateBuildInfoMainThread(BuildTarget buildTarget) {
            string[] activeDefineSymbols = EditorUserBuildSettings.activeScriptCompilationDefines;
            return GenerateBuildInfo(activeDefineSymbols, buildTarget);
        }

        /// <param name="activeDefineSymbols">Obtain from Unity main-thread only api EditorUserBuildSettings.activeScriptCompilationDefines</param>
        /// <param name="activeBuildTarget"></param>
        private static BuildInfo GenerateBuildInfo(string[] activeDefineSymbols, BuildTarget activeBuildTarget) {
            var shortCommitHash = GitUtil.GetShortCommitHashOrFallback();
            // expected to be separated by semi-colon ';'
            // Assumption your editor is on selected platform which will be connecting to HR server.
            // E.g. if you have an Android build, we assume your editor is set to Android platform.
            var allDefineSymbols = GetAllAndroidMonoBuildDefineSymbols(activeDefineSymbols);
            var omittedProjectRegex = String.Join("|", AssemblyOmission.GetOmittedProjects(allDefineSymbols).Select(name => Regex.Escape(name)));
            var hostname = IsHumanControllingUs() ? IpHelper.GetIpAddress() : null; 
            var buildInfo = new BuildInfo {
                projectIdentifier = "SG-29580",
                commitHash = shortCommitHash,
                defineSymbols = allDefineSymbols, 
                projectOmissionRegex = omittedProjectRegex,
                buildMachineHostName = hostname,
                buildMachinePort = RequestHelper.port,
                activeBuildTarget = activeBuildTarget.ToString(),
            };
            //  Note: add a string to uniquely identify the Unity project. Could use filepath to /MyProject/Assets/ (editor Application.dataPath)
            //  or application identifier (com.company.appname).
            //  Do this when supporting multiple projects: SG-28807
            //  The matching code is in Runtime assembly which compares server response with built BuildInfo.
            return buildInfo;
        }

        public static bool IsHumanControllingUs() {
            if (Application.isBatchMode) {
                return false;
            }

            var isCI = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));
            return !isCI;
        }

        private static readonly string[] editorSymbolsToRemove = {
            "PLATFORM_ARCH_64",
            "UNITY_64",
            "UNITY_INCLUDE_TESTS",
            "UNITY_EDITOR",
            "UNITY_EDITOR_64",
            "UNITY_EDITOR_WIN",
            "ENABLE_UNITY_COLLECTIONS_CHECKS",
            "ENABLE_BURST_AOT",
            "RENDER_SOFTWARE_CURSOR",
            "PLATFORM_STANDALONE_WIN",
            "PLATFORM_STANDALONE",
            "UNITY_STANDALONE_WIN",
            "UNITY_STANDALONE",
            "ENABLE_MOVIES",
            "ENABLE_OUT_OF_PROCESS_CRASH_HANDLER",
            "ENABLE_WEBSOCKET_HOST",
            "ENABLE_CLUSTER_SYNC",
            "ENABLE_CLUSTERINPUT",
        };

        private static readonly string[] androidSymbolsToAdd = { 
            "CSHARP_7_OR_LATER",
            "CSHARP_7_3_OR_NEWER",
            "PLATFORM_ANDROID",
            "UNITY_ANDROID",
            "UNITY_ANDROID_API",
            "ENABLE_EGL",
            "DEVELOPMENT_BUILD",
            "ENABLE_CLOUD_SERVICES_NATIVE_CRASH_REPORTING",
            "PLATFORM_SUPPORTS_ADS_ID",
            "UNITY_CAN_SHOW_SPLASH_SCREEN",
            "UNITY_HAS_GOOGLEVR",
            "UNITY_HAS_TANGO",
            "ENABLE_SPATIALTRACKING",
            "ENABLE_RUNTIME_PERMISSIONS",
            "ENABLE_ENGINE_CODE_STRIPPING",
            "UNITY_ASTC_ONLY_DECOMPRESS",
            "ANDROID_USE_SWAPPY",
            "ENABLE_ONSCREEN_KEYBOARD",
            "ENABLE_UNITYADS_RUNTIME",
            "UNITY_UNITYADS_API",
        };
        
        // Currently there is no better way. Alternatively we could hook into unity's call to csc.exe and parse the /define: arguments. 
        //   Hardcoding the differences was less effort and is less error prone.
        // I also looked into it and tried all the Build interfaces like this one https://docs.unity3d.com/ScriptReference/Build.IPostBuildPlayerScriptDLLs.html
        //   and logging EditorUserBuildSettings.activeScriptCompilationDefines in the callbacks - result: all same like editor, so I agree that hardcode is best. 
        public static string GetAllAndroidMonoBuildDefineSymbols(string[] defineSymbols) {
            var defines = new HashSet<string>(defineSymbols);
            defines.ExceptWith(editorSymbolsToRemove);
            defines.UnionWith(androidSymbolsToAdd);
            var definesArray = defines.ToArray();
            return String.Join(";", definesArray);
        }
    }
}