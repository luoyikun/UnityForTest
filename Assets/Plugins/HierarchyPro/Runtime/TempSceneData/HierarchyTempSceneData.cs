#if UNITY_EDITOR
//#define DEBUG_ONLY
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditorInternal;
using System.Text;
using UnityEditor.SceneManagement;

//This class using only for the current editor session and objects will not save to the scene asset. 
//Just that the Unity engine requires that the MonoBehaviour scripts places outside the editor folder, even it using only for editor.

namespace EMX.HierarchyPlugin.Editor
{


	[Serializable]
	public class UndoIncrementalClass
	{
		const int L50 = 49;
		[SerializeField]
		public int STATE;
			[SerializeField]
		public int MAX_VAL = -1;

		public void INC_UNDO(int maxval )
		{
			if ( maxval > MAX_VAL ) MAX_VAL = maxval;
			INC_UNDO();
		}
		public void INC_UNDO()
		{
			STATE++;
			if ( STATE >= L50 ) STATE = 0;
			STATE_CACHE = STATE;
			//Debug.Log( "SET"+ UNDO_STATE );
		}
		[NonSerialized]
		public int STATE_CACHE = 0;

		public void APPLY_TO_CACHE() { STATE_CACHE = STATE; }
		public bool IS_DIF { get { return STATE_CACHE != STATE; } }
	}

	public class HierarchyTempSceneData : MonoBehaviour
	{
		[SerializeField]
		public UndoIncrementalClass UNDO= new UndoIncrementalClass();
		[SerializeField]
		public UndoIncrementalClass BOOKMARK= new UndoIncrementalClass();
		[SerializeField]
		public UndoIncrementalClass SCENE= new UndoIncrementalClass();
		[SerializeField]
		public UndoIncrementalClass LAST= new UndoIncrementalClass();
		[SerializeField]
		public UndoIncrementalClass EXPAND= new UndoIncrementalClass();


		//public UndoState GET_UNDO()
		//{
		//    while ( UNDO_STATE >= UNDO_LIST.Count ) UNDO_LIST.Add( new UndoState() );
		//    UNDO_LIST[ UNDO_STATE ].Reset();
		//    return UNDO_LIST[ UNDO_STATE ];
		//}


		//public int GET_NEAR_UNDO_TYPE()
		//{
		//    //Debug.Log( UNDO_STATE );
		//
		//    Debug.Log( "GET" + UNDO_STATE );
		//    var res = UNDO_LIST[ UNDO_STATE ].undoType;
		//    var i = UNDO_STATE - 1;
		//    if ( i >= 0 ) res |= UNDO_LIST[ i ].undoType;
		//    if ( i < 0 && L50 < UNDO_LIST.Count ) res |= UNDO_LIST[ L50 ].undoType;
		//    i += 2;
		//    if ( i < UNDO_LIST.Count ) res |= UNDO_LIST[ i ].undoType;
		//    return res;
		//}



		[SerializeField]
		public List<HierExpands_Temp> LastSelection_Temp = new List<HierExpands_Temp>();

		[SerializeField]
		public bool BookMarkCategory_Temp_WasInit = false;
		[SerializeField]
		public List<BookMarkCategory_Temp> BookMarkCategory_Temp = new List<BookMarkCategory_Temp>();

		[SerializeField]
		public bool HierExpands_Temp_WasInit = false;
		[SerializeField]
		public List<HierExpands_Temp> HierExpands_Temp = new List<HierExpands_Temp>();


		public const string GO_NAME = "HierarchyTempData";
		public const HideFlags FLAGS =
			HideFlags.DontSave
			| HideFlags.DontSaveInBuild 
            //| HideFlags.DontSaveInEditor 
            | HideFlags.HideInHierarchy;

		public static void RemoveCache()
		{
			if ( Application.isPlaying ) return;
			for ( int i = 0; i < EditorSceneManager.sceneCount; i++ )
			{
				var s = EditorSceneManager.GetSceneAt(i);
				var inst = InstanceSlow(s);
				DestroyImmediate( inst );
				_GetObjectData.Clear();
				_GetScenePath.Clear();
			}
		}
		//EMX_TODO undo performance clears only modified objects
		public static void RemoveCache( int sh )
		{
			_GetObjectData[ sh ].Clear();
			//_GetScenePath.Clear();
		}

		//public const HideFlags FLAGS = HideFlags.DontSave | HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;

