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

        public static (string bigTextString, string upsideDescriptionString, string popupDescriptionString) Get(UnitClass unitClass)
        {
            string bigText = unitClass.TrueName();
            string upsideDescription = unitClass.unitType + " Item Capacity: " + unitClass.GetItemAmount() + "/"+ unitClass.itemCapacity;
            string popupDescription = unitClass.shortName + " Popup Description is here.";

            return (bigText, upsideDescription, popupDescription);
        }




    }
}
