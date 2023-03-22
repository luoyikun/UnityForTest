using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Globalization;

namespace EMX.HierarchyPlugin.Editor
{

	partial class EditorSettingsAdapter
	{

		int pluginID;
		//  Saver s ;
		internal EditorSettingsAdapter( int pId )
		{
			pluginID = pId;
		}


		HighlighterSettings _HIER_HIGH_SET;
		internal HighlighterSettings HIER_HIGH_SET { get { return _HIER_HIGH_SET ?? (_HIER_HIGH_SET = new HighlighterSettings( "HIGHLIGHTER_HIERARCHY", 0 )); } }
		HighlighterSettings _PROJ_HIGH_SET;
		internal HighlighterSettings PROJ_HIGH_SET { get { return _PROJ_HIGH_SET ?? (_PROJ_HIGH_SET = new HighlighterSettings( "HIGHLIGHTER_PROJECT", 1 )); } }


		WindowSettings _HIER_WIN_SET;
		internal WindowSettings HIER_WIN_SET { get { return _HIER_WIN_SET ?? (_HIER_WIN_SET = new WindowSettings( "WIN_HIERARCHY", 0 )); } }
		WindowSettings _PROJ_WIN_SET;
		internal WindowSettings PROJ_WIN_SET { get { return _PROJ_WIN_SET ?? (_PROJ_WIN_SET = new WindowSettings( "WIN_PROJECT", 1 )); } }



		internal bool SAVE_HIGHLIGHTER_SETS_TO_HIDENFOLDER { get { return false; } }


		internal Color BUTTON_TAP_COLOR {
			//get { return GET( "COMMON" + "_BUTTON_TAP_COLOR", new Color32( 50, 225, 255, 255 / 2 ) ); }
			get { return GET( "COMMON" + "_BUTTON_TAP_COLOR", new Color32( 0x42, 0x9C, 0xDD, 255 / 2 ) ); }
			set {
				var r = BUTTON_TAP_COLOR;
				if ( SET( "COMMON" + "_BUTTON_TAP_COLOR", value ) ) p.RepaintWindowInUpdate( pluginID );
			}
		}



		static string[] WELCOME_PRESETS_ARRAY = {
			"",
			"[True$17$True$15$True$11$True$19$True$2$2$116-116-116-72$0-0-0-18]",
            //"[True$19$True$14$True$12$True$18$True$2$2$87-87-87-80$20-20-20-16]",
            "[True$13$True$2$True$10$True$10$True$0$2$24-24-24-80$105-105-105-16]",
			"[True$18$True$16$False$12$True$20$True$2$0$125-125-125-65$105-105-105-16]"
		};

		internal int WELCOME_PRESETS {
			get {
				return GET( "WELCOME_PRESETS", 0 );
			}
			set {
				if ( value != 0 )
				{
					if ( SessionState.GetString( "WELCOME_PRESETS_CACHE", "" ) == "" )
					{
						SessionState.SetString( "WELCOME_PRESETS_CACHE", WP_TO_S() );
					}
					S_TO_WP( WELCOME_PRESETS_ARRAY[ value ] );
				}
				else
				{
					if ( SessionState.GetString( "WELCOME_PRESETS_CACHE", "" ) == "" ) return;
					S_TO_WP( SessionState.GetString( "WELCOME_PRESETS_CACHE", "" ) );
					SessionState.SetString( "WELCOME_PRESETS_CACHE", "" );
				}
			}
		}


		public void CopyPresetToBuffer()
		{
			EditorGUIUtility.systemCopyBuffer = "[" + WP_TO_S() + "]";
		}
		public bool PastePresetToBufferValidate()
		{
			if ( string.IsNullOrEmpty( EditorGUIUtility.systemCopyBuffer ) ) return false;
			var s = EditorGUIUtility.systemCopyBuffer;
			s = s.Trim( new[] { '\n', '\r', '\t', ' ', '[', ']', '(', ')', '{', '}' } );
			var sp = s.Split('$');
			if ( sp.Length < 10 ) return false;
			return true;
		}
		public void PastePresetToBuffer()
		{
			if ( !PastePresetToBufferValidate() ) return;
			S_TO_WP( EditorGUIUtility.systemCopyBuffer );
		}

