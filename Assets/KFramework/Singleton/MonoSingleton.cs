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
                    GameObject gameEntryInstance = GameObject.Find("GameEntry");
                    if (gameEntryInstance != null)
                    {
                        m_instance = gameEntryInstance.AddComponent<T>();
                    }
                    else
                    {
                        gameEntryInstance = GameObject.Find("GameEntry_inited");
                        if (gameEntryInstance != null)
                        {
                            m_instance = gameEntryInstance.AddComponent<T>();
                        }
                        else
                        {
                            gameEntryInstance = new GameObject("GameEntry_inited");
                            m_instance = gameEntryInstance.AddComponent<T>();
                        }
                    }
                }

                return m_instance;
            }
        }
        protected void OnApplicationQuit()
        {
            m_instance = null;
        }
    }
    //public class BehaviourSingleton<T> : MonoBehaviour where T : new()
    //{
    //    protected static T ms_instance = new T();

    //    public static T instance
    //    {
    //        get
    //        {
    //            //if (ms_instance == null)
    //            //{
    //            //    ms_instance = new T();
    //            //}
    //            return ms_instance;
    //        }
    //    }

    //    //public BehaviourSingleton()
    //    void Awake()
    //    {
    //        ms_instance = (T)(System.Object)this;
    //    }

    //}

    public interface IModuleReset
    {
        void Reset();
        void Initialize();
    }

    public class Singleton<T> where T : new()
    {
        protected static T ms_instance;
        public static T instance
        {
            get
            {
                if (ms_instance == null)
                {
                    ms_instance = new T();
                    if (ms_instance is IModuleReset)
                    {
                        ((IModuleReset)ms_instance).Initialize();
                    }
                }
                return ms_instance;
            }
        }

        public Singleton()
        {
            if (ms_instance != null)
            {
                Debug.LogError("Cannot have two instances in singleton");
                return;
            }
            ms_instance = (T)(System.Object)this;

            if (this is IModuleReset)
            {
                //GameEntry.StateController.AddSingleton((System.Object)this as IModuleReset);
            }
        }

    }
}
