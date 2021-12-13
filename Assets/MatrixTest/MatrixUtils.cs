using UnityEngine;

internal static class MatrixUtils
{
    public static void Matrix4x4Change(this Transform transfrom, Matrix4x4 matrix4X4)
    {
        transfrom.localScale = matrix4X4.GetScale();
        transfrom.rotation = matrix4X4.GetRotation();
        transfrom.position = matrix4X4.GetPostion();
    }

    public static Quaternion GetRotation(this Matrix4x4 matrix4X4)
    {
        float qw = Mathf.Sqrt(1f + matrix4X4.m00 + matrix4X4.m11 + matrix4X4.m22) / 2;
        float w = 4 * qw;
        float qx = (matrix4X4.m21 - matrix4X4.m12) / w;
        float qy = (matrix4X4.m02 - matrix4X4.m20) / w;
        float qz = (matrix4X4.m10 - matrix4X4.m01) / w;
        return new Quaternion(qx, qy, qz, qw);
    }

    public static Vector3 GetPostion(this Matrix4x4 matrix4X4)
    {
        var x = matrix4X4.m03;
        var y = matrix4X4.m13;
        var z = matrix4X4.m23;
        return new Vector3(x, y, z);
    }

    public static Vector3 GetScale(this Matrix4x4 m)
    {
        var x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
        var y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
        var z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);
        return new Vector3(x, y, z);
    }

    //pos转矩阵
    public static Matrix4x4 Pos2Matrix(Vector3 targetPos)
    {
        Matrix4x4 matrix = UnityEngine.Matrix4x4.identity;
        
        matrix.m03 = targetPos.x;
        matrix.m13 = targetPos.y;
        matrix.m23 = targetPos.z;

        return matrix;
    }

    //rotate转矩阵
    public static Matrix4x4 Rotate2Matrix(SelfAxle axle,float angle)
    {
        Matrix4x4 matrix = UnityEngine.Matrix4x4.identity;

        

        if (axle == SelfAxle.X)
        {
            matrix.m11 = Mathf.Cos(angle * Mathf.Deg2Rad);
            matrix.m12 = -Mathf.Sin(angle * Mathf.Deg2Rad);
            matrix.m21 = Mathf.Sin(angle * Mathf.Deg2Rad);
            matrix.m22 = Mathf.Cos(angle * Mathf.Deg2Rad);

        }
        else if (axle == SelfAxle.Y)
        {
            matrix.m00 = Mathf.Cos(angle * Mathf.Deg2Rad);
            matrix.m02 = Mathf.Sin(angle * Mathf.Deg2Rad);
            matrix.m20 = -Mathf.Sin(angle * Mathf.Deg2Rad);
            matrix.m22 = Mathf.Cos(angle * Mathf.Deg2Rad);
        }
        else
        {
            matrix.m00 = Mathf.Cos(angle * Mathf.Deg2Rad);
            matrix.m01 = -Mathf.Sin(angle * Mathf.Deg2Rad);
            matrix.m10 = Mathf.Sin(angle * Mathf.Deg2Rad);
            matrix.m11 = Mathf.Cos(angle * Mathf.Deg2Rad);
        }

        return matrix;
    }

    //scale转矩阵
    public static Matrix4x4 Scale2Matrix(Vector3 targetScale)
    {
        Matrix4x4 matrix = UnityEngine.Matrix4x4.identity;
        
        matrix.m00 = targetScale.x;
        matrix.m11 = targetScale.y;
        matrix.m22 = targetScale.z;

        return matrix;
    }
}