using System;
using UnityEngine;

namespace _00_Asset._05_EnhancedScroller_v2.Plugins
{
    /// <summary>
    /// This is the base class that all cell views should derive from
    /// </summary>
    public class EnhancedScrollerCellView : MonoBehaviour
    {
        /// <summary>
        /// The cellIdentifier is a unique string that allows the scroller
        /// to handle different types of cells in a single list. Each type
        /// of cell should have its own identifier
        /// </summary>
        public string cellIdentifier;

        /// <summary>
        /// The cell index of the cell view
        /// This will differ from the dataIndex if the list is looping
        /// </summary>
        [NonSerialized]
        public int CellIndex;

        /// <summary>
        /// The data index of the cell view
        /// </summary>
        [NonSerialized]
        public int DataIndex;

        /// <summary>
        /// Whether the cell is active or recycled
        /// </summary>
        [NonSerialized]
        public bool Active;

        /// <summary>
        /// This method is called by the scroller when the RefreshActiveCellViews is called on the scroller
        /// You can override it to update your cell's view UID
        /// </summary>
        public virtual void RefreshCellView() { }
    }
}