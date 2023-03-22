using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace EMX.HierarchyPlugin.Editor.Events
{
	partial class HierarchyActions //HierarchySelections
    {

        //SELECTION
        internal HierarchyObject[] FILTER_GAMEOBJECTS( UnityEngine.Object[] o )
        {
            if ( pluginID == 0 ) return /* Selection.gameObjects == null ? new GameObject[0] :*/
                    o.Select( g => g as GameObject ).Where( g => g && isSceneObject( g ) ).Select( g => Cache.GetHierarchyObjectByInstanceID( g as GameObject ) ).ToArray();
            return o.Where( ob => ob && !string.IsNullOrEmpty( isProjectObject( ob ) ) ).Select( ob => Cache.GetHierarchyObjectByGUID( ob.GetInstanceID() ) ).ToArray();
        }
        internal HierarchyObject[] SELECTED_GAMEOBJECTS()
        {
            return FILTER_GAMEOBJECTS( Selection.objects );
        }
        internal HierarchyObject[] SELECTED_OBJECTS()
        {
            return FILTER_GAMEOBJECTS( Selection.objects );
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


        internal HierarchyObject activeGameObject()
        {
            if ( pluginID == 0 ) return isSceneObject( Selection.activeGameObject ) ? Cache.GetHierarchyObjectByInstanceID( Selection.activeGameObject ) : null;
            var g = isProjectObject(Selection.activeObject);
            if ( string.IsNullOrEmpty( g ) ) return null;
            return Cache.GetHierarchyObjectByGUID( Selection.activeObject.GetInstanceID() );
        }


#pragma warning disable
        int _IsDraggedCache_lastID, _IsDraggedCache_LastCount = -1;
#pragma warning restore
        internal Dictionary<int, bool> _IsDraggedCache = new Dictionary<int, bool>();

        internal int  _IsSelectedCache_LastCount = -1;
        internal Dictionary<int, bool> _IsSelectedCache = new Dictionary<int, bool>();
        internal int selMax = 0;

        internal void OnSelectionChanged_SaveCache()
        {
            var p = Root.p[0];
            if ( p.TreeController_current == null ) return;

            //p.TreeController_current = p.GetTreeViewontroller( pluginID );
            //p.gui_currentTree = p._gui.GetValue( p.TreeController_current );
            //p.data_currentTree = p._data.GetValue( p.TreeController_current );
            //p.state_currentTree = p._state.GetValue( p.TreeController_current );

            // TreeViewController.IntegerCache
            if ( m_DragSelection == null ) m_DragSelection = p.TreeController_current.GetType().GetField( "m_DragSelection", ~(BindingFlags.InvokeMethod & BindingFlags.Static) );

#if UNITY_2021_1_OR_NEWER

            if ( m_SelectedIDs == null )
            {
                m_SelectedIDs = p.TreeController_current.GetType().GetField( "m_CachedSelection", ~(BindingFlags.InvokeMethod & BindingFlags.Static) );

            }
            current_DragSelection_Methods();

            if ( m_HashSet != null )
            {
                p.current_DragSelection_List = m_HashSet.GetValue( m_DragSelection.GetValue( Root.p[ 0 ].TreeController_current ) ) as HashSet<int>;
                p.current_selectedIDs = m_HashSet.GetValue( m_SelectedIDs.GetValue( p.TreeController_current ) ) as HashSet<int>;
            }
            if ( p.current_DragSelection_List == null ) p.current_DragSelection_List = new HashSet<int>();
            if ( p.current_selectedIDs == null ) p.current_selectedIDs = new HashSet<int>();

#else
      

            // Debug.Log( currentState.GetType().BaseType );
            if ( m_SelectedIDs == null )
            {
                m_SelectedIDs = p.state_currentTree.GetType().GetField( "m_SelectedIDs", ~(BindingFlags.InvokeMethod & BindingFlags.Static) );

                if ( m_SelectedIDs == null )
                    m_SelectedIDs = p.state_currentTree.GetType().BaseType.GetField( "m_SelectedIDs", ~(BindingFlags.InvokeMethod & BindingFlags.Static) );
            }

            //var asd = p.TreeController_current.GetType().GetField("m_CachedSelection", ~(BindingFlags.InvokeMethod & BindingFlags.Static)).
            //GetValue(p.TreeController_current) as IList<int>;

            //var c = p.state_currentTree.GetType().GetProperty( "selectedIDs", ~(BindingFlags.InvokeMethod & BindingFlags.Static) ).GetValue//(p.state_currentTree,null) as IList<int>;
            //
            //Debug.Log( c?.Count );

            p.current_DragSelection_List = m_DragSelection.GetValue( p.TreeController_current ) as IList<int>;
            if ( p.current_DragSelection_List == null ) p.current_DragSelection_List = new List<int>();
            p.current_selectedIDs = m_SelectedIDs.GetValue( p.state_currentTree ) as IList<int>;
            if ( p.current_selectedIDs == null ) p.current_selectedIDs = new List<int>();

#endif



            if ( p.current_DragSelection_List.Count != 0 )
            {
                var o = EditorUtility.InstanceIDToObject(p.current_DragSelection_List.First()) as GameObject;
                if ( o && o.scene.IsValid() ) p.LastActiveScene = o.scene;
            }
            else if ( p.current_selectedIDs.Count != 0 )
            {
                var o = EditorUtility.InstanceIDToObject(p.current_selectedIDs.First()) as GameObject;
                if ( o && o.scene.IsValid() ) p.LastActiveScene = o.scene;
            }
        }

        //MethodInfo m_IsItemDragSelectedOrSelected;
        //bool b_IsItemDragSelectedOrSelected;
        internal bool IsSelected( HierarchyObject id )
        {
            return IsSelected( id.id );
            //if ( id._visibleTreeItem == null ) return IsSelected( id.id );
            //
            //var p = Root.p[0];
            //if ( !b_IsItemDragSelectedOrSelected )
            //{
            //    b_IsItemDragSelectedOrSelected = true;
            //    m_IsItemDragSelectedOrSelected = p.TreeController_current.GetType().GetMethod( "IsItemDragSelectedOrSelected", /(BindingFlags)/~0 );
            //}
            //if ( m_IsItemDragSelectedOrSelected == null ) return IsSelected( id.id );
            //
            //return (bool)m_IsItemDragSelectedOrSelected.Invoke( p.TreeController_current, new object[ 1 ] { id._visibleTreeItem } );
        }

#if !UNITY_2021_1_OR_NEWER
        int _IsSelectedCache_lastID;
#endif
		internal bool IsSelected( int id )
        {
            var p = Root.p[0];
            //
            //


#if UNITY_2021_1_OR_NEWER
            return p.current_DragSelection_List.Count > 0 ? p.current_DragSelection_List.Contains( id ) : p.current_selectedIDs.Contains( id );
#else
            selMax = 0;

            if ( p.current_DragSelection_List.Count != 0 )
            {
                if ( _IsDraggedCache_LastCount != _IsDraggedCache.Count || _IsDraggedCache_lastID != p.current_DragSelection_List[ 0 ] )
                {
                    _IsDraggedCache_lastID = p.current_DragSelection_List[ 0 ];
                    _IsDraggedCache_LastCount = p.current_DragSelection_List.Count;
                    _IsDraggedCache.Clear();
                    _IsDraggedCache = p.current_DragSelection_List.ToDictionary( k => k, k => true );
                }

                selMax = p.current_DragSelection_List.Count;

                return _IsDraggedCache.ContainsKey( id );

                /*if ( !_IsDraggedCache.ContainsKey( id ) )
					_IsDraggedCache.Add( id, current_DragSelection_List.Contains( id ) );  //(adapter._mSelectedO.ContainsKey(__o.id) )

				return _IsDraggedCache[id];*/
            }

            if ( p.current_selectedIDs.Count != 0 )
            {
                if ( _IsSelectedCache_LastCount != _IsSelectedCache.Count || _IsSelectedCache_lastID != p.current_selectedIDs[ 0 ] )
                {
                    _IsSelectedCache_lastID = p.current_selectedIDs[ 0 ];
                    _IsSelectedCache_LastCount = p.current_selectedIDs.Count;
                    _IsSelectedCache.Clear();
                    _IsSelectedCache = p.current_selectedIDs.ToDictionary( k => k, k => true );
                }

                selMax = p.current_selectedIDs.Count;

                return _IsSelectedCache.ContainsKey( id );
                /*if ( !_IsSelectedCache.ContainsKey( id ) )
					_IsSelectedCache.Add( id, current_selectedIDs.Contains( id ) );  //(adapter._mSelectedO.ContainsKey(__o.id) )

				return _IsSelectedCache[id];*/
            }
            return false;
#endif

        }


        internal void DisableHover()
        {
            Root.p[ 0 ].HoverDisabled = true;
            Root.p[ 0 ].hoverID = -1;
        }
        internal void internal_DisableHover()
        {
            if ( !Root.p[ 0 ].hashoveredItem ) return;
            Root.p[ 0 ].hoveredItem.SetValue( Root.p[ 0 ].TreeController_current, null, null );
            Root.p[ 0 ].hoverID = -1;
        }
        bool wasInitcurrent_DragSelection_Methods;
        static FieldInfo m_HashSet;
        internal void current_DragSelection_Methods()
        {
            if ( wasInitcurrent_DragSelection_Methods ) return;
            wasInitcurrent_DragSelection_Methods = true;
            var d1 = m_DragSelection.GetValue( Root.p[0].TreeController_current );
            m_HashSet = d1.GetType().GetField( "m_HashSet", ~(BindingFlags.InvokeMethod & BindingFlags.Static) );
        }
        FieldInfo m_DragSelection, m_SelectedIDs;
        PropertyInfo dragging;
        internal void InternalClearSelection( int[] ids )
        {

            if ( pluginID != 0 || Root.p[ 0 ].lastHierarchyWindw == null ) return;
            var window = Root.p[0].lastHierarchyWindw.Instance;

            if ( !window ) return;
            InternalClearDrag();

            var currentTree = Root.p[0].GetTreeViewontroller(pluginID, window);
            if ( currentTree == null ) return;

            var currentState = Root.p[0]._state.GetValue(currentTree, null);
            if ( currentState == null ) return;

            if ( m_SelectedIDs == null )
            {
                m_SelectedIDs = currentState.GetType().GetField( "m_SelectedIDs", ~(BindingFlags.InvokeMethod & BindingFlags.Static) );
                if ( m_SelectedIDs == null )
                    m_SelectedIDs = currentState.GetType().BaseType.GetField( "m_SelectedIDs", ~(BindingFlags.InvokeMethod & BindingFlags.Static) );
            }

            if ( m_SelectedIDs != null )
            {
                var asd = m_SelectedIDs.GetValue(currentState) as IList<int>;
                if ( asd != null ) asd.Clear();
                else asd = new List<int>();
                foreach ( var id in ids )
                {
                    asd.Add( id );
                }

                m_SelectedIDs.SetValue( currentState, asd );
                if ( dragging == null )
                {
                    dragging = currentTree.GetType().GetProperty( "dragging", ~(BindingFlags.InvokeMethod & BindingFlags.Static) );
                }
                var dr = dragging.GetValue(currentTree, null);
                if ( dr != null )
                {
                    var m = dragging.PropertyType.GetMethod("DragCleanup", ~(BindingFlags.GetField & BindingFlags.GetProperty & BindingFlags.Static));
                    if ( m != null )
                    {
                        m.Invoke( dr, new object[] { true } );
                    }
                }
            }
        }

        internal void InternalClearDrag()
        {

            if ( pluginID != 0 || Root.p[ 0 ].lastHierarchyWindw == null ) return;
            var window = Root.p[0].lastHierarchyWindw.Instance;
            // ClearTree();
            if ( !window ) return;
            var currentTree = Root.p[0].GetTreeViewontroller(pluginID, window);
            if ( m_DragSelection == null )
            {
                if ( currentTree == null ) return;
                m_DragSelection = currentTree.GetType().GetField( "m_DragSelection", (BindingFlags)(-1) );
            }
            // PushAction(() =>
            if ( m_DragSelection != null )
            {
                if ( currentTree == null ) return;
                var current_DragSelection_List = m_DragSelection.GetValue(currentTree) as IList<int>;
                if ( current_DragSelection_List != null ) current_DragSelection_List.Clear();
                m_DragSelection.SetValue( currentTree, current_DragSelection_List );

                if ( dragging == null ) dragging = currentTree.GetType().GetProperty( "dragging", (BindingFlags)(-1) );

                var dr = dragging.GetValue(currentTree, null);
                if ( dr != null )
                { /*var m  = dragging.PropertyType.GetMethod( "DragElement", (BindingFlags)(-1) );
				    if (m != null)
				    {   m.Invoke( dr, new object[] {(UnityEditor.IMGUI.Controls.TreeViewItem) null, new Rect(), -1 });
				        Debug.Log("ASD");
				    }*/
                    var m = dragging.PropertyType.GetMethod("DragCleanup", (BindingFlags)(-1));
                    if ( m != null ) m.Invoke( dr, new object[] { true } );
                }
            }
        }


    }
}
