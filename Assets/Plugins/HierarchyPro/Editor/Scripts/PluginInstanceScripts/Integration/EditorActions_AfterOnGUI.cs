using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Cache = EMX.HierarchyPlugin.Editor.Cache;


namespace EMX.HierarchyPlugin.Editor
{



	internal partial class PluginInstance
	{

		void EditorActions_EveryFrame_AfterOnGUI()
		{

			if ( EVENT.type == EventType.MouseDown && par_e.DOUBLECLICK_TO_EXPAND )
			{
				if ( EVENT.clickCount == 2 && EVENT.button == 0 && fullLineRect.Contains( EVENT.mousePosition ) ) // float labelWidth = Adapter.GET_SKIN().label.CalcSize(new GUIContent(o.ToString())).y;
				{ // if (EVENT.mousePosition.x > adapter.BACKUP_RECT.x && EVENT.mousePosition.x < adapter.BACKUP_RECT.x + labelWidth)
					if ( o.ChildCount() > 0 )
					{
						EXPAND_SWITCHER( o );
						Tools.EventUse();
						RepaintWindowInUpdate( pluginID );
					}
				}
			}

		}

		void ButtonsActionsDetect()
		{


			if ( EVENT.type == EventType.KeyDown && par_e.RIGHTARROW_EXPANDS_HOVERED && (EVENT.keyCode == KeyCode.RightArrow || EVENT.keyCode == KeyCode.LeftArrow) )
			{
				if ( hashoveredItem && hoverID != -1 )
				{ /*bottomInterface.EXPAND_SWITCHER( GetHierarchyObjectByInstanceID(hoverID),  EVENT.keyCode == KeyCode.RightArrow ? true : false);
				EventUse();*/

					var o = Cache.GetHierarchyObjectByInstanceID(hoverID, null);
					if ( o != null && o.Validate() && o.ChildCount() > 0 )
					{
						if ( !ha._IsSelectedCache.ContainsKey( hoverID ) )
						{
							EXPAND_SWITCHER( Cache.GetHierarchyObjectByInstanceID( hoverID, null ), EVENT.keyCode == KeyCode.RightArrow ? true : false );
						}

						else
						{
							foreach ( var item in ha._IsSelectedCache )
							{
								EXPAND_SWITCHER( Cache.GetHierarchyObjectByInstanceID( item.Key, null ), EVENT.keyCode == KeyCode.RightArrow ? true : false );
							}
						}

						Tools.EventUse();
					}
				}
			}



			if ( EVENT.type == EventType.KeyDown && par_e.SELECTION_MOVETOGETHER_UPDOWNARROWS && (EVENT.keyCode == KeyCode.DownArrow || EVENT.keyCode == KeyCode.UpArrow) )
			{
				if ( Selection.objects.Length > 1 )
				{
					if ( MultyLineOffsetUpDown() )
						Tools.EventUse();
				}
			}
			if ( EVENT.type == EventType.KeyDown && par_e.SELECTION_MOVETOGETHER_UPDOWNARROWS && (EVENT.keyCode == KeyCode.LeftArrow || EVENT.keyCode == KeyCode.RightArrow) )
			{
				if ( Selection.objects.Length > 1 )
				{
					if ( MultyLineOffsetLeftRight() )
						Tools.EventUse();
				}
			}
		}





