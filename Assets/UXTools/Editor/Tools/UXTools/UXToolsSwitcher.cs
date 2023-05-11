#if UNITY_EDITOR
using UnityEditor;

namespace ThunderFireUITool
{
    public static class UXToolsSwitcher
    {
        static UXToolsSwitcher()
        {
            var firstTime = FirstImportProcess.FirstTimeImport();
            if (!firstTime) return;
            CreateAllAssets();
        }

        [MenuItem("ThunderFireUXTool/新建配置文件 (Create Assets)/Create All Assets", false, 0)]
        public static void CreateAllAssets()
        {
            CreateAssetFile.Create();
            DecodeData.Decode();
            InspectorLocalizationDecode.Decode();
            CreateLocationLinesData.Create();
            PrefabDefaultSetting.Create();
            PrefabLabelsSettings.Create();
            PrefabOpenedSetting.Create();
            PrefabSettledSetting.Create();
            HierarchyManagementOutSetting.Create();
            HierarchyManagementSetting.Create();
            UIColorCreator.CreateColor();
            UIColorCreator.CreateGradient();
        }
    }
}
#endif