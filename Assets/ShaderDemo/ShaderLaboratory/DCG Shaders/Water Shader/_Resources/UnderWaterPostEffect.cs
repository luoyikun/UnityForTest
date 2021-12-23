//Use Custom-Light for PostEffects in Shader Forge
// C# version by Felipe Mendez (DeadlyCrow).

using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
[RequireComponent(typeof (Camera))]

public class UnderWaterPostEffect : MonoBehaviour {

	Material MaterialToRender;
	string PostEffectName;
	public Shader ShaderToRender;
	public Texture2D NormalDistortion;
	public float speed = 3f, intensity = 0.2f, DistortionScale = 0.15f;
	public Color UnderWaterColor;

	void Awake(){
		if (!ShaderToRender) {
			ShaderToRender = Shader.Find("Hidden/DCG/Water Shader/Underwater");
		}
		if (MaterialToRender != null) 
		{
			applyParameters();
		}
	}

	void OnEnable () 
	{
		if (!ShaderToRender) {
			ShaderToRender = Shader.Find("Hidden/DCG/Water Shader/Underwater");
		}
		UnityEditor.EditorApplication.update = UpdateStringName;
	}
	/*
	void OnDisable()
	{
	
	}
	*/
	void UpdateStringName() {

		if(ShaderToRender)
			PostEffectName = ShaderToRender.name;
		else
			PostEffectName = "No shader has been found";

		applyParameters();
	}

	void OnRenderImage(RenderTexture Source, RenderTexture Destination) 
	{
		if (ShaderToRender != null) {
			if (MaterialToRender != null) {
					Graphics.Blit (Source, Destination, MaterialToRender);
			}
			else{
				MaterialToRender = new Material(ShaderToRender);
			}
		}
	}
	void applyParameters(){
		if (MaterialToRender) {
			MaterialToRender.SetTexture ("_Distortion", NormalDistortion);
			MaterialToRender.SetFloat ("_Intensity", intensity);
			MaterialToRender.SetFloat ("_Speed", speed);
			MaterialToRender.SetColor ("_Tint", UnderWaterColor);
	//		MaterialToRender.SetFloat ("_Fallof", DepthDensity);
	//		MaterialToRender.SetFloat ("_Density", FallofDensity);
	//		MaterialToRender.SetFloat ("_UseDepth", System.Convert.ToSingle (UseDepth));
		//	MaterialToRender.SetFloat ("_Bypass", System.Convert.ToSingle (BypassEffect));
			MaterialToRender.SetFloat ("_Scale", DistortionScale);
		}
	}
}