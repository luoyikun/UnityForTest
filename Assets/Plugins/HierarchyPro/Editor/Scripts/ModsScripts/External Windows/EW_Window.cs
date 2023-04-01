using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class EW_Window : ScriptableObject
	{
	}


	[CustomEditor( typeof( EW_Window ) )]
	class SETGUI_ExternalWindows : MainRoot
	{

		//EMX_TODO Change settings for external and internal windows

		internal static string set_text =  USE_STR + "External Windows - Integration Settings";
		//internal static string set_key = "USE_BOTTOMBAR_MOD";
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
			Draw.TOG_TIT( set_text );
			Draw.Sp( 10 );


			using ( ENABLE( w ).USE( "ENABLE_ALL", 0 ) )
			using ( GRO( w ).UP() )
			{
				GUI.Label( Draw.R, "External Windows HotButtons:" );
				Draw.HELP( w, "Use RightClick on the icon to open quick menu" );
				//if (p.par_e.USE_TOPBAR_MOD)

				using ( ENABLE( w ).USE( "USE_TOPBAR_MOD", 0 ) )
				{
					Draw.TOG( "TopBar - Draw External Mods HotButtons", "DRAW_TOPBAR_HOTBUTTONS", rov: Draw.R );
					// if ( p.par_e.DRAW_TOPBAR_HOTBUTTONS ) 
					using ( ENABLE( w ).USE( "DRAW_TOPBAR_HOTBUTTONS" ) )
						Draw.FIELD( "TopBar Buttons Size", "TOPBAR_HOTBUTTON_SIZE", 3, 60, "px" );
				}

				using ( ENABLE( w ).USE( "USE_RIGHT_ALL_MODS", 0 ) )
				{
					Draw.TOG( "Hierarchy Header - Draw External Mods HotButtons", "DRAW_HEADER_HOTBUTTONS", rov: Draw.R );
					using ( ENABLE( w ).USE( "DRAW_HEADER_HOTBUTTONS", "USE_RIGHT_ALL_MODS", CLASS_ENALBE.operation.AND ) )
					//if ( p.par_e.DRAW_HEADER_HOTBUTTONS )
					{ Draw.FIELD( "Hierarchy Header Buttons Size", "HEADER_HOTBUTTON_SEZE", 3, 60, "px" ); }
				}

				using ( ENABLE( w ).USE( "USE_BOTTOMBAR_MOD", padding: 0 ) )
				{
					Draw.TOG( "Bottom Bar - Draw External Mods HotButtons", "DRAW_BOTTOM_HOTBUTTONS", rov: Draw.R );
					using ( ENABLE( w ).USE( "DRAW_BOTTOM_HOTBUTTONS" ) )
						Draw.FIELD( "Bottom Bar Buttons Size", "BOTTOM_HOTBUTTON_SEZE", 3, 60, "px" );
				}


				if ( p.par_e.DRAW_HEADER_HOTBUTTONS && p.par_e.USE_RIGHT_ALL_MODS || p.par_e.DRAW_TOPBAR_HOTBUTTONS && p.par_e.USE_TOPBAR_MOD
					 || p.par_e.DRAW_BOTTOM_HOTBUTTONS && p.par_e.USE_BOTTOMBAR_MOD )
				{
					Draw.Sp( 10 );
					//using ( GRO( w ).UP() )
					using ( ENABLE( w ).USE( "ENABLE_ALL" ) )
					{
						// Draw.TOG( "HyperGraph", "DRAW_TOPBAR_H1" );
						// Draw.TOG( "Project Folders", "DRAW_TOPBAR_H2" );
						// Draw.TOG( "Hierarchy Bookmarks", "DRAW_TOPBAR_H3" );
						// Draw.TOG( "Hierarchy Scenes", "DRAW_TOPBAR_H4" );
						// Draw.TOG( "Hierarchy Last Selection", "DRAW_TOPBAR_H5" );
						// Draw.TOG( "Hierarchy Expanded Objects", "DRAW_TOPBAR_H6" );
						p.par_e.DrawHotButtonsArray();
					}
				}


				//if (p.par_e.USE_RIGHT_CLICK_MENU_MOD)
				//using (ENABLE(w).USE("USE_RIGHT_CLICK_MENU_MOD", 0))
				//	Draw.TOG("Add External Mods Menu Items to GameOjbect RightClick Menu", "INCLUDE_RIGHTCLIK_MENU_OPENPLUGINWINDOWS_BUTTONS", rov: Draw.R);
				Draw.Sp( 16 );
			}
		}
	}
}
