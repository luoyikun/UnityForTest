using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using EMX.HierarchyPlugin.Editor.Mods;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class ST_Window : ScriptableObject
	{
	}

	[CustomEditor( typeof( ST_Window ) )]
	class SETGUI_Snap : MainRoot
	{

		internal static string set_text =  USE_STR + "Snap Transform Mod (Inspector Window)";
		internal static string set_key = "USE_SNAP_MOD";
		public override VisualElement CreateInspectorGUI()
		{
			return base.CreateInspectorGUI();
		}

		static List<KeyCode> keys = Enumerable.Repeat(0, 122 - 97 + 1).Select((v, i) => (KeyCode)(i + 97)).ToList();


		static void DRAW_HOTKEY( CachedPref sNAP_SNAP_USEHOTKEYS, CachedPref sNAP_SNAP_HOTKEYS )
		{
			DrawToogle( sNAP_SNAP_USEHOTKEYS );
			GUI.enabled = sNAP_SNAP_USEHOTKEYS;
			GUILayout.BeginHorizontal();
			GUILayout.Space( 80 );

			var I_KEY = (int)sNAP_SNAP_HOTKEYS;
			var oldIndex = Mathf.Max(keys.FindIndex(k => (int)k == I_KEY), 0);
			//var ni = EditorGUI.Popup(Draw.R, sNAP_SNAP_HOTKEYS.name, oldIndex, keys.Select(k => k.ToString()).ToArray());
			var ni = EditorGUILayout.Popup( sNAP_SNAP_HOTKEYS.name, oldIndex, keys.Select(k => k.ToString()).ToArray());
			if ( ni != oldIndex ) I_KEY = (int)keys[ ni ];
			if ( sNAP_SNAP_HOTKEYS != I_KEY ) sNAP_SNAP_HOTKEYS.Set( I_KEY );

			GUILayout.Space( 40 );
			GUILayout.EndHorizontal();
			GUI.enabled = true;
		}

		public override void OnInspectorGUI()
		{
			_GUI( (IRepaint)this );
		}
		public static void _GUI( IRepaint w )
		{
			Draw.RESET( w );

			Draw.BACK_BUTTON( w );
			Draw.TOG_TIT( set_text, set_key, AreYouSureText: "Scripts compilation will start now. Are you sure?", WIKI: WIKI_2_SNAP );
			Draw.HELP( w, "Snap buttons available in the Inspector next to the position and rotation fields" );
			Draw.Sp( 10 );

			using ( GRO( w ).UP( 0 ) )
			{

				using ( ENABLE( w ).USE( set_key ) )
				{








					GUILayout.Space( 10 );



					//  GUILayout.Label("HotKeys to Invert Snapping States");
					DrawToogle( Mods.SnapMod.SNAP_FASTINVERT_USEHOTKEYS );
					GUI.enabled = Mods.SnapMod.SNAP_FASTINVERT_USEHOTKEYS;
					var inv = (int)Mods.SnapMod.SNAP_FASTINVERT_HOTKEYS;
					var I_KEY = inv & 0xFFFF;
					GUILayout.BeginHorizontal();
					GUILayout.Space( 80 );
					EditorGUILayout.GetControlRect( GUILayout.Height( EditorGUIUtility.singleLineHeight ) );
					var r = GUILayoutUtility.GetLastRect(); r.width /= 3;
					DrawButton( r, "Ctrl", ref inv, 16, EditorStyles.miniButtonLeft ); r.x += r.width;
					DrawButton( r, "Shift", ref inv, 17, EditorStyles.miniButtonMid ); r.x += r.width;
					DrawButton( r, "Alt", ref inv, 18, EditorStyles.miniButtonRight );

					var oldIndex = Mathf.Max(keys.FindIndex(k => (int)k == I_KEY), 0);
					//var ni = EditorGUI.Popup(Draw.R, oldIndex, keys.Select(k => k.ToString()).ToArray());
					var ni = EditorGUILayout.Popup( oldIndex, keys.Select(k => k.ToString()).ToArray());
					GUILayout.Space( 40 );
					GUILayout.EndHorizontal();
					if ( ni != oldIndex )
					{
						inv &= ~0xFFFF;
						inv |= (int)keys[ ni ];
					}
					if ( Mods.SnapMod.SNAP_FASTINVERT_HOTKEYS != inv ) Mods.SnapMod.SNAP_FASTINVERT_HOTKEYS.Set( inv );
					GUI.enabled = true;
					Draw.HRx2();

					DRAW_HOTKEY( Mods.SnapMod.SNAP_SNAP_USEHOTKEYS, Mods.SnapMod.SNAP_SNAP_HOTKEYS );
					Draw.HRx2();
					DRAW_HOTKEY( Mods.SnapMod.SNAP_SURFACE_USEHOTKEYS, Mods.SnapMod.SNAP_SURFACE_HOTKEYS );
					Draw.HRx2();


					DrawToogle( Mods.SnapMod.SNAP_AUTOAPPLY );
				}
				using ( ENABLE( w ).USE( set_key ) )
				{
					GUILayout.Space( 10 );
					GUILayout.Label( "Grid Snapping" );
					DrawVector( Mods.SnapMod.SNAP_POS );
					GUILayout.Space( 5 );
					DrawVector( Mods.SnapMod.SNAP_ROT );
					GUILayout.Space( 5 );
					DrawVector( Mods.SnapMod.SNAP_SCALE );
					Draw.HELP( w, "You can also you right-click on icon to change snap axis" );
					GUILayout.Space( 10 );
				}
				using ( ENABLE( w ).USE( set_key ) )
				{
					GUILayout.Label( "Surface Placement" );
					DrawToogle( Mods.SnapMod.PLACE_ON_SURFACE_ENABLE );
					var e = GUI.enabled;
					GUI.enabled = false;
					GUILayout.BeginHorizontal();
					var i = EditorGUILayout.Popup(Mods.SnapMod.PLACE_ON_SURFACE_ALIGNBYMOUSE ? 0 : 1, Mods.SnapMod.ALIGN_BY);
					GUILayout.Space( 40 );
					GUILayout.EndHorizontal();
					if ( i != (Mods.SnapMod.PLACE_ON_SURFACE_ALIGNBYMOUSE ? 0 : 1) ) Mods.SnapMod.PLACE_ON_SURFACE_ALIGNBYMOUSE.Set( i == 1 );
					GUI.enabled = e;
					DrawToogle( Mods.SnapMod.CALCULATE_OBJECT_BOUNDS );
					GUILayout.Space( 10 );
					DrawToogle( Mods.SnapMod.ALIGN_BY_NORMAL );
					GUILayout.BeginHorizontal();
					i = EditorGUILayout.Popup( Mods.SnapMod.ALIGN_UP_VECTOR.name, Mods.SnapMod.ALIGN_UP_VECTOR, Mods.SnapMod.VECTORS_STRING );
					GUILayout.Space( 40 );
					GUILayout.EndHorizontal();
					if ( i != Mods.SnapMod.ALIGN_UP_VECTOR ) Mods.SnapMod.ALIGN_UP_VECTOR.Set( i );

					if ( GUI.changed ) UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
				}
			}


			//Draw.HRx4RED();
			Draw.Sp( 10 );

			using ( GRO( w ).UP( 0 ) )
			{

				Draw.TOG_TIT( "Quick tips:" );
				//Draw.HELP_TEXTURE( w, "HELP_SNAP", 0 );
				Draw.HELP( w, "RMB to open a menu.", drawTog: true );
				Draw.HELP( w, "You can setup key hold to enable quick switching between snap states.", drawTog: true );
				//Draw.HELP( w, "Use direction and offset if you use surface snapping and align by normal.", drawTog: true );
			}

		}





		static void DrawVector( VectorPref pref )
		{
			// if (GUILayout.Button(pref.ENABLE.name)) pref.ENABLE.Set(!pref.ENABLE);
			//  if (Event.current.type == EventType.Repaint && pref.ENABLE) GUI.skin.button.Draw(GUILayoutUtility.GetLastRect(), pref.ENABLE.name, true, true, false, true);
			//pref.ENABLE.Set(EditorGUI.ToggleLeft());
			Draw.TOG_TIT( Draw.R15, pref.ENABLE.name, (bool)pref.ENABLE );

			GUILayout.BeginHorizontal();
			var R = Draw.R2;
			R.width /= 5;
			R.x += R.width;
			pref.X.Set( EditorGUI.ToggleLeft( R, "X", (bool)pref.X ) );
			R.x += R.width;
			pref.Y.Set( EditorGUI.ToggleLeft( R, "Y", (bool)pref.Y ) );
			R.x += R.width;
			pref.Z.Set( EditorGUI.ToggleLeft( R, "Z", (bool)pref.Z ) );
			/* if (GUILayout.Button("X", EditorStyles.miniButtonLeft, new GUILayoutOption[0])) pref.X.Set(!pref.X);
             if (Event.current.type == EventType.Repaint && pref.X) GUI.skin.button.Draw(GUILayoutUtility.GetLastRect(), pref.X.name, true, true, false, true);
             if (GUILayout.Button("Y", EditorStyles.miniButtonMid, new GUILayoutOption[0])) pref.Y.Set(!pref.Y);
             if (Event.current.type == EventType.Repaint && pref.Y) GUI.skin.button.Draw(GUILayoutUtility.GetLastRect(), pref.Y.name, true, true, false, true);
             if (GUILayout.Button("Z", EditorStyles.miniButtonRight, new GUILayoutOption[0])) pref.Z.Set(!pref.Z);
             if (Event.current.type == EventType.Repaint && pref.Z) GUI.skin.button.Draw(GUILayoutUtility.GetLastRect(), pref.Z.name, true, true, false, true);*/
			GUILayout.EndHorizontal();
		}

		static void DrawToogle( CachedPref pref )
		{
			//   var b = EditorGUILayout.ToggleLeft(pref.name, pref);
			var b = Draw.TOG(Draw.R, pref.name, pref);
			if ( b != pref ) pref.Set( b );
		}
		static void DrawButton( Rect r, string c, ref int val, int offset, GUIStyle s )
		{

			var oldEn = (val & (1 << offset)) != 0;

			var newEn = Draw.TOG(r, c, oldEn);
			// if (GUI.Button(r, c, s))
			if ( newEn != oldEn )
			{
				if ( oldEn ) val &= ~(1 << offset);
				else val |= (1 << offset);
				oldEn = !oldEn;
			}
			// if (Event.current.type == EventType.Repaint && oldEn) GUI.skin.button.Draw(r, c, true, true, false, true);
		}



	}
}