		public int SceneHashCode;
		public TempSceneTypeList[] tempSceneTypesList = new TempSceneTypeList[0];
		// static HierarchyTempSceneData _i;
		public static HierarchyTempSceneData InstanceFast( Scene s )
		{
			var sh = s.GetHashCode();
			if ( !_Inctances.ContainsKey( sh ) ) _Inctances.Add( sh, null );
			if ( !(_T = _Inctances[ sh ]) ) _T = _Inctances[ sh ] = InstanceSlow( s );
			_T.SceneHashCode = sh;
			return _T;
		}
		public static HierarchyTempSceneData InstanceFast( int sh )
		{
			if ( !_Inctances.ContainsKey( sh ) ) _Inctances.Add( sh, null );
			if ( !(_T = _Inctances[ sh ]) ) _T = _Inctances[ sh ] = InstanceSlow( sh );
			_T.SceneHashCode = sh;
			return _T;
		}
		public static HierarchyTempSceneData InstanceSlow( Scene s )
		{
			// var l = SceneManager.GetActiveScene().GetRootGameObjects().LastOrDefault();
			var oldID = SessionState.GetInt("HierarchyTempSceneData" + s.GetHashCode(), -1);
			GameObject l = EditorUtility.InstanceIDToObject(oldID) as GameObject;
			/*
			foreach (var l2 in s.GetRootGameObjects())
			{
				if (l2 && (l2.hideFlags & FLAGS) == FLAGS && l2.name == GO_NAME) { l = l2; break; }
			}*/

			if ( !l )
			{
				l = new GameObject( GO_NAME );
				l.hideFlags = FLAGS;
				SessionState.SetInt( "HierarchyTempSceneData" + s.GetHashCode(), l.GetInstanceID() );
			}
			HierarchyTempSceneData _T;
			if ( l.GetComponent<HierarchyTempSceneData>() ) _T = l.GetComponent<HierarchyTempSceneData>();
			else _T = l.AddComponent<HierarchyTempSceneData>();
			_T.SceneHashCode = s.GetHashCode();
			return _T;
		}
		public static HierarchyTempSceneData InstanceSlow( int sh )
		{
			// var l = SceneManager.GetActiveScene().GetRootGameObjects().LastOrDefault();
			var oldID = SessionState.GetInt("HierarchyTempSceneData" + sh, -1);
			GameObject l = EditorUtility.InstanceIDToObject(oldID) as GameObject;
			/*
			foreach (var l2 in s.GetRootGameObjects())
			{
				if (l2 && (l2.hideFlags & FLAGS) == FLAGS && l2.name == GO_NAME) { l = l2; break; }
			}*/

			if ( !l )
			{
				l = new GameObject( GO_NAME );
				l.hideFlags = FLAGS;
				SessionState.SetInt( "HierarchyTempSceneData" + sh, l.GetInstanceID() );
			}
			//f ( l.GetComponent<HierarchyTempSceneData>() ) return l.GetComponent<HierarchyTempSceneData>();
			//eturn l.AddComponent<HierarchyTempSceneData>();
			HierarchyTempSceneData _T;
			if ( l.GetComponent<HierarchyTempSceneData>() ) _T = l.GetComponent<HierarchyTempSceneData>();
			else _T = l.AddComponent<HierarchyTempSceneData>();
			_T.SceneHashCode = sh;
			return _T;
		}
		static Dictionary<int, HierarchyTempSceneData> _Inctances = new Dictionary<int, HierarchyTempSceneData>();
		static HierarchyTempSceneData _T;




		GUID? _CURRENT_assetGUID;
		GUID CURRENT_assetGUID {
			get {
				return _CURRENT_assetGUID ?? (_CURRENT_assetGUID = GlobalObjectId.GetGlobalObjectIdSlow( gameObject ).assetGUID).Value;
			}
		}


		// LINKS
		const string GlobalIdVerstion = "GlobalObjectId_V1";
		public static Dictionary<int, Dictionary<int, Dictionary<int, TempSceneObjectPTR>>> _GetObjectData = new Dictionary<int, Dictionary<int, Dictionary<int, TempSceneObjectPTR>>>();
		public static Dictionary<int, Dictionary<int, string>> _GetScenePath = new Dictionary<int, Dictionary<int, string>>();
		public static TempSceneObjectPTR tryGet;

		internal static void ClearCache(Scene s)
        {
			var sh = s.GetHashCode();
			_GetObjectData.Remove(sh);
			_GetScenePath.Remove(sh);

		}


		public static Dictionary<int, TempSceneObjectPTR> GetAllObjectData( SaverType type, Scene scene )
		{
			var t = (int)type;
			if ( _GetObjectData.ContainsKey( scene.GetHashCode() ) && _GetObjectData[ scene.GetHashCode() ].ContainsKey( t ) ) return _GetObjectData[ scene.GetHashCode() ][ t ];
			if ( !scene.IsValid() ) return new Dictionary<int, TempSceneObjectPTR>();
			__( ref scene, t );
			return _GetObjectData[ scene.GetHashCode() ][ t ];
		}

		/*public static Dictionary<int, TempSceneObjectPTR> GetAllObjectData(int t, Scene scene)
		{

			if (t < 100) throw new Exception(t.ToString());
			if (_GetObjectData.ContainsKey(scene.GetHashCode()) && _GetObjectData[scene.GetHashCode()].ContainsKey(t)) return _GetObjectData[scene.GetHashCode()][t];
			if (!scene.IsValid()) return new Dictionary<int, TempSceneObjectPTR>();
			__(ref scene, t);
			return _GetObjectData[scene.GetHashCode()][t];
		}*/



