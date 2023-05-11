using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ThunderFireUITool;
using UnityEditorInternal;

namespace UnityEngine.UI
{
    [CustomEditor(typeof(UXTextMeshPro), true)]
    [CanEditMultipleObjects]
    public class UXTextMeshProEditor : TMPro.EditorUtilities.TMP_EditorPanelUI
    {
        private SerializedProperty localizationType;
        private SerializedProperty text;
        private SerializedProperty ignoreLocalization;
        private SerializedProperty localizationID;
        private SerializedProperty previewID;

        private ReorderableList customList;
        private List<string> textList;
        private bool[] toggleValues;
        private int lastToggleIndex;
        private UXTextMeshPro targetObject;
        private GameObject cloneObj;
        private bool initialHide;
        private string need_replace;
        private string origin_text;
        private bool foldout = false;

        protected override void OnEnable()
        {
            localizationType = serializedObject.FindProperty("m_localizationType");
            text = serializedObject.FindProperty("m_text");
            ignoreLocalization = serializedObject.FindProperty("m_ignoreLocalization");
            localizationID = serializedObject.FindProperty("m_localizationID");
            previewID = serializedObject.FindProperty("m_previewID");

            targetObject = serializedObject.targetObject as UXTextMeshPro;
            toggleValues = new bool[UXGUIConfig.availableLanguages.Count];
            lastToggleIndex = -1;
            need_replace = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_未填充文本);
            textList = new List<string>();
            origin_text = text.stringValue;
            ChangeAvailables();

