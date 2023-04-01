using UnityEngine;

namespace SingularityGroup.HotReload {
    static class ActivityHelpers {
        private static readonly AndroidJavaClass jvmClass = new AndroidJavaClass("com.singularitygroup.ActivityHelpers");
        
        public static void TryLaunchQRScannerApp() {
            var activityStarted = jvmClass.CallStatic<bool>("tryLaunchQRScannerApp", UnityActivity);
            if (!activityStarted) {
                // show in-game dialog that you need to install a QR Scanner app
                Prompts.ShowInstallQRScannerDialog();
            }
        }
        
        static AndroidJavaObject m_activity = null;

        /// <summary>
        /// This is the UnityPlayer activity
        /// </summary>
        public static AndroidJavaObject UnityActivity {
            get {
                if (m_activity == null) {
                    using (AndroidJavaObject unityClass =
                           new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                        m_activity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
                    }
                }

                return m_activity;
            }
        }
    }
}