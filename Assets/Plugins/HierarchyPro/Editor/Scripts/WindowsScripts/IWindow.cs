using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor.Windows
{
    // description
    //false, Event.current.mousePosition, 190, 68, adapter

    public class IWindow : EditorWindow
    {


        internal void Label(Rect r, string s, TextAnchor an)
        {
            var a = Root.p[0].label.alignment;
            Root.p[0].label.alignment = an;
            GUI.Label(r, s, Root.p[0].label);
            Root.p[0].label.alignment = a;
        }

        internal void Label(Rect r, string s)
        {
            GUI.Label(r, s, Root.p[0].label);
        }

        internal void Label(Rect r, GUIContent s)
        {
            GUI.Label(r, s, Root.p[0].label);
        }

        internal bool Button(Rect r, string s)
        {
            return GUI.Button(r, s, Root.p[0].button);
        }

        internal bool Button(Rect r, string s, TextAnchor an)
        {
            var a = Root.p[0].button.alignment;
            Root.p[0].button.alignment = an;
            var res = GUI.Button(r, s, Root.p[0].button);
            Root.p[0].button.alignment = a;
            return res;
        }

        internal bool Button(Rect r, GUIContent s)
        {
            return GUI.Button(r, s, Root.p[0].button);
        }

        internal bool Button(Rect r, GUIContent s, TextAnchor an)
        {
            var a = Root.p[0].button.alignment;
            Root.p[0].button.alignment = an;
            var res = GUI.Button(r, s, Root.p[0].button);
            Root.p[0].button.alignment = a;
            return res;
        }

        protected Events.MouseRawUp mouse_uo_helper;

        internal void PUSH_ONMOUSEUP(Func<Events.MouseRawUp.WantMouseLeaveType, bool> ac, EditorWindow win)
        {
            mouse_uo_helper.PUSH_ONMOUSEUP(ac, win);
        }



        internal Action<Texture2D> pickerAc = null;
        internal int pickerId = -1;
        protected bool m_PIN = false;
        int PinInterator = 0;

        internal virtual bool PIN {
            get { return m_PIN || PinInterator > 0; }
            set { m_PIN = value; }
        }

        void PickerCommandNameUpdate()
        {
            var commandName = Event.current.commandName;
            // if (!string.IsNullOrEmpty(commandName)) MonoBehaviour.print("command = " + commandName);
            if (commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == pickerId) //ObjectSelectorClosed
            { // MonoBehaviour.print(EditorGUIUtility.GetObjectPickerObject());
              //comformAction((Texture2D)EditorGUIUtility.GetObjectPickerObject());
            }

            if (commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerControlID() == pickerId)
            {
                pickerId = -1;

                pickerAc(EditorGUIUtility.GetObjectPickerObject() as Texture2D);
                //CloseThis();

                //ObjectSelectorClosed
                //comformAction((Texture2D)EditorGUIUtility.GetObjectPickerObject());
            }
        }

        internal virtual bool OnLostFocusPIN {
            get { return PIN; }
        }

        internal virtual void OnLostFocus() // Debug.Log( EditorGUIUtility.GetObjectPickerControlID() );
        {
            //Debug.Log( pickerId + " " + OnLostFocusPIN );
            if (pickerId != -1 || OnLostFocusPIN)
            {
                return;
            }
            EditorApplication.CallbackFunction up = null;
            int frame = 0;
            var captureOnstance = this;
            up = () => {
                frame++;
                if (frame > 1)
                {
                    if (captureOnstance) CloseThis();
                    EditorApplication.update -= up;
                }
            };
            EditorApplication.update += up;

            //CloseThis();
        }

        internal virtual void OnSelectionChange()
        {
            if (PIN) return;
            CloseThis();
        }

        protected internal virtual bool CloseThis()
        {
            if (!init) return false;
            init = false;
            if (winType != null) __inputWindow.Remove(winType);
            if (_inputWindow != null) _inputWindow.Close();
            return true;
        }

        protected virtual void OnDestroy()
        {
            if (winType == null) return;
            __inputWindow.Remove(winType);
#if UNITY_2018_3_OR_NEWER
            EditorApplication.quitting -= _apquite;
#endif
        }
        protected virtual void OnEnable()
        {
#if UNITY_2018_3_OR_NEWER
            if (!(this is InputWindow ||
#if !EMX_H_LITE
                this is Root_HighlighterWindowInterface ||
#endif
                this is SearchWindow)) return;
            EditorApplication.quitting -= _apquite;
            EditorApplication.quitting += _apquite;
            inst_t = this;
#endif
        }
#if UNITY_2018_3_OR_NEWER
        EditorWindow inst_t;
        void _apquite()
        {
            if (inst_t) inst_t.Close();
        }
#endif
        protected virtual void OnDestroySwitcher()
        {
#if UNITY_2018_3_OR_NEWER
            EditorApplication.quitting -= _apquite;
#endif
        }






        // internal virtual float HEIGHT_CLAMPER {get {return 0.88f;} }
        bool init = false;

        //  static Color matColor = Color.white;
        internal MousePos lastMousePos;


        // [MenuItem("Example/Color Change")]
        internal static Dictionary<Type, IWindow> __inputWindow = new Dictionary<Type, IWindow>();
        protected IWindow _inputWindow;
        internal Type winType;
        internal Window adapter;

        internal static IWindow private_Init(MousePos inrect, Type type, Window a, string title = "_hpro_", Vector2? minSize = null, Vector2? maxSize = null, bool utils = false,
            bool useAnim = true /*, bool skipPositionAssign = false*/)
        {
            if (!__inputWindow.ContainsKey(type)) __inputWindow.Add(type, null);
            if (__inputWindow[type] != null) __inputWindow[type].CloseThis();

            //  var w = __inputWindow[type] = (FocusRoot)EditorWindow.GetWindow(type, true, title, true);
            //  var w = __inputWindow[type] = (FocusRoot)EditorWindow.GetWindow(type, false, title, true);
            /*  var w = __inputWindow[type] = (FocusRoot)EditorWindow.GetWindow(type, true, title, true);*/

            //var w = __inputWindow[type] = (IWindow)EditorWindow.GetWindow(type,true, ,);
            var w = __inputWindow[type] = CreateInstance(type) as IWindow;
            w.titleContent = new GUIContent(title);
            if (utils) w.Show();
            else w.ShowPopup();
            w.Focus();
            w._inputWindow = __inputWindow[type];

            w.mouse_uo_helper = new Events.MouseRawUp();

            w.init = true;
            w.pickerId = -1;
            w.firstLaunch = true;
            w.lastMousePos = inrect;
            var rect = inrect.GetRect(a);
#if !EMX_H_LITE
            if (type == typeof(Root_HighlighterWindowInterface)) rect.x -= rect.width;
#endif
            LegacyWindowsCheck2019_2.CLAMP_WINDOS_RECT(ref rect);

            w.targetRect = rect;
            w.Screenrect = rect;

            if (Root.p[0].par_e.ENABLE_CUSTOMWINDOWS_OPENANIMATION && useAnim) w.Screenrect.height = 25;

            w.adapter = a;

            w.position = w.Screenrect;
            // Debug.Log( "ASD" );
            w.maxSize = maxSize ?? new Vector2(rect.width, w.Screenrect.height);
            w.minSize = minSize ?? new Vector2(rect.width, w.Screenrect.height);
            w.SIZIBLE_X = minSize.HasValue || maxSize.HasValue;
            w.SIZIBLE_X = minSize.HasValue || maxSize.HasValue;
            w.position = w.Screenrect;

            w.winType = type;
            w.PIN = false;
            w.PinInterator = 2;

            w.lastTime = EditorApplication.timeSinceStartup;
            w.wasAnim = false;

            Tools.EventUse();
            w.Repaint();


            return __inputWindow[type];
        }

        // static bool wasFocus = false;
        protected bool wasAnim = false;
        Rect targetRect;
        Rect Screenrect;
#pragma warning disable
        internal bool firstLaunch = false;
#pragma warning restore


        protected virtual void Update()
        { /* if (!firstLaunch)
             {   firstLaunch = true;
                 foreach (var VARIABLE in Resources.FindObjectsOfTypeAll<EditorWindow>().Where( v => v is FocusRoot ))
                     if (VARIABLE) VARIABLE.Close();
                 var input = __inputWindow.Values.FirstOrDefault(i => i);
                 if (!input) return;
                 input.Focus();
             }*/
            if (!(PinInterator < 1)) PinInterator--;
        }

        internal bool animcomplete;


        internal static float MAX_HEIGHT(Window a) //if (! a.window()) Debug.Log("ASD");
        {
            return a.position.height + 20;
            //Screen.currentResolution.height * (HEIGH_CLAMPER)
        }

        internal void SET_NEW_HEIGHT(Window a, float height)
        {
            if (Event.current != null && Event.current.type != EventType.Repaint || !a.Instance) return;

            var h = MousePos.WidnwoRect(null, lastMousePos.Width, height, a, lastMousePos);
            targetRect.height = h.height;
            // targetRect.y = _inputWindow.position.y;

            if (lastMousePos.Clamp)
            {
                if (targetRect.y + targetRect.height > a.Instance.position.y + MAX_HEIGHT(a))
                    targetRect.y -= (targetRect.y + targetRect.height) -
                                    (a.Instance.position.y + MAX_HEIGHT(a));
            }

            //if (targetRect.height > FillterData.MAX_HEIGHT(adapter)) targetRect.height = FillterData.MAX_HEIGHT(adapter);
            //targetRect.y = h.y;
            if (wasAnim)
            {
                AniimationUpdateR(1);
                animcomplete = true;
                //_inputWindow.position = targetRect;
            }

            Repaint();
        }


        internal double lastTime = 0;

        protected virtual void OnGUI()
        {
            if (!_inputWindow)
            {
                return;
            }
            //if (mouse_uo_helper == null )
            //{
            //	Close();
            //	return;
            //}
            /*  if (Event.current.type == EventType.repaint)
               {
                   // MonoBehaviour.print(Screenrect.height);
                   //  Screenrect.height = Mathf.Lerp(Screenrect.height, targetRect.height, 0.1f);
                   Screenrect.height = Mathf.MoveTowards(Screenrect.height, targetRect.height, Hierarchy.deltaTime * 3.1f * 60);
                   var s = _inputWindow.minSize;
                   s.y = Screenrect.height;
                   _inputWindow.minSize = _inputWindow.maxSize = s;
                   _inputWindow.position = Screenrect;

                   //window().Show();
               }*/


            if (Event.current.type == EventType.Repaint)
            {
                var O = 0;
                var br = new Rect(O, O, position.width - 2 * O, position.height - 2 * O);
                // Adapter.GET_SKIN().textArea.Draw( br , "" , false , false , false , false );
                PluginInstance.STYLE_DEFBOX.Draw(br, "", false, false, false, false);
                /* O = 2;
                 br = new Rect( O , O , position.width - 2 * O , position.height - 2 * O );
                 GUI.Box( br , "" );
                 O = 2;
                 br = new Rect( O , O , position.width - 2 * O , position.height - 2 * O );
                 GUI.Box( br , "" );*/

                if (!wasAnim) // MonoBehaviour.print(Screenrect.height);
                { //  Screenrect.height = Mathf.Lerp(Screenrect.height, targetRect.height, 0.1f);


                    var targetTime = Mathf.Clamp01((float)(EditorApplication.timeSinceStartup - lastTime) * 4.5f);
                    if (targetTime == 1) wasAnim = true;
                    //Screenrect.height = Mathf.MoveTowards(Screenrect.height, targetRect.height, deltaTime * 3.1f * 220);
                    AniimationUpdateR(targetTime);
                    //window().Show();
                }
            }


            if (_inputWindow == null)
            {
                return;
            }

            PickerCommandNameUpdate();

            mouse_uo_helper.Invoke();

            //if (Event.current.type == EventType.layout)
        }

        internal bool SIZIBLE_X = false;
        internal bool SIZIBLE_Y = false;

        protected void AniimationUpdateR(float targetTime)
        {
            Screenrect = _inputWindow.position;
            if (Screenrect.x == 0 && Screenrect.y == 0)
            {
                Screenrect.x = targetRect.x;
                Screenrect.y = targetRect.y;
            }

            // if (Screenrect.x <= 0 || Screenrect.y <= 0) return;
            Screenrect.height = Mathf.Lerp(Screenrect.height, targetRect.height, targetTime);
            Screenrect.y = Mathf.Lerp(Screenrect.y, targetRect.y, targetTime);
            animcomplete = targetTime >= 1;

            var s = _inputWindow.minSize;
            s.y = Screenrect.height;
            if (!SIZIBLE_X) _inputWindow.minSize = s;
            if (!SIZIBLE_Y)
            {
                _inputWindow.maxSize = s;
                // Debug.Log( "ASD" );
            }

            if (_inputWindow.position != Screenrect)
            {
                _inputWindow.position = Screenrect;
            }

            /*if (Screenrect.height != targetRect.height)*/
            _inputWindow.Repaint();
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}