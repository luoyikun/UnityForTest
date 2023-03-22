using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using EMX.HierarchyPlugin.Editor.Mods;

namespace EMX.HierarchyPlugin.Editor
{




	class HierarchyTempGlobalData : ScriptableObject
	{


		[Serializable]
		public class UndoState
		{
			[SerializeField]
			public int undoType;

			internal void CopyFrom( UndoState value )
			{
				undoType = value.undoType;
			}

			internal void Reset()
			{
				undoType = 0;
			}
		}

		[SerializeField]
		public List<UndoState> UNDO_LIST=  new List<UndoState>();




		//[Serializable]
		//internal class UndoData
		//{
		//    [SerializeField]
		//    internal HierarchyTempSceneData tempSceneData;
		//    // [SerializeField]
		//    // internal int undoType;
		//
		//    internal void CopyFrom( UndoData value )
		//    {
		//        tempSceneData = value/*.tempSceneData*/;
		//        //    undoType = value.undoType;
		//    }
		//}


		[SerializeField]
		List<HierarchyTempSceneData> SessionUndoTempSceneDataList = new List<HierarchyTempSceneData>();

		[NonSerialized]
		bool wasInit = false;
		SortedList<int,HierarchyTempSceneData> l;
		void TryToInit()
		{
			if ( !wasInit )
			{
				wasInit = true;
				l = new SortedList<int, HierarchyTempSceneData>();
				for ( int i = SessionUndoTempSceneDataList.Count - 1; i >= 0; i-- )
				{
					if ( !SessionUndoTempSceneDataList[ i ]/*.tempSceneData*/ ) SessionUndoTempSceneDataList.RemoveAt( i );
					else
					{
						var item = SessionUndoTempSceneDataList[ i ];
						l.Add( item/*.tempSceneData*/.GetInstanceID(), item );
						item.UNDO.APPLY_TO_CACHE();
						item.BOOKMARK.APPLY_TO_CACHE();
						item.EXPAND.APPLY_TO_CACHE();
						item.LAST.APPLY_TO_CACHE();
						item.SCENE.APPLY_TO_CACHE();
						//item/*.tempSceneData*/.UNDO_STATE_CACHE = item/*.tempSceneData*/.UNDO_STATE;
						//item/*.tempSceneData*/.BOOKMARKS_STATE_CACHE = item/*.tempSceneData*/.BOOKMARKS_STATE;
					}
				}

			}
		}


		internal void PutScene( HierarchyTempSceneData data )
		{
			int i;
			TryToInit();
			bool D = false;

			if ( !l.ContainsKey( data.GetInstanceID() ) )
			{
				// data.GET_UNDO();
				l.Add( data.GetInstanceID(), data );
				if ( SessionUndoTempSceneDataList.Count != l.Count )
				{
					while ( l.Count > SessionUndoTempSceneDataList.Count ) SessionUndoTempSceneDataList.Add( null );
					while ( l.Count < SessionUndoTempSceneDataList.Count ) SessionUndoTempSceneDataList.RemoveAt( SessionUndoTempSceneDataList.Count - 1 );
				}
				D = true;
			}

			i = 0;
			foreach ( var item in l )
			{
				// if ( SessionUndoTempSceneDataList[ i ] == null ) SessionUndoTempSceneDataList[ i ] = new HierarchyTempSceneData();
				if ( SessionUndoTempSceneDataList[ i ]/*.tempSceneData*/ != item.Value/*.tempSceneData*/ )
				{
					SessionUndoTempSceneDataList[ i ] = item.Value;//.CopyFrom( item.Value );
																   //SessionUndoTempSceneDataList[ i ].CopyFrom( item.Value );
					D = true;
				}

				i++;
			}
			if ( D )
				EditorUtility.SetDirty( this );
			//i = 0;
			//foreach ( var item in l )
			//{
			//    if ( item.Value.undoType != 0 && (SessionUndoTempSceneDataList[ i ].undoType & item.Value.undoType) == 0 )
			//    {
			//        SessionUndoTempSceneDataList[ i ].undoType |= item.Value.undoType;
			//        EditorUtility.SetDirty( this );
			//    }
			//    i++;
			//}
		}



