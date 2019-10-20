using System.Collections.Generic;
using UnityEngine;

namespace _00_Asset._01_Asset_SuperScrollView.Demo.Scripts.Common
{

    public class ItemData
    {
        public int MId;
        public string MName;
        public int MFileSize;
        public string MDesc;
        public string MIcon;
        public int MStarCount;
        public bool MChecked;
        public bool MIsExpand;
    }

    public class DataSourceMgr : MonoBehaviour
    {

        List<ItemData> _mItemDataList = new List<ItemData>();
        System.Action _mOnRefreshFinished = null;
        System.Action _mOnLoadMoreFinished = null;
        int _mLoadMoreCount = 20;
        float _mDataLoadLeftTime = 0;
        float _mDataRefreshLeftTime = 0;
        bool _mIsWaittingRefreshData = false;
        bool _mIsWaitLoadingMoreData = false;
        public int mTotalDataCount = 10000;

        static DataSourceMgr _instance = null;

        public static DataSourceMgr Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Object.FindObjectOfType<DataSourceMgr>();
                }
                return _instance;
            }

        }

        void Awake()
        {
            Init();
        }


        public void Init()
        {
            DoRefreshDataSource();
        }

        public ItemData GetItemDataByIndex(int index)
        {
            if (index < 0 || index >= _mItemDataList.Count)
            {
                return null;
            }
            return _mItemDataList[index];
        }

        public ItemData GetItemDataById(int itemId)
        {
            int count = _mItemDataList.Count;
            for (int i = 0; i < count; ++i)
            {
                if(_mItemDataList[i].MId == itemId)
                {
                    return _mItemDataList[i];
                }
            }
            return null;
        }

        public int TotalItemCount
        {
            get
            {
                return _mItemDataList.Count;
            }
        }

        public void RequestRefreshDataList(System.Action onReflushFinished)
        {
            _mDataRefreshLeftTime = 1;
            _mOnRefreshFinished = onReflushFinished;
            _mIsWaittingRefreshData = true;
        }

        public void RequestLoadMoreDataList(int loadCount,System.Action onLoadMoreFinished)
        {
            _mLoadMoreCount = loadCount;
            _mDataLoadLeftTime = 1;
            _mOnLoadMoreFinished = onLoadMoreFinished;
            _mIsWaitLoadingMoreData = true;
        }

        public void Update()
        {
            if (_mIsWaittingRefreshData)
            {
                _mDataRefreshLeftTime -= Time.deltaTime;
                if (_mDataRefreshLeftTime <= 0)
                {
                    _mIsWaittingRefreshData = false;
                    DoRefreshDataSource();
                    if (_mOnRefreshFinished != null)
                    {
                        _mOnRefreshFinished();
                    }
                }
            }
            if (_mIsWaitLoadingMoreData)
            {
                _mDataLoadLeftTime -= Time.deltaTime;
                if (_mDataLoadLeftTime <= 0)
                {
                    _mIsWaitLoadingMoreData = false;
                    DoLoadMoreDataSource();
                    if (_mOnLoadMoreFinished != null)
                    {
                        _mOnLoadMoreFinished();
                    }
                }
            }

        }

        public void SetDataTotalCount(int count)
        {
            mTotalDataCount = count;
            DoRefreshDataSource();
        }

        public void ExchangeData(int index1,int index2)
        {
            ItemData tData1 = _mItemDataList[index1];
            ItemData tData2 = _mItemDataList[index2];
            _mItemDataList[index1] = tData2;
            _mItemDataList[index2] = tData1;
        }

        public void RemoveData(int index)
        {
            _mItemDataList.RemoveAt(index);
        }

        public void InsertData(int index,ItemData data)
        {
            _mItemDataList.Insert(index,data);
        }

        void DoRefreshDataSource()
        {
            _mItemDataList.Clear();
            for (int i = 0; i < mTotalDataCount; ++i)
            {
                ItemData tData = new ItemData();
                tData.MId = i;
                tData.MName = "Item" + i;
                tData.MDesc = "Item Desc For Item " + i;
                tData.MIcon = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 24));
                tData.MStarCount = Random.Range(0, 6);
                tData.MFileSize = Random.Range(20, 999);
                tData.MChecked = false;
                tData.MIsExpand = false;
                _mItemDataList.Add(tData);
            }
        }

        void DoLoadMoreDataSource()
        {
            int count = _mItemDataList.Count;
            for (int k = 0; k < _mLoadMoreCount; ++k)
            {
                int i = k + count;
                ItemData tData = new ItemData();
                tData.MId = i;
                tData.MName = "Item" + i;
                tData.MDesc = "Item Desc For Item " + i;
                tData.MIcon = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 24));
                tData.MStarCount = Random.Range(0, 6);
                tData.MFileSize = Random.Range(20, 999);
                tData.MChecked = false;
                tData.MIsExpand = false;
                _mItemDataList.Add(tData);
            }
            mTotalDataCount = _mItemDataList.Count;
        }

        public void CheckAllItem()
        {
            int count = _mItemDataList.Count;
            for (int i = 0; i < count; ++i)
            {
                _mItemDataList[i].MChecked = true;
            }
        }

        public void UnCheckAllItem()
        {
            int count = _mItemDataList.Count;
            for (int i = 0; i < count; ++i)
            {
                _mItemDataList[i].MChecked = false;
            }
        }

        public bool DeleteAllCheckedItem()
        {
            int oldCount = _mItemDataList.Count;
            _mItemDataList.RemoveAll(it => it.MChecked);
            return (oldCount != _mItemDataList.Count);
        }

    }

}