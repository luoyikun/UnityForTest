

using System.Collections;
using System.Collections.Generic;


namespace KEvent
//C#在类定义外可以声明方法的签名（Delegate，代理或委托），但是不能声明真正的方法。
{
    public delegate void OnNotificationDelegate(EventData note);

    public class EventManager
    {
        private static EventManager instance = null;

        private Dictionary<string, OnNotificationDelegate> eventListerners = new Dictionary<string, OnNotificationDelegate>();

        //Single 
        public static EventManager GetInstance()
        {
            if (instance == null)
            {
                instance = new EventManager();
                return instance;
            }
            return instance;
        }

        /*
         * 监听事件
         */

        //添加监听事件
        public void AddEventListener(string type, OnNotificationDelegate listener)
        {
            if (!eventListerners.ContainsKey(type))
            {
                eventListerners.Add(type, null);
            }
            eventListerners[type] += listener;
        }

        //移除监听事件
        public void RemoveEventListener(string type, OnNotificationDelegate listener)
        {
            if (!eventListerners.ContainsKey(type))
            {
                return;
            }
            eventListerners[type] -= listener;
        }

        //移除某一类型所有的监听事件
        public void RemoveEventListener(string type)
        {
            if (eventListerners.ContainsKey(type))
            {
                eventListerners.Remove(type);
            }
        }

        /*
         * 派发事件
         */

        //派发数据
        public void DispatchEvent(string type, EventData note)
        {
            if (eventListerners.ContainsKey(type))
            {
                if (eventListerners[type] != null)
                {
                    eventListerners[type](note);
                }
            }
        }

        //派发无数据
        public void DispatchEvent(string type)
        {
            DispatchEvent(type, null);
        }

        //查找是否有当前类型事件监听
        public bool HasEventListener(string type)
        {
            return eventListerners.ContainsKey(type);
        }
    }
}