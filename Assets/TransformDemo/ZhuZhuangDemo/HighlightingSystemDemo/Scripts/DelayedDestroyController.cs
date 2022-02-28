using UnityEngine;
using System.Collections;

public class DelayedDestroyController : MonoBehaviour
{
	public float destroyDelay = 2.5f;
	
	void Start()
	{
		StartCoroutine(DelayedDestroy());
	}
	
	protected IEnumerator DelayedDestroy()
	{
		yield return new WaitForSeconds(destroyDelay);
		
		Destroy(gameObject);
	}
}
