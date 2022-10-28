using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int i = 10;
        object o = i;
        string s = (string)o;
        Debug.Log(s);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
