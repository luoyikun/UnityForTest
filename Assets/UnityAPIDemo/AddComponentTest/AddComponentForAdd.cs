using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddComponentForAdd : MonoBehaviour
{
    bool isCanLogUpdate = true;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
    }

    public void ForAdd()
    {
        Debug.Log("ForAdd");
    }
    void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    private void Awake()
    {
        Debug.Log("Awake");
    }
    // Update is called once per frame
    void Update()
    {
        if (isCanLogUpdate == true)
        {
            isCanLogUpdate = false;
            Debug.Log("Update");
        }
    }
}
