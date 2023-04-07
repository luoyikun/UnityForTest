using Framework.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiServer.Work
{
    class UdpReceiver : Singleton<UdpReceiver>
    {
        //网口、端口号只绑定一次
        private static bool isOpen = false;          //网口、端口号只绑定一次
        private static Socket udpClient;
        private static Thread receiveThread;
        //网口数据接收线程
        private EndPoint receiveIpEndPoint;

        int m_port;

        //发送端相关
        private UdpClient m_sendUdp = new UdpClient();
        string m_sendIp = "200.103.5.211";
        int m_sendPort = 30001;
        public void StartUdpClient(int prot)
        {
            if (isOpen == false)
            {
                m_port = prot;
                udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);   //创建套接字
                udpClient.Bind(new IPEndPoint(IPAddress.Any, prot));               //绑定本地ip和端口号
                receiveIpEndPoint = new IPEndPoint(IPAddress.Any, 0);                                     //确认可接收的Ip和端口号
                receiveThread = new Thread(ReceiveNetData);
                receiveThread.Start();                                                                    //开启接收线程
                isOpen = true;
            }
        }

        private void ReceiveNetData()
        {
            while (true)
            {
                byte[] data = new byte[1024];
                int length = udpClient.ReceiveFrom(data, ref receiveIpEndPoint);
                //if (data[0] != 0x04 && data[0] != 0x46)
                //    print(DateTime.Now + " : " + BitConverter.ToString(data, 0, length));

                ByteBufDeal(data, length);
            }
        }

        public void Stop()
        {
            if (udpClient != null)
            {
                udpClient.Close();
            }
            if (receiveThread != null)
            {
                receiveThread.Abort();
            }
            isOpen = false;
        }

        void ByteBufDeal(byte[] bytes, int len)
        {
            if (bytes == null)
            {
                return;
            }
            if (bytes.Length > 0)
            {
                byte[] data = new byte[len];
                Array.Copy(bytes, data, len);
                string str = Encoding.Default.GetString(data);

                RoomMgr.Instance.SendTask(str);
            }
        }

        public void Send2Other(byte[] bytes)
        {
            //发送信息
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(m_sendIp), m_sendPort);
            m_sendUdp.Send(bytes, bytes.Length, ipEndPoint);
        }
    }
}
