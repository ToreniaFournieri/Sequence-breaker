using System.Collections;
using System.Collections.Generic;
using SequenceBreaker.Master.Units;
using UnityEngine;

namespace SequenceBreaker.Home.EquipView
{

    public static class CharacterStatusContent
    {
        //public string bigTextString;
        //public string upsideDescriptionString;
        //public string popupDescriptionString;

        public static (string bigTextString, string upsideDescriptionString, string detailString) Get(UnitClass unitClass)
        {
            string bigText = unitClass.TrueName();
            string upsideDescription = unitClass.unitType + " Item Capacity: " + unitClass.GetItemAmount() + "/" + unitClass.itemCapacity;

            CalculateUnitStatus.Get.Init(unitClass);

            string detailString = CalculateUnitStatus.Get.detailAbilityString;

            return (bigText, upsideDescription, detailString);
        }




    }
}
