using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class SA_Window : ScriptableObject
	{
	}

	[CustomEditor( typeof( SA_Window ) )]
	class SETGUI_SetActive : MainRoot
	{

		internal static string set_text =  USE_STR + "SetActive GameObject";
		internal static string set_key = "USE_SETACTIVE_MOD";

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

			//   GUI.Button( Draw.R2, "xxx", Draw.s( "preToolbar" ) );
			// GUI.Button( Draw.R, "Common Settings", Draw.s( "insertionMarker" ) );
			Draw.BACK_BUTTON( w );
			Draw.TOG_TIT( set_text, set_key, WIKI: WIKI_4_SETACTIVE );
			Draw.Sp( 10 );

			using ( ENABLE( w ).USE( set_key ) )
			{
				using ( GRO( w ).UP( 0 ) )
				{
					Draw.Sp( 4 );

					// Draw.TOG( "Small style", "SET_ACTIVE_SMALL_BOOL" );
					GUI.Label( Draw.R, "Button style:" );
					Draw.TOOLBAR( new[] { "Default", "Narrow", "Dark", "Narrow Dark" }, "SET_ACTIVE_STYLE_TOOLBAR" );
					Draw.Sp( 10 );

					Draw.TOG( "Change cursor if mouse hovering over the content", "SET_ACTIVE_CHANGE_BUTTON_CURSOR" );
					Draw.Sp( 10 );

					Draw.TOG( "Replace prefab button", "SET_ACTIVE_PREFAB_BUTTON_OFFSET" );
					Draw.Sp( 10 );

					

					Draw.TOG( "Smoothed camera movement to objects on RMB", "SET_ACTIVE_SMOOTH_FRAME" );
					Draw.HELP( w, "RMB to move scene camera to GameObject ('F' key or double click by default)" );
					Draw.Sp( 10 );
				}

			}
			


			Draw.Sp( 10 );
			//Draw.HRx2();
			//GUI.Label( Draw.R, "" + LEFT + " Area:" );
			using ( GRO( w ).UP( 0 ) )
			{
				// Draw.TOG_TIT( "" + LEFT + " Area:" );
				Draw.TOG_TIT( "Quick tips:" );
				Draw.HELP( w, "You can use dragging to enable/disable several gameobjects.", drawTog: true );
				Draw.HELP_TEXTURE( w, "HELP_SETACTIVE", 0 );
				Draw.HELP( w, "If you have selection, and click button for selected objects, new state will apply to all selected objects.", drawTog: true );
				Draw.HELP( w, "If you have selection, and click button for non - selected object, new state will apply to only one object on which you clicked.", drawTog: true );


			}
		}
	}
}
