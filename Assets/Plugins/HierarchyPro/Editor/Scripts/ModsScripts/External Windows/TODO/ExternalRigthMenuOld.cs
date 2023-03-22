using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using EMX.HierarchyPlugin.Editor.Mods.BookObject;

namespace EMX.HierarchyPlugin.Editor.Mods
{
	internal partial class DrawButtonsOld
	{




		internal static void SET_BOOK( GenericMenu menu, ExternalDrawContainer controller, Scene scene, BookObject.BookmarksforGameObjectsModInstance instance )
		{
			//GenericMenu menu = new GenericMenu();
			ADD_TO_MENU_LIST_OF_OBJECTS( MemType.Custom, ref menu, controller, scene );
			menu.AddSeparator( "" );
			SHOW_CATEGORY_MENU( menu, controller, instance, scene, false );
			//	menu.ShowAsContext();
		}
		internal static void SET_BOOK_WIHTOUT_OBJECTS( ExternalDrawContainer controller, Scene scene, BookObject.BookmarksforGameObjectsModInstance instance )
		{
			GenericMenu menu = new GenericMenu();
			SHOW_CATEGORY_MENU( menu, controller, instance, scene, false );
			menu.AddSeparator( "" );
			menu.AddItem( new GUIContent( "- Open " + BookmarksforGameObjectsModWindow.NAME.ToLower() + " settings" ), false, () => {
				Settings.MainSettingsEnabler_Window.Select<Settings.BO_Window>();
			} );
			menu.ShowAsContext();
		}
		/*
		internal static void SET_BOOK_2(BottomController controller, int scene)
		{
			GenericMenu menu = new GenericMenu();
			ADD_TO_MENU_LIST_OF_OBJECTS(MemType.Custom, ref menu);

			menu.AddSeparator("");

			menu.AddItem(GetContent(adapter.par.SHOW_BOOKMARKS_ROWS, "Bookmarks"), false, () =>
			{
				adapter.par.SHOW_BOOKMARKS_ROWS = !(bool)adapter.par.SHOW_BOOKMARKS_ROWS;
				adapter.SavePrefs();
			});

			menu.AddSeparator("");

			CREATE_BUTTON_CUSTOM_MENU(controller, scene, false, menu);
			menu.ShowAsContext();
		}*/

		internal static void SET_LAST( GenericMenu menu, ExternalDrawContainer controller, Scene scene )
		{


			/*menu.AddItem(GetContent(adapter.par.SHOW_LAST_ROWS, "Last Selections"), false, () =>
			//   menu.AddItem(new GUIContent("Enable Last bottom GUI"), adapter.par.SHOW_LAST_ROWS, () =>
			{
				adapter.par.SHOW_LAST_ROWS = !(bool)value;
				adapter.SavePrefs();
			});*/

			/*menu.AddSeparator("");

			menu.AddItem(new GUIContent("Open in New Tab"), false, () => { _6__BottomWindow_BottomInterfaceWindow.ShowW(adapter, _6__BottomWindow_BottomInterfaceWindow.TYPE.LAST, "Last Selection"); });
		*/    //menu.AddSeparator("");


			ADD_TO_MENU_LIST_OF_OBJECTS( MemType.Last, ref menu, controller, scene );


		}



