
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;



namespace EMX.HierarchyPlugin.Editor.Mods
{
    class LayoutsMod
    {

        PluginInstance p;
        Events.MouseRawUp _mouse_uo_helper;
        internal LayoutsMod(PluginInstance _p)
        {
            p = _p;
            _mouse_uo_helper = new Events.MouseRawUp();
        }

        static Type WindowLayout;
        internal static MethodInfo LoadWindowLayout, SaveWindowLayout/*, GetCurrentLayoutPath*/;
        internal static PropertyInfo layoutsModePreferencesPath;
        internal static string d { get { return Folders.CALC_SETTINGS_PATH_EXTERNAL + "/.SavedLayouts/"; } }
        //string d { get { return Folders.PluginInternalFolder + "/Editor/_SAVED_DATA/.SavedLayouts/"; } }
        GUIStyle _button;
        GUIStyle button {
            get {
                if (_button == null)
                {
                    _button = new GUIStyle(EditorStyles.toolbarButton);
                    _button.padding.left = 8;
                    _button.padding.top = 4;
                    _button.padding.bottom = 4;
                    _button.fontStyle = FontStyle.Bold;
                    _button.fontSize -= 1;
                    _button.alignment = TextAnchor.MiddleLeft;
                    _button.fixedHeight = 0;
                    _button.stretchHeight = true;
                    _button.margin = new RectOffset();
                }
                return _button;
            }
        }
        GUIStyle _button_center;
        GUIStyle button_center {
            get {
                if (_button_center == null)
                {
                    _button_center = new GUIStyle(button);
                    _button_center.alignment = TextAnchor.MiddleCenter;
                    _button_center.padding.top = 0;
                    _button_center.fontSize += 3;
                }
                return _button_center;
            }
        }
        GUIStyle _button_cross;
        GUIStyle button_cross {
            get {
                if (_button_cross == null)
                {
                    _button_cross = new GUIStyle(button);
                    _button_cross.alignment = TextAnchor.MiddleCenter;
                    _button_cross.padding = new RectOffset();
                    _button_cross.fontStyle = FontStyle.Normal;
                }
                return _button_cross;
            }
        }

        internal void Subscribe(EditorSubscriber sbs)
        {

            sbs.OnEditorWantsToQuit.Add(OnEditorWantsToQuit);
        }
        bool OnEditorWantsToQuit()
        {
            TRY_AUTOSAVE();
            return true;
        }
        void TRY_AUTOSAVE()
        {
            if (string.IsNullOrEmpty(LAST_LAYOUT) || !GET_SAVED().Contains(LAST_LAYOUT)) return;
            TRY_AUTOSAVE(LAST_LAYOUT);
        }
        void TRY_AUTOSAVE(string item)
        {
            if (!p.par_e.DRAW_TOPBAR_LAYOUTS_BAR || !p.par_e.USE_TOPBAR_MOD) return;
            if (p.par_e.TOPBAR_LAYOUTS_AUTOSAVE)
            {
                if (!p.par_e.TOPBAR_LAYOUTS_SAVE_ONLY_CUSTOM || item.StartsWith("Assets", StringComparison.OrdinalIgnoreCase)) SAVE(item);
            }
        }
        bool OnMouseUp(Events.MouseRawUp.WantMouseLeaveType t)
        {
            stateForDrag_B0 = null;
            return true;
        }
        string dragName;
        Rect swapRect;
        Rect? stateForDrag_B0;
        Vector2 stateForDrag_B2;
        bool wasDragging = false;
        int currentSelection = 0;
        Rect cross;
        float time = 0;


