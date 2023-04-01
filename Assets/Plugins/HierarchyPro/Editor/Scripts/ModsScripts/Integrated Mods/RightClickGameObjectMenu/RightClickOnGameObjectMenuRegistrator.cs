using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EMX.CustomizationHierarchy;
using EMX.HierarchyPlugin.Editor;

namespace EMX.HierarchyPlugin.Editor
{
	class RightClickOnGameObjectMenuRegistrator
	{



		internal static void ContextMenuForPlugin_0( GenericMenu menu, GameObject go ) // 0 only
		{

			if ( Root.p == null || Root.p[ 0 ] == null || !Root.p[ 0 ].par_e.USE_RIGHT_CLICK_MENU_MOD ) return;

			if ( go ) GenerateCustomMenu( menu, go );


			if ( adapter.par_e.INCLUDE_RIGHTCLIK_MENU_OPENPLUGINWINDOWS_BUTTONS || adapter.par_e.RCGO_MENU_PLACE_TO_SUBMENU )
			{
				var subMenu = Root.p[0].par_e.RCGO_MENU_PLACE_TO_SUBMENU ? (Root.HierarchyPro + "/- "+ Root.HierarchyPro+" Windows") : "" + Root.HierarchyPro + " Windows";

				menu.AddSeparator( ExtensionInterface_RightClickOnGameObjectMenuItem.ItemsPlacementFolder );

				menu.AddItem( new GUIContent( subMenu + "/Open Main Settings" ), false, () => {
					Settings.MainSettingsEnabler_Window.Select<Settings.MainSettingsEnabler_Window>();
				} );
				menu.AddSeparator( subMenu + "/" );
				menu.AddItem( new GUIContent( subMenu + "/Open RightClick Menu Settings" ), false, () => {
					Settings.MainSettingsEnabler_Window.Select<Settings.RC_Window>();
				} );


				if ( adapter.par_e.INCLUDE_RIGHTCLIK_MENU_OPENPLUGINWINDOWS_BUTTONS )
				{
					if ( externalMod_MenuItems.Count != 0 )
					{
						menu.AddSeparator( subMenu + "/" );
						foreach ( var item in externalMod_MenuItems )
						{
							var r = item.release;
							menu.AddItem( new GUIContent( subMenu + "/" + item.path ), false, () => { r( 0, item.text ); ; } );
						}
					}
				}

#if !EMX_H_LITE

				if ( adapter.par_e.INCLUDE_RIGHTCLIK_MENU_OPENPLUGINWINDOWS_BUTTONS || adapter.HL_SET_G( 0 ).HIGHLIGHTER_HIERARCHY_BUTTON_LOCATION == 0 )
				{
					if ( go )
					{
						if ( adapter.HL_SET.USE_MANUAL_HIGHLIGHTER_MOD || adapter.HL_SET.USE_AUTO_HIGHLIGHTER_MOD )
						{
							menu.AddSeparator( subMenu + "/" );
							var _o = Cache.GetHierarchyObjectByInstanceID(go);
							menu.AddItem( new GUIContent( subMenu + "/" + "Highlighter" ), false, () => {
								adapter.PUSH_GUI_ONESHOT( 0, () => {
									if ( SessionState.GetInt( "EMX|HighlighterCat", 0 ) > 1 )
										SessionState.SetInt( "EMX|HighlighterCat", 0 );
									adapter.modsController.highLighterMod.OpenHighlighterWindow( _o.lastSelectionRect, _o );
								} );
							} );
						}
						if ( adapter.HL_SET.USE_CUSTOM_PRESETS_MOD )
						{
							menu.AddSeparator( subMenu + "/" );
							var _o = Cache.GetHierarchyObjectByInstanceID(go);
							menu.AddItem( new GUIContent( subMenu + "/" + "Presets Manager" ), false, () => {
								adapter.PUSH_GUI_ONESHOT( 0, () => {
									if ( SessionState.GetInt( "EMX|HighlighterCat", 0 ) != 3 )
										SessionState.SetInt( "EMX|HighlighterCat", 3 );
									adapter.modsController.highLighterMod.OpenHighlighterWindow( _o.lastSelectionRect, _o );
								} );
							} );
						}
					}



				}
#endif

				menu.AddSeparator( ExtensionInterface_RightClickOnGameObjectMenuItem.ItemsPlacementFolder );

			}



		}