		public static void _SetAllObjectData( SaverType type, Scene scene, Dictionary<int, TempSceneObjectPTR> data )
		{

			var t = (int)type;
			if ( !_GetObjectData.ContainsKey( scene.GetHashCode() ) )
			{
				_GetObjectData.Add( scene.GetHashCode(), new Dictionary<int, Dictionary<int, TempSceneObjectPTR>>() );
				_GetScenePath.Add( scene.GetHashCode(), new Dictionary<int, string>() );
			}
			_GetObjectData[ scene.GetHashCode() ].Remove( t );
			_GetObjectData[ scene.GetHashCode() ].Add( t, data );

			_GetScenePath[ scene.GetHashCode() ].Remove( t );
			_GetScenePath[ scene.GetHashCode() ].Add( t, scene.path );


			//	&& _GetObjectData[scene.GetHashCode()].ContainsKey(t)) return _GetObjectData[scene.GetHashCode()][t];
			//if (!scene.IsValid()) return new Dictionary<int, TempSceneObjectPTR>();
			//__(ref scene, t);
			//	return _GetObjectData[scene.GetHashCode()][t];
		}



		public static bool CompareArrays<T>( T[] a1, T[] a2 ) where T : MYEQ
		{
			if ( (a1 == null || a2 == null) && (a1 != null || a2 != null) ) return false;
			if ( a1 == null && a2 == null ) return true;
			if ( a1.Length != a2.Length ) return false;
			for ( int i = 0; i < a1.Length; i++ )
			{
				if ( (a1[ i ] == null || a2[ i ] == null) && (a1[ i ] != null || a2[ i ] != null) ) return false;
				if ( a1[ i ] == null && a2[ i ] == null ) continue;
				if ( !a1[ i ].EQUALS( a2[ i ] ) ) return false;
			}
			return true;
		}
		public static bool CompareArrays( ref string[] a1, ref string[] a2 )
		{
			if ( (a1 == null || a2 == null) && (a1 != null || a2 != null) ) return false;
			if ( a1 == null && a2 == null ) return true;
			if ( a1.Length != a2.Length ) return false;
			for ( int i = 0; i < a1.Length; i++ )
			{
				if ( (a1[ i ] == null || a2[ i ] == null) && (a1[ i ] != null || a2[ i ] != null) ) return false;
				if ( a1[ i ] == null && a2[ i ] == null ) continue;
				if ( a1[ i ] != (a2[ i ]) ) return false;
			}
			return true;


		}


		struct loadHelper
		{
			internal SavedObjectData data;
			internal savedGlobalId savedGL;
		}

		public static void __clearcache( ref Scene scene, int t )
		{
			var sh = scene.GetHashCode();
			if ( !_Inctances.ContainsKey( sh ) ) _Inctances.Add( sh, null );
			if ( !(_T = _Inctances[ sh ]) ) _T = _Inctances[ sh ] = InstanceSlow( scene );
			if ( t >= _T.tempSceneTypesList.Length ) Array.Resize( ref _T.tempSceneTypesList, t + 1 );
			if ( _T.tempSceneTypesList[ t ] == null ) _T.tempSceneTypesList[ t ] = new TempSceneTypeList();
			_T.tempSceneTypesList[ t ].WasSceneScan = false;
		}

