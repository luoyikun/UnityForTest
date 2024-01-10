using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircularDependency : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Stamp[] stamps = new Stamp[] { new Stamp("a","b"), new Stamp("b", "c"), new Stamp("c", "a"),new Stamp("d","e") };
        CircularDependencyChecker checker = new CircularDependencyChecker(stamps);
        string[][] arrStr = checker.Check();
        List<string[]> listStr = new List<string[]>(16);
        listStr.AddRange(arrStr);
        for (int i = 0; i < listStr.Count; i++)
        {
            string[] item = listStr[i];
            string sOut = "";
            for (int idxStr = 0; idxStr < item.Length; idxStr++)
            {
                sOut += item[idxStr];
                sOut += ",";
            }
            Debug.Log(sOut);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}



public sealed class CircularDependencyChecker
{
    private readonly Stamp[] m_Stamps;

    public CircularDependencyChecker(Stamp[] stamps)
    {
        m_Stamps = stamps;
    }

    public string[][] Check()
    {
        HashSet<string> hosts = new HashSet<string>();
        foreach (Stamp stamp in m_Stamps)
        {
            hosts.Add(stamp.HostAssetName);
        }

        List<string[]> results = new List<string[]>();
        foreach (string host in hosts)
        {
            LinkedList<string> route = new LinkedList<string>();
            HashSet<string> visited = new HashSet<string>();
            if (Check(host, route, visited))
            {
                results.Add(route.ToArray());
            }
        }

        return results.ToArray();
    }

    /// <summary>
    /// 检查循环依赖
    /// </summary>
    /// <param name="host">待检查资源</param>
    /// <param name="route">路径表，每次访问新的加入</param>
    /// <param name="visited">已访问表</param>
    /// <returns></returns>
    private bool Check(string host, LinkedList<string> route, HashSet<string> visited)
    {
        visited.Add(host);
        route.AddLast(host);
        Debug.Log(string.Format("{0}加入到已访问，和路径表最后一个", host));
        foreach (Stamp stamp in m_Stamps)
        {
            if (host != stamp.HostAssetName)
            {
                continue;
            }

            if (visited.Contains(stamp.DependencyAssetName))
            {
                Debug.Log("访问表已包含" +  stamp.DependencyAssetName);
                //已经访问节点已经包含了依赖，说明成循环了
                route.AddLast(stamp.DependencyAssetName);
                PublicFunc.DebugObjet2Str(route);
                return true;
            }

            if (Check(stamp.DependencyAssetName, route, visited))
            {
                return true;
            }
        }

        string lastRoute = route.Last.Value;
        route.RemoveLast();
        visited.Remove(host);
        Debug.Log(string.Format("删除路径表的最后一个{0}，已访问表{1}", lastRoute, host));
        return false;
    }
}


public struct Stamp
{
    private readonly string m_HostAssetName;
    private readonly string m_DependencyAssetName;

    public Stamp(string hostAssetName, string dependencyAssetName)
    {
        m_HostAssetName = hostAssetName;
        m_DependencyAssetName = dependencyAssetName;
    }

    public string HostAssetName
    {
        get
        {
            return m_HostAssetName;
        }
    }

    public string DependencyAssetName
    {
        get
        {
            return m_DependencyAssetName;
        }
    }
}
