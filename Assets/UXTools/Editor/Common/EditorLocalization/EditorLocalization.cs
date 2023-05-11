using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ThunderFireUITool
{
    public class EditorLocalization
    {
        public static Dictionary<string, string> List = new Dictionary<string, string>();
        public static string GetLocalization(long key)
        {
            var currentLocal = AssetDatabase.LoadAssetAtPath<EditorLocalizationSettings>(EditorLocalizationConfig
                .LocalizationSettingsFullPath).LocalType;

            var localDataPath = EditorLocalizationConfig
                .LocalizationData + currentLocal.ToString() + EditorLocalizationConfig.Assetsuffix;

            var strList = AssetDatabase.LoadAssetAtPath<EditorLocalizationData>(localDataPath);
            return strList.GetValue(key);
        }

        public static string GetLocalization(string type, string fieldName)
        {
            var currentLocal = AssetDatabase.LoadAssetAtPath<EditorLocalizationSettings>(EditorLocalizationConfig
                 .LocalizationSettingsFullPath).LocalType;
            var localDataPath = EditorLocalizationConfig
                .LocalizationUIInspectorData + currentLocal.ToString() + EditorLocalizationConfig.Assetsuffix;
            var strList = AssetDatabase.LoadAssetAtPath<EditorLocalizationUIInspectorData>(localDataPath);
            return strList.GetValue(type, fieldName);
        }


        public static void refreshDict()
        {
            var currentLocal = AssetDatabase.LoadAssetAtPath<EditorLocalizationSettings>(EditorLocalizationConfig
                 .LocalizationSettingsFullPath).LocalType;
            var localDataPath = EditorLocalizationConfig
                .LocalizationUIInspectorData + currentLocal.ToString() + EditorLocalizationConfig.Assetsuffix;
            var strList = AssetDatabase.LoadAssetAtPath<EditorLocalizationUIInspectorData>(localDataPath);
            strList.refreshDict();
        }
    }
}
