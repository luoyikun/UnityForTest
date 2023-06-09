using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICircleMask : MonoBehaviour
{
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
    public Material m_mat;
    public float bigR;
    public float smallR;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Update()
    {

        //centerWorld = transform.TransformPoint(new Vector3(center.x, center.y, 0));

        //centerWorld2 = transform.TransformPoint(new Vector3(center2.x, center2.y, 0));


        //radiusWorld = bigR;
        //radiusWorld2 = smallR;

        //m_mat.SetVector("_Center", centerWorld);
        //m_mat.SetFloat("_Radius", radiusWorld);

        //m_mat.SetVector("_Center2", centerWorld2);
        //m_mat.SetFloat("_Radius2", radiusWorld2);
    }
    /// <summary>
    /// 基于世界坐标设置clip
    /// </summary>
    /// <param name="bigPos"></param>
    /// <param name="bigR"></param>
    /// <param name="smallPos"></param>
    /// <param name="smallR"></param>
    public void SetClip(Vector2 bigPos,float bigR,Vector2 smallPos,float smallR)
    {
        center = bigPos;
        center2 = smallPos;

        //centerWorld = transform.TransformPoint(new Vector3(center.x, center.y, 0));
        
        //centerWorld2 = transform.TransformPoint(new Vector3(center2.x, center2.y, 0));


        radiusWorld = bigR;
        radiusWorld2 = smallR;

        m_mat.SetVector("_Center", center);
        m_mat.SetFloat("_Radius", radiusWorld);

        m_mat.SetVector("_Center2", center2);
        m_mat.SetFloat("_Radius2", radiusWorld2);
        
    }

    public void SetClipByAnchoredPosition(Vector2 bigPos, float bigR, Vector2 smallPos, float smallR)
    {
        center = bigPos;
        center2 = smallPos;

        centerWorld = transform.TransformPoint(new Vector3(center.x, center.y, 0));

        centerWorld2 = transform.TransformPoint(new Vector3(center2.x, center2.y, 0));


        radiusWorld = bigR;
        radiusWorld2 = smallR;

        m_mat.SetVector("_Center", centerWorld);
        m_mat.SetFloat("_Radius", radiusWorld);

        m_mat.SetVector("_Center2", centerWorld2);
        m_mat.SetFloat("_Radius2", radiusWorld2);

    }

    public void SetClipByWorldPosition(Vector2 bigPos, float bigR, Vector2 smallPos, float smallR)
    {
        center = bigPos;
        center2 = smallPos;

        centerWorld = transform.InverseTransformPoint(new Vector3(center.x, center.y, 0));

        centerWorld2 = transform.InverseTransformPoint(new Vector3(center2.x, center2.y, 0));


        radiusWorld = bigR;
        radiusWorld2 = smallR;

        m_mat.SetVector("_Center", centerWorld);
        m_mat.SetFloat("_Radius", radiusWorld);

        m_mat.SetVector("_Center2", centerWorld2);
        m_mat.SetFloat("_Radius2", radiusWorld2);

    }

}
