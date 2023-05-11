#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using UnityEngine.SceneManagement;
using Cursor = UnityEngine.Cursor;

namespace ThunderFireUITool
{
    public class PrefabCreateWindow : EditorWindow
    {

        private static PrefabCreateWindow m_window;
        private static GameObject[] selectRectList;
        private VisualElement PopupField;
        private VisualElement PopupField2;
        private PopupField<string> normalField;
        private PopupField<string> normalField2;
        private static string currentName = null;
        static List<string> TypeNames;
        private static VisualElement labelFromUXML;
        private IMGUIContainer PosInput;
        private static string componentPath;
        private static bool isPrefab;

        private static string defaultType = "All";

        [MenuItem("Assets/设置为组件 (Set As UXWidget)", false, -800)]
        static void SetAsComponent()
        {
            string[] guids = Selection.assetGUIDs;
            if (guids.Length > 1)
            {
                EditorUtility.DisplayDialog("messageBox",
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_选择多个PrefabTip),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                return;
            }
            foreach (var guid in guids)
            {
                if (!AssetDatabase.GUIDToAssetPath(guid).EndsWith(".prefab"))
                {
                    EditorUtility.DisplayDialog("messageBox",
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_选择文件不是PrefabTip),
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                    return;
                }
            }
            foreach (var guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                OpenWindow(Selection.gameObjects.ToList(), assetPath);
                // Utils.AddGUID(assetPath);

            }
        }

        static PrefabCreateWindow()
        {
            EditorApplication.playModeStateChanged += (obj) =>
            {
                if (HasOpenInstances<PrefabCreateWindow>())
                    m_window = GetWindow<PrefabCreateWindow>();
                if (EditorApplication.isPaused)
                {
                    return;
                }
                if (!(EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode))
                {
                    return;
                }
                if (m_window != null)
                    m_window.CloseWindow();
            };
        }

        public static void OpenWindow()
        {
            if (Selection.gameObjects.Length == 0)
            {
                string message = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_请先选中一个节点);
                EditorUtility.DisplayDialog("messageBox", message, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                SceneViewToolBar.ButtonReleased();
            }
            else
            {
                int width = 383;
                int height = 247;
                selectRectList = Selection.gameObjects;
                if (Selection.gameObjects.Length == 1)
                {
                    string path =
                        PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(selectRectList[0]);
                    if (path != "" && path != null)
                    {
                        string[] strings = path.Split('/');
                        string name = strings[strings.Length - 1].Split('.')[0];
                        string realPath = path.Substring(0, path.LastIndexOf('/'));
                        currentName = name;
                        componentPath = realPath + "/";
                        isPrefab = true;
                    }
                    else
                    {
                        currentName = "";
                        componentPath = "";
                        isPrefab = false;
                    }
                }
                else
                {
                    if (!CombineWidgetLogic.CanCombine(selectRectList))
                    {
                        EditorUtility.DisplayDialog("messageBox",
                            EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_Prefab创建失败Tip),
                            EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                            EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                        SceneViewToolBar.ButtonReleased();
                        return;
                    }
                }
                InitWindowData();
                m_window = GetWindow<PrefabCreateWindow>();
                m_window.minSize = new Vector2(width, height);
                m_window.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_新建组件);
                // m_window.position = new Rect((Screen.currentResolution.width - width) / 2, (Screen.currentResolution.height - height) / 2, width, height);
                m_window.titleContent.image = ToolUtils.GetIcon("createPrefab_w");
            }
        }