		public static void __( ref Scene scene, int t, bool skipLoad = false )
		{

			var sh = scene.GetHashCode();
			if ( !_Inctances.ContainsKey( sh ) ) _Inctances.Add( sh, null );
			if ( !(_T = _Inctances[ sh ]) ) _T = _Inctances[ sh ] = InstanceSlow( scene );
			if ( t >= _T.tempSceneTypesList.Length ) Array.Resize( ref _T.tempSceneTypesList, t + 1 );
			if ( _T.tempSceneTypesList[ t ] == null ) _T.tempSceneTypesList[ t ] = new TempSceneTypeList();
			if ( (_GetScenePath.ContainsKey( sh ) && _GetScenePath[ sh ].ContainsKey( t ) && _GetScenePath[ sh ][ t ] != scene.path) && _T.tempSceneTypesList[ t ].WasSceneScan ) SaveOnScenePathChanged( t, scene );

			if ( !_GetObjectData.ContainsKey( sh ) ) _GetObjectData.Add( sh, new Dictionary<int, Dictionary<int, TempSceneObjectPTR>>() );
			if ( !_GetScenePath.ContainsKey( sh ) ) _GetScenePath.Add( sh, new Dictionary<int, string>() );
			if ( !_GetScenePath[ sh ].ContainsKey( t ) ) _GetScenePath[ sh ].Add( t, scene.path );

			bool skipSiblings = (SaverType)t == SaverType.PresetsManager;

			if ( !_T.tempSceneTypesList[ t ].WasSceneScan )
			{

				if ( skipLoad ) return;

				_T.tempSceneTypesList[ t ].WasSceneScan = true;

				//LOAD DATA
				var LAODED = HierarchyExternalSceneData.LoadObjects((SaverType)t, scene);
				UnityEngine.Object[] result = new UnityEngine.Object[0];
				if ( LAODED.Length != 0 )
				{
					//FIND OBJECTS
					var converted = LAODED.Select(l =>
					{
						var helper = new loadHelper() { data = l };
						_getSavedGlobalIdByGL.TryGetValue(l.identifierTypetargetObjectIdtargetPrefabId, out helper.savedGL);
						return helper;
					}).ToArray();


					///var notAssigned =  converted.Where(c=>c.savedGL == null).ToArray();
					var notAssigned = converted.Where(c => c.savedGL == null || !c.savedGL.callBackObject).ToArray();
					//var assigned = converted.Where(c=>c.savedGL != null).ToArray();
					var assigned = converted.Where(c => c.savedGL != null && c.savedGL.callBackObject).ToArray();
					UnityEngine.Object[] temp = new UnityEngine.Object[0];

					if ( notAssigned.Length != 0 )
					{ //FIXED
					  //var currentGUI = _T.CURRENT_assetGUID.ToString();
						GlobalObjectId ps;
						var parsed = notAssigned.Select(s1 =>
												 {
													 var s = s1.data;
													 //Debug.Log(string.Concat(GlobalIdVerstion,s.identifierTypetargetObjectIdtargetPrefabId));
													 //Debug.Log(_T.CURRENT_assetGUID);
													 GlobalObjectId.TryParse(string.Concat(GlobalIdVerstion, s.identifierTypetargetObjectIdtargetPrefabId), out ps);
													 return ps;
												 }
												 ).ToArray();
						Array.Resize( ref temp, parsed.Length );
						GlobalObjectId.GlobalObjectIdentifiersToObjectsSlow( parsed, temp );

					}
					Array.Resize( ref result, assigned.Length + temp.Length );
					//  UnityEngine.Object[] result = new UnityEngine.Object[assigned.Length + temp.Length];
					for ( int i = 0; i < assigned.Length; i++ ) result[ i ] = assigned[ i ].savedGL.callBackObject;
					for ( int i = assigned.Length; i < result.Length; i++ ) result[ i ] = temp[ i - assigned.Length ];

					//TRY TO FIX MISSINGS
					GameObject[] tempRoots = null;
					Transform currentRoot = null;

					if ( !skipSiblings )
						for ( int i = assigned.Length; i < result.Length; i++ )
						{
							if ( !result[ i ] )
							{
								if ( tempRoots == null ) tempRoots = scene.GetRootGameObjects();
								var names = LAODED[i].namesPathBackup.Split('¾');
								var siblings = LAODED[i].siblingsPathBackup.Split('¾').Select(s => int.Parse(s)).ToArray();
								if ( names.Length == 0 )
								{
									Debug.LogWarning( "names.Length == 0" );
									continue;
								}
								if ( names.Length != siblings.Length )
								{
									Debug.LogWarning( "names.Length != siblings.Length" );
									continue;
								}
								var wrong = 0;
								var deep = 0;
								if ( siblings[ deep ] >= tempRoots.Length ) siblings[ deep ] = Index( ref tempRoots, ref names[ deep ], -1 );
								if ( siblings[ deep ] == -1 ) break;
								var root_i = siblings[ deep ];
								if ( tempRoots[ root_i ].name != names[ deep ] )
								{
									var new_root_i = Index(ref tempRoots, ref names[deep], root_i);
									if ( new_root_i != -1 ) root_i = new_root_i;
									else wrong++;
								}
								currentRoot = tempRoots[ root_i ].transform;
								deep++;
								while ( deep < siblings.Length )
								{


									if ( siblings[ deep ] >= currentRoot.childCount ) siblings[ deep ] = Index( currentRoot, ref names[ deep ], -1 );
									if ( siblings[ deep ] >= currentRoot.childCount )
									{
										Debug.LogWarning( "Cannot Read Scene Data: " + scene.name );
										goto br;
									}
									if ( siblings[ deep ] == -1 ) goto br;
									if ( currentRoot.GetChild( siblings[ deep ] ).name != names[ deep ] )
									{
										var s = Index(currentRoot, ref names[deep], siblings[ deep ]);
										if ( s != -1 )
										{
											siblings[ deep ] = s;
											if ( siblings[ deep ] >= currentRoot.childCount )
											{
												Debug.LogWarning( "Cannot Read Scene Data: " + scene.name );
												goto br;
											}
										}
										else wrong++;
										if ( wrong > 2 ) break;
									}
									currentRoot = currentRoot.GetChild( siblings[ deep ] );
									deep++;
								}

								if ( wrong < 2 || currentRoot.name == names[ names.Length - 1] )
									result[ i ] = currentRoot.gameObject;
							}
br:;
						}
				}


				//FINALIZATION
				TempSceneObjectPTR[] ass = new TempSceneObjectPTR[LAODED.Length];
				for ( int i = 0; i < ass.Length; i++ )
				{
					ass[ i ] = new TempSceneObjectPTR( result[ i ], LAODED[ i ].id_in_external_heap );
					ass[ i ].CopyFrom( LAODED[ i ] );
				}
				_T.tempSceneTypesList[ t ].objects = ass;


				//CREATE CACHE

			}


			var res = new Dictionary<int, TempSceneObjectPTR>();
			foreach ( var item in _T.tempSceneTypesList[ t ].objects )
			{
				if ( !item._target ) continue;
				var id = item._target.GetInstanceID();
				if ( !res.ContainsKey( id ) ) res.Add( id, item );
			}
			_GetObjectData[ scene.GetHashCode() ].Add( t, res );
		}

