#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    //组件标签列表
    [Serializable]
    public class PrefabLabelsSettings : ScriptableObject
    {
        // TODO: label可以初始化
        public List<string> labelList = new List<string>();

        [MenuItem("ThunderFireUXTool/新建配置文件 (Create Assets)/PrefabLabelsSetting")]
        public static void Create()
        {
            SettingAssetsUtils.CreateAssets<PrefabLabelsSettings>(ThunderFireUIToolConfig.PrefabLabelsPath);
        }

        public void AddNewLabel(string newLabel)
        {
            labelList.Add(newLabel);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            OnValueChanged();
        }

        public void RemoveLabel(string label)
        {
            labelList.Remove(label);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            OnValueChanged();
        }

        private void OnValueChanged()
        {
            if (WidgetRepositoryWindow.GetInstance() != null)
            {
                WidgetRepositoryWindow.GetInstance().RefreshWindow();
            }
        }
    }
}
#endif