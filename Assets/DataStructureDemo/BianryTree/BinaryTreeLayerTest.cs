using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryTreeLayerTest : MonoBehaviour
{
    //二叉树原本数据，0代表节点为null
    List<int> listOri = new List<int>(new int[] { 3, 9, 20, 0, 0, 15, 7 });
    BTNode root = new BTNode();
    Queue<BTNode> m_createQueue = new Queue<BTNode>();
    // Start is called before the first frame update
    void Start()
    {
        CreateBT();
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(root);
        Debug.Log(json);
        LevelOrder();
        int level = RecurveLayer(root);
        Debug.Log("层数：" + level);
    }

    /// <summary>
    /// 层序遍历二叉树
    /// </summary>
    void LevelOrder()
    {
        Queue<BTNode> queOrder = new Queue<BTNode>();
        queOrder.Enqueue(root);
        while (queOrder.Count > 0)
        {
            BTNode one = queOrder.Dequeue();
            Debug.Log(one.value);
            if (one.left != null)
            {
                queOrder.Enqueue(one.left);
            }

            if (one.right != null)
            {
                queOrder.Enqueue(one.right);
            }

        }

    }
    public class BTNode
    {
        public int value = 0;
        public int idx = 0;
        public BTNode left = null;
        public BTNode right = null;
    }

    public BTNode CreateOneNote(int value,int idx)
    {
        BTNode one = new BTNode();
        one.value = value;
        one.idx = idx;
        return one;
    }

    /// <summary>
    /// 层序创建二叉树
    /// </summary>
    void CreateBT()
    {
        //创建根,二叉树的索引 = listIdx-1
        root = CreateOneNote(listOri[0],1);
        m_createQueue.Enqueue(root);
        while (m_createQueue.Count > 0)
        {
            BTNode one = m_createQueue.Dequeue();
            int leftIdx = one.idx * 2;
            int rightIdx = one.idx * 2 + 1;

            if (leftIdx <= listOri.Count && listOri[leftIdx - 1] != 0)
            {
                BTNode left = CreateOneNote(listOri[leftIdx - 1], leftIdx);
                one.left = left;
                m_createQueue.Enqueue(left);
            }

            if (rightIdx <= listOri.Count && listOri[rightIdx - 1] != 0)
            {
                BTNode right = CreateOneNote(listOri[rightIdx - 1], rightIdx);
                one.right = right;
                m_createQueue.Enqueue(right);
            }

        }


    }

    int RecurveLayer(BTNode node)
    {

        if (node == null)
        {
            return 0;
        }

        int left = RecurveLayer(node.left);
        int right = RecurveLayer(node.right);

        int layer = Mathf.Max(left, right) + 1;
        Debug.Log("遍历：" + layer);
        return layer;
    }


}
