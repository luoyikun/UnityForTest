using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string str1 = "123";
        string str2 = str1;
        Debug.Log(object.ReferenceEquals(str1, str2));
        str2 = "456";
        Debug.Log(object.ReferenceEquals(str1, str2));
        Debug.Log(str1);

        string str3 = "789";
        string str4 = "789";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
