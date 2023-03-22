using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public class ReturnIntCompare
{
    public void Start() { Debug.Log(_Custom().Result); }

    // _Custom 函数与此函数完全相同
    private Task<int> _Example() { return Task.FromResult(1); }

    private Task<int> _Custom()
    {
        var state_machine = new ReturnIntAsyncStateMachine {builder = AsyncTaskMethodBuilder<int>.Create()};

        state_machine.builder.Start(ref state_machine);

        return state_machine.builder.Task;
    }
}

public struct ReturnIntAsyncStateMachine : IAsyncStateMachine
{
    public AsyncTaskMethodBuilder<int> builder;

    public void MoveNext() { builder.SetResult(1); }

    public void SetStateMachine(IAsyncStateMachine state_machine) { }
}