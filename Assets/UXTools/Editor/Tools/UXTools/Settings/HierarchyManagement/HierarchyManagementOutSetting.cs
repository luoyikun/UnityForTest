using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    public class HierarchyManagementOutSetting : ScriptableObject
    {
        public List<GuidWithIndexOut> guidList = new List<GuidWithIndexOut>();

        [MenuItem("ThunderFireUXTool/新建配置文件 (Create Assets)/HierarchyManagementOutSetting")]
        public static void Create()
        {
            SettingAssetsUtils.CreateAssets<HierarchyManagementOutSetting>(
                ThunderFireUIToolConfig.HierarchyManagementOutSettingPath);
        }

        public void AutoSave()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public delegate int GetIndex(string guid);
        public void CreateGuidList(List<string> guids, GetIndex getIndex)
        {
            var listOut = new List<GuidWithIndexOut>();
            var listForLevel = new List<int>();
            foreach (var item in guids)
            {
                var num = getIndex(item);
                var path = Path.GetFileNameWithoutExtension(
                    AssetDatabase.GUIDToAssetPath(item));
                var prefabName = string.IsNullOrEmpty(path) ? item : path;
                listOut.Add(new GuidWithIndexOut()
                {
                    Guid = item,
                    Index = num,
                    Name = prefabName,
                });
                if (!listForLevel.Contains(num)) listForLevel.Add(num);
            }
            guidList = listOut;
            var hierarchyManagementSetting =
                AssetDatabase.LoadAssetAtPath<HierarchyManagementSetting>(
                    ThunderFireUIToolConfig.HierarchyManagementSettingPath);
            var ls = hierarchyManagementSetting.levelList;
            ls.Clear();
            foreach (var item in guidList)
            {
                if (!ls.Contains(item.Index))
                {
                    ls.Add(item.Index);
                }
            }
            AutoSave();
            hierarchyManagementSetting.AutoSave();
        }
    }

    [Serializable]
    public class GuidWithIndexOut
    {
        public string Name;
        public int Index;
        public string Guid;
    }
}