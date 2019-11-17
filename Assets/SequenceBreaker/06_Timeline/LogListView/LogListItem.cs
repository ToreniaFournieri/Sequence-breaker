using _00_Asset._01_SuperScrollView.Scripts.ListView;
using SequenceBreaker._05_Play.Prepare;
using SequenceBreaker._06_Timeline.BattleLogView;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker._06_Timeline.LogListView
{
    public class LogListItem : MonoBehaviour
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


        public RunBattle runBattle;
        public BattleLogEnhancedScrollController battleLogEnhancedScrollController;
//        public CharacterStatusDisplay characterStatusDisplay;
        
//        //inventory switch debug mode true means infinity inventory.
//        public InventoryTreeViewDataSourceMgr inventoryTreeViewDataSourceMgr;
//        public bool isDebugMode;
        
        int _mItemDataIndex = -1;
        bool _mIsExpand;
        public void Init()
        {
            mExpandBtn.onClick.AddListener( OnExpandBtnClicked );
            mGoListDetail.onClick.AddListener( OnGoListBtnClicked );
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

        public void GoBattleLog()
        {
            battleLogEnhancedScrollController.InitBattleLog(runBattle);
        }

        void OnExpandBtnClicked()
        {
            RunBattle data = LogListDataSourceMgr.Get.GetItemDataByIndex(_mItemDataIndex);
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
            RunBattle data = LogListDataSourceMgr.Get.GetItemDataByIndex(_mItemDataIndex);
            if (data == null)
            {
                return;
            }

//            inventoryTreeViewDataSourceMgr.isInfinityInventoryMode = data.isInfinityInventoryMode;
//            inventoryTreeViewDataSourceMgr.DoRefreshDataSource();
//            characterStatusDisplay.unitList = data.unitClassList;
//            characterStatusDisplay.SetCharacterStatus(0);
            jumpToGameObject.SetActive(true);
            jumpToGameObject.transform.SetAsLastSibling();

        }

        public void SetItemData(RunBattle itemData, int itemIndex)
        {
            _mItemDataIndex = itemIndex;
            mContentText.text = itemData.missionText + "(lv: "+ itemData.missionLevelInitial + ")";
            mDescriptionText.text = itemData.location;

            runBattle = itemData;
            
            _mIsExpand = itemData.mIsExpand;
            OnExpandChanged();
        }


    }
}
