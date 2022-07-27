﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{

    public class TreeViewDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        Button mScrollToButton;
        Button mExpandAllButton;
        Button mCollapseAllButton;
        InputField mScrollToInputItem;
        InputField mScrollToInputChild;
        Button mBackButton;
        // an helper class for TreeView item showing.
        TreeViewItemCountMgr mTreeItemCountMgr = new TreeViewItemCountMgr();
        // Use this for initialization
        void Start()
        {
            int count = TreeViewDataSourceMgr.Get.TreeViewItemCount;
            //tells mTreeItemCountMgr there are how many TreeItems and every TreeItem has how many ChildItems.
            for (int i = 0; i < count; ++i)
            {
                int childCount = TreeViewDataSourceMgr.Get.GetItemDataByIndex(i).ChildCount;
                //second param "true" tells mTreeItemCountMgr this TreeItem is in expand status, that is to say all its children are showing.
                mTreeItemCountMgr.AddTreeItem(childCount, true);
            }


            //initialize the InitListView
            //mTreeItemCountMgr.GetTotalItemAndChildCount() return the total items count in the TreeView, include all TreeItems and all TreeChildItems.
            mLoopListView.InitListView(mTreeItemCountMgr.GetTotalItemAndChildCount(), OnGetItemByIndex);

            mExpandAllButton = GameObject.Find("ButtonPanel/buttonGroup1/ExpandAllButton").GetComponent<Button>();
            mScrollToButton = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToButton").GetComponent<Button>();
            mCollapseAllButton = GameObject.Find("ButtonPanel/buttonGroup3/CollapseAllButton").GetComponent<Button>();
            mScrollToInputItem = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputFieldItem").GetComponent<InputField>();
            mScrollToInputChild = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputFieldChild").GetComponent<InputField>();
            mScrollToButton.onClick.AddListener(OnJumpBtnClicked);
            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackBtnClicked);
            mExpandAllButton.onClick.AddListener(OnExpandAllBtnClicked);
            mCollapseAllButton.onClick.AddListener(OnCollapseAllBtnClicked);


        }

        void OnBackBtnClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }


        //when a TreeItem or TreeChildItem is getting in the scrollrect viewport, 
        //this method will be called with the item’ index as a parameter, 
        //to let you create the item and update its content.
        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0)
            {
                return null;
            }

            /*to check the index'th item is a TreeItem or a TreeChildItem.for example,
             
             0  TreeItem0
             1      TreeChildItem0_0
             2      TreeChildItem0_1
             3      TreeChildItem0_2
             4      TreeChildItem0_3
             5  TreeItem1
             6      TreeChildItem1_0
             7      TreeChildItem1_1
             8      TreeChildItem1_2
             9  TreeItem2
             10     TreeChildItem2_0
             11     TreeChildItem2_1
             12     TreeChildItem2_2

             the first column value is the param 'index', for example, if index is 1,
             then we should return TreeChildItem0_0 to SuperScrollView, and if index is 5,
             then we should return TreeItem1 to SuperScrollView
            */

            TreeViewItemCountData countData = mTreeItemCountMgr.QueryTreeItemByTotalIndex(index);
            if(countData == null)
            {
                return null;
            }
            int treeItemIndex = countData.mTreeItemIndex;
            TreeViewItemData treeViewItemData = TreeViewDataSourceMgr.Get.GetItemDataByIndex(treeItemIndex);
            if (countData.IsChild(index) == false)// if is a TreeItem
            {
                //get a new TreeItem
                LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab1");
                ListItem12 itemScript = item.GetComponent<ListItem12>();
                if (item.IsInitHandlerCalled == false)
                {
                    item.IsInitHandlerCalled = true;
                    itemScript.Init();
                    itemScript.SetClickCallBack(this.OnExpandClicked);
                }
                //update the TreeItem's content
                itemScript.mText.text = treeViewItemData.mName;
                itemScript.SetItemData(treeItemIndex, countData.mIsExpand);
                return item;
            }
            else // if is a TreeChildItem
            {
                //childIndex is from 0 to ChildCount.
                //for example, TreeChildItem0_0 is the 0'th child of TreeItem0
                //and TreeChildItem1_2 is the 2'th child of TreeItem1
                int childIndex = countData.GetChildIndex(index);
                ItemData itemData = treeViewItemData.GetChild(childIndex);
                if (itemData == null)
                {
                    return null;
                }
                //get a new TreeChildItem
                LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab2");
                ListItem13 itemScript = item.GetComponent<ListItem13>();
                if (item.IsInitHandlerCalled == false)
                {
                    item.IsInitHandlerCalled = true;
                    itemScript.Init();
                }
                //update the TreeChildItem's content
                itemScript.SetItemData(itemData, treeItemIndex, childIndex);
                return item;
            }
            
        }
        public void OnExpandClicked(int index)
        {
            mTreeItemCountMgr.ToggleItemExpand(index);
            mLoopListView.SetListItemCount(mTreeItemCountMgr.GetTotalItemAndChildCount(),false);
            mLoopListView.RefreshAllShownItem();
        }
        void OnJumpBtnClicked()
        {
            int itemIndex = 0;
            int childIndex = 0;
            int finalIndex = 0;
            if (int.TryParse(mScrollToInputItem.text, out itemIndex) == false)
            {
                return;
            }
            if (int.TryParse(mScrollToInputChild.text, out childIndex) == false)
            {
                childIndex = 0;
            }
            if (childIndex < 0)
            {
                childIndex = 0;
            }
            TreeViewItemCountData itemCountData = mTreeItemCountMgr.GetTreeItem(itemIndex);
            if(itemCountData == null)
            {
                return;
            }
            int childCount = itemCountData.mChildCount;
            if (itemCountData.mIsExpand == false || childCount == 0 || childIndex == 0)
            {
                finalIndex = itemCountData.mBeginIndex;
            }
            else
            {
                if(childIndex > childCount)
                {
                    childIndex = childCount;
                }
                if (childIndex < 1)
                {
                    childIndex = 1;
                }
                finalIndex = itemCountData.mBeginIndex + childIndex;
            }
            mLoopListView.MovePanelToItemIndex(finalIndex, 0);
        }

        void OnExpandAllBtnClicked()
        {
            int count = mTreeItemCountMgr.TreeViewItemCount;
            for (int i = 0; i < count; ++i)
            {
                mTreeItemCountMgr.SetItemExpand(i, true);
            }
            mLoopListView.SetListItemCount(mTreeItemCountMgr.GetTotalItemAndChildCount(), false);
            mLoopListView.RefreshAllShownItem();
        }

        void OnCollapseAllBtnClicked()
        {
            int count = mTreeItemCountMgr.TreeViewItemCount;
            for (int i = 0; i < count; ++i)
            {
                mTreeItemCountMgr.SetItemExpand(i, false);
            }
            mLoopListView.SetListItemCount(mTreeItemCountMgr.GetTotalItemAndChildCount(), false);
            mLoopListView.RefreshAllShownItem();
        }

    }

}
