using UnityEngine;
public class Reddot : MonoBehaviour
{
    [SerializeField]
    private string path;
    public string Path
    {
        get { return path; }
        set
        {
            path = value;
            RegisterReddot();
        }
    }

    [SerializeField]
    public GameObject reddotFlag;

    private void Awake()
    {
        RegisterReddot();
    }

    private void OnDestroy()
    {
        UnRegisterReddot();
    }

    public void SetReddotShow(bool isShown)
    {
        reddotFlag?.SetActive(isShown);
    }

    private void RegisterReddot()
    {
        if (string.IsNullOrEmpty(Path))
        {
            return;
        }

        ReddotManager.RegisterRedDotUI(this);
    }

    private void UnRegisterReddot()
    {
        if (string.IsNullOrEmpty(Path))
        {
            return;
        }

        ReddotManager.UnRegisterRedDotUI(this);
    }
}