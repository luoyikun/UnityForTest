using UnityEngine;
using System.Collections;

public class RotationController : MonoBehaviour
{
	public float speedX = 20f;
	public float speedY = 30f;
	public float speedZ = 40f;
	
	private Transform tr;
	
	void Awake()
	{
		tr = GetComponent<Transform>();
	}
	
	void Update()
	{
		tr.Rotate(speedX * Time.deltaTime, speedY * Time.deltaTime, speedZ * Time.deltaTime);
	}
}
