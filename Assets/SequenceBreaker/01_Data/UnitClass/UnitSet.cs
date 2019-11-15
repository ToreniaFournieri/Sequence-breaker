
using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker._01_Data.UnitClass
{
    public class UnitSet : ScriptableObject
    {
        public int missionId;
        public int waveId;

        
        public List<List<UnitClass>> unitSetList;

    }
}
