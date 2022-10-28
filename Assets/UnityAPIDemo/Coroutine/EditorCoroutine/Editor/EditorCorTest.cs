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
        yield return new EditorWaitForSeconds(10); //第1个current
        ++m_Updates;       //代码块a 满足第1个cureent条件，cureent执行MoveNext后执行
        Debug.Log(m_Updates);
        yield return new EditorWaitForSeconds(8);//第2个 curent
        ++m_Updates;//代码块b  满足第2个cureent条件，cureent执行MoveNext后执行
        Debug.Log(m_Updates);
    }
}