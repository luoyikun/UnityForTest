#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class TestHierarchyManage : ScriptableObject
{
    [MenuItem("Assets/打开层级管理工具Demo (Open Hierarchy Manage Demo)", false, -799)]
    private static void SelectedGameObject()
    {
        var hierarchyManagementOutSetting = ScriptableObject.CreateInstance<HierarchyManagementOutSetting>();
        AssetDatabase.CreateAsset(hierarchyManagementOutSetting,
            "Assets/UXTools/Res/Samples/HierarchyManage/HierarchyManagementOutSettingDemo.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        var hierarchyManagementSetting = ScriptableObject.CreateInstance<HierarchyManagementSetting>();
        AssetDatabase.CreateAsset(hierarchyManagementSetting,
            "Assets/UXTools/Res/Samples/HierarchyManage/HierarchyManagementSettingDemo.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        HierarchyManagementSetting.isDemo = true;
        Init();
        HierarchyManagementWindow.OpenWindow();
    }
    [MenuItem("Assets/打开层级管理工具Demo (Open Hierarchy Manage Demo)", true)]
    private static bool CheckObjectType()
    {
        Object selectedObject = Selection.activeObject;
        if (selectedObject != null && selectedObject.name == "TestDataConfig")
            return true;
        return false;
    }

    private static void Init()
    {
        // 将项目的数据导入层级管理工具中
        var list = new List<string>();
        var testDataConfig = AssetDatabase.LoadAssetAtPath<TestDataConfig>(
            "Assets/UXTools/Res/Samples/HierarchyManage/Resources/TestDataConfig.asset");
        foreach (var item in testDataConfig.guidList)
            list.Add(item.Guid);
        var hierarchyManagementOutSetting = AssetDatabase.LoadAssetAtPath<HierarchyManagementOutSetting>(
            "Assets/UXTools/Res/Samples/HierarchyManage/HierarchyManagementOutSettingDemo.asset");
        hierarchyManagementOutSetting.CreateGuidList(list, guid =>
        {
            return testDataConfig.guidList.Find(s => s.Guid == guid).Index;
        });

        // 设置最大通道数和每层的最大Level数
        var hierarchyManagementSetting = AssetDatabase.LoadAssetAtPath<HierarchyManagementSetting>(
            "Assets/UXTools/Res/Samples/HierarchyManage/HierarchyManagementSettingDemo.asset");
        hierarchyManagementSetting.SetInitChannelAndLevel(10, 3);

        // 设置保存后的回调
        hierarchyManagementSetting.SetAfterSubmit(l =>
        {
            testDataConfig.guidList.Clear();
            foreach (var item in l)
            {
                testDataConfig.guidList.Add(new GuidWithIndexOut()
                {
                    Guid = item.Guid,
                    Index = item.Index,
                    Name = item.Name,
                });
            }
        });
    }

}
#endif