using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class UIEventTriggerListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    // Delegate
    public delegate void CallbackDelegate(UnityEngine.EventSystems.PointerEventData eventData, GameObject obj);
    public CallbackDelegate onClick;
    public CallbackDelegate onDown;
    public CallbackDelegate onEnter;
    public CallbackDelegate onExit;
    public CallbackDelegate onUp;


    // Method
    public static UIEventTriggerListener GetListener(GameObject target)
    {
        UIEventTriggerListener lister = target.GetComponent<UIEventTriggerListener>();
        if (lister != null)
            return lister;
        else
        {
            lister = target.AddComponent<UIEventTriggerListener>();
            return lister;
        }
    }

    // 点击
    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
            onClick(eventData, gameObject);
    }

    // 按下
    public void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null)
            onDown(eventData, gameObject);
    }

    // 进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null)
            onEnter(eventData, gameObject);
    }

    // 离开
    public void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null)
            onExit(eventData, gameObject);
    }

    // 抬起、释放
    public void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null)
            onUp(eventData, gameObject);
    }

    void OnDestroy()
    {
        onClick = null;
        onDown = null;
        onEnter = null;
        onExit = null;
        onUp = null;
    }
}
