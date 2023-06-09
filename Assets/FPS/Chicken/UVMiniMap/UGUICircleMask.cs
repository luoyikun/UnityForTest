using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UGUICircleMask : MonoBehaviour
{
    Canvas canvas;
    /// <summary>
    /// 遮罩材质
    /// </summary>
    private Material _material;
    // Start is called before the first frame update
    void Start()
    {
        //获取画布
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _material = GetComponent<Image>().material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 世界坐标向画布坐标转换
    /// </summary>
    /// <param name="canvas">画布</param>
    /// <param name="world">世界坐标</param>
    /// <returns>返回画布上的二维坐标</returns>
    private Vector2 WorldToCanvasPos(Canvas canvas, Vector3 world)
    {
        Vector2 position;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
            world, canvas.GetComponent<Camera>(), out position);
        return position;
    }

    public void SetClip(Vector2 worldPos,float radius)
    {
        Vector2 center = WorldToCanvasPos(canvas, worldPos);
        //设置遮罩材料中的圆心变量
        Vector4 centerMat = new Vector4(center.x, center.y, 0, 0);
        _material.SetVector("_Center", centerMat);
        _material.SetFloat("_Slider", radius);
    }
}
