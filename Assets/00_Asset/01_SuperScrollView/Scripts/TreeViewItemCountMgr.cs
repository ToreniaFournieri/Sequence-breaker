using System.Collections.Generic;

namespace _00_Asset._01_SuperScrollView.Scripts
{
    public class TreeViewItemCountData
    {
        public int MTreeItemIndex = 0;
        public int MChildCount = 0;
        public bool MIsExpand = true;
        public int MBeginIndex = 0;
        public int MEndIndex = 0;

        public bool IsChild(int index)
        {
            return (index != MBeginIndex);
        }

        public int GetChildIndex(int index)
        {
            if(IsChild(index) == false)
            {
                return -1;
            }
            return (index - MBeginIndex - 1);
        }

    }
    public class TreeViewItemCountMgr
    {

        List<TreeViewItemCountData> _mTreeItemDataList = new List<TreeViewItemCountData>();
        TreeViewItemCountData _mLastQueryResult = null;
        bool _mIsDirty = true;
        public void AddTreeItem(int count, bool isExpand)
        {
            TreeViewItemCountData data = new TreeViewItemCountData();
            data.MTreeItemIndex = _mTreeItemDataList.Count;
            data.MChildCount = count;
            data.MIsExpand = isExpand;
            _mTreeItemDataList.Add(data);
            _mIsDirty = true;
        }

        public void Clear()
        {
            _mTreeItemDataList.Clear();
            _mLastQueryResult = null;
            _mIsDirty = true;
        }

        public TreeViewItemCountData GetTreeItem(int treeIndex)
        {
            if (treeIndex < 0 || treeIndex >= _mTreeItemDataList.Count)
            {
                return null;
            }
            return _mTreeItemDataList[treeIndex];
        }
        public void SetItemChildCount(int treeIndex, int count)
        {
            if (treeIndex < 0 || treeIndex >= _mTreeItemDataList.Count)
            {
                return;
            }
            _mIsDirty = true;
            TreeViewItemCountData data = _mTreeItemDataList[treeIndex];
            data.MChildCount = count;
        }
        public void SetItemExpand(int treeIndex, bool isExpand)
        {
            if (treeIndex < 0 || treeIndex >= _mTreeItemDataList.Count)
            {
                return;
            }
            _mIsDirty = true;
            TreeViewItemCountData data = _mTreeItemDataList[treeIndex];
            data.MIsExpand = isExpand;
        }

        public void ToggleItemExpand(int treeIndex)
        {
            if (treeIndex < 0 || treeIndex >= _mTreeItemDataList.Count)
            {
                return;
            }
            _mIsDirty = true;
            TreeViewItemCountData data = _mTreeItemDataList[treeIndex];
            data.MIsExpand = !data.MIsExpand;
        }

        public bool IsTreeItemExpand(int treeIndex)
        {
            TreeViewItemCountData data = GetTreeItem(treeIndex);
            if (data == null)
            {
                return false;
            }
            return data.MIsExpand;
        }

        void UpdateAllTreeItemDataIndex()
        {
            if (_mIsDirty == false)
            {
                return;
            }
            _mLastQueryResult = null;
            _mIsDirty = false;
            int count = _mTreeItemDataList.Count;
            if (count == 0)
            {
                return;
            }
            TreeViewItemCountData data0 = _mTreeItemDataList[0];
            data0.MBeginIndex = 0;
            data0.MEndIndex = (data0.MIsExpand ? data0.MChildCount : 0);
            int curEnd = data0.MEndIndex;
            for (int i = 1; i < count; ++i)
            {
                TreeViewItemCountData data = _mTreeItemDataList[i];
                data.MBeginIndex = curEnd + 1;
                data.MEndIndex = data.MBeginIndex + (data.MIsExpand ? data.MChildCount : 0);
                curEnd = data.MEndIndex;
            }
        }

        public int TreeViewItemCount
        {
            get
            {
                return _mTreeItemDataList.Count;
            }
        }

        public int GetTotalItemAndChildCount()
        {
            int count = _mTreeItemDataList.Count;
            if (count == 0)
            {
                return 0;
            }
            UpdateAllTreeItemDataIndex();
            return _mTreeItemDataList[count - 1].MEndIndex + 1;
        }
        public TreeViewItemCountData QueryTreeItemByTotalIndex(int totalIndex)
        {
            if (totalIndex < 0)
            {
                return null;
            }
            int count = _mTreeItemDataList.Count;
            if (count == 0)
            {
                return null;
            }
            UpdateAllTreeItemDataIndex();
            if (_mLastQueryResult != null)
            {
                if (_mLastQueryResult.MBeginIndex <= totalIndex && _mLastQueryResult.MEndIndex >= totalIndex)
                {
                    return _mLastQueryResult;
                }
            }
            int low = 0;
            int high = count - 1;
            TreeViewItemCountData data = null;
            while (low <= high)
            {
                int mid = (low + high) / 2;
                data = _mTreeItemDataList[mid];
                if (data.MBeginIndex <= totalIndex && data.MEndIndex >= totalIndex)
                {
                    _mLastQueryResult = data;
                    return data;
                }
                else if (totalIndex > data.MEndIndex)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }
            return null;
        }

    }

}
