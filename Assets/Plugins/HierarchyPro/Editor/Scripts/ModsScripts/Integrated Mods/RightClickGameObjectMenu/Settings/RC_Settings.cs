using System.Linq;
using System.Collections.Generic;



namespace EMX.HierarchyPlugin.Editor
{

	partial class EditorSettingsAdapter
	{


		//  internal int AS_SAVE_INTERVAL_IN_MIN { get { return Mathf.Clamp(GET("AS_SAVE_INTERVAL_IN_MIN", 5), 1, 60); } set {var r = qwe; SET( "AS_SAVE_INTERVAL_IN_MIN", value); p.RESET_DRAWSTACK(); } }
		internal bool RCGO_MENU_PLACE_TO_SUBMENU { get { return GET("RCGO_MENU_PLACE_TO_SUBMENU", true); } set {var r = RCGO_MENU_PLACE_TO_SUBMENU; SET( "RCGO_MENU_PLACE_TO_SUBMENU", value); } }
		internal bool RCGO_MENU_USE_HOTKEYS { get { return GET("RCGO_MENU_USE_HOTKEYS", true); } set {var r = RCGO_MENU_USE_HOTKEYS; SET( "RCGO_MENU_USE_HOTKEYS", value); } }
		string RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING { get { return GET("RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING", ""); } set {var r = RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING; SET( "RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING", value); } }
        internal bool INCLUDE_RIGHTCLIK_MENU_OPENPLUGINWINDOWS_BUTTONS {
            get { return GET( "INCLUDE_RIGHTCLIK_MENU_OPENPLUGINWINDOWS_BUTTONS", false ); }
            set {
               var r = INCLUDE_RIGHTCLIK_MENU_OPENPLUGINWINDOWS_BUTTONS; SET( "INCLUDE_RIGHTCLIK_MENU_OPENPLUGINWINDOWS_BUTTONS", value );
                p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews();
            }
        }

        internal string[] AllWindows = { "SceneView", "Inspector", "GameView", "SceneHierarchy"/*, "ProjectBrowser"*/};
		Dictionary<string, bool> __CUSTOMMENU_HOTKEYS_WINDOWS;
		internal Dictionary<string, bool> CUSTOMMENU_HOTKEYS_WINDOWS
		{
			get
			{
				if (__CUSTOMMENU_HOTKEYS_WINDOWS == null)
				{
					__CUSTOMMENU_HOTKEYS_WINDOWS = new Dictionary<string, bool>();
					if (RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING == "") RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING = AllWindows.Aggregate((a, b) => a + " " + b);
					if (!string.IsNullOrEmpty(RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING))
						foreach (var item in RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING.Split(' ')) if (!__CUSTOMMENU_HOTKEYS_WINDOWS.ContainsKey(item)) __CUSTOMMENU_HOTKEYS_WINDOWS.Add(item, true);
				}
				return __CUSTOMMENU_HOTKEYS_WINDOWS;
			}
			set
			{
				if (value.Keys.Count == 0) RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING = "";
				else RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING = value.Keys.Aggregate((a, b) => a + " " + b);
				__CUSTOMMENU_HOTKEYS_WINDOWS = value;
			}
		}


	}
}
