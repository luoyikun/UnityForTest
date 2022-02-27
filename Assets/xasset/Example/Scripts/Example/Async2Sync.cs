using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace xasset.example
{
    public class Async2Sync : MonoBehaviour
    {
        public Image image;
        private readonly List<Asset> _assets = new List<Asset>();

        private void OnDestroy()
        {
            ReleaseAssets();
        }

        // Use this for initialization
        public void Load()
        {
            const string assetPath = "Logo";
            var assetType = typeof(Sprite);
            Profiler.BeginSample("LoadAsset:Async2Sync");
            _assets.Add(Asset.LoadAsync(assetPath, assetType,(x)=> { Debug.Log("LoadAsyncOK：" + assetPath); }));
            var asset = Asset.Load(assetPath, assetType);
            image.sprite = asset.Get<Sprite>();
            Profiler.EndSample();
            image.SetNativeSize();
            _assets.Add(asset);
        }

        public void ReleaseAssets()
        {
            foreach (var item in _assets)
            {
                item.Release();
            }

            _assets.Clear();
        }
    }
}