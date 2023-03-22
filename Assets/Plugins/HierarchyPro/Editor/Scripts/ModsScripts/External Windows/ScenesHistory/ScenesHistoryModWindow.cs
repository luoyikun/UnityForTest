using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace EMX.HierarchyPlugin.Editor.Mods
{

	class ScenesHistoryModWindow : ExternalModRoot
    {

        internal override int pluginID { get { return 0; } }
        ScenesHistoryModInstance __instance;
        internal ScenesHistoryModInstance instance { get { return __instance ?? (__instance = new ScenesHistoryModInstance()); } }
        internal const string NAME = "Scenes History";
        const int priority = 10;
        //static MemType cType = MemType.Scenes;


        internal static void SubscribeEditorInstanceStatic( EditorSubscriber sbs )
        {
            if ( !Root.p[ 0 ].par_e.USE_LAST_SCENES_MOD )
            {
                foreach ( var item in Root.p[ 0 ].modsController.activeExternalMods.ToList() )
                    if ( item && item is ScenesHistoryModWindow ) item.Close();
                return;
            }



            //if ( !Root.p[ 0 ].par_e.USE_LAST_SCENES_MOD ) return;

            /*	sbs.OnSceneOpening += instance.SCENE_CHANGE;
				sbs.OnSelectionChanged += instance.CHANGE_SELECTION;
				sbs.OnPlayModeStateChanged += instance.CHANGEPLAYMODE;
				sbs.OnUpdate += instance.Update;*/

            sbs.ExternalMod_Buttons.Add( new ExternalMod_Button( typeof( ScenesHistoryModWindow ) ) {
                text = NAME,
                icon = () => "LAST_SCENES_ICON",
                enabled = Root.p[ 0 ].par_e.DRAW_TOPBAR_H4,
                priority = Root.p[ 0 ].par_e.ORDER_TOPBAR_H4,
                release = ICON_CLICK,
                menuGen = MENU_GEN
            } );

            sbs.ExternalMod_MenuItems.Add( new ExternalMod_MenuItem() {
                text = NAME,
                path = "- Open " + NAME.ToLower() + " in window",
                priority = priority,
                release = ICON_CLICK,
            } );


			ScenesHistoryModInstance.SubscribeEditorInstanceStaticOnSceneChaging( sbs );


		}

		internal override void SubscribeEditorInstance( AdditionalSubscriber sbs )
        {
        }




        internal override void OnEnable()
        {
            base.OnEnable();
            if ( Root.p == null || Root.p.Length == 0 || Root.p[ 0 ] == null ) return;
            AssignContent();
        }

        void AssignContent()
        {
            controller.ASIGN_CONTENT( NAME, "LAST_SCENES_ICON", this );
        }


   



        static void ICON_CLICK( int button, string name )
        {
            if ( button == 0 )
            {
                //controller = ;
                //if (W.minSize.x < 40 || W.minSize.y < 16) {W.minSize = new Vector2(40, 16); }
                //	W.ShowTab();
                //var W = Root.p[0].par_e.ATTACH_TO_INSPECT_ONOPEN ? LastScenesHistoryModWindow.GetWindow<LastScenesHistoryModWindow>(name, true, InspectorType) : LastScenesHistoryModWindow.GetWindow<LastScenesHistoryModWindow>(name, true);
                /*var W = LastScenesHistoryModWindow.GetWindow<LastScenesHistoryModWindow>(name, true);
				W.Show();
				W.Init();*/
                GetExternalWindow<ScenesHistoryModWindow>.Show( name, GameObjectsSelectionHistoryModWindow.bindTypes );
            }
            if ( button == 1 )
            {
                var menu = new GenericMenu();
                MENU_GEN( menu, name, lastController );
                menu.ShowAsContext();
            }
        }
        static void MENU_GEN( GenericMenu menu, string name , ExternalDrawContainer lastController)
        {
            menu.AddItem( new GUIContent( "- Open " + NAME.ToLower() + " in window" ), false, () => {
                GetExternalWindow<ScenesHistoryModWindow>.Show( name, GameObjectsSelectionHistoryModWindow.bindTypes );
                /*var W = LastScenesHistoryModWindow.GetWindow<LastScenesHistoryModWindow>(name, true);
                W.Show();
                W.Init();*/
            } );
            menu.AddSeparator( "" );
            DrawButtonsOld.SET_SCEN( menu, lastController ?? new ExternalDrawContainer( 0 ) { /*type = cType*/ }, EditorSceneManager.GetActiveScene() );
            menu.AddSeparator( "" );
            menu.AddItem( new GUIContent( "- Open " + NAME.ToLower() + " settings" ), false, () => {
                Settings.MainSettingsEnabler_Window.Select<Settings.LS_Window>();
            } );
        }

        //static Type[] lastTypes;
        static ExternalDrawContainer lastController;


        internal override void OnGUI_Draw()
        {
            if ( !Root.p[ 0 ].par_e.USE_LAST_SCENES_MOD )
            {
                Close();
                return;
            }

            lastController = controller;
            adapter.ChangeGUI();
           /* controller.type = cType;*/
            controller.tempRoot = this;
            instance.DoScenes( new Rect( 0, 0, position( IExternalWindowType.SCENE ).width, position( IExternalWindowType.SCENE ).height ), controller, adapter.LastActiveScene );
            adapter.RestoreGUI();
        }


        internal static void ManualDraw( ref Rect r, ScenesHistoryModInstance instance, ExternalDrawContainer controller, IExternalWindow w )
        {
            if ( !Root.p[ 0 ].par_e.USE_LAST_SCENES_MOD )
            {
                return;
            }

            lastController = controller;
            Root.p[ 0 ].ChangeGUI();
            /*controller.type = cType;*/
            controller.tempRoot = w;
            instance.DoScenes( r, controller, Root.p[ 0 ].LastActiveScene );
            Root.p[ 0 ].RestoreGUI();
        }
    }
}
