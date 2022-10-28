using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAtlasAbTest : MonoBehaviour
{
    public Image m_img1;
    // Start is called before the first frame update
    void Start()
    {
        string assetPath = "AssetbundleDemo/SpriteAtlasTestAb/ShanTu/ShanTu";
        var assetType = typeof(GameObject);
        //xasset.Asset.LoadAsync(assetPath, assetType, (asset) => {
        //    m_img1.sprite = asset.Get<Sprite>();

        //});

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
