using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PreviewGuideMono : MonoBehaviour
{
    void Start()
    {
        var objs = Object.FindObjectsOfType<UIBeginnerGuideDataList>();
        foreach (UIBeginnerGuideDataList obj in objs)
        {
            if (obj.PreviewMode)
            {
                UIBeginnerGuideManager.Instance.ClearGuide();
                UIBeginnerGuideManager.Instance.AddGuide(obj);
                UIBeginnerGuideManager.Instance.ShowGuideList();
                UIBeginnerGuideManager.Instance.isPreviewing = true;
                break;
            }
        }

    }
}