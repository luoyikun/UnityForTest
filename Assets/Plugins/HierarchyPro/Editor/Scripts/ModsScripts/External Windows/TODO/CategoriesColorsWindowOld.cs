using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace EMX.HierarchyPlugin.Editor.Mods
{

	/* internal partial class Adapter
     {





         internal partial class BottomInterface
         {*/

	public class CategoriesColorsWindowOld : Windows.IWindow
	{
		// const int WIDTH = 230;
		static BookProject.BookmarksforProjectviewModInstance bm_inst;
		internal static CategoriesColorsWindowOld InitForProjectFolder( MousePos rect, Window win )
		{ //  Debug.Log("ASD");
		  // if (adapter.GET_SKIN() == null) adapter.GET_SKIN() = Adapter.GET_SKIN();

			/*var oldL = adapter.GET_SKIN().label.fontSize;
			adapter.GET_SKIN().label.fontSize = adapter.WINDOW_FONT_10();*/
			if ( rect.type != MousePos.Type.ColorChanger_230_0 )
			{
				Debug.LogWarning( "Mismatch type" );
				rect.SetType( MousePos.Type.ColorChanger_230_0 );
			}

			//, BookProject.BookmarksforProjectviewModInstance bm_inst
			//bm_inst = _bm_inst;
			//Mods.BookProject.BookmarksforProjectviewModWindow
			//Mods.BookProject.BookmarksforProjectviewModInstance.
			if ( win == null || !win.Instance )
			{
				win = Window._windowsList.FirstOrDefault( v => v.pluginID == 1 && v.thisIsAFirstWindow );
				if ( win == null || !win.Instance ) return null;
			}
			var pw = Root.p[ 0 ].modsController.activeExternalMods.FirstOrDefault( v => v is Mods.BookProject.BookmarksforProjectviewModWindow ) as Mods.BookProject.BookmarksforProjectviewModWindow;
			if ( !pw ) return null;

			bm_inst = pw.instance;
			var list = bm_inst.GET_BOOKMARKS();
			// rect = InputData.WidnwoRect(new Vector2(rect.x, rect.y - list.Count * 23 + 32), WIDTH, list.Count * 23 + 32, adapter, disableClamp: true);

			rect.Height = list.Count * 23 + 32 + 10;
			rect.Y -= rect.Height + 40;

			var w = (CategoriesColorsWindowOld)private_Init(rect, typeof(CategoriesColorsWindowOld), win, "Background Colors");
			w.scene = -1;
			w.getter = () => bm_inst.GET_BOOKMARKS().Select( i => (IBgColor)i ).ToList();
			return w;
		}

		Func<List<IBgColor>> getter;

		//	static List<Int32ListArray> list;

		static List<string> allow = new List<string>() { "ColorPicker", "_W__InputWindow" };
		int scene;
		/* internal override bool PIN
		 {
			 get { return false; }
			 set { m_PIN = value; }
		 }*/
		bool wasLoasFocus = false;

		internal override bool PIN {
			get {
				if ( EditorWindow.focusedWindow == this )
				{
					wasLoasFocus = false;
					return true;
				}

				if ( EditorWindow.focusedWindow == null ) return true;
				//MonoBehaviour.print(EditorWindow.focusedWindow.GetType().Name);
				if ( allow.Any( l => EditorWindow.focusedWindow.GetType().Name.Contains( l ) ) )
				{
					wasLoasFocus = true;
					return true;
				}

				if ( wasLoasFocus && this )
				{
					wasLoasFocus = false;
					this.Focus();
					return true;
				}

				/*   if (pinOverride == null)
				   {
					   pinOverride = Resources.FindObjectsOfTypeAll<EditorWindow>().FirstOrDefault(w => allow.Any(l => w.GetType().Name.Contains(l)));
				   }
				   if (pinOverride != null)
				   {
					   return EditorWindow.focusedWindow == (EditorWindow)pinOverride;
				   }*/
				//  return true;
				/* if (/*Resources.FindObjectsOfTypeAll(typeof(EditorWindow)).Any(w => allow.Any(l => w.GetType().Name.Contains(l)) && (EditorWindow.focusedWindow == (EditorWindow)w) ||#1#
					 EditorWindow.focusedWindow == this || pinOverride != null && allow.Any(l => pinOverride.GetType().Name.Contains(l)))
				 {
					 // MonoBehaviour.print("ASD");
					 return true;
				 }*/
				return false;
			}
			set { m_PIN = value; }
		}

		protected override void Update()
		{
			if ( EditorWindow.focusedWindow != this && !PIN ) Close();
			base.Update();
		}

		static Rect rect;
		//static GUIContent colorContent = new GUIContent() { tooltip = "Background Color" };

		protected override void OnGUI()
		{
			if ( !_inputWindow  ) {
				CloseThis();
				return;
			}
			if ( mouse_uo_helper == null )
			{
				Close();
				return;
			}

			//if (adapter == null)
			//{
			//	CloseThis();
			//	return;
			//}

			base.OnGUI();


			// if (Event.current.type == EventType.keyDown) MonoBehaviour.print(Event.current.keyCode);
			if ( Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape )
			{
				Tools.EventUseFast();
				CloseThis();
				Root.p[ 0 ]._SKIP_PREFAB_ESCAPE[ 0 ] = true;

				return;
			}

			if ( Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter) )
			{
				Tools.EventUseFast();
				CloseThis();
				return;
			}


			Root.p[ 0 ].ChangeGUI();


			var label = lastMousePos.Width - 55 - 5;
			rect.Set( 5, 5, label, 23 );
			Label( rect, "Category Name" );
			rect.x = lastMousePos.Width;
			rect.width = 55;
			Label( rect, "Color" );


			List<IBgColor> list = getter();
			//adapter.bottomInterface.GET_BOOKMARKS(ref list, scene);

			for ( int i = 0; i < list.Count; i++ )
			{
				rect.x = 0;
				rect.width = lastMousePos.Width - 55 - 5;
				rect.y += rect.height;
				Root.p[ 0 ].INTERNAL_BOX( rect, "" );
				var al = Root.p[0].GET_SKIN().label.alignment;
				Root.p[ 0 ].GET_SKIN().label.alignment = TextAnchor.MiddleLeft;
				Label( rect, "  " + (i + 1) + ") " + list[ i ].get_name );
				Root.p[ 0 ].GET_SKIN().label.alignment = al;

				rect.x = +rect.width;
				rect.width = 55;
				var c = list[i].BgColor ?? Root.p[0].par_e.BOOKMARKS_FOLDER_DEFULT_BG_COLOR;
				//if (i == 0) c = adapter.par_e.BOOKMARKS_FOLDER_DEFULT_BG_COLOR;
				Root.p[ 0 ].RestoreGUI();
				var newC2 = Tools.PICKER(rect, "Background Color", c);
				Root.p[ 0 ].ChangeGUI();
				if ( c != newC2 )
				{
					GUI.changed = true;
					if ( bm_inst == null )
					{
						HierarchyTempSceneDataGetter.SetUndoListStart( "Apply Bookmarks Color" );
						HierarchyTempSceneDataGetter.SetUndoList( scene, HierarchyTempSceneDataGetter.UNDO_TYPE.BOOKMARK );

						list[ i ].BgColor = newC2;
						HierarchyTempSceneDataGetter.SaveBookOrExpand( SaverType.Bookmarks, SceneManager.GetActiveScene() );
						//Root.p[ 0 ].MarkSceneDirty( SceneManager.GetActiveScene() );
						//if (!SceneManager.GetActiveScene().isDirty) EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
						Root.p[ 0 ].RepaintWindowInUpdate( 0 );
						Root.p[ 0 ].RepaintWindowInUpdate( 1 );

						HierarchyTempSceneDataGetter.SetDirtyList();

					}
					else
					{
						HierarchyCommonData.Instance().SetUndo( "Apply Project Bookmarks Color" );
						list[ i ].BgColor = newC2;
						HierarchyCommonData.Instance().SetDirty();
					}

				}
			}

			Root.p[ 0 ].RestoreGUI();
		}












		//static BookObject.BookmarksforGameObjectsModInstance bo_inst;
		//Scene scene_i;



		internal static CategoriesColorsWindowOld InitForGameObjectsBookmarks( MousePos rect, Window win, Scene scene )
		{ //  Debug.Log("ASD");
		  // if (adapter.GET_SKIN() == null) adapter.GET_SKIN() = Adapter.GET_SKIN();

			/*var oldL = adapter.GET_SKIN().label.fontSize;
			adapter.GET_SKIN().label.fontSize = adapter.WINDOW_FONT_10();*/
			if ( rect.type != MousePos.Type.ColorChanger_230_0 )
			{
				Debug.LogWarning( "Mismatch type" );
				rect.SetType( MousePos.Type.ColorChanger_230_0 );
			}

			if ( win == null || !win.Instance )
			{
				win = Window._windowsList.FirstOrDefault( v => v.pluginID == 0 && v.thisIsAFirstWindow );
				if ( win == null || !win.Instance ) return null;
			}

			//bo_inst = _bm_inst;
			var list = DrawButtonsOld.GET_BOOKMARKS(scene);
			// rect = InputData.WidnwoRect(new Vector2(rect.x, rect.y - list.Count * 23 + 32), WIDTH, list.Count * 23 + 32, adapter, disableClamp: true);

			rect.Height = list.Count * 23 + 32 + 10;
			rect.Y -= rect.Height + 40;

			var w = (CategoriesColorsWindowOld)private_Init(rect, typeof(CategoriesColorsWindowOld), win, "Background Colors");
			w.scene = scene.GetHashCode();
			//w.scene_i = scene;
			w.getter = () => DrawButtonsOld.GET_BOOKMARKS( scene ).Select( i => (IBgColor)i ).ToList();
			return w;
		}
	}




}
/*  }
}*/
