using System;
using System.Collections;
using UnityEngine;

namespace xasset.example
{
    [DisallowMultipleComponent]
    public class PreloadManager : MonoBehaviour
    {
        public IProgressBar progressBar;

        public static PreloadManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            MessageBox.Dispose();
        }

        public void SetVisible(bool visible)
        {
            progressBar?.SetVisible(visible);
        }

        public void SetMessage(string message, float progress)
        {
            SetVisible(true);
            if (progressBar != null)
            {
                progressBar.SetMessage(message);
                progressBar.SetProgress(progress);
            }
        }

        private static string FormatBytes(long bytes)
        {
            return Utility.FormatBytes(bytes);
        }

        public void ShowProgress(Scene loading)
        {
            SetVisible(true);
            loading.completed += scene => { SetVisible(false); };
            loading.updated += scene =>
            {
                if (Download.Working)
                {
                    var current = Download.TotalDownloadedBytes;
                    var max = Download.TotalSize;
                    var speed = Download.TotalBandwidth;
                    SetMessage($"加载中...{FormatBytes(current)}/{FormatBytes(max)}(速度 {FormatBytes(speed)}/s)",
                        current * 1f / max);
                }
                else
                {
                    SetMessage($"加载游戏场景，不消耗流量... {scene.progress * 100:F2}%", scene.progress);
                }
            };
        }

        public void ShowProgress(Download downloading)
        {
            SetVisible(true);
            downloading.completed += scene => { SetVisible(false); };
            downloading.updated += scene =>
            {
                var current = Download.TotalDownloadedBytes;
                var max = Download.TotalSize;
                var speed = Download.TotalBandwidth;
                SetMessage(
                    $"加载中...{FormatBytes(current)}/{FormatBytes(max)}(速度 {FormatBytes(speed)}/s)",
                    current * 1f / max);
            };
        }

        public void ShowProgress(ClearFiles clear)
        {
            SetVisible(true);
            clear.completed += o => { SetVisible(false); };
            clear.updated += (o, file) => { SetMessage($"清理中...{file}{clear.id}/{clear.max}", clear.progress); };
        }

        public void ShowProgress(DownloadFiles download)
        {
            SetVisible(true);
            download.completed += o => { SetVisible(false); };
            download.updated += o =>
            {
                var current = Download.TotalDownloadedBytes;
                var max = Download.TotalSize;
                var speed = Download.TotalBandwidth;
                SetMessage(
                    $"加载中...{FormatBytes(current)}/{FormatBytes(max)}(速度 {FormatBytes(speed)}/s)",
                    current * 1f / max);
            };
        }

        public void DownloadAsync(DownloadFiles download, Action completed)
        {
            StartCoroutine(Downloading(download, completed));
        }

        private IEnumerator Downloading(DownloadFiles download, Action completed)
        {
            ShowProgress(download);
            yield return download;
            if (download.status == OperationStatus.Failed)
            {
                var messageBox = MessageBox.Show("提示！", "下载失败！请检查网络状态后重试。");
                yield return messageBox;
                if (messageBox.ok)
                {
                    download.Retry();
                    DownloadAsync(download, completed);
                }
                else
                {
                    Application.Quit();
                }

                yield break;
            }

            completed?.Invoke();
        }
    }
}