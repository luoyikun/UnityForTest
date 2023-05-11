#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    //设为组件的Prefab列表
    [Serializable]
    public class PrefabSettledSetting : ScriptableObject
    {
        // TODO: List可以初始化
        public List<string> List = new List<string>();

        [MenuItem("ThunderFireUXTool/新建配置文件 (Create Assets)/PrefabSettledSettings")]
        public static void Create()
        {
            var setting = SettingAssetsUtils.CreateAssets<PrefabSettledSetting>(ThunderFireUIToolConfig.PrefabSettledPath);
            var guids = AssetDatabase.FindAssets("t:Prefab", new string[] { ThunderFireUIToolConfig.AssetsRootPath + "UX-GUI-PresetWidget/UXToolPrefabs/" });
            foreach (var guid in guids)
            {
                if (!setting.List.Contains(guid))
                    setting.List.Add(guid);
            }
        }

        public void Add(string newLabel)
        {
            List.Add(newLabel);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            OnValueChanged();
        }

        public void Remove(string label)
        {
            var index = List.FindIndex(i => i == label); // like Where/Single
            if (index >= 0)
            {   // ensure item found
                List.RemoveAt(index);
            }
            //List.Remove(label);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            OnValueChanged();
        }

        public void ResortLast(string label)
        {
            var index = List.FindIndex(i => i == label);
            if (index >= 0)
            {   // ensure item found
                List.RemoveAt(index);
            }
            List.Add(label);

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