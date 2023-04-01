
#if asd

namespace EMX.Hierarchy.Editor.Mods
{

    class ExternalModOldMenu
	{


/*
        static internal void CREATE_BUTTON_CUSTOM_MENU(BottomController controller, int scene)
        {
            controller.adapter.bottomInterface.GET_BOOKMARKS(ref list, scene);
            m_CREATE_BUTTON_CUSTOM_MENU(controller, scene, true);
        }

        static internal GenericMenu CREATE_BUTTON_CUSTOM_MENU(BottomController controller, int scene, bool showMenu, GenericMenu menu)
        {
            controller.adapter.bottomInterface.GET_BOOKMARKS(ref list, scene);
            return m_CREATE_BUTTON_CUSTOM_MENU(controller, scene, showMenu, menu);
        }

        void DO_BUTTON_DESCRIPTION(BottomController controller, int scene)
        {
            if (controller.IS_MAIN) adapter.par.BottomParams.SHOW_ALL_DESCRIPTIONS_INHIER = !adapter.par.BottomParams.SHOW_ALL_DESCRIPTIONS_INHIER;
            else adapter.par.BottomParams.SHOW_ALL_DESCRIPTIONS_INWIN = !adapter.par.BottomParams.SHOW_ALL_DESCRIPTIONS_INWIN;

            controller.adapter.SavePrefs();
            controller.adapter.RepaintWindowInUpdate();
        }

        static internal void DO_BUTTON_COLOR(BottomController controller, int scene) // var pos = InputData.WidnwoRect(controller.IS_MAIN, Event.current.mousePosition, 190, 68, controller.adapter );
        {
            var pos = new MousePos(Event.current.mousePosition, MousePos.Type.ColorChanger_230_0, controller.IS_MAIN, controller.adapter);
            _W__BottomWindow_ColorCategories.Init(pos, controller.adapter, scene);
        }

        static internal GenericMenu m_CREATE_BUTTON_CUSTOM_MENU(BottomController controller, int m_scene, bool showMenu, GenericMenu menu = null)
        {
            return SHOW_CATEGORY_MENU(controller, m_scene, (scene) => { return controller.GetCategoryIndex(scene); }, false, showMenu, _menu: menu);
        }*/

      

