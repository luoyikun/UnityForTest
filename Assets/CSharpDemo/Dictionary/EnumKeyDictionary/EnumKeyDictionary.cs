using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class EnumKeyDictionary : MonoBehaviour
{
    Dictionary<DicKey, int> m_dic = new Dictionary<DicKey, int>();
    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        Profiler.BeginSample("EnumKey");
        m_dic[DicKey.One] = 1;
        Profiler.BeginSample("EnumKey");
    }
}

public enum DicKey
{
    One,
    Two
}
