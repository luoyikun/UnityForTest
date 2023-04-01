using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EMX.HierarchyPlugin.Editor.Settings;
using EMX.HierarchyPlugin.Editor.Mods;


namespace EMX.HierarchyPlugin.Editor.Windows
{
	public partial class Root_HighlighterWindowInterface : Windows.IWindow
	{









		internal static HighlighterMod mod { get { return adapter.pluginID == 0 ? Root.p[ 0 ].modsController.highLighterMod : Root.p[ 0 ].modsController.projectWindowExtensions.highLighterMod; } }

		static float WH303 = 450 + singleLineHeight / 2;
		internal static void Init( MousePos rect, string title, Action<Texture, string> _SetIconImage, Texture _GetTexture, Action<TempColorClass, string> _SetHiglightColor
								   , Func<TempColorClass> _GetHiglightColor
								   , Action<Color32, string> _SetIconColor, Func<Color32?> _GetIconColor, HierarchyObject _source, Window a ) //, PluginInstance inadapter, HighlighterMod mod
		{
			localIdInFIle_to_instanceId.Clear();

			adapter = a;
			rect.Height = WH303;
			if ( rect.type != MousePos.Type.Highlighter_410_0 ) throw new Exception();

			HighlighterCache_Icons.Init_InternatlDefault_IconsList();

			if ( _GetTexture == null )
			{
				current = "";
			}
			else if ( AssetDatabase.GetAssetPath( _GetTexture ).StartsWith( "Library/", StringComparison.OrdinalIgnoreCase ) )
			{
				current = _GetTexture.name;
			}
			else
			{
				current = "GUID=" + AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( _GetTexture ) );
			}
			WriteLastColor();
			//titleWin = title;
			source = _source;
			SetIconImage = _SetIconImage;
			GetHiglightData = _GetHiglightColor;
			__SetHiglightData = _SetHiglightColor;
			HighLighterCommonData.SetLastTempColor( GetHiglightData() );

			GetIconColor = _GetIconColor;
			__SetIconColor = _SetIconColor;

			var hw = CURRENT_WINDOW = Windows.IWindow.private_Init(rect, typeof(Root_HighlighterWindowInterface), a, title, useAnim: true) as Root_HighlighterWindowInterface;
			//	Root_HighlighterWindowInterface.mod = mod;
			repaintW = hw;
			repaintW.SET_NEW_HEIGHT( a, rect.Height );

			if ( !uws )
			{
				uws = true;
				UnityEditor.Undo.undoRedoPerformed -= repaint;
				UnityEditor.Undo.undoRedoPerformed += repaint;
			}

			ClearCacheAndRepaint();

		}
		internal static Root_HighlighterWindowInterface CURRENT_WINDOW;

		protected override void OnDestroy()
		{
			WriteLastColor();
			OnDestroySwitcher();
			source = null;
			base.OnDestroy();
		}




		new static Window adapter = null;
		// static PluginInstance static_adapter { get { return adapter; } }
		static Root_HighlighterWindowInterface repaintW;
		static bool uws = false;
		static void repaint()
		{
			if ( repaintW )
			{
				cache = null;
				repaintW.Repaint();
			}
		}
		//Change Icons Color
		// "Set Icon"
		internal static bool Enable { get { return repaintW; } }

		static string current;

		// static string titleWin;
		internal static HierarchyObject source;
		static Action<Texture, string> SetIconImage;
		static List<string> allow = new List<string>() { "ObjectSelector", "ColorPicker", "InputWindow" };
		//EditorWindow pinOverride;
		bool __wasLoasFocus = false;
		bool wasLoasFocus {
			get {
				return __wasLoasFocus;
			}
			set {
				__wasLoasFocus = value;
			}
		}


