using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("A"), LayerMask.NameToLayer("B"), true);
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(DateTime.Now.ToString("H:mm:ss.fff") + "：Update");
    }

    private void FixedUpdate()
    {
        //Debug.Log(DateTime.Now.ToString("H:mm:ss.fff") + "：FixedUpdate" );
    }
}
