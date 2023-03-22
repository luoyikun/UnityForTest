using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditorInternal;

namespace EMX.HierarchyPlugin.Editor
{

	partial class Tools
	{


		internal static int singleLineHeight = 16;
		// EditorGUIUtility.singleLineHeight


		internal static Type GetTypeByInstanceID( long id, int pluginId )
		{
			if ( pluginId == 0 )
			{
				return EditorUtility.InstanceIDToObject( (int)id ).GetType();
			}
			try
			{
				return InternalEditorUtility.GetTypeWithoutLoadingObject( (int)id );
			}
			catch
			{
				return ot;
			}
		}
		static Type ot = typeof(object);


		internal static void EventUseFast()
		{
			Event.current.Use();
		}
		internal static void EventUse()
		{
			if ( Event.current != null && Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout ) Tools.EventUseFast();
		}


		internal static HierarchyObject[] GetOnlyTopObjects( HierarchyObject[] affectedObjectsArray )
		{
			if ( affectedObjectsArray.Length == 0 ) return affectedObjectsArray;
			if ( affectedObjectsArray[ 0 ].pluginID == 0 )
			{
				var converted = affectedObjectsArray.Select(a => new { a, par = a.go.GetComponentsInParent<Transform>(true).Where(p => p != a.go.transform) });
				return converted.Where( c => c.par.Count( p => affectedObjectsArray.Any( a => a.go == p.gameObject ) ) == 0 ).Select( g => g.a ).ToArray();
			}
			else
			{

			}

			return affectedObjectsArray.Where( g => g.GetAllParents().Count( p => affectedObjectsArray.Contains( p ) ) == 0 ).ToArray();
		}
		internal static GameObject[] GetOnlyTopObjects( GameObject[] affectedObjectsArray )
		{
			if ( affectedObjectsArray.Length == 0 ) return affectedObjectsArray;
			var converted = affectedObjectsArray.Select(a => new { a, par = a.GetComponentsInParent<Transform>(true).Where(p => p != a.transform) });
			return converted.Where( c => c.par.Count( p => affectedObjectsArray.Any( a => a == p.gameObject ) ) == 0 ).Select( g => g.a ).ToArray();
		}


		/*  internal static void TRY_PING_OBJECT( GameObject o )
          {
              var selO = GetHierarchyObjectByInstanceID(o);
              TRY_PING_OBJECT( selO );
          }*/


		internal static void FocusToInspector()
		{
			if ( !EditorWindow.focusedWindow.titleContent.text.ToLower().Contains( "inspector" ) )
			{
				var fi = TODO_Tools.ALL_WINDOWS.FirstOrDefault(w => w.titleContent.text.ToLower().Contains("inspector"));
				if ( !fi )
					fi = Resources.FindObjectsOfTypeAll<EditorWindow>().FirstOrDefault( w => w.titleContent.text.ToLower().Contains( "inspector" ) );
				if ( fi ) fi.Focus();
			}
		}






		internal static void TRY_PING_OBJECT( UnityEngine.Object o )
		{
			if ( !Root.p[ 0 ].par_e.ENABLE_OBJECTS_PING ) return;
			if ( !o ) return;

			if ( o is GameObject ) Root.p[ 0 ].ha_G( 0 ).UPDATE_PING_STYLE();
			else Root.p[ 0 ].ha_G( 1 ).UPDATE_PING_STYLE();

			EditorGUIUtility.PingObject( o );
		}
		internal static void asD( string guid )
		{
			if ( !Root.p[ 0 ].par_e.ENABLE_OBJECTS_PING ) return;

			Root.p[ 0 ].ha_G( 1 ).UPDATE_PING_STYLE();

			TRY_PING_OBJECT( TODO_Tools.GUI_TO_OBJECT( ref guid ) );
		}



		static bool CheckParentTransform( Transform current, Transform parent )
		{
			var p = current;
			while ( p )
			{
				if ( p == parent ) return true;
				p = p.parent;
			}
			return false;
		}


