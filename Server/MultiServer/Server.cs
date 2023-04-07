using Framework.Pattern;
using ProtoDefine;
using SGF.Codec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MultiServer
{
    public class Server : Singleton<Server>
    {
        private IPEndPoint ipEndPoint;
        private Socket serverSocket;
        //连接的客户端列表
        private Client[] clientConn;
        public Dictionary<int, Client> m_dicClient = new Dictionary<int, Client>();
        //最大链接数
        public int maxConn = 50;

        //主定时器
        System.Timers.Timer timer = new System.Timers.Timer(1000);
        //心跳时间
        public long heartBeatTime = 20;

        public override void Init()
        {

            //NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqHeartBeat, OnNetHeartBeat);
            
        }

        void OnNetHeartBeat(Client client, byte[] buf)
        {
            HeartBeat info = PBSerializer.NDeserialize<HeartBeat>(buf);
            if (m_dicClient.ContainsKey(info.id))
            {
                m_dicClient[info.id].lastTickTime = Sys.GetTimeStamp();
            }
        }


        public void StartHeart()
        {
            //定时器
            timer.Elapsed += new System.Timers.ElapsedEventHandler(HandleMainTimer);
            timer.AutoReset = false;
            timer.Enabled = true;
        }

        //主定时器
        public void HandleMainTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            //处理心跳
            HeartBeat();
            timer.Start();
        }


        //心跳
        public void HeartBeat()
        {
            //Console.WriteLine ("[主定时器执行]");
            long timeNow = Sys.GetTimeStamp();

            foreach (var item in m_dicClient)
            {
                Client conn = item.Value;
                if (conn == null) continue;
                if (!conn.isUse) continue;

                if (conn.lastTickTime < timeNow - heartBeatTime)
                {
                    Console.WriteLine("[心跳引起断开连接]" + conn.GetAdress());
                    lock (conn)
                        conn.Close();
                }
            }
        }


        //开启服务器
        public void Start(int port)
        {
            //StartHeart();

            //链接池
            clientConn = new Client[maxConn];
            for (int i = 0; i < maxConn; i++)
            {
                clientConn[i] = new Client();
            }
            //服务器IP地址  
            string sIp = PublicFunc.GetHostAddress();
            IPAddress ipAdd = IPAddress.Parse(sIp);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ipAdd, port)); //绑定IP与端口号
            serverSocket.Listen(maxConn);

            serverSocket.BeginAccept(AcceptCB, null); //开始监听客户端的连接
            Console.WriteLine("[服务器]启动成功!");
            Console.WriteLine(string.Format("IP:{0}--port:{1}",sIp, port));
        }

        private void AcceptCB(IAsyncResult ar)
        {
            try
            {
                Socket socket = serverSocket.EndAccept(ar);
                int index = NewIndex();

                if (index < 0)
                {
                    socket.Close();
                    Console.WriteLine("[AcceptCB警告]链接已满");
                }
                else
                {
                    Client client = clientConn[index];
                    client.Init(socket, this,index);
                    Console.WriteLine("客户端连接 [" + client.GetAdress() + "] conn池ID：" + index);
                    lock (m_dicClient)
                    {
                        m_dicClient[index] = client;
                    }
                }
                serverSocket.BeginAccept(AcceptCB, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("AcceptCb失败:" + e.Message);
            }
        }
        //获取链接池索引，返回负数表示获取失败
        public int NewIndex()
        {
            if (clientConn == null)
                return -1;
            for (int i = 0; i < clientConn.Length; i++)
            {
                if (clientConn[i] == null)
                {
                    clientConn[i] = new Client();
                    return i;
                }
                else if (clientConn[i].isUse == false)
                {
                    return i;
                }
            }
            return -1;
        }

        //发送全部人
        public void SendAll<T>(string key,T content)
        {
            lock(m_dicClient)
            { foreach (var item in m_dicClient)
                {
                    item.Value.SendMsgProto<T>(key, content);
                }
            }
           
        }

        //发送全部人
        public void SendAllVoid(string key)
        {
            lock (m_dicClient)
            {
                foreach (var item in m_dicClient)
                {
                    item.Value.SendMsgProto(key, new VoidSend());
                }
            }
        }

        //发送全部人除了某人by id
        public void SendAllExceptByID<T>(int id,string key, T content)
        {
            lock (m_dicClient)
            {
                foreach (var item in m_dicClient)
                {
                    if (item.Key != id)
                    {
                        item.Value.SendMsgProto<T>(key, content);
                    }
                }
            }
        }

        //发送全部人除了某人by client
        public void SendAllExceptByClient<T>(Client client, string key, T content)
        {
            lock (m_dicClient)
            {
                foreach (var item in m_dicClient)
                {
                    if (item.Value != client)
                    {
                        item.Value.SendMsgProto<T>(key, content);
                    }
                }
            }
        }

        public void RemoveClient(int id)
        {
            m_dicClient.Remove(id);
        }
        //发送全部人除了某人by client 原始字节
        public void SendByteBufExceptClient(Client client, string key, byte[] buf)
        {
            lock (m_dicClient)
            {
                foreach (var item in m_dicClient)
                {
                    if (item.Value != client)
                    {
                        item.Value.SendByteBuf(key, buf);
                    }
                }
            }
        }

        public void SendAllByteBuf(string key, byte[] buf)
        {
            lock (m_dicClient)
            {
                foreach (var item in m_dicClient)
                { 
                    item.Value.SendByteBuf(key, buf);
                }
            }
        }

        public void SendAllExceptByClient(Client client, string key)
        {
            lock (m_dicClient)
            {
                foreach (var item in m_dicClient)
                {
                    if (item.Value != client)
                    {
                        item.Value.SendMsgProto(key, new VoidSend());
                    }
                }
            }
        }

        public void SendAllByteBuf(string key, byte[] buf, Client client = null)
        {
            foreach (var item in m_dicClient)
            {
                if (item.Value != client)
                {
                    if (item.Value.isUse == true)
                        item.Value.SendByteBuf(key, buf);
                }
            }
        }
    }
}
