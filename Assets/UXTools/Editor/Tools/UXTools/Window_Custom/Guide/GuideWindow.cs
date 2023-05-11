using System;
using System.Collections.Generic;
using System.Linq;
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GuideWindow : EditorWindow
{
    private static GuideWindow _mWindow;

    public static List<Toggle> toggles;



    [MenuItem("ThunderFireUXTool/新手引导 (Tutorial)", false, 56)]
    // public static void RefreshGuideWindow()
    // {
    //     EditorPrefs.SetBool("UXToolGuide", true);
    // }
    public static void OpenWindow()
    {
        int width = 1200;
        int height = 735;
        // _mWindow = GetWindow<GuideWindow>();
        _mWindow = CreateInstance(typeof(GuideWindow)) as GuideWindow;
        if (_mWindow == null) return;
        _mWindow.minSize = new Vector2(width, height);
        _mWindow.maxSize = new Vector2(width, height);
        _mWindow.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_工具说明);
        _mWindow.position = new Rect((Screen.currentResolution.width - width) / 2,
            (Screen.currentResolution.height - height) / 2, width, height);
        _mWindow.ShowUtility();
    }

    private void OnEnable()
    {
        toggles = new Toggle[Enum.GetValues(typeof(SwitchSetting.SwitchType)).Cast<int>().Max() + 1].ToList();
        for (int i = 0; i < toggles.Count; i++)
        {
            toggles[i] = new Toggle();
            toggles[i].value = SwitchSetting.CheckValid(i);
        }
        DrawUI();
    }

    private void DrawUI()
    {
        var root = rootVisualElement;
        root.style.paddingBottom = 0;
        root.style.paddingLeft = 90;
        root.style.paddingRight = 90;
        root.style.paddingTop = 70;
        var div = UXBuilder.Div(root, new UXBuilderDivStruct());
        var rowTop = UXBuilder.Row(div, new UXBuilderRowStruct()
        {
            align = Align.Center,
            justify = Justify.Center,
            style = new UXStyle() { height = 86 }
        });

        var rowPage = UXBuilder.Row(div, new UXBuilderRowStruct()
        {
            align = Align.Center,
            justify = Justify.Center,
            style = new UXStyle() { height = 579 }
        });

        var image = new Image() { style = { width = 276, height = 86 } };
        image.image =
            AssetDatabase.LoadAssetAtPath<Texture>(
                "Assets/UXTools/Res/UX-GUI-Editor-Tools/Assets/Editor/Res/Icon/ToolLogo.png");
        image.scaleMode = ScaleMode.ScaleToFit;
        rowTop.Add(image);


        DrawPage(rowPage, 0);
    }

    public void DrawPage(VisualElement parent, int pageNum)
    {
        parent.Clear();
        var page = pageNum == 0 ? (VisualElement)new GuideFirstPage(parent) : (VisualElement)new GuideLastPage(parent);
        parent.Add(page);
    }

    public static GuideWindow GetInstance()
    {
        return _mWindow;
    }

    public void CloseWindow()
    {
        _mWindow.Close();
    }


}
