using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public enum LoadingTipStatus
    {
        None,
        WaitContinureDrag,
        WaitRelease,
        WaitLoad,
        Loaded,
    }

    

    public class PullToRefreshDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        LoadingTipStatus _mLoadingTipStatus = LoadingTipStatus.None;
        float _mDataLoadedTipShowLeftTime = 0;
        float _mLoadingTipItemHeight = 100;

        Button _mScrollToButton;
        Button _mAddItemButton;
        Button _mSetCountButton;
        InputField _mScrollToInput;
        InputField _mAddItemInput;
        InputField _mSetCountInput;
        Button _mBackButton;
        // Use this for initialization
        void Start()
        {
            // totalItemCount +1 because the "pull to refresh" banner is also a item.
            mLoopListView.InitListView(DataSourceMgr.Get.TotalItemCount+1, OnGetItemByIndex);
            mLoopListView.mOnBeginDragAction = OnBeginDrag;
            mLoopListView.mOnDragingAction = OnDraging;
            mLoopListView.mOnEndDragAction = OnEndDrag;
            _mSetCountButton = GameObject.Find("ButtonPanel/buttonGroup1/SetCountButton").GetComponent<Button>();
            _mScrollToButton = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToButton").GetComponent<Button>();
            _mAddItemButton = GameObject.Find("ButtonPanel/buttonGroup3/AddItemButton").GetComponent<Button>();
            _mSetCountInput = GameObject.Find("ButtonPanel/buttonGroup1/SetCountInputField").GetComponent<InputField>();
            _mScrollToInput = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputField").GetComponent<InputField>();
            _mAddItemInput = GameObject.Find("ButtonPanel/buttonGroup3/AddItemInputField").GetComponent<InputField>();
            _mScrollToButton.onClick.AddListener(OnJumpBtnClicked);
            _mAddItemButton.onClick.AddListener(OnAddItemBtnClicked);
            _mSetCountButton.onClick.AddListener(OnSetItemCountBtnClicked);
            _mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            _mBackButton.onClick.AddListener(OnBackBtnClicked);
        }

        void OnBackBtnClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }


        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView,int index)
        {
            if(index < 0 || index > DataSourceMgr.Get.TotalItemCount)
            {
                return null;
            }
            LoopListViewItem2 item = null;
            if (index == 0)
            {
                item = listView.NewListViewItem("ItemPrefab0");
                UpdateLoadingTip(item);
                return item;
            }
            int itemDataIndex = index - 1;
            ItemData itemData = DataSourceMgr.Get.GetItemDataByIndex(itemDataIndex);
            if(itemData == null)
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
            
            itemScript.SetItemData(itemData, itemDataIndex);
            return item;
        }

        void UpdateLoadingTip(LoopListViewItem2 item)
        {
            if (item == null)
            {
                return;
            }
            ListItem0 itemScript0 = item.GetComponent<ListItem0>();
            if(itemScript0 == null)
            {
                return;
            }
            if (_mLoadingTipStatus == LoadingTipStatus.None)
            {
                itemScript0.mRoot.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            }
            else if (_mLoadingTipStatus == LoadingTipStatus.WaitRelease)
            {
                itemScript0.mRoot.SetActive(true);
                itemScript0.mText.text = "Release to Refresh";
                itemScript0.mArrow.SetActive(true);
                itemScript0.mWaitingIcon.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _mLoadingTipItemHeight);
            }
            else if (_mLoadingTipStatus == LoadingTipStatus.WaitLoad)
            {
                itemScript0.mRoot.SetActive(true);
                itemScript0.mArrow.SetActive(false);
                itemScript0.mWaitingIcon.SetActive(true);
                itemScript0.mText.text = "Loading ...";
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _mLoadingTipItemHeight);
            }
            else if (_mLoadingTipStatus == LoadingTipStatus.Loaded)
            {
                itemScript0.mRoot.SetActive(true);
                itemScript0.mArrow.SetActive(false);
                itemScript0.mWaitingIcon.SetActive(false);
                itemScript0.mText.text = "Refreshed Success";
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _mLoadingTipItemHeight);
            }
        }

        void OnBeginDrag()
        {

        }

        void OnDraging()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (_mLoadingTipStatus != LoadingTipStatus.None && _mLoadingTipStatus != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(0);
            if(item == null)
            {
                return;
            }
            ScrollRect sr = mLoopListView.ScrollRect;
            Vector3 pos = sr.content.anchoredPosition3D;
            if (pos.y < -_mLoadingTipItemHeight)
            {
                if(_mLoadingTipStatus != LoadingTipStatus.None)
                {
                    return;
                }
                _mLoadingTipStatus = LoadingTipStatus.WaitRelease;
                UpdateLoadingTip(item);
                item.CachedRectTransform.anchoredPosition3D = new Vector3(0, _mLoadingTipItemHeight, 0);
            }
            else
            {
                if (_mLoadingTipStatus != LoadingTipStatus.WaitRelease)
                {
                    return;
                }
                _mLoadingTipStatus = LoadingTipStatus.None;
                UpdateLoadingTip(item);
                item.CachedRectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
            }
        }
        void OnEndDrag()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (_mLoadingTipStatus != LoadingTipStatus.None && _mLoadingTipStatus != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(0);
            if (item == null)
            {
                return;
            }
            mLoopListView.OnItemSizeChanged(item.ItemIndex);
            if (_mLoadingTipStatus != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            _mLoadingTipStatus = LoadingTipStatus.WaitLoad;
            UpdateLoadingTip(item);
            DataSourceMgr.Get.RequestRefreshDataList(OnDataSourceRefreshFinished);
        }

        void OnDataSourceRefreshFinished()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (_mLoadingTipStatus == LoadingTipStatus.WaitLoad)
            {
                _mLoadingTipStatus = LoadingTipStatus.Loaded;
                _mDataLoadedTipShowLeftTime = 0.7f;
                LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(0);
                if (item == null)
                {
                    return;
                }
                UpdateLoadingTip(item);
                mLoopListView.RefreshAllShownItem();
            }
        }

        void Update()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (_mLoadingTipStatus == LoadingTipStatus.Loaded)
            {
                _mDataLoadedTipShowLeftTime -= Time.deltaTime;
                if (_mDataLoadedTipShowLeftTime <= 0)
                {
                    _mLoadingTipStatus = LoadingTipStatus.None;
                    LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(0);
                    if (item == null)
                    {
                        return;
                    }
                    UpdateLoadingTip(item);
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(0, -_mLoadingTipItemHeight, 0);
                    mLoopListView.OnItemSizeChanged(0);
                }
            }

        }


        void OnJumpBtnClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(_mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            if(itemIndex < 0)
            {
                return;
            }
            itemIndex++;
            mLoopListView.MovePanelToItemIndex(itemIndex, 0);
        }

        void OnAddItemBtnClicked()
        {
            if(mLoopListView.ItemTotalCount < 0)
            {
                return;
            }
            int count = 0;
            if (int.TryParse(_mAddItemInput.text, out count) == false)
            {
                return;
            }
            count = mLoopListView.ItemTotalCount + count;
            if (count < 0 || count > (DataSourceMgr.Get.TotalItemCount+1) )
            {
                return;
            }
            mLoopListView.SetListItemCount(count, false);
        }

        void OnSetItemCountBtnClicked()
        {
            int count = 0;
            if (int.TryParse(_mSetCountInput.text, out count) == false)
            {
                return;
            }
            if (count < 0 || count > DataSourceMgr.Get.TotalItemCount)
            {
                return;
            }
            count++;
            mLoopListView.SetListItemCount(count, false);
        }


    }

}
