using System;
using System.Collections.Generic;

namespace xasset
{
    /// <summary>
    ///     批量下载文件的的操作，下载失败后，可以通过 <see cref="Retry" /> 方法对失败的内容重启下载。
    /// </summary>
    public sealed class DownloadFiles : Operation
    {
        /// <summary>
        ///     已经下载的
        /// </summary>
        private readonly List<Download> _downloaded = new List<Download>();

        /// <summary>
        ///     下载失败的
        /// </summary>
        private readonly List<Download> _errors = new List<Download>();

        /// <summary>
        ///     下载中的内容
        /// </summary>
        private readonly List<Download> _progressing = new List<Download>();

        /// <summary>
        ///     需要下载的信息
        /// </summary>
        public readonly List<DownloadInfo> files = new List<DownloadInfo>();

        /// <summary>
        ///     更新时触发
        /// </summary>
        public Action<DownloadFiles> updated;

        /// <summary>
        ///     总下载大小
        /// </summary>
        public long totalSize { get; private set; }

        /// <summary>
        ///     已经下载了的大小
        /// </summary>
        public long downloadedBytes { get; private set; }

        public override void Start()
        {
            base.Start();
            downloadedBytes = 0;
            _progressing.Clear();
            _downloaded.Clear();
            foreach (var info in files)
            {
                totalSize += info.size;
            }

            if (files.Count > 0)
            {
                foreach (var item in files)
                {
                    var download = Download.DownloadAsync(item);
                    _progressing.Add(download);
                    download.retryEnabled = false;
                }
            }
            else
            {
                Finish();
            }
        }

        public void Retry()
        {
            base.Start();
            foreach (var download in _errors)
            {
                Download.Retry(download);
                _progressing.Add(download);
            }

            _errors.Clear();
        }

        protected override void Update()
        {
            if (status != OperationStatus.Processing)
            {
                return;
            }

            if (_progressing.Count > 0)
            {
                var len = 0L;
                for (var index = 0; index < _progressing.Count; index++)
                {
                    var item = _progressing[index];
                    if (item.isDone)
                    {
                        _progressing.RemoveAt(index);
                        index--;
                        _downloaded.Add(item);
                        if (item.status == DownloadStatus.Failed)
                        {
                            _errors.Add(item);
                        }
                    }
                    else
                    {
                        len += item.downloadedBytes;
                    }
                }

                foreach (var item in _downloaded)
                {
                    len += item.downloadedBytes;
                }

                downloadedBytes = len;
                progress = downloadedBytes * 1f / totalSize;
                updated?.Invoke(this);
                return;
            }

            updated = null;
            Finish(_errors.Count > 0 ? "部分文件下载失败。" : null);
        }
    }
}