        internal static void InitProperties()
        {
            if (WindowLayout == null)
            {
                WindowLayout = typeof(EditorWindow).Assembly.GetType("UnityEditor.WindowLayout") ?? throw new Exception("UnityEditor.WindowLayout");
                // new ParameterModifier[1] {new ParameterModifier(4)}
                LoadWindowLayout = WindowLayout.GetMethod("LoadWindowLayout", new[] { typeof(string), typeof(bool), typeof(bool), typeof(bool) }) ?? throw new Exception("LoadWindowLayout");
                //  LoadWindowLayout // path false true true keepMainWindow
                SaveWindowLayout = WindowLayout.GetMethod("SaveWindowLayout", ~(BindingFlags.Instance)) ?? throw new Exception("SaveWindowLayout");
                layoutsModePreferencesPath = WindowLayout.GetProperty("layoutsModePreferencesPath", ~(BindingFlags.Instance)) ?? throw new Exception("layoutsModePreferencesPath");
                //GetCurrentLayoutPath = WindowLayout.GetMethod("GetCurrentLayoutPath", ~(BindingFlags.Instance)) ?? throw new Exception("SaveWindowLayout");
            }
        }
        internal void DrawLayers()
        {


            /*
            InternalEditorUtility.sav

            SaveWindowLayout.
            Menu.a
            WindowsL
            InternalEditorUtility.lay
           */
            //  var R = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true), GUILayout.Width(0));
            var R = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            //R.height += 9;
            if (p.par_e.TOPBAR_LAYOUTS_EXPAND_HEIGHT) R.height += 8;
            var sh = R.height;
            //R.y += 8 + p.par_e.TOPBAR_LAYOUTS_MIN_Y_OFFSET;
            // R.height += p.par_e.TOPBAR_LAYOUTS_MAN_Y_OFFSET - p.par_e.TOPBAR_LAYOUTS_MIN_Y_OFFSET;
#if UNITY_2021_1_OR_NEWER //2021
#endif
            if (Event.current.type != EventType.Layout)
            {


                InitProperties();

                //R.height += p.par_e.TOPBAR_LAYOUTS_MAN_Y_OFFSET - p.par_e.TOPBAR_LAYOUTS_MIN_Y_OFFSET;
                if (R.height <= 0) R.height = 1;
                //R.y += 8;
#if !UNITY_2021_1_OR_NEWER //2021
#endif
                // R.height += 4;
                var height_lerp = Mathf.Clamp01((R.height - sh / 2) * 2 / sh);
                //button.padding.bottom = button_center.padding.bottom =
                //	 Mathf.RoundToInt( 12 * height_lerp );

                if (p.par_e.TOPBAR_LAYOUTS_EXPAND_HEIGHT) button.padding.bottom = button_center.padding.bottom = Mathf.RoundToInt(12 * height_lerp);
                else
                {
                    // button.padding.bottom = EditorStyles.toolbarButton.padding.bottom;
                    // button_center.padding.bottom = EditorStyles.toolbarButton.padding.bottom;
                    button.padding.bottom = button_center.padding.bottom = 4;

                }

                if (GET_SAVED().Length < 10) R.width -= R.height;
                var fullRect = R;

                bool skipReset = false;
                if (Event.current.type == EventType.Repaint)
                {
                    if (time != 0)
                    {
                        time -= p.deltaTime * 4;
                        if (time < 0)
                        {
                            time = 0;
                            skipReset = true;
                        }
                    }
                }

                R.width /= GET_SAVED().Length;
                if (p.par_e.TOPBAR_LAYOUTS_USE_FIXED_WIDTH) R.width = p.par_e.TOPBAR_LAYOUTS_FIXED_WIDTH_AMOUNT;
                var cache = GET_SAVED();
                Rect capR = R;
                for (int i = 0; i < cache.Length + 1; i++)
                {
                    bool breakAfterEnd = false;

                    var _R = R;

                    if (i == cache.Length)
                    {
                        if (!stateForDrag_B0.HasValue) break;
                        i = currentSelection;
                        _R = capR;
                        breakAfterEnd = true;
                    }
                    else
                        if (stateForDrag_B0.HasValue && dragName == cache[i])
                    {
                        capR = R;
                        R.x += R.width;
                        continue;
                    }


                    var item = cache[i];
                    var name = Path.GetFileNameWithoutExtension(item);
                    /*if (GUI.Button(R, name, button))
                    {
                        SET(item);
                    }
                    if (LAST_LAYOUT == item && Event.current.type == EventType.Repaint)*/

                    var contains = _R.Contains(Event.current.mousePosition);


                    var controlId = EditorGUIUtility.GetControlID(FocusType.Passive, _R);
                    if (!pos.ContainsKey(item)) pos.Add(item, null);
                    if (Event.current.type == EventType.Repaint)
                    {
                        //var p = pos.ContainsKey(item)? pos
                        if (!pos[item].HasValue) pos[item] = _R;
                        var p = pos[item];
                        if ((p.Value.x != _R.x || p.Value.width != _R.width) && (!stateForDrag_B0.HasValue || dragName != item))
                        {
                            if (time == 0 && !skipReset) time = 1;
                            var x = Mathf.Lerp(p.Value.x, _R.x, 1 - time);
                            var width = Mathf.Lerp(p.Value.width, _R.width, 1 - time);
                            pos[item] = new Rect(x, _R.y, width, _R.height);
                            RepaintBar();
                        }

                    }
                    if (item == LAST_LAYOUT && p.par_e.TOPBAR_LAYOUTS_DRAWCROSS)
                    {
                        //  var controlId = EditorGUIUtility.GetControlID(FocusType.Passive, _R);
                        //  button.Draw(p.Value, CONT(name, "- Left click to select\n- Right click to open menu\n- Middle click to remove"), controlId,
                        cross = pos[item] ?? _R;
                        //cross.x += cross.width - cross.height / 3 * 2 + 2;
                        cross.x += cross.width - sh / 3 * 2 + 2;
                        cross.width = sh / 3;
                        cross.y += Mathf.Lerp(0, cross.height / 3 - 4, height_lerp);
                        cross.width = cross.height = Mathf.Lerp(cross.height, cross.height / 3, height_lerp);
                        if (cross.Contains(Event.current.mousePosition)) contains = false;

                    }
                    if (Event.current.type == EventType.Repaint)
                    {
                        button.Draw(pos[item].Value, CONT(name, "- Click to select\n- Right-Click to open menu\n- Middle-click to remove"), controlId, LAST_LAYOUT == item, contains);
                    }
                    if (item == LAST_LAYOUT && p.par_e.TOPBAR_LAYOUTS_DRAWCROSS)
                    {
                        if (GUI.Button(cross, new GUIContent("✕", "Remove " + item + " from tab"), p.STYLE_DEFBUTTON_middle))
                        {
                            var l = cache.ToList();
                            l.RemoveAt(i);
                            SET_SAVED(l);
                            RepaintBar();
                        }
                    }

                    if (contains && Event.current.type == EventType.MouseUp && stateForDrag_B0.HasValue && stateForDrag_B0.Value.Contains(Event.current.mousePosition))
                    {
                        if (!wasDragging)
                        {
                            _mouse_uo_helper.WantMouseLeave(Events.MouseRawUp.WantMouseLeaveType.Other);
                            Event.current.Use();
                            TRY_AUTOSAVE(item);
                            SET(item);
                        }

                    }



                    if (contains && Event.current.type == EventType.MouseDown)
                    {
                        int capt_i = i;
                        if (Event.current.button == 0)
                        {
                            if (stateForDrag_B0 == null) _mouse_uo_helper.PUSH_ONMOUSEUP(OnMouseUp);
                            stateForDrag_B0 = _R;
                            swapRect = _R;
                            dragName = item;
                            wasDragging = false;
                            currentSelection = i;
                            stateForDrag_B2 = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                            Event.current.Use();
                            RepaintBar();
                        }
                        if (Event.current.button == 1)
                        {
                            var menu = new GenericMenu();

                            menu.AddItem(new GUIContent("Save Current Layout To " + name), false, () => {
                                SAVE(item);
                            });
                            menu.AddSeparator("");

                            menu.AddItem(new GUIContent("Enable Layouts AutoSave"), p.par_e.TOPBAR_LAYOUTS_AUTOSAVE, () => {
                                p.par_e.TOPBAR_LAYOUTS_AUTOSAVE = !p.par_e.TOPBAR_LAYOUTS_AUTOSAVE;
                            });
                            if (item.StartsWith("Assets", StringComparison.OrdinalIgnoreCase))
                            {
                                menu.AddSeparator("");
                                menu.AddItem(new GUIContent("Rename " + name), false, () => {
                                    SaveUI("Rename " + name, name, (ffa) => {
                                        foreach (var @as in System.IO.Path.GetInvalidFileNameChars())
                                        {
                                            ffa = ffa.Replace(@as.ToString(), "");
                                        }
                                        if (string.IsNullOrEmpty(ffa)) return;
                                        var s = GET_SAVED().ToList();
                                        var oldI = s.IndexOf(item);
                                        if (oldI != -1)
                                        {
                                            var ind = s.IndexOf(item);
                                            s.Remove(item);
                                            SET_SAVED(s);
                                            SAVE(d + ffa + ".wlt", ind);
                                            RepaintBar();
                                        }

                                    });
                                });
                            }
                            menu.AddSeparator("");
                            menu.AddItem(new GUIContent("Remove " + name), false, () => {
                                var l = cache.ToList();
                                l.RemoveAt(capt_i);
                                SET_SAVED(l);
                                RepaintBar();
                            });
                            menu.AddSeparator("");
                            menu.AddItem(new GUIContent("Open TopBar Settings"), false, () => {
                                Settings.MainSettingsEnabler_Window.Select<Settings.TB_Window>();
                            });
                            menu.ShowAsContext();
                            Event.current.Use();
                        }
                        if (Event.current.button == 2)
                        {
                            var l = cache.ToList();
                            l.RemoveAt(capt_i);
                            SET_SAVED(l);
                            Event.current.Use();
                            RepaintBar();
                        }
                    }


                    if (breakAfterEnd) break;

                    R.x += R.width;
                }

                _mouse_uo_helper.Invoke();


                if (stateForDrag_B0.HasValue)
                {

                    if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
                    {
                        _mouse_uo_helper.WantMouseLeave(Events.MouseRawUp.WantMouseLeaveType.Escape);
                        Event.current.Use();
                    }
                    else
                    {
                        var hoverRect = stateForDrag_B0.Value;
                        var dif = GUIUtility.GUIToScreenPoint(Event.current.mousePosition).x - stateForDrag_B2.x;
                        hoverRect.x = dif + hoverRect.x;
                        hoverRect.x = Mathf.Max(fullRect.x, hoverRect.x);
                        hoverRect.x = Mathf.Min(fullRect.x + fullRect.width - hoverRect.width, hoverRect.x);

                        pos[dragName] = hoverRect;

                        if (hoverRect.x < swapRect.x - hoverRect.width / 1.5f)
                        {
                            swapRect.x -= swapRect.width;
                            swapRect.x = Mathf.Max(fullRect.x, swapRect.x);
                            if (currentSelection > 0)
                            {
                                var t = cache[currentSelection];
                                cache[currentSelection] = cache[currentSelection - 1];
                                cache[currentSelection - 1] = t;
                                SET_SAVED(cache.ToList());
                                currentSelection--;
                            }
                        }
                        else if (hoverRect.x > swapRect.x + hoverRect.width / 1.5f)
                        {
                            swapRect.x += swapRect.width;
                            swapRect.x = Mathf.Min(fullRect.x + fullRect.width - swapRect.width, swapRect.x);
                            if (currentSelection < cache.Length - 1)
                            {
                                var t = cache[currentSelection];
                                cache[currentSelection] = cache[currentSelection + 1];
                                cache[currentSelection + 1] = t;
                                SET_SAVED(cache.ToList());
                                currentSelection++;
                            }
                        }

                        if (Mathf.Abs(dif) > 1) wasDragging = true;

                        /*  if (EditorGUIUtility.isProSkin) p.gl.DRAW_TAP_GLOW(hoverRect);
                          else p.gl.DRAW_TAP_GLOW(hoverRect, new Color(1, 1, 1, 0.5f));*/

                        if (Event.current.isMouse) Tools.EventUse();
                        RepaintBar();
                    }
                }



                //WindowLayout.LoadWindowLayout(layoutPath, false)
                R.width = R.height;
                if (cache.Length < 10 && GUI.Button(R, CONT("+", "Add layout"), button_center))
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Create new from current layout/Add to " + ASSET_FOLDER + ""), false, () => {
                        SaveUI("New layout name", "MyLayout", (res) => {
                            foreach (var item in System.IO.Path.GetInvalidFileNameChars())
                            {
                                res = res.Replace(item.ToString(), "");
                            }
                            if (string.IsNullOrEmpty(res)) return;
                            SAVE(d + res + ".wlt");
                            RepaintBar();
                        });
                    });
                    /*   menu.AddItem(new GUIContent("Create New From Current Layout/Add To Internal Unity Layouts"), false, () =>
                       {


                           var last = (string)typeof(EditorWindow).Assembly.GetType("UnityEditor.Toolbar").GetProperty("lastLoadedLayoutName", ~BindingFlags.Instance).GetValue(null, null);



                       });*/
                    var saved = cache.ToList();
                    //if ( !Directory.Exists( d ) ) Directory.CreateDirectory( d );
                    var my = !Directory.Exists(d) ? new string[0] : System.IO.Directory.GetFiles(d, "*.wlt")/*.Where(path => path.EndsWith(".wlt"))*/.ToArray();
                    if (my.Length == 0)
                    {
                        menu.AddDisabledItem(new GUIContent("Load from assets folder"));
                    }
                    else
                    {
                        foreach (var item in my)
                        {
                            string layoutPath = item;
                            menu.AddItem(new GUIContent("Load from " + ASSET_FOLDER + "/" + Path.GetFileNameWithoutExtension(layoutPath)), saved.Contains(item),

                                () => { ADD(layoutPath); }
                                                          );

                        }
                        if (my.Length == 0)
                        {
                            menu.AddDisabledItem(new GUIContent("Load from " + ASSET_FOLDER + "/No layouts"));
                        }
                        menu.AddSeparator("Load from " + ASSET_FOLDER + "/");
                        menu.AddItem(new GUIContent("Load from " + ASSET_FOLDER + "/Open " + ASSET_FOLDER + ""), false,
                               () => {
                                   Settings.SETGUI_MODS_ENABLER.REV(d);
                               }
                                                         );
                    }

