using UnityEngine;
using System.Collections;
using ProtoDefine;

public class WJYPDUSender : PDUSender {

    public uint m_uid = 100;
    PtPdu m_netPdu = new PtPdu();
   // Use this for initialization
   void Start () {
	
	}

    public override bool syncSkill(uint myID, int skillID)
	{
		//Packet pkt = new Packet ();
		//Vector3 vPos = PlayerSelf.self.Agent.gameObject.transform.position;
		//Vector3 vForward = PlayerSelf.self.Agent.gameObject.transform.forward;

		//pkt = pkt < myID < 999 < skillID < vPos.x < vPos.y < vPos.z < vForward.x < vForward.y < vForward.z;
		//Server.self.SendMessage(2557, ref pkt);
        
		return false;
	}

    public override bool syncAttack(int iCurAttackStep)
    {
        //Packet pkt = new Packet();
        //Vector3 vPos = PlayerSelf.self.Agent.gameObject.transform.position;
        //Vector3 vForward = PlayerSelf.self.Agent.gameObject.transform.forward;
        //uint myID = PlayerSelf.self.m_PlayerInfo.m_Attributes.PlayerID;
        //pkt = pkt < myID < 998 < iCurAttackStep < vPos.x < vPos.y < vPos.z < vForward.x < vForward.y < vForward.z;
        //Server.self.SendMessage(2557, ref pkt);
        return false;
    }

    //向服务器发送PDU
    public override bool sendPDU(PDURunner.PDU pdu)
    {
        if (InitPVP_WJY.m_isNet == false)
        {
            pdu.UID = m_uid;
            Packet pkt = new Packet();
            float sendTime = (float)pdu.time;
            pkt = pkt + pdu.UID + (int)pdu.type + pdu.position.x + pdu.position.y + pdu.position.z + pdu.forward.x + pdu.forward.y + pdu.forward.z + pdu.speed +
                sendTime + pdu.anim;
            //自发自接收，需要把位置为0
            pkt.posSet(0);
            //发送给服务器，这里暂时模拟，直接发给远程玩家管理器
            RemotePlayerMgr.self.OnReceivePDU(pkt);
        }
        else
        {
            m_netPdu.id = m_uid;
            m_netPdu.type = (byte)pdu.type;
            m_netPdu.x = pdu.position.x;
            m_netPdu.y = pdu.position.y;
            m_netPdu.z = pdu.position.z;

            m_netPdu.dirX = pdu.forward.x;
            m_netPdu.dirY = pdu.forward.y;
            m_netPdu.dirZ = pdu.forward.z;

            m_netPdu.sendTime = pdu.time;
            m_netPdu.speed = pdu.speed;
            GameSocket.Instance.SendMsgProto<PtPdu>(MsgIdDefine.RspPdu, m_netPdu);
        }
        return true;
    }

    //向服务器发送我进入战场时，我的信息
    public override bool sendCreatePlayer()
    {
        //Packet pkt = new Packet();
        //pkt = pkt < PlayerSelf.self.m_PlayerInfo.m_Attributes < PlayerSelf.self.m_PlayerInfo.m_CurSkill < PlayerSelf.self.m_PlayerInfo.m_CurEquip < PlayerSelf.self.m_FightAttr < PlayerSelf.self.m_Skills.Count;

        //foreach (obj_skill skill in PlayerSelf.self.m_Skills)
        //{
        //    pkt = pkt < skill;
        //}
        //Server.self.SendMessage(2555, ref pkt);
        //GameObject.FindObjectOfType<InitPVP_WJY>().OnCreatePlayer(ref pkt);

        return true;
    }

    //向服务器发送流血
    public override bool sendInjuryBloodByAttack(bool bBigBlood, Vector3 pot, Vector3 dir)
    {
        //Packet pkt = new Packet(); 
        //uint myID = PlayerSelf.self.m_PlayerInfo.m_Attributes.PlayerID;
        //float hp = PlayerSelf.self.Agent.CurHealth;//当前的血量
        //pkt = pkt < myID < 997 < bBigBlood < pot.x < pot.y < pot.z < dir.x < dir.y < dir.z < hp;
        //Server.self.SendMessage(2557, ref pkt);
        return false;
    } 

    //向服务器发送受技能攻击流血
    public override bool sendInjuryBloodBySkill(int iEffetID)
    {
        //Packet pkt = new Packet();
        //uint myID = PlayerSelf.self.m_PlayerInfo.m_Attributes.PlayerID;
        //float hp = PlayerSelf.self.Agent.CurHealth;//当前的血量
        //pkt = pkt < myID < 996 < iEffetID < hp;
        //Server.self.SendMessage(2557, ref pkt);
        return false;
    } 
	

}
