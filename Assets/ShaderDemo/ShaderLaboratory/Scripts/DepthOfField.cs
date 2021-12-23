using UnityEngine;

[ExecuteInEditMode]
public class DepthOfField : PostEffectsBase
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

    private Camera _camera;
    public Camera Camera
    {
        get
        {
            if (_camera == null)
                _camera = GetComponent<Camera>();
            return _camera;
        }
    }

    private void OnEnable()
    {
        Camera.depthTextureMode |= DepthTextureMode.Depth;
    }


    [Range(1f, 4.0f)]
    public int blurSpread = 2;

    [Range(0.01f, 1.0f)]
    public float focusStart = 0.1f;

    [Range(0.01f, 1.0f)]
    public float focusEnd = 0.2f;

    [Range(1, 4)]
    public int iterations = 2;
   
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {        
        if (Material != null)
        {
            int rtW = source.width;
            int rtH = source.height;

            Material.SetFloat("_FocusStart", focusStart);
            Material.SetFloat("_FocusEnd", focusEnd);

            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer0.filterMode = FilterMode.Bilinear;

            Graphics.Blit(source, buffer0);

            for (int i = 0; i < iterations; i++)
            {
                Material.SetFloat("_BlurSize", 1.0f + i * blurSpread);

                RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
                Graphics.Blit(buffer0, buffer1, Material, 0);
                                                           
                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;


                buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
                Graphics.Blit(buffer0, buffer1, Material, 1);
                                                             
                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
            }

            Material.SetTexture("_BlurMainTex", buffer0);

            Graphics.Blit(source, destination,Material,2);

            RenderTexture.ReleaseTemporary(buffer0);
        }
        else
        {
            Debug.Log("没有shader，没法干活儿");
            Graphics.Blit(source, destination);
        }
    }
}
