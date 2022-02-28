using System.Collections.Generic;
using UnityEngine;

namespace xasset
{
    /// <summary>
    ///     加载路径管理器。
    /// </summary>
    public static class PathManager
    {
        /// <summary>
        ///     Bundle 的加载地址缓存
        /// </summary>
        internal static readonly Dictionary<string, string> BundleWithPathOrUrLs = new Dictionary<string, string>();

        /// <summary>
        ///     安装包数据目录
        /// </summary>
        public static string PlayerDataPath { get; set; } = $"{Application.streamingAssetsPath}/{Utility.buildPath}";

        /// <summary>
        ///     使用 UnityWebRequest 从安装包目录加载文件的时候需要使用的协议
        /// </summary>
        private static string LocalProtocol { get; set; }

        /// <summary>
        ///     平台名字，主要用在网络下载和编辑器
        /// </summary>
        public static string PlatformName { get; set; } = Utility.GetPlatformName();

        /// <summary>
        ///     根据别名获取实际加载路径
        /// </summary>
        /// <param name="asset">别名</param>
        public static void GetActualPath(ref string asset)
        {
            if (Versions.Manifest.GetActualPath(asset, out var value))
            {
                asset = value;
            }
        }

        /// <summary>
        ///     获取指定文件相对安装包的加载地址，专供 UnityWebRequest 使用。
        /// </summary>
        /// <param name="file">指定文件的文件名</param>
        /// <returns>指定文件相对安装包的加载地址</returns>
        public static string GetPlayerDataURL(string file)
        {
            return $"{LocalProtocol}{PlayerDataPath}/{file}";
        }

        /// <summary>
        ///     获取指定文件相对安装包的加载路径，专供 AssetBundle 加载使用。
        /// </summary>
        /// <param name="file">指定文件的文件名</param>
        /// <returns>指定文件相对安装包的加载路径</returns>
        private static string GetPlayerDataPath(string file)
        {
            return $"{PlayerDataPath}/{file}";
        }

        /// <summary>
        ///     获取临时目录，目前主要用来保存安装包内的 versions.json 文件。
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetTemporaryPath(string file)
        {
            var path = $"{Application.temporaryCachePath}/{file}";
            Utility.CreateDirectoryIfNecessary(path);
            return path;
        }

        /// <summary>
        ///     设置 bundle 的加载地址缓存
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="url"></param>
        internal static void SetBundlePathOrURl(string assetBundleName, string url)
        {
            BundleWithPathOrUrLs[assetBundleName] = url;
        }

        /// <summary>
        ///     获取 bundle 的加载地址缓存
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        internal static string GetBundlePathOrURL(ManifestBundle bundle)
        {
            var assetBundleName = bundle.nameWithAppendHash;
            //var assetBundleName = bundle.name;
            if (BundleWithPathOrUrLs.TryGetValue(assetBundleName, out var path))
            {
                return path;
            }

            var containsKey = Versions.StreamingAssets.Contains(assetBundleName);
            if (Versions.OfflineMode || containsKey)
            {
                path = GetPlayerDataPath(assetBundleName);
                BundleWithPathOrUrLs[assetBundleName] = path;
                return path;
            }

            if (Versions.IsDownloaded(bundle))
            {
                path = Downloader.GetDownloadDataPath(assetBundleName);
                BundleWithPathOrUrLs[assetBundleName] = path;
                return path;
            }

            path = Downloader.GetDownloadURL(assetBundleName);
            BundleWithPathOrUrLs[assetBundleName] = path;
            return path;
        }

        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            if (Application.platform != RuntimePlatform.OSXEditor &&
                Application.platform != RuntimePlatform.OSXPlayer &&
                Application.platform != RuntimePlatform.IPhonePlayer)
            {
                if (Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    LocalProtocol = "file:///";
                }
                else
                {
                    LocalProtocol = string.Empty;
                }
            }
            else
            {
                LocalProtocol = "file://";
            }
        }
    }
}