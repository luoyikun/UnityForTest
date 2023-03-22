using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class HG_Window : ScriptableObject
	{
	}


	[CustomEditor( typeof( HG_Window ) )]
	class SETGUI_HyperGraph : MainRoot
	{
		internal static string set_text = "BB / EW - HyperGraph";
		internal static string set_key = "USE_HYPERGRAPH_MOD";
		public override VisualElement CreateInspectorGUI()
		{
			return base.CreateInspectorGUI();
		}
		public override void OnInspectorGUI()
		{
			_GUI( (IRepaint)this );
		}
		public static void _GUI( IRepaint w )
		{
			Draw.RESET( w );

			Draw.BACK_BUTTON( w );
			Draw.TOG_TIT( set_text, set_key  ,WIKI: WIKI_5_HYPERGRAPH);
			Draw.Sp( 10 );



			using ( ENABLE( w ).USE( set_key ) )
			{

				{
					Draw.TOG( "Draw hot button for this mod", "DRAW_TOPBAR_H1" );
				}

				Draw.Sp( 10 );

				using ( GRO( w ).UP( 0 ) )
				{
					//using ( GRO( w ).UP( 0 ) )
					//Draw.Sp( 4 );

					Draw.TOG_TIT( "Common settings:" );

					Draw.Sp( 4 );

					Draw.TOG( "Attach Windows to Inspector OnOpen", "ATTACH_TO_INSPECT_ONOPEN" );

					// Draw.Sp( 4 );
					//Draw.TOG( "Draw hot button for this mod", "DRAW_TOPBAR_H1" );
					//Draw.HRx05( Draw.R05 );
					Draw.HRx05( Draw.R05 );

					Draw.TOG( "Use unityevents mode", "HYPERGRAPH_EVENTS_MODE_BOOL" );
					Draw.HELP( w, "Use 'autohide' button int the hypergraph to enable hiding when object will changed", drawTog: true );
					Draw.HELP( w, "Use 'autoreload' button int the hypergraph to disable reloading when object will changed", drawTog: true );
					Draw.HRx05( Draw.R05 );
					Draw.TOG( "Reset scroll on reload", "HYPERGRAPH_RESET_SCROLL_ONRELOAD" );
					Draw.Sp( 10 );

				}
				Draw.Sp( 10 );

				using ( GRO( w ).UP( 0 ) )
				{
					Draw.TOG_TIT( "Interface settings:" );
					Draw.Sp( 4 );
					Draw.FIELD( "Interface font size", "HYPERGRAPH_INT_FONTSIZE", 4, 50 );
					Draw.TOG( "Draw gameobject name", "HYPERGRAPH_DRAWBOLD_LABEL" );
					Draw.TOG( "Display loading indicator", "HYPERGRAPH_SHOWUPDATINGINDICATOR" );


					Draw.Sp( 10 );
				}
				Draw.Sp( 10 );

				using ( GRO( w ).UP( 0 ) )
				{
					Draw.TOG_TIT( "Objects settings:" );
					Draw.Sp( 4 );
					Draw.FIELD( "Field names font size", "HYPERGRAPH_OB_FONTSIZE", 4, 50 );
					Draw.TOG( "Clip field names", "HYPERGRAPH_CLIP_NAMES" );
					//Draw.TOG( "Red null references", "HYPERGRAPH_RED_HIGKLIGHTING" );
					Draw.Sp( 10 );
					GUI.Label( Draw.R, "Field names alignment:" );
					Draw.TOOLBAR( new[] { "Left", "Right" }, "HYPERGRAPH_FIELD_NAMES_ALIGNMENT" );
					Draw.Sp( 10 );
					GUI.Label( Draw.R, "Use red color for unassigned fields:" );
					Draw.TOOLBAR( new[] { "None", "Red marker", "Red marker and name" }, "HYPERGRAPH_DRAW_RED_FOR_NULLS" );
					Draw.TOG( "Use bold lines", "HYPERGRAPH_USE_BOLD_LINES" );
					Draw.Sp( 10 );

				}







				Draw.Sp( 10 );
				using ( GRO( w ).UP( 0 ) )
				{
					Draw.TOG_TIT( "Scan Options:" );
					Draw.TOG( "Display arrays", "HYPERGRAPH_SKIP_ARRAYS_BOOL_INVERCE" );
					Draw.TOG( "Display assets references", "HYPERGRAPH_DISPLAY_ASSETS" );
					Draw.TOG( "Display self references", "HYPERGRAPH_CONNECT_TO_SELFT" );
					Draw.Sp( 4 );
					//}

					Draw.Sp( 10 );
					//using ( GRO( w ).UP( 0 ) )
					//{
					Draw.TOG_TIT( "Scan performance:", EnableRed: false );
					Draw.TOOLBAR( new[] { "20%", "40%", "60%", "80%", "∞" }, "HYPERGRAPH_SCANPERFOMANCE04" );
					Draw.HELP( w, "High value ​​reduces performance" );
					Draw.Sp( 10 );
					Draw.Sp( 4 );
				}
			}










			Draw.Sp( 10 );
			//Draw.HRx2();
			//GUI.Label( Draw.R, "" + LEFT + " Area:" );
			using ( GRO( w ).UP( 0 ) )
			{

				// Draw.TOG_TIT( "" + LEFT + " Area:" );

				Draw.TOG_TIT( "Quick tips:" );
				Draw.HELP_TEXTURE( w, "TAP_HYPER" );
				Draw.HELP( w, "RMB on the icon to open a special menu for quick access to mod functions.", drawTog: true );
				Draw.HELP_TEXTURE( w, "HELP_HYPER" );
				Draw.HELP( w, "LMB to select object, or drag.", drawTog: true );
				Draw.HELP( w, "You can drag objects from the hierarchy or in the hierarchy window.", drawTog: true );
				Draw.HELP( w, "Use events mode for tracking unity events references only.", drawTog: true );
				Draw.HELP( w, "You can disable scanning for arrays or internal structures.", drawTog: true );
				//Draw.HELP(w,"Use right-click to remove object.", drawTog: true);
				//Draw.HELP(w,"You can also add descriptions and assign many objects to one button.", drawTog: true);


				//Draw.HRx1();
				//Draw.HELP( w, "You can find an example scene with hypergraph demos.", drawTog: true );
				//Draw.HELP(w,"You can add your own items using 'ExtensionInterface_RightClickOnGameObjectMenuItem'.", drawTog: true);
				if ( Draw.BUT( "Select Example Scene" ) ) { Selection.objects = new[] { Root.icons.example_folders[ 4 ] }; }

				/*   using (ENABLE(w).USE(set_key))
				   {
				   }*/
			}
		}
	}
}
