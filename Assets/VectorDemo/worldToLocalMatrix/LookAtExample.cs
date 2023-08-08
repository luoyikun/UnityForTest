using UnityEngine;

public class LookAtExample : MonoBehaviour
{
    public Transform target;

    private void Update()
    {
        // ��ȡ��ǰ���嵽Ŀ��ķ�������
        Vector3 direction = target.position - transform.position;

        // ����һ���µı任����ʹ��ǰ���峯��Ŀ��λ��
        Matrix4x4 lookAtMatrix = Matrix4x4.LookAt(transform.position, transform.position + direction, Vector3.up);

        // Ӧ�ñ任���󵽵�ǰ�������ת������
        //transform.rotation = Quaternion.LookRotation(lookAtMatrix.GetColumn(2), lookAtMatrix.GetColumn(1));

        transform.rotation = lookAtMatrix.rotation;
    }
}

