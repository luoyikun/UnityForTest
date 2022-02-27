using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace xasset.editor
{
    [CreateAssetMenu(menuName = "xasset/Settings", fileName = "Settings", order = 0)]
    public sealed class Settings : ScriptableObject
    {
        /// <summary>
        ///     采集资源或依赖需要过滤掉的文件
        /// </summary>
        [Header("Bundle")] [Tooltip("采集资源或依赖需要过滤掉的文件")]
        public List<string> excludeFiles =
            new List<string>
            {
                ".spriteatlas",
                ".giparams",
                "LightingData.asset"
            };

        /// <summary>
        ///     bundle 的扩展名，例如：.bundle, .unity3d, .ab, 团队版不用给 bundle 加 扩展名。
        /// </summary>
        [Tooltip("bundle 的扩展名，例如：.bundle, .unity3d, .ab, 团队版不用给 bundle 加 扩展名。")]
        public string bundleExtension;

        /// <summary>
        ///     播放器的运行模式。Preload 模式不更新资源，并且打包的时候会忽略分包配置。
        /// </summary>
        [Tooltip("播放器的运行模式")] public ScriptPlayMode scriptPlayMode = ScriptPlayMode.Simulation;

        public bool requestCopy;

        public static string BundleExtension { get; private set; }
        public static List<string> ExcludeFiles { get; private set; }

        /// <summary>
        ///     打包输出目录
        /// </summary>
        public static string PlatformBuildPath
        {
            get
            {
                var dir = $"{Utility.buildPath}/{GetPlatformName()}";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                return dir;
            }
        }

        /// <summary>
        ///     安装包资源目录, 打包安装包的时候会自动根据分包配置将资源拷贝到这个目录
        /// </summary>
        public static string BuildPlayerDataPath => $"{Application.streamingAssetsPath}/{Utility.buildPath}";

        public void Initialize()
        {
            BundleExtension = bundleExtension;
            ExcludeFiles = excludeFiles;
        }

        public static Settings GetDefaultSettings()
        {
            return EditorUtility.FindOrCreateAsset<Settings>("Assets/xasset/Settings.asset");
        }

        /// <summary>
        ///     获取包含在安装包的资源
        /// </summary>
        /// <returns></returns>
        public List<ManifestBundle> GetBundlesInBuild(BuildVersions versions)
        {
            var bundles = new List<ManifestBundle>();
            foreach (var version in versions.data)
            {
                var manifest = Manifest.LoadFromFile(GetBuildPath(version.file));
                bundles.AddRange(manifest.bundles);
            }

            return bundles;
        }

        public static string GetBuildPath(string file)
        {
            return $"{PlatformBuildPath}/{file}";
        }

        public static string GetPlatformName()
        {
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.StandaloneOSX:
                    return "OSX";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                default:
                    return Utility.nonsupport;
            }
        }

        public static bool IsExcluded(string path)
        {
            return ExcludeFiles.Exists(path.EndsWith) || path.EndsWith(".cs") || path.EndsWith(".dll");
        }

        public static IEnumerable<string> GetDependencies(string path)
        {
            var set = new HashSet<string>(AssetDatabase.GetDependencies(path, true));
            set.Remove(path);
            set.RemoveWhere(IsExcluded);
            return set.ToArray();
        }
    }
}