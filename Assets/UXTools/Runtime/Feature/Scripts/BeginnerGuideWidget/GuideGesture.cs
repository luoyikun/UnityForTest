using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum GestureType
{
    ThumbClick,
    ThumbDrag,
    ThumbLongPress,
    ThumbRotate,
    ThumbSlideUp,
    ThumbSlideDown,
    ThumbSlideLeft,
    ThumbSlideRight,
    ForeFingerClick,
    ForeFingerDrag,
    ForeFingerLongPress,
    ForeFingerRotate,
    ForeFingerSlideUp,
    ForeFingerSlideDown,
    ForeFingerSlideLeft,
    ForeFingerSlideRight
}
public enum ObjectSelectType
{
    auto,
    select
}

public class GuideGesture : GuideWidgetBase
{
    private GameObject GestureAnimation;
    private Animator GestureAnimator;
    public GameObject GestureObject;
    public GestureType gesType = GestureType.ThumbClick;

    private GuideGestureData gestureData;
    private AnimationCurve dragCurve;
    private Vector3 dragStartPos;
    private Vector3 dragEndPos;

    private Tween dragTween;
    // void Update(){
    //     if(gestureData.selectedObject!=null){
    //         transform.SetPositionAndRotation(gestureData.selectedObject.transform.position, gestureData.selectedObject.transform.rotation);
    //     }
    // }

    public override void Init(GuideWidgetData data)
    {
        gestureData = data as GuideGestureData;
        if (gestureData != null)
        {
            gestureData.ApplyTransformData(transform);
            if (gestureData.objectSelectType == ObjectSelectType.select && gestureData.selectedObject != null)
            {
                transform.position = gestureData.selectedObject.transform.position;
            }
            if (gestureData.Open)
            {
                if (gestureData.UseCustomGesture == true)
                {
                    GestureAnimator = GestureObject.GetComponent<Animator>();
                    LoadCustomGesture(GestureObject);
                    return;
                }
                gesType = gestureData.gestureType;
                dragCurve = gestureData.dragCurve;
                dragStartPos = gestureData.dragStartPos;
                dragEndPos = gestureData.dragEndPos;

                LoadGesture(gesType);

                GestureAnimator = GestureAnimation.GetComponent<Animator>();
                if (gestureData.gestureType == GestureType.ThumbDrag || gestureData.gestureType == GestureType.ForeFingerDrag)
                {
                    transform.localPosition = dragStartPos;
                }
            }
        }
        else {
            LoadGesture(gesType);
        }
    }

    public override List<int> GetControlledInstanceIds()
    {
        Transform[] allChild = gameObject.GetComponentsInChildren<Transform>(true);
        List<int> list = new List<int>();
        foreach (var child in allChild)
        {
            list.Add(child.GetInstanceID());
        }
        Transform startPosControllerTrans = this.transform.Find("StartPosController");
        Transform endPosControllerTrans = this.transform.Find("EndPosController");
        if (startPosControllerTrans != null)
        {
            list.Add(startPosControllerTrans.GetInstanceID());
        }
        if (endPosControllerTrans != null)
        {
            list.Add(endPosControllerTrans.GetInstanceID());
        }
        return list;
    }

    public override void Show()
    {
        PlayAnimation();
    }

    public override void Stop()
    {
        StopAnimation();
    }
    public void LoadGesture(GestureType type)
    {
        GameObject go = GestureAnimation;

        if (type == GestureType.ThumbClick)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Gesture/clickPrefab_thumb"), transform);
        }
        else if (type == GestureType.ThumbDrag)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Gesture/dragPrefab_thumb"), transform);
        }
        else if (type == GestureType.ThumbLongPress)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Gesture/longclickPrefab_thumb"), transform);
        }
        else if (type == GestureType.ThumbRotate)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Gesture/rotate_thumb"), transform);
        }
        else if (type == GestureType.ThumbSlideDown)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Gesture/slideDown_thumb"), transform);
        }
        else if (type == GestureType.ThumbSlideUp)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Gesture/slideUp_thumb"), transform);
        }
        else if (type == GestureType.ThumbSlideLeft)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Gesture/slideLeft_thumb"), transform);
        }
        else if (type == GestureType.ThumbSlideRight)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Gesture/slideRight_thumb"), transform);
        }
        else if (type == GestureType.ForeFingerClick)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Gesture/clickPrefab_forefinger"), transform);
        }
        else if (type == GestureType.ForeFingerDrag)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Gesture/dragPrefab_forefinger"), transform);
        }
        else if (type == GestureType.ForeFingerLongPress)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Gesture/longclickPrefab_forefinger"), transform);
        }
        else if (type == GestureType.ForeFingerRotate)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Gesture/rotate_forefinger"), transform);
        }
        else if (type == GestureType.ForeFingerSlideDown)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Gesture/slideDown_forefinger"), transform);
        }
        else if (type == GestureType.ForeFingerSlideUp)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Gesture/slideUp_forefinger"), transform);
        }
        else if (type == GestureType.ForeFingerSlideLeft)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Gesture/slideLeft_forefinger"), transform);
        }
        else if (type == GestureType.ForeFingerSlideRight)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Gesture/slideRight_forefinger"), transform);
        }

        Object.DestroyImmediate(go);
    }
    public void LoadCustomGesture(GameObject obj)
    {
        GameObject go = GestureAnimation;
        GestureAnimation = Instantiate(obj, transform);
        Object.DestroyImmediate(go);
    }

    public void PlayAnimation()
    {
        if(gestureData==null){
            return;
        }
        if (gestureData.UseCustomGesture == true)
        {
            GestureAnimator.Play("Base Layer.start");
            return;
        }
        if (GestureAnimator != null)
        {
            GestureAnimator.Play("start");
        }
        //只有拖动手势需要额外的位移动画
        if (gestureData.gestureType == GestureType.ThumbDrag || gestureData.gestureType == GestureType.ForeFingerDrag)
        {
            //Debug.Log("dragEndPos" + dragEndPos.ToString());
            dragTween = transform.DOLocalMove(dragEndPos, 3).SetEase(dragCurve).SetLoops(-1);
        }
    }

    public void StopAnimation()
    {
        if (GestureAnimator != null)
        {
            GestureAnimator.speed = 0;
        }
        //只有拖动手势需要关闭额外的位移动画
        if ((gestureData.gestureType == GestureType.ThumbDrag || gestureData.gestureType == GestureType.ForeFingerDrag) && dragTween != null)
        {
            dragTween.Kill();
        }
    }
    public void SetTarget(GameObject go)
    {
        GestureObject = go;
    }
}