		static int Index( ref GameObject[] g, ref string s, int start )
		{
			if ( start == -1 ) start = g.Length - 1;
			for ( int i = 0; i < g.Length; i++ ) if ( g[ (i + start) % g.Length ].name == s ) return (i + start) % g.Length; return -1;
		}
		static int Index( Transform g, ref string s, int start )
		{
			if ( start == -1 ) start = g.childCount - 1;
			for ( int i = 0; i < g.childCount; i++ ) if ( g.GetChild( (i + start) % g.childCount ).name == s ) return (i + start) % g.childCount; return -1;
		}

		public static void SaveToExternalFile( SaverType type, Scene scene )
		{
			var t = (int)type;
			SaveToExternalFile( t, scene );

		}


		static internal void SaveOnScenePathChanged( int i, Scene scene )
		{
			SaveToExternalFile( i, scene, true );
		}
		static public void SaveOnScenePathChanged( Scene scene )
		{
			if ( Application.isPlaying ) return;
			var sh = scene.GetHashCode();
			for ( int i = 0; i < _T.tempSceneTypesList.Length; i++ )
			{
				if ( (_GetScenePath.ContainsKey( sh ) && _GetScenePath[ sh ].ContainsKey( i ) && _GetScenePath[ sh ][ i ] != scene.path) && _T.tempSceneTypesList[ i ].WasSceneScan )
					SaveToExternalFile( i, scene, true, true );
			}
		}

		public static void SaveToExternalFile( int t, Scene scene, bool skipCache = false , bool saveOnSceneSaved = false)
		{
			if ( !scene.IsValid() ) return;

			var sh = scene.GetHashCode();
			if ( !_GetScenePath.ContainsKey( sh ) ) _GetScenePath.Add( sh, new Dictionary<int, string>() );
			if ( !_GetScenePath[ sh ].ContainsKey( t ) ) _GetScenePath[ sh ].Add( t, scene.path );
			_GetScenePath[ sh ][ t ] = scene.path;


			TempSceneObjectPTR[] target = null;
			if ( !skipCache )
			{
				if ( !_GetObjectData.ContainsKey( scene.GetHashCode() ) || !_GetObjectData[ scene.GetHashCode() ].ContainsKey( t ) ) __( ref scene, t, skipLoad: true );
				if ( !_GetObjectData[ scene.GetHashCode() ].ContainsKey( t ) ) return;
				var __target = _GetObjectData[scene.GetHashCode()][t];
				target = __target.Values.Where( g => g._target ).ToArray();

				if ( t >= _T.tempSceneTypesList.Length ) Array.Resize( ref _T.tempSceneTypesList, t + 1 );
				if ( _T.tempSceneTypesList[ t ] == null ) _T.tempSceneTypesList[ t ] = new TempSceneTypeList();
				_T.tempSceneTypesList[ t ].objects = target;
			}
			else
			{
				if ( t >= _T.tempSceneTypesList.Length ) return;
				if ( !_T.tempSceneTypesList[ t ].WasSceneScan ) return;
				target = _T.tempSceneTypesList[ t ].objects.Where( g => g._target ).ToArray();
			}



#if DEBUG_ONLY
            foreach (var item in __target.Values)
            {
                if (!item._target) Debug.Log(item.stringValue);
            }
#endif
			var savedId = target.Select(g => { return getSavedGlobalId(g._target); }).ToArray();
			var GID_FIXER = savedId.Where(s => !s.InitGlobalID).ToArray();
			if ( GID_FIXER.Length != 0 )
			{
				var input = GID_FIXER.Select(fxr => fxr.callBackObject).ToArray();
				GlobalObjectId[] globalIds = new GlobalObjectId[input.Length];
				GlobalObjectId.GetGlobalObjectIdsSlow( input, globalIds );
				for ( int i = 0; i < globalIds.Length; i++ )
				{
					GID_FIXER[ i ].InitGlobalID = true;
					var s = globalIds[i].ToString();
					s = s.Substring( s.IndexOf( '-' ) );
					s = s.Substring( s.IndexOf( '-' ) );
					GID_FIXER[ i ].identifierTypetargetObjectIdtargetPrefabId = s;
					if ( !_getSavedGlobalIdByGL.ContainsKey( GID_FIXER[ i ].identifierTypetargetObjectIdtargetPrefabId ) )
						_getSavedGlobalIdByGL.Add( GID_FIXER[ i ].identifierTypetargetObjectIdtargetPrefabId, GID_FIXER[ i ] );
					else
						_getSavedGlobalIdByGL[ GID_FIXER[ i ].identifierTypetargetObjectIdtargetPrefabId ] = GID_FIXER[ i ];
				}
			}

			SavedObjectData[] res = new SavedObjectData[savedId.Length];
			for ( int i = 0; i < savedId.Length; i++ )
			{
				res[ i ] = new SavedObjectData( savedId[ i ].identifierTypetargetObjectIdtargetPrefabId.GetHashCode() ) {
					indentifierVersion = GlobalIdVerstion,
					identifierTypetargetObjectIdtargetPrefabId = savedId[ i ].identifierTypetargetObjectIdtargetPrefabId,
					saverType = t,
					namesPathBackup = savedId[ i ].namesPathBackup,
					siblingsPathBackup = savedId[ i ].siblingsPathBackup,

				};
				target[ i ].id_in_external_heap = res[ i ].id_in_external_heap;
				target[ i ].CopyTo( res[ i ] );
			}
			HierarchyExternalSceneData.WriteObjects( (SaverType)t, scene, res , saveOnSceneSaved);
		}





