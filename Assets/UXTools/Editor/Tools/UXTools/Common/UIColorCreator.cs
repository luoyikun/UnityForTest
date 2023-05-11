#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace ThunderFireUITool
{
    static public class UIColorCreator
    {
        [MenuItem("ThunderFireUXTool/新建配置文件 (Create Assets)/UI Color Assets")]
        public static void CreateColor()
        {
            var config = ScriptableObject.CreateInstance<UIColorAsset>();
            config.Save();
            if (!Directory.Exists(UIColorConfig.ColorConfigPath))
            {
                Directory.CreateDirectory(UIColorConfig.ColorConfigPath);
            }

            var assetPath = UIColorConfig.ColorConfigPath + UIColorConfig.ColorConfigName + ".asset";
            if (File.Exists(assetPath))
            {
                File.Delete(assetPath);
            }

            AssetDatabase.CreateAsset(config, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("ThunderFireUXTool/新建配置文件 (Create Assets)/UI Gradient Assets")]
        public static void CreateGradient()
        {
            var config = ScriptableObject.CreateInstance<UIGradientAsset>();
            config.Save();
            if (!Directory.Exists(UIColorConfig.ColorConfigPath))
            {
                Directory.CreateDirectory(UIColorConfig.ColorConfigPath);
            }

            var assetPath = UIColorConfig.ColorConfigPath + UIColorConfig.GradientConfigName + ".asset";
            if (File.Exists(assetPath))
            {
                File.Delete(assetPath);
            }

            AssetDatabase.CreateAsset(config, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif