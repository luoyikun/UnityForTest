using UnityEngine;
using UnityEditor;



namespace EMX.HierarchyPlugin.Editor
{

	partial class EditorSettingsAdapter
	{

		PluginInstance p { get { return Root.p[ pluginID ]; } }

		//internal bool ENABLE_ALL { get { return Folders.GET_INTERNAL( "ENABLE_ALL", true ); } set { var r = ENABLE_ALL; Folders.SET_INTERNAL( "ENABLE_ALL", value ); } }
		internal bool ENABLE_ALL { get { return GET( "ENABLE_ALL", true ); } set { var r = ENABLE_ALL; SET( "ENABLE_ALL", value ); } }


		internal int SETTINGS_FILTER_INDEX {
			get { return SessionState.GetInt( "EMX|HierarchyPRO|Settings|CategoryInd", 0 ); }
			set { SessionState.SetInt( "EMX|HierarchyPRO|Settings|CategoryInd", value ); }
			//get {return GET( "DRAW_CATEGORIES", 0 ); } 
			//set { var r = DRAW_CATEGORIES; SET( "DRAW_CATEGORIES", value ); } 
		}
		internal int OPEN_SETTINGS_WINDOW { get { return GET( "OPEN_SETTINGS_WINDOW", 0 ); } set { var r = OPEN_SETTINGS_WINDOW; SET( "OPEN_SETTINGS_WINDOW", value ); } }

		internal bool LOG_SETTINGS_RED_MODE { get { return GET( "LOG_SETTINGS_RED_MODE", false ); } set { var r = LOG_SETTINGS_RED_MODE; SET( "LOG_SETTINGS_RED_MODE", value ); } }
		internal bool LOG_SETTINGS_WHITE_MODE { get { return GET( "LOG_SETTINGS_WHITE_MODE", true ); } set { var r = LOG_SETTINGS_WHITE_MODE; SET( "LOG_SETTINGS_WHITE_MODE", value ); } }

		// internal bool CREATE_SETTINGS_SPECIAL_WINDOW { get { return GET( "CREATE_SETTINGS_SPECIAL_WINDOW", true ); } set { var r = OPEN_SETTINGS_IN_INSPECTOR; SET( "CREATE_SETTINGS_SPECIAL_WINDOW", value ); } }
		// internal bool OPEN_SETTINGS_IN_INSPECTOR { get { return GET( "OPEN_SETTINGS_IN_INSPECTOR", true ); } set {var r = OPEN_SETTINGS_IN_INSPECTOR; SET( "OPEN_SETTINGS_IN_INSPECTOR", value ); } }

		internal bool USE_COLORS_FOR_CATEGORIES { get { return GET( "USE_COLORS_FOR_CATEGORIES", true ); } set { var r = USE_COLORS_FOR_CATEGORIES; SET( "USE_COLORS_FOR_CATEGORIES", value ); } }
		//internal Color COLORS_FOR_CATEGORIES { get { return GET( "COLORS_FOR_CATEGORIES", new Color32( 255, 185, 55, 255 ) ); } set { var r = COLORS_FOR_CATEGORIES; SET( "COLORS_FOR_CATEGORIES", value ); } }
		internal Color COLORS_FOR_CATEGORIES { get { return GET( "COLORS_FOR_CATEGORIES", Color.white ); } set { var r = COLORS_FOR_CATEGORIES; SET( "COLORS_FOR_CATEGORIES", value ); } }




