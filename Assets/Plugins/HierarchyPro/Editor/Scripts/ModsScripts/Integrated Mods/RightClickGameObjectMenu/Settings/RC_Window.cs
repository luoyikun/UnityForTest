using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class RC_Window : ScriptableObject
    {
    }


    [CustomEditor( typeof( RC_Window ) )]
    class SETGUI_RightClickMenu : MainRoot
    {
        internal static string set_text =  USE_STR + "RightClick Objects Menu (Hierarchy Window)";
        internal static string set_key = "USE_RIGHT_CLICK_MENU_MOD";
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
            Draw.TOG_TIT( set_text, set_key, WIKI: WIKI_2_RIGHTCLICK );

            using ( GRO(w).UP( 0 ) )
            {

                Draw.Sp( 10 );

                // Draw.TOG_TIT(set_text, set_key);
                Draw.HELP(w, "This is a special customizable gameobject menu" );
                //Draw.Sp(10);

                using ( ENABLE(w).USE( set_key ) )
                {


                    Draw.Sp( 10 );
                    using ( GRO(w).UP( 0 ) )
                    {

						Draw.TOG( "Use subcategory for menu items '" + Root.HierarchyPro + "/..'", "RCGO_MENU_PLACE_TO_SUBMENU", rov: Draw.R );

						Draw.HRx05( Draw.R );

						Draw.TOG( "Allow using hotkeys", "RCGO_MENU_USE_HOTKEYS", rov: Draw.R );
                        using ( ENABLE(w).USE( "RCGO_MENU_USE_HOTKEYS", 0 ) )
                        {

                            GUI.Label( Draw.R, "Other windows that will catch hotkeys events" );

                            var htks = p.par_e.CUSTOMMENU_HOTKEYS_WINDOWS;

                            var oldw1 = htks.ContainsKey("SceneView");
                            var w1 = GUI.Toolbar(Draw.R, oldw1 ? 1 : 0, new[] { "No", "SceneView" }, EditorStyles.toolbarButton) == 1;
                            var oldw2 = htks.ContainsKey("GameView");
                            var w2 = GUI.Toolbar(Draw.R, oldw2 ? 1 : 0, new[] { "No", "GameView" }, EditorStyles.toolbarButton) == 1;
                            var oldw3 = htks.ContainsKey("Inspector");
                            var w3 = GUI.Toolbar(Draw.R, oldw3 ? 1 : 0, new[] { "No", "Inspector" }, EditorStyles.toolbarButton) == 1;

                            if ( w1 != oldw1 || w2 != oldw2 || w3 != oldw3 )
                            {
                                var res = new Dictionary<string, bool>();
                                if ( w1 ) res.Add( "SceneView", true );
                                if ( w2 ) res.Add( "GameView", true );
                                if ( w3 ) res.Add( "Inspector", true );
                                p.par_e.CUSTOMMENU_HOTKEYS_WINDOWS = res;
                            }
                        }

                        Draw.HRx05( Draw.R );

                        // Draw.Sp( 16 );
#if !EMX_H_LITE
                        Draw.TOG( "Include hierarchy pro external and highlighter windows", "INCLUDE_RIGHTCLIK_MENU_OPENPLUGINWINDOWS_BUTTONS", rov: Draw.R );
#endif
                        Draw.Sp( 8 );

					}

					Draw.Sp( 10 );
                    //Draw.HRx2();
                    //GUI.Label( Draw.R, "" + LEFT + " Area:" );
                    using ( GRO(w).UP( 0 ) )
                    {
                        Draw.TOG_TIT( "Quick tips:" );
                       // Draw.HELP_TEXTURE(w, "HELP_RIGHT_MENU", 0 );
                        Draw.HELP(w, "RMB on the gameobject to menu.", drawTog: true );
                        Draw.HELP(w, "Hotkeys work only for windows selected in the settings, that means if you are working with animator window, hotkeys for hierarchy will not working.", drawTog: true );

                        Draw.Sp( 10 );

                        Draw.TOG_TIT( "You can add your own menu items:", EnableRed: false );
                        Draw.HELP(w, "Use 'EMX." + Root.CUST_NS + ".ExtensionInterface_RightClickOnGameObjectMenuItem' interface", drawTog: true );

                        Draw.Sp( 3 );
                        if ( Draw.BUT( "Select Script with Custom Examples" ) ) { Selection.objects = new[] { Root.icons.example_folders[ 0 ] }; }
                        Draw.Sp( 20 );
                    }
                }
            }
        }
    }

}
