#if UNITY_EDITOR && ODIN_INSPECTOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
#if UNITY_2021_2_OR_NEWER
using PrefabStage = UnityEditor.SceneManagement.PrefabStage;
#else
using PrefabStage = UnityEditor.Experimental.SceneManagement.PrefabStage;
#endif

namespace ThunderFireUITool
{
    [InitializeOnLoad]
    internal class PrefabExtension
    {
        static PrefabExtension()
        {
            PrefabUtility.prefabInstanceUpdated += OnPrefabInstanceApply;
            PrefabStage.prefabStageClosing += OnPrefabClosing;
        }

        static void OnPrefabInstanceApply(GameObject instance)
        {
            UIAtlasCheckUserData data = AssetDatabase.LoadAssetAtPath<UIAtlasCheckUserData>(ThunderFireUIToolConfig.UICheckUserDataFullPath);
            if (data != null && data.UICheckEnable)
            {
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(instance);
                UIPrefabCheckTool tool = UIPrefabCheckTool.CheckAtlasButton();
                tool.CheckPrefabUI(prefab);
            }
        }

        static void OnPrefabClosing(PrefabStage prefabStage)
        {
            UIAtlasCheckUserData data = AssetDatabase.LoadAssetAtPath<UIAtlasCheckUserData>(ThunderFireUIToolConfig.UICheckUserDataFullPath);
            if (data != null && data.UICheckEnable)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabStage.GetAssetPath());
                UIPrefabCheckTool tool = UIPrefabCheckTool.CheckAtlasButton();
                tool.CheckPrefabUI(prefab);
            }
        }

        [MenuItem("Assets/检查Prefab资源 (Check Prefab Res)", false, -803)]
        static void OnRightClickPrefab()
        {
            string[] guids = Selection.assetGUIDs;
            if (guids.Length != 1) return;

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            UIPrefabCheckTool tool = UIPrefabCheckTool.CheckAtlasButton();
            tool.CheckPrefabUI(prefab);
        }
    }
}
#endif