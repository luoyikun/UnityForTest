
using Newtonsoft.Json;
using ProtoDefine;
using SGF.Codec;
using SGF.Network.Core;
using System;
using System.Net.Sockets;

namespace MultiServer
{
    public class Client
    {
        private Server server;

        private MessageHandle msg = new MessageHandle();
        //常量
        public const int BUFFER_SIZE = 1024;
        //Socket
        public Socket clientSocket;
        //是否使用
        public bool isUse = false;
        //Buff
        public byte[] readBuff = new byte[BUFFER_SIZE];
        public int buffCount = 0;
        //沾包分包
        public byte[] lenBytes = new byte[sizeof(UInt32)];
        public Int32 msgLength = 0;
        //心跳时间
        public long lastTickTime = long.MinValue;


        private DataBuffer _databuffer = new DataBuffer();
        private sSocketData _socketData = new sSocketData();

        public PlayerInfo m_player = new PlayerInfo();

        public bool m_isLogin = false;
        public string m_id = "";
        public Client()
        {
            readBuff = new byte[BUFFER_SIZE];
        }
        public void Init(Socket socket, Server server,int id)
        {
            //数据初始化
            readBuff = new byte[BUFFER_SIZE];
            this.clientSocket = socket;
            this.server = server;
            isUse = true;
            buffCount = 0;
            m_player.id = id;
            m_player.name = id.ToString();

            lastTickTime = Sys.GetTimeStamp();
            //开始接收数据
            Start();
        }

        public void Start()
        {
            if (clientSocket == null || clientSocket.Connected == false) return;
            clientSocket.BeginReceive(readBuff, 0 , BUFFER_SIZE, SocketFlags.None, ReceiveCallback, null);
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                //if (clientSocket == null || clientSocket.Connected == false) return;
                //int count = clientSocket.EndReceive(ar);
                //if (count == 0)
                //{
                //    Console.WriteLine("收到 [" + GetAdress() + "] 断开链接");
                //    Close();
                //    return;
                //}
                //msg.ReadMessage(count);
                if (clientSocket == null || clientSocket.Connected == false) return;
                int receiveLength = clientSocket.EndReceive(ar);
                if (receiveLength == 0)
                {
                    Console.WriteLine("收到 [" + GetAdress() + "] 断开链接：receiveLength = 0");
                    Close();
                    return;
                }
                if (receiveLength > 0)
                {
                    _databuffer.AddBuffer(readBuff, receiveLength);//将收到的数据添加到缓存器中
                    while (_databuffer.GetData(out _socketData))//取出一条完整数据
                    {
                        sEvent_NetMessageData tmpNetMessageData = new sEvent_NetMessageData();
                        tmpNetMessageData._eventData = _socketData._data;
                        tmpNetMessageData.m_key = _socketData.key;
                        tmpNetMessageData.m_client = this;
                        //锁死消息中心消息队列，并添加数据
                        lock (MessageCenter.Instance._netMessageDataQueue)
                        {
                            //Debug.Log("Get Server:" + tmpNetMessageData.m_key);
                            MessageCenter.Instance._netMessageDataQueue.Enqueue(tmpNetMessageData);
                        }
                    }
                }

                Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("收到 [" + GetAdress() + "] 断开链接 " + e.Message);
                Close();
            }
        }
        //private void OnProcessMessage(RequestCode requestCode, ActionCode actionCode, string data)
        //{
        //    server.HandleRequest(requestCode, actionCode, data, this);
        //}
        //剩余的Buff
        public int BuffRemain()
        {
            return BUFFER_SIZE - buffCount;
        }
        //获取客户端地址
        public string GetAdress()
        {
            if (!isUse)
                return "无法获取地址";
            return clientSocket.RemoteEndPoint.ToString();
        }

        //public void Send(ActionCode actionCode, ReasonCode reasonCode, string data)
        //{
        //    try
        //    {
        //        byte[] bytes = MessageHandle.PackData(actionCode, reasonCode, data);
        //        clientSocket.Send(bytes);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("无法发送消息:" + e);
        //    }
        //}
        //关闭
        public void Close()
        {
            if (!isUse)
                return;
            Console.WriteLine("[断开链接]" + GetAdress());
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            isUse = false;
            KEvent.EventManager.GetInstance().DispatchEvent(KEvenet.EventType.ClientOutline, new EventDataEx<Client>(this));
            //Server.Instance.SendAllExceptByClient(this, MsgIdDefine.RspExit, m_player);
            //Server.Instance.RemoveClient(m_player.id);
        }


        public void SendMsgProto(string msgId)
        {
            NetMessage msg = new NetMessage();
            string[] bufMsgId = msgId.Split(',');
            msg.head.moduleId = short.Parse(bufMsgId[0]);
            msg.head.cmd = short.Parse(bufMsgId[1]);
            msg.content = PBSerializer.NSerialize(new VoidSend());
            msg.head.packetLength = 2 + 2 + msg.content.Length;

            string json = JsonConvert.SerializeObject(new VoidSend());
            Console.WriteLine("[发送]--" + m_player.id + "--" + msgId + "--" + json);
            if (clientSocket == null || clientSocket.Connected == false)
            {
                Console.WriteLine("[断开链接]" + GetAdress());
                return;
            }
            SendMsg(msg);
        }
        public void SendMsgProto<T>(string msgId, T content)
        {

            NetMessage msg = new NetMessage();
            string[] bufMsgId = msgId.Split(',');
            msg.head.moduleId = short.Parse(bufMsgId[0]);
            msg.head.cmd = short.Parse(bufMsgId[1]);
            msg.content = PBSerializer.NSerialize(content);
            msg.head.packetLength = 2 + 2 + msg.content.Length;

            if (msgId != MsgIdDefine.RspPlayerSync && msgId != MsgIdDefine.RspHeartBeat)
            {
                string json = JsonConvert.SerializeObject(content);
                Console.WriteLine("[发送]--" + m_player.id + "--" + msgId + "--" + json);
            }
            if (clientSocket == null || clientSocket.Connected == false)
            {
                Console.WriteLine("[断开链接]" + GetAdress());
                return;
            }
            SendMsg(msg);

        }

        public void SendMsg(NetMessage netMsg)
        {
            byte[] tmp = null;
            int len = netMsg.Serialize(out tmp);
            byte[] buf1 = new byte[len];
            Array.Copy(tmp, buf1, len);

            clientSocket.BeginSend(buf1, 0, buf1.Length, SocketFlags.None, new AsyncCallback(_onSendMsg), clientSocket);
        }

        public void SendByteBuf(string msgId, byte[] buf)
        {
            NetMessage msg = new NetMessage();
            string[] bufMsgId = msgId.Split(',');
            msg.head.moduleId = short.Parse(bufMsgId[0]);
            msg.head.cmd = short.Parse(bufMsgId[1]);
            msg.content = buf;
            msg.head.packetLength = 2 + 2 + msg.content.Length;

            if (clientSocket == null || clientSocket.Connected == false)
            {
                Console.WriteLine("[断开链接]" + GetAdress());
                return;
            }
            SendMsg(msg);
        }

        /// <summary>
        /// 发送消息结果回掉，可判断当前网络状态
        /// </summary>
        /// <param name="asyncSend"></param>
        private void _onSendMsg(IAsyncResult asyncSend)
        {
            try
            {
                Socket client = (Socket)asyncSend.AsyncState;
                client.EndSend(asyncSend);
            }
            catch (Exception e)
            {
                Console.WriteLine("[发送失败]" + GetAdress() + e.StackTrace);
            }
        }

    }
}
