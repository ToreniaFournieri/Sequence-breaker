using System;
using System.Collections.Generic;
using _00_Asset._05_EnhancedScroller_v2.Plugins;
using _00_Asset.I2.Localization.Scripts.Manager;
using SequenceBreaker.Environment;
using SequenceBreaker.Play.Prepare;
using SequenceBreaker.Translate;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SequenceBreaker.Timeline.BattleLogView
{
    /// <summary>
    /// This demo shows how you can use the calculated size of the cell view to drive the scroller's cell sizes.
    /// This can be good for cases where you do not know how large each cell will need to be until the contents are
    /// populated. An example of this would be text cells containing unknown information.
    /// </summary>
    public sealed class BattleLogEnhancedScrollController : MonoBehaviour, IEnhancedScrollerDelegate
    {
        [FormerlySerializedAs("DataList")] public List<DataList> dataList; // Data from outside to show the log. 
        private List<Data> _data;


        /// <summary>
        /// This member tells the scroller that we need
        /// the cell views to figure out how much space to use.
        /// This is only set to true on the first pass to reduce
        /// processing required.
        /// </summary>
        private readonly bool _calculateLayout;

        private GameObject _jumpIndex;


        public EnhancedScroller scroller;
        public EnhancedScrollerCellView cellViewPrefab;

        public Transform parentJumpIndex;
        public GameObject jumpIndexPrefab;

        public RectTransform canvasToGetWidth;



        // this is temp.
        public Sprite allyImage;
        public Sprite enemyImage;

        //public SimpleObjectPool unitIconObjectPool;
        //public SimpleObjectPool mainTextObjectPool;
        //public SimpleObjectPool hPShieldBarObjectPool;

        // Searching
        public GameObject searchBar;
        public RectTransform log;
        public Text searchResultText;
        private (int number, int content) _searchedCurrentIndex;
        private List<int> _searchedIndexList;
        private Text _previousSearchedText;



        /// <summary>
        /// Battle Information
        /// </summary>
        ///

        // sample implemented 2019.8.6

        public RunBattle runBattle;
        
        public BattleLogEnhancedScrollController(bool calculateLayout)
        {
            _calculateLayout = calculateLayout;
        }
        // end sample implemented 2019.8.6

        private void Awake()
        {
            searchBar.transform.gameObject.SetActive(false);
            searchResultText.text = null;
        }

        /// <summary>
        /// Populates the data with some random Lorum Ipsum text
        /// </summary>
        //private void LoadData()
        //{

        //}

        public void InitBattleLog(RunBattle initBattle)
        {
            runBattle = initBattle;
            DrawBattleLog();
        }


        public void DrawBattleLog()
        {
            scroller.Delegate = this;
            _data = new List<Data>();

            //2019.10.3 always DataList is set 0.
            _data = runBattle.dataList[0];

            DataList setDatalist = ScriptableObject.CreateInstance<DataList>();
            setDatalist.data = _data;

            dataList.Add(setDatalist);

            if (_data != null)
            {
                GetCanvasSize();
                SetJumpIndex();
            }
            ResizeScroller();
        }

        private void GetCanvasSize()
        {
            //populate the scroller with some text
            foreach (var t1 in _data)
            {
// Get canvas width
                float widthOfCanvas = canvasToGetWidth.rect.width;
                int count = 1;

                //float hight = 0f;

                int additionalHight = 0;

                if (t1.MainTextList != null)
                {
                    foreach (string text in t1.MainTextList)
                    {
                        string[] lines = text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        count += lines.Length;
                    }
                    count++;
                    additionalHight = 235 ;


                } else if (t1.BigText != null)
                {
                    string[] lines = t1.BigText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    count = lines.Length + 1;
                    additionalHight = 135;

                }

                //Debug.Log((count) + " text: " + t1.MainText);
                int cellSize = (count) * int.Parse(LocalizationManager.GetTranslation("FONT-BattleLogHightSize")) + additionalHight;
                const int cellMinSize = 220;
                if (cellSize < cellMinSize) { cellSize = cellMinSize; }

                if (t1.IsHeaderInfo)
                {
                    cellSize = 1300;
                }

                //could be index is incorrect. 2019.9.5
                t1.CellSize = cellSize;


            }
        }

        private void SetJumpIndex()
        {
            if (_data != null)
            {

                int maxCount = _data.Count;
                int maxTurn = 1;
                if (maxCount > 1) { maxTurn = _data[maxCount - 1].Turn + 1; }


                foreach (Transform child in parentJumpIndex)
                {
                    Destroy(child.gameObject);
                }



                for (int i = 1; i <= maxTurn; i++)
                {
                    _jumpIndex = Instantiate(jumpIndexPrefab, parentJumpIndex);


                    int index = _data.FindIndex((obj) => obj.Turn == i);
                    _jumpIndex.GetComponentInChildren<Text>().text = i.ToString();

                    GetComponentInParent<EventTrigger>();
                    EventTrigger.Entry entry = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};

                    entry.callback.AddListener((data) => { JumpButton_OnClick(index); });

                    _jumpIndex.GetComponent<EventTrigger>().triggers.Add(entry);
                }

            }

        }


        public void FilterBySearchText(Text searchText)
        {
            Debug.Log("attempt to search word: " + searchText.text);
            _searchedIndexList = new List<int>();
//            var filteredData = new List<Data>();
            foreach (Data data in _data)
            {

                // mainText if found, set is Matched true
                bool isMatched = (data.MainTextList != null && data.MainTextList.Contains(searchText.text)) || data.FirstLine != null && data.FirstLine.Contains(searchText.text);

                // firstLine if found set is Matched true

                if (isMatched)
                {
//                    filteredData.Add(data);
                    _searchedIndexList.Add(data.Index);
                }
                else
                {
                    searchResultText.text = "no result";

                }
            }

            if (_previousSearchedText == null) { _previousSearchedText = searchText; }

            if (_searchedIndexList.Count > 0)
            {
                _searchedCurrentIndex = (0, _searchedIndexList[0]);
                searchResultText.text = (_searchedCurrentIndex.number + 1) + " / " + _searchedIndexList.Count;
                JumpButton_OnClick(_searchedCurrentIndex.content);
            }


        }

        public void SetJumpNextSearchedText()
        {
            if (_searchedIndexList.Count - 1 > _searchedCurrentIndex.number)
            {
                _searchedCurrentIndex = (_searchedCurrentIndex.number + 1, _searchedIndexList[_searchedCurrentIndex.number + 1]);
                searchResultText.text = (_searchedCurrentIndex.number + 1) + " / " + _searchedIndexList.Count;
                JumpButton_OnClick(_searchedCurrentIndex.content);
            }
        }

        public void SetJumpPreviousSearchedText()
        {
            if (_searchedCurrentIndex.number > 0)
            {
                _searchedCurrentIndex = (_searchedCurrentIndex.number - 1, _searchedIndexList[_searchedCurrentIndex.number - 1]);
                searchResultText.text = (_searchedCurrentIndex.number + 1) + " / " + _searchedIndexList.Count;
                JumpButton_OnClick(_searchedCurrentIndex.content);
            }
        }

        /// <summary>
        /// This function adds a new record, resizing the scroller and calculating the sizes of all cells
        /// </summary>


        /// <summary>
        /// This function will expand the scroller to accommodate the cells, reload the data to calculate the cell sizes,
        /// reset the scroller's size back, then reload the data once more to display the cells.
        /// </summary>
        private void ResizeScroller()
        {
            // capture the scroller dimensions so that we can reset them when we are done
            var rectTransform = scroller.GetComponent<RectTransform>();
            var size = rectTransform.sizeDelta;

            // set the dimensions to the largest size possible to accommodate all the cells
            rectTransform.sizeDelta = new Vector2(size.x, float.MaxValue);

            //// First Pass: reload the scroller so that it can populate the text UI elements in the cell view.
            //// The content size fitter will determine how big the cells need to be on subsequent passes.
            //_calculateLayout = true;
            //scroller.ReloadData();

            //// reset the scroller size back to what it was originally
            rectTransform.sizeDelta = size;

            //// Second Pass: reload the data once more with the newly set cell view sizes and scroller content size
            //_calculateLayout = false;
            scroller.ReloadData();
        }

        public void JumpButton_OnClick(int jumpIndex)
        {
            scroller.JumpToDataIndex(jumpIndex, 0f, 0f);
        }

        #region EnhancedScroller Handlers

        public int GetNumberOfCells(EnhancedScroller enhancedScroller)
        {
            //return battleLogLists.Count;
            return _data.Count;
        }

        public float GetCellViewSize(EnhancedScroller enhancedScroller, int dataIndex)
        {
            // we pull the size of the cell from the model.
            // First pass (frame countdown 2): this size will be zero as set in the LoadData function
            // Second pass (frame countdown 1): this size will be set to the content size fitter in the cell view
            // Third pass (frame countdown 0): this set value will be pulled here from the scroller
            //return battleLogLists[dataIndex].cellSize;
            return _data[dataIndex].CellSize;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller enhancedScroller, int dataIndex, int cellIndex)
        {
            CellView cellView = enhancedScroller.GetCellView(cellViewPrefab) as CellView;
            if (cellView != null)
            {
                //cellView.unitIconObjectPool = unitIconObjectPool;
                //cellView.mainTextObjectPool = mainTextObjectPool;
                //cellView.hPShieldBarObjectPool = hPShieldBarObjectPool;

                // tell the cell view to calculate its layout on the first pass,
                // otherwise just use the size set in the data.
                //cellView.SetData(battleLogLists[dataIndex], _calculateLayout);
                Data nextData = null;
                if (dataIndex < _data.Count - 1)
                {
                    nextData = _data[dataIndex + 1];
                }

                Sprite setSprite = null;
                if (_data[dataIndex].Affiliation == Affiliation.Ally)
                {
                    setSprite = allyImage;
                }
                else if (_data[dataIndex].Affiliation == Affiliation.Enemy)
                {
                    setSprite = enemyImage;
                }

                cellView.SetData(_data[dataIndex], nextData, _calculateLayout, setSprite);

                return cellView;
            }

            return null;
        }

        #endregion


        public void SetSearchBarOpen()
        {
            if (searchBar.transform.gameObject.activeSelf == false)
            {
                RectTransform rectTransform = log;
                var offsetMin = log.offsetMin;
                rectTransform.offsetMin = new Vector2(offsetMin.x, offsetMin.y);
                var offsetMax = log.offsetMax;
                rectTransform.offsetMax = new Vector2(offsetMax.x, offsetMax.y - 100);
                //log = rectTransform;
                searchBar.transform.gameObject.SetActive(true);
            }
        }

        public void SetSearchBarClose()
        {
            RectTransform rectTransform = log;
            var offsetMin = log.offsetMin;
            rectTransform.offsetMin = new Vector2(offsetMin.x, offsetMin.y);
            var offsetMax = log.offsetMax;
            rectTransform.offsetMax = new Vector2(offsetMax.x, offsetMax.y + 100);
            //log = rectTransform;
            searchBar.transform.gameObject.SetActive(false);
            //_data = _dataOfAll;
            //scroller.ReloadData();

        }


    }
}