		internal override bool OnLostFocusPIN {
			get {

				if ( Resources.FindObjectsOfTypeAll( typeof( EditorWindow ) ).Any( w => allow.Any( l => w.GetType().Name.Contains( l ) ) ) )
				{
					wasLoasFocus = false;
					return true;
				}
				// Debug.Log( EditorWindow.focusedWindow );

				if ( EditorWindow.focusedWindow == null ) return true;

				//Debug.Log( EditorWindow.focusedWindow.GetType().Name );

				//MonoBehaviour.print(EditorWindow.focusedWindow.GetType().Name);
				if ( allow.Any( l => EditorWindow.focusedWindow.GetType().Name.Contains( l ) ) )
				{
					wasLoasFocus = true;
					fixFocus = false;
					return true;
				}
				if ( wasLoasFocus && this )
				{
					wasLoasFocus = false;
					this.Focus();
					return true;
				}
				return false;
			}
		}
		internal override bool PIN {
			get {



				if ( EditorWindow.focusedWindow == this )
				{
					wasLoasFocus = false;
					return true;
				}
				if ( EditorWindow.focusedWindow == null ) return true;
				//MonoBehaviour.print(EditorWindow.focusedWindow.GetType().Name);
				if ( allow.Any( l => EditorWindow.focusedWindow.GetType().Name.Contains( l ) ) )
				{
					wasLoasFocus = true;
					fixFocus = false;
					return true;
				}
				if ( wasLoasFocus && this )
				{
					wasLoasFocus = false;
					this.Focus();
					return true;
				}
				/*   if (pinOverride == null)
				   {
					   pinOverride = Resources.FindObjectsOfTypeAll<EditorWindow>().FirstOrDefault(w => allow.Any(l => w.GetType().Name.Contains(l)));
				   }
				   if (pinOverride != null)
				   {
					   return EditorWindow.focusedWindow == (EditorWindow)pinOverride;
				   }*/
				//  return true;
				/* if (/*Resources.FindObjectsOfTypeAll(typeof(EditorWindow)).Any(w => allow.Any(l => w.GetType().Name.Contains(l)) && (EditorWindow.focusedWindow == (EditorWindow)w) ||#1#
					 EditorWindow.focusedWindow == this || pinOverride != null && allow.Any(l => pinOverride.GetType().Name.Contains(l)))
				 {
					 // MonoBehaviour.print("ASD");
					 return true;
				 }*/
				return m_PIN;

			}
			set { m_PIN = value; }
		}
		static void REPAINT_ALL_HIERW()
		{
			//adapter.RepaintWindowInUpdate();
			Root.p[ 0 ].RepaintWindowInUpdate_PlusResetStack( adapter.pluginID );
			foreach ( var w in __inputWindow ) if ( w.Value ) w.Value.Repaint();
		}


		protected override void Update()     //  MonoBehaviour.print(GUIUtility.GetControlID(FocusType.Keyboard));
		{   //  MonoBehaviour.print(GUIUtility.GetControlID(FocusType.Passive));
			//  MonoBehaviour.print(Resources.FindObjectsOfTypeAll(typeof(EditorWindow)).Select(w => w.GetType().Name).Aggregate((a, b) => a + " " + b));

			if ( wasLoasFocus && !fixFocus && EditorWindow.focusedWindow == this )
			{
			
					//CloseThis();
					//return;
			}
			if ( wasLoasFocus && !fixFocus )
			{
				
					if ( !EditorWindow.focusedWindow && this )
					{
						this.Focus();
						wasLoasFocus = false;
					}

				/*if ( allow.All( l => !EditorWindow.focusedWindow.GetType().Name.Contains( l ) ))
				{
					{   CloseThis();
						return ;
					}
				}*/
			}


			if ( !PIN && EditorWindow.focusedWindow != this ) { 
				CloseThis(); 
			}

			if ( LostFocus )
			{
				if ( EditorWindow.focusedWindow && EditorWindow.focusedWindow.GetType().Name == "IconData" )
				{
					CloseThis();
					return;
				}
				LostFocus = false;
			}


			base.Update();
		}
		GUIContent emptyContent = new GUIContent();
		GUIContent nullContent = new GUIContent();


		//static GUIStyle HIGHLIGHTER_COLOR_FG, HIGHLIGHTER_COLOR_DECORATION, HIGHLIGHTER_COLOR_PICKER;
		/*static bool stylesWasInit = false;

		static void InitStyles()
		{
			if (adapter == null) adapter = Initializator.AdaptersByName.First().Value;

			stylesWasInit = true;

			HIGHLIGHTER_COLOR_FG = adapter.InitializeStyle("HIGHLIGHTER_COLOR_FG", 0.5f);
			HIGHLIGHTER_COLOR_DECORATION = adapter.InitializeStyle("HIGHLIGHTER_COLOR_DECORATION", 0.5f);
			HIGHLIGHTER_COLOR_PICKER = adapter.InitializeStyle("HIGHLIGHTER_COLOR_PICKER", 0.5f);


		}*/
		static void _labelRich( Rect r, GUIContent s, GUIStyle style = null )
		{
			var ra = label.richText;
			label.richText = true;
			if ( style == null ) GUI.Label( r, s, label );
			else GUI.Label( r, s, style );
			label.richText = ra;
		}
		static void _labelRich( Rect r, string s )
		{
			var ra = label.richText;
			label.richText = true;
			GUI.Label( r, s, label );
			label.richText = ra;
		}
		new static void Label( Rect r, string s, TextAnchor an )
		{
			var a = label.alignment;
			label.alignment = an;
			_labelRich( r, s );
			label.alignment = a;
		}
		static void Label( Rect r, GUIContent s, GUIStyle style )
		{
			var a = label.alignment;
			_labelRich( r, s, style );
			label.alignment = a;
		}
		new static void Label( Rect r, string s )
		{
			_labelRich( r, s );
		}
		new static void Label( Rect r, GUIContent s )
		{
			_labelRich( r, s );
		}

