using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace KohmaiWorks.Scroller
{
    public class CellView : EnhancedScrollerCellView
    {
        public Text reactText;
        public GameObject separateLine;
        public Text unitInfo;
        public Text firstLine;
        public Text mainText;
        public Image backgroundImage;
        public Image nestedLinePrevious;
        public Image nestedLine;
        public GameObject iconMask;
        public Image unitIcon;
        public Image hPBar;
        public Image shieldBar;

        public GameObject headerContent;
        public Text HeaderInfo;
        public ShowTargetUnit showTargetUnit;
        public SimpleObjectPool unitIconObjectPool;


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

        public void SetData(Data data, Data nextData, bool calculateLayout, Sprite unitSprite)
        //public void SetData(BattleLogClass battleLogLists, bool calculateLayout)
        {
            reactText.text = data.reactText;

            if (data.isHeaderInfo)
            {
                HeaderInfo.text = data.headerText;
                showTargetUnit.unitIconObjectPool = unitIconObjectPool;
                showTargetUnit.SetUnitInfo(data.characters);
                headerContent.SetActive(true);
            }
            else
            {
                //Need to false incase recycle object.
                headerContent.SetActive(false);
            }

            unitInfo.text = data.unitInfo;
            firstLine.text = data.firstLine;
            mainText.text = data.mainText;
            //someTextText.text = battleLogLists.Log;
            Color darkRed = new Color32(24, 12, 12, 255); // dark red
            Color darkGreen = new Color32(12, 24, 12, 255); // dark green
            Color color = new Color32(17, 17, 17, 255);
            Color unitColor = new Color32(120, 120, 120, 255);

            unitIcon.color = unitColor;

            if (unitSprite != null)
            {
                unitIcon.sprite = unitSprite;
                iconMask.SetActive(true);
            }
            else
            {
                iconMask.SetActive(false);
            }

            hPBar.fillAmount = data.hPRatio;
            shieldBar.fillAmount = data.shieldRatio;

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


            if (data.nestLevel > 0)
            {
                separateLine.SetActive(false);
            }
            else
            {
                separateLine.SetActive(true);
            }
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