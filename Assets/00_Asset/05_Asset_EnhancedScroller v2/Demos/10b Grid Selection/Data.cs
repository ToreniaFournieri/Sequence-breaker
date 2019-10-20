namespace _00_Asset._05_Asset_EnhancedScroller_v2.Demos._10b_Grid_Selection
{
    public delegate void SelectedChangedDelegate(bool val);

    /// <summary>
    /// Data class to store information
    /// </summary>
    public class Data
    {
        public string SomeText;

        public SelectedChangedDelegate SelectedChanged;

        /// <summary>
        /// The selection state
        /// </summary>
        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                // if the value has changed
                if (_selected != value)
                {
                    // update the state and call the selection handler if it exists
                    _selected = value;
                    if (SelectedChanged != null) SelectedChanged(_selected);
                }
            }
        }
    }
}