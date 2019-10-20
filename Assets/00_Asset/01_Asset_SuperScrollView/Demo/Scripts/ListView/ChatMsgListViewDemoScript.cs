using _00_Asset._01_Asset_SuperScrollView.Demo.Scripts.Common;
using _00_Asset._01_Asset_SuperScrollView.Scripts.ListView;
using UnityEngine;
using UnityEngine.UI;

namespace _00_Asset._01_Asset_SuperScrollView.Demo.Scripts.ListView
{

    public class ChatMsgListViewDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        Button _mScrollToButton;
        InputField _mScrollToInput;
        Button _mBackButton;
        Button _mAppendMsgButton;

        // Use this for initialization
        void Start()
        {
            mLoopListView.InitListView(ChatMsgDataSourceMgr.Get.TotalItemCount, OnGetItemByIndex);
            _mScrollToButton = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToButton").GetComponent<Button>();
            _mScrollToInput = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputField").GetComponent<InputField>();
            _mScrollToButton.onClick.AddListener(OnJumpBtnClicked);
            _mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            _mBackButton.onClick.AddListener(OnBackBtnClicked);
            _mAppendMsgButton = GameObject.Find("ButtonPanel/buttonGroup1/AppendButton").GetComponent<Button>();
            _mAppendMsgButton.onClick.AddListener(OnAppendMsgBtnClicked);
        }

        void OnBackBtnClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }

        void OnAppendMsgBtnClicked()
        {
            ChatMsgDataSourceMgr.Get.AppendOneMsg();
            mLoopListView.SetListItemCount(ChatMsgDataSourceMgr.Get.TotalItemCount, false);
            mLoopListView.MovePanelToItemIndex(ChatMsgDataSourceMgr.Get.TotalItemCount-1, 0);
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

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= ChatMsgDataSourceMgr.Get.TotalItemCount)
            {
                return null;
            }

            ChatMsg itemData = ChatMsgDataSourceMgr.Get.GetChatMsgByIndex(index);
            if (itemData == null)
            {
                return null;
            }
            LoopListViewItem2 item = null;
            if (itemData.MPersonId == 0)
            {
                item = listView.NewListViewItem("ItemPrefab1");
            }
            else
            {
                item = listView.NewListViewItem("ItemPrefab2");
            }
            ListItem4 itemScript = item.GetComponent<ListItem4>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            itemScript.SetItemData(itemData,index);
            return item;
        }

    }

}
