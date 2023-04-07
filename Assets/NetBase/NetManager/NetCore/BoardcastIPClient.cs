
using Framework.Pattern;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class BoardcastIPClient : SingletonMono<BoardcastIPClient>
{
    int port = 12345;
    private string strInfo;

    const string specialText = "MessageFormServerBroadCast&";

    bool serverIsRun = false;
    bool clientIsRun = false;

    Thread serverThread = null;
    UdpClient UdpSend = null;

    Thread clientThread = null;
    UdpClient UdpListen = null;

    float flowedime = 0;
    string m_serverIP = "";

    public UnityAction<string,int> m_onGetBoardcastIP;

    public string m_ipServer1 = "192.168.51.91";
    public string m_ipServer2 = "192.168.51.140";
    const bool m_isRelease = true;
    private void Start()
    {

    }

    public void StartBoardcastIPClient()//客户端收消息直到收到服务器的IP
    {
        strInfo = "Starting Client...";
        if (clientThread != null && clientThread.IsAlive) return;

        clientThread = new Thread(() =>
        {
            UdpListen = new UdpClient(new IPEndPoint(IPAddress.Any, port));

            while (clientIsRun)
            {
                Thread.Sleep(10);
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);
                byte[] bufRev = UdpListen.Receive(ref endpoint);//this method will block, Close() can stop it
                string msg = Encoding.Unicode.GetString(bufRev, 0, bufRev.Length);
                if (msg.Contains(specialText))
                {
                    if (m_isRelease == true)
                    {
                        string[] bufMsg = msg.Split('&');

                        int port = int.Parse(bufMsg[1]);

                        m_serverIP = endpoint.Address.ToString();

                        UdpListen.Close();
                        Loom.QueueOnMainThread((param) =>
                        {
                            StopClient();
                            Debug.Log("GetBoardcastIP:" + m_serverIP + "-" + port);
                            if (m_onGetBoardcastIP != null)
                            {
                                m_onGetBoardcastIP(m_serverIP, port);
                            }
                        }, null);

                        return;
                    }
                    else
                    {
                        string[] bufMsg = msg.Split('&');
                        int port = int.Parse(bufMsg[1]);

                        m_serverIP = endpoint.Address.ToString();


                        Loom.QueueOnMainThread((param) =>
                        {
                            if (IsIpInTwoServerMatch(m_serverIP))
                            {
                                UdpListen.Close();
                                StopClient();
                                Debug.Log("GetBoardcastIP:" + m_serverIP + "-" + port);
                                if (m_onGetBoardcastIP != null)
                                {
                                    m_onGetBoardcastIP(m_serverIP, port);
                                }
                            }
                        }, null);

                        return;
                    }
                }
                else
                {
                    strInfo = "I'm client, receive from ip:" + endpoint.Address.ToString() + ",but it no use!";
                }
            }

            UdpListen.Close();
        });

        clientThread.IsBackground = true;
        InitClient();
        clientThread.Start();
        strInfo = "Receiving...";
    }
    bool IsIpInTwoServerMatch(string ip)
    {
        //if (ip == m_ipServer1 && (PublicFunc.GetID() == FM059PublicFunc.m_player1 || PublicFunc.GetID() == FM059PublicFunc.m_player3))
        //{
        //    return true;
        //}
        //else if (ip == m_ipServer2 && (PublicFunc.GetID() == FM059PublicFunc.m_player2 || PublicFunc.GetID() == FM059PublicFunc.m_player4))
        //{
        //    return true;
        //}
        return false;
    }

    void InitClient()
    {
        clientIsRun = true;
        flowedime = 0f;
        if (UdpListen != null) UdpListen.Close();
    }

    void StopClient()
    {
        clientIsRun = false;
        Debug.Log("停止接受广播IP");
        if (UdpListen != null)
        {

            UdpListen.Close();
        }
        if (clientThread != null && clientThread.IsAlive) clientThread.Abort();
    }

    private void OnApplicationQuit()
    {
        if (clientThread != null)
        {
            //Debug.Log("停止clientThread");
            clientThread.Abort();
        }

        if (UdpListen != null)
        {
            //Debug.Log("停止UdpListen");
            UdpListen.Close();
        }
    }

}
