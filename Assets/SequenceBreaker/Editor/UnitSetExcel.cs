using System;
using System.Collections.Generic;
using SequenceBreaker._01_Data.UnitClass;

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

        public List<UnitClass> GetUnitSet()
        {
            List<UnitClass> unitList = new List<UnitClass>();
            
            
            
            
            return unitList;
        }


    }
}
