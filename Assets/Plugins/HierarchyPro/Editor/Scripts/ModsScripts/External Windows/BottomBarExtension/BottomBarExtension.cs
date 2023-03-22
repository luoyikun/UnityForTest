using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using EMX.HierarchyPlugin.Editor.Mods.BookObject;
using EMX.HierarchyPlugin.Editor.Mods.HyperGraph;
using EMX.HierarchyPlugin.Editor.Settings;

using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;



namespace EMX.HierarchyPlugin.Editor.Mods
{






	internal class BottomBarExtension
	{


		//        private GUIStyle refStyle;
		//        private Color refColor;
		//#pragma warning disable
		//        private Color tst = new Color32(240, 240, 255, 255);
		//        private Color ts2t = new Color32(248, 240, 225, 255);
		//#pragma warning restore
		internal Rect hiperRect;
		internal int DRAW_FOLD_ICONS_CONTROLID;
		internal GUIContent iconsContent = new GUIContent();
		internal int PluginId;
		bool USE_DOWN = true;
		//        internal Rect? lastRect;
		//        internal List<int> Keys = new List<int>();
		//        internal Dictionary<int, bool> NEED_READ_LIST = new Dictionary<int, bool>();
		//
		//        // GUIContent emptyContent = new GUIContent() { tooltip = "" };
		internal GUIContent FoldContent = new GUIContent() { tooltip = "Minimize The Dock" };
		//        // internal GUIContent plusContentLabel = new GUIContent() { tooltip = "Add GameObject" };
		//        internal GUIContent plusContentLabel = new GUIContent() { tooltip = "Create a snapshot of objects that have been expanded" };
		//        internal GUIContent plusContentSceneLabel = new GUIContent() { tooltip = "Create a multi-scenes buttons from open scenes" };
		//        internal GUIContent plusContent = new GUIContent() { text = "+" };
		//        internal GUIContent hierCollapce = new GUIContent() { text = "►" }; //-
		//        internal GUIContent hierCollapceLabel = new GUIContent() { tooltip = "Collapse all expanded items" };
		//#pragma warning disable
		//        internal GUIContent ContentSelBackLabel = new GUIContent() { tooltip = "Selection Backward" + (true ? " Ctrl+Shift+Z" : "") };
		//        private GUIContent ContentSelForwLabel = new GUIContent() { tooltip = "Selection Forward" + (true ? " Ctrl+Shift+Y" : "") };
		//#pragma warning restore
		//        internal GUIContent ContentSelBack = new GUIContent() { text = "◄" };//
		//        internal GUIContent ContentSelForw = new GUIContent() { text = "►" };
		//
		//        private float lastHeight = -1;
		//        private Rect oldr;
		//        internal float? defaultextraInsertionMarkerIndent;
		//
		//        //  float HEIGHT = 56;
		private const int HYPER_OFFSET = 900000;
		Color coloAlpha = new Color(1, 1, 1, 0.6f);
		//        private const int SPACE = 2;
		GUIContent faveContent = new GUIContent();

		//internal List<Adapter.BottomInterface.BottomController> WindowController = new List<Adapter.BottomInterface.BottomController>();
		//internal List<Adapter.BottomInterface.BottomController> FavoritControllers = new List<Adapter.BottomInterface.BottomController>();


		PluginInstance p;
		internal BottomBarExtension( PluginInstance p, int PluginId )
		{
			this.p = p;
			this.PluginId = PluginId;

		}

		internal void SubscribeEditorInstance( EditorSubscriber sbs )
		{

			//         return;
			//pragma warning disable
			if ( !p.par_e.USE_BOTTOMBAR_MOD )
			{
				foreach ( var item in Window._windowsList )
				{
					if ( item.bottomParams != null )
					{
						item.bottomParams.w = null;
						item.bottomParams = null;
					}
				}
				return;
			}

			sbs.OnAssignWindowFirstFrame += OnAssignWindowFirstFrame;
			sbs.BuildedOnGUI_first.Add( _bakeSettings );
			// Debug.Log( sbs.sbs_i );
			sbs.BuildedOnGUI_last += _draw;
			sbs.OnUpdate += Update;
			//drag rect ???
			sbs.REBUILD_ADDITIONAL_EVENTS.Add( hypersbs, new List<Action>() { TrySubscribeEnabledMods } );
			//this.hyperGraph = new HyperGraph( adapter, this );
			//this.favorGraph = new FavorGraph( adapter, this );
			//pragma warning restore

		}

		AdditionalSubscriber hypersbs = new AdditionalSubscriber(){
			ID = 0
		};

		internal void TrySubscribeEnabledMods()
		{
			var b  =BOTTOM_DRAWFOR_ONEHIERARHYWIN;
			for ( int i = BottomBarWindowInstance._AssignList.Count - 1; i >= 0; i-- )
			{
				if ( !BottomBarWindowInstance._AssignList[ i ].OvValide() )
				{
					BottomBarWindowInstance._AssignList[ i ].UnAssign();
					BottomBarWindowInstance._AssignList.RemoveAt( i );
					continue;
				}
				if ( b && !BottomBarWindowInstance._AssignList[ i ].w.thisIsAFirstWindow )
				{
					BottomBarWindowInstance._AssignList[ i ].UnAssign();
					BottomBarWindowInstance._AssignList.RemoveAt( i );
					continue;
				}
			}

			hypersbs.Clear();
			foreach ( var item in BottomBarWindowInstance._AssignList )
			{
				item.SubscribeEditorInstance( hypersbs );
			}
		}


		public void Update()
		{
			for ( int i = BottomBarWindowInstance._AssignList.Count - 1; i >= 0; i-- )
			{
				BottomBarWindowInstance._AssignList[ i ].Update();
			}
		}



		internal void ClearAction()
		{
			var b = BOTTOM_DRAWFOR_ONEHIERARHYWIN;
			for ( int i = BottomBarWindowInstance._AssignList.Count - 1; i >= 0; i-- )
			{
				if ( !BottomBarWindowInstance._AssignList[ i ].OvValide() )
				{
					BottomBarWindowInstance._AssignList[ i ].UnAssign();
					BottomBarWindowInstance._AssignList.RemoveAt( i );
					continue;
				}
				if ( b && !BottomBarWindowInstance._AssignList[ i ].w.thisIsAFirstWindow )
				{
					BottomBarWindowInstance._AssignList[ i ].UnAssign();
					BottomBarWindowInstance._AssignList.RemoveAt( i );
					continue;
				}
			}

			foreach ( var item in BottomBarWindowInstance._AssignList )
			{
				item.HierarchyController.ClearAction();
				//item.HyperGraphController.ClearAction();
			}


			//hyperGraph.currentActionID = -1;
			//favorGraph.currentActionID = -1;
			//// HiperGraph.currentActionWindow = null;
			//hyperGraph.WindowHyperController.currentAction = null;
			//hyperGraph.HierHyperController.currentAction = null;
			//favorGraph.WindowFavorController.currentAction = null;
			//favorGraph.HierFavorController.currentAction = null;
		}

		void _bakeSettings()
		{
			_BOTTOM_DRAWFOR_ONEHIERARHYWIN = p.WIN_SET.BOTTOMBAR_BOTTOM_DRAWFOR_ONEHIERARHYWIN;
			//internal int BOTTOM_LINE_HEIGHT = 20;

			//_SHOW_SCENES_ROWS = p.par_e.BOTTOMBAR_SHOW_SCENES_ROWS;
			//_SHOW_HIERARCHYSLOTS_ROWS = p.par_e.BOTTOMBAR_SHOW_HIERARCHYSLOTS_ROWS;
			//_SHOW_LAST_ROWS = p.par_e.BOTTOMBAR_SHOW_LAST_ROWS;
			//_SHOW_BOOKMARKS_ROWS = p.par_e.BOTTOMBAR_SHOW_BOOKMARKS_ROWS;

			_USE_MAIN_HYPER = p.par_e.USE_HYPERGRAPH_MOD;
			_USE_MAIN_SCENES_ROWS = p.par_e.USE_LAST_SCENES_MOD;
			_USE_MAIN_HIERARCHYSLOTS_ROWS = p.par_e.USE_HIER_EXPANDED_MOD;
			_USE_MAIN_LAST_ROWS = p.par_e.USE_LAST_SELECTION_MOD;
			_USE_MAIN_BOOKMARKS_ROWS = p.par_e.USE_BOOKMARKS_HIERARCHY_MOD;


			// _BOOKMARKS_LINEHEIGHT = p.WIN_SET.BOTTOMBAR_BOOKMARKS_LINEHEIGHT;
			// _LAST_LINEHEIGHT = p.WIN_SET.BOTTOMBAR_LAST_LINEHEIGHT;
			// _HIER_LINEHEIGHT = p.WIN_SET.BOTTOMBAR_HIER_LINEHEIGHT;
			// _SCENES_LINEHEIGHT = p.WIN_SET.BOTTOMBAR_SCENES_LINEHEIGHT;



			_SHOW_PARENT_TREE = p.WIN_SET.BOTTOMBAR_SHOW_PARENT_TREE;
			_SHOW_PARENT_TREE_CURRENTOBJECT = p.WIN_SET.BOTTOMBAR_SHOW_PARENT_TREE_CURRENTOBJECT;
			//BOTTOM_DRAWFOR_ONEHIERARHYWIN = 
			//BOTTOM_AUTOHIDE = 


			if ( !DRAW_VALID( p.window ) ) return;

			moduleRect = p.window.bottomParams.GetNavigatorRect();


			if ( bakedType != null ) //&& Event.current.type != EventType.Used
			{
				bakedType = null;
				//Debug.Log( p.deltaTime + " " + Event.current.type + " " + "bake to null" );
			}
			if ( moduleRect.Contains( Event.current.mousePosition ) )
			{

				if ( Event.current.isScrollWheel || Event.current.isMouse )
				//    if ( Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp )
				{
					bakedType = Event.current.type;
					Event.current.type = EventType.Used;
					//Debug.Log( p.deltaTime + " " +"bake to used" );
					//EventUse();
					//GUIUtility.hotControl = 0;
					//adapter.RepaintWindow( true );
				}
				p.ha.internal_DisableHover();
				//  //EMX_REMOVE
			}




		}

