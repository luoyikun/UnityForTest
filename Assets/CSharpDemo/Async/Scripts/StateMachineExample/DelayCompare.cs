using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public class DelayCompare
{
    public async Task Start() { Debug.Log(await _Custom()); }

    // _Custom 函数与此函数完全相同
    private async Task<int> _Example()
    {
        await Task.Delay(1000);

        return 1;
    }

    private Task<int> _Custom()
    {
        var state_machine = new DelayAsyncStateMachine {builder = AsyncTaskMethodBuilder<int>.Create()};

        state_machine.builder.Start(ref state_machine);

        return state_machine.builder.Task;
    }
}

public struct DelayAsyncStateMachine : IAsyncStateMachine
{
    public AsyncTaskMethodBuilder<int> builder;

    private int         _state;
    private TaskAwaiter _awaiter;

    public void MoveNext()
    {
        if(_state == 0)
        {
            _awaiter = Task.Delay(1000).GetAwaiter();

            // lucky check
            if(_awaiter.IsCompleted)
            {
                _state = 1;
                goto state1;
            }

            // 我们只希望 _awaiter 被赋值一次
            // 此时说明 Delay 的内容没有完成
            // 需要告诉 builder 等待 _awaiter 完成, 才可以继续向下 MoveNext
            // 此处通过断点调试, 查看堆栈会非常清晰
            _state = 1;
            builder.AwaitUnsafeOnCompleted(ref _awaiter, ref this);

            return;
        }

        state1:
        if(_state == 1)
        {
            _awaiter.GetResult();
            builder.SetResult(1);
        }
    }

    public void SetStateMachine(IAsyncStateMachine state_machine) { builder.SetStateMachine(state_machine); }
}