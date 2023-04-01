using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class RM_Window : ScriptableObject
	{


	}

	[CustomEditor( typeof( RM_Window ) )]
	class SETGUI_RightBar : MainRoot
	{
		internal static string set_text =  USE_STR + "Right Bar Mods Pack [9 buildin/9 custom]";
		internal static string set_key = "USE_RIGHT_ALL_MODS";

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
			Draw.RESET( w );

			//   GUI.Button( Draw.R2, "xxx", Draw.s( "preToolbar" ) );
			// GUI.Button( Draw.R, "Common Settings", Draw.s( "insertionMarker" ) );
			Draw.BACK_BUTTON( w );
			Draw.TOG_TIT( set_text, set_key ,WIKI: WIKI_4_RIGHTBAR);
			Draw.Sp( 10 );

			using ( ENABLE( w ).USE( set_key ) )
			{

				using ( GRO( w ).UP( 0 ) )
				{
					///     Draw.FIELD( "Header bg opacity", "RIGHT_BG_OPACITY", 0, 1 );


					Draw.TOG_TIT( "Content:", EnableRed: false );

					GUI.Label( Draw.R, "Adjust width of buttons to content size:" );
					Draw.TOOLBAR( new[] { "Default", "Shrink clicking area", "Compact style (beta)" }, "RIGHTDOCK_SHRINK_BUTTONS_INT" );
					Draw.DRAW_NEW( Draw.last );

					var en = GUI.enabled;
					GUI.enabled &= p.par_e.RIGHTDOCK_SHRINK_BUTTONS_INT != 2;
					Draw.TOG( "Draw vertical separation lines", "RIGHTDOCK_DRAW_VERTICAL_SEPARATORS" );
					GUI.enabled = en;
					Draw.FIELD( "Content background opacity", "RIGHT_BG_OPACITY", 0, 1 );

					Draw.Sp( 8 );


					Draw.TOG( "Change cursor if mouse hovering over the content", "RIGHTDOCK_CHACGE_CURSOR" );

					Draw.Sp( 8 );


					Draw.FIELD( "Content text font size", "RIGHTMOD_LABELS_FONT_SIZE", 4, 60, overrideObject: p.par_e.HIER_WIN_SET );//
					Draw.COLOR( "Content text color", "RIGHT_LABELS_COLOR" );

					Draw.HRx05( Draw.R );


					//	Draw.HRx1();






					Draw.TOG( "Draw '-' for empty labels", "RIGHT_DRAW_HYPHEN_FOR_EMPTY_LABELS" );
					using ( ENABLE( w ).USE( "RIGHT_DRAW_HYPHEN_FOR_EMPTY_LABELS" ) )
					{
						Draw.TOG( "Skip '-' for description mod", "RIGHT_SKIP_HYPHEN_FOR_DESCRIPTIONS" );
						Draw.TOG( "Skip '-' for tags mod", "RIGHT_SKIP_HYPHEN_FOR_TAGS" );
					}


					Draw.Sp( 8 );





					Draw.TOG_TIT( "Header:", EnableRed: false );
					Draw.FIELD( "Header text font size", "RIGHTMOD_HEADER_FONT_SIZE", 4, 60, overrideObject: p.par_e.HIER_WIN_SET );//
					Draw.COLOR( "Header text color", "RIGHT_HEADER_COLOR" );
					Draw.FIELD( "Header background opacity", "RIGHT_HEADER_BG_OPACITY", 0, 1 );
					Draw.Sp( 10 );
					//Draw.HRx05( Draw.R );
					Draw.TOG( "Bind header to the scene line", "RIGHT_HEADER_BIND_TO_SCENE_LINE" );
					Draw.Sp( 10 );

					//Draw.HRx05( Draw.R );
					//	using ( GRO( w ).UP( 0 ) )
					{
						Draw.TOG( "Draw external mods hotbuttons on hierarchy header", "DRAW_HEADER_HOTBUTTONS", rov: Draw.R );
						using ( ENABLE( w ).USE( "DRAW_HEADER_HOTBUTTONS" ) )
						{
							Draw.FIELD( "External mods buttons size", "HEADER_HOTBUTTON_SEZE", 3, 60, "px" );
							//using ( GRO( w ).UP( 30 ) )
							if ( p.par_e.DRAW_HEADER_HOTBUTTONS ) p.par_e.DrawHotButtonsArray();
						}
					}

					Draw.Sp( 8 );

					Draw.TOG_TIT( "Right Padding:", EnableRed: false );

					Draw.FIELD( "Right Padding", "RIGHT_RIGHT_PADDING", -100, 200, "px" );
					Draw.TOG( "Right padding affect setactive and playmodekeeper mods", "RIGHT_RIGHT_PADDING_AFFECT_TO_SETACTIVE_AND_KEEPER" );

					Draw.TOG( "Auto hiding modules if window width less than...", "RIGHTDOCK_TEMPHIDE" );
					using ( ENABLE( w ).USE( "RIGHTDOCK_TEMPHIDE" ) ) Draw.FIELD( "If width <", "RIGHTDOCK_TEMPHIDEMINWIDTH", 100, 500, "px" );




					//}


					Draw.Sp( 10 );
					//using ( GRO( w ).UP( 0 ) )
					//{
					// GUI.Label( Draw.R, Draw.CONT( "Hiding mods" ) ); Draw.Sp( 2 ); //, Draw.s( "preBackground" )
					Draw.TOG_TIT( "Hiding mods:" );


					RightAutoHider();
					// Draw.TOG( "Lock modules if no special key is pressed", "RIGHT_SPRITEORDER_UPPERCASE" ); //RIGHT_HIDEMODS_UNTIL_NOKEY_INDEX
					using ( ENABLE( w ).USE( "RIGHT_LOCK_MODS_UNTIL_NOKEY" , 0) )
					{
						if ( p.par_e.RIGHT_LOCK_ONLY_IF_NOCONTENT && p.par_e.RIGHT_USE_HIDE_ISTEAD_LOCK )
						{
							p.par_e.RIGHT_USE_HIDE_ISTEAD_LOCK = p.par_e.RIGHT_LOCK_ONLY_IF_NOCONTENT = false;
						}
						using ( ENABLE( w ).USE( "RIGHT_USE_HIDE_ISTEAD_LOCK", 0, true ) )
						{
							Draw.TOG( "Lock only with empty content", "RIGHT_LOCK_ONLY_IF_NOCONTENT" );
							if ( !p.par_e.RIGHT_USE_HIDE_ISTEAD_LOCK && p.par_e.RIGHT_LOCK_ONLY_IF_NOCONTENT )
							{
								Draw.HELP( w, LOCK_CONTENT[ p.par_e.RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & 3 ] + " key selected to access locked modules without content" );
							}
						}
						using ( ENABLE( w ).USE( "RIGHT_LOCK_ONLY_IF_NOCONTENT", 0, true ) )
						{
							Draw.TOG( "Hide and lock instead of just locking", "RIGHT_USE_HIDE_ISTEAD_LOCK" );
							if ( p.par_e.RIGHT_USE_HIDE_ISTEAD_LOCK && !p.par_e.RIGHT_LOCK_ONLY_IF_NOCONTENT )
							{
								Draw.HELP( w, LOCK_CONTENT[ p.par_e.RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & 3 ] + " key selected to display modules" );
							}
						}


					}


					using ( ENABLE( w ).USE( "RIGHT_USE_HIDE_ISTEAD_LOCK", 0, true ) )
					{
						Draw.TOG( "Hide and display only if objects are selected", "RIGHT_DRAW_MODS_FOR_SELECTED_ONLY" );
						if ( p.par_e.RIGHT_DRAW_MODS_FOR_SELECTED_ONLY && !p.par_e.RIGHT_USE_HIDE_ISTEAD_LOCK )
						{
							Draw.HELP( w, "You can press 'alt' to display all modules" );
							Draw.TOG( "Display if mouse hovers too", "RIGHT_DRAW_MODS_FOR_HOVER_ALSO" );
						}

					}

					Draw.Sp( 8 );

					// Draw.TOOLBAR( new[] { "TopBar", "Scene Name", "Bottom", "Disable" }, "HOTBUTTONS_BAR_PLACE" );
				}


				Draw.Sp( 10 );
				//MAIN
				using ( GRO( w ).UP( 0 ) )
				{
					// GUI.Label( Draw.R, Draw.CONT( "Tags/Layers/SpritesLayers mods" ) ); Draw.Sp( 2 ); //, Draw.s( "preBackground" )
					Draw.TOG_TIT( "Components Icons:" , WIKI: WIKI_4_ICONS);
					//Draw.TOG( "Clipping icons:" );
					//GUI.Label( Draw.R, "Clipping Graphic & Events for Icons:" );
					//Draw.TOG_TIT(  "Clipping Graphic & Events for Icons:", EnableRed:false );
					SETGUI_Icons.DRAW_CLAMP( w );

					Draw.Sp( 5 );


				}



				Draw.Sp( 10 );
				//MAIN
				using ( GRO( w ).UP( 0 ) )
				{
					// GUI.Label( Draw.R, Draw.CONT( "Tags/Layers/SpritesLayers mods" ) ); Draw.Sp( 2 ); //, Draw.s( "preBackground" )
					Draw.TOG_TIT( "Tags/Layers/SpritesLayers mods:", WIKI: WIKI_4_RM_LAY );
					Draw.TOG( "Tags displays only uppercase chars", "RIGHT_TAGS_UPPERCASE" );
					Draw.TOG( "Layers displays only uppercase chars", "RIGHT_LAYERS_UPPERCASE" );
					Draw.TOG( "SpritesLayers displays only uppercase chars", "RIGHT_SPRITEORDER_UPPERCASE" );
					// Draw.TOOLBAR( new[] { "TopBar", "Scene Name", "Bottom", "Disable" }, "HOTBUTTONS_BAR_PLACE" );
				}
				Draw.Sp( 10 );
				//MAIN
				using ( GRO( w ).UP( 0 ) )
				{
					Draw.TOG_TIT( "Freeze mod:", WIKI: WIKI_4_RM_LOC );
					// GUI.Label( Draw.R, Draw.CONT( "Freeze mod" ) ); Draw.Sp( 2 ); //, Draw.s( "preBackground" )
					Draw.TOG( "Lock selection in scene view too", "RIGHT_FREEZE_LOCK_SCENE_VIEW" );
					// Draw.TOOLBAR( new[] { "TopBar", "Scene Name", "Bottom", "Disable" }, "HOTBUTTONS_BAR_PLACE" );
				}


				Draw.Sp( 10 );

				using ( GRO( w ).UP( 0 ) )
				{
					Draw.TOG_TIT( "Textures Memory mod:", WIKI: WIKI_4_RM_MEM );
					// GUI.Label( Draw.R, Draw.CONT( "Textures Memory mod" ) ); Draw.Sp( 2 ); //, Draw.s( "preBackground" )
					Draw.TOG( "Enable broadcast scan", "RIGHT_MOD_BROADCAST_ENABLED" );
					Draw.HELP( w, "This means that the parent objects will display the total value of all children. It also counts the count of same textures and models, which will help to identify objects that are inefficient using unique models or textures with a large amount of memory" );
					Draw.HELP( w, "You can enable/disable broadcast using LeftClick in hierarchy window" );
					Draw.FIELD( "Broadcasting Performance", "RIGHT_MOD_BROADCASTING_PREFOMANCE01", 5, 95, "%" );
					Draw.HELP( w, "(High values may reduce performance)" );
				}



				Draw.Sp( 10 );

				using ( GRO( w ).UP( 0 ) )
				{
					Draw.TOG_TIT( "Use custom modules:", "RIGHT_USE_CUSTOMMODULES", WIKI: WIKI_4_RM_CUSTOM );
					using ( ENABLE( w ).USE( "RIGHT_USE_CUSTOMMODULES", 0 ) )
					{
						Draw.HELP( w, "You can add your custom mods using 'EMX." + Root.CUST_NS + ".ExtensionInterface_CustomRightMod.Slot_1'.", drawTog: true );
						if ( Draw.BUT( "Select Script with Custom Examples" ) ) { Selection.objects = new[] { Root.icons.example_folders[ 1 ] }; }
					}
				}
				Draw.Sp( 10 );
				using ( GRO( w ).UP( 0 ) )
				{
					Draw.TOG_TIT( "Search Window:", WIKI: WIKI_4_SEARCH );
					Draw.FIELD( "Input window width factor", "ADDITIONA_INPUT_WINDOWS_WIDTH", 0.5f, 5, "x" );
					Draw.FIELD( "Search window width factor", "ADDITIONA_SEARCH_WINDOWS_WIDTH", 0.5f, 5, "x" );
					Draw.TOG( "Snap search to left when opening", "BIND_SEARCH_TO_LEFT" );
					Draw.TOG( "Include disabled objects", "SEARCH_SHOW_DISABLED_OBJECT" );
					Draw.TOG( "Search only inside the current root", "SEARCH_USE_ROOT_ONLY" );
					Draw.HELP( w, "Objects located outside the root of selected object will not be included" );
					Draw.HELP( w, "You can also select an object then hold control and then click an item to search, in this case, the search will inside the selected object only" );
				}



			}



			Draw.Sp( 10 );
			using ( GRO( w ).UP( 0 ) )
			{
				Draw.TOG_TIT( "Quick tips:" );

				// GUI.Label( Draw.R, "Quick tips:" );
				Draw.HELP_TEXTURE( w, "HELP_RIGHTMOD" );
				Draw.HELP( w, "You can change width of columns or ordering.", drawTog: true );
				Draw.HELP( w, "LMB to open menu.", drawTog: true );

				Draw.HELP_TEXTURE( w, "HELP_SEARCH" );
				Draw.HELP( w, "RMB to search.", drawTog: true );
				Draw.HELP( w, "RMB on the header will search all objects with any not empty content for that mod.", drawTog: true );
				Draw.HELP( w, "RMB on the content will search all objects with the same content.", drawTog: true );
				//Draw.HELP( w, "If you select a one root object and then use ctrl+right-click on any child content, the search window will only scan the child content of the selected object.", drawTog: true );


				Draw.HRx2();
				Draw.HELP( w, "You can add your own mod 'EMX." + Root.CUST_NS + ".ExtensionInterface_CustomRightMod.Slot_1'.", drawTog: true );
				//Draw.HELP(w,"You can add your own items using 'ExtensionInterface_RightClickOnGameObjectMenuItem'.", drawTog: true);
				if ( Draw.BUT( "Select Script with Custom Examples" ) )
				{
					Selection.objects = new[] { Root.icons.example_folders[ 1 ]
	};
				}
			}



		}
		static  string[] LOCK_CONTENT = { "Disabled", "Alt", "Shift", "Ctrl" };
		static void RightAutoHider()
		{
			Rect _R = Draw.R;
			// _R = EditorGUILayout.GetControlRect();
			// _R.width -= 80;
			GUI.Label( _R, "Lock modules if no special key is pressed:" );
			//  R.x += R.width;
			//  R.width = 80;
			_R = Draw.R;
			var old_i = p.par_e.RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & 3;
			var new_i = GUI.Toolbar(_R, old_i, LOCK_CONTENT, EditorStyles.miniButton);
			if ( new_i != old_i )
			{
				p.par_e.RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX = (p.par_e.RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & ~3) | new_i;
			}

			//    TOOLTIP( R2 , "If a module already has a content, you shouldn't use a key to change them." );

			//             var   lineRect = EditorGUILayout.GetControlRect( );
			//             var new_S_HideRightIfNoFunction = TOOGLE_POP( ref lineRect , "Hide <b>Right Bar</b> if " + key + " not pressed" , _S_HideRightIfNoFunction ? 1 : 0 , "Show Always" , "Hide" ) == 1;
			//             GUILayout.Space( EditorGUIUtility.singleLineHeight );
			//             lineRect = EditorGUILayout.GetControlRect();
			//             var new_S_HideBttomIfNoFunction = TOOGLE_POP( ref lineRect , "Hide <b>Bottom Bar</b> if " + key + " not pressed" , _S_HideBttomIfNoFunction ? 1 : 0 , "Show Always" , "Hide" ) == 1;
			//             GUILayout.Space( EditorGUIUtility.singleLineHeight );
			//             GUI.enabled = on;
			//
			//             if ( new_A != par.USE_BUTTON_TO_INTERACT_AHC || _S_HideRightIfNoFunction != new_S_HideRightIfNoFunction || _S_HideBttomIfNoFunction != new_S_HideBttomIfNoFunction ) {
			//                 par.USE_BUTTON_TO_INTERACT_AHC = new_A;
			//                 _S_HideRightIfNoFunction = new_S_HideRightIfNoFunction;
			//                 _S_HideBttomIfNoFunction = new_S_HideBttomIfNoFunction;
			//                 DRAW_STACK.ValueChanged();
			//             }
		}
	}
}