		static internal int lastScanIndex = 0;
		static int folder_i = 0;
		internal static IEnumerable<HierarchyObject> AllSceneObjectsInterator( int pluginID, bool GetInScenes = false, UnityEngine.Object activeParent = null,
				string extension = null )        // in this example we return less than N if necessary
		{

			//	if (adapter == null) yield break;

			GameObject activeGO;
			Transform activeTransform = null;
			bool haveActiveParentTransform = false;
			if ( pluginID == 0 )
			{
				activeGO = activeParent as GameObject;
				if ( activeGO ) activeTransform = activeGO.transform;
				haveActiveParentTransform = activeTransform;
			}

			string activePath = null;
			if ( pluginID == 1 )
			{
				if ( activeParent ) activePath = AssetDatabase.GetAssetPath( activeParent );
				haveActiveParentTransform = !string.IsNullOrEmpty( activePath );
				folder_i = 0;
				/*activeGO = activeParent as GameObject;
                if (activeGO) activeTransform = activeGO.transform;
                haveActiveParentTransform = activeTransform;*/
			}
			// Debug.Log((bool)activeParent + " " + (bool)activeGO);


			if ( pluginID == 0 || GetInScenes )
			{
				var asd = 0;
				for ( int index = SceneManager.sceneCount - 1; index >= 0; index-- )
				{
					var s = SceneManager.GetSceneAt(index);
					if ( !s.IsValid() || !s.isLoaded ) continue;
					var source = s.GetRootGameObjects();


					List<Transform> O_T = new List<Transform>();
					List<int> O_index = new List<int>();

					// DoubleList<Transform, int> offsetter = new DoubleList<Transform, int>();

					for ( int sss = source.Length - 1; sss >= 0; sss-- )
					{
						if ( !source[ sss ] ) continue;
						O_T.Add( source[ sss ].transform );
						O_index.Add( source[ sss ].transform.childCount - 1 );


						var current_T = O_T[O_T.Count - 1];
						var childIndex = O_index[O_index.Count - 1];

						do
						{
							asd++;
							scan_interator = asd.ToString();
							lastScanIndex = source.Length - 1 - sss;
							current_T = O_T[ O_T.Count - 1 ];
							childIndex = O_index[ O_index.Count - 1 ];


							if ( childIndex < 0 || !current_T || childIndex >= current_T.childCount )
							{
								if ( O_T.Count == 1 ) break;

								O_T.RemoveAt( O_T.Count - 1 );
								O_index.RemoveAt( O_index.Count - 1 );

								if ( current_T && VALIDATE_FLAGS( current_T.gameObject.hideFlags ) && (!haveActiveParentTransform
										|| CheckParentTransform( current_T, activeTransform )) ) yield return Cache.GetHierarchyObjectByInstanceID( current_T.gameObject );
								continue;
							}

							/* if (childIndex >= current_T.childCount)
                            {
                            O_index[O_index.Count - 1] = current_T.childCount - 1;
                            continue;
                            }*/


							var child = current_T.GetChild(O_index[O_index.Count - 1]);
							O_index[ O_index.Count - 1 ] = O_index[ O_index.Count - 1 ] - 1;

							if ( child.childCount == 0 )
							{
								if ( VALIDATE_FLAGS( current_T.gameObject.hideFlags ) && (!haveActiveParentTransform
										|| CheckParentTransform( current_T, activeTransform )) ) yield return Cache.GetHierarchyObjectByInstanceID( child.gameObject );
								continue;
							}

							O_T.Add( child );
							O_index.Add( child.childCount - 1 );


						} while ( O_T.Count > 0 );

						lastScanIndex = source.Length - 1 - sss;
						if ( O_T[ 0 ] && VALIDATE_FLAGS( O_T[ 0 ].gameObject.hideFlags ) && (!haveActiveParentTransform
								|| CheckParentTransform( O_T[ 0 ], activeTransform )) ) yield return Cache.GetHierarchyObjectByInstanceID( O_T[ 0 ].gameObject );

						//if (O_T[0] && (O_T[0].gameObject.hideFlags & fillter) == 0 && (!haveActiveParentTransform || CheckParentTransform( O_T[0], activeTransform ))) yield return adapter.GetHierarchyObjectByInstanceID( O_T[0].gameObject );
						O_T.Clear();
						O_index.Clear();
					}
				}
			}

			if ( pluginID == 1 )     //  var LLL = Application.dataPath.Length - "Assets".Length;
			{   // var paths =  System.IO.Directory.GetFiles( Application.dataPath,"*.*",System.IO.SearchOption.AllDirectories ).Where(p=>!p.EndsWith(".meta")).Select(p=>p.Replace('\\','/').Substring(LLL)).ToArray();
				var paths = TODO_Tools.ALL_ASSETS_PATHS;
				for ( ; folder_i < paths.Length; folder_i++ )
				{
					//scan_interator = (int)(folder_i / (float)paths.Length) * 100 + "%";
					scan_interator = folder_i + "/" + paths.Length;
					if ( !paths[ folder_i ].StartsWith( "Assets", StringComparison.OrdinalIgnoreCase ) ) continue;
					if ( haveActiveParentTransform && !paths[ folder_i ].StartsWith( activePath, StringComparison.OrdinalIgnoreCase ) ) continue;
					if ( extension != null && !paths[ folder_i ].EndsWith( extension, StringComparison.OrdinalIgnoreCase ) ) yield return null;
					var path = AssetDatabase.AssetPathToGUID(paths[ folder_i ]);
					if ( string.IsNullOrEmpty( path ) ) continue;
					var g = AssetDatabase.AssetPathToGUID(paths[ folder_i ]);
					yield return Cache.GetHierarchyObjectByGUID( ref g, ref es, null );
				}
			}
		}
		static string scan_interator = "";
		static internal string GET_SCAN_PROGRESS()
		{
			return scan_interator;
		}

