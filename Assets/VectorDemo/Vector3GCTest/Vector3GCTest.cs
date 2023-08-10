using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Vector3GCTest : MonoBehaviour
{
    Transform m_trans;
    public Transform m_target;
    // Start is called before the first frame update
    void Start()
    {
        m_trans = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Matrix4x4 worldToLocalMatrix = m_trans.worldToLocalMatrix;
        Vector3 tarToLocal = worldToLocalMatrix.MultiplyPoint(m_target.position);
        Debug.Log(tarToLocal);
    }
}

public struct NoRefStr
{
    int x;
    int y;
}

public struct RefStr
{
    string x ;
}
