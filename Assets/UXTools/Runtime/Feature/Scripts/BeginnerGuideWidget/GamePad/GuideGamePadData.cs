using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = System.Object;

[System.Serializable]
public class GuideGamePadData : GuideWidgetData
{
    //GuideDescAnim = "1,{Action3}|1,{Action4}|1,{Action3}|1,{Action4}|1,{Action3}|1,{Action4}| 1 |2, {JoystickMenu} | 1";
    //GuideDescAnim = "2,{Action3}{Action4}| 1 | 2,{Action3}| 1";
    public string GamePadAnimStr;

    public List<TimeAndKeys> guideList = new List<TimeAndKeys>();

    public override string Serialize()
    {
        UpdateTransformData();

        List<string> actionList = new List<string>();
        foreach (var item in guideList)
        {
            if (item.keys.Count > 0)
            {
                string str = "";
                foreach (var it in item.keys)
                {
                    if (it.actions == null)
                    {
                        it.actionName = "";
                        continue;
                    }
                    it.actionName = it.actions.action.name;
                    string[] s = it.actions.name.Split('/');
                    if(s.Length > 0)
                        str = str + "{" + s[s.Length - 1] + "}";
                }
                actionList.Add(item.time.ToString() + "," + str);
            }
            else
            {
                actionList.Add(item.time.ToString() + "");
            }
        }
        GamePadAnimStr = String.Join("|", actionList);

        string data = JsonUtility.ToJson(this);
        return data;
    }
}

[Serializable]
public struct TimeAndKeys
{
    public float time;
    public List<InputActions> keys;
}
[Serializable]
public class InputActions
{
    public InputActionReference actions;
    public string actionName;
}