		internal static void Add_HierExpands( ExternalDrawContainer controller, Scene scene )
		{

			GameObject[] snapShot_hierarchy = Tools.CREATE_EXPAND_GO_SNAPSHOT(scene.GetHashCode());
			//	string[] snapShot_project = adapter.IS_PROJECT() ? CREATE_EXPAND_GO_SNAPSHOT_FORPROJECT() : new string[0];
			//	string[] paths = snapShot_project.Select(AssetDatabase.GUIDToAssetPath).ToArray();
			HierarchyTempSceneDataGetter.TryToInitBookOrExpand( SaverType.SceneHierarchyExands, scene );

			SHOW_STRING( "Hierarchy expanded objects", "Expanded " + (HierarchyTempSceneData.InstanceFast( scene ).HierExpands_Temp.Count + 1), ( value ) => {
				if ( string.IsNullOrEmpty( value ) ) return;

				HierarchyTempSceneDataGetter.SetUndoListStart( "Apply hierarchy snapshot create" );
				HierarchyTempSceneDataGetter.SetUndoList( scene, HierarchyTempSceneDataGetter.UNDO_TYPE.EXPAND );


				HierarchyTempSceneData.InstanceFast( scene ).HierExpands_Temp.Add( new HierExpands_Temp() {
					name = value,
					targets = snapShot_hierarchy.ToList()
				} );


				HierarchyTempSceneDataGetter.SaveBookOrExpand( SaverType.SceneHierarchyExands, scene );
				//adapter.MarkSceneDirty(scene);
				adapter.RepaintExternalNow();

				HierarchyTempSceneDataGetter.SetDirtyList();

				//var d = adapter.MOI.des(scene);
				//var ier = d.HierarchyCache();

				//UniversalAddAndRefresh(ref ier, new HierarchySnapShotArray() { array = snapShot_hierarchy, GUIDarray = snapShot_project, PATHarray = paths, name = value }, scene);
			}, controller );
			controller.ClearAction();
		}



		internal static void SET_HIER( GenericMenu menu, ExternalDrawContainer controller, Scene scene )
		{

			/*menu.AddItem(GetContent(adapter.par.SHOW_HIERARCHYSLOTS_ROWS, "Expanded Items"), false, () =>
			//  menu.AddItem(new GUIContent("Enable HIerarchy Botom GUI"), adapter.par.SHOW_HIERARCHYSLOTS_ROWS, () =>
			{
				adapter.par.SHOW_HIERARCHYSLOTS_ROWS = !(bool)value;
				adapter.SavePrefs();
			});


			menu.AddSeparator("");*/


			ADD_TO_MENU_LIST_OF_OBJECTS( MemType.Hier, ref menu, controller, scene );

			menu.AddSeparator( "" );

			menu.AddItem( new GUIContent( "Collapse all" ), false, () => { Tools.SET_EXPAND_NULL( scene ); } );
			menu.AddSeparator( "" );

			menu.AddItem( new GUIContent( "+ Save current states" ), false, () => { Add_HierExpands( controller, scene ); } );

		}



		internal static void Add_Scenes( ExternalDrawContainer controller, Scene scene )
		{

			//	if (scene == -1) scene = SceneManager.GetActiveScene().GetHashCode();
			//var all = new object[SceneManager.sceneCount].Select((s, i) => SceneManager.GetSceneAt(i)).Where(s => s.GetHashCode() != scene).ToArray();
			ScenesHistoryModInstance.AddSceneButton_OnSceneChanging( true );
			//adapter.bottomInterface.Scene_SetDityMemory((scene), all, true);
			//RefreshMemCache(asd ?? adapter.GET_ACTIVE_SCENE);
			controller.ClearAction();
		}



		internal static void SET_SCEN( GenericMenu menu, ExternalDrawContainer controller, Scene scene )
		{


			/*menu.AddItem(GetContent(adapter.par.SHOW_SCENES_ROWS, "Scenes"), false, () =>
			//  menu.AddItem(new GUIContent("Enable Scenes Botom GUI"), adapter.par.SHOW_SCENES_ROWS, () =>
			{
				adapter.par.SHOW_SCENES_ROWS = !(bool)value;
				adapter.SavePrefs();
			});

			menu.AddSeparator("");*/


			ADD_TO_MENU_LIST_OF_OBJECTS( MemType.Scenes, ref menu, controller, scene );


			menu.AddSeparator( "" );

			menu.AddItem( new GUIContent( "+ Add all opened scenes" ), false, () => { Add_Scenes( controller, scene ); } );


		}