        public static void OpenWindow(List<GameObject> objList)
        {
            int width = 500;
            int height = 200;
            selectRectList = objList.ToArray();
            InitWindowData();
            m_window = GetWindow<PrefabCreateWindow>();
            m_window.minSize = new Vector2(width, height);
            m_window.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_新建组件);
            // m_window.position = new Rect((Screen.currentResolution.width - width) / 2, (Screen.currentResolution.height - height) / 2, width, height);
        }

        public static void OpenWindow(List<GameObject> objList, string path)
        {
            // Debug.Log("open with path");
            int width = 500;
            int height = 200;
            selectRectList = objList.ToArray();
            string[] strings = path.Split('/');
            string name = strings[strings.Length - 1].Split('.')[0];
            string realPath = path.Substring(0, path.LastIndexOf('/'));
            currentName = name;
            componentPath = realPath + "/";
            isPrefab = true;
            InitWindowData();
            m_window = GetWindow<PrefabCreateWindow>();
            m_window.minSize = new Vector2(width, height);
            m_window.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_新建组件);
            // m_window.position = new Rect((Screen.currentResolution.width - width) / 2, (Screen.currentResolution.height - height) / 2, width, height);
        }

        private void OnEnable()
        {
            InitWindowData();
            // currentName = "";
            VisualElement root = rootVisualElement;
            Label TextLabel = labelFromUXML.Q<Label>("textlabel");
            Label PosLabel = labelFromUXML.Q<Label>("poslabel");
            Button PosButton = labelFromUXML.Q<Button>("posbutton");
            IMGUIContainer TextInput = labelFromUXML.Q<IMGUIContainer>("textinput");
            VisualElement Confirm = labelFromUXML.Q<VisualElement>("confirm");
            VisualElement Cancel = labelFromUXML.Q<VisualElement>("cancel");

            PosInput = labelFromUXML.Q<IMGUIContainer>("posinput");
            PosInput.SetEnabled(false);
            GUIStyle style2 = new GUIStyle();
            style2.normal.textColor = Color.black;
            style2.fontSize = 12;
            PosInput.onGUIHandler += () => { componentPath = EditorGUILayout.TextField(componentPath, style2); };
            // PosInput.style.cursor = ;


            PopupField = labelFromUXML.Q<VisualElement>("popupfield");
            PopupField2 = labelFromUXML.Q<VisualElement>("popupfield2");
            TextLabel.text = prefabNameDes;
            PosLabel.text = prefabPosDes;
            PosButton.clicked += SelectComponentPath;

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.black;
            style.fontSize = 15;
            //TextInput.onGUIHandler += () => { currentName = GUI.TextField(new Rect(2.5f, 2.5f, 201, 20), currentName, style); };
            TextInput.onGUIHandler += () => { currentName = EditorGUILayout.TextField(currentName, style); };
            SelectorItem textinputS = new SelectorItem(labelFromUXML.Q<VisualElement>("textinputSelector"), TextInput);
            //EditorUIUtil.CreateUIEButton(Confirm, Submit);
            //EditorUIUtil.CreateUIEButton(Cancel, CloseWindow);

            if (isPrefab)
            {
                PosButton.SetEnabled(false);
                TextInput.SetEnabled(false);
            }
            RefreshPopupField();

            PopupField2.Clear();
            List<string> packNames = new List<string>();
            packNames.Add("Pack");
            packNames.Add("UnPack");
            normalField2 = new PopupField<string>(packNames, "Pack");
            normalField2.RegisterValueChangedCallback(x => ChangeValue(x.newValue));
            normalField2.style.position = Position.Absolute;
            normalField2.style.left = 0;
            normalField2.style.right = 0;
            normalField2.style.top = 0;
            normalField2.style.bottom = 0;
            PopupField2.Add(normalField2);

            labelFromUXML.Q<Label>("poptext").text = prefabTypeDes;
            labelFromUXML.Q<Label>("poptext2").text = prefabPackDes;
            Confirm.Q<Label>("text").text = OKText;
            Cancel.Q<Label>("text").text = CancelText;

            new SelectorItem(labelFromUXML.Q<VisualElement>("cancelSelector"), Cancel, false);
            new SelectorItem(labelFromUXML.Q<VisualElement>("confirmSelector"), Confirm, false);

            Confirm.RegisterCallback((MouseDownEvent e) =>
            {
                Submit();
            });
            Cancel.RegisterCallback((MouseDownEvent e) =>
            {
                CloseWindow();
            });

            rootVisualElement.RegisterCallback((MouseDownEvent e) =>
            {
                textinputS.UnSelected();

            });
            normalField.RegisterCallback((MouseDownEvent e) =>
            {
                textinputS.UnSelected();

            });

            root.Add(labelFromUXML);
        }

        private void OnDisable()
        {
            SceneViewToolBar.ButtonReleased();
        }

        public static string prefabNameDes;
        public static string prefabTypeDes;
        public static string prefabPosDes;
        public static string prefabPackDes;
        public static string OKText;
        public static string CancelText;

        public static void InitWindowData()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "Constant/prefabModify_popup.uxml");
            labelFromUXML = visualTree.CloneTree();
            prefabNameDes = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_组件名称) + " :";
            prefabTypeDes = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_组件类型) + " :";
            prefabPosDes = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_组件储存位置) + " :";
            prefabPackDes = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_组件生成模式) + " :";
            OKText = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定);
            CancelText = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消);
            List<string> labelList = new List<string>();
            labelList.Add(WidgetRepositoryConfig.noneLabelText);
            labelList.AddRange(SettingAssetsUtils.GetAssets<PrefabLabelsSettings>().labelList);
            labelList.Add(WidgetRepositoryConfig.addNewLabelText);
            TypeNames = labelList;

            defaultType = WidgetRepositoryWindow.GetInstance() == null ||
                          WidgetRepositoryWindow.GetInstance().filtration == "All"
                ? "All"
                : WidgetRepositoryWindow.GetInstance().filtration;
        }



        private void RefreshPopupField()
        {
            PopupField.Clear();
            normalField = new PopupField<string>(TypeNames, defaultType);
            normalField.RegisterValueChangedCallback(x => ChangeValue(x.newValue));
            normalField.style.position = Position.Absolute;
            normalField.style.left = 0;
            normalField.style.right = 0;
            normalField.style.top = 0;
            normalField.style.bottom = 0;
            PopupField.Add(normalField);
        }

        private void CloseWindow()
        {
            if (m_window != null)
            {
                componentPath = "";
                currentName = "";
                isPrefab = false;
                m_window.Close();
            }

        }


        private void Submit()
        {
            // string path = AssetDatabase.LoadAssetAtPath<DefaultPrefabPathSetting>(ThunderFireUIToolConfig.DefaultPrefabPathSettingFullPath).PathList[(int)PrefabPath.Component];
            string path = componentPath;
            string label = (normalField.value != WidgetRepositoryConfig.addNewLabelText && normalField.value != WidgetRepositoryConfig.noneLabelText) ? normalField.value : null;
            bool isPack = normalField2.value == "Pack";
            if (currentName == "")
            {
                EditorUtility.DisplayDialog("messageBox",
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_Prefab名称合法Tip),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                return;
            }
            // Debug.Log(path);
            // Debug.Log(label);
            // Debug.Log(isPack);
            // Debug.Log(currentName);
            // Debug.Log(isPrefab);

            if (Directory.Exists(path))
            {
                if (selectRectList.Length == 1)
                {
                    ToolUtils.CreatePrefabWithPack(selectRectList[0].gameObject, currentName, path, isPrefab, label, isPack);
                }

                if (selectRectList.Length > 1)
                {

                    GameObject root = CombineWidgetLogic.GenCombineRootRect(selectRectList);
                    ToolUtils.CreatePrefabWithPack(root, currentName, path, isPrefab, label, isPack);
                }
            }
            else
            {
                EditorUtility.DisplayDialog("messageBox",
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_Prefab存储位置不存在Tip),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                return;
            }

            componentPath = "";
            currentName = "";
            isPrefab = false;
            m_window.Close();
        }


        private void ChangeValue(string add)
        {
            if (add == "+")
            {
                ShowAddLabelWindow();
            }
        }


        private void ShowAddLabelWindow()
        {
            PrefabAddNewLabelWindow.OpenWindow(OnAddLabelSuccess, OnAddLabelCancel);
        }

        private void OnAddLabelSuccess(string newLabel)
        {
            InitWindowData();
            RefreshPopupField();
            normalField.value = newLabel;

        }

        private void OnAddLabelCancel()
        {
            normalField.value = TypeNames[0];
        }

        private void SelectComponentPath()
        {
            if (PosInput != null)
            {
                string path = SelectPath();
                if (path != null)
                {
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.black;
                    style.fontSize = 12;
                    componentPath = path;
                    PosInput.onGUIHandler += () => { componentPath = EditorGUILayout.TextField(componentPath, style); };
                }
            }
        }

        private string SelectPath()
        {
            string folderPath = PlayerPrefs.GetString("LastParticleCheckPath");
            string path = EditorUtility.OpenFolderPanel(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_选择路径), folderPath, "");
            if (path != "")
            {
                int index = path.IndexOf("Assets");
                if (index != -1)
                {
                    PlayerPrefs.SetString("LastParticleCheckPath", path);
                    path = path.Substring(index);
                    return path + "/";
                }
                else
                {
                    EditorUtility.DisplayDialog("messageBox",
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_目录不在Assets下Tip),
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                }
            }
            return null;
        }
    }
}
#endif