		static string es = "";
		internal static HideFlags SearchFlags = HideFlags.HideInInspector;
		static bool VALIDATE_FLAGS( HideFlags flags )
		//  {   return (flags & Adapter.flagsSHOW) != Adapter.flagsSHOW;
		{
			return (flags & SearchFlags) != SearchFlags;
		}

		/*    static IEnumerable<Transform> GetT(Transform t)
            {
                for (int i = t.childCount - 1; i >= 0; i--)
                {
                    yield return GetT(t.GetChild(i));
                }
                yield return t;
                /*if ((t.gameObject.hideFlags & fillter) != 0) return;

                for (int i = 0, len = t.childCount; i < len; i++) WriteT(t.GetChild(i));#1#
            }*/

		internal static int AllSceneObjectsInteratorCount( int pluginID, bool GetInScenes = false )
		{
			if ( pluginID == 1 && !GetInScenes ) return -1;
			int res = 0;
			for ( int i = 0; i < SceneManager.sceneCount; i++ )
			{
				var s = SceneManager.GetSceneAt(i);
				if ( !s.IsValid() || !s.isLoaded ) continue;
				res += s.GetRootGameObjects().Length;
			}
			return res;
		}

		internal static int AllSceneObjectsInteratorProgress()
		{
			return lastScanIndex;
		}


		static GUIContent guiContent = new GUIContent();
		internal static GUIContent GET_CONTENT( string name ) { guiContent.text = name; return guiContent; }




		internal static int GetByte( int el, int offset, int length )
		{
			var res = el >> offset & ~((-1) << length);
			return res;
		}
		internal static void SetByte( ref int el, int offset, int length, int Value/*, int? defaultValue = null*/ )
		{
			//  var el = GetElement(index, defaultValue);
			el &= ~((~((-1) << length)) << offset);
			Value &= ~((-1) << length);
			el |= Value << offset;
			// return el;
			/*if ( index >= list.Count ) while ( index >= list.Count ) list.Add( 0 );
            list[ index ] = el;*/
		}
		internal static int GetByte( string _el, int offset, int length )
		{
			int el = _el == "" || _el == null ? 0 : int.Parse(_el);
			var res = el >> offset & ~((-1) << length);
			return res;
		}
		internal static void SetByte( ref string _el, int offset, int length, int Value/*, int? defaultValue = null*/ )
		{
			//  var el = GetElement(index, defaultValue);
			int el = _el == "" || _el == null ? 0 : int.Parse(_el);
			el &= ~((~((-1) << length)) << offset);
			Value &= ~((-1) << length);
			el |= Value << offset;
			_el = el.ToString();
			// return el;
			/*if ( index >= list.Count ) while ( index >= list.Count ) list.Add( 0 );
            list[ index ] = el;*/
		}



		internal static bool IsObjectNull( /*Type type ,*/ object result )
		{   //if ( !type.IsClass ) return false;
			if ( result is UnityEngine.Object ) return !(result as UnityEngine.Object);

			return result == null;
			//return result == null || result.ToString() == "null" && result == Adapter.DefaultValue( type );
		}




		static Type lastType;

		internal static void Copy( Component comp )
		{
			ComponentUtility.CopyComponent( comp );
			lastType = comp.GetType();
			/*  var s = new SerializedObject(comp);
              var current = s.GetIterator();
              if (current == null) return;

              var result = new List<SerializedProperty>();
              while (current.Next(true))
              {
                  result.Add(current.Copy());
              }

              if (!__copyPastDic.ContainsKey(comp.GetType())) __copyPastDic.Add(comp.GetType(), new List<__CopyPasteS>());

              if (__copyPastDic[comp.GetType()].Count > 0)
                  __copyPastDic[comp.GetType()][0] = new __CopyPasteS(result.ToArray());
              else
                  __copyPastDic[comp.GetType()].Add(new __CopyPasteS(result.ToArray()));*/
			//     PrefabUtility.SetPropertyModifications(newGO, PrefabUtility.GetPropertyModifications(gameObject));
		}

