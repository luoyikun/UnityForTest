using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://blog.csdn.net/ljlstart/article/details/50778049

namespace Sort
{
    public class QuickSort : MonoBehaviour
    {

        private void Start()
        {
            int[] array = new int[8] { 5 ,2, 2, 1, 7 ,3, 4, 4 };
            quick_sort(array, 0, array.Length-1);
            string s = JsonConvert.SerializeObject(array); ;
            Debug.Log(s);
        }

        //由小到大
        void quick_sort(int[] s, int l, int r)
        {
            if (l < r)
            {
                int i = l, j = r, x = s[l];
                while (i < j)
                {
                    while (i < j && s[j] >= x) // 从右向左找第一个小于x的数
                    {   
                        j--;
                    }
                    if (i < j)
                    {
                        s[i] = s[j];
                        i++;
                    }
                    while (i < j && s[i] < x) // 从左向右找第一个大于等于x的数
                    {
                        i++;
                    }
                    if (i < j)
                    {
                        s[j] = s[i];
                        j--;
                    }
                }

                //这时 i == j，开始取的基准数 x 填入现在的坑位中
                s[i] = x;
                //按照i位置再分为左半边，右半边，进行排序
                quick_sort(s, l, i - 1); // 递归调用
                quick_sort(s, i + 1, r); 

            }
        }

    }

}