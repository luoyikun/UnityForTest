#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ThunderFireUITool
{
    public enum WidgetInstantiateMode
    {
        None,
        Prefab,
        UnPack
    }

    /// <summary>
    /// Widget: 游戏中会重复使用的Prefab 如: 一级页签|二级页签|确认按钮 等
    /// 提供UI Style 方便统一进行风格迭代和更换
    /// </summary>
    public class WidgetRepositoryWindow : EditorWindow
    {
        private static WidgetRepositoryWindow m_window;
        public static bool clickFlag = false;

        [MenuItem("ThunderFireUXTool/组件库 (Widget Repository)", false, 54)]
        public static void OpenWindow()
        {
            int width = 1272 + 13 + 12;
            int height = 636;
            m_window = GetWindow<WidgetRepositoryWindow>();
            m_window.minSize = new Vector2(width, height);
            m_window.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_组件库);
            m_window.titleContent.image = ToolUtils.GetIcon("component_w");
        }

        static WidgetRepositoryWindow()
        {
            EditorApplication.playModeStateChanged += (obj) =>
            {
                if (HasOpenInstances<WidgetRepositoryWindow>())
                    m_window = GetWindow<WidgetRepositoryWindow>();
                if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    if (m_window)
                        m_window.RefreshWindow();
                }
            };
        }

        [UnityEditor.Callbacks.DidReloadScripts(0)]
        private static void OnScriptReload()
        {
            if (HasOpenInstances<WidgetRepositoryWindow>())
                m_window = GetWindow<WidgetRepositoryWindow>();
        }

        public static WidgetRepositoryWindow GetInstance()
        {
            return m_window;
        }
        //UI
        private VisualElement leftContainer;
        private VisualElement rightContainer;
        private ScrollView labelScroll;
        private ScrollView widgetScroll;
        private UXBuilderSlider slider;

        //Data
        private List<string> labelList;
        private List<AssetsItem> asstesItems = new List<AssetsItem>();
        private List<FileInfo> prefabInfoList;
        public string filtration = "All";
        private bool RightContainerDragIn = false;//判断拖拽操作是否是拖进来
        private bool RightContainerDrag = false;//判断拖拽的起点是否在组件库右容器
        private GameObject LoadPrefab = null;
        private Texture texture;
        private VisualTreeAsset visualTree;
        private List<AssetsItem> fliterItems = new List<AssetsItem>();

        private void OnEnable()
        {
            InitWindowUI();
            InitWindowData();
            EditorApplication.hierarchyWindowItemOnGUI += (int instanceID, Rect selectionRect) =>
            {
                if (Event.current.type == EventType.MouseDrag)
                {
                    UXCustomSceneView.ClearDelegate();
                    InitDragState();//因拖拽事件在层级面板发生重置组件库的拖拽状态
                }
            };
            EditorApplication.delayCall += RefreshWindow;
#if UNITY_2021_2_OR_NEWER
            DragAndDrop.AddDropHandler(OnHierarchyGUI);
#endif
        }
        private void OnDisable()
        {
            SceneViewToolBar.ButtonReleased();
#if UNITY_2021_2_OR_NEWER
            DragAndDrop.RemoveDropHandler(OnHierarchyGUI);
#endif
        }


        private void InitWindowData()
        {
            labelList = SettingAssetsUtils.GetAssets<PrefabLabelsSettings>().labelList;

            asstesItems.Clear();

            prefabInfoList = PrefabUtils.GetPrefabList();
            for (int i = 0; i < prefabInfoList.Count; i++)
            {
#if UNITY_2020_3_OR_NEWER
                AssetsItem item = new AssetsItem(prefabInfoList[i], false, () => EditorApplication.delayCall += RefreshWindow, slider.value / slider.highValue);
#else
                AssetsItem item = new AssetsItem(prefabInfoList[i], false, () => EditorApplication.delayCall += RefreshWindow);
#endif
                asstesItems.Add(item);
            }
            visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "ComponentLeftButton.uxml");
        }

        #region UI
        private void InitWindowUI()
        {
            //Debug.Log("InitWindowUI");
            VisualElement root = rootVisualElement;
            var row = UXBuilder.Row(root, new UXBuilderRowStruct()
            {
                style = new UXStyle() { height = Length.Percent(100) }
            });
            leftContainer = UXBuilder.Div(row, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    borderTopWidth = 10,
                    height = Length.Percent(100),
                    width = 204,
                    backgroundColor = new Color(26f / 255f, 26f / 255f, 26f / 255f)
                }
            });

            labelScroll = UXBuilder.ScrollView(leftContainer, new UXBuilderScrollViewStruct());

            var div = new VisualElement()
            {
                style =
                {
                    paddingTop = 36, paddingLeft = 36, paddingRight = 36, paddingBottom = 36,
                    height = Length.Percent(100),
                }
            };
            row.Add(div);

            rightContainer = UXBuilder.Div(div, new UXBuilderDivStruct());
            div.Add(rightContainer);
            //检测rightContainer拖入和拖出改变光标形态
            rightContainer.RegisterCallback((DragEnterEvent e) =>
            {
                RightContainerDragIn = true;
            });
