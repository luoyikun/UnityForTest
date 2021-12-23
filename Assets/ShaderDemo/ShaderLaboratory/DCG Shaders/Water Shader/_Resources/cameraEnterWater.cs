using UnityEngine;
using System.Collections;

public class cameraEnterWater : MonoBehaviour {

	public bool ApplyUnderWaterEffects;

	bool isCameraUnderWater;

	UnityStandardAssets.ImageEffects.DepthOfField DOF;
	UnderWaterPostEffect UC;

	void Awake(){
		DOF = GetComponent<UnityStandardAssets.ImageEffects.DepthOfField> ();
		UC = GetComponent<UnderWaterPostEffect> ();
	}

	void OnTriggerEnter(Collider col){
		if (col.tag == "Water") {
			playerTransform.IsUnderWater = true;
	

			if(ApplyUnderWaterEffects)
			{
				if(DOF.isActiveAndEnabled && UC.isActiveAndEnabled)
				{
					ApplyEffects();
				}else{
					DOF.enabled = true;
					UC.enabled = true;
					ApplyEffects();
				}
			}

			print("Getting Underwater...");
		}
	}
	void ApplyEffects()
	{
		isCameraUnderWater = playerTransform.IsUnderWater;
		if (isCameraUnderWater) {
			DOF.aperture = 12f;
			DOF.blurType = UnityStandardAssets.ImageEffects.DepthOfField.BlurType.DX11;
			DOF.focalLength = 1.6f;
			DOF.focalSize = 0.3f;
			DOF.maxBlurSize = 5f;
			UC.enabled = true;
		
		} 
		else 
		{
			DOF.aperture = 10.7f;
			DOF.blurType = UnityStandardAssets.ImageEffects.DepthOfField.BlurType.DX11;
			DOF.focalLength = 2;
			DOF.focalSize = 1f;
			DOF.maxBlurSize = 4;
			UC.enabled = false;
		}
	}


	void OnTriggerExit(Collider col){
		if (col.tag == "Water") {
			playerTransform.IsUnderWater = false;
			print("Getting Above water...");

			if(ApplyUnderWaterEffects)
			{
				if(DOF.isActiveAndEnabled && UC.isActiveAndEnabled)
				{
					ApplyEffects();
				}
			}
		}
	}

}
