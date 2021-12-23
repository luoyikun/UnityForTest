using UnityEngine;

[ExecuteInEditMode]
public class ColorDispersion : PostEffectsBase
{
    public Shader shader;
    private Material _material;
    private Material Material
    {
        get
        {
            if (_material == null)
                _material = CheckShaderAndMaterial(shader, _material);
            return _material;
        }
    }

    [Range(0,0.01f)]
    public float scale = 0;

    public float speed = 5;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Material != null)
        {
            Material.SetFloat("_Scale", scale);
            Material.SetFloat("_Speed", speed);

            Graphics.Blit(source, destination,Material);
        }
        else
        {
            Graphics.Blit(source, destination);
            Debug.Log("Material == null !");
        }
    }

}
