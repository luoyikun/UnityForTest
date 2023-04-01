using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UniTaskTest : IDisposable
{
    public UniTaskTest()
    {
        // BUG 请一定在游戏某处开启全局 exception 回调监听!
        UniTaskScheduler.UnobservedTaskException += _LogException;
    }

    public void Dispose() { UniTaskScheduler.UnobservedTaskException -= _LogException; }

    private void _LogException(Exception e) { Debug.LogError($"[UniTask] Unobserved Exception = {e.Message}"); }

    public async UniTask StartTask()
    {
        try
        {
            await _TestUniTask();
        }
        catch(Exception)
        {
            // 此处完全可以 catch 到结果
            Debug.LogError("Catch UniTask");
        }

        try
        {
            // 会被全局的 UnobservedTaskException 处理
            _TestUniTaskVoid().Forget();
        }
        catch(Exception)
        {
            // BUG catch 无效!
            Debug.LogError("Catch UniTaskVoid");
        }

        try
        {
            // 此处最特殊, UnobservedTaskException 无法捕获此处的异常
            // 但是在 Editor 下, 会被 UnitySynchronizationContext invoke 出异常
            // BUG 请不要使用 async void!
            _TestUniTask_Async_Void();
        }
        catch(Exception)
        {
            // BUG catch 无效!
            Debug.LogError("Catch async void");
        }

        try
        {
            // 会被全局的 UnobservedTaskException 处理
            _TestUniTask_ContinueWith();
        }
        catch(Exception)
        {
            // BUG catch 无效!
            Debug.LogError("Catch ContinueWith");
        }
    }

    private async UniTask _TestUniTask()
    {
        await UniTask.CompletedTask;
        throw new Exception(nameof(_TestUniTask));
    }

    private async UniTaskVoid _TestUniTaskVoid()
    {
        await UniTask.CompletedTask;
        throw new Exception(nameof(_TestUniTaskVoid));
    }

    private async void _TestUniTask_Async_Void()
    {
        await UniTask.CompletedTask;
        throw new Exception(nameof(_TestUniTask_Async_Void));
    }

    private void _TestUniTask_ContinueWith()
    {
        UniTask.CompletedTask.ContinueWith(() => throw new Exception(nameof(_TestUniTask_ContinueWith))).Forget();
    }
}