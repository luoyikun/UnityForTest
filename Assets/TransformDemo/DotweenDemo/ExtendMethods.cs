using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DOTweenDemo
{
    public static class ExtendMethods
    {
        public static Vector3 GetRGB(this Color color)
        {
            return new Vector3(color.r, color.g, color.b);
        }
        public static Color ToColor(this Vector3 vec3)
        {
            return new Color(vec3.x, vec3.y, vec3.z);
        }

        public static tween DoMove(this Transform transform, Vector3 target, float time)
        {
            tween myTween = new tween("DoMove", transform, target, time);
            Coroutine coroutine = DOTweenMgr.Instance.StartCoroutine(DOTweenMgr.Instance.UniversalVector3Iter(myTween));
            myTween.SetCoroutine(coroutine);
            return myTween;
        }

        public static tween DoRotate(this Transform transform, Vector3 target, float time)
        {
            tween myTween = new tween("DoRotate", transform, target, time);
            Coroutine coroutine = DOTweenMgr.Instance.StartCoroutine(DOTweenMgr.Instance.YieldRotate(myTween));
            myTween.SetCoroutine(coroutine);
            return myTween;
        }

        //1. 在协程中插值运算，float f = myTween.time; f >= 0.0f; f -= Time.deltaTime，每帧递减运动时间
        //2. myTween.transform.rotation = Quaternion.Lerp(myTween.m_rotation, myTween.m_tarRotation, 1.0f-f/myTween.time);      tranfrom当前四元数 = 运动开始时 与 目标的差值 ，1.0f-f/myTween.time 的值在每帧越来越靠近 1，说明越来越向目标
        public static IEnumerator YieldRotate(this MonoBehaviour mono, tween myTween)
        {
            for (; myTween.currentLoop < myTween.loops; myTween.currentLoop++)
            {
                myTween.Reset();


                for (float f = myTween.time; f >= 0.0f; f -= Time.deltaTime)
                {
                    //changeEveryFrame(myTween, distance * Time.deltaTime);
                    myTween.transform.rotation = Quaternion.Lerp(myTween.m_rotation, myTween.m_tarRotation, 1.0f-f/myTween.time);
                    yield return null;
                    while (myTween.isPause == true)
                    {
                        yield return null;
                    }
                }
            }
            myTween.OnComplete();
        }


        //总长度/时间 = 每秒要移动的长度  ，然后每帧移动长度 = 每秒要移动的长度 *Time.deltaTime
        public static IEnumerator UniversalVector3Iter(this MonoBehaviour mono, tween myTween)
        {
            for (; myTween.currentLoop < myTween.loops; myTween.currentLoop++)
            {
                myTween.Reset();
                Vector3 distance = (myTween.target - myTween.origin) / myTween.time;
                for (float f = myTween.time; f >= 0.0f; f -= Time.deltaTime)
                {
                    changeEveryFrame(myTween, distance * Time.deltaTime);
                    yield return null;
                    while (myTween.isPause == true)
                    {
                        yield return null;
                    }
                }
            }
            myTween.OnComplete();
        }

        public static void changeEveryFrame(tween myTween, Vector3 changedValue)
        {
            if (myTween.tweenType == "DoMove")
            {
                myTween.transform.Translate(changedValue);
            }
            if (myTween.tweenType == "DoRotate")
            {
                //myTween.transform.Rotate(changedValue);
            }
            if (myTween.tweenType == "DoScale")
            {
                myTween.transform.localScale += changedValue;
            }
            if (myTween.tweenType == "DoColor")
            {
                myTween.material.color = (myTween.material.color.GetRGB() + changedValue).ToColor();
            }
            if (myTween.tweenType == "DoFade")
            {
                Color c = new Color(myTween.material.color.r, myTween.material.color.g,
                    myTween.material.color.b, myTween.material.color.a + changedValue.x);
                myTween.material.color = c;
            }
        }

    }
}
