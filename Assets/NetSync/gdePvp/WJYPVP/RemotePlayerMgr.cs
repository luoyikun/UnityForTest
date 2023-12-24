using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoDefine;

/// <summary>
/// 远程玩家管理器，管理所有远程镜像
/// </summary>
public class RemotePlayerMgr : MonoBehaviour
{


    //存放远程玩家的信息，key为PlayerID号
    Dictionary<string, PDUProcessor> m_dicRemote = new Dictionary<string, PDUProcessor>();

    public GameObject m_remoteTmp;
    public static RemotePlayerMgr m_remotePlayerMgr = null;

    public bool m_isRecvMe = false;
    string m_myID = "";
    public static RemotePlayerMgr self
    {
        get
        {
            if (m_remotePlayerMgr == null)
            {
                GameObject obj = GameObject.Find("RemotePlayerMgr");
                m_remotePlayerMgr = obj.GetComponent<RemotePlayerMgr>();
            }
            return m_remotePlayerMgr;
        }
    }


    // Use this for initialization
    public void Start()
    {
        m_remoteTmp.SetActive(false);
        m_myID = PublicFunc.GetJsonString(Application.streamingAssetsPath + "/ID.txt");
        NetEventMgr.Instance.AddListener<PtPdu>(MsgIdDefine.RspPdu, OnRspPdu);
        NetEventMgr.Instance.AddListener<PtString>(MsgIdDefine.RspRoomAddOne, OnRspRoomAddOne);
        NetEventMgr.Instance.AddListener<RspRoomInfo>(MsgIdDefine.RspRoomInfo, OnRspRoomInfo);
        NetEventMgr.Instance.AddListener<PtString>(MsgIdDefine.RspRoomDeleOne, OnRspRoomDeleOne);
    }

    void OnRspRoomAddOne(PtString data)
    {
        if (m_isRecvMe == true)
        {
            AddOne(data.value);
        }
        else
        {
            if (data.value != m_myID)
            {
                AddOne(data.value);
            }
        }
    }

    void OnRspRoomDeleOne(PtString data)
    {
        if (m_dicRemote.ContainsKey(data.value))
        {
            GameObject.Destroy(m_dicRemote[data.value].gameObject);
            m_dicRemote.Remove(data.value);
        }
    }

    void OnRspRoomInfo(RspRoomInfo data)
    {
        for (int i = 0; i < data.listPlayer.Count; i++)
        {
            PlayerInfo player = data.listPlayer[i];
            if (m_isRecvMe == true)
            {
                AddOne(player.name);
            }
            else
            {
                if (player.name != m_myID)
                {
                    AddOne(player.name);
                }
            }
        }
    }

    /// <summary>
    /// 上线玩家，创建remote
    /// </summary>
    /// <param name="data"></param>
    public void AddOne(string data)
    {
        if (m_dicRemote.ContainsKey(data) == false)
        {
            GameObject remoteObj = Instantiate(m_remoteTmp);
            remoteObj.SetActive(true);
            remoteObj.name = "Remote" + data;
            PDUProcessor pro = remoteObj.GetComponent<PDUProcessor>();
            m_dicRemote[data] = pro;
        }

    }
    void OnRspPdu(PtPdu data)
    {
        PDURunner.PDU curPdu = new PDURunner.PDU();
        curPdu.UID = data.id;
        curPdu.type = (PDURunner.PDUType)data.type;
        curPdu.position.x = data.x;
        curPdu.position.y = data.y;
        curPdu.position.z = data.z;

        curPdu.forward.x = data.dirX;
        curPdu.forward.y = data.dirY;
        curPdu.forward.z = data.dirZ;

        curPdu.time = data.sendTime;
        curPdu.speed = data.speed;
        m_dicRemote[(curPdu.UID).ToString()].currentPDU = curPdu;
    }


    //接收服务器传来的PDU响应函数
    public void OnReceivePDU(Packet pkt)
    {
        PDURunner.PDU currentPDU = new PDURunner.PDU();

        pkt.to(ref currentPDU.UID);
        int type = 0;
        pkt.to(ref type);
        currentPDU.type = (PDURunner.PDUType)type;


        {
            pkt.to(ref currentPDU.position.x);
            pkt.to(ref currentPDU.position.y);
            pkt.to(ref currentPDU.position.z);
            pkt.to(ref currentPDU.forward.x);
            pkt.to(ref currentPDU.forward.y);
            pkt.to(ref currentPDU.forward.z);
            pkt.to(ref currentPDU.speed);
            pkt.to(ref currentPDU.time);
            pkt.to(ref currentPDU.anim);

        }
    }

}
