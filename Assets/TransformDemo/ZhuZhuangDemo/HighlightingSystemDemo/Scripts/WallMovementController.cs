using UnityEngine;
using System.Collections;

public class WallMovementController : MonoBehaviour
{
	public bool moveX;
	public bool moveY;
	public bool moveZ;
	
	public Vector3 amplitude = Vector3.one;
	
	private Transform tr;
	private float counter;
	private Vector3 initialOffsets;
	
	void Awake()
	{
		tr = GetComponent<Transform>();
		initialOffsets = tr.position;
		counter = 0f;
	}
	
	void Update()
	{
		counter += Time.deltaTime * 1.2f;
		
		Vector3 newPosition = new Vector3
		(
			moveX ? initialOffsets.x + amplitude.x * Mathf.Sin(counter) : initialOffsets.x, 
			moveY ? initialOffsets.y + amplitude.y * Mathf.Sin(counter) : initialOffsets.y, 
			moveZ ? initialOffsets.z + amplitude.z * Mathf.Sin(counter) : initialOffsets.z
		);
		
		tr.position = newPosition;
	}
}
