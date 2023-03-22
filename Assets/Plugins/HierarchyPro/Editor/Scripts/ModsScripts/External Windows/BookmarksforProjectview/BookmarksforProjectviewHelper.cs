using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;



namespace EMX.HierarchyPlugin.Editor.Mods.BookProject
{

	internal class FavorControllerWindow : ExternalDrawContainer
    {
        //  public FavorControllerWindow(PluginInstance adapter) : base() { }
        internal FavorControllerWindow( int p ) : base( p ) { }


        internal override bool hide_hierarchy_ui_buttons {
            get { return true; }
        }

        internal override float HEIGHT(int i) {
            return !ReferenceEquals(tempRoot, null) && tempRoot.OvValide()? tempRoot.position(IExternalWindowType.PROJECT_FOLD).height : 0; 
        }

        internal override float WIDTH(int i) {
            return !ReferenceEquals(tempRoot, null) &&tempRoot.OvValide()? tempRoot.position(IExternalWindowType.PROJECT_FOLD).width : 0; 
        }

        internal bool breaking;
        internal Rect __ModuleRect;



        internal int GetCategoryIndex( int scene )
        {

            return Mathf.Clamp( adapter.par_e.GET( "BOOKMARKS_FOLDER_CATEGORY", 0 ), 0, ((BookmarksforProjectviewModWindow)tempRoot).instance.GET_BOOKMARKS().Count - 1 );
            /*adapter.bottomInterface.GET_BOOKMARKS(ref list, scene);

			var d = adapter.MOI.des(scene);

			if (d == null) return 0;

			if (d.FavoritCategorySelected > list.Count - 1) d.FavoritCategorySelected = list.Count - 1;

			return Mathf.Clamp(d.FavoritCategorySelected, 0, list.Count - 1);*/
        }

        internal void SetCategoryIndex( int index, int scene )
        {
            GetCategoryIndex( scene );
            adapter.par_e.SET( "BOOKMARKS_FOLDER_CATEGORY", index );
            /*var d = adapter.MOI.des(scene);

			if (d == null) return;

			if (d.FavoritCategorySelected == index) return;

			d.FavoritCategorySelected = index;
			adapter.SetDirtyDescription(d, scene);
			//Hierarchy_GUI.SetDirtyObject( adapter );
			adapter.MarkSceneDirty(scene);*/
        }

        internal string GetCurerentCategoryName()
        {
            var c = tempRoot.titleContent;
            var ind = c.text.LastIndexOf(' ');
            if ( ind == -1 || ind >= c.text.Length ) return "";
            var nn = c.text.Remove(c.text.LastIndexOf(' '));
            return nn;
        }

        internal void SetCurerentCategoryName( string name )
        {
            var c = tempRoot.titleContent;
            c.text = name + " " + adapter.pluginname;
            tempRoot.titleContent = c;
            tempRoot.RepaintNow();
        }

    } // class FavorControllerWindow




    partial class BookmarksforProjectviewModInstance
    {





        internal List<FolderBookmark> GET_BOOKMARKS() //  var scene = EditorSceneManager.GetActiveScene();
        {

            var list = HierarchyCommonData.Instance().FoolderBookmarkList;

            if ( list.Count < 1 || list[ 0 ].category_name != "Default" )
            {
                var ind = list.FindIndex(c=>c.category_name == "Default");
                if ( ind != -1 )
                {
                    var t = list[ind];
                    list.RemoveAt( ind );
                    if ( list.Count == 0 ) list.Add( t );
                    else list.Insert( 0, t );
                }
                else
                {
                    var t = new FolderBookmark();
                    t.SetAsDefault();
                    if ( list.Count == 0 ) list.Add( t );
                    else list.Insert( 0, t );
                }
            }
            list[ 0 ].SetAsDefault();
            //if (list.Count == 0) // var b = adapter.MOI.des(scene).GetBookMarks();
            //{
            //	var t =new FolderBookmark();
            //	t.SetAsDefault();
            //    list.Add(t);
            //}
            //	if (adapter.MOI.des(scene).GetHash4() == null) adapter.MOI.des(scene).SetHash4(new List<Int32List>());
            //	list[0].array = adapter.MOI.des(scene).GetHash4();



            for ( int i = 0; i < list.Count; i++ )
                CheckUniqueFavoritID( ref list[ i ].InstanceID );
            return list;
        }

