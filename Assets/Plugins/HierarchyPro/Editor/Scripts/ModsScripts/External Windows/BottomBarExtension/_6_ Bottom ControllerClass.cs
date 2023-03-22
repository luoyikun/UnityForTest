
/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;



namespace EMX.HierarchyPlugin.Editor.Mods
{


	
	
	
	
	
	
	internal partial class HyperGraphOld {
		internal void DOCK_HYPER()
		{	
			_6__BottomWindow_HyperGraphWindow.ShowW( adapter );
			// };
			
		}
		
		internal class HyperControllerWindow : Adapter.BottomInterface.UniversalGraphController {
			public HyperControllerWindow(Adapter adapter) : base( adapter )
			{
			}
			
			internal override bool hide_hierarchy_ui_buttons { get { return true; } }
			internal override float HEIGHT
			{	get { return adapter.bottomInterface.hyperGraph.editorWindow ? adapter.bottomInterface.hyperGraph.editorWindow.position.height : 0; }
			
				set { }
			}
			internal override float WIDTH
			{	get { return adapter.bottomInterface.hyperGraph.editorWindow ? adapter.bottomInterface.hyperGraph.editorWindow.position.width : 0; }
			
				set { }
			}
			
		}
		
		internal bool HYPER_FULL_ENABLE()
		{	return adapter.HYPER_ENABLE() || adapter.bottomInterface.hyperGraph.editorWindow;
		}
		
		
		
		//  EditorWindow hyperwindow;
		
		
		
	}
	
	
	
	
	
	
	
	
	
	
	internal partial class BottomInterface {
	
	
	
		
		internal class BottomControllerDefault : BottomController {
			static List<Int32ListArray> list;
			
			internal BottomControllerDefault(Adapter adapter) : base( adapter )
			{
			}
			
			internal override bool CheckCategoryIndex(int scene)
			{	return true;
			}
			
			internal override string GetCurerentCategoryName()
			{	return "";
			}
			
			internal override int GetCategoryIndex(int scene)
			{	adapter.bottomInterface.GET_BOOKMARKS( ref list, scene );
			
				var d = adapter.MOI.des(scene);
				
				if (d == null) return 0;
				
				if (d.FavoritCategorySelected > list.Count - 1) d.FavoritCategorySelected = list.Count - 1;
				
				return Mathf.Clamp( d.FavoritCategorySelected, 0, list.Count - 1 );
			}
			
			internal override void SetCategoryIndex(int index, int scene)
			{	var d = adapter.MOI.des(scene);
			
				if (d == null) return;
				
				if (d.FavoritCategorySelected == index) return;
				
				d.FavoritCategorySelected = index;
				adapter.SetDirtyDescription( d, scene );
				//Hierarchy_GUI.SetDirtyObject( adapter );
				adapter.MarkSceneDirty( scene );
			}
		}
		
		
		internal abstract class BottomController : UniversalGraphController {
			internal BottomController(Adapter adapter) : base( adapter )
			{	// this.adapter = adapter;
			}
			internal override bool hide_hierarchy_ui_buttons { get { return false; } }
			internal override float HEIGHT { get { return 0; } set { } }
			//internal override float WIDTH { get { return 0; } set { } }
			
			internal override float WIDTH
			{	get
				{	return adapter.window().position.width;
				}
				
				set
				{
				}
			}
			// internal override float DEFAULT_WIDTH (UniversalGraphController c) { return 1; }
			
			
			internal abstract bool CheckCategoryIndex(int scene);
			internal abstract int GetCategoryIndex(int scene);
			internal abstract void SetCategoryIndex(int index, int scene);
			
			internal abstract string GetCurerentCategoryName();
			
			internal bool IS_MAIN = false;
			
			internal Rect ModuleRect;
			internal Rect CustomLineRect;
			internal Rect lastRect;
			
			internal bool wasDrag;
			internal EditorWindow selection_window;
			internal int? selection_button;
			internal bool selection_info;
			internal Func<bool, float, bool> __selection_action;
			internal Func<bool, float, bool> selection_action
			{	get
				{	return __selection_action;
				}
				
				set
				{	__selection_action = value;
				
					if (value != null )
					{	if ( REFERENCE_WINDOW == adapter.window() ) adapter.PUSH_EVENT_HELPER_RAW();
						else
						{	var bi = REFERENCE_WINDOW as _6__BottomWindow_BottomInterfaceWindow;
						
							if ( bi ) bi.PUSH_ONMOUSEUP( adapter.EVENT_HELPER_ONUP );
						}
					}
				}
			}
			internal EditorWindow REFERENCE_WINDOW;
			public virtual void REPAINT(Adapter adapter)
			{	if (REFERENCE_WINDOW == adapter.window()) adapter.RepaintWindowInUpdate();
				else
					if (REFERENCE_WINDOW) REFERENCE_WINDOW.Repaint();
					
					
			}
			
			
		}
		
