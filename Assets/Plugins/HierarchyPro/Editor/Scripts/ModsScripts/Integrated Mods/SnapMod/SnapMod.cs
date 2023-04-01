using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor.Mods
{




	class SnapMod
	{



		static string SnapModSwitcherDir { get { return Folders.PluginInternalFolder + "/Editor/"; } }
		static string SnapModSwitcherFile = "SnapModSwitcher.cs";
		static string SwitcherContent = @"#define USE
#if USE
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace EMX." + Root.PN_NS + @".Editor.Mods
{


	[CanEditMultipleObjects, CustomEditor(typeof(Transform), true)]
    public partial class SnapButtonsForInspectorGUI : UnityEditor.Editor
    {
    }
}
#endif";

		internal static bool SET_ENABLE(bool value)
		{

			if (!Directory.Exists(SnapModSwitcherDir)) Directory.CreateDirectory(SnapModSwitcherDir);
			var path = SnapModSwitcherDir + SnapModSwitcherFile;
			var ex = File.Exists(path);
			if (!ex && !value) return false;
			MonoScript mono = ex ? AssetDatabase.LoadAssetAtPath<MonoScript>(path) : null;
			if (!mono && !value) return false;

			if (!mono)
			{
				File.WriteAllText(path, SwitcherContent);
				AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
			}

			var l = File.ReadAllLines(path);
			var oldVal = !l[0].Trim(new[] { ' ', '\r', '\n' }).StartsWith("//");
			if (oldVal == value)
			{
				return false;
			}
			if (value) l[0] = l[0].Replace("/", "");
			else l[0] = "//" + l[0];
			File.WriteAllLines(path, l);
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
			Root.RequestScriptReload();
			return true;
		}










		////////
		//////// SETTINGS PARAMS ////////
		////////

		public static CachedPref SNAP_SNAP_USEHOTKEYS = new CachedPref("SNAP_SNAP_USE", true) { name = "Press key to activate snap" };
		public static CachedPref SNAP_SNAP_HOTKEYS = new CachedPref("SNAP_SNAP_HOTKEYS", (int)KeyCode.K) { name = "Ctrl+shift+..." };

		public static CachedPref SNAP_SURFACE_USEHOTKEYS = new CachedPref("SNAP_SURFACE_USE", true) { name = "Press key to activate surface" };
		public static CachedPref SNAP_SURFACE_HOTKEYS = new CachedPref("SNAP_SURFACE_HOTKEYS", (int)KeyCode.L) { name = "Ctrl+shift+..." };

		public static CachedPref SNAP_FASTINVERT_USEHOTKEYS = new CachedPref("SNAP_INVERT_USE", false) { name = "Use key holding to activate snap" };
		public static CachedPref SNAP_FASTINVERT_HOTKEYS = new CachedPref("SNAP_INVERT_HOTKEYS", (int)KeyCode.C) { name = "Hold:" };

		public static VectorPref SNAP_POS = new VectorPref("SNAP_TOOGLES_POS", enableName: "Enable position snapping %#k");
		public static VectorPref SNAP_ROT = new VectorPref("SNAP_TOOGLES_ROT", enableName: "Enable rotation snapping");
		public static VectorPref SNAP_SCALE = new VectorPref("SNAP_TOOGLES_SCALE", true, enableName: "Enable scale snapping");
		public static CachedPref SNAP_AUTOAPPLY = new CachedPref("SNAP_TOOGLES_SNAP_AUTOAPPLY", false) { name = "Auto-apply snap (for new selected object)" };

		public static CachedPref PLACE_ON_SURFACE_ENABLE = new CachedPref("SNAP_TOOGLES_PLACE_ON_SURFACE_ENABLE", false) { name = "Enable surface placement %#l" };
		public static CachedPref PLACE_ON_SURFACE_ALIGNBYMOUSE = new CachedPref("SNAP_TOOGLES_PLACE_ON_SURFACE_ALIGNBYMOUSE", true) { name = "" };
		// public static CachedPref PLACE_ON_SURFACE_BOUNDS = new CachedPref( "SNAP_TOOGLES_PLACE_ON_SURFACE_BOUNDS", true ) { name = "Calc bounds offset" };
		//public static CachedPref PLACE_ON_SURFACE_BOUNDS = new CachedPref("SNAP_TOOGLES_PLACE_ON_SURFACE_BOUNDS", true) { name = "Enable object's bound calculation (if disabled)" };
		public static CachedPref CALCULATE_OBJECT_BOUNDS = new CachedPref("CALCULATE_OBJECT_BOUNDS", false) { name = "Enable object's bound calculation (if disabled object will snap to pivot)" };

		public static CachedPref ALIGN_BY_NORMAL = new CachedPref("SNAP_TOOGLES_ALIGN_BY_NORMAL", false) { name = "Enable align by surface normal" };
		public static CachedPref ALIGN_UP_VECTOR = new CachedPref("SNAP_TOOGLES_ALIGN_UP_VECTOR", 0) { name = "Surface snapping direction" };

		////////
		//////// UPDATING ////////
		////////

		// vars
		static Vector3 p, v;
		static bool b;
		static System.Reflection.PropertyInfo dragActive;
		// strings
		struct PRF { public string key; public float def; }
		static PRF _PRF(string s, float v) { return new PRF() { key = s, def = v }; }
		static PRF[] POS_PREFKEYS = { _PRF("MoveSnapX", 1), _PRF("MoveSnapY", 1), _PRF("MoveSnapZ", 1) };
		static PRF[] ROT_PREFKEYS = { _PRF("RotationSnap", 15), _PRF("RotationSnap", 15), _PRF("RotationSnap", 15) };
		static PRF[] SCALE_PREFKEYS = { _PRF("ScaleSnap", 0.1f), _PRF("ScaleSnap", 0.1f), _PRF("ScaleSnap", 0.1f) };
		static string[] UNDO_TEXT = { "Move", "Rotate", "Scale" };
		public static Vector3[] VECTORS = { Vector3.up, Vector3.forward, Vector3.down, Vector3.right, Vector3.left, Vector3.back };
		public static string[] VECTORS_STRING = { "Look up", "Look forward", "Look down", "Look right", "Look left", "Look back" };
		public static string[] ALIGN_BY = { "Alignment by mouse position", "Alignment by camera projection" };

		// initialization
		internal static void Subscribe(EditorSubscriber sbs)
		{
			sbs.OnUpdate += EditorApplication_UPDATESNAPPING;
			sbs.duringSceneGui += SceneView_PLACEONSURFACE;
			sbs.duringSceneGui += modifierKeysChanged_SCENE;
			sbs.OnModifyKeyChanged += modifierKeysChanged_KEYS;
			sbs.OnGlobalKeyPressed += globalKeyPressed;
			//  EditorApplication.update += EditorApplication_UPDATESNAPPING;
			if (!wasInit)
			{
				dragActive = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.TransformManipulator").GetProperty("active");
				var snapSettings = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.SnapSettings");
				var initMethod = snapSettings.GetMethod("Initialize", (System.Reflection.BindingFlags)int.MaxValue);
				if (initMethod != null) initMethod.Invoke(null, null);
				wasInit = true;
			}
		}


		static void globalKeyPressed(bool used)
		{
			if (used) return;
			if (Event.current.type == EventType.KeyDown && Event.current.shift && Event.current.control)
			{
				bool has = false;
				if (SNAP_SNAP_USEHOTKEYS && (int)Event.current.keyCode == (int)SNAP_SNAP_HOTKEYS) { SNAP_POS.ENABLE.Set(!SNAP_POS.ENABLE); has = true; }
				if (SNAP_SURFACE_USEHOTKEYS && (int)Event.current.keyCode == (int)SNAP_SURFACE_HOTKEYS) { PLACE_ON_SURFACE_ENABLE.Set(!PLACE_ON_SURFACE_ENABLE); has = true; }
				if (has) UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
			}
		}



		static bool wasInit;
		// save object params
		static void SET_DIRTY(Transform t)
		{
			if (!Application.isPlaying) EditorUtility.SetDirty(t);
			//  if (!t.gameObject.scene.isDirty) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(t.gameObject.scene);
			if (!Application.isPlaying) Root.p[0].MarkSceneDirty(t.gameObject.scene);
		}

		public struct Keys
		{
			public KeyCode keyCode; public bool ctrl; public bool shift; public bool alt;
			public Keys(KeyCode keyCode, bool control, bool shift, bool alt) : this()
			{
				this.keyCode = keyCode;
				this.ctrl = control;
				this.shift = shift;
				this.alt = alt;
			}
		}
		internal static Dictionary<int, Keys> sceneKeyCode = new Dictionary<int, Keys>();
		static void modifierKeysChanged_SCENE(SceneView sv)
		{
			if (!SNAP_FASTINVERT_USEHOTKEYS) return;
			if (!sceneKeyCode.ContainsKey(GUIUtility.keyboardControl)) sceneKeyCode.Add(GUIUtility.keyboardControl, new Keys());
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None)
			{
				sceneKeyCode[GUIUtility.keyboardControl] = new Keys(Event.current.keyCode, Event.current.control, Event.current.shift, Event.current.alt);
			}
			if (Event.current.type == EventType.KeyUp)
			{
				sceneKeyCode[GUIUtility.keyboardControl] = new Keys();
			}
		}
		static System.Reflection.FieldInfo ins;
		internal static void modifierKeysChanged_KEYS()
		{
			if (SNAP_FASTINVERT_HOTKEYS == 0 || !SNAP_FASTINVERT_USEHOTKEYS) return;

			if (ins == null)
			{
				var wa = Resources.FindObjectsOfTypeAll<EditorWindow>().FirstOrDefault(w => w.GetType().FullName == "UnityEditor.InspectorWindow");
				if (!wa) return;
				ins = wa.GetType().GetField("m_AllInspectors", (System.Reflection.BindingFlags)(-1));
			}
			if (EditorWindow.focusedWindow) EditorWindow.focusedWindow.Repaint();

			var wrapp = ins.GetValue(null);
			//  var i = new System.Collections.ArrayList().ToArray();
			//var i = ins.GetValue(null) as IReadOnlyList<EditorWindow>;
			foreach (var item in wrapp as System.Collections.IList) if ((EditorWindow)item) ((EditorWindow)item).Repaint();
		}



		static void EditorApplication_UPDATESNAPPING()
		{
			was = false;
			//sceneKeyCode = KeyCode.None;
			var any = SNAP_POS.ENABLE || SNAP_ROT.ENABLE || SNAP_SCALE.ENABLE || IsSnapInverted() && !SNAP_POS.ENABLE && !SNAP_ROT.ENABLE && !SNAP_SCALE.ENABLE;
			if (!any || !SNAP_AUTOAPPLY && !(bool)dragActive.GetValue(null, null)) return;

			foreach (var t in Selection.GetTransforms(flags))
			{
				PosSnappingUpdater(t);
				RotSnappingUpdater(t);
				ScaleSnappingUpdater(t);
			}
		}

		internal static bool IsSnapInverted()
		{
			if (!SnapMod.SNAP_FASTINVERT_USEHOTKEYS) return false;
			var inv = (int)SnapMod.SNAP_FASTINVERT_HOTKEYS;
			var I_KEY = inv & 0xFFFF;
			var ICTRL = (inv & (1 << 16)) != 0;
			var ISHIFT = (inv & (1 << 17)) != 0;
			var IALT = (inv & (1 << 18)) != 0;
			// var res = (KeyCode)I_KEY == Event.current.keyCode;
			var res = false;
			foreach (var item in SnapMod.sceneKeyCode)
			{
				if (!res)
				{
					res = (KeyCode)I_KEY == item.Value.keyCode;
					res &= ICTRL == item.Value.ctrl;
					res &= ISHIFT == item.Value.shift;
					res &= IALT == item.Value.alt;
				}
			}

			return res;
			// Debug.Log((KeyCode)Event.current.keyCode  + " " + Event.current.control + " " + Event.current.shift + " " + Event.current.alt);
			//  return SnapMod.IsSnapInverted = Event.current.shift;
		}
		static void PosSnappingUpdater(Transform t)
		{
			var en = (bool)SNAP_POS.ENABLE;
			if (IsSnapInverted()) en = !en;
			if (!en) return;
			v = SNAP_POS.USE_LOCAL ? t.localPosition : t.position;
			if (DO_SNAPACTION(t, SNAP_POS, ref POS_PREFKEYS, 1))
			{
				if (SNAP_POS.USE_LOCAL) t.localPosition = v;
				else t.position = v;
				SET_DIRTY(t);
			}
		}
		static void RotSnappingUpdater(Transform t)
		{
			var en = (bool)SNAP_ROT.ENABLE;
			if (IsSnapInverted()) en = !en;
			if (!en) return;

			v = SNAP_ROT.USE_LOCAL ? t.localRotation.eulerAngles : t.rotation.eulerAngles;
			if (DO_SNAPACTION(t, SNAP_ROT, ref ROT_PREFKEYS, 1))
			{
				if (SNAP_ROT.USE_LOCAL) t.localRotation = Quaternion.Euler(v);
				else t.rotation = Quaternion.Euler(v);
				SET_DIRTY(t);
			}
		}
		static void ScaleSnappingUpdater(Transform t)
		{
			var en = (bool)SNAP_SCALE.ENABLE;
			if (IsSnapInverted()) en = !en;
			if (!en) return;
			v = t.localScale;
			if (DO_SNAPACTION(t, SNAP_SCALE, ref SCALE_PREFKEYS, 1))
			{
				t.localScale = v;
				SET_DIRTY(t);
			}
		}


		static bool DO_SNAPACTION(Transform t, VectorPref pref, ref PRF[] prefKeys, int undoTextIndex)
		{
			p = v;

			if (pref.X) v.x = (float)Math.Round(v.x / EditorPrefs.GetFloat(prefKeys[0].key, prefKeys[0].def), 0) * EditorPrefs.GetFloat(prefKeys[0].key, prefKeys[0].def);
			if (pref.Y) v.y = (float)Math.Round(v.y / EditorPrefs.GetFloat(prefKeys[1].key, prefKeys[1].def), 0) * EditorPrefs.GetFloat(prefKeys[1].key, prefKeys[1].def);
			if (pref.Z) v.z = (float)Math.Round(v.z / EditorPrefs.GetFloat(prefKeys[2].key, prefKeys[2].def), 0) * EditorPrefs.GetFloat(prefKeys[2].key, prefKeys[2].def);

			if (float.IsNaN(v.x)) v.x = p.x;
			if (float.IsNaN(v.y)) v.y = p.y;
			if (float.IsNaN(v.z)) v.z = p.z;

			b = p != v;
			if (b) Undo.RecordObject(t, UNDO_TEXT[undoTextIndex]);
			return b;
		}



        static SelectionMode flags {
            get {
                return
                    SelectionMode.TopLevel |
#if UNITY_2019_1_OR_NEWER
SelectionMode.Editable
#else
SelectionMode.OnlyUserModifiable
#endif
;
            }
        }


        static bool was = false;
		// static Vector2 mouseTunning;
		static Vector2[] mouseTunning;
		private static void SceneView_PLACEONSURFACE(SceneView sceneView)
		{
			if (!sceneView) return;
			/*
            if (Event.current.type == EventType.KeyDown && Event.current.control && Event.current.shift)
            {
                if (Event.current.keyCode == KeyCode.L) PLACE_ON_SURFACE_ENABLE.Set(!PLACE_ON_SURFACE_ENABLE);
                if (Event.current.keyCode == KeyCode.K) SNAP_POS.ENABLE(w).Set(!SNAP_POS.ENABLE);
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
            */
			if (!PLACE_ON_SURFACE_ENABLE || UnityEditor.Tools.current == Tool.Rotate ||
#if UNITY_2017_3_OR_NEWER
				UnityEditor.Tools.current == Tool.Transform ||
#endif
				UnityEditor.Tools.current == Tool.Rect) return;

			if (Event.current.type == EventType.MouseDown && (Selection.GetTransforms(flags).Length > 0))
			{   /* if ( PLACE_ON_SURFACE_ALIGNBYMOUSE )
                         mouseTunning = new[] { (HandleUtility.WorldToGUIPoint( UnityEditor.Tools.handlePosition ) - Event.current.mousePosition) };
                     else*/
				mouseTunning = Selection.GetTransforms(flags).Select(t => HandleUtility.WorldToGUIPoint(t.position) - Event.current.mousePosition).ToArray();
			}

			if (Event.current.rawType == EventType.Repaint) was = false;


			if (!(bool)dragActive.GetValue(null, null) && !was) return;

			was = true;
			var selection = Selection.GetTransforms(flags);
			var excludeList = CreateExcludeList(selection);


			for (int i = 0; i < selection.Length; i++)
			{
				var t = selection[i];

				PosSnappingUpdater(t);

				p = t.position;
				/* var ray = PLACE_ON_SURFACE_ALIGNBYMOUSE ?
                               HandleUtility.GUIPointToWorldRay( Event.current.mousePosition + mouseTunning[Math.Min( mouseTunning.Length - 1, i )] ) :
                               HandleUtility.GUIPointToWorldRay( HandleUtility.WorldToGUIPoint( p ) );*/
				var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition + mouseTunning[Math.Min(mouseTunning.Length - 1, i)]);

				SNAP_VARIANT_2(t, ray, excludeList);

				t.position = v;
				SET_DIRTY(t);


			}

			EditorApplication_UPDATESNAPPING();

		}


		static public Vector3 ClosestPointOnPlane(Plane plane, Vector3 point)
		{
			float num = Vector3.Dot(plane.normal, point) + plane.distance;
			return point - plane.normal * num;
		}

		static void SNAP_VARIANT_2(Transform t, Ray ray, Dictionary<int, int> excludeList)
		{
			object obj = null;
			float mindist = float.MaxValue;
			foreach (var hit in Physics.RaycastAll(ray))
			{
				if (hit.collider == null || hit.collider.isTrigger || excludeList.ContainsKey(hit.collider.transform.GetInstanceID())) continue;
				if (Vector3.SqrMagnitude(hit.point - ray.origin) > mindist && Vector3.SqrMagnitude(hit.point - ray.origin) != float.PositiveInfinity) continue;
				mindist = Vector3.SqrMagnitude(hit.point - ray.origin);
				obj = hit;
			}

			if (obj != null)
			{
				Undo.RecordObject(t, UNDO_TEXT[0]);
				RaycastHit raycastHit = (RaycastHit)obj;
				v = raycastHit.point;
				//Debug.Log(raycastHit.collider.gameObject.name);

				t.position = v;


				if (CALCULATE_OBJECT_BOUNDS && CalcBoundsForGameObjectHierarchy(t.gameObject))
				{
					var plane = new Plane(raycastHit.normal, raycastHit.point);
					var offset = 0f;

					for (int i = 0; i < boundsVertices.Length; i++)
					{

#if !UNITY_2017_1_OR_NEWER
                        var projection = ClosestPointOnPlane (plane, boundsVertices[i] );
#else
						var projection = plane.ClosestPointOnPlane(boundsVertices[i]);
#endif
						var difference = projection - boundsVertices[i];

						if (Vector2.Dot(difference.normalized, raycastHit.normal) > 0)
						{
							if (difference.sqrMagnitude > offset) offset = difference.sqrMagnitude;
						}
					}

					if (offset != 0)
					{
						offset = (float)Math.Sqrt(offset);
						//Debug.Log(offset);
						v += raycastHit.normal * offset;
					}
				}

				if (ALIGN_BY_NORMAL)
				{
					Undo.RecordObject(t, UNDO_TEXT[1]);
					switch (ALIGN_UP_VECTOR)
					{
						case 0: t.up = raycastHit.normal; break;
						case 1: t.forward = raycastHit.normal; break;
						case 2: t.up = -raycastHit.normal; break;
						case 3: t.right = raycastHit.normal; break;
						case 4: t.right = -raycastHit.normal; break;
						case 5: t.forward = -raycastHit.normal; break;
					}
					SET_DIRTY(t);
				}
			}
			else
			{
				v = t.position;
			}
		}


		static Dictionary<int, int> CreateExcludeList(Transform[] t)
		{
			var result = new Dictionary<int, int>();
			foreach (var transform in t)
			{
				foreach (var componentsInChild in transform.GetComponentsInChildren<Transform>())
					if (!result.ContainsKey(componentsInChild.GetInstanceID())) result.Add(componentsInChild.GetInstanceID(), 0);
				if (!result.ContainsKey(transform.GetInstanceID())) result.Add(transform.GetInstanceID(), 0);
			}
			return result;
		}

		static Vector3[] MIN_MAX = new Vector3[2];
		static Vector3[] boundsVertices = new Vector3[8];

		static Vector3 min_static = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		static Vector3 max_static = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		// BASED ON MESH VERTICES
		static bool CalcBoundsForGameObjectHierarchy(GameObject go)
		{

			MIN_MAX[0] = min_static;
			MIN_MAX[1] = max_static;
			bool skip = true;
			//var t = go.transform;
			var m = go.GetComponentsInChildren<MeshFilter>().ToList();
			m.Add(go.GetComponent<MeshFilter>());
			foreach (var mf in m)
			{
				if (!mf) continue;
				if (!mf.sharedMesh || !mf.GetComponent<MeshRenderer>() || !mf.GetComponent<MeshRenderer>().enabled) continue;
				var vertices = mf.sharedMesh.vertices;
				for (int i = 0; i < vertices.Length; i++)
				{
					if (skip) skip = false;
					var v = mf.transform.TransformPoint(vertices[i]);
					for (int DIR = 0; DIR < 3; DIR++)
					{
						if (v[DIR] < MIN_MAX[0][DIR]) MIN_MAX[0][DIR] = v[DIR];
						if (v[DIR] > MIN_MAX[1][DIR]) MIN_MAX[1][DIR] = v[DIR];
					}
				}
			}
			if (skip) MIN_MAX[0] = MIN_MAX[1] = Vector3.zero;

			var result = MIN_MAX[0] != Vector3.zero || MIN_MAX[1] != Vector3.zero;

			//MIN_MAX[0] = .(MIN_MAX[0]);
			//MIN_MAX[1] = go.transform.TransformPoint(MIN_MAX[1]);

			for (int i = 0; i < 8; i++) boundsVertices[i].Set(MIN_MAX[(i % 2) / 1].x, MIN_MAX[(i % 4) / 2].y, MIN_MAX[(i % 8) / 4].z);

			return result;

		}

		// BASED ON GAMEOBJECT BOUNDS
		/*    static bool CalcBoundsForGameObjectHierarchy( GameObject go )
                {

                    Bounds bounds = new Bounds( );
                    bounds.center = go.transform.position;
                    bounds.extents = Vector3.zero;

                    if ( go.GetComponent<Renderer>( ) )
                    {
                        bounds = go.GetComponent<Renderer>( ).bounds;
                    }

                    foreach ( Renderer renderer in go.GetComponentsInChildren<Renderer>( ) )
                    {
                        if ( !renderer ) continue;
                        bounds.Encapsulate( renderer.bounds );
                    }

                    MIN_MAX[0] = (bounds.min);
                    MIN_MAX[1] = (bounds.max);
                    for ( int i = 0; i < 8; i++ ) boundsVertices[i].Set( MIN_MAX[(i % 2) / 1].x, MIN_MAX[(i % 4) / 2].y, MIN_MAX[(i % 8) / 4].z );

                    return bounds.extents != Vector3.zero;

                }*/



	}









	public partial class SnapButtonsForInspectorGUI : UnityEditor.Editor
	{
		//params
		bool DRAW_SURFACE_BUTTON = true;


		////////
		//////// INSPECTOR DECORATOR ////////
		////////

		// inspector vars
		static Type decoratedEditorType;
		UnityEditor.Editor EDITOR_INSTANCE;
		internal void OnDisable() { if (EDITOR_INSTANCE) DestroyImmediate(EDITOR_INSTANCE); }



		public override void OnInspectorGUI()
		{
			if (targets == null || targets.Length == 0)
			{
				// base.DrawDefaultInspector();
				return;
			}
			if (Root.p[0] == null || Root.p[0].par_e == null || !Root.p[0].par_e.USE_SNAP_MOD)
			{
				SnapMod.SET_ENABLE(false);
				return;
			}
			if (decoratedEditorType == null) decoratedEditorType = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.TransformInspector", true);
			if (!EDITOR_INSTANCE) EDITOR_INSTANCE = UnityEditor.Editor.CreateEditor(targets, decoratedEditorType);
			if (buttonStyle == null || !snapContent.image) InitStyles();


			var start_y = EditorGUILayout.GetControlRect(GUILayout.Height(0), GUILayout.ExpandHeight(false)).y;
			var window_width = GUILayoutUtility.GetLastRect().x + GUILayoutUtility.GetLastRect().width;

			//////// BUTTONS ////////
			CUSTOM_GUI(start_y, window_width);

			// RIGHT PADDING
			GUILayout.BeginHorizontal(); GUILayout.BeginVertical();

			//////// INTERNAL GUI ////////
			EDITOR_INSTANCE.OnInspectorGUI();

			// RIGHT PADDING
			GUILayout.EndVertical(); GUILayout.Space(buttonRectWidth); GUILayout.EndHorizontal();

			/*  int controlID = GUIUtility.GetControlID(FocusType.Passive);
              if (Event.current.Equals(Event.KeyboardEvent("W")))
              {   Debug.Log(Event.current.GetTypeForControl(controlID));
              }
              if (Input.GetKey(KeyCode.C))
              {   Debug.Log(Event.current.GetTypeForControl(controlID));
              }
              if (Event.current.type == EventType.Repaint)
              {   var os = SnapMod.sceneKeyCode;
                  SnapMod.sceneKeyCode = KeyCode.None;
                  // if (os != KeyCode.None ) SnapMod. modifierKeysChanged_KEYS();
              }*/
		}







		////////
		//////// GUI BUTTONS DRAWING ////////
		////////

		// buttons vars
		const float BUT_SIZE = 16;

		float buttonRectWidth { get { return (DRAW_SURFACE_BUTTON ? BUT_SIZE * 2 : BUT_SIZE) - 3; } }
		Color enableColor = new Color(0.9f, 0.6f, 0.3f, 1f);
		GUIStyle buttonStyle;
		Rect r = new Rect();
		GUIContent snapContent = new GUIContent("", "- Click to enable/disable snap\n- Right-Click to select axises\n- Use CTRL to use Unity internal snapping");
		GUIContent raycastContent = new GUIContent("", "- Click to enable/disable surface placement raycast\n- Right-Click to select align mode\n- Use CTRL+SHIFT to use Unity surface snapping");
		GUIContent normalContent = new GUIContent("", "- Align Object by surface normal if used surface placement raycast\n- Right-Click to choose up Vector");

		void InitStyles()
		{


			snapContent.image = Root.p[0].GetOldIcon("SNAP_MAGN").texture;
			raycastContent.image = Root.p[0].GetOldIcon("SNAP_SURF").texture;
			normalContent.image = Root.p[0].GetOldIcon("SNAP_NORM").texture;


			buttonStyle = new GUIStyle(GUI.skin.button);
			buttonStyle.padding = new RectOffset(3, 3, 3, 3);
		}

		bool IsSnapInverted()
		{
			if (!SnapMod.SNAP_FASTINVERT_USEHOTKEYS) return false;
			if (!SnapMod.sceneKeyCode.ContainsKey(GUIUtility.keyboardControl)) SnapMod.sceneKeyCode.Add(GUIUtility.keyboardControl, new SnapMod.Keys());
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None)
			{
				SnapMod.sceneKeyCode[GUIUtility.keyboardControl] = new SnapMod.Keys(Event.current.keyCode, Event.current.control, Event.current.shift, Event.current.alt);
			}
			if (Event.current.type == EventType.KeyUp)
			{
				SnapMod.sceneKeyCode[GUIUtility.keyboardControl] = new SnapMod.Keys();
			}
			return SnapMod.IsSnapInverted();
		}

		void CUSTOM_GUI(float start_y, float window_width)
		{
			r.x = window_width - buttonRectWidth;
			r.y = start_y + 1;
			r.width = BUT_SIZE;
			r.height = BUT_SIZE;



			DO_SNAP_BUTTON(r, SnapMod.SNAP_POS);
			if (DRAW_SURFACE_BUTTON) DO_SURFACE_BUTTON(new Rect(r.x + BUT_SIZE, r.y, r.width, r.height));
			r.y += r.height + 2;
			DO_SNAP_BUTTON(r, SnapMod.SNAP_ROT);
			if (DRAW_SURFACE_BUTTON) DO_NORMAL_BUTTON(new Rect(r.x + BUT_SIZE, r.y, r.width, r.height));
			r.y += r.height + 2;
			DO_SNAP_BUTTON(r, SnapMod.SNAP_SCALE);

		}

		void DO_SNAP_BUTTON(Rect r, VectorPref pref)
		{
			if (GUI.Button(r, snapContent, buttonStyle))
			{
				if (Event.current.button == 0) pref.ENABLE.Set(!pref.ENABLE);
				else
				{
					var menu = new GenericMenu();
					menu.AddItem(new GUIContent(pref.ENABLE.name), pref.ENABLE, () => pref.X.Set(!pref.X));
					menu.AddSeparator("");
					menu.AddItem(new GUIContent(SnapMod.SNAP_FASTINVERT_USEHOTKEYS.name), SnapMod.SNAP_FASTINVERT_USEHOTKEYS, () => SnapMod.SNAP_FASTINVERT_USEHOTKEYS.Set(!SnapMod.SNAP_FASTINVERT_USEHOTKEYS));
					menu.AddSeparator("");
					menu.AddItem(new GUIContent(SnapMod.SNAP_AUTOAPPLY.name), SnapMod.SNAP_AUTOAPPLY, () => SnapMod.SNAP_AUTOAPPLY.Set(!SnapMod.SNAP_AUTOAPPLY));
					menu.AddSeparator("");
					menu.AddItem(new GUIContent(pref.X.name), pref.X, () => pref.X.Set(!pref.X));
					menu.AddItem(new GUIContent(pref.Y.name), pref.Y, () => pref.Y.Set(!pref.Y));
					menu.AddItem(new GUIContent(pref.Z.name), pref.Z, () => pref.Z.Set(!pref.Z));
					menu.AddSeparator("");
					menu.AddItem(new GUIContent(pref.USE_LOCAL.name), pref.USE_LOCAL, () => pref.USE_LOCAL.Set(!pref.USE_LOCAL));
					menu.AddSeparator("");
					menu.AddItem(new GUIContent("Open unity snap settings"), false, () =>
#if UNITY_2019_3_OR_NEWER
					EditorApplication.ExecuteMenuItem( "Edit/Grid and Snap Settings..." )
#else
					EditorApplication.ExecuteMenuItem("Edit/Snap Settings...")
#endif
					);
					menu.AddItem(new GUIContent("Open plugin snap settings"), false, () => { Settings.MainSettingsEnabler_Window.Select<Settings.ST_Window>(); });
					menu.ShowAsContext();
				}
			}
			var en = (bool)pref.ENABLE;
			if (IsSnapInverted()) en = !en;
			if (Event.current.type == EventType.Repaint && en)
			{
				var oldColor = GUI.color;
				GUI.color *= enableColor;
				buttonStyle.Draw(r, snapContent, true, true, false, true);
				GUI.color = oldColor;
			}
		}

		void DO_SURFACE_BUTTON(Rect r)
		{
			var on = GUI.enabled;
			GUI.enabled &= UnityEditor.Tools.current != Tool.Rotate &&
#if UNITY_2017_3_OR_NEWER
					   UnityEditor.Tools.current != Tool.Transform &&
#endif
					   UnityEditor.Tools.current != Tool.Rect;
			if (GUI.Button(r, raycastContent, buttonStyle))
			{
				if (Event.current.button == 0)
				{
					SnapMod.PLACE_ON_SURFACE_ENABLE.Set(!SnapMod.PLACE_ON_SURFACE_ENABLE);
				}
				else
				{
					var menu = new GenericMenu();
					menu.AddItem(new GUIContent(SnapMod.PLACE_ON_SURFACE_ENABLE.name), SnapMod.PLACE_ON_SURFACE_ENABLE, () => SnapMod.PLACE_ON_SURFACE_ENABLE.Set(!SnapMod.PLACE_ON_SURFACE_ENABLE));
					menu.AddSeparator("");
					menu.AddItem(new GUIContent(SnapMod.ALIGN_BY[0]), SnapMod.PLACE_ON_SURFACE_ALIGNBYMOUSE, () => SnapMod.PLACE_ON_SURFACE_ALIGNBYMOUSE.Set(true));
					// menu.AddItem( new GUIContent( SnapMod.ALIGN_BY[1] ), !SnapMod.PLACE_ON_SURFACE_ALIGNBYMOUSE, () => SnapMod.PLACE_ON_SURFACE_ALIGNBYMOUSE.Set( false ) );
					menu.AddDisabledItem(new GUIContent(SnapMod.ALIGN_BY[1]));
					menu.AddSeparator("");
					menu.AddItem(new GUIContent(SnapMod.CALCULATE_OBJECT_BOUNDS.name), SnapMod.CALCULATE_OBJECT_BOUNDS, () => SnapMod.CALCULATE_OBJECT_BOUNDS.Set(!SnapMod.CALCULATE_OBJECT_BOUNDS));
					menu.AddSeparator("");
					menu.AddItem(new GUIContent("Open plugin snap settings"), false, () => { Settings.MainSettingsEnabler_Window.Select<Settings.ST_Window>(); });
					menu.ShowAsContext();
				}
			}
			if (GUI.enabled && Event.current.type == EventType.Repaint && SnapMod.PLACE_ON_SURFACE_ENABLE)
			{
				var oldColor = GUI.color;
				GUI.color *= enableColor;
				buttonStyle.Draw(r, raycastContent, true, true, false, true);
				GUI.color = oldColor;
			}
			GUI.enabled = on;
		}

		void DO_NORMAL_BUTTON(Rect r)
		{
			var en = GUI.enabled;
			var on = GUI.enabled;
			GUI.enabled = SnapMod.PLACE_ON_SURFACE_ENABLE && UnityEditor.Tools.current != Tool.Rotate &&
#if UNITY_2017_3_OR_NEWER
					  UnityEditor.Tools.current != Tool.Transform &&
#endif
					  UnityEditor.Tools.current != Tool.Rect;
			if (GUI.Button(r, normalContent, buttonStyle))
			{
				if (Event.current.button == 0)
				{
					SnapMod.ALIGN_BY_NORMAL.Set(!SnapMod.ALIGN_BY_NORMAL);
				}
				else
				{
					var menu = new GenericMenu();
					menu.AddItem(new GUIContent(SnapMod.ALIGN_BY_NORMAL.name), SnapMod.ALIGN_BY_NORMAL, () => SnapMod.ALIGN_BY_NORMAL.Set(!SnapMod.ALIGN_BY_NORMAL));
					menu.AddSeparator("");
					for (int i = 0; i < SnapMod.VECTORS.Length; i++)
					{
						var captureI = i;
						menu.AddItem(new GUIContent(SnapMod.VECTORS_STRING[i]), SnapMod.ALIGN_UP_VECTOR == i, () => SnapMod.ALIGN_UP_VECTOR.Set(captureI));
					}
					menu.AddSeparator("");
					menu.AddItem(new GUIContent("Open plugin snap settings"), false, () => { Settings.MainSettingsEnabler_Window.Select<Settings.ST_Window>(); });
					menu.ShowAsContext();
				}
			}
			if (GUI.enabled && Event.current.type == EventType.Repaint && SnapMod.ALIGN_BY_NORMAL)
			{
				var oldColor = GUI.color;
				GUI.color *= enableColor;
				buttonStyle.Draw(r, normalContent, true, true, false, true);
				GUI.color = oldColor;
			}
			GUI.enabled = en;
		}


	}








	////////
	//////// PREFS CLASSES ////////
	////////

	class VectorPref
	{
		public CachedPref ENABLE;
		public CachedPref X;
		public CachedPref Y;
		public CachedPref Z;
		public CachedPref USE_LOCAL;

		public VectorPref(string keyPrefix, bool defaultUseLocalValue = false, bool lockUseLocalValue = false, string enableName = null)
		{
			ENABLE = new CachedPref(keyPrefix + "_ENABLE", false) { name = enableName ?? "Enable" };
			X = new CachedPref(keyPrefix + "_X", true) { name = "Snap x-axis" };
			Y = new CachedPref(keyPrefix + "_Y", true) { name = "Snap y-axis" };
			Z = new CachedPref(keyPrefix + "_Z", true) { name = "Snap z-axis" };

			USE_LOCAL = new CachedPref(keyPrefix + "_USE_LOCAL", defaultUseLocalValue, lockUseLocalValue) { name = "Use local space" };
		}
	}

	class CachedPref
	{
		public CachedPref(string registryKey, bool defaulValue, bool lockValue = false)
		{
			this.m_registryKey = registryKey;
			this.m_boolDefaultValue = defaulValue;
			this.m_lock = lockValue;
		}

		public CachedPref(string registryKey, int defaulValue, bool lockValue = false)
		{
			this.m_registryKey = registryKey;
			this.m_intDefaultValue = defaulValue;
			this.m_lock = lockValue;
		}

		public static implicit operator bool(CachedPref d) { return d.m_boolValue; }
		public void Set(bool value)
		{
			if (m_lock) return;
			m_boolValue = value;
		}

		public static implicit operator int(CachedPref d) { return d.m_intValue; }
		public void Set(int value)
		{
			if (m_lock) return;
			m_intValue = value;
		}


		public string name;

		string m_registryKey;
		bool m_lock;

		bool m_boolDefaultValue;
		bool? m_boolCache;
		bool m_boolValue
		{
			get { return m_boolCache ?? (m_boolCache = Root.p[0].par_e.GET(m_registryKey, m_boolDefaultValue)).Value; }
			set {
				if ( m_boolValue == value ) return;
				Root.p[0].par_e.SET(m_registryKey, (m_boolCache = value).Value); }
		}

		int m_intDefaultValue;
		int? m_intCache;
		int m_intValue
		{
			get { return m_intCache ?? (m_intCache = Root.p[0].par_e.GET(m_registryKey, m_intDefaultValue)).Value; }
			set {
                if ( m_intValue == value ) return;
                Root.p[0].par_e.SET(m_registryKey, (m_intCache = value).Value); }
		}
	}
}




