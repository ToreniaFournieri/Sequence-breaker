using _00_Asset._05_Asset_EnhancedScroller_v2.Plugins;
using UnityEngine;
using UnityEngine.UI;

namespace _00_Asset._05_Asset_EnhancedScroller_v2.Demos._06_Snapping
{
    public class SlotCellView : EnhancedScrollerCellView
    {
        /// <summary>
        /// These are the UI elements that will be updated when the data changes
        /// </summary>
        public Image slotImage;

        /// <summary>
        /// This function sets up the data for the cell view
        /// </summary>
        /// <param name="data">The data to use</param>
        public void SetData(SlotData data)
        {
            // update the cell view's UI
            if (data.Sprite == null)
            {
                // this is a blank slot, so set the background color to no alpha
                slotImage.color = new Color(0, 0, 0, 0);
            }
            else
            {
                // this slot has an image so set its sprite
                slotImage.sprite = data.Sprite;
                slotImage.color = Color.white;
            }
        }
    }
}