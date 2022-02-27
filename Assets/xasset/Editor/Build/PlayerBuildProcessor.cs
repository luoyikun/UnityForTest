using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace xasset.editor
{
    /// <summary>
    ///     打包安装包的后处理，打包前自动根据分包配置拷贝资源到安装包资源目录，打包后，把拷贝的资源删除，避免漫长的资源导入等待过程。
    ///     打包后，避免在自己的后处理中调用 AssetDatabase.Refresh，否则可能还会触发资源导入，这是 Unity 的设计缺陷。
    /// </summary>
    public class PlayerBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public void OnPostprocessBuild(BuildReport report)
        {
            var directory = Settings.BuildPlayerDataPath;
            if (!Directory.Exists(directory))
            {
                return;
            }

            Directory.Delete(directory, true);
            if (Directory.GetFiles(Application.streamingAssetsPath).Length == 0)
            {
                Directory.Delete(Application.streamingAssetsPath);
            }
        }

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            BuildScript.CopyToStreamingAssets();
        }
    }
}