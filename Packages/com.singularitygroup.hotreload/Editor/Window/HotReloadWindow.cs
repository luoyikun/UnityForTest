
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using SingularityGroup.HotReload.DTO;
using SingularityGroup.HotReload.Editor.Semver;
using SingularityGroup.HotReload.RuntimeDependencies;
using UnityEditor;
using UnityEngine;

[assembly: InternalsVisibleTo("SingularityGroup.HotReload.EditorSamples")]

namespace SingularityGroup.HotReload.Editor {
    class HotReloadWindow : EditorWindow {
        public static HotReloadWindow Current { get; private set; }

        static readonly Dictionary<string, PatchInfo> pendingPatches = new Dictionary<string, PatchInfo>();

        List<HotReloadTabBase> tabs;
        int selectedTab;

        Vector2 scrollPos;


        internal HotReloadRunTab runTab;
        internal HotReloadAboutTab aboutTab;

        ShowOnStartupEnum _showOnStartupOption;

        /// <summary>
        /// This token is cancelled when the EditorWindow is disabled.
        /// </summary>
        /// <remarks>
        /// Use it for all tasks.
        /// When token is cancelled, scripts are about to be recompiled and this will cause tasks to fail for weird reasons.
        /// </remarks>
        public CancellationToken cancelToken;
        CancellationTokenSource cancelTokenSource;

        static readonly PackageUpdateChecker packageUpdateChecker = new PackageUpdateChecker();

        [MenuItem("Window/Hot Reload &#H")]
        internal static void Open() {
            // opening the window on CI systems was keeping Unity open indefinitely
            if (EditorWindowHelper.IsHumanControllingUs()) {
                if (Current) {
                    Current.Show();
                    Current.Focus();
                } else {
                    Current = GetWindow<HotReloadWindow>();
                }
            }
        }


        internal static void RegisterWarnings(IEnumerable<string> newWarnings) {
            foreach (var warning in newWarnings) {
                Log.Warning("[{0}] {1}", CodePatcher.TAG, warning);
            }
        }

        void OnEnable() {
            Current = this;

            if (cancelTokenSource != null) {
                cancelTokenSource.Cancel();
            }
            cancelTokenSource = new CancellationTokenSource();
            cancelToken = cancelTokenSource.Token;

            runTab = new HotReloadRunTab(this);
            // var androidTab = new HotReloadOnDeviceTab(this);
            //optionsTab = new HotReloadOptionsTab(this);
            aboutTab = new HotReloadAboutTab(this);
            tabs = new List<HotReloadTabBase> {
                runTab,
                // androidTab,
                aboutTab,
            };

            this.minSize = new Vector2(300, 150f);
            var tex = Resources.Load<Texture>(HotReloadWindowStyles.IsDarkMode ? "Icon_DarkMode" : "Icon_LightMode");
            this.titleContent = new GUIContent(" Hot Reload", tex);
            this._showOnStartupOption = HotReloadPrefs.GetShowOnStartupEnum();

            foreach (var patch in CodePatcher.I.PendingPatches) {
                pendingPatches.Add(patch.id, new PatchInfo(patch));
            }
            packageUpdateChecker.StartCheckingForNewVersion();
        }

        void Update() {
            foreach (var tab in tabs) {
                tab.Update();
            }
        }

        void OnDisable() {
            if (cancelTokenSource != null) {
                cancelTokenSource.Cancel();
                cancelTokenSource = null;
            }

            if (Current == this) {
                Current = null;
            }
        }

        internal void SelectTab(Type tabType) {
            selectedTab = tabs.FindIndex(x => x.GetType() == tabType);
        }

        void OnGUI() {
            using(var scope = new EditorGUILayout.ScrollViewScope(scrollPos, false, false)) {
                scrollPos = scope.scrollPosition;
                // RenderDebug();
                RenderTabs();
            }
            GUILayout.FlexibleSpace(); // GUI below will be rendered on the bottom
            RenderBottomBar();
        }

        void RenderDebug() {
            if (GUILayout.Button("RESET WINDOW")) {
                OnDisable();

                RequestHelper.RequestLogin("test", "test", 1).Forget();

                HotReloadPrefs.RemoteServer = false;
                HotReloadPrefs.RemoteServerHost = null;
                HotReloadPrefs.LicenseEmail = null;
                HotReloadPrefs.ExposeServerToLocalNetwork = true;
                HotReloadPrefs.LicensePassword = null;
                HotReloadPrefs.RenderAuthLogin = false;
                HotReloadPrefs.FirstLogin = true;
                HotReloadPrefs.RefreshManuallyTip = false;
                HotReloadPrefs.LoggedBurstHint = false;
                HotReloadPrefs.DontShowPromptForDownload = false;
                foreach (var settingCache in HotReloadPrefs.SettingCacheKeys) {
                    EditorPrefs.SetBool(settingCache, true);
                }
                foreach (var presenter in RequiredSettings.Presenters) {
                    presenter.DebugReset();
                }
                OnEnable();
                InstallUtility.DebugClearInstallState();
                InstallUtility.CheckForNewInstall();
                EditorPrefs.DeleteKey(Attribution.LastLoginKey);
            }
        }