		internal class savedGlobalId
		{
			internal UnityEngine.Object callBackObject;
			internal bool InitGlobalID;
			internal string identifierTypetargetObjectIdtargetPrefabId, namesPathBackup, siblingsPathBackup;
		}
		static Dictionary<string, savedGlobalId> _getSavedGlobalIdByGL = new Dictionary<string, savedGlobalId>();
		static Dictionary<int, savedGlobalId> _getSavedGlobalIdByID = new Dictionary<int, savedGlobalId>();
		static savedGlobalId _getsavedGlobalId;
		//static savedGlobalId getSavedGlobalId(GameObject o)
		static savedGlobalId getSavedGlobalId( UnityEngine.Object o )
		{

			if ( _getSavedGlobalIdByID.TryGetValue( o.GetInstanceID(), out _getsavedGlobalId ) ) return _getsavedGlobalId;

			var go = o as GameObject;
			string siblings = "";
			string namesPathBackup = "";
			if ( go )
			{
				namesPathBackup = o.name;
				siblings = go.transform.GetSiblingIndex().ToString();
				Transform parent = go.transform.parent;
				while ( parent )
				{
					namesPathBackup = String.Concat( parent.name, "¾", namesPathBackup );
					siblings = String.Concat( parent.GetSiblingIndex().ToString(), "¾", siblings );
					parent = parent.parent;
				}
			}


			_getSavedGlobalIdByID.Add( o.GetInstanceID(), _getsavedGlobalId = new savedGlobalId() {
				callBackObject = o,
				InitGlobalID = false,
				namesPathBackup = namesPathBackup,
				siblingsPathBackup = siblings
			} );
			//_getSavedGlobalIdByGL.Add( _getsavedGlobalId.identifierTypetargetObjectIdtargetPrefabId, _getsavedGlobalId );

			return _getsavedGlobalId;
		}





	}


	public interface IBgColor
	{
		Color? BgColor { get; set; }
		string get_name { get; }
	}


	[Serializable]
	public class BookMarkCategory_Temp : IBgColor
	{
		public string get_name { get { return category_name; } }
		public Color? BgColor {
			get {
				//  if (isDefault && bgColor.)
				return bgColor;
			}
			set { bgColor = value.Value; }
		}

		public bool isDefault = false;
		public string _category_name;
		public string category_name {
			get {
				if ( isDefault ) return "Default";
				return _category_name;
			}
			set {
				if ( isDefault ) return;
				_category_name = Folders.FIX_NAME( value );
			}
		}
		//static public Color32 BgOverrideColor_default_dark = new Color32(0xB6,0xCC,0xD6, 127);
		static public Color32 BgOverrideColor_default_dark = new Color32(255,255,255, 0);
		static public Color32 BgOverrideColor_default_light = new Color32(255,255,255, 0);
		public void SetAsDefault( bool andResetValues = false )
		{
			if ( isDefault ) return;
			isDefault = true;
			if ( andResetValues )
			{
				//if (EditorGUIUtility.isProSkin) bgColor = new Color32( 0x82, 0x9C, 0xB9, 127 );
				//else bgColor = new Color( 0.87f, 0.87f, 0.87f, 1 );
				if ( EditorGUIUtility.isProSkin ) bgColor = BgOverrideColor_default_dark;
				else bgColor = BgOverrideColor_default_light;
			}
			_category_name = "Default";
		}

		public List<HierExpands_Temp> targets = new List<HierExpands_Temp>();
		public Color bgColor = Color.clear;
	}
	[Serializable]
	public class HierExpands_Temp
	{
		public string _name;
		public string name {
			get {
				return _name;
			}
			set {
				_name = Folders.FIX_NAME( value );
			}
		}
		public List<GameObject> targets = new List<GameObject>();
	}

	[Serializable]
	public class TempSceneTypeList
	{
		public bool WasSceneScan = false;
		public TempSceneObjectPTR[] objects = new TempSceneObjectPTR[0];
	}
	[Serializable]
	public class IconExternalData : ICloneable, MYEQ
	{
		public string icon_guid;
		public string icon_path;
		public bool has_icon_color;
		public Color icon_color;

