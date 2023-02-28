using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class BubbleSort : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int[] array = new int[] { 1,2,3, 4, 5, 6 };
        BubbleSort1(array);
        string s = JsonConvert.SerializeObject(array); ;
        Debug.Log(s);
        Debug.Log("循环次数:" + m_time);
    }

    int m_time = 0;
    public  void BubbleSort1(int[] array)
    {
        m_time = 0;
        bool hasExchagend = false;
        for (int i = 0; i < array.Length - 1; i++)
        {
            hasExchagend = false;
            for (int j = 0; j < array.Length - 1 - i; j++)
            {
                m_time++;
                if (array[j] > array[j + 1])
                {
                    Exchange(ref array[j], ref array[j + 1]);
                    hasExchagend = true;
                }
            }
            if (!hasExchagend)
            {
                return;
            }
        }
    }


    void Exchange(ref int a, ref int b)
    {
        int temp = b; //如何不使用中间变量使两个数交换
        b = a;
        a = temp;
    }

}
