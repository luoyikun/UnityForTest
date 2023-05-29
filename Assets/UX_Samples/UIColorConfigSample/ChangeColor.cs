using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour
{
    public UXImage img1;
    public UXImage img2;
    // Start is called before the first frame update
    void Start()
    {
        UIColorUtils.LoadGamePlayerConfig();
        img1.color = UIColorUtils.GetDefColor(UIColorGenDef.UIColorConfigDef.Def_示例1);
        img2.m_ColorType = UXImage.ColorType.Gradient_Color;
        img2.Direction = UXImage.GradientDirection.Horizontal;
        img2.gradient = UIColorUtils.GetDefGradient(UIGradientGenDef.UIGradientConfigDef.Def_渐变1);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
