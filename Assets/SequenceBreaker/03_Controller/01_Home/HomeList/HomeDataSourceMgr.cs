using System.Collections.Generic;
using SequenceBreaker._01_Data._03_UnitClass;
using UnityEngine;

namespace SequenceBreaker._03_Controller._01_Home.HomeList
{

//    public class HomeItemData
//    {
//        public int MId;
//        public string MContentString;
//        public string MDescriptionString;
//        public List<UnitClass> MUnitClassList;
//        public string MInventorySavedFileName;
//
//        public bool MChecked;
//        public bool MIsExpand;
//    }

    public class HomeDataSourceMgr : MonoBehaviour
    {

        List<HomeContentData> _mItemDataList = new List<HomeContentData>();
        System.Action _mOnRefreshFinished = null;
        System.Action _mOnLoadMoreFinished = null;
        int _mLoadMoreCount = 20;
        float _mDataLoadLeftTime = 0;
        float _mDataRefreshLeftTime = 0;
        bool _mIsWaittingRefreshData = false;
//        bool _mIsWaitLoadingMoreData = false;
//        public int mTotalDataCount;


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
                    _instance = Object.FindObjectOfType<HomeDataSourceMgr>();
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
            _mIsWaittingRefreshData = true;
        }

        public void RequestLoadMoreDataList(int loadCount,System.Action onLoadMoreFinished)
        {
            _mLoadMoreCount = loadCount;
            _mDataLoadLeftTime = 1;
            _mOnLoadMoreFinished = onLoadMoreFinished;
//            _mIsWaitLoadingMoreData = true;
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
//            if (_mIsWaitLoadingMoreData)
//            {
//                _mDataLoadLeftTime -= Time.deltaTime;
//                if (_mDataLoadLeftTime <= 0)
//                {
//                    _mIsWaitLoadingMoreData = false;
//                    DoLoadMoreDataSource();
//                    if (_mOnLoadMoreFinished != null)
//                    {
//                        _mOnLoadMoreFinished();
//                    }
//                }
//            }

        }

        public void SetDataTotalCount(int count)
        {
//            mTotalDataCount = count;
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

//        void DoLoadMoreDataSource()
//        {
//            int count = _mItemDataList.Count;
//            for (int k = 0; k < _mLoadMoreCount; ++k)
//            {
//                int i = k + count;
//                HomeItemData tData = new HomeItemData();
//                tData.MId = i;
//                tData.MStarCount = Random.Range(0, 6);
//                tData.MFileSize = Random.Range(20, 999);
//                tData.MChecked = false;
//                tData.MIsExpand = false;
//                _mItemDataList.Add(tData);
//            }
//        }

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