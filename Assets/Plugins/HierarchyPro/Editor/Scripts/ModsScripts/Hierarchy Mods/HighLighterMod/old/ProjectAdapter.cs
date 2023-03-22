using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace EMX.HierarchyPlugin.Editor.Mods
{
	internal partial class HighlighterModForProject
	{

		PluginInstance adapter { get { return Root.p[ 0 ]; } }


		internal Mods.HighlighterMod highLighterMod;
		internal Mods.BackgroundDecorations backgroundDecorations;

		//HighlighterMod projecthighlighterMod;
		//PluginInstance plug = null;



		internal void Subscribe( EditorSubscriber sbs )
		{

			highLighterMod = null;
			backgroundDecorations = null;
			var sbs2 = new EditorSubscriber(Root.p[1]);
			var has = false;

			if ( adapter.par_e.USE_PROJECT_MANUAL_HIGHLIGHTER_MOD || adapter.par_e.USE_PROJECT_AUTO_HIGHLIGHTER_MOD )
			{
				highLighterMod = new HighlighterMod( adapter, 1 );
				highLighterMod._s( sbs2, adapter.par_e.PROJ_WIN_SET, adapter.par_e.PROJ_HIGH_SET );
				//_s(sbs, adapter.par_e.HIER_WIN_SET, adapter.par_e.HIER_HIGH_SET);

				sbs2.OnUndoAction = null;
				sbs2.OnSceneOpening = null;

				sbs.BuildedOnGUI_last += release_gl;

				has = true;
			}

			if ( adapter.par_e.PROJ_WIN_SET.USE_BACKGROUNDDECORATIONS_MOD && adapter.par_e.USE_PROJECT_SETTINGS )
			{
				//Debug.Log("ASD");
				backgroundDecorations = new BackgroundDecorations( 1 );
				backgroundDecorations.SubscribeFirst( sbs2 );// background colors mod //INVOKE FIRSTGUI ONLY AFTER RIGHT MODS CALCULATION
				backgroundDecorations.SubscribeLast( sbs2 );// background colors mod //INVOKE FIRSTGUI ONLY AFTER RIGHT MODS CALCULATION
				has = true;
			}

			// projecthighlighterMod = new HighlighterMod(adapter);

			// plug = new PluginInstance(0);
			//  plug.
			if ( has )
			{
				sbs.OnProjectWindow += OnProjectWindow;
				sbs2.copy_to( sbs );

				adapter.g[ 1 ].OnAssignWindowFirstFrame = sbs2.OnAssignWindowFirstFrame;
				adapter.g[ 1 ].BuildedOnGUI_first = sbs2.BuildedOnGUI_first;
				adapter.g[ 1 ].BuildedOnGUI_last = sbs2.BuildedOnGUI_last;
				adapter.g[ 1 ].BuildedOnGUI_last_butBeforeGL = sbs2.BuildedOnGUI_last_butBeforeGL;
				adapter.g[ 1 ].BuildedOnGUI_middle = sbs2.BuildedOnGUI_middle;
				//adapter.g[1].BuildedOnGUI_middle_plusLayout = sbs2.BuildedOnGUI_middle_plusLayout;
			}

			//var a = new HighlighterAdapter()
			//{
			//    USE_AUTO_HIGHLIGHTER_MOD = () => adapter.par_e.USE_PROJECT_AUTO_HIGHLIGHTER_MOD,
			//    USE_MANUAL_HIGHLIGHTER_MOD = () => adapter.par_e.USE_PROJECT_MANUAL_HIGHLIGHTER_MOD,
			//    USE_CUSTOM_PRESETS_MOD = () => false,
			//};




			// projecthighlighterMod._s(sbs, adapter.par_e.PROJ_WIN_SET, adapter.par_e.PROJ_HIGH_SET);


		}

		internal void RESET_DRAW_STACKS()
		{
			if ( highLighterMod != null ) highLighterMod.ResetStack();
		}

		void release_gl()
		{
			//gl.ReleaseStack();
		}




		EventType? oldEvent;
		// Dictionary<EditorWindow, object> tree_cache = new Dictionary<EditorWindow, object>();
		private void OnProjectWindow( string arg1, Rect rect )
		{
			if ( oldEvent.HasValue && oldEvent == Event.current.type ) return;
			adapter.pluginID = 1;
			oldEvent = Event.current.type;
			if ( !INIT_TREE() ) oldEvent = null;
			adapter.pluginID = 0;
		}

		PropertyInfo onGUIRowCallback;
		Window p_w = new Window();
		bool INIT_TREE()
		{
			if ( onGUIRowCallback == null )
			{
				onGUIRowCallback = adapter.m_TreeViewFieldInfo.FieldType.GetProperty( "onGUIRowCallback", (BindingFlags)(-1) ) ?? throw new Exception( "Project onGUIRowCallback" );
				if ( onGUIRowCallback == null ) return false;
			}

			Window.AssignInstance( 1, ref p_w, adapter.ProjectBrowserWindowType );
			var w = p_w.Instance;
			if ( !w ) return false;
			//  var w = window();
			//  if (w == null)
			//  {
			//     return false;
			//  }
			/* if (m_TreeViewFieldInfo == null)
             {
                 return false;
             }*/

			/* if (pluginID == Initializator.PROJECT_ID)
             {
                 if (!wasInitWindowsField.ContainsKey(w.GetInstanceID()) || wasInitWindowsField[w.GetInstanceID()] != m_TreeViewFieldInfo)
                 {
                     wasInitWindowsField.Remove(w.GetInstanceID());
                     wasInitWindowsField.Add(w.GetInstanceID(), m_TreeViewFieldInfo);
                     tree_cache.Clear();
                     bottomInterface.NEED_READ_LIST.Clear();
                 }
             }*/

			//	var tree = adapter.m_TreeViewFieldInfoForProject(w).GetValue(w);
			var tree = adapter.GetTreeViewontroller(1,w);// m_TreeView(w);
			if ( !wasInitWindows.ContainsKey( w.GetInstanceID() ) )
			{
				wasInitWindows.Add( w.GetInstanceID(), tree );
				INIT_TREE( w.GetInstanceID() );
			}
			if ( wasInitWindows[ w.GetInstanceID() ] != tree )
			{
				wasInitWindows[ w.GetInstanceID() ] = tree;
				INIT_TREE( w.GetInstanceID() );
			}
			return true;
		}
		Dictionary<int, object> wasInitWindows = new Dictionary<int, object>();
		Dictionary<int, FieldInfo> wasInitWindowsField = new Dictionary<int, FieldInfo>();

		Dictionary<object, bool> wasassigned = new Dictionary<object, bool>();
		void INIT_TREE( int wID )
		{
			var tree = wasInitWindows[wID];

			if ( tree == null ) return;

			if ( wasassigned.ContainsKey( tree ) ) return;


			wasassigned.Add( tree, true );
			var eventInfo = (Action<int, Rect>)onGUIRowCallback.GetValue(tree, null);
			eventInfo -= proj_Additional;
			eventInfo += proj_Additional;
			onGUIRowCallback.SetValue( tree, eventInfo, null );

			adapter.RepaintWindowInUpdate( 1 );
		}


		//   MethodInfo GetInstanceIDFromGUID;
		static void proj_Additional( int id, Rect rect )
		{

			if ( !Root.SKIP_TEMP_DIS )
			{
				if ( Root.TemperaryPluginDisabled ) return;

				Root.p[ 0 ].pluginID = 1;
				try
				{
					Root.p[ 0 ].gui( id, ref rect );
				}
				catch ( Exception ex )
				{
					Root.TemperaryDisableThePlugin_FromRoot( ex );
				}
				Root.p[ 0 ].pluginID = 0;
			}
			else
			{
				Root.p[ 0 ].pluginID = 1;
				Root.p[ 0 ].gui( id, ref rect );
				Root.p[ 0 ].pluginID = 0;
			}



			//adapter.modsController.highLighterMod.adapter.pluginID = 1;
			// if (Event.current.type != EventType.Repaint) return;

			//if (GetInstanceIDFromGUID == null) GetInstanceIDFromGUID = typeof(AssetDatabase).GetMethod("GetInstanceIDFromGUID", (BindingFlags)(-1))??throw new Exception();


			// if (p.par_e.DRAW_EXTENSION_IN_PROJECT)
			{

			}
		}


	}
}
