using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace EMX.HierarchyPlugin.Editor.Mods
{





	class LastSelectionHistoryModInstance
    {


        static PluginInstance adapter { get { return Root.p[ 0 ]; } }
        static MemType cType = MemType.Last;


        /*static bool compare_additive(GameObject[] source)
		{
			var so = Selection.gameObjects;
			if (Mathf.Abs(so.Length - source.Length) != 1) return false;
			return false;
		}*/

        internal void SubscribeEditorInstance( AdditionalSubscriber sbs )
        {

            //sbs.OnSelectionChanged -= LastSelectionHistoryModWindowInstance.OnSelectionChangeStatic;
            //sbs.OnSelectionChanged -= OnSelectionChangeStatic;
            sbs.OnSelectionChanged += OnSelectionChangeStatic;

			sbs.OnSelectionChanged += GameObjectsSelectionHistoryModWindow.OnSelectionChangeStatic;


			/*	sbs.OnSceneOpening += instance.SCENE_CHANGE;
				sbs.OnSelectionChanged += instance.CHANGE_SELECTION;
				sbs.OnPlayModeStateChanged += instance.CHANGEPLAYMODE;
				sbs.OnUpdate += instance.Update;*/
		}

		static int lastSelectionID = -1;
        internal static void OnSelectionChangeStatic()
        {
            //Debug.Log(Selection.activeGameObject.FirstOrDefault());
            //adapter.PUSH_GUI_ONESHOT(_asd);

            if ( lastSelectionID == adapter.SelectionID ) return;
            lastSelectionID = adapter.SelectionID;

            if ( !Selection.activeGameObject ) return;
            //	Debug.Log(DrawButtonsOld.GET_DISPLAY_PARAMS(MemType.Last).LastIndex);
            var skip = adapter.SkipSwitch && adapter.SkipRemove;
            adapter.PUSH_GUI_ONESHOT( 0, () => {
                if ( !skip )
                {
                    var ex = DrawButtonsOld.GET_OBJECTS_LIST(cType, null, adapter.LastActiveScene);
                    var res2 = Selection.gameObjects.ToDictionary(g=>g.GetInstanceID(),g=>g);
                    GameObject[] res;
                    if ( res2.Count != 0 )
                    {
                        if ( res2.Count > 1 )
                        {
                            res2.Remove( Selection.activeGameObject.GetInstanceID() );
                            var t = res2.Values.ToArray();
                            res = new GameObject[ t.Length + 1 ];
                            res[ 0 ] = Selection.activeGameObject;
                            for ( int i = 0; i < t.Length; i++ ) res[ i + 1 ] = t[ i ];
                            // res.Insert( 0, Selection.activeGameObject );
                        }
                        else
                        {
                            res = new[] { Selection.activeGameObject };
                        }

                        if ( ex.Count == 0 || Event.current != null && (Event.current.control || Event.current.shift)
                        && (ex[ 0 ].gos_get().FirstOrDefault() == res[ 0 ] || ex[ 0 ].gos_get_last() == res[ res.Length - 1 ])
                        )
                        {
                            if ( ex.Count == 0 ) ex.Add( new DrawButtonsOld.GameObjectMemory( new GameObject[ 0 ], 0, -1, null , MemType.Last ) );
                            ex[ 0 ].gos_set( res );
                        }
                        else
                        {
                            ex.Insert( 0, new DrawButtonsOld.GameObjectMemory( res, 0, -1, null , MemType.Last) );
                        }

                        for ( int i = 0; i < ex.Count; i++ )
                        {
                            if ( !DrawButtonsOld.was_draw_dic.ContainsKey( ex[ i ].unique_id ) ) DrawButtonsOld.was_draw_dic.Add( ex[ i ].unique_id, false );
                            DrawButtonsOld.was_draw_dic[ ex[ i ].unique_id ] = false;
                        }
                        for ( int i = 0; i < ex.Count; i++ )
                        {
                            if ( !ex[ i ].IsValid() || !ex[ i ].IsActive() ) { ex[ i ] = null; continue; }
                            if ( DrawButtonsOld.was_draw_dic.ContainsKey( ex[ i ].unique_id ) && DrawButtonsOld.was_draw_dic[ ex[ i ].unique_id ] ) { ex[ i ] = null; continue; }
                            if ( DrawButtonsOld.was_draw_dic.ContainsKey( ex[ i ].unique_id ) ) DrawButtonsOld.was_draw_dic[ ex[ i ].unique_id ] = true;
                        }
                        for ( int i = ex.Count - 1; i >= 0; i-- )
                        {
                            if ( ex[ i ] == null ) ex.RemoveAt( i );
                        }
                        while ( ex.Count > 30 ) ex.RemoveAt( 30 );
                        DrawButtonsOld.SET_OBJECTS_LIST( ex, cType, null, adapter.LastActiveScene, skipUndo: true );
                        if ( ex.Count > 0 ) DrawButtonsOld.GET_DISPLAY_PARAMS( MemType.Last ).LastSelectedRoot = ex[ 0 ].unique_id;
                    }

                }
                adapter.RepaintExternalNow();
            } );


        }




        internal static void MoveSelect( int offset )
        {
            if ( !adapter.par_e.ENABLE_ALL || !adapter.par_e.USE_LAST_SELECTION_MOD ) return;

            var memoryRoot = DrawButtonsOld.GET_OBJECTS_LIST(cType, null, adapter.LastActiveScene);
            if ( memoryRoot == null || memoryRoot.Count == 0 ) return;



            for ( int i = 0; i < memoryRoot.Count; i++ )
            {
                if ( !DrawButtonsOld.was_draw_dic.ContainsKey( memoryRoot[ i ].unique_id ) ) DrawButtonsOld.was_draw_dic.Add( memoryRoot[ i ].unique_id, false );
                DrawButtonsOld.was_draw_dic[ memoryRoot[ i ].unique_id ] = false;
            }
            //	Debug.Log(DrawButtonsOld.GET_DISPLAY_PARAMS(MemType.Last).LastIndex);
            //Debug.Log(DrawButtonsOld.GET_DISPLAY_PARAMS(MemType.Last).LastSelectedRoot);

            var estimIndex = DrawButtonsOld.GET_DISPLAY_PARAMS(cType).LastIndex;
            if ( DrawButtonsOld.GET_DISPLAY_PARAMS( cType ).LastSelectedRoot != -1 )
            {
                for ( int i = 0; i < memoryRoot.Count; i++ )
                {

                    if ( !memoryRoot[ i ].IsValid() || !memoryRoot[ i ].IsActive() ) continue;
                    if ( DrawButtonsOld.was_draw_dic.ContainsKey( memoryRoot[ i ].unique_id ) && DrawButtonsOld.was_draw_dic[ memoryRoot[ i ].unique_id ] ) continue;
                    if ( DrawButtonsOld.was_draw_dic.ContainsKey( memoryRoot[ i ].unique_id ) ) DrawButtonsOld.was_draw_dic[ memoryRoot[ i ].unique_id ] = true;
                    //Debug.Log(memoryRoot[i].unique_id + " " + i + " " + DrawButtonsOld.GET_DISPLAY_PARAMS(MemType.Last).LastSelectedRoot);
                    if ( DrawButtonsOld.GET_DISPLAY_PARAMS( cType ).LastSelectedRoot == memoryRoot[ i ].unique_id )
                    {
                        estimIndex = i;
                        break;
                    }
                }
            }


            for ( int i = 0; i < memoryRoot.Count; i++ )
            {
                if ( !DrawButtonsOld.was_draw_dic.ContainsKey( memoryRoot[ i ].unique_id ) ) DrawButtonsOld.was_draw_dic.Add( memoryRoot[ i ].unique_id, false );
                DrawButtonsOld.was_draw_dic[ memoryRoot[ i ].unique_id ] = false;
            }

            do
            {
                estimIndex += offset;
                if ( estimIndex < 0 || estimIndex >= memoryRoot.Count ) break;

                if ( !memoryRoot[ estimIndex ].IsValid() || !memoryRoot[ estimIndex ].IsActive() ) continue;
                if ( DrawButtonsOld.was_draw_dic.ContainsKey( memoryRoot[ estimIndex ].unique_id ) && DrawButtonsOld.was_draw_dic[ memoryRoot[ estimIndex ].unique_id ] ) continue;
                if ( DrawButtonsOld.was_draw_dic.ContainsKey( memoryRoot[ estimIndex ].unique_id ) ) DrawButtonsOld.was_draw_dic[ memoryRoot[ estimIndex ].unique_id ] = true;

                adapter.SkipSwitch = true;
                if ( !memoryRoot[ estimIndex ].OnClick( true, adapter.LastActiveScene.GetHashCode(), new ExternalDrawContainer( 0 ) { /*type = cType*/ } ) )
                {
                    adapter.RepaintWindow( 0, true );
                }
                else
                {
                    //DrawButtonsOld.GET_DISPLAY_PARAMS(MemType.Last).LastIndex = estimIndex;
                }
                break;

            } while ( true );
        }






        Rect Round( Rect r )
        {
            r.x = Mathf.RoundToInt( r.x );
            r.y = Mathf.RoundToInt( r.y );
            r.width = Mathf.RoundToInt( r.width );
            r.height = Mathf.RoundToInt( r.height );
            return r;
        }

        internal DrawButtonsOld dob = new DrawButtonsOld();
        internal void DoLast( Rect line, ExternalDrawContainer controller, Scene scene )
        {

            var rowParams = DrawButtonsOld.GET_DISPLAY_PARAMS(cType);

           // if ( Event.current.type == EventType.Repaint )
           // {
           //     if ( rowParams.BgOverrideColor.a != 0 )
           //         EditorGUI.DrawRect( line, rowParams.BgOverrideColor * GUI.color );
           // }

            var swap = rowParams.SortButtonsOrder % 2 == 0;
            // var swap = false;
            line = Round( line );
            var plus = line;
            // var POFF = 3;
            // plus.x += POFF;
            plus.width = ExternalModStyles.LINE_HEIGHT_FOR_BUTTONS( line.height );
            plus.x = (!swap ? line.width - plus.width * 2 : 0) + line.x + 1;
            //  plus.height -= 3;

            // if ( UNITY_CURRENT_VERSION >= UNITY_2019_3_0_VERSION ) plus.x += adapter.TOTAL_LEFT_PADDING_FORBOTTOM;


            if ( Event.current.type == EventType.MouseDown && plus.Contains( Event.current.mousePosition ) )
            {
                controller.selection_button = 20;
                controller.selection_window = controller.tempRoot;
                var captureRect = plus;
                controller.selection_action = ( mouseUp, deltaTIme ) => {
                    if ( mouseUp && captureRect.Contains( Event.current.mousePosition ) )
                    {
                        if ( swap ) MainMenuItems.MoveSelNext();
                        else MainMenuItems.MoveSelPrev();
                    }

                    return Event.current.delta.x == 0 && Event.current.delta.x == 0;
                }; // ACTION
            }

            if ( Event.current.type == EventType.Repaint ) //MonoBehaviour.print(Adapter.GET_SKIN().button.normal.textColor);
            {
                adapter.STYLE_LASTSEL_BUTTON.Draw( plus, ExternalModStyles.ContentSelBack, false, false, false,
                    plus.Contains( Event.current.mousePosition ) && controller.selection_button == 20 );
            }

            GUI.Label( plus, swap ? ExternalModStyles.ContentSelForwLabel : ExternalModStyles.ContentSelBackLabel );
            EditorGUIUtility.AddCursorRect( plus, MouseCursor.Link );

            plus.x += plus.width;
            if ( Event.current.type == EventType.MouseDown && plus.Contains( Event.current.mousePosition ) )
            {
                controller.selection_button = 21;
                controller.selection_window = controller.tempRoot;
                var captureRect = plus;
                controller.selection_action = ( mouseUp, deltaTIme ) => {
                    if ( mouseUp && captureRect.Contains( Event.current.mousePosition ) )
                    {
                        if ( swap ) MainMenuItems.MoveSelPrev();
                        else MainMenuItems.MoveSelNext();
                    }

                    return Event.current.delta.x == 0 && Event.current.delta.x == 0;
                }; // ACTION
            }

            if ( Event.current.type == EventType.Repaint )
            {
                adapter.STYLE_LASTSEL_BUTTON.Draw( plus, ExternalModStyles.ContentSelForw, false, false, false,
                    plus.Contains( Event.current.mousePosition ) && controller.selection_button == 21 );
            }

            GUI.Label( plus, swap ? ExternalModStyles.ContentSelBackLabel : ExternalModStyles.ContentSelForwLabel );
            EditorGUIUtility.AddCursorRect( plus, MouseCursor.Link );

            line.x += !swap ? 0 : plus.width * 2;
            line.width -= plus.width * 2;
            //line.y -= 1;


            if ( !dob.DrawButtons( line, cType,
				// RowsParams[PLUGIN_ID.LAST].HiglighterValue
				/* &&*/
				//rowParams.BgOverrideColor.a != 0 ? rowParams.BgOverrideColor : Color.white

				rowParams.BgOverrideColor
                , controller, scene ) )
            {
                var tooltip = DrawButtonsOld.GETTOOLTIPPEDCONTENT(cType, null, controller);
                // tooltip.text = "-";
                tooltip.text = "";
                GUI.Label( line, tooltip );
                GUI.Label( line, "- - -", adapter.STYLE_LABEL_10_middle );
            }
        }



    }
}
