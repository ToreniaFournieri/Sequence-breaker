using UnityEngine;

namespace SuperScrollView
{

    public class LoopGridViewItem : MonoBehaviour
    {
        // indicates the item’s index in the list the mItemIndex can only be from 0 to itemTotalCount -1.
        int _mItemIndex = -1;
        // the row index, the item is in. starting from 0.
        int _mRow = -1;
        // the column index, the item is in. starting from 0.
        int _mColumn = -1;
        //indicates the item’s id. 
        //This property is set when the item is created or fetched from pool, 
        //and will no longer change until the item is recycled back to pool.
        int _mItemId = -1;
        LoopGridView _mParentGridView = null;
        bool _mIsInitHandlerCalled = false;
        string _mItemPrefabName;
        RectTransform _mCachedRectTransform;
        int _mItemCreatedCheckFrameCount = 0;

        object _mUserObjectData = null;
        int _mUserIntData1 = 0;
        int _mUserIntData2 = 0;
        string _mUserStringData1 = null;
        string _mUserStringData2 = null;

        LoopGridViewItem _mPrevItem;
        LoopGridViewItem _mNextItem;

        public object UserObjectData
        {
            get { return _mUserObjectData; }
            set { _mUserObjectData = value; }
        }
        public int UserIntData1
        {
            get { return _mUserIntData1; }
            set { _mUserIntData1 = value; }
        }
        public int UserIntData2
        {
            get { return _mUserIntData2; }
            set { _mUserIntData2 = value; }
        }
        public string UserStringData1
        {
            get { return _mUserStringData1; }
            set { _mUserStringData1 = value; }
        }
        public string UserStringData2
        {
            get { return _mUserStringData2; }
            set { _mUserStringData2 = value; }
        }

        public int ItemCreatedCheckFrameCount
        {
            get { return _mItemCreatedCheckFrameCount; }
            set { _mItemCreatedCheckFrameCount = value; }
        }


        public RectTransform CachedRectTransform
        {
            get
            {
                if (_mCachedRectTransform == null)
                {
                    _mCachedRectTransform = gameObject.GetComponent<RectTransform>();
                }
                return _mCachedRectTransform;
            }
        }

        public string ItemPrefabName
        {
            get
            {
                return _mItemPrefabName;
            }
            set
            {
                _mItemPrefabName = value;
            }
        }

        public int Row
        {
            get
            {
                return _mRow;
            }
            set
            {
                _mRow = value;
            }
        }
        public int Column
        {
            get
            {
                return _mColumn;
            }
            set
            {
                _mColumn = value;
            }
        }

        public int ItemIndex
        {
            get
            {
                return _mItemIndex;
            }
            set
            {
                _mItemIndex = value;
            }
        }
        public int ItemId
        {
            get
            {
                return _mItemId;
            }
            set
            {
                _mItemId = value;
            }
        }


        public bool IsInitHandlerCalled
        {
            get
            {
                return _mIsInitHandlerCalled;
            }
            set
            {
                _mIsInitHandlerCalled = value;
            }
        }

        public LoopGridView ParentGridView
        {
            get
            {
                return _mParentGridView;
            }
            set
            {
                _mParentGridView = value;
            }
        }

        public LoopGridViewItem PrevItem
        {
            get { return _mPrevItem; }
            set { _mPrevItem = value; }
        }
        public LoopGridViewItem NextItem
        {
            get { return _mNextItem; }
            set { _mNextItem = value; }
        }

    }
}
