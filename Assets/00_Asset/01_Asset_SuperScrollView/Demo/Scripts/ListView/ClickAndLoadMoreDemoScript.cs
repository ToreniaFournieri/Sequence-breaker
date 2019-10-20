using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ClickAndLoadMoreDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        LoadingTipStatus _mLoadingTipStatus = LoadingTipStatus.None;
        int _mLoadMoreCount = 20;

        Button _mScrollToButton;
        InputField _mScrollToInput;
        Button _mBackButton;
        // Use this for initialization
        void Start()
        {
            // totalItemCount +1 because the last "load more" button is also a item.
            mLoopListView.InitListView(DataSourceMgr.Get.TotalItemCount + 1, OnGetItemByIndex);
            _mScrollToButton = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToButton").GetComponent<Button>();
            _mScrollToInput = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputField").GetComponent<InputField>();
            _mScrollToButton.onClick.AddListener(OnJumpBtnClicked);
            _mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            _mBackButton.onClick.AddListener(OnBackBtnClicked);
        }

        void OnBackBtnClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }


        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0)
            {
                return null;
            }
            LoopListViewItem2 item = null;
            if (index == DataSourceMgr.Get.TotalItemCount)
            {
                item = listView.NewListViewItem("ItemPrefab0");
                if (item.IsInitHandlerCalled == false)
                {
                    item.IsInitHandlerCalled = true;
                    ListItem11 itemScript0 = item.GetComponent<ListItem11>();
                    itemScript0.mRootButton.onClick.AddListener(OnLoadMoreBtnClicked);
                }
                UpdateLoadingTip(item);
                return item;
            }
            ItemData itemData = DataSourceMgr.Get.GetItemDataByIndex(index);
            if (itemData == null)
            {
                return null;
            }
            item = listView.NewListViewItem("ItemPrefab1");
            ListItem2 itemScript = item.GetComponent<ListItem2>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            itemScript.SetItemData(itemData, index);
            return item;
        }

        void UpdateLoadingTip(LoopListViewItem2 item)
        {
            if (item == null)
            {
                return;
            }
            ListItem11 itemScript0 = item.GetComponent<ListItem11>();
            if (itemScript0 == null)
            {
                return;
            }
            if (_mLoadingTipStatus == LoadingTipStatus.None)
            {
                itemScript0.mText.text = "Click to Load More";
                itemScript0.mWaitingIcon.SetActive(false);
            }
            else if (_mLoadingTipStatus == LoadingTipStatus.WaitLoad)
            {
                itemScript0.mWaitingIcon.SetActive(true);
                itemScript0.mText.text = "Loading ...";
            }
        }
       
        void OnLoadMoreBtnClicked()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (_mLoadingTipStatus != LoadingTipStatus.None)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(DataSourceMgr.Get.TotalItemCount);
            if (item == null)
            {
                return;
            }
            _mLoadingTipStatus = LoadingTipStatus.WaitLoad;
            UpdateLoadingTip(item);
            DataSourceMgr.Get.RequestLoadMoreDataList(_mLoadMoreCount, OnDataSourceLoadMoreFinished);
        }

        void OnDataSourceLoadMoreFinished()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (_mLoadingTipStatus == LoadingTipStatus.WaitLoad)
            {
                _mLoadingTipStatus = LoadingTipStatus.None;
                mLoopListView.SetListItemCount(DataSourceMgr.Get.TotalItemCount + 1, false);
                mLoopListView.RefreshAllShownItem();
            }
        }

        void OnJumpBtnClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(_mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            if (itemIndex < 0)
            {
                return;
            }
            mLoopListView.MovePanelToItemIndex(itemIndex, 0);
        }

    }

}
