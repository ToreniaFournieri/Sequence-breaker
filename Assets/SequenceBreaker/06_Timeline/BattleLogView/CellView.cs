using _00_Asset._05_EnhancedScroller_v2.Plugins;
using SequenceBreaker._00_System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SequenceBreaker._06_Timeline.BattleLogView
{
    public sealed class CellView : EnhancedScrollerCellView
    {
        public Text reactText;
        public GameObject separateLine;
        public Text unitInfo;
        public Text firstLine;
        public Text mainText;
        public Image backgroundImage;
        public Image nestedLinePrevious;
        public Image nestedLine;

        // Battle Unit icon
        public GameObject mainUnit;
        public GameObject iconMask;
        public Image unitIcon;
        public Image hPBar;
        public Image shieldBar;
        public GameObject barrierObject;
        public Text barrierRemains;

        public GameObject headerContent;
        [FormerlySerializedAs("HeaderInfo")] public Text headerInfo;
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
                headerInfo.text = data.headerText;
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
            Color darkRed = new Color32(36, 20, 20, 255); // dark red
            Color darkGreen = new Color32(20, 36, 20, 255); // dark green
            Color color = new Color32(24, 24, 24, 255);
            Color unitColor = new Color32(120, 120, 120, 255);

            unitIcon.color = unitColor;

            if (data.isDead == true)
            {
                mainUnit.SetActive(false);
            } else
            {
                mainUnit.SetActive(true);
            }

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

            //Set barrier remains 
            if (data.barrierRemains > 0)
            { barrierObject.SetActive(true); }
            else
            { barrierObject.SetActive(false); }
            barrierRemains.text = data.barrierRemains.ToString();

            switch (data.affiliation)
            {
                case Affiliation.Ally:
                    color = darkGreen;
                    break;
                case Affiliation.Enemy:
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