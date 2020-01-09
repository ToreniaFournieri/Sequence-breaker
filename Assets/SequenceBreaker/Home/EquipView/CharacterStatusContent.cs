using System.Collections;
using System.Collections.Generic;
using SequenceBreaker.Master.Units;
using SequenceBreaker.Translate;
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


            string bigText = Word.Get("Pilot") + " " + unitClass.pilot.pilotName + "(lv:" + unitClass.pilot.pilotLevel + ")";
            string upsideDescription = Word.Get("Unit Type") + ":" + unitClass.unitType + "     Unique Id: " + unitClass.uniqueId;

            CalculateUnitStatus.Get.Init(unitClass);

            string detailString = CalculateUnitStatus.Get.detailAbilityString;

            return (bigText, upsideDescription, detailString);
        }

        //public static string GetDetail2(UnitClass unitClass)
        //{
        //    CalculateUnitStatus.Get.Init(unitClass);
        //    string detailString = CalculateUnitStatus.Get.detailString2;

        //    return detailString;
        //}


    }
}