                    menu.AddSeparator("");
                    // if (ModeService.HasCapability(ModeCapability.LayoutWindowMenu, true))
                    {
                        //bool was = false;
                        if (System.IO.Directory.Exists((string)layoutsModePreferencesPath.GetValue(null, null)))
                        {
                            foreach (string str in ((IEnumerable<string>)System.IO.Directory.GetFiles((string)layoutsModePreferencesPath.GetValue(null, null), "*.wlt"))
                                // .Where<string>( (Func<string, bool>)(path => path.EndsWith( ".wlt" )) )
                                .ToArray())
                            {
                                //was = true;
                                string layoutPath = str;
                                menu.AddItem(new GUIContent("Load from unity layouts/" + Path.GetFileNameWithoutExtension(layoutPath)), saved.Contains(layoutPath),
                            () => { ADD(layoutPath); }
                                    );
                            }
                        }
                        //EMX_TODO mod services exluded since 2020
                        /*	var GetModeDataSection = typeof(EditorApplication).Assembly.GetType("UnityEditor.ModeService").GetMethod("GetModeDataSection", ~BindingFlags.Instance);
							if (GetModeDataSection.Invoke(null, new object[] { ModeService.currentIndex, "layouts" }) is IList<object>)
							{
								var modeDataSection = GetModeDataSection.Invoke(null, new object[] { ModeService.currentIndex, "layouts" }) as IList<object>;
								foreach (string str in modeDataSection.Cast<string>())
								{
									string layoutPath = str;
									if (File.Exists(layoutPath))
									{
										if (was)
										{
											was = false;
											menu.AddSeparator("Load From Unity Layouts/");
										}
										string withoutExtension = Path.GetFileNameWithoutExtension(layoutPath);
										menu.AddItem(new GUIContent("Load From Unity Layouts/" + withoutExtension), saved.Contains(layoutPath),
											() => { ADD(layoutPath); }
											);
									}
								}
							}*/
                    }