		static MethodInfo pasteValudateMethod = null;
		internal static bool PastValidate( Component comp )
		{
			if ( pasteValudateMethod == null )
			{
				pasteValudateMethod = typeof( ComponentUtility ).GetMethods( (BindingFlags)~0 )
					.First( m => m.Name == "PasteComponentValues" && m.GetParameters().Length > 1 );
			}


			return (bool)pasteValudateMethod.Invoke( null, new object[ 2 ] { new UnityEngine.Object[ 1 ] { comp }, true } );
			// EditorUtility.pas
			// Tools.PastValidate
			// EditorUtility.CopySerialized
			// ComponentUtility.PasteComponentAsNew
			// return lastType == (comp.GetType());
			// return __copyPastDic.ContainsKey(comp.GetType());
		}
		internal static bool PastValidate( GameObject comp )
		{
			if ( pasteValudateMethod == null )
			{
				pasteValudateMethod = typeof( ComponentUtility ).GetMethods( (BindingFlags)~0 )
					.First( m => m.Name == "PasteComponentValues" && m.GetParameters().Length > 1 );
			}


			return (bool)pasteValudateMethod.Invoke( null, new object[ 2 ] { new UnityEngine.Object[ 1 ] { comp }, true } );
			// EditorUtility.pas
			// Tools.PastValidate
			// EditorUtility.CopySerialized
			// ComponentUtility.PasteComponentAsNew
			// return lastType == (comp.GetType());
			// return __copyPastDic.ContainsKey(comp.GetType());
		}


		internal static void Paste( Component comp )
		{
			ComponentUtility.PasteComponentValues( comp );
			/* if (!PastValidate(comp)) return;

             var s = new SerializedObject(comp);
             //s.
             foreach (var p in __copyPastDic[comp.GetType()].First().props)
             {
                MonoBehaviour.print(p.propertyPath);
                 s.CopyFromSerializedProperty(p);
             }
             s.ApplyModifiedProperties();
             s.SetIsDifferentCacheDirty();*/

			//     PrefabUtility.SetPropertyModifications(newGO, PrefabUtility.GetPropertyModifications(gameObject));
		}

		internal static void PasteComponentAsNew( Component comp )
		{
			if ( !comp || !comp.gameObject ) return;
			ComponentUtility.PasteComponentAsNew( comp.gameObject );
		}
		internal static bool PasteComponentAsNew( GameObject go )
		{
			if ( !go ) return false;
			return ComponentUtility.PasteComponentAsNew( go );
		}
		static GUIContent tooltip = new GUIContent();
		internal static Color PICKER( Rect inputrect, string tooltip, Color color, bool DRAW_REPAINT = true )       /*55x23*/
		{
			//  if ( GUI.enabled ) EditorGUIUtility.AddCursorRect( inputrect, MouseCursor.Link );

			// if ( !stylesWasInit ) InitStyles();

			var cRect = new Rect(inputrect.x + 1, inputrect.y + 1, inputrect.width - 2, inputrect.height - 2);
			var result = GUI.enabled ?
#if UNITY_2018_1_OR_NEWER
		 EditorGUI.ColorField(cRect, new GUIContent(), color, false, true, false)
#else
                 EditorGUI.ColorField( cRect, new GUIContent(), color, false, true, false, null )0
                // return EditorGUI.ColorField( cRect, new GUIContent(), oldCol, false, true, false, null );
#endif
					: color;

			/*    if ( DRAW_REPAINT )
                {
                    if ( Event.current.type == EventType.Repaint )
                    {
                        var a = GUI.color;
                        if ( !GUI.enabled ) GUI.color = new Color( GUI.color.r, GUI.color.g, GUI.color.b, 0.4f );
                        HIGHLIGHTER_COLOR_DECORATION.Draw( inputrect, ecc, 0 );
                        GUI.color = a;
                        GUI.color *= Adapter.SettingsBGColor;
                        HIGHLIGHTER_COLOR_FG.Draw( inputrect, ecc, 0 );
                        GUI.color = a;
                        HIGHLIGHTER_COLOR_PICKER.Draw( new Rect( inputrect.x + inputrect.width - 3 - 17, inputrect.y + 3, 17, 17 ), ecc, 0 );
                    }
                }*/
			if ( !string.IsNullOrEmpty( tooltip ) )
			{
				Tools.tooltip.tooltip = tooltip;
				GUI.Label( inputrect, Tools.tooltip );
			}

			return result;
		}











