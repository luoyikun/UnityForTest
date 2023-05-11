using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreUIAdapter : MonoBehaviour
{
    public UIAdapter adpter;
    RectTransform Panel;
    Vector2 LastOffsetMax;
    Vector2 LastOffsetMin;
    Vector3[] PanelCorner = new Vector3[4];
    Vector3[] adaptercorners = new Vector3[4];
    // Start is called before the first frame update
    void Start()
    {
        Panel = GetComponent<RectTransform>();
        LastOffsetMax = Vector2.zero;
        LastOffsetMin = Vector2.zero;
        //Panel.offsetMax = new Vector2((1-adpter.anchorMax.x)*Screen.width, (1-adpter.anchorMax.y)*Screen.height);
        //Panel.offsetMin = new Vector2((0-adpter.anchorMin.x)*Screen.width, (0-adpter.anchorMin.y)*Screen.height);
        Panel.GetWorldCorners(PanelCorner);
            
        adpter.transform.GetComponent<RectTransform>().GetWorldCorners(adaptercorners);
        Refresh();
    }


    void Refresh()
    {
        Vector2 NowoffsetMax = new Vector2((adpter.oldAnchorMax.x-adpter.anchorMax.x)*Screen.width, (adpter.oldAnchorMax.y-adpter.anchorMax.y)*Screen.height);
        Vector2 NowoffsetMin = new Vector2((adpter.oldAnchorMin.x-adpter.anchorMin.x)*Screen.width, (adpter.oldAnchorMin.y-adpter.anchorMin.y)*Screen.height);
        if(NowoffsetMax!=LastOffsetMax){
            LastOffsetMax = NowoffsetMax;
            Vector2 off = Panel.offsetMax; 
            if(PanelCorner[2].x+LastOffsetMax.x>adaptercorners[2].x)
            {
                off.x += LastOffsetMax.x;
            }
            if(PanelCorner[2].y+LastOffsetMax.y>adaptercorners[2].y)
            {
                off.y += LastOffsetMax.y;
            }
            Panel.offsetMax = off;
        }
        if(NowoffsetMin!=LastOffsetMin){
            LastOffsetMin = NowoffsetMin;
            Vector2 off = Panel.offsetMin;
            if(PanelCorner[0].x+LastOffsetMin.x<adaptercorners[0].x)
            {
                off.x += LastOffsetMin.x;
            }
            if(PanelCorner[0].y+LastOffsetMin.y<adaptercorners[0].y)
            {
                off.y += LastOffsetMin.y;
            }
            Panel.offsetMin = off;
        }
    }
}
