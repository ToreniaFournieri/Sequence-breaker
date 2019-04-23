using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace KohmaiWorks.Scroller
{
    public class CellView : EnhancedScrollerCellView
    {
        public Text someTextText;
        public Image image;

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
        //public void SetData(BattleLogClass battleLogLists, bool calculateLayout)
        {
            someTextText.text = data.someText;
            //someTextText.text = battleLogLists.Log;
            Color darkRed = new Color32(17, 8, 8, 255); // dark red
            Color darkGreen = new Color32(8, 17, 8, 255); // dark green
            Color color = new Color32(17, 17, 17, 255);

            switch (data.affiliation)
            {
                case Affiliation.ally:
                    color = darkGreen;
                    break;
                case Affiliation.enemy:
                    color = darkRed;
                    break;
                default:
                    break;
            }

            image.color = color;

            // Only calculate the layout on the first pass.
            // This will save processing on subsequent passes.
            if (calculateLayout)
            {
                // force update the canvas so that it can calculate the size needed for the text immediately
                Canvas.ForceUpdateCanvases();

                // set the data's cell size and add in some padding so the the text isn't up against the border of the cell
                data.cellSize = textRectTransform.rect.height + textBuffer.top + textBuffer.bottom;
                //battleLogLists.cellSize = textRectTransform.rect.height + textBuffer.top + textBuffer.bottom;
            }
        }
    }
}