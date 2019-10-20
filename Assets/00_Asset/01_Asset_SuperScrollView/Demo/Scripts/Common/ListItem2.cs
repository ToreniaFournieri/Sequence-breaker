using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListItem2 : MonoBehaviour
    {
        public Text mNameText;
        public Image mIcon;
        public Image[] mStarArray;
        public Text mDescText;
        public Text mDescText2;
        public Color32 mRedStarColor = new Color32(249, 227, 101, 255);
        public Color32 mGrayStarColor = new Color32(215, 215, 215, 255);
        public GameObject mContentRootObj;
        int _mItemDataIndex = -1;
        public LoopListView2 mLoopListView;
        public void Init()
        {
            for(int i = 0;i<mStarArray.Length;++i)
            {
                int index = i;
                ClickEventListener listener = ClickEventListener.Get(mStarArray[i].gameObject);
                listener.SetClickEventHandler(delegate (GameObject obj) { OnStarClicked(index); });
            }
            
        }

        void OnStarClicked(int index)
        {
            ItemData data = DataSourceMgr.Get.GetItemDataByIndex(_mItemDataIndex);
            if(data == null)
            {
                return;
            }
            if(index == 0 && data.MStarCount == 1)
            {
                data.MStarCount = 0;
            }
            else
            {
                data.MStarCount = index + 1;
            }
            SetStarCount(data.MStarCount);
        }

        public void SetStarCount(int count)
        {
            int i = 0;
            for(; i<count;++i)
            {
                mStarArray[i].color = mRedStarColor;
            }
            for (; i < mStarArray.Length; ++i)
            {
                mStarArray[i].color = mGrayStarColor;
            }
        }

        public void SetItemData(ItemData itemData,int itemIndex)
        {
            _mItemDataIndex = itemIndex;
            mNameText.text = itemData.MName;
            mDescText.text = itemData.MFileSize.ToString() + "KB";
            mDescText2.text = itemData.MDesc;
            mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.MIcon);
            SetStarCount(itemData.MStarCount);
        }


    }
}
