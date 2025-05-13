using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class LambdaTest : MonoBehaviour
{
    public delegate void AmandaAction();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int i = 2;
        Profiler.BeginSample("Lambda1");
        
        OnLambda(() => { int i3 = i; i3 = 1; });
        Profiler.EndSample();

        int i2 = 2;
        Profiler.BeginSample("Lambda2");
        OnLambda2(() => { int i4 = i2;  i4 = 1; });
        Profiler.EndSample();
    }

    public void OnLambda(AmandaAction act)
    {
        act?.Invoke();
    }

    public void OnLambda2(Action act)
    {
        act?.Invoke();
    }
}