		static bool _buttonRich( Rect r, string s )
		{
			var ra = Root.p[0].STYLE_DEFBUTTON.richText;
			Root.p[ 0 ].STYLE_DEFBUTTON.richText = true;
			var res = GUI.Button(r, s, Root.p[0].STYLE_DEFBUTTON);
			Root.p[ 0 ].STYLE_DEFBUTTON.richText = ra;
			return res;
		}
		static bool _buttonRich( Rect r, GUIContent s )
		{
			var ra = Root.p[0].STYLE_DEFBUTTON.richText;
			Root.p[ 0 ].STYLE_DEFBUTTON.richText = true;
			var res = GUI.Button(r, s, Root.p[0].STYLE_DEFBUTTON);
			Root.p[ 0 ].STYLE_DEFBUTTON.richText = ra;
			return res;
		}
		new static bool Button( Rect r, string s )
		{
			return _buttonRich( r, s );
		}
		new static bool Button( Rect r, string s, TextAnchor an )
		{
			var a = button.alignment;
			button.alignment = an;
			var res = _buttonRich(r, s);
			button.alignment = a;
			return res;
		}
		new static bool Button( Rect r, GUIContent s )
		{
			return _buttonRich( r, s );
		}
		new static bool Button( Rect r, GUIContent s, TextAnchor an )
		{
			var a = button.alignment;
			button.alignment = an;
			var res = _buttonRich(r, s);
			button.alignment = a;
			return res;

		}
		//         static   TextAnchor label_a;
		//         static   TextAnchor button_a;
		//         static  Texture2D[] button_t = new Texture2D[4];
		//         static  Texture2D[][] button_t2x = new Texture2D[4][];
		static bool changedgui = false;
		static internal GUIStyle __label;
		static internal GUIStyle label {
			get {
				if ( __label == null ) { 
					//__label = Root.p[ 0 ].label;
					__label = new GUIStyle( Root.p[ 0 ].label );
					__label.clipping = TextClipping.Clip;
					__label.alignment = TextAnchor.MiddleLeft;
				}
				__label.fontSize = Root.p[ 0 ].FONT_10();
				return __label;
			}
		}

		static internal GUIStyle __button;
		static internal GUIStyle button {
			get {
				if ( __button == null ) {
					__button = new GUIStyle( Root.p[ 0 ].button );
					__button.clipping = TextClipping.Clip;
					__button.alignment = TextAnchor.MiddleLeft;
				}
				__button.fontSize = Root.p[ 0 ].FONT_10();
				return __button;
			}
		}
		//static GUIStyle changed_button, changed_label;

		static void DrawTexture( Rect rect, Texture2D t, Color c )
		{
			var c2 = GUI.color;
			GUI.color *= c;
			GUI.DrawTexture( rect, t );
			GUI.color = c2;
		}


		//internal static void CHANGE_GUI() { CHANGE_GUI(adapter); }
		internal static void CHANGE_GUI()
		{
			if ( changedgui )
			{
				RESTORE_GUI();
				// throw new Exception( "Cannot change highlighter gui" );
			}
			changedgui = true;
			// Hierarchy.ChangeGUI();

			//if ( changed_button == null )
			//{
			//	changed_button = new GUIStyle( Root.p[ 0 ].STYLE_DEFBUTTON );
			//	changed_button.alignment = TextAnchor.MiddleLeft;
			//}
			//if ( changed_label == null )
			//{
			//	changed_label = new GUIStyle( Root.p[ 0 ].label );
			//	changed_label.alignment = TextAnchor.MiddleLeft;
			//}
			//__button = changed_button;
			//__label = changed_label;
		}
		internal static void RESTORE_GUI()
		{
			if ( !changedgui )     // throw new Exception( "highlighter gui not changed" );
			{
			}
			changedgui = false;
			//Hierarchy.RestoreGUI();
			/*  GET_SKIN().label.alignment = label_a;
			  GET_SKIN().button.alignment = button_a;
			  GET_SKIN().button.normal.background = button_t[0];
			  GET_SKIN().button.normal.scaledBackgrounds = button_t2x[0];
			  GET_SKIN().button.hover.background = button_t[1];
			  GET_SKIN().button.hover.scaledBackgrounds = button_t2x[1];
			  GET_SKIN().button.active.background = button_t[2];
			  GET_SKIN().button.active.scaledBackgrounds = button_t2x[2];
			  GET_SKIN().button.focused.background = button_t[3];
			  GET_SKIN().button.focused.scaledBackgrounds = button_t2x[3];*/
			//__button = Root.p[ 0 ].button;
			//__label = Root.p[ 0 ].label;
		}

