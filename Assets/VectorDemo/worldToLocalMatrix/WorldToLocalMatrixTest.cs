using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldToLocalMatrixTest : MonoBehaviour
{
    public Transform m_target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DoWorldToLocalMatrix();
    }

    void DoWorldToLocalMatrix()
    {
        // ��ȡĿ�������ڱ��ؿռ��е�λ��
        Vector3 localPosition = transform.worldToLocalMatrix.MultiplyPoint(m_target.position);

        Debug.Log("Target's local position: " + localPosition);

    }
}
