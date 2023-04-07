using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System;
using System.Text;
using System.IO;
using ProtoBuf;
using SGF.Network.Core;
using SGF.Codec;
using UnityEngine.Events;
using Newtonsoft.Json;


namespace Net
{
    public class SocketManager
    {
        public HeartBeatHandler m_heart;

        //private static SocketManager _instance;
        //public static SocketManager Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            _instance = new SocketManager();


        //        }
        //        return _instance;
        //    }
        //}
        private string _currIP;
        private int _currPort;

        private bool _isConnected = false;
        public bool IsConnceted { get { return _isConnected; } }


        private Socket clientSocket = null;
        private Thread receiveThread = null;

        private DataBuffer _databuffer = new DataBuffer();

        byte[] _tmpReceiveBuff = new byte[4096];
        private sSocketData _socketData = new sSocketData();

        public UnityAction m_onConnectOk;
        public UnityAction m_onReconnectOk;

        public string m_name = "";

        public string m_ipPort;

        public bool m_isBreakLineReconnection = false;

        public bool m_isContecting = false;
        public void StartUp()
        {
#if UNITY_EDITOR
            //UnityEditor.EditorApplication.playmodeStateChanged += CloseContect;
            //UnityEditor.EditorApplication.playmodeStateChanged += CloseContect;
#endif
        }



        public void CloseContect()
        {
            //if (!_isConnected)
            //    return;

            //_isConnected = false;

            if (clientSocket != null)
            {
                _isConnected = false;
                //clientSocket.Disconnect(true);
                //clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                clientSocket = null;
                Debug.Log("关闭了socket:" + m_name);
            }


            if (receiveThread != null)
            {
                receiveThread.Abort();
                receiveThread = null;
            }

            //if (m_heart != null)
            //{
            //    StopHeart();
            //}
            //Debug.Log("关闭了socket的接收线程:" + m_name);

        }
        /// <summary>
        /// 断开
        /// </summary>
        private void _close()
        {
            //if (!_isConnected)
            //    return;

            //_isConnected = false;

            if (clientSocket != null)
            {
                _isConnected = false;
                //clientSocket.Disconnect(true);
                //clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                clientSocket = null;
                Debug.Log("关闭了socket:" + m_name);
            }
            

            if (receiveThread != null)
            {
                receiveThread.Abort();
                receiveThread = null;
            }

            //if (m_heart != null)
            //{
            //    StopHeart();
            //}
            //Debug.Log("关闭了socket的接收线程:" + m_name);
        }

        public void ReConnect()
        {
            _close();
            Connect(_currIP, _currPort);
        }

