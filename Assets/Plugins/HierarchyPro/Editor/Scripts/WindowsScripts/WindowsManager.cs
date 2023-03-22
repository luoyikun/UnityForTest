using System.Linq;

using EMX.HierarchyPlugin.Editor.Windows;



namespace EMX.HierarchyPlugin.Editor
{

	internal class WindowsManager //WindowsTools
	{


		int pluginID;
		internal WindowsManager(int pId)
		{
			pluginID = pId;

		}


		internal void CloseWindows()
		{
			foreach (var w in IWindow.__inputWindow.ToDictionary(t => t.Key, v => v.Value))
			{
				if (!w.Value)
				{
					IWindow.__inputWindow.Remove(w.Key);
					continue;
				}
				if (w.Value is SearchWindow || w.Value is InputWindow
#if !EMX_H_LITE
					|| w.Value is Root_HighlighterWindowInterface
#endif
                    ) ((IWindow)w.Value).CloseThis();
			}
		}

		internal int InputWindowsCount()
		{
			int res = 0;
			foreach (var w in IWindow.__inputWindow)
			{
				if (!w.Value) continue;
				if (w.Value is SearchWindow || w.Value is InputWindow 
#if !EMX_H_LITE
					|| w.Value is Root_HighlighterWindowInterface
#endif
					) res++;
			}
			return res;
		}
	}
}
