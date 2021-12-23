using UnityEngine;

[ExecuteInEditMode]
public class BrightnessSaturationAndContrast : PostEffectsBase
{
    public Shader briSatConShader;
    private Material briSatConMaterial;
    public Material BriSatConMaterial
    {
        get
        {
            briSatConMaterial = CheckShaderAndMaterial(briSatConShader,briSatConMaterial);
            return briSatConMaterial;
        }
    }

    [Range(0.0f, 3.0f)]
    public float brightness = 1.0f;

    [Range(0.0f, 3.0f)]
    public float saturation = 1.0f;

    [Range(0.0f, 3.0f)]
    public float contrast = 1.0f;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if(BriSatConMaterial != null)
        {
            BriSatConMaterial.SetFloat("_Brightness", brightness);
            BriSatConMaterial.SetFloat("_Saturation", saturation);
            BriSatConMaterial.SetFloat("_Contrast", contrast);

            Graphics.Blit(src, dest, BriSatConMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
