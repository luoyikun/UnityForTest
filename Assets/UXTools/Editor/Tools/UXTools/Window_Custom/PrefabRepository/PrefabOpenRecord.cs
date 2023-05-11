#if UNITY_EDITOR
using UnityEditor;

namespace ThunderFireUITool
{
    [InitializeOnLoad]
    public class PrefabOpenRecord
    {
        static PrefabOpenRecord()
        {
            PrefabStageUtils.AddOpenedEvent((p) =>
            {
                var prefabStage = PrefabStageUtils.GetCurrentPrefabStage();
                string guid = AssetDatabase.AssetPathToGUID(prefabStage.GetAssetPath());
                //Debug.Log(guid);
                if (SwitchSetting.CheckValid(SwitchSetting.SwitchType.RecentlyOpened))
                {
                    var RecentOpened = SettingAssetsUtils.GetAssets<PrefabOpenedSetting>();
                    var RecentList = RecentOpened.List;
                    if (!RecentList.Contains(guid))
                    {
                        RecentOpened.Add(guid);
                    }
                    else
                    {
                        RecentOpened.ResortLast(guid);
                    }
                }
                if (!SwitchSetting.CheckValid(SwitchSetting.SwitchType.PrefabMultiOpen)) return;
                EditorApplication.delayCall += () => PrefabTabs.OpenTab(guid, false);
                EditorApplication.delayCall += () => ResolutionController.RefreshResolution();
            });
            PrefabStageUtils.AddClosingEvent((p) =>
            {
                if (!SwitchSetting.CheckValid(SwitchSetting.SwitchType.PrefabMultiOpen)) return;
#if UNITY_2020_1_OR_NEWER
                var prefabStage = PrefabStageUtils.GetCurrentPrefabStage();
                if (prefabStage == null)
                {
                    PrefabTabs.CloseTab("", false);
                    ResolutionController.RefreshResolution();
                }
#else
                PrefabTabs.CloseTab("", false);
                EditorApplication.delayCall += () => ResolutionController.RefreshResolution();
#endif
            });
        }
    }
}
#endif