using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCShow : MonoBehaviour
{
    public float uiWidth = 400f;
    public int updateFrameRate = 40;
    private float _fps;
    private uint _usedHeapSize;
    private long _gcMemory;

    void Update()
    {
        if (Time.frameCount % updateFrameRate == 0)
        {
            _fps = 1f / Time.unscaledDeltaTime;
            _usedHeapSize = UnityEngine.Profiling.Profiler.usedHeapSize / 1024;
            _gcMemory = System.GC.GetTotalMemory(false) / 1024;
        }
    }

    void OnGUI()
    {
        GUI.depth = 0;
        GUI.BeginGroup(new Rect(Screen.width - uiWidth, 0, uiWidth, Screen.height));
        GUILayout.Label("��ǰ֡��:" + Mathf.Round(_fps));
        GUILayout.Label("��ǰ���ڴ�:" + _usedHeapSize + "KB");
        GUILayout.Label("���ڴ�(GC):" + _gcMemory + "KB");

        //GUILayout.Label("Delay:" + NetWork.instance.Delay * 2);
    }
}