        Dictionary<int, int> _mFavIdCache = new Dictionary<int, int>();

        internal void CheckUniqueFavoritID( ref int id )
        {
            if ( id != 0 )
            {
                if ( !_mFavIdCache.ContainsKey( id ) ) _mFavIdCache.Add( id, -1 );
                return;
            }
            int result;
            do result = UnityEngine.Random.Range( 10001, int.MaxValue );
            while ( _mFavIdCache.ContainsKey( result ) );
            _mFavIdCache.Add( result, -1 );
            id = result;
            //return result;
        }



        internal class scanneddata
        {
            internal Dictionary<string, int> extensions;
            internal List<scanneditem> scanneditems;
        }

        internal class scanneditem
        {
            PluginInstance adapter;
            internal HierarchyObject CurrentObject = null;

            internal scanneditem( PluginInstance adapter, string name, string rootpath, string fullPath, string[] folders )
            {
                if ( name.LastIndexOf( '.' ) == -1 )
                {
                    m_name = name;
                    extension = "";
                }

                else
                {
                    m_name = name.Remove( name.LastIndexOf( '.' ) );
                    extension = name.Substring( name.LastIndexOf( '.' ) + 1 ).ToLower();
                }

                m_folders = folders;
                Array.Resize( ref m_folders, m_folders.Length - 1 );
                this.fullPath = fullPath.Trim( '/' );
                this.rootPath = rootpath.Trim( '/' );
                this.adapter = adapter;
            }

            internal folderClass GetFoldByIndex( int index )
            {
                if ( foldersObjects == null )
                {
                    foldersObjects = new folderClass[ folders.Length ];
                    var cp = rootPath;
                    int realI = 0 ;
                    for ( int i = 0; i < folders.Length; i++ )
                    {
                        cp += '/' + folders[ i ];

                        var ob = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( cp );
                        if ( !ob )
                        {
                            realI++;
                            foldersObjects[ i ] = null;
                            continue;
                        }
                        //var guid = AssetDatabase.AssetPathToGUID(cp);
                        //foldersObjects[i] = adapter.GetHierarchyObjectByGUID(ref guid, null);
                        foldersObjects[ i ] = new folderClass() {
                            folder_object = ob,
                            path = cp
                        };
                        realI++;
                    }
                    if ( realI != foldersObjects.Length ) Array.Resize( ref foldersObjects, realI );
                }

                return foldersObjects[ index ];
            }

            internal int DEEP {
                get { return folders.Length; }
            }

            string m_name;
            internal string extension;

            internal string name {
                get { return m_name; }
            }

            internal string fullPath;
            internal string rootPath;
            folderClass[] foldersObjects;
            string[] m_folders;

            internal string[] folders {
                get { return m_folders; }
            }
        }
        internal class folderClass
        {
            //internal bool folder_expanded = true;
            internal UnityEngine.Object folder_object;
            internal string path;
        }
        internal static Dictionary<string, scanneddata> scanned_folder = new Dictionary<string, scanneddata>();


