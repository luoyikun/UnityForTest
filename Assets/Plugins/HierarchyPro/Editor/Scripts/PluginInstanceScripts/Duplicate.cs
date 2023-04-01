using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace EMX.HierarchyPlugin.Editor
{


	class DuplicateHelper
	{
		//int pluginID;
		PluginInstance p;
		internal DuplicateHelper(int pId)
		{
			//pluginID = pId;
			p = Root.p[pId];
		}



		List<HierarchyObject> oldObs = null;
		// const string START = " ⅝⅝";
		//  const string END = "⅞⅞ ";
		bool needDuplicateCache = false;
		List<HierarchyObject> needRestoreGameObjectNames = null;

		internal void Duplicate_FirstFrame_OnGUI()
		{
			if (p.pluginID == 1) return;

			if (needDuplicateCache && p.EVENT.commandName != "Duplicate" && p.EVENT.commandName != "Paste")
			{
				needDuplicateCache = false;
				SaveDataFromName(GetBroadCastSelection());
				if (oldObs != null)
				{
					RemoveDataFromName(oldObs);
					oldObs = null;
				}
			}

			if (needRestoreGameObjectNames != null && p.EVENT.commandName != "Duplicate" && p.EVENT.commandName != "Paste" && p.EVENT.commandName != "Copy")
			{
				RemoveDataFromName(needRestoreGameObjectNames); //REMOVE
				needRestoreGameObjectNames = null;
			}

			if (p.EVENT.type == EventType.ExecuteCommand && p.EVENT.commandName == "Paste")
			{
				needDuplicateCache = true;
				// MonoBehaviour.print(Selection.activeGameObject.name);
			}

			if (p.EVENT.type == EventType.ExecuteCommand && p.EVENT.commandName == "Copy")
			{
				needRestoreGameObjectNames = AddDataToName(GetBroadCastSelection(), false); //ADD
			}

			//MonoBehaviour.print(GUIUtility.systemCopyBuffer);

			if (p.EVENT.type == EventType.ExecuteCommand && p.EVENT.commandName == "Duplicate")
			{
				oldObs = AddDataToName(GetBroadCastSelection(), true); //ADD
				needDuplicateCache = true;
			}
		}
		#pragma warning disable
		class SceneSaverTypes
		{
			internal Scene scene;
			internal int[] saverTypes;
		}
		#pragma warning restore
		SceneSaverTypes getSceneSaverTypes;
		internal void SaveDataFromName(HierarchyObject[] transs)
		{

			if (transs.Length == 0) return;

			Dictionary<int, SceneSaverTypes> saved_scenes = new Dictionary<int, SceneSaverTypes>();

			foreach (var o in transs)
			{
				if (!o.Validate(true)) continue;
				{
					//   var dh = needRestoreGameObjectName.GetComponent<StringFlush>();
					var dh = o.go.GetComponent<StringFlush>();
					//  var dh = o.cachedComp as StringFlush;
					if (dh)
					{

						//  if ( !string.IsNullOrEmpty( dh.cachedData ) )
						{
							//  var o = Cache.GetHierarchyObjectByInstanceID(needRestoreGameObjectName.GetInstanceID(), needRestoreGameObjectName);
							var data = dh.cachedData;
							//  foreach ( var s1 in data.Split( '⅜' ) )
							foreach (var s1 in data)
							{
								// if ( s1.Length < 4 ) continue;
								//   var s2 = s1.Substring(4);
								var k = s1.Remove(s1.IndexOf(']'));
								var key = int.Parse(k);
								var m = Root.p[p.pluginID].modsController.saveModsInterator[key];
								var res = m.LoadFromString(s1.Substring(s1.IndexOf(']') + 1), o);
								if (res)
								{

									if (!saved_scenes.TryGetValue(o.scene, out getSceneSaverTypes)) saved_scenes.Add(o.scene, getSceneSaverTypes = new SceneSaverTypes() { saverTypes = new int[0] });
									var type = m.GetSaverTypes;
									for (int i = 0; i < type.Count; i++)
									{
										if (!getSceneSaverTypes.saverTypes.Contains((int)type[i]))
										{
											Array.Resize(ref getSceneSaverTypes.saverTypes, getSceneSaverTypes.saverTypes.Length + 1);
											getSceneSaverTypes.saverTypes[getSceneSaverTypes.saverTypes.Length - 1] = (int)type[i];
										}
									}
								}

							}

							if (Application.isPlaying) GameObject.Destroy(dh);
							else GameObject.DestroyImmediate(dh, false);
						}
					}
				}
			}

			foreach (var sc in saved_scenes)
				foreach (var ty in sc.Value.saverTypes)
					HierarchyTempSceneDataGetter.TEMP_SAVE_TO_FILE((SaverType)ty, sc.Value.scene);
		}

		internal List<HierarchyObject> AddDataToName(HierarchyObject[] transs, bool writeUndo)
		{

			var result = new List<HierarchyObject>();

			if (Root.p[p.pluginID].DisabledSavedData()) return result;
			StringFlush dh = null;
			string s = null;
			foreach (var o in transs)
			{
				if (!o.Validate(true)) continue;
				//  if ( !needRestoreGameObjectName.scene.IsValid() || !needRestoreGameObjectName.scene.isLoaded ) continue;
				// var buildString = "";

				// var o = Cache.GetHierarchyObjectByInstanceID(needRestoreGameObjectName.id), needRestoreGameObjectName);
				bool wasInit = false;

				for (int i = 0, L = Root.p[p.pluginID].modsController.saveModsInterator.Count; i < L; i++)
				{
					var m = Root.p[p.pluginID].modsController.saveModsInterator[i];
					// var s = m.SaveToString(o);

					if (m.SaveToString(o, ref s) && !string.IsNullOrEmpty(s))
					{
						//if ( !string.IsNullOrEmpty( buildString ) ) buildString += "⅜";
						if (!wasInit)
						{
							dh = o.go.AddComponent<StringFlush>();
							o.cachedComp = dh;
							dh.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
							if (writeUndo && !Application.isPlaying) Undo.RegisterCreatedObjectUndo(dh, "Duplicate");
							wasInit = true;
						}
						dh.cachedData.Add(i.ToString() + "]" + s); // key = [M]1231231]
					}
				}

				// if ( !string.IsNullOrEmpty( buildString ) )
				if (wasInit)
				{
					// = buildString;
					if (!Application.isPlaying) EditorUtility.SetDirty(dh);
					result.Add(o);
				}
			}

			return result;
		}

		internal void RemoveDataFromName(List<HierarchyObject> transs)
		{
			foreach (var needRestoreGameObjectName in transs)
			{
				if (needRestoreGameObjectName.Validate(true))
				{
					//  var dh = needRestoreGameObjectName.go.GetComponent<StringFlush>();
					var dh = needRestoreGameObjectName.cachedComp as StringFlush;
					if (dh)
					{
						if (Application.isPlaying) GameObject.Destroy(dh);
						else GameObject.DestroyImmediate(dh, false);
					}
				}
			}
		}


		internal HierarchyObject[] GetBroadCastSelection()
		{
			if (p.pluginID == 1) return Root.p[p.pluginID].ha.SELECTED_OBJECTS(); //SpeedUp

			var result = new List<HierarchyObject>();
			var sel = Root.p[p.pluginID].ha.SELECTED_OBJECTS();
			var top = Tools.GetOnlyTopObjects(sel);

			//  if ( pluginID == 0 )
			{
				/* foreach ( var gameObject in top )
                 {
                     var tempList = new List<Transform>();
                     getChilds_Hierarchy( gameObject.go.transform, ref tempList );
                     result.AddRange( tempList.Select( t => Cache.GetHierarchyObjectByInstanceID( t.gameObject ) ) );
                 }*/
				return top.SelectMany(t => t.go.GetComponentsInChildren<Transform>(true)).Select(t => Cache.GetHierarchyObjectByInstanceID(t.gameObject.GetInstanceID(), t.gameObject)).ToArray();
			}
			/*  if ( pluginID == 1 )
              {
                  foreach ( var gameObject in top )
                  {
                      var tempList = new List<HierarchyObject>();
                      getChilds_Project( gameObject, ref tempList );
                      result.AddRange( tempList );
                  }
              }
              return result;*/
		}
		/* void getChilds_Hierarchy( Transform t, ref List<Transform> result )
         {
             result.Add( t );

             for ( int i = 0, l = t.transform.childCount ; i < l ; i++ )
                 getChilds_Hierarchy( t.transform.GetChild( i ), ref result );
         }

         static SortedList<int, HierarchyObject> tl;

         void getChilds_Project( HierarchyObject t, ref List<HierarchyObject> result )
         {
             result.Add( t );

             GetPathToChildrens( ref t.project.assetPath, out tl );
             var childs = tl.Values.ToArray();

             for ( int i = 0 ; i < childs.Length ; i++ )
                 getChilds_Project( childs[ i ], ref result );
         }*/

	}
}






