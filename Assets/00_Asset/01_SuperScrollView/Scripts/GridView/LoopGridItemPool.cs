using System.Collections.Generic;
using UnityEngine;

namespace _00_Asset._01_SuperScrollView.Scripts.GridView
{
    public class GridItemPool
    {
        GameObject _mPrefabObj;
        string _mPrefabName;
        int _mInitCreateCount = 1;
        List<LoopGridViewItem> _mTmpPooledItemList = new List<LoopGridViewItem>();
        List<LoopGridViewItem> _mPooledItemList = new List<LoopGridViewItem>();
        static int _mCurItemIdCount = 0;
        RectTransform _mItemParent = null;
        public GridItemPool()
        {

        }
        public void Init(GameObject prefabObj, int createCount, RectTransform parent)
        {
            _mPrefabObj = prefabObj;
            _mPrefabName = _mPrefabObj.name;
            _mInitCreateCount = createCount;
            _mItemParent = parent;
            _mPrefabObj.SetActive(false);
            for (int i = 0; i < _mInitCreateCount; ++i)
            {
                LoopGridViewItem tViewItem = CreateItem();
                RecycleItemReal(tViewItem);
            }
        }
        public LoopGridViewItem GetItem()
        {
            _mCurItemIdCount++;
            LoopGridViewItem tItem = null;
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
            tItem.ItemId = _mCurItemIdCount;
            return tItem;

        }

        public void DestroyAllItem()
        {
            ClearTmpRecycledItem();
            int count = _mPooledItemList.Count;
            for (int i = 0; i < count; ++i)
            {
                Object.DestroyImmediate(_mPooledItemList[i].gameObject);
            }
            _mPooledItemList.Clear();
        }
        public LoopGridViewItem CreateItem()
        {

            GameObject go = Object.Instantiate<GameObject>(_mPrefabObj, Vector3.zero, Quaternion.identity, _mItemParent);
            go.SetActive(true);
            RectTransform rf = go.GetComponent<RectTransform>();
            rf.localScale = Vector3.one;
            rf.anchoredPosition3D = Vector3.zero;
            rf.localEulerAngles = Vector3.zero;
            LoopGridViewItem tViewItem = go.GetComponent<LoopGridViewItem>();
            tViewItem.ItemPrefabName = _mPrefabName;
            return tViewItem;
        }
        void RecycleItemReal(LoopGridViewItem item)
        {
            item.gameObject.SetActive(false);
            _mPooledItemList.Add(item);
        }
        public void RecycleItem(LoopGridViewItem item)
        {
            item.PrevItem = null;
            item.NextItem = null;
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
