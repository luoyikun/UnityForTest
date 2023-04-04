using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 远程玩家管理器，管理所有远程镜像
/// </summary>
public class RemotePlayerMgr : MonoBehaviour {

    //接收PDU传过来的动画名
    public string sAniName = "";

    //普攻的第几段
    public int m_iRemotePlayerAttackStep = 0;

    //存放远程玩家的信息，key为PlayerID号
    public  Dictionary<uint, GameObject> remotePlayer = new Dictionary<uint, GameObject>();

    public GameObject m_oneRemote;
    public static RemotePlayerMgr m_remotePlayerMgr = null;
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
        //注册接收PDU函数
        //FUNC pFunc = new FUNC(OnReceivePDU);
        //MessageManager.self.RegisterHandler(2557, pFunc);

        ////注册当新玩家进入房间，创建新玩家函数
        //pFunc = new FUNC(OnCreatePlayer);
        //MessageManager.self.RegisterHandler(2555, pFunc);
	}

    //新玩家进入时，接收到服务器消息创建新玩家
    //public void OnCreatePlayer(Packet pkt)
    //{
    //    Player_Info pi = new Player_Info();//玩家信息
    //    SrVector vec = new SrVector();//玩家位置
    //    Fight_Attr_Ex fightAttr = new Fight_Attr_Ex();//玩家战斗属性

    //    pkt.to(ref pi.m_Attributes);
    //    pkt.to(ref pi.m_CurSkill);
    //    pkt.to(ref pi.m_CurEquip);
    //    pkt.to(ref fightAttr);

    //    CLASS2_PROFESSION cp = (CLASS2_PROFESSION)pi.m_Attributes.profession;//根据玩家职业载入prefab

    //    string ModelName = "";
    //    switch (cp)
    //    {
    //        case CLASS2_PROFESSION.C2R_BOWMAN:
    //            {
    //                ModelName = "Prefabs/ch_player01a";
    //            }
    //            break;
    //        case CLASS2_PROFESSION.C2R_DOUBLESWORD:
    //            {
    //                ModelName = "Prefabs/ch_player03a";
    //            }
    //            break;
    //        case CLASS2_PROFESSION.C2R_HAMMER:  // 斧头客
    //            {
    //                ModelName = "Prefabs/ch_player02a";
    //            }
    //            break;
    //        case CLASS2_PROFESSION.C2R_SWORD:
    //            {
    //                ModelName = "Prefabs/ch_player01a";
    //            }
    //            break;
    //    }

    //    LoadResourceCallBack func = new LoadResourceCallBack(PVP_PlayerLoadingCB);//委托
    //    int temp = 0;
    //    ResourceManager.self.LoadResource(ModelName, func, pi, vec, 1, temp, pkt, fightAttr);
    //}

    ////创建玩家的回调
    //public void PVP_PlayerLoadingCB(UnityEngine.Object obj, params object[] pas)
    //{
    //    Player_Info pi = (Player_Info)pas[0];//玩家信息
    //    SrVector vec = (SrVector)pas[1];//玩家位置
    //    int ILookSize = (int)pas[2];
    //    int sizeOfSkills = (int)pas[3];
    //    Packet pktSkills = (Packet)pas[4];//技能packet包
    //    Fight_Attr_Ex fightAttrTmp = (Fight_Attr_Ex)pas[5];
    //    PlayerCi pc = new PlayerCi();
    //    pc.FillItemObj(CLASS_ITEM.CI_SKILL, ref pktSkills);//往技能列表中填充
    //    pc.m_PlayerInfo = pi;
    //    pc.m_FightAttr = fightAttrTmp;
    //    pc.AttackTimeList.Clear();
    //    GameObject gobj = FIGHTING.Application.InitObject(obj) as GameObject;//FIGHTING初始化一个gameobject
    //    Agent ag = gobj.GetComponent<Agent>();
    //    ag.mPlayer = pc;
    //    pc.Agent = ag;
    //    ag.mPlayer.m_Position = PLAYER_POSITION.PP_3V3_PVP;
    //    //时装初始化
    //    pc.InitSuitObj();

    //    //武器初始化
    //    TIMER_NO_PARAM pFunc = new TIMER_NO_PARAM(pc.InitWeaponObj);
    //    TimeEvent_NoParam tn = new TimeEvent_NoParam(0.3f, pFunc);
    //    TimerManager.self.AddTimer(tn);

    //    pc.m_PlayerID = pc.m_PlayerInfo.m_Attributes.PlayerID;
    //    gobj.transform.position = PlayerSelf.self.Agent.gameObject.transform.position;//后期修正，要对面传过来的位置进行设置

    //    ag.EnemyType = E_EnemyType.PLAYER;

    //    gobj.GetComponent<AnimComponent>().TypeOfFSM = E_AnimFSMTypes.Player;
    //    gobj.GetComponent<AnimSet>().InitPlayerSkillMain();
    //    gobj.GetComponent<AnimSet>().InitAttackData();

    //    gobj.AddComponent<PDUProcessor>();
    //    gobj.GetComponent<CharacterController>().enabled = false;
    //    gobj.AddComponent<WJYBehaviorMonitor>();
        
    //    remotePlayer[pc.m_PlayerID] = gobj;
    //    Scene.Instance.CurrentGameZone._Enemies.Add(ag);//目前进来的都是敌人

    //    float hp = ag.mPlayer.m_FightAttr.m_life;
    //    UIManagerNew.self.SetBossMaxHp(hp);
    //    UIManagerNew.self.SetBossCurHp(hp);
    //    UIManagerNew.self.m_MainIconWindow.ShowBossHp(true);//UI层显示敌方玩家的血条

    //    //远程玩家头上显示血条
    //    ag.hpBarLoad(hp);

    //}

    //接收服务器传来的PDU响应函数
    public void OnReceivePDU(Packet pkt)
    {
        PDURunner.PDU currentPDU = new PDURunner.PDU();

        pkt.to(ref currentPDU.UID);
        int type = 0;
        pkt.to(ref type);
        currentPDU.type = (PDURunner.PDUType)type;

        //使用技能，普通，受击特效，掉血同步，这里先注释掉
        //if (type == 999)//技能
        //{
        //    int skillID = 0;
        //    Vector3 pos = new Vector3();
        //    Vector3 forward = new Vector3();
        //    pkt.to(ref skillID);
        //    pkt.to(ref pos.x);
        //    pkt.to(ref pos.y);
        //    pkt.to(ref pos.z);
        //    pkt.to(ref forward.x);
        //    pkt.to(ref forward.y);
        //    pkt.to(ref forward.z);
        //    remotePlayer[currentPDU.UID].transform.position = pos;
        //    remotePlayer[currentPDU.UID].transform.LookAt(pos + forward);
        //    remotePlayer[currentPDU.UID].GetComponent<PDUProcessor>().isInSkill = true;
        //    remotePlayer[currentPDU.UID].GetComponent<PDUProcessor>().state = PDUProcessor.PlayerSyncState.Skill;

        //    //remotePlayer[currentPDU.UID].GetComponent<Agent>().mPlayer.ExeSkill(skillID);
        //    Agent ag = remotePlayer[currentPDU.UID].GetComponent<Agent>();
        //    ag.mPlayer.DoSkill(skillID);


        //}
        //else if (type == 998)//普通攻击
        //{
        //    Vector3 pos = new Vector3();
        //    Vector3 forward = new Vector3();
        //    int iCurAttackStep = 0;
        //    pkt.to(ref iCurAttackStep);
        //    pkt.to(ref pos.x);
        //    pkt.to(ref pos.y);
        //    pkt.to(ref pos.z);
        //    pkt.to(ref forward.x);
        //    pkt.to(ref forward.y);
        //    pkt.to(ref forward.z);
        //    remotePlayer[currentPDU.UID].transform.position = pos;
        //    remotePlayer[currentPDU.UID].transform.LookAt(pos + forward);

        //    remotePlayer[currentPDU.UID].GetComponent<PDUProcessor>().isInSkill = true;
        //    remotePlayer[currentPDU.UID].GetComponent<PDUProcessor>().state = PDUProcessor.PlayerSyncState.Skill;

        //    Agent ag = remotePlayer[currentPDU.UID].GetComponent<Agent>();
        //    m_iRemotePlayerAttackStep = iCurAttackStep;

        //    ag.mPlayer.DoAttackByStep(iCurAttackStep);     
        //}
        //else if (type == 997) //受伤掉血特效
        //{
        //    bool bBigBlood = false;
        //    Vector3 pos = new Vector3();
        //    Vector3 dir = new Vector3();
        //    pkt.to(ref bBigBlood);
        //    pkt.to(ref pos.x);
        //    pkt.to(ref pos.y);
        //    pkt.to(ref pos.z);
        //    pkt.to(ref dir.x);
        //    pkt.to(ref dir.y);
        //    pkt.to(ref dir.z);

        //    if (bBigBlood == true)
        //    {
        //        tEvent evt = EventCenter.self.StartEvent("CEM_PlayBloodBigEffect", true);
        //        evt.set("pos", pos);
        //        evt.set("dir", dir);
        //        evt.DoEvent();
        //    }
        //    else
        //    {
        //        tEvent evt = EventCenter.self.StartEvent("CEM_PlayBloodEffect", true);
        //        evt.set("pos", pos);
        //        evt.set("dir", dir);
        //        evt.DoEvent();
        //    }

        //    float hp = 0.0f;
        //    pkt.to(ref hp);
        //    UIManagerNew.self.m_MainIconWindow.SetBossCurHp(hp);//更新远程玩家血条显示

        //    Scene.Instance.Hits += 1;//我打在远程玩家身上，我本地要显示连击数

        //    //远程玩家头上血条更新
        //    Agent ag = remotePlayer[currentPDU.UID].GetComponent<Agent>();
        //    ag.hpBarCurHpSet(hp);
        //}
        //else  if (type == 996) //受技能掉血
        //{
        //    int iEffectID = 0;
        //    pkt.to(ref iEffectID);
        //    string sEffectID = iEffectID.ToString();
            
        //    Agent ag = remotePlayer[currentPDU.UID].GetComponent<Agent>();
        //    ag.PlayEffect(sEffectID);
        //    //ag.mPlayer.AnimComponent.FSM.CurrentAnimState.PlayEffect(sEffectID);

        //    float hp = 0.0f;
        //    pkt.to(ref hp);
        //    UIManagerNew.self.m_MainIconWindow.SetBossCurHp(hp);

        //    Scene.Instance.Hits += 1;

        //    //远程玩家头上血条更新
        //    ag.hpBarCurHpSet(hp);
        //}
        //else //PDU信息
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

            sAniName = currentPDU.anim;

            //if (!remotePlayer.ContainsKey(currentPDU.UID))
            //{
            //    Debug.LogError("No such player created!!!!!!");
            //    return;
            //}
#if UseDelayReceive
            //使用延迟，相当于模拟网络延迟，延迟多少s进行一次设置pdu
	        StartCoroutine(delayForReceivePDU(m_delayReceiveTime, currentPDU));
#else
            //remotePlayer[currentPDU.UID].GetComponent<PDUProcessor>().currentPDU = currentPDU;
            m_oneRemote.GetComponent<PDUProcessor>().currentPDU = currentPDU;
#endif
        }
    }
    IEnumerator delayForReceivePDU(float fTime, PDURunner.PDU curPDU)
    {
        yield return new WaitForSeconds(fTime);
        //remotePlayer[curPDU.UID].GetComponent<PDUProcessor>().currentPDU = curPDU;
        m_oneRemote.GetComponent<PDUProcessor>().currentPDU = curPDU;
    }

}