		//  System.Reflection.PropertyInfo rowToID;
		System.Reflection.PropertyInfo lastClickedID;
		System.Reflection.MethodInfo EnsureRowIsVisible;
		System.Reflection.MethodInfo GetIndexOfID;
		System.Reflection.MethodInfo NewSelectionFromUserInteraction;
		Type GetIndexOfIDAss;
		bool MultyLineOffsetLeftRight()
		{


			var w = window.Instance;
			var treeView = TreeController_current;

			var rows = Root.p[pluginID].data_GetRows.Invoke(data_currentTree, null) as System.Collections.IList;
			if ( rows == null ) return false;
			if ( rows.Count == 0 ) return false;

			if ( GetIndexOfIDAss == null ) GetIndexOfIDAss = Assembly.GetAssembly( typeof( EditorWindow ) ).GetType( "UnityEditor.IMGUI.Controls.TreeViewController" );
			if ( GetIndexOfID == null ) GetIndexOfID = GetIndexOfIDAss.GetMethod( "GetIndexOfID", (BindingFlags)(-1) );
			if ( EnsureRowIsVisible == null ) EnsureRowIsVisible = GetIndexOfIDAss.GetMethod( "EnsureRowIsVisible", (BindingFlags)(-1) );

			//if ( lastClickedID == null ) lastClickedID = state_currentTree.GetType().GetProperty( "lastClickedID", (BindingFlags)(-1) );
			//var last = (int)lastClickedID.GetValue(state_currentTree, null);
			// var res = new Dictionary<int, int>();
			//	var sel_dic = _IsSelectedCache.ToDictionary(item => (int)GetIndexOfID.Invoke(null, new object[] { rows, item.Key }), item=>item.Key);
			// if (Selection.objects.Any(s=>!s)) Debug.Log("ASD");

			var so = Selection.objects.ToArray();
			if ( so.Length == 0 ) return false;
			int min_index = -1, max_index = -1, min_value = int.MaxValue, max_value = int.MinValue;
			for ( int i = 0; i < so.Length; i++ )
			{
				if ( !so[ i ] ) continue;
				var asdasd = (int)GetIndexOfID.Invoke( null, new object[] {rows,so[i].GetInstanceID()} );
				if ( asdasd < max_value )
				{
					max_value = asdasd;
					max_index = i;
				}
				if ( asdasd > min_value )
				{
					min_value = asdasd;
					min_index = i;
				}
			}
			if ( min_index == -1 || max_index == -1 )
			{
				//Debug.Log( "asd" );
				return false;
			}
			var o = EVENT.keyCode == KeyCode.LeftArrow ? so[min_index] : so[max_index];

			//Dictionary<int, int> sel_dic = Selection.objects.Where(s => s).ToDictionary(
			//		item =>
			//	(int)GetIndexOfID.Invoke(null,
			//		new object[] {rows,
			//		item.GetInstanceID()}),
			//		item => item.GetInstanceID());
			//var newlast = EVENT.keyCode == KeyCode.LeftArrow ? sel_dic[sel_dic.Keys.Min()] : sel_dic[sel_dic.Keys.Max()];
			/* var o  =Cache.GetHierarchyObjectByInstanceID(newlast, null);
             if (o == null || !o.Validate()) return false;*/
			//var o = EditorUtility.InstanceIDToObject(newlast);
			if ( !o ) return false;
			Selection.objects = new[] { o };
			/* if ( NewSelectionFromUserInteraction == null ) NewSelectionFromUserInteraction = GetIndexOfIDAss.GetMethod( "NewSelectionFromUserInteraction", (BindingFlags)(-1) );
             NewSelectionFromUserInteraction.Invoke( treeView, new object[] { res.Values.ToList(), newlast } );*/
			return true;
		}

