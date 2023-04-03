using UnityEditor;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    internal class OpenDialogueButton : IGUIComponent {
        private readonly string _text;
        private readonly string _url;
        private readonly string _title;
        private readonly string _message;
        private readonly string _ok;
        private readonly string _cancel;
        
        public OpenDialogueButton(string text, string url, string title, string message, string ok, string cancel) {
            _text = text;
            _url = url;
            _title = title;
            _message = message;
            _ok = ok;
            _cancel = cancel;
        }

        public void OnGUI() {
             Render(_text, _url, _title, _message, _ok, _cancel);
        }

        public static void Render(string text, string url, string title, string message, string ok, string cancel) {
            if (GUILayout.Button(new GUIContent(text.StartsWith(" ") ? text : " " + text, EditorGUIUtility.IconContent("BuildSettings.Web.Small").image))) {
                if (EditorUtility.DisplayDialog(title, message, ok, cancel)) {
                    Application.OpenURL(url);
                }
            }
        }
    }
}
