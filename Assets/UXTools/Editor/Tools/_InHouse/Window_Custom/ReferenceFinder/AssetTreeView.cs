#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;

namespace ThunderFireUITool
{
    //带数据的TreeViewItem
    public class AssetViewItem : TreeViewItem
    {
        public ReferenceFinderData.AssetDescription data;
    }

    //资源引用树
    public class AssetTreeView : TreeView
    {
        //图标宽度
        const float kIconWidth = 18f;
        //列表高度
        const float kRowHeights = 20f;
        public AssetViewItem assetRoot;

        private GUIStyle stateGUIStyle = new GUIStyle { richText = true, alignment = TextAnchor.MiddleCenter };


        //列信息
        enum MyColumns
        {
            Name,
            Path,
            State,
            RefCount
        }

        public AssetTreeView(TreeViewState state, ClickColumn multicolumnHeader) : base(state, multicolumnHeader)
        {
            rowHeight = kRowHeights;
            columnIndexForTreeFoldouts = 0;
            showAlternatingRowBackgrounds = true;
            showBorder = false;
            customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
            extraSpaceBeforeIconAndLabel = kIconWidth;
        }
        //响应双击事件
        protected override void DoubleClickedItem(int id)
        {
            var item = (AssetViewItem)FindItem(id, rootItem);
            //在ProjectWindow中高亮双击资源
            if (item != null)
            {
                var assetObject = AssetDatabase.LoadAssetAtPath(item.data.path, typeof(UnityEngine.Object));
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = assetObject;
                EditorGUIUtility.PingObject(assetObject);
            }
        }

        protected override void ExpandedStateChanged()
        {
            SortExpandItem();
        }

        public void SortExpandItem()
        {
            if (SortHelper.curSortType == SortType.None)
            {
                return;
            }
            var expandItemList = GetExpanded();
            foreach (var i in expandItemList)
            {
                var item = (AssetViewItem)FindItem(i, rootItem);
                SortHelper.SortChild(item.data);
            }
            ReferenceInfo curWindow = EditorWindow.GetWindow<ReferenceInfo>();
            curWindow.needUpdateAssetTree = true;
        }

        //生成ColumnHeader
        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth, bool isDepend)
        {
            var columns = new List<MultiColumnHeaderState.Column>
            {
			//图标+名称
			new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("名字"),
                headerTextAlignment = TextAlignment.Center,
                sortedAscending = false,
                width = 200,
                minWidth = 60,
                autoResize = false,
                allowToggleVisibility = false,
                canSort = true,
                sortingArrowAlignment = TextAlignment.Center
            },
			//路径
			new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("路径"),
                headerTextAlignment = TextAlignment.Center,
                sortedAscending = false,
                width = 360,
                minWidth = 60,
                autoResize = false,
                allowToggleVisibility = false,
                canSort = true,
                sortingArrowAlignment = TextAlignment.Center
            },
			//状态
			new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("状态"),
                headerTextAlignment = TextAlignment.Center,
                sortedAscending = false,
                width = 60,
                minWidth = 60,
                autoResize = false,
                allowToggleVisibility = true,
                canSort = false
            },
        };
            if (!isDepend)
            {
                columns.Add(
                   //引用数
                   new MultiColumnHeaderState.Column
                   {
                       headerContent = new GUIContent("引用数"),
                       headerTextAlignment = TextAlignment.Center,
                       sortedAscending = false,
                       width = 60,
                       minWidth = 60,
                       autoResize = true,
                       allowToggleVisibility = true,
                       canSort = false
                   });
            }
            var state = new MultiColumnHeaderState(columns.ToArray());
            return state;
        }

        protected override TreeViewItem BuildRoot()
        {
            return assetRoot;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (AssetViewItem)args.item;
            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), item, (MyColumns)args.GetColumn(i), ref args);
            }
        }

        //绘制列表中的每项内容
        void CellGUI(Rect cellRect, AssetViewItem item, MyColumns column, ref RowGUIArgs args)
        {
            CenterRectUsingSingleLineHeight(ref cellRect);
            switch (column)
            {
                case MyColumns.Name:
                    {
                        var iconRect = cellRect;
                        iconRect.x += GetContentIndent(item);
                        iconRect.width = kIconWidth;
                        if (iconRect.x < cellRect.xMax)
                        {
                            var icon = GetIcon(item.data.path);
                            if (icon != null)
                                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
                        }
                        args.rowRect = cellRect;
                        base.RowGUI(args);
                    }
                    break;
                case MyColumns.Path:
                    {
                        GUI.Label(cellRect, item.data.path);
                    }
                    break;
                case MyColumns.State:
                    {
                        GUI.Label(cellRect, ReferenceFinderData.GetInfoByState(item.data.state), stateGUIStyle);
                    }
                    break;
                case MyColumns.RefCount:
                    {
                        GUI.Label(cellRect, ReferenceInfo.data.GetRefCount(item.data, (item.parent as AssetViewItem)?.data), stateGUIStyle);
                    }
                    break;
            }
        }

        //根据资源信息获取资源图标
        private Texture2D GetIcon(string path)
        {
            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            if (obj != null)
            {
                Texture2D icon = AssetPreview.GetMiniThumbnail(obj);
                if (icon == null)
                    icon = AssetPreview.GetMiniTypeThumbnail(obj.GetType());
                return icon;
            }
            return null;
        }
    }
}
#endif
