using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace xasset
{
    /// <summary>
    ///     运行时初始化操作，主要执行以下流程：
    ///     1. 从安装包读取 versions 文件，放到临时目录
    ///     2. 根据安装包的 versions 文件把包内的清单文件按需读取到下载目录。（下载目录没有才读取）
    ///     3. 读取下载目录的 versions 文件，从服务器下载而来，然后按需采集需要加载的清单文件。
    ///     4. 使用自动分帧加载清单文件，避免卡顿。
    /// </summary>
    public class InitializeVersions : Operation
    {
        private readonly List<BuildVersion> _loading = new List<BuildVersion>();
        private readonly List<UnityWebRequestAsyncOperation> _operations = new List<UnityWebRequestAsyncOperation>();

        private BuildVersions _local;

        private string _savePath;

        private Step _step;

        public override void Start()
        {
            base.Start();
            // 加载安装包内的版本文件，保存在临时目录。
            _savePath = PathManager.GetTemporaryPath(Versions.Filename);
            var copy = DownloadAsync(PathManager.GetPlayerDataURL(Versions.Filename), _savePath);
            _operations.Add(copy);
            _step = Step.LoadingLocalVersions;
        }

        private static UnityWebRequestAsyncOperation DownloadAsync(string url, string savePath)
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }

            var request = UnityWebRequest.Get(url);
            request.downloadHandler = new DownloadHandlerFile(savePath);
            return request.SendWebRequest();
        }


        protected override void Update()
        {
            if (status != OperationStatus.Processing)
            {
                return;
            }

            if (Progressing())
            {
                return;
            }

            Clear();

            switch (_step)
            {
                case Step.LoadingLocalVersions:
                    LoadLocalVersions();
                    break;
                case Step.LoadingServerVersions:
                    LoadServerVersions();
                    break;
                case Step.UpdateLoading:
                    UpdateLoading();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateLoading()
        {
            while (_loading.Count > 0)
            {
                var version = _loading[0];
                Versions.LoadVersion(version);
                _loading.RemoveAt(0);
                if (Updater.busy)
                {
                    return;
                }
            }

            Finish();
        }

        private void Clear()
        {
            foreach (var operation in _operations)
            {
                operation.webRequest.Dispose();
            }

            _operations.Clear();
        }

        private bool Progressing()
        {
            foreach (var request in _operations)
            {
                if (!request.isDone)
                {
                    return true;
                }
            }

            return false;
        }

        private void LoadServerVersions()
        {
            var versions = new Dictionary<string, BuildVersion>();
            var server = BuildVersions.Load(Downloader.GetDownloadDataPath(Versions.Filename));
            // 服务器的版本比本地新的时候，只要清单存在就放到待加载的队列。
            if (_local == null || server.timestamp > _local.timestamp)
            {
                foreach (var item in server.data)
                {
                    if (!Versions.Exist(item))
                    {
                        continue;
                    }

                    if (versions.ContainsKey(item.name))
                    {
                        Debug.LogWarningFormat("version exist:{0}", item.name);
                        continue;
                    }

                    versions[item.name] = item;
                    _loading.Add(item);
                }
            }

            // 本地版本存在的时候，加载不再加载队列的部分。
            if (_local != null)
            {
                foreach (var item in _local.data)
                {
                    if (versions.ContainsKey(item.name))
                    {
                        continue;
                    }

                    versions[item.name] = item;
                    _loading.Add(item);
                }

                // 加载安装包的版本数据。
                Versions.ReloadPlayerVersions(_local);
            }

            if (_loading.Count == 0)
            {
                Finish("找不到版本文件");
                return;
            }

            _step = Step.UpdateLoading;
        }

        private void LoadLocalVersions()
        {
            _local = BuildVersions.Load(_savePath);
            foreach (var item in _local.data)
            {
                if (!Versions.Exist(item))
                {
                    _operations.Add(DownloadAsync(PathManager.GetPlayerDataURL(item.file),
                        Downloader.GetDownloadDataPath(item.file)));
                }
            }

            _step = Step.LoadingServerVersions;
        }

        private enum Step
        {
            LoadingLocalVersions,
            LoadingServerVersions,
            UpdateLoading
        }
    }
}