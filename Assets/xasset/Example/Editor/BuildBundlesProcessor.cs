using UnityEditor;
using xasset.editor;

namespace xasset.example.editor
{
    [InitializeOnLoad]
    public static class BuildBundlesProcessor
    {
        static BuildBundlesProcessor()
        {
            BuildScript.preprocessBuildBundles += PreprocessBuildBundles;
            BuildScript.postprocessBuildBundles += PostprocessBuildBundles;
        }

        private static void PreprocessBuildBundles(BuildTask task)
        {
        }

        private static void PostprocessBuildBundles(BuildTask task)
        {
        }
    }
}