using System;
using ET;
using UnityEngine;

public class ETTaskTest : IDisposable
{
    public ETTaskTest() { ETTask.ExceptionHandler += _LogException; }

    public void Dispose() { ETTask.ExceptionHandler -= _LogException; }

    private void _LogException(Exception e) { Debug.LogError($"[ETTask] Unobserved Exception = {e.Message}"); }

    public async ETTask StartTasks()
    {
        try
        {
            await _TestETTask();
        }
        catch(Exception e)
        {
            // 可以正常 catch
            Debug.LogError(e);
        }

        try
        {
            // 无法正确 catch, 会被 ExceptionHandler 捕获
            _TestETVoid().Coroutine();
        }
        catch(Exception e)
        {
            // BUG catch 无效!
            Debug.LogError(e);
        }

        try
        {
            // 此处最特殊, ExceptionHandler 无法捕获此处的异常
            // 但是在 Editor 下, 会被 UnitySynchronizationContext invoke 出异常
            // BUG 请不要使用 async void!
            _TestETTask_Async_Void();
        }
        catch(Exception e)
        {
            // BUG catch 无效!
            Debug.LogError(e);
        }
    }


    private async ETTask _TestETTask()
    {
        await ETTask.CompletedTask;
        throw new Exception(nameof(_TestETTask));
    }

    private async ETVoid _TestETVoid()
    {
        await ETTask.CompletedTask;
        throw new Exception(nameof(_TestETVoid));
    }

    private async void _TestETTask_Async_Void()
    {
        await ETTask.CompletedTask;
        throw new Exception(nameof(_TestETTask_Async_Void));
    }

    /// <summary>
    /// ET 不支持此种写法, 可能和 ETTask.CompletedTask 有关
    /// </summary>
    /// <exception cref="Exception"></exception>
    private void _TestETTask_ContinueWith()
    {
        ETTask.CompletedTask.OnCompleted(() => throw new Exception(nameof(_TestETTask_ContinueWith)));
    }
}