        static internal GenericMenu SHOW_CATEGORY_MENU(ExternalDrawContainer controller, int scene, Func<int, int> CAT_INDEX, bool disableSwither, bool showMenu = true,
            GenericMenu _menu = null) //  Debug.Log(curentIndex);
        {
            var menu = _menu ?? new GenericMenu();
            var VAR_CAT_INDEX = CAT_INDEX(scene);


            adapter.bottomInterface.GET_BOOKMARKS(ref list, scene);
            var capture_list = list;


            menu.AddItem(new GUIContent("Open in New Tab"), false, () => { _6__BottomWindow_BottomInterfaceWindow.ShowW(adapter, _6__BottomWindow_BottomInterfaceWindow.TYPE.CUSTOM, list[controller.GetCategoryIndex(scene)].name); });


            if (!Application.isPlaying)
            {
                menu.AddItem(new GUIContent("Background Colors"), false, () =>
                {
                    adapter.GUI_ONESHOTPUSH(() => // var pos = InputData.WidnwoRect(controller.IS_MAIN, Event.current.mousePosition, 190, 68, controller.adapter);
                    {
                        var pos = new MousePos(Event.current.mousePosition, MousePos.Type.ColorChanger_230_0, controller.IS_MAIN, controller.adapter);
                        _W__BottomWindow_ColorCategories.Init(pos, controller.adapter, scene);
                    });

                
                });
            }


            /* if (disableSwither) menu.AddDisabledItem( new GUIContent( "Show Descriptions" ) );
             else*/
            //   if (disableSwither)
            menu.AddItem(new GUIContent("Show Descriptions"), controller.IS_MAIN && adapter.par.BottomParams.SHOW_ALL_DESCRIPTIONS_INHIER || !controller.IS_MAIN
                                                              && adapter.par.BottomParams.SHOW_ALL_DESCRIPTIONS_INWIN, () => { DO_BUTTON_DESCRIPTION(controller, scene); });
            menu.AddSeparator("");

            // if ( adapter.par.SHOW_BOOKMARKS_ROWS && !adapter.par.BOTTOM_AUTOHIDE )
            {
                if (!disableSwither)
                {
                    menu.AddItem(new GUIContent("1) 'Default'"), VAR_CAT_INDEX == 0, () => { controller.SetCategoryIndex(0, scene); });
                    for (int INDEX = 1; INDEX < capture_list.Count; INDEX++)
                    {
                        var capptureI = INDEX;
                        menu.AddItem(new GUIContent((INDEX + 1) + ") '" + capture_list[INDEX].name + "'"), VAR_CAT_INDEX == INDEX, () => { controller.SetCategoryIndex(capptureI, scene); });
                    }

                    if (!Application.isPlaying)
                    {
                        menu.AddItem(new GUIContent("+ Add Category"), false, () =>
                        {
                            adapter.OneFrameActionOnUpdateAC += () => { AddFavCategory(capture_list, VAR_CAT_INDEX, scene, controller); };

                            adapter.OneFrameActionOnUpdate = true;
                        });
                    }

                    if (capture_list.Count > 1) menu.AddSeparator("");
                }
            }


            if (VAR_CAT_INDEX == 0 || Application.isPlaying) menu.AddDisabledItem(new GUIContent("Rename '" + capture_list[VAR_CAT_INDEX].name + "'"));
            else
                menu.AddItem(new GUIContent("Rename '" + capture_list[VAR_CAT_INDEX].name + "'"), false, () =>
                {
                    if (VAR_CAT_INDEX == 0) return;

                    SHOW_STRING("Rename", capture_list[VAR_CAT_INDEX].name, (value) =>
                    {
                        if (string.IsNullOrEmpty(value)) return;
                        adapter.bottomInterface.GET_BOOKMARKS(ref capture_list, scene);
                        if (capture_list.Any(b => b.name == value)) return;


                        var oldValue = capture_list[CAT_INDEX(scene)].name;
                        var newValue = value;

                        foreach (var item in Resources.FindObjectsOfTypeAll<_6__BottomWindow_BottomInterfaceWindow1>())
                        {
                            var cat = ((_6__BottomWindow_BottomInterfaceWindow.BottomControllerWindow)item.current_controller).GetCurerentCategoryName();
                            if (cat == oldValue)
                            {
                                Undo.RecordObject(item, "Rename");
                                ((_6__BottomWindow_BottomInterfaceWindow.BottomControllerWindow)item.current_controller).SetCurerentCategoryName(newValue);
                                EditorUtility.SetDirty(item);
                            }
                        }

                        adapter.CreateUndoActiveDescription("Rename", scene);

                        capture_list[VAR_CAT_INDEX].name = value;
                        adapter.MarkSceneDirty(scene);
                    }, controller);
                });

            if (!Application.isPlaying)
            {
                if (VAR_CAT_INDEX == 0 || Application.isPlaying) menu.AddDisabledItem(new GUIContent("Remove '" + capture_list[VAR_CAT_INDEX].name + "'Category"));
                else
                    menu.AddItem(new GUIContent("Remove '" + capture_list[VAR_CAT_INDEX].name + "' Category"), false, () =>
                    {
                        if (
                            EditorUtility.DisplayDialog("Remove Category?", "Are you sure?", "Yes", "Cancel"))
                        {
                            if (VAR_CAT_INDEX == 0) return;
                            if (VAR_CAT_INDEX >= capture_list.Count) return;

                            adapter.CreateUndoActiveDescription("Remove Category", scene);


                            var oldValue = capture_list[CAT_INDEX(scene)].name;
                            foreach (var item in Resources.FindObjectsOfTypeAll<_6__BottomWindow_BottomInterfaceWindow1>())
                            {
                                var cat = ((_6__BottomWindow_BottomInterfaceWindow.BottomControllerWindow)item.current_controller).GetCurerentCategoryName();
                                if (cat == oldValue)
                                {
                                    item.Close();
                                }
                            }

                            capture_list.RemoveAt(VAR_CAT_INDEX);
                            adapter.SetDirtyActiveDescription(scene);

                            adapter.bottomInterface.RefreshMemCache(scene);

                            controller.REPAINT(adapter);
                            static internalEditorUtility.RepaintAllViews();
                        }
                    });
            }

            if (showMenu)
                menu.ShowAsContext();
            return menu;
        }








