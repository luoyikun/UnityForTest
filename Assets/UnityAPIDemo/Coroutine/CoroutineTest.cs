using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace CoroutineTest
{
    public class CoroutineTest : MonoBehaviour
    {

        void Start()
        {
            StartCoroutine(tt());//����Э��
            for (int i = 0; i < 200; i++)   //ѭ��A
            {
                Debug.Log("*************************" + i);
                Thread.Sleep(10);
            }
        }


        IEnumerator tt()
        {
            for (int i = 0; i < 100; i++) //ѭ��B
            {
                Debug.Log("-------------------" + i);
            }

            yield return new WaitForSeconds(1); //Э��1

            for (int i = 0; i < 100; i++) //ѭ��C
            {
                Debug.Log(">>>>>>>>>>>>>>>>>>>>" + i);
                yield return null; //Э��1
            }
        }

        // ��������
        void Update()
        {
            Debug.Log("Update");
        }

        //���ڸ���
        void LateUpdate()
        {
            Debug.Log("------LateUpdate");
        }


    }
}

