using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace EMX.HierarchyPlugin.Editor
{
	class FunEditorFontsModification
	{
		static Dictionary<object, float> _styleCache = new Dictionary<object, float>();
		internal static void Modificate( int SIZE, bool reset = false )
		{

			if ( GUI.skin == null ) return;

			//GUIStyle a;
			//Vector2 qwe = a.contentOffset;
			//RectOffset wer = a.padding;
			var fs = typeof(GUIStyle).GetProperty("fontSize");
			var contentOffset = typeof(GUIStyle).GetProperty("contentOffset");
			var padding = typeof(GUIStyle).GetProperty("padding");
			var clipping = typeof(GUIStyle).GetProperty("clipping");
			//var fixedHeight = typeof(GUIStyle).GetProperty("fixedHeight");
			foreach ( var item in typeof( EditorWindow ).Assembly.GetTypes() )
			{
				foreach ( var f in item.GetFields( ~BindingFlags.Instance ) )
				{
					// if ( !f.IsStatic ) continue;
					if ( f.FieldType.Name == "GUIStyle" )
					{
						// if (f.FieldType.IsGenericParameter && f.FieldType.GetGenericParameterConstraints().Length != 0 ) continue; //TODO
						//if (f.FieldType.IsConstructedGenericType) continue; //TODO
						// Debug.Log(f.Name+ " " + item    );

						try
						{
							var gs = f.GetValue(null);
							if ( gs == null ) continue;
							var oldFs = (int)fs.GetValue( gs );
							fs.SetValue( gs, SIZE );
							//var oldFixedHeight = (float)fixedHeight.GetValue( gs );
							//if ( oldFixedHeight != 0 && SIZE != oldFs )
							//{
							//	//var oldcontentOffset = (Vector2)contentOffset.GetValue( gs );
							//	//oldcontentOffset.y += SIZE - oldFs;
							//	//contentOffset.SetValue( gs, oldcontentOffset );
							//	var oldpadding = (RectOffset)padding.GetValue( gs );
							//	oldpadding.top += SIZE - oldFs;
							//	padding.SetValue( gs, oldpadding );
							//	//var dif = (SIZE )/(float)oldFs;
							//	//fixedHeight.SetValue( gs, oldFixedHeight * dif );
							//}
							//var oldpadding = (RectOffset)padding.GetValue( gs );
							//if ( oldpadding.top != 0 && SIZE != oldFs )
							//{
							//	var oldcontentOffset = (Vector2)contentOffset.GetValue( gs );
							//	var res = oldcontentOffset.y + (SIZE - oldFs)/4f;
							//	var minus = res < 0;
							//	res = Mathf.Abs( res ) % 1;
							//	if ( res > 0.3f )
							//	{
							//		var newRes = oldpadding.top;
							//		if ( newRes == 0 ) newRes += SIZE - oldFs;
							//		if ( newRes == 0 ) newRes += SIZE - oldFs;
							//		oldpadding.top = newRes;
							//		padding.SetValue( gs, oldpadding );
							//		Debug.Log( oldpadding.top + " " + oldpadding.top + " " + gs.ToString() );
							//		if ( minus ) oldcontentOffset.y = Mathf.Ceil( oldcontentOffset.y );
							//		else oldcontentOffset.y = Mathf.Floor( oldcontentOffset.y );
							//		contentOffset.SetValue( gs, oldcontentOffset );
							//	}
							//	//contentOffset.SetValue( gs, oldcontentOffset );
							//	//var dif = (SIZE )/(float)oldFs;
							//	//fixedHeight.SetValue( gs, oldFixedHeight * dif );
							//}

							var oldpadding = (RectOffset)padding.GetValue( gs );
							if ( oldpadding.top != 0 && oldpadding.top != oldpadding.bottom && SIZE != oldFs )
							{
								var oldcontentOffset = (Vector2)contentOffset.GetValue( gs );
								if ( !_styleCache.ContainsKey( gs ) ) _styleCache.Add( gs, 0 );
								var ov = _styleCache[gs];
								ov -= (SIZE - oldFs) / 2f;
								//oldcontentOffset.y -= (SIZE - oldFs) / 2f;
								_styleCache[ gs ] = oldcontentOffset.y = ov;
								contentOffset.SetValue( gs, oldcontentOffset );
								var tc = (TextClipping)clipping.GetValue( gs );
								if ( tc == TextClipping.Clip ) clipping.SetValue( gs, TextClipping.Overflow );

							}
							if ( oldpadding.top != 0 && oldpadding.top != oldpadding.bottom && reset == true )
							{
								var tc = (TextClipping)clipping.GetValue( gs );
								if ( tc != TextClipping.Clip ) clipping.SetValue( gs, TextClipping.Clip );
							}

						}
						catch { }
					}
				}
			}
			bas = typeof( EditorWindow ).Assembly.GetType( "UnityEditor.View" );
			view = typeof( EditorWindow ).Assembly.GetType( "UnityEditor.GUIView" );
			RepaintImmediately = view.GetMethod( "RepaintImmediately", ~(BindingFlags.Static | BindingFlags.GetField) );
			//  var p = bas.GetField("m_Parent", (BindingFlags)(-1));
			var m_Children = bas.GetField("m_Children",  ~(BindingFlags.Static | BindingFlags.InvokeMethod));

			ScriptableObject target = null;
			foreach ( var item in Resources.FindObjectsOfTypeAll<ScriptableObject>() )
			{
				if ( !bas.IsAssignableFrom( item.GetType() ) ) continue;
				target = item;
				break;
				// Replace( item.GetType(), item, 215 );
			}
			Scan( m_Children, target, SIZE );

			if ( reset ) Root.RequestScriptReload();
			//  Rep( m_Children, target, SIZE );
			//   var sceneView = Resources.FindObjectsOfTypeAll<SceneView>().First();
			//  Debug.Log(target.GetType().Name);
			/* var par = p.GetValue(target);
             while(p.GetValue(target) != null ) par = p.GetValue(target);
             Debug.Log(par.GetType().Name);*/
		}
		static Type view, bas;
		static MethodInfo RepaintImmediately;
		static void Rep( FieldInfo m_Children, object o, int size )
		{

			// Replace( o.GetType(), o, size );
			if ( view.IsAssignableFrom( o.GetType() ) )
			{
				try
				{
					RepaintImmediately.Invoke( o, null );
				}
				catch { }
			}
			var a = m_Children.GetValue(o) as object[];
			if ( a == null ) return;
			foreach ( var chld in a )
			{
				Rep( m_Children, chld, size );
			}
		}
		static void Scan( FieldInfo m_Children, object o, int size )
		{
			Replace( o.GetType(), o, size );

			var a = m_Children.GetValue(o) as object[];
			if ( a == null ) return;
			foreach ( var chld in a )
			{
				Scan( m_Children, chld, size );
			}
		}

		static void Replace( Type t, object o, int size )
		{
			if ( t == null ) return;
			var fs =    typeof(GUIStyle).GetProperty("fontSize");


			foreach ( var f in t.GetFields() )
			{
				if ( o == null && !f.IsStatic ) continue;
				if ( f.FieldType.Name == "GUIStyle" )
				{

					var gs = f.GetValue(o);
					if ( gs == null ) continue;
					try
					{
						fs.SetValue( gs, size );
					}
					catch { }
				}
			}


			foreach ( var nst in t.GetNestedTypes( (BindingFlags)(-1) ) )
			{
				Replace( nst, null, size );
			}

			if ( o == null ) return;
			Replace( t.BaseType, o, size );
		}
	}
}
