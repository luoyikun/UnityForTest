using UnityEngine;
using UnityEditor;



namespace EMX.HierarchyPlugin.Editor.Mods
{
	partial class ExternalModStyles
	{


		static private PluginInstance adapter { get { return Root.p[0]; } }


		internal static GUIContent FoldContent = new GUIContent() { tooltip = "Minimize The Dock" };

		// internal GUIContent plusContentLabel = new GUIContent() { tooltip = "Add GameObject" };
		internal static GUIContent plusContentLabel = new GUIContent() { tooltip = "Create a snapshot of objects that have been expanded" };
		internal static GUIContent plusContentSceneLabel = new GUIContent() { tooltip = "Create a multi-scenes buttons from open scenes" };

		internal static GUIContent plusContent = new GUIContent() { text = "+" };
		internal static GUIContent hierCollapce = new GUIContent() { text = "≡" }; //-
		internal static GUIContent hierCollapceLabel = new GUIContent() { tooltip = "Collapse all expanded items" };

		//internal static GUIContent ContentSelBackLabel = null;
		//private GUIContent ContentSelForwLabel = null;

		internal static GUIContent ContentSelBackLabel = new GUIContent() { tooltip = "Selection Backward (Use Menu or HotKey)" };
		internal static GUIContent ContentSelForwLabel = new GUIContent() { tooltip = "Selection Forward (Use Menu or HotKey)" };



		internal static GUIContent ContentSelBack = new GUIContent() { text = "◄" }; //
		internal static GUIContent ContentSelForw = new GUIContent() { text = "►" };



		protected static GUIContent ContentHiperempty = new GUIContent() { };
		protected static GUIContent ContentHiperUndo = new GUIContent() { tooltip = "Previous HyperGraph Selection" };
		protected static GUIContent ContenHiperRedo = new GUIContent() { tooltip = "Forward HyperGraph Selection" };
		protected static GUIContent ContenHiperRefresh = new GUIContent() { tooltip = "Refresh HyperGraph Connections" };


		static internal Color dragColor = new Color(0.2f, 0.5f, 0.8f, 0.1f);

		static int _LINE_HEIGTH { get { return 20; } }
		internal static int LINE_HEIGHT_FOR_BUTTONS(float currentH) { return Mathf.Min(50, Mathf.RoundToInt(Mathf.Lerp(_LINE_HEIGTH, currentH, (currentH - _LINE_HEIGTH) / _LINE_HEIGTH/ _LINE_HEIGTH / _LINE_HEIGTH))); }


		public static readonly GUIContent HyperGraphClose_Content = new GUIContent() { tooltip = "Close" };
		public static readonly GUIContent HyperGraphWindow_Content = new GUIContent() { tooltip = "Open Dockable Window" };



		protected static GUIStyle HIPERUI_BUTTONGLOW;
		protected static Texture2D ZOOM_MINUS;
		protected static Texture2D ZOOM_PLUS;
		protected static Texture2D ZOOM_THUMB;
		protected static Texture2D HIPERUI_CLOSE;
		protected static Texture2D HIPERGRAPH_DOCK;
		protected static Texture2D GRID;
		protected static Texture2D REFRESH_TEXTURE;
		protected static GUIStyle m_HIPERUI_GAMEOBJECT;
		protected static GUIStyle m_HIPERUI_INOUT_A;
		protected static GUIStyle m_HIPERUI_INOUT_B;
		protected static GUIStyle m_HIPERUI_LINE_BLUEGB;
		protected static GUIStyle m_HIPERUI_LINE_BLUEGB_PERSONAL;
		protected static GUIStyle m_HIPERUI_LINE_BOX;
		protected static GUIStyle m_HIPERUI_LINE_RDTRIANGLE;
		protected static GUIStyle m_HIPERUI_LINE_RDTRIANGLE_INVERSE;
		protected static GUIStyle m_HIPERUI_MARKER_BOX;



#pragma warning disable
		//static GUIStyle ARROW;
#pragma warning restore
		static StyleInitHelper STYLES_WAS_INIT = false;
		//?FAV
		protected static GUIStyle GET_DRAG_BOX;
		protected static GUIStyle GET_DRAG_LINE;
		protected static GUIStyle shadow;
		protected static Texture2D FAVORIT_FILTER_ICON;
		protected static Texture2D FAVORIT_FOLDERS_ICON_OFF;
		protected static Texture2D FAVORIT_FOLDERS_ICON;
		protected static Texture2D FAVORIT_LIST_ICON_ON;
		protected static Texture2D FAVORIT_LIST_ICON;
		protected static Texture2D STAR;
		protected static Texture2D SEPARATOR;


