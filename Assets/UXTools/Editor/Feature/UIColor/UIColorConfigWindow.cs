using ThunderFireUITool;
using UnityEditor;
using UnityEngine;

public class UIColorConfigWindow : EditorWindow
{
    private int select = 0;
    private string[] names = new string[2];
    private UIColorAsset colorConfigScriptObject;
    private UIGradientAsset gradientConfigScriptObject;
    private Editor colorConfigEditor;
    private Editor gradientConfigEditor;

    private static UIColorConfigWindow c_window;
    private Vector2 scrollPos;

    private string SaveString;

    [MenuItem("ThunderFireUXTool/颜色配置 (UIColorConfig)")]
    public static void ShowObjectWindow()
    {//
        var window = GetWindow<UIColorConfigWindow>(true, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_颜色预设编辑器), true);
        window.minSize = new Vector2(550, 450);
    }

    private void OnEnable()
    {
        names[0] = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_颜色);
        names[1] = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_渐变); 
        colorConfigScriptObject = SettingAssetsUtils.GetAssets<UIColorAsset>();//AssetDatabase.LoadAssetAtPath<UIColorAsset>(UIColorConfig.ColorConfigPath + UIColorConfig.ColorConfigName + ".asset");
        gradientConfigScriptObject = SettingAssetsUtils.GetAssets<UIGradientAsset>();//AssetDatabase.LoadAssetAtPath<UIGradientAsset>(UIColorConfig.ColorConfigPath + UIColorConfig.GradientConfigName + ".asset");
        colorConfigEditor = Editor.CreateEditor(colorConfigScriptObject);

        SaveString = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_保存);

        gradientConfigEditor = Editor.CreateEditor(gradientConfigScriptObject);
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        select = GUILayout.Toolbar(select, names, GUILayout.Width(120), GUILayout.Height(25));
        //Debug.Log(select);
        if (select == 0)
        {
            colorConfigEditor.OnInspectorGUI();
        }
        else
        {
            gradientConfigEditor.OnInspectorGUI();
        }
        EditorGUILayout.EndScrollView();
        if (GUILayout.Button(SaveString))
        {
            if (select == 0)
            {
                colorConfigScriptObject.Save();
                if(ColorChooseWindow.r_window!=null){
                ColorChooseWindow.r_window.Refresh();
                }
            }
            else
            {
                gradientConfigScriptObject.Save();
                if(GradientChooseWindow.r_window!=null){
                GradientChooseWindow.r_window.Refresh();
                }
            }
            
        }
    }
}