		string WP_TO_S()
		{
			var res = "";
			res += HIER_WIN_SET.USE_LINE_HEIGHT.ToString() + '$';
			res += HIER_WIN_SET.LINE_HEIGHT.ToString() + '$';
			res += HIER_WIN_SET.USE_OVERRIDE_DEFAULT_ICONS_SIZE.ToString() + '$';
			res += HIER_WIN_SET.OVERRIDE_DEFAULT_ICONS_SIZE.ToString() + '$';
			res += HIER_WIN_SET.USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE.ToString() + '$';
			res += HIER_WIN_SET.OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE.ToString() + '$';
			res += HIER_WIN_SET.USE_CHILD_INDENT.ToString() + '$';
			res += HIER_WIN_SET.CHILD_INDENT.ToString() + '$';
			res += HIER_WIN_SET.USE_BACKGROUNDDECORATIONS_MOD.ToString() + '$';
			res += HIER_WIN_SET.DRAW_HIERARHCHY_CHESS_LINES.ToString() + '$';
			res += HIER_WIN_SET.DRAW_HIERARHCHY_CHESS_FILLS.ToString() + '$';
			res += c_to_s( HIER_WIN_SET.COLOR_HIERARHCHY_CHESS_LINES ).ToString() + '$';
			res += c_to_s( HIER_WIN_SET.COLOR_HIERARHCHY_CHESS_FILLS ).ToString();
			return res;

		}
		void S_TO_WP( string s )
		{
			s = s.Trim( new[] { '\n', '\r', '\t', ' ', '[', ']', '(', ')', '{', '}' } );
			var sp = s.Split('$');
			if ( sp.Length < 10 ) return;
			try
			{
				int i = 0;
				var USE_LINE_HEIGHT = bool.Parse( sp[ i++ ] );
				var LINE_HEIGHT = int.Parse( sp[ i++ ] );
				var USE_OVERRIDE_DEFAULT_ICONS_SIZE = bool.Parse( sp[ i++ ] );
				var OVERRIDE_DEFAULT_ICONS_SIZE = int.Parse( sp[ i++ ] );
				var USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE = bool.Parse( sp[ i++ ] );
				var OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE = int.Parse( sp[ i++ ] );
				var USE_CHILD_INDENT = bool.Parse( sp[ i++ ] );
				var CHILD_INDENT = int.Parse( sp[ i++ ] );
				var USE_BACKGROUNDDECORATIONS_MOD = bool.Parse( sp[ i++ ] );
				var DRAW_HIERARHCHY_CHESS_LINES = int.Parse( sp[ i++ ] );
				var DRAW_HIERARHCHY_CHESS_FILLS = int.Parse( sp[ i++ ] );
				var COLOR_HIERARHCHY_CHESS_LINES = s_to_c( sp[ i++ ] );
				var COLOR_HIERARHCHY_CHESS_FILLS = s_to_c( sp[ i++ ] );

				HIER_WIN_SET.USE_LINE_HEIGHT = USE_LINE_HEIGHT;
				HIER_WIN_SET.LINE_HEIGHT = LINE_HEIGHT;
				HIER_WIN_SET.USE_OVERRIDE_DEFAULT_ICONS_SIZE = USE_OVERRIDE_DEFAULT_ICONS_SIZE;
				HIER_WIN_SET.OVERRIDE_DEFAULT_ICONS_SIZE = OVERRIDE_DEFAULT_ICONS_SIZE;
				HIER_WIN_SET.USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE = USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE;
				HIER_WIN_SET.OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE = OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE;
				HIER_WIN_SET.USE_CHILD_INDENT = USE_CHILD_INDENT;
				HIER_WIN_SET.CHILD_INDENT = CHILD_INDENT;
				HIER_WIN_SET.USE_BACKGROUNDDECORATIONS_MOD = USE_BACKGROUNDDECORATIONS_MOD;
				HIER_WIN_SET.DRAW_HIERARHCHY_CHESS_LINES = DRAW_HIERARHCHY_CHESS_LINES;
				HIER_WIN_SET.DRAW_HIERARHCHY_CHESS_FILLS = DRAW_HIERARHCHY_CHESS_FILLS;
				HIER_WIN_SET.COLOR_HIERARHCHY_CHESS_LINES = COLOR_HIERARHCHY_CHESS_LINES;
				HIER_WIN_SET.COLOR_HIERARHCHY_CHESS_FILLS = COLOR_HIERARHCHY_CHESS_FILLS;
			}
			catch ( Exception ex )
			{
				EditorUtility.DisplayDialog( Root.HierarchyPro + " Settings", "Cannot paste settings from buffer " + s + "\n\n" + ex.StackTrace
					, "Ok" );
			}

		}
		Color s_to_c( string s )
		{
			var sp = s.Split( '-' );
			var res = new Color32();
			res.r = byte.Parse( sp[ 0 ] );
			res.g = byte.Parse( sp[ 1 ] );
			res.b = byte.Parse( sp[ 2 ] );
			res.a = byte.Parse( sp[ 3 ] );
			return res;
		}
		string c_to_s( Color _c )
		{
			Color32 c = _c;
			return c.r + "-" + c.g + "-" + c.b + "-" + c.a;
		}

		//for ( int i = 0; i < 4; i++ )
		//return c[ 0 ] + "-" + c[ 1 ] + "-" + c[ 2 ] + "-" + c[ 3 ];


		internal bool WELCOME_BACKGROUND {
			get { return HIER_WIN_SET.DRAW_HIERARHCHY_CHESS_LINES != 0 && HIER_WIN_SET.DRAW_HIERARHCHY_CHESS_FILLS != 0; }
			set {
				if ( value )
				{
					HIER_WIN_SET.USE_BACKGROUNDDECORATIONS_MOD = true;
					HIER_WIN_SET.DRAW_HIERARHCHY_CHESS_LINES = 2;
					HIER_WIN_SET.DRAW_HIERARHCHY_CHESS_FILLS = 2;
				}
				else
				{
					HIER_WIN_SET.DRAW_HIERARHCHY_CHESS_LINES = 0;
					HIER_WIN_SET.DRAW_HIERARHCHY_CHESS_FILLS = 0;
				}
			}
		}
		internal bool WELCOME_CHILD {
			get { return HIER_WIN_SET.DRAW_HIERARHCHY_CHILD_LINES != 0; }
			set {
				HIER_WIN_SET.DRAW_HIERARHCHY_CHILD_LINES = value ? 1 : 0;
			}
		}
		internal bool WELCOME_FILL_RIGHT {
			get { return RIGHT_BG_OPACITY > 0.1f && RIGHTDOCK_DRAW_VERTICAL_SEPARATORS; }
			set {
				if ( value )
				{
					RIGHTDOCK_DRAW_VERTICAL_SEPARATORS = true;
					RIGHT_BG_OPACITY = 0.5f;
				}
				else
				{
					RIGHTDOCK_DRAW_VERTICAL_SEPARATORS = false;
					RIGHT_BG_OPACITY = 0.0f;
				}
			}
		}
		internal bool WELCOME_SHOW_IN_EVERY_PROJECTS {
			get { return EditorPrefs.GetInt( Folders.PREFS_PATH + "|showWelcomeInEvery", 1 ) == 1; }
			set { EditorPrefs.SetInt( Folders.PREFS_PATH + "|showWelcomeInEvery", value ? 1 : 0 ); }
		}
		internal bool WELCOME_WERE_IN_EVERY_PROJECTS {
			//get { return Folders.GET_INTERNAL( "WELCOME_WERE_IN_EVERY_PROJECTS", false ); }
			//set { var r = WELCOME_WERE_IN_EVERY_PROJECTS; Folders.SET_INTERNAL( "WELCOME_WERE_IN_EVERY_PROJECTS", value ); }
			get { return GET( "WELCOME_WERE_IN_EVERY_PROJECTS", false ); }
			set { var r = WELCOME_WERE_IN_EVERY_PROJECTS; SET( "WELCOME_WERE_IN_EVERY_PROJECTS", value ); }
		}


		internal partial class WindowSettings
		{
			internal string KEY = "";
			internal int pluginID;

			internal WindowSettings( string key, int pluginID )
			{
				KEY = key;
				this.pluginID = pluginID;
			}

			PluginInstance p { get { return Root.p[ 0 ]; } }
			EditorSettingsAdapter s { get { return Root.p[ 0 ].par_e; } }




