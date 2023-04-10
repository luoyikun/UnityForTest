//#define UseDelayReceive

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoDefine;

public class InitPVP_WJY : MonoBehaviour {

    //模拟延时接收
    public float m_delayReceiveTime = 0.0f;
    //本地玩家初始化是否完成
    bool LocalPlayerInitialized = false;

    //记录向服务器发送请求的时间
    float sendSyncTime = 0;

    public float m_timeBloodBig = 2.0f;
    public int m_effectID = 1;
    public const bool m_isNet = true;
    public const float m_reciveNetTimeDiff = 0.5f; //假设一次传输延迟是m_reciveNetTimeDiff，发占一半，收占一半
    void Start () {
        
  
        //注册接收服务器时间函数
        //FUNC pFunc = new FUNC(OnSyncTimeReturn);
        //MessageManager.self.RegisterHandler(2560, pFunc);

        TimeManager.self.Start();

        InvokeRepeating("SendSyncTime",0.0f, 1.0f);//每秒请求一次服务器时间

        if (m_isNet == false)
        {
            InvokeRepeating("OnSyncTimeReturn", m_reciveNetTimeDiff, 1.0f);//得到服务器时间
        }
        else
        {
            NetEventMgr.Instance.AddListener<PtLong>(MsgIdDefine.RspHeartBeat, OnRspHeartBeat);
        }
    }

    void OnRspHeartBeat(PtLong data)
    {
        float reciveNetTimeDiff = Time.time - sendSyncTime;
        float serverTime = (float)data.value + reciveNetTimeDiff * 0.5f;
        TimeManager.self.currentTime = serverTime;
    }
        //向服务器发送请求服务器时间
    void SendSyncTime()
    {
        sendSyncTime = Time.time;
        GameSocket.Instance.SendMsgProtoVoid(MsgIdDefine.ReqHeartBeat);
    }


    public void OnSyncTimeReturn()
    {
        float receiveSyncTime = Time.time;
        float serverTime = Time.time - m_reciveNetTimeDiff * 0.5f; 

        TimeManager.self.currentTime = serverTime + (receiveSyncTime - sendSyncTime) * 0.5f;
    }




	void Update () {
        TimeManager.self.Update(Time.deltaTime);
        //if (!LocalPlayerInitialized)
        //{
        //    if(PlayerSelf.self.Agent != null)
        //    {
        //        PlayerSelf.self.m_Position = PLAYER_POSITION.PP_3V3_PVP;
        //        PDURunner localRunner = PlayerSelf.self.Agent.gameObject.AddComponent<PDURunner>();
        //        PlayerSelf.self.Agent.gameObject.AddComponent<WJYBehaviorMonitor>();
        //        PlayerSelf.self.Agent.gameObject.AddComponent<WJYPDUSender>();

        //        float hp = PlayerSelf.self.Agent.mPlayer.m_FightAttr.m_life;//得到我当前血量同时第一次也是最大血量
        //        PlayerSelf.self.Agent.gameObject.GetComponent<Agent>().hpBarLoad(hp);//增加血条
        //        LocalPlayerInitialized = true;
        //        return;
        //    }
        //}

        //m_timeBloodBig -= Time.deltaTime;
        //if (m_timeBloodBig <= 0.0f)
        //{
        //    m_timeBloodBig = 2.0f;

        //    Vector3 vPos = PlayerSelf.self.Agent.gameObject.transform.position;
        //    Vector3 vForward = PlayerSelf.self.Agent.gameObject.transform.forward;

        //    tEvent evt = EventCenter.self.StartEvent("CEM_PlayEffectById");
        //    evt.set("id", m_effectID);
        //    evt.set("trans", PlayerSelf.self.Agent.gameObject.transform);
        //    evt.set("dir", PlayerSelf.self.Agent.gameObject.transform.forward);
        //    evt.set("adSource", this.audio);
        //    evt.set("backFunc", null);
        //    evt.DoEvent();
        //}

    }

    //void OnGUI()
    //{
    //    //显示服务器时间
    //    GUI.skin.label.alignment = TextAnchor.UpperLeft;
    //    GUI.skin.label.fontSize = 30;
    //    GUI.Label(new Rect(0, 0, 300, 300), (TimeManager.self.VcurrentTime).ToString());

       

    //    //显示我当前第几段攻击
    //    GUI.skin.label.alignment = TextAnchor.UpperLeft;
    //    GUI.skin.label.fontSize = 30;
    //    GUI.Label(new Rect(0, 120, 500, 300), "My CurStpe" + (PlayerSelf.self.mCurStep).ToString());

