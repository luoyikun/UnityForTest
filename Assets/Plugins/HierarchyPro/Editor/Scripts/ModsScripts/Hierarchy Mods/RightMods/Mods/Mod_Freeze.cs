using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace EMX.HierarchyPlugin.Editor.Mods
{

	internal class Mod_Freeze : RightModBaseClass
    {
        /*  MyStruct dasd;
          struct MyStruct
          {
              internal bool? asd;
          }*/

        static internal GameObject[] ignoreLock = new GameObject[0];

        internal override void Subscribe( EditorSubscriber sbs )
        {

            sbs.OnSelectionChanged += onSelectionChange;

            //base.Subscribe(sbs);
        }

        internal override void ModuleCommonGenericMenu( GenericMenu menu, GameObject o, object data, string sub )
        {
        }

        void onSelectionChange()
        {
            bool wasShanged = false;
            var so = adapter.ha.SELECTED_GAMEOBJECTS();

            if ( adapter.par_e.RIGHT_FREEZE_LOCK_SCENE_VIEW )
            {
                foreach ( var gameObject in so )
                {
                    if ( !gameObject.go ) continue;
                    if ( (gameObject.go.hideFlags & HideFlags.NotEditable) != 0 )
                    {
                        if ( UNDO.wasUndo && !ignoreLock.Contains( gameObject.go ) ) ArrayUtility.Add( ref ignoreLock, gameObject.go );
                        if ( !ignoreLock.Contains( gameObject.go ) )
                        {
                            wasShanged = true;
                        }
                    }
                }
            }
            if ( wasShanged )
            {
                var lastSel = so.Select(s1 => s1.go).Where(g => g && (g.hideFlags & HideFlags.NotEditable) == 0).ToArray();
                var so2 = lastSel.Where(l => l && !ignoreLock.Any(i => i && i.GetInstanceID() == l.GetInstanceID())).ToArray();
                Selection.objects = so2;
                adapter.ha.InternalClearSelection( so2.Select( l => l ? l.GetInstanceID() : -1 ).ToArray() );
            }
            if ( ignoreLock.Length != 0 ) ignoreLock = new GameObject[ 0 ];
        }

        bool? lastUpdateState = null;

        void UpdateSceneFlags( bool value )
        {
            if ( lastUpdateState == value ) return;

            lastUpdateState = value;
            // adapter.OneFrameActionOnUpdate = true;
            adapter.PUSH_UPDATE_ONESHOT( 0, () => {
                for ( int asd = 0; asd < SceneManager.sceneCount; asd++ )
                {
                    var s = SceneManager.GetSceneAt(asd);

                    if ( !s.isLoaded || !s.IsValid() ) continue;


                    //  var list = getDoubleList(s.GetHashCode());
                    var all = HierarchyTempSceneData.GetAllObjectData(SaverType.ModFreezer, s);

                    foreach ( var item in all )
                    {

                        //  for ( int i = 0 ; i < list.Count ; i++ )
                        if ( !item.Value.target ) continue;

                        /*  var t = Cache.GetHierarchyObjectByPair(ref list.listKeys, i);

                          if ( !t.Validate() )
                          {
                              list.RemoveAt( i );
                              i--;
                              continue;
                          }*/

                        if ( item.Value.boolValue )
                            item.Value.target.hideFlags |= HideFlags.NotEditable;
                        else
                            item.Value.target.hideFlags &= ~HideFlags.NotEditable;
                    }
                }
            } );
        }

        protected override void OnEnableChange( bool value )
        {
            UpdateSceneFlags( value );
            //base.OnEnableChange(value);
        }

        /* DoubleList<GoGuidPair, bool> dl = new DoubleList<GoGuidPair, bool>();

         public DoubleList<GoGuidPair, bool> getDoubleList(int s)
         {
             var d = adapter.MOI.des(s);

             if (d == null) return new DoubleList<GoGuidPair, bool>();

             dl.listKeys = d.GetFreezeHashKeys();
             dl.listValues = d.GetFreezeHashValues();
             return dl;
         }

         void setDoubleList(int s)
         {
             var dl = getDoubleList(s);
             var d = adapter.MOI.des(s);
             d.SetFreezeHashKeys(dl.listKeys);
             d.SetFreezeHashValues(dl.listValues);
         }*/
        // Dictionary<int, bool> wasFreezeInit = new Dictionary<int, bool>();

        public static bool IsValid()
        {
            if ( !Root.p[ 0 ].par_e.ENABLE_ALL ) return false;
            var m = Root.p[0].modsController.rightModsManager.rightMods.FirstOrDefault(m2 => m2 is Mod_Freeze) as Mod_Freeze;
            if ( m == null || m.savedData == null || !m.savedData.enabled ) return false;
            var obs = Root.p[0].ha.SELECTED_GAMEOBJECTS();
            if ( obs.Length == 0 ) return false;
            return true;
        }

        public static void ToggleFreeze()
        {

            if ( !Root.p[ 0 ].par_e.ENABLE_ALL ) return;
            var m = Root.p[0].modsController.rightModsManager.rightMods.FirstOrDefault(m2 => m2 is Mod_Freeze) as Mod_Freeze;
            if ( m == null || m.savedData == null || !m.savedData.enabled ) return;


            var obs = Root.p[0].ha.SELECTED_GAMEOBJECTS();
            if ( obs.Length == 0 ) return;
            // var top = obs.Where(g => g.GetComponentsInParent<Transform>(true).Count(p => obs.Contains(p.gameObject)) == 1).Select(g => g.gameObject).ToArray();
            var top = Tools.GetOnlyTopObjects(obs);
            foreach ( var item in top )
            {
                var targetO = item.go;

                /*var old = o.hideFlags & HideFlags.NotEditable;
				if ( old != 0 ) {
					o.hideFlags &= ~old;
				} else {
					o.hideFlags |= HideFlags.NotEditable;
				}*/

                var old = targetO.hideFlags & HideFlags.NotEditable;
                var checkValue = targetO.hideFlags;
                if ( old != 0 ) targetO.hideFlags &= ~old;
                else targetO.hideFlags |= HideFlags.NotEditable;
                if ( checkValue != targetO.hideFlags )
                {
                    /*	if ((targetO.hideFlags & HideFlags.NotEditable) != 0)
							m.getDoubleList(targetO.scene.GetHashCode()).SetByKey(new GoGuidPair() { go = targetO.gameObject }, true);
						else
							m.getDoubleList(targetO.scene.GetHashCode()).RemoveAll(new GoGuidPair() { go = targetO.gameObject });

						m.SetLockToggle(EditorSceneManager.GetActiveScene().GetHashCode());
						*/

                    if ( (targetO.hideFlags & HideFlags.NotEditable) != 0 )
                        HierarchyTempSceneDataGetter.SetObjectData( SaverType.ModFreezer, Cache.GetHierarchyObjectByInstanceID( targetO ), true );
                    //getDoubleList(VARIABLE.gameObject.scene.GetHashCode()).SetByKey(new GoGuidPair() {go = VARIABLE.gameObject.gameObject}, true);
                    else
                        HierarchyTempSceneDataGetter.RemoveObjectData( SaverType.ModFreezer, Cache.GetHierarchyObjectByInstanceID( targetO ) );
                    // getDoubleList( tr.gameObject.scene.GetHashCode() ).RemoveAll( new GoGuidPair() { go = tr.gameObject.gameObject } );
                    m.SetLockToggle( targetO.scene.GetHashCode() );
                    Root.p[ 0 ].RESET_DRAWSTACK( 0 );
                }





                var stateForDrag = targetO.hideFlags;

                foreach ( var VARIABLE in targetO.GetComponentsInChildren<Transform>( true ) )
                    VARIABLE.gameObject.hideFlags = stateForDrag & HideFlags.NotEditable | VARIABLE.gameObject.hideFlags & ~HideFlags.NotEditable;

            }
            Root.p[ 0 ].RepaintWindow( 0, true );
        }

        public static void UnclockAll()
        {   /* var obs = Selection.gameObjects;
             if (obs.Length == 0) return;*/
            if ( !Root.p[ 0 ].par_e.ENABLE_ALL ) return;
            var m = Root.p[0].modsController.rightModsManager.rightMods.FirstOrDefault(m2 => m2 is Mod_Freeze) as Mod_Freeze;
            if ( m == null || m.savedData == null || !m.savedData.enabled ) return;


            var all = HierarchyTempSceneData.GetAllObjectData(SaverType.ModFreezer, EditorSceneManager.GetActiveScene());
            foreach ( var item in all )
            {
                if ( !item.Value.target ) continue;
                item.Value.target.hideFlags &= ~HideFlags.NotEditable;
                item.Value.target.transform.hideFlags &= ~HideFlags.NotEditable;
            }
            HierarchyTempSceneDataGetter.SetAllObjectDataAndSave( SaverType.ModFreezer, EditorSceneManager.GetActiveScene(), new Dictionary<int, TempSceneObjectPTR>() );
            m.SetLockToggle( EditorSceneManager.GetActiveScene().GetHashCode() );
            /*     foreach ( var item in Resources.FindObjectsOfTypeAll<Transform>() )
				 {   item.gameObject.hideFlags &= ~HideFlags.NotEditable;
				 }*/
            Root.p[ 0 ].RESET_DRAWSTACK( 0 );
            Root.p[ 0 ].RepaintWindow( 0, true );
        }





        public Mod_Freeze( int restWidth, int sib, bool enable, PluginInstance adapter ) : base( restWidth, sib, enable, adapter )
        {
            UpdateSceneFlags( enable );
            /*  List<int> compare = new List<int>();
            
            UpdateSceneFlags(
            foreach (var gameObject in Utilities.AllSceneObjectsInterator())
            {
              compare.Add(gameObject.GetInstanceID());
            }
            List<int> compare2 = Utilities.AllSceneObjects(EditorSceneManager.GetActiveScene()).Select(g => g.GetInstanceID()).ToList();
            compare2.Reverse();
            MonoBehaviour.print(compare2.Count + " " +  compare.Count);
            if (compare2.Count != compare.Count) Debug.LogError("ASD");
            for (int i = 0; i < compare2.Count; i++)
            {
              if (compare[i] != compare2[i]) Debug.LogError("ASD11");
            }*/
            /* foreach (var gameObject in Utilities.AllSceneObjectsInterator())
                                     MonoBehaviour.print(gameObject.name);*/
        }

        // internal static Color colCache;
        internal static Color target = new Color(0.2f, 0.2f, 0.2f, 1);
        GUIContent content = new GUIContent();

        bool OnRawUp( Events.MouseRawUp.WantMouseLeaveType t )
        {
            if ( stateForDrag != null ) adapter.RepaintAllViews();

            stateForDrag = null;
            return true;
        }

        public override void Draw()
        {
            if ( !START_DRAW( drawRect, adapter.o ) ) return;

            var o = adapter.o.go;


            /* if ( !wasFreezeInit.ContainsKey( _o.scene ) ) {
                 wasFreezeInit.Add( _o.scene , true );
            
             }*/
            UpdateSceneFlags( true );

            if ( o.activeInHierarchy || (o.hideFlags & HideFlags.NotEditable) != 0 )
            {
                var oldRect = drawRect;

                //  Adapter.GET_SKIN().button.fontSize = 4;
                var oldW = drawRect.width;
                var oldH = drawRect.height;
                drawRect.width = drawRect.height = 12;
                drawRect.x += (oldW - drawRect.width) / 2;
                drawRect.y += (oldH - drawRect.height) / 2;
                //  MonoBehaviour.print(drawRect);
                /* colorText11.SetPixel(0, 0, (o.hideFlags & HideFlags.NotEditable) != 0 ? Color.blue : Color.gray);
                 colorText11.Apply();*/

                var c = Color.white;

                if ( !EditorGUIUtility.isProSkin )
                { //colCache = GUI.color;
                    c *= target;
                }

                //Adapter.DrawTexture( drawRect, (o.hideFlags & HideFlags.NotEditable) != 0 ? adapter.GetIcon( "LOCK" ) : adapter.GetIcon( "UNLOCK" ) );
                Draw_GUITexture( drawRect, (o.hideFlags & HideFlags.NotEditable) != 0 ? adapter.GetNewIcon( NewIconTexture.RightMods, "LOCK" ) : adapter.GetNewIcon( NewIconTexture.RightMods, "UNLOCK" ), c, true );

                if ( !EditorGUIUtility.isProSkin )
                { //GUI.color = colCache;
                }

                //  content.tooltip = objectIsHiddenAndLock ? "Object hidden" : "Left Click/Left DRAG Show/Hide GameObject \n( Right Click/Right DRAG - Focus on the object in the SceneView )";
                if ( oldRect.Contains( EVENT.mousePosition ) )
                    content.tooltip = (o.hideFlags & HideFlags.NotEditable) != 0 ? "This Object Locked" : "This Object Unlocked";

                /*  if ( !o.activeInHierarchy )
                  {   // var defColor = GUI.color;
                      c = Adapter.EditorBGColor;
                      c.a = 0.7f;
                      // GUI.color *= c;
                
                      Draw_AdapterTexture( drawRect, c, USE_GO: true );
                   //   GUI.DrawTexture( drawRect, EditorGUIUtility.whiteTexture );
                      // GUI.color = defColor;
                  }*/


                drawRect.y -= 2;


                Draw_Action( oldRect, SET_ACTIVE_ACTION_HASH, null );



                Draw_ModuleButton( oldRect, content, BUTTON_ACTION_HASH, true, useContentForButton: true, drawPointer: true );
                //   oldRect.y += 2;


                //  if ( adapter.ModuleButton( oldRect, content, true ) )
                { /*if (EVENT.button == 0)
					{
					
					
					    // Undo.RecordObject(o, "GameObject Lock");
					    var old = o.hideFlags & HideFlags.NotEditable;
					    if (old != 0) o.hideFlags &= ~old;
					    else o.hideFlags |= HideFlags.NotEditable;
					    // Hierarchy.SetDirty(o);
					    foreach (var VARIABLE in o.GetComponentsInChildren<Transform>(true))
					    {
					        //Und
					        //  Undo.RecordObject(VARIABLE.gameObject, "GameObject Lock");
					        VARIABLE.gameObject.hideFlags = o.hideFlags & HideFlags.NotEditable | VARIABLE.gameObject.hideFlags & ~HideFlags.NotEditable;
					        //Hierarchy.SetDirty(VARIABLE.gameObject);
					    }
					    Hierarchy.RepaintAllView();
					}*/


                    // EditorGUIUtility.ExitGUI();
                }
            }


            END_DRAW( adapter.o , savedData.temp_i);
        }

        static HideFlags? stateForDrag = null;

        bool Validate( HierarchyObject o )
        {
            return o.go && ((o.go.hideFlags & HideFlags.NotEditable) != 0);
        }

        internal void SetLockToggle( int scene )
        {
            // setDoubleList( scene );

            // if ( !Application.isPlaying )
            {
                //adapter.SetDirtyDescription(adapter.MOI.des(scene), scene);
                //adapter.MarkSceneDirty(scene);
            }
        }


        /* FillterData.Init(EVENT.mousePosition, SearchHelper, LayerMask.LayerToName(o.layer),
                     Validate(o) ?
                     CallHeaderFiltered(LayerMask.LayerToName(o.layer)) :
                     CallHeader(),
                     this);*/
        /** CALL HEADER */
        internal override Windows.SearchWindow.FillterData_Inputs CallHeader()
        {
            var result = new Windows.SearchWindow.FillterData_Inputs(callFromExternal_objects)
            {
                Valudator = Validate,
                SelectCompareString = (d, i) => i.ToString(),
                SelectCompareCostInt = (d, i) =>
                {
                    var cost = i;
                    cost += d.go.activeInHierarchy ? 0 : 100000000;
                    return cost;
                }
            };
            return result;
        }

        /* internal FillterData.FillterData_Inputs CallHeaderFiltered(string filter)
         {
             var result = CallHeader();
             result.Valudator = s => Validate(s) && LayerMask.LayerToName(s.layer) == filter;
             return result;
         }*/
        /** CALL HEADER */
        /* internal override bool CallHeader(out GameObject[] obs, out int[] contentCost)
         {
             obs = Utilities.AllSceneObjects().Where(Validate).ToArray();
             contentCost = obs.Select(o => o.activeInHierarchy ? 0 : 1).ToArray();
             return true;
         }*/
        DrawStackMethodsWrapper __BUTTON_ACTION_HASH = null;

        DrawStackMethodsWrapper BUTTON_ACTION_HASH {
            get { return __BUTTON_ACTION_HASH ?? (__BUTTON_ACTION_HASH = new DrawStackMethodsWrapper( BUTTON_ACTION )); }
        }

        void BUTTON_ACTION( Rect worldOffset, Rect inputRect, DrawStackMethodsWrapperData data, HierarchyObject _o )
        {
            if ( EVENT.button == 1 )
            {
                Tools.EventUse();
                /*   int[] contentCost = new int[0];
                   GameObject[] obs = new GameObject[0];
                
                   if (Validate(o))
                   {
                       if (EditorSceneManager.GetActiveScene().rootCount != 0) CallHeader(out obs, out contentCost);
                
                       FillterData.Init(EVENT.mousePosition, SearchHelper, "All", obs, contentCost, null, this);
                   } else
                   {
                       if (EditorSceneManager.GetActiveScene().rootCount != 0) CallHeader(out obs, out contentCost);
                
                       FillterData.Init(EVENT.mousePosition, SearchHelper, "All", obs, contentCost, null, this);
                   }*/

                var mp = new MousePos(EVENT.mousePosition, MousePos.Type.Search_356_0, !callFromExternal(), adapter);
                Windows.SearchWindow.Init( mp, SearchHelper, "All", CallHeader(), this, adapter.window, _o );
                // EditorGUIUtility.ic
            }
        }






        DrawStackMethodsWrapper __SET_ACTIVE_ACTION_HASH = null;

        DrawStackMethodsWrapper SET_ACTIVE_ACTION_HASH {
            get {
                if ( __SET_ACTIVE_ACTION_HASH == null )
                {
                    __SET_ACTIVE_ACTION_HASH = new DrawStackMethodsWrapper( SET_ACTIVE_ACTION );
                }

                return __SET_ACTIVE_ACTION_HASH;
            }
        }

        // int SET_ACTIVE_ACTION_HASH = "SET_ACTIVE_ACTION".GetHashCode();
        void SET_ACTIVE_ACTION( Rect worldOffset, Rect inputRect, DrawStackMethodsWrapperData data, HierarchyObject _o )
        {
            var tR = inputRect;
            var o = _o.go;

            if ( EVENT.rawType == EventType.MouseUp )
            {
                OnRawUp( Events.MouseRawUp.WantMouseLeaveType.MouseUp );
                // Hierarchy.RepaintAllView();
            }

            var contains = (tR.Contains(EVENT.mousePosition) || (adapter.hashoveredItem && adapter.hoverID == _o.id));

            if ( stateForDrag.HasValue && contains && EVENT.button == 0 )
            {
                //// GUI.DrawTexture( tR, adapter.STYLE_DEFBUTTON.active.background );
                adapter.gl.DRAW_TAP_GLOW( tR );
                var any = false;

                foreach ( var tr in o.GetComponentsInChildren<Transform>( true ) )
                { /* var res = stateForDrag.Value & HideFlags.NotEditable | VARIABLE.gameObject.hideFlags & ~HideFlags.NotEditable;
					 if ((res & VARIABLE.gameObject.hideFlags) != res )
					 {   VARIABLE.gameObject.hideFlags |= res;
					     any = true;
					 }*/
                    var check = tr.gameObject.hideFlags;

                    if ( stateForDrag.Value != 0 ) tr.gameObject.hideFlags &= ~stateForDrag.Value;
                    else tr.gameObject.hideFlags |= HideFlags.NotEditable;

                    any |= check != tr.gameObject.hideFlags;

                    if ( check != tr.gameObject.hideFlags )
                    {

                        //Undo.RecordObject( tr.gameObject, "ASD" );
                        if ( (tr.gameObject.hideFlags & HideFlags.NotEditable) != 0 )
                            HierarchyTempSceneDataGetter.SetObjectData( SaverType.ModFreezer, Cache.GetHierarchyObjectByInstanceID( tr.gameObject ), true );
                        //getDoubleList(VARIABLE.gameObject.scene.GetHashCode()).SetByKey(new GoGuidPair() {go = VARIABLE.gameObject.gameObject}, true);
                        else
                            HierarchyTempSceneDataGetter.RemoveObjectData( SaverType.ModFreezer, Cache.GetHierarchyObjectByInstanceID( tr.gameObject ) );
                        // getDoubleList( tr.gameObject.scene.GetHashCode() ).RemoveAll( new GoGuidPair() { go = tr.gameObject.gameObject } );
                        SetLockToggle( tr.gameObject.scene.GetHashCode() );
                        RS();

                        //Undo.undoRedoPerformed();
                    }

                    /* if (Selection.gameObjects.Any(g => g == VARIABLE.gameObject))
                     {
                         Selection.objects = Selection.objects;
                     Hierarchy.RepaintAllViews();
                     }*/
                }

                //if ( any ) ResetStack();

                if ( EVENT.isMouse ) Tools.EventUse();
            }

            if ( tR.Contains( EVENT.mousePosition ) )
            {
                if ( tR.Contains( EVENT.mousePosition ) && EVENT.type == EventType.MouseDown && EVENT.button == 0 )
                {
                    var targetOarr = new[] { o };
                    var sel = adapter.ha.SELECTED_GAMEOBJECTS();

                    if ( sel.Any( c => c.id == _o.id ) /*&& EVENT.control*/) targetOarr = Tools.GetOnlyTopObjects( sel ).Select( g => g.go ).ToArray();
                    ;

                    //  targetOarr = Utilities.GetOnlyTopObjects( targetOarr , adapter ).Select( g => g.go ).ToArray(); ;


                    var old = o.hideFlags & HideFlags.NotEditable;

                    // bool needSelect = false;
                    foreach ( var targetO in targetOarr )
                    {
                        var checkValue = targetO.hideFlags;

                        if ( old != 0 ) targetO.hideFlags &= ~old;
                        else targetO.hideFlags |= HideFlags.NotEditable;

                        if ( checkValue != targetO.hideFlags )
                        {
                            if ( (targetO.hideFlags & HideFlags.NotEditable) != 0 )
                                // getDoubleList( targetO.scene.GetHashCode() ).SetByKey( new GoGuidPair() { go = targetO.gameObject }, true );
                                // else
                                // getDoubleList( targetO.scene.GetHashCode() ).RemoveAll( new GoGuidPair() { go = targetO.gameObject } );
                                HierarchyTempSceneDataGetter.SetObjectData( SaverType.ModFreezer, Cache.GetHierarchyObjectByInstanceID( targetO.gameObject ), true );
                            //getDoubleList(VARIABLE.gameObject.scene.GetHashCode()).SetByKey(new GoGuidPair() {go = VARIABLE.gameObject.gameObject}, true);
                            else
                                HierarchyTempSceneDataGetter.RemoveObjectData( SaverType.ModFreezer, Cache.GetHierarchyObjectByInstanceID( targetO.gameObject ) );
                            //ResetStack();
                            RS();

                            SetLockToggle( targetO.scene.GetHashCode() );
                        }


                        if ( stateForDrag == null ) adapter.PUSH_ONMOUSEUP( 0, OnRawUp );

                        stateForDrag = old;

                        var any = false;

                        //  bool needRepaint = false;
                        foreach ( var VARIABLE in targetO.GetComponentsInChildren<Transform>( true ) ) // Undo.RecordObject(VARIABLE, "Change Lock/Unlock state");
                        { // VARIABLE.gameObject.hideFlags = stateForDrag.Value & HideFlags.NotEditable | VARIABLE.gameObject.hideFlags & ~HideFlags.NotEditable;

                            var check = targetO.hideFlags;

                            if ( stateForDrag.Value != 0 ) targetO.hideFlags &= ~stateForDrag.Value;
                            else targetO.hideFlags |= HideFlags.NotEditable;

                            any |= check != targetO.hideFlags;

                            if ( check != targetO.hideFlags )
                            {
                                if ( (targetO.hideFlags & HideFlags.NotEditable) != 0 )
                                    //  getDoubleList( targetO.scene.GetHashCode() ).SetByKey( new GoGuidPair() { go = targetO.gameObject }, true );
                                    //  else
                                    //   getDoubleList( targetO.scene.GetHashCode() ).RemoveAll( new GoGuidPair() { go = targetO.gameObject } );
                                    HierarchyTempSceneDataGetter.SetObjectData( SaverType.ModFreezer, Cache.GetHierarchyObjectByInstanceID( targetO.gameObject ), true );
                                //getDoubleList(VARIABLE.gameObject.scene.GetHashCode()).SetByKey(new GoGuidPair() {go = VARIABLE.gameObject.gameObject}, true);
                                else
                                    HierarchyTempSceneDataGetter.RemoveObjectData( SaverType.ModFreezer, Cache.GetHierarchyObjectByInstanceID( targetO.gameObject ) );
                                SetLockToggle( targetO.scene.GetHashCode() );
                                
                            }


                            //EditorUtility.SetDirty(VARIABLE);
                            /*  if ( Selection.gameObjects.Any( g => g == VARIABLE.gameObject ) )
                              {   needSelect = true;
                                  // Hierarchy.RepaintAllViews();
                              }*/
                        }

                        if ( any ) RS(); //ResetStack();
                    }

                    // if ( needSelect ) Selection.objects = Selection.objects;
                    adapter.RepaintAllViews();
                    //  Hierarchy.RepaintAllView();
                }

                if ( EVENT.isMouse && EVENT.button == 0 ) Tools.EventUse();
            }
        }

        //EMX_TODO change label after freeze apply
        void RS()
        {
           // Root.p[ 0 ].invoke_onHierarchyChanged();
            Cache.ClearHierarchyObjects( false );
           // Root.p[ 0 ].modsController.highLighterMod.ClearCacheAdditional();
           // Root.p[ 0 ].RESET_DRAWSTACK( 0 );
            /*/*/
        }
    }
}
