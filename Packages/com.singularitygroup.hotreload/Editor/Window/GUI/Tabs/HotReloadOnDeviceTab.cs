using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
using EditorGUI = UnityEditor.EditorGUI;
using SessionPrefs = SingularityGroup.HotReload.Editor.HotReloadPrefs.SessionPrefs;

namespace SingularityGroup.HotReload.Editor {
    internal class HotReloadOnDeviceTab : HotReloadTabBase {
        private readonly QRCodeSection qrCodeSection;
        private readonly HotReloadOptionsSection optionsSection;

        // cached because changing built target triggers C# domain reload
        // Also I suspect selectedBuildTargetGroup has chance to freeze Unity for several seconds (unconfirmed).
        private readonly Lazy<BuildTargetGroup> currentBuildTarget = new Lazy<BuildTargetGroup>(
            () => BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));

        private readonly Lazy<bool> isCurrentBuildTargetSupported = new Lazy<bool>(() => {
            var target = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            return HotReloadBuildHelper.IsMonoSupported(target);
        });
        
        // Resources.Load uses cache, so it's safe to call it every frame.
        //  Retrying Load every time fixes an issue where you import the package and constructor runs, but resources aren't loadable yet.
        private Texture iconCheck => Resources.Load<Texture>("icon_check_circle");
        private Texture iconWarning => Resources.Load<Texture>("icon_warning_circle");

        [SuppressMessage("ReSharper", "Unity.UnknownResource")] // Rider doesn't check packages
        public HotReloadOnDeviceTab(HotReloadWindow window) : base(window,
            "On-Device",
            GetIconName(),
            "Make changes to a build running on-device.")
        {
            qrCodeSection = new QRCodeSection();
            optionsSection = new HotReloadOptionsSection();
        }

        private GUIStyle headlineStyle;

        /*
         * EditorGUILayout.LabelField is designed to work with the Unity editor's inspector window,
         * which has its own layout and padding system. This can cause some issues with custom GUIStyles,
         * as the padding and layout settings of the custom style may not match the inspector window's settings.
         */
        public override void OnGUI() {
            // header with explainer image
            {
                if (headlineStyle == null) {
                    // start with textArea for the background and border colors
                    headlineStyle = new GUIStyle(GUI.skin.label) {
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleLeft
                    };
                    headlineStyle.normal.textColor = HotReloadWindowStyles.H2TitleStyle.normal.textColor;

                    // bg color
                    if (HotReloadWindowStyles.IsDarkMode) {
                        headlineStyle.normal.background = EditorTextures.DarkGray40;
                    } else {
                        headlineStyle.normal.background = EditorTextures.LightGray225;
                    }
                    // layout
                    headlineStyle.padding = new RectOffset(8, 8, 0, 0);
                    headlineStyle.margin = new RectOffset(6, 6, 6, 6);
                }
                GUILayout.Space(9f); // space between logo and headline

                GUILayout.Label("Make changes to a build running on-device",
                    headlineStyle, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 1.4f));
                // image showing how Hot Reload works with a phone
                // var bannerBox = GUILayoutUtility.GetRect(flowchart.width * 0.6f, flowchart.height * 0.6f);
                // GUI.DrawTexture(bannerBox, flowchart, ScaleMode.ScaleToFit);
            }

            GUILayout.Space(16f);

            // loading again is smooth, pretty sure AssetDatabase.LoadAssetAtPath is caching -Troy
            var settingsObject = HotReloadSettingsEditor.LoadSettingsOrDefault();
            var so = new SerializedObject(settingsObject);
            
