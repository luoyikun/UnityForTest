using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sort
{
    public class QuickSort : MonoBehaviour
    {
        private void Start()
        {
            int[] array = new int[8] { 2, 5, 1, 3, 5, 8, 6, 4 };
            quick_sort(array, 0, array.Length-1);
            string s = JsonConvert.SerializeObject(array); ;
            Debug.Log(s);
        }

        void quick_sort(int[] s, int l, int r)
        {
            if (l < r)
            {
                //Swap(s[l], s[(l + r) / 2]); //将中间的这个数和第一个数交换 参见注1

                int i = l, j = r, x = s[l];
                while (i < j)
                {
                    while (i < j && s[j] >= x) // 从右向左找第一个小于x的数
                        --j;
                    if (i < j)
                        s[i++] = s[j];
                    while (i < j && s[i] < x) // 从左向右找第一个大于等于x的数
                        ++i;
                    if (i < j)
                        s[j--] = s[i];
                }

                s[i] = x;
                quick_sort(s, l, i - 1); // 递归调用
                quick_sort(s, i + 1, r);

            }
        }

    }


}