			internal bool USE_LINE_HEIGHT {
				get { return s.GET( KEY + "_USE_LINE_HEIGHT", pluginID == 0 ); }
				set {
					var r = USE_LINE_HEIGHT;
					if ( s.SET( KEY + "_USE_LINE_HEIGHT", value ) )
					{
						if ( !USE_LINE_HEIGHT ) foreach ( var item in PluginInstance.WindowsData( pluginID ) ) if ( item.Value.w.Instance ) item.Value.w.RESET_LINE_HEIGHT( pluginID );
						p.RepaintWindowInUpdate_PlusResetStack( pluginID );
					}
				}
			}
			internal int LINE_HEIGHT { get { return Mathf.Clamp( s.GET( KEY + "_LINE_HEIGHT", 20 ), 10, 80 ); } set { var r = LINE_HEIGHT; if ( s.SET( KEY + "_LINE_HEIGHT", value ) ) p.RepaintWindowInUpdate_PlusResetStack( pluginID ); } }

			internal bool USE_CHILD_INDENT {
				get { return s.GET( KEY + "_USE_CHILD_INDENT", false ); }
				set {
					var r = USE_CHILD_INDENT;
					if ( s.SET( KEY + "_USE_CHILD_INDENT", value ) )
					{
						if ( !USE_CHILD_INDENT ) foreach ( var item in PluginInstance.WindowsData( pluginID ) ) if ( item.Value.w.Instance ) item.Value.w.RESET_CHILD_INDENT( pluginID );
						p.RepaintWindowInUpdate_PlusResetStack( pluginID );
					}
				}
			}
			internal int CHILD_INDENT { get { return s.GET( KEY + "_CHILD_INDENT", 14 ); } set { var r = CHILD_INDENT; if ( s.SET( KEY + "_CHILD_INDENT", value ) ) p.RepaintWindowInUpdate_PlusResetStack( pluginID ); } }

			internal bool USE_OVERRIDE_DEFAULT_ICONS_SIZE {
				get { return s.GET( KEY + "_USE_OVERRIDE_DEFAULT_ICONS_SIZE",false ); }
				set {
					var r = USE_OVERRIDE_DEFAULT_ICONS_SIZE;
					if ( s.SET( KEY + "_USE_OVERRIDE_DEFAULT_ICONS_SIZE", value ) )
					{
						if ( !USE_OVERRIDE_DEFAULT_ICONS_SIZE ) foreach ( var item in PluginInstance.WindowsData( pluginID ) ) if ( item.Value.w.Instance ) item.Value.w.RESET_DEFAULT_ICON_SIZE( pluginID );
						p.RepaintWindowInUpdate_PlusResetStack( pluginID );
					}
				}
			}
			internal int OVERRIDE_DEFAULT_ICONS_SIZE { get { return s.GET( KEY + "_OVERRIDE_DEFAULT_ICONS_SIZE", pluginID == 0 ? (int)11 : (int)16 ); } set { var r = OVERRIDE_DEFAULT_ICONS_SIZE; if ( s.SET( KEY + "_OVERRIDE_DEFAULT_ICONS_SIZE", value ) ) p.RepaintWindowInUpdate_PlusResetStack( pluginID ); } }

			internal bool USE_HORISONTAL_SCROLL { get { return s.GET( KEY + "_USE_HORIZONTAL_SCROLL", false ); } set { var r = USE_HORISONTAL_SCROLL; if ( s.SET( KEY + "_USE_HORIZONTAL_SCROLL", value ) ) p.RepaintWindowInUpdate_PlusResetStack( pluginID ); } }
			// For Right Modules


			// internal int BOTOM_LABELS_FONT_SIZE {
			//     get { return s.GET( KEY + "_BOTOM_LABELS_FONT_SIZE", 11 ); }
			//     set {
			//         var r = BOTOM_LABELS_FONT_SIZE;
			//         if ( s.SET( KEY + "_BOTOM_LABELS_FONT_SIZE", value ) ) p.RepaintWindowInUpdate_PlusResetStack( pluginID );
			//     }
			// }

			internal int COMMON_LABELS_FONT_SIZE {
				get { return s.GET( KEY + "_COMMON_LABELS_FONT_SIZE", 11 ); }
				set {
					var r = COMMON_LABELS_FONT_SIZE;
					if ( s.SET( KEY + "_COMMON_LABELS_FONT_SIZE", value ) ) p.RepaintWindowInUpdate_PlusResetStack( pluginID );
				}
			}
			internal int RIGHTMOD_HEADER_FONT_SIZE {
				get { return s.GET( KEY + "_RIGHTMOD_HEADER_FONT_SIZE", 10 ); }
				set {
					var r = RIGHTMOD_HEADER_FONT_SIZE;
					if ( s.SET( KEY + "_RIGHTMOD_HEADER_FONT_SIZE", value ) )
					{
						RightModsStyles.ClearLabels();
						p.RepaintWindowInUpdate_PlusResetStack( pluginID );
					}
				}
			}

			internal int RIGHTMOD_LABELS_FONT_SIZE {
				get { return s.GET( KEY + "_PLUGIN_LABELS_FONT_SIZE", 11 ); }
				set {
					var r = RIGHTMOD_LABELS_FONT_SIZE;
					if ( s.SET( KEY + "_PLUGIN_LABELS_FONT_SIZE", value ) )
					{
						RightModsStyles.ClearLabels();
						p.RepaintWindowInUpdate_PlusResetStack( pluginID );
					}
				}
			}
			internal int ICONSMOD_LABELS_FONT_SIZE {
				get { return s.GET( KEY + "_ICONSMOD_LABELS_FONT_SIZE", 12 ); }
				set {
					var r = ICONSMOD_LABELS_FONT_SIZE;
					if ( s.SET( KEY + "_ICONSMOD_LABELS_FONT_SIZE", value ) ) p.RepaintWindowInUpdate_PlusResetStack( pluginID );
				}
			}

			internal bool USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE {
				get { return s.GET( KEY + "_USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE", false ); }
				set {
					var r = USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE;
					if ( s.SET( KEY + "_USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE", value ) )
					{
						if ( !USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE ) foreach ( var item in PluginInstance.WindowsData( pluginID ) ) if ( item.Value.w.Instance ) item.Value.w.RESET_GAMEOBJECTS_NAMES( pluginID );
						NewClearHelper.OnFontSizeChanged();
						p.RepaintWindowInUpdate_PlusResetStack( pluginID );
					}
				}
			}
			internal int OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE {
				get { return s.GET( KEY + "_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE", EditorStyles.label.fontSize == 0 ? 12 : EditorStyles.label.fontSize ); }
				set {
					var r = OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE;
					if ( s.SET( KEY + "_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE", value ) ) p.RepaintWindowInUpdate_PlusResetStack( pluginID );
					NewClearHelper.OnFontSizeChanged();
				}
			}






