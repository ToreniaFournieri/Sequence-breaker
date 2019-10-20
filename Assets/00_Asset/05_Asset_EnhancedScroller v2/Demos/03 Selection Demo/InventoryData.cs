using UnityEngine;
using System.Collections;

namespace EnhancedScrollerDemos.SelectionDemo
{
    /// <summary>
    /// This delegate handles any changes to the selection state of the data
    /// </summary>
    /// <param name="val">The state of the selection</param>
    public delegate void SelectedChangedDelegate(bool val);

    /// <summary>
    /// This class represents an inventory record
    /// </summary>
    public class InventoryData
    {
        /// <summary>
        /// The name of the inventory item
        /// </summary>
        public string ItemName;

        /// <summary>
        /// The cost of the inventory item
        /// </summary>
        public int ItemCost;

        /// <summary>
        /// The damage the item can do
        /// </summary>
        public int ItemDamage;

        /// <summary>
        /// The armor the item provides
        /// </summary>
        public int ItemDefense;

        /// <summary>
        /// The weight of the item
        /// </summary>
        public int ItemWeight;

        /// <summary>
        /// This description of the inventory item
        /// </summary>
        public string ItemDescription;

        /// <summary>
        /// The path to the resources folder for the sprite
        /// representing this inventory item
        /// </summary>
        public string SpritePath;

        /// <summary>
        /// The delegate to call if the data's selection state
        /// has changed. This will update any views that are hooked
        /// to the data so that they show the proper selection state UI.
        /// </summary>
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