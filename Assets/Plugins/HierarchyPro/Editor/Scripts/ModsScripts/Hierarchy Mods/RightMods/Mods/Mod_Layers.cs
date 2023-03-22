using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace EMX.HierarchyPlugin.Editor.Mods
{



	class Mod_Layers : RightModBaseClass
    {
        public Mod_Layers( int restWidth, int sib, bool enable, PluginInstance adapter ) : base( restWidth, sib, enable, adapter ) { }
        internal override void Subscribe( EditorSubscriber sbs ) { }
		internal override bool USE_CONTENT_SHRINKING() { return true; }

		private Dictionary<int, string> layers {
            get {
                Dictionary<int, string> result = new Dictionary<int, string>();
                for ( int i = 0; i < 32; i++ )
                {
                    if ( string.IsNullOrEmpty( LayerMask.LayerToName( i ) ) ) continue;
                    result.Add( i, LayerMask.LayerToName( i ) );
                }

                return result;
            }
        }


        void SetLayer( GameObject o, string layer )
        {
            if ( o && adapter.ha.SELECTED_GAMEOBJECTS().All( selO => selO.go != o ) )
            {
                Undo.RecordObject( o, "Change Layer" );
                o.layer = LayerMask.NameToLayer( layer );
                adapter.SetDirty( o );
                adapter.MarkSceneDirty( o.scene );
                if ( adapter.par_e.ENABLE_OBJECTS_PING ) Tools.TRY_PING_OBJECT( o );
            }
            else
            {
                foreach ( var objectToUndo in adapter.ha.SELECTED_GAMEOBJECTS() )
                {
                    Undo.RecordObject( objectToUndo.go, "Change Layer" );
                    objectToUndo.go.layer = LayerMask.NameToLayer( layer );
                    adapter.SetDirty( objectToUndo.go );
                    adapter.MarkSceneDirty( objectToUndo.scene );
                    //  if (Hierarchy.par.ENABLE_PING_Fix) Tools.TRY_PING_OBJECT(objectToUndo);
                }
            }

            ResetStack();
        }


        internal override void ModuleCommonGenericMenu( GenericMenu menu, GameObject activeGo, object _c, string sub = "" )
        {
            var l = layers;
            // var select = -1;

            var ordered = l.OrderBy(f => f.Key).Select(f => f.Value).ToArray();
            Action<int> Callback = (res) => { SetLayer(activeGo, ordered[res]); };


            if ( activeGo || adapter.ha.SELECTED_GAMEOBJECTS().Length != 0 )
            {
                var content =  _c as GUIContent;
                if ( content == null && Selection.activeObject ) content = new GUIContent();
                if ( content != null )
                {
                    var oldSelect = content.text;
                    for ( int i = 0; i < ordered.Length; i++ )
                    {
                        var ind = i;
                        content.text = sub + ordered[ i ];
                        // menu.AddItem(new GUIContent(content), GET_STRING(ordered[i], adapter.par.UPPER_LAYERS) == oldSelect, () => Callback(ind));
                        menu.AddItem( new GUIContent( content ), ordered[ i ] == oldSelect, () => Callback( ind ) );
                    }
                }

            }



            menu.AddSeparator( sub );

            /*    menu.AddItem(new GUIContent("Show 'Tags And Layers' Settings"), false, () => {
                    Selection.objects = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
                });
                menu.AddSeparator("");*/
            var pos = new MousePos(EVENT.mousePosition, MousePos.Type.Input_128_68, !callFromExternal(), adapter);
            // var pos = InputData.WidnwoRect(!callFromExternal(), EVENT.mousePosition, 128, 68, adapter );
            var w = adapter.window;
            if ( !activeGo && adapter.ha.SELECTED_GAMEOBJECTS().Length == 0 ) menu.AddDisabledItem( new GUIContent( sub + "+ Assign a New Layer" ) );
            else menu.AddItem( new GUIContent( sub + "+ Assign a New Layer" ), false, () => {
                Action<string> act = (str) =>
                    {
                        if (string.IsNullOrEmpty(str)) return;
                        str = str.Trim();
                        var lowwer = ordered.Select(ord => ord.ToLower()).ToList();
                        var ind = lowwer.IndexOf(str.ToLower());
                        if (ind != -1)
                        {
                            SetLayer(activeGo, ordered[ind]);
                            /*Undo.RecordObject(o, "Change Layer");
                            o.layer = LayerMask.NameToLayer(ordered[ind]);
                            Hierarchy.SetDirty(o);
                            if (Hierarchy.par.ENABLE_PING_Fix) Tools.TRY_PING_OBJECT(o);*/
                        }
                        else
                        {
                            var loadAss = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
                            if (loadAss == null || loadAss.Length == 0)
                            {
                                EditorUtility.DisplayDialog("Error", "Error load layers", "Ok");
                                return;
                            }

                            var tagManager = new SerializedObject(loadAss[0]);
                            var findProperty = tagManager.FindProperty("layers");
                            if (findProperty == null)
                            {
                                EditorUtility.DisplayDialog("Error", "Error load layers", "Ok");
                                return;
                            }

                            for (int j = 8; j < findProperty.arraySize; j++)
                            {
                                SerializedProperty layerSP = findProperty.GetArrayElementAtIndex(j);
                                if (layerSP.stringValue == "")
                                {
                                    layerSP.stringValue = str;
                                    tagManager.ApplyModifiedProperties();
                                    break;
                                }

                                if (j == findProperty.arraySize - 1)
                                {
                                    EditorUtility.DisplayDialog("Error", "No free slots", "Ok");
                                    return;
                                }
                            }

                            // if (Selection.gameObjects == null || Selection.gameObjects.All(selO => selO != o))
                            //{
                            SetLayer(activeGo, str);
                            /* Undo.RecordObject(o, "Change Layer");
                             o.layer = LayerMask.NameToLayer(str);
                             Hierarchy.SetDirty(o);
                             if (Hierarchy.par.ENABLE_PING_Fix) Tools.TRY_PING_OBJECT(o);*/
                            /*   }
                               else
                               {
                            
                               }*/
                        }
                    };

                Windows.InputWindow.Init( pos, "New layer name's", w, act );
                //EditorUtility.DisplayDialogComplex(
            } );
            /*   menu.AddSeparator("");
               menu.AddItem(new GUIContent("SubMenu/MenuItem3"), false, Callback, "item 3");*/

            menu.AddSeparator( sub );
            menu.AddItem( new GUIContent( sub + "Show only uppercase letters" ), adapter.par_e.RIGHT_LAYERS_UPPERCASE != 0, () => {
                adapter.par_e.RIGHT_LAYERS_UPPERCASE = 1 - adapter.par_e.RIGHT_LAYERS_UPPERCASE;
                // adapter.SavePrefs();
            } );
            menu.AddSeparator( sub );

            menu.AddItem( new GUIContent( sub + "Show 'Tags And Layers' Settings" ), false, () => {
                Selection.objects = AssetDatabase.LoadAllAssetsAtPath( "ProjectSettings/TagManager.asset" );
                Tools.FocusToInspector();
            } );

        }

        //static Color alpha = new Color(1, 1, 1, 0.3f);
        static GUIContent content = new GUIContent();

        public override void Draw()
        {
            if ( !START_DRAW( drawRect, adapter.o ) ) return;




			// var o = _o.go;

			// if (drawRect.Contains(EVENT.mousePosition))
			content.text = content.tooltip = LayerMask.LayerToName( adapter.o.go.layer );
            // content.tooltip = base.ContextHelper;
            MT.GET_STRING( content, callFromExternal() ? 0 : adapter.par_e.RIGHT_LAYERS_UPPERCASE );
            if ( content.text == "" ) content.text = "...Missing";
            bool hasContent = false;
            var style = !callFromExternal() ? RightModsStyles.STYLE_LABEL_8_right : RightModsStyles.STYLE_LABEL_8_WINDOWS_right;
            Rect r;
            if ( LayerMask.LayerToName( adapter.o.go.layer ) != "Default" )
            { /*var fs = Adapter.GET_SKIN().label.fontSize;
                var al = Adapter.GET_SKIN().label.alignment;
                Adapter.GET_SKIN().label.alignment = TextAnchor.MiddleLeft;
                if () Adapter.GET_SKIN().label.fontSize = adapter.FONT_8();
                else Adapter.GET_SKIN().label.fontSize = adapter.WINDOW_FONT_8();*/

                // Adapter.GET_SKIN().label.fontSize = Hierarchy.FONT_8();

                /*   GUI.enabled = o.activeInHierarchy;
                   GUI.Label( drawRect, content, !callFromExternal()  ? adapter.STYLE_LABEL_8 : adapter.STYLE_LABEL_8_WINDOWS );
                   GUI.enabled = true;*/
                hasContent = true;
                r = adapter.modsController.rightModsManager.DrawCursorRect( ref drawRect, style, content );
				Draw_Label( r, content, style, true );
			}

			else
            { /* var c = GUI.color;
                 GUI.color *= alpha;
                 GUI.Label( drawRect, "-", adapter.STYLE_LABEL_8_right );
                     GUI.color = c;*/

                r = adapter.modsController.rightModsManager.DrawCursorRect( ref drawRect, style, GetEmptyConten_Common );
				Draw_Label( r, GetEmptyConten_Common, style, true );

				/* var a = Adapter.GET_SKIN().label.alignment;
                 Adapter.GET_SKIN().label.alignment = __;*/
				/*  Adapter.GET_SKIN().label.alignment = a;*/
				//  GUI.Label(drawRect, "-");
			}

			// var bg = Adapter.GET_SKIN().button.active.background;
			// Adapter.GET_SKIN().button.active.background = Hierarchy.GetIcon("BUT");


			Draw_ModuleButton( r, content, BUTTON_ACTION_HASH, hasContent, drawPointer: true );
            /*   if (adapter.ModuleButton( drawRect, null, hasContent ))
               {
            
            
               }*/
            /* if (drawRect.Contains(EVENT.mousePosition))
             {*/

            //Adapter.GET_SKIN().button.active.background = bg;


            /* var res = EditorGUI.Popup(drawRect, select, ordered);
             if (GUI.changed && res != -1)
             {
            
             }
             //}*/


			END_DRAW( adapter.o , savedData.temp_i);
			//if (adapter.EVENT.type == EventType.Layout) 
             if (savedData.temp_i != -1)   adapter.o.lastContentRectLayout[ savedData.temp_i ].SET(ref r);
		}


		bool Validate( GameObject o )
        {
            return LayerMask.LayerToName( o.layer ) != "Default";
        }

        bool Validate( HierarchyObject o )
        {
            return LayerMask.LayerToName( o.go.layer ) != "Default";
        }

        internal override Windows.SearchWindow.FillterData_Inputs CallHeader()
        {
            var result = new Windows.SearchWindow.FillterData_Inputs(callFromExternal_objects)
            {
                Valudator = Validate,
                SelectCompareString = (d, i) => LayerMask.LayerToName(d.go.layer),
                SelectCompareCostInt = (d, i) =>
                {
                    var cost = i;
                    cost += d.go.activeInHierarchy ? 0 : 100000000;
                    return cost;
                }
            };
            return result;
            /* obs = .Where(Validate).ToArray();
             contentCost = obs
                .Select((d, i) => new { name = LayerMask.LayerToName(d.layer), startIndex = i, obj = d })
                .OrderBy(d => d.name)
                 .Select((d, i) => {
                     var cost = i;
                     cost += d.obj.activeInHierarchy ? 0 : 100000000;
                     return new { d.startIndex, cost = cost };
                 })
                .OrderBy(d => d.startIndex)
                .Select(d => d.cost).ToArray();
             return true;*/
        }

        internal Windows.SearchWindow.FillterData_Inputs CallHeaderFiltered( string filter )
        {
            var result = CallHeader();
            result.Valudator = s => Validate( s ) && LayerMask.LayerToName( s.go.layer ) == filter;
            return result;
            /* obs = Utilities.AllSceneObjects().Where(s => Validate(s) && LayerMask.LayerToName(s.layer) == filter).ToArray();
             contentCost = obs
                .Select((d, i) => new { name = LayerMask.LayerToName(d.layer), startIndex = i, obj = d })
                .OrderBy(d => d.name)
                 .Select((d, i) => {
                     var cost = i;
                     cost += d.obj.activeInHierarchy ? 0 : 100000000;
                     return new { d.startIndex, cost = cost };
                 })
                .OrderBy(d => d.startIndex)
                .Select(d => d.cost).ToArray();*/
        }




        DrawStackMethodsWrapper __BUTTON_ACTION_HASH = null;

        DrawStackMethodsWrapper BUTTON_ACTION_HASH {
            get { return __BUTTON_ACTION_HASH ?? (__BUTTON_ACTION_HASH = new DrawStackMethodsWrapper( BUTTON_ACTION )); }
        }

        void BUTTON_ACTION( Rect worldOffset, Rect inputRect, DrawStackMethodsWrapperData data, HierarchyObject _o )
        {
            var o = _o.go;
            var content = data.content;
            if ( EVENT.button == adapter.MOUSE_BUTTON_0 )
            {
                GenericMenu menu = new GenericMenu();
                ModuleCommonGenericMenu( menu, o, content );

                menu.ShowAsContext();
                Tools.EventUse();
            }


            if ( EVENT.button == adapter.MOUSE_BUTTON_1 )
            {
                Tools.EventUse();
                /*int[] contentCost = new int[0];
                GameObject[] obs = new GameObject[0];*/
                var mp = new MousePos(EVENT.mousePosition, MousePos.Type.Search_356_0, !callFromExternal(), adapter);
                Windows.SearchWindow.Init( mp, SearchHelper, LayerMask.LayerToName( o.layer ),
                    Validate( o ) ? CallHeaderFiltered( LayerMask.LayerToName( o.layer ) ) : CallHeader(),
                    this, adapter.window, _o );
                // EditorGUIUtility.ic
            }
        }
    }




}


