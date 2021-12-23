using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class playerTransform : MonoBehaviour {

	public static bool IsUnderWater, IsTouchingWater;

	public bool under,touch;

	void Update(){
		under = IsUnderWater;
		touch = IsTouchingWater;
	}
	void OnTriggerEnter(Collider col){
		if (col.tag == "Water") {
			playerTransform.IsTouchingWater = true;
			print("Touching water...");
		}
	}
	void OnTriggerExit(Collider col){
		if (col.tag == "Water") {
			playerTransform.IsTouchingWater = false;
			print("Getting out of water...");
		}
	}
}