		static List<ExternalMod_MenuItem> externalMod_MenuItems = new List<ExternalMod_MenuItem>();
		public static void RegistrateMenuItem( List<ExternalMod_MenuItem> _externalMod_MenuItems )
		{
			if ( _externalMod_MenuItems == null ) externalMod_MenuItems = new List<ExternalMod_MenuItem>();
			else externalMod_MenuItems = _externalMod_MenuItems.OrderBy( b => b.priority ).ToList();
		}

		static KeyKodeData resultKeyCode;

		internal static void GenerateHotkeys()
		{
			RegistratedKeys.Clear();
			foreach ( var _item in CustomMenuItems )
			{
				var item = _item.Value;
				if ( TryAddToHotKeys( item.Name, ref resultKeyCode ) )
				{
					Action resultAction = null;

					Func<HierarchyObject, bool> enableCap = (inputObject) => item.IsEnable(inputObject.go) && !item.NeedExcludeFromMenu(inputObject.go);
					resultAction = () => {
						var sel = adapter.ha.SELECTED_GAMEOBJECTS().Where(enableCap).Select(s => s.go).ToArray();

						if ( sel.Length == 0 ) return;

						CommonAction_Hierarchy( item, sel.Select( s => s ).ToArray() );
					};

					RegistratedKeys.Add( resultKeyCode, resultAction );
				}
			}

		}



		internal static void SubscribeEvents( EditorSubscriber sbs )
		{
			if ( !adapter.par_e.USE_RIGHT_CLICK_MENU_MOD ) return;
			sbs.OnGlobalKeyPressed += OnGlobalKeyPressed;
			sbs.BuildedOnGUI_first.Add( HierarchyWindowEvent );
		}


		static void OnGlobalKeyPressed( bool used )
		{
			if ( Event.current != null && Event.current.rawType == EventType.KeyDown )
			{
				if ( !used )
				{
					if ( !adapter.par_e.RCGO_MENU_USE_HOTKEYS ) return;
					if ( Event.current.keyCode != KeyCode.None && EditorWindow.focusedWindow )
					{
						var n = EditorWindow.focusedWindow.GetType().Name;
						if ( n.Contains( "SceneHierarchy" ) ) goto skipHier;
						foreach ( var item in adapter.par_e.CUSTOMMENU_HOTKEYS_WINDOWS )
						{
							if (/*IS_HIERARCHY() &&*/ item.Key == "SceneHierarchy" ) continue;
							//if (IS_PROJECT() && item.Key == "ProjectBrowser") continue;
							if ( n.Contains( item.Key ) )
							{
								CheckKeyDown( Event.current.control, Event.current.shift, Event.current.alt, Event.current.keyCode );
							}
						}
					}
skipHier:;
				}
			}
		}

		static void HierarchyWindowEvent()
		{
			if ( adapter.EVENT != null && adapter.EVENT.isKey && adapter.EVENT.rawType == EventType.KeyDown )
			{
				CheckKeyDown( Event.current.control, Event.current.shift, Event.current.alt, Event.current.keyCode );
			}
		}


		static KeyKodeData tempKey;

		static internal Dictionary<KeyKodeData, Action> RegistratedKeys = new Dictionary<KeyKodeData, Action>();


		static bool TryAddToHotKeys( string name, ref KeyKodeData resultKeyCode )
		{
			name = name.Trim();

			if ( string.IsNullOrEmpty( name ) ) return false;

			var split = name.Split(' ');

			if ( split.Length < 2 ) return false;

			var last = split[split.Length - 1].ToUpper();

			if ( string.IsNullOrEmpty( last ) ) return false;

			// char key = last[last.Length - 1];
			string key = last.Trim(new[] { '#', '%', '&', '_' });

			if ( !avaliableKeys.ContainsKey( key ) ) return false;

			bool ctrl = last.Contains('%');
			bool shift = last.Contains('#');
			bool alt = last.Contains('&');
			var keyCode = avaliableKeys[key];

			// if (!ctrl && !shift && !alt && !last.Contains( '_' )) return false;

			resultKeyCode = new KeyKodeData( ctrl, shift, alt, keyCode );

			if ( RegistratedKeys.ContainsKey( resultKeyCode ) ) return false;

			return true;
		}


