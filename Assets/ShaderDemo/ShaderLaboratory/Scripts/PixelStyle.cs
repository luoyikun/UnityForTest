using UnityEngine;

[ExecuteInEditMode]
public class PixelStyle : PostEffectsBase
{
    public Shader shader;
    private Material _material = null;
    public Material Material
    {
        get
        {
            if(_material == null)
                _material = CheckShaderAndMaterial(shader,_material);
            return _material;
        }
    }


    public int pixelAmount = 50;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Material != null)
        {
            Material.SetFloat("_PixelAmount", pixelAmount);

            int rtW = source.width;
            int rtH = source.height;

            Graphics.Blit(source, destination,Material);
        }
        else
        {
            Debug.Log("没有shader，没法干活儿");
            Graphics.Blit(source, destination);
        }
    }
}
