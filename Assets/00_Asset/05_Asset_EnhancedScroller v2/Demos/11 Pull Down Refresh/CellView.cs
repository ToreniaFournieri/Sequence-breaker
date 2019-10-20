using _00_Asset._05_Asset_EnhancedScroller_v2.Plugins;
using UnityEngine.UI;

namespace _00_Asset._05_Asset_EnhancedScroller_v2.Demos._11_Pull_Down_Refresh
{
    /// <summary>
    /// This is the view of our cell which handles how the cell looks.
    /// </summary>
    public class CellView : EnhancedScrollerCellView
    {
        /// <summary>
        /// A reference to the UI Text element to display the cell data
        /// </summary>
        public Text someTextText;

        /// <summary>
        /// This function just takes the Demo data and displays it
        /// </summary>
        /// <param name="data"></param>
        public void SetData(Data data)
        {
            // update the UI text with the cell data
            someTextText.text = data.SomeText;
        }
    }
}