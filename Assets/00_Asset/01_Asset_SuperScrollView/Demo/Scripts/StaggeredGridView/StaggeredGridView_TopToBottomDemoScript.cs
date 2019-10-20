using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{

    public class StaggeredGridViewTopToBottomDemoScript : MonoBehaviour
    {
        public LoopStaggeredGridView mLoopListView;

        Button _mScrollToButton;
        Button _mAddItemButton;
        Button _mSetCountButton;
        InputField _mScrollToInput;
        InputField _mAddItemInput;
        InputField _mSetCountInput;
        Button _mBackButton;
        int[] _mItemHeightArrayForDemo = null;

        // Use this for initialization
        void Start()
        {
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
            InitItemHeightArrayForDemo();

            GridViewLayoutParam param = new GridViewLayoutParam();
            param.MPadding1 = 10;
            param.MPadding2 = 10;
            param.MColumnOrRowCount = 3;
            param.MItemWidthOrHeight = 217f;
            mLoopListView.InitListView(DataSourceMgr.Get.TotalItemCount, param, OnGetItemByIndex);
        }

        LoopStaggeredGridViewItem OnGetItemByIndex(LoopStaggeredGridView listView, int index)
        {
            if (index < 0)
            {
                return null;
            }
            ItemData itemData = DataSourceMgr.Get.GetItemDataByIndex(index);
            if(itemData == null)
            {
                return null;
            }
            //create one row
            LoopStaggeredGridViewItem item = listView.NewListViewItem("ItemPrefab0");
            ListItem5 itemScript = item.GetComponent<ListItem5>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            
            itemScript.SetItemData(itemData, index);
            float itemHeight = 300 + _mItemHeightArrayForDemo[index % _mItemHeightArrayForDemo.Length]*10f;
            item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemHeight);
            return item;
        }


        void InitItemHeightArrayForDemo()
        {
            _mItemHeightArrayForDemo = new int[100];
            for (int i = 0; i < _mItemHeightArrayForDemo.Length; ++i)
            {
                _mItemHeightArrayForDemo[i] = Random.Range(0, 20);
            }
        }


        void OnBackBtnClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
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
                itemIndex = 0;
            }
            mLoopListView.MovePanelToItemIndex(itemIndex, 0);
        }

        void OnAddItemBtnClicked()
        {
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
            mLoopListView.SetListItemCount(count,false);
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
            mLoopListView.SetListItemCount(count,false);
        }

    }



}
