using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace xasset.example
{
    public class CheckUpdateScreen : MonoBehaviour
    {
        public Text version;

        private void Start()
        {
            SetVersion();
        }

        private void OnDestroy()
        {
            MessageBox.CloseAll();
        }

        public void SetVersion()
        {
            version.text = $"资源版本：{Versions.ManifestVersion}";
        }

        public void ClearAll()
        {
            Versions.ClearDownload();
        }

        public void ClearHistory()
        {
            MessageBox.Show("提示", "保留历史版本数据可以获得更快的更新体验，请确认是否清理？", ok =>
            {
                if (ok)
                {
                    PreloadManager.Instance.ShowProgress(Versions.ClearAsync());
                }
            }, "清理", "退出");
        }

        public void StartCheck()
        {
            StartCoroutine(Checking());
        }

        private void GetDownloadSizeAsync()
        {
            StartCoroutine(GetDownloadSize());
        }

        private IEnumerator GetDownloadSize()
        {
            var getDownloadSize = Versions.GetDownloadSizeAsync();
            yield return getDownloadSize;
            if (getDownloadSize.downloadSize > 0)
            {
                var messageBox = MessageBox.Show("提示",
                    $"发现更新({Utility.FormatBytes(getDownloadSize.downloadSize)})，是否下载？", null, "下载", "跳过");
                yield return messageBox;
                if (messageBox.ok)
                {
                    PreloadManager.Instance.DownloadAsync(getDownloadSize.DownloadAsync(), OnComplete);
                    yield break;
                }
            }

            OnComplete();
        }

        private IEnumerator Checking()
        {
            PreloadManager.Instance.SetVisible(true);
            PreloadManager.Instance.SetMessage("获取版本信息...", 0);
            var checking = Versions.CheckUpdateAsync();
            yield return checking;
            if (checking.status == OperationStatus.Failed)
            {
                MessageBox.Show("提示", "更新版本信息失败，请检测网络链接后重试。", ok =>
                {
                    if (ok)
                    {
                        StartCheck();
                    }
                    else
                    {
                        OnComplete();
                    }
                }, "重试", "跳过");
                yield break;
            }

            PreloadManager.Instance.SetMessage("获取版本信息...", 1);
            if (checking.downloadSize > 0)
            {
                var messageBox = MessageBox.Show("提示", $"发现版本更新({Utility.FormatBytes(checking.downloadSize)})，是否下载？",
                    null, "下载", "跳过");
                yield return messageBox;
                if (messageBox.ok)
                {
                    PreloadManager.Instance.DownloadAsync(checking.DownloadAsync(), GetDownloadSizeAsync);
                    yield break;
                }
            }

            GetDownloadSizeAsync();
        }

        private void OnComplete()
        {
            SetVersion();
            PreloadManager.Instance.SetMessage("更新完成", 1);
            PreloadManager.Instance.StartCoroutine(LoadScene());
        }

        private static IEnumerator LoadScene()
        {
            var scene = Scene.LoadAsync("Menu");
            PreloadManager.Instance.ShowProgress(scene);
            yield return scene;
        }
    }
}