                    //  menu.AddItem(new GUIContent("Load From Unity Layouts"), false, () => { });
                    menu.ShowAsContext();
                }
            }
        }

        internal static string ASSET_FOLDER = "[.SavedLayout] folder";
        //string ASSET_FOLDER = "Custom Folder";
        //Default.wlt
        //Tall.wlt
        //2 by 3.wlt
        //4 Split.wlt
        string[] _get_defaults;
        string[] get_defaults {
            get {
                if (_get_defaults != null) return _get_defaults;
                if (!System.IO.Directory.Exists((string)layoutsModePreferencesPath.GetValue(null, null))) return (_get_defaults = Enumerable.Repeat("", 30).ToArray());
                _get_defaults = ((IEnumerable<string>)System.IO.Directory.GetFiles((string)layoutsModePreferencesPath.GetValue(null, null), "*.wlt"))
                        .Where<string>((Func<string, bool>)(path => path.EndsWith(
                           "Default.wlt", StringComparison.OrdinalIgnoreCase) ||
                           path.EndsWith("2 by 3.wlt", StringComparison.OrdinalIgnoreCase) ||
                           path.EndsWith("4 Split.wlt", StringComparison.OrdinalIgnoreCase)))
                        .ToArray<string>();
                _get_defaults = _get_defaults.Reverse().ToArray();
                if (_get_defaults.Length < 30)
                {
                    var si = _get_defaults.Length;
                    Array.Resize(ref _get_defaults, 30);
                    for (int i = si; i < _get_defaults.Length; i++)
                    {
                        _get_defaults[i] = "";
                    }
                }
                return _get_defaults;
            }
        }


        GUIContent _c = new GUIContent();
        MethodInfo RepaintToolbar;
        void RepaintBar()
        {
            if (RepaintToolbar == null)
                RepaintToolbar = typeof(EditorWindow).Assembly.GetType("UnityEditor.Toolbar").GetMethod("RepaintToolbar", ~BindingFlags.Instance);
            RepaintToolbar.Invoke(null, null);
        }

        GUIContent CONT(string text, string tooltiop)
        {
            _c.text = text;
            _c.tooltip = tooltiop;
            return _c;
        }

        string LAST_LAYOUT {
            get { return Root.p[0].par_e.GET("LAST_LAYOUT", ""); }
            set {
                if (LAST_LAYOUT == value) return;
                Root.p[0].par_e.SET("LAST_LAYOUT", value);
            }
        }

        void SAVE(string path, int index = -1)
        {
            /*if (path.StartsWith("Assets"))
            {
                File.Write
            }*/
            // if ( !Directory.Exists( d ) ) Directory.CreateDirectory( d );
            var saved = GET_SAVED().ToList();
            if (saved.Count >= 10) return;
            SaveWindowLayout.Invoke(null, new object[] { path });
            if (!saved.Contains(path))
            {
                if (index == -1 || index >= saved.Count) saved.Add(path);
                else saved.Insert(index, path);
                if (!pos.ContainsKey(path)) pos.Add(path, null);
                pos[path] = null;
            }
            // saved.Remove(path);
            // saved.Add(path);
            SET_SAVED(saved);
            LAST_LAYOUT = path;
        }
        void ADD(string path)
        {
            /*if (path.StartsWith("Assets"))
            {
                File.Write
            }*/
            //if ( !Directory.Exists( d ) ) Directory.CreateDirectory( d );
            var saved = GET_SAVED().ToList();
            if (saved.Count >= 10) return;
            // SaveWindowLayout.Invoke(null, new object[] { path });
            if (!saved.Contains(path))
            {
                saved.Add(path);
                if (!pos.ContainsKey(path)) pos.Add(path, null);
                pos[path] = null;
            }
            SET_SAVED(saved);
            RepaintBar();
            //LAST_LAYOUT = path;
        }

        void SET(string path)
        {
            ////  LoadWindowLayout // path false true true keepMainWindow
            if (!File.Exists(path))
            {
                EditorUtility.DisplayDialog("Cannot load layout", "Layout doesn't exist:\n...-" + path, "Ok");
                return;
            }
            var t = File.ReadAllText(path);
            if (t.Length < 10)
            {
                EditorUtility.DisplayDialog("Cannot read layout", "Cannot read layout:\n...-" + path, "Ok");
                return;
            }

            LoadWindowLayout.Invoke(null, new object[4] { path, false, true, false });
            LAST_LAYOUT = path;
        }

        string[] _savedCache = null;
        Dictionary<string, Rect?> pos = new Dictionary<string, Rect?>();


        internal void ClearTimers()
        {
            pos.Clear();
            time = 0;
        }

        //Rect?[] pos = new Rect?[100];
        //bool wasInitLayouts { get { Root.p[0].par_e.GET("INTERNAL_LAYOUT_WASINIT", false); } set { Root.p[0].par_e.SET( "INTERNAL_LAYOUT_WASINIT", value); } }


        string[] GET_SAVED()
        {
            if (_savedCache != null) return _savedCache;
            // var r = System.IO.Directory.GetFiles((string)layoutsModePreferencesPath.GetValue(null, null)).Where(path => path.EndsWith(".wlt")).ToList();
            var r = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var l = Root.p[0].par_e.GET("INTERNAL_LAYOUT_" + i.ToString(), get_defaults[i]);
                if (string.IsNullOrEmpty(l)) continue;
                int tl = -1;
                if ((tl = l.LastIndexOf('?')) != -1 && tl != l.Length - 1) l = d + l.Substring(tl + 1);
                r.Add(l);
            }
            return _savedCache = r.ToArray();
        }
        void SET_SAVED(List<string> saved)
        {
            _savedCache = null;
            for (int i = 0; i < 10; i++)
            {
                var res = i < saved.Count ? saved[i] : "";
                if (res.StartsWith(d, StringComparison.OrdinalIgnoreCase)) res = "?" + res.Substring(d.Length);
                var l = Root.p[0].par_e.GET("INTERNAL_LAYOUT_" + i.ToString(), get_defaults[i]);
                // Debug.Log( res );
                if (l != res) Root.p[0].par_e.SET("INTERNAL_LAYOUT_" + i.ToString(), res);
                //Debug.Log(Root.p[0].par_e.GET("INTERNAL_LAYOUT_" + i.ToString(), "-"));
            }
        }


        void SaveUI(string windowName, string s, Action<string> action)
        {
            var w = Root.p[0].window;
            Action result = () => {
                var adapter = Root.p[0];
                var pos = new MousePos(Event.current.mousePosition, MousePos.Type.Input_190_68, false, adapter);
                Windows.InputWindow.Init(pos, windowName, w, action, null, s);
            };

            if (Event.current == null)
            {
                if (Root.p[0].PUSH_GUI_ONESHOT(0, result)) Root.p[0].RepaintWindowInUpdate(0);
                else
                {
                    var adapter = Root.p[0];
                    var p = new Vector2(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2);
                    var pos = new MousePos(p, MousePos.Type.Input_190_68, false, adapter);
                    Windows.InputWindow.Init(pos, windowName, w, action, null, s);

                }
            }

            else
            {
                result();
            }
        }
    }
}





/*

public class _test_class
{
	[InitializeOnLoadMethod] //special unity's editor API attribute
	static void AddButtonOoToolBar() //should be a static
	{
		EMX.CustomizationHierarchy.TopGUI.onLeftLayoutGUI += ( rect )=> { //you can subscribe to the left or right area

			if ( GUI.Button( rect, "GO MY 1 LAYOUT" ) ) Debug.Log( "Hello Unity!" );

		};
	}
	//Don't forget enable 'Use Custom Buttons' toggles in the top bar settings
}
*/