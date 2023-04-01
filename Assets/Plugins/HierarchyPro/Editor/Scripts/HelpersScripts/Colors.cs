using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace EMX.HierarchyPlugin.Editor
{

	class Colors
	{
		internal static Color c1 = (Color)new Color32(56, 56, 56, 255);
		internal static Color c1111 = (Color)new Color32(62, 62, 62, 255);
			internal static Color LINE = new Color(.1f, .1f, .1f, 0.1F);
		internal static Color colorStatic = new Color(Color.gray.r, Color.gray.g, Color.gray.b, Color.gray.a / 4);

#if UNITY_2019_3_OR_NEWER
		internal static Color c2 = (Color)new Color32(0xc8, 0xc8, 0xc8, 255);
		internal static Color setc2 = (Color)new Color32(222, 222, 222, 255);
		internal static Color c2222 = (Color)new Color32(0xe8, 0xe8, 0xe8, 255);
		static Color m_SelectColorPersonal = (Color)new Color32(0x3a, 0x72, 0xb0, 255);
		static Color m_SelectColorPersonalNonFOcus = new Color32(0xae, 0xae, 0xae, 255);
#else
		internal static Color c2 = (Color)new Color32(194, 194, 194, 255);
		internal static Color setc2 = (Color)new Color32(222, 222, 222, 255);
		internal static Color c2222 = (Color)new Color32(217, 217, 217, 255);
				static Color m_SelectColorPersonal = new Color32(62, 125, 231, 255);
		static Color m_SelectColorPersonalNonFOcus = new Color32(143, 143, 143, 255);
#endif


		internal static Color EditorBGColor
		{
			get { return EditorGUIUtility.isProSkin ? c1 : c2; }
		}
		internal static Color SettingsBGColor
		{
			get { return EditorGUIUtility.isProSkin ? c1 : setc2; }
		}

		internal static Color SceneColor
		{
			get { return EditorGUIUtility.isProSkin ? c1111 : c2222; }
		}

		//internal static Color B_ACTIVE = new Color(.6f, .75f, .8f, 0.6f);
		internal static Color B_ACTIVE = new Color(.55f, .6f, .6f, 0.6f);
		internal static Color B_PASSIVE = new Color(.8f, .8f, .8f, 0.1F);
		internal static Color redTTexure = new Color(0.6f, 0.3f, 0.1f, 1);


		static Color m_HoverColorPersonal = new Color32(170, 170, 170, 255);
#if UNITY_2019_3_OR_NEWER
		static Color m_HoverColorPro = new Color32(69, 69, 69, 255);
#else
         static Color m_HoverColorPro = new Color32( 51, 51, 51, 255  );
#endif
		internal static Color HoverColor;

		// static Color m_PrefabColorPro = new Color32( 76, 128, 217, 255  );
		// static Color tttCC;
		//static Color m_SelectColorPro = new Color32(62, 95, 150, 255);
        static Color m_SelectColorPro = new Color32(44, 93, 135, 255);
		//  static Color m_SelectColorProNonFocus = new Color32( 104, 104, 104, 255  );
		static Color m_SelectColorProNonFocus = new Color32(72, 72, 72, 255);

		//  internal static Color? backedSelectColor;
		internal static Color SelectColor;

		internal static Color SelectColorOverrided(bool active)
		{
			if (active)
			{
				if (EditorGUIUtility.isProSkin) return m_SelectColorPro;
				else return m_SelectColorPersonal;
			}

			else
			{
				if (EditorGUIUtility.isProSkin) return m_SelectColorProNonFocus;
				else return m_SelectColorPersonalNonFOcus;
			}
		}
		internal static void SelectRect(Rect drawRect, float alpha = 1, bool? overrideSelect = null)
		{
			Color tttCC;
			if (overrideSelect.HasValue)
				tttCC = SelectColorOverrided(overrideSelect.Value);
			else
				tttCC = SelectColor;
			tttCC.a = alpha;
			EditorGUI.DrawRect(drawRect, tttCC);
		}


		internal static void UpdateColorsBefore_OnGUI(PluginInstance p)
		{
			// if ( backedSelectColor.HasValue ) return backedSelectColor.Value;


			//SELECTION
			if (EditorWindow.focusedWindow == p.window.Instance && (string.IsNullOrEmpty(GUI.GetNameOfFocusedControl()) || GUI.GetNameOfFocusedControl() != "SearchFilter"))
			{
				if (EditorGUIUtility.isProSkin) SelectColor = m_SelectColorPro;
				else SelectColor = m_SelectColorPersonal;
			}

			else
			{
				if (EditorGUIUtility.isProSkin) SelectColor = m_SelectColorProNonFocus;
				else SelectColor = m_SelectColorPersonalNonFOcus;
			}

			//HOVER
			if (p.ha.hoveredBackgroundColor.HasValue)
			{
				HoverColor = Color.Lerp(EditorBGColor, new Color(p.ha.hoveredBackgroundColor.Value.r, p.ha.hoveredBackgroundColor.Value.g, p.ha.hoveredBackgroundColor.Value.b, 1),
						 p.ha.hoveredBackgroundColor.Value.a);

			}
			else
			{
				if (EditorGUIUtility.isProSkin) HoverColor = m_HoverColorPro;
				else HoverColor = m_HoverColorPersonal;
			}


		}


		internal class ColorList
		{

			public static implicit operator Color32(ColorList c)
			{
				return c.color;
			}


			internal string key;
			internal int index;
			internal Color32 def;
			internal Color32 color
			{
				get { return Root.p[0].par_e.GET(key + index, def); }
				set {
					var r = color;
					Root.p[0].par_e.SET(key + index, value); }
			}
		}
		static List<ColorList> _GetLastHiglightColorList;
		static List<ColorList> _GetLastTextColorList;
		internal static List<ColorList> GetLastHiglightColorList
		{
			get
			{
				return _GetLastHiglightColorList ?? (_GetLastHiglightColorList = GetDefaultHighLichterList().Select((c, i) => new ColorList()
				{
					key = "LAST_HIGHLIGHT_COLOR",
					def = c,
					index = i
				}).ToList());
			}
		}
		internal static List<ColorList> GetLastTextColorList
		{
			get
			{
				return _GetLastTextColorList ?? (_GetLastTextColorList = GetDefaultTextColorsList().Select((c, i) => new ColorList()
				{
					key = "LAST_TEXT_COLOR",
					def = c,
					index = i
				}).ToList());
			}
		}



		static List<Color32> GetDefaultTextColorsList()
		{
			var res = new List<Color32>()
		{
			new Color32(49, 58, 63, 255),
			new Color32(255, 77, 67, 255),
			new Color32(38, 42, 45, 255),
			new Color32(136, 202, 96, 255),
			new Color32(255, 111, 111, 255),
			new Color32(13, 66, 98, 255),
			new Color32(0, 204, 153, 255),
			new Color32(0, 101, 153, 255),
			new Color32(130, 181, 63, 255),
			new Color32(65, 139, 202, 255),
			new Color32(232, 119, 85, 255),
			new Color32(82, 62, 125, 255),
			new Color32(246, 126, 4, 255),
			new Color32(25, 168, 40, 255),
			new Color32(245, 186, 31, 255),
		};

			if (EditorGUIUtility.isProSkin) return res;

			return res.Select(c => (Color)c).Select(c => (Color32)new Color(1 - (1 - c.r) / 2, 1 - (1 - c.g) / 2, 1 - (1 - c.b) / 2, c.a)).ToList();
			//};
		}

		static List<Color32> GetDefaultHighLichterList()
		{
			var res = new List<Color32>()
		{
			new Color32(58, 103, 100, 255),
			new Color32(245, 41, 78, 255),
			new Color32(98, 125, 196, 255),
			new Color32(207, 27, 39, 255),
			new Color32(225, 35, 69, 255),
			new Color32(54, 180, 70, 255),
			new Color32(241, 89, 16, 255),
			new Color32(45, 80, 112, 255),
			new Color32(226, 110, 33, 255),
			new Color32(1, 114, 132, 255),
			new Color32(137, 121, 96, 255),
			new Color32(61, 148, 139, 255),
			new Color32(149, 33, 44, 255),
			new Color32(224, 224, 224, 255),
			new Color32(1, 254, 211, 255),
		};

			if (EditorGUIUtility.isProSkin) return res;

			return res.Select(c => (Color)c).Select(c => (Color32)new Color(1 - (1 - c.r) / 2, 1 - (1 - c.g) / 2, 1 - (1 - c.b) / 2, c.a)).ToList();
			//};
		}


		internal static bool Eq( Color32 a,  Color32 b)
		{
			return a.r == b.r &&
				a.g == b.g &&
				a.b == b.b &&
				a.a == b.a;
		}
	}
}
