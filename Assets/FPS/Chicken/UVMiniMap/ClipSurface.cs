using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipSurface : MonoBehaviour
{
    public Vector3 m_ver;
    public float m_radius;
    public Material m_mat;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 posWorld = transform.TransformPoint(m_ver);
        Vector4 vec4 = new Vector4(posWorld.x, posWorld.y, 0, m_radius);
        m_mat.SetVector("_Circle", vec4);
    }
}
