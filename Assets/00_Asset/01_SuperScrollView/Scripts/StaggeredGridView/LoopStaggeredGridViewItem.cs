using _00_Asset._01_SuperScrollView.Scripts.Common;
using UnityEngine;

namespace _00_Asset._01_SuperScrollView.Scripts.StaggeredGridView
{

    public class LoopStaggeredGridViewItem : MonoBehaviour
    {
        //indicates the item’s index in the GridView,
        // can be from 0 to itemTotalCount -1.
        int _mItemIndex = -1;

        //indicates the item’s index in a column or row. 
        //Here the word “Group” means: for an vertical GridView, a group means a column
        //for an horizontal GridView, a group means a row.
        int _mItemIndexInGroup = -1;

        //indicates the item’s id.
        //This property is set when the item is created or fetched from pool, 
        //and will no longer change until the item is recycled back to pool.
        int _mItemId = -1;

        float _mPadding;
        float _mExtraPadding;
        bool _mIsInitHandlerCalled = false;
        string _mItemPrefabName;
        RectTransform _mCachedRectTransform;
        LoopStaggeredGridView _mParentListView = null;
        float _mDistanceWithViewPortSnapCenter = 0;
        int _mItemCreatedCheckFrameCount = 0;
        float _mStartPosOffset = 0;

        object _mUserObjectData = null;
        int _mUserIntData1 = 0;
        int _mUserIntData2 = 0;
        string _mUserStringData1 = null;
        string _mUserStringData2 = null;

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

        public float DistanceWithViewPortSnapCenter
        {
            get { return _mDistanceWithViewPortSnapCenter; }
            set { _mDistanceWithViewPortSnapCenter = value; }
        }

        public float StartPosOffset
        {
            get { return _mStartPosOffset; }
            set { _mStartPosOffset = value; }
        }

        public int ItemCreatedCheckFrameCount
        {
            get { return _mItemCreatedCheckFrameCount; }
            set { _mItemCreatedCheckFrameCount = value; }
        }

        public float Padding
        {
            get { return _mPadding; }
            set { _mPadding = value; }
        }
        public float ExtraPadding
        {
            get { return _mExtraPadding; }
            set { _mExtraPadding = value; }
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

        public int ItemIndexInGroup
        {
            get
            {
                return _mItemIndexInGroup;
            }
            set
            {
                _mItemIndexInGroup = value;
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

        public LoopStaggeredGridView ParentListView
        {
            get
            {
                return _mParentListView;
            }
            set
            {
                _mParentListView = value;
            }
        }

        public float TopY
        {
            get
            {
                ListItemArrangeType arrageType = ParentListView.ArrangeType;
                if (arrageType == ListItemArrangeType.TopToBottom)
                {
                    return CachedRectTransform.anchoredPosition3D.y;
                }
                else if (arrageType == ListItemArrangeType.BottomToTop)
                {
                    return CachedRectTransform.anchoredPosition3D.y + CachedRectTransform.rect.height;
                }
                return 0;
            }
        }

        public float BottomY
        {
            get
            {
                ListItemArrangeType arrageType = ParentListView.ArrangeType;
                if (arrageType == ListItemArrangeType.TopToBottom)
                {
                    return CachedRectTransform.anchoredPosition3D.y - CachedRectTransform.rect.height;
                }
                else if (arrageType == ListItemArrangeType.BottomToTop)
                {
                    return CachedRectTransform.anchoredPosition3D.y;
                }
                return 0;
            }
        }


        public float LeftX
        {
            get
            {
                ListItemArrangeType arrageType = ParentListView.ArrangeType;
                if (arrageType == ListItemArrangeType.LeftToRight)
                {
                    return CachedRectTransform.anchoredPosition3D.x;
                }
                else if (arrageType == ListItemArrangeType.RightToLeft)
                {
                    return CachedRectTransform.anchoredPosition3D.x - CachedRectTransform.rect.width;
                }
                return 0;
            }
        }

        public float RightX
        {
            get
            {
                ListItemArrangeType arrageType = ParentListView.ArrangeType;
                if (arrageType == ListItemArrangeType.LeftToRight)
                {
                    return CachedRectTransform.anchoredPosition3D.x + CachedRectTransform.rect.width;
                }
                else if (arrageType == ListItemArrangeType.RightToLeft)
                {
                    return CachedRectTransform.anchoredPosition3D.x;
                }
                return 0;
            }
        }

        public float ItemSize
        {
            get
            {
                if (ParentListView.IsVertList)
                {
                    return CachedRectTransform.rect.height;
                }
                else
                {
                    return CachedRectTransform.rect.width;
                }
            }
        }

        public float ItemSizeWithPadding
        {
            get
            {
                return ItemSize + _mPadding;
            }
        }

    }
}
