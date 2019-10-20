using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed public class DropEngine 
{
    // This value is temporary.
    private double itemDropRatio = 0.100;

    // Internal infomation
    private List<UnitClass> enemyUnitList;
    private List<Item> dropItemList;

    public List<Item> GetDropedItems(List<UnitClass> enemyUnitList, int seed)
    {
        dropItemList = new List<Item>();
        System.Random r = new System.Random(seed);

        //1.Correct all item of enemy units
        foreach (UnitClass unit in enemyUnitList)
        {
            foreach (Item item in unit.itemList)
            {
                //2.[UNIMPLEMENTED] Remove undropable item if it is.                

                //3.Drop judgement using seed. NextDouble range is between 0.0 to 1.0.
                if (itemDropRatio >= r.NextDouble())
                {
                    dropItemList.Add(item);
                }
            }
        }

        return dropItemList;
    }

}
