using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public static partial class EditorUIUtils
    {
        public static bool IMGButton(string name, Color color, float width = 25, float height = 25, string tooltip = null)
        {
            GUI.backgroundColor = color;
            return GUILayout.Button(ToolUtils.GetIcon(name), GUILayout.Width(width), GUILayout.Height(height));
        }

    }
}