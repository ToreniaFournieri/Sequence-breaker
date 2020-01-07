using _00_Asset._01_SuperScrollView.Demo.Scripts.Common;
using _00_Asset._01_SuperScrollView.Scripts.ListView;
using UnityEngine;
using UnityEngine.UI;

namespace _00_Asset._01_SuperScrollView.Demo.Scripts.ListView
{

    public class GridViewDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        Button _mScrollToButton;
        Button _mAddItemButton;
        Button _mSetCountButton;
        InputField _mScrollToInput;
        InputField _mAddItemInput;
        InputField _mSetCountInput;
        Button _mBackButton;
        const int MItemCountPerRow = 3;// how many items in one row
        int _mListItemTotalCount = 0;

        // Use this for initialization
        void Start()
        {
            _mListItemTotalCount = DataSourceMgr.Get.TotalItemCount;
            int count = _mListItemTotalCount / MItemCountPerRow;
            if(_mListItemTotalCount % MItemCountPerRow > 0)
            {
                count++;
            }
            //count is the total row count
            mLoopListView.InitListView(count, OnGetItemByIndex);

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
            //count1 is the total row count
            mLoopListView.SetListItemCount(count1,false);
            mLoopListView.RefreshAllShownItem();
        }


        /*when a row is getting show in the scrollrect viewport, 
       this method will be called with the row’ index as a parameter, 
       to let you create the row  and update its content.

       SuperScrollView uses single items with subitems that make up the columns in the row.
       so in fact, the GridView is ListView.
       if one row is make up with 3 subitems, then the GridView looks like:

            row0:  subitem0 subitem1 subitem2
            row1:  subitem3 subitem4 subitem5
            row2:  subitem6 subitem7 subitem8
            row3:  subitem9 subitem10 subitem11
            ...
        */
        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 )
            {
                return null;
            }
            //create one row
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab1");
            ListItem6 itemScript = item.GetComponent<ListItem6>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            //update all items in the row
            for (int i = 0;i< MItemCountPerRow; ++i)
            {
                int itemIndex = index * MItemCountPerRow + i;
                if(itemIndex >= _mListItemTotalCount)
                {
                    itemScript.mItemList[i].gameObject.SetActive(false);
                    continue;
                }
                ItemData itemData = DataSourceMgr.Get.GetItemDataByIndex(itemIndex);
                //update the subitem content.
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

        void OnJumpBtnClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(_mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            if(itemIndex < 0)
            {
                itemIndex = 0;
            }
            itemIndex++;
            int count1 = itemIndex / MItemCountPerRow;
            if (itemIndex % MItemCountPerRow > 0)
            {
                count1++;
            }
            if(count1 > 0)
            {
                count1--;
            }
            mLoopListView.MovePanelToItemIndex(count1, 0);
        }

        void OnAddItemBtnClicked()
        {
            int count = 0;
            if (int.TryParse(_mAddItemInput.text, out count) == false)
            {
                return;
            }
            SetListItemTotalCount(_mListItemTotalCount + count);
        }

        void OnSetItemCountBtnClicked()
        {
            int count = 0;
            if (int.TryParse(_mSetCountInput.text, out count) == false)
            {
                return;
            }
            SetListItemTotalCount(count);
        }


    }

}
