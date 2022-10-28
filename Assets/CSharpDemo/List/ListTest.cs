using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ListOri;
public class ListTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ListOri.List<int> list = new ListOri.List<int>(64);//这里list.Count = 0，只是把内部数组预先分配了空间

        for (int i = 0; i < 64; i++)
        {
            list.Add(i);
        }
        if (list.Contains(32))
        {
            Debug.Log("Find");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
