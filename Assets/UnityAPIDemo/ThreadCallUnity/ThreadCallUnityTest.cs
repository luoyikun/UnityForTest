using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace ThreadTest
{
    public class ThreadCallUnityTest : MonoBehaviour
    {
        public Text mText;
        void Start()
        {

            // 用Loom的方法调用一个线程
            //Loom.RunAsync(
            //    () =>
            //    {
            //        Thread thread = new Thread(RefreshText);
            //        thread.Start();
            //    }
            //    );
        }

        void Update()
        {
            Loom.QueueOnMainThread((param) =>
            {
                Debug.Log("11");
            }, null);
        }

        private void RefreshText()
        {
            // 用Loom的方法在Unity主线程中调用Text组件
            Loom.QueueOnMainThread((param) =>
            {
                mText.text = "Hello Loom!";
            }, null);
        }
    }
}
