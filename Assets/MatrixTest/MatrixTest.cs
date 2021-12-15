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
        transform.Matrix4x4_Transfrom(new Vector3(1, 2, 3));
        transform.Matrix4x4_Scale(new Vector3(4, 4, 4));
        transform.Matrix4x4_Rotation(SelfAxle.X, 46f);
        

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
