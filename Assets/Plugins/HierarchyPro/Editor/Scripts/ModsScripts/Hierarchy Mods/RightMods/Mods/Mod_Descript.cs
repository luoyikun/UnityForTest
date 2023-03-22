using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace EMX.HierarchyPlugin.Editor.Mods
{


	internal class Mod_Descript : RightModBaseClass, IModSaver
    {


        public Mod_Descript( int restWidth, int sibbildPos, bool enable, PluginInstance adapter ) : base( restWidth, sibbildPos, enable, adapter )
        {
            // adapter.OnClearObjects -= UA;
            //  adapter.OnClearObjects += UA;
            //adapter.onUndoAction -= UA;

            /* if ( !wasInitDes )
         {
             adapter.onUndoAction -= UA;
             adapter.onUndoAction += UA;
             adapter.bottomInterface.onSceneChange -= UA;
             adapter.bottomInterface.onSceneChange += UA;

             wasInitDes = true;
             var list = getDoubleList(_o.scene);
             for ( int i = list.Count - 1 ; i >= 0 ; i-- )
             {
                 var pp = adapter.GetHierarchyObjectByPair(ref list.listKeys, i);
                 if ( !pp.Validate() || string.IsNullOrEmpty( list.listValues[ i ] ) )
                 {
                     list.RemoveAt( i );
                 }
             }
         }*/

        }

        internal override void Subscribe( EditorSubscriber sbs )
        {
            //sbs.OnUndoAction += UA;
            sbs.saveModsInterator.Add( this );
            //base.Subscribe(sbs);
        }
        bool IModSaver.SaveToString( HierarchyObject o, ref string result )
        {

            var data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModDescription, o);
            if ( data != null && !string.IsNullOrEmpty( data.stringValue ) )
            {
                result = data.stringValue;
                return true;
            }
            return false;
        }
        List<SaverType> _GetSaverTypes = new List<SaverType>() { SaverType.ModDescription };
        List<SaverType> IModSaver.GetSaverTypes { get { return _GetSaverTypes; } }
		internal override bool USE_CONTENT_SHRINKING() { return true; }

		bool IModSaver.LoadFromString( string s, HierarchyObject o )
        {
            return HierarchyTempSceneDataGetter.SetObjectData( SaverType.ModDescription, o, s, true );
        }
        // void UA()
        //{
        //  ResetStack();
        //  __cacheDescriptionDic.Clear();
        // }
        public override float GetInputWidth()
        {
            return 0.5f;
        }


        internal override void ModuleCommonGenericMenu( GenericMenu menu, GameObject activeGo, object _c, string sub = "" )
        {
            var pos = new MousePos(Event.current.mousePosition, MousePos.Type.Input_190_68, !callFromExternal(), adapter);

            if ( !activeGo && adapter.ha.SELECTED_GAMEOBJECTS().Length == 0 ) menu.AddDisabledItem( new GUIContent( sub + "+ Add new description" ) );
            else menu.AddItem( new GUIContent( sub + "+ Add new description" ), false, () => {
                CREATE_NEW_ESCRIPTION( adapter, pos, null, false );
            } );

        }

        GUIContent content = new GUIContent();
        public override void Draw()
        {

            if ( adapter.pluginID == 1 )
            {
                if ( !adapter.o.project.IsMainAsset )
                {
                    GUI.Label( drawRect, "-", RightModsStyles.STYLE_LABEL_8_WINDOWS_middle );
                }
            }


            if ( !START_DRAW( drawRect, adapter.o ) )
            {
                return;
            }

			//if ( adapter.o.lastContentRect[savedData.temp_i].HasValue ) adapter.o.lastContentRect[savedData.temp_i] = null;

			//content.tooltip = base.ContextHelper;
            bool hasContent = false;


            var data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModDescription, adapter.o);
            var style = !callFromExternal() ? RightModsStyles.STYLE_LABEL_8_right : RightModsStyles.STYLE_LABEL_8_WINDOWS_right;

            Rect r;
            if ( data != null && !string.IsNullOrEmpty( data.stringValue ) )
            {
                content.tooltip = content.text = data.stringValue;
                hasContent = true;
                r = adapter.modsController.rightModsManager.DrawCursorRect( ref drawRect, style, content );
				Draw_Label( r, content, style, true );
			}
			else
            {
                if ( adapter.par_e.RIGHT_SKIP_HYPHEN_FOR_DESCRIPTIONS )
                {
                    //if ( content.text != "" ) content.text = "";
                    r = adapter.modsController.rightModsManager.DrawCursorRect( ref drawRect, style, GetEmptyConten_ForceEmpty );
                }
                else
                {
                    //if ( content.text != RightModsManager.GetEmptyConten_Common ) content.text = RightModsManager.GetEmptyConten_Common;
                    r = adapter.modsController.rightModsManager.DrawCursorRect( ref drawRect, style, GetEmptyConten_Common );
					if ( !string.IsNullOrEmpty( GetEmptyConten_Common.text ) ) Draw_Label( r, GetEmptyConten_Common, style, true );
				}
			}
            /*if ( HasKey( _o.scene, _o ) ) //  MonoBehaviour.print(Resources.FindObjectsOfTypeAll<DescriptionHelper>().Length);
            {
                var ptr = cacheDescriptionDic[_o.scene][_o];
                content.tooltip = content.text = d.GetHash2()[ ptr ];
                Draw_Label( drawRect, content, !callFromExternal() ? adapter.STYLE_LABEL_8_right : adapter.STYLE_LABEL_8_WINDOWS, true );
            }*/



            Draw_ModuleButton( r, null, BUTTON_ACTION_HASH, hasContent, drawPointer: true );

			END_DRAW( adapter.o , savedData.temp_i);
            //if (adapter.EVENT.type == EventType.Layout)
             if (savedData.temp_i != -1)   adapter.o.lastContentRectLayout[ savedData.temp_i ].SET(ref r);
		}


		DrawStackMethodsWrapper __BUTTON_ACTION_HASH = null;
        DrawStackMethodsWrapper BUTTON_ACTION_HASH {
            get { return __BUTTON_ACTION_HASH ?? (__BUTTON_ACTION_HASH = new DrawStackMethodsWrapper( BUTTON_ACTION )); }
        }

        void BUTTON_ACTION( Rect worldOffset, Rect inputRect, DrawStackMethodsWrapperData data, HierarchyObject _o )
        {

            if ( Event.current.button == adapter.MOUSE_BUTTON_0 )
            {
                var pos = new MousePos(Event.current.mousePosition, MousePos.Type.Input_190_68, !callFromExternal(), adapter);
                CREATE_NEW_ESCRIPTION( adapter, pos, _o, false );
            }

            if ( Event.current.button == adapter.MOUSE_BUTTON_1 )
            {


                var list = HierarchyTempSceneData.GetAllObjectData(SaverType.ModDescription, _o.go.scene);


                //var list = getDoubleList(_o.scene);

                Tools.EventUse();
                /* int[] contentCost = new int[0];
                 GameObject[] obs = new GameObject[0];*/
                // var o = SetPair(_o);

                var mp = new MousePos(Event.current.mousePosition, MousePos.Type.Search_356_0, !callFromExternal(), adapter);

                if ( list.ContainsKey( _o.id ) && !string.IsNullOrEmpty( list[ _o.id ].stringValue ) )
                {

                    Windows.SearchWindow.Init( mp, SearchHelper, list[ _o.id ].stringValue == "All" ? "All " : list[ _o.id ].stringValue,
                        CallHeaderFiltered( list[ _o.id ].stringValue ),
                        this, adapter.window, _o );
                }
                else
                {

                    Windows.SearchWindow.Init( mp, SearchHelper, "All",
                        CallHeader(),
                        this, adapter.window, _o );
                }

                // EditorGUIUtility.ic
            }
        }



        /** CALL HEADER */
        internal override Windows.SearchWindow.FillterData_Inputs CallHeader()
        {
            Action<Windows.SearchWindow.FillterData_Inputs> updateCache = (Windows.SearchWindow.FillterData_Inputs input) =>
            {
				/* dictionary_cache.Clear();
				 dictionary_cacheSorted.Clear();

				 if (adapter.IS_HIERARCHY())
				 {
					 for (int i = 0; i < SceneManager.sceneCount; i++)
					 {
						 var s = SceneManager.GetSceneAt(i);
						 if (!s.IsValid() || !s.isLoaded) continue;
						 var g = getDoubleList(s.GetHashCode());
						 foreach (var item in g)
						 {
							 if (!dictionary_cache.ContainsKey(item.Key)) dictionary_cache.Add(item.Key, item.Value);
						 }

						 dictionary_cacheSorted.listKeys.AddRange(g.listKeys);
						 dictionary_cacheSorted.listValues.AddRange(g.listValues);
					 }
				 }
				 else
				 {
					 var g = getDoubleList(-1);
					 foreach (var item in g)
					 {
						 if (!dictionary_cache.ContainsKey(item.Key)) dictionary_cache.Add(item.Key, item.Value);
					 }

					 dictionary_cacheSorted.listKeys.AddRange(g.listKeys);
					 dictionary_cacheSorted.listValues.AddRange(g.listValues);
				 }

 #pragma warning disable
				 var tr = new HierarchyObject[dictionary_cacheSorted.listKeys.Count];
				 for (int i = 0; i < dictionary_cacheSorted.listKeys.Count; i++)
					 tr[i] = adapter.GetHierarchyObjectByPair(ref dictionary_cacheSorted.listKeys, i);
 #pragma warning restore*/

				/*  input.analizedObjects = tr.ToList();
                  input.analizeEnumerator = null;
                  input.SKIP_SKANNING = true;*/
			};


            var result = new Windows.SearchWindow.FillterData_Inputs(callFromExternal_objects)
            {
                analizeEnumerator = null,
                Valudator = _o =>
                {
                    var data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModDescription, _o);
                    if (data != null && !string.IsNullOrEmpty(data.stringValue)) return true;
                    return false;
					//dictionary_cache.ContainsKey(SetPair(o)) && !string.IsNullOrEmpty(dictionary_cache[SetPair(o)])
				},
                SelectCompareString = (_o, i) =>
                {
                    var data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModDescription, _o);
                    if (data != null && !string.IsNullOrEmpty(data.stringValue)) return data.stringValue;
                    return "";
                }
				// dictionary_cache.ContainsKey(SetPair(o)) ? dictionary_cache[SetPair(o)] : ""
				,
                SelectCompareCostInt = (d, i) =>
                {
                    var cost = i;
                    if (d.go) cost += d.go.activeInHierarchy ? 0 : 100000000;
                    return cost;
                },
            };

            // updateCache(result);
            result.UpdateCache = updateCache;

            //  result.analizedObjects = dictionary_cacheSorted.listKeys.Select( g => adapter.GetHierarchyObjectByPair( g ) ).ToList();


            return result;
        }

        internal Windows.SearchWindow.FillterData_Inputs CallHeaderFiltered( string filter )
        {
            var result = CallHeader();
            result.Valudator = _o => {
                var data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModDescription, _o);
                if ( data != null && String.Equals( filter, data.stringValue, StringComparison.OrdinalIgnoreCase ) ) return true;
                return false;
                //    if ( !dictionary_cache.ContainsKey( SetPair( o ) ) ) return false;
                //  return !string.IsNullOrEmpty( dictionary_cache[ SetPair( o ) ] ) && String.Equals( filter, dictionary_cache[ SetPair( o ) ], StringComparison.CurrentCultureIgnoreCase );
            };
            return result;
        }





        internal void CREATE_NEW_ESCRIPTION( PluginInstance adapter, MousePos pos, HierarchyObject _o, bool singleObject ) //  var o = _o.go;
        {


            Action<string> act = (str) =>
            {

                if (_o != null && !_o.Validate()) return;

                HierarchyTempSceneDataGetter.SetUndoListStart("Apply Description");
				//adapter.SET_UNDO(adapter.MOI.des(scene), "Change description");
				// cacheDic.Clear();
                //Dictionary<int, bool> _sceneDirty = new Dictionary<int, bool>();

                bool result = false;
				//  try
				{
					//   DoubleList<GoGuidPair, string> list = getDoubleList(scene);
					var remove = string.IsNullOrEmpty(str);
                    if (singleObject || _o != null && adapter.ha.SELECTED_GAMEOBJECTS().All(selO => selO != _o))
                    {
                        HierarchyTempSceneDataGetter.SetUndoList(_o.scene);

                        if (!remove) result |= HierarchyTempSceneDataGetter.SetObjectData(SaverType.ModDescription, Cache.GetHierarchyObjectByInstanceID(_o.go), str);
                        else result |= HierarchyTempSceneDataGetter.RemoveObjectData(SaverType.ModDescription, Cache.GetHierarchyObjectByInstanceID(_o.go));

						//result |= AddDescriptionToList(ref list, _o, scene, str);
						Tools.TRY_PING_OBJECT(_o.go);
                        //adapter.MarkSceneDirty(_o.go.scene);
                    }
                    else
                    {
                        foreach (var gameObject in adapter.ha.SELECTED_GAMEOBJECTS())
                        {
                            HierarchyTempSceneDataGetter.SetUndoList(_o.scene);

                            if (!remove) result |= HierarchyTempSceneDataGetter.SetObjectData(SaverType.ModDescription, Cache.GetHierarchyObjectByInstanceID(gameObject.go), str);
                            else result |= HierarchyTempSceneDataGetter.RemoveObjectData(SaverType.ModDescription, Cache.GetHierarchyObjectByInstanceID(gameObject.go));

                            Tools.TRY_PING_OBJECT(gameObject.go);
                            //if (!_sceneDirty.ContainsKey(gameObject.scene.GetHashCode()))
                            //{
                            //    _sceneDirty.Add(gameObject.scene.GetHashCode(), false);
                            //    adapter.MarkSceneDirty(gameObject.scene);
                            //}
							//  result |= AddDescriptionToList(ref list, gameObject, scene, str);
						}
                    }
                }

                HierarchyTempSceneDataGetter.SetDirtyList();
				/* catch (Exception ex)
				 {
					 Debug.LogError(ex.Message + "\n\n" + ex.StackTrace);
				 }*/

				//if (!result) return;
                //UA();
				/*  var d = adapter.MOI.des(scene);
				  if ((!Application.isPlaying || adapter.pluginID != 0 || Hierarchy_GUI.Instance(adapter).SaveToScriptableObject == "FOLDER") && (d as UnityEngine.Object))
				  {
					  adapter.SetDirtyDescription(d, d.gameObject ? d.gameObject.scene.GetHashCode() : adapter.GET_ACTIVE_SCENE);
					  if (!Application.isPlaying) adapter.MarkSceneDirty(d.gameObject ? d.gameObject.scene.GetHashCode() : adapter.GET_ACTIVE_SCENE);
				  }*/
            };
            if ( _o == null ) _o = Cache.GetHierarchyObjectByInstanceID( Selection.activeGameObject );
            var data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModDescription, _o);
            string input = null;
            if ( data != null && !string.IsNullOrEmpty( data.stringValue ) ) input = data.stringValue;

            Windows.InputWindow.Init( pos, "New description", adapter.window, act, act, textInputSet: input );
        }










        /*  protected bool AddDescriptionToList( ref DoubleList<GoGuidPair, string> list, HierarchyObject _o, int scene, string str, bool SaveRegistrator = true )
          {
              var o = SetPair(_o);
              if ( string.IsNullOrEmpty( str ) )
              {
                  list.RemoveAll( o );
              }
              else
              {
                  str = str.Trim();



                  if ( list.ContainsKey( o ) )
                  {
                      if ( str == list[ o ] ) return false;
                      list[ o ] = str;
                  }
                  else
                  {
                      list.Add( o, str );
                  }
              }

              UA();



              return true;
          }*/


        /*

        internal void SetValue( string value, int s, HierarchyObject o )
        {

            if ( !o.Validate() ) return;


            var d = adapter.MOI.des(s);
            if ( d == null ) return;

            UA();

            var list = getDoubleList(s);
            var p = SetPair(o);
            if ( list.ContainsKey( p ) )
            {
                list[ p ] = value;
            }
            else list.Add( p, value );

            //if (  string.IsNullOrEmpty( value ) )list.RemoveAll(p);
            //  Debug.Log(p.GetHashCode());

            if ( !Application.isPlaying )
            {
                adapter.SetDirtyDescription( d, s );
                adapter.MarkSceneDirty( s );
            }
        }

        internal bool HasKey( int s, HierarchyObject _o )
        {
            if ( !_o.Validate() ) return false;
            //   if (adapter.pluginID == 0)Debug.Log(cacheDic.Count);
            if ( !cacheDescriptionDic.ContainsKey( s ) )
            {
                var d = adapter.MOI.des(s);
                var h = d.GetHash1_Fix2_0();
                cacheDescriptionDic.Add( s, new Dictionary<HierarchyObject, int>() );
                // Debug.Log("ASD" + adapter.pluginID);
                while ( d.GetHash1_Fix2_0().Count != d.GetHash2().Count )
                {
                    if ( d.GetHash2().Count < d.GetHash1_Fix2_0().Count ) d.GetHash2().Add( "" );
                    else d.GetHash2().RemoveAt( d.GetHash2().Count - 1 );
                }

                for ( int i = 0 ; i < h.Count ; i++ )
                {
                    var getO = adapter.GetHierarchyObjectByPair(ref h, i);
                    if ( !getO.Validate() ) continue;
                    if ( cacheDescriptionDic[ s ].ContainsKey( getO ) )
                    {
                        d.GetHash1_Fix2_0().RemoveAt( i );
                        d.GetHash2().RemoveAt( i );
                        i--;
                    }
                    else
                    {
                        cacheDescriptionDic[ s ].Add( getO, i );
                    }
                }
            }

            return cacheDescriptionDic[ s ].ContainsKey( _o );
        }

        internal string GetValue( int s, HierarchyObject o )
        {
            if ( !HasKey( s, o ) ) return null;
            var d = adapter.MOI.des(s);
            if ( d == null || !cacheDescriptionDic[ s ].ContainsKey( o ) ) return null;
            var ptr = cacheDescriptionDic[s][o];
            return d.GetHash2()[ ptr ];
        }

    */
    }
}




















