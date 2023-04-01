using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class HLM_Window : ScriptableObject
	{
	}


	[CustomEditor( typeof( HLM_Window ) )]
	class SETGUI_HL_HierarchyManual : MainRoot
	{
		internal static string set_text =  USE_STR + "HighLighter for Hierarchy";
		internal static string set_key = "USE_HIERARCHY_MANUAL_HIGHLIGHTER_MOD";
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

			Draw.BACK_BUTTON( w  );
			Draw.TOG_TIT( set_text , WIKI: WIKI_3_M_HIGH);
			Draw.Sp( 10 );




			using ( GRO( w ).UP( 0 ) )
			{


				using ( ENABLE( w ).USE( (Draw.TOG_TIT( "Use " + SETGUI_HL_HierarchyAuto.set_text, SETGUI_HL_HierarchyAuto.set_key, rightOffset: 1 )) ) ) { }
				//if ( Draw.BUT( Draw.last, HighlighterAutoModSettingsEditor.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<HLA_Window>(); }
				Draw.HELP_TEXTURE( Draw.last, "HL_B" );
				using ( ENABLE( w ).USE( (Draw.TOG_TIT( "Use Manual Highlighter", SETGUI_HL_HierarchyManual.set_key, rightOffset: 1, EnableRed: false )) ) ) { }
				//if ( Draw.BUT( Draw.last, HighlighterManualModSettingsEditor.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<HLM_Window>(); }
				Draw.HELP_TEXTURE( Draw.last, "HL_A" );

				using ( ENABLE( w ).USE( set_key, SETGUI_HL_HierarchyAuto.set_key, CLASS_ENALBE.operation.OR ) )
				{
					BUTTONPLACE( w, p.par_e.HIER_HIGH_SET, 0 );
				}

			}




			Draw.Sp( 10 );
			using ( GRO( w ).UP( 0 ) )
			{
				using ( ENABLE( w ).USE( set_key, SETGUI_HL_HierarchyAuto.set_key, CLASS_ENALBE.operation.OR ) )
				{

					ASD( w, p.par_e.HIER_HIGH_SET, 0 );








					//	adapter.par.COLOR_ICON_SIZE = Mathf.Clamp(EditorGUI.IntField(lineRect, "<b>Custom Icons</b> size '" + EditorGUIUtility.singleLineHeight + "'", adapter.par.COLOR_ICON_SIZE), 10, 30);
					//	adapter._S_USEdefaultIconSize = adapter.TOGGLE_LEFT(lineRect, "<i>Use Default Icons size:</i>", adapter._S_USEdefaultIconSize);
					// adapter._S_defaultIconSize = Mathf.Clamp(EditorGUI.IntField(lineRect, "<b>Defaul Iconst</b> size '" + EditorGUIUtility.singleLineHeight + "'", adapter._S_defaultIconSize), 10, 30);

					//if (adapter.IS_HIERARCHY())
					//{
					//
					//	EditorGUI.BeginChangeCheck();
					//	lineRect.y += lineRect.height;
					//	adapter.par.SHOW_NULLS = adapter.TOGGLE_LEFT(lineRect, "Show Locator for Object without Component", adapter.par.SHOW_NULLS);
					//
					//	lineRect.y += lineRect.height;
					//	adapter.par.SHOW_PREFAB_ICON = adapter.TOGGLE_LEFT(lineRect, "Show Prefab icon", adapter.par.SHOW_PREFAB_ICON);
					//
					//	lineRect.y += lineRect.height;
					//	adapter.par.SHOW_MISSINGCOMPONENTS = adapter.TOGGLE_LEFT(lineRect, "Show Warning if Object has missing Component", adapter.par.SHOW_MISSINGCOMPONENTS);
					//	if (EditorGUI.EndChangeCheck())
					//	{
					//		if (adapter.OnClearObjects != null) adapter.OnClearObjects();
					//	}
					//}


				}

			}



			Draw.Sp( 10 );
			using ( ENABLE( w ).USE( SETGUI_HL_HierarchyAuto.set_key ) )
			{

				using ( GRO( w ).UP( 0 ) )
				{
					Draw.TOG_TIT( "Auto mode:" );
					Draw.TOG( "Auto adding default filters on initialization", /*-*/"AUTOHIGHLIGHTER_USE_DEFAULT_FILTERS", overrideObject: p.par_e.HIER_HIGH_SET );
					Draw.HELP( w, "Few default presets will be always included by default.\n\nIf you uncheck this toggle, the auto highlighter will be created with empty list for every new projects." );
					Draw.Sp( 5 );
					if ( Draw.BUT( "Add default presets to auto highlighter manually" ) ) HighLighterCommonData.AddFiltersToAutoHighlighter();
					Draw.Sp( 10 );
				}
			}


			Draw.Sp( 10 );

			using ( GRO( w ).UP( 0 ) )
			{

				// Draw.TOG_TIT( "" + LEFT + " Area:" );

				Draw.TOG_TIT( "Quick tips:" );
				Draw.HELP( w, "To open left drown down window, LMB on the little button to the left of the object name (by default).", drawTog: true );
				//Draw.HELP( w, "Use special button to open highlighter for selected objects.", drawTog: true );
				//Draw.HELP_TEXTURE( w, "HELP_HIGHLIGHTER", 0 );

			}
		}


		internal static void BUTTONPLACE( IRepaint w, EditorSettingsAdapter.HighlighterSettings KEY, int pluginID )
		{


			Draw.TOG_TIT( "Open window - button location:", EnableRed: false );
			Settings.Draw.TOOLBAR( new[] { "None", "Left 16x16", "Left wide", "Icon 16x16" }, /*-*/"HIGHLIGHTER_HIERARCHY_BUTTON_LOCATION", overrideObject: KEY );
			Draw.HELP( w, "You also can open window using GameObject menu" );
			GUI.Label( Settings.Draw.R, "Draw little marker at button:" );
			Settings.Draw.TOOLBAR( new[] { "None", "Only Opened", "Always Hovered" }, /*-*/"HIGHLIGHTER_HIERARCHY_DRAW_BUTTON_RECTMARKER", overrideObject: KEY, tooltips: new[] { "None", "The marker will be drawn only if the Drop-Down window window is opened", "When the mouse hovering over the object line, the highlight mod will draw a small marker." } );
			Draw.FIELD( "Marker Size:", /*-*/"HIGHLIGHTER_HIERARCHY_BUTTON_RECTMARKER_SIZE", 1, 10, overrideObject: KEY );
			Draw.TOG( "Change cursor if mouse hovering over the button", /*-*/"HIGHLIGHTER_CHANGE_BUTTON_CURSOR", overrideObject: KEY );
		}

		//TEXTURE_STYLE

		internal static void ASD( IRepaint w, EditorSettingsAdapter.HighlighterSettings KEY, int pluginID )
		{



			using ( MainRoot.GRO( w ).UP( 0 ) )
			{
				//KEY.pluginName +
				Draw.TOG_TIT( "Common Settings:" );
				Draw.FIELD( KEY.pluginName + " highlighter opacity:", /*-*/"HIGHLIGHTER_COLOR_OPACITY", 0, 1, overrideObject: KEY );
				Draw.TOG( "Overlap left unity column:", /*-*/"HIGHLIGHTER_LEFT_OVERLAP", overrideObject: KEY );
				Draw.Sp( 5 );
				using ( MainRoot.GRO( w ).UP( 0 ) )
				{
					Draw.TOG( "Group broadcasted child:", /*-*/"HIGHLIGHTER_GROUPING_CHILD_MODE", overrideObject: KEY );
					// using ( MainRoot.ENABLE(w).USE(/*-*/"HIGHLIGHTER_GROUPING_CHILD_MODE", 0, inverce: true, overrideObject: KEY ) ) 
					using ( MainRoot.ENABLE( w ).USE(/*-*/"HIGHLIGHTER_GROUPING_CHILD_MODE", 0, overrideObject: KEY ) )
						Draw.FIELD( "Grouped texture grow size (beta):", /*-*/"HIGHLIGHTER_TEXTURE_GROW", 0, 16, overrideObject: KEY );
					using ( MainRoot.ENABLE( w ).USE(/*-*/"HIGHLIGHTER_GROUPING_CHILD_MODE", 0, overrideObject: KEY, inverce: true ) )
						Draw.FIELD( "Vertical each line padding:", /*-*/"HIGHLIGHTER_BGCOLOR_PADDING", 0, 16, overrideObject: KEY );
					Draw.Sp( 4 );
				}
			}

			Draw.Sp( 10 );

			using ( MainRoot.GRO( w ).UP( 0 ) )
			{


				Draw.TOG_TIT( "GameObjects Icons Style:" );
				Draw.TOG( "Draw shadow for icons:", /*-*/"HIGHLIGHTER_DRAW_ICONS_SHADOW", overrideObject: KEY );
				GUI.Label( Settings.Draw.R, "Custom icons location:" );
				Settings.Draw.TOOLBAR( new[] { "Next to 'Label'", "Next to 'Foldout'", "Align 'Left'" }, /*-*/"HIGHLIGHTER_CUSTOM_ICON_LOCATION", overrideObject: KEY );
				Draw.TOG( "Draw marker if custom icons assigned:", /*-*/"HIGHLIGHTER_DRAW_ICON_IF_CUSTOM_ASIGNED", overrideObject: KEY );
			}

			Draw.Sp( 10 );

			using ( MainRoot.GRO( w ).UP( 0 ) )
			{
				Draw.TOG_TIT( "GameObjects Background Style:" );
				Settings.Draw.TOOLBAR( new[] { "None'", "Box", "TextArea", "External" }, /*-*/"HIGHLIGHTER_TEXTURE_STYLE", overrideObject: KEY );

				using ( MainRoot.ENABLE( w ).USE(/*-*/"HIGHLIGHTER_TEXTURE_GUID_ALLOW", 0, overrideObject: KEY ) )
				{
					Texture2D newScript = MainRoot.p.HL_SET_G(pluginID).HIghlighterExternalTexture;
					try
					{
						var c = GUI.color;
						if ( !newScript ) GUI.color *= Color.red;
						GUILayout.BeginHorizontal();
						newScript = (Texture2D)EditorGUILayout.ObjectField( MainRoot.p.HL_SET_G( pluginID ).HIghlighterExternalTexture, typeof( Texture2D ), false );
						GUILayout.Space( 16 );
						GUILayout.EndHorizontal();
						GUI.color = c;
					}
					catch
					{
						newScript = MainRoot.p.HL_SET_G( pluginID ).HIghlighterExternalTexture;
					}
					if ( newScript != MainRoot.p.HL_SET_G( pluginID ).HIghlighterExternalTexture ) MainRoot.p.HL_SET_G( pluginID ).HIghlighterExternalTexture = newScript;
				}

				using ( MainRoot.ENABLE( w ).USE(/*-*/"HIGHLIGHTER_TEXTURE_BORDER_ALLOW", 0, overrideObject: KEY ) )
				{
					Draw.FIELD( "Texture borders:", /*-*/"HIGHLIGHTER_TEXTURE_BORDER", 0, 16, overrideObject: KEY );
					using ( MainRoot.ENABLE( w ).USE( "DO_FOLD_INVERSION_TOGGLE_ALLOW", 0, overrideObject: KEY ) )
					{
						Draw.TOG( "Auto inverse colors for fold buttons:", /*-*/"DO_FOLD_INVERSION", overrideObject: KEY );
						Draw.HELP( w, "If you applied background colors that match the color of the fold button, color for fold button will be inverted" );
					}

					Draw.Sp( 10 );
					Draw.HRx2();

					Draw.TOG( "Use special shader:", /*-*/"HIGHLIGHTER_USE_SPECUAL_SHADER", overrideObject: KEY );
					using ( MainRoot.ENABLE( w ).USE(/*-*/"HIGHLIGHTER_USE_SPECUAL_SHADER", 0, overrideObject: KEY ) )
					{
						GUI.Label( Draw.R, "Highlighter special drawing shader:" );
						Settings.Draw.TOOLBAR( new[] { "Normal", "Additive" }, /*-*/"HIGHLIGHTER_USE_SPECUAL_SHADER_TYPE", overrideObject: KEY );


						Shader newScript = MainRoot.p.HL_SET_G(pluginID).SHADER_A.ExternalShaderReference;
						GUILayout.BeginHorizontal();
						var rect = EditorGUILayout.GetControlRect();
						GUILayout.Space( 16 );
						GUILayout.EndHorizontal();
						rect.width /= 2;
						var og = GUI.enabled;
						GUI.enabled = og & MainRoot.p.HL_SET_G( pluginID ).HIGHLIGHTER_USE_SPECUAL_SHADER_TYPE == 0;

						try
						{
							newScript = (Shader)EditorGUI.ObjectField( rect, MainRoot.p.HL_SET_G( pluginID ).SHADER_A.ExternalShaderReference, typeof( Shader ), false );

						}

						catch
						{
							newScript = MainRoot.p.HL_SET_G( pluginID ).SHADER_A.ExternalShaderReference;
						}

						if ( newScript != MainRoot.p.HL_SET_G( pluginID ).SHADER_A.ExternalShaderReference )
						{
							MainRoot.p.HL_SET_G( pluginID ).SHADER_A.ExternalShaderReference = newScript;
						}

						GUI.enabled = og & MainRoot.p.HL_SET_G( pluginID ).HIGHLIGHTER_USE_SPECUAL_SHADER_TYPE == 1;
						newScript = MainRoot.p.HL_SET_G( pluginID ).SHADER_B.ExternalShaderReference;
						rect.x += rect.width;

						try
						{
							newScript = (Shader)EditorGUI.ObjectField( rect, MainRoot.p.HL_SET_G( pluginID ).SHADER_B.ExternalShaderReference, typeof( Shader ), false );
						}

						catch
						{
							newScript = MainRoot.p.HL_SET_G( pluginID ).SHADER_B.ExternalShaderReference;
						}

						GUI.enabled = og;

						if ( newScript != MainRoot.p.HL_SET_G( pluginID ).SHADER_B.ExternalShaderReference )
						{
							MainRoot.p.HL_SET_G( pluginID ).SHADER_B.ExternalShaderReference = newScript;
						}
					}
				}
			}



		}
	}
}















