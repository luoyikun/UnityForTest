using System;
using UnityEngine;
using UnityEditor;
using EMX.HierarchyPlugin.Editor.Events;

namespace EMX.HierarchyPlugin.Editor.Mods
{

	internal class IExternalWindowType
	{
		internal const int HYPER_GRAPH = 10;
		internal const int CUSTOM = 20;
		internal const int LAST = 30;
		internal const int SCENE = 40;
		internal const int EXPAND = 50;
		internal const int PROJECT_FOLD = 60;
	}
	internal interface IExternalWindow
	{
		bool FORCE_REPAINT_THROUGH_LAYOUT { get; set; }
		GUIContent titleContent { get; set; }
		Rect position( int index );

		bool PUSH_EVENT_HELPER_RAW( MouseRawUp.WantMouseLeaveType t );
		void PUSH_ONMOUSEUP( Func<Events.MouseRawUp.WantMouseLeaveType, bool> a );
		void RepaintNow();
		bool OvValide();
		void Close();

		EditorWindow GET_WIN();
	}

	public abstract class ExternalModRoot : EditorWindow, IExternalWindow
	{

		internal static bool EQUALS( IExternalWindow w1, IExternalWindow w2 )
		{
			if ( ReferenceEquals( w1, null ) && ReferenceEquals( w2, null ) ) return true;
			if ( ReferenceEquals( w1, null ) || ReferenceEquals( w2, null ) ) return false;
			return w1.GET_WIN() == w2.GET_WIN();
		}

		Rect p;
		new public Rect position( int ind )
		{
			p = base.position;
			p.x = p.y = 0;
			return p;
		}
		public bool OvValide()
		{
			return Alive();
		}
		public EditorWindow GET_WIN() { return this; }

		internal abstract void SubscribeEditorInstance( AdditionalSubscriber sbs );

		//internal ExternalWindowRoot targetWindow = null;
		internal virtual bool Alive()
		{
			return currentWindow;
		}

		public void RepaintNow()
		{

			if ( currentWindow ) currentWindow.Repaint();
		}

		internal ExternalDrawContainer _wndowController;
		internal ExternalDrawContainer controller { get { return _wndowController ?? (_wndowController = new ExternalDrawContainer( pluginID )); } }

		internal PluginInstance adapter { get { return Root.p[ 0 ]; } }
		Events.MouseRawUp _mouse_uo_helper;
		internal Events.MouseRawUp mouse_uo_helper { get { return _mouse_uo_helper ?? (_mouse_uo_helper = new Events.MouseRawUp()); } }
		internal EditorWindow currentWindow;

		abstract internal int pluginID { get; }

		Vector2 size = new Vector2(300, 200);

		bool wasInit = false;


		internal void Init( bool skipPos = false )
		{
			if ( wasInit ) return;
			wasInit = true;

			if ( !skipPos && base.position.x < 10 && base.position.y < 10 )
			{
				base.position = new Rect( WinBounds.MAX_WINDOW_WIDTH.x + WinBounds.MAX_WINDOW_WIDTH.y / 2 - size.x / 2, WinBounds.MAX_WINDOW_HEIGHT.x + WinBounds.MAX_WINDOW_HEIGHT.y / 2 - size.y / 2, size.x, size.y );
			}
			//base.Init();
		}

		public bool PUSH_EVENT_HELPER_RAW( Events.MouseRawUp.WantMouseLeaveType t )
		{
			return ON_GUI_POST_PUSH_EVENT_HELPER_RAW( controller, this, mouse_uo_helper, t );
		}

		public void PUSH_ONMOUSEUP( Func<Events.MouseRawUp.WantMouseLeaveType, bool> ac )
		{
			ON_GUI_POST_PUSH_PUSH_ONMOUSEUP( mouse_uo_helper, this, ref ac );
		}


		static Type _InspectorType;
		internal static Type InspectorType {
			get { if ( _InspectorType == null ) _InspectorType = typeof( EditorWindow ).Assembly.GetType( "UnityEditor.InspectorWindow" ); return _InspectorType; }
		}


