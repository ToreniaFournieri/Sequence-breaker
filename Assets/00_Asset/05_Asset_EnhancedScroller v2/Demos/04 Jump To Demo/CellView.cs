using _00_Asset._05_Asset_EnhancedScroller_v2.Plugins;
using UnityEngine.UI;

namespace _00_Asset._05_Asset_EnhancedScroller_v2.Demos._04_Jump_To_Demo
{
    public class CellView : EnhancedScrollerCellView
    {
        public Text cellText;

        public void SetData(Data data)
        {
            cellText.text = data.CellText;
        }
    }
}