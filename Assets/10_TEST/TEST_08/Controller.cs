using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;

namespace KohmaiWorks.Scroller08
{
    /// <summary>
    /// This demo shows how you can use the calculated size of the cell view to drive the scroller's cell sizes.
    /// This can be good for cases where you do not know how large each cell will need to be until the contents are
    /// populated. An example of this would be text cells containing unknown information.
    /// </summary>
    public class Controller : MonoBehaviour, IEnhancedScrollerDelegate
    {
        private List<Data> _data;

        /// <summary>
        /// This member tells the scroller that we need
        /// the cell views to figure out how much space to use.
        /// This is only set to true on the first pass to reduce
        /// processing required.
        /// </summary>
        private bool _calculateLayout;

        public EnhancedScroller scroller;
        public EnhancedScrollerCellView cellViewPrefab;

        public Transform parentJumpIndex;
        public GameObject jumpIndexPrefab;

        private BattleEngine battle = new BattleEngine();

        void Start()
        {
            scroller.Delegate = this;
            LoadData();
            SetJumpIndex();
        }

        /// <summary>
        /// Populates the data with some random Lorum Ipsum text
        /// </summary>
        private void LoadData()
        {
            _data = new List<Data>();

            battle.Battle();


            // populate the scroller with some text
            for (var i = 0; i < battle.logList.Count; i++)
            {
                _data.Add(new Data() { cellSize = 0, someText = battle.logList[i].Log });

            }

            ResizeScroller();
        }

        private void SetJumpIndex()
        {
            int maxCount = battle.logList.Count;
            int maxTurn = battle.logList[maxCount - 1].OrderCondition.Turn;

            for (int i = 1; i < maxTurn; i++)
            {
                GameObject JumpIndex = Instantiate(jumpIndexPrefab, parentJumpIndex);

                int index = battle.logList.FindIndex((obj) => obj.OrderCondition.Turn == i);
                JumpIndex.GetComponentInChildren<Text>().text = i.ToString();

                EventTrigger trigger = GetComponentInParent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                 
                entry.callback.AddListener((data) => { JumpButton_OnClick(index); });

                JumpIndex.GetComponent<EventTrigger>().triggers.Add(entry);
            }

        }


        /// <summary>
        /// This function adds a new record, resizing the scroller and calculating the sizes of all cells
        /// </summary>
        public void AddNewRow()
        {
            // first, clear out the cells in the scroller so the new text transforms will be reset
            scroller.ClearAll();

            // reset the scroller's position so that it is not outside of the new bounds
            scroller.ScrollPosition = 0;

            // second, reset the data's cell view sizes
            foreach (var item in _data)
            {
                item.cellSize = 0;
            }

            // now we can add the data row
            _data.Add(new Data() { cellSize = 0, someText = _data.Count.ToString() + " New Row Added!" });

            ResizeScroller();

            // optional: jump to the end of the scroller to see the new content
            scroller.JumpToDataIndex(_data.Count - 1, 1f, 1f);
        }

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

            // First Pass: reload the scroller so that it can populate the text UI elements in the cell view.
            // The content size fitter will determine how big the cells need to be on subsequent passes.
            _calculateLayout = true;
            scroller.ReloadData();

            // reset the scroller size back to what it was originally
            rectTransform.sizeDelta = size;

            // Second Pass: reload the data once more with the newly set cell view sizes and scroller content size
            _calculateLayout = false;
            scroller.ReloadData();
        }

        public void JumpButton_OnClick(int jumpIndex)
        {
            scroller.JumpToDataIndex(jumpIndex, 0f, 0f);
        }

        #region EnhancedScroller Handlers

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return _data.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            // we pull the size of the cell from the model.
            // First pass (frame countdown 2): this size will be zero as set in the LoadData function
            // Second pass (frame countdown 1): this size will be set to the content size fitter in the cell view
            // Third pass (frmae countdown 0): this set value will be pulled here from the scroller
            return _data[dataIndex].cellSize;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            CellView cellView = scroller.GetCellView(cellViewPrefab) as CellView;

            // tell the cell view to calculate its layout on the first pass,
            // otherwise just use the size set in the data.
            cellView.SetData(_data[dataIndex], _calculateLayout);

            return cellView;
        }


        #endregion
    }
}
