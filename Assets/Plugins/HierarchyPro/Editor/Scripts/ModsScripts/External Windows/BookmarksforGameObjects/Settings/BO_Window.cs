using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using EMX.HierarchyPlugin.Editor.Mods;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class BO_Window : ScriptableObject
	{
	}
	[CustomEditor( typeof( BO_Window ) )]
	class SETGUI_BookmarkObjectsQuick : MainRoot
	{

		internal static string set_text =  USE_STR + "BB / EW - Bookmarks Quick Buffer";
		internal static string set_key = "USE_BOOKMARKS_HIERARCHY_MOD";
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

			Draw.BACK_BUTTON( w );
			Draw.TOG_TIT( set_text, set_key, WIKI: WIKI_5_BOOKMARKS );
			Draw.Sp( 10 );

			using ( ENABLE( w ).USE( set_key ) )
			{


				var p = Mods.DrawButtonsOld.GET_DISPLAY_PARAMS(MemType.Custom);

				// Draw.TOG( "Draw hot button for this mod", "DRAW_TOPBAR_H1" );
				// Draw.TOG( "HyperGraph", "DRAW_TOPBAR_H1" );
				// Draw.TOG( "Project Folders", "DRAW_TOPBAR_H2" );
				// Draw.TOG( "Hierarchy Bookmarks", "DRAW_TOPBAR_H3" );
				// Draw.TOG( "Hierarchy Scenes", "DRAW_TOPBAR_H4" );
				// Draw.TOG( "Hierarchy Expanded Objects", "DRAW_TOPBAR_H6" );
				// 
				/// using ( GRO( w ).UP( 0 ) )
				{
					Draw.TOG( "Draw hot button for this mod", "DRAW_TOPBAR_H3" );
				}
				Draw.Sp( 10 );

				using ( GRO( w ).UP( 0 ) )
				{
					Draw.TOG_TIT( "Categories:" );
					Draw.TOG( "Draw category name", "BOOKMARKS_OB_DRAW_CATEGORY_NAME" );
					Draw.HRx05( Draw.R05 );
					Draw.COLOR( "Override buttons colors", p._BgOverrideColor_KEY, overrideObject: p );
					Draw.HRx05( Draw.R05 );
					Draw.TOG( "Use special background colors", "BOOKMARKS_OB_DRAW_BG_COLOR" );
					using ( ENABLE( w ).USE( "BOOKMARKS_OB_DRAW_BG_COLOR" ) )
					{
						if ( GUI.Button( Draw.R15, "Open categories background colors window" ) )
						{
							var pos = new MousePos(Event.current.mousePosition, MousePos.Type.ColorChanger_230_0, false, Root.p[0]);
							CategoriesColorsWindowOld.InitForGameObjectsBookmarks( pos, null, Root.p[ 0 ].LastActiveScene );
						}
					}

					//Draw.HELP( w, "Use special background colors" );
					//
					

					Draw.Sp( 4 );
				}



				Draw.Sp( 10 );
				using ( GRO( w ).UP( 0 ) )
				{
					Draw.TOG_TIT( "Buttons:" );

					Draw.FIELD( "Font size", p._fontSize_KEY, 4, 30, overrideObject: p );
					Draw.FIELD( "Rows number", p._Rows_KEY, 1, 10, overrideObject: p );
					Draw.FIELD( "Max buttons", p._MaxItems_KEY, 1, 30, overrideObject: p );
					Draw.Sp( 5 );

					using ( GRO( w ).UP( 0 ) )
					{
						GUI.Label( Draw.R, "Buttons direction order starts from:" );
						Draw.TOOLBAR( new[] { "TOP/LEFT", "TOP/RIGHT", "BOTTOM/LEFT", "BOTTOM/RIGHT" }, p._SortButtonsOrder_KEY, overrideObject: p );
						Draw.Sp( 4 );
					}
					Draw.Sp( 5 );

					Draw.TOG( "Tooltips for buttons", p._DrawTooltips_KEY, overrideObject: p );

					Draw.HRx05( Draw.R15 );

					Draw.TOG( "Use 'Alt' + click on button to duplicate gameobjects", "BOOKMARKS_OB_ALT_TO_INSTANTIATE" );
					using ( ENABLE( w ).USE( "BOOKMARKS_OB_ALT_TO_INSTANTIATE" ) )
					{
						GUI.Label( Draw.R, "Position for new instance:" );
						Draw.TOOLBAR( new[] { "Prefab Position", "Align With SceneView" }, "BOOKMARKS_OB_INSTANTIATE_POSITION" );
						GUI.Label( Draw.R, "Parent for new instance:" );
						Draw.TOOLBAR( new[] { "Root", "Get Selection's Parent" }, "BOOKMARKS_OB_INSTANTIATE_PARENT" );
					}
					Draw.Sp( 10 );
				}


				Draw.Sp( 10 );
				using ( GRO( w ).UP( 0 ) )
				{
					Draw.TOG_TIT( "Other Modules Interaction:" );
					Draw.TOG( "Draw highlighter colors", p._DrawHiglighter_KEY, overrideObject: p );
					using ( ENABLE( w ).USE( set_key ) ) Draw.FIELD( "Highlighter colors opacity", p._HiglighterOpacity_KEY, 0f, 1f, overrideObject: p );

					Draw.HRx05( Draw.R );
					using ( ENABLE( w ).USE( "USE_RIGHT_ALL_MODS" ,0) )  Draw.TOG( "Draw descriptions from description module", "BOOKMARKS_OB_SHOWDESCRIPTS" );
					Draw.Sp( 4 );
				}

			}



			Draw.Sp( 10 );
			//Draw.HRx2();
			//GUI.Label( Draw.R, "" + LEFT + " Area:" );
			using ( GRO( w ).UP( 0 ) )
			{

				// Draw.TOG_TIT( "" + LEFT + " Area:" );

				Draw.TOG_TIT( "Quick tips:" );
				//Draw.HELP( w, "Shift/Ctrl - to apply additional selection.", drawTog: true );
				//Draw.HELP( w, "Alt - you can enable instance creating for this buttons.", drawTog: true );
				//Draw.HELP( w, "shift/ctrl + LMB - add object to selection", drawTog: true );
				//Draw.HELP( w, "alt + LMB - create instance", drawTog: true );

				//Draw.Sp( 10 );

				//Draw.HELP( w, "Use left-click to select object.", drawTog: true );
				//Draw.HELP( w, "Use left-drag to change button position select object.", drawTog: true );
				//Draw.HELP( w, "Use left-click to special button to change or create category.", drawTog: true );
				//Draw.HELP( w, "Use right-click to remove object.", drawTog: true );
				Draw.HELP( w, "DRAG - change ordering", drawTog: true );
				Draw.HELP( w, "Drag objects to other windows - to hierarchy or to variable in the inspector", drawTog: true );
				Draw.HELP( w, "LMB - select( Ctrl / Shift - multi selection )", drawTog: true );
				Draw.HELP( w, "alt+LMB - create instance", drawTog: true );
				Draw.HELP( w, "MMB - remove", drawTog: true );
				Draw.HELP( w, "RMB - remove", drawTog: true );
				Draw.Sp( 10 );

				Draw.HELP( w, "You can create different categories", drawTog: true );
				Draw.HELP( w, "You can assign more than one object to one button", drawTog: true );
				Draw.HELP( w, "You can add descriptions", drawTog: true );
				Draw.HELP_TEXTURE( w, "TAP_BOOK" );
				Draw.HELP( w, "Right-click on the icon to open a special menu for quick access to mod functions", drawTog: true );
				Draw.HELP_TEXTURE( w, "DRAG_BOOK" );

			}
		}
	}


}