		internal static KeyValuePair<string, KeyValuePair<FieldAdapter, object>>[] GET_FIELDS_AND_VALUES( UnityEngine.Object obj, Type type, bool includeArrays = true, int searchVals = 0 )
		{
			var res = new Dictionary<string, KeyValuePair<FieldAdapter, object>>();
			var fff = GET_FIELDS(type, includeArrays);

			foreach ( var f in fff )
			{
				foreach ( var item in f.Value.GetAllValues( obj, 0, searchVals ) )
				{
					res.Add( item.Key, new KeyValuePair<FieldAdapter, object>( f.Value, item.Value ) { } );
				}
			}

			return res.Select( d => new KeyValuePair<string, KeyValuePair<FieldAdapter, object>>( d.Key, d.Value ) ).ToArray();
		}


		static BindingFlags propFlags = BindingFlags.Public | BindingFlags.Instance;
		static Type monoType = typeof(MonoBehaviour);
		static Type compType = typeof(Component);
		static Type rootType = typeof(object);
		internal static void OnThreadChange()
		{
			_SCAN_FIELDS_CACHE = new ConcurrentDictionary<Type, FieldsLocker>();
			_SCAN_FIELDS_CACHE_NOARRAYS = new ConcurrentDictionary<Type, FieldsLocker>();
		}

