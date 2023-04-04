using UnityEngine;
using System.Collections;


public class WJYPDUReceiver : PDUReceiver {
    
	// Use this for initialization
	void Start () {
		processor = GetComponent<PDUProcessor> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
