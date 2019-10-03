using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System.Linq;
using System;

namespace KohmaiWorks.Scroller
{
    /// <summary>
    /// This demo shows how you can use the calculated size of the cell view to drive the scroller's cell sizes.
    /// This can be good for cases where you do not know how large each cell will need to be until the contents are
    /// populated. An example of this would be text cells containing unknown information.
    /// </summary>
    public class BattleLogEnhancedScrollController : MonoBehaviour, IEnhancedScrollerDelegate
    {
        public List<DataList> DataList; // Data from outside to show the log. 
        private List<Data> _data;
        //private List<Data> _dataOfAll;
        //private List<Data> _dataOfFiltered;
        //private List<BattleLogClass> battleLogLists;

        /// <summary>
        /// This member tells the scroller that we need
        /// the cell views to figure out how much space to use.
        /// This is only set to true on the first pass to reduce
        /// processing required.
        /// </summary>
        private bool _calculateLayout;

        private GameObject JumpIndex;


        public EnhancedScroller scroller;
        public EnhancedScrollerCellView cellViewPrefab;
        public EnhancedScrollerCellView cellTurnStartPrefab;

        public Transform parentJumpIndex;
        public GameObject jumpIndexPrefab;
        //public SimpleObjectPool jumpIndexObjectPool;

        public RectTransform canvasToGetWidth;

        // this is temp.
        public Sprite allyImage;
        public Sprite enemyImage;

        public SimpleObjectPool unitIconObjectPool;


        // Searching
        public GameObject searchBar;
        public RectTransform log;
        public Text searchResultText;
        private (int number, int content) searchedCurrentIndex;
        private List<int> searchedIndexList;
        private Text previousSearchedText;



        /// <summary>
        /// Battle Information
        /// </summary>
        ///
        // sample implemented 2019.8.6

        public GameObject Battle;
        // end sanple implemented 2019.8.6


        //private BattleEngine _battle;
        private void Awake()
        {
            searchBar.transform.gameObject.SetActive(false);
            searchResultText.text = null;
        }

        void Start()
        {

        }

        /// <summary>
        /// Populates the data with some random Lorum Ipsum text
        /// </summary>
        private void LoadData()
        {

        }