		static protected void INIT_STYLES()
		{
			if (STYLES_WAS_INIT) return;


			//ARROW = adapter.InitializeStyle("ARROW", 0.4f, 0.4f, 0, 0, externalPMod: true);


			m_HIPERUI_GAMEOBJECT = adapter.InitializeStyle("HIPERUI_GAMEOBJECT", 0f, 0f, 0, 0, externalPMod: true);
			m_HIPERUI_GAMEOBJECT.fontStyle = FontStyle.Bold;
			//HIPERUI_GAMEOBJECT.padding.left = 16;

			// HIPERUI_GAMEOBJECT.border.left = 30;
			m_HIPERUI_GAMEOBJECT.focused.textColor = m_HIPERUI_GAMEOBJECT.active.textColor =
				m_HIPERUI_GAMEOBJECT.hover.textColor =
					m_HIPERUI_GAMEOBJECT.normal.textColor = new Color32(42, 42, 42, 255);


			m_HIPERUI_LINE_RDTRIANGLE =
				adapter.InitializeStyle("HIPERUI_LINE_RDTRIANGLE", 0.25f, 0.5f, 0.25f, 0.7f, externalPMod: true);

			HIPERUI_BUTTONGLOW = adapter.InitializeStyle("HIPERUI_BUTTONGLOW", 0.25f, 0.25f, 0.25f, 0.25f, externalPMod: true);
			ZOOM_MINUS = adapter.GetExternalModOld("ZOOM_MINUS");
			ZOOM_PLUS = adapter.GetExternalModOld("ZOOM_PLUS");
			ZOOM_THUMB = adapter.GetExternalModOld("ZOOM_THUMB");
			HIPERUI_CLOSE = adapter.GetExternalModOld("HIPERUI_CLOSE");
			HIPERGRAPH_DOCK = adapter.GetExternalModOld("HIPERGRAPH_DOCK");
			REFRESH_TEXTURE = adapter.GetExternalModOld("REFRESH");
			GRID = adapter.GetExternalModOld(EditorGUIUtility.isProSkin ? "GRID" : "GRID_PERSONAL");
			m_HIPERUI_INOUT_A = adapter.InitializeStyle("HIPERUI_INOUT_A", 0, 0, 0, 0, externalPMod: true);
			m_HIPERUI_INOUT_B = adapter.InitializeStyle("HIPERUI_INOUT_B", 0, 0, 0, 0, externalPMod: true);

			m_HIPERUI_LINE_BLUEGB = adapter.InitializeStyle("HIPERUI_LINE_BLUEGB", 0.25f, 0.25f, 0, 0, externalPMod: true);
			m_HIPERUI_LINE_BLUEGB.alignment = TextAnchor.MiddleRight;
			m_HIPERUI_LINE_BLUEGB.padding.right = 4;

			m_HIPERUI_LINE_BLUEGB_PERSONAL = adapter.InitializeStyle("HIPERUI_LINE_BLUEGB" + " PERSONAL", 0.25f, 0.25f, 0, 0, externalPMod: true);
			m_HIPERUI_LINE_BLUEGB_PERSONAL.alignment = TextAnchor.MiddleRight;
			m_HIPERUI_LINE_BLUEGB_PERSONAL.padding.right = 4;
			m_HIPERUI_LINE_BLUEGB_PERSONAL.normal.textColor = new Color32(40, 40, 40, 255);

			m_HIPERUI_LINE_BOX = adapter.InitializeStyle("HIPERUI_LINE_BOX", 0.4f, 0.4f, 0.4f, 0.4f, externalPMod: true);
			m_HIPERUI_MARKER_BOX = adapter.InitializeStyle("HIPERUI_MARKER_BOX", 0.05f, 0.05f, 0.05f, 0.05f, externalPMod: true);

			//FAV
			//GET_DRAG_BOX = adapter.InitializeStyle("DRAG_BOX", 7, 7, 7, 7, externalPMod: true);
			GET_DRAG_BOX = new GUIStyle();
			GET_DRAG_BOX.normal.background = adapter.GetExternalModOld("DRAG_BOX");
			GET_DRAG_BOX.border = new RectOffset(7, 7, 7, 7);
			GET_DRAG_LINE = new GUIStyle();
			GET_DRAG_LINE.normal.background = adapter.GetExternalModOld("DRAG_LINE");
			GET_DRAG_LINE.border = new RectOffset(6, 0, 6, 0);

			FAVORIT_FOLDERS_ICON = adapter.GetExternalModOld("FAVORIT_FOLDERS_ICON");
			FAVORIT_FOLDERS_ICON_OFF = adapter.GetExternalModOld("FAVORIT_FOLDERS_ICON OFF");
			FAVORIT_FILTER_ICON = adapter.GetExternalModOld("FAVORIT_FILTER_ICON");
			FAVORIT_LIST_ICON = adapter.GetExternalModOld("FAVORIT_LIST_ICON");
			FAVORIT_LIST_ICON_ON = adapter.GetExternalModOld("FAVORIT_LIST_ICON ON");
			STAR = adapter.GetExternalModOld("STAR");
			SEPARATOR = adapter.GetExternalModOld("SEPARATOR");
			shadow = adapter.InitializeStyle("SHADOW", 0.25f, 0.25f, 0.25f, 0.25f);


			STYLES_WAS_INIT = true;
		}



