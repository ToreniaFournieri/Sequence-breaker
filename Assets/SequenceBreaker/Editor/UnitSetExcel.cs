using System;
using System.Collections.Generic;
using SequenceBreaker._01_Data.UnitClass;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    [Serializable]
    public class UnitSetExcel
    {
        public int missionId;
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
            UnitWave unitWave = UnitWaveCreate.Create(path);
//            UnitWave unitWave = new UnitWave();
            unitWave.unitWave = new List<UnitClass>();
            string unitPath = "20_Enemy/" + "UnitClassList";
            UnitClassList unitClassList = Resources.Load<UnitClassList>(unitPath);

            Debug.Log("unit1:" + unit1 + " unit2:" + unit2);
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
