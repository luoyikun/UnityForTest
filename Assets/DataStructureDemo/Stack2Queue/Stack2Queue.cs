using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stack2Queue : MonoBehaviour
{
    Stack<int> m_stack1 = new Stack<int>();
    Stack<int> m_stack2 = new Stack<int>();
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
        if (m_stack2.Count > 0)
        {
            value =  m_stack2.Pop();
            return true;
        }

        while (m_stack1.Count > 0)
        {
            m_stack2.Push(m_stack1.Pop());
        }

        if (m_stack2.Count > 0)
        {
            value = m_stack2.Pop();
            return true;
        }

        return false;
    }

    void Push(int value)
    {
        if (m_stack2.Count == 0 && m_stack1.Count == 0)
        {
            m_stack2.Push(value);
        }
        else
        {
            m_stack1.Push(value);
        }
    }
}
