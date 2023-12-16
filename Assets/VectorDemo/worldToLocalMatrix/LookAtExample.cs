using UnityEngine;

public class LookAtExample : MonoBehaviour
{
    public Transform target;

    private void Update()
    {
        // 获取当前物体到目标的方向向量
        Vector3 direction = target.position - transform.position;

        // 创建一个新的变换矩阵，使当前物体朝向目标位置
        Matrix4x4 lookAtMatrix = Matrix4x4.LookAt(transform.position, transform.position + direction, Vector3.up);

        // 应用变换矩阵到当前物体的旋转属性上
        //transform.rotation = Quaternion.LookRotation(lookAtMatrix.GetColumn(2), lookAtMatrix.GetColumn(1));

        transform.rotation = lookAtMatrix.rotation;
    }
}

