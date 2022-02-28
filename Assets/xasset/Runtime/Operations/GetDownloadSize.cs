using System.Collections.Generic;

namespace xasset
{
    /// <summary>
    ///     异步获取下载大小。
    /// </summary>
    public sealed class GetDownloadSize : Operation
    {
        public readonly List<ManifestBundle> bundles = new List<ManifestBundle>();
        public readonly List<DownloadInfo> changes = new List<DownloadInfo>();

        /// <summary>
        ///     需要下载的大小
        /// </summary>
        public long downloadSize { get; private set; }

        /// <summary>
        ///     文件的总大小
        /// </summary>
        public long totalSize { get; private set; }

        public override void Start()
        {
            base.Start();

            if (Versions.OfflineMode)
            {
                Finish();
                return;
            }

            downloadSize = 0;
            if (bundles.Count == 0)
            {
                Finish();
            }
        }

        public DownloadFiles DownloadAsync()
        {
            var downloadFiles = Versions.DownloadAsync(changes.ToArray());
            return downloadFiles;
        }


        protected override void Update()
        {
            if (status != OperationStatus.Processing)
            {
                return;
            }

            while (bundles.Count > 0)
            {
                var bundle = bundles[0];
                var savePath = Downloader.GetDownloadDataPath(bundle.nameWithAppendHash);
                if (!Versions.IsDownloaded(bundle) && !changes.Exists(info => info.savePath == savePath))
                {
                    var info = Versions.GetDownloadInfo(bundle.nameWithAppendHash, bundle.hash, bundle.size);
                    downloadSize += info.downloadSize;
                    changes.Add(info);
                }

                totalSize += bundle.size;
                bundles.RemoveAt(0);
                if (Updater.busy)
                {
                    return;
                }
            }

            Finish();
        }
    }
}