		protected static Color BLUE = new Color(0.05f, 0.6f, 0.9f);

		internal void Label(Rect r, string s, TextAnchor an)
		{
			var a = adapter.label.alignment;
			adapter.label.alignment = an;
			c.text = c.tooltip = s;
			GUI.Label(r, c, adapter.label);
			adapter.label.alignment = a;
		}

		GUIContent c = new GUIContent();
		internal void Label(Rect r, string s)
		{
			c.text = c.tooltip = s;
			GUI.Label(r, c, adapter.label);
		}

		internal void Label(Rect r, GUIContent s)
		{
			GUI.Label(r, s, adapter.label);
		}

		internal bool Button(Rect r, string s)
		{
			c.text = c.tooltip = s;
			return GUI.Button(r, c, adapter.button);
		}

		internal bool Button(Rect r, string s, TextAnchor an)
		{
			c.text = c.tooltip = s;
			var a = adapter.button.alignment;
			adapter.button.alignment = an;
			var res = GUI.Button(r, c, adapter.button);
			adapter.button.alignment = a;
			return res;
		}

		internal bool Button(Rect r, GUIContent s)
		{
			return GUI.Button(r, s, adapter.button);
		}

		internal bool Button(Rect r, GUIContent s, TextAnchor an)
		{
			var a = adapter.button.alignment;
			adapter.button.alignment = an;
			var res = GUI.Button(r, s, adapter.button);
			adapter.button.alignment = a;
			return res;
		}

		GUIContent tootipContent = new GUIContent();

		protected void TOOLTIP(Rect r, string content)
		{
			tootipContent.tooltip = content;
			TOOLTIP(r, tootipContent);
		}
		protected void TOOLTIP(Rect r, GUIContent content)
		{
			/*tootipContent.tooltip = content.tooltip;
			LABEL(r, tootipContent);*/
			//Root.SetMouseTooltip(content, r);
			if (Event.current.type != EventType.Repaint || !r.Contains(Event.current.mousePosition)) return;
			tootipContent.tooltip = content.tooltip;
			GUI.skin.label.Draw(r, tootipContent, false, false, false, false);
			//I.Label(r, tootipContent);
		}