        public void DrawBattleLog()
        {
            scroller.Delegate = this;
            _data = new List<Data>();

            //2019.10.3 always DataList is set 0.
            _data = Battle.gameObject.GetComponent<RunBattle>().DataList[0];

            DataList _setDatalist = new DataList();
            _setDatalist.Data = _data;

            DataList.Add(_setDatalist);

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
            for (var i = 0; i < _data.Count; i++)
            {

                // Get canvas width
                float widthOfCanvas = canvasToGetWidth.rect.width;
                int count = 1;
                if (_data[i].mainText != null)
                {

                    //string[] lines = _battle.logList[i].Log.Split(new Char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] lines = _data[i].mainText.Split(new Char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    count = lines.Length;

                    for (int j = 0; j < lines.Length; j++)
                    {
                        TextGenerator textGen = new TextGenerator();
                        TextGenerationSettings generationSettings = cellViewPrefab.GetComponentInChildren<Text>().GetGenerationSettings
                            (cellViewPrefab.GetComponentInChildren<Text>().rectTransform.rect.size);
                        float widthOfLine = textGen.GetPreferredWidth(lines[j], generationSettings);

                        int numberOfNewLine = (int)(widthOfLine / (widthOfCanvas - 250));
                        if (numberOfNewLine >= 1) { count += numberOfNewLine; }
                    }
                }
                int cellSize = (count + 1) * 48 + 190;
                const int cellMinSize = 220;
                if (cellSize < cellMinSize) { cellSize = cellMinSize; }

                if (_data[i].isHeaderInfo)
                {
                    cellSize = 1300;
                }

                //could be index is incorrect. 2019.9.5
                _data[i].cellSize = cellSize;


                //// _data set
                //string unitNameText = null;
                //string unitHealthText = null;
                //string reactText = null;
                //float shieldRatio = 0f;
                //float hPRatio = 0f;
                //int barrierRemains = 0;
                //bool isDead = true;
                //if (_battle.logList[i].Order != null)
                //{
                //    unitNameText = _battle.logList[i].Order.Actor.Name;
                //    unitHealthText = "[" + _battle.logList[i].Order.Actor.Combat.ShiledCurrent +
                //        "(" + Mathf.Ceil((float)_battle.logList[i].Order.Actor.Combat.ShiledCurrent * 100 / (float)_battle.logList[i].Order.Actor.Combat.ShiledMax) + "%)+"
                //    + _battle.logList[i].Order.Actor.Combat.HitPointCurrent + "("
                //    + Mathf.Ceil((float)_battle.logList[i].Order.Actor.Combat.HitPointCurrent * 100 / (float)_battle.logList[i].Order.Actor.Combat.HitPointMax)
                //     + "%)]";

                //    if (_battle.logList[i].Order.IndividualTarget != null)
                //    {
                //        string preposition = " to ";
                //        if (_battle.logList[i].Order.ActionType == ActionType.ReAttack) { preposition = " of "; }
                //        //else if (_battle.logList[i].Order.ActionType == ActionType.) { preposition = " for "; }

                //        reactText = _battle.logList[i].Order.ActionType.ToString() + preposition + _battle.logList[i].Order.IndividualTarget.Name;
                //    }

                //    shieldRatio = (float)_battle.logList[i].Order.Actor.Combat.ShiledCurrent / (float)_battle.logList[i].Order.Actor.Combat.ShiledMax;
                //    hPRatio = (float)_battle.logList[i].Order.Actor.Combat.HitPointCurrent / (float)_battle.logList[i].Order.Actor.Combat.HitPointMax;

                //    barrierRemains = _battle.logList[i].Order.Actor.Buff.BarrierRemaining;

                //    if (_battle.logList[i].Order.Actor.Combat.HitPointCurrent > 0)
                //    {
                //        isDead = false;
                //    }

                //}

                //List<BattleUnit> characters = null;
                //if (_battle.logList[i].Characters != null)
                //{
                //    characters = _battle.logList[i].Characters;
                //}


                //_dataOfAll.Add(new Data()
                //{
                //    index = i,
                //    cellSize = cellSize,
                //    reactText = reactText,
                //    unitInfo = "<b>" + unitNameText + "</b>  " + unitHealthText,
                //    firstLine = _battle.logList[i].FirstLine,
                //    mainText = _battle.logList[i].Log,
                //    affiliation = _battle.logList[i].WhichAffiliationAct,
                //    nestLevel = _battle.logList[i].OrderCondition.Nest,
                //    isDead = isDead,
                //    barrierRemains = barrierRemains,
                //    shieldRatio = shieldRatio,
                //    hPRatio = hPRatio,
                //    isHeaderInfo = _battle.logList[i].IsHeaderInfo,
                //    headerText = _battle.logList[i].HeaderInfoText,
                //    characters = characters
                //});

                //// set all of data to data (activate)
                //_data = _dataOfAll;
            }
        }

        private void SetJumpIndex()
        {
            if (_data != null)
            {

                int maxCount = _data.Count;
                int maxTurn = 1;
                if (maxCount > 1) { maxTurn = _data[maxCount - 1].turn + 1; }


                foreach (Transform child in parentJumpIndex)
                {
                    Destroy(child.gameObject);
                }



                for (int i = 1; i <= maxTurn; i++)
                {
                    JumpIndex = Instantiate(jumpIndexPrefab, parentJumpIndex);


                    int index = _data.FindIndex((obj) => obj.turn == i);
                    JumpIndex.GetComponentInChildren<Text>().text = i.ToString();

                    EventTrigger trigger = GetComponentInParent<EventTrigger>();
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerEnter;

                    entry.callback.AddListener((data) => { JumpButton_OnClick(index); });

                    JumpIndex.GetComponent<EventTrigger>().triggers.Add(entry);
                }

            }

        }


