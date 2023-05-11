
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class GuideTransformData : GuideWidgetData
{
    public override string Serialize()
    {
        UpdateTransformData();
        string data = JsonUtility.ToJson(this);
        return data;
    }
}
