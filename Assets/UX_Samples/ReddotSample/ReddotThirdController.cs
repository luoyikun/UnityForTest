using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ReddotThirdController : MonoBehaviour
{
    public Reddot reddot;

    public Text Name;
    public Toggle completeToggle;

    public void Init()
    {
        reddot = GetComponent<Reddot>();

        bool show = GetComponentInParent<ReddotDemo>().GetMockReddotData(reddot.Path);
        ReddotManager.SetRedDotData(show, reddot.Path);

        var paths = reddot.Path.Split('/');
        Name.text = paths[1] + ": " + paths[2];

        completeToggle.isOn = !show;

        completeToggle.onValueChanged.AddListener((isOn) =>
        {
            ReddotManager.SetRedDotData(!isOn, reddot.Path);
            GetComponentInParent<ReddotDemo>().SetMockReddotData(!isOn, reddot.Path);
        });
    }
}
