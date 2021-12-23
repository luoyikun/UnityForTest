using UnityEngine;

public class PostEffectsBase : MonoBehaviour
{
    protected void CheckResources()
    {
        bool isSupported = CheckSupport();
        if (!isSupported)
        {
            NotSupported();
        }
    }

    protected bool CheckSupport()//检测平台是否支持后处理
    {
        if (SystemInfo.supportsImageEffects == false)
        {
            Debug.LogWarning("This platform does not support image effects！");
            return false;
        }
        return true;
    }

    protected void NotSupported()
    {
        enabled = false;
    }

    protected void Start()
    {
        CheckResources();        
    }

    protected Material CheckShaderAndMaterial(Shader shader, Material material)
    {
        if (shader == null)
            return null;


        if (shader.isSupported && material != null && material.shader == shader)
            return material;
        else
        {
            material = new Material(shader)
            {
                hideFlags = HideFlags.DontSave
            };
            return material;
        }
    }
}
