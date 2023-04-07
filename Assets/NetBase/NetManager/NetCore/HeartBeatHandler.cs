

using ProtoDefine;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace Net
{
    public class HeartBeatHandler
    {
        private float m_lastHeartBeatTime = 0;
        private Stopwatch m_watch = new Stopwatch();
        public ReqHeartBeatMessage m_reqHeartBeatMessage = new ReqHeartBeatMessage();
        public UnityAction m_act;
        public bool m_isOnline = true;
        
        public void Start()
        {
            m_isOnline = true;
            //NetEventManager.Instance.AddEventListener(MsgIdDefine.RspHeartBeatMessage, OnNetEvHeart);
            //NetEventMgr.Instance.AddListener(MsgIdDefine.RspHeart, OnNetEvHeart);
            MessageCenter.Instance.m_onUpdate += OnUpdate;
        }

        public void RecontectOk()
        {
            m_watch.Stop();
            m_watch.Reset();
            m_watch.Start();

            m_isOnline = true;
            //m_lastHeartBeatTime = Time.time;
        }
        public void Stop()
        {
            MessageCenter.Instance.m_onUpdate -= OnUpdate;
            //NetEventMgr.Instance.RemoveListener(MsgIdDefine.RspHeart, OnNetEvHeart);

            //NetEventManager.Instance.RemoveEventListener(MsgIdDefine.RspHeartBeatMessage, OnNetEvHeart);
        }


        private void OnUpdate()
        {
            //if (Application.platform == RuntimePlatform.WindowsEditor)
            //{
            //    return;
            //}
            if (m_watch.ElapsedMilliseconds > 20000)
            {
                m_watch.Stop();
                m_watch.Reset();
                m_watch.Start();
                if (m_act != null)
                {
                    UnityEngine.Debug.Log("心跳断线重连");
                    m_act();
                }
                m_isOnline = false;
            }

            float current = Time.time;
            if (current - m_lastHeartBeatTime > 10.0f && m_isOnline == true)
            {
                m_lastHeartBeatTime = current;

                //GameSocket.Instance.SendMsgProto(MsgIdDefine.ReqHeartBeatMessage, m_reqHeartBeatMessage);
                //NetManager.Instance.SendMsgProto<ReqHeartBeatMessage>(MsgIdDefine.ReqHeartBeatMessage,m_reqHeartBeatMessage);
                //UnityEngine.Debug.Log("Heart");
                m_watch.Stop();
                m_watch.Reset();
                m_watch.Start();
            }
        }

        public void OnNetEvHeart(byte[] buf)
        {
            UnityEngine.Debug.Log("GetHeart");
            m_watch.Stop();
            m_watch.Reset();
            m_watch.Start();
        }

        public void OnNetEvHeart()
        {
            UnityEngine.Debug.Log("GetHeart");
            m_watch.Stop();
            m_watch.Reset();
            m_watch.Start();
        }

    }
}