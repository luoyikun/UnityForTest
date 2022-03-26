using Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPool
{
    //��Ч�����
    public class EffectPoolMgr : MonoSingleton<EffectPoolMgr>
    {
        float m_timeForcachTotal = 1.0f; //ÿ����sɨ��һ�γ�
        float m_maxTimeDeleOne = 5.0f; //�����Ƴ�һ������ʱ�䣬��СΪ1��Ԫ��
        float m_minTimeDeleOne = 1.0f; //��ʱ�Ƴ����󣬳���m_count������1sһ��

        private float mResReleaseTime = 1f; //һ���������ʱ��

        //float m_outTimeDestroyOne = 60.0f;//������Ч����ʱ��,��һ��ʹ������ص�ʱ�䳬�����٣����ٳ���һ��Ԫ�أ�������ÿ������
        int m_count = 20; //���������ɳ�ʼ����

        //ÿ���صĻ������
        Dictionary<string, Queue<GameObject>> m_dicPool = new Dictionary<string, Queue<GameObject>>();
        //ÿ���صĻ����
        Dictionary<string, List<GameObject>> m_dicUse = new Dictionary<string, List<GameObject>>();


        Dictionary<string, float> m_lastUsedTime = new Dictionary<string, float>(); //ÿһ��������ϴ�ʹ��ʱ��

        //�ϴ�ɾ����ĳ����������Ļ��������
        private List<string> timeUpdateList = new List<string>();

        //Ҫ��ȫ�ͷŵ�һ�����ӣ����ڼ�gcȨֵ
        private List<string> releaseList = new List<string>();

        

        GameObject m_effectPoolObj = null;
        float m_timeForcachClip = 0;
        Transform GetEffectPoolObj()
        {
            if (m_effectPoolObj == null)
            {
                m_effectPoolObj = new GameObject("EffectPool");
                m_effectPoolObj.transform.parent = transform;
            }
            return m_effectPoolObj.transform;
        }

        public GameObject GetEffect(GameObject objPrefab)
        {
            string name = GetNoCloneName(objPrefab.name);
            GameObject obj = null;
            if (m_dicPool.ContainsKey(name))
            {
                if (m_dicPool[name] != null && m_dicPool[name].Count > 0)
                {
                    obj = m_dicPool[name].Dequeue();
                    while (obj == null && m_dicPool[name].Count > 0)
                    {
                        obj = m_dicPool[name].Dequeue();
                        if (obj != null)
                        {
                            //return obj;
                            break;
                        }
                    }
                    if (obj == null)
                    {
                        obj = GameObject.Instantiate(objPrefab) as GameObject;
                    }

                }
                else
                {
                    obj = GameObject.Instantiate(objPrefab) as GameObject;
                }
            }
            else
            {
                m_dicPool[name] = new Queue<GameObject>(m_count);
                obj = GameObject.Instantiate(objPrefab) as GameObject;
            }
            obj.SetActive(true);
            SetInDicUse(obj);
            return obj;
        }

        public void RecycleEffect(GameObject obj)
        {

            if (obj == null)
            {
                return;
            }
            string name = GetNoCloneName(obj.name);


            m_lastUsedTime[name] = Time.time;
            SetOutDicUse(obj);
            obj.transform.SetParent(GetEffectPoolObj());
            obj.SetActive(false);

            if (m_dicPool.ContainsKey(name))
            {
                if (m_dicPool[name] == null)
                {
                    m_dicPool[name] = new Queue<GameObject>(m_count);
                }
                m_dicPool[name].Enqueue(obj);
            }
            else
            {
                m_dicPool[name] = new Queue<GameObject>(m_count);
                m_dicPool[name].Enqueue(obj);
            }
        }

        public static string GetNoCloneName(string name)
        {
            name = name.Replace("(Clone)", "");
            return name;
        }

        void SetInDicUse(GameObject obj)
        {
            string keyName = GetNoCloneName(obj.name);
            if (m_dicUse.ContainsKey(keyName))
            {
                if (m_dicUse[keyName] == null)
                {
                    m_dicUse[keyName] = new List<GameObject>(m_count);
                }
                m_dicUse[keyName].Add(obj);
            }
            else
            {
                m_dicUse[keyName] = new List<GameObject>(m_count);
                m_dicUse[keyName].Add(obj);
            }
        }

        void SetOutDicUse(GameObject obj)
        {
            string keyName = GetNoCloneName(obj.name);
            if (m_dicUse.ContainsKey(keyName))
            {
                m_dicUse[keyName].Remove(obj);
            }
        }

        /// <summary>
        /// ���ݻ������������Խ�࣬ɾ��Խ�죻Խ�٣�ɾ��Խ��
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        float GetDeleOneObjTimeClip(int count)
        {
            //return m_maxTimeDeleOne;
            float time = 0;
            if (count >= m_count)
            {
                count = m_count;
            }
            float clip = count / m_count;

            time = Mathf.Lerp(m_maxTimeDeleOne, m_minTimeDeleOne, clip);
            return time;
        }

        private void FixedUpdate()
        {
            if (m_timeForcachClip >= m_timeForcachTotal)
            {
                m_timeForcachClip = 0;
                timeUpdateList.Clear();
                releaseList.Clear();
                foreach (var item in m_lastUsedTime)
                {
                    string keyName = item.Key;
                    float lastTime = item.Value;
                    if (m_dicUse.ContainsKey(keyName) && m_dicUse[keyName].Count == 0)
                    {
                        if (m_dicPool.ContainsKey(keyName))
                        {
                            if (m_dicPool[keyName].Count > 0)//ĳ�����ﻹ�п��ж���
                            {
                                if (Time.time - lastTime > GetDeleOneObjTimeClip(m_dicPool[keyName].Count)) // ��ʱ�Ƴ�һ�����ж���
                                {
                                    GameObject idleObj = m_dicPool[keyName].Dequeue();
                                    GameObject.Destroy(idleObj);
                                    timeUpdateList.Add(keyName);
                                }
                            }
                            else//ĳ�������޿��ж���
                            {
                                if (Time.time - lastTime > mResReleaseTime)
                                {
                                    releaseList.Add(keyName);
                                }

                            }
                        }
                    }
                }


                for (int i = 0; i < timeUpdateList.Count; i++)
                {
                    m_lastUsedTime[timeUpdateList[i]] = Time.time; // �ϴ�ɾ���������ĳ��obj��ʱ��
                }

                // �������Դ����������
                for (int i = 0; i < releaseList.Count; i++)
                {
                    if (m_dicUse.ContainsKey(releaseList[i]))
                    {
                        m_dicUse[releaseList[i]] = null;
                        m_dicUse.Remove(releaseList[i]);
                    }
                    if (m_dicPool.ContainsKey(releaseList[i]))
                    {
                        m_dicPool[releaseList[i]] = null;
                        m_dicPool.Remove(releaseList[i]);
                    }
                    m_lastUsedTime.Remove(releaseList[i]);
                }

                AddUnloadWeights(releaseList.Count);
            }
            m_timeForcachClip += Time.deltaTime;
        }

        #region ȨֵGC
        //��ǰȨֵ
        private int m_nSumWeights = 0;
        float CHECK_INTERVAL_FRAME = 60;
        private int m_nMaxInterval = 300;	//���5����ִ��һ��ж�أ���Ϊ�ɱ������TODO
        float m_fLastUnloadTime = 0;
        private int m_nWeightsThreshold = 10;   //����Unload����ֵ����Ϊ�ɱ������TODO
        public void AddUnloadWeights(int nWeights = 1)
        {
            m_nSumWeights += nWeights;
        }

        void Update()
        {
            //ÿ60ִ֡��һ�μ��
            if (Time.frameCount % CHECK_INTERVAL_FRAME == 0)
            {
                TryUnloadUnusedAssets();
            }
        }

        void TryUnloadUnusedAssets()
        {
            if ((Time.realtimeSinceStartup - m_fLastUnloadTime >= m_nMaxInterval)
                || (m_nSumWeights >= m_nWeightsThreshold))
            {
                DoUnloadUnusedAssets();
            }
        }

        void DoUnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            m_nSumWeights = 0;
            m_fLastUnloadTime = Time.realtimeSinceStartup;

        }
        #endregion
    }
}
