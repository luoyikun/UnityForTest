
using ProtoDefine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Net
{
    public class RoomMgr : MonoBehaviour
    {
        string m_serverIp;
        int m_serverPort;
        // Start is called before the first frame update
        void Start()
        {

            MessageCenter.Instance.StartUp();
            Loom.Current.StarUp();

            BoardcastIPClient.Instance.StartBoardcastIPClient();
            BoardcastIPClient.Instance.m_onGetBoardcastIP += ConnectServer;

            
        }

        
        private void ConnectServer(string ip, int port)
        {
            m_serverIp = ip;
            m_serverPort = port;
            GameSocket.Instance.Close();
            GameSocket.Instance.Connect(ip, port);
            GameSocket.Instance.m_onConnectOk += ConnectOk2S;
        }

        //连接服务器成功，向服务器发送自己的名字，读配置文件得到
        void ConnectOk2S()
        {
            PtString data = new PtString();
            string sID = PublicFunc.GetJsonString(Application.streamingAssetsPath + "/ID.txt");
            data.value = sID;
            GameSocket.Instance.SendMsgProto(MsgIdDefine.ReqID, data);
            //Heart heart = new Heart();
            //heart.accountId = "123";
            //GameSocket.Instance.SendMsgProto(MsgIdDefine.ReqHeart, heart);
        }

        private void OnApplicationQuit()
        {
            GameSocket.Instance.CloseContect();
        }

    }
}
