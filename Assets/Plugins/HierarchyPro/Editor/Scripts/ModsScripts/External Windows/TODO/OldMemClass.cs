
#if asd


namespace EMX.Hierarchy.Editor.Mods
{

	class OldMemClass
    {




        internal int MemCacheScene
        {
            get { return LastActiveScene.GetHashCode(); }

            set { }
        }

        internal static void RefreshMemCache(int scene)
        {
            /*if (adapter.DISABLE_DES()) return;

            // LastActiveScene = scene;
            if (adapter.IS_PROJECT()) scene = -1;

            cacheInit = true;

            var __LENGTH = 1;
            adapter.bottomInterface.GET_BOOKMARKS(ref list, scene);


            if ((adapter.IS_PROJECT() || Adapter.GET_SCENE_BY_ID(scene).isLoaded) && adapter.MOI.des(scene) != null)
            {
                __LENGTH = list.Count;
            }

            MemCacheScene = scene;

            if (m_memCache.Count == 0 || __LENGTH != m_memCache[MemType.Custom].Length || m_memCache[MemType.Custom].Length == 0)
            {
                //print("CLEAR");
                if (MemTypes == null) MemTypes = (MemType[])Enum.GetValues(typeof(MemType));

                foreach (var key in MemTypes)
                {
                    if (key == MemType.other || key != MemType.Custom && m_memCache.ContainsKey(key)) continue;

                    var LENGTH = key == MemType.Custom ? __LENGTH : 1;

                    if (!m_memCache.ContainsKey(key))
                    {
                        m_memCache.Add(key, new List<MemoryRoot>[LENGTH]);
                        m_memPosition.Add(key, new Rect[LENGTH][]); //adapter.MAX20
                    }

                    else
                    {
                        m_memCache[key] = new List<MemoryRoot>[LENGTH];
                        m_memPosition[key] = new Rect[LENGTH][];
                    }

                    for (int length_index = 0; length_index < LENGTH; length_index++)
                    {
                        m_memCache[key][length_index] = new List<MemoryRoot>();
                        m_memPosition[key][length_index] = new Rect[adapter.MAX20];

                        for (int i = 0; i < adapter.MAX20; i++)
                        {
                            switch (key)
                            {
                                case MemType.Scenes:
                                    m_memCache[key][length_index].Add(new SceneMemory(this));
                                    break;

                                case MemType.Hier:
                                    m_memCache[key][length_index].Add(new HierarchyMemory(this));
                                    break;

                                default:
                                    m_memCache[key][length_index].Add(new GameObjectMemory(this));
                                    break;
                            }

                        }
                    }
                }

                ClearAction();
            } // new

        
            if (adapter.IS_HIERARCHY() && !Adapter.GET_SCENE_BY_ID(scene).isLoaded) return;

            lastDes = adapter.MOI.des(scene);

            if (lastDes == null) return;

            if (lastDes.GetHash3() == null) lastDes.SetHash3(new List<Int32List>());

            if (lastDes.GetHash4() == null) lastDes.SetHash4(new List<Int32List>());

            var last_hierCache = lastDes.HierarchyCache();
            var last_selection = lastDes.GetHash3();
            var last_custom = lastDes.GetHash4();
            var last_bookmarks = list;
            var last_scenes = Hierarchy_GUI.GetLastScenes(adapter);



            for (int length_count = 0, L = m_memCache[MemType.Custom].Length; length_count < L; length_count++)
            {
                if (length_count == 0)
                    for (int i = 0; i < adapter.MAX20; i++)
                    {
                        if (i < last_custom.Count) CheckUniqueFavoritID(ref last_custom[i].InstanceID);

                        m_memCache[MemType.Custom][length_count][i].SetIntValues(i < last_custom.Count ? last_custom[i] : null, i);
                    }

                else
                    for (int i = 0; i < adapter.MAX20; i++)
                    {
                        if (i < last_bookmarks[length_count].array.Count) CheckUniqueFavoritID(ref last_bookmarks[length_count].array[i].InstanceID);

                        m_memCache[MemType.Custom][length_count][i].SetIntValues(i < last_bookmarks[length_count].array.Count ? last_bookmarks[length_count].array[i] : null, i);
                    }
            }


            for (int i = 0; i < adapter.MAX20; i++)
                m_memCache[MemType.Last][0][i].SetIntValues(i < last_selection.Count ? last_selection[i] : null, i);

            for (int i = 0; i < adapter.MAX20; i++)
                m_memCache[MemType.Hier][0][i].SetObjectValues(i < last_hierCache.Count ? last_hierCache[i] : null, i);

            for (int i = 0; i < adapter.MAX20; i++)
            {
                m_memCache[MemType.Scenes][0][i].SetStringValues(i < last_scenes.Count ? last_scenes[i] : null, i);
            }

            if (adapter.ENABLE_BOTTOMDOCK_PROPERTY && adapter.bottomInterface.RowsParams.Length > 2 && adapter.bottomInterface.RowsParams[1].Enable)
                //if (!adapter.hashoveredItem || adapter.pluginID != 0)
                adapter.RepaintWindowInUpdate();*/
        }

