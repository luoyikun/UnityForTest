using UnityEditor;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    internal class OpenURLButton : IGUIComponent {
        private readonly string _text;
        private readonly string _url;
        public OpenURLButton(string text, string url) {
            _text = text;
            _url = url;
        }

        public void OnGUI() {
            Render(_text, _url);
        }

        public static void Render(string text, string url) {
            if (GUILayout.Button(new GUIContent(text.StartsWith(" ") ? text : " " + text, EditorGUIUtility.IconContent("BuildSettings.Web.Small").image))) {
                Application.OpenURL(url);
            }
        }
    }
}
