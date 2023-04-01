using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

//
//
//Enable 'Use Custom Buttons' toggles in the top bar settings
//
//

public class ToolBar_Example_Buttons
{


    // ICONS
    static Dictionary<string, Texture2D> cached_icons = new Dictionary<string, Texture2D>();
    static Texture2D LoadIcon(string name)
    {
        if (cached_icons.ContainsKey(name)) return cached_icons[name];
        var file_path = EMX.HierarchyPlugin.Editor.Folders.PluginInternalFolder + "/Icons ToolBar/" + name;
        var t = AssetDatabase.LoadAssetAtPath<Texture2D>(file_path);
        if (!t) Debug.LogWarning("Canno load HP Layout Icon " + file_path);
        cached_icons.Add(name, t);
        return cached_icons[name];
    }
    static GUIStyle _iconButtonStyle;
    static internal GUIStyle iconButtonStyle {
        get {
            if (_iconButtonStyle == null)
            {
                _iconButtonStyle = EditorStyles.toolbarButton;
                _iconButtonStyle.padding = new RectOffset(2, 2, 2, 2);
            }
            return _iconButtonStyle;
        }
    }


    // BUTTONS
    static GUIContent content = new GUIContent();
    class BUTTON
    {
        public string tooltip;
        public string icon;
        public Action left_click;
        public Action right_click;
    }
    static BUTTON[] BUTTONS = new BUTTON[6] {
        new BUTTON() {
            tooltip ="",
            icon = "layoutbuttons-example-01.png",
            left_click = () => {
                    Debug.Log("Hello Unity!");
            },
            right_click = () => {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Hello Unity"), false, () => Debug.Log("Hello Unity!"));
                    menu.ShowAsContext();
            }
        },
          new BUTTON() {
            tooltip ="",
            icon = "layoutbuttons-example-02.png",
            left_click = () => {
                    Debug.Log("Hello Unity!");
            },
            right_click = () => {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Hello Unity"), false, () => Debug.Log("Hello Unity!"));
                    menu.ShowAsContext();
            }
        },
            new BUTTON() {
            tooltip ="",
            icon = "layoutbuttons-example-03.png",
            left_click = () => {
                    Debug.Log("Hello Unity!");
            },
            right_click = () => {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Hello Unity"), false, () => Debug.Log("Hello Unity!"));
                    menu.ShowAsContext();
            }
        },
              new BUTTON() {
            tooltip ="",
            icon = "layoutbuttons-example-04.png",
            left_click = () => {
                    Debug.Log("Hello Unity!");
            },
            right_click = () => {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Hello Unity"), false, () => Debug.Log("Hello Unity!"));
                    menu.ShowAsContext();
            }
        },
                new BUTTON() {
            icon = "layoutbuttons-example-05.png",
            left_click = () => {
                    Debug.Log("Hello Unity!");
            },
            right_click = () => {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Hello Unity"), false, () => Debug.Log("Hello Unity!"));
                    menu.ShowAsContext();
            }
        },  new BUTTON() {
            tooltip ="",
            icon = "layoutbuttons-example-06.png",
            left_click = () => {
                    Debug.Log("Hello Unity!");
            },
            right_click = () => {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Hello Unity"), false, () => Debug.Log("Hello Unity!"));
                    menu.ShowAsContext();
            }
        }
    };



    // BODY
    [InitializeOnLoadMethod]
    static void AddButtonOoToolBar()
    {

        //LEFT AREA
        EMX.CustomizationHierarchy.TopGUI.onLeftLayoutGUI += (full_rect) => {
            var rect = full_rect;
            rect.width = rect.height * 1.5f;


            foreach (var b in BUTTONS)
            {
                content.tooltip = "";
                if (!string.IsNullOrEmpty(b.tooltip)) content.tooltip += b.tooltip;
                content.tooltip = "- Left-click to use\n- Right-click to open fast context menu";

                content.image = LoadIcon(b.icon);
                if (GUI.Button(rect, content, iconButtonStyle))
                {
                    if (Event.current.button == 0)
                    {
                        if (b.left_click != null) b.left_click();
                    }
                    else
                    {
                        if (b.right_click != null) b.right_click();
                    }
                }
                rect.x += rect.width;
            }
        };



        //RIGHT AREA
        EMX.CustomizationHierarchy.TopGUI.onRightLayoutGUI += (full_rect) => {

            var rect = full_rect;

            if (EMX.CustomizationHierarchy.TopGUI.ButtonFitContent(ref rect, "Hello Unity!")) Debug.Log("Hello Unity!"); rect.x += rect.width;
            if (EMX.CustomizationHierarchy.TopGUI.ButtonFitContent(ref rect, "Hello World!")) Debug.Log("Hello World!"); rect.x += rect.width;

            rect.x += 10;

            rect.width = rect.height;
            if (GUI.Button(Shrink(rect, 2), "Go", button_style_0)) Debug.Log("Go!"); rect.x += rect.width;

            rect.x += 10;

            // we allocate space for GUILayout for case when custom and layout buttons areas are using in one place both
            GUILayout.Space(rect.x - full_rect.x);

        };


    }
    static Rect Shrink(Rect r, int v) { r.x += v; r.y += v; r.width -= v * 2; r.height -= v * 2; return r; }
    static GUIStyle _button_style; static GUIStyle button_style { get { return _button_style ?? (_button_style = new GUIStyle(EditorStyles.toolbarButton) { fixedHeight = 0 }); } }
    static GUIStyle _button_style_0; static GUIStyle button_style_0 { get { return _button_style_0 ?? (_button_style_0 = new GUIStyle(EditorStyles.toolbarButton) { fixedHeight = 0, padding = new RectOffset() }); } }
}






/*#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

//
//
//Enable 'Use Custom Buttons' toggles in the top bar settings
//
//

public class ToolBar_Example_Buttons
{
    [InitializeOnLoadMethod]
    static void AddButtonOoToolBar()
    {
        //you can subscribe to the left or right area
        EMX.CustomizationHierarchy.TopGUI.onLeftLayoutGUI += (full_rect) => {

            var rect = full_rect;


            if (EMX.CustomizationHierarchy.TopGUI.ButtonFitContent(ref rect, "Hello Unity!")) Debug.Log("Hello Unity!"); rect.x += rect.width;
            if (EMX.CustomizationHierarchy.TopGUI.ButtonFitContent(ref rect, "Hello World!")) Debug.Log("Hello World!"); rect.x += rect.width;

            rect.x += 10;

            rect.width = rect.height;
            if (GUI.Button(Shrink(rect, 2), "Go", button_style_0)) Debug.Log("Go!"); rect.x += rect.width;

            rect.x += 10;

            // we allocate space for GUILayout for case when custom and layout buttons areas are using in one place both
            GUILayout.Space(rect.x - full_rect.x);
        };
    }
    static Rect Shrink(Rect r, int v) { r.x += v; r.y += v; r.width -= v * 2; r.height -= v * 2; return r; }
    static GUIStyle _button_style; static GUIStyle button_style { get { return _button_style ?? (_button_style = new GUIStyle(EditorStyles.toolbarButton) { fixedHeight = 0 }); } }
    static GUIStyle _button_style_0; static GUIStyle button_style_0 { get { return _button_style_0 ?? (_button_style_0 = new GUIStyle(EditorStyles.toolbarButton) { fixedHeight = 0, padding = new RectOffset() }); } }
}

#endif*/