		internal bool WAS_INIT = false;

		bool _FORCE_REPAINT_THROUGH_LAYOUT = true;
		public bool FORCE_REPAINT_THROUGH_LAYOUT { get { return _FORCE_REPAINT_THROUGH_LAYOUT; } set { _FORCE_REPAINT_THROUGH_LAYOUT = value; } }

		public void OnGUI()
		{

			if ( Root.TemperaryPluginDisabled || Root.TemperaryPluginDisabled_ForWindows > 3 ) return;

			if ( Event.current.type == EventType.Layout && !FORCE_REPAINT_THROUGH_LAYOUT ) return;
			controller.tempRoot = this;
			if ( !OnGUI_Check() ) return;

			try
			{
				OnGUI_Draw();

				OnGUI_Post();
			}
			catch ( Exception ex )
			{
				Root.TemperaryDisableThePlugin_FromWindow( ex );
			}


			if ( FORCE_REPAINT_THROUGH_LAYOUT && Event.current.type == EventType.Layout ) FORCE_REPAINT_THROUGH_LAYOUT = false;
		}

		internal abstract void OnGUI_Draw();

		internal bool OnGUI_Check()
		{


			WAS_INIT = false;
			if ( !currentWindow )
			{
				currentWindow = this;
				if ( !currentWindow ) return false;
				WAS_INIT = true;
			}


			if ( !adapter.par_e.ENABLE_ALL )
			{
				GUI.Label( new Rect( 0, 0, position( -1 ).width, position( -1 ).height ), "" + Root.HierarchyPro + " disabled!", adapter.STYLE_LABEL_10_middle );
				return false;
			}




			return true;
		}

		internal void OnGUI_Post()
		{
			ON_GUI_POST_UPCONTROLLER_CHECKER( controller, this, mouse_uo_helper );
		}




		internal static void ON_GUI_POST_PUSH_PUSH_ONMOUSEUP( Events.MouseRawUp mouse_uo_helper, IExternalWindow w, ref Func<Events.MouseRawUp.WantMouseLeaveType, bool> ac )
		{
			mouse_uo_helper.PUSH_ONMOUSEUP( ac, w.GET_WIN() );
		}

		internal static bool ON_GUI_POST_PUSH_EVENT_HELPER_RAW( ExternalDrawContainer controller, IExternalWindow w, Events.MouseRawUp mouse_uo_helper,
			Events.MouseRawUp.WantMouseLeaveType t )
		{
			controller.CHECK_MOUSE_UP();
			controller.EVENT_MOUSE_UP();
			var repaint = false;

			/*			if (controlIDsAndOnMouseUp.Count != 0)
						{
							foreach (var controlID in controlIDsAndOnMouseUp.Values.ToArray())
							{
								controlID();
							}

							controlIDsAndOnMouseUp.Clear();
							repaint = true;
						}*/


			if ( repaint )
			{
				w.RepaintNow();
			}
			return true;
		}


		internal static void ON_GUI_POST_UPCONTROLLER_CHECKER( ExternalDrawContainer controller, IExternalWindow w, Events.MouseRawUp mouse_uo_helper )
		{
			if ( (Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout) )
			{
				controller.EVENT_UPDATE();
			}

			if ( Event.current.rawType == EventType.MouseUp )
			{
				controller.EVENT_MOUSE_UP();
			}



			if ( controller.currentAction != null )
			{
				if ( !controller.currentAction( false, Root.p[ 0 ].deltaTime ) )
					// Repaint();
					w.RepaintNow(); //controller.pluginID
			}


			if ( Event.current.rawType == EventType.MouseUp )
			{
				controller.CHECK_MOUSE_UP();
			}



			if ( mouse_uo_helper.Invoke() )
			{
				w.RepaintNow();
				if ( Event.current.isMouse ) Event.current.Use();
			}


		}


		internal virtual void OnEnable()
		{
			Root.SET_EXTERNAl_MOD( this );

		}
		internal virtual void OnDisable()
		{
			Root.REMOVE_EXTERNAl_MOD( this );
		}


	}
}
