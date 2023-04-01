using System;
using System.IO;
using System.Reflection;
using SingularityGroup.HotReload.Editor.Cli;
using UnityEditor;
using UnityEngine;
#if UNITY_2019_4_OR_NEWER
using Unity.CodeEditor;
#endif

namespace SingularityGroup.HotReload.Editor {
    interface IRequiredSettingChecker {
        bool IsApplied();
        void Apply();
        void DebugReset();
        bool ApplyRequiresSaveAssets {get;}
    }

    

    class AutoRefreshSettingChecker : IRequiredSettingChecker {
        const string autoRefreshKey = "kAutoRefresh";
        const string autoRefreshModeKey = "kAutoRefreshMode";

        private int AutoRefreshPreference {
            get {
                // From Unity 2021.3 onwards, the key is "kAutoRefreshMode".
                #if UNITY_2021_3_OR_NEWER
                return EditorPrefs.GetInt(autoRefreshModeKey);
                #else
                return EditorPrefs.GetInt(autoRefreshKey);
                #endif
            }
            set {
                #if UNITY_2021_3_OR_NEWER
                EditorPrefs.SetInt(autoRefreshModeKey, value);
                #else
                EditorPrefs.SetInt(autoRefreshKey, value);
                #endif
            }
        }

        public bool IsApplied() {
            // Before Unity 2021.3, value is 0 or 1. Only value of 1 is a problem.
            // From Unity 2021.3 onwards, the key is "kAutoRefreshMode".
            // kAutoRefreshMode options are:
            //   0: disabled
            //   1: enabled 
            //   2: enabled outside playmode
            // only option 1 is a problem
            return AutoRefreshPreference != 1;
        }

        public void Apply() {
            #if UNITY_2021_3_OR_NEWER
            // On these newer Unity versions, Visual Studio is also checking the kAutoRefresh setting (but it should only check kAutoRefreshMode).
            // In previous HotReload version, we were also changing kAutoRefresh to 0 (which breaks Visual Studio triggering auto refresh
            // even after the user reset the Auto Refresh mode in Unity preferences).
            // To fix it for these users, we set kAutoRefresh to 1 when they apply the setting (Unity 2021.3+ doesn't use it).
            if (EditorPrefs.GetInt(autoRefreshKey) == 0) {
                EditorPrefs.SetInt(autoRefreshKey, 1);
            }
            AutoRefreshPreference = 2; // enabled outside playmode
            #else
            AutoRefreshPreference = 0; // disabled
            #endif
            HotReloadPrefs.RefreshManuallyTip = true;
            // Dialog is rather annoying. Assume the user also wants the other one, to avoid 2 dialogs
            ScriptCompilationSettingChecker.I.Apply();
        }

        public bool ApplyRequiresSaveAssets => ScriptCompilationSettingChecker.I.ApplyRequiresSaveAssets;

        public void DebugReset() {
            AutoRefreshPreference = 1;
            // Dialog is rather annoying. Assume the user also wants the other one, to avoid 2 dialogs
            ScriptCompilationSettingChecker.I.DebugReset();
        }
    }
    
    class ScriptCompilationSettingChecker : IRequiredSettingChecker {
        public static readonly ScriptCompilationSettingChecker I = new ScriptCompilationSettingChecker(); 
        
        const string scriptCompilationKey = "ScriptCompilationDuringPlay";
        
        public bool IsApplied() {
            var status = EditorPrefs.GetInt(scriptCompilationKey);
#           if (UNITY_2021_1_OR_NEWER)
                // we can be sure that all 3 options are available, so recommend 'Recompile After Finished Playing'
                // (Unity removed/re-added the setting in multiple builds, so we don't know what's available)
                return status != 2;
#           else
                // earlier unity versions didn't have the messy settings problem
                return status == GetRecommendedAutoScriptCompilationKey();
#endif
        }

        public void Apply() {
            EditorPrefs.SetInt(scriptCompilationKey, GetRecommendedAutoScriptCompilationKey());
        }

        public bool ApplyRequiresSaveAssets => false;

        static int GetRecommendedAutoScriptCompilationKey() {
            var existingKey = EditorPrefs.GetInt(scriptCompilationKey);
            if (existingKey == 2) {
                return 1;
            }
#           if (UNITY_2021_1_OR_NEWER)
                return 0; // 'Recompile and Continue Playing'
#           else 
                return 1;
#endif
        }
        
        public void DebugReset() {
            EditorPrefs.SetInt(scriptCompilationKey, 2);
        }
    }
    
    class InstallServerSettingChecker : IRequiredSettingChecker {
        ServerDownloader downloader => EditorCodePatcher.serverDownloader;
        
        public bool IsApplied() {
            return downloader.IsDownloaded(HotReloadCli.controller) || downloader.Started;
        }

        public void Apply() {
            EditorCodePatcher.serverDownloader.PromptForDownload();
        }

        public bool ApplyRequiresSaveAssets => false;

        public void DebugReset() {
            File.Delete(downloader.GetExecutablePath(HotReloadCli.controller));
        }
    }
}