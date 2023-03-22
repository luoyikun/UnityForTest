using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Init : MonoBehaviour
{
    [SerializeField]
    private Button _first;

    [SerializeField]
    private Button _second;

    private readonly UniTaskTest    _unitask       = new UniTaskTest();
    private readonly ETTaskTest     _ettask        = new ETTaskTest();
    private readonly NativeTaskTest _nativetask    = new NativeTaskTest();
    private readonly DelayCompare   _delay_compare = new DelayCompare();

    private ButtonExample _button_example;

    private void Awake()
    {
        _button_example = new ButtonExample(_first, _second);

        _ = _button_example.Start();
    }

    public void StartUniTaskException() { _unitask.StartTask().Forget(); }

    public void StartETTaskException() { _ettask.StartTasks().Coroutine(); }

    public void StartNativeTaskException() { _ = _nativetask.StartTasks(); }

    public void DelayCompare() { _ = _delay_compare.Start(); }
}