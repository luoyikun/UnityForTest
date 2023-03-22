using System;
using System.Threading;
using System.Threading.Tasks;
using ET;
using UnityEngine;

public class NativeTaskTest : IDisposable
{
    public NativeTaskTest() { TaskScheduler.UnobservedTaskException += _LogException; }

    public void Dispose() { TaskScheduler.UnobservedTaskException -= _LogException; }

    private void _LogException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        Debug.LogError($"[Task] Unobserved Exception = {e.Exception}");
    }

    public async Task StartTasks()
    {
        try
        {
            await _TestTask();
        }
        catch(Exception e)
        {
            // 可以正确捕获
            Debug.LogError(e);
        }

        try
        {
            _ = _TestTask();
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }

        try
        {
            // 此处最特殊, UnobservedTaskException 无法捕获此处的异常
            // 但是在 Editor 下, 会被 UnitySynchronizationContext invoke 出异常
            // BUG 请不要使用 async void!
            _TestTask_Async_Void();
        }
        catch(Exception e)
        {
            // BUG catch 无效!
            Debug.LogError(e);
        }

        try
        {
            // 会被 UnobservedTaskException 捕获异常
            _TestTask_ContinueWith();
        }
        catch(Exception e)
        {
            // BUG catch 无效!
            Debug.LogError(e);
        }
    }

    private async Task _TestTask()
    {
        await Task.CompletedTask;
        throw new Exception(nameof(_TestTask));
    }

    private async void _TestTask_Async_Void()
    {
        await Task.CompletedTask;
        throw new Exception(nameof(_TestTask_Async_Void));
    }

    private void _TestTask_ContinueWith()
    {
        Task.CompletedTask.ContinueWith(_ => throw new Exception(nameof(_TestTask_ContinueWith))).ConfigureAwait(false);
    }
}