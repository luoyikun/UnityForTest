#if UNITY_EDITOR
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace ThunderFireUITool
{
    public class PrefabInfoModifyWindow : EditorWindow
    {
        private static PrefabInfoModifyWindow m_window;
        private static GameObject selectObj;
        private static string OriginalName = null;
        private static string OriginalLabel = null;
        private static string OriginalPack = null;
        private VisualElement PopupField;
        private VisualElement PopupField2;
        private IMGUIContainer PosInput;
        private PopupField<string> normalField;
        private PopupField<string> normalField2;
        static List<string> TypeNames;
        private string currentName = null;
        private static VisualElement SelectedBuff;
        private Action actionOnDisable = null;
        private static string componentPath;

        public static void OpenWindow(GameObject obj, Action action = null)
        {
            int width = 383;
            int height = 247;
            selectObj = obj;
            OriginalName = obj.name;
            componentPath = AssetDatabase.GetAssetPath(obj);


            InitWindowData();
            m_window = GetWindow<PrefabInfoModifyWindow>();
            m_window.minSize = new Vector2(width, height);
            m_window.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_修改信息);
            // m_window.position = new Rect((Screen.currentResolution.width - width) / 2, (Screen.currentResolution.height - height) / 2, width, height);
            m_window.titleContent.image = ToolUtils.GetIcon("component_w");

            m_window.actionOnDisable = action;
        }

        public static string prefabNameDes;
        public static string prefabTypeDes;
        public static string prefabPosDes;
        public static string prefabPackDes;
        public static string OKText;
        public static string CancelText;

        public static void InitWindowData()
        {
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

            string[] labels = AssetDatabase.GetLabels(selectObj);
            foreach(string lab in labels){
                if(TypeNames.Contains(lab)){
                    OriginalLabel = lab;
                }
                else if(lab=="UnPack"){
                    OriginalPack = "UnPack";
                }
                else if(lab == "Pack"){
                    OriginalPack = "Pack";
                }
            }
            if(OriginalLabel == null){
                OriginalLabel = "All";
            }
            
        }

        private void OnEnable()
        {
            currentName = OriginalName;
            VisualElement root = rootVisualElement;
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "Constant/prefabModify_popup.uxml");
            VisualElement labelFromUXML = visualTree.CloneTree();
            Label TextLabel = labelFromUXML.Q<Label>("textlabel");
            IMGUIContainer TextInput = labelFromUXML.Q<IMGUIContainer>("textinput");
            Label PosLabel = labelFromUXML.Q<Label>("poslabel");
            Button PosButton = labelFromUXML.Q<Button>("posbutton");
            VisualElement Confirm = labelFromUXML.Q<VisualElement>("confirm");
            VisualElement Cancel = labelFromUXML.Q<VisualElement>("cancel");


            PopupField = labelFromUXML.Q<VisualElement>("popupfield");
            PopupField2 = labelFromUXML.Q<VisualElement>("popupfield2");
            TextLabel.text = prefabNameDes;
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.black;
            style.fontSize = 14;
            //TextInput.onGUIHandler += () => { currentName = GUI.TextField(new Rect(2.5f, 2.5f, 201, 20), currentName, style); };
            //TextInput.style.overflow = new StyleEnum<Overflow>(Overflow.Hidden);
            TextInput.onGUIHandler += () => { currentName = EditorGUILayout.TextField(currentName, style); };

            style.fontSize = 14;
            PosInput = labelFromUXML.Q<IMGUIContainer>("posinput");
            PosInput.SetEnabled(false);
            PosInput.onGUIHandler += () => { componentPath = EditorGUILayout.TextField(componentPath, style); };

            PosLabel.text = prefabPosDes;
            PosButton.SetEnabled(false);

            refreshPopupField();

            PopupField2.Clear();
            List<string> packNames = new List<string>();
            packNames.Add("Pack");
            packNames.Add("Unpack");
            normalField2 = new PopupField<string>(packNames, 0);
            normalField2.value = OriginalPack;
            normalField2.RegisterValueChangedCallback(x => ChangeValue(x.newValue));
            normalField2.style.position = Position.Absolute;
            normalField2.style.left = 0;
            normalField2.style.right = 0;
            normalField2.style.top = 0;
            normalField2.style.bottom = 0;
            PopupField2.Add(normalField2);

            //EditorUIUtil.CreateUIEButton(Confirm, Submit);
            //EditorUIUtil.CreateUIEButton(Cancel, closeWindow);
            labelFromUXML.Q<Label>("poptext").text = prefabTypeDes;
            labelFromUXML.Q<Label>("poptext2").text = prefabPackDes;
            Confirm.Q<Label>("text").text = OKText;
            root.Add(labelFromUXML);
            Cancel.Q<Label>("text").text = CancelText;

            VisualElement textinputSelector = labelFromUXML.Q<VisualElement>("textinputSelector");
            SelectorItem textinputS = new SelectorItem(textinputSelector, TextInput, false);

            new SelectorItem(labelFromUXML.Q<VisualElement>("confirmSelector"), Confirm, false);
            Confirm.RegisterCallback((MouseDownEvent e) =>
            {
                Submit();

            });
            Cancel.RegisterCallback((MouseDownEvent e) =>
            {
                closeWindow();

            });

            new SelectorItem(labelFromUXML.Q<VisualElement>("cancelSelector"), Cancel);

            normalField.RegisterCallback((MouseDownEvent e) =>
            {
                textinputS.UnSelected();

            });


            rootVisualElement.RegisterCallback((MouseDownEvent e) =>
            {
                textinputS.UnSelected();

            });

        }
        private void OnDisable()
        {
            actionOnDisable?.Invoke();
        }



        private void closeWindow()
        {
            if (m_window != null)
            {
                m_window.Close();
            }

        }



        private void Submit()
        {
            if (currentName.Equals(OriginalName) && normalField.value.Equals(OriginalLabel) && normalField2.value.Equals(OriginalPack))
            {
                m_window.Close();
                return;
            }

            if (EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_是否要保存修改), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消)))
            {
                AssetDatabase.ClearLabels(selectObj);
                AssetDatabase.SetLabels(selectObj, new string[] { normalField2.value, normalField.value });
                string path = AssetDatabase.GetAssetPath(selectObj);
                AssetDatabase.RenameAsset(path, currentName);
                if (WidgetRepositoryWindow.GetInstance() != null)
                {
                    WidgetRepositoryWindow.GetInstance().RefreshWindow();
                }
                if (PrefabRecentWindow.GetInstance() != null)
                {
                    PrefabRecentWindow.GetInstance().RefreshWindow();
                }
                PrefabTabs.RefreshTabs();
                m_window.Close();
            }
        }

        private void refreshPopupField()
        {
            PopupField.Clear();
            normalField = new PopupField<string>(TypeNames, 0);
            normalField.value = OriginalLabel;
            normalField.RegisterValueChangedCallback(x => ChangeValue(x.newValue));
            normalField.style.position = Position.Absolute;
            normalField.style.left = 0;
            normalField.style.right = 0;
            normalField.style.top = 0;
            normalField.style.bottom = 0;

            //normalField.style.width = 201;
            //normalField.style.height = 25;
            PopupField.Add(normalField);
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
            refreshPopupField();
            normalField.value = newLabel;

        }

        private void OnAddLabelCancel()
        {
            normalField.value = TypeNames[0];
        }




    }
}
#endif