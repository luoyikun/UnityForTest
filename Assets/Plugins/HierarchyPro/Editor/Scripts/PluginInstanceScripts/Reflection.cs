using System;
using System.Linq;
using UnityEditor;
using System.Reflection;

namespace EMX.HierarchyPlugin.Editor
{
	internal partial class PluginInstance
    {

		void Init()
        {

            #region R
            _TreeViewController_0 = SceneHierarchyWindow(0).GetField("m_treeView", ~(BindingFlags.Static | BindingFlags.InvokeMethod));
            if (_TreeViewController_0 == null) if ((_TreeViewController_0 = SceneHierarchyWindow(0).GetField("m_TreeView", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("m_TreeView");
           // _TreeViewController_1 = SceneHierarchyWindow(1).GetField("m_treeView", ~(BindingFlags.Static | BindingFlags.InvokeMethod));
           // if (_TreeViewController_1 == null) if ((_TreeViewController_1 = _ProjectWindow.GetField("m_TreeView", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("m_TreeView");


            //  if (pluginID != 0)
            {
                if ((_AssetsView = ProjectBrowserWindowType.GetField("m_AssetTree", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("m_AssetTree");
                if ((_AssetTreeState = ProjectBrowserWindowType.GetField("m_AssetTreeState", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("m_AssetTreeState");
                if ((_FoldView = ProjectBrowserWindowType.GetField("m_FolderTree", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("m_FolderTree");
                if ((_FolderTreeState = ProjectBrowserWindowType.GetField("m_FolderTreeState", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("m_FolderTreeState");
                if ((_ViewMode = ProjectBrowserWindowType.GetField("m_ViewMode", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("m_ViewMode");
                m_SearchFieldText = ProjectBrowserWindowType.GetField("m_SearchFieldText", ~(BindingFlags.Static | BindingFlags.InvokeMethod))??throw new Exception("m_SearchFieldText");
                m_SearchFilter = ProjectBrowserWindowType.GetField("m_SearchFilter", ~(BindingFlags.Static | BindingFlags.InvokeMethod))??throw new Exception("m_SearchFilter");
                IsSearching = m_SearchFilter.FieldType.GetMethod("IsSearching", ~(BindingFlags.Static | BindingFlags.SetField))??throw new Exception("IsSearching");


                // if ((GetMainAssetOrInProgressProxyInstanceID = typeof(AssetDatabase).GetMethod("GetMainAssetOrInProgressProxyInstanceID", ~(BindingFlags.Instance | BindingFlags.GetField))) == null) throw new Exception("GetMainAssetOrInProgressProxyInstanceID");
                // if ((GetInstanceIDFromGUID = typeof(AssetDatabase).GetMethod("GetInstanceIDFromGUID", ~(BindingFlags.Static | BindingFlags.GetField))) == null) throw new Exception("GetInstanceIDFromGUID");
                // if (GetInstanceIDFromGUID == null)if ((GetMainAssetInstanceIDFromPath = typeof(AssetDatabase).GetMethod("GetMainAssetInstanceID", ~(BindingFlags.Static | BindingFlags.GetField))) == null) throw new Exception("GetMainAssetInstanceID");
            }
            if ((_data = m_TreeViewFieldInfo.FieldType.GetProperty("data", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("data");
            if ((_gui = m_TreeViewFieldInfo.FieldType.GetProperty("gui", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("_gui");
            if ((_state = m_TreeViewFieldInfo.FieldType.GetProperty("state", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("state");
            if ((tree_m_TotalRect = m_TreeViewFieldInfo.FieldType.GetField("m_TotalRect", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("m_TotalRect");
            if ( (tree_m_VisibleRect = m_TreeViewFieldInfo.FieldType.GetField( "m_VisibleRect", ~(BindingFlags.Static | BindingFlags.InvokeMethod) )) == null ) throw new Exception( "m_VisibleRect" );


			s = m_TreeViewFieldInfo.FieldType.GetField( "m_StopIteratingItems", (BindingFlags)~0 );
			if ((tree_m_ContentRect = m_TreeViewFieldInfo.FieldType.GetField("m_ContentRect", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("m_ContentRect");
            if ((m_UseExpansionAnimation = m_TreeViewFieldInfo.FieldType.GetField("m_UseExpansionAnimation", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("m_UseExpansionAnimation");
            if ((tree_animatingExpansion = m_TreeViewFieldInfo.FieldType.GetProperty("animatingExpansion", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("animatingExpansion");
            if ((tree_m_ExpansionAnimator = m_TreeViewFieldInfo.FieldType.GetField("m_ExpansionAnimator", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("tree_m_ExpansionAnimator");
            if ((ExpansionAnimator_endRow = tree_m_ExpansionAnimator.FieldType.GetProperty("endRow", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("ExpansionAnimator_endRow");
            if ((ExpansionAnimator_CullRow = tree_m_ExpansionAnimator.FieldType.GetMethod("CullRow", ~(BindingFlags.Static | BindingFlags.SetField))) == null) throw new Exception("CullRow");
            var guiType = typeof(EditorWindow).Assembly.GetType("UnityEditor.IMGUI.Controls.TreeViewGUI");

            /*  if ( (gui_m_LineHeight = guiType.GetField( "m_LineHeight", ~(BindingFlags.Static | BindingFlags.InvokeMethod) )) == null ) throw new Exception( "gui_LineHeight" );
              if ( (gui_k_IndentWidth = guiType.GetField( "k_IndentWidth", ~(BindingFlags.Static | BindingFlags.InvokeMethod) )) == null ) throw new Exception( "k_IndentWidth" );
              if ( (gui_k_IconWidth = guiType.GetField( "k_IconWidth", ~(BindingFlags.Static | BindingFlags.InvokeMethod) )) == null ) throw new Exception( "k_IconWidth" );
              if ( (gui_customFoldoutYOffset = guiType.GetField( "customFoldoutYOffset", ~(BindingFlags.Static | BindingFlags.InvokeMethod) )) == null ) throw new Exception( "customFoldoutYOffset" );*/
            if ((gui_getFirstAndLastRowVisible = guiType.GetMethod("GetFirstAndLastRowVisible", ~(BindingFlags.Static | BindingFlags.SetField))) == null) throw new Exception("_getFirstAndLastRowVisible");
            if ((gui_GetRowRect = guiType.GetMethod("GetRowRect", ~(BindingFlags.Static | BindingFlags.SetField))) == null) throw new Exception("gui_GetRowRect");
            var dataType = typeof(EditorWindow).Assembly.GetType("UnityEditor.IMGUI.Controls.ITreeViewDataSource");
            if ((data_FindItem_Slow = dataType.GetMethod("FindItem", ~(BindingFlags.Static | BindingFlags.SetField))) == null) throw new Exception("FindItem");
            if ((data_GetItemRowFast = dataType.GetMethod("GetItem", ~(BindingFlags.Static | BindingFlags.SetField))) == null) throw new Exception("GetItem");
            if ((data_m_RootItem = dataType.GetProperty("root", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("root");
            if ((data_rowCount = dataType.GetProperty("rowCount", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("rowCount");
            //  if ( (data_m_Rows = dataType.GetField( "m_Rows", (BindingFlags)(-1) )) == null ) throw new Exception( "m_Rows" );
            if ((data_GetRows = dataType.GetMethod("GetRows", ~(BindingFlags.Static | BindingFlags.SetField))) == null) throw new Exception("m_Rows");
            var mtds = dataType.GetMethods().ToArray();
            //data_m_SetExpandedIDs = m.First( m => m.Name == "SetExpandedIDs" );
            if ((data_m_dataSetExpanded = mtds.First(de => de.Name == "SetExpanded" && de.GetParameters().Any(p => p.ParameterType == typeof(int)))) == null) throw new Exception("SetExpanded");
            if ((data_m_dataIsExpanded = mtds.First(de => de.Name == "IsExpanded" && de.GetParameters().Any(p => p.ParameterType == typeof(int)))) == null) throw new Exception("IsExpanded");
            if ((data_m_dataSetExpandWithChildrens = mtds.First(de => de.Name == "SetExpandedWithChildren" && de.GetParameters().Any(p => p.ParameterType == typeof(int)))) == null) throw new Exception("IsExpanded");
            if ((hierwin_DuplicateGO = SceneHierarchyWindow(0).GetMethod("DuplicateGO", ~(BindingFlags.Static | BindingFlags.SetField))) == null) throw new Exception("DuplicateGO");

            var stateType = typeof(EditorWindow).Assembly.GetType("UnityEditor.IMGUI.Controls.TreeViewState");
            if ((state_scrollPos = stateType.GetField("scrollPos", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("state_scrollPos");
            var guiClipType = typeof(UnityEngine.GUI).Assembly.GetType("UnityEngine.GUIClip");
            if ((guiclip_visibleRect = guiClipType.GetProperty("visibleRect", ~(BindingFlags.Instance | BindingFlags.InvokeMethod))) == null) throw new Exception("visibleRect");
            hoveredItem = m_TreeViewFieldInfo.FieldType.GetProperty("hoveredItem", ~(BindingFlags.Static | BindingFlags.InvokeMethod));
#if UNITY_2019_3_OR_NEWER
            if (hoveredItem == null) throw new Exception("hoveredItem");
#endif
            hashoveredItem = pluginID == 0 && hoveredItem != null;

            var GameObjectTreeViewItemType = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameObjectTreeViewItem") ?? throw new Exception("UnityEditor.GameObjectTreeViewItem");
            showPrefabModeButton = GameObjectTreeViewItemType.GetProperty("showPrefabModeButton", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("showPrefabModeButton");
            var GameObjectTreeViewGUIType = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameObjectTreeViewGUI") ?? throw new Exception("UnityEditor.GameObjectTreeViewGUI");
            PrefabModeButton = GameObjectTreeViewGUIType.GetMethod("PrefabModeButton", ~(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Static)) ?? throw new Exception("PrefabModeButton");


            // tree_m_KeyboardControlIDField = m_TreeViewFieldInfo.FieldType.GetField( "m_KeyboardControlID", ~(BindingFlags.Static | BindingFlags.InvokeMethod) );
            // if ( tree_m_KeyboardControlIDField == null ) if ( (tree_m_KeyboardControlIDField = m_TreeViewFieldInfo.FieldType.GetField( "m_TreeViewKeyboardControlID", ~(BindingFlags.Static | BindingFlags.InvokeMethod) )) == null ) throw new Exception( "m_TreeViewKeyboardControlID" );
            #endregion
        }
    }
}
