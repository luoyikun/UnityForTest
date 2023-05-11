using System;
using System.Collections.Generic;
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;

public class QuickBackgroundData : ScriptableObject
{
    public List<QuickBackgroundDataSingle> list = new List<QuickBackgroundDataSingle>();
}

[Serializable]
public class QuickBackgroundDetail
{
    //use for QuickBackgroundList
    public bool isOpen = false;
    public Vector3 position = default;
    public Vector3 rotation = default;
    public Vector3 scale = Vector3.one;
    public Vector2 size = new Vector2(1920, 1080);
    public Color color =Color.white;
    public Sprite sprite = default;

    public string trans;
    public string img;
}

[Serializable]
public class QuickBackgroundDataSingle
{
    public string name;
    public string guid;
    public QuickBackgroundDetail detail = new QuickBackgroundDetail();

}

