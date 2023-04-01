using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class PM_Window : ScriptableObject
	{
	}


	[CustomEditor( typeof( PM_Window ) )]
	class SETGUI_PresetsManager : MainRoot
	{
		internal static string set_text =  USE_STR + "Presets Manager for Hierarchy";
		internal static string set_key = "USE_CUSTOM_PRESETS_MOD";
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
			Draw.TOG_TIT( set_text, set_key,WIKI: WIKI_3_PRESETS );
			Draw.Sp( 10 );

			using ( ENABLE( w ).USE( set_key ) )
			{
				using ( GRO( w ).UP( 0 ) )
				{
					SETGUI_HL_HierarchyManual.BUTTONPLACE( w, p.par_e.HIER_HIGH_SET, 0 );
				}

				// Draw.Sp( 10 );
				// Draw.HRx2();

				Draw.Sp( 10 );

				using ( GRO( w ).UP( 0 ) )
				{
					Draw.TOG_TIT( "Common Settings:" );
					Draw.TOG( "Save objects references:", "PRESETS_SAVE_GAMEOBJEST" );
					// Draw.Sp( 16 );
					using ( ENABLE( w ).USE( "PRESETS_SAVE_GAMEOBJEST" ) )
					{

						Draw.TOG( "Skip null assign (if value changed with null):", "PRESETS_SKIP_NULL_REPLACE" );
						Draw.HELP( w, "Try to use it, if you have any problems with object references" ); //, I will add a bit later the ability to save only selected variables, and common paste for different scripts
					}
				}


				Draw.Sp( 10 );

				using ( GRO( w ).UP( 0 ) )
				{

					// Draw.TOG_TIT( "" + LEFT + " Area:" );

					Draw.TOG_TIT( "Quick tips:" );
					// Draw.HELP(w, "This function is available in the same window as the highlighter", drawTog: true ); //, I will add a bit later the ability to save only selected variables, and common paste for different scripts
					Draw.HELP( w, "To open left drown down window, LMB on the little button to the left of the object name (by default).", drawTog: true );
					//Draw.HELP_TEXTURE( w, "HELP_HIGHLIGHTER", 0 );
					Draw.HELP( w, "The preset manager will be the last in the tabs list." );

				}
			}
		}
	}
}
