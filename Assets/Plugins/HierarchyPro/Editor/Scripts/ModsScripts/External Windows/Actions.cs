using System;
using System.Linq;
using UnityEngine;
using UnityEditor;



namespace EMX.HierarchyPlugin.Editor.Mods
{

	class Actions
	{

		internal int PluginID = 0;

		internal int currentActionID = -1;
		//PluginInstance adapter { get { return Root.p[0]; } }
		internal void ADD_ACTION(int actionID, Rect? captureCell, Func<bool, bool> update, Action<bool> end,
			ExternalDrawContainer controller)
		{
			Tools.EventUse();

			currentActionID = actionID;
			// currentActionWindow = window();

			if (captureCell.HasValue)
			{
				var pos = EditorGUIUtility.GUIToScreenPoint(new Vector2(captureCell.Value.x, captureCell.Value.y));
				captureCell = new Rect(pos.x, pos.y, captureCell.Value.width, captureCell.Value.height);
			}

			controller.currentAction = (mouseUp, deltaTIme) =>
			{
				if (actionID != currentActionID)
				{ /* if (currentActionWindow != null)
						             {
						                 currentActionWindow.Repaint();*/
					controller.tempRoot.RepaintNow();
					//Repaint(controller);
					// }
					return false;
				}

				var cc = captureCell == null ||
						 captureCell.Value.Contains(EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition));
				/*if (cc) currentActionID = r;
						  else currentActionID = -1;*/

				bool result = update(cc);

				if (mouseUp)
				{
					end(cc);
					/*Undo.RecordObject(Hierarchy_GUI.Initialize(), Hierarchy_GUI.GetLastScenes()[arrayIndex].pin ? "UnPin Scene" : "Pin Scene");
								Hierarchy_GUI.GetLastScenes()[arrayIndex].pin = !Hierarchy_GUI.GetLastScenes()[arrayIndex].pin;
								Hierarchy_GUI.SetDirtyObject();*/
					controller.ClearAction();
				}

				return result;
			}; // ACTION
		}



		internal bool HOVER(int actionID, Rect? rect, ExternalDrawContainer controller)
		{
			if (Event.current.type != EventType.Repaint) return false;

			return currentActionID != -1 && controller.currentAction != null && currentActionID == actionID &&
				   (rect == null || rect.Value.Contains(Event.current.mousePosition));
		}

		/*	internal abstract void SWITCH_ACTIVE_SCAN(bool? overrideActive);

			//     if (par.HiperGraphParams.ENABLE) HierHyperController.wasInit = false;
			// HyperWindow.WindowHyperController.wasInit = false;

			internal void SWITCH_ACTIVE(bool? overrideActive = null)
			{
				if (adapter.IS_PROJECT()) bottomInterface.favorGraph.DOCK_FAVOR();

				SWITCH_ACTIVE_SCAN(overrideActive);

	#if UNITY_EDITOR
				//  MonoBehaviour.print("SWITCH_ACTIVE");
	#endif

				if (adapter.IS_SEARCH_MODE_OR_PREFAB_OPENED())
				{
					return;
				}

				adapter.RESET_SMOOTH_HEIGHT();
				adapter.SavePrefs();
				adapter.RepaintWindowInUpdate();
			}*/




		internal Rect dragRect = new Rect();
		internal Color dragColor = new Color(0.2f, 0.5f, 0.8f, 0.1f);

		internal bool DRAG_VALIDATOR()
		{
			var type = (bool?)DragAndDrop.GetGenericData("Dragging HyperGraph");

			if (type.HasValue && type.Value) return false;

			if (DragAndDrop.objectReferences.Length == 0) return false;


			if (PluginID == 0) return DragAndDrop.objectReferences.Any(g => g is GameObject && ((GameObject)g).scene.IsValid());
			return DragAndDrop.objectReferences.Any(g =>
			!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(g))
			&& !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID((AssetDatabase.GetAssetPath(g))))
			// !string.IsNullOrEmpty(Cache.INSTANCEID_TOGUID(g.GetInstanceID()))
			);
		}

		internal void DRAG_PERFORMER_SCAN(ICHANGE_SELECTION_OVVERIDE a)
		{
			if (PluginID == 0)
			{
				var n = DragAndDrop.objectReferences.First(g => g is GameObject && ((GameObject)g).scene.IsValid());
				a.CHANGE_SELECTION_OVVERIDE(true, n);
			}

			else
			{

			}
		}

		internal void DRAG_PERFORMER(ICHANGE_SELECTION_OVVERIDE a)
		{
			if (DRAG_VALIDATOR())
			{
				DRAG_PERFORMER_SCAN(a);
			}

			// KEEPER_CREATE_LINE(DragAndDrop.objectReferences[0] as MonoScript, int.MaxValue);
		}




	}
	interface ICHANGE_SELECTION_OVVERIDE
	{
		void CHANGE_SELECTION_OVVERIDE(bool a, UnityEngine.Object o);
	}
}