        scanneddata GET_PATHS( string folder )
        {
            if ( !scanned_folder.ContainsKey( folder ) ) //Debug.Log( folder );
            { //var L = (UNITY_SYSTEM_PATH + folder).Length;
              // var paths = Directory.GetFiles( UNITY_SYSTEM_PATH + folder, "*.*", SearchOption.AllDirectories ).Where( f => !f.EndsWith( ".meta" ) ).Select(f => f.Substring(L).Replace('\\', '/')).ToList();
                if ( folder.EndsWith( "/" ) ) throw new Exception( "/" );

                var stw = string.IsNullOrEmpty(folder) ? "Assets" : folder;
                stw += '/';
                var L = stw.Length - 1;
                //	var paths = TODO.ALL_ASSETS_PATHS.Where(p => p.StartsWith(stw)).Select(p => p.Substring(L)) /*.Select(p => p.Replace('\\', '/'))*/.ToArray();

                //var mix = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);

                var paths = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories).Where(f => !f.EndsWith(".meta", StringComparison.OrdinalIgnoreCase)).Select(f => f.Replace('\\', '/')).ToArray();
                //var paths = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories).Where(f => f.EndsWith(".meta")).Select(f => f.Remove(f.Length - 5).Replace('\\', '/')).ToArray();
                List<scanneditem> list = new List<scanneditem>();
                Dictionary<string, int> extensions = new Dictionary<string, int>();

                if ( paths.Length != 0 )
                {

                    /*	var trim = paths[0].IndexOf(Folders.UNITY_SYSTEM_PATH);
						if (trim != -1)
						{
							trim += Folders.UNITY_SYSTEM_PATH.Length;
							paths = paths.Select(p => p.Substring(trim)).ToArray();
						}*/
                    var trim = paths[0].IndexOf(folder);
                    if ( trim != -1 )
                    {
                        trim += folder.Length;
                        paths = paths.Select( p => p.Substring( trim ) ).ToArray();
                    }
                    stw = stw.Trim( '/' );
                    list = paths.Select( p => new { p, fld = p.Trim( '/' ).Split( '/' ).ToArray() } ).Where( p => p.fld.Length != 0 ).Select( p => new scanneditem(
                                  adapter: adapter,
                                  name: p.p.Substring( p.p.LastIndexOf( '/' ) + 1 ),
                                  rootpath: stw,
                                  fullPath: stw + p.p,
                                  folders: p.fld ) ).ToList();


                    /*foreach (var item in list)
					 {   // var guid = AssetDatabase.AssetPathToGUID( stw + item.rootPath );
						 // Debug.Log(item.rootPath + "\n" + item.fullPath + "\n" + item.extension + "\n" + item.folders[0]);
						 Debug.Log( "-" + item.fullPath + "\n" + item.name);
					 }*/
                    // var extensions = paths.Select(p => p.LastIndexOf('.') == -1 ? "" : p.Substring(p.LastIndexOf('.') + 1).ToLower()).GroupBy(p => p).Select((g, i) => new {k = g.Key, i } ).ToDictionary(k => k.k,
                    extensions = list
                       //.Select(p => p.name.LastIndexOf('.') == -1 ? "" : p.name.Substring(p.name.LastIndexOf('.') + 1).ToLower())
                       .Select( p => p.extension )
                       .GroupBy( p => p ).Select( ( g,
                              i ) => new { k = g.Key, i } ).ToDictionary(
                           k => k.k,
                           v => v.i );


                }
                else
                {

                }

                scanned_folder.Add( folder, new scanneddata() { scanneditems = list, extensions = extensions } );
            }

            /*   foreach (var f in adapter.scanned_folder[folder].extensions)
			   {   Debug.Log( folder + "\n" + f.Key);
			   }*/
            return scanned_folder[ folder ];
        }