		bool LostFocus = false;
		internal override void OnLostFocus()
		{
			return;
			//if ( EditorWindow.focusedWindow && EditorWindow.focusedWindow.GetType().Name == "IconData" )
			//{
			//	LostFocus = true;
			//}
			////skipFixFocus = false;
			//if ( pickerId != -1 )
			//{
			//	//skipFixFocus = true;
			//}
			//base.OnLostFocus();
		}


		static void TryToClose()
		{
			if ( Event.current != null && Event.current.control && repaintW ) repaintW.CloseThis();
		}

		/*  float width;
		  float w10;*/
		//static Color32 newCol;
		bool fixFocus = false;
		float LINE_H { get { return singleLineHeight * 1.5f; } }
		bool skipDown = false;
		protected override void OnGUI()
		{
			if ( wasLoasFocus && EditorWindow.focusedWindow == this )
			{
				fixFocus = true;
				skipDown = true;
				/*  Debug.Log(Event.current.mousePosition);
				  if (Event.current.mousePosition.x <= 0 || Event.current.mousePosition.y <= 0 || Event.current.mousePosition.x >= position.width || Event.current.mousePosition.y >= position.height)
				  {   CloseThis();
					  return;
				  }*/
			}
			if ( EditorWindow.focusedWindow == this )
			{
				if ( skipDown && Event.current.type == EventType.MouseDown ) Tools.EventUseFast();
				if ( Event.current.type == EventType.Repaint ) skipDown = false;
			}

			if ( _inputWindow == null ) return;

			if ( source == null || adapter == null )
			{
				CloseThis();
				return;
			}

			if ( !source.Validate() )
			{
				CloseThis();
				return;
			}

			base.OnGUI();

			//	if (!stylesWasInit) InitStyles();

			CHANGE_GUI();


			var PPP = 5;


			var LABEL_HEIGHT = singleLineHeight; /* GET_SKIN().label.fontSize;*/
			Rect FULL_RECT = new Rect(PPP, PPP, _inputWindow.position.width - PPP * 2 + 5, LABEL_HEIGHT * 2);


			//string[] cats = null;
			/*  if (adapter.IS_HIERARCHY()) cats = new [] { "HighLighter",  "HL - Templates", "HL - Settings", "Comps Presets" };
			  else cats = new [] { "HighLighter", "HL - Templates", "HL - Settings"};☰*/
			//	if (adapter.IS_HIERARCHY()) 
			//else cats = new[] { "SINGLE MODE", "AUTO MODE", "" };

			var colors = new[] { new Color32(244, 67, 54, 255), new Color32(25, 118, 210, 255), new Color32(76, 175, 80, 255), new Color32(255, 158, 34, 255) };
			string[] cats = new[] { "MANUAL", "AUTO", "", "☰PRESETS" };
			if ( !mod.set.USE_MANUAL_HIGHLIGHTER_MOD || mod.NowPrefabIsOpened ) { cats[ 0 ] = ""; colors[ 0 ] = Color.clear; }
			if ( !mod.set.USE_AUTO_HIGHLIGHTER_MOD ) { cats[ 1 ] = ""; colors[ 1 ] = Color.clear; }
			if ( !mod.set.USE_CUSTOM_PRESETS_MOD || mod.pluginID == 1 ) { cats[ 3 ] = ""; colors[ 3 ] = Color.clear; }


			var ov = SessionState.GetInt("EMX|HighlighterCat", 0);
			ov = Mathf.Clamp( ov, 0, cats.Length - 1 );
			while ( ov >= 0 && string.IsNullOrEmpty( cats[ ov ] ) ) ov--;
			if ( ov == -1 ) ov = cats.ToList().FindIndex( s => !string.IsNullOrEmpty( s ) );
			if ( ov == -1 ) ov = 0;
			var toolBarRect = new Rect(FULL_RECT.x, FULL_RECT.y, FULL_RECT.width - 5, FULL_RECT.height);
			var TBM = 2;

			var news = UnityVersion.UNITY_CURRENT_VERSION >= UnityVersion.UNITY_2019_3_0_VERSION;
			var style = news ? GUI.skin.button : EditorStyles.toolbarButton;
			var nv = ov;
			if ( !news )
			{
				var oh = EditorStyles.toolbarButton.fixedHeight;
				EditorStyles.toolbarButton.fixedHeight *= TBM;
				nv = GUI.Toolbar( toolBarRect, ov, cats, style );
				if ( nv == 2 ) nv = 0;
				//if (adapter.IS_HIERARCHY())
				{
					var q = toolBarRect;
					q.width /= 4;
					q.x += q.width * 2;
					GUI.Box( q, "" );
				}
				EditorStyles.toolbarButton.fixedHeight = oh;
				if ( ov != nv )
				{
					SessionState.SetInt( "EMX|HighlighterCat", nv );
				}
				toolBarRect.height = EditorStyles.toolbarButton.fixedHeight * TBM;
			}

			nv = Mathf.Clamp( nv, 0, cats.Length - 1 );
			if ( string.IsNullOrEmpty( cats[ nv ] ) ) nv = cats.ToList().FindIndex( c => !string.IsNullOrEmpty( cats[ nv ] ) );
			if ( nv == -1 )
			{
				RESTORE_GUI();
				CloseThis();
				return;
			}
			//nv = Mathf.Clamp(nv, 0, cats.Length - 1);

			toolBarRect.width /= cats.Length;
			for ( int i = 0; i < cats.Length; i++ )     //if (i == nv) colors[i].a /= (byte)(EditorGUIUtility.isProSkin ? 3 : 12);
			{

				if ( /*adapter.IS_HIERARCHY() && */i == 2 || string.IsNullOrEmpty( cats[ i ] ) )
				{



					//  float w;
					// GUI.skin.button.CalcMinMaxWidth( new GUIContent( "clear" ), out w, out w );
					//  if (GUI.Button( new Rect( toolBarRect.x + toolBarRect .width / 2 - w / 2, toolBarRect.y + toolBarRect .height / 2 - EditorGUIUtility.singleLineHeight / 2, w, EditorGUIUtility.singleLineHeight
					/* if (GUI.Button( new Rect( toolBarRect.x, toolBarRect.y + toolBarRect .height / 2 - EditorGUIUtility.singleLineHeight / 2, toolBarRect.width, EditorGUIUtility.singleLineHeight
											 ),
									 "clear", adapter.STYLE_DEFBUTTON ) )
					 {

					 }*/




					toolBarRect.x += toolBarRect.width;
					continue;
				}


				if ( news )
				{
					var fh = EditorStyles.toolbarButton.fixedHeight;
					var aa = EditorStyles.toolbarButton.alignment;
					EditorStyles.toolbarButton.fixedHeight = 0;
					EditorStyles.toolbarButton.alignment = TextAnchor.MiddleCenter;
					var res = GUI.Button(toolBarRect, cats[i], EditorStyles.toolbarButton);
					EditorStyles.toolbarButton.fixedHeight = fh;
					EditorStyles.toolbarButton.alignment = aa;
					if ( res )
					{
						nv = i;
						SessionState.SetInt( "EMX|HighlighterCat", nv );
					}
				}


				/* if ( i == nv )
				 {   if ( EditorGUIUtility.isProSkin )
					 {
					 }
					 else
					 {   colors[i].a /= (byte)(EditorGUIUtility.isProSkin ? 3 : 12);
					 }
				 }*/
				/* if ( !EditorGUIUtility.isProSkin )
				 {   EditorGUI.DrawRect( Shrink( toolBarRect, 0 ), colors[i] );
				 }
				 else*/
				{
					if ( i == nv )     //EditorGUI.DrawRect(Shrink(toolBarRect, 0), colors[i]);
					{
						//colors[ i ].r = (byte)(255 - (255 - colors[ i ].r) / 2);
						//colors[ i ].g = (byte)(255 - (255 - colors[ i ].g) / 2);
						//colors[ i ].b = (byte)(255 - (255 - colors[ i ].b) / 2);
						//colors[ i ].a /= 2;
						//EditorGUI.DrawRect( Shrink( toolBarRect, 5 ), colors[ i ] );

						var g  = Shrink( toolBarRect, -1 );
						var s = g;
						g.height = 2;
						EditorGUI.DrawRect( g, colors[ i ] );
						g = s;
						g.y += g.height;
						g.height = 2;
						g.y -= g.height;
						EditorGUI.DrawRect( g, colors[ i ] );
						g = s;
						g.width = 2;
						EditorGUI.DrawRect( g, colors[ i ] );
						g = s;
						g.x += g.width;
						g.width = 2;
						g.x -= g.width;
						EditorGUI.DrawRect( g, colors[ i ] );
					}
					//else
					{
						colors[ i ].a /= 1;
						var R = Shrink(toolBarRect, 5);
						R.height /= 8;
						EditorGUI.DrawRect( R, colors[ i ] );
						/*     R.y += R.height * 5;
						EditorGUI.DrawRect(R, colors[i]);
						 R.height /= 2;
						 R.y += R.height / 2;
						 if (Event.current.type == EventType.Repaint) EditorStyles.toolbarButton.Draw(R, emptyContent, 0);*/
					}
				}
				/* var casd = GUI.skin.label.normal.textColor;
				 var al = GUI.skin.label.alignment;
				 var fs = GUI.skin.label.fontSize;
				 // GUI.skin.label.normal.textColor = Color.black;
				 GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				 GUI.skin.label.fontSize = EditorStyles.toolbarButton.fontSize ;
				 Label(toolBarRect, cats[i]);
				 GUI.skin.label.normal.textColor = casd;
				 GUI.skin.label.alignment = al;
				 GUI.skin.label.fontSize = fs;*/

				toolBarRect.x += toolBarRect.width;
			}

			var OFF = toolBarRect.height + singleLineHeight / 2;
			FULL_RECT.y += OFF;
			FULL_RECT.height = WH303 - OFF;


			//	if (!adapter.IS_HIERARCHY() && nv == 3) nv = 0;

			if ( nv == 0 )     //   var PRESET_STRING = "Highlighter";
			{
				/*	if (Application.isPlaying)
					{
						var PRESET_STRING = " (Warning! Changes Will Lost after Stop Playing)";
						Label(FULL_RECT, PRESET_STRING);
						FULL_RECT.y += FULL_RECT.height;
					}*/



				if ( !Root.p[ 0 ].par_e.ENABLE_ALL )
				{
					Label( FULL_RECT, "Plugin disabled!" );
				}
				else
				{
					var wasGui = GUI.enabled;
					//	GUI.enabled = adapter.par.USE_HIGLIGHT;



					FULL_RECT.y = DrawHiglighter( FULL_RECT );
					//GUI.enabled = wasGui;
				}

				// FULL_RECT.y += FULL_RECT.height;
				//   FULL_RECT.height += singleLineHeight;
				FULL_RECT.height = singleLineHeight;
				HighLighterStyle.LABEL( FULL_RECT, "Icon:" );
				FULL_RECT.y += FULL_RECT.height - singleLineHeight;
				FULL_RECT.height = 147;

				HighLighterStyle.HR( FULL_RECT, 15 );
				//FULL_RECT.y = WH303 - FULL_RECT.height ;
				DRAWICON( FULL_RECT );


				/*///////////////////*/
				/**   ICON    **/
				// FULL_RECT.y += FULL_RECT.height;
				// FULL_RECT.height = LABEL_HEIGHT;

				//PRESET_STRING = adapter.IS_HIERARCHY() ? "Icon of GameObject" : "Icon";
				// if (Application.isPlaying) {PRESET_STRING += " (Warning! Changes Will Lost after Stop Playing)";
				// Label( FULL_RECT, PRESET_STRING );

				//
				//

				/**   ICON    **/
				/*///////////////////*/
			}


			if ( nv == 2 )    // DrawDettings(FULL_RECT);
			{
			}

			if ( nv == 1 )
			{
				var oy = FULL_RECT.y;
				var oh = FULL_RECT.height;
				FULL_RECT.height = singleLineHeight;
				FULL_RECT.width -= 4;

				//	EditorGUI.BeginChangeCheck();

				//	adapter._S_autorFiltersEnable = adapter.TOGGLE_LEFT(FULL_RECT, "<i>Enable Auto Apply Mode:</i>", adapter._S_autorFiltersEnable);
				//	TOOLTIP(FULL_RECT, "Customized filters will be automatically applied to objects. You can disable auto mode and to use this window as a set of pre-configured highlighter styles list.");
				//	FULL_RECT.y += FULL_RECT.height;

				/*	if (EditorGUI.EndChangeCheck())
					{
						adapter.SavePrefs();
						adapter.RepaintAllViews();
					}*/

				//HR////////////////////
				FULL_RECT.y += 3;
				FULL_RECT.height = 1;
				Root.p[ 0 ].INTERNAL_BOX( FULL_RECT );
				//HR////////////////////
				//  FULL_RECT.height = EditorGUIUtility.singleLineHeight;
				//  LABEL(FULL_RECT, "Highlighter Templates:");
				FULL_RECT.y += FULL_RECT.height + 5;
				FULL_RECT.height = oh - (FULL_RECT.y - oy);

				FULL_RECT.width += 4;
				DrawFilts( FULL_RECT );
				FULL_RECT.width -= 4;

				//HR////////////////////
				FULL_RECT.y += FULL_RECT.height + 3;
				FULL_RECT.height = 1;
				//HR////////////////////
				Root.p[ 0 ].INTERNAL_BOX( FULL_RECT );
			}
			/*///////////////////*/
			/**   PRESETS MANAGER    **/
			if ( nv == 3 )
			{

				//var wasGui2 = GUI.enabled;
				//GUI.enabled = adapter._S_PresetsEnabled;
				/* var PRESET_STRING = "Manager of Presets for Design";
				 if (!adapter._S_PresetsEnabled) PRESET_STRING += " (Disabled)";
				 Label( FULL_RECT, PRESET_STRING );
				 if (Application.isPlaying) Label( new Rect( FULL_RECT.x, FULL_RECT.y + FULL_RECT.height * 0.75f, FULL_RECT.width, FULL_RECT.height ), "(Changes Persist after Stop Playing)" );
				SETUPROOT.pretr = FULL_RECT;
				// RESTORE_GUI();
				SETUPROOT.DRAW_WIKI_BUTTON( adapter.pluginname, "Left Panel", "Presets Manager" );  */
				// CHANGE_GUI();

				//	GUI.enabled = wasGui2;


				// FULL_RECT.y += FULL_RECT.height;
				FULL_RECT.height = 23 + 50 + 2;


				DRAWPRESETS( FULL_RECT );
				//else Label(FULL_RECT, "Presets disabled");
			}





			/*  var wasGui2 = GUI.enabled;
			  GUI.enabled = Hierarchy.par.PresetManagerParams.ENABLE;*/
			// GUI.enabled = wasGui2;

			/**   PRESETS MANAGER    **/
			/*///////////////////*/







			if ( Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Escape || Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return) )
			{
				Tools.EventUseFast();
				CloseThis();
				Root.p[ 0 ].SKIP_PREFAB_ESCAPE = true;
			}
			/*if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
			{   // comformAction(textInput);
				EventUseFast();
				CloseThis();
			}*/



