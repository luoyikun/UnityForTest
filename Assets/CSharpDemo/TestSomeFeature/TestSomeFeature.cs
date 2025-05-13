using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class TestSomeFeature : MonoBehaviour
{
    int m_count = 100000;
    public enum EnType
    { 
        One,
        Two,
        Three,
    }

    Dictionary<EnType,int> m_dicEnum= new Dictionary<EnType,int>();
    Dictionary<int, int> m_dicValue = new Dictionary<int, int>(); 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Profiler.BeginSample("DicEnum");

        for (int i = 0; i < m_count; i++)
        {
            m_dicEnum.Add(EnType.One, m_count);
            m_dicEnum.Remove(EnType.One);
        }
        Profiler.EndSample();

        Profiler.BeginSample("DicValue");
        for (int i = 0; i < m_count; i++)
        {
            m_dicValue.Add(1, m_count);
            m_dicValue.Remove(1);
        }
        Profiler.EndSample();
    }
}
