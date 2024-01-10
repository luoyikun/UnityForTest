using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiGraph : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DirectedGraph graph = new DirectedGraph();

        // ��Ӷ���
        graph.AddVertex('A');
        graph.AddVertex('B');
        graph.AddVertex('C');
        graph.AddVertex('D');

        // ��ӱ�
        graph.AddEdge('A', 'B');
        graph.AddEdge('B', 'C');
        graph.AddEdge('C', 'A');
        graph.AddEdge('C', 'D');
        graph.AddEdge('D', 'A');

        // ��ȡ�������Ⱥͳ���
        Debug.Log("Vertex A: Indegree - " + graph.GetIndegree('A') + ", Outdegree - " + graph.GetOutdegree('A'));
        Debug.Log("Vertex B: Indegree - " + graph.GetIndegree('B') + ", Outdegree - " + graph.GetOutdegree('B'));
        Debug.Log("Vertex C: Indegree - " + graph.GetIndegree('C') + ", Outdegree - " + graph.GetOutdegree('C'));
        Debug.Log("Vertex D: Indegree - " + graph.GetIndegree('D') + ", Outdegree - " + graph.GetOutdegree('D'));

        // ͳ�ƻ�������
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
    //����ͼ�ڵ��ֵ䣬keyΪnode��valueΪ��node�������б�
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
    /// ���ӱߣ���ͱ���ָ��ĵ㣬��Ҫ��ͼ��
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
    /// �������
    /// </summary>
    /// <param name="vertex"></param>
    /// <returns></returns>
    public int GetIndegree(char vertex)
    {
        int indegree = 0;
        //����ÿ���ڵ�������б��������Ŀ�꣬���+1
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
    /// �������
    /// </summary>
    /// <param name="vertex"></param>
    /// <returns></returns>
    public int GetOutdegree(char vertex)
    {
        //һ���ڵ��������ٸ���ȥ�ģ���Ϊ����
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
            //���ʱ����Ѿ���������˵�������˻���
            return true; // Cycle detected
        }
        //���뵽���ʱ�
        visited.Add(vertex);

        if (adjacencyList.ContainsKey(vertex))
        {
            //�����ýڵ�������б�
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