#if UNITY_2020_3_OR_NEWER
            slider = UXBuilder.Slider(rightContainer, new UXBuilderSliderStruct()
            {
                style = new UXStyle()
                {
                    position = Position.Absolute,
                    width = 50,
                    right = 20,
                    top = -30
                },
                onChange = OnSliderValueChanged
            });
            slider.value = slider.highValue;
#endif
            // rightContainer.RegisterCallback<GeometryChangedEvent, KeyValuePair<TextElement, VisualElement>>(MyOverflowEvent,
            //     new KeyValuePair<TextElement, VisualElement>(label, label));
            root.RegisterCallback<GeometryChangedEvent, KeyValuePair<VisualElement, VisualElement>>(geometryChangedEvent,
                new KeyValuePair<VisualElement, VisualElement>(div, root));
            ChangeScrollView();
        }

        private void ChangeScrollView()
        {
            if (widgetScroll != null) rightContainer.Remove(widgetScroll);
            widgetScroll = UXBuilder.ScrollView(rightContainer,
                new UXBuilderScrollViewStruct() { style = new UXStyle() { whiteSpace = WhiteSpace.NoWrap } });
            var ve = widgetScroll.contentContainer;
            ve.style.flexDirection = FlexDirection.Row;
            ve.style.flexWrap = Wrap.Wrap;
            ve.style.overflow = Overflow.Visible;
            widgetScroll.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (clickFlag)
                {
                    clickFlag = false;
                }
                else
                {
                    foreach (var t in fliterItems)
                    {
                        t.SetSelected(false);
                    }
                }
            });
            widgetScroll.RegisterCallback<DragPerformEvent>(evt =>
            {
                // Debug.Log("yeah!!!");
                DoPrefabDrag();
                RightContainerDrag = false;
            });
        }

        private void OnSliderValueChanged(float x)
        {
#if UNITY_2020_3_OR_NEWER
            if (x / slider.highValue < 0.2f)
            {
                slider.value = 0;
            }
            widgetScroll.contentContainer.style.flexDirection = slider.value == 0 ? FlexDirection.Column : FlexDirection.Row;
#if UNITY_2021_3_OR_NEWER
            widgetScroll.mode = slider.value == 0 ? ScrollViewMode.Vertical : ScrollViewMode.Horizontal;
#endif
            ChangeScrollView();
            RefreshRightPrefabContainer();
#endif
        }

        private static void geometryChangedEvent(GeometryChangedEvent evt, KeyValuePair<VisualElement, VisualElement> pair)
        {
            pair.Key.style.width = pair.Value.resolvedStyle.width - 204;
            pair.Key.UnregisterCallback<GeometryChangedEvent, KeyValuePair<VisualElement, VisualElement>>(geometryChangedEvent);
        }

        public void RefreshWindow()
        {
            //Debug.Log("RefreshWindow");
            RefreshLeftTypeList();
            RefreshRightPrefabContainer();
        }

        /// <summary>
        /// 更新左侧类型列表
        /// </summary>
        private void RefreshLeftTypeList()
        {
            labelScroll.Clear();
            //列表最开始创建一个 全部 按钮
            UXBuilder.Button(labelScroll, new UXBuilderButtonStruct()
            {
                type = ButtonType.Default,
                OnClick = () =>
                {
                    RefreshRightPrefabContainer("All");
                    EditorApplication.delayCall += RefreshWindow;
                },
                style = new UXStyle()
                {
                    width = 180,
                    height = 35,
                    fontSize = 15,
                    marginLeft = 12,
                    marginTop = 12,
                    backgroundColor =
                        filtration == "All" ? new Color(36f / 255f, 99f / 255f, 193f / 255f) : Color.white,
                    color = filtration == "All"
                        ? Color.white
                        : new Color(51f / 255f, 51f / 255f, 51f / 255f),
                },
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_全部类型),
            });


            //添加全部 类型 按钮
            for (int i = 0; i < labelList.Count; i++)
            {
                var tmp = labelList[i];
                var btn = UXBuilder.Button(labelScroll, new UXBuilderButtonStruct()
                {
                    type = ButtonType.Default,
                    OnClick = () =>
                    {
                        RefreshRightPrefabContainer(tmp);
                        EditorApplication.delayCall += RefreshWindow;
                    },
                    style = new UXStyle()
                    {
                        width = 180,
                        height = 35,
                        // fontSize = 14,
                        marginLeft = 12,
                        marginTop = 6,
                        fontSize = 14,
                        overflow = Overflow.Hidden,
                        backgroundColor =
                            filtration == tmp ? new Color(36f / 255f, 99f / 255f, 193f / 255f) : Color.white,
                        color = filtration == tmp
                            ? Color.white
                            : new Color(51f / 255f, 51f / 255f, 51f / 255f),
                    },
                    text = tmp,
                });
                btn.tooltip = tmp;
                UIElementUtils.TextOverflowWithEllipsis(btn);
                btn.RegisterCallback<MouseDownEvent, string>(OnLeftTypeButtonRightClicked, tmp);
            }

            //列表最后添加一个 + 按钮

            UXBuilder.Button(labelScroll, new UXBuilderButtonStruct()
            {
                OnClick = () =>
                {
                    OnAddNewLabelButtonClicked();
                    EditorApplication.delayCall += RefreshWindow;
                },
                style = new UXStyle()
                {
                    width = 180,
                    height = 35,
                    fontSize = 20,
                    marginLeft = 12,
                    marginTop = 6,
                    backgroundColor = Color.white,
                    color = new Color(51f / 255f, 51f / 255f, 51f / 255f),
                },
                text = "+",
            });
        }

        /// <summary>
        /// 根据当前所选类型更新右侧Prefab列表
        /// </summary>
        /// <param name="type"></param>
        private void RefreshRightPrefabContainer(string type = null)
        {
            //Debug.Log("RefreshRightPrefabContainer");
            InitWindowData();
            //Debug.Log("RefreshRightPrefabContainer");
            if (type != null)
            {
                filtration = type;
            }
            widgetScroll.Clear();

            if (filtration == ThunderFireUIToolConfig.PrefabRepoDefaultType)
            {
                fliterItems = asstesItems;
            }
            else
            {
                fliterItems = asstesItems.Where(item => item.labels.Contains(filtration)).ToList();
            }

            if (fliterItems.Count == 0) return;
            // widgetScroll.contentContainer.style.height =
            //     Mathf.CeilToInt((float)asstesItems.Count / (float)columnCount) * (height) + 8;
            for (int i = 0; i < fliterItems.Count; i++)
            {
                int tmp = i;
                fliterItems[i].RegisterCallback((MouseDownEvent e) =>
                {
                    for (int j = 0; j < fliterItems.Count; j++)
                        fliterItems[j].SetSelected(false);
                    fliterItems[tmp].SetSelected(true);
                    InitDragState();
                    RightContainerDrag = true;//拖拽起始点在右容器
                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.StartDrag("prefab");
                    LoadPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(fliterItems[tmp].path);
                    string guid = AssetDatabase.AssetPathToGUID(fliterItems[tmp].path);
                    texture = Utils.GetAssetsPreviewTexture(guid);
                    Object[] obj = { LoadPrefab };
                    DragAndDrop.objectReferences = obj;
                    //SceneView.duringSceneGui -= CustomScene;
                    //SceneView.duringSceneGui += CustomScene;
                    UXCustomSceneView.ClearDelegate();
                    UXCustomSceneView.AddDelegate(CustomScene);
                    //EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
                    //fliterItems[i].nameLabel.schedule.Execute(() => fliterItems[i].nameLabel.SetTextWithEllipsis(CustomTextElement.CountCharacterMode.WithNumOfPixels));
                });
                widgetScroll.Add(fliterItems[i]);
            }
            Repaint();
        }

        private void ReLayoutRightContainer()
        {
            //Debug.Log("ReLayoutRightContainer");
            if (asstesItems.Count == 0) return;
            List<FileInfo> tmp1 = PrefabUtils.GetPrefabList();
            //窗口大小是否影响组件排版||是否有新的组件增加
            if (tmp1.Count != prefabInfoList.Count)
            {
                EditorApplication.delayCall += RefreshWindow;
            }
        }
        #endregion


        #region UI Event
        private void OnLeftTypeButtonRightClicked(MouseDownEvent e, string label)
        {
            if (e.button != 1) return;

            var menu = new GenericMenu();

            menu.AddItem(new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_修改信息)), false, () =>
            {
                PrefabLabelModifyWindow.OpenWindow(label, (currentName) => EditorApplication.delayCall += () =>
                  {
                      if (filtration == label)
                          filtration = currentName;
                      RefreshWindow();
                  });
            });

            menu.AddItem(new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_删除)), false, () =>
            {
                PrefabUtils.DeleteLabel(label);
                EditorApplication.delayCall += RefreshWindow;
            });
            menu.ShowAsContext();
        }

        private void OnAddNewLabelButtonClicked()
        {
            PrefabAddNewLabelWindow.OpenWindow(OnAddLabelSuccess);
            //PrefabAddNewLabelWindow.OpenWindow();
        }

        private void OnAddLabelSuccess(string newLabel)
        {
            InitWindowData();
            EditorApplication.delayCall += RefreshWindow;
        }
        #endregion


        /// <summary>
        /// 重置拖拽状态，设置为：拖拽起始点是不在组件库，并且没有拖拽进入组件库的事件发生。
        /// </summary>
        private void InitDragState()
        {
            //RightContainerDragOut = false;
            RightContainerDragIn = false;
            RightContainerDrag = false;
            LoadPrefab = null;
        }
        private void OnGUI()
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            //从层级面板拖入rightcontainer后生成新建prefab文件
            Event e = Event.current;
            switch (e.type)
            {
                case EventType.Layout:
                    ReLayoutRightContainer();
                    //Debug.Log("Layout");
                    break;
                case EventType.MouseEnterWindow:
                    //Debug.Log("MouseEnterWindow");
                    break;
                case EventType.MouseLeaveWindow:
                    //Debug.Log("MouseLeaveWindow");
                    break;
            }
            //Debug.Log(Event.current.mousePosition);
        }

        /// <summary>
        /// override视图中的拖拽
        /// </summary>
        public void UnpackPrefab(bool isPack, GameObject currentPrefab)
        {
            if (!isPack)
            {
                PrefabUtility.UnpackPrefabInstance(currentPrefab, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }
        }

#if UNITY_2021_2_OR_NEWER
        private DragAndDropVisualMode OnHierarchyGUI(int dropTargetInstanceID, HierarchyDropFlags dropMode, Transform parentForDraggedObjects, bool perform)
        {
            if (RightContainerDrag)
            {
                if (perform)
                {
                    GameObject obj = (GameObject)EditorUtility.InstanceIDToObject(dropTargetInstanceID);
                    //UXCustomSceneView.RemoveDelegate(DrawTexture);
                    GameObject currentPrefab = PrefabUtility.InstantiatePrefab(LoadPrefab) as GameObject;
                    bool isPack = AssetDatabase.GetLabels(LoadPrefab).Contains("Pack");
                    if (dropMode.HasFlag(HierarchyDropFlags.DropUpon))
                    {
                        if (dropTargetInstanceID != UnityEngine.SceneManagement.SceneManager.GetActiveScene().handle)
                        {
                            currentPrefab.transform.SetParent(obj.transform);
                        }
                        UnpackPrefab(isPack, currentPrefab);
                    }
                    else if (dropMode.HasFlag(HierarchyDropFlags.DropAfterParent))
                    {
                        if (dropTargetInstanceID != UnityEngine.SceneManagement.SceneManager.GetActiveScene().handle)
                            currentPrefab.transform.SetParent(obj.transform.parent);
                        currentPrefab.transform.SetAsFirstSibling();
                        UnpackPrefab(isPack, currentPrefab);
                    }
                    else if (dropMode.HasFlag(HierarchyDropFlags.DropBetween))
                    {
                        if (dropTargetInstanceID != UnityEngine.SceneManagement.SceneManager.GetActiveScene().handle)
                        {
                            currentPrefab.transform.SetParent(obj.transform.parent);
                            currentPrefab.transform.SetSiblingIndex(obj.transform.GetSiblingIndex() + 1);
                        }
                        UnpackPrefab(isPack, currentPrefab);
                    }
                    else if (dropMode.HasFlag(HierarchyDropFlags.SearchActive))
                    {
                        if (dropTargetInstanceID != UnityEngine.SceneManagement.SceneManager.GetActiveScene().handle)
                            currentPrefab.transform.SetParent(obj.transform);
                        UnpackPrefab(isPack, currentPrefab);
                    }
                    else if (dropMode.HasFlag(HierarchyDropFlags.DropAbove))
                    {
                        currentPrefab.transform.SetAsFirstSibling();
                        UnpackPrefab(isPack, currentPrefab);
                    }

                    Selection.activeObject = currentPrefab;
                }
                return DragAndDropVisualMode.Copy;
            }
            return DragAndDrop.visualMode;
        }
#endif
        private void CustomScene(SceneView sceneView)
        {
            if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                UXCustomSceneView.RemoveDelegate(DrawTexture);
                UXCustomSceneView.AddDelegate(DrawTexture);

                if (Event.current.type == EventType.DragPerform)
                {
                    Vector2 mousePos = Event.current.mousePosition;
                    Transform container = FindContainerLogic.GetObjectParent(Selection.gameObjects);

                    if (container != null)
                    {
                        mousePos.y = sceneView.camera.pixelHeight - mousePos.y;
                        Vector3 WorldPos = sceneView.camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
                        Vector3 localPos = container.InverseTransformPoint(new Vector3(WorldPos.x, WorldPos.y, 0));
                        bool isPack = AssetDatabase.GetLabels(LoadPrefab).Contains("Pack");
                        if (isPack)
                        {
                            DragPerformAsPrefab(container, localPos);
                        }
                        else
                        {
                            DragPerformAsUnPack(container, localPos);
                        }

                        // var PrefabSetting = SettingAssetsUtils.GetAssets<PrefabDefaultSetting>();
                        // if (PrefabSetting.widgetInsMode == WidgetInstantiateMode.None)
                        // {
                        //     ShowChoosePrefabInitTypeDialog(container, localPos);
                        // }
                        // else if (PrefabSetting.widgetInsMode == WidgetInstantiateMode.Prefab)
                        // {
                        //     //DragPerformAsPrefab(container, localPos);
                        // }
                        // else if (PrefabSetting.widgetInsMode == WidgetInstantiateMode.UnPack)
                        // {
                        //     DragPerformAsUnPack(container, localPos);
                        // }
                    }
                    UXCustomSceneView.RemoveDelegate(DrawTexture);
                    UXCustomSceneView.RemoveDelegate(CustomScene);
                }
                Event.current.Use();
            }
        }

        private void ShowChoosePrefabInitTypeDialog(Transform container, Vector3 localPos)
        {
            int option = EditorUtility.DisplayDialogComplex(
                EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_添加Prefab),
                EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_请选择添加方式),
                "prefab", "unpack prefab", "cancel");

            switch (option)
            {
                case 0:
                    {
                        DragPerformAsPrefab(container, localPos);
                        break;
                    }
                case 1:
                    {
                        DragPerformAsUnPack(container, localPos);
                        break;
                    }
                case 2:
                    {
                        break;
                    }
            }
        }

        private void DragPerformAsPrefab(Transform container, Vector3 localPos)
        {
            GameObject currentPrefab = PrefabUtility.InstantiatePrefab(LoadPrefab) as GameObject;
            currentPrefab.transform.SetParent(container);
            currentPrefab.transform.localPosition = localPos;
            Selection.activeObject = currentPrefab;
        }

        private void DragPerformAsUnPack(Transform container, Vector3 localPos)
        {
            GameObject currentPrefab = PrefabUtility.InstantiatePrefab(LoadPrefab) as GameObject;
            currentPrefab.transform.SetParent(container);
            currentPrefab.transform.localPosition = localPos;
            Selection.activeObject = currentPrefab;
            PrefabUtility.UnpackPrefabInstance(currentPrefab, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        }

        private void DrawTexture(SceneView sceneView)
        {
            if (OutBounds(sceneView))
            {
                UXCustomSceneView.RemoveDelegate(DrawTexture);
            }
            Handles.BeginGUI();
            GUI.DrawTexture(new Rect(Event.current.mousePosition.x - texture.width / 2, Event.current.mousePosition.y - texture.height / 2, texture.width, texture.height), texture);
            Handles.EndGUI();
            sceneView.Repaint();
        }

        private bool OutBounds(SceneView sceneView, float offset = 0f)
        {
            if (Event.current.mousePosition.y < sceneView.camera.pixelHeight + offset && Event.current.mousePosition.y > 0 && Event.current.mousePosition.x < sceneView.camera.pixelWidth && Event.current.mousePosition.x > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void DoPrefabDrag()
        {
            if (RightContainerDragIn && !RightContainerDrag)
            {
                // foreach(var t in DragAndDrop.paths)
                List<GameObject> dragObjList = new List<GameObject>();
                foreach (var t in DragAndDrop.objectReferences)
                {
                    // Debug.LogWarning(t.name);
                    GameObject obj = t as GameObject;
                    if (obj != null)
                    {
                        dragObjList.Add(obj);
                    }
                }
                if (dragObjList.Count == 1)
                {
                    string path =
                        PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(DragAndDrop.objectReferences[0]);
                    if (path != "" && path != null)
                        PrefabCreateWindow.OpenWindow(dragObjList, path);
                    else
                        PrefabCreateWindow.OpenWindow(dragObjList);
                }
                else
                {
                    if (CombineWidgetLogic.CanCombine(dragObjList.ToArray()))
                        PrefabCreateWindow.OpenWindow(dragObjList);
                    else
                    {
                        EditorUtility.DisplayDialog("messageBox",
                            EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_Prefab创建失败Tip),
                            EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                            EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                    }
                }

                InitDragState();

            }
        }
    }

}
#endif