			internal bool USE_BACKGROUNDDECORATIONS_MOD {
				get {
					//if ( pluginID == 1 && !p.par_e.USE_PROJECT_SETTINGS ) return false;
					return HardDisableMods.USE_BACKGROUNDDECORATIONS_MOD ?? s.GET( KEY + "_USE_BACKGROUNDDECORATIONS_MOD", true );
				}
				set {
					// if ( pluginID == 1 && !p.par_e.USE_PROJECT_SETTINGS ) return;
					var r = USE_BACKGROUNDDECORATIONS_MOD;
					if ( s.SET( KEY + "_USE_BACKGROUNDDECORATIONS_MOD", value ) ) p.modsController.REBUILD_PLUGINS();
					if ( pluginID == 1 && !value ) Root.RequestScriptReload();
				}
			}




			internal int DRAW_HIERARHCHY_CHESS_LINES { get { return s.GET( KEY + "_DRAW_HIERARHCHY_CHESS_LINES", 2 ); } set { var r = DRAW_HIERARHCHY_CHESS_LINES; if ( s.SET( KEY + "_DRAW_HIERARHCHY_CHESS_LINES", value ) ) p.RepaintWindowInUpdate( pluginID ); } }
			internal Color COLOR_HIERARHCHY_CHESS_LINES {

				//get { return s.GET( KEY + "_COLOR_HIERARHCHY_CHESS_LINES_V2", EditorGUIUtility.isProSkin ? new Color( .1f, .1f, .1f, 0.4F ) : new Color( .1f, .1f, .1f, 0.1F / 3 ) ); }
				get { return s.GET( KEY + "_COLOR_HIERARHCHY_CHESS_LINES_V2", EditorGUIUtility.isProSkin ? (Color) new Color32( 0x62, 0x62, 0x62, 255 ) : new Color( .1f, .1f, .1f, 0.27f ) ); }
				set { var r = COLOR_HIERARHCHY_CHESS_LINES; if ( s.SET( KEY + "_COLOR_HIERARHCHY_CHESS_LINES_V2", value ) ) p.RepaintWindowInUpdate( pluginID ); }
			}


			internal int DRAW_HIERARHCHY_CHESS_FILLS {
				get {
					return s.GET( KEY + "_DRAW_HIERARHCHY_CHESS_FILLS",
//pluginID != 0 ? 2 : (EditorGUIUtility.isProSkin ? 2 : 1)
2
);
				}
				set { var r = DRAW_HIERARHCHY_CHESS_FILLS; if ( s.SET( KEY + "_DRAW_HIERARHCHY_CHESS_FILLS", value ) ) p.RepaintWindowInUpdate( pluginID ); }
			}
			internal Color COLOR_HIERARHCHY_CHESS_FILLS {
				get { return s.GET( KEY + "_COLOR_HIERARHCHY_CHESS_FILLS", EditorGUIUtility.isProSkin ? new Color( 20 / 255f, 20 / 255f, 20 / 255f, 0.1F ) : new Color( 1f, 1f, 1f, 0.16F ) ); }
				set { var r = COLOR_HIERARHCHY_CHESS_FILLS; if ( s.SET( KEY + "_COLOR_HIERARHCHY_CHESS_FILLS", value ) ) p.RepaintWindowInUpdate( pluginID ); }
			}


			internal int DRAW_HIERARHCHY_CHILD_LINES {
				//get { return s.GET( KEY + "_DRAW_HIERARHCHY_CHILD_LINES", pluginID == 0 ? 1 : 0 ); }
				get { return s.GET( KEY + "_DRAW_HIERARHCHY_CHILD_LINES", 1 ); }
				set { var r = DRAW_HIERARHCHY_CHILD_LINES; if ( s.SET( KEY + "_DRAW_HIERARHCHY_CHILD_LINES", value ) ) p.RepaintWindowInUpdate( pluginID ); }
			}
			internal Color COLOR_HIERARHCHY_CHILD_LINES {
				get { return s.GET( KEY + "_COLOR_HIERARHCHY_CHILD_LINES", EditorGUIUtility.isProSkin ? new Color( 0.8f, 0.8f, 0.8f, 0.35f ) : (Color)new Color32( 0x32, 0x32, 0x32, 160 ) ); } //new Color( .5f, .5f, .5f, 100 / 255F ) ); }
				set { var r = COLOR_HIERARHCHY_CHILD_LINES; if ( s.SET( KEY + "_COLOR_HIERARHCHY_CHILD_LINES", value ) ) p.RepaintWindowInUpdate( pluginID ); }
			}
			internal int HIERARHCHY_CHILD_LINES_DOTS {
				get { return s.GET( KEY + "_HIERARHCHY_CHILD_LINES_DOTS", 3 ); }
				set { var r = COLOR_HIERARHCHY_CHILD_LINES; if ( s.SET( KEY + "_HIERARHCHY_CHILD_LINES_DOTS", value ) ) p.RepaintWindowInUpdate( pluginID ); }
			}
			internal float HIERARHCHY_CHILD_LINES_TALPHA { get { return s.GET( KEY + "_HIERARHCHY_CHILD_LINES_TALPHA", 0.66f ); } set { var r = HIERARHCHY_CHILD_LINES_TALPHA; if ( s.SET( KEY + "_HIERARHCHY_CHILD_LINES_TALPHA", value ) ) p.RepaintWindowInUpdate( pluginID ); } }
			internal int HIERARHCHY_CHILD_LINES_DOT_SIZE { get { return s.GET( KEY + "_HIERARHCHY_CHILD_LINES_DOT_SIZE", 3 ); } set { var r = HIERARHCHY_CHILD_LINES_DOT_SIZE; if ( s.SET( KEY + "_HIERARHCHY_CHILD_LINES_DOT_SIZE", value ) ) p.RepaintWindowInUpdate( pluginID ); } }


