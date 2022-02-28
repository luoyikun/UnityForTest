using System;
using System.Collections.Generic;
using UnityEngine;

namespace xasset
{
    //包：子类为本地包，网络包
    public class Bundle : Loadable
    {
        public static readonly Dictionary<string, Bundle> Cache = new Dictionary<string, Bundle>();

        protected ManifestBundle info;
        public static Func<string, ManifestBundle, Bundle> customLoader { get; set; } = null;

        public AssetBundle assetBundle { get; protected set; }

        protected AssetBundleCreateRequest LoadAssetBundleAsync(string url)
        {
            Logger.I("LoadAssetBundleAsync", info.nameWithAppendHash);
            return AssetBundle.LoadFromFileAsync(url);
        }

        protected AssetBundle LoadAssetBundle(string url)
        {
            Logger.I("LoadAssetBundle", info.nameWithAppendHash);
            return AssetBundle.LoadFromFile(url);
        }

        protected void OnLoaded(AssetBundle bundle)
        {
            assetBundle = bundle;
            Finish(assetBundle == null ? "assetBundle == null" : null);
        }

        internal static Bundle LoadInternal(ManifestBundle bundle)
        {
            if (bundle == null)
            {
                throw new NullReferenceException();
            }

            if (!Cache.TryGetValue(bundle.nameWithAppendHash, out var item))
            {
                var url = PathManager.GetBundlePathOrURL(bundle);
                if (customLoader != null)
                {
                    item = customLoader(url, bundle);
                }

                if (item == null)
                {
                    if (url.StartsWith("http://") || url.StartsWith("https://") || url.StartsWith("ftp://"))
                    {
                        item = new DownloadBundle { pathOrURL = url, info = bundle };
                    }
                    else
                    {
                        item = new LocalBundle { pathOrURL = url, info = bundle };
                    }
                }

                Cache.Add(bundle.nameWithAppendHash, item);
            }

            item.Load();
            return item;
        }

        protected override void OnUnload()
        {
            Cache.Remove(info.nameWithAppendHash);
            if (assetBundle == null)
            {
                return;
            }

            assetBundle.Unload(true);
            assetBundle = null;
        }
    }
}