		internal static ConcurrentDictionary<string, FieldAdapter> GET_FIELDS( Type type, bool includeArrays = true )
		{
			var SCAN_FIELDS_CACHE = includeArrays ? _SCAN_FIELDS_CACHE : _SCAN_FIELDS_CACHE_NOARRAYS;

			//  if ( SCAN_FIELDS_CACHE.ContainsKey( type ) ) SCAN_FIELDS_CACHE.Remove( type );
			if ( SCAN_FIELDS_CACHE.ContainsKey( type ) )
			{
				var d = SCAN_FIELDS_CACHE[ type ];
				var fix = d.lockedObject;
				return d.data;
			}

			if ( type == rootType || type == unityObjectType )
			{
				//lock ( SCAN_FIELDS_CACHE ) 
				SCAN_FIELDS_CACHE.TryAdd( type, new FieldsLocker() );
				return SCAN_FIELDS_CACHE[ type ].data;
			}

			//if ( SCAN_FIELDS_CACHE.ContainsKey( type ) ) return SCAN_FIELDS_CACHE[ type ];

			//var result = new ConcurrentDictionary<string, FieldAdapter>();
			var result = new FieldsLocker();

			lock ( result.lockedObject )
				lock ( result )
				{
					//lock ( SCAN_FIELDS_CACHE )
					{   // MonoBehaviour.print(type + " " + result.Count);
						if ( !SCAN_FIELDS_CACHE.ContainsKey( type ) )
						{
							// LogProxy.LogWarning( "SCAN_FIELDS_CACHE contains " + type.FullName );
							SCAN_FIELDS_CACHE.TryAdd( type, result );
						}
					}

					var newResult = SCAN_FIELDS_CACHE[type];



					if ( newResult != result ) return result.data;

					/* if ( listType == null ) {
						 listType = typeof( List<> );
						 arrayType = typeof( ArrayList );
					 }*/
					/*if (type.Name == "Canvas")
					{


						Debug.Log( type.BaseType.Name);
						Debug.Log( type.GetProperties((BindingFlags)( - 1)).Count());

						Debug.Log( type.GetFields((BindingFlags)( - 1)).First().Name);
						Debug.Log( type.GetFields(flags).Count(f =>
						{   if (!unityObjectType.IsAssignableFrom(f.FieldType)) return false;
							return true;
						}));
						Debug.Log( type.GetFields(flags).Count(f =>
						{   if (!unityObjectType.IsAssignableFrom(f.FieldType)) return false;
							if (f.IsPublic) return true;
							return false;
						}));
					}*/
					// foreach ( var item in type.GetFields( flags ).Where( f => {   
					//     if ( f.FieldType.IsPrimitive ) return false;
					//     if ( !f.IsPublic ) return f.IsDefined( serType, true );
					//     return !f.IsDefined( serType2, true );
					// } ) )
					//     if ( !result.ContainsKey( item.Name ) )
					//     {
					//         var fa = FieldAdapter.TryToCreate(item, includeArrays);
					//         if ( !ReferenceEquals( fa, null ) ) result.Add( item.Name, fa );
					//     }
					Exception ex = null;
					var po = MultiThread.po;
					var id = MultiThread.INSTANCE_ID;
					try
					{
						type.GetFields( flags ).AsParallel().ForAll( item => {
							try
							{
								if ( item.FieldType.IsPrimitive ) return;
								if ( !item.IsPublic ) if ( !item.IsDefined( serType, true ) ) return;
								if ( item.IsDefined( serType2, true ) ) return;
								var fa = FieldAdapter.TryToCreate(item, includeArrays);
								po.CancellationToken.ThrowIfCancellationRequested();
								if ( !ReferenceEquals( fa, null ) )
									if ( !result.data.TryAdd( item.Name, fa ) )
										LogProxy.LogWarning( "ConcurrentDictionary AsParallel cannot add 'A' - it s a debug - just comment this ~'576' line int Tools.cs file\n" + item.Name );
							}
							catch ( Exception e )
							{
								ex = e;
							}

						} );
					}
#pragma warning disable
					catch ( OperationCanceledException e )
					{
						return new ConcurrentDictionary<string, FieldAdapter>();
					}
#pragma warning restore

					if ( ex != null )
					{
						LogProxy.LogError( ex.Message + '\n' + ex.StackTrace );
						return new ConcurrentDictionary<string, FieldAdapter>();
					}


					// foreach ( var item in type.GetFields( flags ).Where( f => {   // if ( !searchType.IsAssignableFrom( f.FieldType ) ) return false;
					//     /* if ( Adapter.UnityEventArgsType == f.FieldType  )
					//          Debug.Log( "!@#" );asdasd*/
					//
					//     if ( f.FieldType.IsPrimitive ) return false;
					//
					//     if ( !f.IsPublic )
					//     {
					//         return f.IsDefined( serType, true );
					//     }
					//
					//     /* if ( f.IsNotSerialized ) return false;
					//      var res = f.GetCustomAttributes( serType , false ).Length != 0;
					//      if ( !res ) return false;*/
					//
					//
					//
					//     return !f.IsDefined( serType2, true );
					//     // return f.GetCustomAttributes( serType2 , false ).Length == 0;
					// } ) )
					//
					//     if ( !result.ContainsKey( item.Name ) )
					//     {
					//         var fa = FieldAdapter.TryToCreate(item, includeArrays);
					//
					//         if ( !ReferenceEquals( fa, null ) ) result.Add( item.Name, fa );
					//     }


					//EMX_TODO compType.IsAssignableFrom( type ) added for performance, but it may affect to any components if playmode keeper
					if ( !monoType.IsAssignableFrom( type ) && compType.IsAssignableFrom( type ) )
					{


						//     // var estimProps = type.GetProperties(propFlags).Where(f =>
						//     var estimProps = type.GetProperties(propFlags).AsParallel().Where(f =>
						//  {
						//      if (!f.PropertyType.IsClass) return false;
						//
						//      if (!f.CanRead || !f.CanWrite) return false;
						//
						//	 // if ( !searchType.IsAssignableFrom( f.PropertyType ) ) return false;
						//	 if ( f.GetGetMethod() == null || f.GetSetMethod() == null ) return false;
						//	 //if ( !f.GetGetMethod().IsPublic || !f.GetSetMethod().IsPublic ) return false;
						//	 var res = (f.GetGetMethod().GetMethodImplementationFlags() & MethodImplAttributes.InternalCall) != 0 && (f.GetSetMethod//().GetMethodImplementationFlags() & MethodImplAttributes.InternalCall) != 0;
						//      if (!res) return false;
						//
						//  }).Select(s=>).ToArray();
						//
						//     for ( int i = 0; i < estimProps.Count; i++ )
						//     {
						//         if ( estimProps[ i ].Name.StartsWith( "shared", StringComparison.OrdinalIgnoreCase ) )
						//         {
						//             var ft = estimProps[i];
						//             var fname = ft.Name.Substring("shared".Length);
						//             estimProps.RemoveAll( e => e.Name.Equals( fname, StringComparison.OrdinalIgnoreCase ) );
						//             i = estimProps.IndexOf( ft );
						//         }
						//     }
						//
						//     foreach ( var item in estimProps )
						//     {
						//         if ( !result.ContainsKey( item.Name ) )
						//         {
						//             var fa = FieldAdapter.TryToCreate(item, includeArrays);
						//
						//             //if ( !ReferenceEquals( fa, null ) ) result.Add( item.Name, fa );
						//             if ( !ReferenceEquals( fa, null ) ) result.TryAdd( item.Name, fa );
						//               //  if ( !result.TryAdd( item.Name, fa ) )
						//                    // LogProxy.LogWarning( "ConcurrentDictionary AsParallel cannot add 'D' " + item.Name );
						//         }
						//     }


						// var estimProps = type.GetProperties(propFlags).Where(f =>
						PropertyInfo ff;
						ConcurrentDictionary<string, PropertyInfo> _temp = new ConcurrentDictionary<string, PropertyInfo>();
						try
						{
							type.GetProperties( propFlags ).AsParallel().ForAll( f => {
								if ( !f.PropertyType.IsClass ) return;
								if ( !f.CanRead || !f.CanWrite ) return;
								if ( f.GetGetMethod() == null || f.GetSetMethod() == null ) return;
								var res = (f.GetGetMethod().GetMethodImplementationFlags() & MethodImplAttributes.InternalCall) != 0 && (f.GetSetMethod().GetMethodImplementationFlags() & MethodImplAttributes.InternalCall) != 0;
								if ( !res ) return;
								po.CancellationToken.ThrowIfCancellationRequested();
								string fname = f.Name.ToLower();
								if ( f.Name.StartsWith( "shared", StringComparison.OrdinalIgnoreCase ) )
								{
									fname = fname.Substring( "shared".Length );
									lock ( _temp ) _temp.TryRemove( fname, out ff );
								}
								_temp.TryAdd( fname, f );
							} );
						}
#pragma warning disable
						catch ( OperationCanceledException e )
						{
							return new ConcurrentDictionary<string, FieldAdapter>();
						}
#pragma warning restore

						if ( id != MultiThread.INSTANCE_ID ) return new ConcurrentDictionary<string, FieldAdapter>();
						foreach ( var s in _temp )
						{
							var item = s.Value;
							if ( !result.data.ContainsKey( item.Name ) )
							{
								var fa = FieldAdapter.TryToCreate(item, includeArrays);
								if ( !ReferenceEquals( fa, null ) ) result.data.TryAdd( item.Name, fa );
							}
						}
					}


					var ct = type.BaseType;
					// FieldAdapter ff;
					if ( id != MultiThread.INSTANCE_ID ) return new ConcurrentDictionary<string, FieldAdapter>();

					while ( ct != rootType )
					{

						foreach ( var item in GET_FIELDS( ct, includeArrays ) )
							// if ( !result.ContainsKey( item.Key ) ) result.Add( item.Key, item.Value );
							if ( !result.data.TryAdd( item.Key, item.Value ) )
							{
								// LogProxy.LogWarning( "ConcurrentDictionary AsParallel cannot add 'E' " + item.Key );
								// if ( !result.TryRemove( item.Key, out ff ) )
								//     LogProxy.LogWarning( "ConcurrentDictionary AsParallel cannot add 'F' " + item.Key );
								// if ( !result.TryAdd( item.Key, item.Value ) )
								//     LogProxy.LogWarning( "ConcurrentDictionary AsParallel cannot add 'G' " + item.Key );
							}
						ct = ct.BaseType;
					}

					if ( id != MultiThread.INSTANCE_ID ) return new ConcurrentDictionary<string, FieldAdapter>();



				}


			return result.data;
		}