            // if you build for Android now, will Hot Reload work?
            {
                
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Build Settings Checklist", HotReloadWindowStyles.H3TitleStyle);
                EditorGUI.BeginDisabledGroup(isSupported);
                // One-click to change each setting to the supported value
                if (GUILayout.Button("Fix All", GUILayout.MaxWidth(90f))) {
                    FixAllUnsupportedSettings(so);
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
                
                
                // NOTE: After user changed some build settings, window may not immediately repaint
                // (e.g. toggle Development Build in Build Settings window)
                // We could show a refresh button (to encourage the user to click the window which makes it repaint).
                DrawSectionCheckBuildSupport(so);
            }

            GUILayout.Space(16f);

            //ButtonToOpenBuildSettings();

            // QR-Code with expand/collapse arrow
            {
                SessionPrefs.FoldoutQrCode = EditorGUILayout.Foldout(SessionPrefs.FoldoutQrCode,
                    "QR-Code",
                    true, HotReloadWindowStyles.FoldoutStyle);
                // By default collapsed
                // When you expand reveal text + QR-Code
                if (SessionPrefs.FoldoutQrCode) {
                    var leftIndent = 16f;
                    GUILayout.BeginHorizontal();
                    
                    // indent all controls (this works with non-labels)
                    GUILayout.Space(leftIndent);
                    GUILayout.BeginVertical();

                    HotReloadWindowStyles.H3TitleStyle.wordWrap = true;
                    GUILayout.Label("If auto-pair fails, use this QR-code to connect." +
                                    "\nMake sure you are on the same LAN/WiFi network",
                        HotReloadWindowStyles.H3TitleStyle);

                    if (!ServerHealthCheck.I.IsServerHealthy) {
                        DrawHorizontalCheck(ServerHealthCheck.I.IsServerHealthy,
                            "Hot Reload is running",
                            "Hot Reload is not running");
                    }

                    if (!HotReloadPrefs.ExposeServerToLocalNetwork) {
                        var summary = $"Enable '{new ExposeServerOption().ShortSummary}'";
                        DrawHorizontalCheck(HotReloadPrefs.ExposeServerToLocalNetwork,
                            summary,
                            summary);
                    }

                    HotReloadWindowStyles.H3TitleStyle.wordWrap = false;
                    // explainer image that shows phone needs same wifi to auto connect ?
                    
                    GUILayout.Space(5f);

                    var maxWidth = _window.position.width - leftIndent * 2f - 16f;
                    qrCodeSection.OnGUI_QrCodeOnly(maxWidth);
                    
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.Space(6f);

            // Settings checkboxes (Hot Reload options)
            {
                GUILayout.Label("Mobile", HotReloadWindowStyles.H3TitleStyle);
                if (settingsObject) {
                    optionsSection.DrawGUI(so);
                }
            }
            GUILayout.FlexibleSpace(); // needed otherwise vertical scrollbar is appearing for no reason (Unity 2021 glitch perhaps)
        }

        void ButtonToOpenBuildSettings() {
            // Button to Build and Run for Android 
            EditorGUI.BeginDisabledGroup(!isSupported);
            if (GUILayout.Button("Build with Hot Reload")) {
                EditorApplication.ExecuteMenuItem("File/Build Settings...");
            }

            EditorGUI.EndDisabledGroup();
        }

        // note: changing scripting backend does not force Unity to recreate the GUI, so need to check it when drawing.
        private ScriptingImplementation ScriptingBackend => HotReloadBuildHelper.GetCurrentScriptingBackend();
        public bool isSupported = true;

        /// <summary>
        /// These options are drawn in the On-device tab
        /// </summary>
        // new on-device options should be added here
        public static readonly IOption[] allOptions = new IOption[] {
            new ExposeServerOption(),
            new IncludeInBuildOption(),
            new AllowAndroidAppToMakeHttpRequestsOption(),
        };

        /// <summary>
        /// Change each setting to the value supported by Hot Reload
        /// </summary>
        private void FixAllUnsupportedSettings(SerializedObject so) {
            if (!isCurrentBuildTargetSupported.Value) {
                // try switch to Android platform
                // (we also support Standalone but HotReload on mobile is a better selling point)
                if (!TrySwitchToAndroid()) {
                    // skip changing other options (user won't readthe gray text) - user has to click Fix All again
                    return;
                }
            }
            
            foreach (var buildOption in allOptions) {
                if (!buildOption.GetValue(so)) {
                    buildOption.SetValue(so, true);
                }
            }
            so.ApplyModifiedProperties();
            var settingsObject = so.targetObject as HotReloadSettingsObject;
            if (settingsObject) {
                // when you click fix all, make sure to save the settings, otherwise ui does not update
                HotReloadSettingsEditor.EnsureSettingsCreated(settingsObject);
            }
            
            if (!EditorUserBuildSettings.development) {
                EditorUserBuildSettings.development = true;
            }
            
            HotReloadBuildHelper.SetCurrentScriptingBackend(ScriptingImplementation.Mono2x);
        }

        public static bool TrySwitchToAndroid() {
            var current = EditorUserBuildSettings.activeBuildTarget;
            if (current == BuildTarget.Android) {
                return true;
            }
            var confirmed = EditorUtility.DisplayDialog("Switch Build Target",
                "Switching the build target can take a while depending on project size.",
                "Switch to Android", "Cancel");
            if (confirmed) {
                EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildTargetGroup.Android, BuildTarget.Android);
                Log.Info("Build target is switching to Android.");
                // An error was being logged: "EndLayoutGroup: BeginLayoutGroup must be called first."
                // calling ExitGUI prevents that.
                GUIUtility.ExitGUI();
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Section that user can check before making a Unity Player build.
        /// </summary>
        /// <param name="so"></param>
        /// <remarks>
        /// This section is for confirming your build will work with Hot Reload.<br/>
        /// Options that can be changed after the build is made should be drawn elsewhere.
        /// </remarks>
        public void DrawSectionCheckBuildSupport(SerializedObject so) {
            isSupported = true;
            var selectedPlatform = currentBuildTarget.Value;
            DrawHorizontalCheck(isCurrentBuildTargetSupported.Value,
                $"The {selectedPlatform.ToString()} platform is supported.",
                $"The current platform is {selectedPlatform.ToString()} which is not supported.");

            using (new EditorGUI.DisabledScope(!isCurrentBuildTargetSupported.Value)) {
                // "Allow Mobile Builds to Connect (WiFi)"
                foreach (var option in allOptions) {
                    DrawHorizontalCheck(option.GetValue(so),
                        $"Enable \"{option.ShortSummary}\"",
                        $"Enable \"{option.ShortSummary}\"");
                }

                DrawHorizontalCheck(EditorUserBuildSettings.development,
                    "Development Build is enabled",
                    "Enable \"Development Build\"",
                    suggestedSolutionText: "Only Development Builds are supported. In Build Settings, enable 'Development Build'.");
                // use human readable string that is shown in Player Settings
                var scriptingBackendString = ScriptingBackend == ScriptingImplementation.Mono2x
                    ? "Mono"
                    : ScriptingBackend.ToString();
                
                DrawHorizontalCheck(ScriptingBackend == ScriptingImplementation.Mono2x,
                    $"Scripting Backend = {scriptingBackendString}",
                    $"Scripting Backend = {scriptingBackendString}",
                    suggestedSolutionText: "Only the Mono Scripting Backend is supported. In Player Settings, change 'Scripting Backend' to Mono.");

                // if (isSupported) {
                //     GUILayout.Label("Great! Your current build settings are supported by Hot Reload.\nBuild and Run to try it.", HotReloadWindowStyles.WrapStyle);
                // }
            }
            // dont show the build settings checklist because some are relevant only for the current platform.
        }

        void DrawEmptyCheck() {
            DrawHorizontalCheck(false, String.Empty);
        }

        /// <summary>
        /// Draw a box with a tick or warning icon on the left, with text describing the tick or warning
        /// </summary>
        /// <param name="condition">The condition to check. True to show a tick icon, False to show a warning.</param>
        /// <param name="okText">Shown when condition is true</param>
        /// <param name="notOkText">Shown when condition is false</param>
        /// <param name="suggestedSolutionText">Shown when <paramref name="condition"/> is false</param>
        void DrawHorizontalCheck(bool condition, string okText, string notOkText = null, string suggestedSolutionText = null) {
            if (okText == null) {
                throw new ArgumentNullException(nameof(okText));
            }
            if (notOkText == null) {
                notOkText = okText;
            }

            // include some horizontal space around the icon
            var boxWidth = GUILayout.Width(EditorGUIUtility.singleLineHeight * 1.31f);
            var height = GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.01f);
            GUILayout.BeginHorizontal(HotReloadWindowStyles.BoxStyle, height, GUILayout.ExpandWidth(true));
            var style = HotReloadWindowStyles.NoPaddingMiddleLeftStyle;
            var iconRect = GUILayoutUtility.GetRect(
                Mathf.Round(EditorGUIUtility.singleLineHeight * 1.31f),
                Mathf.Round(EditorGUIUtility.singleLineHeight * 1.01f),
                style, boxWidth, height, GUILayout.ExpandWidth(false));
            // rounded so we can have pixel perfect black circle bg
            iconRect.Set(Mathf.Round(iconRect.x), Mathf.Round(iconRect.y), Mathf.CeilToInt(iconRect.width),
                Mathf.CeilToInt(iconRect.height));
            var text = condition ? okText : notOkText;
            var icon = condition ? iconCheck : iconWarning;
            if (GUI.enabled) {
                DrawBlackCircle(iconRect);
                // resource can be null when building player (Editor Resources not available)
                if (icon) {
                    GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
                }
            } else {
                // show something (instead of hiding) so that layout stays same size
                DrawDisabledCircle(iconRect);
            }
            GUILayout.Space(4f);
            GUILayout.Label(text, style, height);

            if (!condition) {
                isSupported = false;
            }

            GUILayout.EndHorizontal();
            //In new design we dont draw solution text, maybe reused in future though
            // if (!condition && !String.IsNullOrEmpty(suggestedSolutionText)) {
            //     // suggest to the user how they can resolve the issue
            //     EditorGUI.indentLevel++;
            //     GUILayout.Label(suggestedSolutionText, HotReloadWindowStyles.WrapStyle);
            //     EditorGUI.indentLevel--;
            // }
        }

        void DrawDisabledCircle(Rect rect) => DrawCircleIcon(rect,
            Resources.Load<Texture>("icon_circle_gray"),
            Color.clear); // smaller circle draws less attention

        void DrawBlackCircle(Rect rect) => DrawCircleIcon(rect,
            Resources.Load<Texture>("icon_circle_black"),
            new Color(0.14f, 0.14f, 0.14f)); // black is too dark in unity light theme

        void DrawCircleIcon(Rect rect, Texture circleIcon, Color borderColor) {
            // Note: drawing texture from resources is pixelated on the edges, so it has some transperancy around the edges.
            // While building for Android, Resources.Load returns null for our editor Resources. 
            if (circleIcon != null) {
                GUI.DrawTexture(rect, circleIcon, ScaleMode.ScaleToFit);
            }
            
            // Draw smooth circle border
            const float borderWidth = 2f;
            GUI.DrawTexture(rect, EditorTextures.White, ScaleMode.ScaleToFit, true,
                0f,
                borderColor,
                new Vector4(borderWidth, borderWidth, borderWidth, borderWidth),
                Mathf.Min(rect.height, rect.width) / 2f);
        }
        
        static string GetIconName() {
#pragma warning disable CS0618
            switch (EditorUserBuildSettings.activeBuildTarget) {
                case BuildTarget.StandaloneOSX:
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
#if UNITY_2021_3_OR_NEWER
                case BuildTarget.EmbeddedLinux:
#endif
#if !UNITY_2019_2_OR_NEWER
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinuxUniversal:
#endif
                case BuildTarget.StandaloneLinux64:
                    return "BuildSettings.Standalone.Small";
#if UNITY_2020_3_OR_NEWER
                case BuildTarget.GameCoreXboxOne:
#endif
                case BuildTarget.XboxOne:
                    return "BuildSettings.XboxOne.Small";
                case BuildTarget.Switch:
                    return "BuildSettings.Switch.Small";
#if !UNITY_2022_2_OR_NEWER
                case BuildTarget.Lumin:
                    return "BuildSettings.Lumin.small";
#endif
#if UNITY_2019_2_OR_NEWER
                case BuildTarget.Stadia:
                    return "BuildSettings.Stadia.small";
#endif
                default:
                    return "BuildSettings.Android.Small";
            }
#pragma warning restore CS0618
        }
    }
}