		static void PerformUndo()
		{
			bool changes = false;
			foreach ( var item in HierarchyTempGlobalData.Instance().SessionUndoTempSceneDataList )
			{
				if ( item/*.tempSceneData*/.UNDO.STATE != item/*.tempSceneData*/.UNDO.STATE_CACHE )
				{
					changes = true;
					//item/*.tempSceneData*/.UNDO_STATE_CACHE = item/*.tempSceneData*/.UNDO_STATE;
					item.UNDO.APPLY_TO_CACHE();
					HierarchyTempSceneData.RemoveCache( item/*.tempSceneData*/.SceneHashCode );

					// var t = item/*.tempSceneData*/.GET_NEAR_UNDO_TYPE();
					//if ( item/*.tempSceneData*/.BOOKMARKS_STATE != item/*.tempSceneData*/.BOOKMARKS_STATE_CACHE ) //bookmarks
					//{
					//    item/*.tempSceneData*/.BOOKMARKS_STATE_CACHE = item/*.tempSceneData*/.BOOKMARKS_STATE;
					//    if ( DrawButtonsOld.cached_categories.ContainsKey( item/*.tempSceneData*/.SceneHashCode ) )
					//    {
					//        DrawButtonsOld.cached_categories[ item/*.tempSceneData*/.SceneHashCode ].Remove( (int)MemType.Custom );
					//    }
					//    Root.p[ 0 ].RepaintExternalNow();
					//}
#if !EMX_H_LITE

					if ( item.BOOKMARK.IS_DIF )
					{
						if ( DrawButtonsOld.cached_categories.ContainsKey( item/*.tempSceneData*/.SceneHashCode ) )
						{
							var t = DrawButtonsOld.cached_categories[ item/*.tempSceneData*/.SceneHashCode ];
							var p = (int)MemType.Custom;
							var L = Mathf.Max(item.BOOKMARK.MAX_VAL,1);
							for ( int i = 0; i < L; i++ )
								t.Remove( p + i );
						}
						Root.p[ 0 ].RepaintExternalNow();
						item.BOOKMARK.APPLY_TO_CACHE();
					}
					if ( item.EXPAND.IS_DIF )
					{
						if ( DrawButtonsOld.cached_categories.ContainsKey( item/*.tempSceneData*/.SceneHashCode ) )
							DrawButtonsOld.cached_categories[ item/*.tempSceneData*/.SceneHashCode ].Remove( (int)MemType.Hier );
						Root.p[ 0 ].RepaintExternalNow();
						item.EXPAND.APPLY_TO_CACHE();
					}
					if ( item.LAST.IS_DIF )
					{
						if ( DrawButtonsOld.cached_categories.ContainsKey( item/*.tempSceneData*/.SceneHashCode ) )
							DrawButtonsOld.cached_categories[ item/*.tempSceneData*/.SceneHashCode ].Remove( (int)MemType.Last );
						Root.p[ 0 ].RepaintExternalNow();
						item.LAST.APPLY_TO_CACHE();
					}
					if ( item.SCENE.IS_DIF )
					{
						if ( DrawButtonsOld.cached_categories.ContainsKey( item/*.tempSceneData*/.SceneHashCode ) )
							DrawButtonsOld.cached_categories[ item/*.tempSceneData*/.SceneHashCode ].Remove( (int)MemType.Scenes );
						Root.p[ 0 ].RepaintExternalNow();
						item.SCENE.APPLY_TO_CACHE();
					}
#endif
				}
			}
			if ( changes )
			{
				Root.p[ 0 ].RESET_DRAWSTACK( 0 );
				Root.p[ 0 ].RESET_DRAWSTACK( 1 );
			}
		}

		internal static void SubscribeUndoActions( EditorSubscriber sbs )
		{
			if ( Root.p[ 0 ].par_e.USE_UNDO_FOR_PLUGIN_MODULES )
			{
				sbs.OnUndoAction += PerformUndo;
				HierarchyTempGlobalData.Instance().TryToInit();
			}
		}


		//const string  TypeName ="HierarchyTempData.asset";
		internal static Func<HierarchyTempGlobalData> Instance = () =>
		{
			Folders.CheckFolders();
			return  ( Instance = ()=>
			{
				if (_Instance) return _Instance;
				var g = SessionState.GetInt(Folders.PREFS_PATH + "|SObjGUID" + Folders.Clearably.HierarchyTempData_TypeName, -1);
				if (g != -1 && (InternalEditorUtility.GetObjectFromInstanceID( g ) as HierarchyTempGlobalData))
				{
					Folders.CheckFolders(true);
					return (_Instance = InternalEditorUtility.GetObjectFromInstanceID( g ) as HierarchyTempGlobalData);
				}


				var preCache = ScriptableObject.CreateInstance<HierarchyTempGlobalData>();
				preCache.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.DontUnloadUnusedAsset;

				SessionState.SetInt(Folders.PREFS_PATH + "|SObjGUID" + Folders.Clearably.HierarchyTempData_TypeName, preCache.GetInstanceID());
				return (_Instance = preCache);
			})();
		};
		static HierarchyTempGlobalData _Instance;




	}


}
