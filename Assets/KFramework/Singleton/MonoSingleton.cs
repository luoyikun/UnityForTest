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
                    GameObject gameEntryInstance = GameObject.Find("MonoSingleton");
                    if (gameEntryInstance == null)
                    {
                        gameEntryInstance = new GameObject("MonoSingleton");
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
        protected void OnApplicationQuit()
        {
            m_instance = null;
        }
    }

}