/*
internal static Rect DrawIconAligmentSettingsLine(Rect lineRect, PluginInstance adapter)
{


	var nv = adapter.TOOGLE_POP_INVERCE(ref lineRect, "Icons Placement", _S_bgIconsPlace, "Next to 'Label'", "Next to 'Foldout'", "Align 'Left'");
	if (nv != _S_bgIconsPlace)
	{
		_S_bgIconsPlace = nv;
		adapter.RepaintAllViews();
	}


	return lineRect;
}
internal static void DrawHoverPlaceSettingLine(PluginInstance adapter)
{

	lineRect.height *= 10;
	GUI.Label(Settings.Draw.R, "Open highlighter button location:");
#if UNITY_2018_3_OR_NEWER
	Settings.Draw.TOOLBAR(new[] { "<Left", "-Icon-", "<Left and -Icon-" }, "HIGHLIGHTER_HIERARCHY_BUTTON_LOCATION");
#else
			Settings.Draw.TOOLBAR(new[] { "<Left" }, "HIGHLIGHTER_HIERARCHY_BUTTON_LOCATION");
#endif
	GUI.Label(Settings.Draw.R, "Draw rect marker at HighLighter Button");
	Settings.Draw.TOOLBAR(new[] { "None", "Window Open Only", "Window and Hover" }, "HIGHLIGHTER_HIERARCHY_BUTTON_RECTMARKER");
}




void DrawDettings(Rect inputrect)
{
	inputrect.width -= 4;
	var lineRect = inputrect;
	//	lineRect.height = EditorGUIUtility.singleLineHeight;
	lineRect.x += 20;
	lineRect.width -= 40;

	GUILayout.BeginArea(lineRect);

	GUILayout.EndArea();

	adapter.InitializeStyles();
	EditorGUI.BeginChangeCheck();


	lineRect.y += lineRect.height;
	adapter.par.highligterOpacity = Mathf.Clamp(EditorGUI.FloatField(lineRect, "HighLighter Opacity:", adapter.par.highligterOpacity), 0, 1);
	lineRect.y += lineRect.height;
	adapter._S_BottomPaddingForBgColor = Mathf.Clamp(EditorGUI.IntField(lineRect, "Vertical Padding '1':", adapter._S_BottomPaddingForBgColor), 0, 16);
	lineRect.y += lineRect.height;

	var on2 = GUI.enabled;
	GUI.enabled = !adapter.IS_PROJECT();
	lineRect.y += lineRect.height;
	lineRect = DrawHoverPlaceSettingLine(lineRect, adapter);
	lineRect.y += (EditorGUIUtility.singleLineHeight);
	lineRect.y += (EditorGUIUtility.singleLineHeight);
	GUI.enabled = on2;

	lineRect.y += lineRect.height;
	adapter.ENABLE_RICH();
	adapter.par.COLOR_ICON_SIZE = Mathf.Clamp(EditorGUI.IntField(lineRect, "<b>Custom Icons</b> size '" + EditorGUIUtility.singleLineHeight + "'", adapter.par.COLOR_ICON_SIZE), 10, 30);
	adapter.DISABLE_RICH();
	lineRect.y += lineRect.height;
	adapter._S_USEdefaultIconSize = adapter.TOGGLE_LEFT(lineRect, "<i>Use Default Icons size:</i>", adapter._S_USEdefaultIconSize);
	lineRect.y += lineRect.height;
	var on = GUI.enabled;
	GUI.enabled &= adapter._S_USEdefaultIconSize;
	adapter.ENABLE_RICH();
	adapter._S_defaultIconSize = Mathf.Clamp(EditorGUI.IntField(lineRect, "<b>Defaul Iconst</b> size '" + EditorGUIUtility.singleLineHeight + "'", adapter._S_defaultIconSize), 10, 30);
	adapter.DISABLE_RICH();
	GUI.enabled = on;

	lineRect.y += lineRect.height;
	lineRect = DrawIconAligmentSettingsLine(lineRect, adapter);

	lineRect.y += lineRect.height;




	on = GUI.enabled;
	GUI.enabled = HAS_LABEL_ICON();
	var nv = adapter.TOOGLE_POP(ref lineRect, "Draw yellow marks next to the assigned icons", adapter.par.BottomParams.DRAW_FOLDER_STARMARK ? 1 : 0, "Disable", "Enable") == 1;
	if (nv != adapter.par.BottomParams.DRAW_FOLDER_STARMARK)
	{
		adapter.par.BottomParams.DRAW_FOLDER_STARMARK = nv;
		adapter.RepaintAllViews();
	}

	GUI.enabled = on;

	lineRect.y += (EditorGUIUtility.singleLineHeight);
	lineRect.y += (EditorGUIUtility.singleLineHeight);



	if (adapter.IS_HIERARCHY())
	{

		EditorGUI.BeginChangeCheck();
		lineRect.y += lineRect.height;
		adapter.par.SHOW_NULLS = adapter.TOGGLE_LEFT(lineRect, "Show Locator for Object without Component", adapter.par.SHOW_NULLS);

		lineRect.y += lineRect.height;
		adapter.par.SHOW_PREFAB_ICON = adapter.TOGGLE_LEFT(lineRect, "Show Prefab icon", adapter.par.SHOW_PREFAB_ICON);

		lineRect.y += lineRect.height;
		adapter.par.SHOW_MISSINGCOMPONENTS = adapter.TOGGLE_LEFT(lineRect, "Show Warning if Object has missing Component", adapter.par.SHOW_MISSINGCOMPONENTS);
		if (EditorGUI.EndChangeCheck())
		{
			if (adapter.OnClearObjects != null) adapter.OnClearObjects();
		}
	}



	if (EditorGUI.EndChangeCheck())
	{
		adapter.SavePrefs();
		adapter.RepaintAllViews();
	}


}*/
