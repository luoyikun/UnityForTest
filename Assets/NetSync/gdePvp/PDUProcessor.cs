using UnityEngine;
using System.Collections;

/*************************************************************
本地接收到服务器传来的新玩家进入的消息时，在本地创建新玩家（远程玩家），
远程玩家绑定此脚本，受PDU的改变而驱动
**************************************************************/
public class PDUProcessor : MonoBehaviour {

    public uint m_id;
    //加速播放动画,必须大于1
    public float m_speedUpFactor = 2.0f;

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

    //PDU数据包
    private PDURunner.PDU realPDU = null;
    public PDURunner.PDU currentPDU
    {
        get { return realPDU; }
        set { 
            newPDUComing = true;
            this.realPDU = value;
			if((int)realPDU.type == 999)
			{
				Debug.LogError("PDU Type shouldn't be 999");
			}
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
			// 此处应该确保传输过来的时间的正确性
			// fix me later.
			timeDiffer = Mathf.Clamp(timeDiffer, 0, 2);

            smoothTime = realSmoothTime;
            // 根据PDU的角度信息确定插值的时长 = realSmoothTime，这个值是在min和max之间的时间
            //float deltaAngle = (Vector3.Dot(realPDU.forward.normalized, transform.forward.normalized) + 1) * 0.5f;
            //smoothTime = realSmoothTime = MinSmoothTime + (MaxSmoothTime - MinSmoothTime) * deltaAngle + timeDiffer;


            //Debug.Log(string.Format("时间延迟：{0};需要插值时间{1}", timeDiffer, smoothTime));




            // 公式：插值的目标位置 = PDU传输过来的位置 + 朝向 * 速度 * （插值时间 + 消息延迟）
            targetPosition = realPDU.position + realPDU.forward * realPDU.speed * timeDiffer;
			targetForward = realPDU.forward;
            startLerpPosition = transform.position;
            startLerpForward = transform.forward;
            newPDUComing = false;

            //GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //obj.transform.position = targetPosition;
            //obj.name = oldTime.ToString();


            //GameObject obj1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //obj1.transform.position = WJYBehaviorMonitor.m_instance.transform.position;
            //obj1.name = oldTime.ToString() + "player";

            transform.position = targetPosition;

            //if (typeChangeJudge(PDURunner.PDUActChange) == true)
            //{
            //    float fAniTotalTime = animSet[realPDU.anim].length;
            //    float fTimeDelay = (float)(curTime - oldTime);
            //    float fAniDelayPercent = fTimeDelay / fAniTotalTime;
            //    if (fAniDelayPercent > m_aniSmoothTime)
            //    {
            //        //从某点开始播放
            //        animSet[realPDU.anim].normalizedTime = fAniDelayPercent;
            //    }
            //    else 
            //    {
            //        //加速播放延迟帧之前，之后正常速度播放
            //        aniPlaySpeedUp(fTimeDelay);
            //    }
            //}

            //bool bActChange = typeChangeJudge(PDURunner.PDUType.ActChange);
            //Debug.LogError( "Start + RemovtePlayerCurAni = " + GetComponent<WJYBehaviorMonitor>().getAnimation()+",,,aniNameFormPDU = " + realPDU.anim + ",,,aniChangeFormPDU = " +bActChange.ToString() );
            //if ((realPDU.anim).Contains("skill") == false && (typeChangeJudge(PDURunner.PDUType.ActChange) || typeChangeJudge(PDURunner.PDUType.SpeedChange)) && realPDU.anim != "")
            //{
            //    animSet.Play(realPDU.anim);
            //}
            //Debug.LogError("End + RemovtePlayerCurAni = " + GetComponent<WJYBehaviorMonitor>().getAnimation() + ",,,aniNameFormPDU = " + realPDU.anim + ",,,aniChangeFormPDU = " + bActChange.ToString());



            if (state == PlayerSyncState.MovingOrIdle && isInSkill == false)
            {
                //animSet.Play(realPDU.anim);
            }
        }

        //当还剩下平滑插值时间，继续插值
        if (smoothTime > 0)
        {
            smoothTime -= Time.deltaTime;
            transform.position = Vector3.Lerp(targetPosition, startLerpPosition, smoothTime / realSmoothTime);
            transform.forward = Vector3.Slerp(targetForward, startLerpForward, smoothTime / realSmoothTime);
            
            //transform.position = targetPosition;
        }
        else
        {
            if (realPDU != null)
            {
                transform.position += realPDU.forward * realPDU.speed * Time.deltaTime;
            }
        }

  //      if (realPDU != null && GetComponent<WJYBehaviorMonitor>().getAnimation() != realPDU.anim && state == PlayerSyncState.MovingOrIdle && realPDU.anim != "")
  //      {
		//	//animSet.Play(realPDU.anim);
		//}
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
