using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using EMX.HierarchyPlugin.Editor.Mods;

namespace EMX.HierarchyPlugin.Editor.Settings
{
    class TB_Window : ScriptableObject
    {


    }


    [CustomEditor(typeof(TB_Window))]
    class SETGUI_TopBar : MainRoot
    {

        internal static string set_text = USE_STR + "TopBar (Tool Bar)";
        internal static string set_key = "USE_TOPBAR_MOD";
        public override VisualElement CreateInspectorGUI()
        {
            return base.CreateInspectorGUI();
        }
        public override void OnInspectorGUI()
        {
            _GUI((IRepaint)this);
        }



        static void DrawButton(Rect r, int index)
        {
            var target_index = p.par_e.topbar_but.ToList().FindIndex(b => b.TOPBAR_BUT_INDEX == index);
            if (target_index == -1)
            {
                for (int x = 0; x < 4; x++)
                {
                    p.par_e.topbar_but[x].key = x;
                    p.par_e.topbar_but[x].TOPBAR_BUT_INDEX = x;
                }
                target_index = 0;
            }

            var target_button = p.par_e.topbar_but[target_index];
            if (target_button == null) target_button = p.par_e.topbar_but[0];


            GUI.Box(r, "");
            r.height = EditorGUIUtility.singleLineHeight;
            GUI.Label(r, target_button.NAME);
            r.y += r.height;
            // target_button.TOPBAR_BUT_APPLY_Y = GUI.Toggle(r, target_button.TOPBAR_BUT_APPLY_Y, "Apply Y offset");
            if (!target_button.ENABLED) GUI.Label(r, "Disabled!");
            r.y += r.height;
            r.width /= 5;
            r.x += r.width;
            var old_e = GUI.enabled;
            GUI.enabled = index != 0;
            if (GUI.Button(r, "◄"))
            {
                var i = p.par_e.topbar_but.ToList().FindIndex(b => b.TOPBAR_BUT_INDEX == index - 1);
                if (i == -1)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        p.par_e.topbar_but[x].key = x;
                        p.par_e.topbar_but[x].TOPBAR_BUT_INDEX = x;
                    }
                }
                else
                {
                    p.par_e.topbar_but[target_index].TOPBAR_BUT_INDEX = index - 1;
                    p.par_e.topbar_but[i].TOPBAR_BUT_INDEX = index;
                }
            }
            r.x += r.width;
            r.x += r.width;
            GUI.enabled = index != 3;
            if (GUI.Button(r, "►"))
            {
                var i = p.par_e.topbar_but.ToList().FindIndex(b => b.TOPBAR_BUT_INDEX == index + 1);
                if (i == -1)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        p.par_e.topbar_but[x].key = x;
                        p.par_e.topbar_but[x].TOPBAR_BUT_INDEX = x;
                    }
                }
                else
                {
                    p.par_e.topbar_but[target_index].TOPBAR_BUT_INDEX = index + 1;
                    p.par_e.topbar_but[i].TOPBAR_BUT_INDEX = index;
                }
            }
            GUI.enabled = old_e;

        }



        public static void _GUI(IRepaint w)
        {
            Draw.RESET(w);

            Draw.BACK_BUTTON(w);
            Draw.TOG_TIT(set_text, set_key, WIKI: WIKI_2_TOPBAR);
            Draw.Sp(10);
            using (ENABLE(w).USE(set_key))
            {




                Draw.TOG_TIT("Postition Settings:");



                Draw.FIELD("Top Offset:", "TOPBAR_LAYOUTS_MIN_Y_OFFSET", -500, 500);
                Draw.FIELD("Bottom Offset:", "TOPBAR_LAYOUTS_MAX_Y_OFFSET", -500, 500);

                var label_rect_left = EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight));
                label_rect_left.width /= 3;
                var label_rect_root = label_rect_left;
                label_rect_left.x += label_rect_left.width;
                var label_rect_right = label_rect_left;
                label_rect_right.x += label_rect_right.width;
                GUI.Label(label_rect_left, "Left Area:");
                GUI.Label(label_rect_right, "Right Area:");
                label_rect_root.y += label_rect_root.height;
                label_rect_left.y += label_rect_left.height;
                label_rect_right.y += label_rect_right.height;
                EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight));

                GUI.Label(label_rect_root, "Left Offset:");

                Draw.FIELD(label_rect_left, " ", "TOPBAR_LEFT_MIN_BORDER_OFFSET", -500, 500); //" + RIGHT + " area [X] left border offset
                Draw.FIELD(label_rect_right, " ", "TOPBAR_RIGHT_MIN_BORDER_OFFSET", -500, 500); //" + RIGHT + " area [X] right border offset
                                                                                                // Draw.TOG_TIT("" + RIGHT + " Area:");
                label_rect_root.y += label_rect_root.height;
                label_rect_left.y += label_rect_left.height;
                label_rect_right.y += label_rect_right.height;
                EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight));


                GUI.Label(label_rect_root, "Right Offset:");
                Draw.FIELD(label_rect_left, " ", "TOPBAR_LEFT_MAX_BORDER_OFFSET", -500, 500); //" + LEFT + " area [X] left border offset
                Draw.FIELD(label_rect_right, " ", "TOPBAR_RIGHT_MAX_BORDER_OFFSET", -500, 500); //" + LEFT + " area [X] right border offset



                Draw.TOG_TIT("Topbar Buttons Order:");


                using (MainRoot.GRO(w).UP(0))
                {

                    var r = EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight * 3));
                    r.width /= 4;

                    for (int i = 0; i < 4; i++)
                    {
                        DrawButton(r, i);
                        r.x += r.width;
                    }


                    //Draw.TOG("Swap Layout Areas", "TOPBAR_SWAP_LEFT_RIGHT");
                }
                Draw.Sp(10);




                //using ( MainRoot.GRO( w ).UP( 0 ) )
                {
                    Draw.TOG_TIT("Draw Layouts Tab", "DRAW_TOPBAR_LAYOUTS_BAR");
                    //Draw.TOG( "Draw layouts tab", "DRAW_TOPBAR_LAYOUTS_BAR" );
                    using (MainRoot.ENABLE(w).USE("DRAW_TOPBAR_LAYOUTS_BAR"))
                    {

                        Draw.TOG("Expand height for buttons", "TOPBAR_LAYOUTS_EXPAND_HEIGHT");
                        Draw.TOG("Use fixed width for buttons", "TOPBAR_LAYOUTS_USE_FIXED_WIDTH");

                        using (MainRoot.ENABLE(w).USE("TOPBAR_LAYOUTS_USE_FIXED_WIDTH"))
                        {
                            Draw.FIELD("Fixed width:", "TOPBAR_LAYOUTS_FIXED_WIDTH_AMOUNT", 2, 200); //" + RIGHT + " area [X] left border offset
                        }


                        Draw.TOG("Draw cross for selected layout", "TOPBAR_LAYOUTS_DRAWCROSS");

                        Draw.TOG("Autosave selected layout (when you switch to another)", "TOPBAR_LAYOUTS_AUTOSAVE");
                        using (MainRoot.ENABLE(w).USE("TOPBAR_LAYOUTS_AUTOSAVE"))
                        {
                            Draw.TOG("Disable autosave for internal layout", "TOPBAR_LAYOUTS_SAVE_ONLY_CUSTOM");
                            Draw.HELP(w, "Be careful, because you can rewrite even a default layout, however you can of course reset it");
                        }

                        Draw.HELP(w, "You can share your layouts that are stored in the: " +
                              Folders.DataGetterByType(Folders.CacheFolderType.SettingsData, Folders.DATA_SETTINGS_PATH_USE_DEFAULT).GET_PATH_TOSTRING + "/.SavedLayouts/");

                        if (GUI.Button(Draw.R, "Open " + LayoutsMod.ASSET_FOLDER))
                            Settings.SETGUI_MODS_ENABLER.REV(LayoutsMod.d);


                    }
                }





                //  Draw.Sp(3);
                Draw.Sp(5);

            }






