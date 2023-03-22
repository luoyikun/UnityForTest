using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace EMX.HierarchyPlugin.Editor.Windows
{
	/* internal partial class Adapter
     {





         internal partial class BottomInterface
         {*/

	internal class ModulesWindow : IWindow
    {


        internal static ModulesWindow Init( MousePos rect, Window adapter )
        {
            if ( rect.type != MousePos.Type.ModulesListWindow_380_700 )
            {
                Debug.LogWarning( "Mismatch type" );
                rect.SetType( MousePos.Type.ModulesListWindow_380_700 );
            }

            Root.p[ 0 ].GET_SKIN();
            // if (Root.p[0].DEFAUL_SKIN == null ) Root.p[0].DEFAUL_SKIN = Root.p[0].GET_SKIN();

            // rect.y -= rect.height;

            var w = (ModulesWindow) private_Init(rect, typeof(ModulesWindow), adapter, "Right Bar", utils: true);

            w.SET_NEW_HEIGHT( adapter, rect.Height );

            return w;
        }

        static Vector2 sp;
        internal static int HH = 95;
        protected override void OnGUI()
        {
            if ( _inputWindow == null ) return;

            if ( adapter == null )
            {
                CloseThis();
                return;
            }


            base.OnGUI();


            // adapter.InitializeStyle();

            GUILayout.Space( EditorGUIUtility.singleLineHeight );

            GUILayout.Label( "Default Modules:" );

            //Space( 4 );
           
            var R = EditorGUILayout.GetControlRect(GUILayout.Height(HH));

            var en = GUI.enabled;
            //  if (adapter.modules == null) adapter.MOI.InitModules();
            Root.p[ 0 ].ChangeGUI( false );
            DRAW_MODULES( Root.p[ 0 ], R, 3, 3, true );
            Root.p[ 0 ].RestoreGUI();
            GUI.enabled = en;


            GUILayout.Space( EditorGUIUtility.singleLineHeight );

            R = EditorGUILayout.GetControlRect();
            var newM =Settings.Draw.TOG(R, "Use Custom Modules:", Root.p[0].par_e.RIGHT_USE_CUSTOMMODULES );

            if ( newM != Root.p[ 0 ].par_e.RIGHT_USE_CUSTOMMODULES )
            {
                Root.p[ 0 ].par_e.RIGHT_USE_CUSTOMMODULES = newM;
                InternalEditorUtility.RepaintAllViews();
            }

            //EditorGUILayout.HelpBox( "You should inherit the interface to create menu item", MessageType.None );

#pragma warning disable
            HH = 95;
            R = EditorGUILayout.GetControlRect( GUILayout.Height( HH ) );
            GUILayout.Space( EditorGUIUtility.singleLineHeight );
            var H3 = 325;
            var R2 = EditorGUILayout.GetControlRect(GUILayout.MinHeight(H3), GUILayout.Width(position.width - 10), GUILayout.ExpandHeight(true));
#pragma warning restore

            DrawCustomModules1( Root.p[ 0 ], R );
            EditorGUILayout.HelpBox( "Source code of default modules are placed into: '" + Folders.PluginInternalFolder + "/CustomModule_Example.cs'", MessageType.None );
            DrawCustomModules2( Root.p[ 0 ], R, R2 );


            Root.p[ 0 ].RestoreGUI();
        }

        static bool enddd3;

        internal static void DrawCustomModules1( PluginInstance adapter, Rect R )
        {
            enddd3 = GUI.enabled;
            // GUI.enabled &= !Adapter.LITE;

            GUI.enabled &= adapter.par_e.RIGHT_USE_CUSTOMMODULES;

            adapter.ChangeGUI();
            DRAW_MODULES( adapter, R, 3, 3, false );
            adapter.RestoreGUI();


            // GUILayout.  Space(EditorGUIUtility.singleLineHeight);
        }

        internal static void DrawCustomModules2( PluginInstance adapter, Rect R, Rect R2 )
        {
            R = R2;
            //R = EditorGUILayout.GetControlRect(GUILayout.MinHeight(H3), GUILayout.Width(position.width - 10 ), GUILayout.ExpandHeight(true));

            // sp = GUILayout.BeginScrollView( sp, GUILayout.Width(position.width), false);
            //sp = GUI.BeginScrollView( R, sp, new Rect( 0, 0, 500, R.height - GUI.skin.verticalScrollbar.fixedWidth ), true, false );
            // if ( adapter.IS_HIERARCHY() ) 
            // EditorGUI.TextArea( new Rect( 0, 0, 500, R.height - GUI.skin.verticalScrollbar.fixedWidth ), HIERARCHY_MODULE_HELP );
            // else EditorGUI.TextArea( new Rect( 0, 0, 500, R.height - GUI.skin.verticalScrollbar.fixedWidth ), PROJECT_MODULE_HELP );
            //EditorGUILayout.GetControlRect(GUILayout.Height(H3 - GUI.skin.verticalScrollbarDownButton.fixedWidth));

            var r = new Rect( R.x, R.y, R.width, 50);
            EditorGUI.HelpBox( r, "You can add your custom mods using 'EMX." + Root.CUST_NS + ".ExtensionInterface_CustomRightMod.Slot_1'.", MessageType.Info );
            r.y += r.height;
            r.height = 40;
            if ( GUI.Button( r, "Select Script with Custom Examples" ) ) { Selection.objects = new[] { Root.icons.example_folders[ 1 ] }; }
            r.y += r.height;
            if ( GUI.Button( r, "Open " + Settings.SETGUI_RightBar.set_text.Substring( 4 ) + " Settings" ) ) { Settings.MainSettingsEnabler_Window.Select<Settings.RM_Window>(); }



           // GUI.EndScrollView();

            /* if (adapter.IS_HIERARCHY()) HelpBox( "To add your own module, inherit the slot class (HierarchyExtensions.CustomModule_Slot1 / 2 / 3) anywhere in your code", MessageType.None );
             else HelpBox( "To add your own module, inherit the slot class (ProjectExtensions.CustomModule_Slot1 / 2 / 3) anywhere in your code", MessageType.None );*/
            //  END_PADDING();

            GUI.enabled = enddd3;
        }




        // 
        //         static string HIERARCHY_MODULE_HELP = @"
        //     class MyModule : HierarchyExtensions.CustomModule_Slot3
        //     {
        //         public override string NameOfModule { get { return ""MyModule""; } }
        //     
        //         // In this method, you can display information and buttons
        //         public override void Draw(Rect drawRect, GameObject o)
        //         {
        //             //  different built-in methods for changing variables
        //             //        if (GUI.Button(drawRect,""string"")) SHOW_StringInput(...
        //             //        if (GUI.Button(drawRect,""int"")) SHOW_IntInput(...
        //             //        if (GUI.Button(drawRect,""dropdown"")) SHOW_DropDownMenu(...
        //         }
        //     
        //         // ToString(...) method is used for the search box
        //         public override string ToString(GameObject o)
        //         {
        //             return null;
        //         }
        //     }";
        /*
        static string PROJECT_MODULE_HELP = @"
    class MyModule : ProjectExtensions.CustomModule_Slot1
    {
        public override string NameOfModule { get { return ""MyModule""; } }
    
        // In this method, you can display information and buttons
        public override void Draw(Rect drawRect, string assetPath, string assetGuid, int instanceId, bool isFolder, bool isMainAsset)
        {
          // You can invoke different built-in methods for changing variables
          //        if (GUI.Button(drawRect,""string"")) SHOW_StringInput(...
          //        if (GUI.Button(drawRect,""int"")) SHOW_IntInput(...
          //        if (GUI.Button(drawRect,""dropdown"")) SHOW_DropDownMenu(...
        }
    
        // ToString(...) method is used for the search box
        public override string ToString(string assetPath, string assetGuid, int instanceId, bool isFolder, bool isMainAsset)
        {
          return null;
        }
    }";
    */
        static GUIStyle _button;
        static GUIStyle button {
			get {
                if (_button == null )
				{
                    _button = new GUIStyle( Root.p[ 0 ].button );
                    _button.clipping = TextClipping.Clip;
				}
                _button.fontSize = Root.p[ 0 ].button.fontSize;
                return _button;
			}
		}

        internal static void DRAW_MODULES( PluginInstance adapter, Rect RECT, int CELLS, int LINES, bool drawMain )
        {
            var HH = 25;
            // var SH = 3;
            // int LINECOUNT = 3;
            int WIDTH = 78 * 4 / CELLS;
            var PADDING = 5;
            // DrawNew(HH + PADDING);
            //   var RECT = GET_OFFSETRECT( (HH) * LINES + PADDING * 2/*, new[] { Width(WIDTH * 3) }*/, -5 );
            var or = RECT;
            RECT.x += PADDING;
            RECT.y += PADDING;
            RECT.height -= PADDING * 2;
            RECT.width -= PADDING * 2;
            var SX = (RECT.width - WIDTH * CELLS) / (CELLS - 1);
            //RECT.width = WIDTH * 3;
            int interato = 0;

            var res = GUI.enabled;
            or.height += 3;
            adapter.INTERNAL_BOX( or, "" );
            var Y = 0;
            var m = drawMain ? adapter.modsController.rightModsManager.rightMods.Reverse() : adapter.modsController.rightModsManager.rightMods;
            foreach ( var source in m )
            {
                if ( drawMain && (source.savedData.sibling == -1
                             || (
                                  source is Mod_UserModulesRoot
                             /* ||
#endif
#if PROJECT
                              source is EModules.EProjectInternal.Project.M_UserModulesRoot
#endif*/
                             )
                ) ) continue;
                if ( !drawMain
                && !(
                     source is Mod_UserModulesRoot
                /*             ||
     #endif
     #endif
     #if PROJECT
                             source is EModules.EProjectInternal.Project.M_UserModulesRoot
     #endif*/
                )
            ) continue;

                if ( interato == CELLS )
                {
                    Y++;
                    interato = 0;
                }

                var drawRect = new Rect(RECT.x + (WIDTH + SX) * interato, RECT.y + (HH + 3) * Y, WIDTH, HH);
                /* Label("", Width(width), Height(25));
                 lastRect = GetLastRect();*/
                GUI.enabled = res & source.enableOverride();

                if ( source.savedData.enabled )
                { /* Hierarchy.colorText11.SetPixel(0, 0, new Color(0.6f, 0.3f, 0.1f, 1));
                 Hierarchy.colorText11.Apply();*/
                    if ( Event.current.type.Equals( EventType.Repaint ) )
                    {
                        var asd = GUI.color;
                        //GUI.color *= new Color( 0.6f, 0.3f, 0.1f, 1 );
                        // GUI.DrawTexture(drawRect, redTTexure);
                        var glowR = drawRect;
                        // glowR.y -= 4;
                        GUI.color *= Root.p[0].par_e.BUTTON_TAP_COLOR;
                        if (!GUI.enabled) GUI.color *=  new Color( 1,1,1, 0.3f );
                        //glowR.height += 8;

                        adapter.gl.DRAW_TAP_GLOW( glowR, GUI.color );
                        /// GUI.DrawTexture( glowR, adapter.STYLE_DEFBUTTON.active.background );
                        ///
                        /// 
                        /* GUI.DrawTexture( glowR, adapter.GetIcon( "HIPERUI_BUTTONGLOW" ) );
                         GUI.DrawTexture( glowR, adapter.GetIcon( "HIPERUI_BUTTONGLOW" ) );
                         GUI.DrawTexture( glowR, adapter.GetIcon( "HIPERUI_BUTTONGLOW" ) );
                         GUI.DrawTexture( glowR, adapter.GetIcon( "HIPERUI_BUTTONGLOW" ) );
                         GUI.DrawTexture( glowR, adapter.GetIcon( "HIPERUI_BUTTONGLOW" ) );*/
                        /* if (Event.current.type == EventType.repaint)
                         {
                             var t = Adapter.GET_SKIN().box.normal.background;
                             var b = Adapter.GET_SKIN().box.border;
                             var B = Mathf.RoundToInt(Hierarchy.GetIcon("HIPERUI_BUTTONGLOW").width * 0.4f);
                             Adapter.GET_SKIN().box.border = new RectOffset(B, B, B, B);
                             Adapter.GET_SKIN().box.normal.background = Hierarchy.GetIcon("HIPERUI_BUTTONGLOW");
                             Adapter.GET_SKIN().box.Draw(drawRect, false, false, false, false);
                             Adapter.GET_SKIN().box.Draw(drawRect, false, false, false, false);
                             Adapter.GET_SKIN().box.Draw(drawRect, false, false, false, false);
                             Adapter.GET_SKIN().box.Draw(drawRect, false, false, false, false);
                             Adapter.GET_SKIN().box.normal.background = t;
                             Adapter.GET_SKIN().box.border = b;
                         }*/
                        GUI.color = asd;
                    }
                }

                GUIContent content = new GUIContent();
                if ( source.HeaderTexture2D != null )
                {
                    var sq =drawRect;
                    if ( sq.width > sq.height ) sq.width = sq.height; else sq.height = sq.width;
                    sq.x += 4;
                    sq.y += 4;
                    sq.width -= 8;
                    sq.height -= 8;
                    adapter.gl._DrawTexture( sq, adapter.GetNewIcon( NewIconTexture.RightMods, source.HeaderTexture2D ) );
                }
              
                //if ( source.HeaderTexture2D != null ) content.image = adapter.GetOldIcon( source.HeaderTexture2D ).texture;
                else content.text = source.HeaderText;
                content.tooltip = "Enable/Disable";
                // if ( drawMain && !source.enableOverride() ) content.tooltip += " (Pro Only)";
                if ( !drawMain )
                {
                    content.tooltip = source.ContextHelper;
                    if ( !source.enableOverride() ) content.tooltip += source.enableOverrideMessage();
                }

                if ( adapter.SimpleButton( drawRect, content, button ) )
                {
                    //    source.CreateUndo();
                    source.savedData.enabled = !source.savedData.enabled;
                    // source.SetDirty();
                    adapter.RepaintWindow( adapter.pluginID, true );
                    //InternalEditorUtility.RepaintAllViews();
                }

                EditorGUIUtility.AddCursorRect( drawRect, MouseCursor.Link );
                interato++;
            }


            GUI.enabled = res;


        }

    }
}