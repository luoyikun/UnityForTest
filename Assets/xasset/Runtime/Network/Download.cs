using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using UnityEngine;

namespace xasset
{
    public class Download : CustomYieldInstruction
    {
        /// <summary>
        ///     等待下载的队列
        /// </summary>
        private static readonly List<Download> Prepared = new List<Download>();

        /// <summary>
        ///     下载中的队列，通过 <see cref="MaxDownloads" />> 控制并发数量
        /// </summary>
        private static readonly List<Download> Progressing = new List<Download>();

        /// <summary>
        ///     下载失败的队列，在 <see cref="Progressing" /> 处理完后，会自动添加到 Prepared 中重新下载。
        /// </summary>
        private static readonly List<Download> Errors = new List<Download>();

        /// <summary>
        ///     下载缓存，防止重复下载
        /// </summary>
        private static readonly Dictionary<string, Download> Cache = new Dictionary<string, Download>();

        private static float _lastSampleTime;
        private static long _lastTotalDownloadedBytes;
        private readonly byte[] _readBuffer = new byte[ReadBufferSize];
        private long _bandWidth;

        private int _retryTimes;
        private Thread _thread;
        private FileStream _writer;

        private Download()
        {
            status = DownloadStatus.Wait;
            downloadedBytes = 0;
        }

        public bool retryEnabled { get; set; } = true;

        /// <summary>
        ///     最大并行下载数量
        /// </summary>
        public static uint MaxDownloads { get; set; } = 10;

        /// <summary>
        ///     单个下载线程最大下载带宽，0 为限速，单位 byte
        /// </summary>
        public static long MaxBandwidth { get; set; }

        /// <summary>
        ///     获取当前下载的总带宽，可以用来显示速度
        /// </summary>
        public static long TotalBandwidth { get; private set; }

        /// <summary>
        ///     当前是否有下载任务
        /// </summary>
        public static bool Working => Progressing.Count > 0;

        /// <summary>
        ///     当前总下载的字节数
        /// </summary>
        public static long TotalDownloadedBytes
        {
            get
            {
                var value = 0L;
                foreach (var item in Cache)
                {
                    value += item.Value.downloadedBytes;
                }

                return value;
            }
        }

        /// <summary>
        ///     当前总下载大小
        /// </summary>
        public static long TotalSize
        {
            get
            {
                var value = 0L;
                foreach (var item in Cache)
                {
                    value += item.Value.info.size;
                }

                return value;
            }
        }

        /// <summary>
        ///     自动重启下载的次数
        /// </summary>
        public static int MaxRetryTimes { get; set; } = 3;

        /// <summary>
        ///     单线程单次IO读取缓冲大小
        /// </summary>
        public static uint ReadBufferSize { get; set; } = 1024 * 4;

        /// <summary>
        ///     ftp 用户名
        /// </summary>
        public static string FtpUserID { get; set; }

        /// <summary>
        ///     ftp 用户密码
        /// </summary>
        public static string FtpPassword { get; set; }

        public DownloadInfo info { get; private set; }
        public DownloadStatus status { get; private set; }
        public string error { get; private set; }
        public Action<Download> completed { get; set; }
        public Action<Download> updated { get; set; }

        public bool isDone => status == DownloadStatus.Failed || status == DownloadStatus.Success ||
                              status == DownloadStatus.Canceled;

        public float progress => downloadedBytes * 1f / info.size;

        public long downloadedBytes { get; private set; }

        public override bool keepWaiting => !isDone;

        public static void ClearAllDownloads()
        {
            foreach (var download in Progressing)
            {
                download.Cancel();
            }

            Prepared.Clear();
            Progressing.Clear();
            Cache.Clear();
            Errors.Clear();
        }

        public static Download DownloadAsync(string url, string savePath, Action<Download> completed = null,
            long size = 0, string hash = null)
        {
            return DownloadAsync(new DownloadInfo
            {
                url = url,
                savePath = savePath,
                hash = hash,
                size = size
            }, completed);
        }

