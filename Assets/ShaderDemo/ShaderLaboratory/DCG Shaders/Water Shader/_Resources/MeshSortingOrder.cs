using UnityEngine;
using System.Collections;

public class MeshSortingOrder : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		GetComponent<Renderer>().sortingOrder = 1000;
	}
	
	// Update is called once per frame
	void OnEnable () {
		GetComponent<Renderer>().sortingOrder = 1000;
	}
}