		struct EstimItems
		{
			public GUIContent content;
			public bool active;
			public GenericMenu.MenuFunction onClick;
		}
		static List<BookMarkCategory_Temp> __list;
		internal Rect DRAW_BG_LINE;
		internal Color DRAW_BG_COLOR;
		public bool DRAW_BG_Y;

		internal static void ADD_TO_MENU_LIST_OF_OBJECTS( MemType type, ref GenericMenu menu, ExternalDrawContainer controller, Scene scene )
		{

			menu.allowDuplicateNames = true;

			bool was = false;
			if ( type == MemType.Custom ) __list = GET_BOOKMARKS( scene );
			var INV = type == MemType.Last;
			var FP = adapter.par_e.SCENE_HISTORY_DRAW_FULLPATH_FOR_MENU;


			for ( int __c = 0; __c < (type == MemType.Custom ? __list.Count : 1); __c++ )
			{

				var memoryRoot = GET_OBJECTS_LIST(type, controller, scene, __c);


				var duplicated = memoryRoot.GroupBy( m => m.ToString() ).Where(c=>c.Count() > 1).ToDictionary( k => k.Key, v => false );

				//	var memoryRoot = m_memCache[__c];
				var interator = 0;

				var itemscount = memoryRoot.Count(t => t.IsValid());

				if ( itemscount <= 0 )
				{
					if ( type == MemType.Custom )
					{
						menu.AddDisabledItem( new GUIContent( "Category - " + __list[ __c ].category_name + "/" + "no items" ) );
					}

					continue;
				}

				//var rowClass = GET_DISPLAY_PARAMS(type).MaxItems;  //type == MemType.Custom ? GetRowClass(type).MaxItems : int.MaxValue;
				//if (itemscount > rowClass) itemscount = rowClass;
				var _scene = scene;

				if ( type == MemType.Last )
					for ( int asd = 0; asd < memoryRoot.Count; asd++ )
					{
						if ( !was_draw_dic_B.ContainsKey( memoryRoot[ asd ].unique_id ) ) was_draw_dic_B.Add( memoryRoot[ asd ].unique_id, false );
						was_draw_dic_B[ memoryRoot[ asd ].unique_id ] = false;
					}

				List<EstimItems> result = new List<EstimItems>();

				int __i = -1;
				if ( type == MemType.Last ) __i = memoryRoot.Count;

				for ( ; (type == MemType.Last ? __i >= 0 : __i < memoryRoot.Count) && interator < itemscount; )
				{

					if ( type == MemType.Last ) __i--; else __i++;


					var i = __i;
					if ( !memoryRoot[ i ].IsValid() || !memoryRoot[ i ].IsActive() ) continue;
					if ( type == MemType.Last )
						if ( was_draw_dic_B.ContainsKey( memoryRoot[ i ].unique_id ) && was_draw_dic_B[ memoryRoot[ i ].unique_id ] ) continue;
					if ( type == MemType.Last )
						if ( was_draw_dic_B.ContainsKey( memoryRoot[ i ].unique_id ) ) was_draw_dic_B[ memoryRoot[ i ].unique_id ] = true;


					var h = type == MemType.Last || type == MemType.Custom ? memoryRoot[i].gos_get_first() : null;

					//if ((type == MemType.Last || type == MemType.Custom) && (h == null || !h.Validate())) continue;
					++interator;
					string content = memoryRoot[i].ToString();

					if ( type == MemType.Scenes )
					{
						if ( FP ) content = memoryRoot[ i ].FullString();
						else if ( duplicated.ContainsKey( content ) ) content = memoryRoot[ i ].FullString();
					}
					else
					{
						content = memoryRoot[ i ].FullString();
					}

					content = content.Replace( "\n", " + " ).Replace( '/', '\\' );

					if ( type == MemType.Custom ) content = "Category - " + __list[ __c ].category_name + "/" + content;

					var count = memoryRoot[i] is SceneMemory ? memoryRoot[i].additional_GUID.Length : memoryRoot[i].gos_get().Length;
					if ( count > 1 ) content = content + "   " + "... [ " + count + " ]";

					if ( memoryRoot[ i ] is SceneMemory )
					{
						if ( ((SceneMemory)memoryRoot[ i ]).pin ) content = "● " + content;
						else content = "   " + content;
					}


					//                         if ( i == 0 ) {
					//
					//                             Debug.Log( memoryRoot[i].IsSelectedHadrScan() );
					//                             Debug.Log( memoryRoot[i].InstanceID.list.Count );
					//                             Debug.Log( adapter.IsSelected( memoryRoot[i].InstanceID.list[0].GetInstanceID() ) );
					//                             Debug.Log( adapter.selMax );
					//                         }

					result.Add( new EstimItems() {
						content = new GUIContent( content ),
						active = type == MemType.Scenes ? memoryRoot[ i ].GuidEquals() : memoryRoot[ i ].IsSelectedHadrScan( controller ),
						onClick = () => { memoryRoot[ i ].OnClick( false, _scene.GetHashCode(), controller ); }
					} );


				}

				if ( INV ) result.Reverse();
				foreach ( var item in result )
				{
					menu.AddItem( item.content, item.active, item.onClick );
					was = true;
				}
			}

			if ( !was ) menu.AddDisabledItem( new GUIContent( "No items" ) );
		}