			internal int DRAW_CHILDS_COUNT {
				//get { return s.GET( KEY + "_DRAW_CHILDS_COUNT", pluginID == 1 ? 1 : 0 ); }
				get { return s.GET( KEY + "_DRAW_CHILDS_COUNT", pluginID == 0 ? 1 : 0 ); }
				set { var r = DRAW_CHILDS_COUNT; if ( s.SET( KEY + "_DRAW_CHILDS_COUNT", value ) ) p.RepaintWindowInUpdate( pluginID ); }
			}
			internal bool DRAW_CHILDS_INVERCE_COLOR { get { return s.GET( KEY + "_DRAW_CHILDS_INVERCE_COLOR", !EditorGUIUtility.isProSkin ); } set { var r = DRAW_CHILDS_INVERCE_COLOR; if ( s.SET( KEY + "_DRAW_CHILDS_INVERCE_COLOR", value ) ) p.RepaintWindowInUpdate( pluginID ); } }
			internal Color DRAW_CHILDS_COLOR {
				get {
					return s.GET( KEY + "_DRAW_CHILDS_COLOR",
EditorGUIUtility.isProSkin ? new Color( 0.45f, 0.45f, 0.45f, 1f ) : new Color( 0.8f, 0.8f, 0.8f, 1f ) );
				}
				//EditorGUIUtility.isProSkin ? new Color( 1, 1, 1, 0.33f ) : (Color)new Color32( 0x32, 0x32, 0x32, 160 ) );}
				set { var r = DRAW_CHILDS_COLOR; if ( s.SET( KEY + "_DRAW_CHILDS_COLOR", value ) ) p.RepaintWindowInUpdate( pluginID ); }
			}

			internal bool CHILDCOUNT_COMBINE { get { return s.GET( KEY + "_CHILDCOUNT_COMBINE", true ); } set { var r = CHILDCOUNT_COMBINE; if ( s.SET( KEY + "_CHILDCOUNT_COMBINE", value ) ) p.RepaintWindowInUpdate( pluginID ); } }

			internal bool HIDE_CHILDCOUNT_IFROOT { get { return s.GET( KEY + "_HIDE_CHILDCOUNT_IFROOT", false ); } set { var r = HIDE_CHILDCOUNT_IFROOT; if ( s.SET( KEY + "_HIDE_CHILDCOUNT_IFROOT", value ) ) p.RepaintWindowInUpdate( pluginID ); } }
			internal bool HIDE_CHILDCOUNT_IFEXPANDED { get { return s.GET( KEY + "_HIDE_CHILDCOUNT_IFEXPANDED", false ); } set { var r = HIDE_CHILDCOUNT_IFEXPANDED; if ( s.SET( KEY + "_HIDE_CHILDCOUNT_IFEXPANDED", value ) ) p.RepaintWindowInUpdate( pluginID ); } }
			internal int CHILDCOUNT_ALIGMENT { get { return s.GET( KEY + "_CHILDCOUNT_ALIGMENT", pluginID == 0 ? 0 : 1 ); } set { var r = CHILDCOUNT_ALIGMENT; if ( s.SET( KEY + "_CHILDCOUNT_ALIGMENT", value ) ) p.RepaintWindowInUpdate( pluginID ); } }
			internal int CHILDCOUNT_SIZE { get { return s.GET( KEY + "_CHILDCOUNT_SIZE", 12 ); } set { var r = CHILDCOUNT_SIZE; if ( s.SET( KEY + "_CHILDCOUNT_SIZE", value ) ) p.RepaintWindowInUpdate( pluginID ); } }
			internal int CHILDCOUNT_OFFSET_X { get { return s.GET( KEY + "_CHILDCOUNT_OFFSET_X", pluginID == 0 ? 20 : 30 ); } set { var r = CHILDCOUNT_OFFSET_X; if ( s.SET( KEY + "_CHILDCOUNT_OFFSET_X", value ) ) p.RepaintWindowInUpdate( pluginID ); } }
			internal int CHILDCOUNT_OFFSET_Y { get { return s.GET( KEY + "_CHILDCOUNT_OFFSET_Y", pluginID == 0 ? 0 : 5 ); } set { var r = CHILDCOUNT_OFFSET_Y; if ( s.SET( KEY + "_CHILDCOUNT_OFFSET_Y", value ) ) p.RepaintWindowInUpdate( pluginID ); } }

			internal bool HIDE_HOVER_BG {
				get {
					return pluginID == 0 ? s.GET( KEY + "_HIDE_HOVER_BG", false ) : false;
				}
				set {
					var r = HIDE_HOVER_BG;
					if ( pluginID == 0 ) s.SET( KEY + "_HIDE_HOVER_BG", value );
				}
			}

		}



		///  #########################################################################################################################################################################################
		internal bool USE_UNDO_FOR_PLUGIN_MODULES {
			get { return GET( "USE_UNDO_FOR_PLUGIN_MODULES", true ); }
			set {
				var r = USE_UNDO_FOR_PLUGIN_MODULES;
				if ( SET( "USE_UNDO_FOR_PLUGIN_MODULES", value ) ) p.modsController.REBUILD_PLUGINS();
			}
		}