		internal SIZES_CLASS SIZE = new SIZES_CLASS();

		internal class SIZES_CLASS
		{
#pragma warning disable
			//	Adapter adapter;
#pragma warning restore
			internal SIZES_CLASS()
			{
				//this.adapter = adapter;
			}
			/* internal  int OBJECT = 19;
			 internal  int COMP = 12;
			 internal  int VAR = 7;
			 internal  int padding = 11;*/

			public float OBJECT()
			{
				return 19;
				// return 19 * adapter.bottomInterface.hyperGraph.CURRENT_SCALE;
			}

			internal float COMP()
			{
				return 12;
				//  return 12 * adapter.bottomInterface.hyperGraph.CURRENT_SCALE;
			}

			internal float ARROW_WIDTH()
			{
				return 10;
				// return 10 * adapter.bottomInterface.hyperGraph.CURRENT_SCALE;
			}

			internal float VAR()
			{
				return 7;
				// return 7 * adapter.bottomInterface.hyperGraph.CURRENT_SCALE;
			}

			internal float padding_y()
			{
				return 11;
				// return 11 * adapter.bottomInterface.hyperGraph.CURRENT_SCALE;
			}

			internal float SPACE()
			{
				return 2;
				// return 2 * adapter.bottomInterface.hyperGraph.CURRENT_SCALE;
			}

			internal float DEFAULT_PAD()
			{
				return 20;
				// return 20 * adapter.bottomInterface.hyperGraph.CURRENT_SCALE;
			}

			/* private const int DEFAULT_PAD = 20;
			private const int SPACE = 2;*/
		}











		protected int[] perfomanceArray = { 2, 4, 6, 8, 10 };

		protected readonly GUIContent procContent =
			new GUIContent() { tooltip = "Performance of updating references to the current object" };

		protected readonly GUIContent autohideContent =
			new GUIContent() { tooltip = "Automatic Hide HperGraph if selection has been changed" };

		protected readonly GUIContent autorefreshContent =
			new GUIContent() { tooltip = "Automatic reload object if selection has been changed" };

		// readonly GUIContent HyperGraph = new GUIContent() { tooltip = "HyperGraph" };



		GUIStyle __CenteredButton;

		protected GUIStyle CenteredButton
		{
			get
			{
				if (__CenteredButton == null)
				{
					__CenteredButton = new GUIStyle(adapter.STYLE_HYPERGRAPH_DEFBUTTON);
					__CenteredButton.alignment = TextAnchor.MiddleCenter;
				}

				return __CenteredButton;
			}
		}

		GUIStyle __boldLabel;

		protected GUIStyle boldLabel(int fontSize)
		{
			if (__boldLabel == null)
			{
				__boldLabel = new GUIStyle(adapter.label);
				__boldLabel.fontStyle = FontStyle.Bold;
				__boldLabel.alignment = TextAnchor.MiddleLeft;
			}

			__boldLabel.fontSize = fontSize;
			return __boldLabel;
		}

		protected void DRAW_BOLD_LABEL(Rect titleRect, string empty, int fontSize)
		{
			GUI.Label(titleRect, empty, boldLabel(fontSize));
		}

		protected void DRAW_BOLD_LABEL(Rect titleRect, GUIContent empty, int fontSize)
		{
			GUI.Label(titleRect, empty, boldLabel(fontSize));
		}


		protected readonly GUIContent HyperGraphZoomPlus = new GUIContent() { tooltip = "Zoom" };

		protected readonly GUIContent HyperGraphZoomReset = new GUIContent() { tooltip = "Reset" };
		//  readonly GUIContent HyperGraphZoomMinus = new GUIContent() { tooltip = "Close" };

	}
}