		internal static void AddFavCategory( List<BookMarkCategory_Temp> capture_list, int VAR_CAT_INDEX, Scene scene, ExternalDrawContainer controller )
		{
			SHOW_STRING( "New Category Name", capture_list[ VAR_CAT_INDEX ].category_name, ( value ) => {
				if ( string.IsNullOrEmpty( value ) ) return;
				//	adapter.bottomInterface.GET_BOOKMARKS(ref capture_list, scene);
				if ( capture_list.Any( b => b.category_name == value ) ) return;

				//	adapter.CreateUndoActiveDescription("New Category", scene);
				HierarchyTempSceneDataGetter.SetUndoListStart( "Apply bookmarks name" );
				HierarchyTempSceneDataGetter.SetUndoList( scene, HierarchyTempSceneDataGetter.UNDO_TYPE.BOOKMARK );

				var result = new BookMarkCategory_Temp()
				{
					category_name = value,
					bgColor =  ( EditorGUIUtility.isProSkin ) ? BookMarkCategory_Temp.BgOverrideColor_default_dark : BookMarkCategory_Temp.BgOverrideColor_default_light
					//array = new List<Int32List>(),
				};
				capture_list.Add( result );

				HierarchyTempSceneDataGetter.SaveBookOrExpand( SaverType.Bookmarks, scene );
				controller.SetCategoryIndex( capture_list.Count - 1, scene );

				//adapter.SetDirtyActiveDescription(scene);
				HierarchyTempSceneDataGetter.SetDirtyList();


				//adapter.bottomInterface.RefreshMemCache(scene);

				controller.RepaintNow();
			}, controller );
			controller.ClearAction();

		}
		/*internal static void AddAndRefreshCustom(UnityEngine.Object[] o, UnityEngine.Object ActiveGameObject, int CustomIndex, Scene scene)
		{
			if (Application.isPlaying || o.Length == 0) return;

			List<Int32List> targetList = CustomIndex == 0 ? adapter.MOI.des(scene).GetHash4() : adapter.MOI.des(scene).GetBookMarks()[CustomIndex].array;
			AddAndRefreshCustom(o, ActiveGameObject, targetList, scene);
		}*/

