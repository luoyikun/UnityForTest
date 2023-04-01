using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor
{
	class Cache
	{

		internal static Dictionary<string, HierarchyObject> m_PathToObject = new Dictionary<string, HierarchyObject>();
		//static Dictionary<string, SortedList<int, HierarchyObject>> m_PathToChildrens = new Dictionary<string, SortedList<int, HierarchyObject>>();






		internal static TempColorClass LOAD_CUSTOM_ICON_USING_HIGHLIGHTER( HierarchyObject o )
		{

#if !EMX_H_LITE
			var hm = o.pluginID == 0 ? Root.p[ 0 ].modsController.highLighterMod : Root.p[ 0 ].modsController.projectWindowExtensions.highLighterMod;
			return Mods.HighlighterCache_Icons.GetImageForObject_OnlyCacher( o, false, hm );
#else
     
            return null;
#endif
		}





		internal static void ClearProejctObjects()
		{
			m_PathToObject.Clear();
			m_GuidToHierObject.Clear();
			m_instanceIdToHierObject_1.Clear();
			//m_instanceIdToHierObject = m_instanceIdToHierObject.Values.Where(v => v.pluginID != 1).ToDictionary(k => k.id, v => v);
			m_instanceIdToPATH.Clear();
			m_instanceIdToGUID.Clear();
			Root.p[ 0 ].RESET_DRAWSTACK( 1 );
		}
		internal static void ClearProejctObjects( ref string[] importedAssets, ref string[] deletedAssets, ref string[] movedAssets, ref string[] movedFromAssetPaths )
		{
			for ( int i = 0; i < deletedAssets.Length; i++ )
				_Check_ClearProejctObjects( ref deletedAssets[ i ] );
			for ( int i = 0; i < movedAssets.Length; i++ )
				_Check_ClearProejctObjects( ref movedAssets[ i ] );
			for ( int i = 0; i < movedFromAssetPaths.Length; i++ )
				_Check_ClearProejctObjects( ref movedFromAssetPaths[ i ] );
			if ( deletedAssets.Length != 0 || movedAssets.Length != 0 || movedFromAssetPaths.Length != 0 )
				Root.p[ 0 ].RESET_DRAWSTACK( 1 );
		}
		static void _Check_ClearProejctObjects( ref string path )
		{
			if ( !m_PathToObject.ContainsKey( path ) ) return;
			var o  = m_PathToObject[path];

			if ( m_instanceIdToGUID.ContainsKey( o.id ) ) m_GuidToHierObject.Remove( m_instanceIdToGUID[ o.id ] );

			m_PathToObject.Remove( path );
			//m_GuidToHierObject.Clear();
			m_instanceIdToHierObject_1.Remove( o.id );
			m_instanceIdToPATH.Remove( o.id );
			m_instanceIdToGUID.Remove( o.id );
		}



		internal static void ClearHierarchyObjects( bool project )
		{ //  Utilities.ObjectContent_cache.Clear();
		  //  Utilities.ObjectContent_Objectcache.Clear();

			if ( project )
			{
				m_PathToObject.Clear();
				//m_PathToChildrens.Clear();
				m_GuidToHierObject.Clear();
				m_instanceIdToPATH.Clear();
				m_instanceIdToGUID.Clear();
				//m_instanceIdToHierObject = m_instanceIdToHierObject.Values.Where(v => v.pluginID != 1).ToDictionary(k => k.id, v => v);
				m_instanceIdToHierObject_1.Clear();
				Root.p[ 0 ].RESET_DRAWSTACK( 1 );
			}
			else
			{
				//m_instanceIdToHierObject = m_instanceIdToHierObject.Values.Where(v => v.pluginID != 0).ToDictionary(k => k.id, v => v);
				m_instanceIdToHierObject_0.Clear();
				Root.p[ 0 ].RESET_DRAWSTACK( 0 );
			}

			/* Utilities.cache_ObjectContent_byType.Clear();
            Utilities.cache_ObjectContent_byId.Clear();

            //Debug.Log( "Clea" );
            if ( clearObjects )
            {
                m_instanceIdToGUID.Clear();
                m_instanceIdToPATH.Clear();
                m_instanceIdToHierObject.Clear();
                m_GuidToHierObject.Clear();
                m_fakeList.Clear();
                scanned_folder.Clear();
            }


            if ( OnClearObjects != null ) OnClearObjects();


            ColorModule.ClearCacheAdditional();*/
		}

		//  internal static void ClearAdditionalCache( )
		//  {
		// ColorModule.ClearCacheAdditional();

		// }


		/*  internal static long GetFileIDWithOutPrefabChecking( UnityEngine.GameObject prefab, UnityEngine.GameObject gameObject )
          {
              //if ( pluginID != 0 ) throw new Exception( "GetFileIDWithOutPrefabChecking not implemented" );

              if ( cacheSaveToScriptableObject ?? (cacheSaveToScriptableObject = Hierarchy_GUI.Instance( this ).SaveToScriptableObject == "FOLDER").Value )
              {
                  if ( Hierarchy_GUI.HierarchySettings.PrefabIDMode == Hierarchy_GUI.PrefabIDModeEnum.MergedInstances )
                      return m_TryGetGUIDAndLocalFileIdentifier( prefab ?? gameObject );

                  return m_TryGetGUIDAndLocalFileIdentifier( prefab ?? gameObject );
              }

              else
              {
                  if ( Hierarchy_GUI.HierarchySettings.PrefabIDMode == Hierarchy_GUI.PrefabIDModeEnum.MergedInstances && prefab ) //  Debug.Log(prefab.name + " "  + m_TryGetGUIDAndLocalFileIdentifier(prefab ));
                  {
                      return m_TryGetGUIDAndLocalFileIdentifier( prefab );
                  }

                  return gameObject.GetInstanceID();
              }
          }
          static Dictionary<UnityEngine.Object, SerializedObject> cached_so = new Dictionary<UnityEngine.Object, SerializedObject>();
          static SerializedProperty localIdProp;
          static SerializedObject serializedObject;
          static PropertyInfo inspectorModeInfo;
          static internal long m_TryGetGUIDAndLocalFileIdentifier( UnityEngine.Object gameObject ) // #if USE2018
          {
              if ( !gameObject ) return 0;
              if ( !cached_so.ContainsKey( gameObject ) )
              {
                  cached_so.Add( gameObject, new SerializedObject( gameObject ) );
              }
              if ( inspectorModeInfo == null ) inspectorModeInfo = typeof( SerializedObject ).GetProperty( "inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance );
              serializedObject = cached_so[ gameObject ];
              inspectorModeInfo.SetValue( serializedObject, InspectorMode.Debug, null );
              localIdProp = serializedObject.FindProperty( "m_LocalIdentfierInFile" );
              //EMX_TODO FIX FOR MODELS, now it retunr a 100000 always for all objets
              if ( localIdProp.longValue == 100000 )
              {
                  var path = AssetDatabase.GetAssetPath(gameObject);
                  if ( !string.IsNullOrEmpty( path ) ) return AssetDatabase.AssetPathToGUID( path ).GetHashCode();
              }
#if UNITY_2017_1_OR_NEWER
              if ( localIdProp.longValue != 0 ) return localIdProp.longValue;
#endif
              return localIdProp.intValue;
          }


          internal static UnityEngine.Object GetPrefabInstanceHandle( UnityEngine.Object prefab_root, int id )
          {
              if ( !prefab_root ) return null;
              return GetCorrespondingObjectFromSource( prefab_root m id );
          }
          

        */

		//static internal HierarchyObject GetProjectObjectByIsntanceID(int instanceid)
		//{
		//
		//
		//	if (!m_instanceIdToHierObject.TryGetValue(instanceid, out gettedObject) || /*IS_HIERARCHY() && */!gettedObject.go)
		//	{
		//		m_instanceIdToHierObject.Remove(instanceid);
		//			gettedObject = new HierarchyObject(0)
		//			{
		//				go = ___o ?? (EditorUtility.InstanceIDToObject(instanceid) as GameObject),
		//			};
		//			gettedObject.id = instanceid;
		//		m_instanceIdToHierObject.Add(instanceid, gettedObject);
		//	}
		//
		//	//if ( !m_instanceIdToHierObject.ContainsKey( id ) ) return null;
		//	return m_instanceIdToHierObject[instanceid];
		//}



		static HierarchyObject gettedObject;
		internal static Dictionary<int, HierarchyObject> m_instanceIdToHierObject_0 = new Dictionary<int, HierarchyObject>();
		internal static Dictionary<int, HierarchyObject> m_instanceIdToHierObject_1 = new Dictionary<int, HierarchyObject>();
		static internal HierarchyObject GetHierarchyObjectByInstanceID( GameObject ___o )
		{
			if ( !___o ) return null;
			return GetHierarchyObjectByInstanceID( ___o.GetInstanceID(), ___o );
		}
		static internal HierarchyObject GetHierarchyObjectByInstanceID( int instanceid, GameObject ___o )
		{
			if ( !m_instanceIdToHierObject_0.TryGetValue( instanceid, out gettedObject ) || /*IS_HIERARCHY() && */!gettedObject.go )
			{
				m_instanceIdToHierObject_0.Remove( instanceid );

				//if ( IS_HIERARCHY() )
				{
					gettedObject = new HierarchyObject( 0 ) {
						go = ___o ?? (EditorUtility.InstanceIDToObject( instanceid ) as GameObject),
					};

					gettedObject.id = instanceid;

					/* if ( !gettedObject.go ) gettedObject.fileID = instanceid;
                     else gettedObject.fileID = gettedObject.fileID;*/


				}

				// else
				{
					/*if (!m_instanceIdToPATH.ContainsKey(instanceid))
                    {
                        finded = AssetDatabase.GetAssetPath(instanceid);
                        / *try { finded = AssetDatabase.GetAssetPath( AssetDatabase.Path. EditorUtility.InstanceIDToObject( instanceid ) ); }
                        catch { return null; }* /
                        m_instanceIdToPATH.Add(instanceid, finded);
                    }

                    if (!m_instanceIdToGUID.ContainsKey(instanceid)) m_instanceIdToGUID.Add(instanceid, f_path = AssetDatabase.AssetPathToGUID(m_instanceIdToPATH[instanceid]));

                    gettedObject = GetHierarchyObjectByGUID(ref f_path, ref finded);


                    if (!gettedObject.project.IsFolder && gettedObject.id != instanceid)
                    {
                        if (gettedObject.project.nonMainAssets == null) gettedObject.project.nonMainAssets = new Dictionary<int, HierarchyObject>();

                        if (!gettedObject.project.nonMainAssets.ContainsKey(instanceid))
                        {
                            var target = EditorUtility.InstanceIDToObject(instanceid);

                            if (target)
                            {
                                var clone = (HierarchyObject) gettedObject.Clone();
                                clone.project.IsMainAsset = false;
                                clone.project.nonMainAssets = null;
                                clone.project.assetName = target.name;
                                gettedObject.project.nonMainAssets.Add(instanceid, clone);
                            }
                        }

                        if (gettedObject.project.nonMainAssets.ContainsKey(instanceid))
                            gettedObject = gettedObject.project.nonMainAssets[instanceid];
                    }*/
				}


				m_instanceIdToHierObject_0.Add( instanceid, gettedObject );
			}

			//if ( !m_instanceIdToHierObject.ContainsKey( id ) ) return null;
			return m_instanceIdToHierObject_0[ instanceid ];
		}












		static Dictionary<string, HierarchyObject> m_GuidToHierObject = new Dictionary<string, HierarchyObject>();
		//static  bool marker;
		static bool INVOKE_ON_GUID_BACKCHANGED = false;
		static void ON_GUID_BACKCHANGED()
		{
			if ( INVOKE_ON_GUID_BACKCHANGED ) return;

			INVOKE_ON_GUID_BACKCHANGED = true;
			Root.p[ 0 ].PUSH_GUI_ONESHOT( 1, () => {
				Root.p[ 0 ].Modules_SetDirty();
				Root.p[ 0 ].Modules_RefreshBookmarks();
				INVOKE_ON_GUID_BACKCHANGED = false;
			} );
			/* bottomInterface.m_memCache
             ClearHierarchyObjects();*/
		}
		//  static   bool fixget;
		static object[] args = new object[1];
		static string getid_path;
		static int GET_ID_BY_GUID( ref string guid )
		{

			getid_path = AssetDatabase.GUIDToAssetPath( guid );
			if ( string.IsNullOrEmpty( getid_path ) ) return 0;
			UnityEngine.Object ob;
			try
			{
				ob = AssetDatabase.LoadAssetAtPath( getid_path, typeof( UnityEngine.Object ) );
			}
			catch
			{
				return 0;
			}
			if ( !ob ) return 0;
			return ob.GetInstanceID();
			//return AssetDatabase.LoadMainAssetAtPath( getid_path ).GetInstanceID();

			//throw new Exception("GET_ID_BY_GUID");
			/* getid_path = AssetDatabase.GUIDToAssetPath(guid);
			 if (string.IsNullOrEmpty(getid_path)) return 0;
			 if (Root.p[1].GetInstanceIDFromGUID != null)
			 {
				 args[0] = guid;
				 return (int)Root.p[1].GetInstanceIDFromGUID.Invoke(null, args);
			 }
			 else
			 {
				 args[0] = getid_path;
				 return (int)Root.p[1].GetMainAssetInstanceIDFromPath.Invoke(null, args);
			 }*/
		}
		static Dictionary<int, string> m_instanceIdToPATH = new Dictionary<int, string>();
		static Dictionary<int, string> m_instanceIdToGUID = new Dictionary<int, string>();
		static internal HierarchyObject GetHierarchyObjectByGUID( int instanceid )
		{
			/*var p = AssetDatabase.GetAssetPath(ob);
            var guid =  AssetDatabase.AssetPathToGUID(p);
            var ep = "-";
            return GetHierarchyObjectByGUID( ref guid, ref ep );*/
			//var instanceid = ob.GetInstanceID();

			if ( !m_instanceIdToHierObject_1.TryGetValue( instanceid, out gettedObject ) )
			{
				string finded = "-";
				string f_guid = "-";
				if ( !m_instanceIdToPATH.ContainsKey( instanceid ) ) m_instanceIdToPATH.Add( instanceid, finded = AssetDatabase.GetAssetPath( instanceid ) );
				if ( !m_instanceIdToGUID.ContainsKey( instanceid ) ) m_instanceIdToGUID.Add( instanceid, f_guid = AssetDatabase.AssetPathToGUID( m_instanceIdToPATH[ instanceid ] ) );

				gettedObject = GetHierarchyObjectByGUID( ref f_guid, ref finded, instanceid );


				if ( !gettedObject.project.IsFolder && gettedObject.id != instanceid )
				{
					if ( gettedObject.project.nonMainAssets == null ) gettedObject.project.nonMainAssets = new Dictionary<int, HierarchyObject>();

					if ( !gettedObject.project.nonMainAssets.ContainsKey( instanceid ) )
					{
						var target = EditorUtility.InstanceIDToObject(instanceid);

						if ( target )
						{
							var clone = (HierarchyObject)gettedObject.Clone();
							clone.project.IsMainAsset = false;
							clone.project.nonMainAssets = null;
							clone.project.assetName = target.name;
							gettedObject.project.nonMainAssets.Add( instanceid, clone );
						}
					}

					if ( gettedObject.project.nonMainAssets.ContainsKey( instanceid ) )
						gettedObject = gettedObject.project.nonMainAssets[ instanceid ];
				}
				m_instanceIdToHierObject_1.Add( instanceid, gettedObject );
			}

			//if ( !m_instanceIdToHierObject.ContainsKey( id ) ) return null;
			return m_instanceIdToHierObject_1[ instanceid ];
		}
		static internal HierarchyObject GetHierarchyObjectByGUID( ref string guid, ref string estimpath, int? id )
		{
			if ( !m_GuidToHierObject.TryGetValue( guid, out gettedObject ) ) // if (GetInstanceIDFromGUID == null) return null;
			{
				//args[0] = guid;

				var getted_id = id ?? GET_ID_BY_GUID(ref guid);

				if ( getted_id == 0 && !string.IsNullOrEmpty( estimpath ) )
				{
					var estimguid = AssetDatabase.AssetPathToGUID(estimpath);

					if ( !string.IsNullOrEmpty( estimguid ) )
					{


						getted_id = GET_ID_BY_GUID( ref estimguid );

						if ( getted_id != 0 )
						{
							guid = estimguid;
							ON_GUID_BACKCHANGED();
							//marker = true;
						}
					}
				}


				estimpath = getted_id == 0 ? "" : AssetDatabase.GUIDToAssetPath( guid );

				//if (string.IsNullOrEmpty( estimpath )) gettedObject = null;
				// else
				{
					gettedObject = new HierarchyObject( 1 ) {
						project = new HierarchyObject_ProjectExt() {
							assetPath = estimpath == null ? "" : estimpath,
							guid = guid
						},
						id = getted_id, //InternalEditorUtility.GetObjectFromInstanceID
										// fileID = getted_id //InternalEditorUtility.GetObjectFromInstanceID
					};
				}


				//Debug.Log( gettedObject.project.assetPath );

				//var _id =
				//gettedObject.go = EditorUtility.InstanceIDToObject( gettedObject.id ) as GameObject;


				if ( !string.IsNullOrEmpty( gettedObject.project.assetPath ) )
				{
					gettedObject.project.IsFolder = Directory.Exists( Folders.UNITY_SYSTEM_PATH + gettedObject.project.assetPath );

					var ind = gettedObject.project.assetPath.LastIndexOf('/');
					gettedObject.project.assetName = gettedObject.project.assetPath.Substring( ind + 1 );

					//  if (gettedObject.project.IsFolder) gettedObject.project.assetName = "(Folder)" + gettedObject.project.assetName;
					gettedObject.project.assetFolder = ind > -1 ? gettedObject.project.assetPath.Remove( ind ) : "";

					//if (!m_PathToChildrens.ContainsKey(gettedObject.project.assetFolder)) m_PathToChildrens.Add(gettedObject.project.assetFolder, new SortedList<int, HierarchyObject>());

					//gettedObject.project.sibling = m_PathToChildrens[gettedObject.project.assetFolder].Count;
					//m_PathToChildrens[gettedObject.project.assetFolder].Add(gettedObject.project.sibling, gettedObject);

					if ( !gettedObject.project.IsFolder )
					{
						var extInd = gettedObject.project.assetName.LastIndexOf('.');

						if ( extInd != -1 && extInd < gettedObject.project.assetName.Length - 1 )
						{
							gettedObject.project.fileExtension = '.' + gettedObject.project.assetName.Substring( extInd + 1 ).ToLower();
						}
					}

					if ( !gettedObject.project.IsFolder )
					{
						var dot = gettedObject.project.assetName.LastIndexOf('.');
						var slash = gettedObject.project.assetName.LastIndexOf('/');

						if ( dot > 0 && slash < dot ) gettedObject.project.assetName = gettedObject.project.assetName.Remove( dot );
					}


					//gettedObject.project.obj = AssetDatabase.LoadMainAssetAtPath( gettedObject.project.assetName );
				}

				else
				{
					gettedObject.InvalideProjectAsset = true;
				}

				if ( !m_GuidToHierObject.ContainsKey( guid ) )
					m_GuidToHierObject.Add( guid, gettedObject );
				else m_GuidToHierObject[ guid ] = gettedObject;

				// if (gettedObject != null)
				{
					if ( !m_PathToObject.ContainsKey( gettedObject.project.assetPath ) ) m_PathToObject.Add( gettedObject.project.assetPath, gettedObject );
					else m_PathToObject[ gettedObject.project.assetPath ] = gettedObject;
				}
			}

			return gettedObject;
			//	if ( !m_GuidToHierObject.ContainsKey( guid ) ) return null;
			//return m_GuidToHierObject[guid];
		}



	}









	class Prefabs
	{



		internal static UnityEngine.Object GetCorrespondingObjectFromSource( GameObject prefab_root ) //Debug.Log((prefab_root == null) + " " + (Hierarchy_GUI.Instance(this) == null) + " " + ( Hierarchy_GUI.Instance(this).PrefabsDic == null));
		{
			if ( !prefab_root ) return null;

			//  Hierarchy_GUI.Instance(this);
			if ( !HierarchyTempScenePrefabs.InstanceFast( prefab_root.scene ).GetValueByKey( prefab_root.GetInstanceID(), out _tempP ) )
			{
#if UNITY_2018_2_OR_NEWER
				_tempP = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource( prefab_root );
#else
			_tempP = UnityEditor.PrefabUtility.GetPrefabParent( prefab_root );
#endif
				HierarchyTempScenePrefabs.InstanceFast( prefab_root.scene ).PrefabsDicAdd( prefab_root.GetInstanceID(), _tempP );
			}
			return _tempP;
		}


		static UnityEngine.Object _tempP;
		internal static UnityEngine.Object GetCorrespondingObjectFromSource( GameObject prefab_root, int id )
		{
			if ( !prefab_root ) return null;
			//   
			//  Hierarchy_GUI.Instance(this);
			if ( !HierarchyTempScenePrefabs.InstanceFast( prefab_root.scene ).GetValueByKey( id, out _tempP ) )
			{

				if ( UnityEditor.PrefabUtility.IsPartOfAnyPrefab( prefab_root ) )
				{

#if UNITY_2018_2_OR_NEWER
					_tempP = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource( prefab_root );
#else
                      _tempP = UnityEditor.PrefabUtility.GetPrefabParent( prefab_root );
#endif
				}
				else
				{
					_tempP = null;
				}
				HierarchyTempScenePrefabs.InstanceFast( prefab_root.scene ).PrefabsDicAdd( id, _tempP );
			}
			return _tempP;
		}
		internal static GameObject FindPrefabRoot( GameObject o )
		{
			if ( !o ) return null;
#if UNITY_2018_3_OR_NEWER
			return UnityEditor.PrefabUtility.GetOutermostPrefabInstanceRoot( o );
#else
            return UnityEditor.PrefabUtility.FindPrefabRoot( o );
#endif
		}

		internal static GameObject GetPrefabInstanceHandleGameObject( GameObject prefab_root )
		{
			return GetPrefabInstanceHandle( prefab_root ) as GameObject;
		}

		internal static UnityEngine.Object GetPrefabInstanceHandle( GameObject prefab_root )
		{
			if ( !prefab_root ) return null;
			return GetCorrespondingObjectFromSource( prefab_root );
		}



		internal static GameObject ReplacePrefab( GameObject prefab_root, UnityEngine.Object prefab_src )
		{
#if UNITY_2018_3_OR_NEWER
			return UnityEditor.PrefabUtility.SaveAsPrefabAssetAndConnect( prefab_root, AssetDatabase.GetAssetPath( prefab_src ), InteractionMode.AutomatedAction );
#else
            return UnityEditor.PrefabUtility.ReplacePrefab( prefab_root, prefab_src, ReplacePrefabOptions.ConnectToPrefab );
#endif
		}








		public static PrefabInstanceStatus GetPrefabType( GameObject prefab_root )
		{
#if UNITY_2018_3_OR_NEWER
			return (PrefabInstanceStatus)UnityEditor.PrefabUtility.GetPrefabInstanceStatus( prefab_root );
#else
			var t = UnityEditor.PrefabUtility.GetPrefabType(prefab_root);

			if (t == UnityEditor.PrefabType.None) return PrefabInstanceStatus.NotAPrefab;

			if (t == UnityEditor.PrefabType.DisconnectedModelPrefabInstance || t == UnityEditor.PrefabType.DisconnectedPrefabInstance) return PrefabInstanceStatus.Disconnected;

			if (t == UnityEditor.PrefabType.MissingPrefabInstance) return PrefabInstanceStatus.MissingAsset;

			return PrefabInstanceStatus.Connected;
#endif
		}






	}

	public enum PrefabInstanceStatus
	{
		NotAPrefab = 0,
		Connected = 1,
		Disconnected = 2,
		MissingAsset = 3
	}
}
