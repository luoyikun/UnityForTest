using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;

namespace SingularityGroup.HotReload.Editor {
#pragma warning disable CS0618
    /// <summary>Includes HotReload Resources only in development builds</summary>
    /// <remarks>
    /// This build script ensures that HotReload Resources are not included in release builds.
    /// <para>
    /// When HotReload is enabled:<br/>
    ///   - include HotReloadSettingsObject in development Android builds.<br/>
    ///   - exclude HotReloadSettingsObject from the build.<br/>
    /// When HotReload is disabled:<br/>
    ///   - excludes HotReloadSettingsObject from the build.<br/>
    /// </para>
    /// </remarks>
    internal class PrebuildIncludeResources : IPreprocessBuild {
#pragma warning restore CS0618
        public int callbackOrder => 10;
        
        public void OnPreprocessBuild(BuildTarget target, string path) {
            try {
                if (HotReloadBuildHelper.IncludeInThisBuild()) {
                    // move scriptable object into Resources/ folder
                    HotReloadSettingsEditor.AddOrRemoveFromBuild(true);
                } else {
                    // make sure HotReload resources are not in the build
                    HotReloadSettingsEditor.AddOrRemoveFromBuild(false);
                }
            } catch (BuildFailedException) {
                throw;
            } catch (Exception ex) {
                throw new BuildFailedException(ex);
            }
        }

        // Do nothing in post build. settings asset will be dirty if build fails, so not worth fixing just for successful builds.
        // [PostProcessBuild]
        // private static void PostBuild(BuildTarget target, string pathToBuiltProject) {
        // }
    }

#pragma warning disable CS0618
#pragma warning restore CS0618
}
