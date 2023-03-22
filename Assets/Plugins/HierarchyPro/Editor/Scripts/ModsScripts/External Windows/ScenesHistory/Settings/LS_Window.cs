using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class LS_Window : ScriptableObject
    {
    }


    [CustomEditor( typeof( LS_Window ) )]
    class SETGUI_LastScenes : MainRoot
    {

        internal static string set_text =  USE_STR + "BB / EW - Last Scenes";
        internal static string set_key = "USE_LAST_SCENES_MOD";
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

            Draw.BACK_BUTTON( w );
            Draw.TOG_TIT( set_text, set_key ,WIKI: WIKI_5_SCENES );
            Draw.Sp( 10 );

            using ( ENABLE(w).USE( set_key ) )
            {

                //using ( GRO( w ).UP( 0 ) )
                {
                    Draw.TOG( "Draw hot button for this mod", "DRAW_TOPBAR_H4" );
                }
                Draw.Sp( 10 );
                var p = Mods.DrawButtonsOld.GET_DISPLAY_PARAMS(MemType.Scenes);

				//Draw.Sp( 10 );
				using ( GRO( w ).UP( 0 ) )
				{

					// Draw.TOG( "Draw hot button for this mod", "DRAW_TOPBAR_H1" );
					// Draw.TOG( "HyperGraph", "DRAW_TOPBAR_H1" );
					// Draw.TOG( "Project Folders", "DRAW_TOPBAR_H2" );
					// Draw.TOG( "Hierarchy Bookmarks", "DRAW_TOPBAR_H3" );
					// Draw.TOG( "Hierarchy Scenes", "DRAW_TOPBAR_H4" );
					// Draw.TOG( "Hierarchy Expanded Objects", "DRAW_TOPBAR_H6" );

					Draw.TOG_TIT( "Common Settings:" );
					Draw.TOG( "Display full scenes paths for menus", "SCENE_HISTORY_DRAW_FULLPATH_FOR_MENU" );
					Draw.Sp( 4 );
				}

				Draw.Sp( 10 );

				//Draw.Sp( 10 );
				using ( GRO(w).UP( 0 ) )
                {

                   // Draw.TOG( "Draw hot button for this mod", "DRAW_TOPBAR_H1" );
                   // Draw.TOG( "HyperGraph", "DRAW_TOPBAR_H1" );
                   // Draw.TOG( "Project Folders", "DRAW_TOPBAR_H2" );
                   // Draw.TOG( "Hierarchy Bookmarks", "DRAW_TOPBAR_H3" );
                   // Draw.TOG( "Hierarchy Scenes", "DRAW_TOPBAR_H4" );
                   // Draw.TOG( "Hierarchy Expanded Objects", "DRAW_TOPBAR_H6" );

                    Draw.TOG_TIT( "Window:" );
                    Draw.COLOR( "Override buttons colors", p._BgOverrideColor_KEY, overrideObject: p );
					Draw.Sp( 4 );
				}

             

                Draw.Sp( 10 );
                using ( GRO(w).UP( 0 ) )
                {
                    Draw.TOG_TIT( "Buttons:" );
                    Draw.FIELD( "Font size", p._fontSize_KEY, 4, 30, overrideObject: p );
                    Draw.FIELD( "Rows number", p._Rows_KEY, 1, 10, overrideObject: p );
                    Draw.FIELD( "Max buttons", p._MaxItems_KEY, 1, 30, overrideObject: p );

                    Draw.Sp( 5 );
                    using ( GRO(w).UP( 0 ) )
                    {
                        GUI.Label( Draw.R, "Buttons direction order starts from:" );
                        Draw.TOOLBAR( new[] { "TOP/LEFT", "TOP/RIGHT", "BOTTOM/LEFT", "BOTTOM/RIGHT" }, p._SortButtonsOrder_KEY, overrideObject: p );
                        Draw.Sp( 4 );
                    }
                    Draw.Sp( 5 );

                    Draw.TOG( "Draw tooltips for buttons", p._DrawTooltips_KEY, overrideObject: p );
                    Draw.Sp( 4 );

                }

                Draw.Sp( 10 );
                using ( GRO(w).UP( 0 ) )
                {
                    Draw.TOG_TIT( "Other Modules Interaction:" );
                    Draw.TOG( "Draw highlighter colors", p._DrawHiglighter_KEY, overrideObject: p );
                    using ( ENABLE(w).USE( set_key ) ) Draw.FIELD( "Highlighter colors opacity", p._HiglighterOpacity_KEY, 0, 1, overrideObject: p );
                    Draw.Sp( 4 );

                }

             


            }



            Draw.Sp( 10 );
            //Draw.HRx2();
            //GUI.Label( Draw.R, "" + LEFT + " Area:" );
            using ( GRO(w).UP( 0 ) )
            {

                // Draw.TOG_TIT( "" + LEFT + " Area:" );

                Draw.TOG_TIT( "Quick tips:" );

				//Draw.HELP( w, "shift + LMB - add scene as additional.", drawTog: true );
				//Draw.HELP( w, "ctrl + LMB - find scene in project.", drawTog: true );
                //
				//Draw.Sp( 10 );

				//Draw.HELP(w, "Use left-click to select object.", drawTog: true );
				//Draw.HELP(w, "Use left-drag to change button position select object.", drawTog: true );
				//Draw.HELP(w, "Use left-click to pin button.", drawTog: true );
				//Draw.HELP(w, "Use right-click to remove object.", drawTog: true );
				Draw.HELP( w, "DRAG - change ordering", drawTog: true );
				Draw.HELP( w, "LMB - open scene (Shift - additional open / Ctrl - find scene in project)", drawTog: true );
				Draw.HELP( w, "MMB - remove", drawTog: true );
				Draw.Sp( 10 );

				Draw.HELP_TEXTURE(w, "TAP_SCENE" );
                Draw.HELP(w, "Right-click on the icon to open a special menu for quick access to mod functions.", drawTog: true );
                Draw.HELP_TEXTURE(w, "DRAG_SCENE" );
           
            }
        }
    }
}
