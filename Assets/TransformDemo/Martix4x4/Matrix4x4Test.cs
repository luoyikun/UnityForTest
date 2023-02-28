using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matrix4x4Test : MonoBehaviour
{
    public Transform m_centerTrans;
    float _radius = 10;
    // Start is called before the first frame update
    void Start()
    {
        Test();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Test()
    {
        Vector3 centerPos = m_centerTrans.position;

        for (int i = 0; i < 6; i++)
        {
            //按照某个点，某个轴Vector3.up，旋转i*30度
            Matrix4x4 matrix = Matrix4x4.TRS(centerPos, Quaternion.Euler(Vector3.up * (i*60)), Vector3.one);
            //旋转的半径 * 矩阵 = 旋转后的点
            Vector3 point = matrix.MultiplyPoint3x4(new Vector3(_radius, 0, 0));
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = point;
        }
    }

    /// <summary>
    /// 一个点绕着另一个点旋转指定度数，并且返回旋转后的点
    /// </summary>
    /// <param name="position">要旋转的点</param>
    /// <param name="center">围绕旋转的点</param>
    /// <param name="axis">旋转的轴方向</param>
    /// <param name="angle">旋转角度</param>
    /// <returns>旋转后得到的点</returns>
    public static Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle)
    {
        Vector3 point = Quaternion.AngleAxis(angle, axis) * (position - center);
        Vector3 resultVec3 = center + point;
        return resultVec3;
    }
}
