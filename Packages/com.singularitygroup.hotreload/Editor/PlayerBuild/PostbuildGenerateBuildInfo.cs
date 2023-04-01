using System.IO;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    
#pragma warning disable CS0618
    internal class PostbuildGenerateBuildInfo : IPostprocessBuild, IPostGenerateGradleAndroidProject {
#pragma warning restore CS0618
        public int callbackOrder => 10;

        public void OnPostprocessBuild(BuildTarget target, string path) {
            if (target != BuildTarget.Android) {
                // Note: This code might be needed once we support standalone: SG-29499
                //  Test that path to streaming assets works correctly with Standalone 'Build and Run'.
                //  (path might be to exe file, like it was for android apk)
                // var dir = HotReloadBuildHelper.GetStreamingAssetsBuiltPath(target, path);
                // var buildFilePath = Path.Combine(dir, BuildInfo.GetStoredName());
                // GenerateBuildInfo(buildFilePath, target);
            }
        }

        // only called on Android
        public void OnPostGenerateGradleAndroidProject(string unityLibraryPath) {
            if (!HotReloadBuildHelper.IncludeInThisBuild()) {
                return;
            }
            // write BuildInfo json into the built StreamingAssets directory
            var dir = HotReloadBuildHelper.GetStreamingAssetsBuiltPath(BuildTarget.Android, unityLibraryPath);
            var buildFilePath = Path.Combine(dir, BuildInfo.GetStoredName());
            GenerateBuildInfo(buildFilePath, BuildTarget.Android);
        }

        private static void GenerateBuildInfo(string buildFilePath, BuildTarget buildTarget) {
            var buildInfo = BuildInfoHelper.GenerateBuildInfoMainThread(buildTarget);
            // write to StreamingAssets
            // create StreamingAssets folder if not exists (in-case project has no StreamingAssets files)
            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(Path.GetDirectoryName(buildFilePath));
            File.WriteAllText(buildFilePath, buildInfo.ToJson());
        }
    }
}