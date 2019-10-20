using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SuperScrollView
{

    [System.Serializable]
    public class StaggeredGridItemPrefabConfData
    {
        public GameObject mItemPrefab = null;
        public float mPadding = 0;
        public int mInitCreateCount = 0;
    }


    public class StaggeredGridViewInitParam
    {
        // all the default values
        public float MDistanceForRecycle0 = 300; //mDistanceForRecycle0 should be larger than mDistanceForNew0
        public float MDistanceForNew0 = 200;
        public float MDistanceForRecycle1 = 300;//mDistanceForRecycle1 should be larger than mDistanceForNew1
        public float MDistanceForNew1 = 200;
        public float MItemDefaultWithPaddingSize = 20;//item's default size (with padding)

        public static StaggeredGridViewInitParam CopyDefaultInitParam()
        {
            return new StaggeredGridViewInitParam();
        }
    }

    public class ItemIndexData
    {
        public int MGroupIndex;
        public int MIndexInGroup;
    }


    /*
    For an vertical GridView, mColumnOrRowCount is the column count, 
    mItemWidthOrHeight is the item’s width, mPadding1 is the viewport left margin, 
    mPadding2 is the viewport right margin.
    For an horizontal GridView, mColumnOrRowCount is the row count, 
    mItemWidthOrHeight is the item’s height, mPadding1 is the viewport top margin, 
    mPadding2 is the viewport bottom margin. 
    If mCustomColumnOrRowOffsetArray is null, 
    that is to say, you do not set value for this parameter,
    then the GiriView would arrange all the columns or rows averaged.
    If mCustomColumnOrRowOffsetArray is not null, 
    the values of the array is the XOffset/YOffset of each column/row, 
    and mCustomColumnOrRowOffsetArray.length must be same to mColumnOrRowCount.
    */
    public class GridViewLayoutParam
    {
        public int MColumnOrRowCount = 0;//gridview column or row count
        public float MItemWidthOrHeight = 0; //gridview item width or height
        public float MPadding1 = 0;
        public float MPadding2 = 0;
        public float[] MCustomColumnOrRowOffsetArray = null;

        public bool CheckParam()
        {
            if (MColumnOrRowCount <= 0)
            {
                Debug.LogError("mColumnOrRowCount shoud be > 0");
                return false;
            }
            if (MItemWidthOrHeight <= 0)
            {
                Debug.LogError("mItemWidthOrHeight shoud be > 0");
                return false;
            }
            if (MCustomColumnOrRowOffsetArray != null && MCustomColumnOrRowOffsetArray.Length != MColumnOrRowCount)
            {
                Debug.LogError("mGroupOffsetArray.Length != mColumnOrRowCount");
                return false;
            }
            return true;
        }
    }


    public class LoopStaggeredGridView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        Dictionary<string, StaggeredGridItemPool> _mItemPoolDict = new Dictionary<string, StaggeredGridItemPool>();
        List<StaggeredGridItemPool> _mItemPoolList = new List<StaggeredGridItemPool>();
        [SerializeField]
        List<StaggeredGridItemPrefabConfData> mItemPrefabDataList = new List<StaggeredGridItemPrefabConfData>();

        [SerializeField]
        private ListItemArrangeType mArrangeType = ListItemArrangeType.TopToBottom;
        public ListItemArrangeType ArrangeType { get { return mArrangeType; } set { mArrangeType = value; } }

        RectTransform _mContainerTrans;
        ScrollRect _mScrollRect = null;
        int _mGroupCount = 0;

        List<StaggeredGridItemGroup> _mItemGroupList = new List<StaggeredGridItemGroup>();
        List<ItemIndexData> _mItemIndexDataList = new List<ItemIndexData>();

        RectTransform _mScrollRectTransform = null;
        RectTransform _mViewPortRectTransform = null;
        float _mItemDefaultWithPaddingSize = 20;
        int _mItemTotalCount = 0;
        bool _mIsVertList = false;
        System.Func<LoopStaggeredGridView, int,LoopStaggeredGridViewItem> _mOnGetItemByItemIndex;
        Vector3[] _mItemWorldCorners = new Vector3[4];
        Vector3[] _mViewPortRectLocalCorners = new Vector3[4];
        float _mDistanceForRecycle0 = 300;
        float _mDistanceForNew0 = 200;
        float _mDistanceForRecycle1 = 300;
        float _mDistanceForNew1 = 200;
        bool _mIsDraging = false;
        PointerEventData _mPointerEventData = null;
        public System.Action mOnBeginDragAction = null;
        public System.Action mOnDragingAction = null;
        public System.Action mOnEndDragAction = null;
        Vector3 _mLastFrameContainerPos = Vector3.zero;
        bool _mListViewInited = false;
        int _mListUpdateCheckFrameCount = 0;
        GridViewLayoutParam _mLayoutParam = null;

        public List<StaggeredGridItemPrefabConfData> ItemPrefabDataList
        {
            get
            {
                return mItemPrefabDataList;
            }
        }

        public int ListUpdateCheckFrameCount
        {
            get
            {
                return _mListUpdateCheckFrameCount;
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

        public GridViewLayoutParam LayoutParam
        {
            get { return _mLayoutParam; }
        }

        public bool IsInited
        {
            get { return _mListViewInited; }
        }

        public StaggeredGridItemGroup GetItemGroupByIndex(int index)
        {
            int count = _mItemGroupList.Count;
            if(index < 0 || index >= count)
            {
                return null;
            }
            return _mItemGroupList[index];
        }


        public StaggeredGridItemPrefabConfData GetItemPrefabConfData(string prefabName)
        {
            foreach (StaggeredGridItemPrefabConfData data in mItemPrefabDataList)
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

        /*
        InitListView method is to initiate the LoopStaggeredGridView component. There are 4 parameters:
        itemTotalCount: the total item count in the scrollview, this parameter should be >=0.
        layoutParam: this class is very sample, and you need new a GridViewLayoutParam instance and set the values you want.
        onGetItemByItemIndex: when an item is getting in the scrollrect viewport, this Action will be called with the item’ index as a parameter, to let you create the item and update its content.
        LoopStaggeredGridViewItem is the return value of onGetItemByItemIndex
        Every created item has a LoopStaggeredGridViewItem component auto attached
        */
        public void InitListView(int itemTotalCount, GridViewLayoutParam layoutParam,
            System.Func<LoopStaggeredGridView, int, LoopStaggeredGridViewItem> onGetItemByItemIndex,
            StaggeredGridViewInitParam initParam = null)
        {
            _mLayoutParam = layoutParam;
            if(_mLayoutParam == null)
            {
                Debug.LogError("layoutParam can not be null!");
                return;
            }
            if (_mLayoutParam.CheckParam() == false)
            {
                return;
            }
            if (initParam != null)
            {
                _mDistanceForRecycle0 = initParam.MDistanceForRecycle0;
                _mDistanceForNew0 = initParam.MDistanceForNew0;
                _mDistanceForRecycle1 = initParam.MDistanceForRecycle1;
                _mDistanceForNew1 = initParam.MDistanceForNew1;
                _mItemDefaultWithPaddingSize = initParam.MItemDefaultWithPaddingSize;
            }
            _mScrollRect = gameObject.GetComponent<ScrollRect>();
            if (_mScrollRect == null)
            {
                Debug.LogError("LoopStaggeredGridView Init Failed! ScrollRect component not found!");
                return;
            }
            if (_mDistanceForRecycle0 <= _mDistanceForNew0)
            {
                Debug.LogError("mDistanceForRecycle0 should be bigger than mDistanceForNew0");
            }
            if (_mDistanceForRecycle1 <= _mDistanceForNew1)
            {
                Debug.LogError("mDistanceForRecycle1 should be bigger than mDistanceForNew1");
            }
            _mScrollRectTransform = _mScrollRect.GetComponent<RectTransform>();
            _mContainerTrans = _mScrollRect.content;
            _mViewPortRectTransform = _mScrollRect.viewport;
            _mGroupCount = _mLayoutParam.MColumnOrRowCount;
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
            AdjustPivot(_mViewPortRectTransform);
            AdjustAnchor(_mContainerTrans);
            AdjustContainerPivot(_mContainerTrans);
            InitItemPool();
            _mOnGetItemByItemIndex = onGetItemByItemIndex;
            if (_mListViewInited == true)
            {
                Debug.LogError("LoopStaggeredGridView.InitListView method can be called only once.");
            }
            _mListViewInited = true;
            _mViewPortRectTransform.GetLocalCorners(_mViewPortRectLocalCorners);
            _mContainerTrans.anchoredPosition3D = Vector3.zero;
            _mItemTotalCount = itemTotalCount;
            UpdateLayoutParamAutoValue();
            _mItemGroupList.Clear();
            for (int i = 0;i<_mGroupCount;++i)
            {
                StaggeredGridItemGroup group = new StaggeredGridItemGroup();
                group.Init(this, _mItemTotalCount, i, GetNewItemByGroupAndIndex);
                _mItemGroupList.Add(group);
            }
            UpdateContentSize();
        }

        //reset the layout param, such as column count, item width/height,padding size
        public void ResetGridViewLayoutParam(int itemTotalCount, GridViewLayoutParam layoutParam)
        {
            if(_mListViewInited == false)
            {
                Debug.LogError("ResetLayoutParam can not use before LoopStaggeredGridView.InitListView are called!");
                return;
            }
            _mScrollRect.StopMovement();
            SetListItemCount(0,true);
            RecycleAllItem();
            ClearAllTmpRecycledItem();
            _mLayoutParam = layoutParam;
            if (_mLayoutParam == null)
            {
                Debug.LogError("layoutParam can not be null!");
                return;
            }
            if (_mLayoutParam.CheckParam() == false)
            {
                return;
            }
            _mGroupCount = _mLayoutParam.MColumnOrRowCount;
            _mViewPortRectTransform.GetLocalCorners(_mViewPortRectLocalCorners);
            _mContainerTrans.anchoredPosition3D = Vector3.zero;
            _mItemTotalCount = itemTotalCount;
            UpdateLayoutParamAutoValue();
            _mItemGroupList.Clear();
            for (int i = 0; i < _mGroupCount; ++i)
            {
                StaggeredGridItemGroup group = new StaggeredGridItemGroup();
                group.Init(this, _mItemTotalCount, i, GetNewItemByGroupAndIndex);
                _mItemGroupList.Add(group);
            }
            UpdateContentSize();
        }

        void UpdateLayoutParamAutoValue()
        {
            if (_mLayoutParam.MCustomColumnOrRowOffsetArray == null)
            {
                _mLayoutParam.MCustomColumnOrRowOffsetArray = new float[_mGroupCount];
                float itemTotalSize = _mLayoutParam.MItemWidthOrHeight * _mGroupCount;
                float itemPadding = 0;
                if (IsVertList)
                {
                    itemPadding = (ViewPortWidth - _mLayoutParam.MPadding1 - _mLayoutParam.MPadding2 - itemTotalSize) / (_mGroupCount - 1);
                }
                else
                {
                    itemPadding = (ViewPortHeight - _mLayoutParam.MPadding1 - _mLayoutParam.MPadding2 - itemTotalSize) / (_mGroupCount - 1);
                }
                float cur = _mLayoutParam.MPadding1;
                for (int i = 0; i < _mGroupCount; ++i)
                {
                    if (IsVertList)
                    {
                        _mLayoutParam.MCustomColumnOrRowOffsetArray[i] = cur;
                    }
                    else
                    {
                        _mLayoutParam.MCustomColumnOrRowOffsetArray[i] = -cur;
                    }
                    cur = cur + _mLayoutParam.MItemWidthOrHeight + itemPadding;
                }
            }
        }


        //This method is used to get a new item, and the new item is a clone from the prefab named itemPrefabName.
        //This method is usually used in onGetItemByItemIndex.
        public LoopStaggeredGridViewItem NewListViewItem(string itemPrefabName)
        {
            StaggeredGridItemPool pool = null;
            if (_mItemPoolDict.TryGetValue(itemPrefabName, out pool) == false)
            {
                return null;
            }
            LoopStaggeredGridViewItem item = pool.GetItem();
            RectTransform rf = item.GetComponent<RectTransform>();
            rf.SetParent(_mContainerTrans);
            rf.localScale = Vector3.one;
            rf.anchoredPosition3D = Vector3.zero;
            rf.localEulerAngles = Vector3.zero;
            item.ParentListView = this;
            return item;
        }


        //This method may use to set the item total count of the GridView at runtime.
        //If resetPos is set false, then the scrollrect's content position will not changed after this method finished.
        public void SetListItemCount(int itemCount, bool resetPos = true)
        {
            if (itemCount == _mItemTotalCount)
            {
                return;
            }
            int groupCount = _mItemGroupList.Count;
            _mItemTotalCount = itemCount;
            for (int i = 0; i < groupCount; ++i)
            {
                _mItemGroupList[i].SetListItemCount(_mItemTotalCount);
            }
            UpdateContentSize();
            if (_mItemTotalCount == 0)
            {
                _mItemIndexDataList.Clear();
                ClearAllTmpRecycledItem();
                return;
            }
            int count = _mItemIndexDataList.Count;

            if (count > _mItemTotalCount)
            {
                _mItemIndexDataList.RemoveRange(_mItemTotalCount, count - _mItemTotalCount);
            }
            if (resetPos)
            {
                MovePanelToItemIndex(0, 0);
                return;
            }
            if (count > _mItemTotalCount)
            {
                MovePanelToItemIndex(_mItemTotalCount - 1, 0); ;
            }
        }


        //This method will move the scrollrect content’s position
        //to ( the positon of itemIndex-th item + offset )
        public void MovePanelToItemIndex(int itemIndex, float offset)
        {
            _mScrollRect.StopMovement();
            if (_mItemTotalCount == 0 || itemIndex < 0)
            {
                return;
            }
            CheckAllGroupIfNeedUpdateItemPos();
            UpdateContentSize();
            float viewPortSize = ViewPortSize;
            float contentSize = GetContentSize();
            if (contentSize <= viewPortSize)
            {
                if (IsVertList)
                {
                    SetAnchoredPositionY(_mContainerTrans, 0f);
                }
                else
                {
                    SetAnchoredPositionX(_mContainerTrans, 0f);
                }
                return;
            }
            if (itemIndex >= _mItemTotalCount)
            {
                itemIndex = _mItemTotalCount - 1;
            }
            float itemAbsPos = GetItemAbsPosByItemIndex(itemIndex);
            if (itemAbsPos < 0)
            {
                return;
            }
            if (IsVertList)
            {
                float sign = (mArrangeType == ListItemArrangeType.TopToBottom) ? 1 : -1;
                float newYAbs = itemAbsPos + offset;
                if (newYAbs < 0)
                {
                    newYAbs = 0;
                }
                if (contentSize - newYAbs >= viewPortSize)
                {
                    SetAnchoredPositionY(_mContainerTrans, sign * newYAbs);
                }
                else
                {
                    SetAnchoredPositionY(_mContainerTrans, sign * (contentSize - viewPortSize));
                    UpdateListView(viewPortSize + 100, viewPortSize + 100, viewPortSize, viewPortSize);
                    ClearAllTmpRecycledItem();
                    UpdateContentSize();
                    contentSize = GetContentSize();
                    if (contentSize - newYAbs >= viewPortSize)
                    {
                        SetAnchoredPositionY(_mContainerTrans, sign * newYAbs);
                    }
                    else
                    {
                        SetAnchoredPositionY(_mContainerTrans, sign * (contentSize - viewPortSize));
                    }
                }

            }
            else
            {
                float sign = (mArrangeType == ListItemArrangeType.RightToLeft) ? 1 : -1;
                float newXAbs = itemAbsPos + offset;
                if (newXAbs < 0)
                {
                    newXAbs = 0;
                }
                if (contentSize - newXAbs >= viewPortSize)
                {
                    SetAnchoredPositionX(_mContainerTrans, sign * newXAbs);
                }
                else
                {
                    SetAnchoredPositionX(_mContainerTrans, sign * (contentSize - viewPortSize));
                    UpdateListView(viewPortSize + 100, viewPortSize + 100, viewPortSize, viewPortSize);
                    ClearAllTmpRecycledItem();
                    UpdateContentSize();
                    contentSize = GetContentSize();
                    if (contentSize - newXAbs >= viewPortSize)
                    {
                        SetAnchoredPositionX(_mContainerTrans, sign * newXAbs);
                    }
                    else
                    {
                        SetAnchoredPositionX(_mContainerTrans, sign * (contentSize - viewPortSize));
                    }
                }
            }

        }


        //To get the visible item by itemIndex. If the item is not visible, then this method return null.
        public LoopStaggeredGridViewItem GetShownItemByItemIndex(int itemIndex)
        {
            ItemIndexData indexData = GetItemIndexData(itemIndex);
            if (indexData == null)
            {
                return null;
            }
            StaggeredGridItemGroup group = GetItemGroupByIndex(indexData.MGroupIndex);
            return group.GetShownItemByIndexInGroup(indexData.MIndexInGroup);
        }

        //update all visible items.
        public void RefreshAllShownItem()
        {
            int count = _mItemGroupList.Count;
            for (int i = 0; i < count; ++i)
            {
                _mItemGroupList[i].RefreshAllShownItem();
            }
        }


        /*
      For a vertical scrollrect, when a visible item’s height changed at runtime, 
      then this method should be called to let the LoopStaggeredGridView component reposition all visible items’ position of the same group (that is the same column / row).
      For a horizontal scrollrect, when a visible item’s width changed at runtime,
      then this method should be called to let the LoopStaggeredGridView component reposition all visible items’ position of the same group (that is the same column / row).
      */
        public void OnItemSizeChanged(int itemIndex)
        {
            ItemIndexData indexData = GetItemIndexData(itemIndex);
            if (indexData == null)
            {
                return;
            }
            StaggeredGridItemGroup group = GetItemGroupByIndex(indexData.MGroupIndex);
            group.OnItemSizeChanged(indexData.MIndexInGroup);
        }


        /*
        To update a item by itemIndex.if the itemIndex-th item is not visible, then this method will do nothing.
        Otherwise this method will first call onGetItemByIndex(itemIndex) to get a updated item and then reposition all visible items'position. 
        */
        public void RefreshItemByItemIndex(int itemIndex)
        {
            ItemIndexData indexData = GetItemIndexData(itemIndex);
            if (indexData == null)
            {
                return;
            }
            StaggeredGridItemGroup group = GetItemGroupByIndex(indexData.MGroupIndex);
            group.RefreshItemByIndexInGroup(indexData.MIndexInGroup);
        }


        public void ResetListView(bool resetPos = true)
        {
            _mViewPortRectTransform.GetLocalCorners(_mViewPortRectLocalCorners);
            if (resetPos)
            {
                _mContainerTrans.anchoredPosition3D = Vector3.zero;
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

        public void RecycleAllItem()
        {
            int groupCount = _mItemGroupList.Count;
            for (int i = 0; i < groupCount; ++i)
            {
                _mItemGroupList[i].RecycleAllItem();
            }
        }


        public void RecycleItemTmp(LoopStaggeredGridViewItem item)
        {
            if (item == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(item.ItemPrefabName))
            {
                return;
            }
            StaggeredGridItemPool pool = null;
            if (_mItemPoolDict.TryGetValue(item.ItemPrefabName, out pool) == false)
            {
                return;
            }
            pool.RecycleItem(item);

        }


        public void ClearAllTmpRecycledItem()
        {
            int count = _mItemPoolList.Count;
            for (int i = 0; i < count; ++i)
            {
                _mItemPoolList[i].ClearTmpRecycledItem();
            }
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
            foreach (StaggeredGridItemPrefabConfData data in mItemPrefabDataList)
            {
                if (data.mItemPrefab == null)
                {
                    Debug.LogError("A item prefab is null ");
                    continue;
                }
                string prefabName = data.mItemPrefab.name;
                if (_mItemPoolDict.ContainsKey(prefabName))
                {
                    Debug.LogError("A item prefab with name " + prefabName + " has existed!");
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
                LoopStaggeredGridViewItem tItem = data.mItemPrefab.GetComponent<LoopStaggeredGridViewItem>();
                if (tItem == null)
                {
                    data.mItemPrefab.AddComponent<LoopStaggeredGridViewItem>();
                }
                StaggeredGridItemPool pool = new StaggeredGridItemPool();
                pool.Init(data.mItemPrefab, data.mPadding, data.mInitCreateCount, _mContainerTrans);
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
            if (mOnBeginDragAction != null)
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


        public int CurMaxCreatedItemIndexCount
        {
            get { return _mItemIndexDataList.Count; }
        }

       
        void SetAnchoredPositionX(RectTransform rtf,float x)
        {
            Vector3 pos = rtf.anchoredPosition3D;
            pos.x = x;
            rtf.anchoredPosition3D = pos;
        }

        void SetAnchoredPositionY(RectTransform rtf, float y)
        {
            Vector3 pos = rtf.anchoredPosition3D;
            pos.y = y;
            rtf.anchoredPosition3D = pos;
        }

        public ItemIndexData GetItemIndexData(int itemIndex)
        {
            int count = _mItemIndexDataList.Count;
            if(itemIndex < 0 || itemIndex >= count)
            {
                return null;
            }
            return _mItemIndexDataList[itemIndex];
        }




        public void UpdateAllGroupShownItemsPos()
        {
            int groupCount = _mItemGroupList.Count;
            for (int i = 0; i < groupCount; ++i)
            {
                _mItemGroupList[i].UpdateAllShownItemsPos();
            }
        }

        void CheckAllGroupIfNeedUpdateItemPos()
        {
            int groupCount = _mItemGroupList.Count;
            for (int i = 0; i < groupCount; ++i)
            {
                _mItemGroupList[i].CheckIfNeedUpdateItemPos();
            }
        }


        public float GetItemAbsPosByItemIndex(int itemIndex)
        {
            if(itemIndex < 0 || itemIndex >= _mItemIndexDataList.Count)
            {
                return -1;
            }
            ItemIndexData tData = _mItemIndexDataList[itemIndex];
            return _mItemGroupList[tData.MGroupIndex].GetItemPos(tData.MIndexInGroup);
        }

        public LoopStaggeredGridViewItem GetNewItemByGroupAndIndex(int groupIndex,int indexInGroup)
        {
            if (indexInGroup < 0)
            {
                return null;
            }
            if(_mItemTotalCount == 0)
            {
                return null;
            }
            LoopStaggeredGridViewItem newItem = null;
            int index = 0;
            List<int> mItemIndexMap = _mItemGroupList[groupIndex].ItemIndexMap;
            int count = mItemIndexMap.Count;
            if (count > indexInGroup)
            {
                index = mItemIndexMap[indexInGroup];
                newItem = _mOnGetItemByItemIndex(this, index);
                if (newItem == null)
                {
                    return null;
                }
                newItem.StartPosOffset = _mLayoutParam.MCustomColumnOrRowOffsetArray[groupIndex];
                newItem.ItemIndexInGroup = indexInGroup;
                newItem.ItemIndex = index;
                newItem.ItemCreatedCheckFrameCount = _mListUpdateCheckFrameCount;
                return newItem;
            }
            if(count != indexInGroup)
            {
                return null;
            }
            int curMaxCreatedItemIndexCount = _mItemIndexDataList.Count;
            if (curMaxCreatedItemIndexCount >= _mItemTotalCount)
            {
                return null;
            }
            index = curMaxCreatedItemIndexCount;
            newItem = _mOnGetItemByItemIndex(this, index);
            if (newItem == null)
            {
                return null;
            }
            mItemIndexMap.Add(index);
            ItemIndexData indexData = new ItemIndexData();
            indexData.MGroupIndex = groupIndex;
            indexData.MIndexInGroup = indexInGroup;
            _mItemIndexDataList.Add(indexData);
            newItem.StartPosOffset = _mLayoutParam.MCustomColumnOrRowOffsetArray[groupIndex];
            newItem.ItemIndexInGroup = indexInGroup;
            newItem.ItemIndex = index;
            newItem.ItemCreatedCheckFrameCount = _mListUpdateCheckFrameCount;
            return newItem;
        }

        int GetCurShouldAddNewItemGroupIndex()
        {
            float v = float.MaxValue;
            int groupCount = _mItemGroupList.Count;
            int groupIndex = 0;
            for (int i = 0; i < groupCount; ++i)
            {
                float size = _mItemGroupList[i].GetShownItemPosMaxValue();
                if(size < v)
                {
                    v = size;
                    groupIndex = i;
                }
            }
            return groupIndex;
        }

        public void UpdateListViewWithDefault()
        {
            UpdateListView(_mDistanceForRecycle0, _mDistanceForRecycle1, _mDistanceForNew0, _mDistanceForNew1);
            UpdateContentSize();
        }


        void Update()
        {
            if (_mListViewInited == false)
            {
                return;
            }
            UpdateListViewWithDefault();
            ClearAllTmpRecycledItem();
            _mLastFrameContainerPos = _mContainerTrans.anchoredPosition3D;
        }




        public void UpdateListView(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0, float distanceForNew1)
        {
            _mListUpdateCheckFrameCount++;
            bool needContinueCheck = true;
            int checkCount = 0;
            int maxCount = 9999;
            int groupCount = _mItemGroupList.Count;
            for (int i = 0; i < groupCount; ++i)
            {
                _mItemGroupList[i].UpdateListViewPart1(distanceForRecycle0, distanceForRecycle1, distanceForNew0, distanceForNew1);
            }
            while (needContinueCheck)
            {
                checkCount++;
                if (checkCount >= maxCount)
                {
                    Debug.LogError("UpdateListView while loop " + checkCount + " times! something is wrong!");
                    break;
                }
                int groupIndex = GetCurShouldAddNewItemGroupIndex();
                needContinueCheck = _mItemGroupList[groupIndex].UpdateListViewPart2(distanceForRecycle0, distanceForRecycle1, distanceForNew0, distanceForNew1);
            }
               
        }

        public float GetContentSize()
        {
            if (_mIsVertList)
            {
                return _mContainerTrans.rect.height;
            }
            else
            {
                return _mContainerTrans.rect.width;
            }
        }
        public void UpdateContentSize()
        {
            int groupCount = _mItemGroupList.Count;
            float size = 0;
            for (int i = 0; i < groupCount; ++i)
            {
                float s = _mItemGroupList[i].GetContentPanelSize();
                if (s > size)
                {
                    size = s;
                }
            }
            if (_mIsVertList)
            {
                if (_mContainerTrans.rect.height != size)
                {
                    _mContainerTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
                }
            }
            else
            {
                if (_mContainerTrans.rect.width != size)
                {
                    _mContainerTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                }
            }
        }
    }

}
