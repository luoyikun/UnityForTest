#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace ThunderFireUITool
{
    [Serializable]
    public class PrefabDefaultSetting : ScriptableObject
    {
        // public string prefabPath;  //项目中的Prefab存放路径
        public WidgetInstantiateMode widgetInsMode; //放置组件时的默认模式

        [MenuItem("ThunderFireUXTool/新建配置文件 (Create Assets)/PrefabPathSettings")]
        public static void Create()
        {
            var settings = CreateInstance<PrefabDefaultSetting>();
            if (settings == null)
            {
                Debug.LogError("Create PrefabPathSetting Failed!");
                return;
            }

            // settings.prefabPath = ThunderFireUIToolConfig.DefaultPrefabPath;
            settings.widgetInsMode = WidgetInstantiateMode.None;

            if (!Directory.Exists(ThunderFireUIToolConfig.PrefabSettingsPath))
                Directory.CreateDirectory(ThunderFireUIToolConfig.PrefabSettingsPath);

            var assetPath = ThunderFireUIToolConfig.PrefabDefaultSettingPath;
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void ChangeWidgetInsMode(WidgetInstantiateMode mode)
        {
            widgetInsMode = mode;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }


    public class CreatePrefabPathSettings : Editor
    {

    }
}
#endif




