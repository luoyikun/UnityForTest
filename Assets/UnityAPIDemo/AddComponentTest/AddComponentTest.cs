using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddComponentTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = new GameObject();
        AddComponentForAdd add = obj.AddComponent<AddComponentForAdd>();
        add.ForAdd();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
