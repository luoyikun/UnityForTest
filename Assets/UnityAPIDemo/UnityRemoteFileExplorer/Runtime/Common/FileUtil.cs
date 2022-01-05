using System.IO;

namespace RemoteFileExplorer
{
    public class FileUtil
    {
        public static char SeparatorChar = '/';
        public static string Separator = SeparatorChar.ToString();

        /// <summary>
        /// 获取所有子目录，包括自身
        /// </summary>
        public static string[] GetAllDirectories(string path)
        {
            var subDirectories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
            string[] directories = new string[subDirectories.Length + 1];
            directories[0] = path;
            for (int i = 1; i < directories.Length; i++)
            {
                directories[i] = subDirectories[i - 1];
            }
            return directories;
        }   

        /// <summary>
        /// 获取所有子文件
        /// </summary>
        public static string[] GetAllFiles(string path)
        {
            return Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        }

        public static string FixedPath(string path)
        {
            path = path.Replace("\\", Separator);
            if(path.EndsWith(Separator)) path = path.Substring(0, path.Length - 1);
            return path;
        }

        public static string CombinePath(string path1, string path2)
        {
            if(string.IsNullOrEmpty(path2)) return path1;
            if(path2.StartsWith(Separator))
            {
                return path1 + path2;
            }
            else
            {
                return path1 + Separator + path2;
            }
        }

        public static string GetPathParent(string path)
        {
            return Directory.GetParent(path).ToString().Replace("\\", Separator);
        }

        public static string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path).Replace("\\", Separator);
        }
    }
}