        void DRAG_DRAG( Rect rect, bool fullAdd, FavorControllerWindow controller, int sourcescene, ref List<FolderBookmark> sourcecategoryref, int? sourcecategory_I, int? sourcearrayIndex,
            int targetscene, ref List<FolderBookmark> targetcategoryref, int targetcategoryref_I, int targetarrayIndex )
        {
            if ( DragAndDrop.visualMode != DragAndDropVisualMode.Rejected && rect.Contains( Event.current.mousePosition ) )
            {
                switch ( Event.current.type )
                {
                    case EventType.DragUpdated:
                        if ( FAVOR_DRAG_VALIDATOR() ) DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                        else DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

                        Tools.EventUseFast();
                        break;
                    case EventType.Repaint:
                        if (
                            DragAndDrop.visualMode == DragAndDropVisualMode.None ||
                            DragAndDrop.visualMode == DragAndDropVisualMode.Rejected ) break;

                        if ( FAVOR_DRAG_VALIDATOR() )
                        {
                            if ( fullAdd ) GET_DRAG_BOX.Draw( rect, false, false, false, false );
                            else
                            {
                                var drawR = rect;
                                drawR.height = 8;
                                drawR.y = rect.y - 4;
                                if ( Event.current.mousePosition.y > rect.y + rect.height / 2 ) drawR.y += rect.height;
                                GET_DRAG_LINE.Draw( drawR, false, false, false, false );
                            }

                            //if (controller.__ModuleRect)
                            //EditorGUI.DrawRect( rect, Color.grey );
                        }

                        // if (validate()) EditorGUI.DrawRect( dropArea, color ?? Color.grey );
                        break;
                    case EventType.DragPerform:
                        DragAndDrop.AcceptDrag();
                        if ( Event.current.mousePosition.y > rect.y + rect.height / 2 ) targetarrayIndex++;
                        FAVOR_DRAG_PERFORMER( controller, (sourcescene), ref sourcecategoryref, sourcecategory_I, sourcearrayIndex, (targetscene), ref targetcategoryref,
                            targetcategoryref_I, targetarrayIndex );
                        Tools.EventUseFast();
                        break;
                    case EventType.MouseUp:
                        adapter.ha.InternalClearDrag();
                        // DragAndDrop.PrepareStartDrag();
                        break;
                }
            }
        }


        //  List<Int32ListArray> FAVOR_DRAG_PERFORMER_LIST = new  List<Int32ListArray>();
        internal void FAVOR_DRAG_PERFORMER( FavorControllerWindow controller, int sourcescene, ref List<FolderBookmark> sourcecategoryref, int? sourcecategory_I, int? sourcearrayIndex,
            int targetscene, ref List<FolderBookmark> targetcategoryref, int targetcategoryref_I, int targetarrayIndex )
        {
            if ( FAVOR_DRAG_VALIDATOR() )
            { // List<Int32List> l1 = controller.GetCategoryIndex(scene) == 0 ? adapter.MOI.des(scene).GetHash4() : adapter.MOI.des(scene).GetBookMarks()[controller.GetCategoryIndex(scene)].array;
              // var l1 = adapter.MOI.des(s).GetHash4();


                HierarchyCommonData.Instance().SetUndo( "Drag Favorite" );


                BookSlot t = null;
                if ( sourcearrayIndex.HasValue )
                { //adapter.bottomInterface.GET_BOOKMARKS( ref FAVOR_DRAG_PERFORMER_LIST, sourcescene );

                    // Debug.Log( adapter.MOI.des( sourcescene ).GetHash4().Count );
                    /* Debug.Log( sourcecategoryref[0].array.Count );
					 Debug.Log( sourcecategoryref.Count );*/


                    if ( sourcearrayIndex.Value < 0 || sourcearrayIndex.Value >= sourcecategoryref[ sourcecategory_I.Value ].slots.Count ) return;
                    t = sourcecategoryref[ sourcecategory_I.Value ].slots[ sourcearrayIndex.Value ];
                    sourcecategoryref[ sourcecategory_I.Value ].slots.RemoveAt( sourcearrayIndex.Value );
                    //adapter.SetDirtyActiveDescription(sourcescene);
                    HierarchyCommonData.Instance().SetDirty();

                    if ( targetarrayIndex > sourcearrayIndex.Value && sourcecategory_I == targetcategoryref_I ) targetarrayIndex--;
                    /* Debug.Log( sourcecategoryref[sourcecategory_I.Value].array.Count );
					 Debug.Log( adapter.MOI.des( sourcescene ).GetHash4().Count );*/
                }
                else
                {
                    var result = GetDragData().Where(o => o).ToArray();
                    /* if (result.Length != 0)
					   adapter.bottomInterface.AddAndRefreshCustom( result, result[0], targetcategory, targetscene );*/
                    if ( result.Length != 0 )
                    {
                        t = new BookSlot();
                        t.SetFromObjects( result );
                    }
                    /* sourcearrayIndex = targetcategory.Count - 1;
					 sourcecategory = targetcategory;*/
                    //  t = DragAndDrop.objectReferences.Select(o=>adapter.GetHierarchyObjectByInstanceID(o.GetInstanceID())).
                }

                if ( t != null && t.guids != null && t.guids.Count != 0 )
                { // Debug.Log(t .InstanceID);
                  //targetcategoryref[targetcategoryref_I].slots.RemoveAll(a => a.GUIDsActiveGameObject_CheckAndGet == t.GUIDsActiveGameObject_CheckAndGet);

                    if ( targetarrayIndex >= targetcategoryref[ targetcategoryref_I ].slots.Count ) targetcategoryref[ targetcategoryref_I ].slots.Add( t );
                    else targetcategoryref[ targetcategoryref_I ].slots.Insert( targetarrayIndex, t );

                    /*  for (int i = 0; i < 2; i++)
					  {   if (!colabsed_cache[i].ContainsKey(t.InstanceID))
							  colabsed_cache[i].Add(t.InstanceID, new Dictionary<long, bool>());
						  if (!colabsed_cache[i][t.InstanceID].ContainsKey(t.InstanceID))
							  colabsed_cache[i][t.InstanceID].Add(t.InstanceID, true);
					  }*/
                }
                /*var b = l1[arrayIndex];
				l1.RemoveAt( arrayIndex );*/

                // Debug.Log( t );

                adapter.ha.InternalClearDrag();
                //  DragAndDrop.PrepareStartDrag();

                //adapter.SetDirtyActiveDescription(targetscene);
                HierarchyCommonData.Instance().SetDirty();


                //adapter.bottomInterface.RefreshMemCache(targetscene);

                // adapter.RepaintWindowInUpdate();
                controller.RepaintNow();
            }
        }

