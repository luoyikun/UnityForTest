using System;
using System.Collections.Generic;
using System.IO;

namespace xasset
{
    public enum VerifyMode
    {
        Size,
        Hash
    }

    /// <summary>
    ///     Versions 类，持有运行时的所有资源的版本信息和依赖关系。
    /// </summary>
    public static class Versions
    {
        public const string APIVersion = "2022.1p2";
        public const string Filename = "versions.json";
        public static readonly Manifest Manifest = Manifest.LoadFromFile("Manifest");
        internal static readonly List<string> StreamingAssets = new List<string>();

        public static VerifyMode VerifyMode { get; set; } = VerifyMode.Hash;

        /// <summary>
        ///     是否是仿真模式
        /// </summary>
        public static bool SimulationMode { get; private set; }

        /// <summary>
        ///     是否是离线模式
        /// </summary>
        public static bool OfflineMode { get; private set; }

        /// <summary>
        ///     获取清单的版本号
        /// </summary>
        public static string ManifestVersion => Manifest.version.ToString();

        public static Func<Operation> Initializer { get; set; } = () => new InitializeVersions();

        public static bool Initialized { get; internal set; }

        /// <summary>
        ///     获取下载信息。
        /// </summary>
        /// <param name="file">指定的文件名</param>
        /// <param name="hash">指定的文件哈希</param>
        /// <param name="size">指定文件的下载大小</param>
        /// <param name="fastVerify"></param>
        /// <returns></returns>
        public static DownloadInfo GetDownloadInfo(string file, string hash, long size, bool fastVerify = true)
        {
            if (VerifyMode == VerifyMode.Size && fastVerify)
            {
                hash = null;
            }

            var info = new DownloadInfo
            {
                hash = hash,
                size = size,
                savePath = Downloader.GetDownloadDataPath(file),
                url = Downloader.GetDownloadURL(file)
            };
            return info;
        }

        /// <summary>
        ///     加载安装包的版本文件。
        /// </summary>
        /// <param name="versions">安装包的版本文件</param>
        public static void ReloadPlayerVersions(BuildVersions versions)
        {
            StreamingAssets.Clear();
            // 版本数据为空的时候，是仿真模式。
            if (versions == null)
            {
                SimulationMode = true;
                OfflineMode = true;
                return;
            }

            StreamingAssets.AddRange(versions.streamingAssets);
            OfflineMode = versions.offlineMode;
            SimulationMode = false;
        }

        public static bool Changed(BuildVersion version)
        {
            return Manifest.nameWithAppendHash != version.file;
        }

        public static bool Exist(BuildVersion version)
        {
            if (version == null)
            {
                return false;
            }

            var info = new FileInfo(Downloader.GetDownloadDataPath(version.file));
            return info.Exists
                   && info.Length == version.size
                   && VerifyMode == VerifyMode.Size
                   || Utility.ComputeHash(info.FullName) == version.hash;
        }

        public static void LoadVersion(BuildVersion version)
        {
            var path = Downloader.GetDownloadDataPath(version.file);
            var manifest = Manifest.LoadFromFile(path);
            manifest.name = version.name;
            Logger.I("LoadVersion:{0} with file {1}.", version.name, path);
            manifest.nameWithAppendHash = version.file;
            LoadVersion(manifest);
        }

        public static void LoadVersion(Manifest manifest)
        {
            Manifest.Copy(manifest);
        }

        /// <summary>
        ///     清理版本数据，不传参数等于清理不在当前版本的所有历史数据，传参数表示清理指定资源和依赖。
        /// </summary>
        /// <returns></returns>
        public static ClearFiles ClearAsync(params string[] files)
        {
            var clearAsync = new ClearFiles();
            if (files.Length == 0)
            {
                if (Directory.Exists(Downloader.DownloadDataPath))
                {
                    clearAsync.files.AddRange(Directory.GetFiles(Downloader.DownloadDataPath));
                    var usedFiles = new HashSet<string> { Manifest.nameWithAppendHash };
                    foreach (var bundle in Manifest.bundles)
                    {
                        usedFiles.Add(bundle.nameWithAppendHash);
                    }

                    clearAsync.files.RemoveAll(file =>
                    {
                        var name = Path.GetFileName(file);
                        return usedFiles.Contains(name);
                    });
                }
            }
            else
            {
                var assets = new HashSet<string>();
                foreach (var file in files)
                {
                    if (!GetDependencies(file, out var bundle, out var deps))
                    {
                        continue;
                    }

                    assets.Add(Downloader.GetDownloadDataPath(bundle.nameWithAppendHash));
                    foreach (var dep in deps)
                    {
                        assets.Add(Downloader.GetDownloadDataPath(dep.nameWithAppendHash));
                    }
                }

                clearAsync.files.AddRange(assets);
            }

            clearAsync.Start();
            return clearAsync;
        }

