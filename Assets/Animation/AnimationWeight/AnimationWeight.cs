using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationWeight : MonoBehaviour
{
    public Animation anim;

    string aniRunName = "Running@loop";
    string aniDamagedName = "Damaged@loop";

    [Header("受击动画设置")]
    [Range(0.1f, 1f)] public float damageWeight = 0.7f; // 受击动画权重
    [Range(0.1f, 2f)] public float damageSpeed = 1.2f; // 受击动画速度
    [Range(0.1f, 0.5f)] public float fadeDuration = 0.3f;


    // Start is called before the first frame update
    void Start()
    {
        anim[aniRunName].blendMode = AnimationBlendMode.Blend;
        anim[aniRunName].wrapMode = WrapMode.Loop;
        anim[aniRunName].weight = 1.0f; // 确保权重为1

        anim[aniDamagedName].wrapMode = WrapMode.Once;
        anim[aniDamagedName].blendMode = AnimationBlendMode.Additive;
        anim[aniDamagedName].weight = 0f; // 初始权重为0
        anim[aniDamagedName].enabled = false;

        // 设置动画层级（关键修复）
        //anim[aniRunName].layer = 0;      // 基础层
        //anim[aniDamagedName].layer = 1;  // 叠加层

        // 初始播放
        anim.Play(aniRunName);
    }

    void Update()
    {
        // 检测按1键播放受击动画
        if (Input.GetKeyDown(KeyCode.Alpha1) )
        {
            PlayDamageAnimation();
        }
    }

   

    void PlayDamageAnimation()
    {
        // 设置受击动画参数
        anim[aniDamagedName].time = 0f;
        anim[aniDamagedName].speed = damageSpeed;
        anim[aniDamagedName].weight = damageWeight;
        anim[aniDamagedName].enabled = true;

        // 播放受击动画（不停止跑步动画）
        anim.Blend(aniDamagedName, damageWeight, fadeDuration);
    }

}
