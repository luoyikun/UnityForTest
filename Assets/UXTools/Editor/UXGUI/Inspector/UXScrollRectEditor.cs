using UnityEditor;
using UnityEngine;
using System;
using ThunderFireUITool;

namespace UnityEngine.UI
{
    [CustomEditor(typeof(UXScrollRect), true)]
    [CanEditMultipleObjects]
    [InitializeOnLoad]
    public class UXScrollRectEditor : UnityEditor.UI.ScrollRectEditor
    {
        private SerializedProperty m_ItemCell;
        private SerializedProperty m_layoutType;
        private int m_PreviewNum = 0;
        private RectTransform m_Content;

        static UXScrollRectEditor()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        protected override void OnEnable()
        {
            m_ItemCell = serializedObject.FindProperty("m_ItemCell");
            m_layoutType = serializedObject.FindProperty("m_layoutType");
            m_Content = (serializedObject.targetObject as UXScrollRect).content;
            for (int i = 0; i < m_Content.childCount; i++)
            {
                if (m_Content.GetChild(i).hideFlags == HideFlags.DontSave)
                {
                    m_PreviewNum++;
                }
            }
            base.OnEnable();
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go != null && go.hideFlags == HideFlags.DontSave && go.name.StartsWith("UXPreview"))
            {
                Utils.DrawGreenRect(instanceID, selectionRect, go.name.Substring(9));
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.ObjectField(m_ItemCell, new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_窗格元素)));
            m_PreviewNum = EditorGUILayout.IntField(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_窗格数量), m_PreviewNum);
            if (EditorGUI.EndChangeCheck())
            {
                m_PreviewNum = Mathf.Max(Mathf.Min(m_PreviewNum, 20), 0);
                for (int i = m_Content.childCount - 1; i >= 0; i--)
                {
                    if (m_Content.GetChild(i).hideFlags == HideFlags.DontSave)
                    {
                        DestroyImmediate(m_Content.GetChild(i).gameObject);
                    }
                }
                if (m_ItemCell.objectReferenceValue != null)
                {
                    for (int i = 0; i < m_PreviewNum; i++)
                    {
                        Object go = PrefabUtility.InstantiatePrefab(m_ItemCell.objectReferenceValue, m_Content);
                        if (go == null)
                        {
                            go = Instantiate(m_ItemCell.objectReferenceValue, m_Content);
                        }
                        go.hideFlags = HideFlags.DontSave;
                        go.name = "UXPreview" + m_ItemCell.objectReferenceValue.name;
                    }
                }
            }
            EditorGUI.BeginChangeCheck();
            m_layoutType.intValue = Utils.EnumPopupLayoutEx(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_窗格排布方式),
                    typeof(UXScrollRect.LayoutType), m_layoutType.intValue, new string[] {
                        EditorLocalization.GetLocalization("UXScrollRect", "Grid"),
                        EditorLocalization.GetLocalization("UXScrollRect", "Horizontal"),
                        EditorLocalization.GetLocalization("UXScrollRect", "Vertical"),
                    });
            if (EditorGUI.EndChangeCheck())
            {
                DestroyImmediate(m_Content.GetComponent<LayoutGroup>());
                switch (m_layoutType.intValue)
                {
                    case (int)(UXScrollRect.LayoutType.GridLayout):
                        m_Content.gameObject.AddComponent<GridLayoutGroup>();
                        break;
                    case (int)(UXScrollRect.LayoutType.HorizontalLayout):
                        m_Content.gameObject.AddComponent<HorizontalLayoutGroup>();
                        break;
                    case (int)(UXScrollRect.LayoutType.VerticalLayout):
                        m_Content.gameObject.AddComponent<VerticalLayoutGroup>();
                        break;
                    default:
                        break;
                }
            }
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}
