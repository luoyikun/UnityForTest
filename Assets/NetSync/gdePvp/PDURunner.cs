#define UseDelaySend

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PDURunner : MonoBehaviour
{
    // 保存历史所有的发生PDU改变的列表，DEBUG版本激活，release的情况下可以干掉这个数据
    List<PDU> histroyPDUs;

	public enum PDUType
	{
		None			= 0, 	// 没有产生任何改动
		OutOrbit 		= 1,	// 超出轨道
		OverThreshold 	= 2,	// 和本地模拟超过一定的阈值
		SpeedChange 	= 4,	// 速度发生改变
		ActChange 		= 8,	// 动作发生改变
		All 			= OutOrbit | OverThreshold | SpeedChange | ActChange,
	};

    //PDU发送者，负责向服务器发送PDU信息
    PDUSender sender;

    // 当前的PDU信息
    public PDU currentPDU;

    // 是否已经创建好轨道
    public bool hasOrbit;

    // 上一帧的情况，用来判断是否发送PDU
    public Vector3 lastPosition;
    public float lastSpeed;
    public string lastAnim;

    // 轨道宽度
    public float orbitWidth = 1.0f;

    // PDU 位置信息的临时Object
    public GameObject m_PDUCreater;

    // 本地模拟位置信息
    Vector3 localSimulatedPosition;

    // 位置矫正差
    public float DistanceTolerance = 1.0f;//5.0fs

    // 速度矫正差
    public float SpeedTolerance = 1.0f;

    //玩家行为
    BehaviorMonitor behaviorMonitor;

    //获取服务器时间
    //InitPVP_WJY pvpWJY;

    // 预测的基础数据类型
    public class PDU
    {
		public uint UID;            //玩家的唯一id			
		public PDUType type;		//PDU类型
		public Vector3 position;	// 位置
		public Vector3 forward;		// 朝向
		public float speed;			// 速度: 速度为0表示静止
		public float time;			// PDU发出的时间
		public string anim;			// 当前的动作
    }

    // Use this for initialization
    void Start()
    {
        //pvpWJY = GameObject.FindObjectOfType<InitPVP_WJY>();
		behaviorMonitor = GetComponent<BehaviorMonitor>();
		sender = GetComponent<PDUSender>();
        hasOrbit = false;
		m_PDUCreater = new GameObject();
        lastSpeed = behaviorMonitor.getSpeed();
        lastAnim = behaviorMonitor.getAnimation();
		currentPDU = new PDU ();
		CreateNewPDU(PDUType.All);
        sender.sendCreatePlayer();
    }

    // 检查是否还在轨道内，每帧调用
    bool inOrbitJudge()
    {
        Vector3 currentPos = gameObject.transform.position;
        Quaternion rot = transform.localRotation;

        Vector3 vct = m_PDUCreater.transform.InverseTransformPoint(currentPos);
        if ( Mathf.Abs(vct.x) > orbitWidth/2.0)
        {
            return false;
        }

        return true;
    }

    //发送PDU给服务器，传递参数为PDU改变的类型
    void sendPDUtoServer(PDUType iType)
    {
        CreateNewPDU(iType);
#if UseDelaySend
        StartCoroutine(delayForSendPDU(InitPVP_WJY.m_reciveNetTimeDiff));
#else 
        sender.sendPDU(currentPDU);
#endif
    }

    //创建新的PDU
    void CreateNewPDU(PDUType iType)
    {
        Vector3 dir = transform.position - lastPosition;
        m_PDUCreater.transform.position = transform.position;
        m_PDUCreater.transform.LookAt(transform.position + dir.normalized * 5);

        //currentPDU = new PDU();
        currentPDU.type = iType;
        currentPDU.position = m_PDUCreater.transform.position;
        currentPDU.forward = m_PDUCreater.transform.forward;
        currentPDU.speed = behaviorMonitor.getSpeed();
        //currentPDU.time = pvpWJY.ServerTime.currentTime;
        currentPDU.time = TimeManager.self.currentTime;
        currentPDU.anim = behaviorMonitor.getAnimation();

        localSimulatedPosition = currentPDU.position;

    }


    IEnumerator delayForSendPDU(float fTime)
    {
        yield return new WaitForSeconds(fTime);
        sender.sendPDU(currentPDU);
    }

    //判断出PDU改变的类型，发送给服务器
    void DeterminToSendPDU()
    {
        bool bIn = inOrbitJudge();
        PDUType iPDUType = PDUType.None;
        // 本地模拟
        localSimulatedPosition += currentPDU.forward * currentPDU.speed * Time.deltaTime;

        if (!bIn)//超出轨道
        {
            iPDUType |= PDUType.OutOrbit; 
        }

        if ((localSimulatedPosition - transform.position).magnitude > DistanceTolerance)// 如果和本地模拟超过一定的阈值也要发送PDU
        {
            iPDUType |= PDUType.OverThreshold;
        }

        if (Mathf.Abs(lastSpeed - behaviorMonitor.getSpeed()) > SpeedTolerance) // 如果速度发生改变，重新发送PDU
        {
            iPDUType |= PDUType.SpeedChange;
        }

        if (lastAnim != behaviorMonitor.getAnimation())// 如果动作发生改变，重新发送PDU
        {
            iPDUType |= PDUType.ActChange;
        }

        if (iPDUType != PDUType.None)
        {
            sendPDUtoServer(iPDUType);
        }
    }

    // Update is called once per frame
    void Update()
    {
		DeterminToSendPDU();
        DrawDirection();
        DrawOrbit();
        lastPosition = transform.position;
        lastSpeed = behaviorMonitor.getSpeed();
        lastAnim = behaviorMonitor.getAnimation();
    }

    //绘制玩家运动方向
    void DrawDirection()
    {
        Vector3 dir = transform.position - lastPosition;
        Debug.DrawLine(transform.position, transform.position + dir.normalized * 10);
    }

    //绘制玩家两边运动的轨道
    void DrawOrbit()
    {
        Vector3 vctLeft = m_PDUCreater.transform.TransformPoint(-orbitWidth / 2, 0, 0);
        Vector3 vctLeftEnd = m_PDUCreater.transform.TransformPoint(-orbitWidth / 2, 0, 100);
        Debug.DrawLine(vctLeft, vctLeftEnd, Color.green);

        Vector3 vctRight = m_PDUCreater.transform.TransformPoint(orbitWidth / 2, 0, 0);
        Vector3 vctRightEnd = m_PDUCreater.transform.TransformPoint(orbitWidth / 2, 0, 100);
        Debug.DrawLine(vctRight, vctRightEnd, Color.green);
    }

    
}
