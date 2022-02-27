using System.IO;

namespace xasset
{
    public class DownloadInfo
    {
        public string hash;
        public string savePath;
        public long size;
        public string url;

        public long downloadedSize
        {
            get
            {
                var info = new FileInfo(savePath);
                if (info.Exists)
                {
                    return info.Length;
                }

                return 0;
            }
        }

        public long downloadSize => size - downloadedSize;
    }
}