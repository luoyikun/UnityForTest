using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AsyncTest : MonoBehaviour
{

    private CancellationTokenSource _cts;

    private void Awake()
    {
        _cts = new CancellationTokenSource();


        _DelayLog().Forget();
        _DelayLog2().Forget();
        _DelayLog3().Forget();
    }

    public void _OnClickCancel()
    {
        _cts.Cancel();
    }

    private async UniTask _DelayLog()
    {
        await UniTask.Delay(10000, cancellationToken: _cts.Token);
        Debug.Log("Finished");
        await UniTask.Delay(1000);
        Debug.Log("Finished---end");
    }

    private async UniTask _DelayLog2()
    {
        await UniTask.Delay(10000, cancellationToken: _cts.Token);
        Debug.Log("Finished22222222222222");
    }


    private async UniTask _DelayLog3()
    {
        await UniTask.Delay(10000);
        Debug.Log("Finished33333333333333333");
    }

}
