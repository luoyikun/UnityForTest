using System.Collections.Generic;
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GuideLastPage : VisualElement
{
    public GuideLastPage(VisualElement veParent)
    {

        style.width = 1120;
        style.height = 579;
        var content = UXBuilder.Div(this, new UXBuilderDivStruct()
        {
            style = new UXStyle()
            { height = 484, alignItems = Align.Center, justifyContent = Justify.FlexStart }
        });

        UXBuilder.Text(content, new UXBuilderTextStruct()
        {
            style = new UXStyle()
            {
                height = 70,
                unityTextAlign = TextAnchor.MiddleCenter,
                fontSize = 16,
                marginTop = 50,
            },
            text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_请根据需求设置功能开关)
        });

        var rowToggles = UXBuilder.Row(content, new UXBuilderRowStruct()
        {
            align = Align.Center,
            justify = Justify.Center,
            style = new UXStyle()
            {
                height = 320,
                width = 780,
                marginTop = 12,
                paddingRight = 50,
                paddingLeft = 100,
                paddingBottom = 25,
                paddingTop = 25,
                backgroundImage = new StyleBackground(AssetDatabase.LoadAssetAtPath<Texture2D>(
                    "Assets/UXTools/Res/UX-GUI-Editor-Tools/Assets/Editor/Res/Icon/dashRect.png"))
            }
        });

        DrawRowToggles(rowToggles);

        var rowBottom = UXBuilder.Row(this, new UXBuilderRowStruct()
        {
            align = Align.Center,
            justify = Justify.Center,
            style = new UXStyle() { height = 78 }
        });

        var rowPre = UXBuilder.Row(rowBottom, new UXBuilderRowStruct()
        {
            align = Align.Center,
            justify = Justify.FlexStart,
            style = new UXStyle() { width = Length.Percent(50), height = Length.Percent(100) }
        });

        var rowButtons = UXBuilder.Row(rowBottom, new UXBuilderRowStruct()
        {
            align = Align.Center,
            justify = Justify.FlexEnd,
            style = new UXStyle() { width = Length.Percent(50), height = Length.Percent(100) }
        });
        UXBuilder.Button(rowPre, new UXBuilderButtonStruct()
        {
            type = ButtonType.Text,
            text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_上一步查看引导),
            style = new UXStyle() { fontSize = 16, unityFontStyleAndWeight = FontStyle.Italic },
            OnClick = () =>
            {
                GuideWindow.GetInstance().DrawPage(veParent, 0);
            }
        });

        var buttonUrl = UXBuilder.Button(rowButtons, new UXBuilderButtonStruct()
        {
            text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_用户手册),
            style = new UXStyle()
            {
                fontSize = 16,
                paddingBottom = 6,
                paddingTop = 6,
                paddingLeft = 25,
                paddingRight = 25,
                marginRight = 12
            },
            OnClick = () =>
            {
                Application.OpenURL("https://uxtool.netease.com/help");
            }
        });
        var buttonClose = UXBuilder.Button(rowButtons, new UXBuilderButtonStruct()
        {
            text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_开始使用),
            type = ButtonType.Primary,
            style = new UXStyle()
            {
                fontSize = 16,
                paddingBottom = 6,
                paddingTop = 6,
                paddingLeft = 25,
                paddingRight = 25
            },
            OnClick = () =>
            {
                GuideWindow.GetInstance().CloseWindow();
                SwitchSetting.ChangeSwitch(GuideWindow.toggles.ToArray());
            }
        });
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath + "USS/GuideButton.uss");
        buttonClose.styleSheets.Add(styleSheet);
        buttonClose.AddToClassList("ux-button-guide");
    }

    private void DrawRowToggles(VisualElement veParent)
    {

        var colLeft = UXBuilder.Col(veParent, new UXBuilderColStruct()
        {
            span = 12,
            style = new UXStyle() { height = Length.Percent(100), justifyContent = Justify.FlexStart }
        });

        var colRight = UXBuilder.Col(veParent, new UXBuilderColStruct()
        {
            span = 12,
            style = new UXStyle() { height = Length.Percent(100), justifyContent = Justify.FlexStart }
        });

        UXBuilder.Text(colLeft,
            new UXBuilderTextStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_最近打开),
                style = { marginTop = 30, fontSize = 14 }
            });
        AddToggle(colLeft, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_最近打开面板记录), 0);

        UXBuilder.Text(colLeft,
            new UXBuilderTextStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_场景编辑),
                style = { marginTop = 30, fontSize = 14 }
            });
        AddToggle(colLeft, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_对齐吸附), 1);
        AddToggle(colLeft, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_右键选择列表), 2);
        AddToggle(colLeft, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_快速复制), 3);
        AddToggle(colLeft, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_移动快捷键), 4);

        UXBuilder.Text(colRight,
            new UXBuilderTextStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_预制体),
                style = { marginTop = 30, fontSize = 14 }
            });
        AddToggle(colRight, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_Prefab多开), 5);
        AddToggle(colRight, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_Scene中分辨率调整), 6);
        AddToggle(colRight, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_Prefab资源检查), 7);

        UXBuilder.Text(colRight,
            new UXBuilderTextStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_导入),
                style = { marginTop = 30, fontSize = 14 }
            });
        AddToggle(colRight, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_自动将Texture转为Sprite), 8);
    }

    private void AddToggle(VisualElement veParent, string veName, int index)
    {
        var row = UXBuilder.Row(veParent, new UXBuilderRowStruct()
        {
            style = new UXStyle() { marginTop = 8, marginLeft = 12 },
            align = Align.Center
        });
        bool toggleValue;
        if (GuideWindow.toggles.Count <= index)
            toggleValue = true;
        else toggleValue = GuideWindow.toggles[index].value;
        var toggle = new Toggle()
        { value = toggleValue, style = { marginTop = Length.Percent(0), marginBottom = Length.Percent(0) } };
        row.Add(toggle);
        var label = new Label(veName) { style = { fontSize = 14 } };
        row.Add(label);
        GuideWindow.toggles.Add(toggle);
    }
}