		internal BottomController _mHierarchyController;
		internal BottomController HierarchyController
		{	get
			{	if (_mHierarchyController == null) _mHierarchyController = new BottomControllerDefault( adapter ) { IS_MAIN = true };
			
				return _mHierarchyController;
			}
		}
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		public abstract class UniversalGraphController {
			internal Adapter adapter;
			
			internal UniversalGraphController(Adapter adapter)
			{	this.adapter = adapter;
			}
			
			// internal abstract void Repaint();
			
			internal bool wasInit = false;
			internal abstract bool hide_hierarchy_ui_buttons { get; }
			internal abstract float HEIGHT { get; set; }
			internal abstract float WIDTH { get; set; }
			// internal abstract float DEFAULT_WIDTH(UniversalGraphController controller);
			
			internal Vector2 scrollPos = new Vector2();
			Func<bool, float, bool> m_currentAction;
			internal EditorWindow tempWin;
			
			public virtual bool MAIN
			{	get { return false; }
			}
			
			internal Func<bool, float, bool> currentAction
			{	get { return m_currentAction; }
			
				set
				{	m_currentAction = value;
				
					if (value != null  && adapter != null )
					{	if (  adapter.window() == tempWin )
						{	adapter.PUSH_EVENT_HELPER_RAW();
						}
						
						else
						{	var w = tempWin as _6__BottomWindow_FavoritWindow;
						
							if ( w ) w.PUSH_ONMOUSEUP( adapter.EVENT_HELPER_ONUP );
							else
							{	var w2 = tempWin as _6__BottomWindow_HyperGraphWindow;
							
								if ( w2 ) w2.PUSH_ONMOUSEUP( adapter.EVENT_HELPER_ONUP );
							}
						}
					}
				}
			}
		}
		
		
		
		internal abstract class BOTTOM_GRAPH {
			internal bool needRepaint = false;
			internal Adapter adapter;
			internal BottomInterface bottomInterface;
			
			internal EditorWindow editorWindow;
			
			
			public virtual void Update()
			{	if (needRepaint)
				{	needRepaint = false;
				
					if (editorWindow)
						editorWindow.Repaint();
					else
						adapter.RepaintWindowInUpdate();
				}
				
				if (bottomInterface.ENABLE_HYPERGUI())
				{	var oldH = bottomInterface.m_HIPER_HEIGHT();
					bottomInterface._HIPER_HEIGHT = Mathf.MoveTowards( bottomInterface.m_HIPER_HEIGHT(),
					                                adapter.HYPER_ENABLE() ? adapter.par.HiperGraphParams.HEIGHT : 0, adapter.deltaTime * 1600 );
					                                
					if (bottomInterface._HIPER_HEIGHT != oldH)
					{	//  MonoBehaviour.print(deltaTime);
						adapter.RESET_SMOOTH_HEIGHT();
						adapter.RepaintWindowInUpdate();
					}
				}
				
				if (bottomInterface.ENABLE_FAVORGUI())
				{	var oldH = bottomInterface.m_FAV_HEIGHT();
					bottomInterface._FAV_HEIGHT = Mathf.MoveTowards( bottomInterface.m_FAV_HEIGHT(),
					                              adapter.FAV_ENABLE() ? adapter.par.FavoritesNavigatorParams.HEIGHT : 0, adapter.deltaTime * 1600 );
					                              
					if (bottomInterface._FAV_HEIGHT != oldH)
					{	//  MonoBehaviour.print(deltaTime);
						adapter.RESET_SMOOTH_HEIGHT();
						adapter.RepaintWindowInUpdate();
					}
				}
			}
			
			
			
