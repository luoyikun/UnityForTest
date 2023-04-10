using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoDefine;

/// <summary>
/// 远程玩家管理器，管理所有远程镜像
/// </summary>
public class RemotePlayerMgr : MonoBehaviour {


    //存放远程玩家的信息，key为PlayerID号
    Dictionary<uint, GameObject> remotePlayer = new Dictionary<uint, GameObject>();

    public GameObject m_oneRemote;
    public static RemotePlayerMgr m_remotePlayerMgr = null;

    public List<GameObject> m_listRemote = new List<GameObject>();

    void ListRemote2Dic()
    {
        for (int i = 0; i < m_listRemote.Count; i++)
        {
            uint id = m_listRemote[i].GetComponent<PDUProcessor>().m_id;
            remotePlayer[id] = m_listRemote[i];
        }
    }
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
	public void Start () {
        
        ListRemote2Dic();
        NetEventMgr.Instance.AddListener<PtPdu>(MsgIdDefine.RspPdu, OnRspPdu);
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
        remotePlayer[curPdu.UID].GetComponent<PDUProcessor>().currentPDU = curPdu;
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


            if (InitPVP_WJY.m_isNet == false)
            {
                m_oneRemote.GetComponent<PDUProcessor>().currentPDU = currentPDU;
            }
            else
            {

            }

        }
    }
    IEnumerator delayForReceivePDU(float fTime, PDURunner.PDU curPDU)
    {
        yield return new WaitForSeconds(fTime);
        //remotePlayer[curPDU.UID].GetComponent<PDUProcessor>().currentPDU = curPDU;
        m_oneRemote.GetComponent<PDUProcessor>().currentPDU = curPDU;
    }

}
