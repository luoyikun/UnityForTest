using System.Runtime.CompilerServices;

public class VoidCompare
{
    public void Start() { _Custom(); }

    // _Custom 函数与此函数完全相同
    private async void _Example() { return; }

    private void _Custom()
    {
        var state_machine = new VoidAsyncStateMachine {builder = AsyncVoidMethodBuilder.Create()};

        state_machine.builder.Start(ref state_machine);
    }
}

public struct VoidAsyncStateMachine : IAsyncStateMachine
{
    public AsyncVoidMethodBuilder builder;

    public void MoveNext() { builder.SetResult(); }

    public void SetStateMachine(IAsyncStateMachine state_machine) { }
}