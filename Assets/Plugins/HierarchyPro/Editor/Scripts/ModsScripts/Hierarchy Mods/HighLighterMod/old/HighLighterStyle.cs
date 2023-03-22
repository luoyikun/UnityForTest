using System.Linq;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor
{

	class HighLighterStyle
	{


		//	static PluginInstance Adapter { get { return Root.p[0]; } }

		static GUIStyle _label;
		static GUIStyle label
		{
			get
			{
				if (_label == null)
				{
					_label = new GUIStyle(Root.p[0].label);
					_label.richText = true;
				}
				_label.fontSize = Root.p[0].label.fontSize;
				return _label;
			}
		}
		static GUIStyle __foldoutStyle;

		static internal GUIStyle foldoutStyle
		{
			get { return __foldoutStyle ?? (__foldoutStyle = (GUIStyle)"IN Foldout"); }
		}
		static internal float foldoutStyleWidth
		{
			get { return foldoutStyle.fixedWidth + 2; }
		}
		static internal float foldoutStyleHeight
		{
			get { return foldoutStyle.fixedHeight != 0 ? foldoutStyle.fixedHeight : EditorGUIUtility.singleLineHeight; }
		}



		static GUIContent __TOOLTIP = new GUIContent();
		internal static void TOOLTIP(Rect R, string text, bool bold = false)
		{
			__TOOLTIP.text = "";
			__TOOLTIP.tooltip = text;
			if (bold) label.fontStyle = FontStyle.Bold;
			else label.fontStyle = FontStyle.Normal;
			label.alignment = TextAnchor.MiddleLeft;
			GUI.Label(R, __TOOLTIP);
		}


		internal static void LABEL(Rect R, string text)
		{
			label.fontStyle = FontStyle.Normal;
			label.alignment = TextAnchor.MiddleLeft;
			GUI.Label(R, text);
		}

		internal static void LABEL(Rect R, string text, GUIStyle label)
		{
			label.fontStyle = FontStyle.Normal;
			label.alignment = TextAnchor.MiddleLeft;
			GUI.Label(R, text, label);
		}
		internal static void LABEL(Rect R, string text, bool bold, TextAnchor? ta)
		{
			if (bold) label.fontStyle = FontStyle.Bold;
			else label.fontStyle = FontStyle.Normal;
			label.alignment = ta ?? TextAnchor.MiddleLeft;
			GUI.Label(R, text, label);
		}
		static GUIStyle m_InspectorTitlebar;
		static internal void HR(Rect R, float HR = 0)
		{
			if (m_InspectorTitlebar == null)
			{
				m_InspectorTitlebar = GUI.skin.FindStyle("m_InspectorTitlebar") ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("m_InspectorTitlebar");
				if (m_InspectorTitlebar == null)
				{
					m_InspectorTitlebar = GUI.skin.box;
				}
			}
			R.height = 1;
			if (HR != 0)
			{
				float HRP = HR;
				HRP -= R.width / 4;
				R.x -= (HRP - 5);
				R.width += (HRP - 5) * 2;
			}
			R.y -= 8;
			EditorGUI.DrawRect(R, new Color32(20, 20, 20, 255));
		}


		static internal int TOOGLE_POP_INVERCE(ref Rect lineRect, string titile, int value, params string[] cats)
		{
			lineRect.height = EditorGUIUtility.singleLineHeight;
			LABEL(lineRect, "<i>" + titile + ":</i>");
			lineRect.y += lineRect.height;
			lineRect.height = EditorGUIUtility.singleLineHeight + 2;
			return cats.Length - 1 - GUI.Toolbar(lineRect, cats.Length - 1 - value, cats.Reverse().ToArray(), EditorStyles.toolbarButton);
		}
		static internal int TOOGLE_POP(ref Rect lineRect, string titile, int value, params string[] cats)
		{
			lineRect.height = EditorGUIUtility.singleLineHeight;
			LABEL(lineRect, "<i>" + titile + ":</i>");
			lineRect.y += lineRect.height;
			lineRect.height = EditorStyles.toolbarButton.fixedHeight;
			return GUI.Toolbar(lineRect, value, cats, EditorStyles.toolbarButton);
		}

		//bool _enRich1, _enRich2;


		static internal bool TOGGLE_RIGHT(Rect rect, string title, bool value, bool bold = false, bool? defaultStyle = false, bool skipMark = false)
		{

			var oL = EditorStyles.label.alignment;
			var st = EditorStyles.label.fontStyle;
			var oldRT = EditorStyles.label.richText;
			EditorStyles.label.richText = true;
			if (bold) EditorStyles.label.fontStyle = FontStyle.Bold;
			EditorStyles.label.alignment = TextAnchor.MiddleLeft;

			var result = EditorGUI/*Layout*/.Toggle(rect, /*s + " " +*/ title/* + " " + s*/, value);

			EditorStyles.label.richText = oldRT;
			EditorStyles.label.fontStyle = st;
			EditorStyles.label.alignment = oL;

			return result;
		}
		static internal bool TOGGLE_LEFT(Rect rect, string title, bool value, bool bold = false, bool? defaultStyle = false, bool skipMark = false)
		{
			if (defaultStyle == false)
			{

			}
			else
			{
				rect.x += 10;
				rect.width -= 8;
			}

			var oL = EditorStyles.label.alignment;
			EditorStyles.label.alignment = TextAnchor.MiddleLeft;
			var st = EditorStyles.label.fontStyle;

			if (bold) EditorStyles.label.fontStyle = FontStyle.Bold;

			var oldRT = EditorStyles.label.richText;
			EditorStyles.label.richText = true;

			var result = EditorGUI/*Layout*/.ToggleLeft(rect, /*s + " " +*/ title/* + " " + s*/, value);

			EditorStyles.label.richText = oldRT;
			EditorStyles.label.fontStyle = st;
			EditorStyles.label.alignment = oL;

#if USE_RIGHT_CHECK
		var s =  (value ? "✔" : "✖");//☐☑
		
		if (defaultStyle != null && !skipMark)
		{	var oc = GUI.color;
		
			if (!value) GUI.color *= new Color(1, 1, 1, 0.2f);
			
			GUI.Label(new Rect(rect.x + rect.width - rect.height - 2, rect.y, rect.height, rect.height), s, EditorStyles.label);
			GUI.color = oc;
		}
#endif

			return result;
		}
	}
}
