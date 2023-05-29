#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestDataConfig : ScriptableObject
{
    // [MenuItem("Assets/Test Data Config")]
    public static void Create()
    {
        TestDataConfig setting = ScriptableObject.CreateInstance<TestDataConfig>();
        AssetDatabase.CreateAsset(setting, "Assets/UXTools/Res/Samples/HierarchyManage/Resources/TestDataConfig.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public List<GuidWithIndexOut> guidList = new List<GuidWithIndexOut>();
}


[Serializable]
public class GuidWithIndexOut
{
    public string Name;
    public int Index;
    public string Guid;
}
#endif