using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICircleMaskEffect : MonoBehaviour
{
    //UITexture targetTex = null;

    public bool m_isRefresh = false;
    void Start()
    {
        //targetTex = GetComponent<UITexture>();
        //targetTex.onRender += Callback;
    }


    public Vector2 center;//第一个圈,挖空
    public Vector2 radiusPoint;

    public Vector2 center2;//第二个圈,留白
    public Vector2 radiusPoint2;

    private Vector3 centerWorld;
    private Vector3 radiusPointWorld;
    private Vector3 centerWorld2;
    private Vector3 radiusPointWorld2;
    private float radiusWorld = 0.2f;
    private float radiusWorld2 = 0.2f;

    public void SetDataAll(float bigX,float bigY,float bigRX,float bigRY,float smallX,float smallY,float smallRX,float smallRY)
    {
        m_isRefresh = true;
        center = new Vector2(bigX, bigY);
        radiusPoint = new Vector2(bigRX, bigRY);

        center2 = new Vector2(smallX,smallY);
        radiusPoint2 = new Vector2(smallRX, smallRY);
    }

    public void SetDataBig(float bigX, float bigY, float bigRX, float bigRY)
    {
        m_isRefresh = true;
        center = new Vector2(bigX, bigY);
        radiusPoint = new Vector2(bigRX, bigRY);
    }

    public void SetDataSmall(float smallX, float smallY, float smallRX, float smallRY)
    {
        m_isRefresh = true;
        center2 = new Vector2(smallX, smallY);
        radiusPoint2 = new Vector2(smallRX, smallRY);
    }

    private void Callback(Material mat)
    {
        if (m_isRefresh)
        {
            m_isRefresh = false;
            centerWorld = transform.TransformPoint(new Vector3(center.x, center.y, 0));
            radiusPointWorld = transform.TransformPoint(new Vector3(radiusPoint.x, radiusPoint.y, 0));

            centerWorld2 = transform.TransformPoint(new Vector3(center2.x, center2.y, 0));
            radiusPointWorld2 = transform.TransformPoint(new Vector3(radiusPoint2.x, radiusPoint2.y, 0));

            radiusWorld = Vector2.Distance(centerWorld, radiusPointWorld);
            radiusWorld2 = Vector2.Distance(centerWorld2, radiusPointWorld2);

            mat.SetVector("_Center", new Vector2(centerWorld.x, centerWorld.y));
            mat.SetFloat("_Radius", radiusWorld);

            mat.SetVector("_Center2", new Vector2(centerWorld2.x, centerWorld2.y));
            mat.SetFloat("_Radius2", radiusWorld2);
        }
    }
    //[Header("需要用API启用")]
    //public bool isPlaying =false;
    //public void EnableEffect()
    //{
    //    if (isPlaying)
    //    {
    //        return;
    //    }
    //    else
    //    {
    //        isPlaying = true;
    //        targetTex = GetComponent<UITexture>();
    //        targetTex.onRender += Callback;
    //    }
    //}

    //public void DisableEffect()
    //{
    //    if (!isPlaying)
    //    {
    //        return;
    //    }
    //    else
    //    {
    //        isPlaying = false;
    //        targetTex.onRender -= Callback;
    //    }
    //}
}
