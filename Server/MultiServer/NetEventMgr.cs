using Framework.Pattern;
using Newtonsoft.Json;
using SGF.Codec;
using SGF.Network.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiServer
{
    class ListenerHelper
    {
        public Type TMsg;
        public Delegate onMsg;
        public string fromMsg = "";
    }

    class NetEventMgr : Singleton<NetEventMgr>
    {
        private Dictionary<string, ListenerHelper> m_dicMsgListener = new Dictionary<string, ListenerHelper>();

        public void AddListener<TMsg>(string cmd, Action<Client, byte[], TMsg> onMsg)
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

        public void AddListener(string cmd, Action<Client, byte[]> onMsg)
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

        public void AddListener(string cmd, Action<Client, byte[], string> onMsg)
        {
            if (m_dicMsgListener.ContainsKey(cmd) == false)
            {
                ListenerHelper helper = new ListenerHelper()
                {
                    fromMsg = cmd,
                    onMsg = onMsg
                };

                m_dicMsgListener.Add(cmd, helper);

            }
            else
            {
                m_dicMsgListener[cmd].onMsg = Delegate.Combine(m_dicMsgListener[cmd].onMsg, onMsg);

            }
        }

        public void DispatchEvent(Client client, string cmd, byte[] buf)
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
                            if (DataMgr.m_isNetLog == true)
                            {
                                //if (cmd != MsgIdDefine.RspSyncPlayer)
                                {
                                    string log = JsonConvert.SerializeObject(obj);
                                    Console.WriteLine("Rec:ID:(" + client.m_player.id + ")-->Key:" + cmd + "-->" + log);
                                }
                            }
                            helper.onMsg.DynamicInvoke(client, buf, obj);
                        }
                    }
                    else
                    {
                        if (helper.fromMsg == "")
                        {
                            helper.onMsg.DynamicInvoke(client, buf);
                        }
                        else
                        {
                            helper.onMsg.DynamicInvoke(client, buf, cmd);
                        }
                    }
                }
            }
        }

        public void RemoveListener<TMsg>(string cmd, Action<Client, byte[], TMsg> onMsg)
        {
            if (m_dicMsgListener.ContainsKey(cmd))
            {
                m_dicMsgListener[cmd].onMsg = Delegate.Remove(m_dicMsgListener[cmd].onMsg, onMsg);
            }
        }


        public void RemoveListener(string cmd, Action<Client, byte[]> onMsg)
        {
            if (m_dicMsgListener.ContainsKey(cmd))
            {
                m_dicMsgListener[cmd].onMsg = Delegate.Remove(m_dicMsgListener[cmd].onMsg, onMsg);
            }
        }
    }
}
