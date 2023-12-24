using Framework.Pattern;
using ProtoDefine;
using SGF.Codec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiServer
{
    class BroadcastIP : Singleton<BroadcastIP>
    {
        Thread serverThread = null;
        UdpClient UdpSend = null;
        bool serverIsRun = true;
        string specialText = "MessageFormServerBroadCast&";
        int m_port = 12345;
        
        public void StartIpServer(int port)//服务器一直发消息
        {
            string strInfo = "Starting Server...";
            if (serverThread != null && serverThread.IsAlive) return;

            serverThread = new Thread(() =>
            {
                UdpSend = new UdpClient();

                while (serverIsRun)
                {
                    Thread.Sleep(5000);
                    

                    string sIp = PublicFunc.GetHostAddress();
                    //IPAddress ip = IPAddress.Parse(sIp);
                    byte[] buf = Encoding.Unicode.GetBytes(specialText + sIp + "&"+  port);
                    UdpSend.Send(buf, buf.Length, new IPEndPoint(IPAddress.Broadcast, m_port));
                    //UdpSend.Send(buf, buf.Length, new IPEndPoint(ip, Program.port));
                }

                //UdpSend.Close();
            });

            serverThread.IsBackground = true;
            serverIsRun = true;
            serverThread.Start();
            strInfo = "Server Started";
        }


        public void StopServer()
        {
            serverIsRun = false;
            if (UdpSend != null) UdpSend.Close();
            if (serverThread != null && serverThread.IsAlive) serverThread.Abort();
        }

    }
}
