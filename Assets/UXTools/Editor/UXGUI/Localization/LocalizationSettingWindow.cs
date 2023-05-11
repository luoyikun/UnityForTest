using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ThunderFireUITool;
using System;

public class LocalizationSettingWindow : EditorWindow
{
    private static LocalizationSettingWindow c_window;
    private static TextElement localizationFolder;
    private static TextElement previewTablePath;
    private static TextElement runtimeTablePath;

    [MenuItem("ThunderFireUXTool/本地化 (Localization)/设置 (Setting)")]
    public static void OpenWindow()
    {
        int width = 650;
        int height = 350;
        c_window = GetWindow<LocalizationSettingWindow>();
        c_window.minSize = new Vector2(width, height);
        c_window.position = new Rect((Screen.currentResolution.width - width) / 2, (Screen.currentResolution.height - height) / 2, width, height);
        c_window.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_设置);
    }

    public static void CloseWindow()
    {
        if (c_window != null)
        {
            c_window.Close();
        }
    }

    private void OnEnable()
    {
        InitWindowData();
        InitWindowUI();
    }

    private VisualElement Root;
    private VisualElement rightContainer;
    private VisualElement leftContainer;
    private Toggle[] toggles;
    private ConfigurationOption LocalizeOption;
    private ConfigurationOption PathOption;

    private void InitWindowData()
    {
        toggles = new Toggle[LocalizationLanguage.Length];
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i] = new Toggle();
        }
        foreach (int i in UXGUIConfig.availableLanguages)
        {
            toggles[i].value = true;
        }
    }

    private void InitWindowUI()
    {
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "SettingWindow.uxml");
        Root = visualTree.CloneTree();
        rootVisualElement.Add(Root);

        leftContainer = Root.Q<VisualElement>("LeftContainer");
        rightContainer = Root.Q<VisualElement>("RightContainer");

        Label nameLabel = Root.Q<Label>("Title");
        nameLabel.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_本地化设置);

        Button confirmBtn = Root.Q<Button>("ConfirmBtn");
        confirmBtn.clicked += ConfirmOnClick;
        confirmBtn.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定);

        Button cancelBtn = Root.Q<Button>("CancelBtn");
        cancelBtn.clicked += CloseWindow;
        cancelBtn.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消);

        leftContainerRefresh();
        LanguageOnClick();
    }

    private void AddLanguage(string text, int row, int col, ref Toggle toggle)
    {
        toggle.style.position = Position.Absolute;
        toggle.style.left = 50 + 225 * col;
        toggle.style.top = 20 + 30 * row;
        rightContainer.Add(toggle);
        TextElement label = new TextElement();
        label.text = text;
        label.style.position = Position.Absolute;
        label.style.left = 72 + 225 * col;
        label.style.top = 19 + 30 * row;
        label.style.fontSize = 13;
        label.style.color = Color.white;
        rightContainer.Add(label);
    }
    private void LanguageOnClick()
    {
        leftContainerRefresh();
        LocalizeOption.isSelect();
        rightContainer.Clear();
        for (int i = 0; i < LocalizationLanguage.Length; i++)
        {
            AddLanguage(LocalizationLanguage.GetLanguage(i), i / 2, i % 2, ref toggles[i]);
        }
    }

    private void PathOnClick()
    {
        leftContainerRefresh();
        PathOption.isSelect();
        rightContainer.Clear();

        TextElement label = new TextElement();
        label.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_本地化文件夹) + ":";
        label.style.position = Position.Absolute;
        label.style.left = 50;
        label.style.top = 20;
        label.style.fontSize = 13;
        label.style.color = Color.white;
        rightContainer.Add(label);

        localizationFolder = new TextElement();
        localizationFolder.text = UXGUIConfig.LocalizationFolder;
        localizationFolder.style.position = Position.Absolute;
        localizationFolder.style.left = 50;
        localizationFolder.style.top = 40;
        localizationFolder.style.maxWidth = 400;
        localizationFolder.style.fontSize = 13;
        localizationFolder.style.color = Color.white;
        rightContainer.Add(localizationFolder);

        Image mrIcon = new Image();
        mrIcon.style.height = 10;
        mrIcon.image = ToolUtils.GetIcon("More");
        Button MoreBtn = EditorUIUtils.CreateUIEButton("", mrIcon, SelectPath, 30, 20);
        MoreBtn.style.top = 20;
        MoreBtn.style.right = 50;
        MoreBtn.tooltip = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_选择路径);
        rightContainer.Add(MoreBtn);

        label = new TextElement();
        label.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_静态文本表格路径) + ":";
        label.style.position = Position.Absolute;
        label.style.left = 50;
        label.style.top = 80;
        label.style.fontSize = 13;
        label.style.color = Color.white;
        rightContainer.Add(label);

        runtimeTablePath = new TextElement();
        runtimeTablePath.text = UXGUIConfig.RuntimeTablePath;
        runtimeTablePath.style.position = Position.Absolute;
        runtimeTablePath.style.left = 50;
        runtimeTablePath.style.top = 100;
        runtimeTablePath.style.maxWidth = 400;
        runtimeTablePath.style.fontSize = 13;
        runtimeTablePath.style.color = Color.white;
        rightContainer.Add(runtimeTablePath);

        mrIcon = new Image();
        mrIcon.style.height = 10;
        mrIcon.image = ToolUtils.GetIcon("More");
        MoreBtn = EditorUIUtils.CreateUIEButton("", mrIcon, SelectRuntimeFile, 30, 20);
        MoreBtn.style.top = 80;
        MoreBtn.style.right = 50;
        MoreBtn.tooltip = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_选择路径);
        rightContainer.Add(MoreBtn);
        
        label = new TextElement();
        label.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_动态文本表格路径) + ":";
        label.style.position = Position.Absolute;
        label.style.left = 50;
        label.style.top = 140;
        label.style.fontSize = 13;
        label.style.color = Color.white;
        rightContainer.Add(label);

        previewTablePath = new TextElement();
        previewTablePath.text = UXGUIConfig.PreviewTablePath;
        previewTablePath.style.position = Position.Absolute;
        previewTablePath.style.left = 50;
        previewTablePath.style.top = 160;
        previewTablePath.style.maxWidth = 400;
        previewTablePath.style.fontSize = 13;
        previewTablePath.style.color = Color.white;
        rightContainer.Add(previewTablePath);

        mrIcon = new Image();
        mrIcon.style.height = 10;
        mrIcon.image = ToolUtils.GetIcon("More");
        MoreBtn = EditorUIUtils.CreateUIEButton("", mrIcon, SelectPreviewFile, 30, 20);
        MoreBtn.style.top = 140;
        MoreBtn.style.right = 50;
        MoreBtn.tooltip = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_选择路径);
        rightContainer.Add(MoreBtn);

        Button button = EditorUIUtils.CreateUIEButton(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_生成空的静态文本表格), null, CreateRuntimeTextTable, 400, 20);
        button.style.top = 200;
        button.style.left = 50;
        rightContainer.Add(button);

        button = EditorUIUtils.CreateUIEButton(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_生成空的动态文本表格), null, CreatePreviewTextTable, 400, 20);
        button.style.top = 220;
        button.style.left = 50;
        rightContainer.Add(button);
    }

    private void CreateRuntimeTextTable()
    {
        UnityEngine.UI.UXTextTable.CreateRuntimeTable(true);
        runtimeTablePath.text = UXGUIConfig.RuntimeTablePath;
    }

    private void CreatePreviewTextTable()
    {
        UnityEngine.UI.UXTextTable.CreatePreviewTable(true);
        previewTablePath.text = UXGUIConfig.PreviewTablePath;
    }

    private void SelectPath()
    {
        localizationFolder.text = Utils.SelectFolder() ?? localizationFolder.text;
    }

    private void SelectRuntimeFile()
    {
        runtimeTablePath.text = Utils.SelectFile() ?? runtimeTablePath.text;
    }

    private void SelectPreviewFile()
    {
        previewTablePath.text = Utils.SelectFile() ?? previewTablePath.text;
    }

    private void leftContainerRefresh()
    {
        leftContainer.Clear();

        LocalizeOption = new ConfigurationOption(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_语言), LanguageOnClick);
        LocalizeOption.style.top = 0;
        leftContainer.Add(LocalizeOption);

        PathOption = new ConfigurationOption(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_路径), PathOnClick);
        PathOption.style.top = 40;
        leftContainer.Add(PathOption);
    }

    private void ConfirmOnClick()
    {
        UXGUIConfig.availableLanguages.Clear();
        for (int i = 0; i < LocalizationLanguage.Length; i++)
        {
            if (toggles[i].value)
            {
                UXGUIConfig.availableLanguages.Add(i);
            }
        }
        if(localizationFolder != null)
        {
            UXGUIConfig.LocalizationFolder = localizationFolder.text;
            UXGUIConfig.RuntimeTablePath = runtimeTablePath.text;
            UXGUIConfig.PreviewTablePath = previewTablePath.text;
        }
        else
        {
            var uxConfig = ResourceManager.Load<UXGUIConfig>("UXGUIConfig");
            EditorUtility.SetDirty(uxConfig);
            AssetDatabase.SaveAssets();
        }
        Selection.activeGameObject = null;

        CloseWindow();
    }
}