			RESTORE_GUI();

		}



		protected internal override bool CloseThis()
		{
			WriteLastColor();
			HighLighterCommonData.SetDirty();
			base.CloseThis();
			repaintW = null;
			if ( adapter != null ) Root.p[ 0 ].RepaintWindow( adapter.pluginID );
			return true;
		}

		static Color32? lastColor;
		static Color32? lastTextColor;
		void UpdateLastHiglightColors( Color32 new32 )
		{
			REPAINT_ALL_HIERW();
			// InternalEditorUtility.RepaintAllViews();
			lastColor = new32;
		}
		void UpdateLastHiglightTextColors( Color32 new32 )
		{
			REPAINT_ALL_HIERW();
			// InternalEditorUtility.RepaintAllViews();
			lastTextColor = new32;
		}
		static void WriteLastColor()
		{
			if ( lastColor.HasValue )
			{
				if ( HighLighterCommonData.GetBackGroundColorsHistory().Count != 0 )
				{
					HighLighterCommonData.GetBackGroundColorsHistory().RemoveAll( t => t.r == lastColor.Value.r && t.g == lastColor.Value.g && t.b == lastColor.Value.b );
					if ( HighLighterCommonData.GetBackGroundColorsHistory().Count == 0 ) HighLighterCommonData.GetBackGroundColorsHistory().Add( lastColor.Value );
					else HighLighterCommonData.GetBackGroundColorsHistory().Insert( 0, lastColor.Value );
					lastColor = null;
					while ( HighLighterCommonData.GetBackGroundColorsHistory().Count > 20 ) HighLighterCommonData.GetBackGroundColorsHistory().RemoveAt( 20 );
				}
			}
			//  if (Hierarchy_GUI.GetLastHiglightList == null) return;
			if ( lastTextColor.HasValue )
			{
				if ( HighLighterCommonData.GetTextColorsHistory().Count != 0 )
				//  {   Hierarchy_GUI.GetLastHiglightTextList( adapter ).RemoveAll( t => t.Equals( lastTextColor.Value ) );
				{
					HighLighterCommonData.GetTextColorsHistory().RemoveAll( t => t.r == lastTextColor.Value.r && t.g == lastTextColor.Value.g && t.b == lastTextColor.Value.b );
					if ( HighLighterCommonData.GetTextColorsHistory().Count == 0 ) HighLighterCommonData.GetTextColorsHistory().Add( lastTextColor.Value );
					else HighLighterCommonData.GetTextColorsHistory().Insert( 0, lastTextColor.Value );
					lastTextColor = null;
					while ( HighLighterCommonData.GetTextColorsHistory().Count > 20 ) HighLighterCommonData.GetTextColorsHistory().RemoveAt( 20 );
				}
			}

		}

		static GUIContent ecc = new GUIContent();

		internal static Color PICKER( Rect inputrect, GUIContent content, Color color, bool DRAW_REPAINT = true )       /*55x23*/
		{
			if ( GUI.enabled ) EditorGUIUtility.AddCursorRect( inputrect, MouseCursor.Link );

			//	if (!stylesWasInit) InitStyles();

			var result = GUI.enabled ? Draw.COLOR(new Rect(inputrect.x + 1, inputrect.y + 1, inputrect.width - 2, inputrect.height - 2), ecc, color) : color;

			if ( DRAW_REPAINT )
			{
				/*if (Event.current.type == EventType.Repaint)
				{
					var a = GUI.color;
					if (!GUI.enabled) GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.4f);
					HIGHLIGHTER_COLOR_DECORATION.Draw(inputrect, ecc, 0);
					GUI.color = a;
					GUI.color *= SettingsBGColor;
					HIGHLIGHTER_COLOR_FG.Draw(inputrect, ecc, 0);
					GUI.color = a;
					HIGHLIGHTER_COLOR_PICKER.Draw(new Rect(inputrect.x + inputrect.width - 3 - 17, inputrect.y + 3, 17, 17), ecc, 0);
				}*/
				Label( inputrect, content );
			}

			return result;
		}





		Color black = new Color(.2f, .2f, .2f, 1f);
		// Color white = new Color(.5f, .5f, .5f, 1f);
		// Color white = new Color(.5f, .5f, .5f, 1f);

		/*   internal void DrawStroke(Rect R, string text, Color selCol)
		   {   var c = GET_SKIN().label.normal.textColor;
			   //var cds = GET_SKIN().label.fontStyle;
			   GET_SKIN().label.normal.textColor = EditorGUIUtility.isProSkin ? black : white;
			   GET_SKIN().label.normal.textColor = new Color( GET_SKIN().label.normal.textColor.r, GET_SKIN().label.normal.textColor.g, GET_SKIN().label.normal.textColor.b, 0.2f );
			   // GET_SKIN().label.fontStyle = FontStyle.Bold;
			   var outline = 1f;
			   R.x -= outline;
			   R.y -= outline;
			   Label( R, text );
			   R.x += outline;
			   Label( R, text );
			   R.x += outline;
			   Label( R, text );
			   R.y += outline;
			   Label( R, text );
			   R.y += outline;
			   Label( R, text );
			   R.x -= outline;
			   Label( R, text );
			   R.x -= outline;
			   Label( R, text );
			   R.y -= outline;
			   Label( R, text );
			   //GET_SKIN().label.fontStyle = cds;

			   selCol.r = 1 - selCol.r;
			   selCol.g = 1 - selCol.g;
			   selCol.b = 1 - selCol.b;
			   var mid = (selCol.r + selCol.g + selCol.b) / 3;
			   var sat = 0.7f;
			   selCol.r = Mathf.Lerp( selCol.r, mid, sat );
			   selCol.g = Mathf.Lerp( selCol.g, mid, sat );
			   selCol.b = Mathf.Lerp( selCol.b, mid, sat );
			   selCol.a = 1;
			   GET_SKIN().label.normal.textColor = selCol;
			   Label( R, text );

			   GET_SKIN().label.normal.textColor = c;
		   }*/


		int conrollerID;
		private static Action<TempColorClass, string> __SetHiglightData;
		private static void SetHiglightData( TempColorClass a, string b )
		{
			HighLighterCommonData.SetLastTempColor( a );
			__SetHiglightData( a, b );
			TryToClose();

		}
		private static Func<TempColorClass> GetHiglightData;
		private static Action<Color32, string> __SetIconColor;
		private static Func<Color32?> GetIconColor;


		static void SetIconColor( Color32 newCol, string undoString )
		{
			var new32 = (Color32)newCol;
			//	if (new32.a == 0 && (new32.r != 0 || new32.g != 0 || new32.b != 0)) new32.a = 255;
			__SetIconColor( new32, undoString );
		}


		/*
			   // if (!wasFocus)

						var wasGui = GUI.enabled;
						GUI.enabled = Hierarchy.par.USE_HIGLIGHT;

						Label("Highlighter Color (Experimental)");
						GUI.BeginHorizontal();
						//   MonoBehaviour.print(EditorGUIUtility.GetControlID(FocusType.Keyboard));
						//  MonoBehaviour.print(EditorGUIUtility.hotControl);
						//PIN = true;

						var single = GetColor();
						var c = single.list;
						var color = new Color32((byte)c[0], (byte)c[1], (byte)c[2], (byte)c[3]);
						var child = c[4] == 1;
						//if (c.a == 0 || c.r == 0 && c.g == 0 && c.b == 0)
						Hierarchy.RestoreGUI();
						var newc = (Color32)EditorGUILayout.ColorField(color);
						if (newc.a == 0 && (newc.r != 0 || newc.g != 0 || newc.b != 0)) newc.a = 255;
						Hierarchy.ChangeGUI();
						// MonoBehaviour.print(EditorGUIUtility.GetObjectPickerObject());

						if (GUI.Button("X"))
						{
							SetColor(new IntList() { list = new List<int>() { 0, 0, 0, 0, 0 } });
							CloseThis();
						}
						GUI.EndHorizontal();
						var newchild = EditorGUILayout.ToggleLeft("Apply to childes", child);

						if (!color.Equals(newc) || child != newchild)
						{
							single.list[0] = newc.r;
							single.list[1] = newc.g;
							single.list[2] = newc.b;
							single.list[3] = newc.a;
							single.list[4] = newchild ? 1 : 0;
							SetColor(single);
						}

						// if (!wasFocus)
						if (Event.current.keyCode == KeyCode.Escape)
						{
							CloseThis();
						}
						if (Event.current.keyCode == KeyCode.Return)
						{
							// comformAction(textInput);
							CloseThis();
						}

						GUI.enabled = wasGui;*/
	}
}
