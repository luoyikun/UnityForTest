using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ThunderFireUITool
{
    public class PrefabTabsData : ScriptableObject
    {
        [SerializeField]
        private List<string> m_tabs;

        public static List<string> Tabs
        {
            get
            {
                var instance = AssetDatabase.LoadAssetAtPath<PrefabTabsData>(ThunderFireUIToolConfig.PrefabTabsPath);
                if(instance == null)
                {
                    return new List<string>();
                }
                return instance.m_tabs ?? new List<string>();
            }
        }

        public static void SyncTab(List<string> list)
        {
            var instance = AssetDatabase.LoadAssetAtPath<PrefabTabsData>(ThunderFireUIToolConfig.PrefabTabsPath);
            if(instance == null)
            {
                instance = ScriptableObject.CreateInstance<PrefabTabsData>();
                AssetDatabase.CreateAsset(instance, ThunderFireUIToolConfig.PrefabTabsPath);
            }
            if(instance.m_tabs == null)
            {
                instance.m_tabs = new List<string>();
            }
            instance.m_tabs.Clear();
            foreach(string s in list)
            {
                instance.m_tabs.Add(s);
            }
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
