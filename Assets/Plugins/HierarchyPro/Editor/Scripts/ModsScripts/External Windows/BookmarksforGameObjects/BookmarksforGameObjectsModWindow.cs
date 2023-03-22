using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace EMX.HierarchyPlugin.Editor.Mods.BookObject
{

	class BookmarksforGameObjectsModWindow : ExternalModRoot, IHasCustomMenu
    {
        internal override int pluginID { get { return 0; } }
        BookmarksforGameObjectsModInstance __instance;
        internal BookmarksforGameObjectsModInstance instance { get { return __instance ?? (__instance = new BookmarksforGameObjectsModInstance()); } }
        internal const string NAME = "Bookmarks";
        const int priority = 0;
        //static MemType cType = MemType.Custom;
        // internal static void SubscribeButtonsAndMenu( EditorSubscriber sbs )
        // {
        //     if ( !Root.p[ 0 ].par_e.USE_BOOKMARKS_HIERARCHY_MOD ) return;
        //
        //
        //
        // }

        internal static void SubscribeEditorInstanceStatic( EditorSubscriber sbs )
        {
            if ( !Root.p[ 0 ].par_e.USE_BOOKMARKS_HIERARCHY_MOD )
            {
                foreach ( var item in Root.p[ 0 ].modsController.activeExternalMods.ToList() )
                    if ( item && item is BookmarksforGameObjectsModWindow ) item.Close();
                return;
            }
            /*	sbs.OnSceneOpening += instance.SCENE_CHANGE;
				sbs.OnSelectionChanged += instance.CHANGE_SELECTION;
				sbs.OnPlayModeStateChanged += instance.CHANGEPLAYMODE;
				sbs.OnUpdate += instance.Update;*/


            sbs.ExternalMod_Buttons.Add( new ExternalMod_Button( typeof( BookmarksforGameObjectsModWindow ) ) {
                text = NAME,
                icon = () => "BOOKMARKS_ICON",
                priority = Root.p[ 0 ].par_e.ORDER_TOPBAR_H3,
                enabled = Root.p[0].par_e.DRAW_TOPBAR_H3,
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
        }
       

        void IHasCustomMenu.AddItemsToMenu( GenericMenu menu )
        {
            generate_menu( menu, NAME , controller);
        }

        public static void OpenWindow()
        {
            GetExternalWindow<BookmarksforGameObjectsModWindow>.TryToClose_Or_Show( NAME );
        }

        static void ICON_CLICK( int button, string name )
        {
            if ( button == 0 )
            {
                //controller = ;
                //if (W.minSize.x < 40 || W.minSize.y < 16) {W.minSize = new Vector2(40, 16); }
                //	W.ShowTab();
                //var W = Root.p[0].par_e.ATTACH_TO_INSPECT_ONOPEN ? LastScenesHistoryModWindow.GetWindow<LastScenesHistoryModWindow>(name, true, InspectorType) : LastScenesHistoryModWindow.GetWindow<LastScenesHistoryModWindow>(name, true);
                GetExternalWindow<BookmarksforGameObjectsModWindow>.Show( NAME );
            }
            if ( button == 1 )
            {
                var menu = new GenericMenu();
                MENU_GEN( menu, name,  lastController);
                menu.ShowAsContext();
            }
        }
        static void MENU_GEN( GenericMenu menu, string name, ExternalDrawContainer c )
        {
            menu.AddItem( new GUIContent( "- Open " + NAME.ToLower() + " in window" ), false, () => {
                GetExternalWindow<BookmarksforGameObjectsModWindow>.Show( name );
            } );
            menu.AddSeparator( "" );
            generate_menu( menu, NAME , c ?? lastController);
        }


        static void generate_menu( GenericMenu menu, string name, ExternalDrawContainer lastController )
        {
            DrawButtonsOld.SET_BOOK( menu, lastController ?? (lastController = new ExternalDrawContainer( 0 ) { /*type = cType*/ }), EditorSceneManager.GetActiveScene(),
                GetStaticInstance );
            menu.AddSeparator( "" );
            menu.AddItem( new GUIContent( "- Open " + NAME.ToLower() + " settings" ), false, () => {
                Settings.MainSettingsEnabler_Window.Select<Settings.BO_Window>();
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
            controller.ASIGN_CONTENT( controller.GetCurerentCategoryName( adapter.LastActiveScene ), "BOOKMARKS_ICON" ,this);
        }


        internal static BookmarksforGameObjectsModInstance GetStaticInstance { get { return lastInstance ?? (lastInstance = new BookmarksforGameObjectsModInstance()); } }

        internal  static BookmarksforGameObjectsModInstance lastInstance;
        internal  static ExternalDrawContainer lastController;

        internal override void OnGUI_Draw()
        {

            if ( !Root.p[ 0 ].par_e.USE_BOOKMARKS_HIERARCHY_MOD )
            {
                Close();
                return;
            }

            lastInstance = instance;
            lastController = controller;
            adapter.ChangeGUI();
           /* controller.type = cType;*/
            controller.tempRoot = this;
            controller.CHECK_CONTENT( controller.GetCurerentCategoryName( adapter.LastActiveScene ), "BOOKMARKS_ICON", this );
            //UpdateWindowsContent();
            instance.DoCustom( new Rect( 0, 0, position( IExternalWindowType.CUSTOM ).width, position( IExternalWindowType.CUSTOM ).height ), controller, adapter.LastActiveScene );
            adapter.RestoreGUI();
        }

        internal static void ManualDraw( ref Rect r, BookmarksforGameObjectsModInstance instance, ExternalDrawContainer controller, IExternalWindow w )
        {
            if ( !Root.p[ 0 ].par_e.USE_BOOKMARKS_HIERARCHY_MOD )
            {
                return;
            }

            lastInstance = instance;
            lastController = controller;
            Root.p[ 0 ].ChangeGUI();
            /*controller.type = cType;*/
            controller.tempRoot = w;
            instance.DoCustom( r, controller, Root.p[ 0 ].LastActiveScene );
            Root.p[ 0 ].RestoreGUI();
        }
    }
}
