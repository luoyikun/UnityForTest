using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAdapter : MonoBehaviour
{
    [HideInInspector]
    public Vector2 oldAnchorMin;
    [HideInInspector]
    public Vector2 oldAnchorMax;
    [HideInInspector]
    public Vector2 anchorMin;
    [HideInInspector]
    public Vector2 anchorMax;

    RectTransform Panel;
    Rect LastSafeArea = new Rect(0, 0, 0, 0);

    void Awake()
    {
        Panel = GetComponent<RectTransform>();
        oldAnchorMin = Panel.anchorMin;
        oldAnchorMax = Panel.anchorMax;
        Refresh();
    }

    void Update()
    {
        
    }

    void Refresh()
    {
        Rect safeArea = GetSafeArea();

        if (safeArea != LastSafeArea)
            ApplySafeArea(safeArea);
    }

    Rect GetSafeArea()
    {
        return Screen.safeArea;
    }

    void ApplySafeArea(Rect r)
    {
        LastSafeArea = r;

        // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
        anchorMin = r.position;
        anchorMax = r.position + r.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        Panel.anchorMin = anchorMin;
        Panel.anchorMax = anchorMax;

    }
}