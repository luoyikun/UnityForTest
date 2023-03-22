using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class PLA_Window : ScriptableObject
	{
	}


	[CustomEditor(typeof(PLA_Window))]
	class SETGUI_HL_ProjectAuto : MainRoot
	{
		internal static string set_text =  USE_STR + "Auto Highlighter";
		internal static string set_key = "USE_PROJECT_AUTO_HIGHLIGHTER_MOD";
		public override VisualElement CreateInspectorGUI()
		{
			return base.CreateInspectorGUI();
		}
        public override void OnInspectorGUI()
        {
            _GUI( (IRepaint)this );
        }
        public static void _GUI( IRepaint w )
        {
			Draw.RESET(w);

			Draw.BACK_BUTTON(w);
			Draw.TOG_TIT(set_text, set_key,WIKI: WIKI_3_A_HIGH);
			Draw.Sp(10);

			using (ENABLE(w).USE(set_key))
			{

				//HighlighterManualModSettingsEditor.ASD(this, "HIGHLIGHTER_PROJECT", 1);
				//HighlighterManualModSettingsEditor.ASD(this, "HIGHLIGHTER_PROJECT", 1);
				SETGUI_HL_HierarchyManual.ASD(w, p.par_e.PROJ_HIGH_SET, 1);

			}
		}
	}
}

