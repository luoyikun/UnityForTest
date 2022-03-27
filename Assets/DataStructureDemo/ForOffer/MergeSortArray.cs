using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 数组1：1,3,5
/// 数组2: 2,4,6
/// 合并后：1,2,3，4,5,6
/// </summary>
public class MergeSortArray :MonoBehaviour
{


    private void Start()
    {
        int[] array1 = new int[3] { 1, 3, 5 };
        int[] array2 = new int[4] { 2, 4, 6 ,8};
        int len = array1.Length + array2.Length;
        int[] margeArray = new int[len] ;

        int idx1 = 0;
        int idx2 = 0;
        int i = 0;

        while (idx1 <= array1.Length - 1 && idx2 <= array2.Length - 1)
        {
            if (array1[idx1] < array2[idx2])
            {
                margeArray[i] = array1[idx1];
                idx1++;
                i++;
            }
            else
            {
                margeArray[i] = array2[idx2];
                idx2++;
                i++;
            }
        }

        while (idx1 <= array1.Length - 1 )
        {
            margeArray[i] = array1[idx1];
            idx1++;
            i++;
        }

        while (idx2 <= array2.Length - 1)
        {
            margeArray[i] = array2[idx2];
            idx2++;
            i++;
        }

        string s = "";
        for (int iS = 0; iS < len; iS++)
        {
            s += margeArray[iS] + "--";
        }
        Debug.Log(s);
    }
}
