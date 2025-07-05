using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationWeight : MonoBehaviour
{
    public Animation anim;

    string aniRunName = "Running@loop";
    string aniDamagedName = "Damaged@loop";

    [Header("�ܻ���������")]
    [Range(0.1f, 1f)] public float damageWeight = 0.7f; // �ܻ�����Ȩ��
    [Range(0.1f, 2f)] public float damageSpeed = 1.2f; // �ܻ������ٶ�
    [Range(0.1f, 0.5f)] public float fadeDuration = 0.3f;


    // Start is called before the first frame update
    void Start()
    {
        anim[aniRunName].blendMode = AnimationBlendMode.Blend;
        anim[aniRunName].wrapMode = WrapMode.Loop;
        anim[aniRunName].weight = 1.0f; // ȷ��Ȩ��Ϊ1

        anim[aniDamagedName].wrapMode = WrapMode.Once;
        anim[aniDamagedName].blendMode = AnimationBlendMode.Additive;
        anim[aniDamagedName].weight = 0f; // ��ʼȨ��Ϊ0
        anim[aniDamagedName].enabled = false;

        // ���ö����㼶���ؼ��޸���
        //anim[aniRunName].layer = 0;      // ������
        //anim[aniDamagedName].layer = 1;  // ���Ӳ�

        // ��ʼ����
        anim.Play(aniRunName);
    }

    void Update()
    {
        // ��ⰴ1�������ܻ�����
        if (Input.GetKeyDown(KeyCode.Alpha1) )
        {
            PlayDamageAnimation();
        }
    }

   

    void PlayDamageAnimation()
    {
        // �����ܻ���������
        anim[aniDamagedName].time = 0f;
        anim[aniDamagedName].speed = damageSpeed;
        anim[aniDamagedName].weight = damageWeight;
        anim[aniDamagedName].enabled = true;

        // �����ܻ���������ֹͣ�ܲ�������
        anim.Blend(aniDamagedName, damageWeight, fadeDuration);
    }

}
