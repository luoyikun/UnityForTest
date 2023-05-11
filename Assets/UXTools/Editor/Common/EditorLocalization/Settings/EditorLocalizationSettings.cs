using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    //增加支持的语言
    public enum EditorLocalName
    {
        Chinese,
        TraditionalChinese,
        English,
        Japanese,
        Korean,
    }
    //关于编辑器本地化的配置
    [Serializable]
    public class EditorLocalizationSettings : ScriptableObject
    {
        //[OnValueChanged("OnCurrentLocalValueChanged")]
        //当前的本地化
        //public EditorLocalName LocalType = EditorLocalName.Chinese;
        public EditorLocalName LocalType;

        public void ChangeLocalValue(EditorLocalName Type)
        {
            if (Type != LocalType)
            {
                LocalType = Type;
                EditorLocalization.refreshDict();
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }

    public class CreateAssetFile : Editor
    {
        //先临时放这里，后续根据设计在挪地方
        [MenuItem("ThunderFireUXTool/新建配置文件 (Create Assets)/EditorLocalizationSettings")]
        public static void Create()
        {
            var settings = ScriptableObject.CreateInstance<EditorLocalizationSettings>();
            if (settings == null)
                Debug.LogError("Create LocalizationSettings Failed!");

            if (!Directory.Exists(EditorLocalizationConfig.LocalizationSettingsPath))
                Directory.CreateDirectory(EditorLocalizationConfig.LocalizationSettingsPath);

            var assetPath = EditorLocalizationConfig.LocalizationSettingsFullPath;
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