		EventType? bakedType;
		Rect moduleRect;

		// BAKED PARAMS
		internal bool _BOTTOM_DRAWFOR_ONEHIERARHYWIN;  //= true;
		internal bool BOTTOM_DRAWFOR_ONEHIERARHYWIN { get { return _BOTTOM_DRAWFOR_ONEHIERARHYWIN; } } //= true;
																									   //internal int BOTTOM_LINE_HEIGHT = 20;

		//internal bool _SHOW_SCENES_ROWS ; //= true;
		//internal bool _SHOW_HIERARCHYSLOTS_ROWS; //= true;
		//internal bool _SHOW_LAST_ROWS;  //= true;
		//internal bool _SHOW_BOOKMARKS_ROWS; //= true;
		// DrawButtonsOld.ISET_ROWS_ARRAY[ DrawButtonsOld.ISET_C ].Enable = !(bool)value;

		// internal bool SHOW_SCENES_ROWS { get { return DrawButtonsOld.ISET_ROWS_ARRAY[ DrawButtonsOld.ISET_S ].Enable; } } //= true;
		// internal bool SHOW_HIERARCHYSLOTS_ROWS { get { return DrawButtonsOld.ISET_ROWS_ARRAY[ DrawButtonsOld.ISET_E ].Enable; } } //= true;
		// internal bool SHOW_LAST_ROWS { get { return DrawButtonsOld.ISET_ROWS_ARRAY[ DrawButtonsOld.ISET_L ].Enable; } } //= true;
		// internal bool SHOW_BOOKMARKS_ROWS { get { return DrawButtonsOld.ISET_ROWS_ARRAY[ DrawButtonsOld.ISET_C ].Enable; } } //= true;

