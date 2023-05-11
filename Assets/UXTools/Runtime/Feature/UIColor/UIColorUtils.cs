using System.Collections.Generic;
using UnityEngine;

public static class UIColorUtils
{
    public static UIColorAsset UiColorConfig
    {
        get;
        private set;
    }
    public static UIGradientAsset UiGradientConfig
    {
        get;
        private set;
    }
    public static void LoadGamePlayerConfig()
    {
        UiColorConfig = Resources.Load<UIColorAsset>(UIColorConfig.ColorConfigName);
        UiGradientConfig = Resources.Load<UIGradientAsset>(UIColorConfig.GradientConfigName);
        InitRuntimeData();
    }

    private static Dictionary<int, Color> colorDict = new Dictionary<int, Color>();
    private static Dictionary<int, Gradient> gradientDict = new Dictionary<int, Gradient>();

    private static Dictionary<int, string> colorStringDict = new Dictionary<int, string>();
    private static Dictionary<int, Gradient> gradientStringDict = new Dictionary<int, Gradient>();

    public static void InitRuntimeData()
    {
        colorDict.Clear();
        foreach (var single in UiColorConfig.defList)
        {
            var hash = Animator.StringToHash(single.ColorDefName);
            colorDict[hash] = single.colorValue;
            string color_ = ColorUtility.ToHtmlStringRGB(single.colorValue);
            if (color_ != null && color_.Length == 6)
                color_ = "#" + color_;
            colorStringDict[hash] = color_;
        }
        gradientDict.Clear();
        foreach (var single in UiGradientConfig.defList)
        {
            var hash = Animator.StringToHash(single.ColorDefName);
            gradientDict[hash] = single.colorValue;
        }
    }

    public static string GetDefColorStr(UIColorGenDef.UIColorConfigDef def)
    {
        int val = (int)def;
        if (colorStringDict.TryGetValue(val, out var value))
            return value;
        Debug.Log($"ui_color_config 中 不存在 {def}");
        return "#FFFFFF";
    }

    public static Color GetDefColor(UIColorGenDef.UIColorConfigDef def)
    {
        int val = (int)def;
        if (colorDict.ContainsKey(val))
        {
            return colorDict[val];
        }
        Debug.Log($"ui_color_config 中 不存在 {def}");
        return Color.white;
    }
    public static Gradient GetDefGradient(UIGradientGenDef.UIGradientConfigDef def)
    {
        int val = (int)def;
        if (gradientDict.ContainsKey(val))
        {
            return gradientDict[val];
        }
        Debug.Log($"ui_gradient_config 中 不存在 {def}");
        return new Gradient();
    }
}