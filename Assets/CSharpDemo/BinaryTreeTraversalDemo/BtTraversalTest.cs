using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BtTraversalTest
{
    public class BtTraversalTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void InitBt()
        {
            BtNode node0 = new BtNode();
            node0.idx = 0;

            BtNode node1 = new BtNode();
            node1.idx = 1;

            BtNode node2 = new BtNode();
            node2.idx = 2;

            BtNode node3 = new BtNode();
            node3.idx = 3;

            BtNode node4 = new BtNode();
            node4.idx = 4;

            BtNode node5 = new BtNode();
            node5.idx = 5;

        }
    }


    public class BtNode
    {
        public int idx;
        public BtNode left = null;
        public BtNode right = null;
    }
}