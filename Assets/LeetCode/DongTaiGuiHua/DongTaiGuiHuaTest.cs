using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DongTaiGuiHuaTest : MonoBehaviour
{
    //ʹ�ù�ϣmap���䵱����¼������
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
        // n = 0 Ҳ��1��
        if (n == 0)
        {
            return 1;
        }
        if (n <= 2)
        {
            return n;
        }
        //���ж���û�����������������¼��û��
        if (tempMap.ContainsKey(n))
        {
            //����¼�У����������ֱ�ӷ���
            return tempMap[n];
        }
        else
        {
            // ����¼û�У���û�м������ִ�еݹ����,���Ұѽ�����浽����¼map�У���1000000007ȡ�ࣨ�����leetcode��Ŀ�涨�ģ�
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
