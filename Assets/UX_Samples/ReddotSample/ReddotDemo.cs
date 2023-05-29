using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct MockReddotService
{
    public Dictionary<string, bool> mockdata;

    public MockReddotService(string name)
    {
        mockdata = new Dictionary<string, bool>();

        mockdata["Daily Task/Day 1/Task 1"] = true;
        mockdata["Daily Task/Day 1/Task 2"] = true;
        mockdata["Daily Task/Day 1/Task 3"] = true;

        mockdata["Daily Task/Day 2/Task 1"] = true;
        mockdata["Daily Task/Day 2/Task 2"] = true;
        mockdata["Daily Task/Day 2/Task 3"] = true;

        mockdata["Daily Task/Day 3/Task 1"] = true;
        mockdata["Daily Task/Day 3/Task 2"] = true;
        mockdata["Daily Task/Day 3/Task 3"] = true;

        mockdata["Weekly Task/Week 1/Task 1"] = true;
        mockdata["Weekly Task/Week 1/Task 2"] = true;
        mockdata["Weekly Task/Week 1/Task 3"] = true;

        mockdata["Weekly Task/Week 2/Task 1"] = true;
        mockdata["Weekly Task/Week 2/Task 2"] = true;
        mockdata["Weekly Task/Week 2/Task 3"] = true;

        mockdata["Weekly Task/Week 3/Task 1"] = true;
        mockdata["Weekly Task/Week 3/Task 2"] = true;
        mockdata["Weekly Task/Week 3/Task 3"] = true;

        mockdata["Monthly Task/Month 1/Task 1"] = true;
        mockdata["Monthly Task/Month 1/Task 2"] = true;
        mockdata["Monthly Task/Month 1/Task 3"] = true;

        mockdata["Monthly Task/Month 2/Task 1"] = true;
        mockdata["Monthly Task/Month 2/Task 2"] = true;
        mockdata["Monthly Task/Month 2/Task 3"] = true;

        mockdata["Monthly Task/Month 3/Task 1"] = true;
        mockdata["Monthly Task/Month 3/Task 2"] = true;
        mockdata["Monthly Task/Month 3/Task 3"] = true;

    }

    public bool GetMockReddotData(string key)
    {
        if (mockdata.TryGetValue(key, out bool shown))
        {
            return shown;
        }
        else
        {
            Debug.Log(" Don't Contain Key :" + key);
            return false;
        }
    }

    public void SetMockReddotData(bool shown, string key)
    {
        mockdata[key] = shown;
    }
}

public class ReddotDemo : MonoBehaviour
{
    public GameObject ReddotSceondPrefab;
    public GameObject ReddotThirdPrefab;

    public Transform secondListContent;
    public Transform thirdListContent;

    public Button ReddotButton1;
    public Button ReddotButton2;
    public Button ReddotButton3;

    private MockReddotService mockdata;

    // Start is called before the first frame update
    void Start()
    {
        mockdata = new MockReddotService("");

        Reddot reddot1 = ReddotButton1.GetComponent<Reddot>();

        ReddotManager.SetRedDotData(GetMockReddotData(reddot1.Path), reddot1.Path);

        Reddot reddot2 = ReddotButton2.GetComponent<Reddot>();
        ReddotManager.SetRedDotData(GetMockReddotData(reddot2.Path), reddot2.Path);

        //dynamic key
        Reddot reddot3 = ReddotButton3.GetComponent<Reddot>();
        reddot3.Path = "Monthly Task";
        ReddotManager.SetRedDotData(GetMockReddotData(reddot3.Path), reddot3.Path);

        ReddotButton1.onClick.AddListener(() => { InitSecondList(reddot1.Path + "/Day "); });
        ReddotButton2.onClick.AddListener(() => { InitSecondList(reddot2.Path + "/Week "); });
        ReddotButton3.onClick.AddListener(() => { InitSecondList(reddot3.Path + "/Month "); });

        InitSecondList(reddot1.Path + "/Day ");
    }

    private void InitSecondList(string buttonKey)
    {
        ClearAllList();

        for (int i = 1; i <= 3; i++)
        {
            var go = Instantiate(ReddotSceondPrefab, secondListContent);
            Reddot secondReddot = go.GetComponent<Reddot>();
            secondReddot.Path = buttonKey + i;
            ReddotManager.SetRedDotData(GetMockReddotData(secondReddot.Path), secondReddot.Path);

            ReddotManager.RefreshShown(secondReddot.Path);


            Button secondReddotButton = go.GetComponent<Button>();
            secondReddotButton.onClick.AddListener(
            () =>
                {
                    InitThirdList(secondReddot.Path + "/Task ");
                }
            );
            Text text = go.GetComponentInChildren<Text>();
            text.text = secondReddot.Path.Split('/')[1];


            if (i == 1)
            {
                InitThirdList(secondReddot.Path + "/Task ");
            }
        }
    }

    private void InitThirdList(string buttonKey)
    {
        for (int i = thirdListContent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(thirdListContent.GetChild(i).gameObject);
        }

        for (int i = 1; i <= 3; i++)
        {
            var go = Instantiate(ReddotThirdPrefab, thirdListContent);
            Reddot thirdReddot = go.GetComponent<Reddot>();
            thirdReddot.Path = buttonKey + i;

            ReddotThirdController reddotController = go.GetComponent<ReddotThirdController>();
            reddotController.Init();
        }
    }


    private void ClearAllList()
    {
        for (int i = thirdListContent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(thirdListContent.GetChild(i).gameObject);
        }

        for (int i = secondListContent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(secondListContent.GetChild(i).gameObject);
        }
    }

    public void SetMockReddotData(bool isShow, string key)
    {
        mockdata.SetMockReddotData(isShow, key);
    }

    public bool GetMockReddotData(string path)
    {
        foreach (var kvp in mockdata.mockdata)
        {
            if (kvp.Key.StartsWith(path))
            {
                if (kvp.Value)
                    return true;
            }
        }
        return false;
    }
}
