using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhyA : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter:" + collision.gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter:" + other.gameObject.name);
    }
}
