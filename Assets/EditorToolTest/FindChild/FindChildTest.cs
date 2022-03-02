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

    // Update is called once per frame
    void Update()
    {
        
    }

    string GetChildPath2(Transform check, string name)
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

        foreach (Transform t in check.GetComponentsInChildren<Transform>())
        {
            if (t.name == name)
            {
                Debug.Log("得到最终子物体的名字是：" + t.name);
                forreturn = t;
                return t;

            }

        }
        return forreturn;
    }


    static List<string> m_listPath = new List<string>();

    public static string GetChildPath(Transform trans, string childName)
    {
        m_listPath.Clear();
        FindChildGameObject(trans.gameObject, childName);
        string path = "";
        for (int i = 1; i < m_listPath.Count; i++)
        {
            path += m_listPath[i];
            if (i != m_listPath.Count - 1)
                path += "/";
        }
        return path;
    }


    public static GameObject FindChildGameObject(GameObject parent, string childName)
    {
        m_listPath.Add(parent.name);
        if (parent.name == childName)
        {

            return parent;
        }
        if (parent.transform.childCount == 0)
        {
            m_listPath.Remove(parent.name);
            return null;
        }
        GameObject obj = null;
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject go = parent.transform.GetChild(i).gameObject;
            obj = FindChildGameObject(go, childName);
            if (obj != null)
            {

                break;
            }
        }

        return obj;
    }
}