/*  internal override bool CallHeader(out GameObject[] obs, out int[] contentCost)
  {
      for (int i = 0; i < SceneManager.sceneCount; i++)
      {
          var s = SceneManager.GetSceneAt(i);
          if (!s.IsValid()) continue;
          var g = getDoubleList(s);
          list.listKeys.AddRange(g.listKeys);
          list.listValues.AddRange(g.listValues);
      }

      obs = Utilities.AllSceneObjects().Where(o => list.ContainsKey(o) && !string.IsNullOrEmpty(list[o])).ToArray();
      contentCost = obs
          .Select((d, i) => new { name = list[d], startIndex = i })
          .OrderBy(d => d.name)
          .Select((d, i) => new { d.startIndex, cost = i })
          .OrderBy(d => d.startIndex)
          .Select(d => d.cost).ToArray();
      return true;
  }

  internal void CallHeaderFilltered(out GameObject[] obs, out int[] contentCost, string filter)
  {
      DoubleList<GameObject, string> list = new DoubleList<GameObject, string>();
      for (int i = 0; i < SceneManager.sceneCount; i++)
      {
          var s = SceneManager.GetSceneAt(i);
          if (!s.IsValid()) continue;
          var g = getDoubleList(s);
          list.listKeys.AddRange(g.listKeys);
          list.listValues.AddRange(g.listValues);
      }

      obs = Utilities.AllSceneObjects().Where(o => list.ContainsKey(o) && !string.IsNullOrEmpty(list[o]) && String.Equals(filter, list[o], StringComparison.CurrentCultureIgnoreCase)).ToArray();
      contentCost = obs
          .Select((d, i) => new { name = list[d], startIndex = i, obj = d })
          .OrderBy(d => d.name)
          .Select((d, i) => {
              var cost = i;
              cost += d.obj.activeInHierarchy ? 0 : 100000000;
              return new { d.startIndex, cost = cost };
          })
          .OrderBy(d => d.startIndex)
          .Select(d => d.cost).ToArray();
  }*/

/* int ToContentCost(GameObject o)
{
    var aud = o.GetComponent<AudioSource>();
    if (aud.clip == null) return 1;
    return 0;
}*/
