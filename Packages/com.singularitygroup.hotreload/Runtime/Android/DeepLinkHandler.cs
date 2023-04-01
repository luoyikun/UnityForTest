using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace SingularityGroup.HotReload {
    internal class DeepLinkHandler {
        private static DeepLinkHandler I;

        public DeepLinkHandler() {
            AppCallbackListener.onApplicationFocus += OnApplicationFocus;
        }

        /// When app is launched through deeplink, url is decoded to PatchServerInfo
        ///  and OnNewServer is called
        public Action<PatchServerInfo> OnNewServer = (_) =>
            Log.Info("Dropped serverInfo from deeplink because OnNewServer is not set");
        
        /// <summary>
        /// Subscribe to deeplink listener
        /// </summary>
        public static void SetupDeepLinkHandling(Action<PatchServerInfo> onNewServer) {
            if (I == null) {
                I = new DeepLinkHandler();
            }

            if (!PlayerEntrypoint.RuntimeSupportsHotReload) {
                return;
            }
            if (Application.platform != RuntimePlatform.Android) {
                // deep links are only setup for Android
                return;
            }
            Log.Debug("Started checking for deeplink");
            I.OnNewServer = onNewServer;
            #if SUPPORTS_deepLinkActivated
            Application.deepLinkActivated += url => I.TryHandleDeepLink(url);
            #endif
        }

        /// <summary>
        /// Handle deeplink used to start app (if present).
        /// </summary>
        /// <returns>True if app was launched through a deeplink</returns>
        public static bool TryHandleLaunchDeepLink() {
            return TryHandleDeepLink(GetDeepLink());
        }
        
        static bool TryHandleDeepLink(string deeplink) {
            if (String.IsNullOrEmpty(deeplink)) {
                return false;
            }

            Log.Info("Found deeplink: {0}", deeplink);

            var uri = new Uri(deeplink);
            PatchServerInfo info;
            var error = PatchServerInfo.TryParse(uri, out info);
            if (error != null) {
                Log.Warning($"The URI was invalid: {error}");
                return false;
            }

            I.OnNewServer.Invoke(info);
            return true;
        }
        
        #if !SUPPORTS_deepLinkActivated
        private static AndroidJavaClass _deepLinkForwarderActivity;
        private static AndroidJavaClass DeepLinkForwarderActivity {
            get {
                if (_deepLinkForwarderActivity == null) {
                    _deepLinkForwarderActivity =
                        new AndroidJavaClass("com.singularitygroup.deeplinkforwarder.DeepLinkForwarderActivity");
                }
                return _deepLinkForwarderActivity;
            }
        }

        // We don't get a callback when deeplink is used, so always check for a new one on app resumed.
        private void OnApplicationFocus(bool hasFocus) {
            if (hasFocus && PlayerEntrypoint.IsPlayerWithHotReload() && Application.platform == RuntimePlatform.Android) {
                // on this old Unity version, Application.absoluteURL also doesn't contain the deeplink 
                var unreadDeepLink = GetDeepLink();
                if (unreadDeepLink != null) {
                    TryHandleDeepLink(unreadDeepLink);
                }
            }
        }

        #endif
        private static string GetDeepLink() {
            if (Application.platform == RuntimePlatform.Android) {
                #if !SUPPORTS_deepLinkActivated
                // On old Unity versions, Application.absoluteURL doesn't contain the deeplink on Android
                return DeepLinkForwarderActivity.CallStatic<string>("getUnreadDeepLink");
                #else
                return Application.absoluteURL;
                #endif
            } else {
                return null;
            }
        }
    }
}