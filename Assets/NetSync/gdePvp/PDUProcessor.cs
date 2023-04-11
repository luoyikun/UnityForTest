using UnityEngine;
using System.Collections;

/*************************************************************
本地接收到服务器传来的新玩家进入的消息时，在本地创建新玩家（远程玩家），
远程玩家绑定此脚本，受PDU的改变而驱动
**************************************************************/
public class PDUProcessor : MonoBehaviour {

    public uint m_id;


    //最大平滑转弯时间
    public float MaxSmoothTime = 0.05f;

    //最小平滑转弯时间
    public float MinSmoothTime = 0.01f;

    //动画延迟的阀值，小于此阀值加速播放到延迟帧，大于此阀值直接跳掉延迟帧
    public float m_aniSmoothTime = 0.2f;

    //真实平滑转弯时间
    public float realSmoothTime;

    //当前剩下的平滑转弯时间
    public float smoothTime = 0.0f;

    //目标位置
    Vector3 targetPosition;

    //目标朝向
    Vector3 targetForward;

    //目标插值位置
    Vector3 startLerpPosition;

    //目标插值朝向
    Vector3 startLerpForward;

    //服务器时间管理
    //InitPVP_WJY pvpWJY;

	//是否在施放技能
	public bool isInSkill = false;

    public float m_minMoveSpeed = 0.1f;
    //PDU数据包
    private PDURunner.PDU realPDU = null;
    public PDURunner.PDU currentPDU
    {
        get { return realPDU; }
        set { 
            newPDUComing = true;
            this.realPDU = value;
        }
    }

    //有新PDU包标志位
    bool newPDUComing = false;

    //动画组件
    //Animation animSet;

	// 远程用户当前的同步状态
	public enum PlayerSyncState
	{
		StateError = 0,
		MovingOrIdle = 1,
		Skill = 2,
		Terminate = 3,
	};

	public PlayerSyncState state = PlayerSyncState.StateError;

	// Use this for initialization
	void Start () {
        //animSet = GetComponent<Animation>();
        //pvpWJY = GameObject.FindObjectOfType<InitPVP_WJY>();
		// 因为当Processor开始Start()的时候表示已经CreateRemotePlayer成功了，所以默认为此时状态为MoveOrIdle
		state = PlayerSyncState.MovingOrIdle;
	}

	// Update is called once per frame
	void Update () {
        

        //当新PDU传入时改变远程玩家位置，朝向，动画，速度
	    if(newPDUComing)
        {
            //DeterminStateByAnimation(realPDU.anim); 
            //float curTime = pvpWJY.ServerTime.currentTime;
            float curTime = TimeManager.self.currentTime;
            float oldTime = realPDU.time;

			// 消息延迟时间
			float timeDiffer = curTime - oldTime;
			if(timeDiffer < 0 || timeDiffer > 2)
				Debug.LogError("server time error : " + timeDiffer);

			timeDiffer = Mathf.Clamp(timeDiffer, 0, 2);

            smoothTime = realSmoothTime;
            
            // 公式：插值的目标位置 = PDU传输过来的位置 + 朝向 * 速度 * （插值时间 + 消息延迟）
            targetPosition = realPDU.position + realPDU.forward * realPDU.speed * timeDiffer;
			targetForward = realPDU.forward;
            startLerpPosition = transform.position;
            startLerpForward = transform.forward;
            newPDUComing = false;

            if (realPDU.speed <= m_minMoveSpeed)
            {
                smoothTime = 0;
                transform.position = realPDU.position;
                transform.forward = realPDU.forward;
            }
            //transform.position = targetPosition;
        }

        //当还剩下平滑插值时间，继续插值
        if (smoothTime > 0)
        {
            smoothTime -= Time.deltaTime;
            transform.position = Vector3.Lerp(targetPosition, startLerpPosition, smoothTime / realSmoothTime);
            transform.forward = Vector3.Slerp(targetForward, startLerpForward, smoothTime / realSmoothTime);

        }
        else
        {
            if (realPDU != null && realPDU.speed > m_minMoveSpeed)
            {
                transform.position += realPDU.forward * realPDU.speed * Time.deltaTime;
            }
        }
	}
	void DeterminStateByAnimation(string anim)
	{
        if (anim.Contains("run") || anim.Contains("idle"))
        {
            state = PlayerSyncState.MovingOrIdle;
        }
        else if (anim.Contains("skill") || anim.Contains("attack"))
        {
            state = PlayerSyncState.Skill;
        }
	}

	//判断PDU改变的类型
	bool typeChangeJudge(PDURunner.PDUType iType)
	{
		bool bChange = ((realPDU.type & iType) == iType) ? true : false;
		return bChange;
	}

	// 延迟播放动画
	void aniPlaySpeedUp(float fDelayTime)
	{
		//float fAniTime = animSet[realPDU.anim].length;
		//float fLeftTime = fAniTime - fDelayTime;
		//float fSpeedUpTime = (fAniTime - fLeftTime) / (m_speedUpFactor - 1.0f);
		//animSet[realPDU.anim].speed = m_speedUpFactor;
		
		//StartCoroutine(delayForSpeedNormal(fSpeedUpTime));
	}
	IEnumerator delayForSpeedNormal(float fTime)
	{
		yield return new WaitForSeconds(fTime);
		//animSet[realPDU.anim].speed = 1.0f;
	}
}