		int id = 0;
		static int _id = 0;
		public IconExternalData()
		{
			id = _id++;
		}

		public static Dictionary<int, Texture2D> icon_cacher = new Dictionary<int, Texture2D>();

		object ICloneable.Clone()
		{
			return MemberwiseClone();
		}
		public IconExternalData Clone()
		{
			var res = (IconExternalData)MemberwiseClone();
			res.id = _id++;
			return res;
		}


		public Texture2D GetCheckedTexture()
		{

			if ( icon_cacher.ContainsKey( id ) ) return icon_cacher[ id ];
			if ( icon_guid == null || icon_guid == "" )
			{
				icon_cacher.Add( id, null );
			}
			else
			{
				bool _else = true;
				if ( icon_guid.StartsWith( "Library/", StringComparison.OrdinalIgnoreCase ) )
				{
					var resource = icon_guid.Substring(icon_guid.IndexOf('/') + 1);
					if ( (resource.StartsWith( "sv_icon_dot" ) || resource.StartsWith( "sv_label_" )) )
					{
						var t = EditorGUIUtility.IconContent(resource).image;
						icon_cacher.Add( id, t as Texture2D );
						_else = false;
					}
				}
				if ( _else )
				{
					var path = AssetDatabase.GUIDToAssetPath(icon_guid);

					if ( string.IsNullOrEmpty( path ) )
					{
						var newGuid = AssetDatabase.AssetPathToGUID(icon_path);
						if ( string.IsNullOrEmpty( newGuid ) )
						{
							icon_cacher.Add( id, null );
							goto skip;
						}
						icon_guid = newGuid;
						//adapter.SetDirtyActiveDescription((o.scene));
						path = icon_path;
					}
					if ( path != icon_path )
					{
						icon_path = path;
						//adapter.SetDirtyActiveDescription((o.scene));
					}
					var t = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
					icon_cacher.Add( id, t );
				}
skip:;
			}
			return icon_cacher[ id ];
		}

		public void SetExternalCheckedTexture( Texture2D texture )
		{
			icon_cacher.Remove( id );
			if ( !texture )
			{
				icon_path = "";
				icon_guid = "";
			}
			else
			{
				icon_path = AssetDatabase.GetAssetPath( texture );
				icon_guid = AssetDatabase.AssetPathToGUID( icon_path );
			}
		}
		public void SetExternalCheckedTexture( string guid, string path )
		{
			icon_cacher.Remove( id );
			if ( guid == null || guid == "" )
			{
				icon_path = "";
				icon_guid = "";
			}
			else
			{
				icon_path = path;
				icon_guid = guid;
			}
		}


		public bool EQUALS( object _d )
		{
			var d = _d as IconExternalData;
			if ( d == null ) return false;
			if ( icon_guid != d.icon_guid ) return false;
			if ( icon_path != d.icon_path ) return false;
			if ( has_icon_color != d.has_icon_color ) return false;
			if ( icon_color != d.icon_color ) return false;
			return true;
		}

		static StringBuilder sb = new StringBuilder();
		public string Save()
		{
			if ( (icon_guid == null || icon_guid == "") ) return null;
			sb.Clear();
			sb.AppendLine( 1.ToString() );
			sb.AppendLine( icon_guid );
			sb.AppendLine( icon_path );
			sb.AppendLine( has_icon_color.ToString() );
			var c = (Color32)icon_color;
			sb.AppendLine( string.Concat( c.r, " ", c.g, " ", c.b, " ", c.a ) );

			return Convert.ToBase64String( Encoding.UTF8.GetBytes( sb.ToString() ) );
		}
		static Color32 cr;
		public void Load( string b64 )
		{
			var sr = new StringReader(Encoding.UTF8.GetString(Convert.FromBase64String(b64)));
			sr.ReadLine(); //version
			icon_guid = (sr.ReadLine());
			icon_path = (sr.ReadLine());
			has_icon_color = bool.Parse( sr.ReadLine() );
			var split = sr.ReadLine().Split(' ');
			cr.r = (byte)int.Parse( split[ 0 ] );
			cr.g = (byte)int.Parse( split[ 1 ] );
			cr.b = (byte)int.Parse( split[ 2 ] );
			cr.a = (byte)int.Parse( split[ 3 ] );
			icon_color = cr;
		}


	}

	[Serializable]
	public class HighlighterExternalData : MYEQ
	{

		public string[] TempColorData = new string[0];

		public HighlighterExternalData Clone()
		{
			var res = new HighlighterExternalData();
			res.TempColorData = TempColorData.ToArray();
			return res;
		}


		public bool EQUALS( object _d )
		{
			var d = _d as HighlighterExternalData;
			if ( d == null ) return false;
			if ( (TempColorData == null || d.TempColorData == null) && (TempColorData != null || d.TempColorData != null) ) return false;
			if ( TempColorData == null && d.TempColorData == null ) return true;
			if ( TempColorData.Length != d.TempColorData.Length ) return false;
			for ( int i = 0; i < TempColorData.Length; i++ )
				if ( TempColorData[ i ] != d.TempColorData[ i ] ) return false;
			return true;
		}

