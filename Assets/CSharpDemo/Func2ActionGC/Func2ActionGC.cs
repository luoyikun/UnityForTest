using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Func2ActionGC : MonoBehaviour
{
    Action<int> m_act;
    // Start is called before the first frame update
    void Start()
    {
        m_act = PrintMessage;
    }

    // Update is called once per frame
    void Update()
    {
        Profiler.BeginSample("Test");
        DoFunc(m_act);
        Profiler.EndSample();
    }

    public void  PrintMessage(int message)
    {
        //Debug.Log(message);
        int i = message;
    }

    public void DoFunc(Action<int> func)
    {
        if (func != null)
        {
            func(123);
        }
    }

}
