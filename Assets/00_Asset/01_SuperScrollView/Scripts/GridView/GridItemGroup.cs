namespace _00_Asset._01_SuperScrollView.Scripts.GridView
{
    //if GridFixedType is GridFixedType.ColumnCountFixed, then the GridItemGroup is one row of the gridview
    //if GridFixedType is GridFixedType.RowCountFixed, then the GridItemGroup is one column of the gridview
    public class GridItemGroup
    {
        int _mCount = 0;
        int _mGroupIndex = -1;//the row index or the column index of this group
        LoopGridViewItem _mFirst = null;
        LoopGridViewItem _mLast = null;
        public int Count
        {
            get { return _mCount; }
        }

        public LoopGridViewItem First
        {
            get { return _mFirst; }
        }

        public LoopGridViewItem Last
        {
            get { return _mLast; }
        }

        public int GroupIndex
        {
            get { return _mGroupIndex; }
            set { _mGroupIndex = value; }
        }


        public LoopGridViewItem GetItemByColumn(int column)
        {
            LoopGridViewItem cur = _mFirst;
            while(cur != null)
            {
                if(cur.Column == column)
                {
                    return cur;
                }
                cur = cur.NextItem;
            }
            return null;
        }
        public LoopGridViewItem GetItemByRow(int row)
        {
            LoopGridViewItem cur = _mFirst;
            while (cur != null)
            {
                if (cur.Row == row)
                {
                    return cur;
                }
                cur = cur.NextItem;
            }
            return null;
        }


        public void ReplaceItem(LoopGridViewItem curItem,LoopGridViewItem newItem)
        {
            newItem.PrevItem = curItem.PrevItem;
            newItem.NextItem = curItem.NextItem;
            if(newItem.PrevItem != null)
            {
                newItem.PrevItem.NextItem = newItem;
            }
            if(newItem.NextItem != null)
            {
                newItem.NextItem.PrevItem = newItem;
            }
            if(_mFirst == curItem)
            {
                _mFirst = newItem;
            }
            if(_mLast == curItem)
            {
                _mLast = newItem;
            }
        }

        public void AddFirst(LoopGridViewItem newItem)
        {
            newItem.PrevItem = null;
            newItem.NextItem = null;
            if (_mFirst == null)
            {
                _mFirst = newItem;
                _mLast = newItem;
                _mFirst.PrevItem = null;
                _mFirst.NextItem = null;
                _mCount++;
            }
            else
            {
                _mFirst.PrevItem = newItem;
                newItem.PrevItem = null;
                newItem.NextItem = _mFirst;
                _mFirst = newItem;
                _mCount++;
            }
        }

        public void AddLast(LoopGridViewItem newItem)
        {
            newItem.PrevItem = null;
            newItem.NextItem = null;
            if (_mFirst == null)
            {
                _mFirst = newItem;
                _mLast = newItem;
                _mFirst.PrevItem = null;
                _mFirst.NextItem = null;
                _mCount++;
            }
            else
            {
                _mLast.NextItem = newItem;
                newItem.PrevItem = _mLast;
                newItem.NextItem = null;
                _mLast = newItem;
                _mCount++;
            }
        }

        public LoopGridViewItem RemoveFirst()
        {
            LoopGridViewItem ret = _mFirst;
            if (_mFirst == null)
            {
                return ret;
            }
            if(_mFirst == _mLast)
            {
                _mFirst = null;
                _mLast = null;
                --_mCount;
                return ret;
            }
            _mFirst = _mFirst.NextItem;
            _mFirst.PrevItem = null;
            --_mCount;
            return ret;
        }
        public LoopGridViewItem RemoveLast()
        {
            LoopGridViewItem ret = _mLast;
            if (_mFirst == null)
            {
                return ret;
            }
            if (_mFirst == _mLast)
            {
                _mFirst = null;
                _mLast = null;
                --_mCount;
                return ret;
            }
            _mLast = _mLast.PrevItem;
            _mLast.NextItem = null;
            --_mCount;
            return ret;
        }


        public void Clear()
        {
            LoopGridViewItem current = _mFirst;
            while (current != null)
            {
                current.PrevItem = null;
                current.NextItem = null;
                current = current.NextItem;
            }
            _mFirst = null;
            _mLast = null;
            _mCount = 0;
        }

    }
}
