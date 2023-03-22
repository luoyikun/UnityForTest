using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;




namespace EMX.HierarchyPlugin.Editor.Mods
{





	internal class ProjectGuiExtension
	{

		PluginInstance p;
		internal ProjectGuiExtension(PluginInstance p)
		{
			this.p = p;
		}


		internal void Subscribe(EditorSubscriber sbs)
		{
			// sbs.OnPlayModeStateChanged += PlaymodeStateChanged;
			sbs.OnProjectWindow += asdasd;
			//  proj = new PluginInstance() { pluginID = 1 };
			//  sbs.BuildedOnGUI_middle += Draw;
		}

		GUIStyle _label;
		GUIStyle label
		{
			get
			{
				if (_label == null)
				{
					_label = new GUIStyle(GUI.skin.label);
					_label.alignment = TextAnchor.MiddleRight;
				}
				return _label;
			}
		}



#pragma warning disable
		class projectFile
		{
			internal string ex;
			internal bool isFoldout;
			internal Rect? lastRect;
		}
#pragma warning restore
		static Dictionary<int, projectFile> ex_cache = new Dictionary<int, projectFile>();
		projectFile ex;

		void OnGUI(Rect rect, int id, bool folded, bool box, string backedPath = null)
		{
			if (!ex_cache.TryGetValue(id, out ex))
			{
				//var path = AssetDatabase.GUIDToAssetPath(id);
				var path = backedPath ?? AssetDatabase.GetAssetPath(id);
				if (string.IsNullOrEmpty(path)) return;
				else if (!File.Exists(path)) ex_cache.Add(id, null);
				else
				{
					//  args1[0] = arg1;
					//  var _id = (int)GetInstanceIDFromGUID.Invoke(null, args1);
					//  var ob = EditorUtility.InstanceIDToObject(_id);
					if (AssetDatabase.IsSubAsset(id)) ex_cache.Add(id, null);
					else
					{
						/*  }
                          if ((ob = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path)) != AssetDatabase.LoadMainAssetAtPath(path)) ex_cache.Add(arg1, null);
                          else
                          {*/




						var fileName = path.Substring(path.LastIndexOf('/') + 1);
						var dot = fileName.LastIndexOf('.');
						if (dot == -1) ex_cache.Add(id, null);

						// else ex_cache.Add(arg1, ex = new projectFile() { ex = fileName.Substring(dot + 1), isFoldout = AssetDatabase.LoadAllAssetsAtPath(path).Length > 1 });
						else ex_cache.Add(id, ex = new projectFile() { ex = fileName.Substring(dot + 1), isFoldout = folded });
					}
				}
			}
			if (ex != null)
			{

				if (Event.current.type != EventType.Repaint)
				{
					//  if (ex.lastRect == null) ex.isFoldout = true;
					ex.lastRect = null;
				}
				else
				{

					if (!ex.lastRect.HasValue) ex.lastRect = rect;
					var child = ex.lastRect.Value.y + ex.lastRect.Value.x != rect.x + rect.y;
					if (!child)
					{
						//  var box = rect.height >= rect.width;
						if (box)
						{
							rect.x += rect.width;
							rect.y += rect.height - 16;
							rect.height = p.par_e.DRAW_EXTENSION_FONT_SIZE + 4;
							rect.y -= p.par_e.DRAW_EXTENSION_FONT_SIZE;
						}
						else
						{


							if (ex.isFoldout)
								rect.x -= 15;
						}
						rect.width = 100;
						rect.x -= 100;

						label.fontSize = p.par_e.DRAW_EXTENSION_FONT_SIZE;
						rect.x += p.par_e.DRAW_EXTENSION_OFFSET_X;
						rect.y += p.par_e.DRAW_EXTENSION_OFFSET_Y;

						if (box && EditorGUIUtility.isProSkin)
						{
							label.normal.textColor = Color.black;
							GUI.Label(new Rect(rect.x - 1f, rect.y - 1f, rect.width, rect.height), ex.ex, label);
							GUI.Label(new Rect(rect.x + 2f, rect.y + 2f, rect.width, rect.height), ex.ex, label);
						}
						label.normal.textColor = p.par_e.DRAW_EXTENSION_COLOR;
						GUI.Label(rect, ex.ex, label);
					}

				}


			}
		}



