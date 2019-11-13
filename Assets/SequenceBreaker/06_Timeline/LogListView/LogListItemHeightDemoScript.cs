using _00_Asset._01_SuperScrollView.Scripts.ListView;
using SequenceBreaker._05_Play.Prepare;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker._06_Timeline.LogListView
{
    public class LogListItemHeightDemoScript : MonoBehaviour
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
            mLoopListView.InitListView(LogListDataSourceMgr.Get.TotalItemCount, OnGetItemByIndex);

//            _mSetCountButton = GameObject.Find("ButtonPanel/buttonGroup1/SetCountButton").GetComponent<Button>();
//            _mScrollToButton = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToButton").GetComponent<Button>();
//            _mAddItemButton = GameObject.Find("ButtonPanel/buttonGroup3/AddItemButton").GetComponent<Button>();
//            _mSetCountInput = GameObject.Find("ButtonPanel/buttonGroup1/SetCountInputField").GetComponent<InputField>();
//            _mScrollToInput = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputField").GetComponent<InputField>();
//            _mAddItemInput = GameObject.Find("ButtonPanel/buttonGroup3/AddItemInputField").GetComponent<InputField>();
//            _mScrollToButton.onClick.AddListener(OnJumpBtnClicked);
//            _mAddItemButton.onClick.AddListener(OnAddItemBtnClicked);
//            _mSetCountButton.onClick.AddListener(OnSetItemCountBtnClicked);
//            _mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
//            _mBackButton.onClick.AddListener(OnBackBtnClicked);
        }

        void OnBackBtnClicked()
        {
            
            // [ need to think it is correct (default of asset?)
//            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }


        //this seems wrong.. 2019.11.10 torenia
        public void UpdateNewContents()
        {
            mLoopListView.SetListItemCount(LogListDataSourceMgr.Get.TotalItemCount, false);
            mLoopListView.MovePanelToItemIndex(LogListDataSourceMgr.Get.TotalItemCount-1, 0);

        }
        

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {

            if (index < 0 || index >= LogListDataSourceMgr.Get.TotalItemCount)
            {
                return null;
            }

            RunBattle itemData = LogListDataSourceMgr.Get.GetItemDataByIndex(index);
            if (itemData == null)
            {
                return null;
            }
            LoopListViewItem2 item = listView.NewListViewItem("LogListCell");
            LogListItem itemScript = item.GetComponent<LogListItem>();
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
            if (count < 0 || count > LogListDataSourceMgr.Get.TotalItemCount)
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
            if (count < 0 || count > LogListDataSourceMgr.Get.TotalItemCount)
            {
                return;
            }
            mLoopListView.SetListItemCount(count, false);
        }


    
    }
}
