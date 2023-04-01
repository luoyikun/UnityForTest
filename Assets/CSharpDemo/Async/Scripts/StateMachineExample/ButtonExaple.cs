using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ButtonExample
{
    private readonly Button _first;
    private readonly Button _second;

    public ButtonExample(Button first, Button second)
    {
        _first  = first;
        _second = second;
    }

    public async Task Start()
    {
        await _first;
        Debug.Log("First Clicked!");
        await _second;
        Debug.Log("Second Clicked!");
    }
}


public class AwaitableButton : INotifyCompletion
{
    public bool IsCompleted => _is_completed;

    private readonly Button _btn;

    private Action _continuation;
    private bool   _is_completed;

    public AwaitableButton(Button btn) { _btn = btn; }

    private void _OnClicked()
    {
        _btn.onClick.RemoveListener(_OnClicked);
        _continuation();

        _is_completed = true;
    }

    public void OnCompleted(Action continuation)
    {
        _continuation = continuation;
        _btn.onClick.AddListener(_OnClicked);
    }

    public void GetResult() { }
}

public static class ButtonEx
{
    public static AwaitableButton GetAwaiter(this Button self) { return new AwaitableButton(self); }
}