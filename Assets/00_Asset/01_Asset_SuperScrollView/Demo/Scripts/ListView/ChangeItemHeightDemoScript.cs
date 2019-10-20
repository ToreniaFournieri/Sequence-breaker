using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{

    public class ChangeItemHeightDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
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
            mLoopListView.InitListView(DataSourceMgr.Get.TotalItemCount, OnGetItemByIndex);

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



        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= DataSourceMgr.Get.TotalItemCount)
            {
                return null;
            }

            ItemData itemData = DataSourceMgr.Get.GetItemDataByIndex(index);
            if (itemData == null)
            {
                return null;
            }
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab1");
            ListItem8 itemScript = item.GetComponent<ListItem8>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }

            itemScript.SetItemData(itemData, index);
            return item;
        }

        void OnJumpBtnClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(_mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            mLoopListView.MovePanelToItemIndex(itemIndex, 0);
        }

        void OnAddItemBtnClicked()
        {
            if (mLoopListView.ItemTotalCount < 0)
            {
                return;
            }
            int count = 0;
            if (int.TryParse(_mAddItemInput.text, out count) == false)
            {
                return;
            }
            count = mLoopListView.ItemTotalCount + count;
            if (count < 0 || count > DataSourceMgr.Get.TotalItemCount)
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
            mLoopListView.SetListItemCount(count, false);
        }


    }

}
