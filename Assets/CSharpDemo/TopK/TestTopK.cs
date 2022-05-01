using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DataStructure;
using TestBH;

public class TestTopK : MonoBehaviour
{
    const int m_count = 10;
    const int m_top = 5;
    List<int> m_listOri = new List<int>(m_count);
    // Start is called before the first frame update
    void Start()
    {
        DataInit();

        BinaryHeap<Node> minHeap = new BinaryHeap<Node>(HeapType.MinHeap);
        
        for (int i = 0; i < m_top; i++)
        {
            minHeap.Push(new Node(m_listOri[i]));
        }

        for (int i = m_top; i < m_count; i++)
        {
            int topNum = minHeap.GetRoot().value;

            if (m_listOri[i] > topNum) //这里不能>=，因为是最小堆，只有大于头节点才插入，除头节点外，子节点都是比头节点大
            {
                minHeap.PopRoot();
                minHeap.Push(new Node(m_listOri[i]));
            }
        }
       
        for (int i = m_top-1; i >= 0 ; i--)
        {
            Debug.Log(minHeap.items[i].value);
        }
    }

    void DataInit()
    {

        //for (int i = 0; i < m_count; i++)
        //{
        //    int num = UnityEngine.Random.Range(0, m_count);
        //    m_listOri.Add(num);
        //}

        int[] buf = new int[m_count] { 5, 5, 1, 1, 9, 9, 2, 2, 3, 3 };
        m_listOri.AddRange(buf);
        
    }
}
