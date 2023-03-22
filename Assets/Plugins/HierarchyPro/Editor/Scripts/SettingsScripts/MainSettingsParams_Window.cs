using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class MainSettingsParams_Window : ScriptableObject
	{

	}



	[CustomEditor( typeof( MainSettingsParams_Window ) )]
	class SETGUI_MainSettings : MainRoot
	{


		internal static void DRAW_SWAP_BUTTONS_GUI( IRepaint w )
		{
			Draw.TOG( "Use OnMosueDown instead of OnMouseUp for right bar modules (beta)", "ONDOWN_ACTION_INSTEAD_ONUP" );
			Draw.TOG( "Swap LMB/RMB for right bar (beta)", "USE_SWAP_FOR_BUTTONS_ACTION" );
			//using ( ENABLE(w).USE( "USE_SWAP_FOR_BUTTONS_ACTION", 0 ) ) 
			Draw.HELP( w, "- default: LMB - do action or open menu, RMB - search\n- swapped: LMB - search, RMB - action." );
		}



		public override void OnInspectorGUI()
		{
			_GUI( (IRepaint)this );
		}
		public static void _GUI( IRepaint w )
		{

			Draw.RESET( w );

			Draw.BACK_BUTTON( w );

			// GUI.Button( Draw.R2, "  Main Hierarchy Settings", Draw.s( "preToolbar" ) );
			Draw.TOG_TIT( "Main Hierarchy Settings" , WIKI: WIKI_1_STYLE);

			// GUI.Button( Draw.R, "Common Settings", Draw.s( "insertionMarker" ) );
			using ( GRO( w ).UP() )
			{



				QWE( w, p.par_e.HIER_WIN_SET, () => {
					//   using ( GRO(w).UP( 0 ) )
					{

						Draw.TOG( "Funny fonts overwriting for editor styles (test)", "USE_WHOLE_FUN_UNITY_FONT_SIZE", ( valBefore ) => {
							if ( !valBefore.AS_BOOL && EditorUtility.DisplayDialog( "" + Root.HierarchyPro + "", "Do you want to overwrite all editor fonts size? After when you disable it, you, probably, have to restart the editor", "Yes, I wanna try", "No, please stop!" ) ) return true;
							if ( valBefore.AS_BOOL && EditorUtility.DisplayDialog( "" + Root.HierarchyPro + "", "Do you want to disable fun custom editor fonts size? You would also need to restart the editor to fully restore fonts sizes", "Yes", "No" ) ) return true;
							return false;
						},
					( valAfter ) => {
						p.modsController.REBUILD_PLUGINS();
						if ( !valAfter.AS_BOOL ) FunEditorFontsModification.Modificate( 12, true );
					}, EnableRed: false );
						using ( ENABLE( w ).USE( Draw.GetSetter( "USE_WHOLE_FUN_UNITY_FONT_SIZE" ) ) ) Draw.FIELD( "Fun unity font size '12'", "WHOLE_FUN_UNITY_FONT_SIZE", 4, 60 );
						if ( p.par_e.USE_WHOLE_FUN_UNITY_FONT_SIZE && p.par_e.SHOULD_SCRIPT_RELOAD )
						{
							var r=  EditorGUILayout.GetControlRect(GUILayout.Height(32));
							r.x += r.width / 3 * 2;
							r.width /= 3;
							r.x -= 40;
							var c= GUI.color;
							GUI.color *= Color.red;
							if ( GUI.Button( r, "Reload Scripts Requests" ) ) Root.RequestScriptReload();
							GUI.color = c;
						}
					}
					Draw.HRx05( Draw.R );
					Draw.COLOR( "Buttons tap color", /*-*/"BUTTON_TAP_COLOR" );

					Draw.HRx05( Draw.R );
					//	Draw.Sp( 10 );
					{
						GUI.Label( Draw.R, Draw.CONT( "Quick settings presets (Copy settings above to os text buffer):" ) );
						var R = Draw.R15;
						R.width /= 2;
						if ( GUI.Button( R, Draw.CONT( "Copy", @"Copy settings above (and background colors) to os buffer, settings like:
- Line Height/Indent
- Labels/Icons Size
- Background Chess Colors" ) ) )
						{
							Root.p[ 0 ].par_e.CopyPresetToBuffer();
						}
						R.x += R.width;
						var e = GUI.enabled;
						GUI.enabled &= Root.p[ 0 ].par_e.PastePresetToBufferValidate();
						if ( GUI.Button( R, Draw.CONT( "Paste", @"Paste settings above (and background colors) from os buffer, settings like:
- Line Height/Indent
- Labels/Icons Size
- Background Chess Colors" ) ) )
						{
							Root.p[ 0 ].par_e.PastePresetToBuffer();
						}
						GUI.enabled = e;
						Draw.Sp( 10 );
					}

					// Draw.HRx2();









				}, () => {


					Draw.Sp( 10 );
					using ( MainRoot.GRO( w ).UP( 0 ) )
					{

						Draw.TOG_TIT( "Events:", EnableRed: true , WIKI: WIKI_1_EVENTS);


						//Draw.Sp( 10 );

						Draw.TOG_TIT( "Arrow Keys UP/DOWN/LEFT/RIGHT:", EnableRed: false );//GUI.Label( Draw.R, Draw.CONT( "Arrow Keys UP/DOWN/LEFT/RIGHT:" ) );




						Draw.TOG( "UP/DOWN - Move multiline selection and keep distance between selected elements", "SELECTION_MOVETOGETHER_UPDOWNARROWS" );
						Draw.HRx05( Draw.R );

						using ( MainRoot.ENABLE( w ).USE( "HIDE_HOVER_BG", true, overrideObject: p.par_e.HIER_WIN_SET, padding: 0 ) )
						{
							Draw.TOG( "LEFT/RIGHT - Expand hovered item (even if item wasn't select)", "RIGHTARROW_EXPANDS_HOVERED" );
							Draw.HELP( w, "Use these keys when the mouse hovers over the object line to expand hovered tree item" );
						}
						using ( MainRoot.ENABLE( w ).USE( "ENABLE_ALL" ) )
							Draw.TOG( "Disable internal hover rect", "HIDE_HOVER_BG", overrideObject: p.par_e.HIER_WIN_SET );

						Draw.HRx05( Draw.R );

						Draw.TOG( "DOUBLE CLICK - Expands item (instead of framing in scene view)", "DOUBLECLICK_TO_EXPAND", EnableRed: false );
						Draw.HELP( w, "In this case, if you need move the camera to object (by default double-click), you can use right click on the SetActive button" );

						Draw.HRx05( Draw.R );

						Draw.TOG( "ESCAPE - Closes edit prefab mode", "ESCAPE_CLOSES_PREFABMODE", EnableRed: false );
						using ( ENABLE( w ).USE( Draw.GetSetter( "ESCAPE_CLOSES_PREFABMODE" ) ) ) Draw.TOG( "Close only if Hierarchy or SceneView are focus", "CLOSE_PREFAB_KEY_FORHIER_ANDSCENE" );

						Draw.HRx05( Draw.R );

						DRAW_SWAP_BUTTONS_GUI( w );
						//  Draw.TOG( "Use horizontal scroll", "USE_HORISONTAL_SCROLL" );
						//    using ( GRO(w).UP( 0 ) )


						Draw.Sp( 10 );
					}

					//Draw.HRx1();



				}, () => {



					Draw.Sp( 10 );
					using ( MainRoot.GRO( w ).UP( 0 ) )
					{

						Draw.TOG_TIT( "Additional:", EnableRed: true );
						//FINAL

						Draw.TOG( "Use opening animation for asset's windows", "ENABLE_CUSTOMWINDOWS_OPENANIMATION" );
						Draw.TOG( "Ping changed objects", "ENABLE_OBJECTS_PING" );
						Draw.TOG( "Track and log compile time", "TRACKING_COMPILE_TIME" );
						Draw.Sp( 10 );
					}



					Draw.Sp( 10 );
					using ( GRO( w ).UP( 0 ) )
					{
						Draw.TOG_TIT( "Other Internal Plugin Settings:", EnableRed: true , WIKI: WIKI_1_OTHER);

						GUI.Button( Draw.R2, "Reset settings to default:", Draw.s( "preToolbar" ) );
						Draw.HELP( w, "To reset only one settings param - use RMB on the line" );
						Draw.DRAW_NEW( Draw.last );
						Draw.HELP( w, "If you want to reset all settings, remove '.EditorSettings' folder (see cache settings to find it)" );
						Draw.Sp( 8 );


						//using ( ENABLE( w ).USE( "ENABLE_ALL", 0 ) )
						{
							//if ( use_color ) GUI.color = C * Color.Lerp( new Color32( 255, 150, 150, 255 ), Color.white, 0 );

							//GUI.color = C;


							//var s1 = "It helps to improve asset, but, in case if exception will continue to throw, every time it will throw it to console";
							var s1 = "It helps to improve the asset, but, in case if it continues to throw exceptions, it will appear in the console every time.";

							var s2 = "If the asset has any loop issues, the asset will temporarily stop, and it will log a one simple message to the console, one time only, with a full information of the bug (it is also informative as the red mode)";
							//var s2 = "If plugin has any loop issues, asset will temporary stop, and it log once a simple message in the console with a full information about bug (it also informative method as a red mode)";
							GUI.Button( Draw.R2, "Logs settings:", Draw.s( "preToolbar" ) );
							//using ( GRO( w ).UP( 15 ) )
							{

								var C2 = GUI.color;
								if ( p.par_e.LOG_SETTINGS_RED_MODE ) GUI.color = Color.red * 2;
								Draw.TOG( new GUIContent( "Enable full exceptions [red mode]", s1 ), "LOG_SETTINGS_RED_MODE" );
								GUI.color = C2;

								using ( ENABLE( w ).USE( "LOG_SETTINGS_RED_MODE", 0, inverce: true ) )
								{
									Draw.TOG( new GUIContent( "Enable simple log",
										s2 ),
									"LOG_SETTINGS_WHITE_MODE" );
									Draw.DRAW_NEW( Draw.last );
								}
							}
							if ( !p.par_e.LOG_SETTINGS_RED_MODE && !p.par_e.LOG_SETTINGS_WHITE_MODE )
							{
								GUI.Label( Draw.R, "   [Log disabled]" );
								Draw.HELP( w, "(No any logs and exceptions) If plugin has any repeated issues, asset will temporary disabled, and reloaded again when it will possible" );
							}
							if ( p.par_e.LOG_SETTINGS_RED_MODE ) Draw.HELP( w, s1 );
							if ( p.par_e.LOG_SETTINGS_WHITE_MODE && !p.par_e.LOG_SETTINGS_RED_MODE ) Draw.HELP( w, s2 );


							Draw.Sp( 10 );
							GUI.Button( Draw.R2, "Undo settings:", Draw.s( "preToolbar" ) );
							//using ( GRO( w ).UP( 15 ) )
							{
								Draw.TOG( new GUIContent( "Use undo for plugin modules", "A new undo implementation is available now, but if you have any issues with it or performance problems, you can disable it" ), "USE_UNDO_FOR_PLUGIN_MODULES" );
								Draw.HELP( w, "This option enables undo recording for mods like: descriptions, highlighter, playmode preserver, bookmarks, icons, presets manager, and etc" ); //This is a new feature, disable it if you have any issues
							}

							Draw.Sp( 10 );
							GUI.Button( Draw.R2, "Settings window type:", Draw.s( "preToolbar" ) );
							//using ( GRO( w ).UP( 15 ) )
							{
								GUI.Label( Draw.R, "What kind of window will use for displaying settings:" );
								var c = GUI.color;
								if ( p.par_e.OPEN_SETTINGS_WINDOW != 0 ) GUI.color *= new Color32( 255, 80, 40, 255 );
								Draw.TOOLBAR( new[] { "New Settings Window", "New Inspector", "Inspector" }, "OPEN_SETTINGS_WINDOW" );
								//Draw.TOG( "Open settings categories in current inspector", "OPEN_SETTINGS_IN_INSPECTOR" );
								Draw.Sp( 5 );
								GUI.color = c;

								using ( ENABLE( w ).USE( "OPEN_SETTINGS_WINDOW", 0 ) )
								{
									if ( GUI.Button( Draw.R, "Create Settings Objects For Each Category In The Project" ) )
									{

										MainSettingsEnabler_Window.CheckSettings();
										var s = MainSettingsEnabler_Window.settings.First();
										UnityEditor.EditorUtility.FocusProjectWindow();
										Selection.objects = new[] { s };
										UnityEditor.ProjectWindowUtil.ShowCreatedAsset( s );
									}
									Draw.HELP( w, "You can select settings object in the project window" );
								}
								Draw.Sp( 5 );

								Draw.TOG( "Different colors for settings buttons", "USE_COLORS_FOR_CATEGORIES" );
								Draw.COLOR( "Color for settings", "COLORS_FOR_CATEGORIES" );
								Draw.Sp( 5 );
							}


						}



						Draw.Sp( 10 );

						GUI.Button( Draw.R2, "Some other features:", Draw.s( "preToolbar" ) );
						Draw.Sp( 10 );

						Draw.TOG( "Use hover color for modules buttons (bugs)", "USE_HOVER_FOR_BUTTONS" );
						using ( ENABLE( w ).USE( "USE_HOVER_FOR_BUTTONS", 0 ) ) Draw.HELP( w, "For some strange reason unity 2019 has strange behavior, it works only for internal styles with null textures, but mby in any latest version it will be fixed." );
						Draw.TOG( "Use expansion animation for hierarchy lines (bugs)", "USE_EXPANSION_ANIMATION" );
						using ( ENABLE( w ).USE( "USE_EXPANSION_ANIMATION", 0 ) ) Draw.HELP( w, "I could not catch a rectangle for animating elements, the unity always returns strange 0 y positions" );
						Draw.TOG( "Use dynamic GL batching for drawing", "USE_DINAMIC_BATCHING" );
						using ( ENABLE( w ).USE( "USE_DINAMIC_BATCHING", 0 ) ) Draw.HELP( w, "You can turn it off if you see any artifacts with textures." );

					}
					///  #########################################################################################################################################################################################

				} );




			}
		}


		internal static void QWE( IRepaint w, EditorSettingsAdapter.WindowSettings KEY, Action a1, Action a2, Action a3 )
		{


			using ( MainRoot.GRO( w ).UP( 0 ) )
			{
				Draw.Sp( 10 );

				// Draw.TOG_TIT( "Style" );
				Draw.TOG( "Override lines height", /*-*/"USE_LINE_HEIGHT", overrideObject: KEY, EnableRed: false );
				using ( MainRoot.ENABLE( w ).USE( Draw.GetSetter(/*-*/"USE_LINE_HEIGHT", overrideObject: KEY ) ) )
				{
					Draw.FIELD( "Line Height", /*-*/"LINE_HEIGHT", 4, 60, overrideObject: KEY );
					//    if ( p.par_e.USE_LINE_HEIGHT && p.par_e.LINE_HEIGHT < 16 ) Draw.HELP(w, "Warning! Since Unity 2017, line height < 16 sometimes throws DrawSelection Exeption!", new Color( 0.9f, 0.7f, 0.3f, 1 ) );
				}

				Draw.TOG( "Override child indent", /*-*/"USE_CHILD_INDENT", overrideObject: KEY, EnableRed: false );
				using ( MainRoot.ENABLE( w ).USE( Draw.GetSetter(/*-*/"USE_CHILD_INDENT", overrideObject: KEY ) ) ) Draw.FIELD( "Child Indent", /*-*/"CHILD_INDENT", 0, 60, overrideObject: KEY );
				Draw.TOG( "Override default icons size", /*-*/"USE_OVERRIDE_DEFAULT_ICONS_SIZE", overrideObject: KEY, EnableRed: false );
				using ( MainRoot.ENABLE( w ).USE( Draw.GetSetter(/*-*/"USE_OVERRIDE_DEFAULT_ICONS_SIZE", overrideObject: KEY ) ) ) Draw.FIELD( "Default icons size", /*-*/"OVERRIDE_DEFAULT_ICONS_SIZE", 2, 60, overrideObject: KEY );


				//     Draw.TOG( "Use horizontal scroll bar 'TEST'", "USE_HORISONTAL_SCROLL" );
				//  using ( GRO(w).UP( 0 ) )
				{
					Draw.TOG( "Override font size for Labels of GameObject names", /*-*/"USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE", overrideObject: KEY, EnableRed: false );
					using ( MainRoot.ENABLE( w ).USE( Draw.GetSetter(/*-*/"USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE", overrideObject: KEY ) ) )
						Draw.FIELD( "Objects Labels font size", /*-*/"OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE", 4, 60, overrideObject: KEY );
				}
				// FS

				Draw.HRx05( Draw.R );
				Draw.FIELD( "Hierarchy pro windows text font size", "COMMON_LABELS_FONT_SIZE", 4, 60, overrideObject: KEY );//
				Draw.HRx05( Draw.R );
				//Draw.HRx1();

				///  #########################################################################################################################################################################################

				a1();

			}



			a2();
			//OTHER



			// Draw.TOG( "Double click expands item", "DRAW_HIERARHCHY_CHESS_FILLS" );

			// Draw.HRx2();
			Draw.Sp( 10 );
			using ( MainRoot.GRO( w ).UP( 0 ) )
			{
				Draw.TOG_TIT( "Use background decorations", /*-*/"USE_BACKGROUNDDECORATIONS_MOD", overrideObject: KEY );

				using ( MainRoot.ENABLE( w ).USE( Draw.GetSetter(/*-*/"USE_BACKGROUNDDECORATIONS_MOD", overrideObject: KEY ), 0 ) )
				{


					Draw.TOG( "Draw hierarchy child lines", /*-*/"DRAW_HIERARHCHY_CHILD_LINES", overrideObject: KEY );
					using ( MainRoot.ENABLE( w ).USE( Draw.GetSetter(/*-*/"DRAW_HIERARHCHY_CHILD_LINES", overrideObject: KEY ) ) )
					{
						Draw.COLOR( "Child lines color", /*-*/"COLOR_HIERARHCHY_CHILD_LINES", overrideObject: KEY );
						Draw.TOOLBAR( new[] { "Solid", "Solid/Dotted", "Dotted", "Dotted/Dissolving" }, /*-*/"HIERARHCHY_CHILD_LINES_DOTS", overrideObject: KEY );
						switch ( KEY.HIERARHCHY_CHILD_LINES_DOTS )
						{
							case 0: Draw.HELP( w, "Solid lined for all objects" ); break;
							case 1: Draw.HELP( w, "Dotted lines for parent objects" ); break;
							case 2: Draw.HELP( w, "Dotted lines for all objects" ); break;
							case 3: Draw.HELP( w, "Dotted lines for all objects + additional dissolving for each parent objects" ); break;
						}
						var en = GUI.enabled;
						GUI.enabled &= KEY.HIERARHCHY_CHILD_LINES_DOTS != 0;
						Draw.FIELD( "Dot size:", /*-*/"HIERARHCHY_CHILD_LINES_DOT_SIZE", 1, 20, overrideObject: KEY );
						GUI.enabled = en;
						var S = KEY.HIERARHCHY_CHILD_LINES_DOTS < 3 ? "Parent lines transparency" :  "Parent lines fading factor (for deep hierarchy):";
						Draw.FIELD( S, /*-*/"HIERARHCHY_CHILD_LINES_TALPHA", 0, 1, overrideObject: KEY );
					}
					//Draw.HRx1();
					Draw.HRx05( Draw.R );


					GUI.Label( Draw.R, Draw.CONT( "Draw background chess fills:" ) );
					Draw.TOOLBAR( new[] { "No", "Clamped fills", "Full fills" }, /*-*/"DRAW_HIERARHCHY_CHESS_FILLS", overrideObject: KEY, enabled: new[] { true, KEY.pluginID == 0, true } );
					using ( MainRoot.ENABLE( w ).USE( Draw.GetSetter(/*-*/"DRAW_HIERARHCHY_CHESS_FILLS", overrideObject: KEY ) ) ) Draw.COLOR( "Background fill color", /*-*/"COLOR_HIERARHCHY_CHESS_FILLS", overrideObject: KEY );


					GUI.Label( Draw.R, Draw.CONT( "Draw horizontal separation lines:" ) );
					Draw.TOOLBAR( new[] { "No", "Clamped separations", "Full separations" }, /*-*/"DRAW_HIERARHCHY_CHESS_LINES", overrideObject: KEY, enabled: new[] { true, KEY.pluginID == 0, true } );
					using ( MainRoot.ENABLE( w ).USE( Draw.GetSetter(/*-*/"DRAW_HIERARHCHY_CHESS_LINES", overrideObject: KEY ) ) ) Draw.COLOR( "Horizontal lines color", /*-*/"COLOR_HIERARHCHY_CHESS_LINES", overrideObject: KEY );


					//Draw.HRx1();
					Draw.HRx05( Draw.R );

					//CHILD COUNT
					Draw.TOG( "Draw child count numbers", /*-*/"DRAW_CHILDS_COUNT", overrideObject: KEY );
					using ( MainRoot.ENABLE( w ).USE( Draw.GetSetter(/*-*/"DRAW_CHILDS_COUNT", overrideObject: KEY ) ) )
					{

						Draw.TOG( "Combine children values", /*-*/"CHILDCOUNT_COMBINE", overrideObject: KEY );
						Draw.DRAW_NEW( Draw.last );

						Draw.FIELD( "Font size", /*-*/"CHILDCOUNT_SIZE", 1, 30, overrideObject: KEY );
						Draw.COLOR( "Numbers color", /*-*/"DRAW_CHILDS_COLOR", overrideObject: KEY );
						Draw.TOG( "Invert colors", /*-*/"DRAW_CHILDS_INVERCE_COLOR", overrideObject: KEY );
						Draw.TOG( "Hide numbers for root object", /*-*/"HIDE_CHILDCOUNT_IFROOT", overrideObject: KEY );
						Draw.TOG( "Hide numbers for expanded object", /*-*/"HIDE_CHILDCOUNT_IFEXPANDED", overrideObject: KEY );
						Draw.TOOLBAR( new[] { "Align Left", "Align Middle", "Align Right" }, /*-*/"CHILDCOUNT_ALIGMENT", overrideObject: KEY );
						Draw.FIELD( "Numbers offset X", /*-*/"CHILDCOUNT_OFFSET_X", -200, 200, overrideObject: KEY );
						Draw.FIELD( "Numbers offset Y", /*-*/"CHILDCOUNT_OFFSET_Y", -200, 200, overrideObject: KEY );

					}
				}
			}
			a3();

		}

	}
}
