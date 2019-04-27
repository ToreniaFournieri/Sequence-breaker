using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace KohmaiWorks.Scroller
{
    public class CellView : EnhancedScrollerCellView
    {
        public Text reactText;
        public Text unitName;
        public Text unitHealth;
        public Text firstLine;
        public Text mainText;
        public Image backgroundImage;
        public Image nestedLinePrevious;
        public Image nestedLine;
        public Image icon;

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

        public void SetData(Data data, Data nextData, bool calculateLayout)
        //public void SetData(BattleLogClass battleLogLists, bool calculateLayout)
        {
            reactText.text = data.reactText;
            unitName.text = data.unitName;
            unitHealth.text = data.unitHealth;
            firstLine.text = data.firstLine;
            mainText.text = data.mainText;
            //someTextText.text = battleLogLists.Log;
            Color darkRed = new Color32(24, 12, 12, 255); // dark red
            Color darkGreen = new Color32(12, 24, 12, 255); // dark green
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

            backgroundImage.color = color;

            nestedLinePrevious.rectTransform.sizeDelta = new Vector2(4 * data.nestLevel, 136);

            if (nextData != null)
            {
                nestedLine.rectTransform.sizeDelta = new Vector2(4 * nextData.nestLevel, 0);
            }

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