        public static Download DownloadAsync(DownloadInfo info, Action<Download> completed = null)
        {
            if (!Cache.TryGetValue(info.url, out var download))
            {
                download = new Download
                {
                    info = info
                };
                Prepared.Add(download);
                Cache.Add(info.url, download);
            }
            else
            {
                Logger.W("Download url {0} already exist.", info.url);
            }

            if (completed != null)
            {
                download.completed += completed;
            }

            return download;
        }


        public static void UpdateAll()
        {
            if (Prepared.Count > 0)
            {
                for (var index = 0; index < Mathf.Min(Prepared.Count, MaxDownloads - Progressing.Count); index++)
                {
                    var download = Prepared[index];
                    Prepared.RemoveAt(index);
                    index--;
                    Progressing.Add(download);
                    download.Start();
                }
            }

            if (Progressing.Count > 0)
            {
                for (var index = 0; index < Progressing.Count; index++)
                {
                    var download = Progressing[index];
                    download.updated?.Invoke(download);

                    if (!download.isDone)
                    {
                        continue;
                    }

                    if (download.status == DownloadStatus.Failed)
                    {
                        if (download.retryEnabled)
                        {
                            Errors.Add(download);
                        }

                        Logger.E("Unable to download {0} with error {1}", download.info.url, download.error);
                    }
                    else
                    {
                        Logger.I("Success to download {0}", download.info.url);
                    }

                    Progressing.RemoveAt(index);
                    index--;
                    download.Complete();
                }

                if (!(Time.realtimeSinceStartup - _lastSampleTime >= 1))
                {
                    return;
                }

                TotalBandwidth = TotalDownloadedBytes - _lastTotalDownloadedBytes;
                _lastTotalDownloadedBytes = TotalDownloadedBytes;
                _lastSampleTime = Time.realtimeSinceStartup;
            }
            else
            {
                if (Cache.Count <= 0)
                {
                    return;
                }

                Cache.Clear();
                if (Errors.Count > 0)
                {
                    foreach (var download in Errors)
                    {
                        Retry(download);
                    }

                    Errors.Clear();
                }

                _lastTotalDownloadedBytes = 0;
                _lastSampleTime = Time.realtimeSinceStartup;
            }
        }

        public static void Retry(Download download)
        {
            download.status = DownloadStatus.Wait;
            download._retryTimes = 0;
            Prepared.Add(download);
            Cache[download.info.url] = download;
        }

        private void Retry()
        {
            status = DownloadStatus.Wait;
            Start();
        }

        public void UnPause()
        {
            Start();
        }

        public void Pause()
        {
            status = DownloadStatus.Wait;
        }

        public void Cancel()
        {
            error = "User Cancel.";
            status = DownloadStatus.Canceled;
        }

        private void Complete()
        {
            if (completed == null)
            {
                return;
            }

            completed.Invoke(this);
            completed = null;
        }

        private bool deleteFile;

        private void Run()
        {
            try
            {
                deleteFile = true;
                
                Downloading();
                CloseWrite();

                if (status == DownloadStatus.Failed || status == DownloadStatus.Canceled ||
                    status == DownloadStatus.Wait)
                {
                    return;
                }

                if (downloadedBytes != info.size)
                {
                    if (deleteFile)
                    { 
                        File.Delete(info.savePath);
                    }
                    error = $"Download lenght {downloadedBytes} mismatch to {info.size}";
                    if (CanRetry())
                    {
                        return;
                    }

                    status = DownloadStatus.Failed;
                    return;
                }

                if (!string.IsNullOrEmpty(info.hash))
                {
                    var hash = Utility.ComputeHash(info.savePath);
                    if (info.hash != hash)
                    {
                        File.Delete(info.savePath);
                        error = $"Download hash {hash} mismatch to {info.hash}";
                        if (CanRetry())
                        {
                            return;
                        }

                        status = DownloadStatus.Failed;
                        return;
                    }
                }

                status = DownloadStatus.Success;
            }
            catch (Exception e)
            {
                CloseWrite();
                error = e.Message;
                if (CanRetry())
                {
                    return;
                }

                status = DownloadStatus.Failed;
            }
        }

        private void CloseWrite()
        {
            if (_writer == null)
            {
                return;
            }

            _writer.Flush();
            _writer.Close();
            _writer = null;
        }

