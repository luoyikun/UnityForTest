using UnityEngine;
using System.Collections;

public class BehaviorMonitor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public virtual float getSpeed() { return float.NaN; }

    public virtual string getAnimation() { return string.Empty;  }
    

	// Update is called once per frame
	void Update () {
	
	}
}
