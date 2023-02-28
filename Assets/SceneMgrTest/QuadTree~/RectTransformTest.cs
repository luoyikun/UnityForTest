using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectTransformTest : MonoBehaviour
{
    public RectTransform m_rectTrans;
    // Start is called before the first frame update
    void Start()
    {
        GetRect(m_rectTrans);
    }

    public static Rect GetRect(RectTransform rectTransform)
    {
        Rect rect = rectTransform.rect;
        
        return new Rect(rectTransform.position.x + rect.x, rectTransform.position.y + rect.y, rect.width, rect.height);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
