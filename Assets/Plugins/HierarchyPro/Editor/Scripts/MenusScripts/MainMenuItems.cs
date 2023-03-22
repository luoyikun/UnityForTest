
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor
{
	class MainMenuItems
	{


		//const string MENU_PATH_OLD = "Window/" + Root.PN + "/";
		const string MENU_PATH = "Tools/" + Root.HierarchyPro + "/";


		


		// removing chars like %#z or %#y will disable hotkeys
		//---------------------------------------------//
		const string SETTINGS = MENU_PATH + "Settings";
		//---------------------------------------------//
		const string SEL_BACK = MENU_PATH + "Selection backward &%#z";
		const string SEL_FORW = MENU_PATH + "Selection forward &%#y";
		//---------------------------------------------//
		const string FRZ_TOGGLE = MENU_PATH + "Lock(unlock) gameobject &#l";
		const string FRZ_UNLOCKALL = MENU_PATH + "Unlock all &%#l";
		//---------------------------------------------//
		const string EXT_HYPERGRAPH_B = MENU_PATH + "Open hypergraph in bottom bar %#x";
		const string EXT_HYPERGRAPH = MENU_PATH + "Open hypergraph window &%#x";
		const string EXT_BOOKMARKS = MENU_PATH + "Open bookmarks mod &%#n";
		const string EXT_PROJECTFOLDERS = MENU_PATH + "Open project folders mMod &%#m";
		//---------------------------------------------//
		const string EXT_MINIMIZE_BOTTOM = MENU_PATH + "Minimize bottom bar %#m";
		//---------------------------------------------//



		internal const int P = 10000;
		static PluginInstance adapter { get { return Root.p[ 0 ]; } }

		[MenuItem( SETTINGS, true, P + 3 )]
		static bool OpenSettings_IsValid() { return true; }
		[MenuItem( SETTINGS, false, P + 3 )]
		static void OpenSettings() { Settings.MainSettingsEnabler_Window.Select<Settings.MainSettingsEnabler_Window>(); }

		//---------------------------------------------//

		//[MenuItem( MENU_PATH_OLD + "Moved to Tools", true, P + 16 )]
		//public static bool Movasdaslid() { return false; }
		//[MenuItem( MENU_PATH_OLD + "Moved to Tools", false, P + 16 )]
		//public static void Movasdassid() { }

#if !EMX_H_LITE
		[MenuItem( SEL_BACK, true, P + 16 )]
		public static bool MoveSelPrev_IsValid() { return adapter.par_e.ENABLE_ALL && !adapter.par_e.USE_LAST_SELECTION_MOD; }
		[MenuItem( SEL_BACK, false, P + 16 )]
		public static void MoveSelPrev() { Mods.LastSelectionHistoryModInstance.MoveSelect( +1 ); }
		[MenuItem( SEL_FORW, true, P + 17 )]
		public static bool MoveSelNext_IsValid() { return adapter.par_e.ENABLE_ALL && !adapter.par_e.USE_LAST_SELECTION_MOD; }
		[MenuItem( SEL_FORW, false, P + 17 )]
		public static void MoveSelNext() { Mods.LastSelectionHistoryModInstance.MoveSelect( -1 ); }
#endif

		//---------------------------------------------//

		[MenuItem( FRZ_TOGGLE, true, P + 85 )]
		public static bool ToggleLock_IsValid() { return Mods.Mod_Freeze.IsValid(); }
		[MenuItem( FRZ_TOGGLE, false, P + 85 )]
		public static void ToggleLock() { Mods.Mod_Freeze.ToggleFreeze(); }
		[MenuItem( FRZ_UNLOCKALL, true, P + 89 )]
		public static bool UnlockAll_IsValid() { return Mods.Mod_Freeze.IsValid(); }
		[MenuItem( FRZ_UNLOCKALL, false, P + 89 )]
		public static void UnlockAll() { Mods.Mod_Freeze.UnclockAll(); }

		//---------------------------------------------//



		/*
                [MenuItem(MENU_PATH + "Welcome Screen", false, P + 129)]
                public static void OpenWelcomeScreen() { WelcomeScreen.Init(null); }
                */



#if !EMX_H_LITE



		[MenuItem( EXT_MINIMIZE_BOTTOM, true, P + 190 )]
		public static bool MinimizeBottomBar_IsValid() { return adapter.par_e.ENABLE_ALL && adapter.par_e.USE_BOTTOMBAR_MOD; }
		[MenuItem( EXT_MINIMIZE_BOTTOM, false, P + 190 )]
		public static void MinimizeBottomBar() { Root.p[ 0 ].modsController.bottomBarForHierarchy.SWITCH_MINIMIZR_FOR_FIRST_HIERARCY(); }


		[MenuItem( EXT_HYPERGRAPH, true, P + 285 )]
		public static bool HyperGraph_IsValid() { return adapter.par_e.ENABLE_ALL && adapter.par_e.USE_HYPERGRAPH_MOD; }
		[MenuItem( EXT_HYPERGRAPH, false, P + 285 )]
		public static void HyperGraph() { Mods.HyperGraph.HyperGraphModWindow.OpenWindow(); }

		[MenuItem( EXT_HYPERGRAPH_B, true, P + 286 )]
		public static bool HyperGraphFast_IsValid() { return adapter.par_e.ENABLE_ALL && adapter.par_e.USE_HYPERGRAPH_MOD && adapter.par_e.USE_BOTTOMBAR_MOD; }
		[MenuItem( EXT_HYPERGRAPH_B, false, P + 286 )]
		public static void HyperGraphFast() { Root.p[ 0 ].modsController.bottomBarForHierarchy.SWITCH_HYPER_ACTIVE_FOR_FIRST_HIERARCY(); }

		[MenuItem( EXT_BOOKMARKS, true, P + 387 )]
		public static bool OpenBookmark_IsValid() { return adapter.par_e.ENABLE_ALL && adapter.par_e.USE_BOOKMARKS_HIERARCHY_MOD; }
		[MenuItem( EXT_BOOKMARKS, false, P + 387 )]
		public static void OpenBookmark() { EMX.HierarchyPlugin.Editor.Mods.BookObject.BookmarksforGameObjectsModWindow.OpenWindow(); }

		[MenuItem( EXT_PROJECTFOLDERS, true, P + 388 )]
		public static bool ProjectFolders_IsValid() { return adapter.par_e.ENABLE_ALL && adapter.par_e.USE_BOOKMARKS_PROJECT_MOD; }
		[MenuItem( EXT_PROJECTFOLDERS, false, P + 388 )]
		public static void ProjectFolders() { Mods.BookProject.BookmarksforProjectviewModWindow.OpenWindow(); }





#endif



	}
}

