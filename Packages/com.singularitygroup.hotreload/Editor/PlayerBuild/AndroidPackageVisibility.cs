using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;

[assembly: InternalsVisibleTo("SingularityGroup.HotReload.EditorTests")]

namespace SingularityGroup.HotReload.Editor {
    static class AndroidPackageVisibility {

        /// Older versions of AGP fail to build when &lt;queries&gt; is in any manifest
        public static bool AndroidProjectSupportsPackageVisibility(string unityLibraryPath) {
            // Newer Unity versions use AGP version that supports package visibility.
            // Assumption: user didn't downgrade AGP version below Unity's default.
        #if UNITY_2022_1_OR_NEWER
            // Unity 2022.1.0 "Android Gradle Plugin version upgraded from 3.6.0 to 4.0.1."
            return true;
        #elif UNITY_2021_2_OR_NEWER && !UNITY_2022_1_OR_NEWER
            // Unity 2021.1.16 "Gradle Plugin version upgraded from 3.6.0 to 4.0.1."
            return true;
        #else
            var baseGradleFilepath = FindBaseProjectGradle(unityLibraryPath);
            if (baseGradleFilepath == null) {
                Log.Debug($"Did not find base project gradle file in {unityLibraryPath}");
                return false;
            }

            var agpVersion = GetAGPVersion(new FileInfo(baseGradleFilepath));
            if (agpVersion == null) {
                Log.Debug("Failed to find AGP version, skipping <queries> patch for Scan QR-Code button");
                return false;
            }

            Log.Debug($"Testing {agpVersion} supports <queries> - version was read from file {baseGradleFilepath}");
            return AGPSupportsPackageVisibility(agpVersion);
        #endif
        }

        public static Version GetAGPVersion(FileInfo baseGradleFilepath) {
            try {
                var contents = File.ReadAllText(baseGradleFilepath.FullName);
                return GetAGPVersion(contents);
            } catch (Exception ex) {
                if (ex is IOException) {
                    // ignore file not found
                } else {
                    Log.Warning("Failed to read AGP version from {0}: {1}", baseGradleFilepath, ex);
                }
                return null;
            }
        }

        internal static Version GetAGPVersion(string baseGradleContents) {
            //classpath 'com.android.tools.build:gradle:3.4.0'
            var match = Regex.Match(baseGradleContents,
                // match x.x or x.x.x
                @"[\s\(]['""]com.android.tools.build:gradle:(\d+\.\d+(?:\.\d+)?)['""]");
            if (match.Success) {
                return new Version(match.Groups[1].Value);
            }
            return null;
        }
        
        public static bool AGPSupportsPackageVisibility(Version v) {
            /* Google released a series of patch versions of the Android Gradle Plugin to address this:
             3.3.3
             3.4.3
             3.5.4
             3.6.4
             4.0.1
             https://stackoverflow.com/a/62969918*/
            // note: if version omits build number, its -1 (so we always use `v.Build < 1` to check for 0)
            if (v.Major <= 4) {
                if (v.Major == 3 && v.Minor < 3) {
                    return false;
                }
                if (v.Major == 3 && v.Minor == 3 && v.Build < 3) {
                    return false;
                }
                if (v.Major == 3 && v.Minor == 4 && v.Build < 3) {
                    return false;
                }
                if (v.Major == 3 && v.Minor == 5 && v.Build < 4) {
                    return false;
                }
                if (v.Major == 3 && v.Minor == 6 && v.Build < 4) {
                    return false;
                }
                if (v.Major == 4 && v.Minor <= 0 && v.Build < 1) {
                    return false;
                }
            }
            return true;
        }

        // Assume unityLibraryPath is to {gradleProject}/unityLibrary/ which is roughly the same across Unity versions 2018/2019/2020/2021/2022
        private static string FindBaseProjectGradle(string unityLibraryPath) {
            var dir = new DirectoryInfo(unityLibraryPath);

            var testDirPath = dir.FullName;
            if (File.Exists(Path.Combine(testDirPath, "settings.gradle"))) {
                return Path.Combine(dir.FullName, "build.gradle");
            }

            testDirPath = dir.Parent.FullName;
            if (File.Exists(Path.Combine(testDirPath, "settings.gradle"))) {
                return Path.Combine(dir.Parent.FullName, "build.gradle");
            }

            return null;
        }

    }
}