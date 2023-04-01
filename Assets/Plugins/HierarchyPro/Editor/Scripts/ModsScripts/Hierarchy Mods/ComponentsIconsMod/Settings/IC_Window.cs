using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class IC_Window : ScriptableObject
	{
	}


	[CustomEditor( typeof( IC_Window ) )]
	class SETGUI_Icons : MainRoot
	{
		internal static string set_text =  USE_STR + "Icons for Components";
		internal static string set_key = "USE_COMPONENTS_ICONS_MOD";

		public override void OnInspectorGUI()
		{
			_GUI( (IRepaint)this );
		}

		public static void DRAW_CLAMP( IRepaint w )
		{
			//using ( ENABLE( w ).USE( "USE_RIGHT_ALL_MODS", padding: 0 ) )
			//{
			//	Draw.TOG( "Clamp graphic behind the right modules", "COMPONENTS_CLAMP_GRAPHIC" );
			//	using ( ENABLE( w ).USE( "COMPONENTS_CLAMP_GRAPHIC", inverce: true, padding: 0 ) )
			//		Draw.TOG( "Clamp events behind the right modules", "COMPONENTS_CLAMP_EVENTS" );
			//}

			using ( ENABLE( w ).USE( "USE_COMPONENTS_ICONS_MOD", padding: 0 ) )
			{

				//Draw.TOG_TIT(  "Clipping graphic method:", EnableRed:false );
				GUI.Label( Draw.R, "Graphic clipping method:" );
				Draw.TOOLBAR( new[] { "None", "All graphic & events behind right bar" }, "COMPONENTS_CLAMP_GRAPHIC" );
				Draw.HRx05( Draw.R );
				if ( !p.par_e.COMPONENTS_CLAMP_GRAPHIC )
				{
					Draw.FIELD( "Graphic Clipping - Indent from right window border", "COMPONENTS_DISABLED_CLAMP_OFFEST", -300, 100 );
					Draw.Sp( 8 );
					//GUI.Label( Draw.R, "Clipping click events for icons :" );
					Draw.TOOLBAR( new[] { "None", "Clip events behind right bar" }, "COMPONENTS_CLAMP_EVENTS" );
					//Draw.TOG( "Clamp only click events for icons - behind the right bar", "COMPONENTS_CLAMP_EVENTS" );
				}
				else
				{
				}
				//var e = GUI.enabled;
				//GUI.enabled &= !p.par_e.COMPONENTS_CLAMP_GRAPHIC;
				//GUI.enabled = e;


				//Draw.TOG( "Clamp all icons - behind the right bar", "COMPONENTS_CLAMP_GRAPHIC" );
				//using ( ENABLE( w ).USE( "COMPONENTS_CLAMP_GRAPHIC", inverce: true, padding: 0 ) )
				//Draw.TOG( "Clamp only click events for icons - behind the right bar", "COMPONENTS_CLAMP_EVENTS" );
			}
		}

		public static void _GUI( IRepaint w )
		{
			Draw.RESET( w );

			Draw.BACK_BUTTON( w  );
			Draw.TOG_TIT( set_text, set_key,WIKI: WIKI_4_ICONS );
			Draw.Sp( 10 );

			using ( ENABLE( w ).USE( set_key ) )
			{

				using ( GRO( w ).UP( 0 ) )
				{
					Draw.Sp( 5 );
					Draw.FIELD( "Icons size", "COMPONENTS_ICONS_SIZE", 1, 30 );
					Draw.Sp( 2 );

					Draw.FIELD( "Space between Icons", "COMPONENTS_ICONS_SPACE", -20, 20 );
					Draw.FIELD( "Additional space between categories", "COMPONENTS_ICONS_CAT_SPACE", -20, 20 );
					Draw.FIELD( "Icons margin left", "COMPONENTS_NEXT_TO_NAME_PADDING", -100, 100 );
					Draw.FIELD( "Icons margin top", "COMPONENTS_MARGIN_TOP", -100, 100 );
					Draw.Sp( 2 );
					Draw.TOG( "Draw shadow for icons", "COMPONENTS_DRAW_ICONS_SHADOW" );

					Draw.TOG( "Change cursor if mouse hovering over the icon", "COMPONENTS_CHANGE_MOUSE_CURSOR" );

					Draw.Sp( 16 );
					SETGUI_MainSettings.DRAW_SWAP_BUTTONS_GUI( w );
					Draw.Sp( 16 );
					//Draw.HRx1();
					{

						Draw.TOG_TIT( "Clipping Graphic & Events for Icons:", EnableRed: false );
						// Draw.TOG_TIT( "Clamp offset if from the right border:", EnableRed:false);
						//Draw.TOG_TIT(  "Clipping Graphic & Events for Icons:", EnableRed:false );


						DRAW_CLAMP( w );

					}
					Draw.Sp( 4 );

					//}

					//using ( GRO( w ).UP( 0 ) )
					//{

				}

				Draw.Sp( 10 );

				using ( GRO( w ).UP( 0 ) )
				{
					Draw.TOG_TIT( Draw.CONT( "[MENU] - Icons Context Menu:" ), EnableRed: true );
					//using ( GRO( w ).UP( 0 ) )
					GUI.Label( Draw.R, "Methods/Fields/Properties:" );
					Draw.Sp( 2 );
					Draw.TOG( "Exclude methods for non-monobehaviour components from menu", "COMPONENTS_MENU_INCLUDEONLYMONOSCRIPTS" );

					{
						Draw.TOG( "Include static methods", "COMPONENTS_MENU_INCLUDESTATICFIELDS" );
						Draw.TOG( "Scan base classes", "COMPONENTS_MENU_INCLUDEBASECLASES" );
					}
					Draw.Sp( 2 );
				}

				Draw.Sp( 10 );
				using ( GRO( w ).UP( 0 ) )
				{
					Draw.TOG_TIT( Draw.CONT( "[DRAW_IN_HIER] attribute:" ) );
					Draw.FIELD( "Text font size", "ICONSMOD_LABELS_FONT_SIZE", 4, 60, overrideObject: p.par_e.HIER_WIN_SET );//
					Draw.HRx1();

					Draw.TOG( "Draw methods buttons", "COMPONENTS_ATTRIBUTES_BUTTONS" );
					using ( ENABLE( w ).USE( Draw.GetSetter( "COMPONENTS_ATTRIBUTES_BUTTONS" ) ) )
						Draw.TOG( "Display methods name for buttons", "COMPONENTS_ATTRIBUTES_NAME_FOR_BUTTONS" );
					Draw.TOG( "Draw fields/properties/enums", "COMPONENTS_ATTRIBUTES_FIELDS" );
					using ( ENABLE( w ).USE( Draw.GetSetter( "COMPONENTS_ATTRIBUTES_FIELDS" ) ) )
						Draw.TOG( "Red null values", "COMPONENTS_ATTRIBUTES_DISPLAYING_NULLSISRED" );
					Draw.HELP( w, "Use [DRAW_IN_HIER] attribute to display vars or method in hierarchy, you cam find little example in examples folder", drawTog: true );
					Draw.FIELD( "Attributes margin left", "COMPONENTS_ATTRIBUTES_MARGIN", -20, 100 );

					Draw.Sp( 4 );
				}

				Draw.Sp( 10 );
				using ( GRO( w ).UP( 0 ) )
				{
					// Draw.TOG_TIT( Draw.CONT( "Unity Components Icons:" ) );
					// Draw.TOG( "Draw unity icons", "COMPONENTS_DRAW_DEFAULT_ICONS" );
					Draw.TOG_TIT( "Draw Unity Components Icons:", "COMPONENTS_DRAW_DEFAULT_ICONS", EnableRed: true );
					using ( ENABLE( w ).USE( Draw.GetSetter( "COMPONENTS_DRAW_DEFAULT_ICONS" ), 0 ) )
					{
						Draw.Sp( 4 );
						GUI.Label( Draw.R, "Hidden icons:" );
						DrawHidenIcons();
						Draw.HELP( w, "Use icons menu to hide icons", drawTog: true );
					}
					Draw.Sp( 4 );
				}


				Draw.Sp( 10 );
				using ( GRO( w ).UP( 0 ) )
				{
					//  Draw.TOG_TIT( Draw.CONT( "MonoBehaviour Icons:" ) );
					Draw.TOG_TIT( "Draw MonoBehaviour Icons:", "COMPONENTS_DRAW_MONO_ICONS", EnableRed: true );
					
					using ( ENABLE( w ).USE( Draw.GetSetter( "COMPONENTS_DRAW_MONO_ICONS" ) ) )
					{
					Draw.Sp( 4 );
						Draw.TOG( "Use transparent style for mono icons", "COMPONENTS_DRAW_ICONS_MONO_BG_INVERSE" );
					Draw.Sp( 4 );
						GUI.Label( Draw.R, Draw.CONT( "Grouping monobehaviour scripts:" ) );
						Draw.TOOLBAR( new[] { "Common\n[1] Icon", "Enable/Disable\n[2] Icons", "Each\nOwn Icon" }, "COMPONENTS_MONO_SPLIT_MODE", 40 );
						if ( p.par_e.COMPONENTS_MONO_SPLIT_MODE == 2 )
						{
							GUI.Label( Draw.R, Draw.CONT( "Monobehaviour icons style:" ) );
							Draw.TOOLBAR( new[] { "Blank Icon", "Icon With\n1 Char", "Icon With\n2 Chars" }, "COMPONENTS_MONO_ICON_TYPE", 40 );
						}
					}
					Draw.Sp( 4 );
				}

				Draw.Sp( 10 );
				using ( GRO( w ).UP( 0 ) )
				{
					// Draw.TOG_TIT( Draw.CONT( "Custom Icons:" ) );
					// Draw.TOG( "Draw custom global icons", "COMPONENTS_DRAW_GLOBALCUSTOM_ICONS" );
					Draw.TOG_TIT( "Draw Custom Assigned Icons:", "COMPONENTS_DRAW_GLOBALCUSTOM_ICONS", EnableRed: true );

					using ( ENABLE( w ).USE( Draw.GetSetter( "COMPONENTS_DRAW_GLOBALCUSTOM_ICONS" ), 0 ) )
					{
						{
							Draw.Sp( 10 );
							Draw.TOG( "Draw custom icons assigned in inspector", "COMPONENTS_DRAW_CUSTOM_ICONS_FROM_ISPECTOR" );
							Draw.HELP( w, "Use the inspector to assign custom icons for script files", drawTog: true );
							Draw.Sp( 4 );
						}
						//using ( ENABLE( w ).USE( Draw.GetSetter( "COMPONENTS_DRAW_CUSTOM_ICONS_FROM_SETTINGS" ), 0 ) )
						using ( GRO( w ).UP( 0 ) )
						{
							Draw.TOG( "Draw custom icons assigned in the list below", "COMPONENTS_DRAW_CUSTOM_ICONS_FROM_SETTINGS" );
							DrawUserIcons();
						}
					}
					Draw.Sp( 4 );
				}





				Draw.Sp( 10 );
				//Draw.HRx2();
				//GUI.Label( Draw.R, "" + LEFT + " Area:" );
				using ( GRO( w ).UP() )
				{

					// Draw.TOG_TIT( "" + LEFT + " Area:" );

					Draw.TOG_TIT( "Quick tips:" );
					Draw.HELP_TEXTURE( w, "HELP_CUSTOM_ICONS_DRAG", 0 );
					Draw.HELP( w, "LMB to open icon's menu.", drawTog: true );
					Draw.HELP( w, "Ctrl+DRAG to start dragging and move the component.", drawTog: true );
					Draw.HELP( w, "Ctrl+Shift+DRAG to start dragging and copy the component.", drawTog: true );
					Draw.HELP_TEXTURE( w, "HELP_SEARCH" );
					Draw.HELP( w, "RMB on icon to search.", drawTog: true );


					Draw.HRx1();
					Draw.HELP_TEXTURE( w, "HELP_CUSTOM_ICONS_ATT" );
					Draw.HELP( w, "Add [DRAW_IN_HIER] attribute in your code to draw 'fields','properties','methods' and 'enums' in the hierarchy window.", drawTog: true );
					Draw.HELP( w, "You can invoke method or change int, float, string or enum values right in the hierarchy.", drawTog: true );
					//Draw.HELP( w, "[DRAW_IN_HIER] works with public, private or internal fields.", drawTog: true );
					//Draw.HELP(w,"You can add your own items using 'ExtensionInterface_RightClickOnGameObjectMenuItem'.", drawTog: true);
					Draw.HRx1();
					if ( Draw.BUT( "Select Example Scene" ) ) { Selection.objects = new[] { Root.icons.example_folders[ 2 ] }; }

				}
			}
		}




		static Vector2 DrawHidenIconsscrollPos;
		static void DrawHidenIcons()
		{
			var list = HierarchyCommonData.Instance().GetComponentIconHidedList();
			var RECT = Draw.RH(50);
			//  RECT.y -= 10;
			//  RECT.x += 20;
			//  RECT.width = Math.Min( W + 10 , RECT.width ) - 20;
			//  RECT.width = RECT.width - 20;
			DrawHidenIconsscrollPos = GUI.BeginScrollView( RECT, DrawHidenIconsscrollPos, new Rect( 0, 0, list.Count * 32, 32 ), true, false );
			Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
			//Assembly asm = typeof(Image).Assembly;
			p.INTERNAL_BOX( new Rect( 0, 0, RECT.width, 32 ), "" );

			if ( list.Count == 0 ) p.INTERNAL_BOX( new Rect( 0, 0, RECT.width, 32 ), "Use left-click in the hierarchy on the icon to hide" );

			for ( int i = 0; i < list.Count; i++ )
			{
				RECT = new Rect( i * 32, 0, 32, 32 );
				//print(list[i] + " " + asm.GetType(list[i]));
				Type target = null;

				foreach ( var assembly in asms )
				{
					target = assembly.GetType( list[ i ] );

					if ( target != null ) break;
				}

				//  if (Event.current.type.Equals(EventType.Repaint)) GUI.DrawTexture(lastRect, Utilites.ObjectContent(null, asm.GetType(list[i])).image);
				var find = EditorGUIUtility.ObjectContent(null, target);

				if ( Event.current.type.Equals( EventType.Repaint ) && find.image != null ) GUI.DrawTexture( RECT, find.image );

				if ( !GUI.enabled ) PluginInstance.FadeRect( RECT, 0.7f );

				// if (!GUI.enabled) if (Event.current.type.Equals(EventType.Repaint)) GUI.DrawTexture(lastRect,Hierarchy.sec);
				RECT.x += RECT.width / 2;
				RECT.height = RECT.width = RECT.width / 2;

				if ( GUI.Button( RECT, "X" ) )
				{
					//   Hierarchy_GUI.Undo(this, "Restore Icon");
					//   Hierarchy_GUI.Instance(this).HiddenComponents.RemoveAt(i);
					//   Hierarchy_GUI.SetDirtyObject(this);
					//   DRAW_STACK.ValueChanged();
					HierarchyCommonData.Instance().ComponentIconHidedListRemoveAt( i );
					p.RESET_DRAWSTACK( 0 );
				}
			}

			GUI.EndScrollView();
		}





		static DrawCustomIconsClassOld __CI;
		internal static DrawCustomIconsClassOld CI {
			get {
				var res = __CI ?? (__CI = new DrawCustomIconsClassOld());
				res.A = p;
				return res;
			}
		}

		static void DrawUserIcons()
		{
			//   var boxRect = EditorGUILayout.GetControlRect(GUILayout.Height(0));
			//  boxRect.height = CI.CusomIconsHeight + 12;

			//  var R = EditorGUILayout.GetControlRect(GUILayout.Height(CI.CusomIconsHeight));
			Draw.Sp( 10 );
			var R = Draw.RH(CI.CusomIconsHeight);
			var boxRect = R;
			boxRect.height = CI.CusomIconsHeight + 8;
			p.INTERNAL_BOX( boxRect, "" );
			//R.x += 7;
			// R.y += 6;
			GUI.BeginScrollView( R, Vector2.zero, new Rect( 0, 0, R.width, DrawCustomIconsClassOld.IC_H * (CI.customIcons.Count + 1) ), false, false /*, GUILayout.Width(W), GUILayout.ExpandWidth(true)*/);

			var lr = R;

			CI.DrawCustomIcons( EditorWindow.focusedWindow, lr );
			EditorGUILayout.GetControlRect( GUILayout.Height( 10 ) );

			GUI.EndScrollView();

			CI.Updater( EditorWindow.focusedWindow );
		}


	}
}
