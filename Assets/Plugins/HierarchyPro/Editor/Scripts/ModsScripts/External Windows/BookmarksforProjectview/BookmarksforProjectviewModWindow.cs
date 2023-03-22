using System.Linq;
using UnityEngine;
using UnityEditor;



namespace EMX.HierarchyPlugin.Editor.Mods.BookProject
{

	class BookmarksforProjectviewModWindow : ExternalModRoot, IHasCustomMenu
	{
		internal override int pluginID { get { return 0; } }
		BookmarksforProjectviewModInstance __instance;
		internal BookmarksforProjectviewModInstance instance { get { return __instance ?? (__instance = new BookmarksforProjectviewModInstance()); } }
		const string NAME = "Project Folders";
		const int priority = 50;
	

		internal static void SubscribeEditorInstanceStatic(EditorSubscriber sbs )
        {
            if ( !Root.p[ 0 ].par_e.USE_BOOKMARKS_PROJECT_MOD )
            {
                foreach ( var item in Root.p[ 0 ].modsController.activeExternalMods.ToList() )
                    if ( item && item is BookmarksforProjectviewModWindow ) item.Close();
                return;
            }

            sbs.ExternalMod_Buttons.Add( new ExternalMod_Button( typeof( BookmarksforProjectviewModWindow ) ) {
                text = NAME,
                icon = () => "PROJECT_FOLDERS_ICON",
                enabled = Root.p[0].par_e.DRAW_TOPBAR_H2,
                priority = Root.p[ 0 ].par_e.ORDER_TOPBAR_H2,
                release = ICON_CLICK,
                menuGen = MENU_GEN,
				//additionObject = instance
            } );

            sbs.ExternalMod_MenuItems.Add( new ExternalMod_MenuItem() {
                text = NAME,
                path = "- Open " + NAME.ToLower() + " in window",
                priority = priority,
                release = ICON_CLICK,
            } );
            //sbs.ExternalMod_MenuItems
        }
      
        //static Type[] lastTypes;
        internal override void SubscribeEditorInstance(AdditionalSubscriber sbs)
		{
			FORCE_REPAINT_THROUGH_LAYOUT = true;
			instance.SubscribeEditorInstance(  sbs );
		}
       	/*	sbs.OnSceneOpening += instance.SCENE_CHANGE;
				sbs.OnSelectionChanged += instance.CHANGE_SELECTION;
				sbs.OnPlayModeStateChanged += instance.CHANGEPLAYMODE;
				sbs.OnUpdate += instance.Update;*/


		public static void OpenWindow()
		{
			GetExternalWindow<BookmarksforProjectviewModWindow>.TryToClose_Or_Show(NAME);
		}
		static void ICON_CLICK(int button, string name)
		{
			if (button == 0)
			{
				//controller = ;
				//if (W.minSize.x < 40 || W.minSize.y < 16) {W.minSize = new Vector2(40, 16); }
				//	W.ShowTab();
				//var W = Root.p[0].par_e.ATTACH_TO_INSPECT_ONOPEN ? LastScenesHistoryModWindow.GetWindow<LastScenesHistoryModWindow>(name, true, InspectorType) : LastScenesHistoryModWindow.GetWindow<LastScenesHistoryModWindow>(name, true);
				/*var W = BookmarksforProjectviewModWindow.GetWindow<BookmarksforProjectviewModWindow>(name, true);
				W.Show();
				W.Init();*/
				GetExternalWindow<BookmarksforProjectviewModWindow>.Show(name);

			}
			if (button == 1)
			{
				var menu = new GenericMenu();
				MENU_GEN( menu, name, lastController );

				menu.ShowAsContext();
			}
		}
        static void MENU_GEN( GenericMenu menu, string name, ExternalDrawContainer lastController )
        {
            menu.AddItem(new GUIContent("- Open " + NAME.ToLower() + " in window"), false, () =>
				{
					GetExternalWindow<BookmarksforProjectviewModWindow>.Show(name);
					/*var W = BookmarksforProjectviewModWindow.GetWindow<BookmarksforProjectviewModWindow>(name, true);
					W.Show();
					W.Init();*/
				});
				menu.AddSeparator("");
				generate_menu(menu, name, lastController);
        }



