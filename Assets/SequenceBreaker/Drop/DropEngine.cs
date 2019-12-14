using System.Collections.Generic;
using SequenceBreaker.Master.Items;
using SequenceBreaker.Master.Units;

namespace SequenceBreaker.Drop
{
    public sealed class DropEngine
    {
        // This value is temporary.
        private double _itemDropRatio = 0.100;

        // Internal information
        private List<UnitClass> _enemyUnitList;
        private List<Item> _dropItemList;

        //public List<Item> GetDroppedItems(List<UnitClass> enemyUnitList, int seed)
        public List<Item> GetDroppedItems(List<UnitClass> enemyUnitList, System.Random random)
        {
            _dropItemList = new List<Item>();
            //System.Random r = new System.Random(seed);

            //1.Correct all item of enemy units
            foreach (UnitClass unit in enemyUnitList)
            {

                unit.UpdateItemCapacity();

                int _itemCapacityCount = 0;

                foreach (Item item in unit.itemList)
                {
                    if (_itemCapacityCount >= unit.itemCapacity)
                    {
                        continue;
                    }
                    _itemCapacityCount++;

                    //2.[UNIMPLEMENTED] Remove undroppable item if it is.                

                    //3.Drop judgement using seed. NextDouble range is between 0.0 to 1.0.
                    if (_itemDropRatio >= random.NextDouble())
                    {
                        _dropItemList.Add(item);
                    }
                }
            }

            return _dropItemList;
        }

    }
}
