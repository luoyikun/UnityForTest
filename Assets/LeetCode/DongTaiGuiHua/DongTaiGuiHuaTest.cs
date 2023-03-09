using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DongTaiGuiHuaTest : MonoBehaviour
{
    //使用哈希map，充当备忘录的作用
    Dictionary<int, int> tempMap = new Dictionary<int, int>();

    // Start is called before the first frame update
    void Start()
    {
        int step = numWaysBuDiGui(10);
        Debug.Log(step);

        step = numWaysByDongTaiGuiHua(10);
        Debug.Log(step);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int numWaysBuDiGui(int n)
    {
        // n = 0 也算1种
        if (n == 0)
        {
            return 1;
        }
        if (n <= 2)
        {
            return n;
        }
        //先判断有没计算过，即看看备忘录有没有
        if (tempMap.ContainsKey(n))
        {
            //备忘录有，即计算过，直接返回
            return tempMap[n];
        }
        else
        {
            // 备忘录没有，即没有计算过，执行递归计算,并且把结果保存到备忘录map中，对1000000007取余（这个是leetcode题目规定的）
            int step = numWaysBuDiGui(n - 1) + numWaysBuDiGui(n - 2);
            tempMap[n] = step;
            return step;
        }
    }

    public int numWaysByDongTaiGuiHua(int n)
    {
        if (n <= 1)
        {
            return 1;
        }
        if (n == 2)
        {
            return 2;
        }
        int a = 1;
        int b = 2;
        int temp = 0;
        for (int i = 3; i <= n; i++)
        {
            temp = (a + b) % 1000000007;
            a = b;
            b = temp;
        }
        return temp;
    }
}
