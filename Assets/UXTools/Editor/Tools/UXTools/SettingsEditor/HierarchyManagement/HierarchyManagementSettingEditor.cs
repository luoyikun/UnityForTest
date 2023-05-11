using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ThunderFireUITool
{
    [CustomEditor(typeof(HierarchyManagementSetting))]
    public class HierarchyManagementSettingEditor : Editor
    {
        SerializedProperty tagsProperty;
        SerializedProperty guidsProperty;
        SerializedProperty channelsProperty;
        private void OnEnable()
        {
            tagsProperty = serializedObject.FindProperty("tagColors");
            guidsProperty = serializedObject.FindProperty("guidList");
            channelsProperty = serializedObject.FindProperty("channelNameList");

        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.PropertyField(tagsProperty);
            // EditorGUILayout.PropertyField(guidsProperty);
            // EditorGUILayout.PropertyField(channelsProperty);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
    
}