using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class PK_Window : ScriptableObject
    {
    }


    [CustomEditor( typeof( PK_Window ) )]
    class SETGUI_PlayModeKeeper : MainRoot
    {

        internal static string set_text =  USE_STR + "PlayMode Preserving [Play/Stop]";
        internal static string set_key = "USE_PLAYMODE_SAVER_MOD";
        public override VisualElement CreateInspectorGUI()
        {
            return base.CreateInspectorGUI();
        }
        public override void OnInspectorGUI()
        {
            _GUI( (IRepaint)this );
        }
        public static void _GUI( IRepaint w )
        {
            Draw.RESET(w);

            Draw.BACK_BUTTON( w );
            Draw.TOG_TIT( set_text, set_key ,WIKI: WIKI_4_PLAYMODE);
            Draw.Sp( 10 );

            using ( ENABLE(w).USE( set_key ) )
            {
                using ( GRO(w).UP(0) )
                {


					Draw.Sp( 4 );

					Draw.TOG( "Draw icons for not included objects", "PLAYMODESAVER_DRAW_ICONS_FOR_UNASSIGNED" );
                    using ( ENABLE(w).USE( "PLAYMODESAVER_HIDE_ICONS_FOR_UNASSIGNED", inverce: true ) )
                        Draw.FIELD( "Not included icons opacity", "PLAYMODESAVER_OPACITY_DISABLED_ICONS" , 0, 1);

					// Draw.TOG( "Save GameObjects References", "PLAYMODESAVER_CHANGE_BUTTON_CURSOR" );
					Draw.Sp( 2 );

					Draw.TOG( "Change cursor if mouse hovering over the content", "PLAYMODESAVER_CHANGE_BUTTON_CURSOR" );


                    Draw.Sp( 10 );
                    using ( GRO(w).UP(0) )
                    {
                        Draw.TOG( "Save GameObjects References", "PLAYMODESAVER_SAVE_UNITYOBJECT" );
                    }

                    Draw.Sp( 10 );
                    GUI.Label( Draw.R, "Special preserving options:" );
                    Draw.HELP(w, "Use left keeper button in the hierarchy window to enable special keeper options.", drawTog: true );
                    Draw.TOG( "Save adding/removing components for included objects", "PLAYMODESAVER_USE_ADD_REMOVE_COMPONENTS" );
                    Draw.TOG( "Save setactive states gameobjects for included objects", "PLAYMODESAVER_SAVE_ENABLINGDISABLING_GAMEOBJEST_MENU" );
                    Draw.TOG( "Save gameobjects hierarchy position for included objects", "PLAYMODESAVER_SAVE_GAMEOBJET_HIERARCHY_MENU" );
                    Draw.Sp( 4 );
                //}

                Draw.Sp( 10 );
                //using ( GRO(w).UP(0) )
               // {
               
                    Draw.TOG_TIT( "Automatically include components from the list below", "PLAYMODESAVER_SAVE_USE_PERMANENT_LIST_OF_MONOSCRIPTS" );
                    using ( ENABLE(w).USE( "PLAYMODESAVER_SAVE_USE_PERMANENT_LIST_OF_MONOSCRIPTS", 0 ) )
                    {
                        DrawDataKeeperScripts( Draw.RH( 0 ) );
                    }
                    Draw.HELP(w, "Not that - special preserving options won't be applied to objects with these components, you have to manually enable them.", drawTog: true );
					Draw.Sp( 4 );
				}




                Draw.Sp( 10 );
                //Draw.HRx2();
                //GUI.Label( Draw.R, "" + LEFT + " Area:" );
                using ( GRO(w).UP(0) )
                {

                    // Draw.TOG_TIT( "" + LEFT + " Area:" );

                    Draw.TOG_TIT( "Quick tips:" );
                  //  Draw.HELP_TEXTURE(w, "HELP_KEEPER" );
                    Draw.HELP(w, "1 gui button - you can include individual component or enable special preserving options for selected object.", drawTog: true );
                    Draw.HELP(w, "2 gui button - includes all components for selected object, but, in this case special keeper options will not apply.", drawTog: true );
                    Draw.HELP(w, "You can include component in the keeper during playback, it is handy, because it saves the object once time only.", drawTog: true );
                    Draw.HELP(w, "If you disable this mod, included objects will be saved, but keeper won't preserve until you turn module on again.", drawTog: true );
					Draw.HELP( w, "You can undo preserved operations.", drawTog: true );
                }
            }
        }







        static void DrawDataKeeperScripts( Rect boxRect )
        { //  var boxRect = DRAW_GREENLINE_ANDGETRECT( ref LAST_RECT, A.par.DataKeeperParams.USE_SCRIPTS, true );
          //    var boxRect = EditorGUILayout.GetControlRect(GUILayout.Height(0));
            boxRect.x += 4;
            //  boxRect.y += boxRect.height;
            PKeeperSet.ARRAY = HierarchyCommonData.Instance().PlayModeSaverPreservedScripts_GET;
            boxRect.height = PKeeperSet.KEEPER_HEIGHT + 16;

            var R = boxRect;
            R.height = 25;
            R.y += 10;

            boxRect.width -= 10;
            p.INTERNAL_BOX( boxRect, "" );

            // R = EditorGUILayout.GetControlRect( GUILayout.Height( PKC.KEEPER_HEIGHT ) );
            R = boxRect;
            R.x += 8;
            R.y += 8;
            R.width -= 16;
            R.height -= 16;

            PKeeperSet.DRAW_KEEPER_SCRIPTS( EditorWindow.focusedWindow, R );
            PKeeperSet.KEEPER_UPDATE( EditorWindow.focusedWindow );

            GUILayout.Space( boxRect.height );
        }
















       // float? tileCalcKeeperIcons;








       static DRAW_KEEPER_SCRIPTS_Class __PKC;

      static  internal DRAW_KEEPER_SCRIPTS_Class PKeeperSet {
            get {
                var res = __PKC ?? (__PKC = new DRAW_KEEPER_SCRIPTS_Class());
                res.A = p;
                return res;
            }
        }

        public class DRAW_KEEPER_SCRIPTS_Class
        {
            internal List<string> ARRAY;
            public float KEEPER_HEIGHT {
                get { return ARRAY.Count * IC_H + IC_H; }
            }

            GUIContent KEEPER_PlusContent = new GUIContent()
            {
                text = "+",
                tooltip = "Drag MonoBehaviour Script "
            };

            GUIContent KEEPER_PlusContentEmpty = new GUIContent()
            {
                text = "",
                tooltip = "Drag MonoBehaviour Script"
            };

            public const float IC_H = 36;
            public float MouseY = -1;
            EditorWindow win;

            /*   List<Hierarchy_GUI.DataKeeperValue> ARRAY
               {
                   return Hierarchy_GUI.Instance(A).m_DataKeeper_Values;
               }
               */
            public PluginInstance A;

            public void DRAW_KEEPER_SCRIPTS( EditorWindow win, Rect lr )
            {
                this.win = win;

                A.ChangeGUI();
                int i;

                // content.tooltip = "User Icons";
                // GUILayout.Label("");
                //  var lr = GetControlRect( EditorGUIUtility.singleLineHeight );
                A.INTERNAL_BOX( new Rect( lr.x, lr.y, lr.width, lr.height ), KEEPER_PlusContentEmpty );

                KEEPER_MOUSE_Y = EditorGUIUtility.GUIToScreenPoint( Vector2.zero ).y;
                /*if (dragContent)
                {
                    Adapter. INTERNAL_BOX(new Rect(0, 0, W, KEEPER_HEIGHT), content);
                }*/
                //    ExampleDragDropGUI(new Rect(0, 0, W, KEEPER_HEIGHT), null);

                if ( KEEPER_UPDATE_currentY.Length != ARRAY.Count )
                {
                    KEEPER_UPDATE_currentY = new float[ ARRAY.Count ];
                    for ( i = 0; i < ARRAY.Count; i++ )
                        KEEPER_UPDATE_currentY[ i ] = i * IC_H;
                }

                // var lineRect = new Rect(0, 0, W, H);
                for ( i = 0; i < ARRAY.Count; i++ )
                {
                    // var customIcon = GET_ARRAT()[i];
                    var r = new Rect(lr.x, lr.y + KEEPER_UPDATE_currentY[i], lr.width, IC_H);

                    if ( dragIndex == i )
                    {
                        r.x = Event.current.mousePosition.x - IC_H / 2;
                        r.y = Event.current.mousePosition.y - IC_H / 2;
                    }

                    // GUI.BeginClip(r);
                    DRAW_KEEPER_LINE( i, lr, r.x, r.y );
                    // GUI.EndClip();
                    // lineRect.y += H;
                }


                var lineRect = new Rect(lr.x, lr.y + ARRAY.Count * IC_H, lr.width, IC_H);
                CustomDragData.ExampleDragDropGUI( A, lineRect, null, CustomDragData.DRAG_VALIDATOR_ONLYMONO, KEEREPR_DRAG_PERFORM );
                /*  if (lineRect.Contains(Event.current.mousePosition))
                  {
                      if (Event.current.type.Equals(EventType.Repaint)) GUI.DrawTexture(lineRect,Hierarchy.sec);
                  }*/
                var olds = A.button.fontSize;
                A.button.fontSize = 20;
                var butres = GUI.Button(lineRect, KEEPER_PlusContent, A.button);
                A.button.fontSize = olds;

                if ( butres )
                {
                    if ( Event.current.button == 0 ) KEEPER_CREATE_LINE( null, int.MaxValue );
                }

                A.RestoreGUI();
            }

            void KEEREPR_DRAG_PERFORM()
            {
                KEEPER_CREATE_LINE( DragAndDrop.objectReferences[ 0 ] as MonoScript, int.MaxValue );
            }

            int dragIndex = -1;

            void DRAW_KEEPER_LINE( int i, Rect lr, float xOffset, float yOffset )
            { // ExampleDragDropGUI(lineRect, new CustomDragData() { index = i });

                MonoScript script = null;
                if ( !string.IsNullOrEmpty( ARRAY[ i ] ) )
                {
                    var path = AssetDatabase.GUIDToAssetPath(ARRAY[i]);
                    if ( !string.IsNullOrEmpty( path ) ) script = AssetDatabase.LoadAssetAtPath<MonoScript>( path );
                }


                /*  if (!string.IsNullOrEmpty(ARRAY[i].Key))
                  {
                      var scriptPath = AssetDatabase.GUIDToAssetPath(ARRAY[i].Key);
                      if (!string.IsNullOrEmpty(scriptPath)) script = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
                  }*/


                var r = new Rect(0, 0, IC_H, IC_H);

                r.x += xOffset;
                r.y += yOffset;

                var oldsl = A.label.fontSize;
                A.label.fontSize = 16;
                //GUI.Label(r, "■");
                //  if (GUI.Button(r, "▲"))
                GUI.Label( r, "=", A.label );
                A.label.fontSize = oldsl;

                if ( r.Contains( Event.current.mousePosition ) ) win.Repaint();

                if ( r.Contains( Event.current.mousePosition ) && Event.current.type == EventType.MouseDown )
                {
                    if ( Event.current.button == 0 )
                    {
                        dragIndex = i;
                        /*InternalEditorUtility.repa*/
                        win.Repaint();

                        A.RepaintWindowInUpdate( 0 );
                    }
                }

                EditorGUIUtility.AddCursorRect( r, MouseCursor.Link );
                /* if (dragContent)*/

                // ExampleDragDropGUI(r, new CustomDragData() { index = i });

                A.RestoreGUI();

                r.Set( r.width + 10, (r.height - EditorGUIUtility.singleLineHeight) / 2, lr.width - IC_H * 2, EditorGUIUtility.singleLineHeight );

                r.x += xOffset;
                r.y += yOffset;


                MonoScript newScript = script;
                try
                {
                    newScript = (MonoScript)EditorGUI.ObjectField( r, script, typeof( MonoScript ), false );
                }
                catch
                {
                    newScript = script;
                }


                A.ChangeGUI();

                if ( newScript != script )
                { //var v = ARRAY[i];
                  // v.Key = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newScript));
                  // ARRAY[i] = v;
                  // Hierarchy_GUI.Instance(A).DataKeeper_SetScript(i, newScript);

                    HierarchyCommonData.Instance().SetUndo( "Apply PlayModeKeeper Settings" );

                    ARRAY[ i ] = AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( newScript ) );
                    HierarchyCommonData.Instance().PlayModeSaverPreservedScripts_GET = ARRAY;

                    HierarchyCommonData.Instance().SetDirty();

                    A.RESET_DRAWSTACK( 0 );
                    A.RepaintWindowInUpdate( 0 );
                    // Hierarchy.RepaintAllView();
                }


                r.Set( lr.width - IC_H + 5, 0, IC_H - 10, IC_H );
                r.x += xOffset;
                r.y += yOffset;
                if ( GUI.Button( r, "X", A.button ) )
                {
                    if ( Event.current.button == 0 )
                    {
                        KEEPER_REMOVE_LINE( i );
                        //CreateLine(null, null, int.MaxValue);
                    }

                    //dragContent = true;
                }
            }

            float KEEPER_MOUSE_Y;
            float[] KEEPER_UPDATE_currentY = new float[0];

            public void KEEPER_UPDATE( EditorWindow win )
            {
                if ( Event.current.type == EventType.Repaint && KEEPER_UPDATE_currentY.Length != 0 )
                {
                    var tempDragIndex = dragIndex == -1
                        ? -1
                        : Mathf.Clamp(Mathf.RoundToInt((EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition).y - KEEPER_MOUSE_Y - IC_H / 2) / (float)IC_H), 0,
                            KEEPER_UPDATE_currentY.Length - 1);


                    for ( int i = 0, sib = 0; i < KEEPER_UPDATE_currentY.Length; i++, sib++ )
                    { // if (tempDragIndex == i && i > dragIndex) sib--;
                      //if (tempDragIndex == i && i < dragIndex) sib++;
                        if ( dragIndex != -1 && i > dragIndex && i <= tempDragIndex ) sib = i - 1;
                        else if ( dragIndex != -1 && i < dragIndex && i >= tempDragIndex ) sib = i + 1;
                        else sib = i;
                        KEEPER_UPDATE_currentY[ i ] = Mathf.Lerp( KEEPER_UPDATE_currentY[ i ], sib * IC_H, 0.5f );
                    }

                    //print(tempDragIndex);
                    if ( dragIndex != -1 )
                    {
                        win.Repaint();
                        //Hierarchy.RepaintAllView();
                    }
                }

                if ( Event.current.rawType == EventType.MouseUp )
                {
                    var tempDragIndex = dragIndex == -1
                        ? -1
                        : Mathf.Clamp(Mathf.RoundToInt((EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition).y - KEEPER_MOUSE_Y - IC_H / 2) / (float)IC_H), 0,
                            KEEPER_UPDATE_currentY.Length - 1);
                    if ( dragIndex != -1 && tempDragIndex != -1 && tempDragIndex != dragIndex )
                    {
                        KEEPER_SWITCH_POSES( dragIndex, tempDragIndex );
                        KEEPER_UPDATE_currentY = new float[ 0 ];
                        A.RepaintWindowInUpdate( 0 );
                        win.Repaint();
                        // Hierarchy.RepaintWindow();
                    }

                    dragIndex = -1;
                }
            }


            void KEEPER_SWITCH_POSES( int i1, int i2 )
            {
                // Hierarchy_GUI.Undo(A, "Change PlayMode Data Keeper");


                // var min = Math.Min(i1, i2);
                // var max = Math.Max(i1, i2);

                HierarchyCommonData.Instance().SetUndo( "Apply PlayModeKeeper Settings" );

                var v1 = ARRAY[i1];
                ARRAY.RemoveAt( i1 );

                if ( i2 >= ARRAY.Count ) ARRAY.Add( v1 );
                else ARRAY.Insert( i2, v1 );

                // Hierarchy_GUI.SetDirtyObject(A);
                HierarchyCommonData.Instance().PlayModeSaverPreservedScripts_GET = ARRAY;
                A.RESET_DRAWSTACK( 0 );
                A.RepaintWindowInUpdate( 0 );
                // Hierarchy.RepaintAllView();

                HierarchyCommonData.Instance().SetDirty();
            }

            void KEEPER_REMOVE_LINE( int index )
            {
                if ( index < 0 || index >= ARRAY.Count ) return;
                //   Hierarchy_GUI.Undo(A, "Change PlayMode Data Keeper");

                HierarchyCommonData.Instance().SetUndo( "Apply PlayModeKeeper Settings" );
                //  Hierarchy_GUI.Instance(A).DataKeeper_RemoveAt(index);
                ARRAY.RemoveAt( index );
                //    Hierarchy_GUI.SetDirtyObject(A);
                HierarchyCommonData.Instance().PlayModeSaverPreservedScripts_GET = ARRAY;
                A.RESET_DRAWSTACK( 0 );
                A.RepaintWindowInUpdate( 0 );
                //  Hierarchy.RepaintAllView();
                HierarchyCommonData.Instance().SetDirty();
            }

            void KEEPER_CREATE_LINE( MonoScript script, int index )
            { //  string key = null;
              //Hierarchy_GUI.CustomIconParams value = new Hierarchy_GUI.CustomIconParams();
              // if (script != null) key = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(script));

                //   var init = Hierarchy_GUI.Initialize();
                //  Hierarchy_GUI.Undo(A, "Change PlayMode Data Keeper");
                //  if (index >= ARRAY.Count) Hierarchy_GUI.Instance(A).DataKeeper_AddScript(script);
                //   else Hierarchy_GUI.Instance(A).DataKeeper_InsertScript(index, script);
                HierarchyCommonData.Instance().SetUndo( "Apply PlayModeKeeper Settings" );


                var v1 = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(script));
                if ( index >= ARRAY.Count ) ARRAY.Add( v1 );
                else ARRAY.Insert( index, v1 );

                // Hierarchy_GUI.SetDirtyObject(A);
                HierarchyCommonData.Instance().PlayModeSaverPreservedScripts_GET = ARRAY;
                A.RESET_DRAWSTACK( 0 );
                A.RepaintWindowInUpdate( 0 );

                HierarchyCommonData.Instance().SetDirty();

                // Hierarchy.RepaintAllView();
            }
        }
    }
















}