		internal bool USE_DINAMIC_BATCHING { get { return GET( "USE_DINAMIC_BATCHING", true ); } set { var r = USE_DINAMIC_BATCHING; SET( "USE_DINAMIC_BATCHING", value ); p.__STYLE_DEFBUTTON = null; p.__button = null; } }
		internal bool USE_SWAP_FOR_BUTTONS_ACTION { get { return GET( "USE_SWAP_FOR_BUTTONS_ACTION", false ); } set { var r = USE_SWAP_FOR_BUTTONS_ACTION; SET( "USE_SWAP_FOR_BUTTONS_ACTION", value ); p.__STYLE_DEFBUTTON = null; p.__button = null; } }
		internal bool USE_HOVER_FOR_BUTTONS { get { return GET( "USE_HOVER_FOR_BUTTONS", false ); } set { var r = USE_HOVER_FOR_BUTTONS; SET( "USE_HOVER_FOR_BUTTONS", value ); p.__STYLE_DEFBUTTON = null; p.__button = null; } }
		internal bool USE_WHOLE_FUN_UNITY_FONT_SIZE {
			get { return GET( "USE_WHOLE_FUN_UNITY_FONT_SIZE", false ); }
			set {
				var r = USE_WHOLE_FUN_UNITY_FONT_SIZE;
				if ( SET( "USE_WHOLE_FUN_UNITY_FONT_SIZE", value ) ) { p.modsController.REBUILD_PLUGINS(); }
				NewClearHelper.OnFontSizeChanged();
				if ( !value ) SHOULD_SCRIPT_RELOAD = true;
			}
		}
		internal bool SHOULD_SCRIPT_RELOAD = false;
		internal int WHOLE_FUN_UNITY_FONT_SIZE {
			get { return GET( "WHOLE_FUN_UNITY_FONT_SIZE", 12 ); }
			set {
				var r = WHOLE_FUN_UNITY_FONT_SIZE;
				if ( SET( "WHOLE_FUN_UNITY_FONT_SIZE", value ) )
				{
					SHOULD_SCRIPT_RELOAD = true;
					p.modsController.REBUILD_PLUGINS();
				}
				NewClearHelper.OnFontSizeChanged();
			}
		}
		internal bool RIGHTARROW_EXPANDS_HOVERED { get { return GET( "RIGHTARROW_EXPANDS_HOVERED", true ); } set { var r = RIGHTARROW_EXPANDS_HOVERED; SET( "RIGHTARROW_EXPANDS_HOVERED", value ); } }
		internal bool USE_EXPANSION_ANIMATION { get { return GET( "USE_EXPANSION_ANIMATION", false ); } set { var r = USE_EXPANSION_ANIMATION; SET( "USE_EXPANSION_ANIMATION", value ); } }
		internal bool ESCAPE_CLOSES_PREFABMODE { get { return GET( "ESCAPE_CLOSES_PREFABMODE", true ); } set { var r = ESCAPE_CLOSES_PREFABMODE; SET( "ESCAPE_CLOSES_PREFABMODE", value ); } }
		internal bool CLOSE_PREFAB_KEY_FORALL_WINDOWS { get { return GET( "CLOSE_PREFAB_KEY_FORALL_WINDOWS", false ); } set { var r = CLOSE_PREFAB_KEY_FORALL_WINDOWS; SET( "CLOSE_PREFAB_KEY_FORALL_WINDOWS", value ); } }
		internal bool CLOSE_PREFAB_KEY_FORHIER_ANDSCENE { get { return !CLOSE_PREFAB_KEY_FORALL_WINDOWS; } set { CLOSE_PREFAB_KEY_FORALL_WINDOWS = !value; } }
		//  internal bool DISABLE_DRAWING_ANIMATING_ITEMS { get { return GET( "DISABLE_DRAWING_ANIMATING_ITEMS", true ); } set {var r = qwe; SET( "DISABLE_DRAWING_ANIMATING_ITEMS", value ); } }
		internal bool DOUBLECLICK_TO_EXPAND { get { return GET( "DOUBLECLICK_TO_EXPAND", false ); } set { var r = DOUBLECLICK_TO_EXPAND; SET( "DOUBLECLICK_TO_EXPAND", value ); } }
		internal bool SELECTION_MOVETOGETHER_UPDOWNARROWS { get { return GET( "SELECTION_MOVETOGETHER_UPDOWNARROWS", true ); } set { var r = SELECTION_MOVETOGETHER_UPDOWNARROWS; SET( "SELECTION_MOVETOGETHER_UPDOWNARROWS", value ); } }
		///  #########################################################################################################################################################################################
		internal bool ONDOWN_ACTION_INSTEAD_ONUP { get { return GET( "ONDOWN_ACTION_INSTEAD_ONUP", false ); } set { var r = ONDOWN_ACTION_INSTEAD_ONUP; if ( SET( "ONDOWN_ACTION_INSTEAD_ONUP", value ) ) p.RepaintWindowInUpdate( 0 ); } }
		internal bool ENABLE_CUSTOMWINDOWS_OPENANIMATION { get { return GET( "ENABLE_CUSTOMWINDOWS_OPENANIMATION", true ); } set { var r = ENABLE_CUSTOMWINDOWS_OPENANIMATION; SET( "ENABLE_CUSTOMWINDOWS_OPENANIMATION", value ); } }
		internal bool ENABLE_OBJECTS_PING { get { return GET( "ENABLE_OBJECTS_PING", true ); } set { var r = ENABLE_OBJECTS_PING; SET( "ENABLE_OBJECTS_PING", value ); } }
		internal bool TRACKING_COMPILE_TIME { get { return GET( "TRACKING_COMPILE_TIME", false ); } set { var r = TRACKING_COMPILE_TIME; SET( "TRACKING_COMPILE_TIME", value ); } }
		///  #########################################################################################################################################################################################







		///  #########################################################################################################################################################################################




		// internal bool PLUGIN_FONT_AFFECT_HIERARCHYWINDOW { get { return GET( "PLUGIN_FONT_AFFECT_HIERARCHYWINDOW", false ); } set {var r = qwe; SET( "PLUGIN_FONT_AFFECT_HIERARCHYWINDOW", value ); } }
		///  #########################################################################################################################################################################################



		// class Saver
		// {
		//  int pluginID;
		//  internal Saver( int pId )
		// {
		//      pluginID = pId;
		//  }
		char[] trim = new[] { '\n', '\r' };
		internal void ClearCache()
		{
			HierarchyTempSceneData.RemoveCache();
			cache.Clear();
			Folders.Clear();
		}
		Dictionary<string, object> cache = new Dictionary<string, object>();
		string d { get { return Folders.CALC_SETTINGS_PATH_EXTERNAL + "/.EditorSettings/"; } }
		internal static string[] _check_settings_data_folder = {
			".EditorSettings",
			".Temp",
			".SavedLayouts"};

		string d_alternative( ref string sub ) { return Folders.CALC_SETTINGS_PATH_EXTERNAL + "/" + sub + "/"; }
		//string d_internal { get { return Folders.PluginExternalFolder + "/Editor/_SAVED_DATA/.EditorSettings/"; } }

