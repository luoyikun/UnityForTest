using System;
using UnityEngine;
using UnityEditor;



namespace EMX.HierarchyPlugin.Editor.Mods.HyperGraph
{
	partial class HyperGraphModInstance
	{





		internal void CHANGEPLAYMODE()
		{
			StoptBroadcasting();
			StartBroadcasting();
			SMOOTH_SCENE_CHANGE_ORBREAK();
		}


		Vector2 pp;
		internal void ON_SCROLL( float sc )
		{
			//if (type == ScrollType.HyperGraphScroll)
			{
				var rr = CURRENT_CONTROLLER.RECT( IExternalWindowType.HYPER_GRAPH );
				if ( rr.HasValue )
				{
					var _mid = new Vector2(CURRENT_CONTROLLER.WIDTH( IExternalWindowType.HYPER_GRAPH ) / 2,CURRENT_CONTROLLER.HEIGHT( IExternalWindowType.HYPER_GRAPH ) / 2);
					var mid = (_mid);
					pp = (Event.current.mousePosition - new Vector2( rr.Value.x, rr.Value.y ));
					pp = pp - mid;

					CURRENT_CONTROLLER.scrollPos.x -= pp.x;
					CURRENT_CONTROLLER.scrollPos.y -= pp.y;

				}

				//EditorGUIUtility.GUIToScreenPoint




				if ( sc < 0 ) SET_SCALE( Mathf.CeilToInt( H_SCALE * 10 + 0.001f ) / 10f );
				else SET_SCALE( Mathf.FloorToInt( H_SCALE * 10 - 0.001f ) / 10f );


				if ( rr.HasValue )
				{
					CURRENT_CONTROLLER.scrollPos.x += pp.x;
					CURRENT_CONTROLLER.scrollPos.y += pp.y;
				}


				Tools.EventUse();
			}
			/*
						else if (type == ScrollType.HyperGraphScroll_Window)
						{
							if (sc < 0) SET_SCALE(ScrollType.HyperGraphScroll_Window, Mathf.CeilToInt(adapter.par.HiperGraphParams.WINDIOW_SCALE * 10 + 0.001f) / 10f);
							else SET_SCALE(ScrollType.HyperGraphScroll_Window, Mathf.FloorToInt(adapter.par.HiperGraphParams.WINDIOW_SCALE * 10 - 0.001f) / 10f);

							EventUse();
						}*/
		}


		/*	void OnDestroy()
			{

			}*/


		// internal void SubscribeEditorInstance( EditorSubscriber sbs )
		// {
		//     sbs.OnSceneOpening += SMOOTH_SCENE_CHANGE_ORBREAK;
		//     sbs.OnSelectionChanged += CHANGE_SELECTION;
		//     sbs.OnPlayModeStateChanged += CHANGEPLAYMODE;
		//     sbs.OnUpdate += Update;
		// }
		internal void SubscribeEditorInstance( AdditionalSubscriber sbs )
		{
			shouldInitScroll = true;
			sbs.OnSceneOpening += SMOOTH_SCENE_CHANGE_ORBREAK;
			sbs.OnSelectionChanged += CHANGE_SELECTION;
			sbs.OnPlayModeStateChanged += CHANGEPLAYMODE;
			sbs.OnUpdate += Update;
		}

		internal void SMOOTH_SCENE_CHANGE_ORBREAK()
		{
			//  Debug.Log( CurrentSelection );
			if ( CurrentSelection == Selection.activeObject ) return;
			SCENE_CHANGE();
		}

		internal void SCENE_CHANGE()
		{
			CURRENT_CONTROLLER.wasInit = false;
			CHANGE_SELECTION_OVVERIDE( true );
			CurrentSelection = null;
			StoptBroadcasting();
			RepaintNow();
		}

		internal void CHANGE_SELECTION()
		{
			if ( !H_AUTOCHANGE ) return;
			CHANGE_SELECTION_OVVERIDE();
			RepaintNow();
		}


		bool OBJECT_ISVALID( UnityEngine.Object o )
		{
			//if (adapter.IS_HIERARCHY()) 
			return o is GameObject && ((GameObject)o).scene.IsValid() && ((GameObject)o).scene.isLoaded;
			//else return o && !string.IsNullOrEmpty(adapter.bottomInterface.INSTANCEID_TOGUID(o.GetInstanceID()));
		}


		public void CHANGE_SELECTION_OVVERIDE( bool skipAutoHide = false, UnityEngine.Object selection = null )
		{
#if UNITY_EDITOR
			// MonoBehaviour.print("CHANGE_SELECTION_OVVERIDE");
#endif
			if ( CURRENT_CONTROLLER == null ) return;
			WASDRAW = false;
			CURRENT_CONTROLLER.wasInit = false;
			//adapter.bottomInterface.hyperGraph.WindowHyperController.wasInit = false;
			var active = selection ?? Selection.activeObject;
			UnityEngine.Object newSelection = null;


			if ( active && OBJECT_ISVALID( active ) )
			{
				newSelection = active;
			}

			if ( !skipAutoHide && H_AUTOHIDE && CurrentSelection != newSelection )
			{
				// Debug.Log( "ASD" );
				//SWITCH_ACTIVE_SCAN( false );
				CURRENT_WIN.Close();
				return;
			}

			if ( CurrentSelection != newSelection )
			{
				if ( newSelection )
				{
					comps = null;
					CurrentSelection = newSelection;

					if ( !skipUndo ) SETUNDO();

					skipUndo = false;
					StoptBroadcasting();
					StartBroadcasting();
				}

				else //StoptBroadcasting();
				{ }
			}
			RepaintNow();
		}







		const float h_offset = -20;
		bool shouldInitScroll = true;

		private void INITIALIZE( ExternalDrawContainer controller, float w )
		{
			if ( controller.wasInit ) return;

			// currentController = controller;




			//	adapter.OnScroll -= ON_SCROLL;
			//	adapter.OnScroll += ON_SCROLL;


			//var v1 = controller.wasInit;
			//	var v2 = adapter.bottomInterface.hyperGraph.WindowHyperController.wasInit;

			if ( CurrentSelection == null ) CHANGE_SELECTION_OVVERIDE( true );

			//controller.wasInit = v1;
			//adapter.bottomInterface.hyperGraph.WindowHyperController.wasInit = v2;
			controller.wasInit = true;
			var sh = shouldInitScroll;
			if ( adapter.par_e.HYPERGRAPH_RESET_SCROLL_ONRELOAD ) sh = true;


			if ( sh ) controller.scrollPos.x = w / 2;
			//controller.scrollPos.x = RECT.width / 2;
			var h = DRAWOBJECT(controller, true) ;

			if ( sh ) controller.scrollPos.y = Math.Max( (controller.HEIGHT( IExternalWindowType.HYPER_GRAPH )) / 2 - h / 2, 10 ); //- 25

			shouldInitScroll = false;
		}

		//void RESET_SCROLL()
		//{
		//
		//}


		private void REFRESH()
		{
			CurrentSelection = null;
			CHANGE_SELECTION_OVVERIDE( true );
			RepaintNow();
		}

		internal void RefreshWithCurrentSelection()
		{
			comps = null;
			var temp = CurrentSelection;
			if ( UndoPos >= 0 && UndoPos < UndoList.Count ) temp = UndoList[ UndoPos ];

			CurrentSelection = null;
			CHANGE_SELECTION_OVVERIDE( true, temp );
			RepaintNow();
		}

	}
}
