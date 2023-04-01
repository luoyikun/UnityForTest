using System;
using UnityEngine;
using UnityEditor;
using EMX.HierarchyPlugin.Editor.Mods.BookObject;

namespace EMX.HierarchyPlugin.Editor
{

	class GetExternalWindow<T> where T : Mods.ExternalModRoot
	{


		internal static void TryToClose_Or_Show(string name, params Type[] dock)
		{
			if (!lastW) Show(name, dock);
			else lastW.Close();
		}
		static T lastW;
		internal static void Show(string name, params Type[] dock)
		{

			if (!Root.p[0].par_e.ENABLE_ALL)
			{
				EditorUtility.DisplayDialog("" + Root.HierarchyPro + " disabled", "" + Root.HierarchyPro + " disabled", "Ok");
				return;
			}

			if (dock != null && dock.Length != 0)
			{
				var W = EditorWindow.GetWindow<T>(name, true, dock);
				W.Show();
				W.minSize = new Vector2(20, 20);
				W.Init(W.rootVisualElement.parent != null && W.rootVisualElement.parent.childCount > 1);
				lastW = W;
			}
			else
			{
				var W = EditorWindow.GetWindow<T>(name, true);
				W.Show();
				W.minSize = new Vector2(20, 20);
				W.Init();
				lastW = W;
			}
			if (lastW)
			{


				string icon = null;
				var Ta = typeof(T);
				if (Ta != typeof(BookmarksforGameObjectsModWindow))
				{
					foreach (var item in Root.p[0].modsController.toolBarModification.hotButtons.externalMod_Buttons)
					{
						if (item.windowType == Ta)
						{
							icon = item.icon();
							break;
						}
					}
					if (icon != null)
					{
						var tc = lastW.titleContent;
						tc.image = Root.p[0].GetOldIcon(icon, true).texture;
						lastW.titleContent = tc;
					}
				}

			}

		}
	}
}