		static internal void CheckKeyDown( bool ctrl, bool shift, bool alt, KeyCode keyCode )
		{
			if ( !adapter.par_e.RCGO_MENU_USE_HOTKEYS ) return;
			tempKey.ctrl = ctrl;
			tempKey.shift = shift;
			tempKey.alt = alt;
			tempKey.keyCode = (int)keyCode;

			if ( RegistratedKeys.ContainsKey( tempKey ) && RegistratedKeys[ tempKey ] != null )
			{
				RegistratedKeys[ tempKey ]();
				//Tools.EventUseFast();
				Tools.EventUse();
			}
		}
		static Dictionary<string, KeyCode> avaliableKeys = new Dictionary<string, KeyCode>()
	{
		{ "1", KeyCode.Alpha1 },
		{ "2", KeyCode.Alpha2 },
		{ "3", KeyCode.Alpha3 },
		{ "4", KeyCode.Alpha4 },
		{ "5", KeyCode.Alpha5 },
		{ "6", KeyCode.Alpha6 },
		{ "7", KeyCode.Alpha7 },
		{ "8", KeyCode.Alpha8 },
		{ "9", KeyCode.Alpha9 },
		{ "0", KeyCode.Alpha0 },
		{ "Q", KeyCode.Q },
		{ "W", KeyCode.W },
		{ "E", KeyCode.E },
		{ "R", KeyCode.R },
		{ "T", KeyCode.T },
		{ "Y", KeyCode.Y },
		{ "U", KeyCode.U },
		{ "I", KeyCode.I },
		{ "O", KeyCode.O },
		{ "P", KeyCode.P },
		{ "A", KeyCode.A },
		{ "S", KeyCode.S },
		{ "D", KeyCode.D },
		{ "F", KeyCode.F },
		{ "G", KeyCode.G },
		{ "H", KeyCode.H },
		{ "J", KeyCode.J },
		{ "K", KeyCode.K },
		{ "L", KeyCode.L },
		{ "Z", KeyCode.Z },
		{ "X", KeyCode.X },
		{ "C", KeyCode.C },
		{ "V", KeyCode.V },
		{ "B", KeyCode.B },
		{ "N", KeyCode.N },
		{ "M", KeyCode.M },
		{ ",", KeyCode.Comma },
		{ ".", KeyCode.Period },
		{ "/", KeyCode.Slash },
		{ ";", KeyCode.Semicolon },
		{ "'", KeyCode.Quote },
		{ "[", KeyCode.LeftBracket },
		{ "]", KeyCode.RightBracket },
		{ "-", KeyCode.Minus },
		{ "=", KeyCode.Equals },
		{ "\\", KeyCode.Backslash},
		{ "HOME", KeyCode.Home },
		{ "END", KeyCode.End },
		{ "PAGEUP", KeyCode.PageUp },
		{ "PAGEDOWN", KeyCode.PageDown },
		{ "UP", KeyCode.UpArrow },
		{ "LEFT", KeyCode.LeftArrow },
		{ "DOWN", KeyCode.DownArrow },
		{ "RIGHT", KeyCode.RightArrow },
		{ "RETURN", KeyCode.Return },
	};


		internal struct KeyKodeData : IEqualityComparer<KeyKodeData>, IEquatable<KeyKodeData>, IComparable<KeyKodeData>
		{
			internal KeyKodeData( bool ctrl, bool shift, bool alt, KeyCode keyCode )
			{
				this.ctrl = ctrl;
				this.shift = shift;
				this.alt = alt;
				this.keyCode = (int)keyCode;
			}

			internal bool ctrl;
			internal bool shift;
			internal bool alt;
			internal int keyCode;


			public int CompareTo( KeyKodeData other )
			{
				return GetHashCode() - other.GetHashCode();
			}

