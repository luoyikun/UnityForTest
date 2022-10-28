using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DOTweenDemo
{
    public class tween
    {
        public string tweenType;
        public int loops;
        public int currentLoop;

        public Transform transform;
        public Material material;
        public Vector3 originalPosition;
        public Vector3 originalRotation;
        public Vector3 originalScale;

        public Vector3 origin;
        public Vector3 target;

        public float time;
        public bool isPause;
        public bool autoKill;
        public Coroutine coroutine;

        public delegate void Callback();
        public Callback onComplete;
        public Callback onKill;
        public Callback onPause;

        public Quaternion m_rotation;
        public Quaternion m_tarRotation;
        public tween(string type, Transform trans, Vector3 tar, float ti,int ploops = 1)
        {
            tweenType = type;
            transform = trans;
            material = trans.GetComponent<MeshRenderer>().material;
            target = tar;
            time = ti;
            setOrigin(trans);
            loops = ploops;
            currentLoop = 0;
            isPause = false;
            autoKill = true;
            coroutine = null;
            onComplete = null;
            m_rotation = trans.rotation;
            m_tarRotation = Quaternion.LookRotation(tar, Vector3.up);
        }
        public void Reset()
        {
            if (tweenType == "DoMove")
            {
                transform.position = origin;
            }
            if (tweenType == "DoRotate")
            {
                transform.eulerAngles = origin; //todo：改为角度
            }
            if (tweenType == "DoScale")
            {
                transform.localScale = origin;
            }
            if (tweenType == "DoColor")
            {
                material.color = origin.ToColor();
            }
            if (tweenType == "DoFade")
            {
                Color c = new Color(material.color.r, material.color.g, material.color.b, origin.x);
                material.color = c;
            }
        }
        public void setOrigin(Transform transform)
        {
            if (tweenType == "DoMove")
            {
                origin = transform.position;
            }
            if (tweenType == "DoRotate")
            {
                origin = transform.rotation.eulerAngles;
            }
            if (tweenType == "DoScale")
            {
                origin = transform.localScale;
            }
            if (tweenType == "DoColor")
            {
                origin = material.color.GetRGB();
            }
            if (tweenType == "DoFade")
            {
                origin = new Vector3(material.color.a, 0, 0);
            }
        }
        public tween SetLoops(int l)
        {
            loops = l;
            return this;
        }
        public tween SetCoroutine(Coroutine c)
        {
            coroutine = c;
            return this;
        }
        public tween SetAutoKill(bool auto)
        {
            autoKill = auto;
            return this;
        }
        public tween SetOnComplete(Callback c)
        {
            onComplete += c;
            return this;
        }
        public tween SetOnKill(Callback c)
        {
            onKill += c;
            return this;
        }
        public tween SetOnPause(Callback c)
        {
            onPause += c;
            return this;
        }
        public void Pause()
        {
            isPause = true;
        }
        public void Play()
        {
            isPause = false;
        }
        public void Restart()
        {
            Reset();
            Play();
        }

        public void Complete()
        {
            OnComplete();
        }
        public void Kill()
        {
            DOTweenMgr.Instance.StopCoroutine(coroutine);
        }
        public void OnComplete()
        {
            if (onComplete != null)
            {
                onComplete();
            }
            if (autoKill)
            {
                Kill();
            }
        }
        public void OnKill()
        {
            if (onKill != null)
            {
                onKill();
            }
        }
        public void OnPause()
        {
            if (onPause != null)
            {
                onPause();
            }
        }
    }
}