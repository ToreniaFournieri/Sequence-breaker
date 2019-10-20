using System.Collections.Generic;
using UnityEngine;

namespace _00_Asset._01_Asset_SuperScrollView.Scripts.ListView
{
    public class ItemPool
    {
        GameObject _mPrefabObj;
        string _mPrefabName;
        int _mInitCreateCount = 1;
        float _mPadding = 0;
        float _mStartPosOffset = 0;
        List<LoopListViewItem2> _mTmpPooledItemList = new List<LoopListViewItem2>();
        List<LoopListViewItem2> _mPooledItemList = new List<LoopListViewItem2>();
        static int _mCurItemIdCount = 0;
        RectTransform _mItemParent = null;
        public ItemPool()
        {

        }
        public void Init(GameObject prefabObj, float padding, float startPosOffset, int createCount, RectTransform parent)
        {
            _mPrefabObj = prefabObj;
            _mPrefabName = _mPrefabObj.name;
            _mInitCreateCount = createCount;
            _mPadding = padding;
            _mStartPosOffset = startPosOffset;
            _mItemParent = parent;
            _mPrefabObj.SetActive(false);
            for (int i = 0; i < _mInitCreateCount; ++i)
            {
                LoopListViewItem2 tViewItem = CreateItem();
                RecycleItemReal(tViewItem);
            }
        }
        public LoopListViewItem2 GetItem()
        {
            _mCurItemIdCount++;
            LoopListViewItem2 tItem = null;
            if (_mTmpPooledItemList.Count > 0)
            {
                int count = _mTmpPooledItemList.Count;
                tItem = _mTmpPooledItemList[count - 1];
                _mTmpPooledItemList.RemoveAt(count - 1);
                tItem.gameObject.SetActive(true);
            }
            else
            {
                int count = _mPooledItemList.Count;
                if (count == 0)
                {
                    tItem = CreateItem();
                }
                else
                {
                    tItem = _mPooledItemList[count - 1];
                    _mPooledItemList.RemoveAt(count - 1);
                    tItem.gameObject.SetActive(true);
                }
            }
            tItem.Padding = _mPadding;
            tItem.ItemId = _mCurItemIdCount;
            return tItem;

        }

        public void DestroyAllItem()
        {
            ClearTmpRecycledItem();
            int count = _mPooledItemList.Count;
            for (int i = 0; i < count; ++i)
            {
                GameObject.DestroyImmediate(_mPooledItemList[i].gameObject);
            }
            _mPooledItemList.Clear();
        }
        public LoopListViewItem2 CreateItem()
        {

            GameObject go = GameObject.Instantiate<GameObject>(_mPrefabObj, Vector3.zero, Quaternion.identity, _mItemParent);
            go.SetActive(true);
            RectTransform rf = go.GetComponent<RectTransform>();
            rf.localScale = Vector3.one;
            rf.anchoredPosition3D = Vector3.zero;
            rf.localEulerAngles = Vector3.zero;
            LoopListViewItem2 tViewItem = go.GetComponent<LoopListViewItem2>();
            tViewItem.ItemPrefabName = _mPrefabName;
            tViewItem.StartPosOffset = _mStartPosOffset;
            return tViewItem;
        }
        void RecycleItemReal(LoopListViewItem2 item)
        {
            item.gameObject.SetActive(false);
            _mPooledItemList.Add(item);
        }
        public void RecycleItem(LoopListViewItem2 item)
        {
            _mTmpPooledItemList.Add(item);
        }
        public void ClearTmpRecycledItem()
        {
            int count = _mTmpPooledItemList.Count;
            if (count == 0)
            {
                return;
            }
            for (int i = 0; i < count; ++i)
            {
                RecycleItemReal(_mTmpPooledItemList[i]);
            }
            _mTmpPooledItemList.Clear();
        }
    }
}