		internal static void AddAndRefreshCustom( UnityEngine.GameObject[] gos, ExternalDrawContainer controller, Scene scene )
		{
			if ( Application.isPlaying || gos.Length == 0 ) return;


			/*if (adapter.IS_HIERARCHY() && LastActiveScene.GetHashCode() != scene)
			{
				LastActiveScene = (scene);
			}*/



			//adapter.SET_UNDO(adapter.MOI.des(scene), "Add Custom Selection Button");
			HierarchyTempSceneDataGetter.TryToInitBookOrExpand( SaverType.Bookmarks, scene );
			var temp_sc = HierarchyTempSceneData.InstanceFast(scene);



			var newObject = new GameObjectMemory(gos, -1, controller.GetCategoryIndex(scene), temp_sc , MemType.Custom);

			var targetList = DrawButtonsOld.GET_OBJECTS_LIST(MemType.Custom, controller, scene);


			var findIndex = targetList.FindIndex(l => l.unique_id == newObject.unique_id);
			if ( findIndex != -1 ) targetList.RemoveAt( findIndex );


			/*var target = new Int32List() { };
			if (adapter.IS_HIERARCHY()) SET_INT32_AS_HIERARCHY(target, o.Select(d => d.GetInstanceID()).ToArray(), ActiveGameObject.GetInstanceID());
			else SET_INT32_AS_PROJECT(target, o.Select(d => d.GetInstanceID()).ToArray(), ActiveGameObject.GetInstanceID());
			*/


			if ( targetList.Count == 0 ) targetList.Add( newObject );
			else targetList.Insert( 0, newObject );



			while ( targetList.Count > 30 ) targetList.RemoveAt( 30 );



			SET_OBJECTS_LIST( targetList, MemType.Custom, controller, scene );

			//adapter.SetDirtyDescription(adapter.MOI.des(scene), scene);
			//adapter.MarkSceneDirty(scene);

			//RefreshMemCache(scene);

			controller.ClearAction();
			controller.RepaintNow();

			/*
			foreach (var w in adapter.bottomInterface.WindowController)
				if (w.REFERENCE_WINDOW)
					w.REFERENCE_WINDOW.Repaint();

			foreach (var w in adapter.bottomInterface.FavoritControllers)
				if (w.REFERENCE_WINDOW)
					w.REFERENCE_WINDOW.Repaint();*/
		}











		static internal List<BookMarkCategory_Temp> GET_BOOKMARKS( Scene scene ) //  var scene = EditorSceneManager.GetActiveScene();
		{


			HierarchyTempSceneDataGetter.TryToInitBookOrExpand( SaverType.Bookmarks, scene );
			return HierarchyTempSceneData.InstanceFast( scene ).BookMarkCategory_Temp;


			/*
			DrawButtonsOld.GET_OBJECTS_LIST(MemType.Custom,)
			var list = HierarchyCommonData.Instance().FoolderBookmarkList;
			if (list.Count == 0) // var b = adapter.MOI.des(scene).GetBookMarks();
			{
				list.Add(new FolderBookmark()
				{
					name = "Default",
					//BgColor = adapter.par_e.BOOKMARKS_FOLDER_DRAW_BG_COLOR ? adapter.par_e.BOOKMARKS_FOLDER_DEFULT_BG_COLOR : (Color?)null
				});
			}*/
			//	if (adapter.MOI.des(scene).GetHash4() == null) adapter.MOI.des(scene).SetHash4(new List<Int32List>());
			//	list[0].array = adapter.MOI.des(scene).GetHash4();

			/*for (int i = 0; i < list.Count; i++)
				CheckUniqueFavoritID(ref list[i].InstanceID);*/
		}


