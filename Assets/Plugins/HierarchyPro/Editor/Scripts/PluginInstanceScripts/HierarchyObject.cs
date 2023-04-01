using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace EMX.HierarchyPlugin.Editor
{



#pragma warning disable
	internal struct AutoHighlighterColor
	{
		internal bool filterAssigned;
		internal TempColorClass filterColor;

		internal bool internalIcon;
		internal TempColorClass drawIcon;
		internal TempColorClass localTempColor;
		internal Color BACKGROUNDEsourceBgColorD;
		internal int _BACKGROUNDED;
		internal int BACKGROUNDED {
			get { return _BACKGROUNDED; }
			set {
				//Debug.Log("set - " + value);
				_BACKGROUNDED = value;
			}
		}
		internal int FLAGS;
		internal TempColorClass MIXINCOLOR;

		internal Rect? BG_RECT;
		internal int switchType;
	}
#pragma warning restore

	internal class HierarchyObject : IEqualityComparer<HierarchyObject>, IComparable<HierarchyObject>, IEquatable<HierarchyObject>, ICloneable
	{



		internal int pluginID;
		internal int id;
		internal GameObject go;
		internal HierarchyObject_ProjectExt project;
		internal bool InvalideProjectAsset = false;
		internal Component cachedComp;
		Type cachedType = null;
#pragma warning disable
		internal AutoHighlighterColor ah;
#pragma warning restore
		internal HierarchyObject( int pluginID )
		{
			this.pluginID = pluginID;
		}



		//   internal bool cache_prefab;
		//   internal int switchType;
		//   internal Rect? BG_RECT = null;


		internal UnityEngine.Object _GetHardLoadObjectSlow; // return InternalEditorUtility.GetLoadedObjectFromInstanceID( id );
		internal UnityEngine.Object GetHardLoadObjectSlow() // return InternalEditorUtility.GetLoadedObjectFromInstanceID( id );
		{
			if ( pluginID == 0 ) return go;
			if ( _GetHardLoadObjectSlow == null ) _GetHardLoadObjectSlow = EditorUtility.InstanceIDToObject( id );
			return _GetHardLoadObjectSlow;
		}


		internal bool CompAsNull, HasMissing;
		static Component[] _comps_empty = new Component[0];
		Component[] _comps;
		bool?[] enabled = new bool?[0];
		internal Component[] GetComponents()
		{
			if ( pluginID == 1 ) return _comps_empty;
			if ( _comps != null ) return _comps;
			if ( !go ) return _comps_empty;
			_comps = go.GetComponents<Component>();
			for ( int i = 0; i < _comps.Length; i++ ) if ( !_comps[ i ] ) HasMissing = true;
			if ( _comps.Length == 1 ) CompAsNull = true;
			if ( HasMissing ) _comps = _comps.Where( c => c ).ToArray();
			if ( enabled.Length != _comps.Length ) Array.Resize( ref enabled, _comps.Length );
			return _comps;
		}
		internal bool CheckComponents()
		{
			if ( _comps == null ) return false;
			if ( enabled.Length != _comps.Length ) Array.Resize( ref enabled, _comps.Length );
			bool res = false;
			for ( int i = 0; i < enabled.Length; i++ )
			{
				if ( !(_comps[ i ] is Behaviour) ) continue;
				var m = _comps[i] as Behaviour;
				if ( !m ) continue;
				if ( !enabled[ i ].HasValue ) enabled[ i ] = m.enabled;
				if ( enabled[ i ] != m.enabled )
				{
					res = true;
					enabled[ i ] = m.enabled;
				}
			}
			return res;
		}


		/// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		internal int scene {
			get {
				if ( pluginID == 0 ) return go.scene.GetHashCode();
				return -1;
			}
		}
		public override string ToString()
		{
			if ( pluginID == 0 ) return go ? go.name : "";


			return project != null ? !string.IsNullOrEmpty( project.assetName ) ? project.assetName : project.assetPath : "";
		}

		public string name {
			get { return ToString(); }
		}

		bool? _HasBrefabButton;
		public bool HasBrefabButton {
			get {
				if ( !_HasBrefabButton.HasValue )
				{
					if ( !ROOT_P.ha.hasShowingPrefabHeader ) return (_HasBrefabButton = false).Value;
					var tree = GetTreeItem();
					_HasBrefabButton = (bool)ROOT_P.showPrefabModeButton.GetValue( tree, null );
				}
				return _HasBrefabButton.Value;
			}

		}

		internal bool Validate()
		{
			if ( pluginID == 0 )
			{

				return go;
			}
			return true;
			// return !string.IsNullOrEmpty( AssetDatabase.AssetPathToGUID( project.assetPath ) );
		}

		internal bool Validate( int sceneHash )
		{
			if ( pluginID == 0 ) return go && go.scene.GetHashCode() == sceneHash;
			return true;
			// return !string.IsNullOrEmpty( AssetDatabase.AssetPathToGUID( project.assetPath ) );
		}



		internal bool Validate( bool checkScene )
		{
			if ( pluginID == 0 ) return go && go.scene.IsValid();
			return true;
			// return !string.IsNullOrEmpty( AssetDatabase.AssetPathToGUID( project.assetPath ) );
		}

		static HierarchyObject()
		{
			NewClearHelper.OnFontSizeChanged += clear_labels_size;
		}
		static void clear_labels_size()
		{
			content_size.Clear();
		}
		static Dictionary<int, Vector2> content_size = new Dictionary<int, Vector2>();
		static Vector2 tv2;
		static GUIContent tcont = new GUIContent();
		PluginInstance ROOT_P { get { return Root.p[ 0 ]; } }
		internal Vector2 GetContentSize()
		{

			var key = ToString().GetHashCode() ^ ROOT_P.ha.INTERNAL_LABEL_STYLES[0].style.fontSize;
			if ( !content_size.TryGetValue( key, out tv2 ) )
			{
				tcont.text = ToString();
				content_size.Add( key, tv2 = ROOT_P.ha.INTERNAL_LABEL_STYLES[ 0 ].style.CalcSize( tcont ) );
			}
			return tv2;

		}

		internal Type GET_TYPE()
		{
			if ( pluginID == 0 ) return go.GetType();
			if ( cachedType == null ) //InternalEditorUtility.GetTypeWithoutLoadingObject
			{
				cachedType = Tools.GetTypeByInstanceID( id, pluginID );
				if ( cachedType == null ) cachedType = typeof( UnityEngine.Object );
			}

			return cachedType;
		}

		bool lastActive;
		internal bool Active()
		{
			if ( pluginID == 0 )
			{
				if ( PluginInstance.KeepLastActiveGameObbject ) return lastActive;
				if ( lastActive != go && go.activeInHierarchy ) lastActive = go && go.activeInHierarchy;
				return go && go.activeInHierarchy;
			}

			return true;
		}


		internal HierarchyObject[] GetAllParents()
		{
			var result = new List<HierarchyObject>();
			var current = parent();

			while ( current != null )
			{
				result.Add( current );
				if ( current.parent() == current ) break;
				current = current.parent();
			}

			return result.ToArray();
		}

		internal HierarchyObject rootSlow()
		{
			return GetAllParents().LastOrDefault() ?? this;
		}

		internal HierarchyObject parent()
		{
			if ( pluginID == 0 )
			{
				if ( !go ) return null;
				var p = go.transform.parent;
				if ( !p ) return null;
				return Cache.GetHierarchyObjectByInstanceID( p.gameObject );
			}




			if ( project.assetFolder == null || project.assetFolder == "" || project.assetFolder == "" ) return null;
			if ( !project.IsMainAsset ) return Cache.m_PathToObject[ project.assetPath ];
			if ( !Cache.m_PathToObject.ContainsKey( project.assetFolder ) )
			{
				HierarchyObject load;
				//if ( _visibleTreeItem == null ) _visibleTreeItem = GetTreeItem();

				if ( _visibleTreeItem != null && _visibleTreeItem.parent != null )
				{
					load = Cache.GetHierarchyObjectByGUID( _visibleTreeItem.parent.id );
				}
				else
				{
					var guid = AssetDatabase.AssetPathToGUID(project.assetFolder);
					if ( string.IsNullOrEmpty( guid ) ) return null;
					load = Cache.GetHierarchyObjectByGUID( ref guid, ref project.assetFolder, null );
				}

				if ( !Cache.m_PathToObject.ContainsKey( project.assetFolder ) )
					Cache.m_PathToObject.Add( project.assetFolder, load );
				if ( load == null ) return null;
			}
			return Cache.m_PathToObject[ project.assetFolder ];
		}

		internal int GetSiblingIndex_Cache()
		{
			if ( pluginID == 0 ) return go.transform.GetSiblingIndex();

			if ( !_sibling_memory.ContainsKey( id ) ) return 0;
			return _sibling_memory[ id ];
			//throw new Exception("ASD");
			//return project.sibling;
		}


		/// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


		int backedLastSibForProject()
		{
			var item = GetTreeItem();
			if ( item == null || item.children.Count == 0 ) return 0;
			return item.children[ item.children.Count - 1 ].id;
			/* if ( !__mBackedLastSib.HasValue )
             {
                 var item = GetTreeItem();
                 if ( item == null ) return 0;
                 var children = adapter.m_data_treeitem_children.GetValue(item, null) as IList;
                 if ( children == null || children.Count == 0 ) __mBackedLastSib = 0;
                 else __mBackedLastSib = (int)adapter.m_data_treeitem_m_ID.GetValue( children[ children.Count - 1 ] );
             }
             return __mBackedLastSib.Value;*/
		}
		//int? __mBackedLastSib;


		internal UnityEditor.IMGUI.Controls.TreeViewItem _visibleTreeItem;
		internal Rect lastFullLineRect;
		internal Rect lastSelectionRect;

		internal UnityEditor.IMGUI.Controls.TreeViewItem GetTreeItem()
		{
			if ( _visibleTreeItem != null ) return _visibleTreeItem;
			return (_visibleTreeItem = ROOT_P.GetTreeItem( id ));
		}


		internal bool IsLastSibling()
		{
			if ( pluginID == 0 ) return go.transform.parent && go.transform.GetSiblingIndex() == go.transform.parent.childCount - 1;



			if ( _visibleTreeItem == null ) return false;
			var p2 =  _visibleTreeItem.parent;
			if ( p2 != null && p2.children[ p2.children.Count - 1 ] != null )
				return p2.children[ p2.children.Count - 1 ].id == id;
			return false;
			// {
			//     var p2 =  _visibleTreeItem.parent;
			//     if ( p2 != null &&  p2.children[ p2.children.Count - 1 ] != null )
			//     {
			//         //if ( p._visibleTreeItem.children.Count == 3 ) Debug.Log//( p._visibleTreeItem.displayName + " " + /p._visibleTreeItem.children.Count );
			//         return p2.children[ p2.children.Count - 1 ].id == id;
			//     }
			//     return false;
			// }
			// //var p = parent();
			// //return p != null && p.backedLastSibForProject() == id;
			// var p = parent();
			// 
			// if ( p == null ) return false;
			//
			// ///if ( p.GetTreeItem() != null && p._visibleTreeItem.hasChildren && //p._visibleTreeItem.children[ 0 ] != null )
			// //p._visibleTreeItem = p.GetTreeItem();
			//
			//
			// if ( p._visibleTreeItem != null && p._visibleTreeItem.hasChildren && //p._visibleTreeItem.children[ p._visibleTreeItem.children.Count - 1 ] != null )
			// {
			//     //if ( p._visibleTreeItem.children.Count == 3 ) Debug.Log//( p._visibleTreeItem.displayName + " " + /p._visibleTreeItem.children.Count );
			//     return p._visibleTreeItem.children[ p._visibleTreeItem.children.Count /- /1 ].id == id;
			// }
			//
			// return false;
			//
			// //if ( !_sibling_count.ContainsKey( p.id ) ) return false;
			// //if ( !_sibling_memory.ContainsKey( id ) ) return false;
			// //
			// //return _sibling_memory[ id ] == _sibling_count[ p.id ];
		}



		public int parentCount()
		{
			if ( pluginID == 0 ) return go.GetComponentsInParent<Transform>( true ).Length;

			if ( project.parentCount.HasValue ) return project.parentCount.Value;

			project.parentCount = Math.Max( 0, project.assetPath.ToCharArray().Count( c => c == '/' ) - 1 );
			return project.parentCount.Value;
		}

		internal bool ParentIsNull()
		{
			if ( pluginID == 0 ) return !go.transform.parent;
			return project.assetFolder == "" || project.assetFolder == null || project.assetFolder == "Assets";
		}

		//  static SortedList<string, HierarchyObject> tl;
		// static object[] ob_arr = new object[1];
		//int? backedChild;
		internal void WriteSibling()
		{
			if ( Root.p[ 0 ].EVENT.type == EventType.Layout )
			{
				if ( _sibling_count.ContainsKey( id ) ) _sibling_count[ id ] = -1;
			}
			else
			{
				var p = parent();
				if ( p == null ) return;
				if ( !_sibling_count.ContainsKey( p.id ) ) _sibling_count.Add( p.id, -1 );
				_sibling_count[ p.id ]++;
				if ( !_sibling_memory.ContainsKey( id ) ) _sibling_memory.Add( id, _sibling_count[ p.id ] );
				else _sibling_memory[ id ] = _sibling_count[ p.id ];
			}
		}
		internal static Dictionary<int, int> _sibling_memory = new Dictionary<int, int>();
		internal static Dictionary<int, int> _sibling_count = new Dictionary<int, int>();
		internal static Dictionary<string, int> _child_count = new Dictionary<string, int>();
		internal int ChildCount()
		{
			if ( pluginID == 0 ) return go ? go.transform.childCount : 0;

			/*if ( !project.IsFolder ) return 0;
            adapter.GetPathToChildrens( ref project.assetPath , out tl );
            return tl.Count;*/

			//    if ( backedChild.HasValue ) return backedChild.Value;

			/*   var data = adapter.m_data.GetValue(adapter.m_TreeView(adapter.window()), null);
               ob_arr[ 0 ] = id;
               var item = adapter.m_dataFindItem.Invoke(data, ob_arr);*/
			//   if (name == "WindowsScripts")
			//{
			//	Debug.Log("ASD");
			//}

			//	if (_visibleTreeItem != null && _visibleTreeItem.hasChildren) Debug.Log(name + " " + _visibleTreeItem.children.Count);



			if ( _visibleTreeItem != null && !_visibleTreeItem.hasChildren ) return 0;

			if ( _visibleTreeItem != null && _visibleTreeItem.children[ _visibleTreeItem.children.Count - 1 ] != null ) return _visibleTreeItem.children.Count;


			//return _visibleTreeItem.hasChildren ? _visibleTreeItem.children.Count : 0;
			//var item = GetTreeItem();
			//if (item == null) return 0;
			//return item.children.Count;
			if ( project.assetPath == null || project.assetPath == "" ) return 0;


			if ( !_child_count.ContainsKey( project.assetPath ) )
			{
				if ( !project.IsFolder ) _child_count.Add( project.assetPath, 0 );
				else
				{
					//	Debug.Log(Folders.UNITY_SYSTEM_PATH + project.assetPath);
					//var td = Directory.GetDirectories(Folders.UNITY_SYSTEM_PATH + project.assetPath, "*.*", SearchOption.TopDirectoryOnly);
					//var tf = Directory.GetFiles(Folders.UNITY_SYSTEM_PATH + project.assetPath, "*.*", SearchOption.TopDirectoryOnly);
					// _child_count.Add( project.assetPath,
					//   td.Count( d => File.Exists( d + ".meta" ) ) +
					//   tf.Count( f => !f.EndsWith( ".meta" ) && f[ 0 ] != '~' && f[ 0 ] != '.' ) );
					if ( !Directory.Exists( Folders.UNITY_SYSTEM_PATH + project.assetPath ) )
					{
						FORCE_ASSET_ON_IMPORT();
						_child_count.Add( project.assetPath, 0 );
					}
					else
					{
						var td = Directory.GetDirectories(Folders.UNITY_SYSTEM_PATH + project.assetPath, "*.meta", SearchOption.TopDirectoryOnly);
						var tf = Directory.GetFiles(Folders.UNITY_SYSTEM_PATH + project.assetPath, "*.meta", SearchOption.TopDirectoryOnly);
						_child_count.Add( project.assetPath, td.Length + tf.Count( f => f[ 0 ] != '~' && f[ 0 ] != '.' ) );
					}

				}
			}
			return _child_count[ project.assetPath ];

			/* ob_arr[0] = row;
             var item =  adapter.m_dataGetItem.Invoke(data, ob_arr );
             TreeViewUtility.*/
			// var children = adapter.m_data_treeitem_children.GetValue(item, null) as IList;
			//  return (backedChild = (children != null ? children.Count : 0)).Value;
			/*
            if (project.childCount.HasValue) return project.childCount.Value;
            //if ( project.assetName != null ) return (project.childCount = 0).Value;
            if (!project.IsFolder) return 0;
            //adapter.GetPathToChildrens( ref project.assetPath , out tl );
            
            //project.childCount = adapter.m_PathToChildrens.ContainsKey( project.assetPath ) ? adapter.m_PathToChildrens[project.assetPath].Count : 0;
            
            project.childCount =
                Directory.GetDirectories( UNITY_SYSTEM_PATH + project.assetPath, "*.*", SearchOption.TopDirectoryOnly ).Count( d => File.Exists( d + ".meta" ) ) +
                Directory.GetFiles( UNITY_SYSTEM_PATH + project.assetPath, "*.*", SearchOption.TopDirectoryOnly )
                    .Count( f => !f.EndsWith( ".meta" ) );
            
            //if (project.assetName == "lightmaps") Debug.Log(project.childCount.Value);
            
            return (project.childCount).Value;*/
		}



		void FORCE_ASSET_ON_IMPORT()
		{

		}



		/// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


		/*
        long? m_fileID;
        internal long fileID
        {
            get
            {
                if ( m_fileID.HasValue ) return m_fileID.Value;
                if ( pluginID != 0 ) m_fileID = project.guid.GetHashCode();
                else
                    m_fileID = go ? ObjectTools.GetFileIDWithOutPrefabChecking( ObjectTools.GetPrefabInstanceHandle( go ) as GameObject, go ) : 0;
                return m_fileID.Value;
            }
            set
            {
                if ( value == 0 ) return;
                m_fileID = value;
            }
        }

            */




		public object Clone()
		{
			var result = (HierarchyObject)MemberwiseClone();
			//  result.localTempColor = new TempColorClass().AssignFromList( new SingleList() { list = Enumerable.Repeat( 0, 9 ).ToList() }, true );
			// result.localTempColor = new TempColorClass().AssignFromList( 0, true );
			//result.m_fileID = null;  ////////////////// WERE TRUE
			result.project = (HierarchyObject_ProjectExt)result.project.Clone();
			return result;
		}




		/// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



		public static bool operator ==( HierarchyObject x, HierarchyObject y )
		{
			if ( ReferenceEquals( x, null ) || ReferenceEquals( y, null ) ) return ReferenceEquals( x, null ) && ReferenceEquals( y, null );

			return x.Equals( y );
		}

		public static bool operator !=( HierarchyObject x, HierarchyObject y )
		{
			return !(x == y);
		}

		public bool Equals( HierarchyObject x, HierarchyObject y )
		{
			if ( ReferenceEquals( x, null ) || ReferenceEquals( y, null ) ) return ReferenceEquals( x, null ) && ReferenceEquals( y, null );
			//if ( x == null || y == null ) return x == null && y == null;
			return x.Equals( y );
		}

		public int GetHashCode( HierarchyObject obj )
		{
			return obj.GetHashCode();
		}

		public int CompareTo( HierarchyObject other )
		{
			if ( pluginID == 0 ) return id - other.id;
			// if ( pluginID == 0 ) return (int)((fileID - other.fileID) % int.MaxValue);

			return project.guid.CompareTo( other.project.guid );
		}

		public bool Equals( HierarchyObject other )
		{
			if ( ReferenceEquals( other, null ) ) return false;
			//  if ( pluginID == 0 ) return fileID == other.fileID;
			if ( pluginID == 0 ) return id == other.id;
			if ( project == null || other.project == null ) return false;
			return project.guid == other.project.guid;
		}

		public override int GetHashCode()
		{
			//   if ( pluginID == 0 ) return (int)(fileID % int.MaxValue);
			if ( pluginID == 0 ) return id;
			else return project.guid.GetHashCode();
		}

		public override bool Equals( object obj )
		{
			if ( ReferenceEquals( obj, null ) ) return false;

			return GetHashCode() == obj.GetHashCode();
		}

		bool init_GetIconForExternal;
		TempColorClass _GetIconForExternal;
		static TempColorClass _GetIconForExternal_Empty = new TempColorClass();
		internal CachedRect[] lastContentRectLayout = new CachedRect[6];
		internal int? hierarchyIndex;

		internal TempColorClass GetIconForExternal()
		{
			if ( !init_GetIconForExternal )
			{
				if ( !go ) return _GetIconForExternal_Empty;

				init_GetIconForExternal = true;
				_GetIconForExternal = Cache.LOAD_CUSTOM_ICON_USING_HIGHLIGHTER( this );
				if ( _GetIconForExternal == null ) _GetIconForExternal = TODO_Tools.GetObjectBuildinIcon( go, Tools.unityGameObjectType );
			}
			return _GetIconForExternal ?? _GetIconForExternal_Empty;
		}
	}

	internal struct CachedRect
	{
		internal Rect rect;
		internal bool assigned;

		internal void SET( ref Rect r )
		{
			assigned = true;
			rect = r;
		}
		internal void CLEAR()
		{
			assigned = false;
		}
	}


	internal class HierarchyObject_ProjectExt : ICloneable
	{
		//internal UnityEngine.Object obj;
		internal string guid;
		internal string assetPath;
		internal string assetFolder;
		internal string assetName;
		internal string fileExtension;
		internal bool IsFolder;

		internal Dictionary<int, HierarchyObject> nonMainAssets;

		//#pragma warning disable
		internal bool IsMainAsset = true;
		// internal int? childCount;
		//#pragma warning restore

		internal int? parentCount;
		//public int sibling;

		public object Clone()
		{
			return MemberwiseClone();
		}
	}



}