			public int Compare( KeyKodeData x, KeyKodeData y )
			{
				return x.GetHashCode() - y.GetHashCode();
			}

			public int GetHashCode( KeyKodeData obj )
			{
				return obj.GetHashCode();
			}

			public override bool Equals( object obj )
			{
				return Equals( (KeyKodeData)obj );
			}

			public bool Equals( KeyKodeData x, KeyKodeData y )
			{
				return x.GetHashCode() == y.GetHashCode();
			}

			public bool Equals( KeyKodeData other )
			{
				return GetHashCode() == other.GetHashCode();
			}
			public override int GetHashCode()
			{
				var result = keyCode;

				if ( ctrl ) result += 10000000;

				if ( shift ) result += 20000000;

				if ( alt ) result += 40000000;

				return result;
			}



		}





		internal static Dictionary<Type, ExtensionInterface_RightClickOnGameObjectMenuItem> CustomMenuItems = new Dictionary<Type, ExtensionInterface_RightClickOnGameObjectMenuItem>();


		static bool GenerateCustomMenu( GenericMenu menu, GameObject tapedGO )
		{
			var sorted = CustomMenuItems.Values.OrderBy(e => e.PositionInMenu).ToArray();
			var curerntPos = int.MinValue;
			bool wasRoot = false;
			bool drawn = false;
			if ( ExtensionInterface_RightClickOnGameObjectMenuItem.ItemsPlacementFolder == ""
				|| sorted.Any( c => c.Name.StartsWith( ExtensionInterface_RightClickOnGameObjectMenuItem.ItemsPlacementFolder, StringComparison.OrdinalIgnoreCase ) )
				)
			{
				wasRoot = true;
				menu.AddSeparator( "" );
			}

			for ( int i = 0; i < sorted.Length; i++ )
			{

				if ( sorted[ i ].NeedExcludeFromMenu( tapedGO ) ) continue;

				var root = sorted[i].Name.Substring(ExtensionInterface_RightClickOnGameObjectMenuItem.ItemsPlacementFolder.Length).Count(c => c == '/') == 0;



				if ( curerntPos != int.MinValue && root && Math.Abs( sorted[ i ].PositionInMenu - curerntPos ) > 1 )     //MonoBehaviour.print(sorted[i].Name);
				{
					if ( drawn ) menu.AddSeparator( ExtensionInterface_RightClickOnGameObjectMenuItem.ItemsPlacementFolder );
				}

				var item = sorted[i];
				var name = GetName_Hierarchy(item);

				if ( !sorted[ i ].IsEnable( tapedGO ) )
				{
					menu.AddDisabledItem( new GUIContent( name ) );
				}

				else
				{
					var undoName = name;
					Func<HierarchyObject, bool> enableCap = (inputObject) => item.IsEnable(inputObject.go) && !item.NeedExcludeFromMenu(inputObject.go);
					drawn = true;
					menu.AddItem( new GUIContent( name ), false, () => {
						var sel = adapter.ha.SELECTED_GAMEOBJECTS().Where(enableCap).Select(s => s.go).ToList();

						if ( !sel.Contains( tapedGO ) )
						{
							Undo.RecordObject( tapedGO, undoName );
							item.OnClick( new[] { tapedGO } );

							if ( tapedGO )
							{
								adapter.SetDirty( tapedGO );
								adapter.MarkSceneDirty( tapedGO.scene );
							}

							//adapter.RepaintWindow();
						}

						else
						{
							sel.Remove( tapedGO );

							if ( sel.Count == 0 ) sel.Add( tapedGO );
							else sel.Insert( 0, tapedGO );

							CommonAction_Hierarchy( item, sel.ToArray() );
						}

						if ( adapter.window.Instance ) adapter.window.Instance.Focus();
						adapter.RepaintWindow( 0, true );
					} );
				}

				//if (root) 
				curerntPos = sorted[ i ].PositionInMenu;
			}

			return wasRoot;
		}
		static int[] qwe = new int[3];
		static PluginInstance adapter { get { return Root.p[ 0 ]; } }
		static string GetName_Hierarchy( ExtensionInterface_RightClickOnGameObjectMenuItem item )
		{
			//return string.IsNullOrEmpty(item.Name) ? "- Unidentified -" : item.Name.Replace("$", "");
			var res = string.IsNullOrEmpty(item.Name) ? "- Unidentified -" : (item.Name.Replace("$", ""));
			if ( !adapter.par_e.RCGO_MENU_USE_HOTKEYS )
			{
				qwe[ 0 ] = res.LastIndexOf( '%' );
				qwe[ 1 ] = res.LastIndexOf( '#' );
				qwe[ 2 ] = res.LastIndexOf( '&' );
				for ( int i = 0; i < 3; i++ ) if ( qwe[ i ] == -1 ) qwe[ i ] = int.MaxValue;
				var min = qwe.Min();
				if ( min != int.MaxValue ) res = res.Remove( min );
			}
			if ( !Root.p[ 0 ].par_e.RCGO_MENU_PLACE_TO_SUBMENU && res.IndexOf( '/' ) == -1 ) res = "* " + res;
			//if ( !Root.p[ 0 ].par_e.RCGO_MENU_PLACE_TO_SUBMENU && res.StartsWith(ExtensionInterface_RightClickOnGameObjectMenuItem.ItemsPlacementFolder) ) res = ExtensionInterface_RightClickOnGameObjectMenuItem.ItemsPlacementFolder +  + res.Substring( ExtensionInterface_RightClickOnGameObjectMenuItem.ItemsPlacementFolder.Length );
			return res;
		}