        public void AddItemsToMenu(GenericMenu menu)
		{
			generate_menu(menu, NAME,  lastController);
		}
		static void generate_menu(GenericMenu menu, string name, ExternalDrawContainer lastController)
		{
			menu.AddItem(new GUIContent("- Open " + NAME.ToLower() + " settings"), false, () =>
			{
				Settings.MainSettingsEnabler_Window.Select<Settings.BF_Window>();
			});
		}
	

	

		float? oldHeight;
		float? oldWidth;

		bool mayscroll;

        internal override void OnEnable()
        {
            base.OnEnable();
            if ( Root.p == null || Root.p.Length == 0 || Root.p[ 0 ] == null ) return;
            AssignContent();
        }

        void AssignContent()
        {
            controller.ASIGN_CONTENT( NAME, "PROJECT_FOLDERS_ICON" ,this);
        }

        static ExternalDrawContainer lastController;

        internal override void OnGUI_Draw()
		{

			if (!Root.p[0].par_e.USE_BOOKMARKS_PROJECT_MOD)
			{
				Close();
				return;
			}


			/*if (WAS_INIT)
			{
				instance.CHECK_SCAN();
			}*/

			var check = controller as FavorControllerWindow;
			if (check == null) _wndowController = check = new FavorControllerWindow(0);
			lastController = _wndowController;

			if (Event.current.type == EventType.ScrollWheel && new Rect(0, 0, position( IExternalWindowType.PROJECT_FOLD ).width, position( IExternalWindowType.PROJECT_FOLD ).height).Contains(Event.current.mousePosition))
			{
				if (mayscroll)
				{
					//if (adapter.OnScroll != null) adapter.OnScroll(Adapter.ScrollType.HyperGraphScroll_Window, Event.current.delta.y);
					instance.ON_SCROLL(Event.current.delta.y);
					mayscroll = false;
				}
			}

			if (Event.current.type == EventType.Repaint)
			{
				mayscroll = true;
			}


			if (!oldHeight.HasValue) oldHeight = position( IExternalWindowType.PROJECT_FOLD ).height;
			if (oldHeight.Value != position( IExternalWindowType.PROJECT_FOLD ).height)
			{
				var oldH = oldHeight.Value;
				oldHeight = position( IExternalWindowType.PROJECT_FOLD ).height;
				//  controller.HEIGHT = (startHeight + (startPos.y - p.y));
				//  CHECK_HEIGHT();
				check.scrollPos.y -= (oldH - check.HEIGHT( IExternalWindowType.PROJECT_FOLD  )) / 2;
				// Hierarchy.BottomInterface.HyperGraph.RESET_SMOOTH_HEIGHT();
			}


			if (!oldWidth.HasValue) oldWidth = position( IExternalWindowType.PROJECT_FOLD ).width;
			if (oldWidth.Value != position( IExternalWindowType.PROJECT_FOLD ).width)
			{
				var oldW = oldWidth.Value;
				oldWidth = position( IExternalWindowType.PROJECT_FOLD ).width;
				//  controller.HEIGHT = (startHeight + (startPos.y - p.y));
				//  CHECK_HEIGHT();
				check.scrollPos.x -= (oldW - check.WIDTH( IExternalWindowType.PROJECT_FOLD  )) / 2;
				// Hierarchy.BottomInterface.HyperGraph.RESET_SMOOTH_HEIGHT();
			}




			/*	if (!wasInit)
				{
					if (Event.current.type == EventType.Repaint)
					{ // MonoBehaviour.print(hyperwindow.position);
						if (position.x < 15 && position.y < 50)
						{
							var p = position;
							p.x = WinBounds.MAX_WINDOW_WIDTH.x + (WinBounds.MAX_WINDOW_WIDTH.y - p.width) / 2;
							p.y = WinBounds.MAX_WINDOW_HEIGHT.x + (WinBounds.MAX_WINDOW_HEIGHT.y - p.height) / 2;
							position = p;
						}

						wasInit = true;
					}

					// return;
				}*/

			adapter.ChangeGUI();

			instance.EXTERNAL_HYPER_DRAWER(new Rect(0, 0, position( IExternalWindowType.PROJECT_FOLD ).width, position( IExternalWindowType.PROJECT_FOLD ).height), check, this);

			adapter.RestoreGUI();
		}
	}
}
