using System.Collections.Generic;
using SequenceBreaker._01_Data.Items.Item;
using SequenceBreaker._01_Data.UnitClass;

namespace SequenceBreaker._02_Drop
{
    public sealed class DropEngine 
    {
        // This value is temporary.
        private double _itemDropRatio = 0.100;

        // Internal information
        private List<UnitClass> _enemyUnitList;
        private List<Item> _dropItemList;

        public List<Item> GetDroppedItems(List<UnitClass> enemyUnitList, int seed)
        {
            _dropItemList = new List<Item>();
            System.Random r = new System.Random(seed);

            //1.Correct all item of enemy units
            foreach (UnitClass unit in enemyUnitList)
            {
                foreach (Item item in unit.itemList)
                {
                    //2.[UNIMPLEMENTED] Remove undroppable item if it is.                

                    //3.Drop judgement using seed. NextDouble range is between 0.0 to 1.0.
                    if (_itemDropRatio >= r.NextDouble())
                    {
                        _dropItemList.Add(item);
                    }
                }
            }

            return _dropItemList;
        }

    }
}
