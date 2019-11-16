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
            unitWave.unitWave = new List<int>();
            string unitPath = "20_Enemy/" + "UnitMasterList";
            UnitMasterList unitMasterList = Resources.Load<UnitMasterList>(unitPath);

            if (unit1 != null) { unitWave.unitWave.Add(GetUnitClass(unitMasterList, unit1)); }
            if (unit2 != null) { unitWave.unitWave.Add(GetUnitClass(unitMasterList, unit2)); }
            if (unit3 != null) { unitWave.unitWave.Add(GetUnitClass(unitMasterList, unit3)); }
            if (unit4 != null) { unitWave.unitWave.Add(GetUnitClass(unitMasterList, unit4)); }
            if (unit5 != null) { unitWave.unitWave.Add(GetUnitClass(unitMasterList, unit5)); }
            if (unit6 != null) { unitWave.unitWave.Add(GetUnitClass(unitMasterList, unit6)); }
            if (unit7 != null) { unitWave.unitWave.Add(GetUnitClass(unitMasterList, unit7)); }
            
            return unitWave;
        }

        private int GetUnitClass(UnitMasterList unitMasterList ,string unitString)
        {
            if (unitString != null && unitMasterList != null)
            {
                foreach (var unit in unitMasterList.unitList)
                {
                    if (unit.unitName == unitString)
                    {
                        return unit.uniqueId;
                    }
                }
            }

            return -1;
        }


    }
}
