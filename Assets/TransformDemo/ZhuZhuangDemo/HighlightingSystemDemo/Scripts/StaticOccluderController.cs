using UnityEngine;
using System.Collections;

public class StaticOccluderController : MonoBehaviour
{
	void Awake()
	{
		HighlightableObject ho = gameObject.AddComponent<HighlightableObject>();
		ho.OccluderOn();
	}
}