#if !EMX_H_LITE
            Draw.Sp(10);
            //	using ( GRO( w ).UP( 0 ) )
            {
                Draw.TOG_TIT("Draw External Mods HotButtons on TopBar", "DRAW_TOPBAR_HOTBUTTONS", EnableRed: false);
                using (ENABLE(w).USE("DRAW_TOPBAR_HOTBUTTONS"))
                {
                    Draw.FIELD("TopBar Buttons Size", "TOPBAR_HOTBUTTON_SIZE", 3, 60, "px");
                    p.par_e.DrawHotButtonsArray();
                }
            }
            Draw.Sp(5);
#endif



            Draw.Sp(10);

            //using ( MainRoot.GRO( w ).UP( 0 ) )
            {






                //Draw.Sp( 10 );
                //  Draw.HRx2();
                using (MainRoot.GRO(w).UP(0))
                {

                    // Draw.TOG_TIT("Draw Layouts Tab", "DRAW_TOPBAR_LAYOUTS_BAR");
                    Draw.TOG_TIT("Draw custom TopGUI.onLeftLayoutGUI for left area", "DRAW_TOPBAR_CUSTOM_LEFT");
                    Draw.HELP(w, "You can add your gui at the top bars, using EM.LayoutsButtonsCustomization");
                    Draw.Sp(5);

                }


                Draw.Sp(10);
                //Draw.HRx2();
                //GUI.Label( Draw.R, "" + LEFT + " Area:" );
                using (MainRoot.GRO(w).UP(0))
                {



                    Draw.TOG_TIT("Draw custom TopGUI.onRightLayoutGUI for right area ", "DRAW_TOPBAR_CUSTOM_RIGHT");
                    Draw.HELP(w, "You can add your gui at the top bars, using EM.LayoutsButtonsCustomization");
                    Draw.Sp(4);
                }
                	Draw.Sp(10);

            }


            Draw.Sp(10);
            //Draw.HRx2();
            //GUI.Label( Draw.R, "" + LEFT + " Area:" );
            // using (MainRoot.GRO(w).UP(0))
            {

                // Draw.TOG_TIT( "" + LEFT + " Area:" );

              //  Draw.TOG_TIT("Quick tips:");
              //  // Draw.HELP_TEXTURE( "HELP_LAYOUT", 0);
              //  Draw.HELP(w, "DRAG to change button position.", drawTog: true);
              //  Draw.HELP(w, "MMB to remove button, .", drawTog: true);
              //  Draw.HELP(w, "RMB to open menu.", drawTog: true);
              //  //Draw.HRx2();
              //  Draw.Sp(10);
              //
              //  Draw.TOG_TIT("You can add your own gui on topbar:");
              //  Draw.HELP(w, "Use TopGUI.cs class", drawTog: true);
              //  Draw.Sp(3);
              //  // if (Draw.BUT("Select Script with Custom Examples")) { Selection.objects = new[] { Root.icons.example_folders[3] }; }
              //  Draw.Sp(20);


                Draw.TOG_TIT("Quick tips:");
                Draw.HELP_TEXTURE(w, "HELP_LAYOUT", 0);
                Draw.HELP(w, "DRAG to change button position.", drawTog: true);
                Draw.HELP(w, "MMB to remove button, .", drawTog: true);
                Draw.HELP(w, "RMB to open menu.", drawTog: true);
                //Draw.HRx2();
                Draw.Sp(10);

                Draw.TOG_TIT("You can add your own gui on topbar:", EnableRed: false);
                Draw.HELP(w, "Use TopGUI.cs class", drawTog: true);
                Draw.Sp(3);
                if (Draw.BUT("Select Script with Custom Examples")) { Selection.objects = new[] { Root.icons.example_folders[3] }; }
                Draw.Sp(20);


                //HOT BUTTONS

            }












            /*string RIGHT = "Right";
            string LEFT=  "Left";*/
            //string RIGHT = "Left";
            //string LEFT=  "Right";
            //if ( p.par_e.TOPBAR_SWAP_LEFT_RIGHT )
            //{
            //	var t = RIGHT;
            //	RIGHT = LEFT;
            //	LEFT = t;
            //}



            /*
            using ( GRO( w ).UP( 0 ) )
            {
                Draw.TOG_TIT( "Layouts and Hot Buttons:" );


                using ( GRO( w ).UP( 0 ) )
                {
                    Draw.TOG( "Swap Layout and Hot Buttons Areas", "TOPBAR_SWAP_LEFT_RIGHT" );
                }
                Draw.Sp( 10 );




                //using ( GRO( w ).UP( 0 ) )
                {
                    Draw.TOG_TIT( "Draw Layouts Tab", "DRAW_TOPBAR_LAYOUTS_BAR" ,EnableRed: false);
                    //Draw.TOG( "Draw layouts tab", "DRAW_TOPBAR_LAYOUTS_BAR" );
                    using ( ENABLE( w ).USE( "DRAW_TOPBAR_LAYOUTS_BAR" ) )
                    {
                        Draw.TOG( "Draw cross for selected layout", "TOPBAR_LAYOUTS_DRAWCROSS" );
                        Draw.FIELD( "Addition [Y] top border adjustment", "TOPBAR_LAYOUTS_MIN_Y_OFFSET", -500, 500 );
                        Draw.FIELD( "Addition [Y] bottom border adjustment", "TOPBAR_LAYOUTS_MAN_Y_OFFSET", -500, 500 );

                        Draw.TOG( "Autosave selected layout (when you switch to another)", "TOPBAR_LAYOUTS_AUTOSAVE" );
                        using ( ENABLE( w ).USE( "TOPBAR_LAYOUTS_AUTOSAVE" ) )
                        {
                            Draw.TOG( "Disable autosave for internal layout", "TOPBAR_LAYOUTS_SAVE_ONLY_CUSTOM" );
                            Draw.HELP( w, "Be careful, because you can rewrite even a default layout, however you can of course reset it" );
                        }

                        Draw.HELP( w, "You can share your layouts that are stored in the: " +
                            Folders.DataGetterByType( Folders.CacheFolderType.SettingsData, Folders.DATA_SETTINGS_PATH_USE_DEFAULT ).GET_PATH_TOSTRING + "/.SavedLayouts/" );

                        if ( GUI.Button( Draw.R, "Open " + LayoutsMod.ASSET_FOLDER ) )
                            Settings.SETGUI_MODS_ENABLER.REV( LayoutsMod.d );


                    }
                }





#if !EMX_H_LITE
                Draw.Sp( 10 );
                //	using ( GRO( w ).UP( 0 ) )
                {
                    Draw.TOG_TIT( "Draw External Mods HotButtons on TopBar", "DRAW_TOPBAR_HOTBUTTONS",EnableRed: false );
                    using ( ENABLE( w ).USE( "DRAW_TOPBAR_HOTBUTTONS" ) )
                    {
                        Draw.FIELD( "TopBar Buttons Size", "TOPBAR_HOTBUTTON_SIZE", 3, 60, "px" );
                        p.par_e.DrawHotButtonsArray();
                    }
                }
#endif
                Draw.Sp( 3 );

            }


            Draw.Sp( 10 );


            //using ( GRO( w ).UP( 0 ) )
            {






                //Draw.Sp( 10 );
                //  Draw.HRx2();
                using ( GRO( w ).UP( 0 ) )
                {
                    Draw.TOG_TIT( "" + RIGHT + " Area:" );

                    Draw.FIELD( "" + RIGHT + " area [X] left border offset", "TOPBAR_LEFT_MIN_BORDER_OFFSET", -500, 500 );
                    Draw.FIELD( "" + RIGHT + " area [X] right border offset", "TOPBAR_LEFT_MAX_BORDER_OFFSET", -500, 500 );

                    Draw.Sp( 5 );

                    Draw.TOG( "Use custom buttons for " + RIGHT.ToLower() + " area", "DRAW_TOPBAR_CUSTOM_LEFT" );
                    Draw.HELP( w, "You can add your gui at the top bars, using EMX." + Root.CUST_NS + "" );
                    Draw.Sp( 5 );

                }


                Draw.Sp( 10 );
                //Draw.HRx2();
                //GUI.Label( Draw.R, "" + LEFT + " Area:" );
                using ( GRO( w ).UP( 0 ) )
                {
                    Draw.TOG_TIT( "" + LEFT + " Area:" );

                    Draw.FIELD( "" + LEFT + " area [X] left border offset", "TOPBAR_RIGHT_MIN_BORDER_OFFSET", -500, 500 );
                    Draw.FIELD( "" + LEFT + " area [X] right border offset", "TOPBAR_RIGHT_MAX_BORDER_OFFSET", -500, 500 );


                    Draw.Sp( 5 );


                    Draw.TOG( "Use custom buttons for " + LEFT.ToLower() + " area ", "DRAW_TOPBAR_CUSTOM_RIGHT" );
                    Draw.HELP( w, "You can add your gui at the top bars, using EMX." + Root.CUST_NS + "" );
                    Draw.Sp( 4 );
                }
                //	Draw.Sp(10);

            }


            Draw.Sp( 10 );
            //Draw.HRx2();
            //GUI.Label( Draw.R, "" + LEFT + " Area:" );
            using ( GRO( w ).UP( 0 ) )
            {

                // Draw.TOG_TIT( "" + LEFT + " Area:" );

                Draw.TOG_TIT( "Quick tips:" );
                Draw.HELP_TEXTURE( w, "HELP_LAYOUT", 0 );
                Draw.HELP( w, "DRAG to change button position.", drawTog: true );
                Draw.HELP( w, "MMB to remove button, .", drawTog: true );
                Draw.HELP( w, "RMB to open menu.", drawTog: true );
                //Draw.HRx2();
                Draw.Sp( 10 );

                Draw.TOG_TIT( "You can add your own gui on topbar:", EnableRed: false );
                Draw.HELP( w, "Use TopGUI.cs class", drawTog: true );
                Draw.Sp( 3 );
                if ( Draw.BUT( "Select Script with Custom Examples" ) ) { Selection.objects = new[] { Root.icons.example_folders[ 3 ] }; }
                Draw.Sp( 20 );

                //HOT BUTTONS

            }
        }

            */
        }
    }
}
