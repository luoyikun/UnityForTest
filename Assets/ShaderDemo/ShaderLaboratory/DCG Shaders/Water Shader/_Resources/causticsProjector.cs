using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class causticsProjector : MonoBehaviour {

	public Texture2D[] images;
	public float Delay;
	public int curImage = 0;
	float curTime = 0;
	Projector p;
	public bool setParameters = true;
	public float CausticIntensity, HeightCut, UnderWaterCut, CausticsSize = 0.33f;
	public bool UpdateProjector;


	void Update () {
		if (UpdateProjector) {
			UpdateProjector =false;
			p.material = null;

			print("Projector updated !");
		}

		if (p) 
		{
			if (p.material ==null ) 
			{
				Material instance = new Material(Shader.Find("DCG/Water Shader/Caustics"));
				instance.name = "Caustics Material";
			//	instance.hideFlags = HideFlags.HideAndDontSave;

				p.material = instance;

				instance.SetTexture("_Caustic",images[0]);
		
				instance.SetFloat("_CausticsScale",CausticsSize);
				instance.SetFloat("_CausticIntensity",CausticIntensity);
				instance.SetColor("_CausticColor",Color.white);

				instance.SetFloat("_HeightCut",HeightCut);
				instance.SetFloat("_UnderwaterCut",UnderWaterCut);
			}

			if (setParameters) 
			{
				p.material .SetFloat("_HeightCut",HeightCut);
				p.material .SetFloat("_UnderwaterCut",UnderWaterCut);
				p.material .SetFloat("_CausticsScale",CausticsSize);
				p.material .SetFloat("_CausticsIntensity",CausticIntensity);
			}

		curTime += Time.deltaTime;

			
			if(curTime >=Delay)
			{
				curTime = 0;
				p.material .SetTexture("_Caustic",images[curImage]);
				if (curImage < (images.Length -1)) 
				{
					curImage+=1;
				} 
				else 
				{
					curImage = 0;
				}
			}
		}
		else 
		{
			p = GetComponent<Projector> ();
			print("Projector Found !");
		}
	}
}
