using Framework.Pattern;
using Newtonsoft.Json;
using SGF.Codec;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetEventMgr : Singleton<NetEventMgr>
{
    private Dictionary<string, ListenerHelper> m_dicMsgListener = new Dictionary<string, ListenerHelper>();
    bool m_isNetLog = true;
    public void AddListener<TMsg>(string cmd, Action<TMsg> onMsg)
    {
        if (m_dicMsgListener.ContainsKey(cmd) == false)
        {
            ListenerHelper helper = new ListenerHelper()
            {
                TMsg = typeof(TMsg),
                onMsg = onMsg
            };

            m_dicMsgListener.Add(cmd, helper);

        }
        else
        {
            m_dicMsgListener[cmd].onMsg = Delegate.Combine(m_dicMsgListener[cmd].onMsg, onMsg);

        }
    }

    public void AddListener(string cmd, Action onMsg)
    {
        
        if (m_dicMsgListener.ContainsKey(cmd) == false)
        {
            ListenerHelper helper = new ListenerHelper()
            {
                
                onMsg = onMsg
            };

            m_dicMsgListener.Add(cmd, helper);

        }
        else
        {
            m_dicMsgListener[cmd].onMsg = Delegate.Combine(m_dicMsgListener[cmd].onMsg, onMsg);

        }
    }

    public void DispatchEvent(string cmd, byte[] buf)
    {
        try
        {
            if (m_dicMsgListener.ContainsKey(cmd))
            {
                var helper = m_dicMsgListener[cmd];
                if (helper != null)
                {
                    if (helper.TMsg != null)
                    {
                        object obj = PBSerializer.NDeserialize(buf, helper.TMsg);
                        if (obj != null)
                        {
                            if (m_isNetLog == true)
                            {
                                string log = JsonConvert.SerializeObject(obj);
                                //if (cmd != MsgIdDefine.RspPlayerSync /*&& cmd != MsgIdDefine.RspMechanism*/)
                                {
                                    Debug.Log("NetRecv-->Key:" + cmd + "-->" + log);
                                }
                            }

                            helper.onMsg.DynamicInvoke(obj);
                        }
                    }
                    else
                    {
                        if (m_isNetLog == true)
                        {
                            //if (cmd != MsgIdDefine.RspPlayerSync/* && cmd != MsgIdDefine.RspMechanism*/)
                            {
                                Debug.Log("NetRecv-->Key:" + cmd);
                            }
                        }
                        helper.onMsg.DynamicInvoke();
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("DispatchEvent:(" + cmd + ")--" + e);
        }
    }

    public void RemoveListener<TMsg>(string cmd, Action<TMsg> onMsg)
    {
        if (m_dicMsgListener.ContainsKey(cmd))
        {
            m_dicMsgListener[cmd].onMsg = Delegate.Remove(m_dicMsgListener[cmd].onMsg, onMsg);
        }
    }


    public void RemoveListener(string cmd, Action onMsg)
    {
        if (m_dicMsgListener.ContainsKey(cmd))
        {
            m_dicMsgListener[cmd].onMsg = Delegate.Remove(m_dicMsgListener[cmd].onMsg, onMsg);
        }
    }
}

class ListenerHelper
{
    public Type TMsg = null;
    public Delegate onMsg;
}