        // GEGEGENENENERIRIRICICIC MEEEENNNUUU
        // GEGEGENENENERIRIRICICIC MEEEENNNUUU
        // GEGEGENENENERIRIRICICIC MEEEENNNUUU






        static internal void SET_BOOK_REF(ref GenericMenu menu)
        {
            //    CREATE_BUTTON_CUSTOM_MENU( FavoritControllers, -1, false, menu );
        }



        void SET_BOOK(object value)
        {
            GenericMenu menu = new GenericMenu();


            ADD_TO_MENU_LIST_OF_OBJECTS(MemType.Custom, ref menu);

            menu.AddSeparator("");

            menu.AddItem(GetContent(adapter.par.SHOW_BOOKMARKS_ROWS, "Bookmarks"), false, () =>
            //    menu.AddItem(new GUIContent("Enable Bookmarks Botom GUI"), adapter.par.SHOW_BOOKMARKS_ROWS, () =>
            {
                adapter.par.SHOW_BOOKMARKS_ROWS = !(bool)value;
                adapter.SavePrefs();
            });

            menu.AddSeparator("");

            CREATE_BUTTON_CUSTOM_MENU(HierarchyController, LastActiveScene.GetHashCode(), false, menu);


            menu.ShowAsContext();
        }

        static internal void SET_BOOK_2(BottomController controller, int scene)
        {
            GenericMenu menu = new GenericMenu();
            ADD_TO_MENU_LIST_OF_OBJECTS(MemType.Custom, ref menu);

            menu.AddSeparator("");

            menu.AddItem(GetContent(adapter.par.SHOW_BOOKMARKS_ROWS, "Bookmarks"), false, () =>
            {
                adapter.par.SHOW_BOOKMARKS_ROWS = !(bool)adapter.par.SHOW_BOOKMARKS_ROWS;
                adapter.SavePrefs();
            });

            menu.AddSeparator("");

            CREATE_BUTTON_CUSTOM_MENU(controller, scene, false, menu);
            menu.ShowAsContext();
        }

        void SET_LAST(object value)
        {
            GenericMenu menu = new GenericMenu();


            menu.AddItem(GetContent(adapter.par.SHOW_LAST_ROWS, "Last Selections"), false, () =>
            //   menu.AddItem(new GUIContent("Enable Last Botom GUI"), adapter.par.SHOW_LAST_ROWS, () =>
            {
                adapter.par.SHOW_LAST_ROWS = !(bool)value;
                adapter.SavePrefs();
            });

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Open in New Tab"), false, () => { _6__BottomWindow_BottomInterfaceWindow.ShowW(adapter, _6__BottomWindow_BottomInterfaceWindow.TYPE.LAST, "Last Selection"); });
            menu.AddSeparator("");


            ADD_TO_MENU_LIST_OF_OBJECTS(MemType.Last, ref menu);


            menu.ShowAsContext();
        }

        void SET_HIER(object value)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(GetContent(adapter.par.SHOW_HIERARCHYSLOTS_ROWS, "Expanded Items"), false, () =>
            //  menu.AddItem(new GUIContent("Enable HIerarchy Botom GUI"), adapter.par.SHOW_HIERARCHYSLOTS_ROWS, () =>
            {
                adapter.par.SHOW_HIERARCHYSLOTS_ROWS = !(bool)value;
                adapter.SavePrefs();
            });


            menu.AddSeparator("");


            ADD_TO_MENU_LIST_OF_OBJECTS(MemType.Hier, ref menu);

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Collapse All"), false, () => { SET_EXPAND_NULL(); });
            menu.AddSeparator("");

