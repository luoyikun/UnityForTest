
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class GuideGestureData : GuideWidgetData
{
    //使用自定义的引导动画
    public bool UseCustomGesture = false;
    public GameObject GestureObject;

    //手势类型
    public GestureType gestureType = GestureType.ThumbClick;
    //跟随物体
    public ObjectSelectType objectSelectType = ObjectSelectType.auto;
    public GameObject selectedObject;

    /// <summary>
    /// 只有drag类型才需要
    /// </summary>
    public Vector3 dragStartPos;
    public Vector3 dragEndPos;
    public AnimationCurve dragCurve;
    public string startPosName;
    public string endPosName;


    public override string Serialize()
    {
        UpdateTransformData();
#if UNITY_EDITOR
        UpdateDragPos();
#endif
        string data = JsonUtility.ToJson(this);
        return data;
    }
    public void SetTarget(GameObject go)
    {
        GestureObject = go;
    }
#if UNITY_EDITOR
    private void UpdateDragPos()
    {
        if (gestureType == GestureType.ThumbDrag||gestureType == GestureType.ForeFingerDrag)
        {
            var startPosControllerTrans = transform.Find(startPosName);
            var endPosControllerTrans = transform.Find(endPosName);
            if (startPosControllerTrans != null && endPosControllerTrans != null)
            {
                dragStartPos = transform.parent.InverseTransformPoint(startPosControllerTrans.position);
                dragEndPos = transform.parent.InverseTransformPoint(endPosControllerTrans.position);
            }
        }
    }

#endif
}
