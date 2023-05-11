#if UNITY_EDITOR && ODIN_INSPECTOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    public class UIAtlasCheckUserData : ScriptableObject
    {
        [MenuItem("ThunderFireUXTool/新建配置文件 (Create Assets)/UIAtlasCheckUserData", false, 50)]
        public static UIAtlasCheckUserData Create()
        {
            var settings = CreateInstance<UIAtlasCheckUserData>();
            if (settings == null)
                Debug.LogError("Create UIAtlasCheckUserData Failed!");

            var folderPath = Application.dataPath + ThunderFireUIToolConfig.SettingsPath;
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var assetPath = ThunderFireUIToolConfig.UICheckUserDataFullPath;
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return settings;
        }

        public bool UICheckEnable = false;

        public void Save(bool enable)
        {
            UICheckEnable = enable;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif