using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class HierarchyManagementSetting : ScriptableObject
    {
        public List<ManagementChannel> managementChannelList = new List<ManagementChannel>();
        public List<ManagementLevel> managementLevelList = new List<ManagementLevel>();
        public List<PrefabDetail> prefabDetailList = new List<PrefabDetail>();
        public List<TagColor> tagColors = new List<TagColor>();
        public List<GuidWithIndex> guidList = new List<GuidWithIndex>();
        public List<int> levelList = new List<int>();
        public List<string> channelNameList = new List<string>() { "HUD", "界面", "弹窗" };
        public int maxPrefabNum = 0;
        public int range = 10;
        public int maxChannelCount = 3;
        public Action<List<GuidWithIndex>> AfterSubmit = l => { };

        public static bool isDemo = false;

        [MenuItem("ThunderFireUXTool/新建配置文件 (Create Assets)/HierarchyManagementSetting")]
        public static void Create()
        {
            SettingAssetsUtils.CreateAssets<HierarchyManagementSetting>(
                ThunderFireUIToolConfig.HierarchyManagementSettingPath);
        }

        public void CovertToSetting()
        {
            managementChannelList.Clear();
            managementLevelList.Clear();
            prefabDetailList.Clear();
            maxPrefabNum = 0;
            var hierarchyManagementOutSetting = SettingAssetsUtils.GetAssets<HierarchyManagementOutSetting>();
            if (isDemo)
                hierarchyManagementOutSetting = AssetDatabase.LoadAssetAtPath<HierarchyManagementOutSetting>(
                    ThunderFireUIToolConfig.AssetsRootPath + "Samples/HierarchyManage/HierarchyManagementOutSettingDemo.asset");
            var ls = new List<GuidWithIndex>();
            foreach (var item in hierarchyManagementOutSetting.guidList)
            {
                if (!File.Exists(AssetDatabase.GUIDToAssetPath(item.Guid))) continue;
                var it = guidList.Find(s => s.Guid == item.Guid);
                ls.Add(new GuidWithIndex()
                {
                    Guid = item.Guid,
                    Name = item.Name,
                    Index = item.Index,
                    Tags = it == null ? new List<TagDetail>() : it.Tags,
                });
                if (levelList.FindAll(s => s == item.Index).Count == 0)
                {
                    levelList.Add(item.Index);
                }
            }

            if (ls.Count != hierarchyManagementOutSetting.guidList.Count)
                EditorUtility.DisplayDialog("messageBox", "导入的内容中，有不存在于项目中的Prefab，请在asset文件中检查",
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
            guidList = ls;

            guidList.Sort((x, y) =>
            {
                return x.Index == y.Index
                    ? guidList.IndexOf(x).CompareTo(guidList.IndexOf(y))
                    : x.Index.CompareTo(y.Index);
            });
            levelList.Sort();

            for (int i = 0; i < maxChannelCount; i++)
            {
                if (channelNameList.Count <= i)
                {
                    channelNameList.Add("Channel " + i);
                }
            }

            for (int i = 0; i < maxChannelCount; i++)
            {
                managementChannelList.Add(new ManagementChannel() { ID = i, Name = channelNameList[i] });
            }

            foreach (var item in levelList)
            {
                var channelID = item / range;
                var levelIndex = item;

                var level = new ManagementLevel()
                { ChannelID = channelID, Index = levelIndex, ID = managementLevelList.Count };
                managementChannelList[channelID].LevelIDList.Add(level.ID);
                managementLevelList.Add(level);

            }

            foreach (var item in managementChannelList)
            {
                if (item.LevelIDList.Count == 0)
                {
                    var num = item.ID * range;
                    var ind = managementLevelList.FindLastIndex(s => s.Index < num);

                    foreach (var it in managementChannelList)
                    {
                        for (var i = 0; i < it.LevelIDList.Count; i++)
                        {
                            var t = it.LevelIDList[i];
                            if (managementLevelList[t].Index > num) it.LevelIDList[i] += 1;
                        }
                    }

                    foreach (var it in managementLevelList)
                    {
                        if (it.Index > num) it.ID += 1;
                    }

                    var level = new ManagementLevel()
                    {
                        ID = ind + 1,
                        Index = num,
                        ChannelID = item.ID,
                    };
                    managementLevelList.Insert(ind + 1, level);
                    item.LevelIDList.Add(level.ID);
                    levelList.Add(num);
                }
                levelList.Sort();
            }

            foreach (var item in guidList)
            {
                var channelID = item.Index / range;
                var level = managementLevelList.Find(s => s.Index == item.Index);
                var tags = new List<TagDetail>();
                foreach (var tag in item.Tags)
                {
                    var tagDetail = TagDetail.DeepCopyByXml(tag);
                    tags.Add(tagDetail);
                }

                var prefabDetail = new PrefabDetail()
                {
                    ID = prefabDetailList.Count,
                    ChannelID = channelID,
                    LevelID = level.ID,
                    Guid = item.Guid,
                    Tags = tags
                };
                prefabDetailList.Add(prefabDetail);
                level.PrefabDetailIDList.Add(prefabDetail.ID);
            }

            foreach (var item in managementLevelList)
            {
                var len = item.PrefabDetailIDList.Count;
                if (len > maxPrefabNum) maxPrefabNum = len;
            }
        }

        public void CovertToList()
        {
            guidList.Clear();
            var hierarchyManagementOutSetting = SettingAssetsUtils.GetAssets<HierarchyManagementOutSetting>();
            if (isDemo)
                hierarchyManagementOutSetting = AssetDatabase.LoadAssetAtPath<HierarchyManagementOutSetting>(
                    ThunderFireUIToolConfig.AssetsRootPath + "Samples/HierarchyManage/HierarchyManagementOutSettingDemo.asset");
            hierarchyManagementOutSetting.guidList.Clear();
            foreach (var item in prefabDetailList)
            {
                var tags = new List<TagDetail>();
                foreach (var tag in item.Tags)
                {
                    var tagDetail = TagDetail.DeepCopyByXml(tag);
                    tags.Add(tagDetail);
                }
                var path = Path.GetFileNameWithoutExtension(
                    AssetDatabase.GUIDToAssetPath(item.Guid));
                var prefabName = string.IsNullOrEmpty(path) ? item.Guid : path;
                var guidWithIndex = new GuidWithIndex()
                {
                    Guid = item.Guid,
                    Index = managementLevelList[item.LevelID].Index,
                    Tags = tags,
                    Name = prefabName,
                };
                var guidWithIndexOut = new GuidWithIndexOut()
                {
                    Guid = item.Guid,
                    Index = managementLevelList[item.LevelID].Index,
                    Name = prefabName,
                };
                guidList.Add(guidWithIndex);
                hierarchyManagementOutSetting.guidList.Add(guidWithIndexOut);
            }
            levelList.Clear();
            foreach (var item in managementLevelList)
            {
                levelList.Add(item.Index);
            }

            guidList.Sort((x, y) =>
            {
                return x.Index == y.Index
                    ? guidList.IndexOf(x).CompareTo(guidList.IndexOf(y))
                    : x.Index.CompareTo(y.Index);
            });
            levelList.Sort();

            channelNameList.Clear();
            foreach (var item in managementChannelList)
            {
                channelNameList.Add(item.Name);
            }

            AfterSubmit(guidList);
            hierarchyManagementOutSetting.AutoSave();
            AutoSave();
        }

        public void SetInitChannelAndLevel(int levelRange, int maxChannelNum)
        {
            range = levelRange;
            maxChannelCount = maxChannelNum;
            AutoSave();
        }

        public void SetAfterSubmit(Action<List<GuidWithIndex>> action)
        {
            AfterSubmit = action;
            AutoSave();
        }

        public void AutoSave()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    [Serializable]
    public class ManagementChannel
    {
        public int ID;
        public string Name;
        public List<int> LevelIDList = new List<int>();
    }

    [Serializable]
    public class ManagementLevel
    {
        public int ID;
        public int Index;
        public int ChannelID;
        public List<int> PrefabDetailIDList = new List<int>();

    }

    [Serializable]
    public class PrefabDetail
    {
        public int ID;
        public string Guid;
        public int ChannelID;
        public int LevelID;
        public List<TagDetail> Tags = new List<TagDetail>();

        public static PrefabDetail DeepCopyByXml(PrefabDetail obj)
        {
            object retval;
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (PrefabDetail)retval;
        }

    }

    [Serializable]
    public class TagDetail
    {
        public string Name;
        public int Num;

        public override bool Equals(object obj)
        {
            var atom = obj as TagDetail;
            return atom != null && Name == atom.Name;
        }

        public override int GetHashCode()
        {
            return Num;
        }

        public static TagDetail DeepCopyByXml(TagDetail obj)
        {
            object retval;
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (TagDetail)retval;
        }
    }

    [Serializable]
    public class TagColor
    {
        public string Name;
        public Color Color;
    }

    [Serializable]
    public class GuidWithIndex
    {
        public string Name;
        public int Index;
        public string Guid;
        public List<TagDetail> Tags = new List<TagDetail>();
    }
}