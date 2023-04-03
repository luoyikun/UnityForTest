using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEditor.Android;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
#pragma warning disable CS0618
    /// <remarks>
    /// <para>
    /// This class sets option in the AndroidManifest that you choose in HotReload build settings.
    /// </para>
    /// <para>
    /// - To connect to the HotReload server through the local network, we need to permit access to http://192...<br/>
    /// - Starting with Android 9, insecure http requests are not allowed by default and must be whitelisted
    /// </para>
    /// </remarks>
    internal class PostbuildModifyAndroidManifest : IPostGenerateGradleAndroidProject {
#pragma warning restore CS0618
        public int callbackOrder => 10;

        private const string manifestFileName = "AndroidManifest.xml";

        public void OnPostGenerateGradleAndroidProject(string path) {
            if (!HotReloadBuildHelper.IncludeInThisBuild()) {
                return;
            }
            // Note: we have it on the roadmap to support having multiple apps installed with hot reload: SG-29580
            //AddDeeplinkForwarder(manifestFilePath);

            var manifestFilePath = FindAndroidManifest(path);
            
            EnsureLibraryWillBuild(path);
            
            var options = HotReloadSettingsEditor.LoadSettingsOrDefault();

            // Note: in future we may support users with custom configuration for usesCleartextTraffic
            if (options.AllowAndroidAppToMakeHttpRequests) {
                #if UNITY_2022_1_OR_NEWER
                // Unity 2022 or newer → do nothing, we rely on Unity option to control the flag
                #else
                // Unity 2021 or older → put manifest flag in if Unity is making a Development Build
                if (manifestFilePath == null) {
                    // not found
                    Log.Warning($"[{CodePatcher.TAG}] Unable to find {manifestFileName}");
                    return;
                }
                SetUsesCleartextTraffic(manifestFilePath);
                #endif
            }
        }

        private static string ApplicationIdentifer => Application.identifier;

        /// identifier that is used in the deeplink uri scheme
        /// (initially tried Application.identifier, but that was giving unexpected results based on PlayerSettings)
        //  SG-29580
        //  Something to uniqly identify the application, but it must be something which is highly likely
        //  to be the same at build time (studio might have logic to set e.g. product name to MyGameProd or MyGameTest)
        public static string ApplicationIdentiferSlug => "app";
/*
        public static string ApplicationIdentiferSlug => Regex.Replace(ApplicationIdentifer, @"[^a-zA-Z0-9\.\-]", "")
            .Replace("..", ".") // happens if your companyname in Unity ends with a dot
            .ToLowerInvariant();

        private static void AddDeeplinkForwarder(string manifestFilePath) {
            // add the hotreload-${identifier} uri scheme to the AndroidManifest.xml file
            // it should be added as part of an intent-filter for the activity "com.singularitygroup.deeplinkforwarder.DeepLinkForwarderActivity"
            var contents = File.ReadAllText(manifestFilePath);
            if (contents.Contains("android:name=\"com.singularitygroup.deeplinkforwarder.DeepLinkForwarderActivity\"")) {
                // user has already set this themselves, don't replace it
                return;
            }

            //note: not using android:host or any other data attr because android still shows a chooser for all ur hotreload apps
            // Therefore must use a unique uri scheme to ensure only one app can handle it.
            var activityWithIntentFilter = @"
<activity android:name=""com.singularitygroup.deeplinkforwarder.DeepLinkForwarderActivity"">
    <intent-filter>
        <action android:name=""android.intent.action.VIEW"" />
        <category android:name=""android.intent.category.DEFAULT"" />
        <category android:name=""android.intent.category.BROWSABLE"" />
        <data android:scheme=""hotreload-" + ApplicationIdentiferSlug + @""" />
    </intent-filter>
</activity>";
            var newContents = Regex.Replace(contents,
                @"</application>",
                activityWithIntentFilter + "\n    </application>"
            );
            File.WriteAllText(manifestFilePath, newContents);
        }
*/
        // Assume unityLibraryPath is to {gradleProject}/unityLibrary/ which is roughly the same across Unity versions 2018/2019/2020/2021/2022
        private static string FindAndroidManifest(string unityLibraryPath) {
            // find the AndroidManifest.xml file which we can edit
            var dir = new DirectoryInfo(unityLibraryPath);
            var manifestFilePath = Path.Combine(dir.FullName, "src", "main", manifestFileName);
            if (File.Exists(manifestFilePath)) {
                return manifestFilePath;
            }

            Log.Info("[{0}] Did not find {1} at {2}, searching for manifest file inside {3}", CodePatcher.TAG, manifestFileName, manifestFilePath, dir.FullName);
            var manifestFiles = dir.GetFiles(manifestFileName, SearchOption.AllDirectories);
            if (manifestFiles.Length == 0) {
                return null;
            }

            foreach (var file in manifestFiles) {
                if (file.FullName.Contains("src")) {
                    // good choice
                    return file.FullName;
                }
            }
            // fallback to the first file found
            return manifestFiles[0].FullName;
        }

        /// <summary>
        /// Set option android:usesCleartextTraffic="true"

        /// </summary>
        /// <param name="manifestFilePath">Absolute filepath to the unityLibrary AndroidManifest.xml file</param>
        private static void SetUsesCleartextTraffic(string manifestFilePath) {
            // Ideally we would create or modify a "Network Security Configuration file" to permit access to local ip addresses
            // https://developer.android.com/training/articles/security-config#manifest
            // but that becomes difficult when the user has their own configuration file - would need to search for it and it may be inside an aar.
            var contents = File.ReadAllText(manifestFilePath);
            if (contents.Contains("android:usesCleartextTraffic=")) {
                // user has already set this themselves, don't replace it
                return;
            }
            var newContents = Regex.Replace(contents,
                @"<application\s",
                "<application android:usesCleartextTraffic=\"true\" "
            );
            newContents += $"\n<!-- [{CodePatcher.TAG}] Added android:usesCleartextTraffic=\"true\" to permit connecting to the Hot Reload http server running on your machine. -->";
            newContents += $"\n<!-- [{CodePatcher.TAG}] This change only happens in Unity development builds. You can disable this in the Hot Reload settings window. -->";
            File.WriteAllText(manifestFilePath, newContents);
        }

        /// To launch qr-code scanner, we need to add package visibility needs to &lt;queries&gt; in the AndroidManifest.xml
        static void EnsureLibraryWillBuild(string unityLibraryPath) {
            if (AndroidPackageVisibility.AndroidProjectSupportsPackageVisibility(unityLibraryPath)) {
                return;
            }
            // Replace the helper library aar with the legacy aar
            // make sure helper library aar exists at expected location
            // only replace it if exists because we don't want to break the build
            var helperLibraryAarPath = Path.Combine(unityLibraryPath, "libs", "hotreload-helper.aar");
            var helperLibraryLegacyAarPath = TryFindLegacyAarFilepath();
            if (helperLibraryLegacyAarPath != null && File.Exists(helperLibraryAarPath)) {
                Log.Info("Replacing hotreload-helper.aar with hotreload-helper.aar.legacy because your Android gradle plugin version may not support package visibility");
                File.Copy(helperLibraryLegacyAarPath, helperLibraryAarPath, true);
            }
        }
        
        [CanBeNull]
        public static string TryFindLegacyAarFilepath() {
            const string basePath = "Packages/com.singularitygroup.hotreload/Runtime/Android";
            const string legacyAarName = "hotreload-helper.aar.legacy";
            var executablePath = Path.Combine(basePath, legacyAarName); 
            
            if(File.Exists(executablePath)) {
                return Path.GetFullPath(executablePath);
            }
            
            //Not found in packages. Try to find in assets folder.
            //fast path - this is the expected folder
            var altFolderPath = executablePath.Replace(basePath, "Assets/HotReload/Runtime/Android");
            if(Directory.Exists(altFolderPath)) {
                return Path.GetFullPath(altFolderPath);
            }
            //slow path - try to find the executable somewhere in the assets folder
            var files = Directory.GetFiles("Assets", legacyAarName, SearchOption.AllDirectories);
            if(files.Length > 0) {
                return Path.GetFullPath(files[0]);
            }
            return null;
        }
    }
}