using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class AB_Extensions : ScriptableObject
	{
	}
	[CustomEditor(typeof(AB_Extensions))]
	class SETGUI_AboutExtensions : MainRoot
	{

		internal static string set_text = "About what you can Extend and Implement";
		internal static string set_key = "";
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
			Draw.TOG_TIT(set_text);


			Draw.Sp(10);
			//Draw.HRx2();
			using (GRO(w).UP(0))
			{
				Draw.TOG_TIT( SETGUI_RightClickMenu.set_text.Substring(USE_STR.Length), EnableRed: false);
				Draw.HELP(w,"You can add your custom menu items using 'EMX." + Root.CUST_NS + ".ExtensionInterface_RightClickOnGameObjectMenuItem'.", drawTog: true);
				if (Draw.BUT("Select Script with Custom Examples")) { Selection.objects = new[] { Root.icons.example_folders[0] }; }
				if (Draw.BUT("Open " + SETGUI_RightClickMenu.set_text.Substring(USE_STR.Length) + " Settings")) { MainSettingsEnabler_Window.Select<RC_Window>(); }
			}

			Draw.Sp(10);
			//Draw.HRx2();
			using (GRO(w).UP(0))
			{
				Draw.TOG_TIT(SETGUI_RightBar.set_text.Substring(USE_STR.Length), EnableRed: false);
				Draw.HELP(w,"You can add your custom mods using 'EMX." + Root.CUST_NS + ".ExtensionInterface_CustomRightMod.Slot_1'.", drawTog: true);
				if (Draw.BUT("Select Script with Custom Examples")) { Selection.objects = new[] { Root.icons.example_folders[1] }; }
				if (Draw.BUT("Open " + SETGUI_RightBar.set_text.Substring(USE_STR.Length) + " Settings")) { MainSettingsEnabler_Window.Select<RM_Window>(); }
			}
			Draw.Sp(10);
		//	Draw.HRx2();
			using (GRO(w).UP(0))
			{
				Draw.TOG_TIT(SETGUI_TopBar.set_text.Substring(USE_STR.Length), EnableRed: false);
				Draw.HELP(w,"You can add your topbar OnGUI function using 'TopGUI'.", drawTog: true);
				if (Draw.BUT("Select Script with Custom Examples")) { Selection.objects = new[] { Root.icons.example_folders[3] }; }
				if (Draw.BUT("Open " + SETGUI_TopBar.set_text.Substring(USE_STR.Length) + " Settings")) { MainSettingsEnabler_Window.Select<TB_Window>(); }
			}
			Draw.Sp(10);
			//Draw.HRx2();
			using (GRO(w).UP(0))
			{
				Draw.TOG_TIT(SETGUI_Icons.set_text.Substring(USE_STR.Length), EnableRed: false);
				Draw.HELP(w,"You can add [DRAW_IN_HIER] attribute to any: public, private fields/methods/properties.", drawTog: true);
				if (Draw.BUT("Select Example Scene")){	Selection.objects = new[] { Root.icons.example_folders[2] };	}
				if (Draw.BUT("Open " + SETGUI_Icons.set_text.Substring(USE_STR.Length) + " Settings")) { MainSettingsEnabler_Window.Select<IC_Window>(); }
			}



		}
	}


}

