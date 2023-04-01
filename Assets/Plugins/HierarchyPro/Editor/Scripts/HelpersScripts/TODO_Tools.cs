using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;



namespace EMX.HierarchyPlugin.Editor
{

	class TODO_Tools //todo performance improvements
	{
		public static EditorWindow[] ALL_WINDOWS {
			get {
				if ( __ALL_WINDOWS != null ) return __ALL_WINDOWS;

				var tv = Resources.FindObjectsOfTypeAll<EditorWindow>();

				if ( tv.Any( w => w.GetType().Name.Contains( "SceneView" ) ) &&
					tv.Any( w => w.GetType().Name.Contains( "SceneHierarchy" ) ) ) __ALL_WINDOWS = tv;

				return tv;
			}

			set { }
		}
		static EditorWindow[] __ALL_WINDOWS;

		static string[] _getAls;

		internal static string[] ALL_ASSETS_PATHS {
			get {
				var r = _getAls ?? (_getAls = AssetDatabase.GetAllAssetPaths().Select(p => new pathcomparerclass(p)).OrderBy(p => p).Select(p => p.p).ToArray());
				/* Debug.Log(" --- ");
                 for (int i = 0; i < 15; i++)
                 {   Debug.Log(r[i]);
                 }*/
				return r;
			}
			set { _getAls = null; }
		}






















		// static Dictionary<Type, TempColorClass> ObjectContent_Objectcache = new Dictionary<int, TempColorClass>();
		internal   static Dictionary<Type, TempColorClass> TypeToIcon = new Dictionary<Type, TempColorClass>();
		internal static Dictionary<int, TempColorClass> InstanceIDToIcon = new Dictionary<int, TempColorClass>();
		//  static  TempColorClass __internal_ObjectContentTempColor = new TempColorClass();
		internal static TempColorClass GetObjectBuildinIcon( Type type )
		{
			if ( !TypeToIcon.ContainsKey( type ) )
			{
				var g = EditorGUIUtility.ObjectContent(null, type);
				var result = new TempColorClass().AddIcon((Texture2D)g.image, g.text);
				TypeToIcon.Add( type, result );
			}
			return TypeToIcon[ type ];
		}
		internal static TempColorClass GetObjectBuildinIcon( UnityEngine.Object o, Type type )
		{
			if ( !o ) return GetObjectBuildinIcon( type );
			var key = o.GetInstanceID();
			if ( !InstanceIDToIcon.ContainsKey( key ) )
			{
				var g = EditorGUIUtility.ObjectContent(o, type);
				var result = new TempColorClass().AddIcon((Texture2D)g.image);
				InstanceIDToIcon.Add( key, result );
			}
			if ( Application.isPlaying ) return InstanceIDToIcon[ key ];
			return InstanceIDToIcon[ key ].AddIcon( (Texture2D)EditorGUIUtility.ObjectContent( o, type ).image );
			/*  else
              {   return __internal_ObjectContentTempColor.AddIcon( EditorGUIUtility.ObjectContent( o, type ).image );
              }*/
		}


		//  static object[] invoke_args = new object[1];
		internal static UnityEngine.Object GUI_TO_OBJECT( ref string guid )
		{
			// invoke_args[0] = guid;
			// return UnityEditor.EditorUtility.InstanceIDToObject((int)Root.p[0].GetInstanceIDFromGUID.Invoke(null, invoke_args));
			var path = AssetDatabase.GUIDToAssetPath(guid);
			if ( path == null || path == "" ) return null;
			return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( path );
		}









		internal static GUIContent CacheDisableConten = new GUIContent("Cache disabled!", null, "Cache disabled!");

		static Assembly[] __ass_raw;
		internal static Assembly[] ass_raw {
			get {
				if ( __ass_raw == null )
				{
					__ass_raw = System.AppDomain.CurrentDomain.GetAssemblies().Where( a => !a.FullName.EndsWith( "Editor" ) && !a.FullName.StartsWith( "UnityEditor" ) )
								.ToArray();
				}

				return __ass_raw;
			}
		}
		static Dictionary<string, Type> ass;
		internal static Type GET_TYPE_BY_STRING( string str )
		{
			if ( string.IsNullOrEmpty( str ) ) return null;

			if ( ass == null ) ass =
					ass_raw.SelectMany( a => a.GetTypes() ).ToLookup( pair => pair.FullName, pair => pair )
					.ToDictionary( group => group.Key, group => group.First() );

			if ( ass.ContainsKey( str ) ) return ass[ str ];

			return null;
		}

		internal static Type GET_TYPE_BY_STRING_SHORT( string str )
		{
			var res = GET_TYPE_BY_STRING(str);
			if ( res == null ) res = __GET_TYPE_BY_STRING_SHORT( str );
			return res;
		}

		static Type __GET_TYPE_BY_STRING_SHORT( string str )
		{
			if ( string.IsNullOrEmpty( str ) ) return null;
			var res = GET_TYPE_BY_STRING(str);
			if ( res != null ) return res;

			str = str.Split( '.' ).Last();

			if ( ass == null ) ass =
				  ass_raw.SelectMany( a => a.GetTypes() ).ToLookup( pair => pair.Name, pair => pair )
				  .ToDictionary( group => group.Key, group => group.First() );
			if ( ass.ContainsKey( str ) ) return ass[ str ];
			/*
            if (ass == null) ass =
                  ass_raw.SelectMany(a => a.GetTypes()).ToLookup(pair => pair.Name, pair => pair)
                  .ToDictionary(group => group.Key, group => group.First());
            if (ass.ContainsKey(str)) return ass[str];*/

			return null;
		}

	}









	/// ########################################################################################################################################################################################################
	public struct pathcomparerclass : IComparer<pathcomparerclass>, IComparable<pathcomparerclass>
	{
		public pathcomparerclass( string p )
		{
			this.p = p;
			var ls = p.LastIndexOf('/');
			var ld = p.LastIndexOf('.');
			if ( ls != -1 && ld != -1 && ld > ls && !AssetDatabase.IsValidFolder( p ) ) //name = p.Substring(ls + 1);
			{ // isFolder = false;
				comparePath = p.Remove( ls ).Replace( "/", "//" ) + p.Substring( ls );
			}
			else
			{
				comparePath = p.Replace( "/", "//" );
			}
		}

		string comparePath;

		public string p;

		public int Compare( pathcomparerclass x, pathcomparerclass y )
		{
			return x.comparePath.CompareTo( y.comparePath );
		}

		public int CompareTo( pathcomparerclass other )
		{
			return Compare( this, other );
		}






	}









}
