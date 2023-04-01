using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace EMX.HierarchyPlugin.Editor.Mods
{

	class Mod_PrefabApply : RightModBaseClass
    {
        public Mod_PrefabApply(int restWidth, int sib, bool enable, PluginInstance adapter) : base(restWidth, sib, enable, adapter) { }

        GUIContent content = new GUIContent();
        internal override void Subscribe(EditorSubscriber sbs) { }

        internal override void ModuleCommonGenericMenu( GenericMenu menu, GameObject o, object data, string sub )
        {
        }

        public override void Draw()
        {
            if (!START_DRAW(drawRect, adapter.o)) return ;

            var o = adapter.o.go;
            var prefab_root = Prefabs.FindPrefabRoot(o);
            if (!prefab_root)
            {
                END_DRAW(adapter.o, savedData.temp_i);
                return ;
            }

            var prefab_src = Prefabs.GetCorrespondingObjectFromSource(prefab_root);

            var oldRect = drawRect;

            if (prefab_src != null)
            {
                var oldW = drawRect.width;
                var oldH = drawRect.height;
                if (o != prefab_root)
                    drawRect.width = drawRect.height = 6;
                else
                    drawRect.width = drawRect.height = 12;
                drawRect.x += (oldW - drawRect.width) / 2;
                drawRect.y += (oldH - drawRect.width) / 2;

                var c = Color.white;
                //  if ( o != prefab_root ) c.a *= 0.5f;
                // Adapter.DrawTexture(drawRect, adapter.GetIcon("PREF"));
                Draw_GUITexture(drawRect, adapter.GetNewIcon(NewIconTexture.RightMods , "PREF"), c, USE_GO: true);

                //**// if (o != prefab_root)
                //**//  {
                /* var col = Hierarchy.LINE;
                               col.r = col.g = col.b = 0.2f;
                               col.a = .6f;
                               Hierarchy.colorText11.SetPixel(0, 0, col);
                               Hierarchy.colorText11.Apply();
                               GUI.DrawTexture(drawRect, Hierarchy.colorText11, ScaleMode.ScaleAndCrop, true, 1); */

                //**// Adapter.FadeRect(drawRect, 0.5f);
                //**// }

                //**// if ( !o.activeInHierarchy) Adapter.FadeRect(drawRect, 0.5f);

                if (oldRect.Contains(EVENT.mousePosition))
                    content.tooltip = "'" + prefab_src.name + "' prefab\n(Click - Select prefab file in project)\n(Ctrl+Click - Apply changes to Prefab)";
            }
            else
            {
                if (oldRect.Contains(EVENT.mousePosition))
                    content.tooltip = "";
            }


            // drawRect.y -= 2;
            /*if (adapter.ModuleButton(oldRect, content, true))
            {
            
            }*/
            str.prefab_root = prefab_root;
            str.prefab_src = prefab_src;
            Draw_ModuleButton(oldRect, content, BUTTON_ACTION_HASH, true, str, true, drawPointer: true);


            END_DRAW(adapter.o, savedData.temp_i);

        }

        prefab_str str;

        bool Validate(GameObject o)
        {
            if (!o) return false;
            var prefab_root = Prefabs.FindPrefabRoot(o);
            var prefab_src = Prefabs.GetCorrespondingObjectFromSource(prefab_root);
            return prefab_src != null;
        }

        bool ValidateObyTop(GameObject o)
        {
            if (!o) return false;
            var prefab_root = Prefabs.FindPrefabRoot(o);
            var prefab_src = Prefabs.GetCorrespondingObjectFromSource(prefab_root);
            return prefab_src != null && o == prefab_root;
        }

        bool Validate(GameObject o, UnityEngine.Object prefabsrc)
        {
            if (!o) return false;
            var prefab_root = Prefabs.FindPrefabRoot(o);
            var prefab_src = Prefabs.GetCorrespondingObjectFromSource(prefab_root);
            return prefab_src != null && prefab_src == prefabsrc && o == prefab_root;
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
                Valudator = (oo) => ValidateObyTop(oo.go),
                SelectCompareString = (d, i) => "",
                SelectCompareCostInt = (d, i) =>
                {
                    var cost = i;
                    cost += d.go.activeInHierarchy ? 0 : 100000000;
                    return cost;
                }
            };
            return result;
        }

        internal Windows.SearchWindow.FillterData_Inputs CallHeaderFiltered(UnityEngine.Object prefabsrc)
        {
            var result = CallHeader();
            result.Valudator = s => Validate(s.go, prefabsrc);
            result.SelectCompareString = (d, i) => i.ToString();
            return result;
        }

        /** CALL HEADER */
        /*
                    internal override bool CallHeader(out GameObject[] obs, out int[] contentCost)
                    {
                        obs = Utilities.AllSceneObjects().Where(ValidateObyTop).ToArray();
                        contentCost = obs.Select(o => 0).ToArray();
                        return true;
                    }
        
                    void CallHeaderFiltered(out GameObject[] obs, out int[] contentCost, UnityEngine.Object prefabsrc)
                    {
                        obs = Utilities.AllSceneObjects().Where(s => Validate(s, prefabsrc)).ToArray();
                        contentCost = obs.Select(o => o.activeInHierarchy ? 0 : 1).ToArray();
                    }*/
        internal struct prefab_str
        {
            internal UnityEngine.Object prefab_src;
            internal GameObject prefab_root;
        }

        DrawStackMethodsWrapper __BUTTON_ACTION_HASH = null;

        DrawStackMethodsWrapper BUTTON_ACTION_HASH
        {
            get { return __BUTTON_ACTION_HASH ?? (__BUTTON_ACTION_HASH = new DrawStackMethodsWrapper(BUTTON_ACTION)); }
        }

        void BUTTON_ACTION(Rect worldOffset, Rect inputRect, DrawStackMethodsWrapperData data, HierarchyObject _o)
        {
#pragma warning disable
            var content = data.content;
#pragma warning restore
            var o = _o.go;
            var args = (prefab_str) data.args;
            var prefab_src = args.prefab_src;
            var prefab_root = args.prefab_root;
            if (EVENT.button == adapter.MOUSE_BUTTON_0 && !Application.isPlaying)
            {
                if (!EVENT.control)
                {
                    Tools.EventUse();
                    Selection.objects = new[] {prefab_src};
                }
                else
                {
                    if (adapter.ha.SELECTED_GAMEOBJECTS().All(selO => selO.go != o))
                    {
                        Prefabs.ReplacePrefab(prefab_root, prefab_src);
                        // adapter.logProxy.Log( "Updated prefab : " + AssetDatabase.GetAssetPath( prefab_src ) );
                        if (adapter.par_e.ENABLE_OBJECTS_PING) Tools.TRY_PING_OBJECT(o);
                    }
                    else
                    {
                        Dictionary<GameObject, UnityEngine.Object> result = new Dictionary<GameObject, UnityEngine.Object>();
                        bool error = false;
                        foreach (var selob in adapter.ha.SELECTED_GAMEOBJECTS())
                        {
                            if (!Validate(selob.go)) continue;
                            var p = Prefabs.FindPrefabRoot(selob.go);
                            var src = Prefabs.GetCorrespondingObjectFromSource(p);
                            if (result.ContainsKey(p)) continue;
                            if (result.Values.Any(v => v == src))
                            {
                                var first = result.First(v => v.Value == src);
                                LogProxy.LogWarning("Two or more selected objects refer to the same prefab. \n" +
                                                            "- '" + (first.Key.transform.parent == null ? "" : first.Key.transform.parent.name + "/") + first.Key.name + "'"
                                                            + " \n" +
                                                            "- '" + (p.transform.parent == null ? "" : p.transform.parent.name + "/") + p.name + "'"
                                );
                                error = true;
                            }

                            result.Add(p, src);
                        }

                        if (!error)
                        {
                            foreach (var kp in result)
                            {
                                Prefabs.ReplacePrefab(kp.Key, kp.Value);
                                //adapter.logProxy.Log( "Updated prefab : " + AssetDatabase.GetAssetPath( kp.Value ) );
                            }
                        }
                    }
                }
            }

            if (EVENT.button == adapter.MOUSE_BUTTON_1)
            {
                Tools.EventUse();
                /*  int[] contentCost = new int[0];
                  GameObject[] obs = new GameObject[0];*/
                var mp = new MousePos(EVENT.mousePosition, MousePos.Type.Search_356_0, !callFromExternal(), adapter);

                if (Validate(o)) // if (EditorSceneManager.GetActiveScene().rootCount != 0) CallHeaderFiltered(out obs, out contentCost, prefab_src);
                {
                    // FillterData.Init(EVENT.mousePosition, SearchHelper + ": '" + prefab_src.name + "'", "'" + prefab_src.name + "'", obs, contentCost, null, this);

                    Windows.SearchWindow.Init(mp, SearchHelper + ": '" + prefab_src.name + "'", "'" + prefab_src.name + "'",
                        CallHeaderFiltered(prefab_src),
                        this, adapter.window, _o);
                }
                else //  if (EditorSceneManager.GetActiveScene().rootCount != 0) CallHeader(out obs, out contentCost);
                {
                    //   FillterData.Init(EVENT.mousePosition, SearchHelper + ": 'All'", "'All'", obs, contentCost, null, this);

                    Windows.SearchWindow.Init(mp, SearchHelper + ": 'All'", "'All'",
                        CallHeader(),
                        this, adapter.window, _o);
                }


                // EditorGUIUtility.ic
            }
        }
    }
}
