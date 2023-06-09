using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UGUICircleMask : MonoBehaviour
{
    Canvas canvas;
    /// <summary>
    /// ���ֲ���
    /// </summary>
    private Material _material;
    // Start is called before the first frame update
    void Start()
    {
        //��ȡ����
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _material = GetComponent<Image>().material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// ���������򻭲�����ת��
    /// </summary>
    /// <param name="canvas">����</param>
    /// <param name="world">��������</param>
    /// <returns>���ػ����ϵĶ�ά����</returns>
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
        //�������ֲ����е�Բ�ı���
        Vector4 centerMat = new Vector4(center.x, center.y, 0, 0);
        _material.SetVector("_Center", centerMat);
        _material.SetFloat("_Slider", radius);
    }
}