		bool MultyLineOffsetUpDown()
		{
			int? newlast = null;
			try
			{
				var w = window.Instance;
				var treeView = TreeController_current;

				var rows = Root.p[pluginID].data_GetRows.Invoke(data_currentTree, null) as System.Collections.IList;
				if ( rows == null ) return false;
				if ( rows.Count == 0 ) return false;

				if ( GetIndexOfIDAss == null ) GetIndexOfIDAss = Assembly.GetAssembly( typeof( EditorWindow ) ).GetType( "UnityEditor.IMGUI.Controls.TreeViewController" );
				if ( GetIndexOfID == null ) GetIndexOfID = GetIndexOfIDAss.GetMethod( "GetIndexOfID", (BindingFlags)(-1) );
				if ( EnsureRowIsVisible == null ) EnsureRowIsVisible = GetIndexOfIDAss.GetMethod( "EnsureRowIsVisible", (BindingFlags)(-1) );

				if ( lastClickedID == null ) lastClickedID = state_currentTree.GetType().GetProperty( "lastClickedID", (BindingFlags)(-1) );
				var last = (int)lastClickedID.GetValue(state_currentTree, null);
				var res = new Dictionary<int, int>();
				//	var sel_dic = _IsSelectedCache.ToDictionary(item => (int)GetIndexOfID.Invoke(null, new object[] { rows, item.Key }), item=>item.Key);
				// if (Selection.objects.Any(s=>!s)) Debug.Log("ASD");
				var sel_dic = Selection.objects.Where(s => s).ToDictionary(
					item =>
				(int)GetIndexOfID.Invoke(null,
					new object[] {rows,
					item.GetInstanceID()}),
					item => item.GetInstanceID());
				//orderedSelection.Sort();

				if ( EVENT.shift )
				{
					int expand = 0;

					foreach ( var item in sel_dic )
					{
						if ( !res.ContainsKey( item.Key ) ) res.Add( item.Key, item.Value );

						//if (res.ContainsKey(item.Key+ 1) || res.ContainsKey(item.Key - 1)) hasNear = true;
					}

					if ( res.Values.Any( v => v == last ) )
					{
						var f = res.First(v => v.Value == last);
						var up = res.ContainsKey(f.Key - 1);
						var down = res.ContainsKey(f.Key + 1);

						if ( up && down || !up && !down ) expand = 0;
						else expand = up ? -1 : 1;
					}

					if ( expand != 0 && (expand == 1 && EVENT.keyCode == KeyCode.DownArrow || expand == -1 && EVENT.keyCode == KeyCode.UpArrow) )
					//remove
					{
						foreach ( var item in res.ToDictionary( k => k.Key, v => v.Value ) )
						{
							if ( EVENT.keyCode == KeyCode.DownArrow && res.ContainsKey( item.Key + 1 ) && !res.ContainsKey( item.Key - 1 ) )
							{
								res.Remove( item.Key );
							}

							if ( EVENT.keyCode == KeyCode.UpArrow && !res.ContainsKey( item.Key + 1 ) && res.ContainsKey( item.Key - 1 ) )
							{
								res.Remove( item.Key );
							}
						}

						newlast = EVENT.keyCode != KeyCode.UpArrow ? res[ res.Keys.Min() ] : res[ res.Keys.Max() ];
					}

					else
					//add
					{
						var change = EVENT.keyCode == KeyCode.UpArrow ? -1 : 1;
						if ( EVENT.keyCode == KeyCode.DownArrow && sel_dic.Keys.Max() + change < rows.Count ||
					 EVENT.keyCode == KeyCode.UpArrow && sel_dic.Keys.Min() + change > 0 )
						{
							foreach ( var item in sel_dic )
							{
								int rowIndex = Mathf.Clamp(item.Key + change, 0, rows.Count - 1);

								EnsureRowIsVisible.Invoke( treeView, new object[] { rowIndex, false } );
								var r = rows[rowIndex] as UnityEditor.IMGUI.Controls.TreeViewItem;
								if ( r == null || r.depth == 0 ) goto skip;
								//if ( rowToID == null ) rowToID = rows[rowIndex].GetType().GetProperty( "id", (BindingFlags)(-1) );
								var rowID = r.id;
								//var rowID = (int)rowToID.GetValue(rows[rowIndex], null);

								if ( !res.ContainsKey( rowIndex ) ) res.Add( rowIndex, rowID );

								//if (item.Value == last) newlast = rowID;
							}

							newlast = EVENT.keyCode == KeyCode.UpArrow ? res[ res.Keys.Min() ] : res[ res.Keys.Max() ];
skip:;
						}
					}
				}

				else if ( !EVENT.control )
				{
					var change = EVENT.keyCode == KeyCode.UpArrow ? -1 : 1;

					if ( EVENT.keyCode == KeyCode.DownArrow && sel_dic.Keys.Max() + change < rows.Count ||
						EVENT.keyCode == KeyCode.UpArrow && sel_dic.Keys.Min() + change > 0 )
					{
						foreach ( var item in sel_dic )
						{
							int rowIndex = Mathf.Clamp(item.Key + change, 0, rows.Count - 1);

							EnsureRowIsVisible.Invoke( treeView, new object[] { rowIndex, false } );
							var r = rows[rowIndex] as UnityEditor.IMGUI.Controls.TreeViewItem;
							if ( r == null || r.depth == 0 ) goto skip;
							//if ( rowToID == null ) rowToID = rows[rowIndex].GetType().GetProperty( "id", (BindingFlags)(-1) );
							var rowID = r.id;
							//var rowID = (int)rowToID.GetValue(rows[rowIndex], null);

							res.Add( rowIndex, rowID );

							//if (item.Value == last) newlast = rowID;
						}

						newlast = EVENT.keyCode == KeyCode.UpArrow ? res[ res.Keys.Min() ] : res[ res.Keys.Max() ];
skip:;
					}
				}


				if ( NewSelectionFromUserInteraction == null ) NewSelectionFromUserInteraction = GetIndexOfIDAss.GetMethod( "NewSelectionFromUserInteraction", (BindingFlags)(-1) );
				if ( newlast.HasValue ) NewSelectionFromUserInteraction.Invoke( treeView, new object[] { res.Values.ToList(), newlast ?? res.First().Value } );
				//this.EnsureRowIsVisible(row, false);
				//this.SelectionByKey(rows[row]);
			}

			catch ( Exception ex )
			{
				Debug.LogError( "[" + Root.HierarchyPro + "] Multi-selection caused an error, disable this option in the settings\n\n" + ex.Message + "\n\n" + ex.StackTrace );
				return false;
			}

			return newlast.HasValue;
		}



		internal void EXPAND_SWITCHER( HierarchyObject o, bool? expandOverride = null )
		{
			try
			{
				if ( pluginID == 0 && (!o.go || o.go.transform.childCount != 0) || pluginID == 1 && o.project.IsFolder )
				{
					var expanded = expandOverride ?? !(bool)data_m_dataIsExpanded.Invoke(data_currentTree, new[] { (object)o.id });
					SetExpanded( window.Instance, o.id, expanded );
					Tools.EventUseFast();
				}
			}
			catch ( Exception ex ) { Debug.LogError( ex.Message + "\n\n" + ex.StackTrace ); }
			RepaintWindowInUpdate( pluginID );
		}

