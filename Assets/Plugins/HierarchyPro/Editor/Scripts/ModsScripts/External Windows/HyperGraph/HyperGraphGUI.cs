using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace EMX.HierarchyPlugin.Editor.Mods.HyperGraph
{


	partial class HyperGraphModInstance : ExternalModStyles, ICHANGE_SELECTION_OVVERIDE
	{



		internal bool H_AUTOHIDE { get { return adapter.par_e.GET( "HYPERGRAPH_AUTOHIDE_" + CURRENT_CONTROLLER.controller_type, false ); } set { var r = H_AUTOHIDE; adapter.par_e.SET( "HYPERGRAPH_AUTOHIDE_" + CURRENT_CONTROLLER.controller_type, value ); } }
		//internal bool H_AUTOHIDE { get { return adapter.par_e.GET( "HYPERGRAPH_AUTOHIDE_HIERARCHY_", false ); } set { var r = H_AUTOHIDE; adapter.par_e.SET( "H_AUTOHIDE", value ); } }
		internal bool H_AUTOCHANGE { get { 
				if ( H_AUTOHIDE ) return true;
				return adapter.par_e.GET( "HYPERGRAPH_AUTOCHANGE_" + CURRENT_CONTROLLER.controller_type, true );
			} set {
				if ( H_AUTOHIDE ) return;
				var r = H_AUTOCHANGE; adapter.par_e.SET( "HYPERGRAPH_AUTOCHANGE_" + CURRENT_CONTROLLER.controller_type, value ); 
			} }
		internal float H_SCALE { get { return Mathf.Clamp( adapter.par_e.GET( "HYPERGRAPH_SCALE_" + CURRENT_CONTROLLER.controller_type, 1f ), 0.5f, 2f ); } set { var r = H_SCALE; adapter.par_e.SET( "HYPERGRAPH_SCALE_" + CURRENT_CONTROLLER.controller_type, value ); } }




		public int INTERFACE_SIZE {
			get {
				return Mathf.RoundToInt( FONT_SIZE_INTERFACE() * 2.5f );

			}
		}

		internal HyperGraphModInstance( Func<ExternalDrawContainer> d )
		{
			_CURRENT_CONTROLLER = d;
		}


		protected Vector2 _mConvertRect( Vector2 r )
		{ /* r.x = r.x / CURRENT_SCALE - CURRENT_CONTROLLER.scrollPos.x;
			     r.y = r.y / CURRENT_SCALE - CURRENT_CONTROLLER.scrollPos.y;*/
			/* r.x = r.x * CURRENT_SCALE + CURRENT_CONTROLLER.scrollPos.x;
			 r.y = r.y * CURRENT_SCALE + CURRENT_CONTROLLER.scrollPos.y;*/
			/*r.width *= CURRENT_SCALE;
			r.height *= CURRENT_SCALE;*/
			return r;
		}

		protected void _mConvertRect( ref Rect r )
		{
			r.x = r.x * CURRENT_SCALE + CURRENT_CONTROLLER.scrollPos.x;
			r.y = r.y * CURRENT_SCALE + CURRENT_CONTROLLER.scrollPos.y;
			r.width *= CURRENT_SCALE;
			r.height *= CURRENT_SCALE;
		}

		protected void _mConvertRect_Unscalable( ref Rect r )
		{
			r.x = r.x * CURRENT_SCALE + CURRENT_CONTROLLER.scrollPos.x;
			r.y = r.y * CURRENT_SCALE + CURRENT_CONTROLLER.scrollPos.y;
		}


		GUIContent LABEL_content = new GUIContent();

		protected void LABEL( Rect rect, string content )
		{
			LABEL_content.text = content;
			LABEL( rect, LABEL_content );
		}

		protected void LABEL( Rect rect, GUIContent content )
		{
			_mConvertRect( ref rect );
			Label( rect, content );
		}

		protected void Draw( Rect rect, GUIStyle style, bool b1, bool b2, bool b3, bool b4 )
		{
			LABEL_content.text = null;
			Draw( rect, LABEL_content, style, b1, b2, b3, b4 );
		}

		protected void Draw( Rect rect, GUIContent content, GUIStyle style, bool b1, bool b2, bool b3, bool b4 )
		{
			_mConvertRect( ref rect );
			style.Draw( rect, content, b1, b2, b3, b4 );
		}

		protected void DrawTexture_Unscalable( Rect point, Rect ScreenRect, Color color )
		{
			_mConvertRect_Unscalable( ref point );
			Graphics.DrawTexture( point, Texture2D.whiteTexture, ScreenRect, 0, 0, 0, 0, color );
		}


		protected void GL_VERTEX3( Vector3 r )
		{
			r.x = r.x * CURRENT_SCALE + CURRENT_CONTROLLER.scrollPos.x;
			r.y = r.y * CURRENT_SCALE + CURRENT_CONTROLLER.scrollPos.y;
			GL.Vertex3( r.x, r.y, 0 );
		}

		protected void TOOLTIP_WITH_SCALE( Rect r, GUIContent content )
		{
			_mConvertRect( ref r );
			TOOLTIP( r, content );
		}







		protected private GUIStyle HIPERUI_GAMEOBJECT {
			get {
				m_HIPERUI_GAMEOBJECT.padding.left = (int)(16 * CURRENT_SCALE);
				m_HIPERUI_GAMEOBJECT.padding.right = (int)(3 * CURRENT_SCALE);
				return m_HIPERUI_GAMEOBJECT;
			}

			set { m_HIPERUI_GAMEOBJECT = value; }
		}

		protected GUIStyle HIPERUI_INOUT_A {
			get {
				m_HIPERUI_INOUT_A.padding.left = (int)(3 * CURRENT_SCALE);
				m_HIPERUI_INOUT_A.padding.right = (int)(3 * CURRENT_SCALE);
				return m_HIPERUI_INOUT_A;
			}

			set { m_HIPERUI_INOUT_A = value; }
		}

		protected GUIStyle HIPERUI_INOUT_B {
			get {
				m_HIPERUI_INOUT_B.padding.left = (int)(3 * CURRENT_SCALE);
				m_HIPERUI_INOUT_B.padding.right = (int)(3 * CURRENT_SCALE);
				return m_HIPERUI_INOUT_B;
			}

			set { m_HIPERUI_INOUT_B = value; }
		}

		protected GUIStyle HIPERUI_LINE_BLUEGB {
			get {
				m_HIPERUI_LINE_BLUEGB.padding.left = (int)(3 * CURRENT_SCALE);
				m_HIPERUI_LINE_BLUEGB.padding.right = (int)(4 * CURRENT_SCALE);
				return m_HIPERUI_LINE_BLUEGB;
			}

			set { m_HIPERUI_LINE_BLUEGB = value; }
		}

		protected GUIStyle HIPERUI_LINE_BLUEGB_PERSONAL {
			get {
				m_HIPERUI_LINE_BLUEGB_PERSONAL.padding.left = (int)(3 * CURRENT_SCALE);
				m_HIPERUI_LINE_BLUEGB_PERSONAL.padding.right = (int)(4 * CURRENT_SCALE);
				return m_HIPERUI_LINE_BLUEGB_PERSONAL;
			}

			set { m_HIPERUI_LINE_BLUEGB_PERSONAL = value; }
		}

		protected GUIStyle HIPERUI_LINE_BOX {
			get {
				m_HIPERUI_LINE_BOX.padding.left = (int)(3 * CURRENT_SCALE);
				m_HIPERUI_LINE_BOX.padding.right = (int)(3 * CURRENT_SCALE);
				return m_HIPERUI_LINE_BOX;
			}

			set { m_HIPERUI_LINE_BOX = value; }
		}

		protected GUIStyle HIPERUI_LINE_RDTRIANGLE {
			get {
				m_HIPERUI_LINE_RDTRIANGLE.padding.left = (int)(3 * CURRENT_SCALE);
				m_HIPERUI_LINE_RDTRIANGLE.padding.right = (int)(3 * CURRENT_SCALE);
				return m_HIPERUI_LINE_RDTRIANGLE;
			}

			set { m_HIPERUI_LINE_RDTRIANGLE = value; }
		}
		protected GUIStyle HIPERUI_LINE_RDTRIANGLE_INVERSE {
			get {
				if ( m_HIPERUI_LINE_RDTRIANGLE_INVERSE == null )
				{
					m_HIPERUI_LINE_RDTRIANGLE_INVERSE = new GUIStyle( HIPERUI_LINE_RDTRIANGLE );
					m_HIPERUI_LINE_RDTRIANGLE_INVERSE.alignment = TextAnchor.MiddleRight;
				}
				m_HIPERUI_LINE_RDTRIANGLE_INVERSE.padding.left = (int)(3 * CURRENT_SCALE);
				m_HIPERUI_LINE_RDTRIANGLE_INVERSE.padding.right = (int)(3 * CURRENT_SCALE);
				return m_HIPERUI_LINE_RDTRIANGLE_INVERSE;
			}

			set { m_HIPERUI_LINE_RDTRIANGLE_INVERSE = value; }
		}
		protected GUIStyle HIPERUI_MARKER_BOX {
			get {
				//m_HIPERUI_MARKER_BOX.padding.left = (int)(3 * CURRENT_SCALE);
				//m_HIPERUI_MARKER_BOX.padding.right = (int)(3 * CURRENT_SCALE);
				return m_HIPERUI_MARKER_BOX;
			}

			set { m_HIPERUI_MARKER_BOX = value; }
		}


















		internal void SWITCH_ACTIVE_SCAN( bool? overrideActive )
		{

			if ( !WASSCAN && !ReferenceEquals( CURRENT_WIN, null ) && CURRENT_WIN.OvValide() )
			{
				comps = null;
				StartBroadcasting();
			}
		}

		internal void DRAG_PERFORMER_SCAN()
		{
			var n = DragAndDrop.objectReferences.First(g => g is GameObject && ((GameObject)g).scene.IsValid());
			CHANGE_SELECTION_OVVERIDE( true, n );
		}

		public void Update()
		{

			if ( ReferenceEquals( CURRENT_WIN, null ) || !CURRENT_WIN.OvValide() ) return;

			if ( SCANNING_COMPS.Count != 0 )
			{
				lock ( SCANNING_COMPS )
				{
					for ( int i = SCANNING_COMPS.Count - 1; i >= 0; i-- )
					{
						var objectDisplay = SCANNING_COMPS[i];

						if ( objectDisplay.WasAccessorInitialize_InMainThread ) //   SCANNING_COMPS.RemoveAt(i);
						{
							continue;
						}

						var ob = EditorUtility.InstanceIDToObject(objectDisplay.gameObjectId) as GameObject;

						if ( !ob ) continue;

						/*  lock (objectDisplay)
										{*/

						if ( objectDisplay.AllowInitialize_InMainThread() )
						{
							objectDisplay.InitializeAccessor_InMainThread();
							//   SCANNING_COMPS.RemoveAt(i);
							// SCANNING_COMPS_REMOVER.Add(ob.GetInstanceID());
						}

						// }
					}

					SCANNING_COMPS.Clear();
				}
			}

			/* foreach (var objectDisplay in SCANNING_COMPS)
					 {

					 }*/
			/*  if (SCANNING_COMPS_REMOVER.Count != 0)
					  {
						  foreach (var i in SCANNING_COMPS_REMOVER)
						  {
							  SCANNING_COMPS.Remove(i);
						  }
						  SCANNING_COMPS_REMOVER.Clear();
					  }*/
			/* if (BottomInterface.HiperGraph.currentAction != null)
					 {
						 Repaint();
					 }*/



			//if (CURRENT_WIN.OvValide()) 
			CalcBroadCast();
		}

		internal void CHECK_SCAN()
		{
			if ( !WASSCAN )
			{
				comps = null;
				// StoptBroadcasting();
				StartBroadcasting();
			}
		}


		/*    int framecount = 0;
				  double lastTime;*/

		const float GR_SIZE = 120;

		/*  float GR_SIZE {
					get { return 120 * HALF_SCALE(); }
				}*/





		Rect RECT = new Rect();
		Rect LOCAL_RECT = new Rect();

		// Rect CONTENT_RECT;
		Rect InterfaceRect = new Rect();


		void RepaintNow()
		{
			if ( !ReferenceEquals( CURRENT_WIN, null ) && CURRENT_WIN.OvValide() ) CURRENT_WIN.RepaintNow();

		}


		Color HIPER_INTERFACE = new Color(0.75f, 0.75f, 0.75f, 0.5f);


		PluginInstance adapter { get { return Root.p[ 0 ]; } }

		internal Rect CAPTURE_CLIP_RECT;
		IExternalWindow CURRENT_WIN;
		Func<ExternalDrawContainer > _CURRENT_CONTROLLER = null;
		ExternalDrawContainer CURRENT_CONTROLLER { get { return _CURRENT_CONTROLLER(); } }
		float CURRENT_SCALE = 1;
		internal Actions __actions = new Actions();
		internal Actions actions { get { return __actions ?? (__actions = new Actions()); } }
		Rect dragRect;
		[NonSerialized]
		float? lastWidth, lastHeight;
		bool bold_lines = false;

		internal void EXTERNAL_HYPER_DRAWER( Rect lineRect, ExternalDrawContainer controller, IExternalWindow win )
		{

			bold_lines = adapter.par_e.HYPERGRAPH_USE_BOLD_LINES;
			controller = CURRENT_CONTROLLER;
			controller.tempRoot = win;

			CURRENT_WIN = win;
			INITIALIZE( controller, lineRect.width );

			if ( ReferenceEquals( CURRENT_WIN, null ) || !win.OvValide() )
			{
				return;
			}
			if (  //controller.controller_type == 0 && 
				(win.position( IExternalWindowType.HYPER_GRAPH ).width != lastWidth || win.position( IExternalWindowType.HYPER_GRAPH ).height != lastHeight) )
			{


				if ( !lastWidth.HasValue ) lastWidth = win.position( IExternalWindowType.HYPER_GRAPH ).width;
				if ( !lastHeight.HasValue ) lastHeight = win.position( IExternalWindowType.HYPER_GRAPH ).height;
				//RefreshWithCurrentSelection();
				//CURRENT_CONTROLLER.wasInit = false;

				controller.scrollPos.x += (win.position( IExternalWindowType.HYPER_GRAPH ).width - lastWidth.Value) / 2 ;
				controller.scrollPos.y += (win.position( IExternalWindowType.HYPER_GRAPH ).height - lastHeight.Value) / 2;

				lastWidth = win.position( IExternalWindowType.HYPER_GRAPH ).width;
				lastHeight = win.position( IExternalWindowType.HYPER_GRAPH ).height;

				//lastWidth = win.position( IExternalWindowType.HYPER_GRAPH ).width;
				//lastHeight = win.position( IExternalWindowType.HYPER_GRAPH ).height;
				//controller.wasInit = false;
				//INITIALIZE( controller, lineRect.width );
			}
			else
			{
			}
			INIT_STYLES();

			CURRENT_SCALE = H_SCALE;
			CHECK_RIGHTCLICK( lineRect );


			////////////////// DRAG
			dragRect = lineRect;
			dragRect.height = 5;

			if ( controller.MAIN ) DRAG( controller, win );

			lineRect.y += dragRect.height;
			lineRect.height -= dragRect.height;
			////////////////// DRAG ////////////////////////////////


			InterfaceRect = lineRect;
			InterfaceRect.x = InterfaceRect.y = 0;
			InterfaceRect.height = INTERFACE_SIZE / 2;

			RECT.width = lineRect.width;
			RECT.height = lineRect.height;
			LOCAL_RECT = RECT;
			LOCAL_RECT.x -= CURRENT_CONTROLLER.scrollPos.x / CURRENT_SCALE;
			LOCAL_RECT.y -= CURRENT_CONTROLLER.scrollPos.y / CURRENT_SCALE;
			LOCAL_RECT.width /= CURRENT_SCALE;
			LOCAL_RECT.height /= CURRENT_SCALE;
			//_mConvertRect( ref LOCAL_RECT );

			if ( controller.MAIN ) CAPTURE_CLIP_RECT = lineRect;

			////////////////// MAIN
			// CLIP_RECT = lineRect;
			GUI.BeginClip( lineRect );

			if ( Event.current.type != EventType.Repaint ) INTERFACE( controller );


			// GUI.matrix *=  Matrix4x4.Scale(new Vector3(0.5f, 0.5f, 1));


			INITIALIZE( controller, lineRect.width );


			//CONTENT_RECT.x = scrollPos.x;
			// CONTENT_RECT.y = scrollPos.y;
			// GUI.BeginGroup(CONTENT_RECT);

			MAIN( controller );


			// GUI.matrix = Matrix4x4.Scale(new Vector3(1, 1, 1));

			// GUI.EndGroup();
			////////////////// MAIN ////////////////////////////////

			if ( Event.current.type == EventType.Repaint ) INTERFACE( controller );

			GUI.EndClip();


			CustomDragData.ExampleDragDropGUI( adapter, lineRect, null, actions.DRAG_VALIDATOR, DRAG_PERFORMER, dragColor );

			////////////////// INTERFACE

			//InterfaceRect.y -= 4;


			/*  lineRect.y += InterfaceRect.height;
					  lineRect.height -= InterfaceRect.height;*/
			////////////////// INTERFACE ////////////////////////////////
		}

		void DRAG_PERFORMER()
		{
			actions.DRAG_PERFORMER( this );
		}
		//  Rect CLIP_RECT;



		void DRAG( ExternalDrawContainer controller, IExternalWindow win )
		{
			if ( controller.hide_hierarchy_ui_buttons ) return;

			EditorGUIUtility.AddCursorRect( dragRect, MouseCursor.SplitResizeUpDown );

			if ( Event.current.type == EventType.Repaint )
			{
				adapter.box.Draw( dragRect, false, false, false, false );
				var r = dragRect;
				r.width = 2;
				var asd = GUI.color;
				GUI.color *= BLUE;
				GUI.DrawTexture( r, Texture2D.whiteTexture );
				r.x += r.width * 2;
				GUI.DrawTexture( r, Texture2D.whiteTexture );
				r.x += dragRect.width - r.width * 2 - r.width;
				GUI.DrawTexture( r, Texture2D.whiteTexture );
				r.x -= r.width * 2;
				GUI.DrawTexture( r, Texture2D.whiteTexture );
				GUI.color = asd;
			}


			var EVENT_ID = 100;

			if ( dragRect.Contains( Event.current.mousePosition ) && Event.current.button == 0 &&
				Event.current.type == EventType.MouseDown )
			{
				Tools.EventUse();
				var startPos = EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition);
				//var startHeight = controller.HEIGHT( IExternalWindowType.HYPER_GRAPH );
				var qwe = (BottomBarWindowInstance)win;
				var startHeight = qwe.GRAPH_HEIGHT_MEM;
				actions.ADD_ACTION( EVENT_ID, null, contains => {
					var p = EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition);

					if ( startPos.y == p.y ) return false;

					//  var newHeight = startHeight + (startPos.y - p.y);
					// Debug.Log( adapter.window().position.height );

					/*
					adapter.TEMP_LEFT_CACHE_FOR_BOTTOM = adapter.TOTAL_LEFT_PADDING_FORBOTTOM;
					var oldH = controller.HEIGHT( IExternalWindowType.HYPER_GRAPH );
					var nh = Mathf.RoundToInt((startHeight + (startPos.y - p.y)));
					nh = CHECK_HEIGHT( nh );
					controller.HEIGHT( IExternalWindowType.HYPER_GRAPH ) = nh;
					controller.scrollPos.y -= (oldH - controller.HEIGHT( IExternalWindowType.HYPER_GRAPH )) / 2;
					adapter.RESET_SMOOTH_HEIGHT();*/


					var oldH = qwe.GRAPH_HEIGHT_MEM;
					//var oldH = controller.HEIGHT( IExternalWindowType.HYPER_GRAPH );
					var nh = Mathf.RoundToInt((startHeight + (startPos.y - p.y)));
					nh = qwe.CHECK_HEIGHT( nh );
					qwe.GRAPH_HEIGHT_MEM = nh;
					controller.scrollPos.y -= (oldH - controller.HEIGHT( IExternalWindowType.HYPER_GRAPH )) / 2;
					//adapter.RESET_SMOOTH_HEIGHT();




					return true;
				}, contains => {
					//adapter.SavePrefs();
				}, controller );
			}

			if ( actions.HOVER( EVENT_ID, null, controller ) )
			{
				var asd = GUI.color;
				var b = BLUE;
				b.a = 0.4f;
				GUI.color *= b;
				var h = dragRect;
				h.height -= 3;
				h.y += 1;
				GUI.DrawTexture( h, Texture2D.whiteTexture );
				GUI.color = asd;

				var m = Event.current.mousePosition;
				EditorGUIUtility.AddCursorRect( new Rect( m.x - 100, m.y - 100, 200, 200 ),
					MouseCursor.SplitResizeUpDown );
			}
		}


		GUIStyle __toolbalStyle;
		GUIStyle toolbalStyle {
			get {
				if ( __toolbalStyle == null )
				{
					__toolbalStyle = new GUIStyle( EditorStyles.toolbarButton );
					__toolbalStyle.fixedHeight = 0;
					__toolbalStyle.alignment = TextAnchor.MiddleLeft;
				}
				__toolbalStyle.fontSize = FONT_SIZE_INTERFACE();
				return __toolbalStyle;
			}
		}

		const float ACTIVE_A = 0.3f;
		float ACTIVE_WIDTH { get { return INTERFACE_SIZE / 4; } }

		void DRAW_INTERFACE_BUTTON( int EVENT_ID, string label, bool ACTIVE, Rect r, ExternalDrawContainer controller, bool full = false )
		{
			H_AR = actions.HOVER( EVENT_ID, r, controller );



			var ac  =r;
			if ( !full )
			{
				ac.width = ACTIVE_WIDTH;
				r.x += ACTIVE_WIDTH;
				r.width -= ACTIVE_WIDTH;
			}


			toolbalStyle.Draw( r, label, H_AR, H_AR, H_AR, false );
			if ( ACTIVE )
			{
				//if ( full ) ac = new Rect( ac.x + 2, ac.y + 2, ac.width - 4, ac.height - 4 );
				//else ac = new Rect( ac.x , ac.y + 2, ac.width , ac.height - 4 );
				ac = new Rect( ac.x + 2, ac.y + 2, ac.width - 4, ac.height - 4 );
				adapter.gl.DRAW_TAP_GLOW_ADDITIONAL_MAT( ac, ACTIVE_A );
			}
			EditorGUIUtility.AddCursorRect( r, MouseCursor.Link );
		}

		//bool H_AUTOHIDE {
		//	get {
		//		if ( CURRENT_CONTROLLER.controller_type == 0 ) return adapter.par_e.HYPERGRAPH_AUTOHIDE_WINDOW;
		//		return adapter.par_e.H_AUTOHIDE;
		//	}
		//	set {
		//		if ( CURRENT_CONTROLLER.controller_type == 0 ) adapter.par_e.HYPERGRAPH_AUTOHIDE_WINDOW = value;
		//		adapter.par_e.H_AUTOHIDE = value;
		//	}
		//}

		Color asd;
		bool H_AR;
		void INTERFACE( ExternalDrawContainer controller )
		{
			var procrect = InterfaceRect;
			var hiderect = InterfaceRect;
			procrect.width /= 2;
			hiderect.width /= 2.5f;
			hiderect.x = InterfaceRect.x + InterfaceRect.width - hiderect.width;
			hiderect.width /= 2;
			var autorefreshrect = hiderect;
			hiderect.x += hiderect.width;

			EditorGUIUtility.AddCursorRect( procrect, MouseCursor.Link );

			Label( procrect, procContent );
			Label( autorefreshrect, autorefreshContent );

			if ( !controller.hide_hierarchy_ui_buttons ) Label( hiderect, autohideContent );

			procrect.width /= 5;


			InterfaceRect.y += InterfaceRect.height;
			/*  var hr = InterfaceRect;
			  hr.y += 2;
			 // hr.height -= 4;
			  hr.width /= 2;
			  hr.width /= 3;
			  var HR1 = hr;
			  hr.x += hr.width;
			  var HR2 = hr;
			  hr.x += hr.width;
			  var HR3 = hr;
			  hr.width = InterfaceRect.width / 2 / 5 * 4;
			  hr.x = InterfaceRect.width / 2 + InterfaceRect.width / 10;
			  var HR4 = hr;*/


			var hr = InterfaceRect;
			hr.y += 2;
			// hr.height -= 4;
			hr.width /= 2;
			hr.width /= 3;
			hr.x = InterfaceRect.width / 2;
			var HR1 = hr;
			hr.x += hr.width;
			var HR2 = hr;
			hr.x += hr.width;
			var HR3 = hr;
			hr.width = InterfaceRect.width / 2 / 5 * 4;
			hr.x = 0;
			var HR4 = hr;

			/*__TOOLTIP.text = "";
			__TOOLTIP.tooltip = "Arrays objects will be included";
			Label(HR1, __TOOLTIP);
			__TOOLTIP.tooltip = "Assets objects will be included";
			Label(HR2, __TOOLTIP);
			__TOOLTIP.tooltip = "References that point to themselves objects will be included";
			Label(HR3, __TOOLTIP);
			__TOOLTIP.tooltip = "Only refs to UnityEvents will be shown";
			Label(HR4, __TOOLTIP);*/
			TOOLTIP( HR1, "Arrays objects will be included" );
			TOOLTIP( HR2, "Assets objects will be included" );
			TOOLTIP( HR3, "References that point to themselves objects will be included" );
			TOOLTIP( HR4, "Only refs using UnityEvents will be shown" );


			EditorGUIUtility.AddCursorRect( autorefreshrect, MouseCursor.Link );

			if ( !controller.hide_hierarchy_ui_buttons )
				EditorGUIUtility.AddCursorRect( hiderect, MouseCursor.Link );

			var len = 5;

			var EVENT_ID = 110;

			if ( Event.current.type == EventType.Repaint )
			{
				asd = GUI.color;
				//GUI.color = new Color(1,1,1,0.5f);
				GUI.color *= HIPER_INTERFACE;
				toolbalStyle.fixedHeight = INTERFACE_SIZE / 2;
				//EditorStyles.toolbar.Draw(InterfaceRect, false, false, false, false);
				// Adapter.GET_SKIN().box.Draw(InterfaceRect, false, false, false, false);

				for ( int i = 0; i < len; i++ )
				{
					var st = ((i + 1) * 20) + "%";
					if ( i == len - 1 ) st = "∞";

					H_AR = actions.HOVER( EVENT_ID, procrect, controller );
					var active = adapter.par_e.HYPERGRAPH_SCANPERFOMANCE == perfomanceArray[ i ] ;

					var ac  = procrect;
					if ( active )
					{
						ac.width = ACTIVE_WIDTH;
						//procrect.x += ACTIVE_WIDTH;
						//procrect.width -= ACTIVE_WIDTH;
						ac = new Rect( ac.x + 2, ac.y + 2, ac.width - 4, ac.height - 4 );
					}
					procrect.x += 3;
					procrect.width -= 3;

					toolbalStyle.Draw( procrect, st, H_AR, H_AR, H_AR, false );
					if ( active )
					{
						adapter.gl.DRAW_TAP_GLOW_ADDITIONAL_MAT( ac, ACTIVE_A );
					}
					if ( H_AR )
					{
						adapter.gl.DRAW_TAP_GLOW_ADDITIONAL_MAT( procrect, ACTIVE_A );
					}


					procrect.x += procrect.width;
					procrect.width += 3;
					//if ( active ) procrect.width += ACTIVE_WIDTH;
					EVENT_ID++;
				}

				// if () GUI.DrawTexture(h, Texture2D.whiteTexture);
				if ( !H_AUTOHIDE )
				{
					H_AR = actions.HOVER( EVENT_ID, autorefreshrect, controller );
					toolbalStyle.Draw( autorefreshrect, "AutoReload", H_AR, H_AR, H_AR, false );
					if ( H_AUTOCHANGE || H_AR )
					{
						var ac = new Rect( autorefreshrect.x + 2, autorefreshrect.y + 2, autorefreshrect.width - 4, autorefreshrect.height - 4 );

						adapter.gl.DRAW_TAP_GLOW_ADDITIONAL_MAT( ac , ACTIVE_A );
					}
				}


				EVENT_ID++;

				if ( !controller.hide_hierarchy_ui_buttons )
				{
					H_AR = actions.HOVER( EVENT_ID, hiderect, controller );
					toolbalStyle.Draw( hiderect, "AutoHide", H_AR, H_AR, H_AR, false );
					if ( H_AUTOHIDE || H_AR ) {
						var ac = new Rect( hiderect.x + 2, hiderect.y + 2, hiderect.width - 4, hiderect.height - 4 );

						adapter.gl.DRAW_TAP_GLOW_ADDITIONAL_MAT( ac, ACTIVE_A );
					}
				}

				var en = GUI.enabled;
				//GUI.enabled = !adapter.par_e.HYPERGRAPH_EVENTS_MODE_BOOL;
				DRAW_INTERFACE_BUTTON( ++EVENT_ID, "Arrays", !adapter.par_e.HYPERGRAPH_SKIP_ARRAYS_BOOL, HR1, controller );
				DRAW_INTERFACE_BUTTON( ++EVENT_ID, "Assets", adapter.par_e.HYPERGRAPH_DISPLAY_ASSETS, HR2, controller );
				DRAW_INTERFACE_BUTTON( ++EVENT_ID, "SelfRef", adapter.par_e.HYPERGRAPH_CONNECT_TO_SELFT, HR3, controller );
				//GUI.enabled = en;
				DRAW_INTERFACE_BUTTON( ++EVENT_ID, "EVENTS MODE", adapter.par_e.HYPERGRAPH_EVENTS_MODE_BOOL, HR4, controller, true );


				GUI.color = asd;
			}

			else
			{
				for ( int i = 0; i < len; i++ )
				{
					if ( procrect.Contains( Event.current.mousePosition ) && Event.current.button == 0 &&
						Event.current.type == EventType.MouseDown )
					{
						var captureI = i;
						Tools.EventUse();
						actions.ADD_ACTION( EVENT_ID, procrect, contains => { return false; }, contains => {
							if ( contains )
							{
								adapter.par_e.HYPERGRAPH_SCANPERFOMANCE = perfomanceArray[ captureI ];
							}
						}, controller );
					}

					procrect.x += procrect.width;
					EVENT_ID++;
				}

				if ( !H_AUTOHIDE )
				{
					if ( autorefreshrect.Contains( Event.current.mousePosition ) && Event.current.button == 0 &&
				   Event.current.type == EventType.MouseDown )
					{
						Tools.EventUse();
						actions.ADD_ACTION( EVENT_ID, autorefreshrect, contains => { return false; }, contains => {
							if ( contains )
							{
								H_AUTOCHANGE = !H_AUTOCHANGE;
							}
						}, controller );
					}
				}


				EVENT_ID++;

				if ( !controller.hide_hierarchy_ui_buttons )
				{
					if ( hiderect.Contains( Event.current.mousePosition ) && Event.current.button == 0 &&
						Event.current.type == EventType.MouseDown )
					{
						Tools.EventUse();
						actions.ADD_ACTION( EVENT_ID, hiderect, contains => { return false; }, contains => {
							if ( contains )
							{
								H_AUTOHIDE = !H_AUTOHIDE;
							}
						}, controller );
					}
				}


				var en = GUI.enabled;
				GUI.enabled = !adapter.par_e.HYPERGRAPH_EVENTS_MODE_BOOL;

				if ( GUI.enabled )
				{
					++EVENT_ID;

					if ( HR1.Contains( Event.current.mousePosition ) && Event.current.button == 0 && Event.current.type == EventType.MouseDown )
					{
						Tools.EventUse();
						actions.ADD_ACTION( EVENT_ID, HR1, contains => { return false; }, contains => {
							if ( contains )
							{
								adapter.par_e.HYPERGRAPH_SKIP_ARRAYS_BOOL = !adapter.par_e.HYPERGRAPH_SKIP_ARRAYS_BOOL;
								RefreshWithCurrentSelection();
							}
						}, controller );
					}

					++EVENT_ID;

					if ( HR2.Contains( Event.current.mousePosition ) && Event.current.button == 0 && Event.current.type == EventType.MouseDown )
					{
						Tools.EventUse();
						actions.ADD_ACTION( EVENT_ID, HR2, contains => { return false; }, contains => {
							if ( contains )
							{
								adapter.par_e.HYPERGRAPH_DISPLAY_ASSETS = !adapter.par_e.HYPERGRAPH_DISPLAY_ASSETS;
								RefreshWithCurrentSelection();
							}
						}, controller );
					}

					++EVENT_ID;

					if ( HR3.Contains( Event.current.mousePosition ) && Event.current.button == 0 && Event.current.type == EventType.MouseDown )
					{
						Tools.EventUse();
						actions.ADD_ACTION( EVENT_ID, HR3, contains => { return false; }, contains => {
							if ( contains )
							{
								adapter.par_e.HYPERGRAPH_CONNECT_TO_SELFT = !adapter.par_e.HYPERGRAPH_CONNECT_TO_SELFT;
								RefreshWithCurrentSelection();
							}
						}, controller );
					}
				}

				else
				{
					++EVENT_ID;
					++EVENT_ID;
					++EVENT_ID;
				}

				GUI.enabled = en;
				++EVENT_ID;

				if ( HR4.Contains( Event.current.mousePosition ) && Event.current.button == 0 && Event.current.type == EventType.MouseDown )
				{
					Tools.EventUse();
					actions.ADD_ACTION( EVENT_ID, HR4, contains => { return false; }, contains => {
						if ( contains )
						{
							adapter.par_e.HYPERGRAPH_EVENTS_MODE_BOOL = !adapter.par_e.HYPERGRAPH_EVENTS_MODE_BOOL;
							RefreshWithCurrentSelection();
						}
					}, controller );
				}
			}


			EVENT_ID++;

			var restorex = InterfaceRect.x;

			if ( !H_AUTOHIDE )
			{
				InterfaceRect.y += InterfaceRect.height + 15;
				InterfaceRect.width = LINE_HEIGHT - 3;
				InterfaceRect.height = LINE_HEIGHT - 3;

				//MonoBehaviour.print(Adapter.GET_SKIN().button.normal.textColor);

				if ( Event.current.type == EventType.Repaint && EditorGUIUtility.isProSkin )
					adapter.box.Draw( InterfaceRect, false, false, false, false );

				DO_BUTTON( controller, InterfaceRect, ContentSelBack, CenteredButton, EVENT_ID, DO_UNDO );
				//  Adapter.GET_SKIN().button.Draw(plus, ContentSelBack, false, false, false, plus.Contains(Event.current.mousePosition) && selection_button == 20);

				GUI.Label( InterfaceRect, ContentHiperUndo, style );

				InterfaceRect.x += InterfaceRect.width;

				if ( Event.current.type == EventType.Repaint && EditorGUIUtility.isProSkin )
					adapter.box.Draw( InterfaceRect, false, false, false, false );

				DO_BUTTON( controller, InterfaceRect, ContentSelForw, CenteredButton, EVENT_ID + 1, DO_REDO );
				// Adapter.GET_SKIN().button.Draw(plus, ContentSelForw, false, false, false, plus.Contains(Event.current.mousePosition) && selection_button == 21);

				GUI.Label( InterfaceRect, ContenHiperRedo, style );


				InterfaceRect.x += InterfaceRect.width;
				InterfaceRect.x += InterfaceRect.width;


				if ( Event.current.type == EventType.Repaint && EditorGUIUtility.isProSkin )
					adapter.box.Draw( InterfaceRect, false, false, false, false );

				DO_BUTTON( controller, InterfaceRect, ContentHiperempty, GUI.skin.button, EVENT_ID + 2,
					REFRESH );
				GUI.DrawTexture( new Rect( InterfaceRect.x, InterfaceRect.y, InterfaceRect.width, InterfaceRect.height ), REFRESH_TEXTURE );
				// Adapter.GET_SKIN().button.Draw(plus, ContentSelForw, false, false, false, plus.Contains(Event.current.mousePosition) && selection_button == 21);
				/*   if (Event.current.type == EventType.repaint)*/
				GUI.Label( InterfaceRect, ContenHiperRefresh, style );
			}

			EVENT_ID += 3;
			InterfaceRect.x = restorex;


			InterfaceRect.y += InterfaceRect.height + 3;

			if ( adapter.par_e.HYPERGRAPH_SHOWUPDATINGINDICATOR && CurrentSelection != null )
			{
				if ( SKANNING && Event.current.type == EventType.Repaint || WASDRAW )
				{
					InterfaceRect.width = InterfaceRect.height = 44;

					GUI.DrawTexture( InterfaceRect, Root.icons.LOADING_TEXTURE() );
					InterfaceRect.y += InterfaceRect.height + 3;
					InterfaceRect.height = LINE_HEIGHT;
					var prog = InterfaceRect;
					var val = SKANNING_PROGRESS;
					if ( !SKANNING && WASDRAW )
					{
						WASDRAW = false;
						RepaintNow();
						val = 1;
					}

					else
					{
						WASDRAW = true;
					}

					prog.width *= val;
					//GUI.DrawTexture(prog, adapter.GetIcon("BUTBLUE"));
					adapter.gl.DRAW_TAP_GLOW( prog );
					Label( InterfaceRect, Mathf.RoundToInt( val * 100 ) + "%" );
				}
			}


			var ACTIVE_RECT = RECT;
			ACTIVE_RECT.height -= INTERFACE_SIZE;
			ACTIVE_RECT.y += INTERFACE_SIZE;

			/** CLOASE **/
			if ( !controller.hide_hierarchy_ui_buttons )
			{
				var closeRect = ACTIVE_RECT;
				closeRect.width = 14;
				closeRect.height = 14;
				closeRect.x = RECT.width - closeRect.width - 5;
				closeRect.y += 2;
				EditorGUIUtility.AddCursorRect( closeRect, MouseCursor.Link );

				Label( closeRect, HyperGraphClose_Content );

				EVENT_ID = 200;

				if ( Event.current.type == EventType.Repaint )
				{
					GUI.DrawTexture( closeRect, HIPERUI_CLOSE );
				}

				if ( closeRect.Contains( Event.current.mousePosition ) && Event.current.button == 0 &&
					Event.current.type == EventType.MouseDown )
				{
					Tools.EventUse();
					actions.ADD_ACTION( EVENT_ID, closeRect, contains => { return false; }, contains => {
						if ( contains ) CURRENT_WIN.Close();
						//SWITCH_ACTIVE_SCAN(false);
					}, controller );
				}

				if ( actions.HOVER( EVENT_ID, closeRect, controller ) )
				{
					HIPERUI_BUTTONGLOW.Draw( closeRect, false, false, false, false );
				}

				/** CLOASE **/


				/** DOCK **/
				/*if (!editorWindow)
				{
					closeRect.x -= closeRect.width - 1;
					EditorGUIUtility.AddCursorRect(closeRect, MouseCursor.Link);


					Label(closeRect, HyperGraphWindow_Content);

					EVENT_ID = 201;

					if (Event.current.type == EventType.Repaint)
					{
						GUI.DrawTexture(closeRect, HIPERGRAPH_DOCK);
					}

					if (closeRect.Contains(Event.current.mousePosition) && Event.current.button == 0 &&
						Event.current.type == EventType.MouseDown)
					{
						Tools.EventUse();
						actions.ADD_ACTION(EVENT_ID, closeRect, contains => { return false; }, contains =>
						{
							if (contains) DOCK_HYPER();
						}, controller);
					}

					if (actions.HOVER(EVENT_ID, closeRect, controller))
					{
						HIPERUI_BUTTONGLOW.Draw(closeRect, false, false, false, false);
					}
				}*/

				/** DOCK **/
			}

			/** DRAW_ZOOMER **/
			DRAW_ZOOMER( ACTIVE_RECT, 205, controller );
			/** DRAW_ZOOMER **/
		}
		GUIStyle _style;
		GUIStyle style {
			get {
				if ( _style == null ) _style = new GUIStyle( adapter.STYLE_LABEL_8_middle );
				_style.fontSize = FONT_SIZE();
				return _style;
			}
		}

		int LINE_HEIGHT { get { return Mathf.RoundToInt( EditorGUIUtility.singleLineHeight ); } } //adapter.bottomInterface.LINE_HEIGHT(null)

		private void DO_BUTTON( ExternalDrawContainer controller, Rect rect, GUIContent content, GUIStyle style, int EVENT_ID,
			Action action = null )
		{
			EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

			if ( Event.current.type == EventType.Repaint )
			{

				style.Draw( rect, content, false, false, false, false );

				TOOLTIP( rect, content );

				if ( actions.HOVER( EVENT_ID, rect, controller ) )
				{
					var h = rect;
					var GLOW = 8;
					h.x -= GLOW;
					h.y -= GLOW;
					h.width += GLOW * 2;
					h.height += GLOW * 2;


					HIPERUI_BUTTONGLOW.Draw( h, false, false, false, false );
				}
			}


			if ( Event.current.type == EventType.MouseDown && Event.current.button == 0 &&
				rect.Contains( Event.current.mousePosition ) )
			{
				Tools.EventUse();
				actions.ADD_ACTION( EVENT_ID, rect, contains => { return false; }, contains => {
					if ( contains )
					{
						if ( action != null )
						{
							action();
						}
					}
				}, controller );
			}
		}




		bool WASDRAW = false;


		//    Color colorWhite = new Color32(211, 211, 211, 255);
		void MAIN( ExternalDrawContainer controller )
		{
			DRAW_GRID( controller );


			/*   /*  if (ScrollWhell!= 0)#1# {
					   MonoBehaviour.print(ScrollWhell);
					  }*/
			////////////////////////////////////////////////////////
			////////////////// MAIN ////////////////////////////////
			////////////////////////////////////////////////////

			//GUI.Label(new Rect(0,0,20,20),autorefreshContent);



			DRAWOBJECT( controller );


			var ACTIVE_RECT = RECT;
			ACTIVE_RECT.height -= INTERFACE_SIZE;
			ACTIVE_RECT.y += INTERFACE_SIZE;


			var titleRect = ACTIVE_RECT;
			titleRect.height = FONT_SIZE_INTERFACE() + 1;


			if ( adapter.par_e.HYPERGRAPH_DRAWBOLD_LABEL )
				if ( CurrentSelection ) DRAW_BOLD_LABEL( titleRect, "HyperGraph: '" + CurrentSelection.ToString() + "'", FONT_SIZE_INTERFACE() - 2 );
				else DRAW_BOLD_LABEL( titleRect, "HyperGraph", FONT_SIZE_INTERFACE() );


			////////////////////////////////////////////////////////
			////////////////// MAIN ////////////////////////////////
			/////////////////////////////////////////////////////////

			CHECK_DRAG( controller );

			if ( Event.current.type == EventType.Repaint )
			{
				var shRect = RECT;
				/*  shRect.height -= INTERFACE_SIZE;
							  shRect.y += INTERFACE_SIZE;*/
				shRect.height = Math.Max( shRect.height, shadow.border.bottom * 2 );
				shadow.Draw( shRect, false, false, false, false );
			}
		}



		/** DRAW_ZOOMER **/
		void DRAW_ZOOMER( Rect ACTIVE_RECT, int EVENT_ID, ExternalDrawContainer controller )
		{

			var position = ACTIVE_RECT;
			position.width = 40;
			position.height = 14;
			position.x = RECT.width - position.width;
			position.y = RECT.height - 2 - position.height;
			EditorGUIUtility.AddCursorRect( position, MouseCursor.Link );

			EVENT_ID++;
			HyperGraphZoomReset.text = Mathf.RoundToInt( CURRENT_SCALE * 100 ).ToString() + "%";
			DRAW_BOLD_LABEL( position, HyperGraphZoomReset, FONT_SIZE() ); // TOOLTIP

			if ( position.Contains( Event.current.mousePosition ) && Event.current.button == 0 &&
				Event.current.type == EventType.MouseDown )
			{
				Tools.EventUse();
				actions.ADD_ACTION( EVENT_ID, position, contains => { return false; }, contains => {
					if ( contains )
					{
						SET_SCALE( 1 );
					}
				}, controller );
			}

			if ( actions.HOVER( EVENT_ID, position, controller ) )
				HIPERUI_BUTTONGLOW.Draw( position, false, false, false, false );

			position.width = position.height;
			position.x -= position.width + 2;
			EditorGUIUtility.AddCursorRect( position, MouseCursor.Link );


			/** BUTTON **/
			EVENT_ID++;
			Label( position, HyperGraphZoomPlus ); // TOOLTIP

			if ( position.Contains( Event.current.mousePosition ) && Event.current.button == 0 &&
				Event.current.type == EventType.MouseDown )
			{
				Tools.EventUse();
				actions.ADD_ACTION( EVENT_ID, position, contains => { return false; }, contains => {
					if ( contains )
					{
						SET_SCALE( Mathf.CeilToInt( CURRENT_SCALE * 4 + 0.001f ) / 4f );
					}
				}, controller );
			}

			GUI.DrawTexture( position, ZOOM_PLUS ); // TESTURE

			if ( actions.HOVER( EVENT_ID, position, controller ) )
				HIPERUI_BUTTONGLOW.Draw( position, false, false, false, false );

			/** **/


			position.width = 70;
			position.x -= position.width + 2;

			EVENT_ID++;
			// var news = GUI.HorizontalSlider(position, par.HiperGraphParams.SCALE, 0.5f, 3);
			Label( position, HyperGraphZoomPlus ); // TOOLTIP
			var news = HorizontalSlider(ref EVENT_ID, position, CURRENT_SCALE, 0.5f, 2, controller);

			if ( news != CURRENT_SCALE )
			{
				SET_SCALE( news );
			}


			position.width = position.height;
			position.x -= position.width + 2;
			EditorGUIUtility.AddCursorRect( position, MouseCursor.Link );


			/** BUTTON **/
			EVENT_ID++;
			Label( position, HyperGraphZoomPlus ); // TOOLTIP

			if ( position.Contains( Event.current.mousePosition ) && Event.current.button == 0 &&
				Event.current.type == EventType.MouseDown )
			{
				Tools.EventUse();
				actions.ADD_ACTION( EVENT_ID, position, contains => { return false; }, contains => {
					if ( contains )
						if ( contains )
						{
							SET_SCALE( Mathf.FloorToInt( CURRENT_SCALE * 4 - 0.001f ) / 4f );
							// par.HiperGraphParams.SCALE -= 0.25f;
						}
				}, controller );
			}

			GUI.DrawTexture( position, ZOOM_MINUS ); // TESTURE

			if ( actions.HOVER( EVENT_ID, position, controller ) )
				HIPERUI_BUTTONGLOW.Draw( position, false, false, false, false );

			/** **/
		}

		/** DRAW_ZOOMER **/
		void SET_SCALE( float _newScale ) //  var oldH = par.HiperGraphParams.SCALE;
		{
			var olds = 0f;
			var dif = 0f;

			olds = H_SCALE;
			H_SCALE = _newScale;
			dif = H_SCALE / olds;

			CURRENT_CONTROLLER.scrollPos.x =
				(CURRENT_CONTROLLER.scrollPos.x -
				 CURRENT_CONTROLLER.WIDTH( IExternalWindowType.HYPER_GRAPH ) / 2) * dif +
				CURRENT_CONTROLLER.WIDTH( IExternalWindowType.HYPER_GRAPH ) / 2;
			CURRENT_CONTROLLER.scrollPos.y =
					(CURRENT_CONTROLLER.scrollPos.y -
					 CURRENT_CONTROLLER.HEIGHT( IExternalWindowType.HYPER_GRAPH ) / 2 ) * dif +
					CURRENT_CONTROLLER.HEIGHT( IExternalWindowType.HYPER_GRAPH ) / 2;


			RepaintNow();
		}

		Dictionary<int, float?> HorizontalSlider_helper = new Dictionary<int, float?>();

		float HorizontalSlider( ref int EVENT_ID, Rect position, float value, float left, float right,
			ExternalDrawContainer controller )
		{
			var helperID = EVENT_ID;

			if ( !HorizontalSlider_helper.ContainsKey( helperID ) ) HorizontalSlider_helper.Add( helperID, null );

			var thumbRect = position;
			thumbRect.width = 10;


			var workL = position.width - thumbRect.width;
			thumbRect.x = position.x + workL * (value - left) / (right - left);
			//thumbRect.y += 1;
			thumbRect.height -= 1;

			position.y += position.height / 4;
			position.height = position.height / 2 - 1;

			EVENT_ID++;

			if ( position.Contains( Event.current.mousePosition ) &&
				!thumbRect.Contains( Event.current.mousePosition ) && Event.current.button == 0 &&
				Event.current.type == EventType.MouseDown )
			{
				Tools.EventUse();
				var screenLeft = EditorGUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
				var screenRight = EditorGUIUtility.GUIToScreenPoint(new Vector2(position.x + position.width,
					position.y + position.height));
				var oldMouse = -1f;
				actions.ADD_ACTION( EVENT_ID, position, contains => {
					if ( oldMouse != CALC_MOUSE( screenLeft, screenRight ) )
					{
						oldMouse = CALC_MOUSE( screenLeft, screenRight );
						HorizontalSlider_helper[ helperID ] = (right - left) * oldMouse + left;
						RepaintNow();
					}

					return true;
				}, contains => { }, controller );
				RepaintNow();
			}

			GUI.DrawTexture( position, adapter.BoxTexture() ); // TESTURE

			if ( actions.HOVER( EVENT_ID, position, controller ) )
				HIPERUI_BUTTONGLOW.Draw( position, false, false, false, false );

			EditorGUIUtility.AddCursorRect( position, MouseCursor.Link );


			EVENT_ID++;

			if ( thumbRect.Contains( Event.current.mousePosition ) && Event.current.button == 0 &&
				Event.current.type == EventType.MouseDown )
			{
				Tools.EventUse();
				var screenLeft = EditorGUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
				var screenRight = EditorGUIUtility.GUIToScreenPoint(
					new Vector2(position.x + position.width - thumbRect.width, position.y + position.height));
				// var offset = Event.current.mousePosition - thumbRect.position;
				var oldMouse = CALC_MOUSE(screenLeft, screenRight);
				var off = CALC_MOUSE(screenLeft, screenRight, thumbRect.position) - oldMouse;
				actions.ADD_ACTION( EVENT_ID, thumbRect, contains => {
					if ( oldMouse != CALC_MOUSE( screenLeft, screenRight ) ) // var old = oldMouse;
					{
						oldMouse = CALC_MOUSE( screenLeft, screenRight );
						// sum -= old - oldMouse;
						HorizontalSlider_helper[ helperID ] = (right - left) * (oldMouse + off) + left;
						RepaintNow();
					}

					return true;
				}, contains => { }, controller );
			}

			GUI.DrawTexture( thumbRect, ZOOM_THUMB ); // TESTURE

			if ( actions.HOVER( EVENT_ID, thumbRect, controller ) )
				HIPERUI_BUTTONGLOW.Draw( thumbRect, false, false, false, false );

			EditorGUIUtility.AddCursorRect( position, MouseCursor.Link );


			/* var result = Mathf.RoundToInt(thumbRect.x - position.x) * (right - left) / workL + left;
					 return Mathf.Clamp(result, left, right);*/
			var result = HorizontalSlider_helper[helperID] ?? value;
			HorizontalSlider_helper[ helperID ] = null;
			return Mathf.Clamp( result, left, right );
		}


		float CALC_MOUSE( Vector2 screenLeft, Vector2 screenRight, Vector2? offset = null )
		{
			var full = screenRight.x - screenLeft.x;
			var m = EditorGUIUtility.GUIToScreenPoint((offset ?? Event.current.mousePosition)).x - screenLeft.x;
			return m / full;
		}



		private void CHECK_RIGHTCLICK( Rect rrrr ) // GUI.Label(rrrr, "");
		{
			if ( rrrr.Contains( Event.current.mousePosition ) && Event.current.button == 1 &&
				(Event.current.type == EventType.MouseDown || Event.current.type == EventType.Used) ) // MonoBehaviour.print("asd");
			{
				Tools.EventUse();
				/*  RepaintWindow();


							var menu = new GenericMenu();

							menu.ShowAsContext();
							Tools.EventUse();*/
			}
		}

		private void CHECK_DRAG( ExternalDrawContainer controller )
		{
			var EVENT_ID = 99;

			if ( RECT.Contains( Event.current.mousePosition ) && Event.current.button == 2 &&
				Event.current.type == EventType.MouseDown )
			{
				Tools.EventUse();
				var startPos = EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition);
				var startScroll = controller.scrollPos;
				actions.ADD_ACTION( EVENT_ID, null, contains => {
					var p = EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition);
					var result = (startPos.x == p.x) && startPos.y == p.y;

					if ( !result && controller.MAIN ) adapter.RepaintWindowInUpdate( 0 );

					controller.scrollPos.x = startScroll.x - (startPos.x - p.x);
					controller.scrollPos.y = startScroll.y - (startPos.y - p.y);
					return result;
				}, contains => { }, controller );
			}

			if ( actions.HOVER( EVENT_ID, null, controller ) )
			{
				var m = Event.current.mousePosition;
				EditorGUIUtility.AddCursorRect( new Rect( m.x - 100, m.y - 100, 200, 200 ), MouseCursor.Pan );
			}
		}


		//   Color BUTBLUECOLOR = new Color(1, 1, 1, 0.15f);
		Color BUTBLUECOLOR = new Color(0, 0, 0, 0.4f);

		Rect R = new Rect();

		void DRAW_GRID( ExternalDrawContainer controller )
		{
			if ( Event.current.type != EventType.Repaint ) return;

			var startX = Mathf.FloorToInt(-controller.scrollPos.x / GR_SIZE) * GR_SIZE;
			var startY = Mathf.FloorToInt(-controller.scrollPos.y / GR_SIZE) * GR_SIZE;
			R.width = GR_SIZE;
			R.height = GR_SIZE;
			var T = GRID;
			if ( !T ) throw new Exception( "GRID" );
			for ( float y = startY, ly = startY + GR_SIZE + RECT.height; y < ly; y += GR_SIZE )
			{
				for ( float x = startX, lx = startX + GR_SIZE + RECT.width; x < lx; x += GR_SIZE )
				{
					R.x = x + controller.scrollPos.x;
					R.y = y + controller.scrollPos.y;
					GUI.DrawTexture( R, T );
				}
			}

			/*	R.width = DEFAULTWIDTH(controller) * CURRENT_SCALE;
				R.x = -DEFAULTWIDTH(controller) / 2 * CURRENT_SCALE + controller.scrollPos.x;
				R.y = 0;
				R.height = controller.HEIGHT;
				var asd = GUI.color;
				GUI.color *= BUTBLUECOLOR;

				if (EditorGUIUtility.isProSkin)  // GUI.DrawTexture(R, adapter.GetIcon("BUTBLUE"));

				GUI.color = asd;*/
		}


		/* adapter.par.HiperGraphParams.HEIGHT = Mathf.Clamp(adapter.par.HiperGraphParams.HEIGHT, 20,
												   Math.Max( 20, adapter.window().position.height* 0.5f ) );*/
		//internal  static void CHECK_HEIGHT()
		//{
		//
		//	adapter.par.HiperGraphParams.HEIGHT = Mathf.Clamp( adapter.par.HiperGraphParams.HEIGHT, 20,
		//										  Math.Max( 20, (adapter.window().position.height + bottomInterface.HEIGHT) / 2 ) );
		//}


	}
}