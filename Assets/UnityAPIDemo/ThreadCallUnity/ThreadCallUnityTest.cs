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

            // ��Loom�ķ�������һ���߳�
            Loom.RunAsync(
                () =>
                {
                    Thread thread = new Thread(RefreshText);
                    thread.Start();
                }
                );
        }

        void Update()
        {

        }
        private void RefreshText()
        {
            // ��Loom�ķ�����Unity���߳��е���Text���
            Loom.QueueOnMainThread((param) =>
            {
                mText.text = "Hello Loom!";
            }, null);
        }
    }
}
