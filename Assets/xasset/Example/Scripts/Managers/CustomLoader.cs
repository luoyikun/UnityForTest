using System;
using System.IO;
using UnityEngine;

namespace xasset.example
{
    public enum LoadMode
    {
        LoadByName,
        LoadByNameWithoutExtension,
        LoadByRelativePath
    }

    [DisallowMultipleComponent]
    public class CustomLoader : MonoBehaviour
    {
        [Tooltip("通过关键字进行路径匹配，为路径生成短链接，可以按需使用")]
        public string[] filters =
        {
            "Scenes", "Prefabs", "Textures"
        };

        [Tooltip("加载模式")] public LoadMode loadMode = LoadMode.LoadByRelativePath;


        private string LoadByNameWithoutExtension(string assetPath)
        {
            if (filters == null || filters.Length == 0)
            {
                return null;
            }

            if (!Array.Exists(filters, assetPath.Contains))
            {
                return null;
            }

            var assetName = Path.GetFileNameWithoutExtension(assetPath);
            return assetName;
        }

        private string LoadByName(string assetPath)
        {
            if (filters == null || filters.Length == 0)
            {
                return null;
            }

            if (!Array.Exists(filters, assetPath.Contains))
            {
                return null;
            }

            var assetName = Path.GetFileName(assetPath);
            return assetName;
        }

        public void Initialize()
        {
            switch (loadMode)
            {
                case LoadMode.LoadByName:
                    Manifest.customLoader += LoadByName;
                    break;
                case LoadMode.LoadByNameWithoutExtension:
                    Manifest.customLoader += LoadByNameWithoutExtension;
                    break;
                default:
                    Manifest.customLoader = null;
                    break;
            }
        }
    }
}