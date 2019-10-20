using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{

    public class StaggeredGridItemGroup
    {

        LoopStaggeredGridView _mParentGridView;
        ListItemArrangeType _mArrangeType = ListItemArrangeType.TopToBottom;
        List<LoopStaggeredGridViewItem> _mItemList = new List<LoopStaggeredGridViewItem>();
        RectTransform _mContainerTrans;
        ScrollRect _mScrollRect = null;
        public int MGroupIndex = 0;
        GameObject _mGameObject;
        List<int> _mItemIndexMap = new List<int>();
        RectTransform _mScrollRectTransform = null;
        RectTransform _mViewPortRectTransform = null;
        float _mItemDefaultWithPaddingSize = 0;
        int _mItemTotalCount = 0;
        bool _mIsVertList = false;
        System.Func<int, int, LoopStaggeredGridViewItem> _mOnGetItemByIndex;
        Vector3[] _mItemWorldCorners = new Vector3[4];
        Vector3[] _mViewPortRectLocalCorners = new Vector3[4];
        int _mCurReadyMinItemIndex = 0;
        int _mCurReadyMaxItemIndex = 0;
        bool _mNeedCheckNextMinItem = true;
        bool _mNeedCheckNextMaxItem = true;
        ItemPosMgr _mItemPosMgr = null;
        bool _mSupportScrollBar = true;
        int _mLastItemIndex = 0;
        float _mLastItemPadding = 0;
        Vector3 _mLastFrameContainerPos = Vector3.zero;
        int _mListUpdateCheckFrameCount = 0;

        public void Init(LoopStaggeredGridView parent, int itemTotalCount, int groupIndex,
            System.Func<int, int, LoopStaggeredGridViewItem> onGetItemByIndex)
        {
            MGroupIndex = groupIndex;
            _mParentGridView = parent;
            _mArrangeType = _mParentGridView.ArrangeType;
            _mGameObject = _mParentGridView.gameObject;
            _mScrollRect = _mGameObject.GetComponent<ScrollRect>();
            _mItemPosMgr = new ItemPosMgr(_mItemDefaultWithPaddingSize);
            _mScrollRectTransform = _mScrollRect.GetComponent<RectTransform>();
            _mContainerTrans = _mScrollRect.content;
            _mViewPortRectTransform = _mScrollRect.viewport;
            if (_mViewPortRectTransform == null)
            {
                _mViewPortRectTransform = _mScrollRectTransform;
            }
            _mIsVertList = (_mArrangeType == ListItemArrangeType.TopToBottom || _mArrangeType == ListItemArrangeType.BottomToTop);
            _mOnGetItemByIndex = onGetItemByIndex;
            _mItemTotalCount = itemTotalCount;
            _mViewPortRectTransform.GetLocalCorners(_mViewPortRectLocalCorners);
            if (_mItemTotalCount < 0)
            {
                _mSupportScrollBar = false;
            }
            if (_mSupportScrollBar)
            {
                _mItemPosMgr.SetItemMaxCount(_mItemTotalCount);
            }
            else
            {
                _mItemPosMgr.SetItemMaxCount(0);
            }
            _mCurReadyMaxItemIndex = 0;
            _mCurReadyMinItemIndex = 0;
            _mNeedCheckNextMaxItem = true;
            _mNeedCheckNextMinItem = true;
        }


        public List<int> ItemIndexMap
        {
            get { return _mItemIndexMap; }
        }

        public void ResetListView()
        {
            _mViewPortRectTransform.GetLocalCorners(_mViewPortRectLocalCorners);
        }

        //To get the visible item by itemIndex. If the item is not visible, then this method return null.
        public LoopStaggeredGridViewItem GetShownItemByItemIndex(int itemIndex)
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
            for(int i = 0;i<count;++i)
            {
                LoopStaggeredGridViewItem item = _mItemList[i];
                if(item.ItemIndex == itemIndex)
                {
                    return item; 
                }
            }
            return null;
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

        bool IsDraging
        {
            get { return _mParentGridView.IsDraging; }
        }

        /*
         All visible items is stored in a List<LoopStaggeredGridViewItem> , which is named mItemList;
         this method is to get the visible item by the index in visible items list. The parameter index is from 0 to mItemList.Count.
        */
        public LoopStaggeredGridViewItem GetShownItemByIndexInGroup(int indexInGroup)
        {
            int count = _mItemList.Count;
            if (count == 0)
            {
                return null;
            }
            if (indexInGroup < _mItemList[0].ItemIndexInGroup || indexInGroup > _mItemList[count - 1].ItemIndexInGroup)
            {
                return null;
            }
            int i = indexInGroup - _mItemList[0].ItemIndexInGroup;
            return _mItemList[i];
        }

        public int GetIndexInShownItemList(LoopStaggeredGridViewItem item)
        {
            if (item == null)
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


        //update all visible items.
        public void RefreshAllShownItem()
        {
            int count = _mItemList.Count;
            if (count == 0)
            {
                return;
            }
            RefreshAllShownItemWithFirstIndexInGroup(_mItemList[0].ItemIndexInGroup);
        }


        /*
      For a vertical scrollrect, when a visible item’s height changed at runtime, then this method should be called to let the LoopListView2 component reposition all visible items’ position.
      For a horizontal scrollrect, when a visible item’s width changed at runtime, then this method should be called to let the LoopListView2 component reposition all visible items’ position.
      */
        public void OnItemSizeChanged(int indexInGroup)
        {
            LoopStaggeredGridViewItem item = GetShownItemByIndexInGroup(indexInGroup);
            if (item == null)
            {
                return;
            }
            if (_mSupportScrollBar)
            {
                if (_mIsVertList)
                {
                    SetItemSize(indexInGroup, item.CachedRectTransform.rect.height, item.Padding);
                }
                else
                {
                    SetItemSize(indexInGroup, item.CachedRectTransform.rect.width, item.Padding);
                }
            }
            UpdateAllShownItemsPos();
        }


        /*
        To update a item by itemIndex.if the itemIndex-th item is not visible, then this method will do nothing.
        Otherwise this method will first call onGetItemByIndex(itemIndex) to get a updated item and then reposition all visible items'position. 
        */
        public void RefreshItemByIndexInGroup(int indexInGroup)
        {
            int count = _mItemList.Count;
            if(count == 0)
            {
                return;
            }
            if (indexInGroup < _mItemList[0].ItemIndexInGroup || indexInGroup > _mItemList[count - 1].ItemIndexInGroup)
            {
                return;
            }
            int firstItemIndexInGroup = _mItemList[0].ItemIndexInGroup;
            int i = indexInGroup - firstItemIndexInGroup;
            LoopStaggeredGridViewItem curItem = _mItemList[i];
            Vector3 pos = curItem.CachedRectTransform.anchoredPosition3D;
            RecycleItemTmp(curItem);
            LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(indexInGroup);
            if (newItem == null)
            {
                RefreshAllShownItemWithFirstIndexInGroup(firstItemIndexInGroup);
                return;
            }
            _mItemList[i] = newItem;
            if (_mIsVertList)
            {
                pos.x = newItem.StartPosOffset;
            }
            else
            {
                pos.y = newItem.StartPosOffset;
            }
            newItem.CachedRectTransform.anchoredPosition3D = pos;
            OnItemSizeChanged(indexInGroup);
            ClearAllTmpRecycledItem();
        }


        public void RefreshAllShownItemWithFirstIndexInGroup(int firstItemIndexInGroup)
        {
            int count = _mItemList.Count;
            if (count == 0)
            {
                return;
            }
            LoopStaggeredGridViewItem firstItem = _mItemList[0];
            Vector3 pos = firstItem.CachedRectTransform.anchoredPosition3D;
            RecycleAllItem();
            for (int i = 0; i < count; ++i)
            {
                int curIndex = firstItemIndexInGroup + i;
                LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(curIndex);
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
                if (_mSupportScrollBar)
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
            UpdateAllShownItemsPos();
            ClearAllTmpRecycledItem();
        }


        public void RefreshAllShownItemWithFirstIndexAndPos(int firstItemIndexInGroup, Vector3 pos)
        {
            RecycleAllItem();
            LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(firstItemIndexInGroup);
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
            if (_mSupportScrollBar)
            {
                if (_mIsVertList)
                {
                    SetItemSize(firstItemIndexInGroup, newItem.CachedRectTransform.rect.height, newItem.Padding);
                }
                else
                {
                    SetItemSize(firstItemIndexInGroup, newItem.CachedRectTransform.rect.width, newItem.Padding);
                }
            }
            _mItemList.Add(newItem);
            UpdateAllShownItemsPos();
            _mParentGridView.UpdateListViewWithDefault();
            ClearAllTmpRecycledItem();
        }





        void SetItemSize(int itemIndex, float itemSize, float padding)
        {
            _mItemPosMgr.SetItemSize(itemIndex, itemSize + padding);
            if (itemIndex >= _mLastItemIndex)
            {
                _mLastItemIndex = itemIndex;
                _mLastItemPadding = padding;
            }
        }

        bool GetPlusItemIndexAndPosAtGivenPos(float pos, ref int index, ref float itemPos)
        {
            return _mItemPosMgr.GetItemIndexAndPosAtGivenPos(pos, ref index, ref itemPos);
        }


        public float GetItemPos(int itemIndex)
        {
            return _mItemPosMgr.GetItemPos(itemIndex);
        }


        public Vector3 GetItemCornerPosInViewPort(LoopStaggeredGridViewItem item, ItemCornerEnum corner = ItemCornerEnum.LeftBottom)
        {
            item.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
            return _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[(int)corner]);
        }


        public void RecycleItemTmp(LoopStaggeredGridViewItem item)
        {
            _mParentGridView.RecycleItemTmp(item);
        }


        public void RecycleAllItem()
        {
            foreach (LoopStaggeredGridViewItem item in _mItemList)
            {
                RecycleItemTmp(item);
            }
            _mItemList.Clear();
        }

        public void ClearAllTmpRecycledItem()
        {
            _mParentGridView.ClearAllTmpRecycledItem();
        }

        LoopStaggeredGridViewItem GetNewItemByIndexInGroup(int indexInGroup)
        {
            return _mParentGridView.GetNewItemByGroupAndIndex(MGroupIndex, indexInGroup);
        }


        public int HadCreatedItemCount
        {
            get
            {
                return _mItemIndexMap.Count;
            }
        }


        public void SetListItemCount(int itemCount)
        {
            if (itemCount == _mItemTotalCount)
            {
                return;
            }
            int oldItemTotalCount = _mItemTotalCount;
            _mItemTotalCount = itemCount;
            UpdateItemIndexMap(oldItemTotalCount);
            if (oldItemTotalCount < _mItemTotalCount)
            {
                _mItemPosMgr.SetItemMaxCount(_mItemTotalCount);
            }
            else
            {
                _mItemPosMgr.SetItemMaxCount(HadCreatedItemCount);
                _mItemPosMgr.SetItemMaxCount(_mItemTotalCount);
            }
            RecycleAllItem();
            if (_mItemTotalCount == 0)
            {
                _mCurReadyMaxItemIndex = 0;
                _mCurReadyMinItemIndex = 0;
                _mNeedCheckNextMaxItem = false;
                _mNeedCheckNextMinItem = false;
                _mItemIndexMap.Clear();
                return;
            }
            
            if (_mCurReadyMaxItemIndex >= _mItemTotalCount)
            {
                _mCurReadyMaxItemIndex = _mItemTotalCount - 1;
            }
            _mNeedCheckNextMaxItem = true;
            _mNeedCheckNextMinItem = true;
        }

        void UpdateItemIndexMap(int oldItemTotalCount)
        {
            int count = _mItemIndexMap.Count;
            if (count == 0)
            {
                return;
            }
            if (_mItemTotalCount == 0)
            {
                _mItemIndexMap.Clear();
                return;
            }
            if(_mItemTotalCount >= oldItemTotalCount)
            {
                return;
            }
            int targetItemIndex = _mParentGridView.ItemTotalCount;
            if (_mItemIndexMap[count - 1] < targetItemIndex)
            {
                return;
            }
            int low = 0;
            int high = count - 1;
            int result = 0;
            while (low <= high)
            {
                int mid = (low + high) / 2;
                int index = _mItemIndexMap[mid];
                if (index == targetItemIndex)
                {
                    result = mid;
                    break;
                }
                else if (index < targetItemIndex)
                {
                    low = mid + 1;
                    result = low;
                }
                else
                {
                    break;
                }
            }
            int startIndex = 0;
            for(int i = result; i< count; ++i)
            {
                if(_mItemIndexMap[i] >= targetItemIndex)
                {
                    startIndex = i;
                    break;
                }
            }
            _mItemIndexMap.RemoveRange(startIndex, count - startIndex);
        }


        public void UpdateListViewPart1(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0, float distanceForNew1)
        {
            if (_mSupportScrollBar)
            {
                _mItemPosMgr.Update(false);
            }
            _mListUpdateCheckFrameCount = _mParentGridView.ListUpdateCheckFrameCount;
            bool needContinueCheck = true;
            int checkCount = 0;
            int maxCount = 9999;
            while (needContinueCheck)
            {
                checkCount++;
                if (checkCount >= maxCount)
                {
                    Debug.LogError("UpdateListViewPart1 while loop " + checkCount + " times! something is wrong!");
                    break;
                }
                if(_mIsVertList)
                {
                    needContinueCheck = UpdateForVertListPart1(distanceForRecycle0, distanceForRecycle1, distanceForNew0, distanceForNew1);
                }
                else
                {
                    needContinueCheck = UpdateForHorizontalListPart1(distanceForRecycle0, distanceForRecycle1, distanceForNew0, distanceForNew1);
                }
            }
            _mLastFrameContainerPos = _mContainerTrans.anchoredPosition3D;
        }


        public bool UpdateListViewPart2(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0, float distanceForNew1)
        {
            if (_mIsVertList)
            {
                return UpdateForVertListPart2(distanceForRecycle0, distanceForRecycle1, distanceForNew0, distanceForNew1);
            }
            else
            {
                return UpdateForHorizontalListPart2(distanceForRecycle0, distanceForRecycle1, distanceForNew0, distanceForNew1);
            }
        }


        public bool UpdateForVertListPart1(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0, float distanceForNew1)
        {
            if (_mItemTotalCount == 0)
            {
                if (_mItemList.Count > 0)
                {
                    RecycleAllItem();
                }
                return false;
            }
            if (_mArrangeType == ListItemArrangeType.TopToBottom)
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
                    if (_mSupportScrollBar)
                    {
                        bool succeed = GetPlusItemIndexAndPosAtGivenPos(curY, ref index, ref pos);
                        if (succeed == false)
                        {
                            return false;
                        }
                        pos = -pos;
                    }
                    LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(index);
                    if (newItem == null)
                    {
                        return false;
                    }
                    if (_mSupportScrollBar)
                    {
                        SetItemSize(index, newItem.CachedRectTransform.rect.height, newItem.Padding);
                    }
                    _mItemList.Add(newItem);
                    newItem.CachedRectTransform.anchoredPosition3D = new Vector3(newItem.StartPosOffset, pos, 0);
                    return true;
                }
                LoopStaggeredGridViewItem tViewItem0 = _mItemList[0];
                tViewItem0.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 topPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                Vector3 downPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[0]);

                if (!IsDraging && tViewItem0.ItemCreatedCheckFrameCount != _mListUpdateCheckFrameCount
                    && downPos0.y - _mViewPortRectLocalCorners[1].y > distanceForRecycle0)
                {
                    _mItemList.RemoveAt(0);
                    RecycleItemTmp(tViewItem0);
                    if (!_mSupportScrollBar)
                    {
                        CheckIfNeedUpdateItemPos();
                    }
                    return true;
                }

                LoopStaggeredGridViewItem tViewItem1 = _mItemList[_mItemList.Count - 1];
                tViewItem1.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 topPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                Vector3 downPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[0]);
                if (!IsDraging && tViewItem1.ItemCreatedCheckFrameCount != _mListUpdateCheckFrameCount
                    && _mViewPortRectLocalCorners[0].y - topPos1.y > distanceForRecycle1)
                {
                    _mItemList.RemoveAt(_mItemList.Count - 1);
                    RecycleItemTmp(tViewItem1);
                    if (!_mSupportScrollBar)
                    {
                        CheckIfNeedUpdateItemPos();
                    }
                    return true;
                }


                if (topPos0.y - _mViewPortRectLocalCorners[1].y < distanceForNew0)
                {
                    if (tViewItem0.ItemIndexInGroup < _mCurReadyMinItemIndex)
                    {
                        _mCurReadyMinItemIndex = tViewItem0.ItemIndexInGroup;
                        _mNeedCheckNextMinItem = true;
                    }
                    int nIndex = tViewItem0.ItemIndexInGroup - 1;
                    if (nIndex >= _mCurReadyMinItemIndex || _mNeedCheckNextMinItem)
                    {
                        LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(nIndex);
                        if (newItem == null)
                        {
                            _mCurReadyMinItemIndex = tViewItem0.ItemIndexInGroup;
                            _mNeedCheckNextMinItem = false;
                        }
                        else
                        {
                            if (_mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.height, newItem.Padding);
                            }
                            _mItemList.Insert(0, newItem);
                            float y = tViewItem0.CachedRectTransform.anchoredPosition3D.y + newItem.CachedRectTransform.rect.height + newItem.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(newItem.StartPosOffset, y, 0);
                            CheckIfNeedUpdateItemPos();
                            if (nIndex < _mCurReadyMinItemIndex)
                            {
                                _mCurReadyMinItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }

                if (_mViewPortRectLocalCorners[0].y - downPos1.y < distanceForNew1)
                {
                    if (tViewItem1.ItemIndexInGroup > _mCurReadyMaxItemIndex)
                    {
                        _mCurReadyMaxItemIndex = tViewItem1.ItemIndexInGroup;
                        _mNeedCheckNextMaxItem = true;
                    }
                    int nIndex = tViewItem1.ItemIndexInGroup + 1;
                    if(nIndex >= _mItemIndexMap.Count)
                    {
                        return false;
                    }
                    if (nIndex <= _mCurReadyMaxItemIndex || _mNeedCheckNextMaxItem)
                    {
                        LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(nIndex);
                        if (newItem == null)
                        {
                            _mCurReadyMaxItemIndex = tViewItem1.ItemIndexInGroup;
                            _mNeedCheckNextMaxItem = false;
                            CheckIfNeedUpdateItemPos();
                            return false;
                        }
                        else
                        {
                            if (_mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.height, newItem.Padding);
                            }
                            _mItemList.Add(newItem);
                            float y = tViewItem1.CachedRectTransform.anchoredPosition3D.y - tViewItem1.CachedRectTransform.rect.height - tViewItem1.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(newItem.StartPosOffset, y, 0);
                            CheckIfNeedUpdateItemPos();

                            if (nIndex > _mCurReadyMaxItemIndex)
                            {
                                _mCurReadyMaxItemIndex = nIndex;
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
                    if (_mSupportScrollBar)
                    {
                        bool succeed = GetPlusItemIndexAndPosAtGivenPos(-curY, ref index, ref pos);
                        if(succeed == false)
                        {
                            return false;
                        }
                    }
                    LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(index);
                    if (newItem == null)
                    {
                        return false;
                    }
                    if (_mSupportScrollBar)
                    {
                        SetItemSize(index, newItem.CachedRectTransform.rect.height, newItem.Padding);
                    }
                    _mItemList.Add(newItem);
                    newItem.CachedRectTransform.anchoredPosition3D = new Vector3(newItem.StartPosOffset, pos, 0);
                    return true;
                }
                LoopStaggeredGridViewItem tViewItem0 = _mItemList[0];
                tViewItem0.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 topPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                Vector3 downPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[0]);

                if (!IsDraging && tViewItem0.ItemCreatedCheckFrameCount != _mListUpdateCheckFrameCount
                    && _mViewPortRectLocalCorners[0].y - topPos0.y > distanceForRecycle0)
                {
                    _mItemList.RemoveAt(0);
                    RecycleItemTmp(tViewItem0);
                    if (!_mSupportScrollBar)
                    {
                        CheckIfNeedUpdateItemPos();
                    }
                    return true;
                }

                LoopStaggeredGridViewItem tViewItem1 = _mItemList[_mItemList.Count - 1];
                tViewItem1.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 topPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                Vector3 downPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[0]);
                if (!IsDraging && tViewItem1.ItemCreatedCheckFrameCount != _mListUpdateCheckFrameCount
                     && downPos1.y - _mViewPortRectLocalCorners[1].y > distanceForRecycle1)
                {
                    _mItemList.RemoveAt(_mItemList.Count - 1);
                    RecycleItemTmp(tViewItem1);
                    if (!_mSupportScrollBar)
                    {
                        CheckIfNeedUpdateItemPos();
                    }
                    return true;
                }


                if (_mViewPortRectLocalCorners[0].y - downPos0.y < distanceForNew0)
                {
                    if (tViewItem0.ItemIndexInGroup < _mCurReadyMinItemIndex)
                    {
                        _mCurReadyMinItemIndex = tViewItem0.ItemIndexInGroup;
                        _mNeedCheckNextMinItem = true;
                    }
                    int nIndex = tViewItem0.ItemIndexInGroup - 1;
                    if (nIndex >= _mCurReadyMinItemIndex || _mNeedCheckNextMinItem)
                    {
                        LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(nIndex);
                        if (newItem == null)
                        {
                            _mCurReadyMinItemIndex = tViewItem0.ItemIndexInGroup;
                            _mNeedCheckNextMinItem = false;
                        }
                        else
                        {
                            if (_mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.height, newItem.Padding);
                            }
                            _mItemList.Insert(0, newItem);
                            float y = tViewItem0.CachedRectTransform.anchoredPosition3D.y - newItem.CachedRectTransform.rect.height - newItem.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(newItem.StartPosOffset, y, 0);
                            CheckIfNeedUpdateItemPos();
                            if (nIndex < _mCurReadyMinItemIndex)
                            {
                                _mCurReadyMinItemIndex = nIndex;
                            }
                            return true;
                        }

                    }
                }

                if (topPos1.y - _mViewPortRectLocalCorners[1].y < distanceForNew1)
                {
                    if (tViewItem1.ItemIndexInGroup > _mCurReadyMaxItemIndex)
                    {
                        _mCurReadyMaxItemIndex = tViewItem1.ItemIndexInGroup;
                        _mNeedCheckNextMaxItem = true;
                    }
                    int nIndex = tViewItem1.ItemIndexInGroup + 1;
                    if (nIndex >= _mItemIndexMap.Count)
                    {
                        return false;
                    }

                    if (nIndex <= _mCurReadyMaxItemIndex || _mNeedCheckNextMaxItem)
                    {
                        LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(nIndex);
                        if (newItem == null)
                        {
                            _mCurReadyMaxItemIndex = tViewItem1.ItemIndexInGroup;
                            _mNeedCheckNextMaxItem = false;
                            CheckIfNeedUpdateItemPos();
                            return false;
                        }
                        else
                        {
                            if (_mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.height, newItem.Padding);
                            }
                            _mItemList.Add(newItem);
                            float y = tViewItem1.CachedRectTransform.anchoredPosition3D.y + tViewItem1.CachedRectTransform.rect.height + tViewItem1.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(newItem.StartPosOffset, y, 0);
                            CheckIfNeedUpdateItemPos();
                            if (nIndex > _mCurReadyMaxItemIndex)
                            {
                                _mCurReadyMaxItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }

            }

            return false;

        }


        public bool UpdateForVertListPart2(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0, float distanceForNew1)
        {
            if (_mItemTotalCount == 0)
            {
                if (_mItemList.Count > 0)
                {
                    RecycleAllItem();
                }
                return false;
            }
            if (_mArrangeType == ListItemArrangeType.TopToBottom)
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
                    if (_mSupportScrollBar)
                    {
                        bool succeed = GetPlusItemIndexAndPosAtGivenPos(curY, ref index, ref pos);
                        if (succeed == false)
                        {
                            return false;
                        }
                        pos = -pos;
                    }
                    LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(index);
                    if (newItem == null)
                    {
                        return false;
                    }
                    if (_mSupportScrollBar)
                    {
                        SetItemSize(index, newItem.CachedRectTransform.rect.height, newItem.Padding);
                    }
                    _mItemList.Add(newItem);
                    newItem.CachedRectTransform.anchoredPosition3D = new Vector3(newItem.StartPosOffset, pos, 0);
                    return true;
                }

                LoopStaggeredGridViewItem tViewItem1 = _mItemList[_mItemList.Count - 1];
                tViewItem1.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 downPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[0]);
                
                if (_mViewPortRectLocalCorners[0].y - downPos1.y < distanceForNew1)
                {
                    if (tViewItem1.ItemIndexInGroup > _mCurReadyMaxItemIndex)
                    {
                        _mCurReadyMaxItemIndex = tViewItem1.ItemIndexInGroup;
                        _mNeedCheckNextMaxItem = true;
                    }
                    int nIndex = tViewItem1.ItemIndexInGroup + 1;
                    if (nIndex <= _mCurReadyMaxItemIndex || _mNeedCheckNextMaxItem)
                    {
                        LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(nIndex);
                        if (newItem == null)
                        {
                            _mCurReadyMaxItemIndex = tViewItem1.ItemIndexInGroup;
                            _mNeedCheckNextMaxItem = false;
                            CheckIfNeedUpdateItemPos();
                            return false;
                        }
                        else
                        {
                            if (_mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.height, newItem.Padding);
                            }
                            _mItemList.Add(newItem);
                            float y = tViewItem1.CachedRectTransform.anchoredPosition3D.y - tViewItem1.CachedRectTransform.rect.height - tViewItem1.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(newItem.StartPosOffset, y, 0);
                            CheckIfNeedUpdateItemPos();

                            if (nIndex > _mCurReadyMaxItemIndex)
                            {
                                _mCurReadyMaxItemIndex = nIndex;
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
                    if (_mSupportScrollBar)
                    {
                        bool succeed = GetPlusItemIndexAndPosAtGivenPos(-curY, ref index, ref pos);
                        if(succeed == false)
                        {
                            return false;
                        }
                    }
                    LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(index);
                    if (newItem == null)
                    {
                        return false;
                    }
                    if (_mSupportScrollBar)
                    {
                        SetItemSize(index, newItem.CachedRectTransform.rect.height, newItem.Padding);
                    }
                    _mItemList.Add(newItem);
                    newItem.CachedRectTransform.anchoredPosition3D = new Vector3(newItem.StartPosOffset, pos, 0);
                    return true;
                }
               
                LoopStaggeredGridViewItem tViewItem1 = _mItemList[_mItemList.Count - 1];
                tViewItem1.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 topPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                
                if (topPos1.y - _mViewPortRectLocalCorners[1].y < distanceForNew1)
                {
                    if (tViewItem1.ItemIndexInGroup > _mCurReadyMaxItemIndex)
                    {
                        _mCurReadyMaxItemIndex = tViewItem1.ItemIndexInGroup;
                        _mNeedCheckNextMaxItem = true;
                    }
                    int nIndex = tViewItem1.ItemIndexInGroup + 1;
                    if (nIndex <= _mCurReadyMaxItemIndex || _mNeedCheckNextMaxItem)
                    {
                        LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(nIndex);
                        if (newItem == null)
                        {
                            _mCurReadyMaxItemIndex = tViewItem1.ItemIndexInGroup;
                            _mNeedCheckNextMaxItem = false;
                            CheckIfNeedUpdateItemPos();
                            return false;
                        }
                        else
                        {
                            if (_mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.height, newItem.Padding);
                            }
                            _mItemList.Add(newItem);
                            float y = tViewItem1.CachedRectTransform.anchoredPosition3D.y + tViewItem1.CachedRectTransform.rect.height + tViewItem1.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(newItem.StartPosOffset, y, 0);
                            CheckIfNeedUpdateItemPos();
                            if (nIndex > _mCurReadyMaxItemIndex)
                            {
                                _mCurReadyMaxItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }

            }

            return false;

        }




        public bool UpdateForHorizontalListPart1(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0, float distanceForNew1)
        {
            if (_mItemTotalCount == 0)
            {
                if (_mItemList.Count > 0)
                {
                    RecycleAllItem();
                }
                return false;
            }
            if (_mArrangeType == ListItemArrangeType.LeftToRight)
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
                    if (_mSupportScrollBar)
                    {
                        bool succeed = GetPlusItemIndexAndPosAtGivenPos(-curX, ref index, ref pos);
                        if (succeed == false)
                        {
                            return false;
                        }
                    }
                    LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(index);
                    if (newItem == null)
                    {
                        return false;
                    }
                    if (_mSupportScrollBar)
                    {
                        SetItemSize(index, newItem.CachedRectTransform.rect.width, newItem.Padding);
                    }
                    _mItemList.Add(newItem);
                    newItem.CachedRectTransform.anchoredPosition3D = new Vector3(pos, newItem.StartPosOffset, 0);
                    return true;
                }
                LoopStaggeredGridViewItem tViewItem0 = _mItemList[0];
                tViewItem0.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 leftPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                Vector3 rightPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[2]);

                if (!IsDraging && tViewItem0.ItemCreatedCheckFrameCount != _mListUpdateCheckFrameCount
                    && _mViewPortRectLocalCorners[1].x - rightPos0.x > distanceForRecycle0)
                {
                    _mItemList.RemoveAt(0);
                    RecycleItemTmp(tViewItem0);
                    if (!_mSupportScrollBar)
                    {
                        CheckIfNeedUpdateItemPos();
                    }
                    return true;
                }

                LoopStaggeredGridViewItem tViewItem1 = _mItemList[_mItemList.Count - 1];
                tViewItem1.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 leftPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                Vector3 rightPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[2]);
                if (!IsDraging && tViewItem1.ItemCreatedCheckFrameCount != _mListUpdateCheckFrameCount
                    && leftPos1.x - _mViewPortRectLocalCorners[2].x > distanceForRecycle1)
                {
                    _mItemList.RemoveAt(_mItemList.Count - 1);
                    RecycleItemTmp(tViewItem1);
                    if (!_mSupportScrollBar)
                    {
                        CheckIfNeedUpdateItemPos();
                    }
                    return true;
                }


                if (_mViewPortRectLocalCorners[1].x - leftPos0.x < distanceForNew0)
                {
                    if (tViewItem0.ItemIndexInGroup < _mCurReadyMinItemIndex)
                    {
                        _mCurReadyMinItemIndex = tViewItem0.ItemIndexInGroup;
                        _mNeedCheckNextMinItem = true;
                    }
                    int nIndex = tViewItem0.ItemIndexInGroup - 1;
                    if (nIndex >= _mCurReadyMinItemIndex || _mNeedCheckNextMinItem)
                    {
                        LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(nIndex);
                        if (newItem == null)
                        {
                            _mCurReadyMinItemIndex = tViewItem0.ItemIndexInGroup;
                            _mNeedCheckNextMinItem = false;
                        }
                        else
                        {
                            if (_mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.width, newItem.Padding);
                            }
                            _mItemList.Insert(0, newItem);
                            float x = tViewItem0.CachedRectTransform.anchoredPosition3D.x - newItem.CachedRectTransform.rect.width - newItem.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(x, newItem.StartPosOffset, 0);
                            CheckIfNeedUpdateItemPos();
                            if (nIndex < _mCurReadyMinItemIndex)
                            {
                                _mCurReadyMinItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }


                if (rightPos1.x - _mViewPortRectLocalCorners[2].x < distanceForNew1)
                {
                    if (tViewItem1.ItemIndexInGroup > _mCurReadyMaxItemIndex)
                    {
                        _mCurReadyMaxItemIndex = tViewItem1.ItemIndexInGroup;
                        _mNeedCheckNextMaxItem = true;
                    }
                    int nIndex = tViewItem1.ItemIndexInGroup + 1;
                    if (nIndex >= _mItemIndexMap.Count)
                    {
                        return false;
                    }
                    if (nIndex <= _mCurReadyMaxItemIndex || _mNeedCheckNextMaxItem)
                    {
                        LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(nIndex);
                        if (newItem == null)
                        {
                            _mCurReadyMaxItemIndex = tViewItem1.ItemIndexInGroup;
                            _mNeedCheckNextMaxItem = false;
                            CheckIfNeedUpdateItemPos();
                        }
                        else
                        {
                            if (_mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.width, newItem.Padding);
                            }
                            _mItemList.Add(newItem);
                            float x = tViewItem1.CachedRectTransform.anchoredPosition3D.x + tViewItem1.CachedRectTransform.rect.width + tViewItem1.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(x, newItem.StartPosOffset, 0);
                            CheckIfNeedUpdateItemPos();

                            if (nIndex > _mCurReadyMaxItemIndex)
                            {
                                _mCurReadyMaxItemIndex = nIndex;
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
                    if (_mSupportScrollBar)
                    {
                        bool succeed = GetPlusItemIndexAndPosAtGivenPos(curX, ref index, ref pos);
                        if (succeed == false)
                        {
                            return false;
                        }
                        pos = -pos;
                    }
                    LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(index);
                    if (newItem == null)
                    {
                        return false;
                    }
                    if (_mSupportScrollBar)
                    {
                        SetItemSize(index, newItem.CachedRectTransform.rect.width, newItem.Padding);
                    }
                    _mItemList.Add(newItem);
                    newItem.CachedRectTransform.anchoredPosition3D = new Vector3(pos, newItem.StartPosOffset, 0);
                    return true;
                }
                LoopStaggeredGridViewItem tViewItem0 = _mItemList[0];
                tViewItem0.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 leftPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                Vector3 rightPos0 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[2]);

                if (!IsDraging && tViewItem0.ItemCreatedCheckFrameCount != _mListUpdateCheckFrameCount
                    && leftPos0.x - _mViewPortRectLocalCorners[2].x > distanceForRecycle0)
                {
                    _mItemList.RemoveAt(0);
                    RecycleItemTmp(tViewItem0);
                    if (!_mSupportScrollBar)
                    {
                        CheckIfNeedUpdateItemPos();
                    }
                    return true;
                }

                LoopStaggeredGridViewItem tViewItem1 = _mItemList[_mItemList.Count - 1];
                tViewItem1.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 leftPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                Vector3 rightPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[2]);
                if (!IsDraging && tViewItem1.ItemCreatedCheckFrameCount != _mListUpdateCheckFrameCount
                    && _mViewPortRectLocalCorners[1].x - rightPos1.x > distanceForRecycle1)
                {
                    _mItemList.RemoveAt(_mItemList.Count - 1);
                    RecycleItemTmp(tViewItem1);
                    if (!_mSupportScrollBar)
                    {
                        CheckIfNeedUpdateItemPos();
                    }
                    return true;
                }


                if (rightPos0.x - _mViewPortRectLocalCorners[2].x < distanceForNew0)
                {
                    if (tViewItem0.ItemIndexInGroup < _mCurReadyMinItemIndex)
                    {
                        _mCurReadyMinItemIndex = tViewItem0.ItemIndexInGroup;
                        _mNeedCheckNextMinItem = true;
                    }
                    int nIndex = tViewItem0.ItemIndexInGroup - 1;
                    if (nIndex >= _mCurReadyMinItemIndex || _mNeedCheckNextMinItem)
                    {
                        LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(nIndex);
                        if (newItem == null)
                        {
                            _mCurReadyMinItemIndex = tViewItem0.ItemIndexInGroup;
                            _mNeedCheckNextMinItem = false;
                        }
                        else
                        {
                            if (_mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.width, newItem.Padding);
                            }
                            _mItemList.Insert(0, newItem);
                            float x = tViewItem0.CachedRectTransform.anchoredPosition3D.x + newItem.CachedRectTransform.rect.width + newItem.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(x, newItem.StartPosOffset, 0);
                            CheckIfNeedUpdateItemPos();
                            if (nIndex < _mCurReadyMinItemIndex)
                            {
                                _mCurReadyMinItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }


                if (_mViewPortRectLocalCorners[1].x - leftPos1.x < distanceForNew1)
                {
                    if (tViewItem1.ItemIndexInGroup > _mCurReadyMaxItemIndex)
                    {
                        _mCurReadyMaxItemIndex = tViewItem1.ItemIndexInGroup;
                        _mNeedCheckNextMaxItem = true;
                    }
                    int nIndex = tViewItem1.ItemIndexInGroup + 1;
                    if (nIndex >= _mItemIndexMap.Count)
                    {
                        return false;
                    }
                    if (nIndex <= _mCurReadyMaxItemIndex || _mNeedCheckNextMaxItem)
                    {
                        LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(nIndex);
                        if (newItem == null)
                        {
                            _mCurReadyMaxItemIndex = tViewItem1.ItemIndexInGroup;
                            _mNeedCheckNextMaxItem = false;
                            CheckIfNeedUpdateItemPos();
                        }
                        else
                        {
                            if (_mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.width, newItem.Padding);
                            }
                            _mItemList.Add(newItem);
                            float x = tViewItem1.CachedRectTransform.anchoredPosition3D.x - tViewItem1.CachedRectTransform.rect.width - tViewItem1.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(x, newItem.StartPosOffset, 0);
                            CheckIfNeedUpdateItemPos();

                            if (nIndex > _mCurReadyMaxItemIndex)
                            {
                                _mCurReadyMaxItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }


            }

            return false;

        }




        public bool UpdateForHorizontalListPart2(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0, float distanceForNew1)
        {
            if (_mItemTotalCount == 0)
            {
                if (_mItemList.Count > 0)
                {
                    RecycleAllItem();
                }
                return false;
            }
            if (_mArrangeType == ListItemArrangeType.LeftToRight)
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
                    if (_mSupportScrollBar)
                    {
                        bool succeed = GetPlusItemIndexAndPosAtGivenPos(-curX, ref index, ref pos);
                        if (succeed == false)
                        {
                            return false;
                        }
                    }
                    LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(index);
                    if (newItem == null)
                    {
                        return false;
                    }
                    if (_mSupportScrollBar)
                    {
                        SetItemSize(index, newItem.CachedRectTransform.rect.width, newItem.Padding);
                    }
                    _mItemList.Add(newItem);
                    newItem.CachedRectTransform.anchoredPosition3D = new Vector3(pos, newItem.StartPosOffset, 0);
                    return true;
                }

                LoopStaggeredGridViewItem tViewItem1 = _mItemList[_mItemList.Count - 1];
                tViewItem1.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 rightPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[2]);
               
                if (rightPos1.x - _mViewPortRectLocalCorners[2].x < distanceForNew1)
                {
                    if (tViewItem1.ItemIndexInGroup > _mCurReadyMaxItemIndex)
                    {
                        _mCurReadyMaxItemIndex = tViewItem1.ItemIndexInGroup;
                        _mNeedCheckNextMaxItem = true;
                    }
                    int nIndex = tViewItem1.ItemIndexInGroup + 1;
                    if (nIndex <= _mCurReadyMaxItemIndex || _mNeedCheckNextMaxItem)
                    {
                        LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(nIndex);
                        if (newItem == null)
                        {
                            _mCurReadyMaxItemIndex = tViewItem1.ItemIndexInGroup;
                            _mNeedCheckNextMaxItem = false;
                            CheckIfNeedUpdateItemPos();
                        }
                        else
                        {
                            if (_mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.width, newItem.Padding);
                            }
                            _mItemList.Add(newItem);
                            float x = tViewItem1.CachedRectTransform.anchoredPosition3D.x + tViewItem1.CachedRectTransform.rect.width + tViewItem1.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(x, newItem.StartPosOffset, 0);
                            CheckIfNeedUpdateItemPos();

                            if (nIndex > _mCurReadyMaxItemIndex)
                            {
                                _mCurReadyMaxItemIndex = nIndex;
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
                    if (_mSupportScrollBar)
                    {
                        bool succeed = GetPlusItemIndexAndPosAtGivenPos(curX, ref index, ref pos);
                        if (succeed == false)
                        {
                            return false;
                        }
                        pos = -pos;
                    }
                    LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(index);
                    if (newItem == null)
                    {
                        return false;
                    }
                    if (_mSupportScrollBar)
                    {
                        SetItemSize(index, newItem.CachedRectTransform.rect.width, newItem.Padding);
                    }
                    _mItemList.Add(newItem);
                    newItem.CachedRectTransform.anchoredPosition3D = new Vector3(pos, newItem.StartPosOffset, 0);
                    return true;
                }
                
                LoopStaggeredGridViewItem tViewItem1 = _mItemList[_mItemList.Count - 1];
                tViewItem1.CachedRectTransform.GetWorldCorners(_mItemWorldCorners);
                Vector3 leftPos1 = _mViewPortRectTransform.InverseTransformPoint(_mItemWorldCorners[1]);
                
                if (_mViewPortRectLocalCorners[1].x - leftPos1.x < distanceForNew1)
                {
                    if (tViewItem1.ItemIndexInGroup > _mCurReadyMaxItemIndex)
                    {
                        _mCurReadyMaxItemIndex = tViewItem1.ItemIndexInGroup;
                        _mNeedCheckNextMaxItem = true;
                    }
                    int nIndex = tViewItem1.ItemIndexInGroup + 1;
                    if (nIndex <= _mCurReadyMaxItemIndex || _mNeedCheckNextMaxItem)
                    {
                        LoopStaggeredGridViewItem newItem = GetNewItemByIndexInGroup(nIndex);
                        if (newItem == null)
                        {
                            _mCurReadyMaxItemIndex = tViewItem1.ItemIndexInGroup;
                            _mNeedCheckNextMaxItem = false;
                            CheckIfNeedUpdateItemPos();
                        }
                        else
                        {
                            if (_mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.CachedRectTransform.rect.width, newItem.Padding);
                            }
                            _mItemList.Add(newItem);
                            float x = tViewItem1.CachedRectTransform.anchoredPosition3D.x - tViewItem1.CachedRectTransform.rect.width - tViewItem1.Padding;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(x, newItem.StartPosOffset, 0);
                            CheckIfNeedUpdateItemPos();

                            if (nIndex > _mCurReadyMaxItemIndex)
                            {
                                _mCurReadyMaxItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }
            }

            return false;

        }




        public float GetContentPanelSize()
        {
            float tTotalSize = _mItemPosMgr.MTotalSize > 0 ? (_mItemPosMgr.MTotalSize - _mLastItemPadding) : 0;
            if (tTotalSize < 0)
            {
                tTotalSize = 0;
            }
            return tTotalSize;
        }


        public float GetShownItemPosMaxValue()
        {
            int count = _mItemList.Count;
            if (count == 0)
            {
                return 0f;
            }
            LoopStaggeredGridViewItem lastItem = _mItemList[_mItemList.Count - 1];
            if (_mArrangeType == ListItemArrangeType.TopToBottom)
            {
                return Mathf.Abs(lastItem.BottomY);
            }
            else if (_mArrangeType == ListItemArrangeType.BottomToTop)
            {
                return Mathf.Abs(lastItem.TopY);
            }
            else if (_mArrangeType == ListItemArrangeType.LeftToRight)
            {
                return Mathf.Abs(lastItem.RightX);
            }
            else if (_mArrangeType == ListItemArrangeType.RightToLeft)
            {
                return Mathf.Abs(lastItem.LeftX);
            }
            return 0f;
        }

        public void CheckIfNeedUpdateItemPos()
        {
            int count = _mItemList.Count;
            if (count == 0)
            {
                return;
            }
            if (_mArrangeType == ListItemArrangeType.TopToBottom)
            {
                LoopStaggeredGridViewItem firstItem = _mItemList[0];
                LoopStaggeredGridViewItem lastItem = _mItemList[_mItemList.Count - 1];
                if (firstItem.TopY > 0 || (firstItem.ItemIndexInGroup == _mCurReadyMinItemIndex && firstItem.TopY != 0))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
                float viewMaxY = GetContentPanelSize();
                if ((-lastItem.BottomY) > viewMaxY || (lastItem.ItemIndexInGroup == _mCurReadyMaxItemIndex && (-lastItem.BottomY) != viewMaxY))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
            }
            else if (_mArrangeType == ListItemArrangeType.BottomToTop)
            {
                LoopStaggeredGridViewItem firstItem = _mItemList[0];
                LoopStaggeredGridViewItem lastItem = _mItemList[_mItemList.Count - 1];
                if (firstItem.BottomY < 0 || (firstItem.ItemIndexInGroup == _mCurReadyMinItemIndex && firstItem.BottomY != 0))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
                float viewMaxY = GetContentPanelSize();
                if (lastItem.TopY > viewMaxY || (lastItem.ItemIndexInGroup == _mCurReadyMaxItemIndex && lastItem.TopY != viewMaxY))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
            }
            else if (_mArrangeType == ListItemArrangeType.LeftToRight)
            {
                LoopStaggeredGridViewItem firstItem = _mItemList[0];
                LoopStaggeredGridViewItem lastItem = _mItemList[_mItemList.Count - 1];
                if (firstItem.LeftX < 0 || (firstItem.ItemIndexInGroup == _mCurReadyMinItemIndex && firstItem.LeftX != 0))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
                float viewMaxX = GetContentPanelSize();
                if ((lastItem.RightX) > viewMaxX || (lastItem.ItemIndexInGroup == _mCurReadyMaxItemIndex && lastItem.RightX != viewMaxX))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
            }
            else if (_mArrangeType == ListItemArrangeType.RightToLeft)
            {
                LoopStaggeredGridViewItem firstItem = _mItemList[0];
                LoopStaggeredGridViewItem lastItem = _mItemList[_mItemList.Count - 1];
                if (firstItem.RightX > 0 || (firstItem.ItemIndexInGroup == _mCurReadyMinItemIndex && firstItem.RightX != 0))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
                float viewMaxX = GetContentPanelSize();
                if ((-lastItem.LeftX) > viewMaxX || (lastItem.ItemIndexInGroup == _mCurReadyMaxItemIndex && (-lastItem.LeftX) != viewMaxX))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
            }

        }


        public void UpdateAllShownItemsPos()
        {
            int count = _mItemList.Count;
            if (count == 0)
            {
                return;
            }

            if (_mArrangeType == ListItemArrangeType.TopToBottom)
            {
                float pos = 0;
                if (_mSupportScrollBar)
                {
                    pos = -GetItemPos(_mItemList[0].ItemIndexInGroup);
                }
                float curY = pos;
                for (int i = 0; i < count; ++i)
                {
                    LoopStaggeredGridViewItem item = _mItemList[i];
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(item.StartPosOffset, curY, 0);
                    curY = curY - item.CachedRectTransform.rect.height - item.Padding;
                }
            }
            else if (_mArrangeType == ListItemArrangeType.BottomToTop)
            {
                float pos = 0;
                if (_mSupportScrollBar)
                {
                    pos = GetItemPos(_mItemList[0].ItemIndexInGroup);
                }
                float curY = pos;
                for (int i = 0; i < count; ++i)
                {
                    LoopStaggeredGridViewItem item = _mItemList[i];
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(item.StartPosOffset, curY, 0);
                    curY = curY + item.CachedRectTransform.rect.height + item.Padding;
                }
            }
            else if (_mArrangeType == ListItemArrangeType.LeftToRight)
            {
                float pos = 0;
                if (_mSupportScrollBar)
                {
                    pos = GetItemPos(_mItemList[0].ItemIndexInGroup);
                }
                float curX = pos;
                for (int i = 0; i < count; ++i)
                {
                    LoopStaggeredGridViewItem item = _mItemList[i];
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(curX, item.StartPosOffset, 0);
                    curX = curX + item.CachedRectTransform.rect.width + item.Padding;
                }
            }
            else if (_mArrangeType == ListItemArrangeType.RightToLeft)
            {
                float pos = 0;
                if (_mSupportScrollBar)
                {
                    pos = -GetItemPos(_mItemList[0].ItemIndexInGroup);
                }
                float curX = pos;
                for (int i = 0; i < count; ++i)
                {
                    LoopStaggeredGridViewItem item = _mItemList[i];
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(curX, item.StartPosOffset, 0);
                    curX = curX - item.CachedRectTransform.rect.width - item.Padding;
                }
            }
        }
    }



}