		EventType? lastEvent;
		//  PluginInstance proj;
		object[] args1 = new object[1];
		Window window = new Window();
		object[] args = new object[2];
		int firstRowVisible, lastRowVisible, numVisibleRows;
		int endRow, rowCount;
		int index = 0; int num = 0;
		bool animatingExpansion;
		Rect m_TotalRect;
		internal Vector2 scrollPos;
		bool GUIInit = false;
		int ViewMode;
		bool nowSearch = false;
		void asdasd(string s, Rect rect)
		{

			if (!p.par_e.DRAW_EXTENSION_IN_PROJECT) return;

			GUIInit = false;

			if (lastEvent != Event.current.type)
			{
				lastEvent = Event.current.type;
				GUIInit = true;
				Window.AssignInstance(1, ref window, p.ProjectBrowserWindowType); // WINDOW INIT
				ViewMode = p.ProjectViewMode(window.Instance);

				// var st = p.m_SearchFieldText.GetValue(window.Instance) as string;
				//  nowSearch = st != null && st != "";

				var sr = p.m_SearchFilter.GetValue(window.Instance);
				if (sr != null)
				{
					nowSearch = (bool)p.IsSearching.Invoke(sr, null);
				}
				else
				{
					nowSearch = false;
				}
			}


			if (ViewMode == 1 || nowSearch)
			{

				// var box = rect.height >= rect.width;
				//  if (!box) return;
				var path = AssetDatabase.GUIDToAssetPath(s);
				if (path == null || path == "" || AssetDatabase.IsValidFolder(path)) return;
				// args1[0] = path;
				// Debug.Log(rect);
				//  Debug.Log(p.GetMainAssetOrInProgressProxyInstanceID.GetParameters().Select(sd => sd.ParameterType.Name).Aggregate((a, b) => a + " " + b) );
				//  Debug.Log(p.GetMainAssetOrInProgressProxyInstanceID.ReturnParameter.ParameterType.Name);
				//  var _id = (int)p.GetMainAssetOrInProgressProxyInstanceID.Invoke(null, args);
				var _id = s.GetHashCode();
				OnGUI(rect, _id, false, true, path);
				return;
			}


			if (GUIInit)
			{
				//TreeController_current = p.m_TreeViewFieldInfoForProject(window.Instance).GetValue(window.Instance);
				TreeController_current = p.GetTreeViewontroller(1 , window.Instance);
				gui_currentTree = p._gui.GetValue(TreeController_current);
				data_currentTree = p._data.GetValue(TreeController_current);
				state_currentTree = p._state.GetValue(TreeController_current);

				args[0] = args[1] = 0;
				p.gui_getFirstAndLastRowVisible.Invoke(gui_currentTree, args);
				firstRowVisible = (int)args[0];
				lastRowVisible = (int)args[1];
				numVisibleRows = lastRowVisible - firstRowVisible + 1;
				m_TotalRect = (Rect)p.tree_m_TotalRect.GetValue(TreeController_current);
				scrollPos = (Vector2)p.state_scrollPos.GetValue(state_currentTree);
				rowCount = (int)p.data_rowCount.GetValue(data_currentTree);

				animatingExpansion = (bool)p.tree_animatingExpansion.GetValue(TreeController_current, null);
				if (animatingExpansion)
				{
					m_ExpansionAnimator = p.tree_m_ExpansionAnimator.GetValue(TreeController_current);
					endRow = (int)p.ExpansionAnimator_endRow.GetValue(m_ExpansionAnimator, null);
				}
				index = 0; num = 0;
			}
			else
			{
				index++;
			}

			int row = CalcRow(ref index, ref num);
			/*  var fakeIndex = index + 1;
			  var fakeNum = num;
			  CalcRow(ref fakeIndex, ref fakeNum);*/
			if (row == -1) return;
			args1[0] = row;
			var currentTreeItemFast = p.data_GetItemRowFast.Invoke(data_currentTree, args1) as UnityEditor.IMGUI.Controls.TreeViewItem;
			OnGUI(rect, currentTreeItemFast.id, currentTreeItemFast.hasChildren, false);
		}
		internal object TreeController_current, gui_currentTree, data_currentTree, state_currentTree, m_ExpansionAnimator;
		int CalcRow(ref int index, ref int num)
		{
			int row = -1;
			for (; index < numVisibleRows; ++index)
			{
				row = firstRowVisible + index;
				if (this.animatingExpansion)
				{
					// int endRow = this.m_ExpansionAnimator.endRow;
					//  if ( this.m_ExpansionAnimator.CullRow( row, this.gui ) )
					args[0] = row;
					args[1] = gui_currentTree;
					var res = (bool)p.ExpansionAnimator_CullRow.Invoke(m_ExpansionAnimator, args);
					if (res)
					{
						++num;
						row = endRow + num;
					}
					else
						row += num;
					// if ( row >= this.data.rowCount )
					if (row >= rowCount)
						continue;
				}
				else
				{
					args[0] = row;
					args[1] = 0f;
					var res = (Rect)p.gui_GetRowRect.Invoke(gui_currentTree, args);
					if ((double)(res.y - scrollPos.y) > m_TotalRect.height)
						continue;
				}
				break;
			}
			return row;
		}

















	}
}
