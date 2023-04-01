using System;
using System.Collections.Generic;
using System.Linq;

using EMX.HierarchyPlugin.Editor.Mods.BookObject;

using UnityEditor;

using UnityEngine;



namespace EMX.HierarchyPlugin.Editor.Mods
{


	internal class BottomBarWindowInstance : IExternalWindow
	{

		internal HyperGraph.HyperGraphModInstance hy;
		internal BookmarksforGameObjectsModInstance m_custom;
		internal LastSelectionHistoryModInstance m_last;
		internal HierarchyExpandedMemInstance m_hier;
		internal ScenesHistoryModInstance m_scene ;

		internal ExternalDrawContainer HierarchyController;//, HyperGraphController;
														   // internal ExternalDrawContainer HyperGraphController { get { return HierarchyController; } set { HierarchyController = value; } }
		int id, pluginId;

		public bool FORCE_REPAINT_THROUGH_LAYOUT { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		int GET_VALUE( string key, int d ) { return adapter.par_e.GET( "BOTTOMBAR_TEMP_" + key + '_' + id, d ); }
		void SET_VALUE( string key, int v ) { adapter.par_e.SET( "BOTTOMBAR_TEMP_" + key + '_' + id, v ); }
		bool GET_VALUE( string key, bool d ) { return adapter.par_e.GET( "BOTTOMBAR_TEMP_" + key + '_' + id, d ); }
		void SET_VALUE( string key, bool v ) { adapter.par_e.SET( "BOTTOMBAR_TEMP_" + key + '_' + id, v ); }


		//internal int GRAPH_HEIGHT()
		//{
		//    return GET_VALUE( "GRAPH_HEIGHT", 200 );
		//}


		internal bool BOTTOM_AUTOHIDE {
			get { return GET_VALUE( "BOTTOM_AUTOHIDE", false ); }
			set {
				SET_VALUE( "BOTTOM_AUTOHIDE", value );
				adapter.modsController.REBUILD_PLUGINS_FAST();
			}
		}


		float? _TEMP_GRAPH_HEIGHT = null;
		internal int GRAPH_HEIGHT {
			get {
				if ( !adapter.par_e.USE_HYPERGRAPH_MOD ) return 0;
				if ( !_TEMP_GRAPH_HEIGHT.HasValue ) Update();
				return Mathf.RoundToInt( _TEMP_GRAPH_HEIGHT.Value );
			}
		}
		internal int GRAPH_HEIGHT_MEM {
			get { return Math.Max( 20, GET_VALUE( "GRAPH_HEIGHT", 200 ) ); }
			set { SET_VALUE( "GRAPH_HEIGHT", value ); }
		}
		internal bool GRAPH_ENABLED {
			get { return GET_VALUE( "GRAPH_ENABLED", false ); }
			set {
				SET_VALUE( "GRAPH_ENABLED", value );
				adapter.modsController.REBUILD_PLUGINS_FAST();
			}
		}
		internal bool HYPER_FULL_ENABLE {
			get { return GRAPH_ENABLED; }
			//set { GRAPH_ENABLED = value; }
		}

		internal BottomBarWindowInstance( int ind, int pluginId )
		{
			id = ind;
			this.pluginId = pluginId;
			HierarchyController = new ExternalDrawContainer( 0 ) { controller_type = 1 };
			// HyperGraphController = new ExternalDrawContainer( 0 ) { controller_type = 1 };
			hy = new HyperGraph.HyperGraphModInstance( () => HierarchyController );
			m_custom = new BookmarksforGameObjectsModInstance();
			m_hier = new HierarchyExpandedMemInstance();
			m_scene = new ScenesHistoryModInstance();
			m_last = new LastSelectionHistoryModInstance();
		}


		Events.MouseRawUp _mouse_uo_helper;
		internal Events.MouseRawUp mouse_uo_helper { get { return _mouse_uo_helper ?? (_mouse_uo_helper = new Events.MouseRawUp()); } }


		public bool PUSH_EVENT_HELPER_RAW( Events.MouseRawUp.WantMouseLeaveType t )
		{
			//Debug.Log( "123" );
			//return ExternalModRoot.ON_GUI_POST_PUSH_EVENT_HELPER_RAW( HyperGraphController, this, mouse_uo_helper, t );
			return ExternalModRoot.ON_GUI_POST_PUSH_EVENT_HELPER_RAW( HierarchyController, this, mouse_uo_helper, t );
		}

		public void PUSH_ONMOUSEUP( Func<Events.MouseRawUp.WantMouseLeaveType, bool> ac )
		{
			//Debug.Log( "ASD" );
			ExternalModRoot.ON_GUI_POST_PUSH_PUSH_ONMOUSEUP( mouse_uo_helper, this, ref ac );
		}


		//public bool PUSH_EVENT_HELPER_RAW( MouseRawUp.WantMouseLeaveType t )
		//{
		//    throw new NotImplementedException();
		//}
		//
		//public void PUSH_ONMOUSEUP( Func<MouseRawUp.WantMouseLeaveType, bool> a )
		//{
		//    throw new NotImplementedException();
		//}

		public void RepaintNow()
		{
			//Debug.Log( "re" );
			adapter.RepaintWindowInUpdate( pluginId );
			//if ( w.Instance ) w.Instance.Repaint();
		}
		public void Close()
		{
			GRAPH_ENABLED = false;
			//BOTTOM_AUTOHIDE = !BOTTOM_AUTOHIDE;
			RepaintNow();
		}
		public bool OvValide()
		{
			//return w.Instance;
			if ( w == null ) return false;
			if ( w.bottomParams == null ) return false;
			var r = w.Instance;
			if ( !r ) return false;
			//if ( !p.window.thisIsAFirstWindow)
			return true;
		}

		public EditorWindow GET_WIN()
		{
			return w.Instance;
		}
		GUIContent __titleContent = new GUIContent();
		public GUIContent titleContent {
			get {
				return __titleContent;
			}
			set { }
		}


		internal Rect? H_POS;
		internal Rect? C_POS;
		internal Rect? S_POS;
		internal Rect? L_POS;
		internal Rect? E_POS;

		public Rect position( int index )
		{
			switch ( index )
			{
				case IExternalWindowType.HYPER_GRAPH: return H_POS.Value;
				case IExternalWindowType.CUSTOM: return C_POS.Value;
				case IExternalWindowType.SCENE: return S_POS.Value;
				case IExternalWindowType.LAST: return L_POS.Value;
				case IExternalWindowType.EXPAND: return E_POS.Value;
			}
			throw new Exception( "Cannot find the type: " + index );
			// get {
			//  return GetNavigatorRect();
			// }
			//set { }
		}


		BottomBarExtension bar;
		internal Window w;
		internal static List<BottomBarWindowInstance> _AssignList = new List<BottomBarWindowInstance>();
		internal static BottomBarWindowInstance Assign( BottomBarExtension bar, Window w )
		{
			int freeInd = 0;
			while ( _AssignList.Any( a => a.id == freeInd ) ) freeInd++;
			w.bottomParams = new BottomBarWindowInstance( freeInd, bar.PluginId ) {
				w = w,
				bar = bar
			};
			_AssignList.Add( w.bottomParams );
			return w.bottomParams;
		}
		bool lastHyperEnabled = false;
		internal void SubscribeEditorInstance( AdditionalSubscriber hypersbs )
		{
			if ( adapter.par_e.USE_HYPERGRAPH_MOD && GRAPH_ENABLED )
			{
				hy.SubscribeEditorInstance( hypersbs );
				if ( lastHyperEnabled != GRAPH_ENABLED )
					hy.SMOOTH_SCENE_CHANGE_ORBREAK();
				lastHyperEnabled = true;
			}
			else
			{
				lastHyperEnabled = false;
			}
			if ( adapter.par_e.USE_BOOKMARKS_HIERARCHY_MOD && bar.SHOW_BOOKMARKS_ROWS
				&& (!BOTTOM_AUTOHIDE || adapter.WIN_SET.BOTTOMBAR_SHOW_PARENT_TREE) ) m_custom.SubscribeEditorInstance( hypersbs );

			// if ( !BOTTOM_AUTOHIDE )
			{
				if ( adapter.par_e.USE_LAST_SELECTION_MOD && bar.SHOW_LAST_ROWS ) m_last.SubscribeEditorInstance( hypersbs );
				if ( adapter.par_e.USE_HIER_EXPANDED_MOD && bar.SHOW_HIERARCHYSLOTS_ROWS ) m_hier.SubscribeEditorInstance( hypersbs );
				if ( adapter.par_e.USE_LAST_SCENES_MOD && bar.SHOW_SCENES_ROWS ) m_scene.SubscribeEditorInstance( hypersbs );
			}


		}

		internal void UnAssign()
		{
			if ( w == null ) return;
			w.bottomParams = null;
			w = null;
		}
		PluginInstance adapter { get { return w.p; } }



		private const int BOTTOMPADDINGOFFSET = 0;




		public void Update()
		{

			//if ( needRepaint )
			//{
			//    needRepaint = false;
			//
			//    if ( editorWindow )
			//        editorWindow.Repaint();
			//    else
			//        adapter.RepaintWindowInUpdate();
			//}
			var target =GRAPH_ENABLED ? CHECK_HEIGHT(GRAPH_HEIGHT_MEM):0;
			if ( _TEMP_GRAPH_HEIGHT != target )
			{
				if ( !OvValide() )
				{
					_TEMP_GRAPH_HEIGHT = target;
					return;
				}
				if ( !_TEMP_GRAPH_HEIGHT.HasValue )
				{
					_TEMP_GRAPH_HEIGHT = target;
					_TEMP_GRAPH_HEIGHT = target = GRAPH_ENABLED ? CHECK_HEIGHT( GRAPH_HEIGHT_MEM ) : 0;
				}
				else
				{
					var oldH = _TEMP_GRAPH_HEIGHT;
					_TEMP_GRAPH_HEIGHT = Mathf.MoveTowards( _TEMP_GRAPH_HEIGHT.Value, target, adapter.deltaTime * 1600 );
					if ( _TEMP_GRAPH_HEIGHT != oldH )
					{
						//adapter.RESET_SMOOTH_HEIGHT();
						adapter.RepaintWindowInUpdate( pluginId );
					}
				}

			}



			//if ( bottomInterface.ENABLE_FAVORGUI() )
			//{
			//    var oldH = bottomInterface.m_FAV_HEIGHT();
			//    bottomInterface._FAV_HEIGHT = Mathf.MoveTowards( bottomInterface.m_FAV_HEIGHT(),
			//                                  adapter.FAV_ENABLE() ? adapter.par.FavoritesNavigatorParams.HEIGHT : 0, adapter.deltaTime * 1600 );
			//
			//    if ( bottomInterface._FAV_HEIGHT != oldH )
			//    {   //  MonoBehaviour.print(deltaTime);
			//        adapter.RESET_SMOOTH_HEIGHT();
			//        adapter.RepaintWindowInUpdate();
			//    }
			//}
		}


		internal int Y_POS()
		{
			var H = HEIGHT ;
			return Mathf.FloorToInt( w.GUIClip_visibleRect.y + w.GUIClip_visibleRect.height - H ); // 
		}

		//static bool initStop = false;
		internal bool Clip( ref Rect selectionRect )
		{
			if ( bar.BOTTOM_DRAWFOR_ONEHIERARHYWIN && !w.thisIsAFirstWindow )
			{
				if ( w.bottomParams != null ) w.bottomParams = null;
				return false;
			}
			//var H = HEIGHT + EditorGUIUtility.singleLineHeight - BOTTOMPADDINGOFFSET;
			if ( selectionRect.y + selectionRect.height < Y_POS() ) return false;
			if ( adapter.s != null ) adapter.s.SetValue( adapter.TreeController_current, true );
			// Debug.Log( row + " " + " " + numVisibleRows + " " + thisIsLast );
			// Debug.Log( "ASD" );
			return true;
		}



		Rect oldr;

		internal Rect GetNavigatorRect() //ref Rect selectRect
		{
			// oldr.Set( adapter.TOTAL_LEFT_PADDING_FORBOTTOM,
			//           Mathf.RoundToInt( adapter.window().position.height + adapter.HierWinScrollPos.y -
			//                             (EditorGUIUtility.singleLineHeight - BOTTOMPADDINGOFFSET) ),
			//           width - adapter.TOTAL_LEFT_PADDING_FORBOTTOM,
			//           (HEIGHT) + (EditorGUIUtility.singleLineHeight - BOTTOMPADDINGOFFSET) );
			// return oldr;
			//- adapter.TOTAL_LEFT_PADDING_FORBOTTOM
			var W = adapter.fullLineRect.x + adapter.fullLineRect.width;
			var H = HEIGHT; // + EditorGUIUtility.singleLineHeight - BOTTOMPADDINGOFFSET
			oldr.Set( 0, Y_POS(), W, H );
			//oldr.Set( 0, Mathf.RoundToInt( w.position.height + adapter.scrollPos.y - H ), W, H );
			return oldr;
			//var tr = (Rect)adapter.tree_m_TotalRect.GetValue( adapter.TreeController_current ); ;
			//tr.height -= 100;
			//adapter.tree_m_TotalRect.SetValue( adapter.TreeController_current, tr );
		}





		internal int HEIGHT {
			get {
				//if ( !_HEIGHT.HasValue ) _HEIGHT = adapter.ENABLE_BOTTOMDOCK_PROPERTY ? REFERENCE_HEIGHT : 0;
				//return Mathf.FloorToInt( !adapter.ENABLE_BOTTOMDOCK_PROPERTY ? 0 : (_HEIGHT ?? 0) );
				//if ( !_HEIGHT.HasValue ) _HEIGHT = REFERENCE_HEIGHT;
				//return Mathf.FloorToInt( (_HEIGHT ?? 0) );
				return REFERENCE_HEIGHT;
			}

			//set { _HEIGHT = value; }
		}

		internal int CHECK_HEIGHT( int h )
		{
			if ( w == null || !GET_WIN() || !_TEMP_GRAPH_HEIGHT.HasValue ) return h;
			//return h;
			var res = h;
			try
			{
				res = Mathf.Clamp( h, 20, Math.Max( 20, Mathf.RoundToInt( (GET_WIN().position.height - HEIGHT + GRAPH_HEIGHT) * 0.8f ) ) );
				if ( res != h ) GRAPH_HEIGHT_MEM = res;
			}
			catch ( Exception ex )
			{
				Debug.LogWarning( ex.Message + "\n" + ex.StackTrace );
			}
			return res;
		}





		//float? ___heightcache;
		//internal float? _HEIGHT {
		//    get {
		//        if ( !___heightcache.HasValue ) ___heightcache = EditorPrefs.GetFloat( adapter.pluginname + "/" + "cached_height", -1 );
		//
		//        return ___heightcache.Value == -1 ? (float?)null : ___heightcache.Value;
		//    }
		//
		//    set {
		//        if ( value == null ) value = -1;
		//
		//        if ( ___heightcache != value ) EditorPrefs.SetFloat( adapter.pluginname + "/" + "cached_height", value.Value );
		//
		//        ___heightcache = value;
		//    }
		//}

		internal int ___REFERENCE_HEIGHT_AUTOHIDE()     //var h = LINE_HEIGHT(true, true);
		{
			return /*h * adapter.par.BOTTOM_MAXCUSTOMROWS +*/ Mathf.RoundToInt( bar.LINE_REFERENCE_HEIGHT + BOTTOMPADDINGOFFSET + 1 + GRAPH_HEIGHT );
		}



		internal int ___REFERENCE_HEIGHT_FULL()     //   var h1 = LINE_HEIGHT(true, true);
		{   //  var h = LINE_HEIGHT(true);
			var otherH = BOTTOMPADDINGOFFSET + 1 + GRAPH_HEIGHT + bar.LINE_REFERENCE_HEIGHT;
			var RowsParams = DrawButtonsOld.ISET_ROWS_ARRAY;
			float fh = 0;
			for ( int i = 0; i < RowsParams.Length; i++ ) fh += RowsParams[ i ].FULL_HEIGHT;
			return Mathf.RoundToInt( fh ) + otherH;
			// return h1 * adapter.par.BOTTOM_MAXCUSTOMROWS + LINE_REFERENCE_HEIGHT + h +  +                       h * adapter.par.BOTTOM_MAXLASTROWS +
		}



		internal int REFERENCE_HEIGHT {
			get {
				var result = (BOTTOM_AUTOHIDE ? ___REFERENCE_HEIGHT_AUTOHIDE() : ___REFERENCE_HEIGHT_FULL());
				// if (adapter.par.USE_HORISONTAL_SCROLL) result += EditorGUIUtility.singleLineHeight;
				return result;
			}
		}



		// internal int HEIGHT {
		//     get {
		//
		//         if ( adapter.DISABLE_DES() ) return 0;
		//
		//         // if ( adapter.m_SearchFilterString != null && !string.IsNullOrEmpty( (string)adapter.m_SearchFilterString.GetValue( adapter.window() ) ) ) return 0;
		//         if ( adapter.IS_SEARCH_MODE_OR_PREFAB_OPENED() ) return 0;
		//
		//         if ( !_HEIGHT.HasValue ) _HEIGHT = adapter.ENABLE_BOTTOMDOCK_PROPERTY ? REFERENCE_HEIGHT : 0;
		//
		//         return Mathf.FloorToInt( !adapter.ENABLE_BOTTOMDOCK_PROPERTY ? 0 : (_HEIGHT ?? 0 /*+ HIPER_HEIGHT()*/) );
		//         //return !adapter.par.ENABLE_ALL ? 0 : (_HEIGHT.Value /*+ HIPER_HEIGHT()*/);
		//     }
		//
		//     set { _HEIGHT = value; }
		// }









	}

}