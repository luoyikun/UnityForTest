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
            BtNode node = CreateBt();
            Debug.Log(RecurveLayer(node));

        }

        int RecurveLayer(BtNode node)
        {
            
            if (node == null )
            {
                return 0;
            }
            Debug.Log("Node:" + node.idx);
            int left = RecurveLayer(node.left);
            int right = RecurveLayer(node.right);
            Debug.Log("left:" + left);
            Debug.Log("right:" + right);
            int layer =  Mathf.Max(left, right) + 1;
            Debug.Log("layer:" + layer);
            return layer;
        }
        // Update is called once per frame
        void Update()
        {

        }

        public BtNode CreateBt()
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

            node0.left = node1;
            node0.right = node2;

            node1.left = node3;
            node1.right = node4;

            node4.right = node5;

            return node0;
        }
    }


    public class BtNode
    {
        public int idx;
        public BtNode left = null;
        public BtNode right = null;
    }
}