/*
if ( s1.StartsWith( "[D]" ) )
{
    var res = s1.Substring(3);
    DescriptionModule.SetValue( res, needRestoreGameObjectName.scene.GetHashCode(), o );
}

else if ( s1.StartsWith( "[C]" ) )
{
    var res = s1.Substring(3).Split(' ');
    var c = Adapter.String4ToColor(res);

    if ( c != null && (c.HAS_BG_COLOR || c.HAS_LABEL_COLOR) ) //var b = Adapter.StringToBool(4, res);
    { //Color32 c = new Color32(result[0], result[1], result[2], result[3]);
        ColorModule.SetHighlighterValue( c, needRestoreGameObjectName.scene.GetHashCode(), o );
    }
}

else if ( s1.StartsWith( "[Q]" ) )
{
    var res = s1.Substring(3).Split(' ');
    var c = Adapter.String4ToList(res);

    if ( c != null ) //Color32 c = new Color32(result[0], result[1], result[2], result[3]);
    {
        ColorModule.IconColorCacher.SetValue( new SingleList() { list = c }, needRestoreGameObjectName.scene.GetHashCode(), SetPair( o ), true );
    }
}

else if ( s1.StartsWith( "[K]" ) )
{
        var res = s1.Substring(3);
        uint resultKeeper;

        if ( uint.TryParse( res, out resultKeeper ) )
        {
            if ( resultKeeper == uint.MaxValue )
            {
                Hierarchy.M_PlayModeKeeper.DataKeeperCache.SetValue( new SingleList() { list = new[] { 1 }.ToList() }, o.go.scene.GetHashCode(), o.go, true );
            }

            else
            {
                var comps = HierarchyExtensions.Utilities.GetComponentFast<Component>.GetAll(o.go).Select(c => c.GetInstanceID()).ToList();
                List<int> tempList = new[] {0}.ToList();

                for ( int i = 0 ; i < 64 ; i++ )
                {
                    if ( i >= comps.Count ) break;

                    if ( (resultKeeper & ((uint)1 << i)) != 0 )
                    {
                        tempList.Add( comps[ i ] );
                    }
                }

                Hierarchy.M_PlayModeKeeper.DataKeeperCache.SetValue( new SingleList() { list = tempList }, o.go.scene.GetHashCode(), o.go, true );
            }
        }
    }
    */