        private int IDOFFSET(MemType type)
        {
            switch (type)
            {
                case MemType.Last:
                    return 1000;

                case MemType.Custom:
                    return 2000;

                case MemType.Scenes:
                    return 3000;

                case MemType.Hier:
                    return 4000;

                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        /*   private  GUIContent GET_CONTENT(MemType type)
           {
               switch (type) {
               case MemType.Last:
                   tooltipLast.text = "";
                   return tooltipLast;
               case MemType.Custom:
                   tooltipCustom.text = "";
                   return tooltipCustom;
               case MemType.Scenes:
                   tooltipScenes.text = "";
                   return tooltipScenes;
               default:
                   throw new ArgumentOutOfRangeException("type", type, null);
               }
           }*/



        void UniversalAddAndRefresh<T>(ref List<T> list, T newObject, int scene)
        {
            if (Application.isPlaying || newObject == null) return;

            // var scene = EditorSceneManager.GetActiveScene();
            adapter.SET_UNDO(adapter.MOI.des(scene), "+ Add Expanded Objects");


            if (list.Count == 0) list.Add(newObject);
            else list.Insert(0, newObject);

            while (list.Count > adapter.MAX20) list.RemoveAt(adapter.MAX20);

            adapter.SetDirtyActiveDescription(scene);
            RefreshMemCache(scene);
            ClearAction();
        }

   

        private void AddAndRefreshCustom(UnityEngine.Object[] o, UnityEngine.Object ActiveGameObject, int CustomIndex, int scene)
        {
            if (Application.isPlaying || o.Length == 0) return;

            List<Int32List> targetList = CustomIndex == 0 ? adapter.MOI.des(scene).GetHash4() : adapter.MOI.des(scene).GetBookMarks()[CustomIndex].array;
            AddAndRefreshCustom(o, ActiveGameObject, targetList, scene);
        }

        private void AddAndRefreshCustom(UnityEngine.Object[] o, UnityEngine.Object ActiveGameObject, List<Int32List> targetList, int scene)
        {
            if (Application.isPlaying || o.Length == 0) return;


            // var scene = EditorSceneManager.GetActiveScene();

            if (adapter.IS_HIERARCHY() && LastActiveScene.GetHashCode() != scene)
            {
                LastActiveScene = (scene);
            }

            // adapter.CreateUndoActiveDescription( "Move Favorite", scene );


            adapter.SET_UNDO(adapter.MOI.des(scene), "Add Custom Selection Button");


            var findIndex = targetList.FindIndex(l => BroadCastCompate(l, o));

            if (findIndex != -1) targetList.RemoveAt(findIndex);


            var target = new Int32List() { };

            if (adapter.IS_HIERARCHY()) SET_INT32_AS_HIERARCHY(target, o.Select(d => d.GetInstanceID()).ToArray(), ActiveGameObject.GetInstanceID());
            else SET_INT32_AS_PROJECT(target, o.Select(d => d.GetInstanceID()).ToArray(), ActiveGameObject.GetInstanceID());


            if (targetList.Count == 0) targetList.Add(target);
            else targetList.Insert(0, target);

            while (targetList.Count > adapter.MAX20) targetList.RemoveAt(adapter.MAX20);

            adapter.SetDirtyDescription(adapter.MOI.des(scene), scene);
            adapter.MarkSceneDirty(scene);

            RefreshMemCache(scene);

            ClearAction();

            foreach (var w in adapter.bottomInterface.WindowController)
                if (w.REFERENCE_WINDOW)
                    w.REFERENCE_WINDOW.Repaint();

            foreach (var w in adapter.bottomInterface.FavoritControllers)
                if (w.REFERENCE_WINDOW)
                    w.REFERENCE_WINDOW.Repaint();
        }

        private bool BroadCastCompate(Int32List List1, UnityEngine.Object[] List2)
        {
            if (List1 == null || List2 == null) return false;

            if (INT32_ISNULL(List1)) return false;

            if (INT32_COUNT(List1) != List2.Length) return false;


            var e1 = INT32_TOOBJECTASLISTCT(List1).Select(l => !l ? -1 : l.GetInstanceID()).ToList();
            //var e1 = List1.list.Select( l => !l ? -1 : l.GetInstanceID() ).ToList();
            e1.Sort();

            var e2 = List2.Select(l => !l ? -1 : l.GetInstanceID()).ToList();
            e2.Sort();
            return e1.SequenceEqual(e2);
        }


        internal void RemoveAndRefresh(MemType type, int ArrayIndex, int CustomIndex, int scene) // var scene = EditorSceneManager.GetActiveScene();
        {
            RefreshMemCache(scene);

            switch (type)
            {
                case MemType.Last:
                    adapter.MOI.des(scene).GetHash3().RemoveAt(ArrayIndex);
                    adapter.SetDirtyDescription(adapter.MOI.des(scene), scene);
                    /*   Hierarchy.SetDirty(M_Descript.des(scene).component);
                       Hierarchy.SetDirty(M_Descript.des(scene).gameObject);*/
                    break;
                case MemType.Custom:
                    adapter.SET_UNDO(adapter.MOI.des(scene), "Remove Custom Selection Button");
                    List<Int32List> targetList = CustomIndex == 0 ? adapter.MOI.des(scene).GetHash4() : adapter.MOI.des(scene).GetBookMarks()[CustomIndex].array;
                    // adapter.MOI.des(scene).GetHash4().RemoveAt(ArrayIndex);
                    targetList.RemoveAt(ArrayIndex);
                    /*   Hierarchy.SetDirty(M_Descript.des(scene).component);
                       Hierarchy.SetDirty(M_Descript.des(scene).gameObject);*/
                    adapter.SetDirtyDescription(adapter.MOI.des(scene), scene);
                    adapter.MarkSceneDirty(scene);
                    break;
                case MemType.Hier:
                    adapter.CreateUndoActiveDescription("Remove Hierarchy", scene);
                    Undo.RecordObject(Hierarchy_GUI.Instance(adapter), "Remove Hierarchy");
                    adapter.MOI.des(scene).HierarchyCache().RemoveAt(ArrayIndex);
                    adapter.SetDirtyActiveDescription(scene);
                    break;
                case MemType.Scenes:
                    Undo.RecordObject(Hierarchy_GUI.Instance(adapter), "Remove Previous Scene Button");
                    Hierarchy_GUI.GetLastScenes(adapter).RemoveAt(ArrayIndex);
                    Hierarchy_GUI.SetDirtyObject(adapter);
                    /*MonoBehaviour.print("SetDown");*/
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }

            RefreshMemCache(scene);

            ClearAction();

            foreach (var w in adapter.bottomInterface.WindowController)
                if (w.REFERENCE_WINDOW)
                    w.REFERENCE_WINDOW.Repaint();

            foreach (var w in adapter.bottomInterface.FavoritControllers)
                if (w.REFERENCE_WINDOW)
                    w.REFERENCE_WINDOW.Repaint();
        }


        internal enum MemType
        {
            Last,
            Custom,
            Scenes,
            Hier,
            other
        }





      static  internal GameObject[] CREATE_EXPAND_GO_SNAPSHOT(int scene)
        {
            var treeView = adapter.m_TreeView(adapter.window());
            var state = adapter.m_state.GetValue(treeView, null);
            var result = (List<int>)adapter.m_expandedIDs.GetValue(state, null);

            return result.Select(id => EditorUtility.InstanceIDToObject(id) as GameObject).Where(o => o && o.scene.GetHashCode() == scene).ToArray(); // *** //
        }

        static internal void SET_EXPAND_GO_SNAPSHOT(GameObject[] ids, string[] GUIDids, string[] PATHids, int scene)
        {
            var treeView = adapter.m_TreeView(adapter.window());
            var state = adapter.m_state.GetValue(treeView, null);
            var result = new List<int>();

            if (adapter.IS_HIERARCHY())
                result.AddRange(((List<int>)adapter.m_expandedIDs.GetValue(state, null))
                    .Where(id => !(EditorUtility.InstanceIDToObject(id) as GameObject) || ((GameObject)EditorUtility.InstanceIDToObject(id)).scene.GetHashCode() != scene));

            if (ids != null && ids.Length != 0)
            {
                result.AddRange(ids.Where(o => o).Select(go => go.GetInstanceID())); // *** //
            }

            if (GUIDids != null && GUIDids.Length != 0)
            {
                if (GUIDids.Length != PATHids.Length) PATHids = new string[GUIDids.Length];

                for (int i = 0; i < GUIDids.Length; i++)
                {
                    var ts = GUIDids[i];
                    var getted = adapter.GetHierarchyObjectByGUID(ref ts, PATHids[i]);

                    if (ts != GUIDids[i])
                    {
                        GUIDids[i] = ts;
                        adapter.ON_GUID_BACKCHANGED();
                    }

                    result.Add(getted.id);
                }

                // result.AddRange( GUIDids.Select( adapter.GetHierarchyObjectByGUID ).Where( o => o != null ).Select( p => p.id ) ); // *** //
            } // GUIDids != null && GUIDids.Length != 0

            var data = adapter.m_data.GetValue(treeView, null);
            adapter.m_SetExpandedIDs.Invoke(data, new[] { result.ToArray() });
        }


        static internal string[] CREATE_EXPAND_GO_SNAPSHOT_FORPROJECT()
        {
            var treeView = adapter.m_TreeView(adapter.window());
            var state = adapter.m_state.GetValue(treeView, null);
            var result = (List<int>)adapter.m_expandedIDs.GetValue(state, null);

            return result.Select(id => AssetDatabase.GetAssetPath(id)).Where(o => !string.IsNullOrEmpty(o)).Select(AssetDatabase.AssetPathToGUID).ToArray(); // *** //
        }

        static internal void SET_EXPAND_NULL()
        {
            var treeView = adapter.m_TreeView(adapter.window());
            var state = adapter.m_state.GetValue(treeView, null);
            var result = new List<int>();

            if (adapter.IS_HIERARCHY()) result.AddRange(((List<int>)adapter.m_expandedIDs.GetValue(state, null)).Where(id => !(EditorUtility.InstanceIDToObject(id) as GameObject)));

            var data = adapter.m_data.GetValue(treeView, null);
            adapter.m_SetExpandedIDs.Invoke(data, new[] { result.ToArray() });
        }









        /*  if (Event.current.type == EventType.MouseDown && buttonRect.Contains(Event.current.mousePosition) && Event.current.button == 1)
        {
            controller.selection_button = idOffset + 200;
            controller.selection_window = controller.REFERENCE_WINDOW;
            controller.lastRect = buttonRect;
            var captureCell = buttonRect;
            //  var captureI = i;
            controller.selection_action = (mouseUp, deltaTIme) =>
            {
                if (mouseUp && captureCell.Contains(Event.current.mousePosition))
                {
                    if (controller.GetCategoryIndex() == 0) return;
        
                    SHOW_STRING(adapter.par.BottomParams.FavoriteCategories[controller.GetCategoryIndex()].name, (value) =>
                   {
                       if (string.IsNullOrEmpty(value)) return;
                       if (adapter.par.BottomParams.FavoriteCategories.Any(b => b.name == value)) return;
                       adapter.par.BottomParams.FavoriteCategories[controller.GetCategoryIndex()].name = value;
                       adapter.SavePrefs();
                   });
        
                }
            }; // ACTION
        }*/


        static void DO_BUTTON(BottomController controller, int scene, Rect buttonRect, int idOffset, Action<BottomController, int> result)
        {
            if (Event.current.type == EventType.MouseDown && buttonRect.Contains(Event.current.mousePosition) /*&& Event.current.button == 0*/)
            {
                controller.selection_button = idOffset;
                controller.selection_window = controller.REFERENCE_WINDOW;
                controller.lastRect = buttonRect;
                adapter.RepaintWindowInUpdate();
                var captureCell = buttonRect;
                controller.selection_action = (mouseUp, deltaTIme) =>
                {
                    if (mouseUp && captureCell.Contains(Event.current.mousePosition))
                    {
                        result(controller, scene);
                    }

                    return Event.current.delta.x == 0 && Event.current.delta.x == 0;
                }; // ACTION
            }

            if (Event.current.type == EventType.Repaint && buttonRect.Contains(Event.current.mousePosition) && controller.selection_action != null && controller.selection_button == idOffset)
            {
                adapter.STYLE_DEFBUTTON_middle.Draw(buttonRect, REALEMPTY_CONTENT, false, false, false, true);
            }
        }



    }
}
#endif