            menu.AddItem(new GUIContent("+ Create New State"), false, () => { DoHier_Plus(null, HierarchyController); });

            menu.ShowAsContext();
        }

        void SET_SCEN(object value)
        {
            GenericMenu menu = new GenericMenu();


            menu.AddItem(GetContent(adapter.par.SHOW_SCENES_ROWS, "Scenes"), false, () =>
            //  menu.AddItem(new GUIContent("Enable Scenes Botom GUI"), adapter.par.SHOW_SCENES_ROWS, () =>
            {
                adapter.par.SHOW_SCENES_ROWS = !(bool)value;
                adapter.SavePrefs();
            });

            menu.AddSeparator("");


            ADD_TO_MENU_LIST_OF_OBJECTS(MemType.Scenes, ref menu);


            menu.AddSeparator("");

            menu.AddItem(new GUIContent("+ Add All Opened Scenes"), false, () => { DoScenes_Plus(null); });


            menu.ShowAsContext();
        }

        static List<Int32ListArray> __list;

        struct EstimItems
        {
            public GUIContent content;
            public bool active;
            public GenericMenu.MenuFunction onClick;
        }

        void ADD_TO_MENU_LIST_OF_OBJECTS(MemType type, ref GenericMenu menu)
        {
            bool was = false;
            if (type == MemType.Custom)
                adapter.bottomInterface.GET_BOOKMARKS(ref __list, LastActiveScene.GetHashCode());
            var INV = type == MemType.Last;
            for (int __c = 0; __c < (type == MemType.Custom ? m_memCache[type].Length : 1); __c++)
            {
                var memoryRoot = m_memCache[type][__c];
                var interator = 0;

                var itemscount = memoryRoot.Count(t => t.IsValid());

                if (itemscount <= 0)
                {
                    if (type == MemType.Custom)
                    {
                        menu.AddDisabledItem(new GUIContent("Category - " + __list[__c].name + "/" + "No Items"));
                    }

                    continue;
                }

                var rowClass = type == MemType.Custom ? GetRowClass(type).MaxItems : int.MaxValue;
                if (itemscount > rowClass) itemscount = rowClass;
                var _scene = LastActiveScene;

                List<EstimItems> result = new List<EstimItems>();
                for (int __i = 0; __i < memoryRoot.Count && interator < itemscount; __i++)
                {
                    var i = __i;
                    if (!memoryRoot[i].IsValid())
                        continue;

                    var h = type == MemType.Last || type == MemType.Custom ? INT32__ACTIVE_TOHIERARCHYOBJECT(memoryRoot[i].InstanceID) : null;

                    if ((type == MemType.Last || type == MemType.Custom) && (h == null || !h.Validate())) continue;
                    ++interator;
                    var content = memoryRoot[i].ToString().Replace('/', '\\');
                    var count = adapter.bottomInterface.INT32_TOOBJECTASLISTCT(memoryRoot[i].InstanceID).Length;
                    if (count > 1) content = "[" + count + "]   " + content;
                    if (type == MemType.Custom) content = "Category - " + __list[__c].name + "/" + content;

                    //                         if ( i == 0 ) {
                    //
                    //                             Debug.Log( memoryRoot[i].IsSelectedHadrScan() );
                    //                             Debug.Log( memoryRoot[i].InstanceID.list.Count );
                    //                             Debug.Log( adapter.IsSelected( memoryRoot[i].InstanceID.list[0].GetInstanceID() ) );
                    //                             Debug.Log( adapter.selMax );
                    //                         }

                    result.Add(new EstimItems()
                    {
                        content = new GUIContent(content),
                        active = memoryRoot[i].IsSelectedHadrScan(),
                        onClick = () => { memoryRoot[i].OnClick(false, _scene.GetHashCode()); }
                    });
                }

                if (INV) result.Reverse();
                foreach (var item in result)
                {
                    menu.AddItem(item.content, item.active, item.onClick);
                    was = true;
                }
            }

            if (!was) menu.AddDisabledItem(new GUIContent("No Items"));
        }

    }
}
#endif