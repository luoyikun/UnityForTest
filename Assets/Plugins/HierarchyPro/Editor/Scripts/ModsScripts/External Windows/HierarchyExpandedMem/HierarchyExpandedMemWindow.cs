using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace EMX.HierarchyPlugin.Editor.Mods
{

	class HierarchyExpandedMemWindow : ExternalModRoot, IHasCustomMenu
    {


        internal override int pluginID { get { return 0; } }
        HierarchyExpandedMemInstance __instance;
        internal HierarchyExpandedMemInstance instance { get { return __instance ?? (__instance = new HierarchyExpandedMemInstance()); } }
        internal const string NAME = "Expanded Mem";
        const int priority = 10;
        //static MemType cType = MemType.Hier;


        internal static void SubscribeEditorInstanceStatic( EditorSubscriber sbs )
        {
            if ( !Root.p[ 0 ].par_e.USE_LAST_SCENES_MOD )
            {
                foreach ( var item in Root.p[ 0 ].modsController.activeExternalMods.ToList() )
                    if ( item && item is HierarchyExpandedMemWindow ) item.Close();
                return;
            }



            // if ( !Root.p[ 0 ].par_e.USE_LAST_SCENES_MOD ) return;

            /*	sbs.OnSceneOpening += instance.SCENE_CHANGE;
				sbs.OnSelectionChanged += instance.CHANGE_SELECTION;
				sbs.OnPlayModeStateChanged += instance.CHANGEPLAYMODE;
				sbs.OnUpdate += instance.Update;*/


            sbs.ExternalMod_Buttons.Add( new ExternalMod_Button( typeof( HierarchyExpandedMemWindow ) ) {
                text = NAME,
                icon = () => "HIER_EXPAND_ICON",
                enabled = Root.p[ 0 ].par_e.DRAW_TOPBAR_H6,
                priority = Root.p[ 0 ].par_e.ORDER_TOPBAR_H6,
                release = ICON_CLICK,
                menuGen = MENU_GEN
            } );

            sbs.ExternalMod_MenuItems.Add( new ExternalMod_MenuItem() {
                text = NAME,
                path = "- Open " + NAME.ToLower() + " in window",
                priority = priority,
                release = ICON_CLICK,
            } );
        }

        internal override void SubscribeEditorInstance( AdditionalSubscriber sbs )
        {
            instance.SubscribeEditorInstance( sbs );
            //sbs.ExternalMod_MenuItems
        }


        public void AddItemsToMenu( GenericMenu menu )
        {
            generate_menu( menu, NAME, lastController );
        }
        static void ICON_CLICK( int button, string name )
        {
            if ( button == 0 )
            {
                //controller = ;
                //if (W.minSize.x < 40 || W.minSize.y < 16) {W.minSize = new Vector2(40, 16); }
                //	W.ShowTab();
                //var W = Root.p[0].par_e.ATTACH_TO_INSPECT_ONOPEN ? LastScenesHistoryModWindow.GetWindow<LastScenesHistoryModWindow>(name, true, InspectorType) : LastScenesHistoryModWindow.GetWindow<LastScenesHistoryModWindow>(name, true);

                GetExternalWindow<HierarchyExpandedMemWindow>.Show( name, GameObjectsSelectionHistoryModWindow.bindTypes );
            }
            if ( button == 1 )
            {
                var menu = new GenericMenu();
                MENU_GEN( menu, name, lastController );

                menu.ShowAsContext();
            }
        }
        static void MENU_GEN( GenericMenu menu, string name, ExternalDrawContainer lastController )
        {
            menu.AddItem( new GUIContent( "- Open " + NAME.ToLower() + " in window" ), false, () => {

                GetExternalWindow<HierarchyExpandedMemWindow>.Show( name, GameObjectsSelectionHistoryModWindow.bindTypes );
            } );
            menu.AddSeparator( "" );
            generate_menu( menu, name, lastController );
        }



        static void generate_menu( GenericMenu menu, string name, ExternalDrawContainer lastController )
        {
            DrawButtonsOld.SET_HIER( menu, lastController ?? new ExternalDrawContainer( 0 ) { /*type = cType*/ }, EditorSceneManager.GetActiveScene() );
            menu.AddSeparator( "" );
            menu.AddItem( new GUIContent( "- Open " + NAME.ToLower() + " settings" ), false, () => {
                Settings.MainSettingsEnabler_Window.Select<Settings.HE_Window>();
            } );
        }
        //static Type[] lastTypes;


        internal override void OnEnable()
        {
            base.OnEnable();
            if ( Root.p == null || Root.p.Length == 0 || Root.p[ 0 ] == null ) return;
            AssignContent();
        }

        void AssignContent()
        {
            controller.ASIGN_CONTENT( NAME, "HIER_EXPAND_ICON", this );
        }

        static ExternalDrawContainer lastController;
        internal override void OnGUI_Draw()
        {
            if ( !Root.p[ 0 ].par_e.USE_HIER_EXPANDED_MOD )
            {
                Close();
                return;
            }

            lastController = controller;
            adapter.ChangeGUI();
           /* controller.type = cType;*/
            controller.tempRoot = this;
            instance.DoHier( new Rect( 0, 0, position( IExternalWindowType.EXPAND ).width, position( IExternalWindowType.EXPAND ).height ), controller, adapter.LastActiveScene );
            adapter.RestoreGUI();
        }

        internal static void ManualDraw( ref Rect r, HierarchyExpandedMemInstance instance, ExternalDrawContainer controller, IExternalWindow w )
        {
            if ( !Root.p[ 0 ].par_e.USE_HIER_EXPANDED_MOD )
            {
                return;
            }

            lastController = controller;
            Root.p[ 0 ].ChangeGUI();
            /*controller.type = cType;*/
            controller.tempRoot = w;
            instance.DoHier( r, controller, Root.p[ 0 ].LastActiveScene );
            Root.p[ 0 ].RestoreGUI();
        }


    }
}