		static void CommonAction_Hierarchy( ExtensionInterface_RightClickOnGameObjectMenuItem item, GameObject[] sel )
		{
			foreach ( var objectToUndo in sel )
				Undo.RecordObject( objectToUndo, GetName_Hierarchy( item ) );

			item.OnClick( sel );

			foreach ( var objectToUndo in sel )
				if ( objectToUndo )
				{
					adapter.SetDirty( objectToUndo );
					adapter.MarkSceneDirty( objectToUndo.scene );
				}

			adapter.RepaintWindow( 0, true );
		}


	}
	/*
	public interface IRightClickOnGameObjectMenuItem
	{
		int PositionInMenu { get; }
		string Name { get; }
		bool IsEnable(GameObject clickedObject);
		bool NeedExcludeFromMenu(GameObject clickedObject);
		void OnClick(GameObject[] affectedObjectsArray);
	}*/
}

namespace EMX.CustomizationHierarchy
{

	public abstract class ExtensionInterface_RightClickOnGameObjectMenuItem
	{



		public static string ItemsPlacementFolder { get { return Root.p[ 0 ].par_e.RCGO_MENU_PLACE_TO_SUBMENU ? "" + Root.HierarchyPro + "/" : ""; } }

		public ExtensionInterface_RightClickOnGameObjectMenuItem()
		{
			//var type = MethodBase.GetCurrentMethod().DeclaringType;
			//var inst = Activator.CreateInstance(type) as IRightClickOnGameObjectMenuItem;
			//	if (inst == null) throw new Exception("inst");
			if ( RightClickOnGameObjectMenuRegistrator.CustomMenuItems.ContainsKey( GetType() ) )
			{
				Debug.LogWarning( "Warning! You already have a '" + GetType().Name + "' item. Please check the 'INIT()' method" );
			}
			RightClickOnGameObjectMenuRegistrator.CustomMenuItems.Remove( GetType() );
			RightClickOnGameObjectMenuRegistrator.CustomMenuItems.Add( GetType(), this );
			RightClickOnGameObjectMenuRegistrator.GenerateHotkeys();

		}

		//	[InitializeOnLoadMethod]
		//protected static void InitializeMod()



		public abstract int PositionInMenu { get; }
		public abstract string Name { get; }
		public abstract bool IsEnable( GameObject clickedObject );
		public abstract bool NeedExcludeFromMenu( GameObject clickedObject );
		public abstract void OnClick( GameObject[] affectedObjectsArray );

		public virtual bool DoRecordUndoOnClick { get { return false; } }

	}
}
