using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ListOri;
public class ListTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ListOri.List<int> list = new ListOri.List<int>(64);//����list.Count = 0��ֻ�ǰ��ڲ�����Ԥ�ȷ����˿ռ�

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