		static StringBuilder sb = new StringBuilder();
		public string Save()
		{
			if ( TempColorData.Length == 0 ) return null;
			sb.Clear();
			sb.AppendLine( 1.ToString() );
			for ( int i = 0; i < TempColorData.Length; i++ )
			{
				if ( i != 0 ) sb.Append( ' ' );
				sb.Append( TempColorData[ i ] == "" || TempColorData[ i ] == null ? "0" : TempColorData[ i ] );
			}
			return Convert.ToBase64String( Encoding.UTF8.GetBytes( sb.ToString() ) );
		}
		//static Color32 cr;
		public void Load( string b64 )
		{
			var sr = new StringReader(Encoding.UTF8.GetString(Convert.FromBase64String(b64)));
			sr.ReadLine(); //version
			var arr = sr.ReadLine();
			if ( arr != null && arr != "" )
			{
				var split = arr.Split(' ');
				TempColorData = new string[ split.Length ];
				for ( int i = 0; i < split.Length; i++ ) TempColorData[ i ] = (split[ i ]); //int.Parse
			}
		}
	}

	public interface MYEQ
	{
		bool EQUALS( object o );
	}


	[Serializable]
	public class TempSceneObjectPTR
	{

		public TempSceneObjectPTR( UnityEngine.Object t, int id_in_external_heap ) { _target = t; this.id_in_external_heap = id_in_external_heap; }

		public int id_in_external_heap;
		//  public bool wasInit ;
		public UnityEngine.Object _target;
		public GameObject target { get { return _target as GameObject; } }

		////	[NonSerialized]
		//internal bool changed;
		public string guid, path;
		//public string data;
		//#if DEBUG_ONLY
		// public bool boolValue { get; private set; }
		//  public string stringValue { get; private set; }
		//#else
		public bool boolValue;
		public string stringValue;
		public HighlighterExternalData[] highLighterData = new HighlighterExternalData[0];
		public IconExternalData[] iconData = new IconExternalData[0];

		public enum EQUALS_TYPE { Project }
		public bool EQUALS( TempSceneObjectPTR d, EQUALS_TYPE t )
		{
			if ( t == EQUALS_TYPE.Project )
			{
				if ( !HierarchyTempSceneData.CompareArrays<HighlighterExternalData>( highLighterData, d.highLighterData ) ) return false;
				if ( !HierarchyTempSceneData.CompareArrays<IconExternalData>( iconData, d.iconData ) ) return false;
				return true;
			}
			return false;
		}


		//#endif
		static char[] trim = { '\r', '\n', ' ' };
		static StringBuilder sb = new StringBuilder();
		public bool SetString( List<int> arr )
		{
			var res = false;
			if ( arr == null || arr.Count == 0 )
			{
				res = stringValue != "";
				stringValue = "";
				return res;
			}
			sb.Clear();
			for ( int i = 0; i < arr.Count; i++ )
			{
				sb.Append( arr[ i ].ToString() );
				sb.Append( ' ' );
			}
			var ex = sb.ToString();
			res = ex != stringValue;
			stringValue = ex; ;
			return res; ;
		}
		public bool HasList()
		{
			if ( string.IsNullOrEmpty( stringValue.Trim( trim ) ) ) return false;
			return true;
		}
		public List<int> GetIntList()
		{
			if ( string.IsNullOrEmpty( stringValue.Trim( trim ) ) ) return new List<int>();
			return stringValue.Trim( trim ).Split( ' ' ).Select( s => int.Parse( s ) ).ToList();
		}

		public void CopyFrom( SavedObjectData savedObjectData )
		{
			boolValue = savedObjectData.boolValue;
			stringValue = savedObjectData.stringValue;

			if ( savedObjectData.highLighterData.Length == 0 && highLighterData.Length != 0 ) Array.Resize( ref highLighterData, 0 );
			else if ( savedObjectData.highLighterData.Length != 0 ) highLighterData = savedObjectData.highLighterData.Select( h => h.Clone() ).ToArray();

			if ( savedObjectData.iconData.Length == 0 && iconData.Length != 0 ) Array.Resize( ref iconData, 0 );
			else if ( savedObjectData.iconData.Length != 0 ) iconData = savedObjectData.iconData.Select( h => h.Clone() ).ToArray();
		}
		public void CopyTo( SavedObjectData savedObjectData )
		{
			savedObjectData.boolValue = boolValue;
			savedObjectData.stringValue = stringValue;

			if ( highLighterData.Length == 0 && savedObjectData.highLighterData.Length != 0 ) Array.Resize( ref savedObjectData.highLighterData, 0 );
			else if ( highLighterData.Length != 0 ) savedObjectData.highLighterData = highLighterData.Select( h => h.Clone() ).ToArray();

			if ( iconData.Length == 0 && savedObjectData.iconData.Length != 0 ) Array.Resize( ref savedObjectData.iconData, 0 );
			else if ( iconData.Length != 0 ) savedObjectData.iconData = iconData.Select( h => h.Clone() ).ToArray();
		}
	}
}
#endif