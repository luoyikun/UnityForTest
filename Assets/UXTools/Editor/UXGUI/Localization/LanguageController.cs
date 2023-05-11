using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System.Collections.Generic;
using ThunderFireUITool;

[InitializeOnLoad]
public class LanguageController
{
    private static PopupField<string> languages;

    static LanguageController()
    {
        EditorApplication.playModeStateChanged += (PlayModeStateChange obj) =>
        {
            if(languages == null)
            {
                List<string> choices = new List<string>();
                choices.Add(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_显示key));
                choices.Add(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_无文字模式));
                choices.Add(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_游戏内语言));
                foreach (int i in UXGUIConfig.availableLanguages)
                {
                    choices.Add(LocalizationLanguage.GetLanguage(i));
                }
                languages = new PopupField<string>(choices, 2);
                languages.style.position = Position.Absolute;
                languages.style.top = 20;
                languages.style.right = 0;
                languages.style.width = 110;
                languages.style.marginRight = 0;
                languages.index = 2;
                languages.RegisterValueChangedCallback(x =>
                {
                    if(x.newValue == choices[0])
                    {
                        
                        LocalizationHelper.SetPreviewLanguage(-3);
                        return;
                    }
                    if(x.newValue == choices[1])
                    {
                        LocalizationHelper.SetPreviewLanguage(-2);
                        return;
                    }
                    if(x.newValue == choices[2])
                    {
                        LocalizationHelper.SetPreviewLanguage(-1);
                        return;
                    }
                    foreach (int i in UXGUIConfig.availableLanguages)
                    {
                        if (x.newValue == LocalizationLanguage.GetLanguage(i))
                        {
                            LocalizationHelper.SetPreviewLanguage(i);
                            break;
                        }
                    }
                });
            }

            if (obj == PlayModeStateChange.EnteredPlayMode)
            {
                var gameViews = Utils.GetPlayViews();
                foreach (EditorWindow gameView in gameViews)
                {
                    gameView.rootVisualElement.Add(languages);
                }
            }
            else if (obj == PlayModeStateChange.ExitingPlayMode)
            {
                var gameViews = Utils.GetPlayViews();
                foreach (EditorWindow gameView in gameViews)
                {
                    if(gameView.rootVisualElement.Contains(languages))
                    {
                        gameView.rootVisualElement.Remove(languages);
                    }
                }
            }
        };
    }

    public static void ShowPanel()
    {
        var gameViews = Utils.GetPlayViews();
        foreach (EditorWindow gameView in gameViews)
        {
            gameView.rootVisualElement.Add(languages);
        }
    }

    public static void HidePanel()
    {
        var gameViews = Utils.GetPlayViews();
        foreach (EditorWindow gameView in gameViews)
        {
            if(gameView.rootVisualElement.Contains(languages))
            {
                gameView.rootVisualElement.Remove(languages);
            }
        }
    }
}