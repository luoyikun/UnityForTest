using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace EMX.HierarchyPlugin.Editor.Mods
{


	/*class HyperGraphDrawContainer : ExternalDrawContainer
	{


		public override bool MAIN
		{
			get { return true; }
		}

		internal override bool hide_hierarchy_ui_buttons
		{
			get { return false; }
		}

		internal override float HEIGHT
		{
			get { return adapter.par.HiperGraphParams.HEIGHT; }

			set { adapter.par.HiperGraphParams.HEIGHT = value; }
		}

		internal override float WIDTH
		{
			get { return adapter.window() == null ? 0 : adapter.window().position.width; }

			set { }
		}

	}*/


	public class ExternalDrawContainer : IEqualityComparer<ExternalDrawContainer>
    {
        internal PluginInstance adapter { get { return Root.p[ 0 ]; } }

        internal int pluginID;
        //internal MemType type;
        internal int id;
        static int _id = 10001;
        internal int controller_type = 0;

        internal ExternalDrawContainer( int pluginID )
        {
            this.pluginID = pluginID;
            id = _id++;
        }

        public override int GetHashCode()
        {
            return id;
        }
        static bool _Equals( ExternalDrawContainer x, ExternalDrawContainer y )
        {
            if ( ReferenceEquals( x, null ) && ReferenceEquals( y, null ) ) return true;
            if ( ReferenceEquals( x, null ) || ReferenceEquals( y, null ) ) return false;
            return x.id == y.id;
        }
        bool IEqualityComparer<ExternalDrawContainer>.Equals( ExternalDrawContainer x, ExternalDrawContainer y )
        {
            return _Equals( x, y );
        }
        int IEqualityComparer<ExternalDrawContainer>.GetHashCode( ExternalDrawContainer obj )
        {
            if ( ReferenceEquals( obj, null ) ) return -1;
            return obj.GetHashCode();
        }
        public override bool Equals( object obj )
        {
            return _Equals( this, obj as ExternalDrawContainer );
        }
        // internal abstract void Repaint();

        internal bool wasInit = false;
        internal virtual bool hide_hierarchy_ui_buttons { get; }
        internal virtual float HEIGHT(int i) {return !ReferenceEquals( tempRoot, null ) && tempRoot.OvValide() ? tempRoot.position(i).height : 0;  }

        internal virtual float WIDTH( int i ) { return !ReferenceEquals( tempRoot, null ) && tempRoot.OvValide() ? tempRoot.position( i ).width : 0; }
		internal virtual Rect? RECT( int i) { return !ReferenceEquals( tempRoot, null ) && tempRoot.OvValide() ? tempRoot.position(i) : (Rect?)null;  }
        // internal abstract float DEFAULT_WIDTH(UniversalGraphController controller);

        internal Vector2 scrollPos = new Vector2();
        Func<bool, float, bool> m_currentAction;
        internal IExternalWindow tempRoot;

        public virtual bool MAIN {
            get { return controller_type == 1; }
        }



        //static Dictionary<int, int> index_session_cache
        internal virtual int GetCategoryIndex( Scene scene )
        {

            var SH = scene.GetHashCode();
            HierarchyTempSceneDataGetter.TryToInitBookOrExpand( SaverType.Bookmarks, scene );
            var index = !ReferenceEquals(tempRoot, null) && tempRoot.OvValide() ? SessionState.GetInt("EMX_BookMarks_Win_" + SH, -1) : SessionState.GetInt("EMX_BookMarks_Temp_" + SH, -1);


            var sd = HierarchyTempSceneData.InstanceFast(scene);

            if ( index == -1 )
            {
                //	var sd = HierarchyExternalSceneData.GetHierarchyExternalSceneData(scene);
                //	index = sd.BookMarksGlobal.FindIndex(b => b.category_name == mem);
                //	index = Mathf.Clamp(index, 0, sd.BookMarksGlobal.Count);
                if ( !ReferenceEquals( tempRoot, null ) && tempRoot.OvValide() )
                {
                    var mem = tempRoot.titleContent.text;
                    index = sd.BookMarkCategory_Temp.FindIndex( b => b.category_name == mem );
                    index = Mathf.Clamp( index, 0, sd.BookMarkCategory_Temp.Count - 1 );
                    SessionState.SetInt( "EMX_BookMarks_Win_" + SH, index );
                }
                else
                {
                    index = Mathf.Clamp( index, 0, sd.BookMarkCategory_Temp.Count - 1 );
                    SessionState.SetInt( "EMX_BookMarks_Temp_" + SH, index );
                }
            }
            return Mathf.Clamp( index, 0, sd.BookMarkCategory_Temp.Count - 1 );
        }

        internal virtual void SetCategoryIndex( int index, Scene scene )
        {
            /*	var sd = HierarchyExternalSceneData.GetHierarchyExternalSceneData(scene);
				index = Mathf.Clamp(index, 0, sd.BookMarksGlobal.Count);
				SessionState.SetInt("EMX/BookMarks/" + scene, index);*/
            HierarchyTempSceneDataGetter.TryToInitBookOrExpand( SaverType.Bookmarks, scene );
            var sd = HierarchyTempSceneData.InstanceFast(scene);
            index = Mathf.Clamp( index, 0, sd.BookMarkCategory_Temp.Count - 1 );
            var SH = scene.GetHashCode();
            if ( !ReferenceEquals( tempRoot, null ) && tempRoot.OvValide() ) SessionState.SetInt( "EMX_BookMarks_Win_" + SH, index );
            else SessionState.SetInt( "EMX_BookMarks_Temp_" + SH, index );
            CHECK_CONTENT( sd.BookMarkCategory_Temp[ index ].category_name, "BOOKMARKS_ICON", tempRoot );
            adapter.RepaintExternalNow();
        }
        internal virtual string GetCurerentCategoryName( Scene scene )
        {
            var sd = HierarchyTempSceneData.InstanceFast(scene);
            var index = GetCategoryIndex(scene);
            //	var sd = HierarchyExternalSceneData.GetHierarchyExternalSceneData(scene);
            CHECK_CONTENT( sd.BookMarkCategory_Temp[ index ].category_name, "BOOKMARKS_ICON", tempRoot );
            return sd.BookMarkCategory_Temp[ index ].category_name;
        }
        internal virtual bool SetCurerentCategoryName( Scene scene, string name )
        {
            if ( string.IsNullOrEmpty( name ) ) return false;
            name = name.Replace( '|', '/' );

            HierarchyTempSceneDataGetter.TryToInitBookOrExpand( SaverType.Bookmarks, scene );
            var sd = HierarchyTempSceneData.InstanceFast(scene);
            //	var sd = HierarchyExternalSceneData.GetHierarchyExternalSceneData(scene);
            var finded = sd.BookMarkCategory_Temp.FindIndex(b => b.category_name == name);
            if ( finded != -1 ) return false;

            var index = GetCategoryIndex(scene);
            if ( sd.BookMarkCategory_Temp[ index ].category_name == name ) return false;

            HierarchyTempSceneDataGetter.SetUndoListStart( "Apply Bookmarks Name" );
            HierarchyTempSceneDataGetter.SetUndoList( scene, HierarchyTempSceneDataGetter.UNDO_TYPE.BOOKMARK );

            //HierarchyExternalSceneData.Undo(scene, "Set Category Name");
            sd.BookMarkCategory_Temp[ index ].category_name = name;
            HierarchyTempSceneDataGetter.SaveBookOrExpand( SaverType.Bookmarks, scene );
            //HierarchyExternalSceneData.SetDirtyFile(scene);
            CHECK_CONTENT( sd.BookMarkCategory_Temp[ index ].category_name, "BOOKMARKS_ICON", tempRoot );
            adapter.RepaintExternalNow();

            HierarchyTempSceneDataGetter.SetDirtyList();

            return true;
        }
        // controller.CHECK_CONTENT(controller.GetCurerentCategoryName(adapter.LastActiveScene ), );

        internal void CHECK_CONTENT( string name, string icon, IExternalWindow r )
        {
            if ( !ExternalModRoot.EQUALS( tempRoot, r ) ) tempRoot = r;
            if ( ReferenceEquals( tempRoot, null ) || !tempRoot.OvValide() ) return;
            //	Debug.Log(tempRoot.titleContent.text + " " +  name);
            if ( tempRoot.titleContent.text != name )
                tempRoot.titleContent = new GUIContent() {
                    text = name,
                    tooltip = name,
                    image = adapter.GetOldIcon( icon ).texture
                };
        }
        internal void ASIGN_CONTENT( string name, string icon, IExternalWindow r )
        {
            if ( !ExternalModRoot.EQUALS( tempRoot, r ) ) tempRoot = r;
            if ( ReferenceEquals( tempRoot, null ) || !tempRoot.OvValide() ) return;
            //	Debug.Log(tempRoot.titleContent.text + " " +  name);
            tempRoot.titleContent = new GUIContent() {
                text = name,
                tooltip = name,
                image = adapter.GetOldIcon( icon ).texture
            };
        }


        internal void RepaintNow()
        {
            if ( ReferenceEquals( tempRoot, null ) || !tempRoot.OvValide() )
            {
                //adapter.RepaintExternalNow();
                return;
            }
            tempRoot.RepaintNow();
        }

        internal Func<bool, float, bool> currentAction {
            get { return m_currentAction; }

            set {
                m_currentAction = value;

                if ( value != null && adapter != null )
                {


                    /*if (adapter.window() == tempWin)
					{
						adapter.PUSH_EVENT_HELPER_RAW();
					}

					else*/
                    {
                        //	var w = tempWin as ExternalWindowRoot;
                        //	if (w) w.PUSH_ONMOUSEUP(adapter.EVENT_HELPER_ONUP);
                        tempRoot.PUSH_ONMOUSEUP( tempRoot.PUSH_EVENT_HELPER_RAW );



                        /*if (w) w.PUSH_ONMOUSEUP(adapter.EVENT_HELPER_ONUP);
						else
						{
							var w2 = tempWin as _6__BottomWindow_HyperGraphWindow;

							if (w2) w2.PUSH_ONMOUSEUP(adapter.EVENT_HELPER_ONUP);
						}*/
                    }

                    tempRoot.RepaintNow();

                }
            }
        }
        internal bool wasDrag;

        internal void ClearAction()
        {
            var change = false;
            change |= wasDrag != false;
            wasDrag = false;
            change |= m_currentAction != null;
            m_currentAction = null;
            change |= selection_action != null;
            selection_action = null;
            change |= selection_button != -1;
            selection_button = -1;
            change |= selection_window != null;
            selection_window = null;
            change |= selection_info != false;
            selection_info = false;
            if ( change && !ReferenceEquals( tempRoot, null ) && tempRoot.OvValide() ) tempRoot.RepaintNow();
        }




        internal void CHECK_MOUSE_UP()
        {


            EVENT_MOUSE_UP_CLEAR();

            if ( currentAction != null )
            {
                currentAction( true, adapter.deltaTime );
                ClearAction();
                // GUIUtility.hotControl = 0; EMX_REMOVE
                //Tools.EventUse();
                tempRoot.RepaintNow();
            }


        }

        internal void EVENT_UPDATE( )
        {
            if ( selection_action != null )
            {
                if ( !selection_action( false, adapter.deltaTime ) )
                    tempRoot.RepaintNow();
                //REPAIN();
            }
        }

        internal void EVENT_MOUSE_UP()
        {
            if ( selection_action != null )
            {
                //MonoBehaviour.print("ASD");
                selection_action( true, adapter.deltaTime );
                ClearAction();

                //GUIUtility.hotControl = 0; EMX_REMOVE
                //Tools.EventUse();
                tempRoot.RepaintNow();
                // RepaintWindow();
            }
        }

        internal void EVENT_MOUSE_UP_CLEAR()
        {
            if ( selection_action != null ) //MonoBehaviour.print("ASD");
            {
                ClearAction();

                // GUIUtility.hotControl = 0; EMX_REMOVE
                //Tools.EventUse();

                tempRoot.RepaintNow();

                // RepaintWindow();
            }
        }


        internal Func<bool, float, bool> __selection_action;

        internal Func<bool, float, bool> selection_action {
            get { return __selection_action; }

            set {
                if ( __selection_action == value ) return;
                __selection_action = value;
                if ( value != null )
                {
                    if ( ExternalModRoot.EQUALS( tempRoot, null ) ) throw new Exception( "ASD" );
                    tempRoot.PUSH_ONMOUSEUP( tempRoot.PUSH_EVENT_HELPER_RAW );
                    tempRoot.RepaintNow();
                }
            }
        }







        internal int selection_button = -1;
        internal IExternalWindow selection_window;
        internal Rect lastRect;
        internal bool selection_info;
        internal Rect ModuleRect;
    }


}
