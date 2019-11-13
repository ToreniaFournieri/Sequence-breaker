using _00_Asset._01_SuperScrollView.Scripts;
using _00_Asset._01_SuperScrollView.Scripts.Common;
using _00_Asset._01_SuperScrollView.Scripts.ListView;
using SequenceBreaker._01_Data.Items.Item;
using SequenceBreaker._04_Home.EquipView.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker._04_Home.EquipView.Character
{
    public sealed class CharacterTreeViewWithStickyHeadScript : MonoBehaviour
    {

        public LoopListView2 mLoopListView;
        Button _mScrollToButton;
        Button _mExpandAllButton;
        Button _mCollapseAllButton;
        InputField _mScrollToInputItem;
        InputField _mScrollToInputChild;
        Button _mBackButton;
        // an helper class for TreeView item showing.
        TreeViewItemCountMgr _mTreeItemCountMgr = new TreeViewItemCountMgr();
        //the sticky head item
        public InventoryListItemSubTitle mStickeyHeadItem;
        RectTransform _mStickeyHeadItemRf;
        float _mStickeyHeadItemHeight = -1;



        // Use this for initialization
        void Start()
        {
            _mTreeItemCountMgr.Clear();
            int count = CharacterTreeViewDataSourceMgr.Get.TreeViewItemCount;
            //tells mTreeItemCountMgr there are how many TreeItems and every TreeItem has how many ChildItems.
            for (int i = 0; i < count; ++i)
            {
                int childCount = CharacterTreeViewDataSourceMgr.Get.GetItemDataByIndex(i).ChildCount;
                //second param "true" tells mTreeItemCountMgr this TreeItem is in expand status, that is to say all its children are showing.
                _mTreeItemCountMgr.AddTreeItem(childCount, true);
            }
            //initialize the InitListView
            //mTreeItemCountMgr.GetTotalItemAndChildCount() return the total items count in the TreeView, include all TreeItems and all TreeChildItems.
            mLoopListView.InitListView(_mTreeItemCountMgr.GetTotalItemAndChildCount(), OnGetItemByIndex);

            //mExpandAllButton = GameObject.Find("ButtonPanel/buttonGroup1/ExpandAllButton").GetComponent<Button>();
            //mScrollToButton = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToButton").GetComponent<Button>();
            //mCollapseAllButton = GameObject.Find("ButtonPanel/buttonGroup3/CollapseAllButton").GetComponent<Button>();
            //mScrollToInputItem = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputFieldItem").GetComponent<InputField>();
            //mScrollToInputChild = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputFieldChild").GetComponent<InputField>();
            //mScrollToButton.onClick.AddListener(OnJumpBtnClicked);
            //mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            //mBackButton.onClick.AddListener(OnBackBtnClicked);
            //mExpandAllButton.onClick.AddListener(OnExpandAllBtnClicked);
            //mCollapseAllButton.onClick.AddListener(OnCollapseAllBtnClicked);

            _mStickeyHeadItemHeight = mStickeyHeadItem.GetComponent<RectTransform>().rect.height;


            mStickeyHeadItem.Init();
            mStickeyHeadItem.SetClickCallBack(OnExpandClicked);
            _mStickeyHeadItemRf = mStickeyHeadItem.gameObject.GetComponent<RectTransform>();

            mLoopListView.ScrollRect.onValueChanged.AddListener(OnScrollContentPosChanged);
            UpdateStickeyHeadPos();

        }


        public void Initialization()
        {
            _mTreeItemCountMgr.Clear();
            int count = CharacterTreeViewDataSourceMgr.Get.TreeViewItemCount;
            //tells mTreeItemCountMgr there are how many TreeItems and every TreeItem has how many ChildItems.
            for (int i = 0; i < count; ++i)
            {
                int childCount = CharacterTreeViewDataSourceMgr.Get.GetItemDataByIndex(i).ChildCount;
                //second param "true" tells mTreeItemCountMgr this TreeItem is in expand status, that is to say all its children are showing.
                _mTreeItemCountMgr.AddTreeItem(childCount, true);
            }

            UpdateStickeyHeadPos();


            _mTreeItemCountMgr.ToggleItemExpand(0);
            OnExpandClicked(0);

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

            TreeViewItemCountData countData = _mTreeItemCountMgr.QueryTreeItemByTotalIndex(index);
            if (countData == null)
            {
                return null;
            }
            int treeItemIndex = countData.MTreeItemIndex;
            CharacterTreeViewItemData treeViewItemData = CharacterTreeViewDataSourceMgr.Get.GetItemDataByIndex(treeItemIndex);
            if (countData.IsChild(index) == false)// if is a TreeItem
            {
                //get a new TreeItem
                LoopListViewItem2 item = listView.NewListViewItem("SubTitle");
                InventoryListItemSubTitle itemScript = item.GetComponent<InventoryListItemSubTitle>();
                //ListItem12 itemScript = item.GetComponent<ListItem12>();
                if (item.IsInitHandlerCalled == false)
                {
                    item.IsInitHandlerCalled = true;
                    itemScript.Init();
                    itemScript.SetClickCallBack(OnExpandClicked);
                }
                //update the TreeItem's content
                item.UserIntData1 = treeItemIndex;
                item.UserIntData2 = 0;
                itemScript.mText.text = treeViewItemData.MName;
                itemScript.SetItemData(treeItemIndex, countData.MIsExpand);
                return item;
            }
            else// if is a TreeChildItem
            {
                //childIndex is from 0 to ChildCount.
                //for example, TreeChildItem0_0 is the 0'th child of TreeItem0
                //and TreeChildItem1_2 is the 2'th child of TreeItem1
                int childIndex = countData.GetChildIndex(index);

                Item item = treeViewItemData.GetChild(childIndex);
                //ItemData itemData = treeViewItemData.GetChild(childIndex);
                if (item == null)
                {
                    return null;
                }
                //get a new TreeChildItem
                LoopListViewItem2 loopListItem = listView.NewListViewItem("Content");
                InventoryListItemContent itemScript = loopListItem.GetComponent<InventoryListItemContent>();

                //ListItem13 itemScript = item.GetComponent<ListItem13>();
                if (loopListItem.IsInitHandlerCalled == false)
                {
                    loopListItem.IsInitHandlerCalled = true;
                    itemScript.Init();
                    itemScript.SetClickCallBack(OnItemDetailClicked);
                    //itemScript.SetClickContentCallBack(OnItemDetailClicked);

                }
                //update the TreeChildItem's content
                loopListItem.UserIntData1 = treeItemIndex;
                loopListItem.UserIntData2 = childIndex;
                itemScript.SetItemData(item, treeItemIndex, childIndex);
                return loopListItem;
            }

        }

        public void OnItemDetailClicked(Item item)
        {

        }

        public void OnExpandClicked(int index)
        {
            _mTreeItemCountMgr.ToggleItemExpand(index);
            mLoopListView.SetListItemCount(_mTreeItemCountMgr.GetTotalItemAndChildCount(), false);
            mLoopListView.RefreshAllShownItem();
        }
        void OnJumpBtnClicked()
        {
            int itemIndex = 0;
            int childIndex = 0;
            int finalIndex = 0;
            if (int.TryParse(_mScrollToInputItem.text, out itemIndex) == false)
            {
                return;
            }
            if (int.TryParse(_mScrollToInputChild.text, out childIndex) == false)
            {
                childIndex = 0;
            }
            if (childIndex < 0)
            {
                childIndex = 0;
            }
            TreeViewItemCountData itemCountData = _mTreeItemCountMgr.GetTreeItem(itemIndex);
            if (itemCountData == null)
            {
                return;
            }
            int childCount = itemCountData.MChildCount;
            if (itemCountData.MIsExpand == false || childCount == 0 || childIndex == 0)
            {
                finalIndex = itemCountData.MBeginIndex;
            }
            else
            {
                if (childIndex > childCount)
                {
                    childIndex = childCount;
                }
                if (childIndex < 1)
                {
                    childIndex = 1;
                }
                finalIndex = itemCountData.MBeginIndex + childIndex;
            }
            mLoopListView.MovePanelToItemIndex(finalIndex, _mStickeyHeadItemHeight);
        }

        void OnExpandAllBtnClicked()
        {
            int count = _mTreeItemCountMgr.TreeViewItemCount;
            for (int i = 0; i < count; ++i)
            {
                _mTreeItemCountMgr.SetItemExpand(i, true);
            }
            mLoopListView.SetListItemCount(_mTreeItemCountMgr.GetTotalItemAndChildCount(), false);
            mLoopListView.RefreshAllShownItem();
        }

        void OnCollapseAllBtnClicked()
        {
            int count = _mTreeItemCountMgr.TreeViewItemCount;
            for (int i = 0; i < count; ++i)
            {
                _mTreeItemCountMgr.SetItemExpand(i, false);
            }
            mLoopListView.SetListItemCount(_mTreeItemCountMgr.GetTotalItemAndChildCount(), false);
            mLoopListView.RefreshAllShownItem();
        }

        void UpdateStickeyHeadPos()
        {
            bool isHeadItemVisible = mStickeyHeadItem.gameObject.activeSelf;
            int count = mLoopListView.ShownItemCount;
            if (count == 0)
            {
                if (isHeadItemVisible)
                {
                    mStickeyHeadItem.gameObject.SetActive(false);
                }
                return;
            }
            LoopListViewItem2 item0 = mLoopListView.GetShownItemByIndex(0);
            Vector3 topPos0 = mLoopListView.GetItemCornerPosInViewPort(item0, ItemCornerEnum.LeftTop);

            LoopListViewItem2 targetItem = null;
            float start = topPos0.y;
            float end = start - item0.ItemSizeWithPadding;
            int targetItemShownIndex = -1;
            if (start <= 0)
            {
                if (isHeadItemVisible)
                {
                    mStickeyHeadItem.gameObject.SetActive(false);
                }
                return;
            }
            if (end < 0)
            {
                targetItem = item0;
                targetItemShownIndex = 0;
            }
            else
            {
                for (int i = 1; i < count; ++i)
                {
                    LoopListViewItem2 item = mLoopListView.GetShownItemByIndexWithoutCheck(i);
                    start = end;
                    end = start - item.ItemSizeWithPadding;
                    if (start >= 0 && end <= 0)
                    {
                        targetItem = item;
                        targetItemShownIndex = i;
                        break;
                    }
                }
            }
            if (targetItem == null)
            {
                if (isHeadItemVisible)
                {
                    mStickeyHeadItem.gameObject.SetActive(false);
                }
                return;
            }
            int itemIndex = targetItem.UserIntData1;
            int childIndex = targetItem.UserIntData2;
            TreeViewItemCountData countData = _mTreeItemCountMgr.GetTreeItem(itemIndex);
            if (countData == null)
            {
                if (isHeadItemVisible)
                {
                    mStickeyHeadItem.gameObject.SetActive(false);
                }
                return;
            }
            if (countData.MIsExpand == false || countData.MChildCount == 0)
            {
                if (isHeadItemVisible)
                {
                    mStickeyHeadItem.gameObject.SetActive(false);
                }
                return;
            }
            if (isHeadItemVisible == false)
            {
                mStickeyHeadItem.gameObject.SetActive(true);
            }
            if (mStickeyHeadItem.TreeItemIndex != itemIndex)
            {
                CharacterTreeViewItemData treeViewItemData = CharacterTreeViewDataSourceMgr.Get.GetItemDataByIndex(itemIndex);
                mStickeyHeadItem.mText.text = treeViewItemData.MName;
                mStickeyHeadItem.SetItemData(itemIndex, countData.MIsExpand);
            }
            mStickeyHeadItem.gameObject.transform.localPosition = Vector3.zero;
            float lastChildPosAbs = -end;
            float lastPadding = targetItem.Padding;
            if (lastChildPosAbs - lastPadding >= _mStickeyHeadItemHeight)
            {
                return;
            }
            for (int i = targetItemShownIndex + 1; i < count; ++i)
            {
                LoopListViewItem2 item = mLoopListView.GetShownItemByIndexWithoutCheck(i);
                if (item.UserIntData1 != itemIndex)
                {
                    break;
                }
                lastChildPosAbs += item.ItemSizeWithPadding;
                lastPadding = item.Padding;
                if (lastChildPosAbs - lastPadding >= _mStickeyHeadItemHeight)
                {
                    return;
                }
            }
            float y = _mStickeyHeadItemHeight - (lastChildPosAbs - lastPadding);
            _mStickeyHeadItemRf.anchoredPosition3D = new Vector3(0, y, 0);
        }


        void OnScrollContentPosChanged(Vector2 pos)
        {
            UpdateStickeyHeadPos();
        }




    }
}
