using UnityEngine;

namespace Framework.Pattern
{
    /// <summary>
    /// MonoBehaviour单例模式
    /// </summary>
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                        "' already destroyed on application quit." +
                        " Won't create again - returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = string.Format("{0}", typeof(T));
           
                        DontDestroyOnLoad(singleton);
                    }

                    return _instance;
                }
            }
            set
            {
                if (_instance != null)
                {
                    Destroy(value);
                    return;
                }
                _instance = value;
                DontDestroyOnLoad(_instance);
            }
        }

        public static bool applicationIsQuitting = false;


        /// <summary>  
        /// When Unity quits, it destroys objects in a random order.  
        /// In principle, a Singleton is only destroyed when application quits.  
        /// If any script calls Instance after it have been destroyed,  
        ///   it will create a buggy ghost object that will stay on the Editor scene  
        ///   even after stopping playing the Application. Really bad!  
        /// So, this was made to be sure we're not creating that buggy ghost object.  
        /// </summary>  
        public virtual void OnDestroy()
        {
            applicationIsQuitting = true;
        }


        public void Startup()
        {

        }
    }
}