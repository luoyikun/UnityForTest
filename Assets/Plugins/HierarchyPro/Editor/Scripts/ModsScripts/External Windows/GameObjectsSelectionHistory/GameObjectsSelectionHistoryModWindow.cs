using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace EMX.HierarchyPlugin.Editor.Mods
{

	class GameObjectsSelectionHistoryModWindow : ExternalModRoot, IHasCustomMenu
	{

		internal override int pluginID { get { return 0; } }
		LastSelectionHistoryModInstance __instance;
		internal LastSelectionHistoryModInstance instance { get { return __instance ?? (__instance = new LastSelectionHistoryModInstance()); } }
		internal const string NAME = "Selection History";
		const int priority = 5;
		//static MemType cType = MemType.Last;


		internal static void SubscribeEditorInstanceStatic( EditorSubscriber sbs )
		{
			if ( !Root.p[ 0 ].par_e.USE_LAST_SELECTION_MOD )
			{
				foreach ( var item in Root.p[ 0 ].modsController.activeExternalMods.ToList() )
					if ( item && item is GameObjectsSelectionHistoryModWindow ) item.Close();
				return;
			}


			//if ( !Root.p[ 0 ].par_e.USE_LAST_SELECTION_MOD ) return;


			sbs.ExternalMod_Buttons.Add( new ExternalMod_Button( typeof( GameObjectsSelectionHistoryModWindow ) ) {
				text = NAME,
				icon = () => "LAST_SELECTION_ICON",
				enabled = Root.p[ 0 ].par_e.DRAW_TOPBAR_H5,
				priority = Root.p[ 0 ].par_e.ORDER_TOPBAR_H5,
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



		internal static void OnSelectionChangeStatic()
		{
			//if ( !Root.p[0].SkipRemove )
			{
				DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Last ).LastIndex = -1;
				var skip = Root.p[0].SkipSwitch && Root.p[0].SkipRemove;
				if ( !skip )
					DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Last ).LastSelectedRoot = -1;
				// DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Custom ).LastIndex = -1;
				// DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Custom ).LastSelectedRoot = -1;
			}
			//if ( DrawButtonsOld.SkipRemove )
			//{
			//    Root.p[ 0 ].PUSH_GUI_ONESHOT_FORCE_FIRSTONLY( 0, () => {
			//        DrawButtonsOld.SkipRemove = false;
			//    } );
			//}
		}

		void IHasCustomMenu.AddItemsToMenu( GenericMenu menu )
		{
			generate_menu( menu, NAME, lastController );
		}




		internal static Type[] bindTypes = { typeof(BookObject.BookmarksforGameObjectsModWindow) , typeof(GameObjectsSelectionHistoryModWindow) ,
			typeof(ScenesHistoryModWindow), typeof(HierarchyExpandedMemWindow)
			};
		static void ICON_CLICK( int button, string name )
		{
			if ( button == 0 )
			{
				//controller = ;
				//if (W.minSize.x < 40 || W.minSize.y < 16) {W.minSize = new Vector2(40, 16); }
				//	W.ShowTab();
				//var W = Root.p[0].par_e.ATTACH_TO_INSPECT_ONOPEN ? LastScenesHistoryModWindow.GetWindow<LastScenesHistoryModWindow>(name, true, InspectorType) : LastScenesHistoryModWindow.GetWindow<LastScenesHistoryModWindow>(name, true);

				GetExternalWindow<GameObjectsSelectionHistoryModWindow>.Show( name, bindTypes );
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
				GetExternalWindow<GameObjectsSelectionHistoryModWindow>.Show( name, bindTypes );
			} );
			menu.AddSeparator( "" );
			generate_menu( menu, name, lastController );
		}

		static void generate_menu( GenericMenu menu, string name, ExternalDrawContainer lastController )
		{
			DrawButtonsOld.SET_LAST( menu, lastController ?? new ExternalDrawContainer( 0 ) { /*type = cType*/ }, EditorSceneManager.GetActiveScene() );
			menu.AddSeparator( "" );
			menu.AddItem( new GUIContent( "- Open " + NAME.ToLower() + " settings" ), false, () => {
				Settings.MainSettingsEnabler_Window.Select<Settings.LO_Window>();
			} );
		}
		internal static ExternalDrawContainer lastController;
		//static Type[] lastTypes;

		// ExternalDrawContainer controller = new ExternalDrawContainer();

		internal override void OnEnable()
		{
			base.OnEnable();
			if ( Root.p == null || Root.p.Length == 0 || Root.p[ 0 ] == null ) return;
			AssignContent();
		}

		void AssignContent()
		{
			controller.ASIGN_CONTENT( NAME, "LAST_SELECTION_ICON", this );
		}

		internal override void OnGUI_Draw()
		{


			if ( !Root.p[ 0 ].par_e.USE_LAST_SELECTION_MOD )
			{
				Close();
				return;
			}

			lastController = controller;
			adapter.ChangeGUI();
			/*controller.type = cType;*/
			controller.tempRoot = this;
			instance.DoLast( new Rect( 0, 0, position( IExternalWindowType.LAST ).width, position( IExternalWindowType.LAST ).height ), controller, adapter.LastActiveScene );
			adapter.RestoreGUI();

		}



		internal static void ManualDraw( ref Rect r, LastSelectionHistoryModInstance instance, ExternalDrawContainer controller, IExternalWindow w )
		{
			if ( !Root.p[ 0 ].par_e.USE_LAST_SELECTION_MOD )
			{
				return;
			}

			lastController = controller;
			Root.p[ 0 ].ChangeGUI();
			/*controller.type = cType;*/
			controller.tempRoot = w;
			instance.DoLast( r, controller, Root.p[ 0 ].LastActiveScene );
			Root.p[ 0 ].RestoreGUI();
		}
	}
}