    //    ////显示当前PDU传过来的动画
    //    //GUI.skin.label.alignment = TextAnchor.UpperLeft;
    //    //GUI.skin.label.fontSize = 30;
    //    //GUI.Label(new Rect(0, 60, 500, 300), RemotePlayerMgr.self.sAniName);

    //    ////显示接收到远程玩家应该执行的第几段
    //    //GUI.skin.label.alignment = TextAnchor.UpperLeft;
    //    //GUI.skin.label.fontSize = 30;
    //    //GUI.Label(new Rect(0, 180, 500, 300), "RemotePlayer CurStpe" + RemotePlayerMgr.self.m_iRemotePlayerAttackStep.ToString());
    //}


    //    //新玩家进入时，接收到服务器消息创建新玩家
    //    public void OnCreatePlayer(Packet pkt)
    //    {
    //        Player_Info pi = new Player_Info();//玩家信息
    //        SrVector vec = new SrVector();//玩家位置
    //        Fight_Attr_Ex fightAttr = new Fight_Attr_Ex();//玩家战斗属性

    //        pkt.to(ref pi.m_Attributes);
    //        pkt.to(ref pi.m_CurSkill);
    //        pkt.to(ref pi.m_CurEquip);
    //        pkt.to(ref fightAttr);

    //        CLASS2_PROFESSION cp = (CLASS2_PROFESSION)pi.m_Attributes.profession;//根据玩家职业载入prefab

    //        string ModelName = "";
    //        switch (cp)
    //        {
    //            case CLASS2_PROFESSION.C2R_BOWMAN:
    //                {
    //                    ModelName = "Prefabs/ch_player01a";
    //                }
    //                break;
    //            case CLASS2_PROFESSION.C2R_DOUBLESWORD:
    //                {
    //                    ModelName = "Prefabs/ch_player03a";
    //                }
    //                break;
    //            case CLASS2_PROFESSION.C2R_HAMMER:  // 斧头客
    //                {
    //                    ModelName = "Prefabs/ch_player02a";
    //                }
    //                break;
    //            case CLASS2_PROFESSION.C2R_SWORD:
    //                {
    //                    ModelName = "Prefabs/ch_player01a";
    //                }
    //                break;
    //        }

    //        LoadResourceCallBack func = new LoadResourceCallBack(PVP_PlayerLoadingCB);//委托
    //        int temp = 0;
    //        ResourceManager.self.LoadResource(ModelName, func, pi, vec, 1, temp, pkt, fightAttr);
    //    }

    //    //创建玩家的回调
    //    public void PVP_PlayerLoadingCB(UnityEngine.Object obj, params object[] pas)
    //    {
    //        Player_Info pi = (Player_Info)pas[0];//玩家信息
    //        SrVector vec = (SrVector)pas[1];//玩家位置
    //        int ILookSize = (int)pas[2];
    //        int sizeOfSkills = (int)pas[3];
    //        Packet pktSkills = (Packet)pas[4];//技能packet包
    //        Fight_Attr_Ex fightAttrTmp = (Fight_Attr_Ex)pas[5];
    //        PlayerCi pc = new PlayerCi();
    //        pc.FillItemObj(CLASS_ITEM.CI_SKILL, ref pktSkills);//往技能列表中填充
    //        pc.m_PlayerInfo = pi;
    //        pc.m_FightAttr = fightAttrTmp;
    //        pc.AttackTimeList.Clear();
    //        GameObject gobj = FIGHTING.Application.InitObject(obj) as GameObject;//FIGHTING初始化一个gameobject
    //        Agent ag = gobj.GetComponent<Agent>();
    //        ag.mPlayer = pc;
    //        pc.Agent = ag;

    //        //时装初始化
    //        pc.InitSuitObj();

    //        //武器初始化
    //        TIMER_NO_PARAM pFunc = new TIMER_NO_PARAM(pc.InitWeaponObj);
    //        TimeEvent_NoParam tn = new TimeEvent_NoParam(0.3f, pFunc);
    //        TimerManager.self.AddTimer(tn);

    //        pc.m_PlayerID = pc.m_PlayerInfo.m_Attributes.PlayerID;
    //        gobj.transform.position = PlayerSelf.self.Agent.gameObject.transform.position;//后期修正，要对面传过来的位置进行设置

    //        ag.EnemyType = E_EnemyType.PLAYER;

    //        gobj.GetComponent<AnimComponent>().TypeOfFSM = E_AnimFSMTypes.Player;
    //        gobj.GetComponent<AnimSet>().InitPlayerSkillMain();
    //        gobj.GetComponent<AnimSet>().InitAttackData();

