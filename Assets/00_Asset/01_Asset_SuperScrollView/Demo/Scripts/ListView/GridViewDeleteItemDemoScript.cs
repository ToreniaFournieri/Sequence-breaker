using _00_Asset._01_Asset_SuperScrollView.Demo.Scripts.Common;
using _00_Asset._01_Asset_SuperScrollView.Scripts.ListView;
using UnityEngine;
using UnityEngine.UI;

namespace _00_Asset._01_Asset_SuperScrollView.Demo.Scripts.ListView
{

    public class GridViewDeleteItemDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public Button mSelectAllButton;
        public Button mCancelAllButton;
        public Button mDeleteButton;
        public Button mBackButton;
        const int MItemCountPerRow = 3;
        int _mListItemTotalCount = 0;

        // Use this for initialization
        void Start()
        {
            _mListItemTotalCount = DataSourceMgr.Get.TotalItemCount;
            int count = _mListItemTotalCount / MItemCountPerRow;
            if (_mListItemTotalCount % MItemCountPerRow > 0)
            {
                count++;
            }
            mLoopListView.InitListView(count, OnGetItemByIndex);
            mBackButton.onClick.AddListener(OnBackBtnClicked);
            mSelectAllButton.onClick.AddListener(OnSelectAllBtnClicked);
            mCancelAllButton.onClick.AddListener(OnCancelAllBtnClicked);
            mDeleteButton.onClick.AddListener(OnDeleteBtnClicked);
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
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab1");
            ListItem10 itemScript = item.GetComponent<ListItem10>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            for (int i = 0; i < MItemCountPerRow; ++i)
            {
                int itemIndex = index * MItemCountPerRow + i;
                if (itemIndex >= _mListItemTotalCount)
                {
                    itemScript.mItemList[i].gameObject.SetActive(false);
                    continue;
                }
                ItemData itemData = DataSourceMgr.Get.GetItemDataByIndex(itemIndex);
                if (itemData != null)
                {
                    itemScript.mItemList[i].gameObject.SetActive(true);
                    itemScript.mItemList[i].SetItemData(itemData, itemIndex);
                }
                else
                {
                    itemScript.mItemList[i].gameObject.SetActive(false);
                }
            }
            return item;
        }

        void OnSelectAllBtnClicked()
        {
            DataSourceMgr.Get.CheckAllItem();
            mLoopListView.RefreshAllShownItem();
        }

        void OnCancelAllBtnClicked()
        {
            DataSourceMgr.Get.UnCheckAllItem();
            mLoopListView.RefreshAllShownItem();
        }

        void OnDeleteBtnClicked()
        {
            bool isChanged = DataSourceMgr.Get.DeleteAllCheckedItem();
            if (isChanged == false)
            {
                return;
            }
            SetListItemTotalCount(DataSourceMgr.Get.TotalItemCount);
        }


        void SetListItemTotalCount(int count)
        {
            _mListItemTotalCount = count;
            if (_mListItemTotalCount < 0)
            {
                _mListItemTotalCount = 0;
            }
            if (_mListItemTotalCount > DataSourceMgr.Get.TotalItemCount)
            {
                _mListItemTotalCount = DataSourceMgr.Get.TotalItemCount;
            }
            int count1 = _mListItemTotalCount / MItemCountPerRow;
            if (_mListItemTotalCount % MItemCountPerRow > 0)
            {
                count1++;
            }
            mLoopListView.SetListItemCount(count1, false);
            mLoopListView.RefreshAllShownItem();
        }

    }

}
