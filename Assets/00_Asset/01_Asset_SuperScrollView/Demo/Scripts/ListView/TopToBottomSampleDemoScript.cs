using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    class CustomData
    {
        public string MContent;
    }


    public class TopToBottomSampleDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        Button _mScrollToButton;
        Button _mAppendItemButton;
        Button _mInsertItemButton;
        InputField _mScrollToInput;

        List<CustomData> _mDataList = null;

        int _mTotalInsertedCount = 0;

        // Use this for initialization
        void Start()
        {
            InitData();//init the item data list.

            mLoopListView.InitListView(_mDataList.Count, OnGetItemByIndex);

          
            _mScrollToButton = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToButton").GetComponent<Button>();
            _mAppendItemButton = GameObject.Find("ButtonPanel/buttonGroup3/AppendItemButton").GetComponent<Button>();
            _mInsertItemButton = GameObject.Find("ButtonPanel/buttonGroup3/InsertItemButton").GetComponent<Button>();

            _mScrollToInput = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputField").GetComponent<InputField>();
            _mScrollToButton.onClick.AddListener(OnJumpBtnClicked);
            _mAppendItemButton.onClick.AddListener(OnAppendItemBtnClicked);
            _mInsertItemButton.onClick.AddListener(OnInsertItemBtnClicked);
        }


        void InitData()
        {
            _mDataList = new List<CustomData>();
            int count = 100;
            for(int i = 0;i<count;++i)
            {
                CustomData cd = new CustomData();
                cd.MContent = "Item" + i;
                _mDataList.Add(cd);
            }
        }

        void AppendOneData()
        {
            CustomData cd = new CustomData();
            cd.MContent = "Item" + _mDataList.Count;
            _mDataList.Add(cd);
        }

        void InsertOneData()
        {
            _mTotalInsertedCount++;
            CustomData cd = new CustomData();
            cd.MContent = "Item(-" + _mTotalInsertedCount+")";
            _mDataList.Insert(0,cd);
        }


        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= _mDataList.Count)
            {
                return null;
            }

            CustomData itemData = _mDataList[index];
            if (itemData == null)
            {
                return null;
            }
            //get a new item. Every item can use a different prefab, the parameter of the NewListViewItem is the prefab’name. 
            //And all the prefabs should be listed in ItemPrefabList in LoopListView2 Inspector Setting
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab1");
            ListItem16 itemScript = item.GetComponent<ListItem16>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            itemScript.mNameText.text = itemData.MContent;
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

        void OnAppendItemBtnClicked()
        {
            AppendOneData();
            mLoopListView.SetListItemCount(_mDataList.Count, false);
            mLoopListView.RefreshAllShownItem();
        }

        void OnInsertItemBtnClicked()
        {
            InsertOneData();
            mLoopListView.SetListItemCount(_mDataList.Count, false);
            mLoopListView.RefreshAllShownItem();
        }

    }

}
