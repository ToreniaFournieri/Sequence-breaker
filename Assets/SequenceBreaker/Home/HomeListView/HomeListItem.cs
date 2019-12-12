using System.Collections;
using System.Collections.Generic;
using _00_Asset._01_SuperScrollView.Scripts.ListView;
using _00_Asset._08_Easy_Panel_Transitions.Scripts;
using SequenceBreaker.GUIController.Segue;
using SequenceBreaker.Home.EquipView;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker.Home.HomeListView
{
    public class HomeListItem : MonoBehaviour
    {
        public Text mContentText;
        public Text mDescriptionText;
        public GameObject mExpandContentRoot;
        public Text mClickTip;
        public Button mExpandBtn;


        // Go to some List (ally unit list or enemy list
        public Button mGoListDetail;
        // to jump list
        public GameObject jumpToGameObject;

        //public CharacterStatusDisplay characterStatusDisplay;

        //inventory switch debug mode true means infinity inventory.
        //public InventoryTreeViewDataSourceMgr inventoryTreeViewDataSourceMgr;

        int _mItemDataIndex = -1;
        bool _mIsExpand;
        public void Init()
        {
            mExpandBtn.onClick.RemoveAllListeners();
            mGoListDetail.onClick.RemoveAllListeners();

            mExpandBtn.onClick.AddListener(OnExpandBtnClicked);
            mGoListDetail.onClick.AddListener(OnGoListBtnClicked);

            mGoListDetail.onClick.AddListener(SegueAndAnimation);

        }

        public void OnExpandChanged()
        {
            RectTransform rt = gameObject.GetComponent<RectTransform>();
            if (_mIsExpand)
            {
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 284f);
                mExpandContentRoot.SetActive(true);
                //                mClickTip.text = "Shrink";
            }
            else
            {
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 143f);
                mExpandContentRoot.SetActive(false);
                //                mClickTip.text = "Expand";
            }

        }


        void OnExpandBtnClicked()
        {
            HomeListContentData data = HomeListDataSourceMgr.instance.GetItemDataByIndex(_mItemDataIndex);
            //HomeContentData data = HomeDataSourceMgr.Get.GetItemDataByIndex(_mItemDataIndex);
            if (data == null)
            {
                return;
            }
            _mIsExpand = !_mIsExpand;
            data.mIsExpand = _mIsExpand;
            OnExpandChanged();
            LoopListViewItem2 item2 = gameObject.GetComponent<LoopListViewItem2>();
            item2.ParentListView.OnItemSizeChanged(item2.ItemIndex);
        }

        // go detail button clicked
        void OnGoListBtnClicked()
        {
            HomeListContentData data = HomeListDataSourceMgr.instance.GetItemDataByIndex(_mItemDataIndex);

            //HomeContentData data = HomeDataSourceMgr.Get.GetItemDataByIndex(_mItemDataIndex);
            if (data == null)
            {

                return;
            }


            //Debug.Log("On Go list button clicked unitClassList.Count is: " + data.unitClassList.Count );
            //Console.WriteLine("On Go list button clicked unitClassList.Count is: " + data.unitClassList.Count);

            //inventoryTreeViewDataSourceMgr.isInfinityInventoryMode = data.isInfinityInventoryMode;
            //inventoryTreeViewDataSourceMgr.DoRefreshDataSource();
            //characterStatusDisplay.unitList = data.unitClassList;
            //characterStatusDisplay.SetCharacterStatus(0);
            jumpToGameObject.SetActive(true);
            jumpToGameObject.transform.SetAsLastSibling();

        }

        void SegueAndAnimation()
        {
            jumpToGameObject.GetComponent<PanelAnimator>().StartAnimIn();
            SegueController.instance.StackHomeView(jumpToGameObject);
        }

        public void SetItemData(HomeListContentData itemData, int itemIndex)
        {
            _mItemDataIndex = itemIndex;
            mContentText.text = itemData.contentText;
            mDescriptionText.text = itemData.description;

            jumpToGameObject = itemData.jumpToGameObject;
            Init();

            _mIsExpand = itemData.mIsExpand;
            OnExpandChanged();
        }


    }
}


