using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace SingularityGroup.HotReload {
    internal class RetryDialog : MonoBehaviour {
        [Header("UI controls")]
        public Button buttonHide;
        public Button buttonRetryAutoPair;
        public Button buttonScanQrCode;

        public Text textSummary;
        public Text textSuggestion;
        
        [Tooltip("Hidden by default")]
        public Text textForDebugging;
        
        [Header("For HotReload Devs")]
        // In Unity Editor, click checkbox to see info helpful for debugging bugs
        public bool enableDebugging;

        // [Header("Other")]
        // [Tooltip("Used when your project does not create an EventSystem early enough")]
        // public GameObject fallbackEventSystem;

        private static RetryDialog _I;
        
        public string DebugInfo {
            set {
                textForDebugging.text = value;
            }
        }
        
        void Start() {
            buttonHide.onClick.AddListener(() => {
                Hide();   
            });
            
            // launch camera app that can scan QR-Code  https://singularitygroup.atlassian.net/browse/SG-29495
            buttonScanQrCode.onClick.AddListener(ActivityHelpers.TryLaunchQRScannerApp);
            
            buttonRetryAutoPair.onClick.AddListener(() => {
                Hide();   
                // run the auto pair task again
                PlayerEntrypoint.TryConnect(TargetServer).Forget();
            });
        }

        [CanBeNull]
        public static PatchServerInfo TargetServer { private get; set; } = null;
        public static ServerHandshake.Result HandshakeResults { private get; set; } = ServerHandshake.Result.None;

        private void OnEnable() {
            UpdateUI();
        }

        void Update() {
            UpdateUI();
        }
            
        void UpdateUI() {
            // assumes that auto-pair already tried for several seconds
            // suggestions to help the user when auto-pair is failing
            string noWifiNetwork;
            if (Application.isMobilePlatform) {
                noWifiNetwork = "Is this device connected to WiFi?";
            } else {
                noWifiNetwork = "Is this device connected to LAN/WiFi?";
            }
            string networkIsSame = "Make sure Hot Reload is running";
            string waitForCompiling = "Wait for compiling to finish before trying again";
            string targetNetworkIsReachable = "Make sure you're on the same LAN/WiFi network";
            string noTargetServer = "Scan QR-Code to connect to Hot Reload on your PC.";

            if (Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork) {
                textSuggestion.text = noWifiNetwork;
            } else if (HandshakeResults.HasFlag(ServerHandshake.Result.WaitForCompiling)) {
                // Note: Technically the player could do the waiting itself, and handshake again with the server
                // only after compiling finishes... Telling the user to do that is easier to implement though.
                textSuggestion.text = waitForCompiling;
            } else if (TargetServer == null) {
                // suggest scan QR-Code to connect and make sure hot reload running
                textSuggestion.text = noTargetServer;
            } else if (TargetServer.UsesMyNetwork()) {
                textSuggestion.text = networkIsSame;
            } else {
                textSuggestion.text = targetNetworkIsReachable;
            }

            if (textSuggestion.text == noTargetServer) {
                // we need to know which server
                textSummary.text = "Hot Reload is ready to pair";
            } else {
                // default summary
                textSummary.text = "Auto-pair ran into an issue";
            }

            if (enableDebugging && textForDebugging) {
                textForDebugging.enabled = true;
                textForDebugging.text = $"the target = {TargetServer}";
            }
        }

        /// hide this dialog
        void Hide() {
            gameObject.SetActive(false); // this should disable the Update loop?
        }
    }
}