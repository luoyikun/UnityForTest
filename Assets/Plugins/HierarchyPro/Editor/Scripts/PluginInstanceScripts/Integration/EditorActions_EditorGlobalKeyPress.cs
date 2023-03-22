using UnityEngine;
using UnityEditor;



namespace EMX.HierarchyPlugin.Editor
{



	internal partial class PluginInstance
	{





		internal void EditorGlobalKeyPress()
		{
			/*  Debug.Log( Event.current );
              if ( Event.current != null && Event.current.rawType == EventType.MouseUp )
              {   if ( OnRawMouseUp != null )
                  {   OnRawMouseUp();
                      OnRawMouseUp = null;
                  }
              }*/

			if ( Root.TemperaryPluginDisabled ) return;


			bool used = false;

			if (Event.current.type == EventType.KeyDown && par_e.RIGHTARROW_EXPANDS_HOVERED && (Event.current.keyCode == KeyCode.RightArrow || Event.current.keyCode == KeyCode.LeftArrow))
			{
				if (hashoveredItem && hoverID != -1)
				{
					used = true;
					if (!ha._IsSelectedCache.ContainsKey(hoverID))
					{
						EXPAND_SWITCHER(Cache.GetHierarchyObjectByInstanceID(hoverID, null), Event.current.keyCode == KeyCode.RightArrow ? true : false);
					}

					else
					{
						foreach (var item in ha._IsSelectedCache)
						{
							EXPAND_SWITCHER(Cache.GetHierarchyObjectByInstanceID(item.Key, null), Event.current.keyCode == KeyCode.RightArrow ? true : false);
						}
					}

					Tools.EventUse();
				}
			}


			if (!Application.isPlaying)
			{

				if (Event.current != null && Event.current.rawType == EventType.KeyDown)
				{
					// bool used = false;

					if (Event.current.keyCode == KeyCode.Escape && !SKIP_PREFAB_ESCAPE)
					{
						if (!par_e.ESCAPE_CLOSES_PREFABMODE || !EditorWindow.focusedWindow) goto skipPrefab;

						if (!par_e.CLOSE_PREFAB_KEY_FORALL_WINDOWS)
						{
							var n = EditorWindow.focusedWindow.GetType().Name;
							var res = false;
							res |= n.Contains("SceneView");
							res |= n.Contains("Inspector");
							res |= n.Contains("GameView");
							res |= n.Contains("SceneHierarchy");
							res |= n.Contains("ProjectBrowser");

							if (!res) goto skipPrefab;
						}

						ha.CLOSE_PREFAB_MODE();
					skipPrefab:;
						used = true;
					}

					/*if ( !used )
                     {
                         if ( Event.current.keyCode != KeyCode.None && EditorWindow.focusedWindow )
                         {
                             var n = EditorWindow.focusedWindow.GetType().Name;

                             if ( n.Contains( "SceneHierarchy" ) ) goto skipHier;

                             foreach ( var item in CUSTOMMENU_HOTKEYS_WINDOWS )
                             {
                                 if ( IS_HIERARCHY() && item.Key == "SceneHierarchy" ) continue;

                                 if ( IS_PROJECT() && item.Key == "ProjectBrowser" ) continue;

                                 if ( n.Contains( item.Key ) )
                                 {
                                     customMenuModules.CheckKeyDown( Event.current.control, Event.current.shift, Event.current.alt, Event.current.keyCode );
                                 }
                             }
                         }

                         skipHier:;
                     }*/
				}

				if (_OnGlobalKeyPressed != null) _OnGlobalKeyPressed(used);

			}


		}



	}
}
