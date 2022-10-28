using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DictionaryEx
{
    public class DictionaryExTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            TableData<DataKey2, int> dic = new TableData<DataKey2, int>();
            dic.Add(new DataKey2(1, 2), 1);
            dic.Add(new DataKey2(3,4), 2);

            Debug.Log(dic.Find(new DataKey2(1, 2)));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}