		internal bool SHOW_SCENES_ROWS { get { return DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Scenes ).Enable; } } //= true;
		internal bool SHOW_HIERARCHYSLOTS_ROWS { get { return DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Hier ).Enable; } } //= true;
		internal bool SHOW_LAST_ROWS { get { return DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Last ).Enable; } } //= true;
		internal bool SHOW_BOOKMARKS_ROWS { get { return DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Custom ).Enable; } } //= true;


		// internal int _BOOKMARKS_LINEHEIGHT;  //= 22;
		// internal int _LAST_LINEHEIGHT; //= 16;
		// internal int _HIER_LINEHEIGHT; //= 16;
		// internal int _SCENES_LINEHEIGHT;//= 16;
		// internal int BOOKMARKS_LINEHEIGHT { get { return _BOOKMARKS_LINEHEIGHT; } } //= 22;
		// internal int LAST_LINEHEIGHT { get { return _LAST_LINEHEIGHT; } } //= 16;
		// internal int HIER_LINEHEIGHT { get { return _HIER_LINEHEIGHT; } } //= 16;
		// internal int SCENES_LINEHEIGHT { get { return _SCENES_LINEHEIGHT; } } //= 16;

		internal bool _SHOW_PARENT_TREE; // = false;
		internal bool _SHOW_PARENT_TREE_CURRENTOBJECT; //= false;

		internal bool _USE_MAIN_HYPER ; //= true;
		internal bool _USE_MAIN_SCENES_ROWS; //= true;
		internal bool _USE_MAIN_HIERARCHYSLOTS_ROWS; //= true;
		internal bool _USE_MAIN_LAST_ROWS; //= true;
		internal bool _USE_MAIN_BOOKMARKS_ROWS; //= true;

		internal bool SHOW_PARENT_TREE { get { return _SHOW_PARENT_TREE; } } // = false;
		internal bool SHOW_PARENT_TREE_CURRENTOBJECT { get { return _SHOW_PARENT_TREE_CURRENTOBJECT; } } //= false;

		internal bool USE_MAIN_HYPER { get { return _USE_MAIN_HYPER; } } //= true;
		internal bool USE_MAIN_SCENES_ROWS { get { return _USE_MAIN_SCENES_ROWS; } } //= true;
		internal bool USE_MAIN_HIERARCHYSLOTS_ROWS { get { return _USE_MAIN_HIERARCHYSLOTS_ROWS; } } //= true;
		internal bool USE_MAIN_LAST_ROWS { get { return _USE_MAIN_LAST_ROWS; } } //= true;
		internal bool USE_MAIN_BOOKMARKS_ROWS { get { return _USE_MAIN_BOOKMARKS_ROWS; } } //= true;

		// BAKED PARAMS



		internal void OnAssignWindowFirstFrame( Window w )
		{
			if ( !p.par_e.USE_BOTTOMBAR_MOD ) return;
			if ( BOTTOM_DRAWFOR_ONEHIERARHYWIN && !p.window.thisIsAFirstWindow ) return;

			if ( !DRAW_VALID( w ) ) return;
			//if ( p.window.bottomParams == null )
			//{
			//	
			//	//item.hy.SubscribeEditorInstance( hypersbs );
			//	
			//}


			p.window.SET_BOTTOM( p.gui_currentTree, p.window.bottomParams.HEIGHT );

			//if ( p.firstFrame < 1 && Event.current.type == EventType.Repaint )
			//{
			//	//if ( !d )
			//	//{
			//	//
			//	//	d = true;
			//	//	EditorGUIUtility.ExitGUI();
			//	//	p.window.Instance.SendEvent( new Event() { type = EventType.Layout } );
			//	//	p.window.Instance.SendEvent( new Event() { type = EventType.Repaint } );
			//	//}
			//	(typeof( EditorWindow )).GetMethod( "RepaintImmediately", (BindingFlags)~0 ).Invoke( p.window.Instance, null );
			//
			//	Debug.Log( "ASD" );
			//}
		}

		bool DRAW_VALID( Window w )
		{
			if ( w.WAS_FULL_INIT )
			{
				if ( p.EVENT.type == EventType.Layout ) return false; //|| p.EVENT.type == EventType.Used
				if ( p.ha.IS_SEARCH_MODE_OR_PREFAB_OPENED() ) return false;

			}

			if ( p.window.bottomParams == null )
			{
				//OnAssignWindowFirstFrame( p.window );
				BottomBarWindowInstance.Assign( this, w );
				if ( p.window.bottomParams == null ) return false;
				TrySubscribeEnabledMods();

			}// AssignParams( p.window );
			return true;
		}

		float bottomOpacity = 1;

		void _draw()
		{
			if ( bakedType.HasValue )
			{
				Event.current.type = bakedType.Value;
				//Debug.Log( Event.current.type + " " + Event.current.isMouse );
			}


			if ( !DRAW_VALID( p.window ) )
			{
				p.window.RESET_BOTTOM();
				return;
			}

			p.window.SET_BOTTOM( p.gui_currentTree, p.window.bottomParams.HEIGHT );





			p.window.bottomParams.HierarchyController.tempRoot = p.window.bottomParams;
			//p.window.bottomParams.HyperGraphController.tempRoot = p.window.bottomParams;
			p.window.bottomParams.w = p.window;
			bottomOpacity = 1;

			if ( bottomOpacity != 1 )
			{

				EditorGUI.DrawRect( moduleRect, Colors.EditorBGColor );

				var bc = GUI.color;
				GUI.color *= new Color( 1, 1, 1, bottomOpacity );
				NEW_BOTTOM( p.o, p.window.bottomParams );
				//OLD_PAINT( selectRect, w );
				GUI.color = bc;
			}
			else
			{
				//OLD_PAINT( selectRect, w );
				NEW_BOTTOM( p.o, p.window.bottomParams );
			}


			// ExternalModRoot.ON_GUI_POST_UPCONTROLLER_CHECKER( p.window.bottomParams.HyperGraphController, p.window.bottomParams, p.window.bottomParams.mouse_uo_helper );
			ExternalModRoot.ON_GUI_POST_UPCONTROLLER_CHECKER( p.window.bottomParams.HierarchyController, p.window.bottomParams, p.window.bottomParams.mouse_uo_helper );


			if ( !wasPosInit )
			{
				wasPosInit = true;
				pos = p.GetSubHierarchy( 0, p.window.Instance ).GetType().GetProperty( "position", (BindingFlags)~0 );
			}
			if ( pos != null )
			{

				//EMX_TODO check height after update to 2021.2 and 2021.3
				var r = (Rect) pos.GetValue( p.GetSubHierarchy( 0, p.window.Instance ), null);
				// r.height = Mathf.Max( 0, r.height - p.window.bottomParams.HEIGHT );
				//r.height = r.height - p.window.bottomParams.HEIGHT;
				r.height = 1;
				pos.SetValue( p.GetSubHierarchy( 0, p.window.Instance ), r, null );



			}


			//if (Event.current.type == EventType.Repaint)p.window.SET_BOTTOM( p.gui_currentTree, 0 );


			if ( moduleRect.Contains( Event.current.mousePosition ) )
			{
				// if ( Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp )
				if ( Event.current.isScrollWheel || Event.current.isMouse )
				{
					//Debug.Log( p.deltaTime + " " +"bake to used" );
					EventUse();
					//GUIUtility.hotControl = 0;
					//adapter.RepaintWindow( true );
				}
				p.ha.internal_DisableHover();
				//  //EMX_REMOVE
			}



		}
		void SWITCH_HYPER_ACTIVE_FOR_FIRST_HIERARCY_( bool? b, BottomBarWindowInstance bottomParams )
		{
			bottomParams.GRAPH_ENABLED = !bottomParams.GRAPH_ENABLED;
			if ( bottomParams.GRAPH_ENABLED ) bottomParams.hy.SWITCH_ACTIVE_SCAN( null );
		}
		internal void SWITCH_HYPER_ACTIVE_FOR_FIRST_HIERARCY()
		{
			foreach ( var item in BottomBarWindowInstance._AssignList )
			{
				if ( item.OvValide() )
				{
					if ( item.w.thisIsAFirstWindow )
					{
						SWITCH_HYPER_ACTIVE_FOR_FIRST_HIERARCY_( item.w.bottomParams.GRAPH_ENABLED, item.w.bottomParams );
						// item.w.bottomParams.GRAPH_ENABLED = !item.w.bottomParams.GRAPH_ENABLED;
						// if ( item.w.bottomParams.GRAPH_ENABLED ) item.w.bottomParams.hy.SWITCH_ACTIVE_SCAN( null );
					}
				}
			}
		}

		internal void SWITCH_MINIMIZR_FOR_FIRST_HIERARCY()
		{
			foreach ( var item in BottomBarWindowInstance._AssignList )
			{
				if ( item.OvValide() )
				{
					if ( item.w.thisIsAFirstWindow )
					{
						item.BOTTOM_AUTOHIDE = !item.BOTTOM_AUTOHIDE;
						// item.mo = !item.BOTTOM_AUTOHIDE;
					}
				}
			}
		}


		static bool wasPosInit = false;
		static  PropertyInfo pos = null;

		internal int[] DRAW_INDEX = new int[4];
		internal void SORT_DRAW_ROWS()
		{

			var RowsParams = DrawButtonsOld.ISET_ROWS_ARRAY;
			if ( DRAW_INDEX.Length != RowsParams.Length ) DRAW_INDEX = new int[ RowsParams.Length ];
			for ( int i = 0, l = RowsParams.Length; i < l; i++ ) DRAW_INDEX[ i ] = -1;
			for ( int i = 0, l = RowsParams.Length; i < l; i++ )
			{
				var c = RowsParams[i].IndexPos;
				if ( c < 0 || c > l - 1 ) continue;
				DRAW_INDEX[ c ] = i;
			}

			for ( int i = 0, l = RowsParams.Length; i < l; i++ )
				if ( DRAW_INDEX[ i ] == -1 )     // Debug.LogWarning("ASD");
					for ( int J = 0; J < l; J++ )
					{
						if ( DRAW_INDEX.Contains( J ) ) continue;
						DRAW_INDEX[ i ] = J;
					}
		}
		internal static int[] SORT_DRAW_ROWS_AND_GETNEW_ARRAY()
		{

			var  DRAW_INDEX = new int[0];
			var RowsParams = DrawButtonsOld.ISET_ROWS_ARRAY;
			if ( DRAW_INDEX.Length != RowsParams.Length ) DRAW_INDEX = new int[ RowsParams.Length ];
			for ( int i = 0, l = RowsParams.Length; i < l; i++ ) DRAW_INDEX[ i ] = -1;
			for ( int i = 0, l = RowsParams.Length; i < l; i++ )
			{
				var c = RowsParams[i].IndexPos;
				if ( c < 0 || c > l - 1 ) continue;
				DRAW_INDEX[ c ] = i;
			}

			for ( int i = 0, l = RowsParams.Length; i < l; i++ )
				if ( DRAW_INDEX[ i ] == -1 )     // Debug.LogWarning("ASD");
					for ( int J = 0; J < l; J++ )
					{
						if ( DRAW_INDEX.Contains( J ) ) continue;
						DRAW_INDEX[ i ] = J;
					}
			return DRAW_INDEX;
		}

		// internal int LINE_REFERENCE_HEIGHT {get {return (int)EditorGUIUtility.singleLineHeight;}}
		internal int LINE_REFERENCE_HEIGHT = (int)EditorGUIUtility.singleLineHeight;


		Scene scene { get { return Root.p[ 0 ].LastActiveScene; } }
		internal Rect GetLineRect( Rect rect )
		{
			var line = rect;
			// line.height = bar.BOTTOM_LINE_HEIGHT;
			line.height = 0;
			//  line.y -= 2;
			line.x += 2;
			line.width -= 5;
			return line;
		}

		internal Rect GetFoldOutRect( ref Rect lineRect )
		{
			var foldRect = lineRect;
			foldRect.y += 1;
			foldRect.height = LINE_REFERENCE_HEIGHT - 2;
			//  foldRect.height = EditorGUIUtility.singleLineHeight - 2;
			lineRect.y += foldRect.height + 2;
			return foldRect;
		}


		void NEW_BOTTOM( HierarchyObject _o, BottomBarWindowInstance w ) //Rect selectRect,
		{

			//var moduleRect = w.GetNavigatorRect( /*treeView,*/ selectRect.x + selectRect.width);
			// var moduleRect = w.GetNavigatorRect( );
			// var navRect =  navRect;// GetNavigatorRect( /*treeView,*/ selectRect.x + selectRect.width);
			w.HierarchyController.ModuleRect = moduleRect;
			//w.HyperGraphController.ModuleRect = moduleRect;

			var lineRect = GetLineRect(moduleRect);
			var foldRect = GetFoldOutRect(ref lineRect);

			if ( !moduleRect.Contains( Event.current.mousePosition ) && (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp) ) return;


			// FIX LEFT COLUMN
			//if ( Event.current.type == EventType.Repaint )
			//{
			//    if ( UNITY_CURRENT_VERSION >= UNITY_2019_VERSION && Event.current.type == EventType.Repaint )
			//    {
			//        if ( EditorGUIUtility.isProSkin )
			//            Adapter.DrawRect( new Rect( 0, selectRect.y + selectRect.height, adapter.TOTAL_LEFT_PADDING_FORBOTTOM, mTotalRectGet2.height ), leftFixColorPro );
			//        else
			//            Adapter.DrawRect( new Rect( 0, selectRect.y + selectRect.height, adapter.TOTAL_LEFT_PADDING_FORBOTTOM, mTotalRectGet2.height ), leftFixColorPersonal );
			//    }
			//}


			/*  var line = GetLineRect(rect);
              var foldRect = GetFoldOutRect(ref line);*/
			//if ( moduleRect.Contains( Event.current.mousePosition ) )
			//    if ( Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform ||
			//            Event.current.type == EventType.DragExited ) EventUse();




			//BG
			if ( Event.current.type == EventType.Repaint )
			{   //FadeRect
				//Adapter.DrawRect( moduleRect, Colors.EditorBGColor );
				//p.gl._DrawRect
				EditorGUI.DrawRect( moduleRect, Colors.EditorBGColor );
				//FadeRect
				//if ( UnityVersion.UNITY_CURRENT_VERSION < UnityVersion.UNITY_2019_3_0_VERSION )
				var r22 = moduleRect;
				r22.height += 3;
				p.GET_SKIN().window.Draw( r22, /*new GUIContent("Navigator"),*/ false, false, false, false );
				//else

				if ( p.GET_SLOW_oldProSkin )
				{
					var r = moduleRect;
					//var r5 = 8;
					r.x += 2;
					r.y += LINE_REFERENCE_HEIGHT - 1;
					r.width -= 2 * 2;
					r.height -= LINE_REFERENCE_HEIGHT - 1;
					// r.height = w.GRAPH_HEIGHT;
					/// r.height -= r5 * 2;
					p.GET_SKIN().textArea.Draw( r, /*new GUIContent("Navigator"),*/ false, false, false, false );
				}
				
			}

			//FOLD
			DRAW_FOLD_ICONS( w, ref foldRect );
			if ( moduleRect.Contains( Event.current.mousePosition ) )
				if ( Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp )
				{
					if ( anycatsenable )
					{
						if ( foldRect.Contains( Event.current.mousePosition ) )
						{
							w.HierarchyController.selection_window = w;
							w.HierarchyController.selection_button = 0;
							w.HierarchyController.selection_action = ( mouseUp, deltaTIme ) => {
								if ( mouseUp && foldRect.Contains( Event.current.mousePosition ) )
								{
									if ( !USE_DOWN ) w.BOTTOM_AUTOHIDE = !w.BOTTOM_AUTOHIDE;
									//adapter.SavePrefs();
								}
								return Event.current.delta.x == 0 && Event.current.delta.x == 0;

							};
							if ( USE_DOWN )
								p.PUSH_GUI_ONESHOT( PluginId, () => {
									w.BOTTOM_AUTOHIDE = !w.BOTTOM_AUTOHIDE;
								} );
						}
					}
				}


			//LINES
			/* if ( adapter.BOT_DRAW_STACK.START_DRAW_PARTLY_TRYDRAW( _o ) )
             {   if ( Event.current.type == EventType.Repaint ) adapter.BOT_DRAW_STACK.START_DRAW_PARTLY_CREATEINSTANCE( _o );
                 DoLines( lineRect, w );
                 adapter.BOT_DRAW_STACK.END_DRAW( _o );
             }*/

			var line = lineRect;


			if ( w.GRAPH_HEIGHT != 0 && _USE_MAIN_HYPER )
			{
				line.height = w.GRAPH_HEIGHT;

				w.H_POS = line;

				if ( Event.current.type == EventType.ScrollWheel && line
			  .Contains( Event.current.mousePosition ) )
					w.hy.ON_SCROLL( Event.current.delta.y );
				p.ChangeGUI();
				// w.hy.EXTERNAL_HYPER_DRAWER( line, w.HyperGraphController, w );
				w.hy.EXTERNAL_HYPER_DRAWER( line, w.HierarchyController, w );
				p.RestoreGUI();
				//if ( ENABLE_HYPERGUI() ) hyperGraph.DRAW( line, hyperGraph.HierHyperController, w );
				//if ( ENABLE_FAVORGUI() ) favorGraph.DRAW( line, favorGraph.HierFavorController, w );
				line.y += line.height;
			}




			if ( anycatsenable )
			{
				//if ( adapter.DISABLE_DESCRIPTION( (LastActiveScene) ) ) return;




				// HierarchyController.REFERENCE_WINDOW = adapter.window();

				if ( w.HEIGHT > w.___REFERENCE_HEIGHT_AUTOHIDE() )
				{
					SORT_DRAW_ROWS();

					var RowsParams = DrawButtonsOld.ISET_ROWS_ARRAY;

					for ( int __index = 0; __index < DRAW_INDEX.Length; __index++ )
					{
						var i = DRAW_INDEX[__index];

						if ( !RowsParams[ i ].Enable ) continue;



						line.height = RowsParams[ i ].FULL_HEIGHT;

						EditorGUI.DrawRect( line, Color.Lerp( Colors.EditorBGColor, Color.white, 0.05f) );

						switch ( RowsParams[ i ].type )
						{
							case MemType.Custom:
								w.C_POS = line;
								break;
							case MemType.Scenes:
								w.S_POS = line;
								break;
							case MemType.Last:
								w.L_POS = line;
								break;
							case MemType.Hier:
								w.E_POS = line;
								break;
						}
						//if (RowsParams[ i ].type != MemType.Custom)
						DRAW_BY_INDEX( RowsParams[ i ], line, w );

						// if ( RowsParams[ i ].PluginID == PLUGIN_ID.BOOKMARKS ) w.HierarchyController.CustomLineRect = line;

						line.y += line.height;
					}
				}


			}
			// DoLines( lineRect, w );









		}

		void EventUse()
		{
			p.EVENT.Use();
		}

		void EventUseFast()
		{
			p.EVENT.Use();
		}
		//Rect frr;
		//private void DoLines( Rect foldOutRect, IExternalWindow win )
		//{
		//   
		//}


		internal void DRAW_BY_INDEX( ISET_ROW row, Rect line, BottomBarWindowInstance p )
		{
			switch ( row.type )
			{
				// case 0: DRAW_CUSTOM( line, row.RowHeight, controller, scene, drawWindowPanel = true ); break;
				// case 1: DRAW_LAST( line, row.RowHeight, controller, scene, drawWindowPanel = true ); break;
				// case 2: DRAW_HIER( line, row.RowHeight, controller, scene, drawWindowPanel = true ); break;
				// case 3: DRAW_SCENE( line, row.RowHeight, controller, scene, drawWindowPanel = true ); break;

				case MemType.Custom: BookmarksforGameObjectsModWindow.ManualDraw( ref line, p.m_custom, p.HierarchyController, p ); break;
				case MemType.Scenes: ScenesHistoryModWindow.ManualDraw( ref line, p.m_scene, p.HierarchyController, p ); break;
				case MemType.Last: GameObjectsSelectionHistoryModWindow.ManualDraw( ref line, p.m_last, p.HierarchyController, p ); break;
				case MemType.Hier: HierarchyExpandedMemWindow.ManualDraw( ref line, p.m_hier, p.HierarchyController, p ); break;
			}
		}



		//internal void DRAW_CUSTOM( Rect line, int LH, BottomController controller, int scene, bool drawWindowPanel = true )
		//{
		//    if ( !controller.IS_MAIN ) line = DO_DOCKABLE_BUTTON( line, _6__BottomWindow_BottomInterfaceWindow.TYPE.CUSTOM, controller, scene );
		//    DoCustom( line, LH, controller, scene );
		//}

		//Rect DO_DOCKABLE_BUTTON( Rect line, _6__BottomWindow_BottomInterfaceWindow.TYPE type, BottomController controller, int scene )
		//{
		//    frr = line;
		//    frr.width = EditorGUIUtility.singleLineHeight;
		//    DRAW_CATEGORY( frr, controller, scene );
		//    line.x += frr.width;
		//    line.width -= frr.width;
		//    return line;
		//}



		//internal float DRAW_CUSTOM_MINHEIGHT( BottomController controller )     //  return LINE_HEIGHT(controller.IS_MAIN, true) * adapter.par.BOTTOM_MAXCUSTOMROWS;
		//{
		//    return LINE_HEIGHT( controller.IS_MAIN, true ) * RowsParams[ 0 ].Rows;
		//}
		//internal float DRAW_LAST_MINHEIGHT( BottomController controller )     //  return LINE_HEIGHT(controller.IS_MAIN) * adapter.par.BOTTOM_MAXLASTROWS;
		//{
		//    return LINE_HEIGHT( controller.IS_MAIN ) * RowsParams[ 1 ].Rows;
		//}


		//Rect mTotalRectGet2;
		//Color leftFixColorPro = new Color32(45, 45, 45, 255);
		//Color leftFixColorPersonal = new Color32(170, 170, 170, 255);


		static void ROUND_RECT( ref Rect rect )
		{
			rect.x = Mathf.FloorToInt( rect.x );
			rect.y = Mathf.FloorToInt( rect.y );
			rect.width = Mathf.FloorToInt( rect.width );
			rect.height = Mathf.FloorToInt( rect.height );
		}









		void FOLDER_BUTTON(
		   BottomBarWindowInstance w,
		   ref Rect foldRect,
			object value,
			Action<object, int, BottomBarWindowInstance> setValue,
			string iconName, string toolTip, string text = null,
			Vector2? offset = null,
			Action<object, BottomBarWindowInstance> dragAction = null,
			int? fontSIze = null,
			Texture2D iconObject = null )
		{

			++DRAW_FOLD_ICONS_CONTROLID;


			//EditorGUIUtility.AddCursorRect(hiperRect,adapter.HYPER_ENABLE() ? MouseCursor.ArrowMinus : MouseCursor.ArrowPlus);
			var cccc = GUI.color;


			if ( iconName != null )
			{
				var iconRect = hiperRect;
				if ( !EditorGUIUtility.isProSkin )     //
				{
					iconRect.height -= 1;
					iconRect.width -= 1;
					iconName += " PERSONAL";
				}

				GUI.DrawTexture( iconRect, p.GetExternalModOld( iconName ) );

				if ( !(bool)value )
				{
					GUI.color *= coloAlpha;
					GUI.DrawTexture( iconRect, p.GetExternalModOld( iconName + " OFF" ) );
				}
			}
			if ( iconObject )
			{
				var iconRect = hiperRect;

				if ( Event.current.type == EventType.Repaint ) p.GET_SKIN().textArea.Draw( new Rect( iconRect.x - 1, iconRect.y - 1, iconRect.width + 2, iconRect.height + 2 ), "", false, false, false, false );
				//if ( !(bool)value ) GUI.color *= coloAlpha;
				GUI.DrawTexture( iconRect, iconObject );
			}


			GUI.color = cccc;

			// if (GUI.Button(hiperRect, ""))

			if ( Event.current.type == EventType.MouseDown && hiperRect.Contains( Event.current.mousePosition ) )
			{
				w.HierarchyController.selection_button = DRAW_FOLD_ICONS_CONTROLID + HYPER_OFFSET;



				EventUseFast();
				//var captureID = HierarchyController.selection_button;
				w.HierarchyController.selection_window = w;
				var captureRect = hiperRect;
				var b = Event.current.button;
				if ( offset.HasValue ) captureRect.Set( captureRect.x + offset.Value.x, Mathf.RoundToInt( captureRect.y + offset.Value.y ), captureRect.width, captureRect.height );
				w.HierarchyController.selection_action = ( mouseUp, deltaTIme ) =>      //  Debug.Log(mouseUp + " " + captureRect + " " + Event.current.mousePosition);
				{
					if ( mouseUp && captureRect.Contains( Event.current.mousePosition ) )
					{
						if ( !USE_DOWN || dragAction != null ) setValue( value, b, w );
					}
					else if ( Event.current.type == EventType.MouseDrag && !captureRect.Contains( Event.current.mousePosition )
							  && !Event.current.control && !Event.current.shift && !Event.current.alt )
					{
						if ( dragAction != null )
						{
							dragAction( value, w );
							w.HierarchyController.selection_action = null;
						}
					}

					return Event.current.delta.x == 0 && Event.current.delta.x == 0;



				}; // ACTION
				if ( USE_DOWN && dragAction == null ) //setValue( value, w );
					p.PUSH_GUI_ONESHOT( PluginId, () => {
						setValue( value, b, w );
					} );
			}

			if ( Event.current.type == EventType.Repaint )
			{
				if ( w.HierarchyController.selection_action != null && w.HierarchyController.selection_button != -1 )
				{
					var hover = w.HierarchyController.selection_button == HYPER_OFFSET + DRAW_FOLD_ICONS_CONTROLID;
					if ( hover )
					{
						// GUI.DrawTexture( hiperRect, adapter.GetIcon( "BUTBLUE" ) );
						//Debug.Log( hiperRect + " " +Event.current.mousePosition);
						p.gl.DRAW_TAP_GLOW( hiperRect );
						//EditorGUI.DrawRect( hiperRect, Color.white );
					}
				}


			}
			iconsContent.tooltip = toolTip;
			iconsContent.text = text;


			labelLeft8style.fontSize = fontSIze ?? p.FONT_8_FOR_BOTTOM();
			GUI.Label( hiperRect, iconsContent, labelLeft8style );

			/* foldRect.x += hiperRect.width + 5;                */

			foldRect.width -= hiperRect.width;
			hiperRect.x -= hiperRect.width;
		}


		bool anycatsenable {
			get {
				return (USE_MAIN_SCENES_ROWS && SHOW_SCENES_ROWS ||
						USE_MAIN_HIERARCHYSLOTS_ROWS && SHOW_HIERARCHYSLOTS_ROWS ||
					   USE_MAIN_LAST_ROWS && SHOW_LAST_ROWS ||
					   USE_MAIN_BOOKMARKS_ROWS && SHOW_BOOKMARKS_ROWS);
			}
		}
		Rect againRect;
		bool DRAW_CATEGORY_NAME_ONLY( BottomBarWindowInstance w, ref Rect foldRect, Rect treeRect )
		{
			if ( !USE_MAIN_BOOKMARKS_ROWS || !SHOW_BOOKMARKS_ROWS ) return false; // || w.BOTTOM_AUTOHIDE

			var list = DrawButtonsOld.GET_BOOKMARKS(scene);

			faveContent.text = list[ w.HierarchyController.GetCategoryIndex( scene ) ].get_name;
			calcConetnt.text = faveContent.text;
			treeRect.width = CalcWidth( null );
			againRect = treeRect;
			//adapter.bottomInterface.GET_BOOKMARKS( ref list, scene );

			//EditorGUIUtility.AddCursorRect( treeRect, MouseCursor.Zoom );
			EditorGUIUtility.AddCursorRect( treeRect, MouseCursor.Link );

			hiperRect = treeRect;
			FOLDER_BUTTON( w, ref foldRect, null, SET_BOOK, null, "Click - to change current category", faveContent.text, dragAction: asd );
			return true;

		}

		private void asd( object arg1, BottomBarWindowInstance arg2 )
		{
		}




		//void SET_BOOK( object o, BottomBarParams w )
		//{
		//    DrawButtonsOld.SET_BOOK_WIHTOUT_OBJECTS( w.HierarchyController, scene, w.m_custom );
		//}
		//void SET_BOOK( object o, BottomBarParams w )
		//{
		//    DrawButtonsOld.SET_LAST( w.HierarchyController, scene, w.m_custom );
		//}




		void SET_HYPER()
		{
			p.PUSH_UPDATE_ONESHOT( PluginId, () => { ClearAction(); } );

			var menu = new GenericMenu();
			//MENU_GEN( menu, name, lastController );

			HyperGraphModWindow.MENU_GEN( menu, HyperGraphModWindow.NAME, HyperGraphModWindow.lastController );



			menu.AddItem( new GUIContent( "- Open bottom bar settings" ), false, () => {
				MainSettingsEnabler_Window.Select<BB_Window>();
			} );

							p.PUSH_MENU_OPENING_ACTION( menu, ClearAction );



			//menu.AddItem( GetContent( SHOW_LAST_ROWS, HyperGraphModWindow.NAME.ToLower() ), SHOW_LAST_ROWS, () =>
			//   //   menu.AddItem(new GUIContent("Enable Last bottom GUI"), adapter.par.SHOW_LAST_ROWS, () =>
			//   {
			//	   //p.par_e.BOTTOMBAR_SHOW_LAST_ROWS = !(bool)value;
			//   } );
			////menu.AddSeparator( "" );
			//menu.AddItem( new GUIContent( "Open in New Tab" ), false, () => { _6__BottomWindow_BottomInterfaceWindow.ShowW( adapter, _6__BottomWindow_BottomInterfaceWindow.TYPE.LAST, "Last Selection" ); } );
			//menu.AddSeparator( "" );


		}


		void SET_LAST( object value, int button, BottomBarWindowInstance w )
		{

			if ( button == 0 && p.WIN_SET.BOTTOMBAR_QUICK_BUTTON_LEFT_CLICK_CLOSE )
			{
				DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Last ).Enable = !(bool)value;
			}
			else
			{
				p.PUSH_UPDATE_ONESHOT( PluginId, () => { ClearAction(); } );


				GenericMenu menu = new GenericMenu();
				menu.AddItem( GetContent( SHOW_LAST_ROWS, DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Last ).NAME ), SHOW_LAST_ROWS, () =>
				   //   menu.AddItem(new GUIContent("Enable Last bottom GUI"), adapter.par.SHOW_LAST_ROWS, () =>
				   {
					   DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Last ).Enable = !(bool)value;
					   //p.par_e.BOTTOMBAR_SHOW_LAST_ROWS = !(bool)value;
				   } );
				//menu.AddSeparator( "" );
				//menu.AddItem( new GUIContent( "Open in New Tab" ), false, () => { _6__BottomWindow_BottomInterfaceWindow.ShowW( adapter, _6__BottomWindow_BottomInterfaceWindow.TYPE.LAST, "Last Selection" ); } );
				//menu.AddSeparator( "" );


				var asd = Root.p[0].modsController.toolBarModification.hotButtons.externalMod_Buttons
				.FirstOrDefault(ex_mod=>ex_mod.windowType == typeof(GameObjectsSelectionHistoryModWindow) );
				if ( asd == null ) return;
				asd.menuGen( menu, asd.text, w.HierarchyController );


				menu.AddItem( new GUIContent( "- Open bottom bar settings" ), false, () => {
					MainSettingsEnabler_Window.Select<BB_Window>();
				} );
				// DrawButtonsOld.SET_LAST( menu, w.HierarchyController, scene );
				//ADD_TO_MENU_LIST_OF_OBJECTS( MemType.Last, ref menu );
								p.PUSH_MENU_OPENING_ACTION( menu, ClearAction );

			}
		}

		void SET_HIER( object value, int button, BottomBarWindowInstance w )
		{
			if ( button == 0 && p.WIN_SET.BOTTOMBAR_QUICK_BUTTON_LEFT_CLICK_CLOSE )
			{
				DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Hier ).Enable = !(bool)value;
			}
			else
			{
				p.PUSH_UPDATE_ONESHOT( PluginId, () => { ClearAction(); } );

				GenericMenu menu = new GenericMenu();
				menu.AddItem( GetContent( SHOW_HIERARCHYSLOTS_ROWS, DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Hier ).NAME ), SHOW_HIERARCHYSLOTS_ROWS, () =>
				   //  menu.AddItem(new GUIContent("Enable HIerarchy bottom GUI"), adapter.par.SHOW_HIERARCHYSLOTS_ROWS, () =>
				   {
					   DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Hier ).Enable = !(bool)value;
					   //p.par_e.BOTTOMBAR_SHOW_HIERARCHYSLOTS_ROWS = !(bool)value;
				   } );
				//menu.AddSeparator( "" );
				//DrawButtonsOld.SET_HIER( menu, w.HierarchyController, scene );

				var asd = Root.p[0].modsController.toolBarModification.hotButtons.externalMod_Buttons
				.FirstOrDefault(ex_mod=>ex_mod.windowType == typeof(HierarchyExpandedMemWindow) );
				if ( asd == null ) return;
				asd.menuGen( menu, asd.text, w.HierarchyController );



				menu.AddItem( new GUIContent( "- Open bottom bar settings" ), false, () => {
					MainSettingsEnabler_Window.Select<BB_Window>();
				} );

				// ADD_TO_MENU_LIST_OF_OBJECTS( MemType.Hier, ref menu );
								p.PUSH_MENU_OPENING_ACTION( menu, ClearAction );

			}
		}

		void SET_SCEN( object value, int button, BottomBarWindowInstance w )
		{
			if ( button == 0 && p.WIN_SET.BOTTOMBAR_QUICK_BUTTON_LEFT_CLICK_CLOSE )
			{
				DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Scenes ).Enable = !(bool)value;
			}
			else
			{
				p.PUSH_UPDATE_ONESHOT( PluginId, () => { ClearAction(); } );

				GenericMenu menu = new GenericMenu();
				menu.AddItem( GetContent( SHOW_SCENES_ROWS, DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Scenes ).NAME ), SHOW_SCENES_ROWS, () =>
					//  menu.AddItem(new GUIContent("Enable Scenes bottom GUI"), adapter.par.SHOW_SCENES_ROWS, () =>
					{
						DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Scenes ).Enable = !(bool)value;
						// p.par_e.BOTTOMBAR_SHOW_SCENES_ROWS = !(bool)value;
					} );
				//menu.AddSeparator( "" );
				//DrawButtonsOld.SET_SCEN( menu, w.HierarchyController, scene );

				var asd = Root.p[0].modsController.toolBarModification.hotButtons.externalMod_Buttons
				.FirstOrDefault(ex_mod=>ex_mod.windowType == typeof(ScenesHistoryModWindow) );
				if ( asd == null ) return;
				asd.menuGen( menu, asd.text, w.HierarchyController );


				menu.AddItem( new GUIContent( "- Open bottom bar settings" ), false, () => {
					MainSettingsEnabler_Window.Select<BB_Window>();
				} );

				//ADD_TO_MENU_LIST_OF_OBJECTS( MemType.Scenes, ref menu );
				//menu.AddSeparator( "" );
				//menu.AddItem( new GUIContent( "+ Add All Opened Scenes" ), false, () => { DoScenes_Plus( null ); } );
								p.PUSH_MENU_OPENING_ACTION( menu, ClearAction );

			}
		}

		void SET_BOOK( object value, int button, BottomBarWindowInstance w )
		{
			if ( button == 0 && value != null && p.WIN_SET.BOTTOMBAR_QUICK_BUTTON_LEFT_CLICK_CLOSE )
			{
				DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Custom ).Enable = !(bool)value;
			}
			else
			{

				GenericMenu menu = new GenericMenu();
				// DrawButtonsOld.ADD_TO_MENU_LIST_OF_OBJECTS( MemType.Custom, ref menu, w.HierarchyController, scene );
				//ADD_TO_MENU_LIST_OF_OBJECTS( MemType.Custom, ref menu );
				// menu.AddSeparator( "" );
				menu.AddItem( GetContent( SHOW_BOOKMARKS_ROWS, DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Custom ).NAME ), SHOW_BOOKMARKS_ROWS, () =>
				   //    menu.AddItem(new GUIContent("Enable Bookmarks Botom GUI"), adapter.par.SHOW_BOOKMARKS_ROWS, () =>
				   {
					   DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Custom ).Enable = !(bool)value;
					   // p.par_e.BOTTOMBAR_SHOW_BOOKMARKS_ROWS = !(bool)value;
					   // adapter.SavePrefs();
				   } );
				//menu.AddSeparator( "" );
				//DrawButtonsOld.SHOW_CATEGORY_MENU( menu, w.HierarchyController, w.m_custom, scene, false );
				var asd = Root.p[0].modsController.toolBarModification.hotButtons.externalMod_Buttons
				.FirstOrDefault(ex_mod=>ex_mod.windowType == typeof(BookmarksforGameObjectsModWindow) );
				if ( asd == null ) return;
				asd.menuGen( menu, asd.text, w.HierarchyController );

				menu.AddItem( new GUIContent( "- Open bottom bar settings" ), false, () => {
					MainSettingsEnabler_Window.Select<BB_Window>();
				} );

				p.PUSH_MENU_OPENING_ACTION( menu, ClearAction );
				
			}
		}




		void SET_SELECT_OBECJT( object _value, int b, BottomBarWindowInstance w )
		{
			var value = _value as UnityEngine.Object;
			if ( !(bool)value ) return;
			var result = new[] {(UnityEngine.Object) value};
			if ( Event.current.control || Event.current.shift )
				Selection.objects = Selection.objects.Concat( result ).ToArray();
			else if ( Event.current.alt )
				Selection.objects = Selection.objects.Except( result ).ToArray();
			else
				Selection.objects = result;
		}

		void SET_DRAG_OBECJT( object _value, BottomBarWindowInstance w )
		{
			var value = _value as UnityEngine.Object;

			// Debug.Log( value );
			if ( !value ) return;

			var result = new[] {(UnityEngine.Object) value};

			CustomDragData.SetDragData( result, MemType.Last );
			DragAndDrop.StartDrag( "Dragging GameObject" );
			// SetDragData( result, MemType.other );
			//DragAndDrop.StartDrag( "Dragging GameObject" );
			EventUse();

			ClearAction();
			w.RepaintNow();
			//HierarchyController.REPAINT( adapter );


		}








		//static internal void SET_BOOK_2(  int scene )
		//{
		//    GenericMenu menu = new GenericMenu();
		//    ADD_TO_MENU_LIST_OF_OBJECTS( MemType.Custom, ref menu );
		//    menu.AddSeparator( "" );
		//    menu.AddItem( GetContent( SHOW_BOOKMARKS_ROWS, "Bookmarks" ), false, () => {
		//        SHOW_BOOKMARKS_ROWS = !(bool)adapter.par.SHOW_BOOKMARKS_ROWS;
		//    } );
		//    menu.AddSeparator( "" );
		//    CREATE_BUTTON_CUSTOM_MENU( controller, scene, false, menu );
		//    menu.ShowAsContext();
		//}








		static GUIContent calcConetnt = new GUIContent();
		static string ShowContent = "- Show % in bottom bar";
		//static string HideContent = "Hide % Interface";
		static GUIContent GetContent( bool enable, string replace )
		{
			//if ( enable ) return new GUIContent( HideContent.Replace( "%", replace ) );
			//return new GUIContent( ShowContent.Replace( "%", replace ) );
			_GetContent.text = ShowContent.Replace( "%", replace.ToLower() );
			return _GetContent;
		}
		static  GUIContent _GetContent = new GUIContent();



		GUIStyle __iconStyle;
		GUIStyle labelLeft8style {
			get {
				if ( __iconStyle == null )
				{
					__iconStyle = new GUIStyle( p.label );
					__iconStyle.alignment = TextAnchor.MiddleLeft;
					__iconStyle.clipping = TextClipping.Clip;
				}
				return __iconStyle;
			}
		}
		GUIStyle labelLeft8styl2 {
			get {

				return labelLeft8style;
			}
		}


		Vector2 v2offset;
		float CalcWidth( int? fontSIze = null )
		{
			labelLeft8style.fontSize = fontSIze ?? p.FONT_8_FOR_BOTTOM();
			float w = labelLeft8style.CalcSize(calcConetnt).x;// - labelLeft8style.padding.left - labelLeft8style.padding.right;
			return w;
		}
		float DRAW_CATEGORY_NAME_S;

		List<HierarchyObject> hier_tree = new List<HierarchyObject>(100);

		bool DRAW_CATEGORY_NAME( BottomBarWindowInstance w, ref Rect foldRect, Rect treeRect )
		{

			bool wasDraw = false;

			if ( SHOW_PARENT_TREE && (
						(IS_HIERARCHY() && Selection.activeGameObject && Selection.activeGameObject.scene.IsValid()) ||
						IS_PROJECT() && Selection.activeObject && (!Selection.activeGameObject || !Selection.activeGameObject.scene.IsValid())
					) )
			{

				// var O = adapter.GetHierarchyObjectByInstanceID(Selection.activeObject.GetInstanceID());
				var O  =            p.ha.activeGameObject();


				v2offset.Set( treeRect.x, treeRect.y );
				GUI.BeginClip( treeRect );
				treeRect.x = treeRect.y = 0;


				//var  parentRect = treeRect;
				//parentRect.x = parentRect.x + parentRect.width;
				//var CHAR = "► ";
				var CHAR = "/";
				//var CHAR = "| ";
				var FS = p.WIN_SET_G(PluginId).BOTTOMBAR_HEADER_FONT_SIZE;
				//bool f = true;
				bool hasLast = !SHOW_PARENT_TREE_CURRENTOBJECT;
				// var BW = 80;
				// treeRect.x += treeRect.width - BW;
				// treeRect.width = BW;
				var parent = SHOW_PARENT_TREE_CURRENTOBJECT ? O : O.parent();
				int count = 0;
				while ( parent/* && parentRect.x + BW > 0*/ != null )     //calcConetnt.text = "►" + p.name;
				{
					if ( count >= hier_tree.Count ) hier_tree.Add( null );
					hier_tree[ count ] = parent;
					count++;
					parent = parent.parent();
				}

				var parentRect = treeRect;
				//calcConetnt.text = "";

				for ( int i = count - 1; i >= 0; i-- )
				{
					parent = hier_tree[ i ];
					calcConetnt.text = parent.name;
					if ( !SHOW_PARENT_TREE_CURRENTOBJECT && i == 0 ) calcConetnt.text += "/...";
					if ( i != 0 && count != 1 ) calcConetnt.text += CHAR;

					float calcedW = CalcWidth( FS);
					parentRect.width = calcedW;
					wasDraw = true;

					hiperRect = parentRect;

					if ( Event.current.control || Event.current.shift || Event.current.alt )
						EditorGUIUtility.AddCursorRect( hiperRect, Event.current.alt ? MouseCursor.ArrowMinus : MouseCursor.ArrowPlus );
					//else
					//	EditorGUIUtility.AddCursorRect( hiperRect, MouseCursor.SlideArrow );
					UnityEngine.Object obj = null;
					//if ( !hasLast )
					if ( hiperRect.Contains( Event.current.mousePosition ) || w.HierarchyController.selection_action != null ) obj = parent.GetHardLoadObjectSlow();

					FOLDER_BUTTON( w, ref foldRect, obj, SET_SELECT_OBECJT, null, calcConetnt.text, calcConetnt.text, v2offset, SET_DRAG_OBECJT, FS );



					parentRect.x += parentRect.width - 4;
				}





#if FALSE
				while ( parent/* && parentRect.x + BW > 0*/ != null || hasLast )     //calcConetnt.text = "►" + p.name;
				{
					//if ( hasLast ) calcConetnt.text = "";//"..";
					if ( hasLast ) calcConetnt.text = "/. ";
					if ( parent == null ) break;
					if ( hasLast ) calcConetnt.text = parent.name + calcConetnt.text;
					else if ( f ) calcConetnt.text = parent.name;
					else calcConetnt.text = parent.name + CHAR;
					f = false;
					float calcedW = CalcWidth( FS);
					//if (w > BW) w = BW;
					parentRect.width = calcedW;
					parentRect.x += parentRect.width - 4;

					if ( hasLast )
					{
						hasLast = false;
						//continue;
					}
					parent = parent.parent();


					//treeRect.x -= treeRect.width;
				}


				var offset = 0f;
				// if (parentRect.x > 0) offset = parentRect.x;
				if ( parentRect.x > 0 ) offset = parentRect.x;
				parent = SHOW_PARENT_TREE_CURRENTOBJECT ? O : O.parent();
				//parentRect.x -= offset;

				parentRect = treeRect;
				parentRect.x = offset;
				DRAW_CATEGORY_NAME_S = hiperRect.x - offset;
				f = true;
				hasLast = !SHOW_PARENT_TREE_CURRENTOBJECT;
				while ( parent/* && parentRect.x + BW > 0*/ != null )
				{
					//if ( hasLast ) calcConetnt.text = "";// "..";
					if ( hasLast ) calcConetnt.text = "/. ";
					if ( parent == null ) break;
					if ( hasLast ) calcConetnt.text = parent.name + calcConetnt.text;
					else if ( f ) calcConetnt.text = parent.name;
					else calcConetnt.text = parent.name + CHAR;
					f = false;
					float calcedW = CalcWidth( FS);
					// if (w > BW) w = BW;
					parentRect.width = calcedW;
					parentRect.x -= parentRect.width - 4;


					hiperRect = parentRect;
					hiperRect.x = hiperRect.x - offset;


					if ( Event.current.control || Event.current.shift || Event.current.alt )
						EditorGUIUtility.AddCursorRect( hiperRect, Event.current.alt ? MouseCursor.ArrowMinus : MouseCursor.ArrowPlus );
					//else
					//	EditorGUIUtility.AddCursorRect( hiperRect, MouseCursor.SlideArrow );

					UnityEngine.Object obj = null;
					if ( !hasLast )
						if ( hiperRect.Contains( Event.current.mousePosition ) || w.HierarchyController.selection_action != null ) obj = EditorUtility.InstanceIDToObject( parent.id );

					FOLDER_BUTTON( w, ref foldRect, obj, SET_SELECT_OBECJT, null, calcConetnt.text, calcConetnt.text, v2offset, SET_DRAG_OBECJT, FS );

					wasDraw = true;

					if ( hasLast )
					{
						hasLast = false;
						//continue;
					}

					parent = parent.parent();
				}

#endif
				if ( !wasDraw )
				{
					labelLeft8styl2.fontSize = FS;// ?? p.FONT_8_FOR_BOTTOM();
					GUI.Label( treeRect, no_cone, labelLeft8styl2 );
					calcConetnt.text = no_cone.text;
					DRAW_CATEGORY_NAME_S = treeRect.x + CalcWidth( FS );
				}


				GUI.EndClip();



				return true;
			}



			return false;
		}



		GUIContent no_cone = new GUIContent("-");

		bool IS_HIERARCHY() { return !IS_PROJECT(); }
		bool IS_PROJECT() { return false; }
		void FoldActions( BottomBarWindowInstance w, Action<bool?, BottomBarWindowInstance> release )
		{
			if ( Event.current.type == EventType.MouseDown && hiperRect.Contains( Event.current.mousePosition ) )
			{

				var b = Event.current.button;

				w.HierarchyController.selection_button = DRAW_FOLD_ICONS_CONTROLID + HYPER_OFFSET;
				w.HierarchyController.selection_window = w;
				var captureRect = hiperRect;
				w.HierarchyController.selection_action = ( mouseUp, deltaTIme ) => {
					if ( mouseUp && captureRect.Contains( Event.current.mousePosition ) )
					{
						if ( !USE_DOWN )
						{
							if ( b == 1 ) SET_HYPER();
							else release( null, w );
						}
					}
					return Event.current.delta.x == 0 && Event.current.delta.x == 0;
				}; // ACTION
				if ( USE_DOWN )
					p.PUSH_GUI_ONESHOT( PluginId, () => {
						if ( b == 1 ) SET_HYPER();
						else release( null, w );
					} );

			}
			if ( Event.current.type == EventType.Repaint ) //HOVER
				if ( w.HierarchyController.selection_action != null && w.HierarchyController.selection_button != -1
						&& w.HierarchyController.selection_button == DRAW_FOLD_ICONS_CONTROLID + HYPER_OFFSET )
					//GUI.DrawTexture( hiperRect, adapter.GetIcon( "BUTBLUE" ) );
					p.gl.DRAW_TAP_GLOW( hiperRect );
		}

		private void DRAW_FOLD_ICONS( BottomBarWindowInstance w, ref Rect foldRect )
		{

			DRAW_FOLD_ICONS_CONTROLID = 0;
			foldRect.y = Mathf.RoundToInt( foldRect.y );
			hiperRect = foldRect;
			hiperRect.y = Mathf.RoundToInt( hiperRect.y );
			hiperRect.width = 36;
			hiperRect.height = 16;

			hiperRect.x = foldRect.x + foldRect.width - hiperRect.width;




			if ( IS_PROJECT() )     //FAV WINDOW
			{
				//EditorGUIUtility.AddCursorRect( hiperRect, adapter.FAV_ENABLE() ? MouseCursor.ArrowMinus : MouseCursor.ArrowPlus );
				//GUI.DrawTexture( hiperRect, adapter.GetIcon( (hyperGraph.HYPER_FULL_ENABLE() ? "FAVORIT ACTIVE" : "FAVORIT") + (EditorGUIUtility.isProSkin ? "" : " PERSONAL") ) );
				//FoldActions( (Action<bool?>)favorGraph.SWITCH_ACTIVE );
				//iconsContent.tooltip = "Favorites Navigator";
				//iconsContent.text = null;
				//Label( hiperRect, iconsContent );
				//
				//foldRect.width -= hiperRect.width + 5;
				//hiperRect.x -= 20;
				//++DRAW_FOLD_ICONS_CONTROLID;

			}
			else       //HYPER WIN
			{

				if ( USE_MAIN_HYPER )
				{
					EditorGUIUtility.AddCursorRect( hiperRect, w.GRAPH_ENABLED ? MouseCursor.ArrowMinus : MouseCursor.ArrowPlus );
					GUI.DrawTexture( hiperRect, p.GetExternalModOld( (w.HYPER_FULL_ENABLE ? "HIPERGRAPH_ACTIVE" : "HIPERGRAPH") + (p.GET_SLOW_oldProSkin ? "" : " PERSONAL") ) );
					FoldActions( w, SWITCH_HYPER_ACTIVE_FOR_FIRST_HIERARCY_ );
					iconsContent.tooltip = "Open Object References - HyperGraph (CTRL+SHIFT+X)";
					iconsContent.text = null;
					Label( hiperRect, iconsContent );
				}


				foldRect.width -= hiperRect.width + 5;
				hiperRect.x -= 20;
				++DRAW_FOLD_ICONS_CONTROLID;
			}


			hiperRect.height = 18;
			hiperRect.width = hiperRect.height;
			hiperRect.y -= (foldRect.height - hiperRect.height) / 2;
			hiperRect.y -= (18 - EditorGUIUtility.singleLineHeight);
			var OO = 4;
			hiperRect.y = Mathf.RoundToInt( hiperRect.y );
			hiperRect.width -= OO;
			hiperRect.height -= OO;
			//hiperRect.x += OO;

			if ( p.WIN_SET.BOTTOMBAR_DRAW_HOT_BUTTON )
			{
				if ( USE_MAIN_SCENES_ROWS ) FOLDER_BUTTON( w, ref foldRect, SHOW_SCENES_ROWS, SET_SCEN, "NEW_BOTTOM_BUTTON_SCENE",
					GetContent( SHOW_SCENES_ROWS, DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Scenes ).NAME ).text );
				if ( USE_MAIN_HIERARCHYSLOTS_ROWS ) FOLDER_BUTTON( w, ref foldRect, SHOW_HIERARCHYSLOTS_ROWS, SET_HIER, "NEW_BOTTOM_BUTTON_HIERARCHY",
					GetContent( SHOW_SCENES_ROWS, DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Hier ).NAME ).text );
				if ( USE_MAIN_LAST_ROWS )
				{
					hiperRect.x -= p.WIN_SET_G( PluginId ).BOTTOMBAR_ICONS_SPACE;
					FOLDER_BUTTON( w, ref foldRect, SHOW_LAST_ROWS, SET_LAST, "NEW_BOTTOM_BUTTON_LAST",
						GetContent( SHOW_SCENES_ROWS, DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Last ).NAME ).text
						);
				}
				if ( USE_MAIN_BOOKMARKS_ROWS )
				{
					hiperRect.x -= p.WIN_SET_G( PluginId ).BOTTOMBAR_ICONS_SPACE;
					FOLDER_BUTTON( w, ref foldRect, SHOW_BOOKMARKS_ROWS, SET_BOOK, "NEW_BOTTOM_BUTTON_BOOKMARKS",
						GetContent( SHOW_SCENES_ROWS, DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Custom ).NAME ).text );
				}
			}


			DrawHotButton( w, ref foldRect );
			//foldRect.width =  foldRect.x + foldRect.width - (outRect.x + )

			var treeRect = foldRect;
			treeRect.x += treeRect.height;
			treeRect.width = hiperRect.x - treeRect.x;

			var doubleClickRect = treeRect;


			if ( SHOW_PARENT_TREE )
			{
				if ( DRAW_CATEGORY_NAME( w, ref foldRect, treeRect ) )
				{
					doubleClickRect.width = doubleClickRect.x + doubleClickRect.width - DRAW_CATEGORY_NAME_S;
					doubleClickRect.x = DRAW_CATEGORY_NAME_S;
				}
			}
			else if ( p.WIN_SET_G( PluginId ).BOTTOMBAR_SHOW_CAT_NAME )
			{
				// var f = foldRect;
				//f.width /= 2;

				//treeRect.width /= 2;
				//var back = doubleClickRect;
				//doubleClickRect.width = doubleClickRect.x + doubleClickRect.width - (treeRect.x + treeRect.width);
				//doubleClickRect.x = (treeRect.x + treeRect.width);

				if ( DRAW_CATEGORY_NAME_ONLY( w, ref foldRect, treeRect ) )
				{
					doubleClickRect.width = doubleClickRect.x + doubleClickRect.width - (againRect.x + againRect.width);
					doubleClickRect.x = (againRect.x + againRect.width);
				}
				//foldRect.y = f.y;
			}


			if ( doubleClickRect.height != 1 && anycatsenable )
			{
				var dc = p.WIN_SET_G(PluginId).BOTTOMBAR_USE_DOUBLE_CLICK;
				EditorGUIUtility.AddCursorRect( doubleClickRect, MouseCursor.Link );
				Root.SetMouseTooltip( dc ? "Double click to show/hide" : "Click to show/hide", doubleClickRect );
				if ( Event.current.type == EventType.MouseDown )
				{
					//Event.current.button == 0 && 
					// Debug.Log()
					if ( doubleClickRect.Contains( Event.current.mousePosition ) && (!dc || Event.current.clickCount == 2) ) //Event.current.clickCount == 2 &&
					{
						w.BOTTOM_AUTOHIDE = !w.BOTTOM_AUTOHIDE;
						// adapter.SavePrefs();
					}
				}
			}



			if ( anycatsenable )
			{
				hiperRect = foldRect;
				hiperRect.width = hiperRect.height;
				foldRect = hiperRect;
				hiperRect.y -= 1;
				// var foldRectContains = foldRect.Contains(Event.current.mousePosition);
				// var foldAction = HierarchyController.selection_window == w && HierarchyController.selection_button == 0;
				if ( anycatsenable )
				{
					GUI.DrawTexture( hiperRect, p.GetExternalModOld( !w.BOTTOM_AUTOHIDE ? "NEW_BOTTOM_ARROW_DOWN" : "NEW_BOTTOM_ARROW_UP" ) );
					//  EditorStyles.foldout.Draw(foldRect, /*new GUIContent(""), */false, false, !BOTTOM_AUTOHIDE,foldRectContains && foldAction);
					Label( hiperRect, FoldContent );
					EditorGUIUtility.AddCursorRect( hiperRect, MouseCursor.Link );
				}

			}



			// foldRect.width -= hiperRect.width + 5;

		}




		void HOT_BUT_RELEASE( object value, int b, BottomBarWindowInstance w )
		{
			var item = (ExternalMod_Button) value;
			//var b = Event.current.button;
			if ( b == 0 )
			{
				p.PUSH_UPDATE_ONESHOT( PluginId, () => {
					item.release( b, item.text );

				} );

			}
			else
			{
				item.release( b, item.text );

			}
		}



		GUIContent hot_content = new GUIContent();
		void DrawHotButton( BottomBarWindowInstance w, ref Rect foldRect )
		{
#if !EMX_H_LITE
			if ( !p.par_e.DRAW_BOTTOM_HOTBUTTONS || p.modsController.toolBarModification.hotButtons.externalMod_Buttons.Count == 0 ) return;



			//var proprect = _dr;
			//var proprect = p.fullLineRect;
			//proprect.y = 0;
			//if ( p.modsController.rightModsManager.headerEventsBlockRect.HasValue )
			//{
			//    proprect.y = p.modsController.rightModsManager.headerEventsBlockRect.Value.y;
			//    proprect.x = proprect.x + proprect.width - p.rightOffset - p.par_e.RIGHT_RIGHT_PADDING - //p.modsController.rightModsManager.propWidth;
			//}
			//else
			//{
			//    proprect.x = proprect.x + proprect.width - p.WIN_SET.LINE_HEIGHT * 2;
			//}
			//proprect.width = p.par_e.HEADER_HOTBUTTON_SEZE;


			hiperRect.x -= p.WIN_SET_G( PluginId ).BOTTOMBAR_ICONS_SPACE;

			{//HOTS 

				//var hot_r = proprect;
				//	hot_r.x -= hot_r.width;
				hiperRect.width = p.par_e.BOTTOM_HOTBUTTON_SEZE;
				//hiperRect.x -= hiperRect.width;
				//hot_r.x -= 6;
				//int asd = 900;


				foreach ( var item in p.modsController.toolBarModification.hotButtons.externalMod_Buttons_Inverse )
				{

					if ( !item.enabled.HasValue ) throw new Exception( "externalMod_Buttons enabled" );
					if ( !item.enabled.Value ) continue;
					//Root.SetMouseTooltip(item.text + "\n- Left click to open window\n- Right click to open fast context menu", hot_r);


					hot_content.tooltip = item.text + "\n- Click to open window\n- Right-Click to open fast context menu";

					var oldh = hiperRect;

					//var draw_hot = hot_r;
					hiperRect.y += (hiperRect.height - p.par_e.BOTTOM_HOTBUTTON_SEZE) / 2;
					hiperRect.height = hiperRect.height = p.par_e.BOTTOM_HOTBUTTON_SEZE;

					var ic = p.GetOldIcon(item.icon(), true).texture;
					var ac = item;
					FOLDER_BUTTON( w, ref foldRect, ac, HOT_BUT_RELEASE, null, item.text, iconObject: ic );

					hiperRect.x -= Mathf.Max( p.WIN_SET_G( PluginId ).BOTTOMBAR_ICONS_SPACE - 4, 0 );
					hiperRect.x -= 5;

					hiperRect.y = oldh.y;
					hiperRect.height = oldh.height;


					//if ( draw_hot.Contains( p.EVENT.mousePosition ) && p.EVENT.type == EventType.MouseDown && p.window.mouseEventDrag == null && p.window.mouseEvent == null )
					//{
					//	var captureRect = draw_hot;
					//	var button =  p.EVENT.button;
					//	if ( !p.window.rightModIndexAndOnMouseUp.ContainsKey( asd ) )
					//		p.window.rightModIndexAndOnMouseUp.Add( asd, () => {
					//			if ( captureRect.Contains( p.EVENT.mousePosition ) )
					//			{
					//				item.release( button, item.text );
					//			}
					//		} );
					//	p.EVENT.Use();
					//}
					//
					//
					//
					//Root.SetMouseTooltip( hot_content, draw_hot );
					//EditorGUIUtility.AddCursorRect( draw_hot, MouseCursor.Link );
					//if ( p.window.rightModIndexAndOnMouseUp.ContainsKey( 999 ) ) p.gl.DRAW_TAP_GLOW( _dr );
					//
					//
					//if ( Event.current.type == EventType.Repaint )
					//{
					//	var c=  GUI.color;
					//	GUI.color *= new Color( 1, 1, 1, 0.4f );
					//	GUI.skin.button.Draw( draw_hot, hot_content, false, false, false, false );
					//	GUI.color = c;
					//}
					//
					//if ( Event.current.type == EventType.Repaint ) p.gl._DrawTexture( draw_hot, Root.p[ 0 ].GetOldIcon( item.icon() ) );
					//if ( p.window.rightModIndexAndOnMouseUp.ContainsKey( asd ) ) p.gl.DRAW_TAP_GLOW( draw_hot );
					//
					//asd++;

					//	GUILayout.Space(2);

					//hot_r.x -= hot_r.width;
				}

			}
#endif
		}























		// public void Updater()
		// {
		//
		//     foreach ( var scrollPo in adapter.GUIControlToWindowObject )
		//     {
		//         adapter.ScrollPositions[ scrollPo.Key ] = null;
		//         adapter.WasPaint[ scrollPo.Key ] = null;
		//         adapter.WasEvent[ scrollPo.Key ] = null;
		//         adapter.WasInitDraw[ scrollPo.Key ] = null;
		//     }
		//
		//     if ( NeedRefreshBOttom )
		//     {
		//         NeedRefreshBOttom = false;
		//         Scene_RefreshGUIAndClearActions( LastActiveScene );
		//     }
		//
		//     if ( adapter.window() == null ) return;
		//
		//     var old = HEIGHT;
		//     HEIGHT = Mathf.RoundToInt( adapter.ENABLE_BOTTOMDOCK_PROPERTY ? REFERENCE_HEIGHT : 0f );
		//
		//
		//     if ( HEIGHT != old )
		//     {
		//         adapter.RepaintWindowInUpdate();
		//     }
		//
		// }



		internal GUIContent ___GETTOOLTIPPEDCONTENT = new GUIContent();
		private GUIContent GETTOOLTIPPEDCONTENT( MemType type, string upline, ExternalDrawContainer controller )
		{
			___GETTOOLTIPPEDCONTENT.text = "";
			___GETTOOLTIPPEDCONTENT.tooltip = "";

			// if ( type == MemType.Custom ) Debug.Log( (controller.selection_action != null) + " " + IsValidDrag() + " " + (upline != null).ToString() );
			if ( controller.selection_action != null /*|| IsValidDrag() */) return ___GETTOOLTIPPEDCONTENT;

			if ( upline != null ) ___GETTOOLTIPPEDCONTENT.tooltip += upline + "\n";

			switch ( type )
			{
				case MemType.Last:
					// ___GETTOOLTIPPEDCONTENT.tooltip += "You can switch between recently selected GameObjects";
					return ___GETTOOLTIPPEDCONTENT;

				case MemType.Custom:
					// ___GETTOOLTIPPEDCONTENT.tooltip += "You can store these GameObjects in this line";
					return ___GETTOOLTIPPEDCONTENT;

				case MemType.Scenes:

					// content.tooltip = objectIsHiddenAndLock ? "Object hidden" : "Left CLICK/Left DRAG Show/Hide GameObject \n( Right CLICK/Right DRAG - Focus on the object in the SceneView )";
					//   ___GETTOOLTIPPEDCONTENT.tooltip += "Left CLICK - to load Scene\nShift+Left CLICK - to additive load Scene\nCtrl+Left CLICK - to select Scene in Project";
					return ___GETTOOLTIPPEDCONTENT;

				case MemType.Hier:
					___GETTOOLTIPPEDCONTENT.tooltip += "You can create a snapshot of objects that have been expanded";
					return ___GETTOOLTIPPEDCONTENT;

				default:
					throw new ArgumentOutOfRangeException( "type", type, null );
			}
		}



		internal void Label( Rect r, string s, TextAnchor an )
		{
			var a  = p. label.alignment;
			p.label.alignment = an;
			GUI.Label( r, s, p.label );
			p.label.alignment = a;
		}
		internal void Label( Rect r, string s )
		{
			GUI.Label( r, s, p.label );
		}
		internal void Label( Rect r, GUIContent s )
		{
			GUI.Label( r, s, p.label );
		}

		internal bool Button( Rect r, string s )
		{
			return GUI.Button( r, s, p.button );
		}
		internal bool Button( Rect r, string s, TextAnchor an )
		{
			var a  = p.button.alignment;
			p.button.alignment = an;
			var res = GUI.Button( r, s, p.button );
			p.button.alignment = a;
			return res;
		}
		internal bool Button( Rect r, GUIContent s )
		{
			return GUI.Button( r, s, p.button );
		}
		internal bool Button( Rect r, GUIContent s, TextAnchor an )
		{
			var a  = p.button.alignment;
			p.button.alignment = an;
			var res = GUI.Button( r, s, p. button );
			p.button.alignment = a;
			return res;

		}



	}



	class BottomBarExternalHelper
	{














		// internal void INIT_ON_LOAD()     // Application.isPlaying = false;
		// {
		//     cacheInit = false;
		//     hyperGraph.HierHyperController.wasInit = false;
		//     hyperGraph.WindowHyperController.wasInit = false;
		//     onSelectionChange = null;
		//     //  if (onSceneChange != null) onSceneChange();
		//     onSceneChange = null;
		//
		//     HierarchyController.selection_action = null;
		//
		//     foreach ( var windowController in WindowController )
		//         windowController.selection_action = null;
		//
		//     foreach ( var windowController in FavoritControllers )
		//         windowController.selection_action = null;
		//
		//     /* EditorApplication.playmodeStateChanged -= PLAY_MODE_CHANGE;
		//	 EditorApplication.playmodeStateChanged += PLAY_MODE_CHANGE;*/
		// }
		//
		// private void Scene_RefreshGUIAndClearActions( int scene )
		// {
		//     RefreshMemCache( scene );
		//     ClearAction();
		// }
		//
		// private void SelectionChanged()
		// {
		//     ClearAction();
		// }






		// internal void ClearAction()
		// {
		//
		//
		//     foreach ( var windowController in WindowController )
		//     {
		//         windowController.wasDrag = false;
		//         windowController.selection_action = null;
		//         windowController.selection_button = null;
		//         windowController.selection_window = null;
		//     }
		//
		//     foreach ( var windowController in FavoritControllers )
		//     {
		//         windowController.wasDrag = false;
		//         windowController.selection_action = null;
		//         windowController.selection_button = null;
		//         windowController.selection_window = null;
		//     }
		//
		//     HierarchyController.wasDrag = false;
		//     HierarchyController.selection_action = null;
		//     HierarchyController.selection_button = null;
		//     HierarchyController.selection_window = null;
		//
		//
		//     hyperGraph.currentActionID = -1;
		//     favorGraph.currentActionID = -1;
		//     // HiperGraph.currentActionWindow = null;
		//     hyperGraph.WindowHyperController.currentAction = null;
		//     hyperGraph.HierHyperController.currentAction = null;
		//     favorGraph.WindowFavorController.currentAction = null;
		//     favorGraph.HierFavorController.currentAction = null;
		// }


	}
}