using _00_Asset._05_EnhancedScroller_v2.Plugins;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker._06_Timeline.BattleLogView
    {
    public sealed class EnhancedScrollCellView : EnhancedScrollerCellView
    {
        public Text cellText;

        /// <summary>
        /// A reference to the rect transform which will be
        /// updated by the content size fitter
        /// </summary>
        public RectTransform textRectTransform;

        /// <summary>
        /// The space around the text label so that we
        /// aren't up against the edges of the cell
        /// </summary>
        public RectOffset textBuffer;

        public void SetData(Data data, bool calculateLayout)
        {
            cellText.text = data.mainText;


            // Only calculate the layout on the first pass.
            // This will save processing on subsequent passes.
            if (calculateLayout)
            {
                // force update the canvas so that it can calculate the size needed for the text immediately
                Canvas.ForceUpdateCanvases();

                // set the data's cell size and add in some padding so the the text isn't up against the border of the cell
                data.cellSize = textRectTransform.rect.height + textBuffer.top + textBuffer.bottom;
            }
        }


    }
}