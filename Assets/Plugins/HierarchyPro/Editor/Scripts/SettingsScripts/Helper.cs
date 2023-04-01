//#define LOG_FILE_COPY

using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	public interface IRepaint
	{
		void Repaint();
		int ID();
		int? currentWidth();
	}

	class Draw
	{

		static Dictionary<string, FIELD_SETTER> _setterCache_null = new Dictionary<string, FIELD_SETTER>();
		static Dictionary<object, Dictionary<string, FIELD_SETTER>> _setterCache_dic = new Dictionary<object, Dictionary<string, FIELD_SETTER>>();
		internal static FIELD_SETTER GetSetter( string KEY, /*PluginInstance A, bool UsePar = true, */Action<FIELD_SETTER> ac = null, Func<FIELD_SETTER, bool> valudate = null, object overrideObject = null )
		{

			var _setterCache = _setterCache_null;
			if ( overrideObject != null )
			{
				if ( !_setterCache_dic.ContainsKey( overrideObject ) ) _setterCache_dic.Add( overrideObject, new Dictionary<string, FIELD_SETTER>() );
				_setterCache = _setterCache_dic[ overrideObject ];
			}


			if ( !_setterCache.ContainsKey( KEY ) )
			{
				// var type = UsePar ? A.par_e.GetType() : A.GetType();
				var type = (overrideObject ?? Root.p[0].par_e).GetType();
				var p = type.GetProperty(KEY, ~(BindingFlags.InvokeMethod | BindingFlags.Static));
				var res = new FIELD_SETTER(KEY) { overrideObject = overrideObject };

				if ( p != null )
				{
					res.isprop = true;
					res.prop = p;
				}

				else
				{
					var f = type.GetField(KEY, ~(BindingFlags.InvokeMethod | BindingFlags.Static));
					res.field = f;
				}

				if ( res.field == null && res.prop == null ) throw new Exception( KEY + " field not found\n" );

				if ( ac != null ) res.onChanged = ac;
				if ( valudate != null ) res.onValidateChange = valudate;

				// res.UsePar = UsePar;
				_setterCache.Add( KEY, res );
			}

			_setterCache[ KEY ].A = Root.p[ 0 ];
			return _setterCache[ KEY ];
		}



		static GUIStyle _b;

		//internal static void BACK_BUTTON_()
		//{
		//	var C = GUI.color;
		//	GUI.color = C * Color.Lerp( new Color32( 255, 150, 150, 255 ), Color.white, 0 );
		//	if ( _b == null )
		//	{
		//		_b = new GUIStyle( GUI.skin.button );
		//		_b.alignment = TextAnchor.MiddleLeft;
		//	}
		//	_b.fontSize = GUI.skin.button.fontSize;
		//	if ( Draw.BUT( "<--  Back To Settings List", _b ) ) { MainSettingsEnabler_Window.Select<MainSettingsEnabler_Window>(skipOpenWindow: true); }
		//	GUI.color = C;
		//	HIGLIGHT_BUTTON();
		//	Draw.Sp( 10 );
		//}
		static bool wasP = false;

		internal static void BACK_BUTTON( MainRoot w )
		{
			BACK_BUTTON( (IRepaint)w );
		}
		internal static void BACK_BUTTON( IRepaint w )
		{
			if ( !wasP )
			{
				w.Repaint();
				wasP = true;
			}
			var C = GUI.color;
			GUI.color = C * Color.Lerp( new Color32( 255, 150, 150, 255 ), Color.white, 0 );
			if ( _b == null )
			{
				_b = new GUIStyle( GUI.skin.button );
				_b.alignment = TextAnchor.MiddleLeft;
			}
			_b.fontSize = GUI.skin.button.fontSize;

			var r = R15;
			r.x += 40;
			r.width -= 80;

			if ( w is MainRoot )
			{

				//var oldW = r.width;
				//r.width *= 0.7f;

				if ( Draw.BUT_S( r, "<--  Back To Settings List", _b ) ) { MainSettingsEnabler_Window.Select<MainSettingsEnabler_Window>( true, skipOpenWindow: true ); }
				GUI.color = C;

				//r.x += r.width;
				//r.width = oldW - r.width;
				//// if ( Draw.BUT_S( r, "Show it in Project", _b ) )
				//var en = GUI.enabled;
				//GUI.enabled &= MainSettingsEnabler_Window.lastSettings;
				//if ( Draw.BUT_S( r, "Select in Project", _b ) )
				//{
				//	UnityEditor.EditorUtility.FocusProjectWindow();
				//	Selection.objects = new[] { MainSettingsEnabler_Window.lastSettings };
				//	UnityEditor.ProjectWindowUtil.ShowCreatedAsset( MainSettingsEnabler_Window.lastSettings );
				//}
				//GUI.enabled = en;
			}
			else
			{
				/*var en = GUI.enabled;
				GUI.enabled &= MainSettingsEnabler_Window.lastSettings != null;
				if ( Draw.BUT_S( r, "Open this settings in Inspector", _b ) )
				{
					UnityEditor.EditorUtility.FocusProjectWindow();
					Selection.objects = new[] { MainSettingsEnabler_Window.lastSettingsGET };
					UnityEditor.ProjectWindowUtil.ShowCreatedAsset( MainSettingsEnabler_Window.lastSettingsGET );
				}
				GUI.enabled = en;*/
				GUI.color = C;
			}


			Draw.Sp( 10 );
		}
		internal static void HIGLIGHT_BUTTON()
		{
			//var r = R15;
			//r.x += 40;
			//r.width -= 80;
			//
			//var C = GUI.color;
			//var en = GUI.enabled;
			//GUI.enabled &= MainSettingsEnabler_Window.lastSettings;
			//GUI.color = C * Color.Lerp( new Color32( 255, 150, 150, 255 ), Color.white, 0 );
			//if ( _b == null )
			//{
			//	_b = new GUIStyle( GUI.skin.button );
			//	_b.alignment = TextAnchor.MiddleLeft;
			//}
			//_b.fontSize = GUI.skin.button.fontSize;
			////if ( Draw.BUT( "Show it in Project", _b ) )
			//if ( Draw.BUT( "Select in Project", _b ) )
			//{
			//	Selection.objects = new[] { MainSettingsEnabler_Window.lastSettings };
			//	UnityEditor.ProjectWindowUtil.ShowCreatedAsset( MainSettingsEnabler_Window.lastSettings );
			//}
			//
			//GUI.color = C;
			//GUI.enabled = en;
			////Draw.Sp( 10 );
		}

		[NonSerialized]
		internal static int groupIndex = 0;
		[NonSerialized]
		internal static float padding = 0;

		//static EventType? _lastResetEvent = null;
		internal static int CurrentId;
		internal static void RESET( IRepaint ir )
		{
			currentViewWidth = ir.currentWidth() ?? (EditorGUIUtility.currentViewWidth - 16);
			CurrentId = EditorGUIUtility.GetControlID( FocusType.Passive );
			padding = 0;
			//Debug.Log(  );
			// if ( _lastResetEvent == Event.current.type ) return;
			//_lastResetEvent = Event.current.type;
			groupIndex = 0;
		}
		static  float currentViewWidth;
		static Rect _getRerct( GUILayoutOption gUILayoutOption = null )
		{
			var res = gUILayoutOption != null ? EditorGUILayout.GetControlRect(gUILayoutOption) : EditorGUILayout.GetControlRect();
			res.x = 0;
			res.width = currentViewWidth;
			res.x += padding;
			res.width -= Math.Min( 20, padding ) + padding;
			return last = res;
		}
		static Rect PEEK_NEW_WIDHT()
		{
			Rect res = Rect.zero;
			res.x = 0;
			res.width = currentViewWidth;
			res.x += padding;
			res.width -= Math.Min( 20, padding ) + padding;
			return res;
		}
		static float CALC_PADDING { get { return padding + Math.Min( 20, padding ); } }
		static GUIContent ec = new GUIContent();
		internal static Rect R05 { get { return _getRerct( GUILayout.Height( Mathf.RoundToInt( EditorGUIUtility.singleLineHeight * 0.2f ) ) ); } }
		internal static Rect R15 { get { return _getRerct( GUILayout.Height( Mathf.RoundToInt( EditorGUIUtility.singleLineHeight * 1.5f ) ) ); } }
		internal static Rect R { get { return _getRerct(); } }
		internal static Rect R2 { get { return _getRerct( GUILayout.Height( EditorGUIUtility.singleLineHeight * 2 ) ); } }
		internal static Rect RH( float h ) { return _getRerct( GUILayout.Height( h ) ); }
		internal static Rect RH( float h, int shrink, int shrink2 )
		{
			var r = _getRerct( GUILayout.Height( h ) );
			r.x += shrink;
			r.width -= shrink2;
			return r;
		}
		internal static Rect last;
		internal static Rect lastPlus {
			get {
				var r = last;
				r.x += 16;
				return r;
			}
		}
		static bool hover { get { return last.Contains( Event.current.mousePosition ); } }
		static bool press { get { return Event.current.button == 0 && Event.current.isMouse; } }
		internal static Rect Sp( float sp )
		{
			//  GUILayout.Space( sp );
			return _getRerct( GUILayout.Height( sp ) );
			//GUILayout.Space( sp );
		}
		static Dictionary<string, GUIStyle> _styles = new Dictionary<string, GUIStyle>();
		static Type t;
		internal static GUIStyle s( string style )
		{
			if ( _styles.ContainsKey( style ) ) return _styles[ style ];
			if ( t == null )
			{
				t = typeof( EditorWindow ).Assembly.GetType( "UnityEditor.InspectorWindow+Styles" );
				if ( t == null )
				{
					t = typeof( EditorWindow ).Assembly.GetType( "UnityEditor.PropertyEditor+Styles" );
					//if (t == null) throw new Exception("ASD");
				}
			}
			if ( t == null )
				_styles.Add( style, EditorStyles.toggle );
			else
			{
				var l = new GUIStyle(t.GetField(style, ~BindingFlags.Instance).GetValue(null) as GUIStyle);
				if ( style == "addComponentArea" )
				{
					l.fixedHeight = 0;
					l.stretchHeight = true;
					l.padding.left = 16 + 32;
				}
				else if ( style == "addComponentButtonStyle" )
				{
					l = new GUIStyle( EditorStyles.toolbarButton );
					l.fixedWidth = 0;
					l.stretchWidth = true;
					l.fixedHeight = 0;
					l.stretchHeight = true;
				}
				else if ( style == "preToolbar" )
				{
					l.fixedWidth = 0;
					l.stretchWidth = true;
					l.fixedHeight = 0;
					l.stretchHeight = true;
				}
				else
				{
					l.padding.left = 16;
				}
				l.fixedWidth = 0;
				l.stretchWidth = true;
				l.fixedHeight = 0;
				l.stretchHeight = true;
				l.alignment = TextAnchor.MiddleLeft;
				_styles.Add( style, l );
			}
			return _styles[ style ];
		}

		const string WIKI_LINK = "\n - Open online documentation page in browser\nLink also will be copied to text os buffer";

		internal static bool TOG_TIT( string text, string KEY, Func<FIELD_SETTER, bool> valudate = null, Action<FIELD_SETTER> onChanged = null, float rightOffset = 0,
			Rect? rov = null, object overrideObject = null, string AreYouSureText = null, bool? EnableRed = null, string WIKI = null )
		{
			//   var setter = GetSetter( KEY );
			var setter = valudate == null ? Draw.GetSetter(KEY, overrideObject: overrideObject) : Draw.GetSetter(KEY, onChanged, valudate: valudate, overrideObject: overrideObject);
			var en = setter.AS_BOOL;
			var oldP = Draw.padding;
			Draw.padding = 0;
			if ( rov.HasValue ) last = rov.Value;
			var r = rov ?? R2;
			if ( rightOffset != 0/* && setter.AS_BOOL*/)
			{
				r.width -= last.height;
				last.width -= last.height;
			}
			var wiki_rect = r;
			if ( WIKI != null )
			{
				var S = r.height * 4;
				if ( Event.current.type != EventType.Repaint ) r.width -= S;
				wiki_rect.x = wiki_rect.x + wiki_rect.width;
				if ( Event.current.type == EventType.Repaint ) wiki_rect.x -= S;
				wiki_rect.width = S;
				Root.SetMouseTooltip( "[ " + text + " ]" + WIKI_LINK, wiki_rect );
			}

			DRAW_RESET_TO_DEFAULT( r, setter );


			Root.SetMouseTooltip( CONT( text ), r, Event.current );


			var aaa = GUI.color;
			// if ( rightOffset != 0 && !setter.AS_BOOL ) GUI.color = new Color( 1, 1, 1, 0.5f );
			if ( GUI.Button( r, CONT( text ), s( "addComponentArea" ) ) )
			{
				if ( AreYouSureText == null || EditorUtility.DisplayDialog( AreYouSureText, AreYouSureText, "Yes", "Cancel" ) ) setter.AS_BOOL = !setter.AS_BOOL;
			}
			GUI.color = aaa;
			if ( !EditorGUIUtility.isProSkin ) EditorGUI.DrawRect( r, new Color32( 50, 50, 50, 50 ) );

			if ( Event.current.type == EventType.Repaint )
			{
				var qwe = lastPlus;
#if !UNITY_2019_4_OR_NEWER
				qwe.y += qwe.height / 4;
#endif
				if ( en && Root.p[ 0 ].par_e.USE_COLORS_FOR_CATEGORIES )
				{
					var bc = GUI.backgroundColor;
					GUI.backgroundColor = SettingsScreen.dad;

					EditorStyles.toggle.Draw( qwe, ec, hover, false, en, false );
					GUI.backgroundColor = bc;
				}
				else
				{
					EditorStyles.toggle.Draw( qwe, ec, hover, false, en, false );
				}
				//EditorStyles.toggle.Draw( lastPlus, ec, hover, false, en, false );
			}
			Draw.padding = oldP;
			if ( rightOffset == 0 && EnableRed != false || EnableRed == true )
			{
				r.height = 5;
				r.y -= r.height;
				//EditorGUI.DrawRect( r, new Color32( 255, 50, 50, 50 ) );
				var dad = SettingsScreen.dad;
				dad.a = 50;
				EditorGUI.DrawRect( r, dad );

			}

			if ( WIKI != null )
			{
				var cont = CONT( "-->wiki?", "[ " +Folders.WIKI_PAGE + WIKI + " ]" + WIKI_LINK );
				Root.SetMouseTooltip( cont.tooltip, wiki_rect );
				if ( GUI.Button( (Rect)wiki_rect, cont, EditorStyles.toolbarButton ) )
				{
					Application.OpenURL( Folders.WIKI_PAGE + WIKI );
					EditorGUIUtility.systemCopyBuffer = Folders.WIKI_PAGE + WIKI;
				}
			}

			return setter.AS_BOOL;
		}
		internal static void TOG_TIT( string text, float rightOffset = 0, bool? EnableRed = null, string WIKI = null )
		{
			TOG_TIT( CONT( text ), rightOffset, EnableRed, WIKI: WIKI );
		}
		internal static void TOG_TIT( GUIContent text, float rightOffset = 0, bool? EnableRed = null, string WIKI = null )
		{
			//   var setter = GetSetter( KEY );
			var oldP = Draw.padding;
			Draw.padding = 0;
			var r = R2;
			if ( rightOffset != 0 )
			{
				r.width -= last.height;
				last.width -= last.height;
			}
			var wiki_rect = r;
			if ( WIKI != null )
			{
				var S = r.height * 4;
				if ( Event.current.type != EventType.Repaint ) r.width -= S;
				wiki_rect.x = wiki_rect.x + wiki_rect.width;
				if ( Event.current.type == EventType.Repaint ) wiki_rect.x -= S;
				wiki_rect.width = S;
				Root.SetMouseTooltip( "[ " +text + " ]" + WIKI_LINK , wiki_rect );
			}


			Root.SetMouseTooltip( text, r, Event.current );



			if ( GUI.Button( r, text, s( "addComponentArea" ) ) ) { };//setter.AS_BOOL = !setter.AS_BOOL;
			if ( !EditorGUIUtility.isProSkin ) EditorGUI.DrawRect( r, new Color32( 50, 50, 50, 50 ) );
			//  if (Event.current.type == EventType.Repaint) EditorStyles.toggle.Draw(lastPlus, ec, hover, false, en, false);

			var br = r;


			if ( rightOffset == 0 && EnableRed != false || EnableRed == true )
			{
				r.height = 5;
				r.y -= r.height;
				//EditorGUI.DrawRect( r, new Color32( 255, 50, 50, 50 ) );
				var dad = SettingsScreen.dad;
				dad.a = 50;
				EditorGUI.DrawRect( r, dad );
			}

			if ( WIKI != null )
			{
				var cont = CONT( "-->wiki?", "[ " + Folders.WIKI_PAGE + WIKI + " ]" + WIKI_LINK );
				Root.SetMouseTooltip( cont.tooltip, wiki_rect );
				if ( GUI.Button( (Rect)wiki_rect, cont, EditorStyles.toolbarButton ) )
				{
					Application.OpenURL( Folders.WIKI_PAGE + WIKI );
					EditorGUIUtility.systemCopyBuffer = Folders.WIKI_PAGE + WIKI;
				}
			}

			Draw.padding = oldP;
		}
		internal static bool BUT( Rect r, string text )
		{

			if ( text.StartsWith( "Use " ) ) text = text.Substring( 4 );
			r.x += r.width;
			r.width = r.height;

			if ( GUI.Button( r, Draw.CONT( "-->", text ) ) ) return true;
			return false;
		}
		internal static bool BUT_S( Rect r, string text, GUIStyle st )
		{
			if ( text.StartsWith( "Use " ) ) text = text.Substring( 4 );
			if ( st != null )
			{
				if ( GUI.Button( r, Draw.CONT( text ), st ) ) return true;
			}
			else
			{
				if ( GUI.Button( r, Draw.CONT( text ) ) ) return true;
			}
			return false;
		}
		internal static bool BUT( string text, GUIStyle st = null )
		{
			var r = R15;
			r.x += 40;
			r.width -= 80;
			if ( st != null )
			{
				if ( GUI.Button( r, Draw.CONT( text ), st ) ) return true;
			}
			else
			{
				if ( GUI.Button( r, Draw.CONT( text ) ) ) return true;
			}
			return false;
		}
		internal static bool TOG_TIT( Rect r, string text, bool val )
		{

			if ( GUI.Button( r, CONT( text ), s( "addComponentArea" ) ) ) val = !val;
			if ( !EditorGUIUtility.isProSkin ) EditorGUI.DrawRect( r, new Color32( 50, 50, 50, 50 ) );
			r.x += 16;
			if ( Event.current.type == EventType.Repaint )
			{
				if ( val && Root.p[ 0 ].par_e.USE_COLORS_FOR_CATEGORIES )
				{
					var bc = GUI.backgroundColor;
					GUI.backgroundColor = SettingsScreen.dad;
#if !UNITY_2019_4_OR_NEWER
					r.y += r.height / 4;
#endif
					EditorStyles.toggle.Draw( r, ec, r.Contains( Event.current.mousePosition ), false, val, false );
					GUI.backgroundColor = bc;
				}
				else
				{
#if !UNITY_2019_4_OR_NEWER
					r.y += r.height / 4;
#endif
					EditorStyles.toggle.Draw( r, ec, r.Contains( Event.current.mousePosition ), false, val, false );
				}
				//EditorStyles.toggle.Draw( r, ec, r.Contains( Event.current.mousePosition ), false, val, false );
			}
			return val;
		}
		internal static bool TOG( Rect r, string text, bool val, bool EnableRed = false )
		{
			/*  if (GUI.Button(r, CONT(text), s("addComponentArea"))) val = !val;
              r.x += 16;
              if (Event.current.type == EventType.Repaint) EditorStyles.toggle.Draw(r, ec, r.Contains(Event.current.mousePosition), false, val, false);*/
			Root.SetMouseTooltip( CONT( text ), r, Event.current );

			if ( val && Root.p[ 0 ].par_e.USE_COLORS_FOR_CATEGORIES )
			{
				var bc = GUI.backgroundColor;
				GUI.backgroundColor = SettingsScreen.dad;
				val = EditorGUI.ToggleLeft( r, CONT( text ), val );
				GUI.backgroundColor = bc;
			}
			else
			{
				val = EditorGUI.ToggleLeft( r, CONT( text ), val );
			}


			//val = EditorGUI.ToggleLeft( r, CONT( text ), val );
			return val;
		}
		internal static bool TOG( Rect r, GUIContent text, bool val, bool EnableRed = false )
		{
			Root.SetMouseTooltip( text, r, Event.current );
			val = EditorGUI.ToggleLeft( r, (text), val );
			return val;
		}
		internal static void TOG( GUIContent text, string KEY, Func<FIELD_SETTER, bool> valudate = null, Action<FIELD_SETTER> onChanged = null, Rect? rov = null, object overrideObject = null, bool EnableRed = false )
		{
			var setter = valudate == null ? Draw.GetSetter(KEY, overrideObject: overrideObject) : Draw.GetSetter(KEY, onChanged, valudate: valudate, overrideObject: overrideObject);
			var r = rov ?? R;
			DRAW_RESET_TO_DEFAULT( r, setter );

			var res =setter.AS_BOOL;

			Root.SetMouseTooltip( text, r, Event.current );

			if ( res && Root.p[ 0 ].par_e.USE_COLORS_FOR_CATEGORIES )
			{
				var bc = GUI.backgroundColor;
				GUI.backgroundColor = SettingsScreen.dad;
				res = EditorGUI.ToggleLeft( r, (text), res );
				GUI.backgroundColor = bc;
			}
			else
			{
				res = EditorGUI.ToggleLeft( r, (text), res );
			}
			//if ( res && Root.p[ 0 ].par_e.USE_COLORS_FOR_CATEGORIES )
			//{
			//	var c= SettingsScreen.dad;
			//	c.a = 100;
			//	//EditorGUI.DrawRect( r, c );
			//}

			//res = EditorGUI.ToggleLeft(r, (text), res);
			if ( res != setter.AS_BOOL ) setter.AS_BOOL = res;
			return;
		}
		static Color temp_beta_color;
		internal static void TOG( string text, string KEY, Func<FIELD_SETTER, bool> valudate = null, Action<FIELD_SETTER> onChanged = null, Rect? rov = null, object overrideObject = null, bool EnableRed = false )
		{
			var setter = valudate == null ? Draw.GetSetter(KEY, overrideObject: overrideObject) : Draw.GetSetter(KEY, onChanged, valudate: valudate, overrideObject: overrideObject);

			var r = rov ?? R;
			DRAW_RESET_TO_DEFAULT( r, setter );

			//if ( Draw.padding > 20 )
			{


				Root.SetMouseTooltip( CONT( text ), r, Event.current );

				var res =setter.AS_BOOL;

				if ( res && (text.Contains( "(beta)" ) || text.Contains( "(bugs)" )) )
				{
					temp_beta_color = GUI.color;
					GUI.color *= new Color32( 255, 80, 40, 255 );
				}

				if ( res && Root.p[ 0 ].par_e.USE_COLORS_FOR_CATEGORIES )
				{
					var bc = GUI.backgroundColor;
					GUI.backgroundColor = SettingsScreen.dad;
					res = EditorGUI.ToggleLeft( r, CONT( text ), setter.AS_BOOL );
					GUI.backgroundColor = bc;
				}
				else
				{
					res = EditorGUI.ToggleLeft( r, CONT( text ), setter.AS_BOOL );
				}

				if ( res && (text.Contains( "(beta)" ) || text.Contains( "(bugs)" )) )
				{
					GUI.color = temp_beta_color;
				}


				//if ( res && Root.p[ 0 ].par_e.USE_COLORS_FOR_CATEGORIES )
				//{
				//	var c= SettingsScreen.dad;
				//	c.a = 100;
				//	//EditorGUI.DrawRect( r, c );
				//}

				//var res = EditorGUI.ToggleLeft(r, CONT(text), setter.AS_BOOL);
				if ( res != setter.AS_BOOL ) setter.AS_BOOL = res;
				//if ( GUI.Button( R, text, EditorStyles.toggle ) ) setter.AS_BOOL = !setter.AS_BOOL;
				// if ( Event.current.type == EventType.Repaint ) EditorStyles.toggle.Draw( lastPlus, ec, hover, false, setter.AS_BOOL, false );
				return;
			}

			/*   var en = setter.AS_BOOL;
               var lr = R15;
               var rr = last;
               //  rr.width *= 0.25f;
               // lr.width *= 0.75f;
               rr.width = 70;
               lr.width -= rr.width;
               rr.x += lr.width;
               var c =GUI.color;
               GUI.color = new Color( 0, 0, 0, 0 );
               if ( GUI.Button( lr, Draw.CONT( text ), s( "addComponentButtonStyle" ) ) ) { setter.AS_BOOL = true; }
               if ( GUI.Button( rr, Draw.CONT( "Off" ), s( "addComponentButtonStyle" ) ) ) { setter.AS_BOOL = false; }
               if ( Event.current.type == EventType.Repaint )
               {
                   GUI.color = c;
                   // if ( !en && !lr.Contains( Event.current.mousePosition ) ) GUI.color = c * new Color( 1, 1, 1, 0.4f );
                   //  else GUI.color = c;
                   s( "addComponentButtonStyle" ).Draw( lr, text, lr.Contains( Event.current.mousePosition ), en, false, false );
                   //  if ( en && !rr.Contains( Event.current.mousePosition ) ) GUI.color = c * new Color( 1, 1, 1, 0.4f );
                   //  else GUI.color = c;
                   s( "addComponentButtonStyle" ).Draw( rr, "Off", rr.Contains( Event.current.mousePosition ), !en, false, false );
               }
               GUI.color = c;*/
		}
		//static GUIStyle tb_empty;
		static GUIStyle tb;
		internal static Rect? lastActiveToolBarRect;
		internal static void TOOLBAR( string[] textArray, string KEY, float height = -1, object overrideObject = null,
			Rect? rect = null, string[] tooltips = null, bool[] enabled = null, bool disableResetToDefault = false )
		{
			var setter = GetSetter(KEY, overrideObject: overrideObject);

			int b;
			if ( setter.type == typeof( bool ) ) b = setter.AS_BOOL ? 1 : 0;
			else b = (int)setter.value;//+ (int) stack[i].offset;

			var _R = rect ?? (height == -1 ? Draw.R15 : Draw.RH(height));

			if ( !disableResetToDefault ) DRAW_RESET_TO_DEFAULT( _R, setter );


			if ( tb == null )
			{
				tb = new GUIStyle( EditorStyles.toolbarButton );
				tb.fixedWidth = 0;
				tb.stretchWidth = true;
				tb.stretchHeight = true;
			}
			tb.fixedHeight = _R.height;

			var r = _R;
			var realLeng = 0;
			for ( int i = 0; i < textArray.Length; i++ )
				if ( enabled != null && i < enabled.Length && enabled[ i ] ) realLeng++;
			if ( enabled == null ) realLeng = textArray.Length;
			r.width /= realLeng;
			var newb = b;
			lastActiveToolBarRect = null;
			//var selected = setter.value
			for ( int i = 0; i < textArray.Length; i++ )
			{

				int controlId = GUIUtility.GetControlID(FocusType.Passive, r);
				var cont = tooltips == null ? Draw.CONT(textArray[i]) : Draw.CONT(textArray[i], tooltips[i]);
				var en = GUI.enabled;
				Root.SetMouseTooltip( cont, r, Event.current );
				if ( enabled != null && i < enabled.Length ) GUI.enabled &= enabled[ i ];
				if ( GUI.enabled )
				{
					if ( GUI.Button( r, cont ) ) newb = i;
					if ( Event.current.type == EventType.Repaint && GUI.enabled )
					{
						if ( i == b && Root.p[ 0 ].par_e.USE_COLORS_FOR_CATEGORIES )
						{
							var bc = GUI.backgroundColor;
							GUI.backgroundColor = SettingsScreen.dad;
							tb.Draw( r, cont, controlId, i == b, r.Contains( Event.current.mousePosition ) );
							GUI.backgroundColor = bc;
						}
						else
						{
							tb.Draw( r, cont, controlId, i == b, r.Contains( Event.current.mousePosition ) );
						}
						if ( i == b && Root.p[ 0 ].par_e.USE_COLORS_FOR_CATEGORIES )
						{
							var c= SettingsScreen.dad;
							c.a = 100;
							EditorGUI.DrawRect( r, c );
						}
					}
					if ( i == b ) lastActiveToolBarRect = r;

					r.x += r.width;
				}
				GUI.enabled = en;

			}

			//var newb = GUI.Toolbar(_R, b, textArray.Select(Draw.CONT).ToArray(), tb, GUI.ToolbarButtonSize.Fixed);// EditorStyles.miniButton);
			// EditorGUIUtility.AddCursorRect( _R, MouseCursor.Link );

			if ( b != newb )
			{
				var res = newb;//- (int) stack[i].offset;
				/*  if ( stack[ i ].conform == null || stack[ i ].conform( res ) )
                  {
                      stack[ i ].setter.value = res;
                      ValueChanged();
                  }*/
				if ( setter.type == typeof( bool ) ) setter.value = res != 0;
				else setter.value = res;
			}

		}
		static char[] trim = new char[] { ':',' ' };
		internal static void FIELD( string text, string KEY, float min, float max, string postfix = null, object overrideObject = null )
		{

			//var setter = GetSetter(KEY, overrideObject: overrideObject);

			var _R = Draw.R;
			FIELD_R( _R, text, KEY, min, max, postfix, overrideObject );
		}
		internal static void FIELD(Rect _R, string text, string KEY, float min, float max, string postfix = null, object overrideObject = null)
		{

			//var setter = GetSetter(KEY, overrideObject: overrideObject);

			FIELD_R(_R, text, KEY, min, max, postfix, overrideObject);
		}


		static void DRAW_RESET_TO_DEFAULT( Rect _R, FIELD_SETTER setter )
		{
			if ( Event.current != null && Event.current.type == EventType.MouseDown && Event.current.button == 1 && _R.Contains( Event.current.mousePosition ) )
			{
				//Event.current.Use();
				//EditorGUIUtility.hotControl = -1;

				//Root.p[ 0 ].PUSH_UPDATE_ONESHOT( 0, () => {
				var menu = new GenericMenu();
				menu.AddItem( new GUIContent( "Reset value to default" ), false, () => {
					setter.REMOVE();

				} );
				menu.ShowAsContext();
				//} );
			}
		}

		internal static void FIELD_R( Rect _R, string text, string KEY, float min, float max, string postfix = null, object overrideObject = null, float lablelOffset = 0 )
		{

			FIELD_SETTER setter = GetSetter(KEY, overrideObject: overrideObject);

			// var _R = Draw.R;
			var _r = _R;

			if ( lablelOffset == 0 )
			{
				if ( _R.width < 160 * 1.5f )
				{
					_R.width /= 1.5f;
				}
				else
				{
					_R.width -= 80;
				}
			}
			else _R.width = lablelOffset;


			//if ( lablelOffset == 0 ) _R.width /= 2f;
			//else _R.width = lablelOffset;
			if ( lablelOffset != 0 )
			{
				var al = GUI.skin.label.alignment;
				GUI.skin.label.alignment = TextAnchor.MiddleRight;
				GUI.Label( _R, Draw.CONT( text.Trim( trim ) ) );
				GUI.skin.label.alignment = al;
			}
			else
			{
				GUI.Label( _R, Draw.CONT( text.Trim( trim ) + ":" ) );
			}

			DRAW_RESET_TO_DEFAULT( _R, setter );


			//_R.x += _R.width;
			//_R.width = _r.width - _R.width;
			_R.width = _r.width;

			var F = setter.type == typeof(float);
			var FX = setter.type == typeof(float?);
			// var I = setter.type == typeof(int);
			var IX = setter.type == typeof(int?);

			if ( F || FX )
			{
				var v = (FX ? ((float?)setter.value).Value : (float)setter.value);
				var newv = Mathf.Clamp(S_FLOAT_FIELD(_R, v, null, lablelOffset), min, max);
				if ( v != newv ) setter.value = FX ? (float?)(newv) : newv;
			}
			else
			{

				var v = (IX ? ((int?)setter.value).Value : (int)setter.value);
				var newv = Mathf.Clamp(S_INT_FIELD(_R, v, null, lablelOffset), Mathf.RoundToInt(min), Mathf.RoundToInt(max));
				if ( v != newv ) setter.value = IX ? (int?)(newv) : newv;
			}
			if ( !string.IsNullOrEmpty( postfix ) )
			{
				_R.x += _R.width;
				_R.width = 25;
				GUI.Label( _R, postfix );
			}

		}

		internal static float FIELD( string text, float v, float min, float max )
		{
			var _R = Draw.R;
			var _r = _R;
			_R.width /= 1.5f;
			GUI.Label( _R, Draw.CONT( text.Trim( trim ) + ":" ) );
			_R.x += _R.width;
			_R.width = _r.width - _R.width;

			var newv = Mathf.Clamp(S_FLOAT_FIELD(_R, v, null), min, max);
			return newv;
		}

		internal static void STRING( string text, string KEY )
		{

			var setter = GetSetter(KEY);

			var _R = Draw.R;
			var _r = _R;
			_R.width /= 3f;
			GUI.Label( _R, Draw.CONT( text.Trim( trim ) + ":" ) );
			_R.x += _R.width;
			_R.width = _r.width - _R.width;

			var v = (string)setter.value;
			var newv = EditorGUI.TextField(_R, v);
			if ( v != newv )
			{
				newv = newv.Replace( '\\', '/' );
				var wrong = System.IO.Path.GetInvalidPathChars();
				var res = newv.ToCharArray().Where(c => !wrong.Contains(c)).ToArray();
				setter.value = res.Length == 0 ? "" : res.Select( c => c.ToString() ).Aggregate( ( a, b ) => a + b );
			}

		}



		static   Action<string,string[],string[]> fake_delay;
		internal static string FOLDER( IRepaint w, string text, string KEY, SETGUI_AboutCache.CacheFolderDrawer.FOLDER_DATA d )
		{
			if ( d.isDelayedFinalization ) throw new Exception( ".isDelayedFinalization" );
			return FOLDER( w, text, KEY, d, out fake_delay );
		}
		internal static float FOLDER_PAD = 0;
		internal static string FOLDER( IRepaint w, string text, string KEY,
			SETGUI_AboutCache.CacheFolderDrawer.FOLDER_DATA d, out Action<string, string[], string[]> outFinalization, bool drawDecorations = true, string cf_String = null )
		{

			string res = null;

			if ( drawDecorations )
				Draw.Sp( 5 );
			outFinalization = null;
			using ( drawDecorations ? MainRoot.GRO( w ).UP( 20 ) : new CLASS_GROUP.dsp() { skip = true } )
			{

				var setter = GetSetter(KEY);
				var v = (string)setter.value;


				if ( drawDecorations )
				{

					Draw.HRx1( 20 );

					var _R = Draw.R;
					_R.width -= 20;

					GUI.Label( _R, Draw.CONT( text.Trim( trim ) + ":" ) );

					//_R = Draw.R;
					//_R.width -= 20;

					//prefix + res 
					// GUILayout.BeginHorizontal();
					// var tr  = EditorGUILayout.GetControlRect(GUILayout.Height(1), GUILayout.ExpandWidth(true));
					// GUILayout.EndHorizontal();
					// GUILayout.BeginHorizontal();
					// var left = _R.x - tr.x;
					// Debug.Log( tr + " " + _R); ;
					// var right =((tr.x + tr.width) - (_R.x + _R.width) ) + 40;
					// if ( left < 0 ) left = 0;
					// GUILayout.Space( left );
					// GUILayout.TextArea( d.prefix + v );
					// if ( right < 0 ) right = 0;
					// GUILayout.Space( right );
					// GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					GUILayout.Space( 20 );
					GUILayout.TextArea( d.prefix + v );
					GUILayout.Space( 20 );
					if ( FOLDER_PAD > 0 ) GUILayout.Space( FOLDER_PAD );
					GUILayout.EndHorizontal();
				}


				var _R2 = Draw.R;
				if ( drawDecorations )
					_R2.width -= 20;

				if ( drawDecorations )
				{
					_R2.width /= 2;
					if ( GUI.Button( _R2, "Open Folder" ) )
					{
						SETGUI_MODS_ENABLER.REV( d.prefix + v );
					}
					_R2.x += _R2.width;
				}



				if ( GUI.Button( _R2, cf_String ?? "Change Folder" ) )
				{
					int i   = 0;
					while ( true )
					{
						res = EditorUtility.OpenFolderPanel( text, d.prefix + v, "" );
						if ( string.IsNullOrEmpty( res ) ) break;
						res = res.Replace( '\\', '/' ).Trim( '/' );

						if ( d.isInternal )
						{
							if ( !res.StartsWith( Application.dataPath, StringComparison.OrdinalIgnoreCase ) || res.Length <= Application.dataPath.Length )
							{
								EditorUtility.DisplayDialog( "Warning! Folder cannot be selected", "You have to choose folder in the your 'Assets' folder\n" + res, "Ok" );
								i++;
								if ( i > 2 ) break;
								continue;
							}

							res = res.Substring( Application.dataPath.Length ).Trim( '/' );
							// if ( !(d.prefix + res).StartsWith( "Assets/" ) ) res = "Assets/" + res;
							if ( d.CheckHiddenFoldersForInternal )
							{
								var fold = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( d.prefix + res );
								if ( !fold )
								{
									AssetDatabase.ImportAsset( d.prefix + res, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate );
									AssetDatabase.Refresh( ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate );
									AssetDatabase.Refresh();
									fold = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( d.prefix + res );
									if ( !fold )
									{
										EditorUtility.DisplayDialog( "Warning! Folder cannot be selected", "You have to choose visible folder in the your project\n" + res, "Ok" );
										i++;
										if ( i > 2 ) break;
										continue;
									}
								}
							}

						}
						else
						{
						}




						if ( string.Compare( res, v, true ) != 0 )
						{
							if ( d.valudate( d.prefix + res ) )
							{

								if ( d.trimResult != null ) res = d.trimResult( res );
								Action<string, string[], string[]> delayFinish = null;

								delayFinish = ( newKEY, files, foldes ) => {
									var newSetter = GetSetter(newKEY);

									// var content = EditorUtility.DisplayDialogComplex( "Question?", "Do you want to move or copy content?\n\nfrom:\n...-" +
									/// setter.value + "\n\nto:\n...-" + res, "Move", "Skip", "Copy" );
									// Debug.Log( content ); //1

									newSetter.value = res;
									AssetDatabase.Refresh();
								};



								if ( d.isDelayedFinalization )
								{
									setter.value = res;
									outFinalization = delayFinish;
								}
								else
								{
									if ( d.doMovingConten ) throw new Exception( "Not implemented" );
									var oldPath = d.prefix + (string)setter.value;
									delayFinish( KEY, null, new[] { oldPath } );
								}
							}
						}
						break;
					}

				}


				//  EditorGUILayout.LabelField( "Assets/" + Root.p[ 0 ].par_e.AS_LOCATION );
				//var _r = _R;
				//_R.width /= 1.5f;
				//_R.x += _R.width;
				//_R.width = _r.width - _R.width;
				//
				//var newv = EditorGUI.TextField(_R, v);
				//if ( v != newv )
				//{
				//    newv = newv.Replace( '\\', '/' );
				//    var wrong = System.IO.Path.GetInvalidPathChars();
				//    var res = newv.ToCharArray().Where(c => !wrong.Contains(c)).ToArray();
				//    setter.value = res.Length == 0 ? "" : res.Select( c => c.ToString() ).Aggregate( ( a, b ) => a + b );
				//}
				if ( drawDecorations )
					Draw.HRx1( 20 );
			}
			if ( drawDecorations )
				Draw.Sp( 5 );


			return res;
		}

		internal struct MOVE_COMPLETED_ACTION_DATA
		{
			internal string mem_t_GET_PATH_EXTERNAL;
			internal string mem_t_GET_PATH_TOSTRING;
			internal string temp_t_GET_PATH_EXTERNAL;
			internal string temp_t_GET_PATH_TOSTRING;
			internal string[] files;
			internal string[] folders;


			internal MOVE_COMPLETED_ACTION_DATA( Folders.CACHE_PATH_GETTER mem_t, string[] files, string[] folders )
			{
				this.mem_t_GET_PATH_EXTERNAL = mem_t.GET_PATH_EXTERNAL;
				this.mem_t_GET_PATH_TOSTRING = mem_t.GET_PATH_TOSTRING;
				this.temp_t_GET_PATH_EXTERNAL = null;
				this.temp_t_GET_PATH_TOSTRING = null;
				this.files = files;
				this.folders = folders;
			}

			internal void SET_TARGET( Folders.CACHE_PATH_GETTER temp_t )
			{
				this.temp_t_GET_PATH_EXTERNAL = temp_t.GET_PATH_EXTERNAL;
				this.temp_t_GET_PATH_TOSTRING = temp_t.GET_PATH_TOSTRING;
			}
		}

		internal static void MOVE_COMPLETED_ACTION( MOVE_COMPLETED_ACTION_DATA data )
		{


			// var oldPath_internal = mem_t.GET_PATH_INTERNAL;
			// var newPath_internal = temp_t.GET_PATH_INTERNAL;
			var oldPath = data.mem_t_GET_PATH_EXTERNAL;
			var newPath = data.temp_t_GET_PATH_EXTERNAL;

			if ( string.Equals( oldPath, newPath, StringComparison.OrdinalIgnoreCase ) ) return;
			/*
            var oldPath_internal = (string)setter.value;
            var newPath_internal = res;
            var oldPath = d.prefix + (string)setter.value;
            var newPath = d.prefix + res;
            var do_move =  d.doMovingConten;
            if ( d.isInternal )
            {
                oldPath = Folders.UNITY_SYSTEM_PATH + oldPath.Trim( '/' );
                newPath = Folders.UNITY_SYSTEM_PATH + newPath.Trim( '/' );
            }
            */


			var copy_or_move = EditorUtility.DisplayDialogComplex( "Do copy or move?", "Do you want to copy or move your content to new folder?\n\nfrom:\n...-" +
										  data.mem_t_GET_PATH_TOSTRING + "\nto:\n...-" + data.temp_t_GET_PATH_TOSTRING, "Copy", "No", "Move" );

			//var copy_or_move = EditorUtility.DisplayDialog( "Do copy?", "Do you want to copy your content to new folder?\n\nfrom:\n...-" +
			//  oldPath + "\n\nto:\n...-" + (d.prefix + res), "Yes", "No" ) ? 0 : 1;
#if LOG_FILE_COPY
            Debug.Log( copy_or_move + "\n\n" + oldPath + "\n\n" + newPath );
#endif

			if ( copy_or_move != 1 )
			{


				if ( !Directory.Exists( oldPath ) )
				{
					EditorUtility.DisplayDialog( "Folder is Empty!", "Folder is empty! Nothing copied:\n...-" + oldPath, "Ok" );
				}
				else
				{
					int doOverride = -1;

					EditorUtility.DisplayProgressBar( "Copying/Moving", "Cache is copying or moving:", 0.2f );

					try
					{
						if ( data.files != null ) foreach ( var file in data.files ) MoveFile( file, oldPath, newPath, ref doOverride, copy_or_move );

						EditorUtility.DisplayProgressBar( "Copying/Moving", "Cache is copying or moving", 0.6f );

						if ( data.folders != null ) foreach ( var folder in data.folders ) MoveFolder( folder, oldPath, newPath, ref doOverride, copy_or_move );

					}
					catch ( Exception ex )
					{
						EditorUtility.ClearProgressBar();
						throw new Exception( ex.Message + "\n" + ex.StackTrace, ex );
					}

					EditorUtility.ClearProgressBar();


					if ( copy_or_move == 2 && Directory.Exists( oldPath ) )
					{
						if ( Directory.GetFiles( oldPath, "*.*", SearchOption.TopDirectoryOnly ).Length == 0 &&
							Directory.GetDirectories( oldPath, "*.*", SearchOption.TopDirectoryOnly ).Length == 0 )
						{
							Directory.Delete( oldPath, true );
							if ( File.Exists( oldPath + ".meta" ) ) File.Delete( oldPath + ".meta" );
						}
					}
				}
			}
		}

		/*
        internal static void MOVE_COMPLETE_ACTION()
        {
            Action<string, string[], string[]> delayFinish = null;
            var oldPath_internal = (string)setter.value;
            var newPath_internal = res;
            var oldPath = d.prefix + (string)setter.value;
            var newPath = d.prefix + res;
            var do_move =  d.doMovingConten;

            if ( d.isInternal )
            {
                oldPath = Folders.UNITY_SYSTEM_PATH + oldPath.Trim( '/' );
                newPath = Folders.UNITY_SYSTEM_PATH + newPath.Trim( '/' );
            }

            delayFinish = ( newKEY, files, foldes ) => {
                var newSetter = GetSetter(newKEY);
                if ( do_move )
                {
                    // var content = EditorUtility.DisplayDialogComplex( "Question?", "Do you want to move or copy content?\n\nfrom:\n...-" +
                    /// setter.value + "\n\nto:\n...-" + res, "Move", "Skip", "Copy" );
                    // Debug.Log( content ); //1

                    var copy_or_move = EditorUtility.DisplayDialogComplex( "Do copy or move?", "Do you want to copy or move your content to new folder?\n\nfrom:\n...-" +
                                          oldPath_internal + "\nto:\n...-" + newPath_internal, "Copy", "No", "Move" );


                    //var copy_or_move = EditorUtility.DisplayDialog( "Do copy?", "Do you want to copy your content to new folder?\n\nfrom:\n...-" +
                    //  oldPath + "\n\nto:\n...-" + (d.prefix + res), "Yes", "No" ) ? 0 : 1;
#if LOG_FILE_COPY
                    Debug.Log( copy_or_move );
#endif

                    if ( copy_or_move != 1 )
                    {


                        if ( !Directory.Exists( newPath ) )
                        {
                            EditorUtility.DisplayDialog( "Error!", "Folder doesn't exist:\n...-" + oldPath, "Ok" );
                        }
                        else
                        {
                            int doOverride = -1;

                            if ( files != null ) foreach ( var file in files ) MoveFile( file, oldPath, newPath, ref doOverride, copy_or_move );

                            if ( foldes != null ) foreach ( var fold in foldes ) MoveFolder( fold, oldPath, newPath, ref doOverride, copy_or_move );


                            if ( copy_or_move == 2 )
                            {
                                if ( Directory.GetFiles( oldPath, "*.*", SearchOption.TopDirectoryOnly ).Length == 0 )
                                {
                                    if ( Directory.Exists( oldPath ) ) Directory.Delete( oldPath );
                                    if ( File.Exists( oldPath + ".meta" ) ) File.Delete( oldPath + ".meta" );
                                }
                            }

                            newSetter.value = res;
                        }

                    }
                    else
                    {
                        newSetter.value = res;
                    }
                }
                else
                {
                    newSetter.value = res;
                }
                AssetDatabase.Refresh();
            };

        }
        */

		enum MoveReturn { _break, _continue }

		static MoveReturn MoveFile( string filename, string oldPath, string newPath, ref int doOverride, int copy_or_move )
		{

			if ( filename.EndsWith( ".meta", StringComparison.OrdinalIgnoreCase ) ) return MoveReturn._continue;


			var sourceFile = oldPath.Trim('/') + '/' + filename;
			var targetFile = newPath.Trim('/') + '/' + filename;



			if ( !File.Exists( sourceFile ) )
			{
#if LOG_FILE_COPY
                Debug.Log( "Skip Move File\nsourceFolder: " + sourceFile + "\ntargetFolder: " + targetFile );
#endif
				goto findl;
			}
			else
			{
#if LOG_FILE_COPY
                //Debug.Log( "Move File\nsourceFolder: " + sourceFile + "\ntargetFolder: " + targetFile );
#endif
			}



			if ( File.Exists( targetFile ) )
			{
				if ( doOverride == -1 ) doOverride = EditorUtility.DisplayDialogComplex( "File Exist", "File Exist:\n...-" + targetFile
					  + "\nDo you want to overwrite with file:\n...-" + sourceFile, "Yes for all", "Cancel copy/move", "No for all" );
				if ( doOverride == 1 ) return MoveReturn._break;
				if ( doOverride == 0 )
				{
					File.Delete( targetFile );
					//if ( copy_or_move == 2 )
					//{
					//    if ( File.Exists( targetFile + ".meta" ) )
					//        File.Delete( targetFile + ".meta" );
					//}

				}
				if ( doOverride == 2 ) return MoveReturn._continue;
			}
			if ( !Directory.Exists( targetFile.Remove( targetFile.LastIndexOf( '/' ) ) ) ) Directory.CreateDirectory( targetFile.Remove( targetFile.LastIndexOf( '/' ) ) );
			File.Copy( sourceFile, targetFile );
			if ( copy_or_move == 2 )
			{
				if ( File.Exists( sourceFile + ".meta" ) )
				{
					if ( File.Exists( targetFile + ".meta" ) ) File.Delete( targetFile + ".meta" );
					File.Move( sourceFile + ".meta", targetFile + ".meta" );
				}
			}
			else
			{
				//var asd = targetFile.Substring(targetFile.IndexOf("/Assets/", StringComparison.OrdinalIgnoreCase)).Trim('/');
				//AssetDatabase.ImportAsset( asd, ImportAssetOptions. );
			}



findl:;

			if ( copy_or_move == 2 )
			{
				if ( File.Exists( sourceFile ) ) File.Delete( sourceFile );
				if ( File.Exists( sourceFile + ".meta" ) ) File.Delete( sourceFile + ".meta" );

				var asd = oldPath.Trim('/')+ '/' ;
				while ( true )
				{
					var ind = filename.LastIndexOf( '/' );
					if ( ind == -1 ) break;
					filename = filename.Remove( ind );
					if ( Directory.GetFiles( asd + filename, "*", SearchOption.TopDirectoryOnly ).Length == 0 &&
						Directory.GetDirectories( asd + filename, "*", SearchOption.TopDirectoryOnly ).Length == 0
						)
					{
						Directory.Delete( asd + filename, true );
						if ( File.Exists( asd + filename + ".meta" ) ) File.Delete( asd + filename + ".meta" );
					}
					//ind = filename.LastIndexOf( '/' );
				}

			}





			return MoveReturn._continue;
		}

		static void MoveFolder( string folderName, string oldPath, string newPath, ref int doOverride, int copy_or_move )
		{

			var sourceFolder = oldPath.Trim('/') + '/' + folderName;
			var targetFolder = newPath.Trim('/') + '/' + folderName;


			if ( !Directory.Exists( sourceFolder ) )
			{
#if LOG_FILE_COPY
                Debug.Log( "Skip Move\nsourceFolder: " + sourceFolder + "\ntargetFolder: " + targetFolder );
#endif
				goto findl;
			}
			else
			{
#if LOG_FILE_COPY
                Debug.Log( "Move Folder\nsourceFolder: " + sourceFolder + "\ntargetFolder: " + targetFolder );
#endif
			}


			foreach ( var _sourceFile in Directory.GetFiles( sourceFolder, "*.*", SearchOption.AllDirectories ) )
			{
				var sourceFile = _sourceFile.Replace( '\\', '/' ).Trim( '/' );


				if ( !sourceFile.StartsWith( sourceFolder, StringComparison.OrdinalIgnoreCase ) )
				{
					var si = sourceFolder.IndexOf('/');
					var fi = sourceFile.IndexOf('/');
					if ( fi == -1 || si == -1 )
					{
						Debug.LogWarning( "Cannot move file from: " + sourceFile + " int: " + sourceFolder );
						continue;
					}
					var disk = sourceFolder.Remove(si);
					sourceFile = disk + sourceFile.Substring( fi );
					if ( !sourceFile.StartsWith( sourceFolder, StringComparison.OrdinalIgnoreCase ) )
						Debug.LogWarning( "Cannot move file from: " + sourceFile + " int: " + sourceFolder + " event aver disk changing" );
				}
				if ( sourceFile.Length == sourceFolder.Length ) continue;

				var fileName = sourceFile.Substring(sourceFolder.Length).Trim('/');


				//var targetFile = sourceFile;
				//var targetDir = targetFile.Remove(targetFile.LastIndexOf('/'));

				// var res = MoveFile(sourceFile,targetFile,targetDir, ref doOverride ,  copy_or_move) ;
				var res = MoveFile( fileName, sourceFolder, targetFolder, ref doOverride, copy_or_move );

				if ( res == MoveReturn._break ) break;

			}

findl:;

			if ( copy_or_move == 2 && Directory.Exists( sourceFolder ) )
			{
				var leftFiles = Directory.GetFiles( sourceFolder, "*.*", SearchOption.AllDirectories );
				if ( leftFiles.Count( f => !f.EndsWith( ".meta", StringComparison.OrdinalIgnoreCase ) ) == 0 )
				{
					foreach ( var item in leftFiles ) File.Delete( item );
					Directory.Delete( sourceFolder, true );
					if ( File.Exists( sourceFolder + ".meta" ) ) File.Delete( sourceFolder + ".meta" );
				}
			}
		}

		internal static GUIContent CONT( string text, string tooltip )
		{
			var c = new GUIContent();
			c.text = text;
			c.tooltip = tooltip;
			return c;
		}

		static char[] trim2 = { ' ', '-' };
		internal static GUIContent CONT( string text )
		{
			var c = new GUIContent();
			c.tooltip = c.text = text;
			c.tooltip = c.tooltip.Trim( trim2 );
			return c;
		}

		internal static float S_FLOAT_FIELD( Rect rect, float value, string postFix = null, float labelOffset = 0 )
		{
			var crop = rect;
			if ( labelOffset == 0 )
			{
				if ( crop.width < 160 * 1.5f )
				{
					crop.width /= 1.5f;
				}
				else
				{
					crop.width -= 80;
				}
			}
			else crop.width = labelOffset;
			crop.x += crop.width;
			crop.width = rect.width - crop.width;
			value = EditorGUI.FloatField( crop, value );

			var ac = GUI.color;
			GUI.color *= Color.clear;
			//value = EditorGUI.FloatField( new Rect( 0, rect.y, rect.x * 4, rect.height ), "ASD", value );
			GUI.BeginClip( rect );
			rect.y = rect.x = 0;

			rect.x += rect.width;
			float viewRect ;
			viewRect = EditorGUIUtility.labelWidth;
			rect.x -= viewRect;
			value = EditorGUI.FloatField( rect, "ASDaaaaaaaaaaaaaaaaaaaaa", value );
			rect.x -= viewRect;
			value = EditorGUI.FloatField( rect, "ASDaaaaaaaaaaaaaaaaaaaaa", value );
			rect.x -= viewRect;
			value = EditorGUI.FloatField( rect, "ASDaaaaaaaaaaaaaaaaaaaaa", value );

			//value = EditorGUI.FloatField( rect, "ASD", value );
			GUI.EndClip();

			GUI.color = ac;

			// PREPARE_TEXTFIELD();
			/*  var crop = rect;
             crop.width /= 2;
              var crop1 = crop;

              crop.x += crop.width;
              GUI.SetNextControlName( "MyTextField" );


              if ( GUI.enabled ) value = EditorGUI.FloatField( crop, value );

              if ( GUI.GetNameOfFocusedControl() != "MyTextField" )
              {
                  if ( GUI.enabled )
                  {
                      GUI.BeginClip( crop1 );
                      value = EditorGUI.FloatField( new Rect( 0, 0, crop1.width, rect.height ), " ", value );
                      GUI.EndClip();
                  }
              }

              if ( Event.current.type == EventType.Repaint
                  && GUI.GetNameOfFocusedControl() != "MyTextField" )
              { //if (!EditorGUIUtility.isProSkin)
                  (Root.p[ 0 ]).GET_SKIN().textField.Draw( rect, new GUIContent( value.ToString() + (postFix ?? "") ), false, false, false, false );

                  (Root.p[ 0 ]).GET_SKIN().textField.Draw( rect, new GUIContent( value.ToString() + (postFix ?? "") ), false, false, false, false );
              }
              */
			// RESTORE_TEXTFIELD();

			return value;
		}


		/*  internal int S_INT_FIELD( string title , int value ) {
              var R = EditorGUILayout.GetControlRect();
              // var R = GetControlRect(false, GUILayout.Height(20));
              return S_INT_FIELD( R , title , value );
          }
          internal int S_INT_FIELD( Rect rect , string title , int value ) {

              PREPARE_TEXTFIELD();
              var result = EditorGUI.IntField(rect, title, value);
              RESTORE_TEXTFIELD();

              return result;
          }*/
		static GUIContent fieldEmptyContent = new GUIContent("ASDaaaaaaaaaaaaaaaaaaaaa");
		private static readonly int s_FloatFieldHash = "EditorTextField".GetHashCode();
		internal static int S_INT_FIELD( Rect rect, int value, string postFix = null, float lablelOffset = 0 )
		{

			var crop = rect;
			if ( lablelOffset == 0 )
			{
				if ( crop.width < 160 * 1.5f )
				{
					crop.width /= 1.5f;
				}
				else
				{
					crop.width -= 80;
				}
			}
			else crop.width = lablelOffset;

			crop.x += crop.width;
			crop.width = rect.width - crop.width;
			value = EditorGUI.IntField( crop, value );

			rect.width = rect.width - crop.width;

			//int controlID = GUIUtility.GetControlID(s_FloatFieldHash, FocusType.Keyboard, rect);
			//var prefixLabel =   EditorGUI.PrefixLabel(rect, controlID, fieldEmptyContent);

			var ac = GUI.color;
			GUI.color *= Color.clear;
			rect.width -= 10;
			if ( rect.width < 0 ) rect.width = 0;
			GUI.BeginClip( rect );
			rect.y = rect.x = 0;
			rect.x += rect.width;
			//rect.x += rect.width / 2;
			float viewRect ;//= rect.width / 2.1f; // EditorGUIUtility.currentViewWidth
							//viewRect = EditorStyles.inspectorFullWidthMargins;
							//viewRect = prefixLabel.width;
			viewRect = EditorGUIUtility.labelWidth;

			rect.x -= viewRect;
			//value = EditorGUI.IntField( new Rect( 0, rect.y, rect.x * 4, rect.height ), "ASD", value );
			value = EditorGUI.IntField( rect, "ASDaaaaaaaaaaaaaaaaaaaaa", value );
			rect.x -= viewRect;
			value = EditorGUI.IntField( rect, "ASDaaaaaaaaaaaaaaaaaaaaa", value );
			rect.x -= viewRect;
			value = EditorGUI.IntField( rect, "ASDaaaaaaaaaaaaaaaaaaaaa", value );
			GUI.EndClip();
			GUI.color = ac;


			//  var crop1 = crop;

			// GUI.SetNextControlName( "MyTextField" );

			// if ( GUI.enabled ) 
			// value = EditorGUI.IntField( rect, value );

			/*    if ( GUI.GetNameOfFocusedControl() != "MyTextField" )
                {
                    if ( GUI.enabled )
                    {
                        GUI.BeginClip( crop1 );
                        value = EditorGUI.IntField( new Rect( 0, 0, crop1.width, rect.height ), " ", value );
                        GUI.EndClip();
                    }
                }*/

			/*  if ( Event.current.type == EventType.Repaint
                  && GUI.GetNameOfFocusedControl() != "MyTextField" )
              { //if ( !EditorGUIUtility.isProSkin )
                  (Root.p[ 0 ]).GET_SKIN().textField.Draw( rect, new GUIContent( value.ToString() + (postFix ?? "") ), false, false, false, false );

                  (Root.p[ 0 ]).GET_SKIN().textField.Draw( rect, new GUIContent( value.ToString() + (postFix ?? "") ), false, false, false, false );
                  // RESTORE_TEXTFIELD();
              }*/


			// PREPARE_TEXTFIELD();
			/*  var crop = rect;
              crop.width /= 2;
              var crop1 = crop;

              crop.x += crop.width;
              GUI.SetNextControlName( "MyTextField" );

              if ( GUI.enabled ) value = EditorGUI.IntField( crop, value );

              if ( GUI.GetNameOfFocusedControl() != "MyTextField" )
              {
                  if ( GUI.enabled )
                  {
                      GUI.BeginClip( crop1 );
                      value = EditorGUI.IntField( new Rect( 0, 0, crop1.width, rect.height ), " ", value );
                      GUI.EndClip();
                  }
              }

              if ( Event.current.type == EventType.Repaint
                  && GUI.GetNameOfFocusedControl() != "MyTextField" )
              { //if ( !EditorGUIUtility.isProSkin )
                  (Root.p[ 0 ]).GET_SKIN().textField.Draw( rect, new GUIContent( value.ToString() + (postFix ?? "") ), false, false, false, false );

                  (Root.p[ 0 ]).GET_SKIN().textField.Draw( rect, new GUIContent( value.ToString() + (postFix ?? "") ), false, false, false, false );
                  // RESTORE_TEXTFIELD();
              }*/

			return value;
		}


		internal static void SLIDER()
		{
			/*  var R = GetControlRect();
                     var b = (stack[i].IsNullable ? ((int?) stack[i].setter.value).Value : (int) stack[i].setter.value) + (int) stack[i].offset;
                     var newb = (A.S_SLIDER(R, TOGGLE_PTR + stack[i].text, b, (int) stack[i].min, (int) stack[i].max));

                     if (b != newb)
                     {
                         if (stack[i].IsNullable) stack[i].setter.value = (int?) (newb - (int) stack[i].offset);
                         else stack[i].setter.value = newb - (int) stack[i].offset;

                         ValueChanged();
                     }*/

			throw new Exception();
		}


		internal static void COLOR( string text, string KEY, object overrideObject = null )
		{
			// var R = stack[i].UseLastRect ? lastRect : GetControlRect();
			var _R = R;
			// if ( stack[ i ].UseLastRect ) _R.width += 90;

			_R.width -= 85;

			var setter = GetSetter(KEY, overrideObject: overrideObject);

			DRAW_RESET_TO_DEFAULT( _R, setter );

			if ( text != null ) GUI.Label( _R, CONT( text.Trim() + ":" ) );
			var b = (Color)setter.value;


			// var newb = Tools.PICKER(new Rect(_R.x + _R.width, _R.y - 3, 85, 23), text, b);
			var newb = Tools.PICKER(new Rect(_R.x + _R.width, _R.y, 85, _R.height), text, b);

			if ( b != newb )
			{
				setter.value = newb;
			}
		}


		internal static Color COLOR( Rect r, Color oldColor )
		{
			var _R = r;
			_R.width -= 85;


			// var newb = Tools.PICKER(new Rect(_R.x + _R.width, _R.y - 3, 85, 23), null, oldColor);
			var newb = Tools.PICKER(new Rect(_R.x + _R.width, _R.y, 85, _R.height), null, oldColor);
			return newb;
		}
		internal static Color COLOR_TRUERECT( Rect r, Color oldColor )
		{
			// var newb = Tools.PICKER(new Rect(_R.x + _R.width, _R.y - 3, 85, 23), null, oldColor);
			var newb = Tools.PICKER(r, null, oldColor);
			return newb;
		}

		internal static Color COLOR( Rect r, GUIContent text, Color oldColor )
		{
			var _R = r;
			_R.width -= 85;
			//	if (text != null) GUI.Label(_R, CONT(text.Trim() + ":"));
			GUI.Label( _R, text );
			// return Tools.PICKER( new Rect( _R.x + _R.width, _R.y - 3, 85, 23 ), text.tooltip, oldColor );
			return Tools.PICKER( new Rect( _R.x + _R.width, _R.y, 85, _R.height ), text.tooltip, oldColor );
		}
		static Color oldc;
		internal static void HELP( IRepaint ir, string text, Color? c = null, bool drawTog = false )
		{
			//  EditorGUI.LabelField( R, text, s( "previewMiniLabel" ) );
			var _s = s("previewMiniLabel");
			_s.wordWrap = true;

			if ( c.HasValue )
			{
				oldc = GUI.color;
				GUI.color *= c.Value;
			}
			if ( drawTog ) text = "· " + text;
			var ca = _s.normal.textColor;
			if ( !EditorGUIUtility.isProSkin ) _s.normal.textColor = new Color32( 20, 20, 20, 255 );
			EditorGUI.TextArea( CALC_R( ir, _s, text ), text, _s );
			_s.normal.textColor = ca;
			if ( c.HasValue ) GUI.color = oldc;
			// GUI.Label( CALC_R( _s, text ), text, _s );

		}
		static GUIContent _calcContent = new GUIContent();
		static Rect CALC_R( IRepaint ir, GUIStyle s, string t )
		{
			_calcContent.text = t;
			var h = s.CalcHeight(_calcContent, (ir.currentWidth()?? EditorGUIUtility.currentViewWidth- 16) - CALC_PADDING );
			var r = _getRerct(GUILayout.Height(h));
			return r;
		}

		internal static void HELP_TEXTURE( IRepaint ir, string v, float space = 10 )
		{


			Draw.Sp( space );
			var t = Root.icons.GetHelpTexture(v);
			var rect = PEEK_NEW_WIDHT();
			rect.height = t.height;
			rect.x += (rect.width - t.width) / 2;
			rect.width = t.width - 20;

			var cw = (ir.currentWidth()?? EditorGUIUtility.currentViewWidth) - 20;
			if ( cw < rect.width )
			{
				var dif = (rect.width - cw) / 2;
				var fac = dif / rect.width;
				rect.x += dif;
				rect.width = Mathf.RoundToInt( cw );
				var HH = rect.height * fac * 2;
				rect.height -= HH;

			}
			rect.x = Mathf.RoundToInt( rect.x );

			rect.y = RH( rect.height ).y;

			Color c = Color.white;
			if ( !GUI.enabled ) c.a = 0.4f;
			Root.p[ 0 ].gl._DrawTexture_ForExternalWindow( rect, t, ref c );
			Draw.Sp( 10 );
		}

		internal static void HELP_TEXTURE( Rect r, string v )
		{
			var t = Root.icons.GetHelpTexture(v);
			r.x += r.width - t.width;
			r.width = t.width;
			r.y -= 1;
			r.height = t.height;

			Color c = Color.white;
			if ( !GUI.enabled ) c.a = 0.4f;
			Root.p[ 0 ].gl._DrawTexture_ForExternalWindow( r, t, ref c );
		}
		internal static void DRAW_NEW( Rect r )
		{
			var t = Root.icons.GetOldExternalMod("NEW");
			//r.x += r.width;
			r.x = 0;
			r.width = t.width;
			r.y += (r.height - t.height) / 2;
			r.height = t.height;
			//r.x -= r.width * 2;
			Color c = GUI.color;
			if ( !GUI.enabled ) GUI.color *= new Color( 1, 1, 1, 0.4f );
			GUI.DrawTexture( r, t );
			//Graphics.DrawTexture( r, t, null, 0 );
			GUI.color = c;
		}
		internal static void HELP_TEXTURE_OLD( IRepaint ir, string v )
		{
			var t = Root.icons.GetOldExternalMod(v);
			var rect = RH(t.height);
			rect.x += (rect.width - t.width) / 2;
			rect.width = t.width;

			var cw = (ir.currentWidth()?? EditorGUIUtility.currentViewWidth);
			if ( cw < rect.width )
			{
				rect.x += (rect.width - cw) / 2;
				rect.width = Mathf.RoundToInt( cw );
			}
			rect.x = Mathf.RoundToInt( rect.x );

			Color c = GUI.color;
			if ( !GUI.enabled ) GUI.color *= new Color( 1, 1, 1, 0.4f );
			GUI.DrawTexture( rect, t );
			GUI.color = c;
			//Root.p[ 0 ].gl._DrawTexture_ForExternalWindow( r, t, ref c );
		}



		internal static void HRx4RED()
		{
			Sp( 4 );

			Draw.HRx2();
			//EditorGUI.DrawRect(Draw.R2, Color.red);
			var c = GUI.color;
			GUI.color = Color.red;
			Draw.HRx2();
			GUI.color = c;
			/*	var r = R05;
                if (Event.current.type == EventType.Repaint)
                    s("dragHandle").Draw(r, ec, false, false, false, false);*/
			//Sp(12);
			Draw.HRx2();
			/*   r = R05;
              if ( Event.current.type == EventType.Repaint )
                  s( "dragHandle" ).Draw( r, ec, false, false, false, false );*/
			Sp( 4 );
		}
		internal static void HRx1( float resize = 0 )
		{
			Sp( 4 );
			var r = R05;
			r.width -= resize;
			if ( Event.current.type == EventType.Repaint )
				s( "dragHandle" ).Draw( r, ec, false, false, false, false );
			/*   r = R05;
              if ( Event.current.type == EventType.Repaint )
                  s( "dragHandle" ).Draw( r, ec, false, false, false, false );*/
			Sp( 4 );
		}
		internal static void HRx1( Rect r )
		{
			// r.y += r.height / 4;
			r.height /= 2;
			if ( Event.current.type == EventType.Repaint )
				s( "dragHandle" ).Draw( r, ec, false, false, false, false );
		}
		internal static void HRx05( Rect r )
		{
			r.y += (r.height - 3) / 2;
			r.height = 2f;
			if ( Event.current.type == EventType.Repaint )
				s( "preToolbar" ).Draw( r, ec, false, false, false, false );
		}
		internal static void HRx2()
		{
			Sp( 4 );
			var r = R05;
			if ( Event.current.type == EventType.Repaint )
				s( "dragHandle" ).Draw( r, ec, false, false, false, false );
			Sp( 12 );
			/*   r = R05;
              if ( Event.current.type == EventType.Repaint )
                  s( "dragHandle" ).Draw( r, ec, false, false, false, false );*/
			Sp( 4 );
		}
		internal static void EXPAND( string text )
		{
			GUI.Button( R, text, s( "preDropDown" ) );
		}

		internal static Rect Grow( Rect p, int v )
		{
			v = -v;
			p.x += v;
			p.y += v;
			p.width -= v * 2;
			p.height -= v * 2;
			return p;
		}
	}












	internal class CLASS_ENALBE
	{
		internal class dsp : IDisposable
		{
			internal PluginInstance A;
			internal float usePadding;
			internal bool enableStack;
			internal Color col;

			public void Dispose()
			{

				Draw.padding -= 20;
				Draw.padding -= usePadding;
				GUI.color = col;
				GUI.enabled = enableStack;// A.DRAW_STACK.ENABLE_RESTORE(); //ENABLE
			}
		}

		internal PluginInstance A;
		internal IRepaint ir;
		internal dsp USE( string setter )
		{
			return USE( Draw.GetSetter( setter ) );
		}
		internal enum operation { OR, AND }
		internal dsp USE( string setter1, string setter2, operation p )
		{
			return USE( Draw.GetSetter( setter1 ), Draw.GetSetter( setter2 ), p );
		}
		internal dsp USE( string setter, object overrideObject )
		{
			return USE( Draw.GetSetter( setter, overrideObject: overrideObject ) );
		}
		internal dsp USE( string setter, bool inverce = false, object overrideObject = null, float? padding = null )
		{
			if ( padding.HasValue ) return USE( Draw.GetSetter( setter, overrideObject: overrideObject ), padding: padding.Value, inverce: inverce );
			return USE( Draw.GetSetter( setter, overrideObject: overrideObject ), inverce );
		}
		internal dsp USE( FIELD_SETTER setter, bool inverce = false )
		{
			Draw.padding += 20;
			var o = new dsp() { A = A, enableStack = GUI.enabled, col = GUI.color };
			var was = GUI.enabled;
			GUI.enabled &= inverce ? !setter.AS_BOOL : setter.AS_BOOL;
			if ( !GUI.enabled && was ) GUI.color *= new Color( 1, 1, 1, 0.4f );
			//   A.DRAW_STACK.ENABLE_SET( setter ); //ENABLE
			return o;
		}

		internal dsp USE( string setter, float padding, bool inverce = false, object overrideObject = null )
		{
			return USE( Draw.GetSetter( setter, overrideObject: overrideObject ), padding, inverce );
		}

		internal dsp USE( FIELD_SETTER setter, float padding, bool inverce = false )
		{
			// A.DRAW_STACK.ENABLE_SET( setter ); //ENABLE
			// A.DRAW_STACK.PADDING_SET( padding );
			//  setter.value

			// Draw.padding += 20;
			padding -= 20;
			var res = USE(setter, inverce);
			res.usePadding = padding;
			Draw.padding += padding;
			return res;
			//  return new dsp() { A = A, usePadding = true };
		}
		internal dsp USE( FIELD_SETTER setter1, FIELD_SETTER setter2, operation p )
		{
			dsp res;
			Draw.padding += 20;
			switch ( p )
			{
				case operation.OR:
					res = USE( setter1.AS_BOOL || setter2.AS_BOOL );
					break;
				case operation.AND:
					res = USE( setter1.AS_BOOL && setter2.AS_BOOL );
					break;
				default: throw new Exception( "" );
			}
			res.usePadding = 0;
			return res;
		}

		internal dsp USE( bool v )
		{
			var o = new dsp() { A = A, enableStack = GUI.enabled, col = GUI.color };
			o.usePadding = -20;
			var was = GUI.enabled;
			GUI.enabled &= v;
			if ( !GUI.enabled && was ) GUI.color *= new Color( 1, 1, 1, 0.4f );
			//   A.DRAW_STACK.ENABLE_SET( setter ); //ENABLE
			return o;
		}
	}




	internal class CLASS_GROUP
	{
		internal class dsp : IDisposable
		{
			// internal bool UseSearchSet;
			internal PluginInstance A;
			internal CLASS_GROUP c;
			internal float p;
			internal int groupIndex;
			internal bool skip = false;
			public void Dispose()
			{
				if ( skip ) return;
				// A.DRAW_STACK.END_GROUP();
				GUILayout.EndVertical();
				Draw.padding -= p;
				var res = Draw.Sp(5);
				if ( res.y != 0 )
				{
					if ( !lastRect.ContainsKey( groupIndex ) ) lastRect.Add( groupIndex, Rect.zero );
					if ( Event.current.type == EventType.Repaint )
					{
						if ( lastRect[ groupIndex ] != res )
						{
							c.ir.Repaint();
							//Debug.Log( "ASD" );
						}
						lastRect[ groupIndex ] = res;
					}
				}

				//   if ( UseSearchSet ) A.DRAW_STACK.SEARCH_SET( null ); //SEARCH
			}
		}
		Rect firstRect;



		static System.Collections.Concurrent.ConcurrentDictionary<int,Dictionary<int, Rect>> __lastRect = new System.Collections.Concurrent.ConcurrentDictionary<int,Dictionary<int, Rect>>();
		static Dictionary<int, Rect> lastRect {
			get {
				//EditorGUIUtility.GetControlID
				if ( !__lastRect.ContainsKey( Draw.CurrentId ) ) __lastRect.TryAdd( Draw.CurrentId, new Dictionary<int, Rect>() );
				return __lastRect[ Draw.CurrentId ];
			}
		}
		// Rect lastRect;
		internal PluginInstance A;
		internal IRepaint ir;
		static GUIContent ec = new GUIContent();

		internal dsp UP( float padding = 20 )
		{
			// A.DRAW_STACK.BEGIN_GROUP();
			Draw.groupIndex++;
			var p = Draw.Sp(5);
			firstRect = p;
			if ( Event.current.type == EventType.Repaint )
			{
				if ( lastRect.ContainsKey( Draw.groupIndex ) )
				{
#pragma warning disable
					var l = lastRect[Draw.groupIndex];
					p.height = l.y + l.height - p.y;
					//Debug.Log( p.height );
					p.x = 0;
					p.width = ir.currentWidth() ?? (EditorGUIUtility.currentViewWidth - 16);
					// GUI.skin.box.Draw( p, ec, false, false, false, false );
#pragma warning restore
#if UNITY_2019_4_OR_NEWER
					Draw.s( "preToolbar" ).Draw( p, ec, false, false, false, false );
#endif
					//      p.x += 12;
					//  p.width -= 24;
					//  Draw.s( "preBackground" ).Draw( Draw.Grow(p, -3), ec, false, false, false, false );
				}
			}
			var c = GUI.color;
			GUI.color *= new Color( 1, 1, 1, 0.5f );

			GUILayout.BeginVertical(); //m_ProgressBarBack  m_Tooltip
			GUI.color = c;
			Draw.padding += padding;
			// groupBegin = true;
			return new dsp() { A = A, c = this, p = padding, groupIndex = Draw.groupIndex };
		}

		/* internal dsp UP( string searchSet )
         {
             A.DRAW_STACK.SEARCH_SET( searchSet ); //SEARCH
             A.DRAW_STACK.BEGIN_GROUP();
             return new dsp() { A = A, UseSearchSet = true };
         }*/

	}


	internal class FIELD_SETTER
	{

		internal string KEY;
		internal FIELD_SETTER( string KEY )
		{
			this.KEY = KEY;
		}

		public static bool operator ==( FIELD_SETTER a, FIELD_SETTER b )
		{
			if ( object.ReferenceEquals( a, null ) && object.ReferenceEquals( a, null ) ) return true;
			if ( object.ReferenceEquals( a, null ) || object.ReferenceEquals( a, null ) ) return false;
			return object.Equals( a.value, b.value );
		}
		public static bool operator !=( FIELD_SETTER a, FIELD_SETTER b ) { return !(a == b); }

		internal static void ValueChanged( PluginInstance A )
		{
			//  A.SavePrefs();
			//  A.RESET_DRAW_STACKS();
			Cache.ClearHierarchyObjects( false );
			//  A.RepaintWindowInUpdate();
			UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
		}

		public override bool Equals( object obj )
		{
			throw new Exception( "FIELD_SETTER" );
		}
		public override int GetHashCode()
		{
			throw new Exception( "FIELD_SETTER" );
		}

		internal void REMOVE()
		{
			var oldvalue = value;
			value = oldvalue; //write file info;
			var p = Root.p[ 0 ];
			p.par_e.REMOVE( Root.p[ 0 ].par_e.LAST_KEY, Root.p[ 0 ].par_e.LAST_FILE );
			var defaultValue = value;
			value = oldvalue;
			value = defaultValue;
			//if (overrideObject is WindowSettings )
			//{
			//	for ( int i = 0; i < 2; i++ )
			//	{
			//		foreach ( var item in PluginInstance.WindowsData( i ) )
			//			if ( item.Value.w.Instance )
			//			{
			//				item.Value.w.RESET_LINE_HEIGHT( i );
			//				item.Value.w.RESET_CHILD_INDENT( i );
			//				item.Value.w.RESET_BOTTOM();
			//				item.Value.w.RESET_DEFAULT_ICON_SIZE( i );
			//				item.Value.w.RESET_GAMEOBJECTS_NAMES( i );
			//			}
			//	}
			//	NewClearHelper.OnFontSizeChanged();
			//}
			//RightModsStyles.ClearLabels();
			p.modsController.REBUILD_PLUGINS();
			p.RepaintAllViews();
		}

		internal bool isprop;
		internal FieldInfo field;
		internal PropertyInfo prop;
		internal PluginInstance A;
		internal bool UsePar = true;
		internal Func<FIELD_SETTER, bool> onValidateChange;
		internal Action<FIELD_SETTER> onChanged;
		internal object overrideObject = null;

		// object cachedValue
		internal object value {
			get {
				if ( UsePar ) return isprop ? prop.GetValue( overrideObject ?? A.par_e, null ) : field.GetValue( overrideObject ?? A.par_e );
				else return isprop ? prop.GetValue( A, null ) : field.GetValue( A );
			}

			set {
				if ( this.value == value ) return;


				if ( onChanged != null && !onValidateChange( this ) ) return;

				if ( UsePar )
				{
					object t = overrideObject ?? A.par_e;

					if ( isprop ) prop.SetValue( t, value, null );
					else field.SetValue( t, value );

					//A.par_e = (EditorSettingsAdapter)t;
				}

				else
				{
					if ( isprop ) prop.SetValue( A, value, null );
					else field.SetValue( A, value );
				}

				if ( onChanged != null ) onChanged( this );

				ValueChanged( A );
			}
		}
		internal Type type { get { return this.isprop ? this.prop.PropertyType : this.field.FieldType; } }
		public bool AS_BOOL {
			get {
				var t = this.isprop ? this.prop.PropertyType : this.field.FieldType;
				bool res = false;

				if ( t == typeof( bool ) )
				{
					res |= (bool)this.value;
					//if ( this2 != null ) res |= (bool)this2.value;
				}

				else if ( t == typeof( bool? ) )
				{
					res |= (bool?)this.value ?? true;
					// if ( this2 != null ) res |= (bool?)this2.value ?? true;
				}

				else if ( t == typeof( int ) )
				{
					res |= (int)this.value != 0;
					// if ( this2 != null ) res |= (int)this2.value == 1;
				}

				else if ( t == typeof( int? ) )
				{
					res |= ((int?)this.value ?? 0) != 0;
					//if ( setter2 != null ) res |= ((int?)setter2.value ?? 0) == 1;
				}
				return res;
			}
			set {
				var t = this.isprop ? this.prop.PropertyType : this.field.FieldType;
				if ( t == typeof( bool ) ) this.value = value;
				else if ( t == typeof( bool? ) ) this.value = (bool?)value;
				else if ( t == typeof( int ) ) this.value = value ? 1 : 0;
				else if ( t == typeof( int? ) ) this.value = (int?)(value ? 1 : 0);
			}
		}
	}
}
