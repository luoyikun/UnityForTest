#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

namespace ThunderFireUITool
{

    [InitializeOnLoad]
    public class SceneViewToolBar : EditorWindow
    {
        private static VisualElement toolBarPanel;
        // TODO Temp Public for locationline layer, need a manager for layer
        public static VisualElement toolbarBg;
        private static VisualElement moreOptionPanel;

        private static ResolutionController resolutionController;

        private static EditorLogic editorLogic;
        private static UITButton ButtonBeOccupied;

        private static bool onText;
        private static bool onImage;
        private static bool OverPanel;
        private static bool OverToolBar;
        public static bool HaveToolbar;

        static SceneViewToolBar()
        {
            //检查和恢复recompile之后的toolbar状态
            TryOpenToolbar();
            EditorApplication.update += InitFunction;
        }

        public static void InitFunction()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null) return;
            editorLogic = new EditorLogic();
            editorLogic.Init();
            PrefabTabs.InitPrefabTabs();
            ResolutionController.InitResolutionController();
            if (ResolutionController.loaded)
            {
                sceneView.rootVisualElement.Add(ResolutionController.Root);
            }
            EditorApplication.update -= InitFunction;
        }

        public static void CloseFunction()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null) return;
            if (sceneView.rootVisualElement.Contains(PrefabTabs.prefabTabsPanel))
            {
                sceneView.rootVisualElement.Remove(PrefabTabs.prefabTabsPanel);
            }
            if (ResolutionController.loaded)
            {
                sceneView.rootVisualElement.Remove(ResolutionController.Root);
            }
            editorLogic.Close();
        }

        public static void CloseEditor()
        {
            if (HaveToolbar)
            {
                SceneView sceneView = SceneView.lastActiveSceneView;
                sceneView.rootVisualElement.Remove(toolbarBg);
                HaveToolbar = false;
            }
        }

        public static void OpenEditor()
        {
            //在播放预制体的时候打开编辑器，不修改判断播放预制体的值(临时处理手法)
            if (EditorApplication.isPlaying != true)
            {
                PlayerPrefs.SetString("previewStage", "false");
            }
            if (!HaveToolbar)
            {
                InitToolBar();
                HaveToolbar = true;
            }

            UXSceneViewCursor.Instance.Init();
            SceneView.lastActiveSceneView.in2DMode = true;
        }

        [MenuItem(ThunderFireUIToolConfig.Menu_OpenEditor, false, 101)]
        public static void SwitchEditor()
        {
            bool flag = Menu.GetChecked(ThunderFireUIToolConfig.Menu_OpenEditor);
            Menu.SetChecked(ThunderFireUIToolConfig.Menu_OpenEditor, !flag);
            EditorPrefs.SetBool("EditorOpen", !flag);

            if (!flag)
            {
                //点击之后变为开启状态
                OpenEditor();
            }
            else
            {
                //点击之后变为关闭状态
                CloseEditor();
            }
        }

        [MenuItem(ThunderFireUIToolConfig.Menu_OpenEditor, true)]
        public static bool CheckToolBarState()
        {
            Menu.SetChecked(ThunderFireUIToolConfig.Menu_OpenEditor, HaveToolbar);
            return true;
        }

        public static void TryOpenToolbar()
        {
            //检查EditorOpen是否开启 
            //检查是否已经有一个实例了
            //检查是否有SceneView

            bool open = EditorPrefs.GetBool("EditorOpen", false);
            if (open && !HaveToolbar && SceneView.lastActiveSceneView != null)
            {
                OpenEditor();
            }
        }
        private static void InitToolBar()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;

            if (sceneView == null) return;

            VisualTreeAsset toolbarTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "toolbar.uxml");

            toolbarBg = toolbarTreeAsset.CloneTree().Children().First();
            toolBarPanel = toolbarBg.Q<VisualElement>("toolbar");
            moreOptionPanel = toolbarBg.Q<VisualElement>("morePanel");

            toolBarPanel.style.alignSelf = Align.Center;
            moreOptionPanel.style.display = DisplayStyle.None;

            sceneView.rootVisualElement.Add(toolbarBg);
            toolbarBg.style.position = Position.Absolute;
            toolbarBg.style.bottom = 0;

            toolbarBg.BringToFront();

            createImageButton();
            createTextButton();
            new UITButton(toolbarBg.Q<VisualElement>("widgetRepo"), WidgetRepositoryWindow.OpenWindow, null, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_组件库));
            new UITButton(toolbarBg.Q<VisualElement>("createWidget"), PrefabCreateWindow.OpenWindow, null, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_新建组件));
            new UITButton(toolbarBg.Q<VisualElement>("play"), PreviewLogic.Preview, null, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_预览));
            //new UITButton(toolbarBg.Q<VisualElement>("qucikBg"), QuickBackground.CreateBackGround, null, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_上传));
            new UITButton(toolbarBg.Q<VisualElement>("clock"), PrefabRecentWindow.OpenWindow, null, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_最近打开的模板));
            new UITButton(toolbarBg.Q<VisualElement>("more"), AddMorePanel, RemoveMorePanel, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_更多));


            VisualElement quickBgBtn = toolbarBg.Q<VisualElement>("qucikBg");
            quickBgBtn.tooltip = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_背景图在Hierarchy中显示名称);
            quickBgBtn.RegisterCallback((MouseDownEvent e) =>
            {
                QuickBackground.CreateBackGround();
            });
            quickBgBtn.RegisterCallback((MouseEnterEvent e) =>
            {
                quickBgBtn.style.backgroundColor = new Color(0.14f, 0.39f, 0.76f, 1f);
                quickBgBtn.Q<VisualElement>("Icon").style.backgroundColor = new Color(0.14f, 0.39f, 0.76f, 1f);
                quickBgBtn.Q<VisualElement>("Icon").style.backgroundImage = (StyleBackground)ToolUtils.GetIcon("ToolBar/quickbackground_white");
            });
            quickBgBtn.RegisterCallback((MouseLeaveEvent e) =>
            {
                quickBgBtn.style.backgroundColor = Color.white;
                quickBgBtn.Q<VisualElement>("Icon").style.backgroundColor = Color.white;
                quickBgBtn.Q<VisualElement>("Icon").style.backgroundImage = (StyleBackground)ToolUtils.GetIcon("ToolBar/quickbackground_black");
            });



            VisualElement guideLineBtn = toolbarBg.Q<VisualElement>("guideLine");
            guideLineBtn.tooltip = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_辅助线);
            guideLineBtn.RegisterCallback((MouseDownEvent e) =>
            {
                LocationLineLogic.Instance.CreateLocationLine();
            });
            RegisterMouseHover(guideLineBtn);


            new UIMButton(moreOptionPanel.Q<VisualElement>("setting"), ConfigurationWindow.OpenWindow, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_设置));
            new UIMButton(moreOptionPanel.Q<VisualElement>("about"), AboutWindow.OpenWindow, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_关于));


            VisualElement HideBarBtn = toolbarBg.Q<VisualElement>("HideBtn");
            VisualElement ShowBarBtn = toolbarBg.Q<VisualElement>("ShowBtn");

            HideBarBtn.RegisterCallback((MouseDownEvent e) =>
            {
                toolBarPanel.style.visibility = Visibility.Hidden;
                HideBarBtn.style.visibility = Visibility.Hidden;
                ShowBarBtn.style.visibility = Visibility.Visible;
                toolbarBg.style.bottom = -18f;
            });

            ShowBarBtn.RegisterCallback((MouseDownEvent e) =>
            {
                toolBarPanel.style.visibility = Visibility.Visible;
                HideBarBtn.style.visibility = Visibility.Visible;
                ShowBarBtn.style.visibility = Visibility.Hidden;
                toolbarBg.style.bottom = 0;
            });

            toolbarBg.RegisterCallback((PointerOverEvent e) =>
            {
                OverPanel = true;
            });
            toolbarBg.RegisterCallback((PointerOutEvent e) =>
            {
                OverPanel = false;
            });
            toolBarPanel.RegisterCallback((PointerOverEvent e) =>
            {
                OverToolBar = true;
            });
            toolBarPanel.RegisterCallback((PointerOutEvent e) =>
            {
                OverToolBar = false;
            });
        }

        private static void RegisterMouseHover(VisualElement element)
        {
            element.RegisterCallback((MouseEnterEvent e) =>
            {
                element.style.backgroundColor = new Color(0.14f, 0.39f, 0.76f, 1f);
                element.style.unityBackgroundImageTintColor = Color.white;
            });
            element.RegisterCallback((MouseLeaveEvent e) =>
            {
                element.style.backgroundColor = Color.white;
                element.style.unityBackgroundImageTintColor = Color.black;
            });
        }

        private static void createImageButton()
        {
            VisualElement image = toolBarPanel.Q<VisualElement>("image");
            image.Clear();
            image.style.backgroundColor = Color.white;
            new UITButton(image, () => { UXSceneViewCursor.Instance.StartQuickCreate("Image"); }, UXSceneViewCursor.Instance.StopQuickCreate, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_创建图片));
            image.RegisterCallback((PointerOverEvent e) =>
            {
                onImage = true;
            });
            image.RegisterCallback((PointerOutEvent e) =>
            {
                onImage = false;
            });

        }

        private static void createTextButton()
        {
            VisualElement text = toolBarPanel.Q<VisualElement>("text");
            text.Clear();
            text.style.backgroundColor = Color.white;
            new UITButton(text, () => { UXSceneViewCursor.Instance.StartQuickCreate("Text"); }, UXSceneViewCursor.Instance.StopQuickCreate, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_创建文字));
            text.RegisterCallback((PointerOverEvent e) =>
            {
                onText = true;
            });
            text.RegisterCallback((PointerOutEvent e) =>
            {
                onText = false;
            });

        }


        public static bool DrawState(string name)//避免拖拽从按钮上直接开始
        {
            if (name.Equals("UXImage"))
            {
                return onImage;
            }
            else if (name.Equals("UXText"))
            {
                return onText;
            }
            else
            {
                return false;
            }
        }

        public static void ButtonReleased()
        {
            if (ButtonBeOccupied != null)
            {
                ButtonBeOccupied.nextClick();
                ButtonBeOccupied.UnSelected();
                ButtonBeOccupied = null;
                RemoveMorePanel();
            }
        }

        public static void AddMorePanel()
        {
            moreOptionPanel.style.display = DisplayStyle.Flex;
        }

        public static void RemoveMorePanel()
        {
            moreOptionPanel.style.display = DisplayStyle.None;
        }

        public static bool OverRange()
        {
            return OverPanel || OverToolBar;
        }

        public static UITButton getButtonBeOccupied()
        {
            return ButtonBeOccupied;
        }

        public static void setButtonBeOccupied(UITButton v)
        {
            ButtonBeOccupied = v;
        }
    }
}
#endif