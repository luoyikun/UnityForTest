using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

public class CreatNodes : MonoBehaviour

{

    public float _speed = 3;

    public float forward_speed = 0.006f;

    public float back_speed = 0.0045f;

    float max_dis = 1;

    public float width = 0.15f;

    /// <summary>

    /// 当前的材质

    /// </summary>

    private Material currentMaterial;

    public const int max_click_count = 10;

    //shader最大同时水波的数量是10，要修改请到Wave.shader里面相关代码一起修改

    public Vector4[] uis = new Vector4[max_click_count];

    void Awake()

    {

        currentMaterial = transform.GetComponent<Renderer>().sharedMaterial;

        currentMaterial.SetVectorArray("_ArrayParams", uis);

    }

    private Ray ray;

    private RaycastHit hit;

    bool can_add;

    Vector3 vector3;

    private void FixedUpdate()

    {

        for (int i = 0; i < uis.Length; i++)

        {

            if (uis[i].z > max_dis)

                uis[i].Set(0, 0, 0, 0);

            if (uis[i].x == 0 && uis[i].y == 0)

            {

                if (can_add)

                {

                    //将物体坐标转换成uv坐标

                    uis[i].x = vector3.x + 0.5f;

                    uis[i].y = vector3.y + 0.5f;

                    //头与尾巴的宽度

                    uis[i].z = width;

                    //尾巴的开始点

                    uis[i].w = 0;

                    can_add = false;

                }

            }

            else

            {

                uis[i].z += forward_speed * _speed;

                uis[i].w += back_speed * _speed;

            }

        }

        currentMaterial.SetVectorArray("_ArrayParams", uis);

    }

    private void Update()

    {

        if (Input.GetMouseButtonDown(0))

        {

            // 主相机屏幕点转换为射线

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //射线碰到了物体

            if (Physics.Raycast(ray, out hit))

            {

                if (hit.transform == transform)

                {

                    vector3 = transform.InverseTransformPoint(hit.point);

                    can_add = true;

                }

            }

        }

    }

}