		int DATA_SCENES_PATH_USE_DEFAULT { get { return Folders.DATA_SCENES_PATH_USE_DEFAULT; } set { Folders.DATA_SCENES_PATH_USE_DEFAULT = value; } }
		string __DATA_SCENES_PATH_IN { get { return Folders.__DATA_SCENES_PATH_IN; } set { Folders.__DATA_SCENES_PATH_IN = value; } }
		string __DATA_SCENES_PATH_LY { get { return Folders.__DATA_SETTINGS_PATH_LY; } set { Folders.__DATA_SETTINGS_PATH_LY = value; } }
		string __DATA_SCENES_PATH_EX { get { return Folders.__DATA_SCENES_PATH_EX; } set { Folders.__DATA_SCENES_PATH_EX = value; } }
		// string DATA_SCENES_PATH_TEMP { get { return Folders.DATA_SCENES_PATH_TEMP; } set { Folders.DATA_SCENES_PATH_TEMP = value; } }
		//string GET_SCENESDATA_PATH_INTERNAL { get { return Folders.GET_SCENESDATA_PATH_INTERNAL; } set { Folders.GET_SCENESDATA_PATH_INTERNAL = value; } }
		//string GET_SCENESDATA_PATH_EXTERNAL { get { return Folders.GET_SCENESDATA_PATH_EXTERNAL; } set { Folders.GET_SCENESDATA_PATH_EXTERNAL = value; } }
		//string GET_SCENESDATA_PATH_TOSTRING { get { return Folders.GET_SCENESDATA_PATH_TOSTRING; } set { Folders.GET_SCENESDATA_PATH_TOSTRING = value; } }
		//string GET_SCENESDATA_PATH_TOSTRING_CHANGABLE { get { return Folders.GET_SCENESDATA_PATH_TOSTRING_CHANGABLE; } set { Folders.GET_SCENESDATA_PATH_TOSTRING_CHANGABLE = value; } }
		//string GET_SCENESDATA_PATH_TOSTRING_NOTCHANGABLE { get { return Folders.GET_SCENESDATA_PATH_TOSTRING_NOTCHANGABLE; } set { Folders.GET_SCENESDATA_PATH_TOSTRING_NOTCHANGABLE = value; } }

		int DATA_SETTINGS_PATH_USE_DEFAULT { get { return Folders.DATA_SETTINGS_PATH_USE_DEFAULT; } set { Folders.DATA_SETTINGS_PATH_USE_DEFAULT = value; } }
		string __DATA_SETTINGS_PATH_IN { get { return Folders.__DATA_SETTINGS_PATH_IN; } set { Folders.__DATA_SETTINGS_PATH_IN = value; } }
		string __DATA_SETTINGS_PATH_LY { get { return Folders.__DATA_SETTINGS_PATH_LY; } set { Folders.__DATA_SETTINGS_PATH_LY = value; } }
		string __DATA_SETTINGS_PATH_EX { get { return Folders.__DATA_SETTINGS_PATH_EX; } set { Folders.__DATA_SETTINGS_PATH_EX = value; } }
		//string DATA_SETTINGS_PATH_TEMP { get { return Folders.DATA_SETTINGS_PATH_TEMP; } set { Folders.DATA_SETTINGS_PATH_TEMP = value; } }
		//string GET_SETTINGS_PATH_EXTERNAL { get { return Folders.GET_SETTINGS_PATH_EXTERNAL; } set { Folders.GET_SETTINGS_PATH_EXTERNAL = value; } }
		//string GET_SETTINGS_PATH_TOSTRING { get { return Folders.GET_SETTINGS_PATH_TOSTRING; } set { Folders.GET_SETTINGS_PATH_TOSTRING = value; } }
		//string GET_SETTINGS_PATH_TOSTRING_CHANGABLE { get { return Folders.GET_SETTINGS_PATH_TOSTRING_CHANGABLE; } set { Folders.GET_SETTINGS_PATH_TOSTRING_CHANGABLE = value; } }
		//string GET_SETTINGS_PATH_TOSTRING_NOTCHANGABLE { get { return Folders.GET_SETTINGS_PATH_TOSTRING_NOTCHANGABLE; } set { Folders.GET_SETTINGS_PATH_TOSTRING_NOTCHANGABLE = value; } }

		//static  string __DATA_SCENES_PATH_TEMP = "";
		//public static string DATA_SCENES_PATH_TEMP { get { return __DATA_SCENES_PATH_TEMP; } set { __DATA_SCENES_PATH_TEMP = value; } }
		//static  string __DATA_SETTINGS_PATH_TEMP = "";
		//public static string DATA_SETTINGS_PATH_TEMP { get { return __DATA_SETTINGS_PATH_TEMP; } set { __DATA_SETTINGS_PATH_TEMP = value; } }
		//static  int __DATA_SCENES_ID_TEMP = 0;
		//public static int DATA_SCENES_ID_TEMP { get { return __DATA_SCENES_ID_TEMP; } set { __DATA_SCENES_ID_TEMP = value; } }
		//static  int __DATA_SETTINGS_ID_TEMP = 0;
		//public static int DATA_SETTINGS_ID_TEMP { get { return __DATA_SETTINGS_ID_TEMP; } set { __DATA_SETTINGS_ID_TEMP = value; } }

		int __INT_TEMP = 0; public int INT_TEMP { get { return __INT_TEMP; } set { __INT_TEMP = value; } }
		string __STRING_TEMP = ""; public string STRING_TEMP { get { return __STRING_TEMP; } set { __STRING_TEMP = value; } }

		internal void REMOVE( string key, string f ) //, 
		{
			cache.Remove( key );
			//var f = d + pluginID + "-" + key;
			if ( File.Exists( f ) ) File.Delete( f );

		}

		internal string GET( string k, string def, string alt )
		{
			if ( cache.ContainsKey( k ) ) return (string)cache[ k ];
			if ( !_check_settings_data_folder.Contains( alt ) ) throw new Exception( "Alt exception" );
			var d = d_alternative(ref alt);
			var f = d + pluginID + "-" + k;
			string res = def;
			if ( File.Exists( f ) ) res = File.ReadAllText( f ).Trim( trim );
			cache.Add( k, res );
			return res;
		}

		internal string LAST_FILE, LAST_KEY;

		internal void SET( string k, string val, string alt )
		{
			var d = d_alternative(ref alt);
			var f = d + pluginID + "-" + k;
			LAST_FILE = f;
			LAST_KEY = k;

			if ( (string)cache[ k ] == val ) return;
			if ( !_check_settings_data_folder.Contains( alt ) ) throw new Exception( "Alt exception" );
			else cache[ k ] = val;

			if ( !Directory.Exists( d ) ) Directory.CreateDirectory( d );
			File.WriteAllText( f, val );

		}

