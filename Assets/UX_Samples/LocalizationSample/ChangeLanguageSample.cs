using UnityEngine;
using UnityEngine.UI;

public class ChangeLanguageSample : MonoBehaviour
{
    [SerializeField]
    private int index;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            LocalizationHelper.SetLanguage(index);
        });
    }
}
