using DataStructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TestBH
{
    public class TestBH : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            BinaryHeap<Node> list = new BinaryHeap<Node>(HeapType.MinHeap);
            list.Push(new Node(4));
            list.Push(new Node(1));

            DebugList(list);

            list.Push(new Node(2));
            

            DebugList(list);


            list.Push(new Node(3));
           
            DebugList(list);

            list.PopRoot();
            DebugList(list);
        }

        public string DebugList(BinaryHeap<Node> list)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < list.items.Count; i++)
            {
                str.Append(list.items[i].value);
                str.Append(", ");

            }
            Debug.Log(str);
            return str.ToString();
        }
    }

    public class Node : IComparable<Node>
    {
        public int value = 0;
        public Node(int v)
        {
            value = v;
        }
        public int CompareTo(Node other)
        {
            if (value < other.value)
            {
                return -1;
            }
            else if (value > other.value)
            {
                return 1;
            }
            return 1;
        }
    }
}