        /// <summary>
        /// 连接
        /// </summary>
        private void _onConnet()
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建套接字
                IPAddress ipAddress = IPAddress.Parse(_currIP);//解析IP地址
                IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, _currPort);
                IAsyncResult result = clientSocket.BeginConnect(ipEndpoint, new AsyncCallback(_onConnect_Sucess), clientSocket);//异步连接
                bool success = result.AsyncWaitHandle.WaitOne(5000, true);
                if (!success) //超时
                {
                    _onConnect_Outtime();
                }
            }
            catch (System.Exception _e)
            {
                _onConnect_Fail();
            }
        }

        private void _onConnect_Sucess(IAsyncResult iar)
        {
            try
            {
                Socket client = (Socket)iar.AsyncState;
                client.EndConnect(iar);

                receiveThread = new Thread(new ThreadStart(_onReceiveSocket));
                receiveThread.IsBackground = true;
                receiveThread.Start();
                _isConnected = true;
                
                m_isContecting = false;

                Loom.QueueOnMainThread((param) =>
                {
                    Debug.Log("连接成功:" + m_name);
                    if (m_onConnectOk != null)
                    {
                        m_onConnectOk();
                    }
                }, null);

                //if (m_heart != null && m_isBreakLineReconnection == true)
                //if (m_heart != null)
                //{
                //    //m_isBreakLineReconnection = false;
                //    Loom.QueueOnMainThread((param) =>
                //    {
                //        m_heart.RecontectOk();
                //        if (m_onReconnectOk != null)
                //        {
                //            m_onReconnectOk();
                //        }
                //    }, null);
                //}
            }
            catch (Exception _e)
            {
                Loom.QueueOnMainThread((param) =>
                {

                    Debug.Log(_e.ToString());
                }, null);
                Close();
            }
        }

        private void _onConnect_Outtime()
        {
            Debug.Log("连接超时：" + m_name);
            _close();
        }

        private void _onConnect_Fail()
        {
            Debug.Log("连接失败：" + m_name);
            _close();
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
                Debug.Log("send msg exception:" + e.StackTrace);
            }
        }

        /// <summary>
        /// 接受网络数据
        /// </summary>
        private void _onReceiveSocket()
        {
            while (true)
            {
                if (clientSocket == null)
                {
                    break;
                }

                if (clientSocket != null && !clientSocket.Connected)
                {
                    _isConnected = false;
                    //_ReConnect();
                    break;
                }
                try
                {
                    int receiveLength = clientSocket.Receive(_tmpReceiveBuff);
                    if (receiveLength > 0)
                    {
                        _databuffer.AddBuffer(_tmpReceiveBuff, receiveLength);//将收到的数据添加到缓存器中
                        while (_databuffer.GetData(out _socketData))//取出一条完整数据
                        {
                            sEvent_NetMessageData tmpNetMessageData = new sEvent_NetMessageData();
                            tmpNetMessageData._eventType = _socketData._protocallType;
                            tmpNetMessageData._eventData = _socketData._data;
                            tmpNetMessageData.m_key = _socketData.key;
                            //锁死消息中心消息队列，并添加数据
                            lock (MessageCenter.Instance._netMessageDataQueue)
                            {
                                //Debug.Log("Get Server:" + tmpNetMessageData.m_key);
                                MessageCenter.Instance._netMessageDataQueue.Enqueue(tmpNetMessageData);
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    //clientSocket.Disconnect(true);
                    //clientSocket.Shutdown(SocketShutdown.Both);
                    //clientSocket.Close();
                    break;
                }

                Thread.Sleep(100);
            }
        }





        /// <summary>
        /// 数据转网络结构
        /// </summary>
        /// <param name="_protocalType"></param>
        /// <param name="_data"></param>
        /// <returns></returns>
        private sSocketData BytesToSocketData(eProtocalCommand _protocalType, byte[] _data)
        {
            sSocketData tmpSocketData = new sSocketData();
            tmpSocketData._buffLength = Constants.HEAD_LEN + _data.Length;
            tmpSocketData._dataLength = _data.Length;
            tmpSocketData._protocallType = _protocalType;
            tmpSocketData._data = _data;
            return tmpSocketData;
        }

        /// <summary>
        /// 网络结构转数据
        /// </summary>
        /// <param name="tmpSocketData"></param>
        /// <returns></returns>
        private byte[] SocketDataToBytes(sSocketData tmpSocketData)
        {
            byte[] _tmpBuff = new byte[tmpSocketData._buffLength];
            byte[] _tmpBuffLength = BitConverter.GetBytes(tmpSocketData._buffLength);
            byte[] _tmpDataLenght = BitConverter.GetBytes((UInt16)tmpSocketData._protocallType);

            Array.Copy(_tmpBuffLength, 0, _tmpBuff, 0, Constants.HEAD_DATA_LEN);//缓存总长度
            Array.Copy(_tmpDataLenght, 0, _tmpBuff, Constants.HEAD_DATA_LEN, Constants.HEAD_TYPE_LEN);//协议类型
            Array.Copy(tmpSocketData._data, 0, _tmpBuff, Constants.HEAD_LEN, tmpSocketData._dataLength);//协议数据

            return _tmpBuff;
        }

        /// <summary>
        /// 合并协议，数据
        /// </summary>
        /// <param name="_protocalType"></param>
        /// <param name="_data"></param>
        /// <returns></returns>
        private byte[] DataToBytes(eProtocalCommand _protocalType, byte[] _data)
        {
            return SocketDataToBytes(BytesToSocketData(_protocalType, _data));
        }


        /// <summary>
        /// ProtoBuf序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ProtoBuf_Serializer(ProtoBuf.IExtensible data)
        {
            using (MemoryStream m = new MemoryStream())
            {
                byte[] buffer = null;
                Serializer.Serialize(m, data);
                m.Position = 0;
                int length = (int)m.Length;
                buffer = new byte[length];
                m.Read(buffer, 0, length);
                return buffer;
            }
        }

        /// <summary>
        /// ProtoBuf反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_data"></param>
        /// <returns></returns>
        public static T ProtoBuf_Deserialize<T>(byte[] _data)
        {
            using (MemoryStream m = new MemoryStream(_data))
            {
                return Serializer.Deserialize<T>(m);
            }
        }



        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="_currIP"></param>
        /// <param name="_currPort"></param>
        public void Connect(string _currIP, int _currPort)
        {
            if (!_isConnected)
            {
                this._currIP = _currIP;
                this._currPort = _currPort;
                _onConnet();
            }
        }

        /// <summary>
        /// 发送消息基本方法
        /// </summary>
        /// <param name="_protocalType"></param>
        /// <param name="_data"></param>
        private void SendMsgBase(eProtocalCommand _protocalType, byte[] _data)
        {
            if (_isConnected == false )
            {
                return;
            }

            //if (clientSocket == null || !clientSocket.Connected)
            //{
            //    ReConnect();
            //    return;
            //}

            byte[] _msgdata = DataToBytes(_protocalType, _data);
            clientSocket.BeginSend(_msgdata, 0, _msgdata.Length, SocketFlags.None, new AsyncCallback(_onSendMsg), clientSocket);
        }

        /// <summary>
        /// 以二进制方式发送
        /// </summary>
        /// <param name="_protocalType"></param>
        /// <param name="_byteStreamBuff"></param>
        public void SendMsg(eProtocalCommand _protocalType, ByteStreamBuff _byteStreamBuff)
        {
            if (_isConnected == false )
            {
                return;
            }
            SendMsgBase(_protocalType, _byteStreamBuff.ToArray());
        }

        /// <summary>
        /// 以ProtoBuf方式发送
        /// </summary>
        /// <param name="_protocalType"></param>
        /// <param name="data"></param>
        public void SendMsg(eProtocalCommand _protocalType, ProtoBuf.IExtensible data)
        {
            if (_isConnected == false )
            {
                return;
            }
            SendMsgBase(_protocalType, ProtoBuf_Serializer(data));
        }

        public void Close()
        {
            //Debug.Log()
            _close();
//#if UNITY_EDITOR
//            if (AppConst.IsOutEditor)
//            {
//                _close();
//            }
//#else
//            _close();
//#endif
        }

        public void ForceClose()
        {
            _isConnected = false;

            if (clientSocket != null)
            {
                clientSocket.Close();
                clientSocket = null;
            }

            if (receiveThread != null)
            {
                receiveThread.Abort();
                receiveThread = null;
            }
        }

        private void SendMsgBase(string key, byte[] _data)
        {
            //if (clientSocket == null || !clientSocket.Connected)
            //{
            //    _ReConnect();
            //    return;
            //}

            //byte[] _msgdata = DataToBytes(_protocalType, _data);
            //clientSocket.BeginSend(_msgdata, 0, _msgdata.Length, SocketFlags.None, new AsyncCallback(_onSendMsg), clientSocket);
        }

        public void SendMsgProtoVoid(string msgId)
        {
            if (_isConnected == false )
            {
                return;
            }
            NetMessage msg = new NetMessage();
            string[] bufMsgId = msgId.Split(',');
            msg.head.moduleId = short.Parse(bufMsgId[0]);
            msg.head.cmd = short.Parse(bufMsgId[1]);
            
            msg.content = new byte[1];
            msg.head.packetLength = 2 + 2 + msg.content.Length;
            SendMsg(msg);
            //string json = JsonConvert.SerializeObject(content);
            Debug.Log("ToServer:(" + msgId + ")" );
        }

        public void SendMsgProtoOld<T>(string msgId, T content)
        {

            NetMessage msg = new NetMessage();
            string[] bufMsgId = msgId.Split(',');
            msg.head.moduleId = short.Parse(bufMsgId[0]);
            msg.head.cmd = short.Parse(bufMsgId[1]);
            msg.content = PBSerializer.NSerialize(content);
            msg.head.packetLength = 2 + 2 + msg.content.Length;

            string json = JsonConvert.SerializeObject(content);
            Debug.Log("To " + m_name + "(" + msgId + "):" + json);
            if (clientSocket == null || clientSocket.Connected == false || _isConnected == false )
            {
                Debug.Log(m_name + ":socket被断开了");
                if (m_isContecting == false)
                {
                    m_isContecting = true;
                    ReConnect();
                }
                return;
            }
            SendMsg(msg);

        }

        public void SendMsgProto<T>(string msgId, T content)
        {
            if (_isConnected == false )
            {
                return;
            }
            NetMessage msg = new NetMessage();
            string[] bufMsgId = msgId.Split(',');
            msg.head.moduleId = short.Parse(bufMsgId[0]);
            msg.head.cmd = short.Parse(bufMsgId[1]);
            msg.content = PBSerializer.NSerialize(content);
            msg.head.packetLength = 2 + 2 + msg.content.Length;

            string json = JsonConvert.SerializeObject(content);
            //if (msgId != MsgIdDefine.RspPlayerSync)
            //{
                Debug.Log("To " + m_name + "(" + msgId + "):" + json);
            //}
            if (clientSocket == null || clientSocket.Connected == false || _isConnected == false )
            {
                Debug.Log(m_name + ":socket被断开了");
                if (m_isContecting == false)
                {
                    m_isContecting = true;
                    ReConnect();
                }
                return;
            }
            SendMsg(msg);
            
        }

        public void SendMsgOld(NetMessage netMsg)
        {
            byte[] tmp = null;
            int len = netMsg.Serialize(out tmp);
            byte[] buf1 = new byte[len];
            Array.Copy(tmp, buf1, len);
            string sbuf = "";
            for (int i = 0; i < buf1.Length; i++)
            {
                sbuf += buf1[i] + ",";
            }
            Debug.Log(sbuf);
            clientSocket.BeginSend(buf1, 0, buf1.Length, SocketFlags.None, new AsyncCallback(_onSendMsg), clientSocket);
        }

        public void SendMsg(NetMessage netMsg)
        {
            if (_isConnected == false )
            {
                return;
            }
            byte[] tmp = null;
            int len = netMsg.Serialize(out tmp);
            byte[] buf1 = new byte[len];
            Array.Copy(tmp, buf1, len);
            string sbuf = "";
            for (int i = 0; i < buf1.Length; i++)
            {
                sbuf += buf1[i] + ",";
            }
     
            clientSocket.BeginSend(buf1, 0, buf1.Length, SocketFlags.None, new AsyncCallback(_onSendMsg), clientSocket);
        }

        public void StartHeart()
        {
            m_heart = new HeartBeatHandler();
            //m_heart.m_reqHeartBeatMessage.accountId = (long)DataMgr.m_account.id;
            m_heart.Start();
            //m_heart.m_act = BreakLineReconnection;
        }

        public void StopHeart()
        {
            if (m_heart != null)
                m_heart.Stop();
        }

        public void RestartHeart()
        {
            m_heart.Start();
        }
    }
}