using System.Collections.Generic;
using _00_Asset._01_Asset_SuperScrollView.Scripts.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _00_Asset._01_Asset_SuperScrollView.Scripts.GridView
{
   
    [System.Serializable]
    public class GridViewItemPrefabConfData
    {
        public GameObject mItemPrefab = null;
        public int mInitCreateCount = 0;
    }


    public class LoopGridViewInitParam
    {
        // all the default values
        public float MSmoothDumpRate = 0.3f;
        public float MSnapFinishThreshold = 0.01f;
        public float MSnapVecThreshold = 145;

        public static LoopGridViewInitParam CopyDefaultInitParam()
        {
            return new LoopGridViewInitParam();
        }
    }


    public class LoopGridViewSettingParam
    {
        public object MItemSize = null;
        public object MPadding = null;
        public object MItemPadding = null;
        public object MGridFixedType = null;
        public object MFixedRowOrColumnCount = null;
    }


    public class LoopGridView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        class SnapData
        {
            public SnapStatus MSnapStatus = SnapStatus.NoTargetSet;
            public RowColumnPair MSnapTarget;
            public Vector2 MSnapNeedMoveDir;
            public float MTargetSnapVal = 0;
            public float MCurSnapVal = 0;
            public bool MIsForceSnapTo = false;
            public void Clear()
            {
                MSnapStatus = SnapStatus.NoTargetSet;
                MIsForceSnapTo = false;
            }
        }
        class ItemRangeData
        {
            public int MMaxRow;
            public int MMinRow;
            public int MMaxColumn;
            public int MMinColumn;
            public Vector2 MCheckedPosition;
        }

        Dictionary<string, GridItemPool> _mItemPoolDict = new Dictionary<string, GridItemPool>();
        List<GridItemPool> _mItemPoolList = new List<GridItemPool>();
        [SerializeField]
        List<GridViewItemPrefabConfData> mItemPrefabDataList = new List<GridViewItemPrefabConfData>();

        [SerializeField]
        private GridItemArrangeType mArrangeType = GridItemArrangeType.TopLeftToBottomRight;
        public GridItemArrangeType ArrangeType { get { return mArrangeType; } set { mArrangeType = value; } }
        RectTransform _mContainerTrans;
        ScrollRect _mScrollRect = null;
        RectTransform _mScrollRectTransform = null;
        RectTransform _mViewPortRectTransform = null;
        int _mItemTotalCount = 0;
        [SerializeField]
        int mFixedRowOrColumnCount = 0;
        [SerializeField]
        RectOffset mPadding = new RectOffset();
        [SerializeField]
        Vector2 mItemPadding = Vector2.zero;
        [SerializeField]
        Vector2 mItemSize = Vector2.zero;
        [SerializeField]
        Vector2 mItemRecycleDistance = new Vector2(50,50);
        Vector2 _mItemSizeWithPadding = Vector2.zero;
        Vector2 _mStartPadding;
        Vector2 _mEndPadding;
        System.Func<LoopGridView,int,int,int, LoopGridViewItem> _mOnGetItemByRowColumn;
        List<GridItemGroup> _mItemGroupObjPool = new List<GridItemGroup>();

        //if GridFixedType is GridFixedType.ColumnCountFixed, then the GridItemGroup is one row of the GridView
        //if GridFixedType is GridFixedType.RowCountFixed, then the GridItemGroup is one column of the GridView
        //so mItemGroupList is current all shown rows or columns
        List<GridItemGroup> _mItemGroupList = new List<GridItemGroup>();

        bool _mIsDraging = false;
        int _mRowCount = 0;
        int _mColumnCount = 0;
        public System.Action<PointerEventData> mOnBeginDragAction = null;
        public System.Action<PointerEventData> mOnDragingAction = null;
        public System.Action<PointerEventData> mOnEndDragAction = null;
        float _mSmoothDumpVel = 0;
        float _mSmoothDumpRate = 0.3f;
        float _mSnapFinishThreshold = 0.1f;
        float _mSnapVecThreshold = 145;
        [SerializeField]
        bool mItemSnapEnable = false;
        [SerializeField]
        GridFixedType mGridFixedType = GridFixedType.ColumnCountFixed;
        public System.Action<LoopGridView, LoopGridViewItem> mOnSnapItemFinished = null;
        //in this callback, use CurSnapNearestItemRowColumn to get cur snaped item row column.
        public System.Action<LoopGridView> mOnSnapNearestChanged = null;
        int _mLeftSnapUpdateExtraCount = 1;
        [SerializeField]
        Vector2 mViewPortSnapPivot = Vector2.zero;
        [SerializeField]
        Vector2 mItemSnapPivot = Vector2.zero;
        SnapData _mCurSnapData = new SnapData();
        Vector3 _mLastSnapCheckPos = Vector3.zero;
        bool _mListViewInited = false;
        int _mListUpdateCheckFrameCount = 0;
        ItemRangeData _mCurFrameItemRangeData = new ItemRangeData();
        int _mNeedCheckContentPosLeftCount = 1;
        ClickEventListener _mScrollBarClickEventListener1 = null;
        ClickEventListener _mScrollBarClickEventListener2 = null;

        RowColumnPair _mCurSnapNearestItemRowColumn;

        public List<GridViewItemPrefabConfData> ItemPrefabDataList
        {
            get
            {
                return mItemPrefabDataList;
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

        public float ViewPortWidth
        {
            get { return _mViewPortRectTransform.rect.width; }
        }

        public float ViewPortHeight
        {
            get { return _mViewPortRectTransform.rect.height; }
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

        public Vector2 ItemSize
        {
            get
            {
                return mItemSize;
            }
            set
            {
                SetItemSize(value);
            }
        }

        public Vector2 ItemPadding
        {
            get
            {
                return mItemPadding;
            }
            set
            {
                SetItemPadding(value);
            }
        }

        public Vector2 ItemSizeWithPadding
        {
            get
            {
                return _mItemSizeWithPadding;
            }
        }
        public RectOffset Padding
        {
            get
            {
                return mPadding;
            }
            set
            {
                SetPadding(value);
            }
        }


        public GridViewItemPrefabConfData GetItemPrefabConfData(string prefabName)
        {
            foreach (GridViewItemPrefabConfData data in mItemPrefabDataList)
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
        LoopGridView method is to initiate the LoopGridView component. There are 4 parameters:
        itemTotalCount: the total item count in the GridView, this parameter must be set a value >=0 , then the ItemIndex can be from 0 to itemTotalCount -1.
        onGetItemByRowColumn: when a item is getting in the ScrollRect viewport, and this Action will be called with the item' index and the row and column index as the parameters, to let you create the item and update its content.
        settingParam: You can use this parameter to override the values in the Inspector Setting
        */
        public void InitGridView(int itemTotalCount, 
            System.Func<LoopGridView,int,int,int, LoopGridViewItem> onGetItemByRowColumn, 
            LoopGridViewSettingParam settingParam = null,
            LoopGridViewInitParam initParam = null)
        {
            if (_mListViewInited == true)
            {
                Debug.LogError("LoopGridView.InitListView method can be called only once.");
                return;
            }
            _mListViewInited = true;
            if (itemTotalCount < 0)
            {
                Debug.LogError("itemTotalCount is  < 0");
                itemTotalCount = 0;
            }
            if(settingParam != null)
            {
                UpdateFromSettingParam(settingParam);
            }
            if(initParam != null)
            {
                _mSmoothDumpRate = initParam.MSmoothDumpRate;
                _mSnapFinishThreshold = initParam.MSnapFinishThreshold;
                _mSnapVecThreshold = initParam.MSnapVecThreshold;
            }
            _mScrollRect = gameObject.GetComponent<ScrollRect>();
            if (_mScrollRect == null)
            {
                Debug.LogError("ListView Init Failed! ScrollRect component not found!");
                return;
            }
            _mCurSnapData.Clear();
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
            SetScrollbarListener();
            AdjustViewPortPivot();
            AdjustContainerAnchorAndPivot();
            InitItemPool();
            _mOnGetItemByRowColumn = onGetItemByRowColumn;
            _mNeedCheckContentPosLeftCount = 4;
            _mCurSnapData.Clear();
            _mItemTotalCount = itemTotalCount;
            UpdateAllGridSetting();
        }


        /*
        This method may use to set the item total count of the GridView at runtime. 
        this parameter must be set a value >=0 , and the ItemIndex can be from 0 to itemCount -1.  
        If resetPos is set false, then the ScrollRect’s content position will not changed after this method finished.
        */
        public void SetListItemCount(int itemCount, bool resetPos = true)
        {
            if(itemCount < 0)
            {
                return;
            }
            if(itemCount == _mItemTotalCount)
            {
                return;
            }
            _mCurSnapData.Clear();
            _mItemTotalCount = itemCount;
            UpdateColumnRowCount();
            UpdateContentSize();
            ForceToCheckContentPos();
            if (_mItemTotalCount == 0)
            {
                RecycleAllItem();
                ClearAllTmpRecycledItem();
                return;
            }
            VaildAndSetContainerPos();
            UpdateGridViewContent();
            ClearAllTmpRecycledItem();
            if (resetPos)
            {
                MovePanelToItemByRowColumn(0,0);
                return;
            }
        }

       //fetch or create a new item form the item pool.
        public LoopGridViewItem NewListViewItem(string itemPrefabName)
        {
            GridItemPool pool = null;
            if (_mItemPoolDict.TryGetValue(itemPrefabName, out pool) == false)
            {
                return null;
            }
            LoopGridViewItem item = pool.GetItem();
            RectTransform rf = item.GetComponent<RectTransform>();
            rf.SetParent(_mContainerTrans);
            rf.localScale = Vector3.one;
            rf.anchoredPosition3D = Vector3.zero;
            rf.localEulerAngles = Vector3.zero;
            item.ParentGridView = this;
            return item;
        }


        /*
        To update a item by itemIndex.if the itemIndex-th item is not visible, then this method will do nothing.
        Otherwise this method will call RefreshItemByRowColumn to do real work.
        */
        public void RefreshItemByItemIndex(int itemIndex)
        {
            if(itemIndex < 0 || itemIndex >= ItemTotalCount)
            {
                return;
            }
            int count = _mItemGroupList.Count;
            if (count == 0)
            {
                return;
            }
            RowColumnPair val = GetRowColumnByItemIndex(itemIndex);
            RefreshItemByRowColumn(val.MRow, val.MColumn);
        }


        /*
        To update a item by (row,column).if the item is not visible, then this method will do nothing.
        Otherwise this method will call mOnGetItemByRowColumn(row,column) to get a new updated item. 
        */
        public void RefreshItemByRowColumn(int row,int column)
        {
            int count = _mItemGroupList.Count;
            if (count == 0)
            {
                return;
            }
            if (mGridFixedType == GridFixedType.ColumnCountFixed)
            {
                GridItemGroup group = GetShownGroup(row);
                if (group == null)
                {
                    return;
                }
                LoopGridViewItem curItem = group.GetItemByColumn(column);
                if(curItem == null)
                {
                    return;
                }
                LoopGridViewItem newItem = GetNewItemByRowColumn(row, column);
                if (newItem == null)
                {
                    return;
                }
                Vector3 pos = curItem.CachedRectTransform.anchoredPosition3D;
                group.ReplaceItem(curItem, newItem);
                RecycleItemTmp(curItem);
                newItem.CachedRectTransform.anchoredPosition3D = pos;
                ClearAllTmpRecycledItem();
            }
            else
            {
                GridItemGroup group = GetShownGroup(column);
                if (group == null)
                {
                    return;
                }
                LoopGridViewItem curItem = group.GetItemByRow(row);
                if (curItem == null)
                {
                    return;
                }
                LoopGridViewItem newItem = GetNewItemByRowColumn(row, column);
                if (newItem == null)
                {
                    return;
                }
                Vector3 pos = curItem.CachedRectTransform.anchoredPosition3D;
                group.ReplaceItem(curItem, newItem);
                RecycleItemTmp(curItem);
                newItem.CachedRectTransform.anchoredPosition3D = pos;
                ClearAllTmpRecycledItem();
            }
        }

        //Clear current snap target and then the GridView will auto snap to the CurSnapNearestItem.
        public void ClearSnapData()
        {
            _mCurSnapData.Clear();
        }

        //set cur snap target
        public void SetSnapTargetItemRowColumn(int row, int column)
        {
            if(row < 0)
            {
                row = 0;
            }
            if(column < 0)
            {
                column = 0;
            }
            _mCurSnapData.MSnapTarget.MRow = row;
            _mCurSnapData.MSnapTarget.MColumn = column;
            _mCurSnapData.MSnapStatus = SnapStatus.TargetHasSet;
            _mCurSnapData.MIsForceSnapTo = true;
        }

        //Get the nearest item row and column with the viewport snap point.
        public RowColumnPair CurSnapNearestItemRowColumn
        {
            get { return _mCurSnapNearestItemRowColumn; }
        }


        //force to update the mCurSnapNearestItemRowColumn value
        public void ForceSnapUpdateCheck()
        {
            if (_mLeftSnapUpdateExtraCount <= 0)
            {
                _mLeftSnapUpdateExtraCount = 1;
            }
        }

        //force to refresh the mCurFrameItemRangeData that what items should be shown in viewport.
        public void ForceToCheckContentPos()
        {
            if (_mNeedCheckContentPosLeftCount <= 0)
            {
                _mNeedCheckContentPosLeftCount = 1;
            }
        }

        /*
        This method will move the panel's position to ( the position of itemIndex'th item + offset ).
        */
        public void MovePanelToItemByIndex(int itemIndex, float offsetX = 0, float offsetY = 0)
        {
            if(ItemTotalCount == 0)
            {
                return;
            }
            if(itemIndex >= ItemTotalCount)
            {
                itemIndex = ItemTotalCount - 1;
            }
            if (itemIndex < 0)
            {
                itemIndex = 0;
            }
            RowColumnPair val = GetRowColumnByItemIndex(itemIndex);
            MovePanelToItemByRowColumn(val.MRow, val.MColumn, offsetX, offsetY);
        }

        /*
        This method will move the panel's position to ( the position of (row,column) item + offset ).
        */
        public void MovePanelToItemByRowColumn(int row,int column, float offsetX = 0,float offsetY = 0)
        {
            _mScrollRect.StopMovement();
            _mCurSnapData.Clear();
            if (_mItemTotalCount == 0)
            {
                return;
            }
            Vector2 itemPos = GetItemPos(row, column);
            Vector3 pos = _mContainerTrans.anchoredPosition3D;
            if (_mScrollRect.horizontal)
            {
                float maxCanMoveX = Mathf.Max(ContainerTrans.rect.width - ViewPortWidth, 0);
                if(maxCanMoveX > 0)
                {
                    float x = -itemPos.x + offsetX;
                    x = Mathf.Min(Mathf.Abs(x), maxCanMoveX) * Mathf.Sign(x);
                    pos.x = x;
                } 
            }
            if(_mScrollRect.vertical)
            {
                float maxCanMoveY = Mathf.Max(ContainerTrans.rect.height - ViewPortHeight, 0);
                if(maxCanMoveY > 0)
                {
                    float y = -itemPos.y + offsetY;
                    y = Mathf.Min(Mathf.Abs(y), maxCanMoveY) * Mathf.Sign(y);
                    pos.y = y;
                }
            }
            if(pos != _mContainerTrans.anchoredPosition3D)
            {
                _mContainerTrans.anchoredPosition3D = pos;
            }
            VaildAndSetContainerPos();
            ForceToCheckContentPos();
        }

        //update all visible items.
        public void RefreshAllShownItem()
        {
            int count = _mItemGroupList.Count;
            if (count == 0)
            {
                return;
            }
            ForceToCheckContentPos();
            RecycleAllItem();
            UpdateGridViewContent();
        }


        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            _mCurSnapData.Clear();
            _mIsDraging = true;
            if (mOnBeginDragAction != null)
            {
                mOnBeginDragAction(eventData);
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            _mIsDraging = false;
            ForceSnapUpdateCheck();
            if (mOnEndDragAction != null)
            {
                mOnEndDragAction(eventData);
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            if (mOnDragingAction != null)
            {
                mOnDragingAction(eventData);
            }
        }


        public int GetItemIndexByRowColumn(int row, int column)
        {
            if (mGridFixedType == GridFixedType.ColumnCountFixed)
            {
                return row * mFixedRowOrColumnCount + column;
            }
            else
            {
                return column * mFixedRowOrColumnCount + row;
            }
        }


        public RowColumnPair GetRowColumnByItemIndex(int itemIndex)
        {
            if(itemIndex < 0)
            {
                itemIndex = 0;
            }
            if (mGridFixedType == GridFixedType.ColumnCountFixed)
            {
                int row = itemIndex / mFixedRowOrColumnCount;
                int column = itemIndex % mFixedRowOrColumnCount;
                return new RowColumnPair(row, column);
            }
            else
            {
                int column = itemIndex / mFixedRowOrColumnCount;
                int row = itemIndex % mFixedRowOrColumnCount;
                return new RowColumnPair(row, column);
            }
        }


        public Vector2 GetItemAbsPos(int row, int column)
        {
            float x = _mStartPadding.x + column * _mItemSizeWithPadding.x;
            float y = _mStartPadding.y + row * _mItemSizeWithPadding.y;
            return new Vector2(x, y);
        }


        public Vector2 GetItemPos(int row, int column)
        {
            Vector2 absPos = GetItemAbsPos(row, column);
            float x = absPos.x;
            float y = absPos.y;
            if (ArrangeType == GridItemArrangeType.TopLeftToBottomRight)
            {
                return new Vector2(x, -y);
            }
            else if (ArrangeType == GridItemArrangeType.BottomLeftToTopRight)
            {
                return new Vector2(x, y);
            }
            else if (ArrangeType == GridItemArrangeType.TopRightToBottomLeft)
            {
                return new Vector2(-x, -y);
            }
            else if (ArrangeType == GridItemArrangeType.BottomRightToTopLeft)
            {
                return new Vector2(-x, y);
            }
            return Vector2.zero;
        }

        //get the shown item of itemIndex, if this item is not shown,then return null.
        public LoopGridViewItem GetShownItemByItemIndex(int itemIndex)
        {
            if(itemIndex < 0 || itemIndex >= ItemTotalCount)
            {
                return null;
            }
            if(_mItemGroupList.Count == 0)
            {
                return null;
            }
            RowColumnPair val = GetRowColumnByItemIndex(itemIndex);
            return GetShownItemByRowColumn(val.MRow, val.MColumn);
        }

        //get the shown item of (row, column), if this item is not shown,then return null.
        public LoopGridViewItem GetShownItemByRowColumn(int row, int column)
        {
            if (_mItemGroupList.Count == 0)
            {
                return null;
            }
            if (mGridFixedType == GridFixedType.ColumnCountFixed)
            {
                GridItemGroup group = GetShownGroup(row);
                if (group == null)
                {
                    return null;
                }
                return group.GetItemByColumn(column);
            }
            else
            {
                GridItemGroup group = GetShownGroup(column);
                if (group == null)
                {
                    return null;
                }
                return group.GetItemByRow(row);
            }
        }

        public void UpdateAllGridSetting()
        {
            UpdateStartEndPadding();
            UpdateItemSize();
            UpdateColumnRowCount();
            UpdateContentSize();
            ForceSnapUpdateCheck();
            ForceToCheckContentPos();
        }

        //set mGridFixedType and mFixedRowOrColumnCount at runtime
        public void SetGridFixedGroupCount(GridFixedType fixedType,int count)
        {
            if(mGridFixedType == fixedType && mFixedRowOrColumnCount == count)
            {
                return;
            }
            mGridFixedType = fixedType;
            mFixedRowOrColumnCount = count;
            UpdateColumnRowCount();
            UpdateContentSize();
            if (_mItemGroupList.Count == 0)
            {
                return;
            }
            RecycleAllItem();
            ForceSnapUpdateCheck();
            ForceToCheckContentPos();
        }
        //change item size at runtime
        public void SetItemSize(Vector2 newSize)
        {
            if (newSize == mItemSize)
            {
                return;
            }
            mItemSize = newSize;
            UpdateItemSize();
            UpdateContentSize();
            if (_mItemGroupList.Count == 0)
            {
                return;
            }
            RecycleAllItem();
            ForceSnapUpdateCheck();
            ForceToCheckContentPos();
        }
        //change item padding at runtime
        public void SetItemPadding(Vector2 newPadding)
        {
            if (newPadding == mItemPadding)
            {
                return;
            }
            mItemPadding = newPadding;
            UpdateItemSize();
            UpdateContentSize();
            if (_mItemGroupList.Count == 0)
            {
                return;
            }
            RecycleAllItem();
            ForceSnapUpdateCheck();
            ForceToCheckContentPos();
        }
        //change padding at runtime
        public void SetPadding(RectOffset newPadding)
        {
            if (newPadding == mPadding)
            {
                return;
            }
            mPadding = newPadding;
            UpdateStartEndPadding();
            UpdateContentSize();
            if (_mItemGroupList.Count == 0)
            {
                return;
            }
            RecycleAllItem();
            ForceSnapUpdateCheck();
            ForceToCheckContentPos();
        }


        public void UpdateContentSize()
        {
            float width = _mStartPadding.x + _mColumnCount * _mItemSizeWithPadding.x - mItemPadding.x + _mEndPadding.x;
            float height = _mStartPadding.y + _mRowCount * _mItemSizeWithPadding.y - mItemPadding.y + _mEndPadding.y;
            if (_mContainerTrans.rect.height != height)
            {
                _mContainerTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
            if (_mContainerTrans.rect.width != width)
            {
                _mContainerTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            }
        }


        public void VaildAndSetContainerPos()
        {
            Vector3 pos = _mContainerTrans.anchoredPosition3D;
            _mContainerTrans.anchoredPosition3D = GetContainerVaildPos(pos.x, pos.y);
        }

        public void ClearAllTmpRecycledItem()
        {
            int count = _mItemPoolList.Count;
            for (int i = 0; i < count; ++i)
            {
                _mItemPoolList[i].ClearTmpRecycledItem();
            }
        }


        public void RecycleAllItem()
        {
            foreach (GridItemGroup group in _mItemGroupList)
            {
                RecycleItemGroupTmp(group);
            }
            _mItemGroupList.Clear();
        }

        public void UpdateGridViewContent()
        {
            _mListUpdateCheckFrameCount++;
            if (_mItemTotalCount == 0)
            {
                if (_mItemGroupList.Count > 0)
                {
                    RecycleAllItem();
                }
                return;
            }
            UpdateCurFrameItemRangeData();
            if (mGridFixedType == GridFixedType.ColumnCountFixed)
            {
                int groupCount = _mItemGroupList.Count;
                int minRow = _mCurFrameItemRangeData.MMinRow;
                int maxRow = _mCurFrameItemRangeData.MMaxRow;
                for (int i = groupCount - 1; i >= 0; --i)
                {
                    GridItemGroup group = _mItemGroupList[i];
                    if (group.GroupIndex < minRow || group.GroupIndex > maxRow)
                    {
                        RecycleItemGroupTmp(group);
                        _mItemGroupList.RemoveAt(i);
                    }
                }
                if (_mItemGroupList.Count == 0)
                {
                    GridItemGroup group = CreateItemGroup(minRow);
                    _mItemGroupList.Add(group);
                }
                while (_mItemGroupList[0].GroupIndex > minRow)
                {
                    GridItemGroup group = CreateItemGroup(_mItemGroupList[0].GroupIndex - 1);
                    _mItemGroupList.Insert(0, group);
                }
                while (_mItemGroupList[_mItemGroupList.Count - 1].GroupIndex < maxRow)
                {
                    GridItemGroup group = CreateItemGroup(_mItemGroupList[_mItemGroupList.Count - 1].GroupIndex + 1);
                    _mItemGroupList.Add(group);
                }
                int count = _mItemGroupList.Count;
                for (int i = 0; i < count; ++i)
                {
                    UpdateRowItemGroupForRecycleAndNew(_mItemGroupList[i]);
                }
            }
            else
            {
                int groupCount = _mItemGroupList.Count;
                int minColumn = _mCurFrameItemRangeData.MMinColumn;
                int maxColumn = _mCurFrameItemRangeData.MMaxColumn;
                for (int i = groupCount - 1; i >= 0; --i)
                {
                    GridItemGroup group = _mItemGroupList[i];
                    if (group.GroupIndex < minColumn || group.GroupIndex > maxColumn)
                    {
                        RecycleItemGroupTmp(group);
                        _mItemGroupList.RemoveAt(i);
                    }
                }
                if (_mItemGroupList.Count == 0)
                {
                    GridItemGroup group = CreateItemGroup(minColumn);
                    _mItemGroupList.Add(group);
                }
                while (_mItemGroupList[0].GroupIndex > minColumn)
                {
                    GridItemGroup group = CreateItemGroup(_mItemGroupList[0].GroupIndex - 1);
                    _mItemGroupList.Insert(0, group);
                }
                while (_mItemGroupList[_mItemGroupList.Count - 1].GroupIndex < maxColumn)
                {
                    GridItemGroup group = CreateItemGroup(_mItemGroupList[_mItemGroupList.Count - 1].GroupIndex + 1);
                    _mItemGroupList.Add(group);
                }
                int count = _mItemGroupList.Count;
                for (int i = 0; i < count; ++i)
                {
                    UpdateColumnItemGroupForRecycleAndNew(_mItemGroupList[i]);
                }
            }
        }

        public void UpdateStartEndPadding()
        {
            if (ArrangeType == GridItemArrangeType.TopLeftToBottomRight)
            {
                _mStartPadding.x = mPadding.left;
                _mStartPadding.y = mPadding.top;
                _mEndPadding.x = mPadding.right;
                _mEndPadding.y = mPadding.bottom;
            }
            else if (ArrangeType == GridItemArrangeType.BottomLeftToTopRight)
            {
                _mStartPadding.x = mPadding.left;
                _mStartPadding.y = mPadding.bottom;
                _mEndPadding.x = mPadding.right;
                _mEndPadding.y = mPadding.top;
            }
            else if (ArrangeType == GridItemArrangeType.TopRightToBottomLeft)
            {
                _mStartPadding.x = mPadding.right;
                _mStartPadding.y = mPadding.top;
                _mEndPadding.x = mPadding.left;
                _mEndPadding.y = mPadding.bottom;
            }
            else if (ArrangeType == GridItemArrangeType.BottomRightToTopLeft)
            {
                _mStartPadding.x = mPadding.right;
                _mStartPadding.y = mPadding.bottom;
                _mEndPadding.x = mPadding.left;
                _mEndPadding.y = mPadding.top;
            }
        }


        public void UpdateItemSize()
        {
            if (mItemSize.x > 0f && mItemSize.y > 0f)
            {
                _mItemSizeWithPadding = mItemSize + mItemPadding;
                return;
            }
            do
            {
                if (mItemPrefabDataList.Count == 0)
                {
                    break;
                }
                GameObject obj = mItemPrefabDataList[0].mItemPrefab;
                if (obj == null)
                {
                    break;
                }
                RectTransform rtf = obj.GetComponent<RectTransform>();
                if (rtf == null)
                {
                    break;
                }
                mItemSize = rtf.rect.size;
                _mItemSizeWithPadding = mItemSize + mItemPadding;

            } while (false);

            if (mItemSize.x <= 0 || mItemSize.y <= 0)
            {
                Debug.LogError("Error, ItemSize is invaild.");
            }

        }

        public void UpdateColumnRowCount()
        {
            if (mGridFixedType == GridFixedType.ColumnCountFixed)
            {
                _mColumnCount = mFixedRowOrColumnCount;
                _mRowCount = _mItemTotalCount / _mColumnCount;
                if (_mItemTotalCount % _mColumnCount > 0)
                {
                    _mRowCount++;
                }
                if (_mItemTotalCount <= _mColumnCount)
                {
                    _mColumnCount = _mItemTotalCount;
                }
            }
            else
            {
                _mRowCount = mFixedRowOrColumnCount;
                _mColumnCount = _mItemTotalCount / _mRowCount;
                if (_mItemTotalCount % _mRowCount > 0)
                {
                    _mColumnCount++;
                }
                if (_mItemTotalCount <= _mRowCount)
                {
                    _mRowCount = _mItemTotalCount;
                }
            }
        }




        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>


        bool IsContainerTransCanMove()
        {
            if (_mItemTotalCount == 0)
            {
                return false;
            }
            if (_mScrollRect.horizontal && ContainerTrans.rect.width > ViewPortWidth)
            {
                return true;
            }
            if (_mScrollRect.vertical && ContainerTrans.rect.height > ViewPortHeight)
            {
                return true;
            }
            return false;
        }



        void RecycleItemGroupTmp(GridItemGroup group)
        {
            if (group == null)
            {
                return;
            }
            while(group.First != null)
            {
                LoopGridViewItem item = group.RemoveFirst();
                RecycleItemTmp(item);
            }
            group.Clear();
            RecycleOneItemGroupObj(group);
        }



        void RecycleItemTmp(LoopGridViewItem item)
        {
            if (item == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(item.ItemPrefabName))
            {
                return;
            }
            GridItemPool pool = null;
            if (_mItemPoolDict.TryGetValue(item.ItemPrefabName, out pool) == false)
            {
                return;
            }
            pool.RecycleItem(item);

        }


        void AdjustViewPortPivot()
        {
            RectTransform rtf = _mViewPortRectTransform;
            if (ArrangeType == GridItemArrangeType.TopLeftToBottomRight)
            {
                rtf.pivot = new Vector2(0, 1);
            }
            else if (ArrangeType == GridItemArrangeType.BottomLeftToTopRight)
            {
                rtf.pivot = new Vector2(0, 0);
            }
            else if (ArrangeType == GridItemArrangeType.TopRightToBottomLeft)
            {
                rtf.pivot = new Vector2(1, 1);
            }
            else if (ArrangeType == GridItemArrangeType.BottomRightToTopLeft)
            {
                rtf.pivot = new Vector2(1, 0);
            }
        }

        void AdjustContainerAnchorAndPivot()
        {
            RectTransform rtf = ContainerTrans;

            if (ArrangeType == GridItemArrangeType.TopLeftToBottomRight)
            {
                rtf.anchorMin = new Vector2(0, 1);
                rtf.anchorMax = new Vector2(0, 1);
                rtf.pivot = new Vector2(0, 1);
            }
            else if (ArrangeType == GridItemArrangeType.BottomLeftToTopRight)
            {
                rtf.anchorMin = new Vector2(0, 0);
                rtf.anchorMax = new Vector2(0, 0);
                rtf.pivot = new Vector2(0, 0);
            }
            else if (ArrangeType == GridItemArrangeType.TopRightToBottomLeft)
            {
                rtf.anchorMin = new Vector2(1, 1);
                rtf.anchorMax = new Vector2(1, 1);
                rtf.pivot = new Vector2(1, 1);
            }
            else if (ArrangeType == GridItemArrangeType.BottomRightToTopLeft)
            {
                rtf.anchorMin = new Vector2(1, 0);
                rtf.anchorMax = new Vector2(1, 0);
                rtf.pivot = new Vector2(1, 0);
            }
        }

        void AdjustItemAnchorAndPivot(RectTransform rtf)
        {
            if (ArrangeType == GridItemArrangeType.TopLeftToBottomRight)
            {
                rtf.anchorMin = new Vector2(0, 1);
                rtf.anchorMax = new Vector2(0, 1);
                rtf.pivot = new Vector2(0, 1);
            }
            else if (ArrangeType == GridItemArrangeType.BottomLeftToTopRight)
            {
                rtf.anchorMin = new Vector2(0, 0);
                rtf.anchorMax = new Vector2(0, 0);
                rtf.pivot = new Vector2(0, 0);
            }
            else if (ArrangeType == GridItemArrangeType.TopRightToBottomLeft)
            {
                rtf.anchorMin = new Vector2(1, 1);
                rtf.anchorMax = new Vector2(1, 1);
                rtf.pivot = new Vector2(1, 1);
            }
            else if (ArrangeType == GridItemArrangeType.BottomRightToTopLeft)
            {
                rtf.anchorMin = new Vector2(1, 0);
                rtf.anchorMax = new Vector2(1, 0);
                rtf.pivot = new Vector2(1, 0);
            }
        }





        void InitItemPool()
        {
            foreach (GridViewItemPrefabConfData data in mItemPrefabDataList)
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
                AdjustItemAnchorAndPivot(rtf);
                LoopGridViewItem tItem = data.mItemPrefab.GetComponent<LoopGridViewItem>();
                if (tItem == null)
                {
                    data.mItemPrefab.AddComponent<LoopGridViewItem>();
                }
                GridItemPool pool = new GridItemPool();
                pool.Init(data.mItemPrefab,data.mInitCreateCount, _mContainerTrans);
                _mItemPoolDict.Add(prefabName, pool);
                _mItemPoolList.Add(pool);
            }
        }


        LoopGridViewItem GetNewItemByRowColumn(int row,int column)
        {
            int itemIndex = GetItemIndexByRowColumn(row, column);
            if(itemIndex < 0 || itemIndex >= ItemTotalCount)
            {
                return null;
            }
            LoopGridViewItem newItem = _mOnGetItemByRowColumn(this,itemIndex,row,column);
            if (newItem == null)
            {
                return null;
            }
            newItem.NextItem = null;
            newItem.PrevItem = null;
            newItem.Row = row;
            newItem.Column = column;
            newItem.ItemIndex = itemIndex;
            newItem.ItemCreatedCheckFrameCount = _mListUpdateCheckFrameCount;
            return newItem;
        }


        RowColumnPair GetCeilItemRowColumnAtGivenAbsPos(float ax,float ay)
        {
            ax = Mathf.Abs(ax);
            ay = Mathf.Abs(ay);
            int row = Mathf.CeilToInt((ay - _mStartPadding.y) / _mItemSizeWithPadding.y)-1;
            int column = Mathf.CeilToInt((ax - _mStartPadding.x) / _mItemSizeWithPadding.x)-1;
            if(row < 0)
            {
                row = 0;
            }
            if(row >= _mRowCount)
            {
                row = _mRowCount - 1;
            }
            if(column < 0)
            {
                column = 0;
            }
            if(column >= _mColumnCount)
            {
                column = _mColumnCount - 1;
            }
            return new RowColumnPair(row,column);
        }

        void Update()
        {
            if(_mListViewInited == false)
            {
                return;
            }
            UpdateSnapMove();
            UpdateGridViewContent();
            ClearAllTmpRecycledItem();
        }


        GridItemGroup CreateItemGroup(int groupIndex)
        {
            GridItemGroup ret = GetOneItemGroupObj();
            ret.GroupIndex = groupIndex;
            return ret;
        }
        Vector2 GetContainerMovedDistance()
        {
            Vector2 pos = GetContainerVaildPos(ContainerTrans.anchoredPosition3D.x, ContainerTrans.anchoredPosition3D.y);
            return new Vector2(Mathf.Abs(pos.x), Mathf.Abs(pos.y));
        }


        Vector2 GetContainerVaildPos(float curX, float curY)
        {
            float maxCanMoveX = Mathf.Max(ContainerTrans.rect.width - ViewPortWidth, 0);
            float maxCanMoveY = Mathf.Max(ContainerTrans.rect.height - ViewPortHeight, 0);
            if (mArrangeType == GridItemArrangeType.TopLeftToBottomRight)
            {
                curX = Mathf.Clamp(curX, -maxCanMoveX, 0);
                curY = Mathf.Clamp(curY, 0, maxCanMoveY);
            }
            else if (mArrangeType == GridItemArrangeType.BottomLeftToTopRight)
            {
                curX = Mathf.Clamp(curX, -maxCanMoveX, 0);
                curY = Mathf.Clamp(curY, -maxCanMoveY,0);
            }
            else if (mArrangeType == GridItemArrangeType.BottomRightToTopLeft)
            {
                curX = Mathf.Clamp(curX, 0, maxCanMoveX);
                curY = Mathf.Clamp(curY, -maxCanMoveY,0);

            }
            else if (mArrangeType == GridItemArrangeType.TopRightToBottomLeft)
            {
                curX = Mathf.Clamp(curX, 0,maxCanMoveX);
                curY = Mathf.Clamp(curY, 0,maxCanMoveY);
            }
            return new Vector2(curX, curY);
        }


        void UpdateCurFrameItemRangeData()
        {
            Vector2 distVector2 = GetContainerMovedDistance();
            if (_mNeedCheckContentPosLeftCount <= 0 && _mCurFrameItemRangeData.MCheckedPosition == distVector2)
            {
               return;
            }
            if (_mNeedCheckContentPosLeftCount > 0)
            {
                _mNeedCheckContentPosLeftCount--;
            }
            float distX = distVector2.x - mItemRecycleDistance.x;
            float distY = distVector2.y - mItemRecycleDistance.y;
            if(distX < 0)
            {
                distX = 0;
            }
            if(distY < 0)
            {
                distY = 0;
            }
            RowColumnPair val = GetCeilItemRowColumnAtGivenAbsPos(distX, distY);
            _mCurFrameItemRangeData.MMinColumn = val.MColumn;
            _mCurFrameItemRangeData.MMinRow = val.MRow;
            distX = distVector2.x + mItemRecycleDistance.x + ViewPortWidth;
            distY = distVector2.y + mItemRecycleDistance.y + ViewPortHeight;
            val = GetCeilItemRowColumnAtGivenAbsPos(distX, distY);
            _mCurFrameItemRangeData.MMaxColumn = val.MColumn;
            _mCurFrameItemRangeData.MMaxRow = val.MRow;
            _mCurFrameItemRangeData.MCheckedPosition = distVector2;
        }

       


        void UpdateRowItemGroupForRecycleAndNew(GridItemGroup group)
        {
            int minColumn = _mCurFrameItemRangeData.MMinColumn;
            int maxColumn = _mCurFrameItemRangeData.MMaxColumn;
            int row = group.GroupIndex;
            while(group.First != null && group.First.Column < minColumn)
            {
                RecycleItemTmp(group.RemoveFirst());
            }
            while (group.Last != null && ( ( group.Last.Column > maxColumn ) || ( group.Last.ItemIndex >= ItemTotalCount ) ) )
            {
                RecycleItemTmp(group.RemoveLast());
            }
            if(group.First == null)
            {
                LoopGridViewItem item = GetNewItemByRowColumn(row, minColumn);
                if(item == null)
                {
                    return;
                }
                item.CachedRectTransform.anchoredPosition3D = GetItemPos(item.Row, item.Column);
                group.AddFirst(item);
            }
            while (group.First.Column > minColumn)
            {
                LoopGridViewItem item = GetNewItemByRowColumn(row, group.First.Column-1);
                if (item == null)
                {
                    break;
                }
                item.CachedRectTransform.anchoredPosition3D = GetItemPos(item.Row, item.Column);

                group.AddFirst(item);
            }
            while (group.Last.Column < maxColumn)
            {
                LoopGridViewItem item = GetNewItemByRowColumn(row, group.Last.Column + 1);
                if (item == null)
                {
                    break;
                }
                item.CachedRectTransform.anchoredPosition3D = GetItemPos(item.Row, item.Column);

                group.AddLast(item);
            }
        }

        void UpdateColumnItemGroupForRecycleAndNew(GridItemGroup group)
        {
            int minRow = _mCurFrameItemRangeData.MMinRow;
            int maxRow = _mCurFrameItemRangeData.MMaxRow;
            int column = group.GroupIndex;
            while (group.First != null && group.First.Row < minRow)
            {
                RecycleItemTmp(group.RemoveFirst());
            }
            while (group.Last != null && ( ( group.Last.Row > maxRow )|| (group.Last.ItemIndex >= ItemTotalCount)) )
            {
                RecycleItemTmp(group.RemoveLast());
            }
            if (group.First == null)
            {
                LoopGridViewItem item = GetNewItemByRowColumn(minRow, column);
                if (item == null)
                {
                    return;
                }
                item.CachedRectTransform.anchoredPosition3D = GetItemPos(item.Row, item.Column);
                group.AddFirst(item);
            }
            while (group.First.Row > minRow)
            {
                LoopGridViewItem item = GetNewItemByRowColumn(group.First.Row - 1, column);
                if (item == null)
                {
                    break;
                }
                item.CachedRectTransform.anchoredPosition3D = GetItemPos(item.Row, item.Column);

                group.AddFirst(item);
            }
            while (group.Last.Row < maxRow)
            {
                LoopGridViewItem item = GetNewItemByRowColumn(group.Last.Row + 1,column );
                if (item == null)
                {
                    break;
                }
                item.CachedRectTransform.anchoredPosition3D = GetItemPos(item.Row, item.Column);

                group.AddLast(item);
            }
        }


        void SetScrollbarListener()
        {
            if(ItemSnapEnable == false)
            {
                return;
            }
            _mScrollBarClickEventListener1 = null;
            _mScrollBarClickEventListener2 = null;
            Scrollbar curScrollBar1 = null;
            Scrollbar curScrollBar2 = null;
            if (_mScrollRect.vertical && _mScrollRect.verticalScrollbar != null)
            {
                curScrollBar1 = _mScrollRect.verticalScrollbar;

            }
            if (_mScrollRect.horizontal && _mScrollRect.horizontalScrollbar != null)
            {
                curScrollBar2 = _mScrollRect.horizontalScrollbar;
            }
            if (curScrollBar1 != null)
            {
                ClickEventListener listener = ClickEventListener.Get(curScrollBar1.gameObject);
                _mScrollBarClickEventListener1 = listener;
                listener.SetPointerUpHandler(OnPointerUpInScrollBar);
                listener.SetPointerDownHandler(OnPointerDownInScrollBar);
            }
            if (curScrollBar2 != null)
            {
                ClickEventListener listener = ClickEventListener.Get(curScrollBar2.gameObject);
                _mScrollBarClickEventListener2 = listener;
                listener.SetPointerUpHandler(OnPointerUpInScrollBar);
                listener.SetPointerDownHandler(OnPointerDownInScrollBar);
            }

        }

        void OnPointerDownInScrollBar(GameObject obj)
        {
            _mCurSnapData.Clear();
        }

        void OnPointerUpInScrollBar(GameObject obj)
        {
            ForceSnapUpdateCheck();
        }

        RowColumnPair FindNearestItemWithLocalPos(float x,float y)
        {
            Vector2 targetPos = new Vector2(x, y);
            RowColumnPair val = GetCeilItemRowColumnAtGivenAbsPos(targetPos.x, targetPos.y);
            int row = val.MRow;
            int column = val.MColumn;
            float distance = 0;
            RowColumnPair ret = new RowColumnPair(-1, -1);
            Vector2 pos = Vector2.zero;
            float minDistance = float.MaxValue;
            for (int r = row - 1; r <= row + 1; ++r)
            {
                for (int c = column - 1; c <= column + 1; ++c)
                {
                    if (r >= 0 && r < _mRowCount && c >= 0 && c < _mColumnCount)
                    {
                        pos = GetItemSnapPivotLocalPos(r, c);
                        distance = (pos - targetPos).sqrMagnitude;
                        if(distance < minDistance)
                        {
                            minDistance = distance;
                            ret.MRow = r;
                            ret.MColumn = c;
                        }
                    }
                }
            }
            return ret;
        }

        Vector2 GetItemSnapPivotLocalPos(int row,int column)
        {
            Vector2 absPos = GetItemAbsPos(row, column);
            if (mArrangeType == GridItemArrangeType.TopLeftToBottomRight)
            {
                float x = absPos.x + mItemSize.x * mItemSnapPivot.x;
                float y = -absPos.y - mItemSize.y * (1 - mItemSnapPivot.y);
                return new Vector2(x, y);
            }
            else if(mArrangeType == GridItemArrangeType.BottomLeftToTopRight)
            {
                float x = absPos.x + mItemSize.x * mItemSnapPivot.x;
                float y = absPos.y + mItemSize.y * mItemSnapPivot.y;
                return new Vector2(x, y);
            }
            else if (mArrangeType == GridItemArrangeType.TopRightToBottomLeft)
            {
                float x = -absPos.x - mItemSize.x * (1-mItemSnapPivot.x);
                float y = -absPos.y - mItemSize.y * (1-mItemSnapPivot.y);
                return new Vector2(x, y);
            }
            else if (mArrangeType == GridItemArrangeType.BottomRightToTopLeft)
            {
                float x = -absPos.x - mItemSize.x * (1-mItemSnapPivot.x);
                float y = absPos.y + mItemSize.y * mItemSnapPivot.y;
                return new Vector2(x, y);
            }
            return Vector2.zero;
        }

        Vector2 GetViewPortSnapPivotLocalPos(Vector2 pos)
        {
            float pivotLocalPosX = 0;
            float pivotLocalPosY = 0;
            if (mArrangeType == GridItemArrangeType.TopLeftToBottomRight)
            {
                pivotLocalPosX = -pos.x + ViewPortWidth * mViewPortSnapPivot.x;
                pivotLocalPosY = -pos.y - ViewPortHeight * (1 - mViewPortSnapPivot.y);
            }
            else if (mArrangeType == GridItemArrangeType.BottomLeftToTopRight)
            {
                pivotLocalPosX = -pos.x + ViewPortWidth * mViewPortSnapPivot.x;
                pivotLocalPosY = -pos.y + ViewPortHeight * mViewPortSnapPivot.y;
            }
            else if (mArrangeType == GridItemArrangeType.TopRightToBottomLeft)
            {
                pivotLocalPosX = -pos.x - ViewPortWidth * (1 - mViewPortSnapPivot.x);
                pivotLocalPosY = -pos.y - ViewPortHeight * (1 - mViewPortSnapPivot.y);
            }
            else if (mArrangeType == GridItemArrangeType.BottomRightToTopLeft)
            {
                pivotLocalPosX = -pos.x - ViewPortWidth * (1 - mViewPortSnapPivot.x);
                pivotLocalPosY = -pos.y + ViewPortHeight * mViewPortSnapPivot.y;
            }
            return new Vector2(pivotLocalPosX, pivotLocalPosY);
        }

        void UpdateNearestSnapItem(bool forceSendEvent)
        {
            if (mItemSnapEnable == false)
            {
                return;
            }
            int count = _mItemGroupList.Count;
            if (count == 0)
            {
                return;
            }
            if(IsContainerTransCanMove() == false)
            {
                return;
            }
            Vector2 pos = GetContainerVaildPos(ContainerTrans.anchoredPosition3D.x, ContainerTrans.anchoredPosition3D.y);
            bool needCheck = (pos.y != _mLastSnapCheckPos.y || pos.x != _mLastSnapCheckPos.x);
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
                RowColumnPair curVal = new RowColumnPair(-1,-1);
                Vector2 snapTartetPos = GetViewPortSnapPivotLocalPos(pos);
                curVal = FindNearestItemWithLocalPos(snapTartetPos.x, snapTartetPos.y);
                if (curVal.MRow >= 0)
                {
                    RowColumnPair oldNearestItem = _mCurSnapNearestItemRowColumn;
                    _mCurSnapNearestItemRowColumn = curVal;
                    if (forceSendEvent || oldNearestItem != _mCurSnapNearestItemRowColumn)
                    {
                        if (mOnSnapNearestChanged != null)
                        {
                            mOnSnapNearestChanged(this);
                        }
                    }
                }
                else
                {
                    _mCurSnapNearestItemRowColumn.MRow = -1;
                    _mCurSnapNearestItemRowColumn.MColumn = -1;
                }
            }
        }

        void UpdateFromSettingParam(LoopGridViewSettingParam param)
        {
            if (param == null)
            {
                return;
            }
            if (param.MItemSize != null)
            {
                mItemSize = (Vector2)(param.MItemSize);
            }
            if (param.MItemPadding != null)
            {
                mItemPadding = (Vector2)(param.MItemPadding);
            }
            if (param.MPadding != null)
            {
                mPadding = (RectOffset)(param.MPadding);
            }
            if (param.MGridFixedType != null)
            {
                mGridFixedType = (GridFixedType)(param.MGridFixedType);
            }
            if (param.MFixedRowOrColumnCount != null)
            {
                mFixedRowOrColumnCount = (int)(param.MFixedRowOrColumnCount);
            }
        }

        //snap move will finish at once.
        public void FinishSnapImmediately()
        {
            UpdateSnapMove(true);
        }

        //update snap move. if immediate is set true, then the snap move will finish at once.
        void UpdateSnapMove(bool immediate = false, bool forceSendEvent = false)
        {
            if (mItemSnapEnable == false)
            {
                return;
            }
            UpdateNearestSnapItem(false);
            Vector2 pos = _mContainerTrans.anchoredPosition3D;
            if (CanSnap() == false)
            {
                ClearSnapData();
                return;
            }
            UpdateCurSnapData();
            if (_mCurSnapData.MSnapStatus != SnapStatus.SnapMoving)
            {
                return;
            }
            float v = Mathf.Abs(_mScrollRect.velocity.x) + Mathf.Abs(_mScrollRect.velocity.y);
            if (v > 0)
            {
                _mScrollRect.StopMovement();
            }
            float old = _mCurSnapData.MCurSnapVal;
            _mCurSnapData.MCurSnapVal = Mathf.SmoothDamp(_mCurSnapData.MCurSnapVal, _mCurSnapData.MTargetSnapVal, ref _mSmoothDumpVel, _mSmoothDumpRate);
            float dt = _mCurSnapData.MCurSnapVal - old;

            if (immediate || Mathf.Abs(_mCurSnapData.MTargetSnapVal - _mCurSnapData.MCurSnapVal) < _mSnapFinishThreshold)
            {
                pos = pos + (_mCurSnapData.MTargetSnapVal - old)* _mCurSnapData.MSnapNeedMoveDir;
                _mCurSnapData.MSnapStatus = SnapStatus.SnapMoveFinish;
                if (mOnSnapItemFinished != null)
                {
                    LoopGridViewItem targetItem = GetShownItemByRowColumn(_mCurSnapNearestItemRowColumn.MRow, _mCurSnapNearestItemRowColumn.MColumn);
                    if (targetItem != null)
                    {
                        mOnSnapItemFinished(this, targetItem);
                    }
                }
            }
            else
            {
                pos = pos + dt * _mCurSnapData.MSnapNeedMoveDir;
            }
            _mContainerTrans.anchoredPosition3D = GetContainerVaildPos(pos.x, pos.y);
        }

        GridItemGroup GetShownGroup(int groupIndex)
        {
            if(groupIndex < 0)
            {
                return null;
            }
            int count = _mItemGroupList.Count;
            if (count == 0)
            {
                return null;
            }
            if (groupIndex < _mItemGroupList[0].GroupIndex || groupIndex > _mItemGroupList[count - 1].GroupIndex)
            {
                return null;
            }
            int i = groupIndex - _mItemGroupList[0].GroupIndex;
            return _mItemGroupList[i];
        }

 
        void FillCurSnapData(int row,int column)
        {
            Vector2 itemSnapPivotLocalPos = GetItemSnapPivotLocalPos(row, column);
            Vector2 containerPos = GetContainerVaildPos(ContainerTrans.anchoredPosition3D.x, ContainerTrans.anchoredPosition3D.y);
            Vector2 snapTartetPos = GetViewPortSnapPivotLocalPos(containerPos);
            Vector2 dir = snapTartetPos - itemSnapPivotLocalPos;
            if (_mScrollRect.horizontal == false)
            {
                dir.x = 0;
            }
            if(_mScrollRect.vertical == false)
            {
                dir.y = 0;
            }
            _mCurSnapData.MTargetSnapVal = dir.magnitude;
            _mCurSnapData.MCurSnapVal = 0;
            _mCurSnapData.MSnapNeedMoveDir = dir.normalized;
        }


        void UpdateCurSnapData()
        {
            int count = _mItemGroupList.Count;
            if (count == 0)
            {
                _mCurSnapData.Clear();
                return;
            }

            if (_mCurSnapData.MSnapStatus == SnapStatus.SnapMoveFinish)
            {
                if (_mCurSnapData.MSnapTarget == _mCurSnapNearestItemRowColumn)
                {
                    return;
                }
                _mCurSnapData.MSnapStatus = SnapStatus.NoTargetSet;
            }
            if (_mCurSnapData.MSnapStatus == SnapStatus.SnapMoving)
            {
                if ((_mCurSnapData.MSnapTarget == _mCurSnapNearestItemRowColumn) || _mCurSnapData.MIsForceSnapTo)
                {
                    return;
                }
                _mCurSnapData.MSnapStatus = SnapStatus.NoTargetSet;
            }
            if (_mCurSnapData.MSnapStatus == SnapStatus.NoTargetSet)
            {
                LoopGridViewItem nearestItem = GetShownItemByRowColumn(_mCurSnapNearestItemRowColumn.MRow, _mCurSnapNearestItemRowColumn.MColumn);
                if (nearestItem == null)
                {
                    return;
                }
                _mCurSnapData.MSnapTarget = _mCurSnapNearestItemRowColumn;
                _mCurSnapData.MSnapStatus = SnapStatus.TargetHasSet;
                _mCurSnapData.MIsForceSnapTo = false;
            }
            if (_mCurSnapData.MSnapStatus == SnapStatus.TargetHasSet)
            {
                LoopGridViewItem targetItem = GetShownItemByRowColumn(_mCurSnapData.MSnapTarget.MRow, _mCurSnapData.MSnapTarget.MColumn);
                if (targetItem == null)
                {
                    _mCurSnapData.Clear();
                    return;
                }
                FillCurSnapData(targetItem.Row,targetItem.Column);
                _mCurSnapData.MSnapStatus = SnapStatus.SnapMoving;
            }

        }
       

        bool CanSnap()
        {
            if (_mIsDraging)
            {
                return false;
            }
            if (_mScrollBarClickEventListener1 != null)
            {
                if (_mScrollBarClickEventListener1.IsPressd)
                {
                    return false;
                }
            }
            if (_mScrollBarClickEventListener2 != null)
            {
                if (_mScrollBarClickEventListener2.IsPressd)
                {
                    return false;
                }
            }
            if(IsContainerTransCanMove() == false)
            {
                return false;
            }
            if (Mathf.Abs(_mScrollRect.velocity.x) > _mSnapVecThreshold)
            {
                return false;
            }
            if (Mathf.Abs(_mScrollRect.velocity.y) > _mSnapVecThreshold)
            {
                return false;
            }
            Vector3 pos = _mContainerTrans.anchoredPosition3D;
            Vector2 vPos = GetContainerVaildPos(pos.x, pos.y);
            if(Mathf.Abs(pos.x - vPos.x) >3)
            {
                return false;
            }
            if (Mathf.Abs(pos.y - vPos.y) > 3)
            {
                return false;
            }
            return true;
        }

        GridItemGroup GetOneItemGroupObj()
        {
            int count = _mItemGroupObjPool.Count;
            if (count == 0)
            {
                return new GridItemGroup();
            }
            GridItemGroup ret = _mItemGroupObjPool[count - 1];
            _mItemGroupObjPool.RemoveAt(count - 1);
            return ret;
        }
        void RecycleOneItemGroupObj(GridItemGroup obj)
        {
            _mItemGroupObjPool.Add(obj);
        }


    }

}