            customList = new ReorderableList(textList, typeof(string), false, true, false, false);
            customList.drawElementCallback = (Rect rect, int index, bool selected, bool focused) =>
            {
                EditorGUI.BeginChangeCheck();
                toggleValues[index] = EditorGUI.Toggle(rect, toggleValues[index]);
                if (EditorGUI.EndChangeCheck())
                {
                    if (toggleValues[index])
                    {
                        if (lastToggleIndex == -1)
                        {
                            initialHide = SceneVisibilityManager.instance.IsHidden(targetObject.gameObject);
                        }
                        else
                        {
                            toggleValues[lastToggleIndex] = false;
                        }
                        lastToggleIndex = index;
                        if (cloneObj != null)
                        {
                            DestroyImmediate(cloneObj);
                        }
                        SceneVisibilityManager.instance.Hide(targetObject.gameObject, true);
                        targetObject.ignoreLocalization = true;
                        cloneObj = Instantiate(targetObject.gameObject, targetObject.transform.position, targetObject.transform.rotation, targetObject.transform);
                        cloneObj.transform.localScale = new Vector3(1, 1, 1);
                        RectTransform cloneRect = cloneObj.GetComponent<RectTransform>();
                        cloneRect.anchorMax = new Vector2(1, 1);
                        cloneRect.anchorMin = new Vector2(0, 0);
                        cloneRect.offsetMax = new Vector2(0, 0);
                        cloneRect.offsetMin = new Vector2(0, 0);
                        targetObject.ignoreLocalization = false;
                        cloneObj.GetComponent<UXTextMeshPro>().text = textList[index];
                        cloneObj.hideFlags = HideFlags.HideAndDontSave;
                    }
                    else
                    {
                        ShowObj();
                    }
                }
                GUI.enabled = false;
                EditorGUI.TextField(new Rect(rect) { x = rect.x + 20, width = rect.width - 20 },
                        LocalizationLanguage.GetLanguage(UXGUIConfig.availableLanguages[index]), textList[index]);
                GUI.enabled = true;
            };
            customList.drawHeaderCallback = (Rect rect) =>
            {
                int validNums = 0;
                foreach (var item in textList)
                {
                    if (item != need_replace)
                    {
                        validNums = validNums + 1;
                    }
                }
                GUI.Label(rect, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_本地化文本数目));
                if (validNums != UXGUIConfig.availableLanguages.Count)
                {
                    GUI.contentColor = new Color(0xda / 255f, 0x5b / 255, 0x5b / 255);
                }
                GUI.Label(new Rect(rect) { x = rect.x + rect.width - 28 - validNums / 10 * 6 }, validNums + "");
                GUI.contentColor = Color.white;
                GUI.Label(new Rect(rect) { x = rect.x + rect.width - 20 }, "/" + UXGUIConfig.availableLanguages.Count);
            };
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            if (cloneObj != null)
            {
                DestroyImmediate(cloneObj);
            }
            if (lastToggleIndex != -1 && !initialHide && targetObject != null)
            {
                SceneVisibilityManager.instance.Show(targetObject.gameObject, true);
            }
            base.OnDisable();
        }

        private void ShowObj()
        {
            if (cloneObj != null)
            {
                DestroyImmediate(cloneObj);
            }
            if (!initialHide)
            {
                SceneVisibilityManager.instance.Show(targetObject.gameObject, true);
            }
            if (lastToggleIndex == -1) return;
            toggleValues[lastToggleIndex] = false;
            lastToggleIndex = -1;
        }

        private void ChangeAvailables()
        {
            textList.Clear();
            string id = (LocalizationHelper.TextLocalizationType)localizationType.intValue == LocalizationHelper.TextLocalizationType.RuntimeUse ? localizationID.stringValue : previewID.stringValue;
            if (ignoreLocalization.boolValue || id == "") return;
            string[] translates = UXTextTable.ReadRow(id);
            if (translates == null) return;
            foreach (int i in UXGUIConfig.availableLanguages)
            {
                textList.Add(translates[i] == "" ? need_replace : translates[i]);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            var type = Utils.EnumPopupLayoutEx(EditorLocalization.GetLocalization("UXText", "localizationType"),
                    typeof(LocalizationHelper.TextLocalizationType), localizationType.intValue, new string[] {
                        EditorLocalization.GetLocalization("UXText", "RuntimeUseText"),
                        EditorLocalization.GetLocalization("UXText", "DynamicText")
                    });
            if (EditorGUI.EndChangeCheck())
            {
                localizationType.intValue = (int)type;
                ChangeAvailables();
                ShowObj();
            }

            if (type == (int)LocalizationHelper.TextLocalizationType.RuntimeUse)
            {
                EditorGUI.BeginChangeCheck();
                ignoreLocalization.boolValue = !EditorGUILayout.Toggle(EditorLocalization.GetLocalization("UXText", "Enable localization"), !ignoreLocalization.boolValue);
                if (EditorGUI.EndChangeCheck())
                {
                    ShowObj();
                    if (!ignoreLocalization.boolValue)
                    {
                        if (localizationID.stringValue == "")
                        {
                            localizationID.stringValue = System.Guid.NewGuid().ToString().Substring(0, 8);
                            textList.Clear();
                        }
                        else
                        {
                            ChangeAvailables();
                        }
                    }
                }
                if (text.stringValue != origin_text && !ignoreLocalization.boolValue)
                {
                    ShowObj();
                    localizationID.stringValue = System.Guid.NewGuid().ToString().Substring(0, 8);
                    for (int i = 0; i < textList.Count; i++)
                    {
                        textList[i] = need_replace;
                    }
                    origin_text = text.stringValue;
                }
                if (!ignoreLocalization.boolValue)
                {
                    if (GUILayout.Button(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_打开静态文本表格)))
                    {
                        if (!Application.isPlaying)
                        {
                            UXTextTable.SyncTextTable();
                        }
                        UXTextTable.OpenTextTable();
                    }
                    foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_表格使用提示));
                    if (foldout)
                    {
                        GUI.enabled = false;
                        GUIStyle style = new GUIStyle(EditorStyles.textArea);
                        style.padding = new RectOffset(5, 5, 5, 5);
                        GUILayout.TextArea(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_更新表格并保存后), style);
                        GUI.enabled = true;
                    }
                    EditorGUILayout.BeginHorizontal();
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(localizationID, new GUIContent("key"));
                    GUI.enabled = true;
                    if (GUILayout.Button(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_复制), GUILayout.MaxWidth(50)))
                    {
                        GUIUtility.systemCopyBuffer = localizationID.stringValue;
                    }
                    EditorGUILayout.EndHorizontal();
                    customList.DoLayoutList();
                }
            }
            else if (type == (int)LocalizationHelper.TextLocalizationType.Preview)
            {
                EditorGUI.BeginChangeCheck();
                ignoreLocalization.boolValue = !EditorGUILayout.Toggle(EditorLocalization.GetLocalization("UXText", "Enable localization"), !ignoreLocalization.boolValue);
                if (EditorGUI.EndChangeCheck())
                {
                    ShowObj();
                    if (!ignoreLocalization.boolValue && localizationID.stringValue != "")
                    {
                        ChangeAvailables();
                    }
                }

                if (!ignoreLocalization.boolValue)
                {
                    if (GUILayout.Button(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_打开动态文本表格)))
                    {
                        UXTextTable.OpenPreviewTextTable();
                    }
                    foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_表格使用提示));
                    if (foldout)
                    {
                        GUI.enabled = false;
                        GUIStyle style = new GUIStyle(EditorStyles.textArea);
                        style.padding = new RectOffset(5, 5, 5, 5);
                        GUILayout.TextArea(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_更新表格并保存后), style);
                        GUI.enabled = true;
                    }
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(previewID, new GUIContent("key"));
                    if (EditorGUI.EndChangeCheck())
                    {
                        ShowObj();
                        ChangeAvailables();
                    }
                    if (GUILayout.Button(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_复制), GUILayout.MaxWidth(50)))
                    {
                        GUIUtility.systemCopyBuffer = previewID.stringValue;
                    }
                    EditorGUILayout.EndHorizontal();
                    customList.DoLayoutList();
                }
            }
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}