		//scrollPosField = m_state.PropertyType.GetField("scrollPos");
		static PropertyInfo _m_expandedIDs;
		static PropertyInfo m_expandedIDs( object m_state )
		{
			if ( _m_expandedIDs == null ) _m_expandedIDs = m_state.GetType().GetProperty( "expandedIDs", ~(BindingFlags.Static | BindingFlags.InvokeMethod) );
			return _m_expandedIDs;
		}
		static MethodInfo _m_SetExpandedIDs;
		static MethodInfo m_SetExpandedIDs( object m_data )
		{
			if ( _m_SetExpandedIDs == null ) _m_SetExpandedIDs = m_data.GetType().GetMethods().First( m => m.Name == "SetExpandedIDs" );
			return _m_SetExpandedIDs;
		}

		static PluginInstance adapter { get { return Root.p[ 0 ]; } }
		static internal GameObject[] CREATE_EXPAND_GO_SNAPSHOT( int scene )
		{
			adapter.WriteTreeController();
			if ( adapter.state_currentTree == null ) return new GameObject[ 0 ]; //adapter.window == null || !adapter.window.Instance ||
																				 //var treeView = adapter.GetTreeViewontroller(adapter.window.Instance);
																				 //var state = adapter.state_currentTree.GetValue(treeView, null);
			var result = (List<int>)m_expandedIDs(adapter.state_currentTree).GetValue(adapter.state_currentTree, null);
			return result.Select( id => EditorUtility.InstanceIDToObject( id ) as GameObject ).Where( o => o && o.scene.GetHashCode() == scene ).ToArray(); // *** //
		}

