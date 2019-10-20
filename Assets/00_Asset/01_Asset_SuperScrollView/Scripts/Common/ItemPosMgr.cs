using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SuperScrollView
{

    public class ItemSizeGroup
    {

        public float[] MItemSizeArray = null;
        public float[] MItemStartPosArray = null;
        public int MItemCount = 0;
        int _mDirtyBeginIndex = ItemPosMgr.MItemMaxCountPerGroup;
        public float MGroupSize = 0;
        public float MGroupStartPos = 0;
        public float MGroupEndPos = 0;
        public int MGroupIndex = 0;
        float _mItemDefaultSize = 0;
        int _mMaxNoZeroIndex = 0;
        public ItemSizeGroup(int index,float itemDefaultSize)
        {
            MGroupIndex = index;
            _mItemDefaultSize = itemDefaultSize;
            Init();
        }

        public void Init()
        {
            MItemSizeArray = new float[ItemPosMgr.MItemMaxCountPerGroup];
            if (_mItemDefaultSize != 0)
            {
                for (int i = 0; i < MItemSizeArray.Length; ++i)
                {
                    MItemSizeArray[i] = _mItemDefaultSize;
                }
            }
            MItemStartPosArray = new float[ItemPosMgr.MItemMaxCountPerGroup];
            MItemStartPosArray[0] = 0;
            MItemCount = ItemPosMgr.MItemMaxCountPerGroup;
            MGroupSize = _mItemDefaultSize * MItemSizeArray.Length;
            if (_mItemDefaultSize != 0)
            {
                _mDirtyBeginIndex = 0;
            }
            else
            {
                _mDirtyBeginIndex = ItemPosMgr.MItemMaxCountPerGroup;
            }
        }

        public float GetItemStartPos(int index)
        {
            return MGroupStartPos + MItemStartPosArray[index];
        }

        public bool IsDirty
        {
            get
            {
                return (_mDirtyBeginIndex < MItemCount);
            }
        }
        public float SetItemSize(int index, float size)
        {
            if(index > _mMaxNoZeroIndex && size > 0)
            {
                _mMaxNoZeroIndex = index;
            }
            float old = MItemSizeArray[index];
            if (old == size)
            {
                return 0;
            }
            MItemSizeArray[index] = size;
            if (index < _mDirtyBeginIndex)
            {
                _mDirtyBeginIndex = index;
            }
            float ds = size - old;
            MGroupSize = MGroupSize + ds;
            return ds;
        }

        public void SetItemCount(int count)
        {
            if(count < _mMaxNoZeroIndex)
            {
                _mMaxNoZeroIndex = count;
            }
            if (MItemCount == count)
            {
                return;
            }
            MItemCount = count;
            RecalcGroupSize();
        }

        public void RecalcGroupSize()
        {
            MGroupSize = 0;
            for (int i = 0; i < MItemCount; ++i)
            {
                MGroupSize += MItemSizeArray[i];
            }
        }

        public int GetItemIndexByPos(float pos)
        {
            if (MItemCount == 0)
            {
                return -1;
            }
            
            int low = 0;
            int high = MItemCount - 1;
            if (_mItemDefaultSize == 0f)
            {
                if(_mMaxNoZeroIndex < 0)
                {
                    _mMaxNoZeroIndex = 0;
                }
                high = _mMaxNoZeroIndex;
            }
            while (low <= high)
            {
                int mid = (low + high) / 2;
                float startPos = MItemStartPosArray[mid];
                float endPos = startPos + MItemSizeArray[mid];
                if (startPos <= pos && endPos >= pos)
                {
                    return mid;
                }
                else if (pos > endPos)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }
            return -1;
        }

        public void UpdateAllItemStartPos()
        {
            if (_mDirtyBeginIndex >= MItemCount)
            {
                return;
            }
            int startIndex = (_mDirtyBeginIndex < 1) ? 1 : _mDirtyBeginIndex;
            for (int i = startIndex; i < MItemCount; ++i)
            {
                MItemStartPosArray[i] = MItemStartPosArray[i - 1] + MItemSizeArray[i - 1];
            }
            _mDirtyBeginIndex = MItemCount;
        }

        public void ClearOldData()
        {
            for (int i = MItemCount; i < ItemPosMgr.MItemMaxCountPerGroup; ++i)
            {
                MItemSizeArray[i] = 0;
            }
        }
    }

    public class ItemPosMgr
    {
        public const int MItemMaxCountPerGroup = 100;
        List<ItemSizeGroup> _mItemSizeGroupList = new List<ItemSizeGroup>();
        int _mDirtyBeginIndex = int.MaxValue;
        public float MTotalSize = 0;
        public float MItemDefaultSize = 20;
        int _mMaxNotEmptyGroupIndex = 0;

        public ItemPosMgr(float itemDefaultSize)
        {
            MItemDefaultSize = itemDefaultSize;
        }

        public void SetItemMaxCount(int maxCount)
        {
            _mDirtyBeginIndex = 0;
            MTotalSize = 0;
            int st = maxCount % MItemMaxCountPerGroup;
            int lastGroupItemCount = st;
            int needMaxGroupCount = maxCount / MItemMaxCountPerGroup;
            if (st > 0)
            {
                needMaxGroupCount++;
            }
            else
            {
                lastGroupItemCount = MItemMaxCountPerGroup;
            }
            int count = _mItemSizeGroupList.Count;
            if (count > needMaxGroupCount)
            {
                int d = count - needMaxGroupCount;
                _mItemSizeGroupList.RemoveRange(needMaxGroupCount, d);
            }
            else if (count < needMaxGroupCount)
            {
                if(count > 0)
                {
                    _mItemSizeGroupList[count - 1].ClearOldData();
                }
                int d = needMaxGroupCount - count;
                for (int i = 0; i < d; ++i)
                {
                    ItemSizeGroup tGroup = new ItemSizeGroup(count + i, MItemDefaultSize);
                    _mItemSizeGroupList.Add(tGroup);
                }
            }
            else
            {
                if (count > 0)
                {
                    _mItemSizeGroupList[count - 1].ClearOldData();
                }
            }
            count = _mItemSizeGroupList.Count;
            if((count-1) < _mMaxNotEmptyGroupIndex)
            {
                _mMaxNotEmptyGroupIndex = count - 1;
            }
            if(_mMaxNotEmptyGroupIndex < 0)
            {
                _mMaxNotEmptyGroupIndex = 0;
            }
            if (count == 0)
            {
                return;
            }
            for (int i = 0; i < count - 1; ++i)
            {
                _mItemSizeGroupList[i].SetItemCount(MItemMaxCountPerGroup);
            }
            _mItemSizeGroupList[count - 1].SetItemCount(lastGroupItemCount);
            for (int i = 0; i < count; ++i)
            {
                MTotalSize = MTotalSize + _mItemSizeGroupList[i].MGroupSize;
            }

        }

        public void SetItemSize(int itemIndex, float size)
        {
            int groupIndex = itemIndex / MItemMaxCountPerGroup;
            int indexInGroup = itemIndex % MItemMaxCountPerGroup;
            ItemSizeGroup tGroup = _mItemSizeGroupList[groupIndex];
            float changedSize = tGroup.SetItemSize(indexInGroup, size);
            if (changedSize != 0f)
            {
                if (groupIndex < _mDirtyBeginIndex)
                {
                    _mDirtyBeginIndex = groupIndex;
                }
            }
            MTotalSize += changedSize;
            if(groupIndex > _mMaxNotEmptyGroupIndex && size > 0)
            {
                _mMaxNotEmptyGroupIndex = groupIndex;
            }
        }

        public float GetItemPos(int itemIndex)
        {
            Update(true);
            int groupIndex = itemIndex / MItemMaxCountPerGroup;
            int indexInGroup = itemIndex % MItemMaxCountPerGroup;
            return _mItemSizeGroupList[groupIndex].GetItemStartPos(indexInGroup);
        }

        public bool GetItemIndexAndPosAtGivenPos(float pos, ref int index, ref float itemPos)
        {
            Update(true);
            index = 0;
            itemPos = 0f;
            int count = _mItemSizeGroupList.Count;
            if (count == 0)
            {
                return true;
            }
            ItemSizeGroup hitGroup = null;

            int low = 0;
            int high = count - 1;

            if (MItemDefaultSize == 0f)
            {
                if(_mMaxNotEmptyGroupIndex < 0)
                {
                    _mMaxNotEmptyGroupIndex = 0;
                }
                high = _mMaxNotEmptyGroupIndex;
            }
            while (low <= high)
            {
                int mid = (low + high) / 2;
                ItemSizeGroup tGroup = _mItemSizeGroupList[mid];
                if (tGroup.MGroupStartPos <= pos && tGroup.MGroupEndPos >= pos)
                {
                    hitGroup = tGroup;
                    break;
                }
                else if (pos > tGroup.MGroupEndPos)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }
            int hitIndex = -1;
            if (hitGroup != null)
            {
                hitIndex = hitGroup.GetItemIndexByPos(pos - hitGroup.MGroupStartPos);
            }
            else
            {
                return false;
            }
            if (hitIndex < 0)
            {
                return false;
            }
            index = hitIndex + hitGroup.MGroupIndex * MItemMaxCountPerGroup;
            itemPos = hitGroup.GetItemStartPos(hitIndex);
            return true;
        }

        public void Update(bool updateAll)
        {
            int count = _mItemSizeGroupList.Count;
            if (count == 0)
            {
                return;
            }
            if (_mDirtyBeginIndex >= count)
            {
                return;
            }
            int loopCount = 0;
            for (int i = _mDirtyBeginIndex; i < count; ++i)
            {
                loopCount++;
                ItemSizeGroup tGroup = _mItemSizeGroupList[i];
                _mDirtyBeginIndex++;
                tGroup.UpdateAllItemStartPos();
                if (i == 0)
                {
                    tGroup.MGroupStartPos = 0;
                    tGroup.MGroupEndPos = tGroup.MGroupSize;
                }
                else
                {
                    tGroup.MGroupStartPos = _mItemSizeGroupList[i - 1].MGroupEndPos;
                    tGroup.MGroupEndPos = tGroup.MGroupStartPos + tGroup.MGroupSize;
                }
                if (!updateAll && loopCount > 1)
                {
                    return;
                }

            }
        }

    }
}