using UnityEngine;

namespace SingularityGroup.HotReload {
    class UnityAndroidHelpers {
        private static readonly AndroidJavaClass jvmClass = new AndroidJavaClass("com.singularitygroup.UnityAndroidHelpers");

        /// Read an apk asset file as UTF8 string.
        /// <returns>
        /// returns empty string if asset file not found
        /// </returns>
        public static string ReadTextFromApkAssets(string assetPath) {
            return jvmClass.CallStatic<string>("readFromApkAssets", ActivityHelpers.UnityActivity, assetPath);
        }
    }
}