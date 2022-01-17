using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HasSetTest
{
    public class HashSetTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            HashSet<string> hs = new HashSet<string>();
            hs.Add("123");
            hs.Add("123");

            List<string>  strList = new List<string>(hs);

            for (int i = 0; i < strList.Count; i++)
            {
                Debug.Log(strList[i]);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
