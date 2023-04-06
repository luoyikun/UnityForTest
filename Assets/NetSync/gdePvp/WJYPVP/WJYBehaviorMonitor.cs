using UnityEngine;
using System.Collections;


public class WJYBehaviorMonitor : BehaviorMonitor {

    public static WJYBehaviorMonitor m_instance = null;
    //玩家速度
    public float speed;

    //当前播放的动画
    string currentAnimation;

    //记录玩家上一次位置
    Vector3 lastPosition;

    //玩家的动画组件
    Animation animSet;

	// Use this for initialization
	void Start () {
        m_instance = this;
        animSet = GetComponent<Animation>();
	}

    //得到当前正在播放的动画名字字符串
    string GetCurrentPlayingAnimation()
    {
        // 这里会做优化和修改：
        // 优化1：不遍历，直接获得
        // 优化2：返回index，不用字符串
        //foreach(AnimationState state in animSet)
        //{
        //    if(animSet.IsPlaying(state.name))
        //    {
        //        return state.name;
        //    }
        //}
        return string.Empty;
    }

    public override string getAnimation()
    {
        return currentAnimation;
    }

    public override float getSpeed()
    {
        return speed;
    }
	// Update is called once per frame
	void Update () {
        speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        string sAniName = GetCurrentPlayingAnimation();
        //if (sAniName.Contains("skill") == false && sAniName.Contains("attack") == false)
        {
            currentAnimation = sAniName;
        }
        lastPosition = transform.position;
	}
}
