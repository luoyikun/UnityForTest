using System.Linq;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor.Mods.HyperGraph
{



	// internal  Action OnGUIV;
	/*	HyperGraphWindow hyperwindow
		{
			get { return hyperGraph.editorWindow as HyperGraphModWindow; }
			set { hyperGraph.editorWindow = value; }

		}*/

	/*internal static void ShowW(Adapter adapter)
{
	foreach (var w in Resources.FindObjectsOfTypeAll<_6__BottomWindow_HyperGraphWindow>().Where(w => w.adapter == adapter)) w.Close();
	var wd = EditorWindow.GetWindow<_6__BottomWindow_HyperGraphWindow>("HyperGraph - " + adapter.pluginname);
	wd.adapter = adapter;
	hyperGraph.editorWindow = wd;

	wd.ShowAuxWindow();
	wd.wasInit = false;
}*/

	public partial class HyperGraphModWindow : ExternalModRoot, IHasCustomMenu
	{


		internal override int pluginID { get { return 0; } }
		HyperGraphModInstance __instance;
		internal HyperGraphModInstance instance { get { return __instance ?? (__instance = new HyperGraphModInstance( () => controller )); } }
		internal const string NAME = "HyperGraph";
		const int priority = 0;




		internal static void SubscribeEditorInstanceStatic( EditorSubscriber sbs )
		{
			if ( !Root.p[ 0 ].par_e.USE_HYPERGRAPH_MOD )
			{
				foreach ( var item in Root.p[ 0 ].modsController.activeExternalMods.ToList() )
					if ( item && item is HyperGraphModWindow ) item.Close();
				return;
			}

			//static Type[] lastTypes;

			//if ( !adapter.par_e.USE_HYPERGRAPH_MOD ) return;

			sbs.ExternalMod_Buttons.Add( new ExternalMod_Button( typeof( HyperGraphModWindow ) ) {
				text = NAME,
				icon = () => "HYPER_ICON",
				enabled = Root.p[ 0 ].par_e.DRAW_TOPBAR_H1,
				priority = Root.p[ 0 ].par_e.ORDER_TOPBAR_H1,
				release = ICON_CLICK,
				menuGen = MENU_GEN
			} );

			sbs.ExternalMod_MenuItems.Add( new ExternalMod_MenuItem() {
				text = NAME,
				path = "- Open " + NAME.ToLower() + " in the windows",
				priority = priority,
				release = ICON_CLICK,
			} );
			//sbs.ExternalMod_MenuItems
		}
		internal override void SubscribeEditorInstance( AdditionalSubscriber sbs )
		{
			instance.SubscribeEditorInstance( sbs );
			sbs.OnUpdate += FocusDetector_;
			//FocusDetector( true );
			// if ( lastFocus == false ) return;
			// sbs.OnSelectionChanged += _Init2;

		}
		//void _Init2()
		//{
		//    Init();
		//}

		public void AddItemsToMenu( GenericMenu menu )
		{
			generate_menu( menu, NAME, lastController );
		}
		public static void OpenWindow()
		{
			GetExternalWindow<HyperGraphModWindow>.TryToClose_Or_Show( NAME );
		}

		void _Init()
		{
			Init();
			EditorApplication.update -= _Init;
			//sbs.OnUpdate += FocusDetector_;
		}

		internal override void OnEnable()
		{
			base.OnEnable();
			//}
			//internal override void OnEnable()
			//{
			//    base.OnEnable();
			if ( Root.p == null || Root.p.Length == 0 || Root.p[ 0 ] == null ) return;
			EditorApplication.update += _Init;
			AssignContent();
		}



		internal static void ICON_CLICK( int button, string name )
		{
			if ( button == 0 )
			{
				//controller = ;
				//if (W.minSize.x < 40 || W.minSize.y < 16) {W.minSize = new Vector2(40, 16); }
				//	W.ShowTab();
				HyperGraphModWindow W;
				if ( Root.p[ 0 ].par_e.ATTACH_TO_INSPECT_ONOPEN )
				{
					try
					{
						W = HyperGraphModWindow.GetWindow<HyperGraphModWindow>( name, true, InspectorType );
					}
					catch
					{
						W = HyperGraphModWindow.GetWindow<HyperGraphModWindow>( name, true );
					}
				}
				else
				{
					W = HyperGraphModWindow.GetWindow<HyperGraphModWindow>( name, true );
				}
				W.Show();
				W.Init();
			}
			if ( button == 1 )
			{
				var menu = new GenericMenu();
				MENU_GEN( menu, name, lastController );
				menu.ShowAsContext();
			}
		}

		internal static void MENU_GEN( GenericMenu menu, string name, ExternalDrawContainer lastController )
		{
			if ( !MainMenuItems.MinimizeBottomBar_IsValid() )
			{
				menu.AddDisabledItem( new GUIContent( "- Show HyperGraph in bottom bar" ) );
			}
			else
			{
				menu.AddItem( new GUIContent( "- Open HyperGraph in bottom bar" ), false, () => {
					MainMenuItems.MinimizeBottomBar();
				} );
			}


			menu.AddItem( new GUIContent( "- Open HyperGraph in window" ), false, () => {
				GetExternalWindow<HyperGraphModWindow>.Show( name );
			} );
			menu.AddItem( new GUIContent( "- Open HyperGraph in window and attach to Inspector" ), false, () => {
				GetExternalWindow<HyperGraphModWindow>.Show( name, InspectorType );
				/*var W = HyperGraphModWindow.GetWindow<HyperGraphModWindow>(name, true, InspectorType);
                W.Show();
                W.Init();*/
			} );
			menu.AddSeparator( "" );
			generate_menu( menu, name, lastController );
		}


		static void generate_menu( GenericMenu menu, string name, ExternalDrawContainer lastController )
		{
			menu.AddItem( new GUIContent( "Use UnityEvents mode" ), Root.p[ 0 ].par_e.HYPERGRAPH_EVENTS_MODE_BOOL, () => {
				Root.p[ 0 ].par_e.HYPERGRAPH_EVENTS_MODE_BOOL = !Root.p[ 0 ].par_e.HYPERGRAPH_EVENTS_MODE_BOOL;
			} );


			menu.AddSeparator( "" );

			HyperGraphModInstance targetHyper = null;
			var external = false;
			foreach ( var item in Root.p[ 0 ].modsController.activeExternalMods )
			{
				if ( item is HyperGraphModWindow )
				{
					targetHyper = ((HyperGraphModWindow)item).instance;
					external = true;
					break;
				}
			}
			if ( targetHyper == null && Root.p[ 0 ].par_e.USE_BOTTOMBAR_MOD )
			{
				var c=  new ExternalDrawContainer(0){ controller_type = 1};
				targetHyper = new HyperGraphModInstance( () => c );
			}


			menu.AddItem( new GUIContent( "Auto refresh when selection changed" ), targetHyper.H_AUTOCHANGE, () => {
				targetHyper.H_AUTOCHANGE = !targetHyper.H_AUTOCHANGE;
				if ( external ) Root.p[ 0 ].RepaintExternalNow();
				else Root.p[ 0 ].RepaintWindowInUpdate( 0 );
			} );
			menu.AddItem( new GUIContent( "Auto hide when selection changed" ), targetHyper.H_AUTOHIDE, () => {
				targetHyper.H_AUTOHIDE = !targetHyper.H_AUTOHIDE;
				if ( external ) Root.p[ 0 ].RepaintExternalNow();
				else Root.p[ 0 ].RepaintWindowInUpdate( 0 );
			} );
			menu.AddSeparator( "" );


			menu.AddItem( new GUIContent( "Display arrays" ), Root.p[ 0 ].par_e.HYPERGRAPH_SKIP_ARRAYS_BOOL_INVERCE, () => {
				Root.p[ 0 ].par_e.HYPERGRAPH_SKIP_ARRAYS_BOOL_INVERCE = !Root.p[ 0 ].par_e.HYPERGRAPH_SKIP_ARRAYS_BOOL_INVERCE;
			} );
			menu.AddItem( new GUIContent( "Display assets references" ), Root.p[ 0 ].par_e.HYPERGRAPH_DISPLAY_ASSETS, () => {
				Root.p[ 0 ].par_e.HYPERGRAPH_DISPLAY_ASSETS = !Root.p[ 0 ].par_e.HYPERGRAPH_DISPLAY_ASSETS;
			} );
			menu.AddItem( new GUIContent( "Display self references" ), Root.p[ 0 ].par_e.HYPERGRAPH_CONNECT_TO_SELFT, () => {
				Root.p[ 0 ].par_e.HYPERGRAPH_CONNECT_TO_SELFT = !Root.p[ 0 ].par_e.HYPERGRAPH_CONNECT_TO_SELFT;
			} );

			menu.AddSeparator( "" );
			menu.AddItem( new GUIContent( "Display loading indicator" ), Root.p[ 0 ].par_e.HYPERGRAPH_SHOWUPDATINGINDICATOR, () => {
				Root.p[ 0 ].par_e.HYPERGRAPH_SHOWUPDATINGINDICATOR = !Root.p[ 0 ].par_e.HYPERGRAPH_SHOWUPDATINGINDICATOR;

			} );

			menu.AddItem( new GUIContent( "Field names alignment/Left" ), Root.p[ 0 ].par_e.HYPERGRAPH_FIELD_NAMES_ALIGNMENT == 0, () => {
				Root.p[ 0 ].par_e.HYPERGRAPH_FIELD_NAMES_ALIGNMENT = 0;
			} );
			menu.AddItem( new GUIContent( "Field names alignment/Red Right" ), Root.p[ 0 ].par_e.HYPERGRAPH_FIELD_NAMES_ALIGNMENT == 1, () => {
				Root.p[ 0 ].par_e.HYPERGRAPH_FIELD_NAMES_ALIGNMENT = 1;
			} );


			menu.AddItem( new GUIContent( "Red for null fields/None" ), Root.p[ 0 ].par_e.HYPERGRAPH_DRAW_RED_FOR_NULLS == 0, () => {
				Root.p[ 0 ].par_e.HYPERGRAPH_DRAW_RED_FOR_NULLS = 0;
			} );
			menu.AddItem( new GUIContent( "Red for null fields/Red marker" ), Root.p[ 0 ].par_e.HYPERGRAPH_DRAW_RED_FOR_NULLS == 1, () => {
				Root.p[ 0 ].par_e.HYPERGRAPH_DRAW_RED_FOR_NULLS = 1;
			} );
			menu.AddItem( new GUIContent( "Red for null fields/Red marker and name" ), Root.p[ 0 ].par_e.HYPERGRAPH_DRAW_RED_FOR_NULLS == 2, () => {
				Root.p[ 0 ].par_e.HYPERGRAPH_DRAW_RED_FOR_NULLS = 2;
			} );
			menu.AddItem( new GUIContent( "Clip field names" ), Root.p[ 0 ].par_e.HYPERGRAPH_CLIP_NAMES, () => {
				Root.p[ 0 ].par_e.HYPERGRAPH_CLIP_NAMES = !Root.p[ 0 ].par_e.HYPERGRAPH_CLIP_NAMES;
			} );
			menu.AddItem( new GUIContent( "Use bold lines" ), Root.p[ 0 ].par_e.HYPERGRAPH_USE_BOLD_LINES, () => {
				Root.p[ 0 ].par_e.HYPERGRAPH_USE_BOLD_LINES = !Root.p[ 0 ].par_e.HYPERGRAPH_USE_BOLD_LINES;
			} );

			menu.AddSeparator( "" );

			menu.AddItem( new GUIContent( "- Open HyperGraph Settings" ), false, () => {
				Settings.MainSettingsEnabler_Window.Select<Settings.HG_Window>();
			} );
		}


		bool? lastFocus;
		//	float? oldH, oldW;
		//static VisualElement lastTypes;

		void FocusDetector_()
		{
			FocusDetector( false );
		}
		void FocusDetector( bool skip )
		{

			var enabled = rootVisualElement.parent != null;
			if ( !lastFocus.HasValue )
			{
				lastFocus = enabled;
			}
			if ( lastFocus != enabled )
			{
				lastFocus = enabled;
				//if ( !skip ) adapter.modsController.REBUILD_PLUGINS( true );
				adapter.modsController.REBUILD_PLUGINS_FAST();
				instance.SCENE_CHANGE();
			}
			/*	if (enabled && (oldH != position.height || oldW != position.width))
				{
					oldH = position.height;
					oldW = position.width;
					lastTypes = rootVisualElement.parent;
					//Debug.Log(lastTypes.Length);
				}*/

			//	Debug.Log(rootVisualElement);
			//	Debug.Log();
			//	Debug.Log(rootVisualElement.parent.enabledInHierarchy);
			//	Debug.Log(rootVisualElement.parent.childCount);
			//	Debug.Log(rootVisualElement.panel.visualTree.childCount);
			//	Debug.Log(rootVisualElement.panel.focusController.focusedElement.focusable);
		}



		float? oldHeight;
		float? oldWidth;

		bool mayscroll;
		//	HyperGraphMod hyperGraph { get { return adapter.modsController.ex_HyperGraphMod; } }



		void AssignContent()
		{
			controller.ASIGN_CONTENT( NAME, "HYPER_ICON", this );
		}

		internal static ExternalDrawContainer lastController;

		internal override void OnGUI_Draw()
		{
			/*	if (lastTypes != null)
		{
			rootVisualElement.RemoveFromHierarchy();
			lastTypes.Add(rootVisualElement);
			lastTypes.MarkDirtyRepaint();
			lastTypes = null;
		}*/

			if ( WAS_INIT )
			{
				instance.CHECK_SCAN();
			}
			lastController = controller;

			if ( Event.current.type == EventType.ScrollWheel && position( IExternalWindowType.HYPER_GRAPH )
				// new Rect( 0, 0, position( IExternalWindowType.HYPER_GRAPH ).width, position( IExternalWindowType.HYPER_GRAPH ).height )
				.Contains( Event.current.mousePosition ) )
			{
				if ( mayscroll )
				{
					//if (adapter.OnScroll != null) adapter.OnScroll(Adapter.ScrollType.HyperGraphScroll_Window, Event.current.delta.y);
					instance.ON_SCROLL( Event.current.delta.y );
					mayscroll = false;
				}
			}

			if ( Event.current.type == EventType.Repaint )
			{
				mayscroll = true;
			}


			if ( !oldHeight.HasValue ) oldHeight = position( IExternalWindowType.HYPER_GRAPH ).height;
			if ( oldHeight.Value != position( IExternalWindowType.HYPER_GRAPH ).height )
			{
				var oldH = oldHeight.Value;
				oldHeight = position( IExternalWindowType.HYPER_GRAPH ).height;
				//  controller.HEIGHT = (startHeight + (startPos.y - p.y));
				//  CHECK_HEIGHT();
				controller.scrollPos.y -= (oldH - controller.HEIGHT( IExternalWindowType.HYPER_GRAPH )) / 2;
				// Hierarchy.BottomInterface.HyperGraph.RESET_SMOOTH_HEIGHT();
			}


			if ( !oldWidth.HasValue ) oldWidth = position( IExternalWindowType.HYPER_GRAPH ).width;
			if ( oldWidth.Value != position( IExternalWindowType.HYPER_GRAPH ).width )
			{
				var oldW = oldWidth.Value;
				oldWidth = position( IExternalWindowType.HYPER_GRAPH ).width;
				//  controller.HEIGHT = (startHeight + (startPos.y - p.y));
				//  CHECK_HEIGHT();
				controller.scrollPos.x -= (oldW - controller.WIDTH( IExternalWindowType.HYPER_GRAPH )) / 2;
				// Hierarchy.BottomInterface.HyperGraph.RESET_SMOOTH_HEIGHT();
			}


			/*	if (!wasInit)
				{
					if (Event.current.type == EventType.Repaint)
					{ // MonoBehaviour.print(hyperwindow.position);
						if (position.x < 15 && position.y < 50)
						{
							var p = position;
							p.x = WinBounds.MAX_WINDOW_WIDTH.x + (WinBounds.MAX_WINDOW_WIDTH.y - p.width) / 2;
							p.y = WinBounds.MAX_WINDOW_HEIGHT.x + (WinBounds.MAX_WINDOW_HEIGHT.y - p.height) / 2;
							position = p;
						}

						wasInit = true;
					}

					// return;
				}*/

			adapter.ChangeGUI();
			instance.EXTERNAL_HYPER_DRAWER( new Rect( 0, 0, position( IExternalWindowType.HYPER_GRAPH ).width, position( IExternalWindowType.HYPER_GRAPH ).height ), controller, this );
			adapter.RestoreGUI();



		}


	}
}