		internal string GET( string k, string def )
		{
			if ( cache.ContainsKey( k ) ) return (string)cache[ k ];
			var f = d + pluginID + "-" + k;
			string res = def;
			//  if ( k.Contains( "SHADER_A" ) )
			//  { 
			//      Debug.Log( "ASD" ); 
			//  }
			if ( File.Exists( f ) ) res = File.ReadAllText( f ).Trim( trim );


			//if ( res != def )
			//{
			//    if ( !Directory.Exists( d ) ) Directory.CreateDirectory( d );
			//    File.WriteAllText( f, res.ToString() );
			//}
			cache.Add( k, res );
			return res;
		}

		internal void SET( string k, string val )
		{
			// if ( !cache.ContainsKey( k ) ) GET( k, def );
			var f = d + pluginID + "-" + k;
			LAST_FILE = f;
			LAST_KEY = k;

			if ( /*cache.ContainsKey( k ) && */(string)cache[ k ] == val ) return;

			//if ( k.Contains( "SHADER_A" ) )
			//{
			//    Debug.Log( "ASD" );
			//}
			//if ( !cache.ContainsKey( k ) ) cache.Add( k, val );
			else cache[ k ] = val;
			if ( !Directory.Exists( d ) ) Directory.CreateDirectory( d );
			File.WriteAllText( f, val );
		}
		internal int GET( string k, int def )
		{
			if ( cache.ContainsKey( k ) ) return (int)cache[ k ];
			var f = d + pluginID + "-" + k;
			int res = def;
			if ( File.Exists( f ) ) res = int.Parse( File.ReadAllText( f ).Trim( trim ) );
			//if ( res != def )
			//{
			//    if ( !Directory.Exists( d ) ) Directory.CreateDirectory( d );
			//    File.WriteAllText( f, res.ToString() );
			//}
			cache.Add( k, res );
			return res;
		}
		internal bool SET( string k, int val )
		{
			// if ( !cache.ContainsKey( k ) ) GET( k, def  );

			var f = d + pluginID + "-" + k;
			LAST_FILE = f;
			LAST_KEY = k;

			if ( /*cache.ContainsKey( k ) && */(int)cache[ k ] == val ) return false;
			// if ( !cache.ContainsKey( k ) ) cache.Add( k, val );
			else cache[ k ] = val;
			if ( !Directory.Exists( d ) ) Directory.CreateDirectory( d );
			File.WriteAllText( f, val.ToString() );

			return true;
		}
		internal bool GET( string k, bool def )
		{
			if ( cache.ContainsKey( k ) ) return (bool)cache[ k ];
			var f = d + pluginID + "-" + k;
			bool res = def;
			if ( File.Exists( f ) ) res = bool.Parse( File.ReadAllText( f ).Trim( trim ) );
			//if ( res != def )
			//{
			//    if ( !Directory.Exists( d ) ) Directory.CreateDirectory( d );
			//    File.WriteAllText( f, res.ToString() );
			//}
			cache.Add( k, res );
			return res;
		}

		internal bool SET( string k, bool val )
		{
			//if ( !cache.ContainsKey( k ) ) GET( k, def );

			var f = d + pluginID + "-" + k;
			LAST_FILE = f;
			LAST_KEY = k;

			if ( cache.ContainsKey( k ) && (bool)cache[ k ] == val ) return false;
			else cache[ k ] = val;
			if ( !Directory.Exists( d ) ) Directory.CreateDirectory( d );
			File.WriteAllText( f, val.ToString() );
			return true;
		}
		internal Color32 GET( string k, Color32 def )
		{
			if ( cache.ContainsKey( k ) ) return (Color32)cache[ k ];
			var f = d + pluginID + "-" + k;
			Color32 res = def;
			// if ( k.Contains( "COLOR_HIERARHCHY_CHESS_LINES_V2" ) )
			// { 
			//     Debug.Log( "ASD" ); 
			// }
			if ( File.Exists( f ) )
			{
				var b = File.ReadAllText(f).Trim(trim).Split(' ').Select(c => (byte)int.Parse(c)).ToArray();
				res = new Color32( b[ 0 ], b[ 1 ], b[ 2 ], b[ 3 ] );
			}

			//if ( ((Color)res) != def )
			//{
			//    if ( !Directory.Exists( d ) ) Directory.CreateDirectory( d );
			//    File.WriteAllText( f, res.r + " " + res.g + " " + res.b + " " + res.a );
			//}
			cache.Add( k, res );
			return res;
		}
		//bool CE( Color32 a, Color32 b ) { return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a; }
		internal bool SET( string k, Color32 val )
		{

			var f = d + pluginID + "-" + k;
			LAST_FILE = f;
			LAST_KEY = k;

			// if ( !cache.ContainsKey( k ) ) GET( k, def );
			//  if ( k.Contains( "COLOR_HIERARHCHY_CHESS_LINES_V2" ) )
			//  {
			//      Debug.Log( "ASD" ); 
			//  }
			if ( /*cache.ContainsKey( k ) &&*/  (Color)(Color32)cache[ k ] == val ) return false;
			// if ( !cache.ContainsKey( k ) ) cache.Add( k, val );
			else cache[ k ] = val;
			if ( !Directory.Exists( d ) ) Directory.CreateDirectory( d );
			File.WriteAllText( f, val.r + " " + val.g + " " + val.b + " " + val.a );
			return true;
		}
		internal bool SET( string k, float val )
		{

			// if ( !cache.ContainsKey( k ) ) GET( k, def );

			var f = d + pluginID + "-" + k;
			LAST_FILE = f;
			LAST_KEY = k;


			if ( /*cache.ContainsKey( k ) && */(float)cache[ k ] == val ) return false;
			//if ( !cache.ContainsKey( k ) ) cache.Add( k, val );
			else cache[ k ] = val;
			if ( !Directory.Exists( d ) ) Directory.CreateDirectory( d );
			File.WriteAllText( f, val.ToString() );
			return true;
		}
		internal float GET( string k, float def )
		{
			if ( cache.ContainsKey( k ) ) return (float)cache[ k ];
			var f = d + pluginID + "-" + k;
			float res = def;

			if ( File.Exists( f ) ) res = float.Parse( File.ReadAllText( f ).Trim( trim ).Replace( ',', '.' ), CultureInfo.InvariantCulture );
			//if ( res != def )
			//{
			//    if ( !Directory.Exists( d ) ) Directory.CreateDirectory( d );
			//    File.WriteAllText( f, res.ToString() );
			//}
			cache.Add( k, res );
			return res;
		}
		// }
	}
}
