using UnityEditor;

namespace xasset.editor
{
    /// <summary>
    ///     仿真模式的初始化操作
    /// </summary>
    public class EditorInitializeVersions : Operation
    {
        public override void Start()
        {
            base.Start();
            var manifest = Manifest.LoadFromFile("Simulation");
            foreach (var assetBundleName in AssetDatabase.GetAllAssetBundleNames())
            {
                foreach (var asset in AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName))
                {
                    manifest.AddAsset(asset, null);
                }
            }

            Versions.LoadVersion(manifest);
            Versions.ReloadPlayerVersions(null);
            Finish();
        }
    }
}