		static internal void SET_EXPAND_GO_SNAPSHOT( GameObject[] ids, string[] GUIDids, string[] PATHids, int scene )
		{
			adapter.WriteTreeController();
			if ( adapter.state_currentTree == null ) return; //adapter.window == null || !adapter.window.Instance ||
															 //	var treeView = adapter.m_TreeView(adapter.window());
															 //var state = adapter.m_state.GetValue(treeView, null);
			var result = new List<int>();


			result.AddRange( ((List<int>)m_expandedIDs( adapter.state_currentTree ).GetValue( adapter.state_currentTree, null )).Where( id => {
				if ( !(EditorUtility.InstanceIDToObject( id ) as GameObject) ) return true;
				var go = EditorUtility.InstanceIDToObject(id) as GameObject;
				if ( go.scene.GetHashCode() != scene ) return true;
				return false;
			} ) );

			if ( ids != null && ids.Length != 0 )
			{
				/*result.AddRange(((List<int>)m_expandedIDs(adapter.state_currentTree).GetValue(adapter.state_currentTree, null))
					.Where(id => !(EditorUtility.InstanceIDToObject(id) as GameObject) || ((GameObject)EditorUtility.InstanceIDToObject(id)).scene.GetHashCode() != scene));*/


				result.AddRange( ids.Where( o => o ).Select( go => go.GetInstanceID() ) ); // *** //
			}



			/*if (GUIDids != null && GUIDids.Length != 0)
			{
				if (GUIDids.Length != PATHids.Length) PATHids = new string[GUIDids.Length];

				for (int i = 0; i < GUIDids.Length; i++)
				{
					var ts = GUIDids[i];
					var getted = adapter.GetHierarchyObjectByGUID(ref ts, PATHids[i]);

					if (ts != GUIDids[i])
					{
						GUIDids[i] = ts;
						adapter.ON_GUID_BACKCHANGED();
					}

					result.Add(getted.id);
				}

				// result.AddRange( GUIDids.Select( adapter.GetHierarchyObjectByGUID ).Where( o => o != null ).Select( p => p.id ) ); // *** //
			} // GUIDids != null && GUIDids.Length != 0*/

			//	var data = adapter.m_data.GetValue(treeView, null);
			m_SetExpandedIDs( adapter.data_currentTree ).Invoke( adapter.data_currentTree, new[] { result.ToArray() } );

			adapter.RepaintWindow( 0, true );
		}


		/*static internal string[] CREATE_EXPAND_GO_SNAPSHOT_FORPROJECT()
		{
			var treeView = adapter.m_TreeView(adapter.window());
			var state = adapter.m_state.GetValue(treeView, null);
			var result = (List<int>)adapter.m_expandedIDs.GetValue(state, null);

			return result.Select(id => AssetDatabase.GetAssetPath(id)).Where(o => !string.IsNullOrEmpty(o)).Select(AssetDatabase.AssetPathToGUID).ToArray(); // *** //
		}*/

		static internal void SET_EXPAND_NULL( Scene s )
		{
			//	var treeView = adapter.m_TreeView(adapter.window());
			//	var state = adapter.m_state.GetValue(treeView, null);
			var result = new List<int>();
			adapter.WriteTreeController();

			//	if (adapter.IS_HIERARCHY())
			result.AddRange( ((List<int>)m_expandedIDs( adapter.state_currentTree ).GetValue( adapter.state_currentTree, null )).Where( id => {
				if ( !(EditorUtility.InstanceIDToObject( id ) as GameObject) ) return true;
				var go = EditorUtility.InstanceIDToObject(id) as GameObject;
				if ( go.scene != s ) return true;
				return false;
			}
			) );

			//	var data = adapter.m_data.GetValue(treeView, null);
			//	adapter.m_SetExpandedIDs.Invoke(data, new[] { result.ToArray() });
			m_SetExpandedIDs( adapter.data_currentTree ).Invoke( adapter.data_currentTree, new[] { result.ToArray() } );


			adapter.RepaintWindow( 0, true );
		}




	}
}
