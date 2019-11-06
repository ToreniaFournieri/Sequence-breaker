namespace _00_Asset._05_Asset_EnhancedScroller_v2.Plugins
{
    /// <summary>
    /// All scripts that handle the scroller's callbacks should inherit from this interface
    /// </summary>
    public interface IEnhancedScrollerDelegate
    {
        /// <summary>
        /// Gets the number of cells in a list of data
        /// </summary>
        /// <param unitName="scroller"></param>
        /// <returns></returns>
        int GetNumberOfCells(EnhancedScroller scroller);

        /// <summary>
        /// Gets the size of a cell view given the index of the data set.
        /// This allows you to have different sized cells
        /// </summary>
        /// <param unitName="scroller"></param>
        /// <param unitName="dataIndex"></param>
        /// <returns></returns>
        float GetCellViewSize(EnhancedScroller scroller, int dataIndex);

        /// <summary>
        /// Gets the cell view that should be used for the data index. Your implementation
        /// of this function should request a new cell from the scroller so that it can
        /// properly recycle old cells.
        /// </summary>
        /// <param unitName="scroller"></param>
        /// <param unitName="dataIndex"></param>
        /// <param unitName="cellIndex"></param>
        /// <returns></returns>
        EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex);
    }
}