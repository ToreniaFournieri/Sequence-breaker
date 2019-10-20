using _00_Asset._01_Asset_SuperScrollView.Demo.Scripts.Common;
using _00_Asset._01_Asset_SuperScrollView.Scripts.GridView;
using UnityEngine;
using UnityEngine.UI;

namespace _00_Asset._01_Asset_SuperScrollView.Demo.Scripts.GridView
{

    public class GridViewDemoScript2 : MonoBehaviour
    {
        public LoopGridView mLoopGridView;
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
            /*LoopGridViewSettingParam settingParam = new LoopGridViewSettingParam();
            settingParam.mItemSize = new Vector2(500, 500);
            settingParam.mItemPadding = new Vector2(40, 40);
            settingParam.mPadding = new RectOffset(10, 20, 30, 40);
            settingParam.mGridFixedType = GridFixedType.RowCountFixed;
            settingParam.mFixedRowOrColumnCount = 6;
            mLoopGridView.InitGridView(DataSourceMgr.Get.TotalItemCount, OnGetItemByIndex, settingParam);
            */
            mLoopGridView.InitGridView(DataSourceMgr.Get.TotalItemCount, OnGetItemByRowColumn);
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

       
        LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int itemIndex,int row,int column)
        {
            //get the data to showing
            ItemData itemData = DataSourceMgr.Get.GetItemDataByIndex(itemIndex);
            if (itemData == null)
            {
                return null;
            }
            /*
            get a new item. Every item can use a different prefab, 
            the parameter of the NewListViewItem is the prefab’name. 
            And all the prefabs should be listed in ItemPrefabList in LoopGridView Inspector Setting
            */
            LoopGridViewItem item = gridView.NewListViewItem("ItemPrefab0");

            ListItem18 itemScript = item.GetComponent<ListItem18>();//get your own component
            // IsInitHandlerCalled is false means this item is new created but not fetched from pool.
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();// here to init the item, such as add button click event listener.
            }
            //update the item’s content for showing, such as image,text.
            itemScript.SetItemData(itemData, itemIndex, row, column);
            return item;
        }


        void OnJumpBtnClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(_mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }

            mLoopGridView.MovePanelToItemByIndex(itemIndex, 0);
        }

        void OnAddItemBtnClicked()
        {
            int count = 0;
            if (int.TryParse(_mAddItemInput.text, out count) == false)
            {
                return;
            }
            mLoopGridView.SetListItemCount(count+mLoopGridView.ItemTotalCount,false);
        }

        void OnSetItemCountBtnClicked()
        {
            int count = 0;
            if (int.TryParse(_mSetCountInput.text, out count) == false)
            {
                return;
            }
            if(count > DataSourceMgr.Get.TotalItemCount)
            {
                count = DataSourceMgr.Get.TotalItemCount;
            }
            if(count < 0)
            {
                count = 0;
            }
            mLoopGridView.SetListItemCount(count);
        }

    }

}
