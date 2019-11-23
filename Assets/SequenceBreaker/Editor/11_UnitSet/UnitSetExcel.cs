using System;
using System.Collections.Generic;
using SequenceBreaker.Master.Mission;
using SequenceBreaker.Master.UnitClass;
using SequenceBreaker.Play.Prepare;
using UnityEngine;

namespace SequenceBreaker.Editor._11_UnitSet
{
    [Serializable]
    public class UnitSetExcel
    {

        // header data
        public string missionCategory; //Identify the missions, mainstory or some sub story.
        public int missionId;
        public string missionString;
        public string locationString;
        public int missionLevelInitial;


        // header data and fixed one.
        //public string calculateUnitStatus;


        // item data
        public int waveId;
        
        // order UnitClass ScriptableObject name
        public string unit1;
        public string unit2;
        public string unit3;
        public string unit4;
        public string unit5;
        public string unit6;
        public string unit7;

        public UnitWave GetUnitSet(string path)
        { 
            //MissionMaster mission = MissionCreate.Create(path);
            UnitWave unitWave = new UnitWave();

            string unitPath = "20_Enemy/" + "UnitClassList";
            UnitClassList unitClassList = Resources.Load<UnitClassList>(unitPath);

//            Debug.Log("unit1:" + unit1 + " unit2:" + unit2);
            if (unit1 != "") { unitWave.unitWave.Add(GetUnitClass(unitClassList, unit1)); }
            if (unit2 != "") { unitWave.unitWave.Add(GetUnitClass(unitClassList, unit2)); }
            if (unit3 != "") { unitWave.unitWave.Add(GetUnitClass(unitClassList, unit3)); }
            if (unit4 != "") { unitWave.unitWave.Add(GetUnitClass(unitClassList, unit4)); }
            if (unit5 != "") { unitWave.unitWave.Add(GetUnitClass(unitClassList, unit5)); }
            if (unit6 != "") { unitWave.unitWave.Add(GetUnitClass(unitClassList, unit6)); }
            if (unit7 != "") { unitWave.unitWave.Add(GetUnitClass(unitClassList, unit7)); }
            
            return unitWave;
        }

        private UnitClass GetUnitClass(UnitClassList unitClassList ,string unitString)
        {
            if (unitString != null && unitClassList != null)
            {
                foreach (var unit in unitClassList.unitList)
                {
                    if (unit.name == unitString)
                    {
                        return unit;
                    }
                }
            }

            return null;
        }


    }
}
