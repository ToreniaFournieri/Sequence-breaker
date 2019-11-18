using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker._04_Home.HomeListView
{
    

    public class HomeDataSourceMgr : MonoBehaviour
    {

        List<HomeContentData> _mItemDataList = new List<HomeContentData>();
        System.Action _mOnRefreshFinished = null;
        System.Action _mOnLoadMoreFinished = null;
        int _mLoadMoreCount = 20;
        float _mDataLoadLeftTime = 0;
        float _mDataRefreshLeftTime = 0;
        bool _mIsWaitingRefreshData = false;

        //Home contents
        public HomeContents homeContents;
        
        static HomeDataSourceMgr _instance = null;
        private bool _isHomeContentsNotNull;

        public static HomeDataSourceMgr Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<HomeDataSourceMgr>();
                }
                return _instance;
            }

        }

        private void Start()
        {
            
        }

        void Awake()
        {
            Init();
        }


        public void Init()
        {
            _isHomeContentsNotNull = homeContents != null;

            DoRefreshDataSource();
        }

        public HomeContentData GetItemDataByIndex(int index)
        {
            if (index < 0 || index >= _mItemDataList.Count)
            {
                return null;
            }
            return _mItemDataList[index];
        }

        public HomeContentData GetItemDataById(int itemId)
        {
            int count = _mItemDataList.Count;
            for (int i = 0; i < count; ++i)
            {
                if(_mItemDataList[i].mId == itemId)
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
            _mIsWaitingRefreshData = true;
        }

        public void RequestLoadMoreDataList(int loadCount,System.Action onLoadMoreFinished)
        {
            _mLoadMoreCount = loadCount;
            _mDataLoadLeftTime = 1;
            _mOnLoadMoreFinished = onLoadMoreFinished;
        }

        public void Update()
        {
            if (_mIsWaitingRefreshData)
            {
                _mDataRefreshLeftTime -= Time.deltaTime;
                if (_mDataRefreshLeftTime <= 0)
                {
                    _mIsWaitingRefreshData = false;
                    DoRefreshDataSource();
                    if (_mOnRefreshFinished != null)
                    {
                        _mOnRefreshFinished();
                    }
                }
            }

        }

        public void SetDataTotalCount(int count)
        {
            DoRefreshDataSource();
        }

        public void ExchangeData(int index1,int index2)
        {
            HomeContentData tData1 = _mItemDataList[index1];
            HomeContentData tData2 = _mItemDataList[index2];
            _mItemDataList[index1] = tData2;
            _mItemDataList[index2] = tData1;
        }

        public void RemoveData(int index)
        {
            _mItemDataList.RemoveAt(index);
        }

        public void InsertData(int index,HomeContentData data)
        {
            _mItemDataList.Insert(index,data);
        }

        void DoRefreshDataSource()
        {
            _mItemDataList.Clear();
            if (_isHomeContentsNotNull)
            {
                for (int i = 0; i < homeContents.homeContentList.Count; ++i)
                {
                    _mItemDataList.Add(homeContents.homeContentList[i]);
                }

            }
        }

        public void CheckAllItem()
        {
            int count = _mItemDataList.Count;
            for (int i = 0; i < count; ++i)
            {
                _mItemDataList[i].mChecked = true;
            }
        }

        public void UnCheckAllItem()
        {
            int count = _mItemDataList.Count;
            for (int i = 0; i < count; ++i)
            {
                _mItemDataList[i].mChecked = false;
            }
        }

        public bool DeleteAllCheckedItem()
        {
            int oldCount = _mItemDataList.Count;
            _mItemDataList.RemoveAll(it => it.mChecked);
            return (oldCount != _mItemDataList.Count);
        }

    }

}