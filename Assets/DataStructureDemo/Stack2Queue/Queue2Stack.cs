using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queue2Stack : MonoBehaviour
{
    Queue<int> m_queue1 = new Queue<int>();
    Queue<int> m_queue2 = new Queue<int>();
    // Start is called before the first frame update
    void Start()
    {
        int top = 0;
        if (Pop(ref top) == true)
        {
            Debug.Log(top);
        }

        Push(1);

        if (Pop(ref top) == true)
        {
            Debug.Log(top);
        }

        Push(2);
        Push(3);


        if (Pop(ref top) == true)
        {
            Debug.Log(top);
        }

        Push(4);

        if (Pop(ref top) == true)
        {
            Debug.Log(top);
        }
    }

    bool Pop(ref int value)
    {
        if (m_queue1.Count == 1 && m_queue2.Count == 0)
        {
            value = m_queue1.Dequeue();
            return true;
        }
        else if (m_queue1.Count == 0 && m_queue2.Count == 1)
        {
            value = m_queue2.Dequeue();
            return true;
        }
        else if (m_queue1.Count > 1 && m_queue2.Count == 0)
        {
            while (m_queue1.Count > 1)
            {
                m_queue2.Enqueue(m_queue1.Dequeue());
            }

            value = m_queue1.Dequeue();
            return true;
        }
        else if (m_queue1.Count == 0 && m_queue2.Count  > 1)
        {
            while (m_queue2.Count > 1)
            {
                m_queue1.Enqueue(m_queue2.Dequeue());
            }

            value = m_queue2.Dequeue();
            return true;
        }

        return false;
    }

    void Push(int value)
    {
        if (m_queue1.Count >= 0  && m_queue2.Count == 0)
        {
            m_queue1.Enqueue(value);
        }
        else if (m_queue1.Count == 0 && m_queue2.Count > 0)
        {
            m_queue2.Enqueue(value);
        }
    }
}
