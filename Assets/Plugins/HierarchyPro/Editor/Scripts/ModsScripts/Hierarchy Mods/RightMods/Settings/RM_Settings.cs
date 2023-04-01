using System;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor
{
	public enum VerticesModuleTypeEnum
	{
		Triangles= 0,
		Vertices = 1,
		ChildCount = 2,
		TextureMemory = 3
	}

	partial class EditorSettingsAdapter
	{

		//MAIN
		internal bool RIGHT_DRAW_HYPHEN_FOR_EMPTY_LABELS { get { return GET( "RIGHT_DRAW_HYPHEN_FOR_EMPTY_LABELS_FIX", true ); } set { var r = RIGHT_DRAW_HYPHEN_FOR_EMPTY_LABELS; SET( "RIGHT_DRAW_HYPHEN_FOR_EMPTY_LABELS_FIX", value ); p.RESET_DRAWSTACK( 0 ); } }
		internal bool RIGHT_SKIP_HYPHEN_FOR_DESCRIPTIONS { get { return GET( "RIGHT_SKIP_HYPHEN_FOR_DESCRIPTIONS", true ); } set { var r = RIGHT_SKIP_HYPHEN_FOR_DESCRIPTIONS; SET( "RIGHT_SKIP_HYPHEN_FOR_DESCRIPTIONS", value ); p.RESET_DRAWSTACK( 0 ); } }
		internal bool RIGHT_SKIP_HYPHEN_FOR_TAGS { get { return GET( "RIGHT_SKIP_HYPHEN_FOR_TAGS", false ); } set { var r = RIGHT_SKIP_HYPHEN_FOR_TAGS; SET( "RIGHT_SKIP_HYPHEN_FOR_TAGS", value ); p.RESET_DRAWSTACK( 0 ); } }

		//DB8DFF

		Color32 _pfc_head_def =new Color32( 180, 180, 180, 255 );
		Color32 _pfc_head_def2 =Color.black;

		
		Color32 _pfc_def =new Color32( 0xE4, 0xF9, 0xFF, 255 );
		//Color32 _pfc_def =new Color32( 219, 141, 255, 255 );
		//Color32 _pfc_def =new Color32( 154, 206, 255, 255 );
		// Color32 _pfc_def2 =new Color32( 51, 30, 61, 255 );
		//Color32 _pfc_def2 =new Color32( 0xA3, 0x2B, 0x2B, 255 );
		Color32 _pfc_def2 =new Color32( 0x16, 0x16, 0x16, 255 );

		
		//Color32 _pfc_def2 =new Color32( 30, 48, 61, 255 );
		internal Color RIGHT_LABELS_COLOR {
			get {
				if ( EditorGUIUtility.isProSkin ) return GET( "PLUGIN_LABEL_COLOR_PRO", _pfc_def );
				return GET( "PLUGIN_LABEL_COLOR_PRS", _pfc_def2 );
			}
			set {
				var r = RIGHT_LABELS_COLOR;
				if ( EditorGUIUtility.isProSkin ) SET( "PLUGIN_LABEL_COLOR_PRO", value );
				else SET( "PLUGIN_LABEL_COLOR_PRS", value );
				RightModsStyles.ClearLabels();
				p.RESET_DRAWSTACK( 0 );
			}
		}
		internal Color RIGHT_HEADER_COLOR {
			get {
				if ( EditorGUIUtility.isProSkin ) return GET( "RIGHT_HEADER_COLOR_PRO", _pfc_head_def );
				return GET( "RIGHT_HEADER_COLOR_PRS", _pfc_head_def2 );
			}
			set {
				var r = RIGHT_HEADER_COLOR;
				if ( EditorGUIUtility.isProSkin ) SET( "RIGHT_HEADER_COLOR_PRO", value );
				else SET( "RIGHT_HEADER_COLOR_PRS", value );
				RightModsStyles.ClearLabels();
				p.RESET_DRAWSTACK( 0 );
			}
		}

		internal bool RIGHT_HEADER_BIND_TO_SCENE_LINE { get { return GET( "RIGHT_HEADER_BIND_TO_SCENE_LINE", false ); } set { var r = RIGHT_HEADER_BIND_TO_SCENE_LINE; SET( "RIGHT_HEADER_BIND_TO_SCENE_LINE", value ); p.RESET_DRAWSTACK( 0 ); } }
		internal float RIGHT_HEADER_BG_OPACITY { get { return GET( "RIGHT_HEADER_BG_OPACITY", EditorGUIUtility.isProSkin ? 0.5f : 0.0f ); } set { var r = RIGHT_HEADER_BG_OPACITY; SET( "RIGHT_HEADER_BG_OPACITY", value ); p.RESET_DRAWSTACK( 0 ); } }
		internal float RIGHT_BG_OPACITY { get { return GET( "RIGHT_BG_OPACITY", EditorGUIUtility.isProSkin ? 0.0f : 0.2f ); } set { var r = RIGHT_BG_OPACITY; SET( "RIGHT_BG_OPACITY", value ); p.RESET_DRAWSTACK( 0 ); } }
		internal int RIGHT_RIGHT_PADDING { get { return GET( "RIGHT_RIGHT_PADDING", 0 ); } set { var r = RIGHT_RIGHT_PADDING; SET( "RIGHT_RIGHT_PADDING", value ); p.RESET_DRAWSTACK( 0 ); } } //  200, -100,

		internal bool RIGHT_RIGHT_PADDING_AFFECT_TO_SETACTIVE_AND_KEEPER { get { return GET( "RIGHT_RIGHT_PADDING_AFFECT_TO_SETACTIVE_AND_KEEPER", false ); } set { var r = RIGHT_RIGHT_PADDING_AFFECT_TO_SETACTIVE_AND_KEEPER; SET( "RIGHT_RIGHT_PADDING_AFFECT_TO_SETACTIVE_AND_KEEPER", value ); p.RESET_DRAWSTACK( 0 ); } }


		internal bool RIGHTDOCK_TEMPHIDE { get { return GET( "RIGHTDOCK_TEMPHIDE", false ); } set { var r = RIGHTDOCK_TEMPHIDE; SET( "RIGHTDOCK_TEMPHIDE", value ); p.RESET_DRAWSTACK( 0 ); } }
		internal int RIGHTDOCK_TEMPHIDEMINWIDTH { get { return GET( "RIGHTDOCK_TEMPHIDEMINWIDTH", 300 ); } set { var r = RIGHTDOCK_TEMPHIDEMINWIDTH; SET( "RIGHTDOCK_TEMPHIDEMINWIDTH", value ); p.RESET_DRAWSTACK( 0 ); } } //  200, -100,

		internal int RIGHT_PADDING_LEFT_READABLE { get { return Math.Max( 150, GET( "RIGHT_PADDING_LEFT_READABLE", 250 ) ); } set { var r = RIGHT_PADDING_LEFT_READABLE; SET( "RIGHT_PADDING_LEFT_READABLE", value ); p.RESET_DRAWSTACK( 0 ); } } //  200, -100,



		internal bool RIGHT_USE_CUSTOMMODULES { get { return GET( "RIGHT_USE_CUSTOMMODULES", true ); } set { var r = RIGHT_USE_CUSTOMMODULES; SET( "RIGHT_USE_CUSTOMMODULES", value ); p.modsController.REBUILD_PLUGINS(); p.RESET_DRAWSTACK( 0 ); } }



		internal int RIGHTDOCK_SHRINK_BUTTONS_INT { get { return GET( "RIGHTDOCK_SHRINK_BUTTONS_INT", 0 ); } set { var r = RIGHTDOCK_SHRINK_BUTTONS_INT; SET( "RIGHTDOCK_SHRINK_BUTTONS_INT", value ); p.RESET_DRAWSTACK( 0 ); } }
		//internal bool RIGHTDOCK_SHRINK_BUTTONS { get { return GET( "RIGHTDOCK_SHRINK_BUTTONS", false ); } set {var r = RIGHTDOCK_SHRINK_BUTTONS; SET( "RIGHTDOCK_SHRINK_BUTTONS", value ); p.RESET_DRAWSTACK( 0 ); } }
		internal bool RIGHTDOCK_CHACGE_CURSOR { get { return GET( "RIGHTDOCK_CHACGE_CURSOR", true ); } set { var r = RIGHTDOCK_CHACGE_CURSOR; SET( "RIGHTDOCK_CHACGE_CURSOR", value ); p.RESET_DRAWSTACK( 0 ); } }
		internal bool RIGHTDOCK_DRAW_VERTICAL_SEPARATORS {
			get {
				if ( RIGHTDOCK_SHRINK_BUTTONS_INT > 1 ) return false;
				return GET( "RIGHTDOCK_DRAW_VERTICAL_SEPARATORS", !EditorGUIUtility.isProSkin );
			}
			set { var r = RIGHTDOCK_DRAW_VERTICAL_SEPARATORS; SET( "RIGHTDOCK_DRAW_VERTICAL_SEPARATORS", value ); p.RESET_DRAWSTACK( 0 ); }
		}


		//TAGS
		internal int RIGHT_TAGS_UPPERCASE { get { return GET( "RIGHT_TAGS_UPPERCASE", 1 ); } set { var r = RIGHT_TAGS_UPPERCASE; SET( "RIGHT_TAGS_UPPERCASE", value ); p.RESET_DRAWSTACK( 0 ); } }
		internal int RIGHT_LAYERS_UPPERCASE { get { return GET( "RIGHT_LAYERS_UPPERCASE", 1 ); } set { var r = RIGHT_LAYERS_UPPERCASE; SET( "RIGHT_LAYERS_UPPERCASE", value ); p.RESET_DRAWSTACK( 0 ); } }
		internal int RIGHT_SPRITEORDER_UPPERCASE { get { return GET( "RIGHT_SPRITEORDER_UPPERCASE", 1 ); } set { var r = RIGHT_SPRITEORDER_UPPERCASE; SET( "RIGHT_SPRITEORDER_UPPERCASE", value ); p.RESET_DRAWSTACK( 0 ); } }



		internal bool RIGHT_FREEZE_LOCK_SCENE_VIEW { get { return GET( "RIGHT_FREEZE_LOCK_SCENE_VIEW", true ); } set { var r = RIGHT_FREEZE_LOCK_SCENE_VIEW; SET( "RIGHT_FREEZE_LOCK_SCENE_VIEW", value ); p.RESET_DRAWSTACK( 0 ); } }



		//MEMORY
		internal VerticesModuleTypeEnum RIGHT_MOD_VERTICES_SCAN_TYPE { get { return (VerticesModuleTypeEnum)GET( "RIGHT_MOD_VERTICES_SCAN_TYPE", 0 ); } set { var r = RIGHT_MOD_VERTICES_SCAN_TYPE; SET( "RIGHT_MOD_VERTICES_SCAN_TYPE", (int)value ); p.RESET_DRAWSTACK( 0 ); } }
		internal bool RIGHT_MOD_BROADCAST_ENABLED { get { return GET( "RIGHT_MOD_BROADCAST_ENABLED", false ); } set { var r = RIGHT_MOD_BROADCAST_ENABLED; SET( "RIGHT_MOD_BROADCAST_ENABLED", value ); p.RESET_DRAWSTACK( 0 ); } }
		internal float RIGHT_MOD_BROADCASTING_PREFOMANCE { get { return GET( "RIGHT_MOD_BROADCASTING_PREFOMANCE", 30f ); } set { var r = RIGHT_MOD_BROADCASTING_PREFOMANCE; SET( "RIGHT_MOD_BROADCASTING_PREFOMANCE", value ); p.RESET_DRAWSTACK( 0 ); } }
		internal int RIGHT_MOD_BROADCASTING_PREFOMANCE01 {
			get { return Mathf.RoundToInt( (RIGHT_MOD_BROADCASTING_PREFOMANCE - 10f) / 2f ); }

			set { RIGHT_MOD_BROADCASTING_PREFOMANCE = Mathf.RoundToInt( value * 2 + 10 ); p.RESET_DRAWSTACK( 0 ); }
		}




		//HIDE
		internal bool RIGHT_DRAW_MODS_FOR_SELECTED_ONLY { get { return GET( "RIGHT_DRAW_MODS_FOR_SELECTED_ONLY", false ); } set { var r = RIGHT_DRAW_MODS_FOR_SELECTED_ONLY; SET( "RIGHT_DRAW_MODS_FOR_SELECTED_ONLY", value ); p.RESET_DRAWSTACK( 0 ); } }
		internal bool RIGHT_DRAW_MODS_FOR_HOVER_ALSO { get { return GET( "RIGHT_DRAW_MODS_FOR_HOVER_ALSO", true ); } set { var r = RIGHT_DRAW_MODS_FOR_HOVER_ALSO; SET( "RIGHT_DRAW_MODS_FOR_HOVER_ALSO", value ); p.RESET_DRAWSTACK( 0 ); } }
		internal bool RIGHT_LOCK_MODS_UNTIL_NOKEY { get { return (RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & 3) != 0; } }
		internal int RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX { get { return GET( "RIGHT_HIDEMODS_UNTIL_NOKEY_INDEX", 0 ); } set { var r = RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX; SET( "RIGHT_HIDEMODS_UNTIL_NOKEY_INDEX", value ); p.RESET_DRAWSTACK( 0 ); } }
		internal string RIGHT_LOCK_MODS_UNTIL_NOKEY_TOSTRING {
			get {
				var new_i = RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & 3;
				var cts = new[] {"...", "'Alt'", "'Shift'", "'Ctrl'"};
				var key = cts[new_i];
				return key;
			}
		}
		internal bool RIGHT_LOCK_ONLY_IF_NOCONTENT {
			get { return (RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & 8) != 0; }
			set {
				if ( value ) RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX = RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX | 8;
				else RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX = RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & ~8;
				p.RESET_DRAWSTACK( 0 );
			}
		}
		internal bool RIGHT_USE_HIDE_ISTEAD_LOCK { get { return GET( "RIGHT_USE_HIDE_ISTEAD_LOCK", true ); } set { var r = RIGHT_USE_HIDE_ISTEAD_LOCK; SET( "RIGHT_USE_HIDE_ISTEAD_LOCK", value ); p.RESET_DRAWSTACK( 0 ); } }

	}
}
