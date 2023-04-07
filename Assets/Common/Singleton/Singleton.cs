using System;

namespace Framework.Pattern
{
    /// <summary>
    /// C#单例模式
    /// </summary>
    public abstract class Singleton<T> where T : class,new()
    {
        private static T instance;
        private static object syncRoot = new Object();
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new T();
                    }
                }
                return instance;
            }
        }

        protected Singleton()
        {
            Init();
        }

        public virtual void Init() { }
    }
}