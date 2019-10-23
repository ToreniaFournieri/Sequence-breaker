using System.Collections.Generic;
using _00_Asset._01_Asset_SuperScrollView.Demo.Scripts.Common;
using _00_Asset._01_Asset_SuperScrollView.Scripts.Common;
using _00_Asset._01_Asset_SuperScrollView.Scripts.ListView;
using UnityEngine;
using UnityEngine.UI;

namespace _00_Asset._01_Asset_SuperScrollView.Demo.Scripts.ListView
{

    public class DotElem
    {
        public GameObject MDotElemRoot;
        public GameObject MDotSmall;
        public GameObject MDotBig;
    }

    public class PageViewDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        Button _mBackButton;
        int _mPageCount = 5;
        public Transform mDotsRootObj;
        List<DotElem> _mDotElemList = new List<DotElem>();
        void Start()
        {
            InitDots();
            LoopListViewInitParam initParam = LoopListViewInitParam.CopyDefaultInitParam();
            initParam.MSnapVecThreshold = 99999;
            mLoopListView.mOnBeginDragAction = OnBeginDrag;
            mLoopListView.mOnDragingAction = OnDraging;
            mLoopListView.mOnEndDragAction = OnEndDrag;
            mLoopListView.mOnSnapNearestChanged = OnSnapNearestChanged;
            mLoopListView.InitListView(_mPageCount, OnGetItemByIndex, initParam);

            _mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            _mBackButton.onClick.AddListener(OnBackBtnClicked);
        }


        void InitDots()
        {
            int childCount = mDotsRootObj.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                Transform tf = mDotsRootObj.GetChild(i);
                DotElem elem = new DotElem();
                elem.MDotElemRoot = tf.gameObject;
                elem.MDotSmall = tf.Find("dotSmall").gameObject;
                elem.MDotBig = tf.Find("dotBig").gameObject;
                ClickEventListener listener = ClickEventListener.Get(elem.MDotElemRoot);
                int index = i;
                listener.SetClickEventHandler(delegate (GameObject obj) { OnDotClicked(index); });
                _mDotElemList.Add(elem);
            }
        }


        void OnDotClicked(int index)
        {
            int curNearestItemIndex = mLoopListView.CurSnapNearestItemIndex;
            if (curNearestItemIndex < 0 || curNearestItemIndex >= _mPageCount)
            {
                return;
            }
            if(index == curNearestItemIndex)
            {
                return;
            }
            mLoopListView.SetSnapTargetItemIndex(index);
            
        }

        void UpdateAllDots()
        {
            int curNearestItemIndex = mLoopListView.CurSnapNearestItemIndex;
            if(curNearestItemIndex < 0 || curNearestItemIndex >= _mPageCount)
            {
                return;
            }
            int count = _mDotElemList.Count;
            if(curNearestItemIndex >= count)
            {
                return;
            }
            for(int i = 0;i<count;++i)
            {
                DotElem elem = _mDotElemList[i];
                if(i != curNearestItemIndex)
                {
                    elem.MDotSmall.SetActive(true);
                    elem.MDotBig.SetActive(false);
                }
                else
                {
                    elem.MDotSmall.SetActive(false);
                    elem.MDotBig.SetActive(true);
                }
            }
        }

        void OnSnapNearestChanged(LoopListView2 listView, LoopListViewItem2 item)
        {
            UpdateAllDots();
        }


        void OnBackBtnClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= _mPageCount)
            {
                return null;
            }

            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab1");
            ListItem14 itemScript = item.GetComponent<ListItem14>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            List<ListItem14Elem> elemList = itemScript.mElemItemList;
            int count = elemList.Count;
            int picBeginIndex = pageIndex * count;
            int i = 0;
            for(;i< count;++i)
            {
                ItemData itemData = DataSourceMgr.Get.GetItemDataByIndex(picBeginIndex+i);
                if(itemData == null)
                {
                    break;
                }
                ListItem14Elem elem = elemList[i];
                elem.mRootObj.SetActive(true);
                elem.mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
                elem.mName.text = itemData.mName;
            }
            if(i < count)
            {
                for(;i< count;++i)
                {
                    elemList[i].mRootObj.SetActive(false);
                }
            }
            return item;
        }


        void OnBeginDrag()
        {

        }

        void OnDraging()
        {

        }
        void OnEndDrag()
        {
            float vec = mLoopListView.ScrollRect.velocity.x;
            int curNearestItemIndex = mLoopListView.CurSnapNearestItemIndex;
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(curNearestItemIndex);
            if(item == null)
            {
                mLoopListView.ClearSnapData();
                return;
            }
            if (Mathf.Abs(vec) < 50f)
            {
                mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex);
                return;
            }
            Vector3 pos = mLoopListView.GetItemCornerPosInViewPort(item, ItemCornerEnum.LeftTop);
            if(pos.x > 0)
            {
                if (vec > 0)
                {
                    mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex - 1);
                }
                else
                {
                    mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex);
                }
            }
            else if (pos.x < 0)
            {
                if (vec > 0)
                {
                    mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex);
                }
                else
                {
                    mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex+1);
                }
            }
            else
            {
                if (vec > 0)
                {
                    mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex-1);
                }
                else
                {
                    mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex + 1);
                }
            }
        }


    }

}
