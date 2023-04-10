using Framework.Pattern;
using ProtoDefine;
using SGF.Codec;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace MultiServer.Work
{
    class RoomMgr: Singleton<RoomMgr>
    {
        //房间内所有的客户端 
        Dictionary<string, Client> m_dicClient = new Dictionary<string, Client>(); //后续这种初始化都放到init 或者构造中，防止未使用而分配的内存

        Timer m_time = null;
        long m_serverSec = 0;
        //只转发的消息
        string[] m_listTranspond = new string[] { MsgIdDefine.RspGrab,MsgIdDefine.RspRelease,MsgIdDefine.RspUse,MsgIdDefine.RspGrabHandChange,
            MsgIdDefine.RspRemoteEvent,MsgIdDefine.RspMechanism,MsgIdDefine.RspRotatorChange,MsgIdDefine.RspBelong,MsgIdDefine.RspQianJingDing,
            MsgIdDefine.RspDuiZhong,MsgIdDefine.RspUpdateTask,MsgIdDefine.RspAllExit,MsgIdDefine.RspDiaoDanFixLen,MsgIdDefine.RspDiaoDanRotate,
            MsgIdDefine.RspGongZhuoDengOpen,MsgIdDefine.RspMeDriverChangeState,MsgIdDefine.RspBengQiDong,MsgIdDefine.RspTuiDanQiMoveStateChange,
            MsgIdDefine.RspPlaySound,
        };

        //发送给全部人的消息
        string[] m_listSendAll = new string[] { MsgIdDefine.RspReturnRoom,MsgIdDefine.RspPdu};

        //List<string> m_listPlayerHoldPaoDan = new List<string>();

        //构造函数时，就会注册消息
        public override void Init()
        {
            TimeInit();
            KEvent.EventManager.GetInstance().AddEventListener(KEvenet.EventType.ClientOutline, OnEvClientOutline);
            NetEventMgr.Instance.AddListener<PtString>(MsgIdDefine.ReqID, OnNetReqID);
            NetEventMgr.Instance.AddListener(MsgIdDefine.ReqStart, OnNetStart);
            NetEventMgr.Instance.AddListener(MsgIdDefine.ReqChangeName, OnNetChangeName);
            NetEventMgr.Instance.AddListener(MsgIdDefine.ReqRoomInfo, OnNetRoomInfo);
            NetEventMgr.Instance.AddListener(MsgIdDefine.ReqReturnMain, OnNetReturnMain);
            NetEventMgr.Instance.AddListener(MsgIdDefine.RspPlayerSync, OnNetPlayerSync);
            NetEventMgr.Instance.AddListener<PtString>(MsgIdDefine.RspRestart, OnNetRestart);
            NetEventMgr.Instance.AddListener(MsgIdDefine.ReqHeartBeat, OnNetReqHeartBeat);
            //NetEventMgr.Instance.AddListener<PtString>(MsgIdDefine.RspPlayerTriggerEnterPaoDan, OnNetPlayerTriggerEnterPaoDan);
            //NetEventMgr.Instance.AddListener<PtString>(MsgIdDefine.RspPlayerTriggerExitPaoDan, OnNetPlayerTriggerExitPaoDan);
            for (int i = 0; i < m_listTranspond.Length; i++)
            {
                NetEventMgr.Instance.AddListener(m_listTranspond[i], OnTranspond);
            }

            for (int i = 0; i < m_listSendAll.Length; i++)
            {
                NetEventMgr.Instance.AddListener(m_listSendAll[i], OnSendAll);
            }
        }

        void TimeInit()
        {
            m_time = new Timer(1000);
            m_time.Elapsed += TimeElapsed;
            m_time.Enabled = true;
            m_time.Start();
        }

        void TimeElapsed(Object source, ElapsedEventArgs e)
        {
            m_serverSec += 1;
        }
        //void OnNetPlayerTriggerEnterPaoDan(Client client, byte[] buf, PtString data)
        //{

        //}

        //void OnNetPlayerTriggerExitPaoDan(Client client, byte[] buf, PtString data)
        //{

        //}

        void OnNetRestart(Client client, byte[] buf, PtString data)
        {
            SendAllByteBuf(MsgIdDefine.RspRestart, buf, client.m_player.name);
            if (data.value == "训练完成")
            {
                UdpReceiver.Instance.Send2Other(new byte[] { 0x01 });
            }
            else {
                UdpReceiver.Instance.Send2Other(new byte[] { 0x00 });
            }
        }
        void OnSendAll(Client client, byte[] buf, string msgID)
        {
            SendAllByteBuf(msgID, buf);
        }
        void OnEvClientOutline(EventData data)
        {
            lock (m_dicClient)
            {
                var exdata = data as EventDataEx<Client>;
                Client client = exdata.GetData();
                if (m_dicClient.ContainsKey(client.m_player.name))
                {
                    m_dicClient.Remove(client.m_player.name);

                    PtString sendData = new PtString();
                    sendData.value = client.m_player.name;
                    foreach (var item in m_dicClient)
                    {
                        item.Value.SendMsgProto(MsgIdDefine.RspRoomDeleOne, sendData);
                    }
                }
            }
        }

        void OnNetPlayerSync(Client client, byte[] buf)
        {
            //SendAllByteBuf(MsgIdDefine.RspPlayerSync,buf);
            SendAllByteBuf(MsgIdDefine.RspPlayerSync, buf, client.m_player.name);
        }

        /// <summary>
        /// 接到客户端请求时间的信息，把当前时间s返回
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buf"></param>
        void OnNetReqHeartBeat(Client client, byte[] buf)
        {
            PtLong data = new PtLong();
            data.value = m_serverSec;
            client.SendMsgProto<PtLong>(MsgIdDefine.RspHeartBeat, data);
        }

        //某条消息转发给房间内的其他人
        void OnTranspond(Client client, byte[] buf, string msgID)
        {
            SendAllByteBuf(msgID, buf, client.m_player.name);
        }

        void OnNetChangeName(Client client, byte[] buf)
        {
            PlayerInfo info = PBSerializer.NDeserialize<PlayerInfo>(buf);
            Server.Instance.m_dicClient[info.id].m_player.name = info.name;
            Server.Instance.SendByteBufExceptClient(client, MsgIdDefine.RspChangeName, buf);
        }

        void OnNetRoomInfo(Client client, byte[] buf)
        {
            RspRoomInfo rspRoom = new RspRoomInfo();
            foreach (var item in m_dicClient)
            {
                rspRoom.listPlayer.Add(item.Value.m_player);

            }
            client.SendMsgProto(MsgIdDefine.RspRoomInfo, rspRoom);
        }

        void OnNetReqID(Client client,byte[] buf,PtString data)
        {
            client.m_player.name = data.value;

            //向房间内其他人发送新增加一人
            PlayerInfo addOne = client.m_player;
            foreach (var item in m_dicClient)
            {
                item.Value.SendMsgProto(MsgIdDefine.RspRoomAddOne, addOne);
            }

            //加入到房间列表中
            m_dicClient[data.value] = client;


            //向新来的发送全部人
            RspRoomInfo rspRoom = new RspRoomInfo();
            foreach (var item in m_dicClient)
            {
                rspRoom.listPlayer.Add(item.Value.m_player);

            }
            client.SendMsgProto(MsgIdDefine.RspRoomInfo, rspRoom);


        }

        void OnNetStart(Client client, byte[] buf)
        {

            SendAllByteBuf(MsgIdDefine.RspStart, buf);
        }

        void OnNetReturnMain(Client client, byte[] buf)
        {
            Server.Instance.SendAllByteBuf(MsgIdDefine.RspReturnMain, buf);
        }

        public void SendAllByteBuf(string msgID, byte[] buf, string clientName = "")
        {
            lock (m_dicClient)
            {
                foreach (var item in m_dicClient)
                {
                    if (item.Key != clientName)
                    {
                        if (item.Value.isUse == true)
                            item.Value.SendByteBuf(msgID, buf);
                    }
                }
            }
        }

        /// <summary>
        /// 得到任务，发送给所有连接的人
        /// </summary>
        /// <param name="task"></param>
        public void SendTask(string task)
        {
            PtString data = new PtString();
            data.value = task;
            foreach (var item in m_dicClient)
            {
                item.Value.SendMsgProto(MsgIdDefine.RspGetTask, data);
            }
        }
    }
}