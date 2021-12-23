using UnityEngine;

[ExecuteInEditMode]
public class Bloom : PostEffectsBase {

	public Shader bloomShader;
	private Material bloomMaterial = null;
	public Material Material
    {  
		get {
			bloomMaterial = CheckShaderAndMaterial(bloomShader, bloomMaterial);
			return bloomMaterial;
		}  
	}

	// Blur iterations - larger number means more blur.
	[Range(0, 4)]
	public int iterations = 3;
	
	[Range(1, 8)]
	public int downSample = 2;

    // Blur spread for each iteration - larger value means more blur
    [Range(0.2f, 3.0f)]
    public float blurSpread = 0.6f;

    [Range(0.0f, 4.0f)]
	public float luminanceThreshold = 0.6f;  

	void OnRenderImage (RenderTexture src, RenderTexture dest)
    {
		if (Material != null)
        {
			Material.SetFloat("_LuminanceThreshold", luminanceThreshold);

			int rtW = src.width/downSample;
			int rtH = src.height/downSample;
			
			RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
			buffer0.filterMode = FilterMode.Bilinear;
			
			Graphics.Blit(src, buffer0, Material, 0);
			
			for (int i = 0; i < iterations; i++)
            {
				Material.SetFloat("_BlurSize", 1.0f + i * blurSpread);
				
				RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
				
				// Render the vertical pass
				Graphics.Blit(buffer0, buffer1, Material, 1);
				
				RenderTexture.ReleaseTemporary(buffer0);
				buffer0 = buffer1;
				buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
				
				// Render the horizontal pass
				Graphics.Blit(buffer0, buffer1, Material, 2);
				
				RenderTexture.ReleaseTemporary(buffer0);
				buffer0 = buffer1;
			}

			Material.SetTexture ("_Bloom", buffer0);  
			Graphics.Blit (src, dest, Material, 3);  

			RenderTexture.ReleaseTemporary(buffer0);
		}
        else
        {
			Graphics.Blit(src, dest);
		}
	}
}