    //        gobj.AddComponent<PDUProcessor>();
    //        gobj.GetComponent<CharacterController>().enabled = false;
    //        gobj.AddComponent<WJYBehaviorMonitor>();
    //        ag.CurHealth = 1000;




    //        remotePlayer[pc.m_PlayerID] = gobj;
    //        Scene.Instance.CurrentGameZone._Enemies.Add(ag);
    //    }

    //    //接收服务器传来的PDU响应函数
    //    public void OnReceivePDU(Packet pkt)
    //    {
    //        PDURunner.PDU currentPDU = new PDURunner.PDU();

    //        pkt.to(ref currentPDU.UID);
    //        int type = 0;
    //        pkt.to(ref type);
    //        currentPDU.type = (PDURunner.PDUType)type;

    //        if (type == 999)//技能
    //        {
    //            int skillID = 0;
    //            Vector3 pos = new Vector3();
    //            Vector3 forward = new Vector3();
    //            pkt.to (ref skillID);
    //            pkt.to (ref pos.x);
    //            pkt.to (ref pos.y);
    //            pkt.to (ref pos.z);
    //            pkt.to (ref forward.x);
    //            pkt.to (ref forward.y);
    //            pkt.to (ref forward.z);
    //            remotePlayer[currentPDU.UID].transform.position = pos;
    //            remotePlayer[currentPDU.UID].transform.LookAt(pos + forward);
    //            remotePlayer[currentPDU.UID].GetComponent<PDUProcessor>().isInSkill = true;
    //            remotePlayer[currentPDU.UID].GetComponent<PDUProcessor>().state = PDUProcessor.PlayerSyncState.Skill;

    //            //remotePlayer[currentPDU.UID].GetComponent<Agent>().mPlayer.ExeSkill(skillID);
    //            Agent ag = remotePlayer[currentPDU.UID].GetComponent<Agent>();
    //            ag.mPlayer.DoSkill(skillID);


    //        }
    //        else if (type == 998)//普通攻击
    //        {
    //            Vector3 pos = new Vector3();
    //            Vector3 forward = new Vector3();
    //            int iCurAttackStep = 0;
    //            pkt.to(ref iCurAttackStep);
    //            pkt.to(ref pos.x);
    //            pkt.to(ref pos.y);
    //            pkt.to(ref pos.z);
    //            pkt.to(ref forward.x);
    //            pkt.to(ref forward.y);
    //            pkt.to(ref forward.z);
    //            remotePlayer[currentPDU.UID].transform.position = pos;
    //            remotePlayer[currentPDU.UID].transform.LookAt(pos + forward);

    //            remotePlayer[currentPDU.UID].GetComponent<PDUProcessor>().isInSkill = true;
    //            remotePlayer[currentPDU.UID].GetComponent<PDUProcessor>().state = PDUProcessor.PlayerSyncState.Skill;


    //            Agent ag = remotePlayer[currentPDU.UID].GetComponent<Agent>();



    //            m_iRemotePlayerAttackStep = iCurAttackStep;
    //            ag.mPlayer.mCurStep = iCurAttackStep;
    //            ag.mPlayer.AttackTimeList.Clear();
    //            ag.CurHealth = 123;
    //            ag.mPlayer.DoAttackByStep(iCurAttackStep);


    //        }
    //        else
    //        { 
    //            pkt.to(ref currentPDU.position.x);
    //            pkt.to(ref currentPDU.position.y);
    //            pkt.to(ref currentPDU.position.z);
    //            pkt.to(ref currentPDU.forward.x);
    //            pkt.to(ref currentPDU.forward.y);
    //            pkt.to(ref currentPDU.forward.z);
    //            pkt.to(ref currentPDU.speed);
    //            pkt.to(ref currentPDU.time);
    //            pkt.to(ref currentPDU.anim);

    //            sAniName = currentPDU.anim;

    //            if (!remotePlayer.ContainsKey(currentPDU.UID))
    //            {
    //                Debug.LogError("No such player created!!!!!!");
    //                return;
    //            }
    //#if UseDelayReceive
    //            StartCoroutine(delayForReceivePDU(m_delayReceiveTime, currentPDU));
    //#else
    //            remotePlayer[currentPDU.UID].GetComponent<PDUProcessor>().currentPDU = currentPDU;
    //#endif
    //        }
    //    }
    //    IEnumerator delayForReceivePDU(float fTime, PDURunner.PDU curPDU)
    //    {
    //        yield return new WaitForSeconds(fTime);
    //        remotePlayer[curPDU.UID].GetComponent<PDUProcessor>().currentPDU = curPDU;
    //    }
}