		internal void Duplicate()
		{
			if ( ha.SELECTED_OBJECTS().Length == 0 ) return;
			var olddata = duplicate.AddDataToName(duplicate.GetBroadCastSelection(), true);
			if ( UseRootWindow ) hierwin_DuplicateGO.Invoke( window.Instance, null );
			else hierwin_DuplicateGO.Invoke( _SceneHierarchy.GetValue( window.Instance ), null );
			duplicate.SaveDataFromName( duplicate.GetBroadCastSelection() );
			duplicate.RemoveDataFromName( olddata );
		}

		internal void SetExpanded( int i, bool arg3 )
		{
			SetExpanded( window.Instance, i, arg3 );
		}
		internal void SetExpanded( EditorWindow w, int i, bool arg3 )
		{
			if ( !w ) return;
			var t = GetTreeViewontroller(pluginID, w);
			if ( t == null ) return;
			var d = _data.GetValue(t, null);
			if ( d == null ) return;
			data_m_dataSetExpanded.Invoke( d, new[] { (object)i, arg3 } );
		}

		internal void ExpandWithChildren( int i, bool arg3 )
		{
			ExpandWithChildren( window.Instance, i, arg3 );
		}
		internal void ExpandWithChildren( EditorWindow w, int i, bool arg3 )
		{
			if ( !w ) return;
			var t = GetTreeViewontroller(pluginID, w);
			if ( t == null ) return;
			var d = _data.GetValue(t, null);
			if ( d == null ) return;
			data_m_dataSetExpandWithChildrens.Invoke( d, new[] { (object)i, arg3 } );
		}












	}
}



namespace EMX
{

	public partial class Utility
	{

		public static GameObject[] GetAffectsGameObjects( GameObject go )
		{
			if ( !go ) return new GameObject[ 0 ];
			var sel = Selection.gameObjects.Where(g => g.scene.IsValid()).ToArray();
			if ( sel.Contains( go ) ) return sel;
			return new[] { go };
		}
		public static GameObject[] GetOnlyTopObjects( GameObject[] affectedObjectsArray )
		{
			var converted = affectedObjectsArray.Select(a => new { a, par = a.GetComponentsInParent<Transform>(true).Where(p => p != a.transform) });
			return
				converted.Where( c => c.par.Count( p => affectedObjectsArray.Contains( p.gameObject ) ) == 0 ).
				Select( g => g.a ).ToArray();
		}
		public static GameObject[] GetOnlyTopObjects( List<GameObject> affectedObjectsArray )
		{
			var converted = affectedObjectsArray.Select(a => new { a, par = a.GetComponentsInParent<Transform>(true).Where(p => p != a.transform) });
			return
				converted.Where( c => c.par.Count( p => affectedObjectsArray.Contains( p.gameObject ) ) == 0 ).
				Select( g => g.a ).ToArray();
		}

		public static T GetComponentFast<T>( GameObject go ) where T : Component
		{
			return Cache.GetHierarchyObjectByInstanceID( go ).GetComponents().FirstOrDefault( c => c is T ) as T;
		}

		public static void DuplicateSelection()
		{
			EMX.HierarchyPlugin.Editor.Root.p[ 0 ].Duplicate();
		}

		public static void SetExpanded( int i, bool arg3 )
		{
			EMX.HierarchyPlugin.Editor.Root.p[ 0 ].SetExpanded( i, arg3 );
		}

		public static void SetExpandedWithChildren( int i, bool arg3 )
		{
			EMX.HierarchyPlugin.Editor.Root.p[ 0 ].ExpandWithChildren( i, arg3 );
		}



		//public class Styles
		//{
		//	static GUIStyle _button;
		//	public static GUIStyle button
		//	{
		//		get
		//		{
		//			if (_button == null)
		//			{
		//				_button = new GUIStyle(R.p[0].button);
		//				_button.clipping = TextClipping.Clip;
		//				_button.normal.textColor = new Color32(20, 20, 20, 255);
		//				if (EditorGUIUtility.isProSkin) _button.normal.textColor = new Color32(220, 220, 220, 255);
		//				_button.padding = new RectOffset(2, 2, 2, 2);
		//			}
		//			_button.font = 
		//			var style = !callFromExternal() ? RightModsStyles.STYLE_LABEL_8_right : RightModsStyles.STYLE_LABEL_8_WINDOWS_right;
		//
		//			return _button;
		//		}
		//	}
		//}

	}
}