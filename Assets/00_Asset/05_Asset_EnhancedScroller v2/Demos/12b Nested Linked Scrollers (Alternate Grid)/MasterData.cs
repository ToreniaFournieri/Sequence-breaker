using System.Collections.Generic;

namespace _00_Asset._05_Asset_EnhancedScroller_v2.Demos
{
    /// <summary>
    /// Main cell view data
    /// </summary>
    public class MasterData
    {
        // This value will store the position of the detail scroller to be used 
        // when the scroller's cell view is recycled
        public float NormalizedScrollPosition;

        public float LinkedScrollPosition;

        public List<DetailData> ChildData;
    }
}
