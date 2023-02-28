using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// https://blog.csdn.net/qq_38168462/article/details/119004228
/// ����ֱ�ߵĽ���
/// </summary>
public class IntersectPoint : MonoBehaviour
{
    public 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// ����AB��CD�����߶εĽ���.
    /// </summary>
    /// <param name="a">A��</param>
    /// <param name="b">B��</param>
    /// <param name="c">C��</param>
    /// <param name="d">D��</param>
    /// <param name="intersectPos">AB��CD�Ľ���</param>
    /// <returns>�Ƿ��ཻ true:�ཻ false:δ�ཻ</returns>
    private bool TryGetIntersectPoint(Vector3 a, Vector3 b, Vector3 c, Vector3 d, out Vector3 intersectPos)
    {
        intersectPos = Vector3.zero;

        Vector3 ab = b - a;
        Vector3 ca = a - c;
        Vector3 cd = d - c;

        Vector3 v1 = Vector3.Cross(ca, cd);

        if (Mathf.Abs(Vector3.Dot(v1, ab)) > 1e-6)
        {
            // 4���㲻���棺b��acd�ķ����ϵĵ�˲�Ϊ0
            return false;
        }

        if (Vector3.Cross(ab, cd).sqrMagnitude <= 1e-6)
        {
            // ƽ��
            return false;
        }

        Vector3 ad = d - a;
        Vector3 cb = b - c;
        // �����ų⣺��Χ��
        if (Mathf.Min(a.x, b.x) > Mathf.Max(c.x, d.x) || Mathf.Max(a.x, b.x) < Mathf.Min(c.x, d.x)
           || Mathf.Min(a.y, b.y) > Mathf.Max(c.y, d.y) || Mathf.Max(a.y, b.y) < Mathf.Min(c.y, d.y)
           || Mathf.Min(a.z, b.z) > Mathf.Max(c.z, d.z) || Mathf.Max(a.z, b.z) < Mathf.Min(c.z, d.z)
        )
            return false;

        // ��������
        if (Vector3.Dot(Vector3.Cross(-ca, ab), Vector3.Cross(ab, ad)) > 0
            && Vector3.Dot(Vector3.Cross(ca, cd), Vector3.Cross(cd, cb)) > 0)
        {
            Vector3 v2 = Vector3.Cross(cd, ab);
            float ratio = Vector3.Dot(v1, v2) / v2.sqrMagnitude;
            intersectPos = a + ab * ratio;
            return true;
        }

        return false;
    }


}