        public void FilterBySearchText(Text searchText)
        {
            Debug.Log("attempt to search word: " + searchText.text);
            searchedIndexList = new List<int>();
            List<Data> filteredData = new List<Data>();
            foreach (Data data in _data)
            {

                // mainText if found, set is Matched true
                bool isMatched = false || (data.mainText != null && data.mainText.Contains(searchText.text));

                // firstLine if found set is Matched true

                if (data.firstLine != null && data.firstLine.Contains(searchText.text))
                {
                    isMatched = true;
                }

                if (isMatched)
                {
                    filteredData.Add(data);
                    searchedIndexList.Add(data.index);
                }
                else
                {
                    searchResultText.text = "no result";

                }
            }

            if (previousSearchedText == null) { previousSearchedText = searchText; }

            if (searchedIndexList.Count > 0)
            {
                searchedCurrentIndex = (0, searchedIndexList[0]);
                searchResultText.text = (searchedCurrentIndex.number + 1) + " / " + searchedIndexList.Count;
                JumpButton_OnClick(searchedCurrentIndex.content);
            }


        }

        public void SetJumpNextSearchedText()
        {
            if (searchedIndexList.Count - 1 > searchedCurrentIndex.number)
            {
                searchedCurrentIndex = (searchedCurrentIndex.number + 1, searchedIndexList[searchedCurrentIndex.number + 1]);
                searchResultText.text = (searchedCurrentIndex.number + 1) + " / " + searchedIndexList.Count;
                JumpButton_OnClick(searchedCurrentIndex.content);
            }
        }

        public void SetJumpPreviousSearchedText()
        {
            if (searchedCurrentIndex.number > 0)
            {
                searchedCurrentIndex = (searchedCurrentIndex.number - 1, searchedIndexList[searchedCurrentIndex.number - 1]);
                searchResultText.text = (searchedCurrentIndex.number + 1) + " / " + searchedIndexList.Count;
                JumpButton_OnClick(searchedCurrentIndex.content);
            }
        }

        /// <summary>
        /// This function adds a new record, resizing the scroller and calculating the sizes of all cells
        /// </summary>


        /// <summary>
        /// This function will exand the scroller to accommodate the cells, reload the data to calculate the cell sizes,
        /// reset the scroller's size back, then reload the data once more to display the cells.
        /// </summary>
        private void ResizeScroller()
        {
            // capture the scroller dimensions so that we can reset them when we are done
            var rectTransform = scroller.GetComponent<RectTransform>();
            var size = rectTransform.sizeDelta;

            // set the dimensions to the largest size possible to acommodate all the cells
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

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            //return battleLogLists.Count;
            return _data.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            // we pull the size of the cell from the model.
            // First pass (frame countdown 2): this size will be zero as set in the LoadData function
            // Second pass (frame countdown 1): this size will be set to the content size fitter in the cell view
            // Third pass (frmae countdown 0): this set value will be pulled here from the scroller
            //return battleLogLists[dataIndex].cellSize;
            return _data[dataIndex].cellSize;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            CellView cellView = scroller.GetCellView(cellViewPrefab) as CellView;
            cellView.unitIconObjectPool = unitIconObjectPool;

            // tell the cell view to calculate its layout on the first pass,
            // otherwise just use the size set in the data.
            //cellView.SetData(battleLogLists[dataIndex], _calculateLayout);
            Data nextData = null;
            if (dataIndex < _data.Count - 1) { nextData = _data[dataIndex + 1]; }

            Sprite setSprite = null;
            if (_data[dataIndex].affiliation == Affiliation.ally) { setSprite = allyImage; }
            else if (_data[dataIndex].affiliation == Affiliation.enemy) { setSprite = enemyImage; }

            cellView.SetData(_data[dataIndex], nextData, _calculateLayout, setSprite);

            return cellView;
        }

        #endregion


        public void SetSearchBarOpen()
        {
            if (searchBar.transform.gameObject.activeSelf == false)
            {
                RectTransform rectTransform = log;
                rectTransform.offsetMin = new Vector2(log.offsetMin.x, log.offsetMin.y);
                rectTransform.offsetMax = new Vector2(log.offsetMax.x, log.offsetMax.y - 100);
                //log = rectTransform;
                searchBar.transform.gameObject.SetActive(true);
            }
        }

        public void SetSearchBarClose()
        {
            RectTransform rectTransform = log;
            rectTransform.offsetMin = new Vector2(log.offsetMin.x, log.offsetMin.y);
            rectTransform.offsetMax = new Vector2(log.offsetMax.x, log.offsetMax.y + 100);
            //log = rectTransform;
            searchBar.transform.gameObject.SetActive(false);
            //_data = _dataOfAll;
            //scroller.ReloadData();

        }


    }
}
