using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;

namespace KohmaiWorks.Scroller
{
    /// <summary>
    /// This demo shows how to jump to an index in the scroller. You can jump to a position before
    /// or after the cell. You can also include the spacing before or after the cell.
    /// </summary>
    public class BattleLogEnhancedScrollController : MonoBehaviour, IEnhancedScrollerDelegate
    {
        /// <summary>
        /// In this example we are going to use a standard generic List. We could have used
        /// a SmallList for efficiency, but this is just a demonstration that other list
        /// types can be used.
        /// </summary>
        private List<Data> _data;

        /// <summary>
        /// Reference to the scrollers
        /// </summary>
        public EnhancedScroller vScroller;

        /// <summary>
        /// References to the UI elements
        /// </summary>
        public InputField jumpIndexInput;
        public Toggle useSpacingToggle;
        public Slider scrollerOffsetSlider;
        public Slider cellOffsetSlider;

        /// <summary>
        /// Reference to the cell prefab
        /// </summary>
        public EnhancedScrollerCellView cellViewPrefab;

        /// <summary>
        /// BattleLog Data;
        /// </summary>
        private List<BattleLogClass> battleLogList;
        //public SimpleObjectPool buttonObjectPool;


        /// <summary>
        /// This member tells the scroller that we need
        /// the cell views to figure out how much space to use.
        /// This is only set to true on the first pass to reduce
        /// processing required.
        /// </summary>
        private bool _calculateLayout;

        public EnhancedScroller.TweenType vScrollerTweenType = EnhancedScroller.TweenType.immediate;
        public float vScrollerTweenTime = 0f;

        void Start()
        {
            // set up the scroller delegates
            vScroller.Delegate = this;
            BattleRun();

            // set up some simple data
            _data = new List<Data>();
            for (var i = 0; i < battleLogList.Count; i++)
                //_data.Add(new Data() { cellText = "Cell Data Index " + i.ToString() });
                _data.Add(new Data() { cellText = battleLogList[i].Log });
            ResizeScroller();

            // tell the scroller to reload now that we have the data
            //vScroller.ReloadData();

        }

        private void BattleRun()
        {
            BattleEngine battle = new BattleEngine();
            battleLogList = new List<BattleLogClass>();
            battle.Battle();
            battleLogList = battle.logList;
        }

        //private void AddList()
        //{
        //    for (int i = 0; i < battleLogList.Count; i++)
        //    {
        //        BattleLogClass battleLog = battleLogList[i];
        //        GameObject newLogButton = buttonObjectPool.GetObject();
        //        newLogButton.transform.SetParent(contentPanel);

        //        LogButton logButton = newLogButton.GetComponent<LogButton>();
        //        logButton.Setup(battleLog);


        //    }
        //}


        #region UI Handlers
        public void JumpButtonWithIndex_OnClick(int jumpDataIndex)
        {
            //int jumpDataIndex;

            // extract the integer from the input text
            if (jumpDataIndex != 0)
            {
                // jump to the index
                vScroller.JumpToDataIndex(jumpDataIndex, scrollerOffsetSlider.value, cellOffsetSlider.value, useSpacingToggle.isOn, vScrollerTweenType, vScrollerTweenTime, null, EnhancedScroller.LoopJumpDirectionEnum.Down);
            }
            else
            {
                Debug.LogWarning("The jump value you entered is not a number.");
            }
        }
        public void JumpButton_OnClick()
        {
            int jumpDataIndex;

            // extract the integer from the input text
            if (int.TryParse(jumpIndexInput.text, out jumpDataIndex))
            {
                // jump to the index
                vScroller.JumpToDataIndex(jumpDataIndex, scrollerOffsetSlider.value, cellOffsetSlider.value, useSpacingToggle.isOn, vScrollerTweenType, vScrollerTweenTime, null, EnhancedScroller.LoopJumpDirectionEnum.Down);
            }
            else
            {
                Debug.LogWarning("The jump value you entered is not a number.");
            }
        }

        /// <summary>
        /// This function will exand the scroller to accommodate the cells, reload the data to calculate the cell sizes,
        /// reset the scroller's size back, then reload the data once more to display the cells.
        /// </summary>
        private void ResizeScroller()
        {
            // capture the scroller dimensions so that we can reset them when we are done
            var rectTransform = vScroller.GetComponent<RectTransform>();
            var size = rectTransform.sizeDelta;

            // set the dimensions to the largest size possible to acommodate all the cells
            rectTransform.sizeDelta = new Vector2(size.x, float.MaxValue);

            // First Pass: reload the scroller so that it can populate the text UI elements in the cell view.
            // The content size fitter will determine how big the cells need to be on subsequent passes.
            _calculateLayout = true;
            vScroller.ReloadData();

            // reset the scroller size back to what it was originally
            rectTransform.sizeDelta = size;

            // Second Pass: reload the data once more with the newly set cell view sizes and scroller content size
            _calculateLayout = false;
            vScroller.ReloadData();
        }

        #endregion




        #region EnhancedScroller Handlers

        /// <summary>
        /// This tells the scroller the number of cells that should have room allocated. This should be the length of your data array.
        /// </summary>
        /// <param name="scroller">The scroller that is requesting the data size</param>
        /// <returns>The number of cells</returns>
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            // in this example, we just pass the number of our data elements
            return _data.Count;
        }

        /// <summary>
        /// This tells the scroller what the size of a given cell will be. Cells can be any size and do not have
        /// to be uniform. For vertical scrollers the cell size will be the height. For horizontal scrollers the
        /// cell size will be the width.
        /// </summary>
        /// <param name="scroller">The scroller requesting the cell size</param>
        /// <param name="dataIndex">The index of the data that the scroller is requesting</param>
        /// <returns>The size of the cell</returns>
        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            // in this example, even numbered cells are 30 pixels tall, odd numbered cells are 100 pixels tall for the vertical scroller
            // the horizontal scroller has a fixed cell size of 200 pixels
            Debug.Log("_data dataIndex:" + dataIndex + " cellsize:" + _data[dataIndex].cellSize);
            return _data[dataIndex].cellSize;
            //return GetCellView(scroller, dataIndex, dataIndex).GetComponent<RectTransform>().rect.height;

            //    if (scroller == vScroller)
            //    return (dataIndex % 2 == 0 ? 30f : 100f);
            //else
            //return (200f);
        }

        /// <summary>
        /// Gets the cell to be displayed. You can have numerous cell types, allowing variety in your list.
        /// Some examples of this would be headers, footers, and other grouping cells.
        /// </summary>
        /// <param name="scroller">The scroller requesting the cell</param>
        /// <param name="dataIndex">The index of the data that the scroller is requesting</param>
        /// <param name="cellIndex">The index of the list. This will likely be different from the dataIndex if the scroller is looping</param>
        /// <returns>The cell for the scroller to use</returns>
        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            // first, we get a cell from the scroller by passing a prefab.
            // if the scroller finds one it can recycle it will do so, otherwise
            // it will create a new cell.
            EnhancedScrollCellView cellView = scroller.GetCellView(cellViewPrefab) as EnhancedScrollCellView;

            // set the name of the game object to the cell's data index.
            // this is optional, but it helps up debug the objects in 
            // the scene hierarchy.
            cellView.name = "Cell " + dataIndex.ToString();

            // in this example, we just pass the data to our cell's view which will update its UI

            cellView.SetData(_data[dataIndex], _calculateLayout);


            // return the cell to the scroller
            return cellView;
        }

        #endregion
    }
}