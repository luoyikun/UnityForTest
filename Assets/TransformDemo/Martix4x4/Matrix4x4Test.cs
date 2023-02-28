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
            //����ĳ���㣬ĳ����Vector3.up����תi*30��
            Matrix4x4 matrix = Matrix4x4.TRS(centerPos, Quaternion.Euler(Vector3.up * (i*60)), Vector3.one);
            //��ת�İ뾶 * ���� = ��ת��ĵ�
            Vector3 point = matrix.MultiplyPoint3x4(new Vector3(_radius, 0, 0));
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = point;
        }
    }

    /// <summary>
    /// һ����������һ������תָ�����������ҷ�����ת��ĵ�
    /// </summary>
    /// <param name="position">Ҫ��ת�ĵ�</param>
    /// <param name="center">Χ����ת�ĵ�</param>
    /// <param name="axis">��ת���᷽��</param>
    /// <param name="angle">��ת�Ƕ�</param>
    /// <returns>��ת��õ��ĵ�</returns>
    public static Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle)
    {
        Vector3 point = Quaternion.AngleAxis(angle, axis) * (position - center);
        Vector3 resultVec3 = center + point;
        return resultVec3;
    }
}