//






/*
         var des = DescriptionModule.GetValue(needRestoreGameObjectName.scene.GetHashCode(), o);
         if ( !string.IsNullOrEmpty( des ) ) buildString += "[D]" + des;

         var col = ColorModule.GetValueToString(needRestoreGameObjectName.scene.GetHashCode(), o);
         if ( !string.IsNullOrEmpty( col ) && !string.IsNullOrEmpty( buildString ) ) buildString += "⅜";
         if ( !string.IsNullOrEmpty( col ) ) buildString += "[C]" + col;

         col = ColorModule.IconColorCacher.GetValueToString( needRestoreGameObjectName.scene.GetHashCode(), o );
         if ( !string.IsNullOrEmpty( col ) && !string.IsNullOrEmpty( buildString ) ) buildString += "⅜";
         if ( !string.IsNullOrEmpty( col ) ) buildString += "[Q]" + col;

         if ( Hierarchy.M_PlayModeKeeper.DataKeeperCache.HasKey( o.scene, o ) )
         {
             var v = Hierarchy.M_PlayModeKeeper.DataKeeperCache.GetValue(o.go.scene, o);
             if ( v.list != null && v.list.Count > 0 )
             {
                 uint resultKeeper = 0;
                 if ( v.list[ 0 ] == 1 ) resultKeeper = uint.MaxValue;
                 else
                 {
                     var comps = HierarchyExtensions.Utilities.GetComponentFast<Component>.GetAll(o.go).Select(c => c.GetInstanceID()).ToList();
                     for ( int i = 1 ; i < v.list.Count ; i++ )
                     {
                         var ind = comps.IndexOf(v.list[i]);
                         if ( ind < 0 ) continue;
                         if ( ind >= 64 ) continue;
                         resultKeeper |= ((uint)1) << ind;
                     }
                 }
                 if ( resultKeeper != 0 )
                 {
                     if ( !string.IsNullOrEmpty( buildString ) ) buildString += "⅜";
                     buildString += "[K]" + resultKeeper;
                 }
             }
         }*/
