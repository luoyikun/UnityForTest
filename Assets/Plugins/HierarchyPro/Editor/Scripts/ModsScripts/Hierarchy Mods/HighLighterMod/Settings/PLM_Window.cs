using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class PLM_Window : ScriptableObject
	{
	}


	[CustomEditor(typeof(PLM_Window))]
	class SETGUI_HL_ProjectManual : MainRoot
	{
		internal static string set_text =  USE_STR + "HighLighter for Project";
		internal static string set_key = "USE_PROJECT_MANUAL_HIGHLIGHTER_MOD";
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
			Draw.RESET(w);

			Draw.BACK_BUTTON(w);
			Draw.TOG_TIT(set_text,WIKI: WIKI_3_M_HIGH);
			Draw.Sp(10);





            using ( GRO(w).UP( 0 ) )
            {

              
                using ( ENABLE(w).USE( (Draw.TOG_TIT( "Use " + SETGUI_HL_ProjectAuto.set_text, SETGUI_HL_ProjectAuto.set_key, rightOffset: 1 )) ) ) { }
                //if ( Draw.BUT( Draw.last, HighlighterAutoModSettingsEditor.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<HLA_Window>(); }
                Draw.HELP_TEXTURE(Draw.last, "HL_B" );
				using ( ENABLE( w ).USE( (Draw.TOG_TIT( "Use Manual Highlighter", set_key, rightOffset: 1, EnableRed: false )) ) ) { }
				//if ( Draw.BUT( Draw.last, HighlighterManualModSettingsEditor.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<HLM_Window>(); }
				Draw.HELP_TEXTURE( Draw.last, "HL_A" );
			}
            Draw.Sp( 10 );


			using ( ENABLE( w ).USE( set_key, SETGUI_HL_ProjectAuto.set_key, CLASS_ENALBE.operation.OR ) )
			{
				SETGUI_HL_HierarchyManual.BUTTONPLACE( w, p.par_e.PROJ_HIGH_SET, 1 );
			}

			Draw.Sp( 10 );

			//using ( ENABLE(w).USE(set_key))
			using ( ENABLE(w).USE( set_key, SETGUI_HL_ProjectAuto.set_key, CLASS_ENALBE.operation.OR ) )
            {



                SETGUI_HL_HierarchyManual.ASD(w, p.par_e.PROJ_HIGH_SET, 1);



			}


            Draw.Sp( 10 );

            using ( GRO(w).UP( 0 ) )
            {

                // Draw.TOG_TIT( "" + LEFT + " Area:" );

                Draw.TOG_TIT( "Quick tips:" );
					Draw.HELP( w, "To open left drown down window, LMB on the little button to the left of the object name (by default).", drawTog: true );
				//Draw.HELP( w, "Use special button to open highlighter for selected objects.", drawTog: true );
                //Draw.HELP_TEXTURE(w, "HELP_HIGHLIGHTER", 0 );

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
