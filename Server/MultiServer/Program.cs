using MultiServer.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiServer
{
    class Program
    {
        private static byte[] result = new byte[1024];
        static Socket serverSocket;
        static Thread myThread;

        public static int m_port = 12306;
        public static int m_portRecvOther = 12307;//控制软件发来的端口号
        static void Main(string[] args)
        {
            //UdpReceiver.Instance.StartUdpClient(m_portRecvOther);//udp 用于嵌入式连接
            BroadcastIP.Instance.StartIpServer(m_port);
            //Start();
            Server.Instance.Start(m_port);

            MessageCenter.Instance.StartUp();

            RoomMgr.Instance.StartUp();



            Thread myThread = new Thread(MsgThread);
            myThread.Start();
            //HideSelf();
            while (true)
            {
                Console.ReadKey(false);
            }
            
        }

        static void HideSelf()
        {
            Console.Title = "MultiServer";
            IntPtr ParenthWnd = new IntPtr(0);
            IntPtr et = new IntPtr(0);
            ParenthWnd = FindWindow(null, "MultiServer");

            ShowWindow(ParenthWnd, 0);//隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化



        }
        static void MsgThread()
        {
            while (true)
            {
                MessageCenter.Instance.Update();
                Thread.Sleep(100);
            }
        }


        static void Start()
        {
            //服务器IP地址  
            string sIp = PublicFunc.GetHostAddress();
            IPAddress ip = IPAddress.Parse(sIp);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, m_port));  //绑定IP地址：端口  
            serverSocket.Listen(10);    //设定最多10个排队连接请求  
                                        //通过Clientsoket发送数据  
            myThread = new Thread(ListenClientConnect);
            myThread.Start();
            Console.ReadLine();
        }

        /// <summary>  
        /// 监听客户端连接  
        /// </summary>  
        static void ListenClientConnect()
        {
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start(clientSocket);
                //Debug.Log(clientSocket.RemoteEndPoint.ToString());
            }
        }

        /// <summary>  
        /// 接收消息
        /// </summary>  
        /// <param name="clientSocket"></param>  
        static void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    int receiveNumber = myClientSocket.Receive(result);
                    if (receiveNumber > 0)
                    {
                        myClientSocket.Send(result, receiveNumber, 0);
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);  
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }


        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]   //找子窗体   
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]   //用于发送信息给窗体   
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport("User32.dll", EntryPoint = "ShowWindow")]   //
        private static extern bool ShowWindow(IntPtr hWnd, int type);

        

    }

}