        /// <summary>
        ///     清理所有下载数据
        /// </summary>
        public static void ClearDownload()
        {
            PathManager.BundleWithPathOrUrLs.Clear();
            if (Directory.Exists(Downloader.DownloadDataPath))
            {
                Directory.Delete(Downloader.DownloadDataPath, true);
            }
        }

        /// <summary>
        ///     初始化，会根据 versions.json 文件加载清单。
        /// </summary>
        /// <returns></returns>
        public static Operation InitializeAsync()
        {
            var operation = Initializer();
            operation.Start();
            operation.completed += o => { Initialized = true; };
            return operation;
        }

        /// <summary>
        ///     检查版本更新的操作
        /// </summary>
        /// <returns></returns>
        public static CheckUpdate CheckUpdateAsync()
        {
            var operation = new CheckUpdate();
            operation.Start();
            return operation;
        }

        /// <summary>
        ///     获取更新大小
        /// </summary> 
        /// <returns></returns>
        public static GetDownloadSize GetDownloadSizeAsync()
        {
            var getDownloadSize = new GetDownloadSize();
            getDownloadSize.bundles.AddRange(Manifest.bundles);
            getDownloadSize.Start();
            return getDownloadSize;
        }

        /// <summary>
        ///     批量下载指定集合的内容。
        /// </summary>
        /// <param name="items">要下载内容</param>
        /// <returns></returns>
        public static DownloadFiles DownloadAsync(params DownloadInfo[] items)
        {
            var download = new DownloadFiles();
            download.files.AddRange(items);
            download.Start();
            return download;
        }

        /// <summary>
        ///     判断 bundle 是否已经下载
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="checkStreamingAssets"></param>
        /// <returns></returns>
        public static bool IsDownloaded(ManifestBundle bundle, bool checkStreamingAssets = true)
        {
            if (bundle == null)
            {
                return false;
            }

            if (OfflineMode || checkStreamingAssets && StreamingAssets.Contains(bundle.nameWithAppendHash))
            {
                return true;
            }

            var path = Downloader.GetDownloadDataPath(bundle.nameWithAppendHash);
            var file = new FileInfo(path);
            if (!file.Exists)
            {
                return false;
            }

            if (file.Length == bundle.size && VerifyMode == VerifyMode.Size)
            {
                return true;
            }

            if (file.Length < bundle.size)
            {
                return false;
            }

            return bundle.hash == Utility.ComputeHash(path);
        }

        /// <summary>
        ///     获取指定资源的依赖
        /// </summary>
        /// <param name="assetPath">加载路径</param>
        /// <param name="mainBundle">主 bundle</param>
        /// <param name="dependencies">依赖的 bundle 集合</param>
        /// <returns></returns>
        public static bool GetDependencies(string assetPath, out ManifestBundle mainBundle,
            out ManifestBundle[] dependencies)
        {
            if (Manifest.Contains(assetPath))
            {
                mainBundle = Manifest.GetBundle(assetPath);
                dependencies = Manifest.GetDependencies(mainBundle);
                return true;
            }

            mainBundle = null;
            dependencies = null;
            return false;
        }

        /// <summary>
        ///     判断资源是否包含在当前版本中。
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static bool Contains(string assetPath)
        {
            return Manifest.Contains(assetPath);
        }

        public static ManifestBundle GetBundle(string bundle)
        {
            return Manifest.GetBundle(bundle);
        }

        public static string[] GetDependencies(string path)
        {
            return Manifest.GetDependencies(path);
        }
    }
}