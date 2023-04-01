using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor.Mods.BookProject
{

	/*
	class EmtyString : IStringSetter
	{
		public string GetString(int index, string adapter)
		{
			return null;
		}

		public void SetString(int index, string value, string adapter) { }
	}
	*/
	partial class BookmarksforProjectviewModInstance : ExternalModStyles
    {



        PluginInstance adapter { get { return Root.p[ 0 ]; } }

        internal void DRAG_PERFORMER_SCAN() { }
        internal Rect CAPTURE_CLIP_RECT;
        FavorControllerWindow CURRENT_CONTROLLER;
        ExternalModRoot CURRENT_WIN;
        Rect dragRect;



        internal void SubscribeEditorInstance( AdditionalSubscriber sbs )
        {
            sbs.OnAssetImport += OnAssetImport;
        }
        void OnAssetImport()
        {
            BookmarksforProjectviewModInstance.scanned_folder.Clear();
            //EMX_TODO disabled to performance
            //RepaintNow();
        }

        

        internal void EXTERNAL_HYPER_DRAWER( Rect lineRect, FavorControllerWindow controller, ExternalModRoot win )
        {


            controller.tempRoot = win;
            CURRENT_CONTROLLER = controller;
            CURRENT_WIN = win;

            INIT_STYLES();

            dragRect = lineRect;
            dragRect.height = 5;

            //DRAG(controller);

            lineRect.y += dragRect.height;
            lineRect.height -= dragRect.height;
            // if (Event.current.type != EventType.Repaint) INTERFACE( HierHyperController );

            if ( controller.MAIN ) CAPTURE_CLIP_RECT = lineRect;

            localRect = lineRect;
            localRect.x = 0;
            localRect.y = 0;
            GUI.BeginClip( lineRect );


            if ( Event.current.type != EventType.Repaint ) INTERFACE( localRect, adapter.par_e.BOOKMARKS_FOLDER_LINE_HEIGHT, controller );
            FAVORIT_GUI( localRect, controller );
            if ( Event.current.type == EventType.Repaint ) INTERFACE( localRect, adapter.par_e.BOOKMARKS_FOLDER_LINE_HEIGHT, controller );


            GUI.EndClip();


            //          SETUPROOT.ExampleDragDropGUI( lineRect, null, FAVOR_DRAG_VALIDATOR, FAVOR_DRAG_PERFORMER );

            // if (Event.current.type == EventType.Repaint) INTERFACE( HierHyperController );


            //base.EXTERNAL_HYPER_DRAWER( lineRect, HierHyperController );
        }



        void DrawTexture( Rect r, Texture2D t )
        {
            GUI.DrawTexture( r, t );

        }

        Rect localRect;
        internal Actions __actions = new Actions();
        internal Actions actions { get { return __actions ?? (__actions = new Actions()); } }


        void INTERFACE( Rect RECT, float LH, ExternalDrawContainer controller )
        {
            var ACTIVE_RECT = RECT;
            /* ACTIVE_RECT.height -= INTERFACE_SIZE;
			 ACTIVE_RECT.y += INTERFACE_SIZE;*/

            /** CLOASE **/
            var closeRect = ACTIVE_RECT;
            closeRect.width = 14;
            closeRect.height = 14;
            closeRect.x = RECT.width - closeRect.width;
            closeRect.y += (LH - 14) / 2;
            EditorGUIUtility.AddCursorRect( closeRect, MouseCursor.Link );

            GUI.Label( closeRect, HyperGraphClose_Content, DRAW_LABEL_STYLE );

            var EVENT_ID = idOffset - 10;
            if ( Event.current.type == EventType.Repaint )
            {
                //adapter.gl._DrawTexture(closeRect, HIPERUI_CLOSE);
                DrawTexture( closeRect, HIPERUI_CLOSE );
            }

            if ( closeRect.Contains( Event.current.mousePosition ) && Event.current.button == 0 &&
                Event.current.type == EventType.MouseDown )
            {
                Tools.EventUse();
                actions.ADD_ACTION( EVENT_ID, closeRect, contains => { return false; }, contains => {
                    if ( contains )
                    {
                        //if (!editorWindow) SWITCH_ACTIVE_SCAN(false);
                        //else if (WindowFavorController.REFERENCE_WINDOW) WindowFavorController.REFERENCE_WINDOW.Close();
                        if ( CURRENT_WIN ) CURRENT_WIN.Close();
                    }
                }, controller );
            }

            if ( actions.HOVER( EVENT_ID, closeRect, controller ) )
            {
                HIPERUI_BUTTONGLOW.Draw( closeRect, false, false, false, false );
            }

        }




        internal void SWITCH_ACTIVE_SCAN( bool? overrideActive )
        {
            /*	if (bottomInterface._FAV_HEIGHT == null)
					bottomInterface._FAV_HEIGHT = adapter.FAV_ENABLE() ? adapter.par.FavoritesNavigatorParams.HEIGHT : 0;
				adapter.CHECK_SMOOTH_HEIGHT();

				adapter.par.FavoritesNavigatorParams.ENABLE = overrideActive ?? !adapter.par.FavoritesNavigatorParams.ENABLE;*/
        }




        const int idOffset = 50000;

        // Texture icon;
        // Color? iconColor;
        Rect close_rect;



        /*  private void asd()
		  {
			Debug.Log( "ASD" + Selection.objects.Length );
		  }*/



        void SWAP( int arrayIndex, int next )
        {
            //List<Int32List> l1 = controller.GetCategoryIndex(scene) == 0 ? adapter.MOI.des(scene).GetHash4() : adapter.MOI.des(scene).GetBookMarks()[controller.GetCategoryIndex(scene)].array;
            HierarchyCommonData.Instance().SetUndo( "Apply Project Bookmarks" );
            var l1 = GET_BOOKMARKS();
            // var l1 = adapter.MOI.des(s).GetHash4();
            var b = l1[arrayIndex];
            l1.RemoveAt( arrayIndex );
            if ( next >= l1.Count ) l1.Add( b );
            else l1.Insert( next, b );

            HierarchyCommonData.Instance().SetDirty();
            //adapter.SetDirtyActiveDescription(scene);
        }

        struct MouseEventStruct
        {

            internal Rect LocalModuleRect;
            internal Rect WorldModuleRect;
            internal MousePos MousePosStruct;
            internal FavorControllerWindow controller;
            internal int itemIndex;
            internal int categoryIndex;
            internal BookSlot memoryRoot;
            internal UnityEngine.Object[] hierarchy_obect;
            internal int scene;
            internal int selectionState;
            internal BookSlot otherstring;
            internal object otherobject;

            public MouseEventStruct Clone()
            {
                var c = (MouseEventStruct)MemberwiseClone();
                c.otherstring = otherstring;
                c.otherobject = otherobject;
                c.hierarchy_obect = hierarchy_obect;
                c.memoryRoot = memoryRoot;
                c.controller = controller;
                return c;
            }

            internal MouseEventStruct Set_Get( BookSlot memoryRoot,
                UnityEngine.Object[] hierarchy_obect,
                int scene, int categoryIndex, int itemIndex, FavorControllerWindow controller, int selectionState )
            {
                this.selectionState = selectionState;
                this.memoryRoot = memoryRoot;
                this.hierarchy_obect = hierarchy_obect;
                this.scene = scene;
                this.categoryIndex = categoryIndex;
                this.itemIndex = itemIndex;
                this.controller = controller;
                this.controller.breaking = false;

                return this;
            }
        }

        MouseEventStruct mouseEventStruct;

        void ITEM_ON_DOWN( MouseEventStruct currentStruct )
        {
            if ( currentStruct.controller.currentAction == null ) return;
            if ( currentStruct.controller.breaking ) return;
            if ( currentStruct.memoryRoot != null ) currentStruct.memoryRoot.OnClick( false, (currentStruct.scene) );
            else
            {
                Selection.objects = currentStruct.hierarchy_obect;
            }
        }

        void ITEM_ON_DRAG( MouseEventStruct currentStruct )
        {
            if ( currentStruct.controller.breaking ) return;
            var controller = currentStruct.controller;

            var m = GUIUtility.GUIToScreenPoint(Event.current.mousePosition + Event.current.delta);


            var drag = !currentStruct.WorldModuleRect.Contains(m);
            /*  drag |= m.x < 3;
			  drag |= m.x > controller.WIDTH - 9;*/


            if ( drag && Event.current.type == EventType.MouseDrag && !Event.current.control && !Event.current.shift && !Event.current.alt )
            { /*  List<Int32List> targetList = null;
                
                        targetList = controller.GetCategoryIndex( scene ) == 0 ? adapter.MOI.des( scene ).GetHash4() : adapter.MOI.des( scene ).GetBookMarks()[controller.GetCategoryIndex( scene )].array;
                         Utilities.MoveFromTo( ref targetList, arrayIndex, backupArraIndex );
                         RefreshMemCache( scene );*/

                if ( currentStruct.memoryRoot != null )
                {
                    var result = currentStruct.memoryRoot.ConvertToObjects();// INT32_TOOBJECTASLISTCT(currentStruct.memoryRoot.InstanceID).Where(h => h).ToArray();

                    CustomDragData.SetDragData( result, MemType.Custom );
                    DragAndDrop.StartDrag( "Dragging Assets" );
                    // DragAndDrop.objectReferences =
                    //Debug.Log( result[0] );
                    DragAndDrop.SetGenericData( "FavI1", currentStruct.categoryIndex );
                    DragAndDrop.SetGenericData( "FavI2", currentStruct.itemIndex );
                    DragAndDrop.SetGenericData( "FavI3", currentStruct.itemIndex ); // currentStruct.memoryRoot.ArrayIndex
                }
                else // DragAndDrop.PrepareStartDrag();// reset data
                {
                    adapter.ha.InternalClearDrag();
                    DragAndDrop.objectReferences = currentStruct.hierarchy_obect.Where(g=>g).ToArray();
                    DragAndDrop.StartDrag( "Dragging Assets" );
                }

                DragAndDrop.SetGenericData( "FavI4", currentStruct.hierarchy_obect[ 0 ].GetInstanceID() );
                DragAndDrop.SetGenericData( adapter.pluginname, null );


                Tools.EventUse();
                controller.__ModuleRect = currentStruct.LocalModuleRect;

                currentStruct.controller.breaking = true;
                CURRENT_CONTROLLER.ClearAction();
                controller.RepaintNow();
            }
        }


        void MENUCALL( MouseEventStruct currentStruct )
        {
            var menu = new GenericMenu();

            SHOW_CATEGORY_MENU( currentStruct.controller, (currentStruct.scene), ( s ) => currentStruct.categoryIndex, _menu: menu );

            // adapter.bottomInterface.SET_BOOK_REF( ref menu );
            menu.AddSeparator( "" );
            if ( currentStruct.memoryRoot == null ) menu.AddDisabledItem( new GUIContent( "Remove" ) );
            else
            {
                menu.AddItem( new GUIContent( "Remove" ), false, () => { REMOVE_ON_UP( currentStruct ); } );
            }


			// if ( !Application.isPlaying )
			// {
			//
			//	var pos = new MousePos(Event.current.mousePosition, MousePos.Type.ColorChanger_230_0, false, controller.adapter);
			//
			//	menu.AddSeparator( "" );
			//     var controller = CURRENT_CONTROLLER;
			//     menu.AddItem( new GUIContent( "Open categories background colors window" ), false, () => {
			//         adapter.PUSH_GUI_ONESHOT( 1, () => // var pos = InputData.WidnwoRect(controller.IS_MAIN, Event.current.mousePosition, 190, 68, controller.adapter);
			//           {
			//               CategoriesColorsWindowOld.Init( pos,  -1 );
			//           } );
			//
			//
			//     } );
			// }

			menu.AddSeparator( "" );
			{
				var controller = CURRENT_CONTROLLER;
				var pos = new MousePos(Event.current.mousePosition, MousePos.Type.ColorChanger_230_0, false, controller.adapter);
                var win = adapter.window;
				menu.AddSeparator( "" );
				menu.AddItem( new GUIContent( "Use categories background colors" ), adapter.par_e.BOOKMARKS_FOLDER_DRAW_BG_COLOR, () => {
					adapter.par_e.BOOKMARKS_FOLDER_DRAW_BG_COLOR = !adapter.par_e.BOOKMARKS_FOLDER_DRAW_BG_COLOR;

				} );
				if ( !adapter.par_e.BOOKMARKS_FOLDER_DRAW_BG_COLOR )
				{
					menu.AddDisabledItem( new GUIContent( "Open categories background colors window" ) );
				}
				else
				{
					menu.AddItem( new GUIContent( "Open categories background colors window" ), false, () => {
						adapter.PUSH_UPDATE_ONESHOT( 1, () => // var pos = InputData.WidnwoRect(controller.IS_MAIN, Event.current.mousePosition, 190, 68, controller.adapter);
						 {
							 CategoriesColorsWindowOld.InitForProjectFolder( pos, win );
						 } );


					} );
				}

			}
			menu.AddSeparator( "" );

            menu.AddItem( new GUIContent( "Open project folders mod settings" ), false, () => {
                Settings.MainSettingsEnabler_Window.Select<Settings.BF_Window>();
            } );


            menu.ShowAsContext();
        }

        void DESCRIPTION_ON_UP( MouseEventStruct currentStruct )
        {
            if ( !currentStruct.hierarchy_obect[ 0 ] ) return;

            //CREATE_NEW_ESCRIPTION(adapter, mouseEventStruct.MousePosStruct, currentStruct.hierarchy_obect, -1, false);
            var c = currentStruct.controller;
            var mr = currentStruct.memoryRoot;
            Action<string> action = (t) =>
            {
                if (t == mr.description) return;
                HierarchyCommonData.Instance().SetUndo("Apply Project Bookmarks Description");
                mr.description = t;
                HierarchyCommonData.Instance().SetDirty();
                c.RepaintNow();
            };

            var adapter = Root.p[0];
            var pos = new MousePos(Event.current.mousePosition, MousePos.Type.Input_190_68, false, adapter);
            Windows.InputWindow.Init( pos, "Create Description", Root.p[ 0 ].firstWindow( 0 ), action, null, currentStruct.memoryRoot.description );
            currentStruct.controller.RepaintNow();
            /*	if (Event.current == null)
				{
					Root.p[0].PUSH_GUI_ONESHOT(result);
					Root.p[0].RepaintWindowInUpdate();
				}

				else
				{
					result();
				}*/
        }




        void DESCRIPTION_ON_UP_RIGHT( MouseEventStruct currentStruct )
        {
            /*	var fillter = currentStruct.memoryRoot != null ? GET_DESCRIPTION(currentStruct.memoryRoot).text : GET_DESCRIPTION(currentStruct.hierarchy_obect).text;
				if (string.IsNullOrEmpty(fillter)) return;


				// var pos = new MousePos( Event.current.mousePosition, MousePos.Type.ColorChanger_230_0, controller.IS_MAIN, adapter);
				currentStruct.MousePosStruct.type = MousePos.Type.Search_356_0;

				_W__SearchWindow.Init(currentStruct.MousePosStruct, adapter.DescriptionModule.SearchHelper, fillter,
					adapter.DescriptionModule.CallHeaderFiltered(fillter),
					adapter.DescriptionModule, adapter, currentStruct.hierarchy_obect);*/
        }

        void CATEGORY_TITLE1( MouseEventStruct
            currentStruct ) // adapter.bottomInterface.SHOW_CATEGORY_MENU( currentStruct.controller, Adapter.GET_SCENE_BY_ID( currentStruct.scene ), (s) => currentStruct.categoryIndex, true );
        { }

        void CATEGORY_TITLE2( MouseEventStruct currentStruct )
        {
            SHOW_CATEGORY_MENU( currentStruct.controller, (currentStruct.scene), ( s ) => currentStruct.categoryIndex );
        }

        void FAVORIT_FOLDERS_ICON_METHOD( MouseEventStruct currentStruct )
        {
            var refBookmarks = GET_BOOKMARKS();
            HierarchyCommonData.Instance().SetUndo( "Apply Project Bookmarks" );
            //adapter.CreateUndoActiveDescription("Change favorites params", (currentStruct.scene));
            refBookmarks[ currentStruct.categoryIndex ].ShowContent = !(1 == refBookmarks[ currentStruct.categoryIndex ].ShowContent) ? 1 : 0;
            HierarchyCommonData.Instance().SetDirty();
        }


        /* scanneddata FILTER_ON_DOWN__scanneddata;
		 int FILTER_ON_DOWN__index;
		 IStringSetter FILTER_ON_DOWN__m_favString;
		 int FILTER_ON_DOWN__m_favString_index;
		 int FILTER_ON_DOWN__m_scene;*/
        // Rect FILTER_ON_DOWN__ic;

        void FILTER_ON_DOWN( MouseEventStruct obj )
        {
            var FILTER_ON_DOWN__scanneddata = GET_PATHS(AssetDatabase.GetAssetPath(obj.hierarchy_obect[0]));
            var FILTER_ON_DOWN__index = obj.itemIndex;
            var FILTER_ON_DOWN__m_favString = obj.otherstring;
            var FILTER_ON_DOWN__m_favString_index = obj.categoryIndex;
            var FILTER_ON_DOWN__m_scene = obj.scene;
            //FILTER_ON_DOWN__ic = ic;


            var menu = new GenericMenu();
            var ext = FILTER_ON_DOWN__scanneddata.extensions;


            var list = ext.Select(d => new { k = d.Key, v = d.Value }).OrderBy(d => d.v).Select(d => d.k).ToArray();
            if ( list.Length == 0 ) ArrayUtility.Add( ref list, "*.*" );
            else ArrayUtility.Insert( ref list, 0, "*.*" );
            var index = FILTER_ON_DOWN__index + 1;

            //   Debug.Log(FILTER_ON_DOWN__m_favString);
            for ( int i = 0; i < list.Length; i++ )
            {
                var newI = i;
                menu.AddItem( new GUIContent( list[ i ] ), index == i, () => {
                    HierarchyCommonData.Instance().SetUndo( "Apply Project Bookmarks Change" );
                    //	adapter.CreateUndoActiveDescription("Change filter", (FILTER_ON_DOWN__m_scene));
                    if ( newI == 0 ) FILTER_ON_DOWN__m_favString.FilterString = null;
                    else FILTER_ON_DOWN__m_favString.FilterString = list[ newI ];
                    // Debug.Log(FILTER_ON_DOWN__m_favString);
                    //Debug.Log( (FILTER_ON_DOWN__m_favString is Int32List) + " " + FILTER_ON_DOWN__m_favString.GetString( FILTER_ON_DOWN__m_favString_index, adapter.pluginname ) );
                    //SET_DIRTY( (FILTER_ON_DOWN__m_scene) );
                    HierarchyCommonData.Instance().SetDirty();
                } );
            }

            menu.ShowAsContext();
            /* var newI =  EditorGUI.Popup( FILTER_ON_DOWN__ic, FILTER_ON_DOWN__index, list );
			 Debug.Log( newI );
			 if (newI != FILTER_ON_DOWN__index) {

			 }*/
        }


        void LIST_ON_DOWN( MouseEventStruct currentStruct )
        {
            var refBookmarks = GET_BOOKMARKS();
            //adapter.bottomInterface.GET_BOOKMARKS(ref refBookmarks, (currentStruct.scene));
            HierarchyCommonData.Instance().SetUndo( "Apply Project Bookmarks Change" );
            //	adapter.CreateUndoActiveDescription("Change favorites params", (currentStruct.scene));
            refBookmarks[ currentStruct.categoryIndex ].slots[ currentStruct.itemIndex ].ShowContentInh = 1 - refBookmarks[ currentStruct.categoryIndex ].slots[ currentStruct.itemIndex ].ShowContentInh;
            //SET_DIRTY( (currentStruct.scene) );
            HierarchyCommonData.Instance().SetDirty();
        }




        void REMOVE_ON_UP( MouseEventStruct currentStruct )
        {
            //ExternalModOldMenu.RemoveAndRefresh(Adapter.BottomInterface.MemType.Custom, currentStruct.itemIndex, currentStruct.categoryIndex, (currentStruct.scene));
            HierarchyCommonData.Instance().SetUndo( "Apply Project Bookmarks Remove" );
            var refBookmarks = GET_BOOKMARKS();
            refBookmarks[ currentStruct.categoryIndex ].slots.RemoveAt( currentStruct.itemIndex );
            //SET_DIRTY( (currentStruct.scene) );
            HierarchyCommonData.Instance().SetDirty();

            CURRENT_CONTROLLER.ClearAction();

        }

        /*void RemoveAndRefresh()
		{
			ClearAction();
			CURRENT_CONTROLLER.ClearAction();
			foreach (var w in adapter.bottomInterface.WindowController)
				if (w.REFERENCE_WINDOW)
					w.REFERENCE_WINDOW.Repaint();
		}*/



        // void SET_DIRTY( int scene )
        // {
        //     /*	var sc = scene;
        //		var d = adapter.MOI.des(sc);
        //		if (d == null) return;
        //		adapter.SetDirtyDescription(d, sc);*/
        //     HierarchyCommonData.Instance().SetDirty();
        // }

        int CONTROL_ID;
        Vector2 scrollPos;



        // EmtyString emptyString = new EmtyString();




        void CREATE_NEW_CAT( MouseEventStruct currentStruct )
        {
            var refBookmarks2 = GET_BOOKMARKS();
            var VAR_CAT_INDEX = currentStruct.controller.GetCategoryIndex(-1);
            //var pos = new MousePos(Event.current.mousePosition, MousePos.Type.Input_190_68, false, adapter);

            AddFavCategory( currentStruct.MousePosStruct, refBookmarks2, VAR_CAT_INDEX, currentStruct.scene, currentStruct.controller );
        }

        Rect Shrink( Rect rect, int i )
        {
            rect.x += i;
            rect.y += i;
            rect.width -= i * 2;
            rect.height -= i * 2;
            return rect;
        }



        internal void ClearHeight()
        {
            CURRENT_WIN.FORCE_REPAINT_THROUGH_LAYOUT = true;
            HEIGHT[ CONTROLLER ] = null;
            adapter.RepaintWindow( 1, true );
        }




        void SHOW_STRING( MousePos pos, string winname, string startS, Action<string> action, BookProject.FavorControllerWindow controller )
        {

            //CREATE_NEW_ESCRIPTION(adapter, mouseEventStruct.MousePosStruct, currentStruct.hierarchy_obect, -1, false);

            /*     Action<string> action = (t) =>
				 {
					 if (t == currentStruct.memoryRoot.description) return;
					 HierarchyCommonData.Instance().SetUndo("Create Description");
					 currentStruct.memoryRoot.description = t;
					 HierarchyCommonData.Instance().SetDirty();
				 };*/

            var adapter = Root.p[0];
            Windows.InputWindow.Init( pos, winname, Root.p[ 0 ].firstWindow( controller.pluginID ), action, null, startS );
            controller.RepaintNow();
            /*	if (Event.current == null)
				{
					Root.p[0].PUSH_GUI_ONESHOT(result);
					Root.p[0].RepaintWindowInUpdate();
				}

				else
				{
					result();
				}*/
        }



        internal void AddFavCategory( MousePos pos, List<FolderBookmark> capture_list, int VAR_CAT_INDEX, int scene, BookProject.FavorControllerWindow controller )
        {
            SHOW_STRING( pos, "New Category Name", capture_list[ VAR_CAT_INDEX ].category_name, ( value ) => {
                if ( string.IsNullOrEmpty( value ) ) return;
                //var capture_list = GET_BOOKMARKS();
                if ( capture_list.Any( b => b.category_name == value ) ) return;

                HierarchyCommonData.Instance().SetUndo( "Apply Project Bookmarks New Category" );
                //adapter.CreateUndoActiveDescription("New Category", scene);

                var result = new FolderBookmark()
                {
                    category_name = value,
					//BgColor = adapter.par_e.BOOKMARKS_FOLDER_DRAW_BG_COLOR ? adapter.par_e.BOOKMARKS_FOLDER_DEFULT_BG_COLOR : (Color?)null
				};
                CheckUniqueFavoritID( ref result.InstanceID );

                result.ShowContent = 0;
                capture_list.Add( result );

                HierarchyCommonData.Instance().SetDirty();

                controller.SetCategoryIndex( capture_list.Count - 1, scene );


                controller.RepaintNow();
            }, controller );
        }


        GUIContent content = new GUIContent();
        GUIContent content_des = new GUIContent();
        GUIContent REALEMPTY_CONTENT = new GUIContent();




        void DRAW_LABEL( Rect rect, GUIContent content, int fontSize, TempColorClass styleColor, TextAnchor? align = null )
        {
            DRAW_LABEL( rect, content, fontSize, styleColor != null && styleColor.HAS_LABEL_COLOR ? styleColor.LABELCOLOR : (Color?)null, align );
        }

        GUIStyle _DRAW_LABEL_STYLE;
        GUIStyle DRAW_LABEL_STYLE {
            get {
                if ( _DRAW_LABEL_STYLE == null )
                {
                    _DRAW_LABEL_STYLE = new GUIStyle( GUI.skin.label );
                    _DRAW_LABEL_STYLE.richText = true;
                }
                return _DRAW_LABEL_STYLE;
            }
        }

        void DRAW_LABEL( Rect rect, GUIContent content, int fontSize, Color? styleColor = null, TextAnchor? align = null ) // if (Event.current.type == EventType.Repaint)
        {
            {
                DRAW_LABEL_STYLE.alignment = align ?? TextAnchor.MiddleLeft;
                DRAW_LABEL_STYLE.fontSize = fontSize;
                var cc = GUI.color;
                if ( styleColor != null ) GUI.color *= styleColor.Value;
                //GUI.Label( rect, content );
                // ROUND_RECT( ref rect );
                //Adapter.GET_SKIN().label.Draw( rect, content, false, false, false, false );
                GUI.Label( rect, content, DRAW_LABEL_STYLE );
                if ( styleColor != null ) GUI.color = cc;

            }
        }

        /*   float CALC_LABEL(Rect rect, GUIContent content, int fontSize, TempColorClass styleColor )
		   {   return CALC_LABEL(rect,  content,  fontSize, styleColor != null && styleColor.HAS_LABEL_COLOR ? styleColor.LABELCOLOR : (Color? )null);
		   }*/

        float CALC_LABEL( Rect rect, GUIContent content, int fontSize ) //if (Event.current.type == EventType.Repaint)
        {
            DRAW_LABEL_STYLE.alignment = TextAnchor.MiddleLeft;
            DRAW_LABEL_STYLE.fontSize = fontSize;
            float w, x;
            DRAW_LABEL_STYLE.CalcMinMaxWidth( content, out w, out x );
            return Mathf.Min( rect.width, w );
        }

        GUIContent GET_DESCRIPTION( BookSlot mem )
        {
            if ( mem.description == null || mem.description == "" )
            {
                content_des.text = "";
                content_des.tooltip = "No Description\nClick to add Description";
            }
            else
            {
                content_des.text = content_des.tooltip = mem.description;
            }

            return content_des;
        }

        /*GUIContent GET_DESCRIPTION(Adapter.HierarchyObject o)
		{
			if (o != null )
			{
				if (adapter.DescriptionModule.HasKey(o.scene, o))
				{
					var d = adapter.DescriptionModule.GetValue(o.scene, o);
					content_des.text = content_des.tooltip = d;
				}
				else
				{
					content_des.text = "";
					content_des.tooltip = "No Description\nLeft CLICK to add Description";
				}
			}
			else
			{
				content_des.text = content_des.tooltip = "- ...";
			}

			return content_des;
		}*/


        /*
		TreeItem GET_TREE_ITEM(string id, int category, UniversalGraphController contoller, bool IS_ROOT)
		{
			return GET_TREE_ITEM(id, refBookmarks[category], contoller, IS_ROOT);
		}

		TreeItem GET_TREE_ITEM(string id, Int32ListArray category, UniversalGraphController contoller, bool IS_ROOT)
		{
			var C_I = contoller.MAIN ? 0 : 1;
			var tree = contoller.MAIN ? tree_item_hierarchy : tree_item_windows;
			if (IS_ROOT) id += "1";
			//  if (IS_ROOT)
			if (!tree.ContainsKey(id))
			{
				tree.Add(id, new TreeItem());
				tree[id].Expand = COLABSE_CACHE.Get(C_I, category, id, IS_ROOT);

			}

			return tree[id];
		}
*/




        /*



		  internal TreeData(string[] input)
		  {
			foreach (var path in input)
			{
			  var currentCatalog = root;
			  foreach (var seg in path.Split( '/' ))
			  {
				if (seg.Contains( '.' ))
				{
				  currentCatalog.items.Add( seg, new SceneData() { name = seg, path = path, Select = !path.StartsWith( "Assets/Plugins" ) && !path.StartsWith( "Assets/Editor" ) } );
				}
				else
				{
				  TreeItem newTree = null;
				  if (currentCatalog.treeItems.ContainsKey( seg )) newTree = currentCatalog.treeItems[seg];
				  else
				  {
					newTree = new TreeItem();
					currentCatalog.treeItems.Add( seg, newTree );
				  }
				  currentCatalog = newTree;
				}
			  }
			}
		  }
		}



		void DrawTreeView(TreeData.TreeItem treeData, int deep)
		{
		  foreach (var treeItem in treeData.treeItems)
		  {
			//GUILayout.BeginHorizontal();
			GUILayout.BeginHorizontal();

			var R = EditorGUILayout.GetControlRect( false , GUILayout.Height( H ) , GUILayout.Width( WIDTH * columns[0] ) );
			var drawR = R;
			drawR.width = WIDTH * 2;
			DrawColor( drawR, lineC[line++ % 2] );
			R.x += deep * H;
			R.width -= deep * H;
			drawR = R;
			drawR.width = H;
			drawR.x += drawR.width;
			var newS = EditorGUI.Toggle( drawR , treeItem.Value.Select );
			if (newS != treeItem.Value.Select)
			{
			  treeItem.Value.Select = newS;
			}
			drawR.x += drawR.width;
			drawR.width = R.width - drawR.x;
			GUI.Label( drawR, new GUIContent( treeItem.Key, adapter.GetIcon( "FOLDER" ) ) );

			GUILayout.EndHorizontal();


			//GUILayout.EndHorizontal();
			if (treeItem.Value.Expand) DrawTreeView( treeItem.Value, deep + 1 );
		  }


		  foreach (var treeItem in treeData.items)
		  {
			//GUILayout.BeginHorizontal();
			GUILayout.BeginHorizontal();
			var R = EditorGUILayout.GetControlRect( false , GUILayout.Height( H ) , GUILayout.Width( WIDTH * columns[0] ) );
			var drawR = R;
			drawR.width = WIDTH * 2;
			DrawColor( drawR, lineC[line++ % 2] );
			DrawColor( drawR, treeItem.Value.Select ? sceneC : sceneC5 );
			R.x += deep * H + H;
			R.width -= deep * H + H;
			drawR = R;
			drawR.width = H;
			treeItem.Value.Select = EditorGUI.Toggle( drawR, treeItem.Value.Select );
			drawR.x += drawR.width;
			drawR.width = R.width - drawR.x;

			GUI.Label( drawR, new GUIContent( treeItem.Key, adapter.GetIcon( "SCENE" ) ) );

			DRAWSCENE_GUI( treeItem );
			GUILayout.EndHorizontal();
			//GUILayout.EndHorizontal();
		  }
		}*/








        internal void SHOW_CATEGORY_MENU( ExternalDrawContainer controller, int scene, Func<int, int> CAT_INDEX, GenericMenu _menu = null ) //  Debug.Log(curentIndex);
        {
            var menu = _menu ?? new GenericMenu();
            var VAR_CAT_INDEX = CAT_INDEX(scene);


            var list = GET_BOOKMARKS();
            var capture_list = list;



            menu.AddItem( new GUIContent( "Show descriptions" ), adapter.par_e.BOOKMARKS_FOLDER_SHOW_ALL_DESCRIPTIONS_INHIER, () => {
                adapter.par_e.BOOKMARKS_FOLDER_SHOW_ALL_DESCRIPTIONS_INHIER = !adapter.par_e.BOOKMARKS_FOLDER_SHOW_ALL_DESCRIPTIONS_INHIER;
            } );
            menu.AddSeparator( "" );




            if ( VAR_CAT_INDEX == 0 )
            {
                menu.AddDisabledItem( new GUIContent( "Rename '" + capture_list[ VAR_CAT_INDEX ].category_name + "'" ) );
                menu.AddDisabledItem( new GUIContent( "Remove '" + capture_list[ VAR_CAT_INDEX ].category_name + "'category" ) );
            }
            else
            {
                var cont = CURRENT_CONTROLLER;
                var pos = new MousePos(Event.current.mousePosition, MousePos.Type.Input_190_68, false, adapter);
                menu.AddItem( new GUIContent( "Rename '" + capture_list[ VAR_CAT_INDEX ].category_name + "'" ), false, () => {
                    if ( VAR_CAT_INDEX == 0 ) return;

                    SHOW_STRING( pos, "Rename", capture_list[ VAR_CAT_INDEX ].category_name, ( value ) => {
                        if ( string.IsNullOrEmpty( value ) ) return;
                        if ( capture_list.Any( b => b.category_name == value ) ) return;


                        var oldValue = capture_list[CAT_INDEX(scene)].category_name;
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
                        HierarchyCommonData.Instance().SetUndo( "Apply project folders mod rename" );
                        cont.SetCurerentCategoryName( newValue );
                        HierarchyCommonData.Instance().SetDirty();

                        capture_list[ VAR_CAT_INDEX ].category_name = value;
                        //adapter.MarkSceneDirty(scene);
                        cont.RepaintNow();
                    }, CURRENT_CONTROLLER );
                } );

                menu.AddItem( new GUIContent( "Remove '" + capture_list[ VAR_CAT_INDEX ].category_name + "' Category" ), false, () => {
                    if (
                        EditorUtility.DisplayDialog( "Remove Category?", "Are you sure?", "Yes", "Cancel" ) )
                    {
                        if ( VAR_CAT_INDEX == 0 ) return;
                        if ( VAR_CAT_INDEX >= capture_list.Count ) return;

                        HierarchyCommonData.Instance().SetUndo( "Apply Project Bookmarks Remove Category" );
                        capture_list.RemoveAt( VAR_CAT_INDEX );
                        HierarchyCommonData.Instance().SetDirty();


                        cont.RepaintNow();
                        //static internalEditorUtility.RepaintAllViews();
                    }
                } );
            }


            if ( _menu == null )
            {


               // if ( !Application.isPlaying )
                {
					var pos = new MousePos(Event.current.mousePosition, MousePos.Type.ColorChanger_230_0, false, controller.adapter);
					var win = adapter.window;

					menu.AddSeparator( "" );
                    //	var controller = CURRENT_CONTROLLER;
                    menu.AddItem( new GUIContent( "Use categories background colors" ), adapter.par_e.BOOKMARKS_FOLDER_DRAW_BG_COLOR, () => {
                        adapter.par_e.BOOKMARKS_FOLDER_DRAW_BG_COLOR = !adapter.par_e.BOOKMARKS_FOLDER_DRAW_BG_COLOR;

                    } );
                    if ( !adapter.par_e.BOOKMARKS_FOLDER_DRAW_BG_COLOR )
                    {
                        menu.AddDisabledItem( new GUIContent( "Open categories background colors window" ) );
                    }
                    else
                    {
                        menu.AddItem( new GUIContent( "Open categories background colors window" ), false, () => {
							adapter.PUSH_UPDATE_ONESHOT( 1, () => // var pos = InputData.WidnwoRect(controller.IS_MAIN, Event.current.mousePosition, 190, 68, controller.adapter);
                             {
                                 CategoriesColorsWindowOld.InitForProjectFolder( pos , win);
                             } );


                        } );
                    }

                }
                menu.AddSeparator( "" );

                menu.AddItem( new GUIContent( "Open project folders mod settings" ), false, () => {
                    Settings.MainSettingsEnabler_Window.Select<Settings.BF_Window>();
                } );

                menu.ShowAsContext();
            }
        }




        //  const int EVENT_ADD = 1000000;
    }




}
