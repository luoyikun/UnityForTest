using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace xasset
{
    public static class Utility
    {
        public const string buildPath = "Bundles";

        public const string serverDataPath = "DLC";

        public const string nonsupport = "Nonsupport";

        private static readonly double[] byteUnits =
        {
            1073741824.0, 1048576.0, 1024.0, 1
        };

        private static readonly string[] byteUnitsNames =
        {
            "GB", "MB", "KB", "B"
        };

        public static string GetPlatformName()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                default:
                    return nonsupport;
            }
        }

        public static string FormatBytes(long bytes)
        {
            var size = "0 B";
            if (bytes == 0)
            {
                return size;
            }

            for (var index = 0; index < byteUnits.Length; index++)
            {
                var unit = byteUnits[index];
                if (!(bytes >= unit))
                {
                    continue;
                }

                size = $"{bytes / unit:##.##} {byteUnitsNames[index]}";
                break;
            }

            return size;
        }

        public static string ToHash(IEnumerable<byte> data)
        {
            var sb = new StringBuilder();
            foreach (var t in data)
            {
                sb.Append(t.ToString("x2"));
            }

            return sb.ToString();
        }

        public static string ComputeHash(byte[] bytes)
        {
            var data = MD5.Create().ComputeHash(bytes);
            return ToHash(data);
        }

        public static string ComputeHash(string filename)
        {
            if (!File.Exists(filename))
            {
                return string.Empty;
            }

            using (var stream = File.OpenRead(filename))
            {
                return ToHash(MD5.Create().ComputeHash(stream));
            }
        }

        public static void CreateDirectoryIfNecessary(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(dir) || Directory.Exists(dir))
            {
                return;
            }

            Directory.CreateDirectory(dir);
        }

        public static T LoadScriptableObjectWithJson<T>(string filename) where T : ScriptableObject
        {
            if (!File.Exists(filename))
            {
                return ScriptableObject.CreateInstance<T>();
            }

            var json = File.ReadAllText(filename);
            var asset = ScriptableObject.CreateInstance<T>();
            try
            {
                JsonUtility.FromJsonOverwrite(json, asset);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                File.Delete(filename);
            }

            return asset;
        }

        public static string ComputeHash(Stream stream)
        {
            var buffer = new byte[32768]; // 32 kb
            var amount = (int)(stream.Length - stream.Position);
            using (var hashAlgorithm = MD5.Create())
            {
                while (amount > 0)
                {
                    var bytesRead = stream.Read(buffer, 0, Math.Min(buffer.Length, amount));
                    if (bytesRead <= 0) continue;
                    amount -= bytesRead;
                    if (amount > 0)
                    {
                        hashAlgorithm.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                    }
                    else
                    {
                        hashAlgorithm.TransformFinalBlock(buffer, 0, bytesRead);
                    }
                }

                return ToHash(hashAlgorithm.Hash);
            }
        }
    }
}