        void RenderLogo() {
            var isDarkMode = HotReloadWindowStyles.IsDarkMode;
            var tex = Resources.Load<Texture>(isDarkMode ? "Logo_HotReload_DarkMode" : "Logo_HotReload_LightMode");
            //Can happen during player builds where Editor Resources are unavailable
            if(tex == null) {
                return;
            }
            var targetWidth = 243;
            var targetHeight = 44;
            GUILayout.Space(4f);
            // background padding top and bottom
            float padding = 5f;
            // reserve layout space for the texture
            var backgroundRect = GUILayoutUtility.GetRect(targetWidth + padding, targetHeight + padding, HotReloadWindowStyles.LogoStyle);
            // draw the texture into that reserved space. First the bg then the logo.
            if (isDarkMode) {
                GUI.DrawTexture(backgroundRect, EditorTextures.DarkGray17, ScaleMode.StretchToFill);
            } else {
                GUI.DrawTexture(backgroundRect, EditorTextures.LightGray238, ScaleMode.StretchToFill);
            }
            
            var foregroundRect = backgroundRect;
            foregroundRect.yMin += padding;
            foregroundRect.yMax -= padding;
            // during player build (EditorWindow still visible), Resources.Load returns null
            if (tex) {
                GUI.DrawTexture(foregroundRect, tex, ScaleMode.ScaleToFit);
            }
        }

        void RenderTabs() {
            using(new EditorGUILayout.VerticalScope(HotReloadWindowStyles.BoxStyle)) {
                selectedTab = GUILayout.Toolbar(
                    selectedTab,
                    tabs.Select(t => new GUIContent(t.Title.StartsWith(" ", StringComparison.Ordinal) ? t.Title : " " + t.Title, t.Icon, t.Tooltip)).ToArray(),
                    GUILayout.Height(22f) // required, otherwise largest icon height determines toolbar height
                );
                RenderLogo();

                tabs[selectedTab].OnGUI();
            }
        }

        void RenderBottomBar() {
            SemVersion newVersion;
            var updateAvailable = packageUpdateChecker.TryGetNewVersion(out newVersion); 
            // var updateAvailable = true;
            // newVersion = SemVersion.Parse("9.9.9");
            using(new EditorGUILayout.HorizontalScope("ProjectBrowserBottomBarBg", GUILayout.ExpandWidth(true), GUILayout.Height(updateAvailable ? 28f : 25f))) {
                RenderBottomBarCore(updateAvailable, newVersion);
            }
        }

        void RenderBottomBarCore(bool updateAvailable, SemVersion newVersion) {
            if (updateAvailable) {
                var btn = EditorStyles.miniButton;
                var prevStyle = btn.fontStyle;
                var prevSize = btn.fontSize;
                try {
                    btn.fontStyle = FontStyle.Bold;
                    btn.fontSize = 11;
                    if (GUILayout.Button($"Update To v{newVersion}", btn, GUILayout.MaxWidth(140), GUILayout.ExpandHeight(true))) {
                        packageUpdateChecker.UpdatePackageAsync(newVersion).Forget(CancellationToken.None);
                    }
                } finally {
                    btn.fontStyle = prevStyle;
                    btn.fontSize = prevSize;
                }
            }
            
            aboutTab.documentationButton.OnGUI();

            GUILayout.FlexibleSpace();
            using(var changeScope = new EditorGUI.ChangeCheckScope()) {
                var prevLabelWidth = EditorGUIUtility.labelWidth;
                try {
                    EditorGUIUtility.labelWidth = 105f;

                    using (new GUILayout.VerticalScope()) {
                        GUILayout.FlexibleSpace();
                        _showOnStartupOption = (ShowOnStartupEnum)EditorGUILayout.EnumPopup("Show On Startup", _showOnStartupOption, GUILayout.Width(218f));
                        GUILayout.FlexibleSpace();
                    }
                } finally {
                    EditorGUIUtility.labelWidth = prevLabelWidth;
                }
                
                if(changeScope.changed) {
                    HotReloadPrefs.ShowOnStartup = _showOnStartupOption.ToString();
                }
            }
        }

        struct PatchInfo {
            public readonly string patchId;
            public readonly bool apply;
            public readonly string[] methodNames;

            public PatchInfo(MethodPatchResponse response) : this(response.id, apply: true, methodNames: GetMethodNames(response)) { }

            PatchInfo(string patchId, bool apply, string[] methodNames) {
                this.patchId = patchId;
                this.apply = apply;
                this.methodNames = methodNames;
            }


            static string[] GetMethodNames(MethodPatchResponse response) {
                var methodNames = new string[MethodCount(response)];
                var methodIndex = 0;
                for (int i = 0; i < response.patches.Length; i++) {
                    for (int j = 0; j < response.patches[i].modifiedMethods.Length; j++) {
                        var method = response.patches[i].modifiedMethods[j];
                        var displayName = method.displayName;

                        var spaceIndex = displayName.IndexOf(" ", StringComparison.Ordinal);
                        if (spaceIndex > 0) {
                            displayName = displayName.Substring(spaceIndex);
                        }

                        methodNames[methodIndex++] = displayName;
                    }
                }
                return methodNames;
            }

            static int MethodCount(MethodPatchResponse response) {
                var count = 0;
                for (int i = 0; i < response.patches.Length; i++) {
                    count += response.patches[i].modifiedMethods.Length;
                }
                return count;
            }
        }
    }
}