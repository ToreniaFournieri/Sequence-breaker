using _00_Asset._01_Asset_SuperScrollView.Scripts.Common;
using _00_Asset._01_Asset_SuperScrollView.Scripts.ListView;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker._03_Controller._01_Home.HomeList
{
    public class HomeListItem : MonoBehaviour
    {
        public Text mContentText;
//        public Image mIcon;
//        public Image[] mStarArray;
        public Text mDescriptionText;
        public GameObject mExpandContentRoot;
        public Text mClickTip;
        public Button mExpandBtn;

        // Go to some List (ally unit list or enemy list
        public Button mGoListDetail;
        public Color32 mRedStarColor = new Color32(249, 227, 101, 255);
        public Color32 mGrayStarColor = new Color32(215, 215, 215, 255);
        int _mItemDataIndex = -1;
        bool _mIsExpand;
        public void Init()
        {
//            for (int i = 0; i < mStarArray.Length; ++i)
//            {
//                int index = i;
//                ClickEventListener listener = ClickEventListener.Get(mStarArray[i].gameObject);
////                listener.SetClickEventHandler(delegate (GameObject obj) { OnStarClicked(index); });
//            }

            mExpandBtn.onClick.AddListener( OnExpandBtnClicked );
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


//        void OnStarClicked(int index)
//        {
//            HomeItemData data = HomeDataSourceMgr.Get.GetItemDataByIndex(_mItemDataIndex);
//            if (data == null)
//            {
//                return;
//            }
//            if (index == 0 && data.MStarCount == 1)
//            {
//                data.MStarCount = 0;
//            }
//            else
//            {
//                data.MStarCount = index + 1;
//            }
//            SetStarCount(data.MStarCount);
//        }

//        public void SetStarCount(int count)
//        {
//            int i = 0;
//            for (; i < count; ++i)
//            {
//                mStarArray[i].color = mRedStarColor;
//            }
//            for (; i < mStarArray.Length; ++i)
//            {
//                mStarArray[i].color = mGrayStarColor;
//            }
//        }

        public void SetItemData(HomeContentData itemData, int itemIndex)
        {
            _mItemDataIndex = itemIndex;
            mContentText.text = itemData.contentText;
            mDescriptionText.text = itemData.description;
            
//            mNameText.text = itemData.MContentString;
//            mDescText.text = itemData.MFileSize.ToString() + "KB";
//            mIcon.sprite = HomeResManager.Get.GetSpriteByName(itemData.MIcon);
//            SetStarCount(itemData.MStarCount);
            _mIsExpand = itemData.mIsExpand;
            OnExpandChanged();
        }


    }
}
