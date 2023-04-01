using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    internal class HotReloadAboutTab : HotReloadTabBase {
        private readonly List<IGUIComponent>[] _supportButtons;
        private readonly OpenURLButton _buyLicenseButton;
        private readonly OpenDialogueButton _manageLicenseButton;
        private readonly OpenDialogueButton _manageAccountButton;
        public readonly OpenURLButton documentationButton = new OpenURLButton("Documentation", Constants.DocumentationURL);
        public readonly OpenDialogueButton reportIssueButton = 
               new OpenDialogueButton("Report issue", Constants.ReportIssueURL, "Report issue", "Report issue in our public issue tracker. Requires gitlab.com account (if you don't have one and are not willing to make it, please contact us by other means such as our website).", "Open in browser", "Cancel");

        public HotReloadAboutTab(HotReloadWindow window) : base(window, "Help", "_Help", "Info and support for Hot Reload for Unity.") {
            string contactSpace;
            if (Application.unityVersion.Contains("2018")) {
                contactSpace = "      ";
            } else {
                contactSpace = "         ";
            }
            _supportButtons = new[] {
                new List<IGUIComponent> {
                    documentationButton,
                    new OpenURLButton("Contact" + contactSpace, Constants.ContactURL),
                },
                new List<IGUIComponent> {
                    new OpenURLButton("Unity Forum     ", Constants.ForumURL),
                    reportIssueButton,
                }
            };
            _manageLicenseButton = new OpenDialogueButton("Manage License", Constants.ManageLicenseURL, "Manage License", "Upgrade/downgrade/edit your subscription and edit payment info.", "Open in browser", "Cancel");
            _manageAccountButton = new OpenDialogueButton("Manage Account", Constants.ManageAccountURL, "Manage License", "Login with company code 'naughtycult'. Use the email you signed up with. Your initial password was sent to you by email.", "Open in browser", "Cancel");
            _buyLicenseButton = new OpenURLButton("Get License", Constants.ProductPurchaseURL);
        }

        public override void OnGUI() {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox($"You are running Hot Reload for Unity version {PackageConst.Version}. ", MessageType.Info);
            EditorGUILayout.Space();

            RenderLicenseInfoSection();
            EditorGUILayout.Space();

            _buyLicenseButton.OnGUI();
            EditorGUILayout.Space();

            foreach (var group in _supportButtons) {
                using (new EditorGUILayout.HorizontalScope()) {
                    foreach (var button in group) {
                        button.OnGUI();
                    }
                }
            }
            EditorGUILayout.Space();

            var hasTrial = _window.runTab.TrialLicense;
            var hasPaid = _window.runTab.HasPayedLicense;
            if (hasPaid || hasTrial) {
                using(new EditorGUILayout.HorizontalScope()) {
                    if (hasPaid) {
                        _manageLicenseButton.OnGUI();
                    }
                    _manageAccountButton.OnGUI();
                }
                EditorGUILayout.Space();
            }

            foreach (var settingKey in HotReloadPrefs.SettingCacheKeys) {
                if (!EditorPrefs.GetBool(settingKey, true)) {
                    if (GUILayout.Button("Re-enable all suggestions")) {
                        foreach (var _settingKey in HotReloadPrefs.SettingCacheKeys) {
                            EditorPrefs.SetBool(_settingKey, true);
                        }
                        _window.SelectTab(typeof(HotReloadRunTab));
                    }
                    EditorGUILayout.Space();
                    break;
                }
            }
        }
        
        private void RenderLicenseInfoSection() {
            if (!_window.runTab.FreeLicense) {
                _window.runTab.RenderLicenseInfo(verbose: true, allowHide: false,
                    overrideRenderFreeTrial: false, overrideActionButton: "Activate License",
                    showConsumptions: true);
            }
        }

        public void FocusLicenseFoldout() {
            HotReloadPrefs.ShowLogin = true;
        }
    }
}
