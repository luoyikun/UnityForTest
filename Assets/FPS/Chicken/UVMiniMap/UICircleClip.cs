using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICircleClip : MonoBehaviour
{
    Material m_mat;
    public Vector3 m_vec;
    public float m_raduis;
    public bool m_isOriUseWorld = true;
    // Start is called before the first frame update
    void Start()
    {
        m_mat = GetComponent<RawImage>().material;
    }

    // Update is called once per frame
    void Update()
    {
        
        //if (m_isOriUseWorld == false)
        //{
        //    Vector2 vec = transform.TransformPoint(m_vec);
        //    m_mat.SetVector("_Center", vec);
        //    Vector2 pointInCircle = new Vector3(m_vec.x + m_raduis, m_vec.y, m_vec.z);
        //    Vector2 pointInCircleWorld = transform.TransformPoint(pointInCircle);
        //    m_mat.SetVector("_PointInCicle", pointInCircleWorld);
        //}
        //else
        //{
        //    m_mat.SetVector("_Center", m_vec);
        //    m_mat.SetFloat("_Silder", m_raduis);
        //}
        
    }

    public void SetClip(Vector2 vec, float radius, Vector2 smallPos,float smallR)
    {
        Vector3 centerWrold = transform.TransformPoint(new Vector3(vec.x,vec.y,0));
        m_mat.SetVector("_Center", centerWrold);
        Vector3 pointInCircle = new Vector3(vec.x + radius, vec.y,0);
        Vector3 pointInCircleWorld = transform.TransformPoint(pointInCircle);
        Vector2 diffVecWorld = centerWrold - pointInCircleWorld;
        float bigRSquare = diffVecWorld.x * diffVecWorld.x + diffVecWorld.y * diffVecWorld.y;
        m_mat.SetFloat("_BigRSquare", bigRSquare);

        Vector3 smallPosWorld = transform.TransformPoint(new Vector3(smallPos.x, smallPos.y, 0));
        m_mat.SetVector("_CenterSmall", smallPosWorld);
        Vector3 pointInCircleSmall = new Vector3(smallPos.x + smallR, smallPos.y, 0);
        Vector3 pointInCircleWorldSamll = transform.TransformPoint(pointInCircleSmall);
        Vector2 diffVecWorldSmall = smallPosWorld - pointInCircleWorldSamll;
        float smallRSquare = diffVecWorldSmall.x * diffVecWorldSmall.x + diffVecWorldSmall.y * diffVecWorldSmall.y;
        float smallRWorld = Vector2.Distance(smallPosWorld, pointInCircleWorldSamll);
        m_mat.SetFloat("_SmallRSquare", smallRWorld);
    }
}
