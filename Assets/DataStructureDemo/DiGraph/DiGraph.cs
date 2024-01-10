using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiGraph : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DirectedGraph graph = new DirectedGraph();

        // 添加顶点
        graph.AddVertex('A');
        graph.AddVertex('B');
        graph.AddVertex('C');
        graph.AddVertex('D');

        // 添加边
        graph.AddEdge('A', 'B');
        graph.AddEdge('B', 'C');
        graph.AddEdge('C', 'A');
        graph.AddEdge('C', 'D');
        graph.AddEdge('D', 'A');

        // 获取顶点的入度和出度
        Debug.Log("Vertex A: Indegree - " + graph.GetIndegree('A') + ", Outdegree - " + graph.GetOutdegree('A'));
        Debug.Log("Vertex B: Indegree - " + graph.GetIndegree('B') + ", Outdegree - " + graph.GetOutdegree('B'));
        Debug.Log("Vertex C: Indegree - " + graph.GetIndegree('C') + ", Outdegree - " + graph.GetOutdegree('C'));
        Debug.Log("Vertex D: Indegree - " + graph.GetIndegree('D') + ", Outdegree - " + graph.GetOutdegree('D'));

        // 统计环的数量
        int cycleCount = graph.CountCycles();
        Debug.Log("Number of cycles: " + cycleCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
class DirectedGraph
{
    //有向图节点字典，key为node，value为该node的依赖列表
    private Dictionary<char, List<char>> adjacencyList;

    public DirectedGraph()
    {
        adjacencyList = new Dictionary<char, List<char>>();
    }

    public void AddVertex(char vertex)
    {
        if (!adjacencyList.ContainsKey(vertex))
        {
            adjacencyList[vertex] = new List<char>();
        }
    }

    /// <summary>
    /// 增加边，点和边所指向的点，都要在图中
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void AddEdge(char from, char to)
    {
        if (adjacencyList.ContainsKey(from) && adjacencyList.ContainsKey(to))
        {
            adjacencyList[from].Add(to);
        }
    }

    /// <summary>
    /// 计算入度
    /// </summary>
    /// <param name="vertex"></param>
    /// <returns></returns>
    public int GetIndegree(char vertex)
    {
        int indegree = 0;
        //遍历每个节点的依赖列表，如果包含目标，入度+1
        foreach (var neighbors in adjacencyList.Values)
        {
            if (neighbors.Contains(vertex))
            {
                indegree++;
            }
        }
        return indegree;
    }

    /// <summary>
    /// 计算出度
    /// </summary>
    /// <param name="vertex"></param>
    /// <returns></returns>
    public int GetOutdegree(char vertex)
    {
        //一个节点依赖多少个出去的，即为出度
        if (adjacencyList.ContainsKey(vertex))
        {
            return adjacencyList[vertex].Count;
        }
        return 0;
    }

    public int CountCycles()
    {
        int cycleCount = 0;
        foreach (var vertex in adjacencyList.Keys)
        {
            HashSet<char> visited = new HashSet<char>();
            if (HasCycle(vertex, visited))
            {
                PublicFunc.DebugObjet2Str(visited);
                cycleCount++;
            }
        }
        return cycleCount;
    }

    private bool HasCycle(char vertex, HashSet<char> visited)
    {
        if (visited.Contains(vertex))
        {
            //访问表中已经包含过，说明构成了环形
            return true; // Cycle detected
        }
        //加入到访问表
        visited.Add(vertex);

        if (adjacencyList.ContainsKey(vertex))
        {
            //遍历该节点的依赖列表
            foreach (var neighbor in adjacencyList[vertex])
            {
                if (HasCycle(neighbor, visited))
                {
                    return true;
                }
            }
        }

        visited.Remove(vertex); // Remove from visited set after exploring neighbors

        return false;
    }
}