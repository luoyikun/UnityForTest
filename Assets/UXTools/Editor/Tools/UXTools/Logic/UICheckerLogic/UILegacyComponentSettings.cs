#if UNITY_EDITOR && ODIN_INSPECTOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    public class UILegacyComponentSettings : ScriptableObject
    {
        [MenuItem("ThunderFireUXTool/新建配置文件 (Create Assets)/UILegacyComponentSettings", false, 50)]
        public static UILegacyComponentSettings Create()
        {
            var settings = CreateInstance<UILegacyComponentSettings>();
            if (settings == null)
                Debug.LogError("Create UIAtlasCheckRuleSettings Failed!");

            var folderPath = Application.dataPath + ThunderFireUIToolConfig.SettingsPath;
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var assetPath = ThunderFireUIToolConfig.UICheckLegacyComponentFullPath;

            settings.LegacyComponents = new List<MonoScript>();
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return settings;
        }
        public List<MonoScript> LegacyComponents;
    }
}
#endif