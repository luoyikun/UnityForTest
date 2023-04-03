using UnityEditor;
using UnityEditor.Build;

namespace SingularityGroup.HotReload.Editor {
#pragma warning disable CS0618
    class PostbuildSendProjectState : IPostprocessBuild {
#pragma warning restore CS0618
        public int callbackOrder => 9999;
        public void OnPostprocessBuild(BuildTarget target, string path) {
            if (HotReloadBuildHelper.IncludeInThisBuild()) {
                // after build passes, need to send again because EditorApplication.delayCall isn't called.
                EditorCodePatcher.PostFilesState();
            }
        }
    }
}