			internal int currentActionID = -1;
			internal void ADD_ACTION(int actionID, Rect? captureCell, Func<bool, bool> update, Action<bool> end,
			                         UniversalGraphController controller)
			{	EventUse();
			
				currentActionID = actionID;
				// currentActionWindow = window();
				
				if (captureCell.HasValue)
				{	var pos = EditorGUIUtility.GUIToScreenPoint( new Vector2( captureCell.Value.x, captureCell.Value.y ) );
					captureCell = new Rect( pos.x, pos.y, captureCell.Value.width, captureCell.Value.height );
				}
				
				controller.currentAction = (mouseUp, deltaTIme) =>
				{	if (actionID != currentActionID)
					{	// if (currentActionWindow != null)
						//             {
						//                 currentActionWindow.Repaint();
						Repaint( controller );
						// }
						return false;
					}
					
					var cc = captureCell == null ||
					         captureCell.Value.Contains( EditorGUIUtility.GUIToScreenPoint( Event.current.mousePosition ) );
					//if (cc) currentActionID = r;
					//          else currentActionID = -1;
					
					bool result = update( cc );
					
					if (mouseUp)
					{	end( cc );
						//Undo.RecordObject(Hierarchy_GUI.Initialize(), Hierarchy_GUI.GetLastScenes()[arrayIndex].pin ? "UnPin Scene" : "Pin Scene");
						//            Hierarchy_GUI.GetLastScenes()[arrayIndex].pin = !Hierarchy_GUI.GetLastScenes()[arrayIndex].pin;
						//            Hierarchy_GUI.SetDirtyObject();
						bottomInterface.ClearAction();
					}
					
					return result;
				}; // ACTION
			}
			
			internal void Repaint(UniversalGraphController controller = null)
			{	//
				if (controller != null)
				{	if (controller.MAIN) adapter.RepaintWindowInUpdate();
					else
						if ( controller.tempWin) controller.tempWin.Repaint();
				}
				
				else
					needRepaint = true;
			}
			internal bool HOVER(int actionID, Rect? rect, UniversalGraphController controller)
			{	if (Event.current.type != EventType.Repaint) return false;
			
				return currentActionID != -1 && controller.currentAction != null && currentActionID == actionID &&
				       (rect == null || rect.Value.Contains( Event.current.mousePosition ));
			}
			
			internal abstract void SWITCH_ACTIVE_SCAN(bool? overrideActive);
			
			//     if (par.HiperGraphParams.ENABLE) HierHyperController.wasInit = false;
			// HyperWindow.WindowHyperController.wasInit = false;
			
			internal void SWITCH_ACTIVE(bool? overrideActive = null)
			{
			
				if (adapter.IS_PROJECT()) bottomInterface.favorGraph. DOCK_FAVOR();
				
				SWITCH_ACTIVE_SCAN( overrideActive );
				
#if UNITY_EDITOR
				//  MonoBehaviour.print("SWITCH_ACTIVE");
#endif
				
				if (adapter.IS_SEARCH_MODE_OR_PREFAB_OPENED())
				{	return;
				}
				
				adapter.RESET_SMOOTH_HEIGHT();
				adapter.SavePrefs();
				adapter.RepaintWindowInUpdate();
				
				
			}
			
			
			
			internal void DRAW(Rect lineRect, UniversalGraphController controller, EditorWindow win )
			{	ROUND_RECT( ref lineRect );
				EXTERNAL_HYPER_DRAWER( lineRect, controller, win );
			}
			
			internal virtual void EXTERNAL_HYPER_DRAWER(Rect lineRect, UniversalGraphController HierHyperController, EditorWindow win) { throw new NotImplementedException(); }
			
			internal Rect dragRect = new Rect();
			internal Color dragColor = new Color( 0.2f, 0.5f, 0.8f, 0.1f );
			internal bool DRAG_VALIDATOR()
			{	var type = (bool? )DragAndDrop.GetGenericData( "Dragging HyperGraph" );
			
				if (type.HasValue && type.Value) return false;
				
				if (DragAndDrop.objectReferences.Length == 0) return false;
				
				
				if (adapter.IS_HIERARCHY()) return DragAndDrop.objectReferences.Any( g => g is GameObject && ((GameObject)g).scene.IsValid() );
				else return DragAndDrop.objectReferences.Any( g => !string.IsNullOrEmpty( adapter.bottomInterface.INSTANCEID_TOGUID( g.GetInstanceID() ) ) );
			}
			
			internal abstract void DRAG_PERFORMER_SCAN();
			
			internal void DRAG_PERFORMER()
			{	if (DRAG_VALIDATOR())
				{	DRAG_PERFORMER_SCAN();
				}
				
				// KEEPER_CREATE_LINE(DragAndDrop.objectReferences[0] as MonoScript, int.MaxValue);
			}
		}
	}
	
	
*/