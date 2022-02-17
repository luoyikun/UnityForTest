using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CompoundObjectController : FlashingController
{
	// Cached transform component
	private Transform tr;
	
	// Cached list of child objects
	private List<GameObject> objects;
	
	private int currentShaderID = 2;
	private string[] shaderNames = new string[] {"Bumped Diffuse", "Bumped Specular", "Diffuse", "Diffuse Detail", "Parallax Diffuse", "Parallax Specular" , "Specular", "VertexLit"};
	
	private int ox = -220;
	private int oy = 20;
	
	void Start()
	{
		tr = GetComponent<Transform>();
		objects = new List<GameObject>();
		StartCoroutine(DelayFlashing());
	}
	
	void OnGUI()
	{
		float newX = Screen.width + ox;
		GUI.Label(new Rect(newX, oy, 500, 100), "Compound object controls:");
		
		if (GUI.Button(new Rect(newX, oy + 30, 200, 30), "Add Random Primitive"))
			AddObject();
		
		if (GUI.Button(new Rect(newX, oy + 70, 200, 30), "Change Material"))
			ChangeMaterial();
		
		if (GUI.Button(new Rect(newX, oy + 110, 200, 30), "Change Shader"))
			ChangeShader();
		
		if (GUI.Button(new Rect(newX, oy + 150, 200, 30), "Remove Object"))
			RemoveObject();
	}
	
	void AddObject()
	{
		int primitiveType = Random.Range(0, 4);
		GameObject newObject = GameObject.CreatePrimitive((PrimitiveType)primitiveType);
		Transform newObjectTransform = newObject.GetComponent<Transform>();
		newObjectTransform.parent = tr;
		newObjectTransform.localPosition = Random.insideUnitSphere * 2f;
		objects.Add(newObject);
		
		// Reinitialize highlighting materials, because child objects has changed
		ho.ReinitMaterials();
	}
	
	void ChangeMaterial()
	{
		if (objects.Count < 1)
			AddObject();
		
		currentShaderID = (currentShaderID + 1 >= shaderNames.Length) ? 0 : currentShaderID + 1;
		
		foreach (GameObject obj in objects)
		{
			Renderer renderer = obj.GetComponent<Renderer>();
			Shader newShader = Shader.Find(shaderNames[currentShaderID]);
			renderer.material = new Material(newShader);
		}
		
		// Reinitialize highlightable materials, because material(s) has changed
		ho.ReinitMaterials();
	}
	
	void ChangeShader()
	{
		if (objects.Count < 1)
			AddObject();
		
		currentShaderID = (currentShaderID + 1 >= shaderNames.Length) ? 0 : currentShaderID + 1;
		
		foreach (GameObject obj in objects)
		{
			Renderer renderer = obj.GetComponent<Renderer>();
			Shader newShader = Shader.Find(shaderNames[currentShaderID]);
			renderer.material.shader = newShader;
		}
		
		// Reinitialize highlightable materials, because shader(s) has changed
		ho.ReinitMaterials();
	}
	
	void RemoveObject()
	{
		if (objects.Count < 1)
			return;
		
		GameObject toRemove = objects[objects.Count-1];
		objects.Remove(toRemove);
		Destroy(toRemove);
		
		// Reinitialize highlighting materials, because child objects has changed
		ho.ReinitMaterials();
	}
}
