using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class BubbleSort : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int[] array = new int[] { 1, 3, 2, 4, 5, 6 };
        BubbleSort1(array);
        string s = JsonConvert.SerializeObject(array); ;
        Debug.Log(s);
    }

    public void BubbleSort1(int[] array)
    {
        bool hasExchagend = false;
        for (int i = 0; i < array.Length - 1; i++)
        {
            hasExchagend = false;
            for (int j = array.Length - 1; j > i; j--)
            {
                if (array[j] < array[j - 1])
                {
                    Exchange(ref array[j], ref array[j - 1]);
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
