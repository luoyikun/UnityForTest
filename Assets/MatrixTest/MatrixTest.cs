using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixTest : MonoBehaviour
{
    public Transform obj1;
    public Transform obj2;
    // Start is called before the first frame update
    void Start()
    {


        //transform.Matrix4x4_Scale(new Vector3(2, 2, 2));//缩放
        //transform.Matrix4x4_Transfrom(new Vector3(6, 6, 6));//位移
        //transform.Matrix4x4_Rotation(SelfAxle.X, 90f);//旋转

        //Matrix4x4 scale = MatrixUtils.Scale2Matrix(new Vector3(2, 2, 2));//缩放
        //Matrix4x4 rotate = MatrixUtils.Rotate2Matrix(SelfAxle.X, 45f);
        //Matrix4x4 pos = MatrixUtils.Pos2Matrix(new Vector3(6, 6, 6));

        //Matrix4x4 newMat = pos *rotate *scale;
        //transform.Matrix4x4Change(newMat);

        obj1.Matrix4x4_Rotation(SelfAxle.X, 90f);//旋转
        obj1.Matrix4x4_Rotation(SelfAxle.Y, 90f);//旋转

        obj2.Matrix4x4_Rotation(SelfAxle.Y, 90f);//旋转
        obj2.Matrix4x4_Rotation(SelfAxle.X, 90f);//旋转
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
