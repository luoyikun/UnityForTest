using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    public class HierarchyManagementEvent
    {
        private static HierarchyManagementWindow _managementWindow;
        private static List<ManagementChannel> _managementChannels = new List<ManagementChannel>();
        private static List<ManagementLevel> _managementLevels = new List<ManagementLevel>();
        private static List<PrefabDetail> _prefabDetails = new List<PrefabDetail>();
        private static int _range;

        public static void Init()
        {
            _managementWindow = HierarchyManagementWindow.GetInstance();
            var hierarchyManagementSetting = SettingAssetsUtils.GetAssets<HierarchyManagementSetting>();
            if (HierarchyManagementSetting.isDemo)
                hierarchyManagementSetting = AssetDatabase.LoadAssetAtPath<HierarchyManagementSetting>(
                    ThunderFireUIToolConfig.AssetsRootPath + "Samples/HierarchyManage/HierarchyManagementSettingDemo.asset");
            _managementChannels = hierarchyManagementSetting.managementChannelList;
            _managementLevels = hierarchyManagementSetting.managementLevelList;
            _prefabDetails = hierarchyManagementSetting.prefabDetailList;
            _range = hierarchyManagementSetting.range;
        }
        
        #region Level
        public static void AddLevel(ManagementLevel level, int definedIndex, bool isAfter = false)
        {
            var newLevel = new ManagementLevel()
            {
                ID = !isAfter
                    ? level.ID
                    : level.ID + 1,
                Index = definedIndex,
                ChannelID = definedIndex / _range,
            };

            ManagementChannel channel = _managementChannels[newLevel.ChannelID];
            if (channel.LevelIDList.Count == _range)
            {
                EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_已经达到该层级最大容纳量Tip),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                return;
            }
            foreach (var item in _managementLevels)
            {
                if (item.ID >= newLevel.ID) item.ID += 1;
            }

            foreach (var item in _managementChannels)
            {
                var list = item.LevelIDList;
                for(int i = 0; i < list.Count; i++)
                {
                    if (list[i] >= newLevel.ID)  list[i] += 1;
                }
                // item.LevelIDList = list;
            }
            foreach (var item in _prefabDetails)
            {
                if (item.LevelID >= newLevel.ID) item.LevelID += 1;
            }
            if (channel.LevelIDList.Count - channel.LevelIDList.IndexOf(level.ID) ==
                (channel.ID + 1) * _range - level.Index)
            {
                if(isAfter && definedIndex != level.Index || !isAfter && definedIndex == level.Index) newLevel.Index -= 1;
                var tmp = newLevel.Index;
                for (var i = _managementLevels.Count - 1; i >= 0; i--)
                {
                    var item = _managementLevels[i];
                    if (item.Index != tmp) continue;
                    item.Index -= 1;
                    tmp -= 1;
                }
            }
            else
            {
                var tmp = newLevel.Index;
                foreach (var item in _managementLevels)
                {
                    if (item.Index != tmp) continue;
                    item.Index += 1;
                    tmp += 1;
                }
            }

            _managementLevels.Insert(newLevel.ID, newLevel);
            var ind = !isAfter
                ? channel.LevelIDList.IndexOf(level.ID)
                : channel.LevelIDList.IndexOf(level.ID) + 1;
            channel.LevelIDList.Insert(ind, newLevel.ID);
            _managementWindow.RefreshPaint();
        }
        
        public static void DeleteLevel(ManagementLevel level)
        {
            ManagementChannel channel = _managementChannels[level.ChannelID];
            if (level.PrefabDetailIDList.Count != 0)
            {
                EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_只能删除不含组件信息的层级),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定));
                return;
            }

            if (channel.LevelIDList.Count <= 1)
            {
                EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_至少保留一个层级),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定));
                return;
            }
            _managementLevels.Remove(level);
            channel.LevelIDList.Remove(level.ID);
            foreach (var item in _managementChannels)
            {
                var list = item.LevelIDList;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] > level.ID) list[i] -= 1;
                }
            }
            foreach (var item in _managementLevels)
            {
                if (item.ID > level.ID)
                {
                    item.ID -= 1;
                }
            }
            foreach (var item in _prefabDetails)
            {
                if (item.LevelID > level.ID) item.LevelID -= 1;
            }
            _managementWindow.RefreshPaint();
        }
        
        #endregion
        
        #region Prefab
        public static void AddNewPrefab(ManagementLevel level = null)
        {
            if(level != null)
                PrefabDetailWindow.OpenWindow(level);
        }
        
        public static void OpenPrefabDetail(PrefabDetail prefabDetail)
        {
            PrefabDetailWindow.OpenWindow(prefabDetail);
        }

        public static void DeletePrefab(PrefabDetail prefabDetail)
        {
            ManagementChannel channel = _managementChannels[prefabDetail.ChannelID];
            ManagementLevel level = _managementLevels[prefabDetail.LevelID];
            var path = Path.GetFileNameWithoutExtension(
                AssetDatabase.GUIDToAssetPath(prefabDetail.Guid));
            var tooltipText = string.IsNullOrEmpty(path)
                ? prefabDetail.Guid
                : Path.GetFileName(AssetDatabase.GUIDToAssetPath(prefabDetail.Guid));
            if (EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确认删除该节点) + "："+tooltipText,
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消)))
            {
                level.PrefabDetailIDList.Remove(prefabDetail.ID);
                foreach (var item in _managementLevels)
                {
                    var list = item.PrefabDetailIDList;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] > prefabDetail.ID) list[i] -= 1;
                    }
                }

                var prefab = _prefabDetails.Find(s => s.Guid == prefabDetail.Guid);
                _prefabDetails.Remove(prefab);
                foreach (var item in _prefabDetails)
                {
                    if (item.ID > prefabDetail.ID) item.ID -= 1;
                }

                _managementWindow.chosenPrefab = new PrefabDetail() { ID = -1 };
                DeleteTags(prefabDetail);
                HierarchyManagementWindow.maxPrefabNum = 0;
                foreach (var item in _managementLevels)
                {
                    if (item.PrefabDetailIDList.Count > HierarchyManagementWindow.maxPrefabNum)
                        HierarchyManagementWindow.maxPrefabNum = item.PrefabDetailIDList.Count;
                }
                var hierarchyManagementSetting = SettingAssetsUtils.GetAssets<HierarchyManagementSetting>();
                if (HierarchyManagementSetting.isDemo)
                    hierarchyManagementSetting = AssetDatabase.LoadAssetAtPath<HierarchyManagementSetting>(
                        ThunderFireUIToolConfig.AssetsRootPath + "Samples/HierarchyManage/HierarchyManagementSettingDemo.asset");
                hierarchyManagementSetting.maxPrefabNum = HierarchyManagementWindow.maxPrefabNum;
                _managementWindow.DrawAllPrefabs();
            }
            else
            {
                _managementWindow._firstFlag = true;
                _managementWindow.chosenPrefab = prefabDetail;
                _managementWindow.DrawAllPrefabs();
            }
        }
        
        
        public static void DragSubmit(PrefabDetail prefabDetail, ManagementLevel newLevel)
        {
            _managementLevels[prefabDetail.LevelID].PrefabDetailIDList.Remove(prefabDetail.ID);
            newLevel.PrefabDetailIDList.Insert(0, prefabDetail.ID);
            prefabDetail.LevelID = newLevel.ID;
            prefabDetail.ChannelID = newLevel.ChannelID;
            HierarchyManagementWindow.GetInstance().chosenLevel = new ManagementLevel() { ID = -1 };
            
            var maxNum = 0;
            foreach (var item in _managementLevels)
            {
                if (item.PrefabDetailIDList.Count > maxNum)
                    maxNum = item.PrefabDetailIDList.Count;
            }
            HierarchyManagementWindow.SetMaxPrefabNum(maxNum);
            HierarchyManagementWindow.GetInstance().DrawAllPrefabs();
        }
        
        #endregion

        #region Tag
        private static void DeleteTags(PrefabDetail prefab)
        {
            var prefabs = new List<PrefabDetail>();
            foreach (var item in _prefabDetails)
            {
                prefabs.Add(PrefabDetail.DeepCopyByXml(item));
            }

            foreach (var tag in prefab.Tags)
            {
                //判断是否要进行tag的减
                var flag = prefabs.FindAll(s => s.Tags.Contains(tag) && s.ID != prefab.ID)
                    .Any(item => item.Tags.Find(s => s.Equals(tag)).Num == tag.Num);
                if (!flag)
                {
                    foreach (var item in prefabs.FindAll(s =>
                                 s.Tags.Contains(tag) && s.ID != prefab.ID))
                    {
                        var t = item.Tags.Find(s => s.Equals(tag));
                        if (t.Num > tag.Num)
                            t.Num -= 1;
                    }
                }
            }
            HierarchyManagementWindow.SetPrefabDetails(prefabs);
        }

        #endregion

        
    }
}