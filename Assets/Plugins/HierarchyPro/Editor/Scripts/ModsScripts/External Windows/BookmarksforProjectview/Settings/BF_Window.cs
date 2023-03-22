using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using EMX.HierarchyPlugin.Editor.Mods;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class BF_Window : ScriptableObject
	{
	}


	[CustomEditor( typeof( BF_Window ) )]
	class SETGUI_ProjectFolders : MainRoot
	{

		internal static string set_text =  USE_STR + "EW - Folders for Project";
		internal static string set_key = "USE_BOOKMARKS_PROJECT_MOD";
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
			Draw.TOG_TIT( set_text, set_key, WIKI: WIKI_6_PROJECTFOLDERS );
			Draw.Sp( 10 );




			using ( ENABLE( w ).USE( set_key ) )
			{
				using ( GRO( w ).UP( 0 ) )
				{
					Draw.TOG( "Draw hot button for this mod", "DRAW_TOPBAR_H2" );
					Draw.Sp( 10 );

					// 
					Draw.FIELD( "Lines height", "BOOKMARKS_FOLDER_LINE_HEIGHT", 4, 50 );
					Draw.FIELD( "Labels font size", "BOOKMARKS_FOLDER_FONTSIZE", 4, 50 );
					Draw.FIELD( "Objects icons size", "BOOKMARKS_FOLDER_DEFAULT_ICON_SIZE", 4, 50 );
					Draw.Sp( 10 );

					Draw.TOG( "Use descriptions", "BOOKMARKS_FOLDER_SHOW_ALL_DESCRIPTIONS_INHIER" );
					Draw.HRx05( Draw.R05 );
					Draw.TOG( "Use special background colors", "BOOKMARKS_FOLDER_DRAW_BG_COLOR" );
					using ( ENABLE( w ).USE( "BOOKMARKS_FOLDER_DRAW_BG_COLOR" ) )
					{
						if ( GUI.Button( Draw.R15, "Open categories background colors window" ) )
						{
							var pos = new MousePos(Event.current.mousePosition, MousePos.Type.ColorChanger_230_0, false, Root.p[0]);
							CategoriesColorsWindowOld.InitForProjectFolder( pos, null );
						}
					}
				}

				//using (ENABLE(w).USE("BOOKMARKS_FOLDER_DRAW_BG_COLOR")) Draw.COLOR("Background Color", "BOOKMARKS_FOLDER_DEFULT_BG_COLOR");
			}


			// Draw.TOG( "Draw hot button for this mod", "DRAW_TOPBAR_H1" );
			// Draw.TOG( "HyperGraph", "DRAW_TOPBAR_H1" );
			// Draw.TOG( "Project Folders", "DRAW_TOPBAR_H2" );
			// Draw.TOG( "Hierarchy Bookmarks", "DRAW_TOPBAR_H3" );
			// Draw.TOG( "Hierarchy Scenes", "DRAW_TOPBAR_H4" );
			// Draw.TOG( "Hierarchy Expanded Objects", "DRAW_TOPBAR_H6" );



			Draw.Sp( 10 );
			//Draw.HRx2();
			//GUI.Label( Draw.R, "" + LEFT + " Area:" );
			using ( GRO( w ).UP( 0 ) )
			{

				// Draw.TOG_TIT( "" + LEFT + " Area:" );

				Draw.TOG_TIT( "Quick tips:" );
				Draw.HELP( w, "Use right-click to open special menu for bookmark.", drawTog: true );
				Draw.HELP( w, "You can include all child folders.", drawTog: true );
				Draw.HELP( w, "You can filter objects by file extensions.", drawTog: true );
				Draw.HELP( w, "You can disable tree drawing style for child folders.", drawTog: true );
				Draw.HELP( w, "You can add description.", drawTog: true );
				Draw.HELP_TEXTURE( w, "TAP_FOLDER" );
				Draw.HELP( w, "Use the right-click on the icon to open a special menu for quick access to mod functions.", drawTog: true );
				Draw.HELP_TEXTURE( w, "HELP_FOLDER" );

				//Draw.HELP(w,"Use right-click to remove object.", drawTog: true);
				//Draw.HELP(w,"You can also add descriptions and assign many objects to one button.", drawTog: true);
			}

		}
	}
}
