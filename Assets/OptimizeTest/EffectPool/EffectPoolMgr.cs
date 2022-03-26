using Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPool
{
    //特效缓冲池
    public class EffectPoolMgr : MonoSingleton<EffectPoolMgr>
    {
        float m_timeForcachTotal = 1.0f; //每多少s扫描一次池
        float m_maxTimeDeleOne = 5.0f; //最慢移除一个对象时间，最小为1个元素
        float m_minTimeDeleOne = 1.0f; //超时移除对象，超过m_count，都是1s一个

        private float mResReleaseTime = 1f; //一条池子清空时间

        //float m_outTimeDestroyOne = 60.0f;//单个特效销毁时间,上一次使用这个池的时间超过多少，销毁池里一个元素，而且是每隔多少
        int m_count = 20; //单个池容纳初始数量

        //每个池的缓冲对象
        Dictionary<string, Queue<GameObject>> m_dicPool = new Dictionary<string, Queue<GameObject>>();
        //每个池的活动对象
        Dictionary<string, List<GameObject>> m_dicUse = new Dictionary<string, List<GameObject>>();


        Dictionary<string, float> m_lastUsedTime = new Dictionary<string, float>(); //每一个对象池上次使用时间

        //上次删除过某缓冲池里对象的缓冲池名字
        private List<string> timeUpdateList = new List<string>();

        //要完全释放的一条池子，用于加gc权值
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
        /// 根据缓冲池里数量，越多，删除越快；越少，删除越慢
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
                            if (m_dicPool[keyName].Count > 0)//某个池里还有空闲对象
                            {
                                if (Time.time - lastTime > GetDeleOneObjTimeClip(m_dicPool[keyName].Count)) // 超时移除一个空闲对象
                                {
                                    GameObject idleObj = m_dicPool[keyName].Dequeue();
                                    GameObject.Destroy(idleObj);
                                    timeUpdateList.Add(keyName);
                                }
                            }
                            else//某个池里无空闲对象
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
                    m_lastUsedTime[timeUpdateList[i]] = Time.time; // 上次删除缓冲池里某个obj的时间
                }

                // 清除此资源的所有引用
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

        #region 权值GC
        //当前权值
        private int m_nSumWeights = 0;
        float CHECK_INTERVAL_FRAME = 60;
        private int m_nMaxInterval = 300;	//最大5分钟执行一次卸载，改为由表格配置TODO
        float m_fLastUnloadTime = 0;
        private int m_nWeightsThreshold = 10;   //调用Unload的阈值，改为由表格配置TODO
        public void AddUnloadWeights(int nWeights = 1)
        {
            m_nSumWeights += nWeights;
        }

        void Update()
        {
            //每60帧执行一次检测
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
