using System.Collections.Generic;
using SequenceBreaker._08_Battle._2_BeforeBattle;
using UnityEngine;

namespace SequenceBreaker._03_Controller._03_Log.LogList
{
    public class LogListDataSourceMgr : MonoBehaviour
    {
 
        List<RunBattle> _mItemDataList = new List<RunBattle>();
        System.Action _mOnRefreshFinished = null;
        System.Action _mOnLoadMoreFinished = null;
        int _mLoadMoreCount = 20;
        float _mDataLoadLeftTime = 0;
        float _mDataRefreshLeftTime = 0;
        bool _mIsWaittingRefreshData = false;
//        bool _mIsWaitLoadingMoreData = false;
//        public int mTotalDataCount;


        // temp
        public LogListItemHeightDemoScript logListItemHeightDemoScript;
        

        //RunBattle external
        [SerializeField]
        public List<RunBattle> runBattleList;
//        public HomeContents homeContents;
        
        static LogListDataSourceMgr _instance = null;
        private bool _isRunBattleNotNull;

        public static LogListDataSourceMgr Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Object.FindObjectOfType<LogListDataSourceMgr>();
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
//            _isRunBattleNotNull = runBattleList != null;

            DoRefreshDataSource();
        }

        public void Refresh()
        {
            DoRefreshDataSource();
            
//            if (logListItemHeightDemoScript != null)
//            {
//                if (logListItemHeightDemoScript.mLoopListView != null)
//                {
//                    if (Get.TotalItemCount > 0)
//                    {
                        logListItemHeightDemoScript.UpdateNewContents();
//                    }
//                }
//            }
        }

        public RunBattle GetItemDataByIndex(int index)
        {
            if (index < 0 || index >= _mItemDataList.Count)
            {
                return null;
            }
            return _mItemDataList[index];
        }

        public RunBattle GetItemDataById(int itemId)
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
            RunBattle tData1 = _mItemDataList[index1];
            RunBattle tData2 = _mItemDataList[index2];
            _mItemDataList[index1] = tData2;
            _mItemDataList[index2] = tData1;
        }

        public void RemoveData(int index)
        {
            _mItemDataList.RemoveAt(index);
        }

        public void InsertData(int index,RunBattle data)
        {
            _mItemDataList.Insert(index,data);
        }

        void DoRefreshDataSource()
        {
//            _mItemDataList.Clear();

            _mItemDataList = runBattleList;
            
//                for (int i = 0; i < runBattleList.Count; ++i)
//                {
//                    _mItemDataList.Add(runBattleList[i]);
//                }
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
