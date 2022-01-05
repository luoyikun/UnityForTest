using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.EditorEx;
using UnityEditor;
using UnityEngine;

public class EditorCorTest : EditorWindow
{
    int m_Updates = 0;

    [MenuItem("Tools/EditorCorTest")]
    static void ShowEditroCor()
    {
        EditorWindow.GetWindow(typeof(EditorCorTest));
    
    }

    private void OnGUI()
    {
        if (GUILayout.Button("CorYieldReturnNull"))
        {
            EditorCoroutineUtility.StartCoroutine(CountEditorUpdates(), this);
        }
    }
    

    IEnumerator CountEditorUpdates()
    {
        yield return new EditorWaitForSeconds(10); //��1��current
        ++m_Updates;       //�����a �����1��cureent������cureentִ��MoveNext��ִ��
        Debug.Log(m_Updates);
        yield return new EditorWaitForSeconds(8);//��2�� curent
        ++m_Updates;//�����b  �����2��cureent������cureentִ��MoveNext��ִ��
        Debug.Log(m_Updates);
    }
}