        bool FAVOR_DRAG_VALIDATOR()
        { /* var type = (bool?)DragAndDrop.GetGenericData( "Dragging Assets" );
                 if (type.HasValue && type.Value) return false;*/
            if ( DragAndDrop.objectReferences.Length == 0 ) return false;
            BookSlot s = new BookSlot();
            s.SetFromObjects( DragAndDrop.objectReferences );
            return s.guids.Count != 0;
            //return DragAndDrop.objectReferences.Any(g => !string.IsNullOrEmpty(adapter.bottomInterface.INSTANCEID_TOGUID(g.GetInstanceID())));
        }

        UnityEngine.Object[] GetDragData()
        {
            return DragAndDrop.objectReferences.Where( o => !string.IsNullOrEmpty( isProjectObject( o ) ) ).ToArray();
        }



        internal static bool isProjectObjectBool( UnityEngine.Object o )
        {
            var gameObject = o as GameObject;

            if ( gameObject != null && isSceneObject( gameObject ) ) return false;

            return true;
        }
        internal static string isProjectObject( UnityEngine.Object o )
        {
            var gameObject = o as GameObject;

            if ( !o || gameObject != null && isSceneObject( gameObject ) ) return null;

            return AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( o.GetInstanceID() ) );
        }
        internal static bool isSceneObject( GameObject o )
        {
            return o && o.scene.IsValid();
        }


        float[] pixelCost = new float[2];
        float SCROLL_DOWN_START;

        void SCROLL_DOWN( FavorControllerWindow controller )
        {
            SCROLL_DOWN_START = EditorGUIUtility.GUIToScreenPoint( Event.current.mousePosition ).y;
        }

        void SCROLL_DRAG( FavorControllerWindow controller )
        {
            var M = EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition).y;
            var dif = M - SCROLL_DOWN_START;
            if ( dif != 0 )
            {
                SCROLL_DOWN_START = M;
                var C_I = controller.MAIN ? 0 : 1;
                var refBookmarks = GET_BOOKMARKS();
                refBookmarks[ 0 ].scrollY += dif * pixelCost[ C_I ];
                adapter.RepaintWindowInUpdate( 0 );
            }
        }

        internal void ON_SCROLL( float sc )
        {
            var refBookmarks = GET_BOOKMARKS();
            var dif = 40;
            if ( sc < 0 ) dif *= -1;
            refBookmarks[ 0 ].scrollY += dif;
            //SET_DIRTY( -1 );
            Tools.EventUse();
        }


        void EVENT_EVENT_EVENT( Rect lastRect, string mouseDown, string mouseMove, string mouseUp, object arg, FavorControllerWindow controller, int controlID, bool useHighlight, int button = 0,
            string tooltip = null )
        {
            if ( Event.current.type == EventType.MouseDown && lastRect.Contains( Event.current.mousePosition ) )
            {
                if ( Event.current.button == button )
                {
                    Tools.EventUseFast();

                    var target = this;
                    if ( arg != null && arg is MouseEventStruct )
                    {
                        var clone = (MouseEventStruct)((MouseEventStruct)arg).Clone();
                        var p = GUIUtility.GUIToScreenPoint(new Vector2(clone.LocalModuleRect.x, clone.LocalModuleRect.y));
                        clone.WorldModuleRect = new Rect( p.x + 3, p.y, clone.LocalModuleRect.width - 9, clone.LocalModuleRect.height );
                        arg = clone;
                    }


                    // Debug.Log("ASD");
                    CURRENT_CONTROLLER.ClearAction();

                    INVOKE( mouseDown, arg, target );

                    actions.ADD_ACTION( controlID, lastRect, contains => {
                        if ( controller.currentAction == null ) return true;
                        INVOKE( mouseMove, arg, target );
                        return false;
                    }, contains => {
                        INVOKE( mouseMove, arg, target );
                        if ( contains )
                        {
                            INVOKE( mouseUp, arg, target );
                            //  RemoveAndRefresh(type, arrayIndex);
                        }
                    }, CURRENT_CONTROLLER );

                    /* controller.selection_button = controlID;
					 controller.selection_window = controller.REFERENCE_WINDOW;
					 controller.lastRect = lastRect;
					 var captureCell = lastRect;*/


                    /* controller.selection_action = (mouseUp_b, deltaTIme) =>
					 {
					   adapter.bottomInterface.ClearAction();

					   INVOKE( mouseMove, arg );

					   if (mouseUp_b && captureCell.Contains( Event.current.mousePosition ))
					   {
						 INVOKE( mouseUp, arg );
						 //  RemoveAndRefresh(type, arrayIndex);
					   }
					 }; // ACTION*/
                } //if button
            }

            if ( !string.IsNullOrEmpty( tooltip ) )
            {
                tooltipcontent.tooltip = tooltip;
                Label( lastRect, tooltipcontent );
            }

            if ( Event.current.type == EventType.Repaint && useHighlight && actions.HOVER( controlID, lastRect, controller ) )
                // HIPERUI_BUTTONGLOW.Draw( lastRect, false, false, false, false );
                // Adapter.GET_SKIN().button.Draw( lastRect, true, true, false, false );
                adapter.gl.DRAW_TAP_GLOW( lastRect, Colors.colorStatic );
            /* if (controller.selection_action != null && useHighlight && controller.selection_button == controlID)
			   GUI.DrawTexture( lastRect, adapter.colorStatic );*/
        }

        static GUIContent tooltipcontent = new GUIContent();


        Dictionary<string, MethodInfo> mm = new Dictionary<string, MethodInfo>();

        void INVOKE( string method, object args, object target )
        { //Debug.Log( method + " " + args );
            if ( string.IsNullOrEmpty( method ) ) return;
            if ( !mm.ContainsKey( method ) ) mm.Add( method, target.GetType().GetMethod( method, (BindingFlags)(-1) ) );
            // Debug.Log( mm[method].GetParameters()[0].ParameterType == args.GetType() );

            // try
            {
                mm[ method ].Invoke( target, args == null ? null : new[] { args } );
            }
            /*catch (Exception e)
			{ // adapter.logProxy.LogError( e.Message + "\n" + e.StackTrace );
				throw new Exception(e);
			}*/
        }














    }



}