		internal bool USE_TOPBAR_MOD { get { return HardDisableMods.USE_TOPBAR_MOD ?? GET( "USE_TOPBAR_MOD", true ); } set { var r = USE_TOPBAR_MOD; if ( SET( "USE_TOPBAR_MOD", value ) ) SessionState.SetBool( "EXM_TOOLBAR_BOOL", false ); p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_FONT_MOD { get { return GET( "USE_FONT_MOD", true ); } set { var r = USE_FONT_MOD; if ( SET( "USE_FONT_MOD", value ) ) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_AUTOSAVE_MOD { get { return HardDisableMods.USE_AUTOSAVE_MOD ?? GET( "USE_AUTOSAVE_MOD", false ); } set { var r = USE_AUTOSAVE_MOD; if ( SET( "USE_AUTOSAVE_MOD", value ) ) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_SNAP_MOD { get { return HardDisableMods.USE_SNAP_MOD ?? GET( "USE_SNAP_MOD", false ); } set { var r = USE_SNAP_MOD; if ( SET( "USE_SNAP_MOD", value ) ) p.modsController.REBUILD_PLUGINS(); Mods.SnapMod.SET_ENABLE( value && ENABLE_ALL ); } }
		internal bool USE_RIGHT_CLICK_MENU_MOD { get { return HardDisableMods.USE_RIGHT_CLICK_MENU_MOD ?? GET( "USE_RIGHT_CLICK_MENU_MOD", true ); } set { var r = USE_RIGHT_CLICK_MENU_MOD; if ( SET( "USE_RIGHT_CLICK_MENU_MOD", value ) ) p.modsController.REBUILD_PLUGINS(); } }

		internal bool USE_RIGHT_ALL_MODS { get { return HardDisableMods.USE_RIGHT_ALL_MODS ?? GET( "USE_RIGHT_ALL_MODS", true ); } set { var r = USE_RIGHT_ALL_MODS; if ( SET( "USE_RIGHT_ALL_MODS", value ) ) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_SETACTIVE_MOD { get { return HardDisableMods.USE_SETACTIVE_MOD ?? GET( "USE_SETACTIVE_MOD", true ); } set { var r = USE_SETACTIVE_MOD; if ( SET( "USE_SETACTIVE_MOD", value ) ) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_COMPONENTS_ICONS_MOD { get { return HardDisableMods.USE_COMPONENTS_ICONS_MOD ?? GET( "USE_COMPONENTS_ICONS_MOD", true ); } set { var r = USE_COMPONENTS_ICONS_MOD; if ( SET( "USE_COMPONENTS_ICONS_MOD", value ) ) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_PLAYMODE_SAVER_MOD { get { return HardDisableMods.USE_PLAYMODE_SAVER_MOD ?? GET( "USE_PLAYMODE_SAVER_MOD", false ); } set { var r = USE_PLAYMODE_SAVER_MOD; if ( SET( "USE_PLAYMODE_SAVER_MOD", value ) ) p.modsController.REBUILD_PLUGINS(); } }

		internal bool USE_HIERARCHY_MANUAL_HIGHLIGHTER_MOD { get { return HardDisableMods.USE_HIERARCHY_MANUAL_HIGHLIGHTER_MOD ?? GET( "USE_HIERARCHY_MANUAL_HIGHLIGHTER_MOD", true ); } set { var r = USE_HIERARCHY_MANUAL_HIGHLIGHTER_MOD; if ( SET( "USE_HIERARCHY_MANUAL_HIGHLIGHTER_MOD", value ) ) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_HIERARCHY_AUTO_HIGHLIGHTER_MOD { get { return HardDisableMods.USE_HIERARCHY_AUTO_HIGHLIGHTER_MOD ?? GET( "USE_HIERARCHY_AUTO_HIGHLIGHTER_MOD", true ); } set { var r = USE_HIERARCHY_AUTO_HIGHLIGHTER_MOD; if ( SET( "USE_HIERARCHY_AUTO_HIGHLIGHTER_MOD", value ) ) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_CUSTOM_PRESETS_MOD { get { return HardDisableMods.USE_PRESETSMANAGER_FORCOMPONENTS_MOD ?? GET( "USE_CUSTOM_PRESETS_MOD", true ); } set { var r = USE_CUSTOM_PRESETS_MOD; if ( SET( "USE_CUSTOM_PRESETS_MOD", value ) ) p.modsController.REBUILD_PLUGINS(); } }

		internal bool USE_PROJECT_SETTINGS {
			get { return HardDisableMods.USE_PROJECT_SETTINGS ?? GET( "USE_PROJECT_SETTINGS", false ); }
			set {
				var r = USE_PROJECT_SETTINGS;
				if ( SET( "USE_PROJECT_SETTINGS", value ) ) p.modsController.REBUILD_PLUGINS();
				if ( !value ) Root.RequestScriptReload();

			}
		}
		internal bool USE_PROJECT_MANUAL_HIGHLIGHTER_MOD {
			get { return HardDisableMods.USE_PROJECT_MANUAL_HIGHLIGHTER_MOD ?? GET( "USE_PROJECT_MANUAL_HIGHLIGHTER_MOD", false ); }
			set {
				var r = USE_PROJECT_MANUAL_HIGHLIGHTER_MOD;
				if ( SET( "USE_PROJECT_MANUAL_HIGHLIGHTER_MOD", value ) ) p.modsController.REBUILD_PLUGINS();
				if ( !USE_PROJECT_MANUAL_HIGHLIGHTER_MOD && !USE_PROJECT_AUTO_HIGHLIGHTER_MOD ) Root.RequestScriptReload();
			}
		}
		internal bool USE_PROJECT_AUTO_HIGHLIGHTER_MOD {
			get { return HardDisableMods.USE_PROJECT_AUTO_HIGHLIGHTER_MOD ?? GET( "USE_PROJECT_AUTO_HIGHLIGHTER_MOD", false ); }
			set {
				var r = USE_PROJECT_AUTO_HIGHLIGHTER_MOD;
				if ( SET( "USE_PROJECT_AUTO_HIGHLIGHTER_MOD", value ) ) p.modsController.REBUILD_PLUGINS();
				if ( !USE_PROJECT_MANUAL_HIGHLIGHTER_MOD && !USE_PROJECT_AUTO_HIGHLIGHTER_MOD ) Root.RequestScriptReload();
			}
		}
		internal bool USE_PROJECT_GUI_EXTENSIONS { get { return HardDisableMods.USE_PROJECT_GUI_EXTENSIONS ?? GET( "USE_PROJECT_GUI_EXTENSIONS", false ); } set { var r = USE_PROJECT_GUI_EXTENSIONS; if ( SET( "USE_PROJECT_GUI_EXTENSIONS", value ) ) p.modsController.REBUILD_PLUGINS(); } }

#if !EMX_H_LITE
		internal bool USE_BOOKMARKS_HIERARCHY_MOD { get { return HardDisableMods.USE_BOOKMARKS_HIERARCHY_MOD ?? GET( "USE_BOOKMARKS_HIERARCHY_MOD", true ); } set { var r = USE_BOOKMARKS_HIERARCHY_MOD; if ( SET( "USE_BOOKMARKS_HIERARCHY_MOD", value ) ) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_BOOKMARKS_PROJECT_MOD { get { return HardDisableMods.USE_BOOKMARKS_PROJECT_MOD ?? GET( "USE_BOOKMARKS_PROJECT_MOD", true ); } set { var r = USE_BOOKMARKS_PROJECT_MOD; if ( SET( "USE_BOOKMARKS_PROJECT_MOD", value ) ) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_LAST_SELECTION_MOD { get { return HardDisableMods.USE_LAST_SELECTION_MOD ?? GET( "USE_LAST_SELECTION_MOD", true ); } set { var r = USE_LAST_SELECTION_MOD; if ( SET( "USE_LAST_SELECTION_MOD", value ) ) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_LAST_SCENES_MOD { get { return HardDisableMods.USE_LAST_SCENES_MOD ?? GET( "USE_LAST_SCENES_MOD", true ); } set { var r = USE_LAST_SCENES_MOD; if ( SET( "USE_LAST_SCENES_MOD", value ) ) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_HIER_EXPANDED_MOD { get { return HardDisableMods.USE_HIER_EXPANDED_MOD ?? GET( "USE_HIER_EXPANDED_MOD", true ); } set { var r = USE_HIER_EXPANDED_MOD; if ( SET( "USE_HIER_EXPANDED_MOD", value ) ) p.modsController.REBUILD_PLUGINS(); } }

		internal bool USE_HYPERGRAPH_MOD { get { return HardDisableMods.USE_HYPERGRAPH_MOD ?? GET( "USE_HYPERGRAPH_MOD", true ); } set { var r = USE_HYPERGRAPH_MOD; if ( SET( "USE_HYPERGRAPH_MOD", value ) ) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_BOTTOMBAR_MOD {
			get { return HardDisableMods.USE_BOTTOMBAR_MOD ?? GET( "USE_BOTTOMBAR_MOD", true ); }
			set {
				var r = USE_BOTTOMBAR_MOD;
				if ( SET( "USE_BOTTOMBAR_MOD", value ) )
				{
					p.modsController.REBUILD_PLUGINS();
					if ( !USE_BOTTOMBAR_MOD )
						foreach ( var item in PluginInstance.WindowsData( pluginID ) )
							if ( item.Value.w.Instance )
								item.Value.w.RESET_BOTTOM();

				}
			}
		}
#else
        internal bool USE_BOOKMARKS_HIERARCHY_MOD { get { return false; } set { } }
        internal bool USE_BOOKMARKS_PROJECT_MOD { get { return false; } set { } }
        internal bool USE_LAST_SELECTION_MOD { get { return false; } set { } }
        internal bool USE_LAST_SCENES_MOD { get { return false; } set { } }
        internal bool USE_HIER_EXPANDED_MOD { get { return false; } set { } }

        internal bool USE_HYPERGRAPH_MOD { get { return false; } set { } }
        internal bool USE_BOTTOMBAR_MOD { get { return false; } set { } }
#endif



	}

}
