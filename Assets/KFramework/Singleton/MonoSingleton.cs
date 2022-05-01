using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Singleton
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static T m_instance = null;


        public static T instance
        {
            get
            {
                if (m_instance == null) 
                {
                    string name = typeof(T).ToString();
                    GameObject gameEntryInstance = GameObject.Find(name); //单例的名字都唯一，防止场景里已经有了
                    if (gameEntryInstance == null)
                    {
                        gameEntryInstance = new GameObject(name);
                        DontDestroyOnLoad(gameEntryInstance);
                    }
                    if (gameEntryInstance != null)
                    {
                        m_instance = gameEntryInstance.GetComponent<T>();
                    }
                    if (m_instance == null)
                    {
                        m_instance = gameEntryInstance.AddComponent<T>();
                    }
                }

                return m_instance;
            }
        }

        public void StartUp()
        {

        }
        protected void OnApplicationQuit()
        {
            m_instance = null;
        }
    }














    /// <summary>    /// C#单例模式    /// </summary>    public abstract class Singleton<T> where T : class, new()    {        private static T instance;        private static object syncRoot = new System.Object();        public static T Instance        {            get            {                if (instance == null)                {                    lock (syncRoot)                    {                        if (instance == null)                            instance = new T();                    }                }                return instance;            }        }        protected Singleton()        {            Init();        }        public virtual void Init() { }    }
}
