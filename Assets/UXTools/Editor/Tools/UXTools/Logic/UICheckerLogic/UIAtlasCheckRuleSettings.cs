#if UNITY_EDITOR && ODIN_INSPECTOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    public class UIAtlasCheckRuleSettings : ScriptableObject
    {
        [MenuItem("ThunderFireUXTool/新建配置文件 (Create Assets)/UIAtlasCheckRuleSettings", false, 50)]
        public static UIAtlasCheckRuleSettings Create()
        {
            var settings = CreateInstance<UIAtlasCheckRuleSettings>();
            if (settings == null)
                Debug.LogError("Create UIAtlasCheckRuleSettings Failed!");

            var folderPath = Application.dataPath + ThunderFireUIToolConfig.SettingsPath;
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var assetPath = ThunderFireUIToolConfig.UICheckSettingFullPath;
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return settings;
        }

        public string imageFolderPath = "Assets/Res/GUI/art_source/";
        public string atlasFolderPath = "Assets/Res/GUI/Atlas";
        public string animFolderPath = "Assets/Res/GUI/Animation";
        public int atlasLimit = 4;
        public int imageLimit = 13;

        public void Save(int newAtlasLimit, int newImageLimit, string newImagePath, string newAtlasPath)
        {
            atlasLimit = newAtlasLimit;
            imageLimit = newImageLimit;
            imageFolderPath = newImagePath;
            atlasFolderPath = newAtlasPath;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif