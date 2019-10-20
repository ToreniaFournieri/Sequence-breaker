using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SuperScrollView
{

    public class StaggeredGridItemPool
    {
        GameObject _mPrefabObj;
        string _mPrefabName;
        int _mInitCreateCount = 1;
        float _mPadding = 0;
        List<LoopStaggeredGridViewItem> _mTmpPooledItemList = new List<LoopStaggeredGridViewItem>();
        List<LoopStaggeredGridViewItem> _mPooledItemList = new List<LoopStaggeredGridViewItem>();
        static int _mCurItemIdCount = 0;
        RectTransform _mItemParent = null;
        public StaggeredGridItemPool()
        {

        }
        public void Init(GameObject prefabObj, float padding, int createCount, RectTransform parent)
        {
            _mPrefabObj = prefabObj;
            _mPrefabName = _mPrefabObj.name;
            _mInitCreateCount = createCount;
            _mPadding = padding;
            _mItemParent = parent;
            _mPrefabObj.SetActive(false);
            for (int i = 0; i < _mInitCreateCount; ++i)
            {
                LoopStaggeredGridViewItem tViewItem = CreateItem();
                RecycleItemReal(tViewItem);
            }
        }
        public LoopStaggeredGridViewItem GetItem()
        {
            _mCurItemIdCount++;
            LoopStaggeredGridViewItem tItem = null;
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
        public LoopStaggeredGridViewItem CreateItem()
        {

            GameObject go = GameObject.Instantiate<GameObject>(_mPrefabObj, Vector3.zero, Quaternion.identity, _mItemParent);
            go.SetActive(true);
            RectTransform rf = go.GetComponent<RectTransform>();
            rf.localScale = Vector3.one;
            rf.anchoredPosition3D = Vector3.zero;
            rf.localEulerAngles = Vector3.zero;
            LoopStaggeredGridViewItem tViewItem = go.GetComponent<LoopStaggeredGridViewItem>();
            tViewItem.ItemPrefabName = _mPrefabName;
            tViewItem.StartPosOffset = 0;
            return tViewItem;
        }
        void RecycleItemReal(LoopStaggeredGridViewItem item)
        {
            item.gameObject.SetActive(false);
            _mPooledItemList.Add(item);
        }
        public void RecycleItem(LoopStaggeredGridViewItem item)
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
