using System.Collections.Generic;
using _00_Asset._01_SuperScrollView.Scripts.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _00_Asset._01_SuperScrollView.Scripts.ListView
{
   
    [System.Serializable]
    public class ItemPrefabConfData
    {
        public GameObject mItemPrefab = null;
        public float mPadding = 0;
        public int mInitCreateCount = 0;
        public float mStartPosOffset = 0;
    }


    public class LoopListViewInitParam
    {
        // all the default values
        public float MDistanceForRecycle0 = 300; //mDistanceForRecycle0 should be larger than mDistanceForNew0
        public float MDistanceForNew0 = 200;
        public float MDistanceForRecycle1 = 300;//mDistanceForRecycle1 should be larger than mDistanceForNew1
        public float MDistanceForNew1 = 200;
        public float MSmoothDumpRate = 0.3f;
        public float MSnapFinishThreshold = 0.01f;
        public float MSnapVecThreshold = 145;
        public float MItemDefaultWithPaddingSize = 20;//item's default size (with padding)

        public static LoopListViewInitParam CopyDefaultInitParam()
        {
            return new LoopListViewInitParam();
        }
    }


    public class LoopListView2 : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {

        class SnapData
        {
            public SnapStatus MSnapStatus = SnapStatus.NoTargetSet;
            public int MSnapTargetIndex = 0;
            public float MTargetSnapVal = 0;
            public float MCurSnapVal = 0;
            public bool MIsForceSnapTo = false;
            public bool MIsTempTarget = false;
            public int MTempTargetIndex = -1;
            public float MMoveMaxAbsVec = -1;
            public void Clear()
            {
                MSnapStatus = SnapStatus.NoTargetSet;
                MTempTargetIndex = -1;
                MIsForceSnapTo = false;
                MMoveMaxAbsVec = -1;
            }
        }

        Dictionary<string, ItemPool> _mItemPoolDict = new Dictionary<string, ItemPool>();
        List<ItemPool> _mItemPoolList = new List<ItemPool>();
        [SerializeField]
        List<ItemPrefabConfData> mItemPrefabDataList = new List<ItemPrefabConfData>();

        [SerializeField]
        private ListItemArrangeType mArrangeType = ListItemArrangeType.TopToBottom;
        public ListItemArrangeType ArrangeType { get { return mArrangeType; } set { mArrangeType = value; } }

        List<LoopListViewItem2> _mItemList = new List<LoopListViewItem2>();
        RectTransform _mContainerTrans;
        ScrollRect _mScrollRect = null;
        RectTransform _mScrollRectTransform = null;
        RectTransform _mViewPortRectTransform = null;
        float _mItemDefaultWithPaddingSize = 20;
        int _mItemTotalCount = 0;
        bool _mIsVertList = false;
        System.Func<LoopListView2, int, LoopListViewItem2> _mOnGetItemByIndex;
        Vector3[] _mItemWorldCorners = new Vector3[4];
        Vector3[] _mViewPortRectLocalCorners = new Vector3[4];
        int _mCurReadyMinItemIndex = 0;
        int _mCurReadyMaxItemIndex = 0;
        bool _mNeedCheckNextMinItem = true;
        bool _mNeedCheckNextMaxItem = true;
        ItemPosMgr _mItemPosMgr = null;
        float _mDistanceForRecycle0 = 300;
        float _mDistanceForNew0 = 200;
        float _mDistanceForRecycle1 = 300;
        float _mDistanceForNew1 = 200;
        [SerializeField]
        bool mSupportScrollBar = true;
        bool _mIsDraging = false;
        PointerEventData _mPointerEventData = null;
        public System.Action mOnBeginDragAction = null;
        public System.Action mOnDragingAction = null;
        public System.Action mOnEndDragAction = null;
        int _mLastItemIndex = 0;
        float _mLastItemPadding = 0;
        float _mSmoothDumpVel = 0;
        float _mSmoothDumpRate = 0.3f;
        float _mSnapFinishThreshold = 0.1f;
        float _mSnapVecThreshold = 145;
        float _mSnapMoveDefaultMaxAbsVec = 3400f;
        [SerializeField]
        bool mItemSnapEnable = false;


        Vector3 _mLastFrameContainerPos = Vector3.zero;
        public System.Action<LoopListView2,LoopListViewItem2> mOnSnapItemFinished = null;
        public System.Action<LoopListView2, LoopListViewItem2> mOnSnapNearestChanged = null;
        int _mCurSnapNearestItemIndex = -1;
        Vector2 _mAdjustedVec;
        bool _mNeedAdjustVec = false;
        int _mLeftSnapUpdateExtraCount = 1;
        [SerializeField]
        Vector2 mViewPortSnapPivot = Vector2.zero;
        [SerializeField]
        Vector2 mItemSnapPivot = Vector2.zero;
        ClickEventListener _mScrollBarClickEventListener = null;
        SnapData _mCurSnapData = new SnapData();
        Vector3 _mLastSnapCheckPos = Vector3.zero;
        bool _mListViewInited = false;
        int _mListUpdateCheckFrameCount = 0;

        public List<ItemPrefabConfData> ItemPrefabDataList
        {
            get
            {
                return mItemPrefabDataList;
            }
        }

        public List<LoopListViewItem2> ItemList
        {
            get
            {
                return _mItemList;
            }
        }

        public bool IsVertList
        {
            get
            {
                return _mIsVertList;
            }
        }
        public int ItemTotalCount
        {
            get
            {
                return _mItemTotalCount;
            }
        }

        public RectTransform ContainerTrans
        {
            get
            {
                return _mContainerTrans;
            }
        }

        public ScrollRect ScrollRect
        {
            get
            {
                return _mScrollRect;
            }
        }

        public bool IsDraging
        {
            get
            {
                return _mIsDraging;
            }
        }

        public bool ItemSnapEnable
        {
            get {return mItemSnapEnable;}
            set { mItemSnapEnable = value; }
        }

        public bool SupportScrollBar
        {
            get { return mSupportScrollBar; }
            set { mSupportScrollBar = value; }
        }

        public float SnapMoveDefaultMaxAbsVec
        {
            get { return _mSnapMoveDefaultMaxAbsVec; }
            set { _mSnapMoveDefaultMaxAbsVec = value; }
        }

        public ItemPrefabConfData GetItemPrefabConfData(string prefabName)
        {
            foreach (ItemPrefabConfData data in mItemPrefabDataList)
            {
                if (data.mItemPrefab == null)
                {
                    Debug.LogError("A item prefab is null ");
                    continue;
                }
                if (prefabName == data.mItemPrefab.name)
                {
                    return data;
                }

            }
            return null;
        }

        public void OnItemPrefabChanged(string prefabName)
        {
            ItemPrefabConfData data = GetItemPrefabConfData(prefabName);
            if(data == null)
            {
                return;
            }
            ItemPool pool = null;
            if (_mItemPoolDict.TryGetValue(prefabName, out pool) == false)
            {
                return;
            }
            int firstItemIndex = -1;
            Vector3 pos = Vector3.zero;
            if(_mItemList.Count > 0)
            {
                firstItemIndex = _mItemList[0].ItemIndex;
                pos = _mItemList[0].CachedRectTransform.anchoredPosition3D;
            }
            RecycleAllItem();
            ClearAllTmpRecycledItem();
            pool.DestroyAllItem();
            pool.Init(data.mItemPrefab, data.mPadding, data.mStartPosOffset,data.mInitCreateCount, _mContainerTrans);
            if(firstItemIndex >= 0)
            {
                RefreshAllShownItemWithFirstIndexAndPos(firstItemIndex, pos);
            }
        }

        /*
        InitListView method is to initiate the LoopListView2 component. There are 3 parameters:
        itemTotalCount: the total item count in the listview. If this parameter is set -1, then means there are infinite items, and scrollbar would not be supported, and the ItemIndex can be from –MaxInt to +MaxInt. If this parameter is set a value >=0 , then the ItemIndex can only be from 0 to itemTotalCount -1.
        onGetItemByIndex: when a item is getting in the scrollrect viewport, and this Action will be called with the item’ index as a parameter, to let you create the item and update its content.
        */
        public void InitListView(int itemTotalCount,
            System.Func<LoopListView2, int, LoopListViewItem2> onGetItemByIndex,
            LoopListViewInitParam initParam = null)
        {
            if(initParam != null)
            {
                _mDistanceForRecycle0 = initParam.MDistanceForRecycle0;
                _mDistanceForNew0 = initParam.MDistanceForNew0;
                _mDistanceForRecycle1 = initParam.MDistanceForRecycle1;
                _mDistanceForNew1 = initParam.MDistanceForNew1;
                _mSmoothDumpRate = initParam.MSmoothDumpRate;
                _mSnapFinishThreshold = initParam.MSnapFinishThreshold;
                _mSnapVecThreshold = initParam.MSnapVecThreshold;
                _mItemDefaultWithPaddingSize = initParam.MItemDefaultWithPaddingSize;
            }
            _mScrollRect = gameObject.GetComponent<ScrollRect>();
            if (_mScrollRect == null)
            {
                Debug.LogError("ListView Init Failed! ScrollRect component not found!");
                return;
            }
            if(_mDistanceForRecycle0 <= _mDistanceForNew0)
            {
                Debug.LogError("mDistanceForRecycle0 should be bigger than mDistanceForNew0");
            }
            if (_mDistanceForRecycle1 <= _mDistanceForNew1)
            {
                Debug.LogError("mDistanceForRecycle1 should be bigger than mDistanceForNew1");
            }
            _mCurSnapData.Clear();
            _mItemPosMgr = new ItemPosMgr(_mItemDefaultWithPaddingSize);
            _mScrollRectTransform = _mScrollRect.GetComponent<RectTransform>();
            _mContainerTrans = _mScrollRect.content;
            _mViewPortRectTransform = _mScrollRect.viewport;
            if (_mViewPortRectTransform == null)
            {
                _mViewPortRectTransform = _mScrollRectTransform;
            }
            if (_mScrollRect.horizontalScrollbarVisibility == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport && _mScrollRect.horizontalScrollbar != null)
            {
                Debug.LogError("ScrollRect.horizontalScrollbarVisibility cannot be set to AutoHideAndExpandViewport");
            }
            if (_mScrollRect.verticalScrollbarVisibility == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport && _mScrollRect.verticalScrollbar != null)
            {
                Debug.LogError("ScrollRect.verticalScrollbarVisibility cannot be set to AutoHideAndExpandViewport");
            }
            _mIsVertList = (mArrangeType == ListItemArrangeType.TopToBottom || mArrangeType == ListItemArrangeType.BottomToTop);
            _mScrollRect.horizontal = !_mIsVertList;
            _mScrollRect.vertical = _mIsVertList;
            SetScrollbarListener();
            AdjustPivot(_mViewPortRectTransform);
            AdjustAnchor(_mContainerTrans);
            AdjustContainerPivot(_mContainerTrans);
            InitItemPool();
            _mOnGetItemByIndex = onGetItemByIndex;
            if(_mListViewInited == true)
            {
                Debug.LogError("LoopListView2.InitListView method can be called only once.");
            }
            _mListViewInited = true;
            ResetListView();
            //SetListItemCount(itemTotalCount, true);
            _mCurSnapData.Clear();
            _mItemTotalCount = itemTotalCount;
            if (_mItemTotalCount < 0)
            {
                mSupportScrollBar = false;
            }
            if (mSupportScrollBar)
            {
                _mItemPosMgr.SetItemMaxCount(_mItemTotalCount);
            }
            else
            {
                _mItemPosMgr.SetItemMaxCount(0);
            }
            _mCurReadyMaxItemIndex = 0;
            _mCurReadyMinItemIndex = 0;
            _mLeftSnapUpdateExtraCount = 1;
            _mNeedCheckNextMaxItem = true;
            _mNeedCheckNextMinItem = true;
            UpdateContentSize();
        }

        void SetScrollbarListener()
        {
            _mScrollBarClickEventListener = null;
            Scrollbar curScrollBar = null;
            if (_mIsVertList && _mScrollRect.verticalScrollbar != null)
            {
                curScrollBar = _mScrollRect.verticalScrollbar;

            }
            if (!_mIsVertList && _mScrollRect.horizontalScrollbar != null)
            {
                curScrollBar = _mScrollRect.horizontalScrollbar;
            }
            if(curScrollBar == null)
            {
                return;
            }
            ClickEventListener listener = ClickEventListener.Get(curScrollBar.gameObject);
            _mScrollBarClickEventListener = listener;
            listener.SetPointerUpHandler(OnPointerUpInScrollBar);
            listener.SetPointerDownHandler(OnPointerDownInScrollBar);
        }

        void OnPointerDownInScrollBar(GameObject obj)
        {
            _mCurSnapData.Clear();
        }

        void OnPointerUpInScrollBar(GameObject obj)
        {
            ForceSnapUpdateCheck();
        }

        public void ResetListView(bool resetPos = true)
        {
            _mViewPortRectTransform.GetLocalCorners(_mViewPortRectLocalCorners);
            if(resetPos)
            {
                _mContainerTrans.anchoredPosition3D = Vector3.zero;
            }
            ForceSnapUpdateCheck();
        }


        /*
        This method may use to set the item total count of the scrollview at runtime. 
        If this parameter is set -1, then means there are infinite items,
        and scrollbar would not be supported, and the ItemIndex can be from –MaxInt to +MaxInt. 
        If this parameter is set a value >=0 , then the ItemIndex can only be from 0 to itemTotalCount -1.  
        If resetPos is set false, then the scrollrect’s content position will not changed after this method finished.
        */
        public void SetListItemCount(int itemCount, bool resetPos = true)
        {
            if(itemCount == _mItemTotalCount)
            {
                return;
            }
            _mCurSnapData.Clear();
            _mItemTotalCount = itemCount;
            if (_mItemTotalCount < 0)
            {
                mSupportScrollBar = false;
            }
            if (mSupportScrollBar)
            {
                _mItemPosMgr.SetItemMaxCount(_mItemTotalCount);
            }
            else
            {
                _mItemPosMgr.SetItemMaxCount(0);
            }
            if (_mItemTotalCount == 0)
            {
                _mCurReadyMaxItemIndex = 0;
                _mCurReadyMinItemIndex = 0;
                _mNeedCheckNextMaxItem = false;
                _mNeedCheckNextMinItem = false;
                RecycleAllItem();
                ClearAllTmpRecycledItem();
                UpdateContentSize();
                return;
            }
            if(_mCurReadyMaxItemIndex >= _mItemTotalCount)
            {
                _mCurReadyMaxItemIndex = _mItemTotalCount - 1;
            }
            _mLeftSnapUpdateExtraCount = 1;
            _mNeedCheckNextMaxItem = true;
            _mNeedCheckNextMinItem = true;
            if (resetPos)
            {
                MovePanelToItemIndex(0, 0);
                return;
            }
            if (_mItemList.Count == 0)
            {
                MovePanelToItemIndex(0, 0);
                return;
            }
            int maxItemIndex = _mItemTotalCount - 1;
            int lastItemIndex = _mItemList[_mItemList.Count - 1].ItemIndex;
            if (lastItemIndex <= maxItemIndex)
            {
                UpdateContentSize();
                UpdateAllShownItemsPos();
                return;
            }
            MovePanelToItemIndex(maxItemIndex, 0);

        }

        //To get the visible item by itemIndex. If the item is not visible, then this method return null.
        public LoopListViewItem2 GetShownItemByItemIndex(int itemIndex)
        {
            int count = _mItemList.Count;
            if (count == 0)
            {
                return null;
            }
            if (itemIndex < _mItemList[0].ItemIndex || itemIndex > _mItemList[count - 1].ItemIndex)
            {
                return null;
            }
            int i = itemIndex - _mItemList[0].ItemIndex;
            return _mItemList[i];
        }


        public LoopListViewItem2 GetShownItemNearestItemIndex(int itemIndex)
        {
            int count = _mItemList.Count;
            if (count == 0)
            {
                return null;
            }
            if (itemIndex < _mItemList[0].ItemIndex )
            {
                return _mItemList[0];
            }
            if (itemIndex > _mItemList[count - 1].ItemIndex)
            {
                return _mItemList[count - 1];
            }
            int i = itemIndex - _mItemList[0].ItemIndex;
            return _mItemList[i];
        }

        public int ShownItemCount
        {
            get
            {
                return _mItemList.Count;
            }
        }

        public float ViewPortSize
        {
            get
            {
                if (_mIsVertList)
                {
                    return _mViewPortRectTransform.rect.height;
                }
                else
                {
                    return _mViewPortRectTransform.rect.width;
                }
            }
        }

        public float ViewPortWidth
        {
            get { return _mViewPortRectTransform.rect.width; }
        }
        public float ViewPortHeight
        {
            get { return _mViewPortRectTransform.rect.height; }
        }


        /*
         All visible items is stored in a List<LoopListViewItem2> , which is named mItemList;
         this method is to get the visible item by the index in visible items list. The parameter index is from 0 to mItemList.Count.
        */
        public LoopListViewItem2 GetShownItemByIndex(int index)
        {
            int count = _mItemList.Count;
            if(index < 0 || index >= count)
            {
                return null;
            }
            return _mItemList[index];
        }

        public LoopListViewItem2 GetShownItemByIndexWithoutCheck(int index)
        {
            return _mItemList[index];
        }

        public int GetIndexInShownItemList(LoopListViewItem2 item)
        {
            if(item == null)
            {
                return -1;
            }
            int count = _mItemList.Count;
            if (count == 0)
            {
                return -1;
            }
            for (int i = 0; i < count; ++i)
            {
                if (_mItemList[i] == item)
                {
                    return i;
                }
            }
            return -1;
        }


        public void DoActionForEachShownItem(System.Action<LoopListViewItem2,object> action,object param)
        {
            if(action == null)
            {
                return;
            }
            int count = _mItemList.Count;
            if(count == 0)
            {
                return;
            }
            for (int i = 0; i < count; ++i)
            {
                action(_mItemList[i],param);
            }
        }


        public LoopListViewItem2 NewListViewItem(string itemPrefabName)
        {
            ItemPool pool = null;
            if (_mItemPoolDict.TryGetValue(itemPrefabName, out pool) == false)
            {
                return null;
            }
            LoopListViewItem2 item = pool.GetItem();
            RectTransform rf = item.GetComponent<RectTransform>();
            rf.SetParent(_mContainerTrans);
            rf.localScale = Vector3.one;
            rf.anchoredPosition3D = Vector3.zero;
            rf.localEulerAngles = Vector3.zero;
            item.ParentListView = this;
            return item;
        }

        /*
        For a vertical scrollrect, when a visible item’s height changed at runtime, then this method should be called to let the LoopListView2 component reposition all visible items’ position.
        For a horizontal scrollrect, when a visible item’s width changed at runtime, then this method should be called to let the LoopListView2 component reposition all visible items’ position.
        */
        public void OnItemSizeChanged(int itemIndex)
        {
            LoopListViewItem2 item = GetShownItemByItemIndex(itemIndex);
            if (item == null)
            {
                return;
            }
            if (mSupportScrollBar)
            {
                if (_mIsVertList)
                {
                    SetItemSize(itemIndex, item.CachedRectTransform.rect.height, item.Padding);
                }
                else
                {
                    SetItemSize(itemIndex, item.CachedRectTransform.rect.width, item.Padding);
                }
            }
            UpdateContentSize();
            UpdateAllShownItemsPos();
        }


        /*
        To update a item by itemIndex.if the itemIndex-th item is not visible, then this method will do nothing.
        Otherwise this method will first call onGetItemByIndex(itemIndex) to get a updated item and then reposition all visible items'position. 
        */
        public void RefreshItemByItemIndex(int itemIndex)
        {
            int count = _mItemList.Count;
            if (count == 0)
            {
                return;
            }
            if (itemIndex < _mItemList[0].ItemIndex || itemIndex > _mItemList[count - 1].ItemIndex)
            {
                return;
            }
            int firstItemIndex = _mItemList[0].ItemIndex;
            int i = itemIndex - firstItemIndex;
            LoopListViewItem2 curItem = _mItemList[i];
            Vector3 pos = curItem.CachedRectTransform.anchoredPosition3D;
            RecycleItemTmp(curItem);
            LoopListViewItem2 newItem = GetNewItemByIndex(itemIndex);
            if (newItem == null)
            {
                RefreshAllShownItemWithFirstIndex(firstItemIndex);
                return;
            }
            _mItemList[i] = newItem;
            if(_mIsVertList)
            {
                pos.x = newItem.StartPosOffset;
            }
            else
            {
                pos.y = newItem.StartPosOffset;
            }
            newItem.CachedRectTransform.anchoredPosition3D = pos;
            OnItemSizeChanged(itemIndex);
            ClearAllTmpRecycledItem();
        }

        //snap move will finish at once.
        public void FinishSnapImmediately()
        {
            UpdateSnapMove(true);
        }

        /*
        This method will move the scrollrect content’s position to ( the positon of itemIndex-th item + offset ),
        and offset is from 0 to scrollrect viewport size. 
        */
        public void MovePanelToItemIndex(int itemIndex, float offset)
        {
            _mScrollRect.StopMovement();
            _mCurSnapData.Clear();
            if (_mItemTotalCount == 0)
            {
                return;
            }
            if(itemIndex < 0 && _mItemTotalCount > 0)
            {
                return;
            }
            if (_mItemTotalCount > 0 && itemIndex >= _mItemTotalCount)
            {
                itemIndex = _mItemTotalCount - 1;
            }
            if (offset < 0)
            {
                offset = 0;
            }
            Vector3 pos = Vector3.zero;
            float viewPortSize = ViewPortSize;
            if (offset > viewPortSize)
            {
                offset = viewPortSize;
            }
            if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                float containerPos = _mContainerTrans.anchoredPosition3D.y;
                if (containerPos < 0)
                {
                    containerPos = 0;
                }
                pos.y = -containerPos - offset;
            }
            else if (mArrangeType == ListItemArrangeType.BottomToTop)
            {
                float containerPos = _mContainerTrans.anchoredPosition3D.y;
                if (containerPos > 0)
                {
                    containerPos = 0;
                }
                pos.y = -containerPos + offset;
            }
            else if (mArrangeType == ListItemArrangeType.LeftToRight)
            {
                float containerPos = _mContainerTrans.anchoredPosition3D.x;
                if (containerPos > 0)
                {
                    containerPos = 0;
                }
                pos.x = -containerPos + offset;
            }
            else if (mArrangeType == ListItemArrangeType.RightToLeft)
            {
                float containerPos = _mContainerTrans.anchoredPosition3D.x;
                if (containerPos < 0)
                {
                    containerPos = 0;
                }
                pos.x = -containerPos - offset;
            }

            RecycleAllItem();
            LoopListViewItem2 newItem = GetNewItemByIndex(itemIndex);
            if (newItem == null)
            {
                ClearAllTmpRecycledItem();
                return;
            }
            if (_mIsVertList)
            {
                pos.x = newItem.StartPosOffset;
            }
            else
            {
                pos.y = newItem.StartPosOffset;
            }
            newItem.CachedRectTransform.anchoredPosition3D = pos;
            if (mSupportScrollBar)
            {
                if (_mIsVertList)
                {
                    SetItemSize(itemIndex, newItem.CachedRectTransform.rect.height, newItem.Padding);
                }
                else
                {
                    SetItemSize(itemIndex, newItem.CachedRectTransform.rect.width, newItem.Padding);
                }
            }
            _mItemList.Add(newItem);
            UpdateContentSize();
            UpdateListView(viewPortSize + 100, viewPortSize + 100, viewPortSize, viewPortSize);
            AdjustPanelPos();
            ClearAllTmpRecycledItem();
            ForceSnapUpdateCheck();
            UpdateSnapMove(false,true);
        }

        //update all visible items.
        public void RefreshAllShownItem()
        {
            int count = _mItemList.Count;
            if (count == 0)
            {
                return;
            }
            RefreshAllShownItemWithFirstIndex(_mItemList[0].ItemIndex);
        }


        public void RefreshAllShownItemWithFirstIndex(int firstItemIndex)
        {
            int count = _mItemList.Count;
            if (count == 0)
            {
                return;
            }
            LoopListViewItem2 firstItem = _mItemList[0];
            Vector3 pos = firstItem.CachedRectTransform.anchoredPosition3D;
            RecycleAllItem();
            for (int i = 0; i < count; ++i)
            {
                int curIndex = firstItemIndex + i;
                LoopListViewItem2 newItem = GetNewItemByIndex(curIndex);
                if (newItem == null)
                {
                    break;
                }
                if (_mIsVertList)
                {
                    pos.x = newItem.StartPosOffset;
                }
                else
                {
                    pos.y = newItem.StartPosOffset;
                }
                newItem.CachedRectTransform.anchoredPosition3D = pos;
                if (mSupportScrollBar)
                {
                    if (_mIsVertList)
                    {
                        SetItemSize(curIndex, newItem.CachedRectTransform.rect.height, newItem.Padding);
                    }
                    else
                    {
                        SetItemSize(curIndex, newItem.CachedRectTransform.rect.width, newItem.Padding);
                    }
                }

                _mItemList.Add(newItem);
            }
            UpdateContentSize();
            UpdateAllShownItemsPos();
            ClearAllTmpRecycledItem();
        }


        public void RefreshAllShownItemWithFirstIndexAndPos(int firstItemIndex,Vector3 pos)
        {
            RecycleAllItem();
            LoopListViewItem2 newItem = GetNewItemByIndex(firstItemIndex);
            if (newItem == null)
            {
                return;
            }
            if (_mIsVertList)
            {
                pos.x = newItem.StartPosOffset;
            }
            else
            {
                pos.y = newItem.StartPosOffset;
            }
            newItem.CachedRectTransform.anchoredPosition3D = pos;
            if (mSupportScrollBar)
            {
                if (_mIsVertList)
                {
                    SetItemSize(firstItemIndex, newItem.CachedRectTransform.rect.height, newItem.Padding);
                }
                else
                {
                    SetItemSize(firstItemIndex, newItem.CachedRectTransform.rect.width, newItem.Padding);
                }
            }
            _mItemList.Add(newItem);
            UpdateContentSize();
            UpdateAllShownItemsPos();
            UpdateListView(_mDistanceForRecycle0, _mDistanceForRecycle1, _mDistanceForNew0, _mDistanceForNew1);
            ClearAllTmpRecycledItem();
        }


        void RecycleItemTmp(LoopListViewItem2 item)
        {
            if (item == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(item.ItemPrefabName))
            {
                return;
            }
            ItemPool pool = null;
            if (_mItemPoolDict.TryGetValue(item.ItemPrefabName, out pool) == false)
            {
                return;
            }
            pool.RecycleItem(item);

        }


        void ClearAllTmpRecycledItem()
        {
            int count = _mItemPoolList.Count;
            for(int i = 0;i<count;++i)
            {
                _mItemPoolList[i].ClearTmpRecycledItem();
            }
        }


        void RecycleAllItem()
        {
            foreach (LoopListViewItem2 item in _mItemList)
            {
                RecycleItemTmp(item);
            }
            _mItemList.Clear();
        }


        void AdjustContainerPivot(RectTransform rtf)
        {
            Vector2 pivot = rtf.pivot;
            if (mArrangeType == ListItemArrangeType.BottomToTop)
            {
                pivot.y = 0;
            }
            else if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                pivot.y = 1;
            }
            else if (mArrangeType == ListItemArrangeType.LeftToRight)
            {
                pivot.x = 0;
            }
            else if (mArrangeType == ListItemArrangeType.RightToLeft)
            {
                pivot.x = 1;
            }
            rtf.pivot = pivot;
        }


        void AdjustPivot(RectTransform rtf)
        {
            Vector2 pivot = rtf.pivot;

            if (mArrangeType == ListItemArrangeType.BottomToTop)
            {
                pivot.y = 0;
            }
            else if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                pivot.y = 1;
            }
            else if (mArrangeType == ListItemArrangeType.LeftToRight)
            {
                pivot.x = 0;
            }
            else if (mArrangeType == ListItemArrangeType.RightToLeft)
            {
                pivot.x = 1;
            }
            rtf.pivot = pivot;
        }

        void AdjustContainerAnchor(RectTransform rtf)
        {
            Vector2 anchorMin = rtf.anchorMin;
            Vector2 anchorMax = rtf.anchorMax;
            if (mArrangeType == ListItemArrangeType.BottomToTop)
            {
                anchorMin.y = 0;
                anchorMax.y = 0;
            }
            else if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                anchorMin.y = 1;
                anchorMax.y = 1;
            }
            else if (mArrangeType == ListItemArrangeType.LeftToRight)
            {
                anchorMin.x = 0;
                anchorMax.x = 0;
            }
            else if (mArrangeType == ListItemArrangeType.RightToLeft)
            {
                anchorMin.x = 1;
                anchorMax.x = 1;
            }
            rtf.anchorMin = anchorMin;
            rtf.anchorMax = anchorMax;
        }


        void AdjustAnchor(RectTransform rtf)
        {
            Vector2 anchorMin = rtf.anchorMin;
            Vector2 anchorMax = rtf.anchorMax;
            if (mArrangeType == ListItemArrangeType.BottomToTop)
            {
                anchorMin.y = 0;
                anchorMax.y = 0;
            }
            else if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                anchorMin.y = 1;
                anchorMax.y = 1;
            }
            else if (mArrangeType == ListItemArrangeType.LeftToRight)
            {
                anchorMin.x = 0;
                anchorMax.x = 0;
            }
            else if (mArrangeType == ListItemArrangeType.RightToLeft)
            {
                anchorMin.x = 1;
                anchorMax.x = 1;
            }
            rtf.anchorMin = anchorMin;
            rtf.anchorMax = anchorMax;
        }

        void InitItemPool()
        {
            foreach (ItemPrefabConfData data in mItemPrefabDataList)
            {
                if (data.mItemPrefab == null)
                {
                    Debug.LogError("A item prefab is null ");
                    continue;
                }
                string prefabName = data.mItemPrefab.name;
                if (_mItemPoolDict.ContainsKey(prefabName))
                {
                    Debug.LogError("A item prefab with unitName " + prefabName + " has existed!");
                    continue;
                }
                RectTransform rtf = data.mItemPrefab.GetComponent<RectTransform>();
                if (rtf == null)
                {
                    Debug.LogError("RectTransform component is not found in the prefab " + prefabName);
                    continue;
                }
                AdjustAnchor(rtf);
                AdjustPivot(rtf);
                LoopListViewItem2 tItem = data.mItemPrefab.GetComponent<LoopListViewItem2>();
                if (tItem == null)
                {
                    data.mItemPrefab.AddComponent<LoopListViewItem2>();
                }
                ItemPool pool = new ItemPool();
                pool.Init(data.mItemPrefab, data.mPadding,data.mStartPosOffset, data.mInitCreateCount, _mContainerTrans);
                _mItemPoolDict.Add(prefabName, pool);
                _mItemPoolList.Add(pool);
            }
        }



        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            _mIsDraging = true;
            CacheDragPointerEventData(eventData);
            _mCurSnapData.Clear();
            if(mOnBeginDragAction != null)
            {
                mOnBeginDragAction();
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            _mIsDraging = false;
            _mPointerEventData = null;
            if (mOnEndDragAction != null)
            {
                mOnEndDragAction();
            }
            ForceSnapUpdateCheck();
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            CacheDragPointerEventData(eventData);
            if (mOnDragingAction != null)
            {
                mOnDragingAction();
            }
        }

        void CacheDragPointerEventData(PointerEventData eventData)
        {
            if (_mPointerEventData == null)
            {
                _mPointerEventData = new PointerEventData(EventSystem.current);
            }
            _mPointerEventData.button = eventData.button;
            _mPointerEventData.position = eventData.position;
            _mPointerEventData.pointerPressRaycast = eventData.pointerPressRaycast;
            _mPointerEventData.pointerCurrentRaycast = eventData.pointerCurrentRaycast;
        }

        LoopListViewItem2 GetNewItemByIndex(int index)
        {
            if(mSupportScrollBar && index < 0)
            {
                return null;
            }
            if(_mItemTotalCount > 0 && index >= _mItemTotalCount)
            {
                return null;
            }
            LoopListViewItem2 newItem = _mOnGetItemByIndex(this, index);
            if (newItem == null)
            {
                return null;
            }
            newItem.ItemIndex = index;
            newItem.ItemCreatedCheckFrameCount = _mListUpdateCheckFrameCount;
            return newItem;
        }


        void SetItemSize(int itemIndex, float itemSize,float padding)
        {
            _mItemPosMgr.SetItemSize(itemIndex, itemSize+padding);
            if(itemIndex >= _mLastItemIndex)
            {
                _mLastItemIndex = itemIndex;
                _mLastItemPadding = padding;
            }
        }

        bool GetPlusItemIndexAndPosAtGivenPos(float pos, ref int index, ref float itemPos)
        {
            return _mItemPosMgr.GetItemIndexAndPosAtGivenPos(pos, ref index, ref itemPos);
        }


        float GetItemPos(int itemIndex)
        {
            return _mItemPosMgr.GetItemPos(itemIndex);
        }

      
        public Vector3 GetItemCornerPosInViewPort(LoopListViewItem2 item, ItemCornerEnum corner = ItemCornerEnum.LeftBottom)
        {
            item.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
            return _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[(int)corner]);
        }
       

        void AdjustPanelPos()
        {
            int count = _mItemList.Count;
            if (count == 0)
            {
                return;
            }
            UpdateAllShownItemsPos();
            float viewPortSize = ViewPortSize;
            float contentSize = GetContentPanelSize();
            if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                if (contentSize <= viewPortSize)
                {
                    Vector3 pos = _mContainerTrans.anchoredPosition3D;
                    pos.y = 0;
                    _mContainerTrans.anchoredPosition3D = pos;
                    _mItemList[0].CachedRectTransform.anchoredPosition3D = new Vector3(_mItemList[0].StartPosOffset,0,0);
                    UpdateAllShownItemsPos();
                    return;
                }
                LoopListViewItem2 tViewItem0 = _mItemList[0];
                tViewItem0.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 topPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                if (topPos0.y < _mViewPortRectLocalCorners[1].y)
                {
                    Vector3 pos = _mContainerTrans.anchoredPosition3D;
                    pos.y = 0;
                    _mContainerTrans.anchoredPosition3D = pos;
                    _mItemList[0].CachedRectTransform.anchoredPosition3D = new Vector3(_mItemList[0].StartPosOffset, 0, 0);
                    UpdateAllShownItemsPos();
                    return;
                }
                LoopListViewItem2 tViewItem1 = _mItemList[_mItemList.Count - 1];
                tViewItem1.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 downPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[0]);
                float d = downPos1.y - _mViewPortRectLocalCorners[0].y;
                if (d > 0)
                {
                    Vector3 pos = _mItemList[0].CachedRectTransform.anchoredPosition3D;
                    pos.y = pos.y - d;
                    _mItemList[0].CachedRectTransform.anchoredPosition3D = pos;
                    UpdateAllShownItemsPos();
                    return;
                }
            }
            else if (mArrangeType == ListItemArrangeType.BottomToTop)
            {
                if (contentSize <= viewPortSize)
                {
                    Vector3 pos = _mContainerTrans.anchoredPosition3D;
                    pos.y = 0;
                    _mContainerTrans.anchoredPosition3D = pos;
                    _mItemList[0].CachedRectTransform.anchoredPosition3D = new Vector3(_mItemList[0].StartPosOffset, 0, 0);
                    UpdateAllShownItemsPos();
                    return;
                }
                LoopListViewItem2 tViewItem0 = _mItemList[0];
                tViewItem0.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 downPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[0]);
                if (downPos0.y > _mViewPortRectLocalCorners[0].y)
                {
                    Vector3 pos = _mContainerTrans.anchoredPosition3D;
                    pos.y = 0;
                    _mContainerTrans.anchoredPosition3D = pos;
                    _mItemList[0].CachedRectTransform.anchoredPosition3D = new Vector3(_mItemList[0].StartPosOffset, 0, 0);
                    UpdateAllShownItemsPos();
                    return;
                }
                LoopListViewItem2 tViewItem1 = _mItemList[_mItemList.Count - 1];
                tViewItem1.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 topPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                float d = _mViewPortRectLocalCorners[1].y - topPos1.y;
                if (d > 0)
                {
                    Vector3 pos = _mItemList[0].CachedRectTransform.anchoredPosition3D;
                    pos.y = pos.y + d;
                    _mItemList[0].CachedRectTransform.anchoredPosition3D = pos;
                    UpdateAllShownItemsPos();
                    return;
                }
            }
            else if (mArrangeType == ListItemArrangeType.LeftToRight)
            {
                if (contentSize <= viewPortSize)
                {
                    Vector3 pos = _mContainerTrans.anchoredPosition3D;
                    pos.x = 0;
                    _mContainerTrans.anchoredPosition3D = pos;
                    _mItemList[0].CachedRectTransform.anchoredPosition3D = new Vector3(0,_mItemList[0].StartPosOffset, 0);
                    UpdateAllShownItemsPos();
                    return;
                }
                LoopListViewItem2 tViewItem0 = _mItemList[0];
                tViewItem0.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 leftPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                if (leftPos0.x > _mViewPortRectLocalCorners[1].x)
                {
                    Vector3 pos = _mContainerTrans.anchoredPosition3D;
                    pos.x = 0;
                    _mContainerTrans.anchoredPosition3D = pos;
                    _mItemList[0].CachedRectTransform.anchoredPosition3D = new Vector3(0, _mItemList[0].StartPosOffset, 0);
                    UpdateAllShownItemsPos();
                    return;
                }
                LoopListViewItem2 tViewItem1 = _mItemList[_mItemList.Count - 1];
                tViewItem1.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 rightPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[2]);
                float d = _mViewPortRectLocalCorners[2].x - rightPos1.x;
                if (d > 0)
                {
                    Vector3 pos = _mItemList[0].CachedRectTransform.anchoredPosition3D;
                    pos.x = pos.x + d;
                    _mItemList[0].CachedRectTransform.anchoredPosition3D = pos;
                    UpdateAllShownItemsPos();
                    return;
                }
            }
            else if (mArrangeType == ListItemArrangeType.RightToLeft)
            {
                if (contentSize <= viewPortSize)
                {
                    Vector3 pos = _mContainerTrans.anchoredPosition3D;
                    pos.x = 0;
                    _mContainerTrans.anchoredPosition3D = pos;
                    _mItemList[0].CachedRectTransform.anchoredPosition3D = new Vector3(0, _mItemList[0].StartPosOffset, 0);
                    UpdateAllShownItemsPos();
                    return;
                }
                LoopListViewItem2 tViewItem0 = _mItemList[0];
                tViewItem0.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 rightPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[2]);
                if (rightPos0.x < _mViewPortRectLocalCorners[2].x)
                {
                    Vector3 pos = _mContainerTrans.anchoredPosition3D;
                    pos.x = 0;
                    _mContainerTrans.anchoredPosition3D = pos;
                    _mItemList[0].CachedRectTransform.anchoredPosition3D = new Vector3(0, _mItemList[0].StartPosOffset, 0);
                    UpdateAllShownItemsPos();
                    return;
                }
                LoopListViewItem2 tViewItem1 = _mItemList[_mItemList.Count - 1];
                tViewItem1.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 leftPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                float d = leftPos1.x - _mViewPortRectLocalCorners[1].x;
                if (d > 0)
                {
                    Vector3 pos = _mItemList[0].CachedRectTransform.anchoredPosition3D;
                    pos.x = pos.x - d;
                    _mItemList[0].CachedRectTransform.anchoredPosition3D = pos;
                    UpdateAllShownItemsPos();
                    return;
                }
            }



        }


        void Update()
        {
            if(_mListViewInited == false)
            {
                return;
            }
            if(_mNeedAdjustVec)
            {
                _mNeedAdjustVec = false;
                if(_mIsVertList)
                {
                    if(_mScrollRect.velocity.y * _mAdjustedVec.y > 0)
                    {
                        _mScrollRect.velocity = _mAdjustedVec;
                    }
                }
                else
                {
                    if (_mScrollRect.velocity.x * _mAdjustedVec.x > 0)
                    {
                        _mScrollRect.velocity = _mAdjustedVec;
                    }
                }
                
            }
            if (mSupportScrollBar)
            {
                _mItemPosMgr.Update(false);
            }
            UpdateSnapMove();
            UpdateListView(_mDistanceForRecycle0, _mDistanceForRecycle1, _mDistanceForNew0, _mDistanceForNew1);
            ClearAllTmpRecycledItem();
            _mLastFrameContainerPos = _mContainerTrans.anchoredPosition3D;
        }

        //update snap move. if immediate is set true, then the snap move will finish at once.
        void UpdateSnapMove(bool immediate = false, bool forceSendEvent = false)
        {
            if (mItemSnapEnable == false)
            {
                return;
            }
            if (_mIsVertList)
            {
                UpdateSnapVertical(immediate,forceSendEvent);
            }
            else
            {
                UpdateSnapHorizontal(immediate,forceSendEvent);
            }
        }



        public void UpdateAllShownItemSnapData()
        {
            if (mItemSnapEnable == false)
            {
                return;
            }
            int count = _mItemList.Count;
            if (count == 0)
            {
                return;
            }
            Vector3 pos = _mContainerTrans.anchoredPosition3D;
            LoopListViewItem2 tViewItem0 = _mItemList[0];
            tViewItem0.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
            float start = 0;
            float end = 0;
            float itemSnapCenter = 0;
            float snapCenter = 0;
            if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                snapCenter = -(1 - mViewPortSnapPivot.y) * _mViewPortRectTransform.rect.height;
                Vector3 topPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                start = topPos1.y;
                end = start - tViewItem0.ItemSizeWithPadding;
                itemSnapCenter = start - tViewItem0.ItemSize * (1 - mItemSnapPivot.y);
                for (int i = 0; i < count; ++i)
                {
                    _mItemList[i].DistanceWithViewPortSnapCenter = snapCenter - itemSnapCenter;
                    if ((i + 1) < count)
                    {
                        start = end;
                        end = end - _mItemList[i + 1].ItemSizeWithPadding;
                        itemSnapCenter = start - _mItemList[i + 1].ItemSize * (1 - mItemSnapPivot.y);
                    }
                }
            }
            else if (mArrangeType == ListItemArrangeType.BottomToTop)
            {
                snapCenter = mViewPortSnapPivot.y * _mViewPortRectTransform.rect.height;
                Vector3 bottomPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[0]);
                start = bottomPos1.y;
                end = start + tViewItem0.ItemSizeWithPadding;
                itemSnapCenter = start + tViewItem0.ItemSize * mItemSnapPivot.y;
                for (int i = 0; i < count; ++i)
                {
                    _mItemList[i].DistanceWithViewPortSnapCenter = snapCenter - itemSnapCenter;
                    if ((i + 1) < count)
                    {
                        start = end;
                        end = end + _mItemList[i + 1].ItemSizeWithPadding;
                        itemSnapCenter = start + _mItemList[i + 1].ItemSize * mItemSnapPivot.y;
                    }
                }
            }
            else if (mArrangeType == ListItemArrangeType.RightToLeft)
            {
                snapCenter = -(1 - mViewPortSnapPivot.x) * _mViewPortRectTransform.rect.width;
                Vector3 rightPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[2]);
                start = rightPos1.x;
                end = start - tViewItem0.ItemSizeWithPadding;
                itemSnapCenter = start - tViewItem0.ItemSize * (1 - mItemSnapPivot.x);
                for (int i = 0; i < count; ++i)
                {
                    _mItemList[i].DistanceWithViewPortSnapCenter = snapCenter - itemSnapCenter;
                    if ((i + 1) < count)
                    {
                        start = end;
                        end = end - _mItemList[i + 1].ItemSizeWithPadding;
                        itemSnapCenter = start - _mItemList[i + 1].ItemSize * (1 - mItemSnapPivot.x);
                    }
                }
            }
            else if (mArrangeType == ListItemArrangeType.LeftToRight)
            {
                snapCenter = mViewPortSnapPivot.x * _mViewPortRectTransform.rect.width;
                Vector3 leftPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                start = leftPos1.x;
                end = start + tViewItem0.ItemSizeWithPadding;
                itemSnapCenter = start + tViewItem0.ItemSize * mItemSnapPivot.x;
                for (int i = 0; i < count; ++i)
                {
                    _mItemList[i].DistanceWithViewPortSnapCenter = snapCenter - itemSnapCenter;
                    if ((i + 1) < count)
                    {
                        start = end;
                        end = end + _mItemList[i + 1].ItemSizeWithPadding;
                        itemSnapCenter = start + _mItemList[i + 1].ItemSize * mItemSnapPivot.x;
                    }
                }
            }
        }



        void UpdateSnapVertical(bool immediate = false, bool forceSendEvent = false)
        {
            if(mItemSnapEnable == false)
            {
                return;
            }
            int count = _mItemList.Count;
            if (count == 0)
            {
                return;
            }
            Vector3 pos = _mContainerTrans.anchoredPosition3D;
            bool needCheck = (pos.y != _mLastSnapCheckPos.y);
            _mLastSnapCheckPos = pos;
            if (!needCheck)
            {
                if (_mLeftSnapUpdateExtraCount > 0)
                {
                    _mLeftSnapUpdateExtraCount--;
                    needCheck = true;
                }
            }
            if (needCheck)
            {
                LoopListViewItem2 tViewItem0 = _mItemList[0];
                tViewItem0.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                int curIndex = -1;
                float start = 0;
                float end = 0;
                float itemSnapCenter = 0;
                float curMinDist = float.MaxValue;
                float curDist = 0;
                float curDistAbs = 0;
                float snapCenter = 0; 
                if (mArrangeType == ListItemArrangeType.TopToBottom)
                {
                    snapCenter = -(1 - mViewPortSnapPivot.y) * _mViewPortRectTransform.rect.height;
                    Vector3 topPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                    start = topPos1.y;
                    end = start - tViewItem0.ItemSizeWithPadding;
                    itemSnapCenter = start - tViewItem0.ItemSize * (1-mItemSnapPivot.y);
                    for (int i = 0; i < count; ++i)
                    {
                        curDist = snapCenter - itemSnapCenter;
                        curDistAbs = Mathf.Abs(curDist);
                        if (curDistAbs < curMinDist)
                        {
                            curMinDist = curDistAbs;
                            curIndex = i;
                        }
                        else
                        {
                            break;
                        }
                        
                        if ((i + 1) < count)
                        {
                            start = end;
                            end = end - _mItemList[i + 1].ItemSizeWithPadding;
                            itemSnapCenter = start - _mItemList[i + 1].ItemSize * (1 - mItemSnapPivot.y);
                        }
                    }
                }
                else if(mArrangeType == ListItemArrangeType.BottomToTop)
                {
                    snapCenter = mViewPortSnapPivot.y * _mViewPortRectTransform.rect.height;
                    Vector3 bottomPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[0]);
                    start = bottomPos1.y;
                    end = start + tViewItem0.ItemSizeWithPadding;
                    itemSnapCenter = start + tViewItem0.ItemSize * mItemSnapPivot.y;
                    for (int i = 0; i < count; ++i)
                    {
                        curDist = snapCenter - itemSnapCenter;
                        curDistAbs = Mathf.Abs(curDist);
                        if (curDistAbs < curMinDist)
                        {
                            curMinDist = curDistAbs;
                            curIndex = i;
                        }
                        else
                        {
                            break;
                        }

                        if ((i + 1) < count)
                        {
                            start = end;
                            end = end + _mItemList[i + 1].ItemSizeWithPadding;
                            itemSnapCenter = start + _mItemList[i + 1].ItemSize *  mItemSnapPivot.y;
                        }
                    }
                }

                if (curIndex >= 0)
                {
                    int oldNearestItemIndex = _mCurSnapNearestItemIndex;
                    _mCurSnapNearestItemIndex = _mItemList[curIndex].ItemIndex;
                    if (forceSendEvent || _mItemList[curIndex].ItemIndex != oldNearestItemIndex)
                    {
                        if (mOnSnapNearestChanged != null)
                        {
                            mOnSnapNearestChanged(this,_mItemList[curIndex]);
                        }
                    }
                }
                else
                {
                    _mCurSnapNearestItemIndex = -1;
                }
            }
            if (CanSnap() == false)
            {
                ClearSnapData();
                return;
            }
            float v = Mathf.Abs(_mScrollRect.velocity.y);
            UpdateCurSnapData();
            if (_mCurSnapData.MSnapStatus != SnapStatus.SnapMoving)
            {
                return;
            }
            if (v > 0)
            {
                _mScrollRect.StopMovement();
            }
            float old = _mCurSnapData.MCurSnapVal;
            if(_mCurSnapData.MIsTempTarget == false)
            {
                if(_mSmoothDumpVel * _mCurSnapData.MTargetSnapVal < 0)
                {
                    _mSmoothDumpVel = 0;
                }
                _mCurSnapData.MCurSnapVal = Mathf.SmoothDamp(_mCurSnapData.MCurSnapVal, _mCurSnapData.MTargetSnapVal, ref _mSmoothDumpVel, _mSmoothDumpRate);
            }
            else
            {
                float maxAbsVec = _mCurSnapData.MMoveMaxAbsVec;
                if(maxAbsVec <= 0)
                {
                    maxAbsVec = _mSnapMoveDefaultMaxAbsVec;
                }
                _mSmoothDumpVel = maxAbsVec * Mathf.Sign(_mCurSnapData.MTargetSnapVal);
                _mCurSnapData.MCurSnapVal = Mathf.MoveTowards(_mCurSnapData.MCurSnapVal, _mCurSnapData.MTargetSnapVal, maxAbsVec * Time.deltaTime);
            }
            float dt = _mCurSnapData.MCurSnapVal - old;
                
            if (immediate || Mathf.Abs(_mCurSnapData.MTargetSnapVal - _mCurSnapData.MCurSnapVal) < _mSnapFinishThreshold)
            {
                pos.y = pos.y + _mCurSnapData.MTargetSnapVal - old;
                _mCurSnapData.MSnapStatus = SnapStatus.SnapMoveFinish;
                if (mOnSnapItemFinished != null)
                {
                    LoopListViewItem2 targetItem = GetShownItemByItemIndex(_mCurSnapNearestItemIndex);
                    if(targetItem != null)
                    {
                        mOnSnapItemFinished(this,targetItem);
                    }
                }
            }
            else
            {
                pos.y = pos.y + dt;
            }

            if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                float maxY = _mViewPortRectLocalCorners[0].y + _mContainerTrans.rect.height;
                pos.y = Mathf.Clamp(pos.y, 0, maxY);
                _mContainerTrans.anchoredPosition3D = pos;
            }
            else if (mArrangeType == ListItemArrangeType.BottomToTop)
            {
                float minY = _mViewPortRectLocalCorners[1].y - _mContainerTrans.rect.height;
                pos.y = Mathf.Clamp(pos.y, minY, 0);
                _mContainerTrans.anchoredPosition3D = pos;
            }

        }

        void UpdateCurSnapData()
        {
            int count = _mItemList.Count;
            if (count == 0)
            {
                _mCurSnapData.Clear();
                return;
            }

            if (_mCurSnapData.MSnapStatus == SnapStatus.SnapMoveFinish)
            {
                if (_mCurSnapData.MSnapTargetIndex == _mCurSnapNearestItemIndex)
                {
                    return;
                }
                _mCurSnapData.MSnapStatus = SnapStatus.NoTargetSet;
            }
            if (_mCurSnapData.MSnapStatus == SnapStatus.SnapMoving)
            {
                if(_mCurSnapData.MIsForceSnapTo)
                {
                    if (_mCurSnapData.MIsTempTarget == true)
                    {
                        LoopListViewItem2 targetItem = GetShownItemNearestItemIndex(_mCurSnapData.MSnapTargetIndex);
                        if (targetItem == null)
                        {
                            _mCurSnapData.Clear();
                            return;
                        }
                        if (targetItem.ItemIndex == _mCurSnapData.MSnapTargetIndex)
                        {
                            UpdateAllShownItemSnapData();
                            _mCurSnapData.MTargetSnapVal = targetItem.DistanceWithViewPortSnapCenter;
                            _mCurSnapData.MCurSnapVal = 0;
                            _mCurSnapData.MIsTempTarget = false;
                            _mCurSnapData.MSnapStatus = SnapStatus.SnapMoving;
                            return;
                        }
                        if (_mCurSnapData.MTempTargetIndex != targetItem.ItemIndex)
                        {
                            UpdateAllShownItemSnapData();
                            _mCurSnapData.MTargetSnapVal = targetItem.DistanceWithViewPortSnapCenter;
                            _mCurSnapData.MCurSnapVal = 0;
                            _mCurSnapData.MSnapStatus = SnapStatus.SnapMoving;
                            _mCurSnapData.MIsTempTarget = true;
                            _mCurSnapData.MTempTargetIndex = targetItem.ItemIndex;
                            return;
                        }
                    }
                    return;
                }
                if ((_mCurSnapData.MSnapTargetIndex == _mCurSnapNearestItemIndex))
                {
                    return;
                }
                _mCurSnapData.MSnapStatus = SnapStatus.NoTargetSet;
            }
            if (_mCurSnapData.MSnapStatus == SnapStatus.NoTargetSet)
            {
                LoopListViewItem2 nearestItem = GetShownItemByItemIndex(_mCurSnapNearestItemIndex);
                if (nearestItem == null)
                {
                    return;
                }
                _mCurSnapData.MSnapTargetIndex = _mCurSnapNearestItemIndex;
                _mCurSnapData.MSnapStatus = SnapStatus.TargetHasSet;
                _mCurSnapData.MIsForceSnapTo = false;
            }
            if (_mCurSnapData.MSnapStatus == SnapStatus.TargetHasSet)
            {
                LoopListViewItem2 targetItem = GetShownItemNearestItemIndex(_mCurSnapData.MSnapTargetIndex);
                if (targetItem == null)
                {
                    _mCurSnapData.Clear();
                    return;
                }
                if(targetItem.ItemIndex == _mCurSnapData.MSnapTargetIndex)
                {
                    UpdateAllShownItemSnapData();
                    _mCurSnapData.MTargetSnapVal = targetItem.DistanceWithViewPortSnapCenter;
                    _mCurSnapData.MCurSnapVal = 0;
                    _mCurSnapData.MIsTempTarget = false;
                    _mCurSnapData.MSnapStatus = SnapStatus.SnapMoving;
                }
                else
                {
                    UpdateAllShownItemSnapData();
                    _mCurSnapData.MTargetSnapVal = targetItem.DistanceWithViewPortSnapCenter;
                    _mCurSnapData.MCurSnapVal = 0;
                    _mCurSnapData.MSnapStatus = SnapStatus.SnapMoving;
                    _mCurSnapData.MIsTempTarget = true;
                    _mCurSnapData.MTempTargetIndex = targetItem.ItemIndex;
                }
                
            }

        }
        //Clear current snap target and then the LoopScrollView2 will auto snap to the CurSnapNearestItemIndex.
        public void ClearSnapData()
        {
            _mCurSnapData.Clear();
        }

        //moveMaxAbsVec param is the max abs snap move speed, if the value <= 0 then LoopListView2 would use SnapMoveDefaultMaxAbsVec
        public void SetSnapTargetItemIndex(int itemIndex,float moveMaxAbsVec = -1)
        {
            if(_mItemTotalCount > 0)
            {
                if(itemIndex >= _mItemTotalCount)
                {
                    itemIndex = _mItemTotalCount - 1;
                }
                if(itemIndex < 0)
                {
                    itemIndex = 0;
                }
            }
            _mScrollRect.StopMovement();
            _mCurSnapData.MSnapTargetIndex = itemIndex;
            _mCurSnapData.MSnapStatus = SnapStatus.TargetHasSet;
            _mCurSnapData.MIsForceSnapTo = true;
            _mCurSnapData.MMoveMaxAbsVec = moveMaxAbsVec;
        }

        //Get the nearest item index with the viewport snap point.
        public int CurSnapNearestItemIndex
        {
            get{ return _mCurSnapNearestItemIndex; }
        }

        public void ForceSnapUpdateCheck()
        {
            if(_mLeftSnapUpdateExtraCount <= 0)
            {
                _mLeftSnapUpdateExtraCount = 1;
            }
        }

        void UpdateSnapHorizontal(bool immediate = false, bool forceSendEvent = false)
        {
            if (mItemSnapEnable == false)
            {
                return;
            }
            int count = _mItemList.Count;
            if (count == 0)
            {
                return;
            }
            Vector3 pos = _mContainerTrans.anchoredPosition3D;
            bool needCheck = (pos.x != _mLastSnapCheckPos.x);
            _mLastSnapCheckPos = pos;
            if (!needCheck)
            {
                if(_mLeftSnapUpdateExtraCount > 0)
                {
                    _mLeftSnapUpdateExtraCount--;
                    needCheck = true;
                }
            }
            if (needCheck)
            {
                LoopListViewItem2 tViewItem0 = _mItemList[0];
                tViewItem0.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                int curIndex = -1;
                float start = 0;
                float end = 0;
                float itemSnapCenter = 0;
                float curMinDist = float.MaxValue;
                float curDist = 0;
                float curDistAbs = 0;
                float snapCenter = 0;
                if (mArrangeType == ListItemArrangeType.RightToLeft)
                {
                    snapCenter = -(1 - mViewPortSnapPivot.x) * _mViewPortRectTransform.rect.width;
                    Vector3 rightPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[2]);
                    start = rightPos1.x;
                    end = start - tViewItem0.ItemSizeWithPadding;
                    itemSnapCenter = start - tViewItem0.ItemSize * (1 - mItemSnapPivot.x);
                    for (int i = 0; i < count; ++i)
                    {
                        curDist = snapCenter - itemSnapCenter;
                        curDistAbs = Mathf.Abs(curDist);
                        if (curDistAbs < curMinDist)
                        {
                            curMinDist = curDistAbs;
                            curIndex = i;
                        }
                        else
                        {
                            break;
                        }

                        if ((i + 1) < count)
                        {
                            start = end;
                            end = end - _mItemList[i + 1].ItemSizeWithPadding;
                            itemSnapCenter = start - _mItemList[i + 1].ItemSize * (1 - mItemSnapPivot.x);
                        }
                    }
                }
                else if (mArrangeType == ListItemArrangeType.LeftToRight)
                {
                    snapCenter = mViewPortSnapPivot.x * _mViewPortRectTransform.rect.width;
                    Vector3 leftPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                    start = leftPos1.x;
                    end = start + tViewItem0.ItemSizeWithPadding;
                    itemSnapCenter = start + tViewItem0.ItemSize * mItemSnapPivot.x;
                    for (int i = 0; i < count; ++i)
                    {
                        curDist = snapCenter - itemSnapCenter;
                        curDistAbs = Mathf.Abs(curDist);
                        if (curDistAbs < curMinDist)
                        {
                            curMinDist = curDistAbs;
                            curIndex = i;
                        }
                        else
                        {
                            break;
                        }

                        if ((i + 1) < count)
                        {
                            start = end;
                            end = end + _mItemList[i + 1].ItemSizeWithPadding;
                            itemSnapCenter = start + _mItemList[i + 1].ItemSize * mItemSnapPivot.x;
                        }
                    }
                }


                if (curIndex >= 0)
                {
                    int oldNearestItemIndex = _mCurSnapNearestItemIndex;
                    _mCurSnapNearestItemIndex = _mItemList[curIndex].ItemIndex;
                    if (forceSendEvent || _mItemList[curIndex].ItemIndex != oldNearestItemIndex)
                    {
                        if (mOnSnapNearestChanged != null)
                        {
                            mOnSnapNearestChanged(this, _mItemList[curIndex]);
                        }
                    }
                }
                else
                {
                    _mCurSnapNearestItemIndex = -1;
                }
            }
            if (CanSnap() == false)
            {
                ClearSnapData();
                return;
            }
            float v = Mathf.Abs(_mScrollRect.velocity.x);
            UpdateCurSnapData();
            if(_mCurSnapData.MSnapStatus != SnapStatus.SnapMoving)
            {
                return;
            }
            if (v > 0)
            {
                _mScrollRect.StopMovement();
            }
            float old = _mCurSnapData.MCurSnapVal;
            if (_mCurSnapData.MIsTempTarget == false)
            {
                if (_mSmoothDumpVel * _mCurSnapData.MTargetSnapVal < 0)
                {
                    _mSmoothDumpVel = 0;
                }
                _mCurSnapData.MCurSnapVal = Mathf.SmoothDamp(_mCurSnapData.MCurSnapVal, _mCurSnapData.MTargetSnapVal, ref _mSmoothDumpVel, _mSmoothDumpRate);
            }
            else
            {
                float maxAbsVec = _mCurSnapData.MMoveMaxAbsVec;
                if (maxAbsVec <= 0)
                {
                    maxAbsVec = _mSnapMoveDefaultMaxAbsVec;
                }
                _mSmoothDumpVel = maxAbsVec * Mathf.Sign(_mCurSnapData.MTargetSnapVal);
                _mCurSnapData.MCurSnapVal = Mathf.MoveTowards(_mCurSnapData.MCurSnapVal, _mCurSnapData.MTargetSnapVal, maxAbsVec * Time.deltaTime);
            }
            float dt = _mCurSnapData.MCurSnapVal - old;

            if (immediate || Mathf.Abs(_mCurSnapData.MTargetSnapVal - _mCurSnapData.MCurSnapVal) < _mSnapFinishThreshold)
            {
                pos.x = pos.x + _mCurSnapData.MTargetSnapVal - old;
                _mCurSnapData.MSnapStatus = SnapStatus.SnapMoveFinish;
                if (mOnSnapItemFinished != null)
                {
                    LoopListViewItem2 targetItem = GetShownItemByItemIndex(_mCurSnapNearestItemIndex);
                    if (targetItem != null)
                    {
                        mOnSnapItemFinished(this, targetItem);
                    }
                }
            }
            else
            {
                pos.x = pos.x + dt;
            }
                
            if (mArrangeType == ListItemArrangeType.LeftToRight)
            {
                float minX = _mViewPortRectLocalCorners[2].x - _mContainerTrans.rect.width;
                pos.x = Mathf.Clamp(pos.x, minX, 0);
                _mContainerTrans.anchoredPosition3D = pos;
            }
            else if (mArrangeType == ListItemArrangeType.RightToLeft)
            {
                float maxX = _mViewPortRectLocalCorners[1].x + _mContainerTrans.rect.width;
                pos.x = Mathf.Clamp(pos.x, 0, maxX);
                _mContainerTrans.anchoredPosition3D = pos;
            }
        }

        bool CanSnap()
        {
            if (_mIsDraging)
            {
                return false;
            }
            if (_mScrollBarClickEventListener != null)
            {
                if (_mScrollBarClickEventListener.IsPressd)
                {
                    return false;
                }
            }

            if (_mIsVertList)
            {
                if(_mContainerTrans.rect.height <= ViewPortHeight)
                {
                    return false;
                }
            }
            else
            {
                if (_mContainerTrans.rect.width <= ViewPortWidth)
                {
                    return false;
                }
            }

            float v = 0;
            if (_mIsVertList)
            {
                v = Mathf.Abs(_mScrollRect.velocity.y);
            }
            else
            {
                v = Mathf.Abs(_mScrollRect.velocity.x);
            }
            if (v > _mSnapVecThreshold)
            {
                return false;
            }
            float diff = 3;
            Vector3 pos = _mContainerTrans.anchoredPosition3D;
            if (mArrangeType == ListItemArrangeType.LeftToRight)
            {
                float minX = _mViewPortRectLocalCorners[2].x - _mContainerTrans.rect.width;
                if (pos.x < (minX - diff) || pos.x > diff)
                {
                    return false;
                }
            }
            else if (mArrangeType == ListItemArrangeType.RightToLeft)
            {
                float maxX = _mViewPortRectLocalCorners[1].x + _mContainerTrans.rect.width;
                if (pos.x > (maxX + diff) || pos.x < -diff)
                {
                    return false;
                }
            }
            else if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                float maxY = _mViewPortRectLocalCorners[0].y + _mContainerTrans.rect.height;
                if (pos.y > (maxY + diff) || pos.y < -diff)
                {
                    return false;
                }
            }
            else if (mArrangeType == ListItemArrangeType.BottomToTop)
            {
                float minY = _mViewPortRectLocalCorners[1].y - _mContainerTrans.rect.height;
                if (pos.y < (minY - diff) || pos.y > diff)
                {
                    return false;
                }
            }
            return true;
        }



        public void UpdateListView(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0, float distanceForNew1)
        {
            _mListUpdateCheckFrameCount++;
            if (_mIsVertList)
            {
                bool needContinueCheck = true;
                int checkCount = 0;
                int maxCount = 9999;
                while (needContinueCheck)
                {
                    checkCount++;
                    if(checkCount >= maxCount)
                    {
                        Debug.LogError("UpdateListView Vertical while loop " + checkCount + " times! something is wrong!");
                        break;
                    }
                    needContinueCheck = UpdateForVertList(distanceForRecycle0, distanceForRecycle1, distanceForNew0, distanceForNew1);
                }
            }
            else
            {
                bool needContinueCheck = true;
                int checkCount = 0;
                int maxCount = 9999;
                while (needContinueCheck)
                {
                    checkCount++;
                    if (checkCount >= maxCount)
                    {
                        Debug.LogError("UpdateListView  Horizontal while loop " + checkCount + " times! something is wrong!");
                        break;
                    }
                    needContinueCheck = UpdateForHorizontalList(distanceForRecycle0, distanceForRecycle1, distanceForNew0, distanceForNew1);
                }
            }

        }



        bool UpdateForVertList(float distanceForRecycle0,float distanceForRecycle1,float distanceForNew0, float distanceForNew1)
        {
            if (_mItemTotalCount == 0)
            {
                if(_mItemList.Count > 0)
                {
                    RecycleAllItem();
                }
                return false;
            }
            if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                int itemListCount = _mItemList.Count;
                if (itemListCount == 0)
                {
                    float curY = _mContainerTrans.anchoredPosition3D.y;
                    if (curY < 0)
                    {
                        curY = 0;
                    }
                    int index = 0;
                    float pos = -curY;
                    if (mSupportScrollBar)
                    {
                        if( GetPlusItemIndexAndPosAtGivenPos(curY, ref index, ref pos) == false)
                        {
                            return false;
                        }
                        pos = -pos;
                    }
                    LoopListViewItem2 newItem = GetNewItemByIndex(index);
                    if (newItem == null)
                    {
                        return false;
                    }
                    if (mSupportScrollBar)
                    {
                        SetItemSize(index, newItem.CachedRectTransform.rect.height, newItem.Padding);
                    }
                    _mItemList.Add(newItem);
                    newItem.CachedRectTransform.anchoredPosition3D = new Vector3(newItem.StartPosOffset, pos, 0);
                    UpdateContentSize();
                    return true;
                }
                LoopListViewItem2 tViewItem0 = _mItemList[0];
                tViewItem0.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 topPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                Vector3 downPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[0]);

                if (!_mIsDraging && tViewItem0.ItemCreatedCheckFrameCount != _mListUpdateCheckFrameCount
                    && downPos0.y - _mViewPortRectLocalCorners[1].y > distanceForRecycle0)
                {
                    _mItemList.RemoveAt(0);
                    RecycleItemTmp(tViewItem0);
                    if (!mSupportScrollBar)
                    {
                        UpdateContentSize();
                        CheckIfNeedUpdataItemPos();
                    }
                    return true;
                }

                LoopListViewItem2 tViewItem1 = _mItemList[_mItemList.Count - 1];
                tViewItem1.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 topPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                Vector3 downPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[0]);
                if (!_mIsDraging && tViewItem1.ItemCreatedCheckFrameCount != _mListUpdateCheckFrameCount
                    && _mViewPortRectLocalCorners[0].y - topPos1.y > distanceForRecycle1)
                {
                    _mItemList.RemoveAt(_mItemList.Count - 1);
                    RecycleItemTmp(tViewItem1);
                    if (!mSupportScrollBar)
                    {
                        UpdateContentSize();
                        CheckIfNeedUpdataItemPos();
                    }
                    return true;
                }



                if (_mViewPortRectLocalCorners[0].y - downPos1.y < distanceForNew1)
                {
                    if(tViewItem1.ItemIndex > _mCurReadyMaxItemIndex)
                    {
                        _mCurReadyMaxItemIndex = tViewItem1.ItemIndex;
                        _mNeedCheckNextMaxItem = true;
                    }
                    int nIndex = tViewItem1.ItemIndex + 1;
                    if (nIndex <= _mCurReadyMaxItemIndex || _mNeedCheckNextMaxItem)
                    {
                        LoopListViewItem2 newItem = GetNewItemByIndex(nIndex);
                        if (newItem == null)
                        {
                            _mCurReadyMaxItemIndex = tViewItem1.ItemIndex;
                            _mNeedCheckNextMaxItem = false;
                            CheckIfNeedUpdataItemPos();
                        }
                        else
                        {
                            if (mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.height, newItem.Padding);
                            }
                            _mItemList.Add(newItem);
                            float y = tViewItem1.CachedRectTransform.anchoredPosition3D.y - tViewItem1.CachedRectTransform.rect.height - tViewItem1.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(newItem.StartPosOffset, y, 0);
                            UpdateContentSize();
                            CheckIfNeedUpdataItemPos();

                            if (nIndex > _mCurReadyMaxItemIndex)
                            {
                                _mCurReadyMaxItemIndex = nIndex;
                            }
                            return true;
                        }
                        
                    }

                }

                if (topPos0.y - _mViewPortRectLocalCorners[1].y < distanceForNew0)
                {
                    if(tViewItem0.ItemIndex < _mCurReadyMinItemIndex)
                    {
                        _mCurReadyMinItemIndex = tViewItem0.ItemIndex;
                        _mNeedCheckNextMinItem = true;
                    }
                    int nIndex = tViewItem0.ItemIndex - 1;
                    if (nIndex >= _mCurReadyMinItemIndex || _mNeedCheckNextMinItem)
                    {
                        LoopListViewItem2 newItem = GetNewItemByIndex(nIndex);
                        if (newItem == null)
                        {
                            _mCurReadyMinItemIndex = tViewItem0.ItemIndex;
                            _mNeedCheckNextMinItem = false;
                        }
                        else
                        {
                            if (mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.height, newItem.Padding);
                            }
                            _mItemList.Insert(0, newItem);
                            float y = tViewItem0.CachedRectTransform.anchoredPosition3D.y + newItem.CachedRectTransform.rect.height + newItem.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(newItem.StartPosOffset, y, 0);
                            UpdateContentSize();
                            CheckIfNeedUpdataItemPos();
                            if (nIndex < _mCurReadyMinItemIndex)
                            {
                                _mCurReadyMinItemIndex = nIndex;
                            }
                            return true;
                        }
                        
                    }

                }

            }
            else
            {
                
                if (_mItemList.Count == 0)
                {
                    float curY = _mContainerTrans.anchoredPosition3D.y;
                    if (curY > 0)
                    {
                        curY = 0;
                    }
                    int index = 0;
                    float pos = -curY;
                    if (mSupportScrollBar)
                    {
                        if(GetPlusItemIndexAndPosAtGivenPos(-curY, ref index, ref pos) == false)
                        {
                            return false;
                        }
                    }
                    LoopListViewItem2 newItem = GetNewItemByIndex(index);
                    if (newItem == null)
                    {
                        return false;
                    }
                    if (mSupportScrollBar)
                    {
                        SetItemSize(index, newItem.CachedRectTransform.rect.height, newItem.Padding);
                    }
                    _mItemList.Add(newItem);
                    newItem.CachedRectTransform.anchoredPosition3D = new Vector3(newItem.StartPosOffset, pos, 0);
                    UpdateContentSize();
                    return true;
                }
                LoopListViewItem2 tViewItem0 = _mItemList[0];
                tViewItem0.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 topPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                Vector3 downPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[0]);

                if (!_mIsDraging && tViewItem0.ItemCreatedCheckFrameCount != _mListUpdateCheckFrameCount
                    && _mViewPortRectLocalCorners[0].y - topPos0.y > distanceForRecycle0)
                {
                    _mItemList.RemoveAt(0);
                    RecycleItemTmp(tViewItem0);
                    if (!mSupportScrollBar)
                    {
                        UpdateContentSize();
                        CheckIfNeedUpdataItemPos();
                    }
                    return true;
                }

                LoopListViewItem2 tViewItem1 = _mItemList[_mItemList.Count - 1];
                tViewItem1.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 topPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                Vector3 downPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[0]);
                if (!_mIsDraging && tViewItem1.ItemCreatedCheckFrameCount != _mListUpdateCheckFrameCount
                     && downPos1.y - _mViewPortRectLocalCorners[1].y > distanceForRecycle1)
                {
                    _mItemList.RemoveAt(_mItemList.Count - 1);
                    RecycleItemTmp(tViewItem1);
                    if (!mSupportScrollBar)
                    {
                        UpdateContentSize();
                        CheckIfNeedUpdataItemPos();
                    }
                    return true;
                }

                if (topPos1.y - _mViewPortRectLocalCorners[1].y < distanceForNew1)
                {
                    if (tViewItem1.ItemIndex > _mCurReadyMaxItemIndex)
                    {
                        _mCurReadyMaxItemIndex = tViewItem1.ItemIndex;
                        _mNeedCheckNextMaxItem = true;
                    }
                    int nIndex = tViewItem1.ItemIndex + 1;
                    if (nIndex <= _mCurReadyMaxItemIndex || _mNeedCheckNextMaxItem)
                    {
                        LoopListViewItem2 newItem = GetNewItemByIndex(nIndex);
                        if (newItem == null)
                        {
                            _mNeedCheckNextMaxItem = false;
                            CheckIfNeedUpdataItemPos();
                        }
                        else
                        {
                            if (mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.height, newItem.Padding);
                            }
                            _mItemList.Add(newItem);
                            float y = tViewItem1.CachedRectTransform.anchoredPosition3D.y + tViewItem1.CachedRectTransform.rect.height + tViewItem1.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(newItem.StartPosOffset, y, 0);
                            UpdateContentSize();
                            CheckIfNeedUpdataItemPos();
                            if (nIndex > _mCurReadyMaxItemIndex)
                            {
                                _mCurReadyMaxItemIndex = nIndex;
                            }
                            return true;
                        }
                        
                    }

                }


                if (_mViewPortRectLocalCorners[0].y - downPos0.y < distanceForNew0)
                {
                    if (tViewItem0.ItemIndex < _mCurReadyMinItemIndex)
                    {
                        _mCurReadyMinItemIndex = tViewItem0.ItemIndex;
                        _mNeedCheckNextMinItem = true;
                    }
                    int nIndex = tViewItem0.ItemIndex - 1;
                    if (nIndex >= _mCurReadyMinItemIndex || _mNeedCheckNextMinItem)
                    {
                        LoopListViewItem2 newItem = GetNewItemByIndex(nIndex);
                        if (newItem == null)
                        {
                            _mNeedCheckNextMinItem = false;
                            return false;
                        }
                        else
                        {
                            if (mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.height, newItem.Padding);
                            }
                            _mItemList.Insert(0, newItem);
                            float y = tViewItem0.CachedRectTransform.anchoredPosition3D.y - newItem.CachedRectTransform.rect.height - newItem.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(newItem.StartPosOffset, y, 0);
                            UpdateContentSize();
                            CheckIfNeedUpdataItemPos();
                            if (nIndex < _mCurReadyMinItemIndex)
                            {
                                _mCurReadyMinItemIndex = nIndex;
                            }
                            return true;
                        }
                        
                    }
                }


            }

            return false;

        }





        bool UpdateForHorizontalList(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0, float distanceForNew1)
        {
            if (_mItemTotalCount == 0)
            {
                if (_mItemList.Count > 0)
                {
                    RecycleAllItem();
                }
                return false;
            }
            if (mArrangeType == ListItemArrangeType.LeftToRight)
            {

                if (_mItemList.Count == 0)
                {
                    float curX = _mContainerTrans.anchoredPosition3D.x;
                    if (curX > 0)
                    {
                        curX = 0;
                    }
                    int index = 0;
                    float pos = -curX;
                    if (mSupportScrollBar)
                    {
                        if(GetPlusItemIndexAndPosAtGivenPos(-curX, ref index, ref pos) == false)
                        {
                            return false;
                        }
                    }
                    LoopListViewItem2 newItem = GetNewItemByIndex(index);
                    if (newItem == null)
                    {
                        return false;
                    }
                    if (mSupportScrollBar)
                    {
                        SetItemSize(index, newItem.CachedRectTransform.rect.width, newItem.Padding);
                    }
                    _mItemList.Add(newItem);
                    newItem.CachedRectTransform.anchoredPosition3D = new Vector3(pos, newItem.StartPosOffset, 0);
                    UpdateContentSize();
                    return true;
                }
                LoopListViewItem2 tViewItem0 = _mItemList[0];
                tViewItem0.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 leftPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                Vector3 rightPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[2]);

                if (!_mIsDraging && tViewItem0.ItemCreatedCheckFrameCount != _mListUpdateCheckFrameCount
                    && _mViewPortRectLocalCorners[1].x - rightPos0.x > distanceForRecycle0)
                {
                    _mItemList.RemoveAt(0);
                    RecycleItemTmp(tViewItem0);
                    if (!mSupportScrollBar)
                    {
                        UpdateContentSize();
                        CheckIfNeedUpdataItemPos();
                    }
                    return true;
                }

                LoopListViewItem2 tViewItem1 = _mItemList[_mItemList.Count - 1];
                tViewItem1.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 leftPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                Vector3 rightPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[2]);
                if (!_mIsDraging && tViewItem1.ItemCreatedCheckFrameCount != _mListUpdateCheckFrameCount
                    && leftPos1.x - _mViewPortRectLocalCorners[2].x> distanceForRecycle1)
                {
                    _mItemList.RemoveAt(_mItemList.Count - 1);
                    RecycleItemTmp(tViewItem1);
                    if (!mSupportScrollBar)
                    {
                        UpdateContentSize();
                        CheckIfNeedUpdataItemPos();
                    }
                    return true;
                }



                if (rightPos1.x - _mViewPortRectLocalCorners[2].x < distanceForNew1)
                {
                    if (tViewItem1.ItemIndex > _mCurReadyMaxItemIndex)
                    {
                        _mCurReadyMaxItemIndex = tViewItem1.ItemIndex;
                        _mNeedCheckNextMaxItem = true;
                    }
                    int nIndex = tViewItem1.ItemIndex + 1;
                    if (nIndex <= _mCurReadyMaxItemIndex || _mNeedCheckNextMaxItem)
                    {
                        LoopListViewItem2 newItem = GetNewItemByIndex(nIndex);
                        if (newItem == null)
                        {
                            _mCurReadyMaxItemIndex = tViewItem1.ItemIndex;
                            _mNeedCheckNextMaxItem = false;
                            CheckIfNeedUpdataItemPos();
                        }
                        else
                        {
                            if (mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.width, newItem.Padding);
                            }
                            _mItemList.Add(newItem);
                            float x = tViewItem1.CachedRectTransform.anchoredPosition3D.x + tViewItem1.CachedRectTransform.rect.width + tViewItem1.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(x, newItem.StartPosOffset, 0);
                            UpdateContentSize();
                            CheckIfNeedUpdataItemPos();

                            if (nIndex > _mCurReadyMaxItemIndex)
                            {
                                _mCurReadyMaxItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }

                if ( _mViewPortRectLocalCorners[1].x - leftPos0.x < distanceForNew0)
                {
                    if (tViewItem0.ItemIndex < _mCurReadyMinItemIndex)
                    {
                        _mCurReadyMinItemIndex = tViewItem0.ItemIndex;
                        _mNeedCheckNextMinItem = true;
                    }
                    int nIndex = tViewItem0.ItemIndex - 1;
                    if (nIndex >= _mCurReadyMinItemIndex || _mNeedCheckNextMinItem)
                    {
                        LoopListViewItem2 newItem = GetNewItemByIndex(nIndex);
                        if (newItem == null)
                        {
                            _mCurReadyMinItemIndex = tViewItem0.ItemIndex;
                            _mNeedCheckNextMinItem = false;
                        }
                        else
                        {
                            if (mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.width, newItem.Padding);
                            }
                            _mItemList.Insert(0, newItem);
                            float x = tViewItem0.CachedRectTransform.anchoredPosition3D.x - newItem.CachedRectTransform.rect.width - newItem.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(x, newItem.StartPosOffset, 0);
                            UpdateContentSize();
                            CheckIfNeedUpdataItemPos();
                            if (nIndex < _mCurReadyMinItemIndex)
                            {
                                _mCurReadyMinItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }

            }
            else
            {

                if (_mItemList.Count == 0)
                {
                    float curX = _mContainerTrans.anchoredPosition3D.x;
                    if (curX < 0)
                    {
                        curX = 0;
                    }
                    int index = 0;
                    float pos = -curX;
                    if (mSupportScrollBar)
                    {
                        if(GetPlusItemIndexAndPosAtGivenPos(curX, ref index, ref pos) == false)
                        {
                            return false;
                        }
                        pos = -pos;
                    }
                    LoopListViewItem2 newItem = GetNewItemByIndex(index);
                    if (newItem == null)
                    {
                        return false;
                    }
                    if (mSupportScrollBar)
                    {
                        SetItemSize(index, newItem.CachedRectTransform.rect.width, newItem.Padding);
                    }
                    _mItemList.Add(newItem);
                    newItem.CachedRectTransform.anchoredPosition3D = new Vector3(pos, newItem.StartPosOffset, 0);
                    UpdateContentSize();
                    return true;
                }
                LoopListViewItem2 tViewItem0 = _mItemList[0];
                tViewItem0.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 leftPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                Vector3 rightPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[2]);

                if (!_mIsDraging && tViewItem0.ItemCreatedCheckFrameCount != _mListUpdateCheckFrameCount
                    && leftPos0.x - _mViewPortRectLocalCorners[2].x > distanceForRecycle0)
                {
                    _mItemList.RemoveAt(0);
                    RecycleItemTmp(tViewItem0);
                    if (!mSupportScrollBar)
                    {
                        UpdateContentSize();
                        CheckIfNeedUpdataItemPos();
                    }
                    return true;
                }

                LoopListViewItem2 tViewItem1 = _mItemList[_mItemList.Count - 1];
                tViewItem1.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 leftPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                Vector3 rightPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[2]);
                if (!_mIsDraging && tViewItem1.ItemCreatedCheckFrameCount != _mListUpdateCheckFrameCount
                    && _mViewPortRectLocalCorners[1].x - rightPos1.x > distanceForRecycle1)
                {
                    _mItemList.RemoveAt(_mItemList.Count - 1);
                    RecycleItemTmp(tViewItem1);
                    if (!mSupportScrollBar)
                    {
                        UpdateContentSize();
                        CheckIfNeedUpdataItemPos();
                    }
                    return true;
                }



                if (_mViewPortRectLocalCorners[1].x - leftPos1.x  < distanceForNew1)
                {
                    if (tViewItem1.ItemIndex > _mCurReadyMaxItemIndex)
                    {
                        _mCurReadyMaxItemIndex = tViewItem1.ItemIndex;
                        _mNeedCheckNextMaxItem = true;
                    }
                    int nIndex = tViewItem1.ItemIndex + 1;
                    if (nIndex <= _mCurReadyMaxItemIndex || _mNeedCheckNextMaxItem)
                    {
                        LoopListViewItem2 newItem = GetNewItemByIndex(nIndex);
                        if (newItem == null)
                        {
                            _mCurReadyMaxItemIndex = tViewItem1.ItemIndex;
                            _mNeedCheckNextMaxItem = false;
                            CheckIfNeedUpdataItemPos();
                        }
                        else
                        {
                            if (mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.width, newItem.Padding);
                            }
                            _mItemList.Add(newItem);
                            float x = tViewItem1.CachedRectTransform.anchoredPosition3D.x - tViewItem1.CachedRectTransform.rect.width - tViewItem1.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(x, newItem.StartPosOffset, 0);
                            UpdateContentSize();
                            CheckIfNeedUpdataItemPos();

                            if (nIndex > _mCurReadyMaxItemIndex)
                            {
                                _mCurReadyMaxItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }

                if (rightPos0.x - _mViewPortRectLocalCorners[2].x < distanceForNew0)
                {
                    if (tViewItem0.ItemIndex < _mCurReadyMinItemIndex)
                    {
                        _mCurReadyMinItemIndex = tViewItem0.ItemIndex;
                        _mNeedCheckNextMinItem = true;
                    }
                    int nIndex = tViewItem0.ItemIndex - 1;
                    if (nIndex >= _mCurReadyMinItemIndex || _mNeedCheckNextMinItem)
                    {
                        LoopListViewItem2 newItem = GetNewItemByIndex(nIndex);
                        if (newItem == null)
                        {
                            _mCurReadyMinItemIndex = tViewItem0.ItemIndex;
                            _mNeedCheckNextMinItem = false;
                        }
                        else
                        {
                            if (mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.width, newItem.Padding);
                            }
                            _mItemList.Insert(0, newItem);
                            float x = tViewItem0.CachedRectTransform.anchoredPosition3D.x + newItem.CachedRectTransform.rect.width + newItem.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(x, newItem.StartPosOffset, 0);
                            UpdateContentSize();
                            CheckIfNeedUpdataItemPos();
                            if (nIndex < _mCurReadyMinItemIndex)
                            {
                                _mCurReadyMinItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }

            }

            return false;

        }






        float GetContentPanelSize()
        {
            if (mSupportScrollBar)
            {
                float tTotalSize = _mItemPosMgr.MTotalSize > 0 ? (_mItemPosMgr.MTotalSize - _mLastItemPadding) : 0;
                if(tTotalSize < 0)
                {
                    tTotalSize = 0;
                }
                return tTotalSize;
            }
            int count = _mItemList.Count;
            if (count == 0)
            {
                return 0;
            }
            if (count == 1)
            {
                return _mItemList[0].ItemSize;
            }
            if (count == 2)
            {
                return _mItemList[0].ItemSizeWithPadding + _mItemList[1].ItemSize;
            }
            float s = 0;
            for (int i = 0; i < count - 1; ++i)
            {
                s += _mItemList[i].ItemSizeWithPadding;
            }
            s += _mItemList[count - 1].ItemSize;
            return s;
        }


        void CheckIfNeedUpdataItemPos()
        {
            int count = _mItemList.Count;
            if (count == 0)
            {
                return;
            }
            if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                LoopListViewItem2 firstItem = _mItemList[0];
                LoopListViewItem2 lastItem = _mItemList[_mItemList.Count - 1];
                float viewMaxY = GetContentPanelSize();
                if (firstItem.TopY > 0 || (firstItem.ItemIndex == _mCurReadyMinItemIndex && firstItem.TopY != 0))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
                if ((-lastItem.BottomY) > viewMaxY || (lastItem.ItemIndex == _mCurReadyMaxItemIndex && (-lastItem.BottomY) != viewMaxY))
                {
                    UpdateAllShownItemsPos();
                    return;
                }

            }
            else if (mArrangeType == ListItemArrangeType.BottomToTop)
            {
                LoopListViewItem2 firstItem = _mItemList[0];
                LoopListViewItem2 lastItem = _mItemList[_mItemList.Count - 1];
                float viewMaxY = GetContentPanelSize();
                if (firstItem.BottomY < 0 || (firstItem.ItemIndex == _mCurReadyMinItemIndex && firstItem.BottomY != 0))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
                if (lastItem.TopY > viewMaxY || (lastItem.ItemIndex == _mCurReadyMaxItemIndex && lastItem.TopY != viewMaxY))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
            }
            else if (mArrangeType == ListItemArrangeType.LeftToRight)
            {
                LoopListViewItem2 firstItem = _mItemList[0];
                LoopListViewItem2 lastItem = _mItemList[_mItemList.Count - 1];
                float viewMaxX = GetContentPanelSize();
                if (firstItem.LeftX < 0 || (firstItem.ItemIndex == _mCurReadyMinItemIndex && firstItem.LeftX != 0))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
                if ((lastItem.RightX) > viewMaxX || (lastItem.ItemIndex == _mCurReadyMaxItemIndex && lastItem.RightX != viewMaxX))
                {
                    UpdateAllShownItemsPos();
                    return;
                }

            }
            else if (mArrangeType == ListItemArrangeType.RightToLeft)
            {
                LoopListViewItem2 firstItem = _mItemList[0];
                LoopListViewItem2 lastItem = _mItemList[_mItemList.Count - 1];
                float viewMaxX = GetContentPanelSize();
                if (firstItem.RightX > 0 || (firstItem.ItemIndex == _mCurReadyMinItemIndex && firstItem.RightX != 0))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
                if ((-lastItem.LeftX) > viewMaxX || (lastItem.ItemIndex == _mCurReadyMaxItemIndex && (-lastItem.LeftX) != viewMaxX))
                {
                    UpdateAllShownItemsPos();
                    return;
                }

            }

        }


        void UpdateAllShownItemsPos()
        {
            int count = _mItemList.Count;
            if (count == 0)
            {
                return;
            }

            _mAdjustedVec = (_mContainerTrans.anchoredPosition3D - _mLastFrameContainerPos) / Time.deltaTime;

            if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                float pos = 0;
                if (mSupportScrollBar)
                {
                    pos = -GetItemPos(_mItemList[0].ItemIndex);
                }
                float pos1 = _mItemList[0].CachedRectTransform.anchoredPosition3D.y;
                float d = pos - pos1;
                float curY = pos;
                for (int i = 0; i < count; ++i)
                {
                    LoopListViewItem2 item = _mItemList[i];
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(item.StartPosOffset, curY, 0);
                    curY = curY - item.CachedRectTransform.rect.height - item.Padding;
                }
                if(d != 0)
                {
                    Vector2 p = _mContainerTrans.anchoredPosition3D;
                    p.y = p.y - d;
                    _mContainerTrans.anchoredPosition3D = p;
                }
                
            }
            else if(mArrangeType == ListItemArrangeType.BottomToTop)
            {
                float pos = 0;
                if (mSupportScrollBar)
                {
                    pos = GetItemPos(_mItemList[0].ItemIndex);
                }
                float pos1 = _mItemList[0].CachedRectTransform.anchoredPosition3D.y;
                float d = pos - pos1;
                float curY = pos;
                for (int i = 0; i < count; ++i)
                {
                    LoopListViewItem2 item = _mItemList[i];
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(item.StartPosOffset, curY, 0);
                    curY = curY + item.CachedRectTransform.rect.height + item.Padding;
                }
                if(d != 0)
                {
                    Vector3 p = _mContainerTrans.anchoredPosition3D;
                    p.y = p.y - d;
                    _mContainerTrans.anchoredPosition3D = p;
                }
            }
            else if (mArrangeType == ListItemArrangeType.LeftToRight)
            {
                float pos = 0;
                if (mSupportScrollBar)
                {
                    pos = GetItemPos(_mItemList[0].ItemIndex);
                }
                float pos1 = _mItemList[0].CachedRectTransform.anchoredPosition3D.x;
                float d = pos - pos1;
                float curX = pos;
                for (int i = 0; i < count; ++i)
                {
                    LoopListViewItem2 item = _mItemList[i];
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(curX, item.StartPosOffset, 0);
                    curX = curX + item.CachedRectTransform.rect.width + item.Padding;
                }
                if (d != 0)
                {
                    Vector3 p = _mContainerTrans.anchoredPosition3D;
                    p.x = p.x - d;
                    _mContainerTrans.anchoredPosition3D = p;
                }

            }
            else if (mArrangeType == ListItemArrangeType.RightToLeft)
            {
                float pos = 0;
                if (mSupportScrollBar)
                {
                    pos = -GetItemPos(_mItemList[0].ItemIndex);
                }
                float pos1 = _mItemList[0].CachedRectTransform.anchoredPosition3D.x;
                float d = pos - pos1;
                float curX = pos;
                for (int i = 0; i < count; ++i)
                {
                    LoopListViewItem2 item = _mItemList[i];
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(curX, item.StartPosOffset, 0);
                    curX = curX - item.CachedRectTransform.rect.width - item.Padding;
                }
                if (d != 0)
                {
                    Vector3 p = _mContainerTrans.anchoredPosition3D;
                    p.x = p.x - d;
                    _mContainerTrans.anchoredPosition3D = p;
                }

            }
            if (_mIsDraging)
            {
                _mScrollRect.OnBeginDrag(_mPointerEventData);
                _mScrollRect.Rebuild(CanvasUpdate.PostLayout);
                _mScrollRect.velocity = _mAdjustedVec;
                _mNeedAdjustVec = true;
            }
        }
        void UpdateContentSize()
        {
            float size = GetContentPanelSize();
            if (_mIsVertList)
            {
                if(_mContainerTrans.rect.height != size)
                {
                    _mContainerTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
                }
            }
            else
            {
                if(_mContainerTrans.rect.width != size)
                {
                    _mContainerTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                }
            }
        }
    }

}