        private bool CanRetry()
        {
            if (!retryEnabled || _retryTimes >= MaxRetryTimes)
            {
                return false;
            }

            Logger.W("Failed to download {0} with error {1}, auto retry {2}", info.url, error, _retryTimes);
            Thread.Sleep(1000);
            Retry();
            _retryTimes++;
            return true;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors spe)
        {
            return true;
        }

        private void Downloading()
        {
            var request = CreateWebRequest();
            using (var response = request.GetResponse())
            {
                if (response.ContentLength > 0)
                {
                    if (info.size == 0)
                    {
                        info.size = response.ContentLength + downloadedBytes;
                    }

                    using (var reader = response.GetResponseStream())
                    {
                        if (downloadedBytes >= info.size)
                        {
                            return;
                        }

                        var startTime = DateTime.Now;
                        while (status == DownloadStatus.Progressing)
                        {
                            if (ReadToEnd(reader))
                            {
                                break;
                            }

                            UpdateBandwidth(ref startTime);
                        }
                    }
                }
                else
                {
                    status = DownloadStatus.Success;
                }
            }
        }

        private void UpdateBandwidth(ref DateTime startTime)
        {
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            while (MaxBandwidth > 0 &&
                   status == DownloadStatus.Progressing &&
                   _bandWidth >= MaxBandwidth / Progressing.Count &&
                   elapsed < 1000)
            {
                var wait = Mathf.Clamp((int)(1000 - elapsed), 1, 33);
                Thread.Sleep(wait);
                elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            }

            if (!(elapsed >= 1000))
            {
                return;
            }

            startTime = DateTime.Now;
            _bandWidth = 0L;
        }

        private WebRequest CreateWebRequest()
        {
            WebRequest request;
            if (info.url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
                request = GetHttpWebRequest();
            }
            else if (info.url.StartsWith("ftp", StringComparison.OrdinalIgnoreCase))
            {
                var ftpWebRequest = (FtpWebRequest)WebRequest.Create(info.url);
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                if (!string.IsNullOrEmpty(FtpUserID))
                {
                    ftpWebRequest.Credentials = new NetworkCredential(FtpUserID, FtpPassword);
                }

                if (downloadedBytes > 0)
                {
                    ftpWebRequest.ContentOffset = downloadedBytes;
                }

                request = ftpWebRequest;
            }
            else
            {
                request = GetHttpWebRequest();
            }

            return request;
        }

        private WebRequest GetHttpWebRequest()
        {
            var request = (HttpWebRequest)WebRequest.Create(info.url);
            request.ProtocolVersion = HttpVersion.Version10;
            if (downloadedBytes > 0)
            {
                request.AddRange(downloadedBytes);
            }

            return request;
        } 

        private bool ReadToEnd(Stream reader)
        {
            var len = reader.Read(_readBuffer, 0, _readBuffer.Length);
            if (len <= 0)
            {
                if (downloadedBytes < info.size)
                {
                    deleteFile = false;
                }
                return true;
            }

            _writer.Write(_readBuffer, 0, len);
            downloadedBytes += len;
            _bandWidth += len;
            return false;
        }

        private void Start()
        {
            if (status != DownloadStatus.Wait)
            {
                return;
            }

            Logger.I("Start download {0}", info.url);
            status = DownloadStatus.Progressing;
            var file = new FileInfo(info.savePath);
            try
            {
                if (file.Exists && file.Length > 0)
                {
                    if (info.size > 0 && file.Length == info.size)
                    {
                        status = DownloadStatus.Success;
                        return;
                    }

                    _writer = File.OpenWrite(info.savePath);
                    downloadedBytes = _writer.Length - 1;
                    if (downloadedBytes > 0)
                    {
                        _writer.Seek(-1, SeekOrigin.End);
                    }
                }
                else
                {
                    var dir = Path.GetDirectoryName(info.savePath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    _writer = File.Create(info.savePath);
                    downloadedBytes = 0;
                }

                _thread = new Thread(Run)
                {
                    IsBackground = true
                };
                _thread.Start();
            }
            catch (Exception e)
            {
                // 这里如果出现异常，一般是文件被外部程序占用了。
                CloseWrite();
                Logger.E(e.Message);
                error = e.Message;
                status = DownloadStatus.Failed;
            }
        }
    }
}