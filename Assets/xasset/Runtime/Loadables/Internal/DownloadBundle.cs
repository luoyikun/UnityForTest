using UnityEngine;

namespace xasset
{
    internal class DownloadBundle : Bundle
    {
        private Download _download;
        private AssetBundleCreateRequest _request;

        public override void LoadImmediate()
        {
            if (isDone)
            {
                return;
            }

            while (!_download.isDone)
            {
                Download.UpdateAll();
            }

            OnLoaded(_request == null ? LoadAssetBundle(_download.info.savePath) : _request.assetBundle);
            _request = null;
        }

        protected override void OnLoad()
        {
            _download = Download.DownloadAsync(Versions.GetDownloadInfo(info.nameWithAppendHash, info.hash, info.size));
            _download.completed += OnDownloaded;
        }

        private void OnDownloaded(Download obj)
        {
            if (_download.status == DownloadStatus.Failed)
            {
                Finish(_download.error);
                return;
            }

            PathManager.SetBundlePathOrURl(info.nameWithAppendHash, obj.info.savePath);
            if (assetBundle != null)
            {
                return;
            }

            var url = obj.info.savePath;
            _request = LoadAssetBundleAsync(url);
        }

        protected override void OnUpdate()
        {
            if (status != LoadableStatus.Loading)
            {
                return;
            }

            if (!_download.isDone)
            {
                progress = _download.downloadedBytes * 1f / _download.info.size * 0.5f;
                return;
            }

            if (_request == null)
            {
                return;
            }

            progress = 0.5f + _request.progress;
            if (!_request.isDone)
            {
                return;
            }

            OnLoaded(_request.assetBundle);
            _request = null;
        }
    }
}