using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindChildTest : MonoBehaviour
{
    public Transform m_par;
    string m_childName = "abc";
    // Start is called before the first frame update
    void Start()
    {
        string path = GetChildPath(m_par, m_childName);
    


        Debug.Log(path);
    }

    //ջ
    static Transform SearchNodeByStack(Transform rootNode, string valueToFind)
    {
        var stack = new Stack<Transform>(new[] { rootNode });
        while (stack.Count > 0)
        {
            var n = stack.Pop();
            if (n.name == valueToFind)
            {
                //��ջ������ҵ�Ŀ�귵��
                return n;
            }

            //��ǰ�ڵ㻹��child��ȫ����ջ
            if (n.childCount > 0)
            {
                for (int i = 0; i < n.childCount; i++)
                {
                    stack.Push(n.GetChild(i));
                }
            } 
        }

        //ջΪ0��û�ҵ�
        return null;
    }

    //������ȣ��ݹ�
    static Transform SearchNodeByRecursion(Transform tree, string valueToFind)
    {
        if (tree.name == valueToFind)
        {
            return tree;
        }
        else
        {
            if (tree.childCount > 0)
            {
                for (int i = 0; i < tree.childCount; i++)
                {
                    var temp = SearchNodeByRecursion(tree.GetChild(i), valueToFind);
                    if (temp != null) return temp;
                }

            }
        }
        return null;
    }

    string GetChildPath(Transform check, string name)
    {
        List<string> listPath = new List<string>();
        string path = "";
        Transform child = GetTransform(check, name);
        Transform parent = child.parent;
        if (child != null)
        {
            listPath.Add(child.name);
            while (parent != null && parent != check )
            {
                listPath.Add(child.parent.name);
                parent = parent.parent;
            }
        }
        listPath.Add(check.name);

        for (int i = listPath.Count-1; i>= 0; i--)
        {
            path += listPath[i];
            if (i != 0)
            {
                path += "/";
            }
        }

        return path;
    }

    Transform GetTransform(Transform check, string name)
    {
        Transform forreturn = null;

        //GetComponentsInChildren
        //foreach (Transform t in check.GetComponentsInChildren<Transform>())
        //{
        //    if (t.name == name)
        //    {
        //        Debug.Log("�õ�����������������ǣ�" + t.name);
        //        forreturn = t;
        //        return t;

        //    }

        //}

        //��ջ�ҵ�Ŀ��
        //forreturn = SearchNodeByStack(check, name);

        //�ݹ�
        forreturn = SearchNodeByRecursion(check, name);
        return forreturn;
    }

}
