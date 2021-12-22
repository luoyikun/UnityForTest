using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DOTweenDemo
{
    public class DOTweenMgr : MonoBehaviour
    {
        static DOTweenMgr m_instance = null;
        public static DOTweenMgr Instance
        {
            get {
                if (m_instance == null)
                {
                    GameObject obj = new GameObject("DOTweenMgr");
                    m_instance = obj.AddComponent<DOTweenMgr>();
                    DontDestroyOnLoad(obj);
                }
                return m_instance;
            }
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
