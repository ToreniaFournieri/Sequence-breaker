using _00_Asset._01_Asset_SuperScrollView.Scripts.Common;
using _00_Asset._01_Asset_SuperScrollView.Scripts.ListView;
using SequenceBreaker._03_Controller._01_Home.Character;
using SequenceBreaker._04_Timeline_Tab.Log;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker._03_Controller._01_Home.HomeList
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
        // to which list
        public GameObject jumpToGameObject;
        

        
        public CharacterStatusDisplay characterStatusDisplay;
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


        void OnExpandBtnClicked()
        {
            HomeContentData data = HomeDataSourceMgr.Get.GetItemDataByIndex(_mItemDataIndex);
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
            HomeContentData data = HomeDataSourceMgr.Get.GetItemDataByIndex(_mItemDataIndex);
            if (data == null)
            {
                return;
            }

            characterStatusDisplay.unitList = data.unitClassList;
            characterStatusDisplay.SetCharacterStatus(0);
            jumpToGameObject.SetActive(true);
            jumpToGameObject.transform.SetAsLastSibling();
            Debug.Log("pressed :" + data.contentText);

        }

        public void SetItemData(HomeContentData itemData, int itemIndex)
        {
            _mItemDataIndex = itemIndex;
            mContentText.text = itemData.contentText;
            mDescriptionText.text = itemData.description;
            
            _mIsExpand = itemData.mIsExpand;
            OnExpandChanged();
        }


    }
}
