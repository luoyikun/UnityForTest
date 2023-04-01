using System;
using UnityEditor;

namespace SingularityGroup.HotReload.Editor {
    internal static class HotReloadPrefs {
        private const string RemoteServerKey = "HotReloadWindow.RemoteServer";
        private const string RemoteServerHostKey = "HotReloadWindow.RemoteServerHost";
        private const string LicenseEmailKey = "HotReloadWindow.LicenseEmail";
        private const string RenderAuthLoginKey = "HotReloadWindow.RenderAuthLogin";
        private const string FirstLoginCachedKey = "HotReloadWindow.FirstLoginCachedKey";
        private const string ShowOnStartupKey = "HotReloadWindow.ShowOnStartup";
        private const string PasswordCachedKey = "HotReloadWindow.PasswordCached";
        private const string ExposeServerToLocalNetworkKey = "HotReloadWindow.ExposeServerToLocalNetwork";
        private const string ErrorHiddenCachedKey = "HotReloadWindow.ErrorHiddenCachedKey";
        private const string RefreshManuallyTipCachedKey = "HotReloadWindow.RefreshManuallyTipCachedKey";
        private const string ShowLoginCachedKey = "HotReloadWindow.ShowLoginCachedKey";
        private const string LoggedBurstHintKey = "HotReloadWindow.LoggedBurstHint";
        
        public const string DontShowPromptForDownloadKey = "ServerDownloader.DontShowPromptForDownload";


        static string[] settingCacheKeys;
        public static string[] SettingCacheKeys = settingCacheKeys ?? (settingCacheKeys = new[] {
            AllowHttpSettingCacheKey,
            AutoRefreshSettingCacheKey,
            ScriptCompilationSettingCacheKey,
            ProjectGenerationSettingCacheKey,
        });
        
        public const string AllowHttpSettingCacheKey = "HotReloadWindow.AllowHttpSettingCacheKey";
        public const string AutoRefreshSettingCacheKey = "HotReloadWindow.AutoRefreshSettingCacheKey";
        public const string ScriptCompilationSettingCacheKey = "HotReloadWindow.ScriptCompilationSettingCacheKey";
        public const string ProjectGenerationSettingCacheKey = "HotReloadWindow.ProjectGenerationSettingCacheKey";


        public static bool RemoteServer {
            get { return EditorPrefs.GetBool(RemoteServerKey, false); }
            set { EditorPrefs.SetBool(RemoteServerKey, value); }
        }
        
        public static bool DontShowPromptForDownload {
            get { return EditorPrefs.GetBool(DontShowPromptForDownloadKey, false); }
            set { EditorPrefs.SetBool(DontShowPromptForDownloadKey, value); }
        }

        public static string RemoteServerHost {
            get { return EditorPrefs.GetString(RemoteServerHostKey); }
            set { EditorPrefs.SetString(RemoteServerHostKey, value); }
        }

        public static string LicenseEmail {
            get { return EditorPrefs.GetString(LicenseEmailKey); }
            set { EditorPrefs.SetString(LicenseEmailKey, value); }
        }
        
        public static string LicensePassword {
            get { return EditorPrefs.GetString(PasswordCachedKey); }
            set { EditorPrefs.SetString(PasswordCachedKey, value); }
        }
        
        public static bool RenderAuthLogin { // false = render free trial
            get { return EditorPrefs.GetBool(RenderAuthLoginKey); }
            set { EditorPrefs.SetBool(RenderAuthLoginKey, value); }
        }
        
        public static bool FirstLogin {
            get { return EditorPrefs.GetBool(FirstLoginCachedKey, true); }
            set { EditorPrefs.SetBool(FirstLoginCachedKey, value); }
        }

        public static string ShowOnStartup { // WindowAutoOpen
            get { return EditorPrefs.GetString(ShowOnStartupKey); }
            set { EditorPrefs.SetString(ShowOnStartupKey, value); }
        }


        public static bool ErrorHidden {
            get { return EditorPrefs.GetBool(ErrorHiddenCachedKey); }
            set { EditorPrefs.SetBool(ErrorHiddenCachedKey, value); }
        }
        
        public static bool ShowLogin {
            get { return EditorPrefs.GetBool(ShowLoginCachedKey, true); }
            set { EditorPrefs.SetBool(ShowLoginCachedKey, value); }
        }
        
        public static bool RefreshManuallyTip {
            get { return EditorPrefs.GetBool(RefreshManuallyTipCachedKey); }
            set { EditorPrefs.SetBool(RefreshManuallyTipCachedKey, value); }
        }
        
        public static bool LoggedBurstHint {
            get { return EditorPrefs.GetBool(LoggedBurstHintKey); }
            set { EditorPrefs.SetBool(LoggedBurstHintKey, value); }
        }
        

        public static ShowOnStartupEnum GetShowOnStartupEnum() {
            ShowOnStartupEnum showOnStartupEnum;
            if (Enum.TryParse(HotReloadPrefs.ShowOnStartup, true, out showOnStartupEnum)) {
                return showOnStartupEnum;
            }
            return ShowOnStartupEnum.Always;
        }
        
        public static bool ExposeServerToLocalNetwork {
            get { return EditorPrefs.GetBool(ExposeServerToLocalNetworkKey, false); }
            set { EditorPrefs.SetBool(ExposeServerToLocalNetworkKey, value); }
        }

        /// <summary>
        /// Prefs for storing temporary UI state of the Hot Reload EditorWindow.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Use this for things like the state of EditorGUILayout.Foldout to keep it collapsed or expanded.
        /// When Unity refreshes (compiles code), the EditorWindow is recreated, and C# field values are lost.<br/>
        /// </para>
        /// <para>
        /// Do not use this class for persistant options, like a checkbox thatthe user can click.<br/>
        /// We may later decide to clear these prefs when a project is closed.
        /// </para>
        /// </remarks>
        internal static class SessionPrefs {
            private const string FoldoutCheckBuildSupportKey = "HotReloadWindow.SessionPrefs.FoldoutCheckBuildSupportKey";
            private const string FoldoutQrCodeKey = "HotReloadWindow.SessionPrefs.FoldoutQrCodeKey";

            public static bool FoldoutCheckBuildSupport {
                get { return EditorPrefs.GetBool(FoldoutCheckBuildSupportKey, true); }
                set { EditorPrefs.SetBool(FoldoutCheckBuildSupportKey, value); }
            }
            
            public static bool FoldoutQrCode {
                // by default collapsed
                get { return EditorPrefs.GetBool(FoldoutQrCodeKey, false); }
                set { EditorPrefs.SetBool(FoldoutQrCodeKey, value); }
            }
        }
    }
}