		internal static void SHOW_CATEGORY_MENU( GenericMenu menu, ExternalDrawContainer controller, BookObject.BookmarksforGameObjectsModInstance instance, Scene scene, bool disableSwither ) //  Debug.Log(curentIndex);
		{
			//	var VAR_CAT_INDEX = CAT_INDEX(scene);
			var VAR_CAT_INDEX = controller.GetCategoryIndex(scene);


			//	var list = GET_OBJECTS_LIST(MemType.Custom, controller, scene);
			var capture_list = GET_BOOKMARKS(scene);


			//menu.AddItem(new GUIContent("Open in New Tab"), false, () => { _6__BottomWindow_BottomInterfaceWindow.ShowW(adapter, _6__BottomWindow_BottomInterfaceWindow.TYPE.CUSTOM, list[controller.GetCategoryIndex(scene)].name); });




			// if ( adapter.par.SHOW_BOOKMARKS_ROWS && !adapter.par.BOTTOM_AUTOHIDE )
			{
				if ( !disableSwither )
				{
					menu.AddItem( new GUIContent( "1) 'Default'" ), VAR_CAT_INDEX == 0, () => { controller.SetCategoryIndex( 0, scene ); } );
					for ( int INDEX = 1; INDEX < capture_list.Count; INDEX++ )
					{
						var capptureI = INDEX;
						menu.AddItem( new GUIContent( (INDEX + 1) + ") '" + capture_list[ INDEX ].category_name + "'" ), VAR_CAT_INDEX == INDEX, () => { controller.SetCategoryIndex( capptureI, scene ); } );
					}

					if ( !Application.isPlaying )
					{
						menu.AddItem( new GUIContent( "+ Add Category" ), false, () => {
							adapter.PUSH_UPDATE_ONESHOT( 0, () => AddFavCategory( capture_list, VAR_CAT_INDEX, scene, controller ) );
						} );
					}

					if ( capture_list.Count > 1 ) menu.AddSeparator( "" );
				}
			}


			if ( VAR_CAT_INDEX == 0 || Application.isPlaying ) menu.AddDisabledItem( new GUIContent( "Rename '" + capture_list[ VAR_CAT_INDEX ].category_name + "'" ) );
			else
				menu.AddItem( new GUIContent( "Rename '" + capture_list[ VAR_CAT_INDEX ].category_name + "'" ), false, () => {
					if ( VAR_CAT_INDEX == 0 ) return;

					SHOW_STRING( "Rename", capture_list[ VAR_CAT_INDEX ].category_name, ( value ) => {
						if ( string.IsNullOrEmpty( value ) ) return;
						//	adapter.bottomInterface.GET_BOOKMARKS(ref capture_list, scene);
						if ( capture_list.Any( b => b.category_name == value ) ) return;


						//var oldValue = capture_list[VAR_CAT_INDEX].category_name;
						var newValue = value;

						/*foreach (var item in Resources.FindObjectsOfTypeAll<_6__BottomWindow_BottomInterfaceWindow1>())
						{
							var cat = ((_6__BottomWindow_BottomInterfaceWindow.BottomControllerWindow)item.current_controller).GetCurerentCategoryName();
							if (cat == oldValue)
							{
								Undo.RecordObject(item, "Rename");
								((_6__BottomWindow_BottomInterfaceWindow.BottomControllerWindow)item.current_controller).SetCurerentCategoryName(newValue);
								EditorUtility.SetDirty(item);
							}
						}*/

						//	adapter.CreateUndoActiveDescription("Rename", scene);

						if ( capture_list[ VAR_CAT_INDEX ].category_name != value )
						{
							HierarchyTempSceneDataGetter.SetUndoListStart( "Apply bookmarks name" );
							HierarchyTempSceneDataGetter.SetUndoList( scene, HierarchyTempSceneDataGetter.UNDO_TYPE.BOOKMARK );

							capture_list[ VAR_CAT_INDEX ].category_name = value;
							HierarchyTempSceneDataGetter.SaveBookOrExpand( SaverType.Bookmarks, scene );
							controller.SetCurerentCategoryName( scene, value );

							HierarchyTempSceneDataGetter.SetDirtyList();
						}

						///adapter.MarkSceneDirty(scene);
					}, controller );
				} );

			if ( !Application.isPlaying )
			{
				if ( VAR_CAT_INDEX == 0 || Application.isPlaying ) menu.AddDisabledItem( new GUIContent( "Remove '" + capture_list[ VAR_CAT_INDEX ].category_name + "'category" ) );
				else
					menu.AddItem( new GUIContent( "Remove '" + capture_list[ VAR_CAT_INDEX ].category_name + "' category" ), false, () => {
						if (
							EditorUtility.DisplayDialog( "Remove category?", "Are you sure?", "Yes", "Cancel" ) )
						{
							if ( VAR_CAT_INDEX <= 0 ) return;
							if ( VAR_CAT_INDEX >= capture_list.Count ) return;

							//adapter.CreateUndoActiveDescription("Remove Category", scene);


							/*var oldValue = capture_list[CAT_INDEX(scene)].name;
							foreach (var item in Resources.FindObjectsOfTypeAll<_6__BottomWindow_BottomInterfaceWindow1>())
							{
								var cat = ((_6__BottomWindow_BottomInterfaceWindow.BottomControllerWindow)item.current_controller).GetCurerentCategoryName();
								if (cat == oldValue)
								{
									item.Close();
								}
							}*/
							HierarchyTempSceneDataGetter.SetUndoListStart( "Apply Bookmarks Remove" );
							HierarchyTempSceneDataGetter.SetUndoList( scene, HierarchyTempSceneDataGetter.UNDO_TYPE.BOOKMARK );

							capture_list.RemoveAt( VAR_CAT_INDEX );
							// HierarchyTempSceneData.InstanceFast(scene).BookMarkCategory_Temp.RemoveAt(VAR_CAT_INDEX);


							HierarchyTempSceneDataGetter.SaveBookOrExpand( SaverType.Bookmarks, scene );
							controller.SetCategoryIndex( controller.GetCategoryIndex( scene ), scene );

							HierarchyTempSceneDataGetter.SetDirtyList();


							controller.RepaintNow();
						}
					} );
			}
			menu.AddSeparator( "" );


			if ( !Application.isPlaying )
			{

				var win = adapter.window;

				var pos = new MousePos(Event.current.mousePosition, MousePos.Type.ColorChanger_230_0, false, controller.adapter);
				//menu.AddSeparator("");
				menu.AddItem( new GUIContent( "Use categories background colors" ), adapter.par_e.BOOKMARKS_OB_DRAW_BG_COLOR, () => {
					adapter.par_e.BOOKMARKS_OB_DRAW_BG_COLOR = !adapter.par_e.BOOKMARKS_OB_DRAW_BG_COLOR;

				} );
				if ( !adapter.par_e.BOOKMARKS_OB_DRAW_BG_COLOR )
				{
					menu.AddDisabledItem( new GUIContent( "Open categories background colors window" ) );
				}
				else
				{
					menu.AddItem( new GUIContent( "Open categories background colors window" ), false, () => {
						adapter.PUSH_UPDATE_ONESHOT( 0, () => // var pos = InputData.WidnwoRect(controller.IS_MAIN, Event.current.mousePosition, 190, 68, controller.adapter);
						  {
							  CategoriesColorsWindowOld.InitForGameObjectsBookmarks( pos, win, scene );
						  } );


					} );
				}
			}


			if ( !adapter.par_e.USE_RIGHT_ALL_MODS ) menu.AddDisabledItem( new GUIContent( "Show Descriptions" ) );
			else
				//   if (disableSwither)
				menu.AddItem( new GUIContent( "Show descriptions" ), adapter.par_e.BOOKMARKS_OB_SHOWDESCRIPTS, () => {
					adapter.par_e.BOOKMARKS_OB_SHOWDESCRIPTS = !adapter.par_e.BOOKMARKS_OB_SHOWDESCRIPTS;
					adapter.RepaintExternalNow();
				} );

		}

	}
}
