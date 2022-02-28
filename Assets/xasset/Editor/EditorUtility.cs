using UnityEditor;
using UnityEngine;

namespace xasset.editor
{
    public static class EditorUtility
    {
        public static T FindOrCreateAsset<T>(string path) where T : ScriptableObject
        {
            var guilds = AssetDatabase.FindAssets($"t:{typeof(T).FullName}");
            foreach (var guild in guilds)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guild);
                if (string.IsNullOrEmpty(assetPath))
                {
                    continue;
                }

                var asset = GetOrCreateAsset<T>(assetPath);
                if (asset == null)
                {
                    continue;
                }

                return asset;
            }

            return GetOrCreateAsset<T>(path);
        }

        private static T GetOrCreateAsset<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                return asset;
            }

            Utility.CreateDirectoryIfNecessary(path);
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }
    }
}