using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class viewAngle : MonoBehaviour
{
    Camera _camera;
    Renderer _renderer;
    float TAnangle;
    float lastfieldOfView = 1;
    public float Mul = 1f;
    float lastMul = 0;
    void Start()
    {
        _camera = Camera.main.GetComponent<Camera>();
        _camera.depthTextureMode = DepthTextureMode.Depth;//设置相机.depthTextureMode模式，为shader获取深度图做准备
        _camera.allowHDR = true;//允许摄像机HDR模式增强电弧强度
        lastfieldOfView = _camera.fieldOfView;
        _renderer = GetComponent<Renderer>();
        transform.localScale = new Vector3(1, 1, 1) * Mul;
    }

    void Update()
    {
        if (lastfieldOfView != _camera.fieldOfView)//减少运行次数
        {
            TAnangle = Mathf.Tan(22.5f * 3.14159f / 180f) / Mathf.Tan(_camera.fieldOfView * 3.14159f / 360f);//以40度为基准，得到随视角变化的放大倍数
            _renderer.material.SetFloat("_viewAngle", TAnangle);//代入毒圈的shader
            lastfieldOfView = _camera.fieldOfView;
        }
        if (lastMul != Mul)
        {
            transform.localScale = new Vector3(1, 1, 1/Mul) * Mul;
            _renderer.material.SetFloat("_Mul", Mul);//代入毒圈的shader
            lastMul = Mul;
        }
    }

}
