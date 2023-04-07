using Framework.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NetEvent
{
    public delegate void OnNotificationDelegate(byte[] bufByte);
}
public class NetEventManager : Singleton<NetEventManager>
{
    Dictionary<object, List<NetEvent.OnNotificationDelegate>> m_dicScriptDelegate = new Dictionary<object, List<NetEvent.OnNotificationDelegate>>();
    private Dictionary<string, NetEvent.OnNotificationDelegate> eventListerners = new Dictionary<string, NetEvent.OnNotificationDelegate>();


    public void ScriptAddDelegate(object key, NetEvent.OnNotificationDelegate listener)
    {
        if (!m_dicScriptDelegate.ContainsKey(key))
        {
            m_dicScriptDelegate[key] = new List<NetEvent.OnNotificationDelegate>();
           
        }
        m_dicScriptDelegate[key].Add(listener);
    }
    /*
        * 监听事件
        */

    //添加监听事件
    public void AddEventListener(string type, NetEvent.OnNotificationDelegate listener)
    {
        if (!eventListerners.ContainsKey(type))
        {
            eventListerners.Add(type, null);
        }
        //eventListerners[type] -= listener;
        eventListerners[type] += listener;
    }

    //移除监听事件
    public void RemoveEventListener(string type, NetEvent.OnNotificationDelegate listener)
    {
        if (!eventListerners.ContainsKey(type))
        {
            return;
        }
        eventListerners[type] -= listener;
    }

    //移除某一类型所有的监听事件
    public void RemoveEventListener(string type)
    {
        if (eventListerners.ContainsKey(type))
        {
            eventListerners.Remove(type);
        }
    }

    /*
        * 派发事件
        */

    //派发数据
    public void DispatchEvent(string type, byte[] buf)
    {
        if (eventListerners.ContainsKey(type))
        {
            if (eventListerners[type] != null)
            {
                eventListerners[type](buf);
            }
        }
    }

    //派发无数据
    public void DispatchEvent(string type)
    {
        DispatchEvent(type, null);
    }

    //查找是否有当前类型事件监听
    public bool HasEventListener(string type)
    {
        return eventListerners.ContainsKey(type);
    }
}
