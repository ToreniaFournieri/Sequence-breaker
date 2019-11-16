
using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker._01_Data.UnitClass
{
    [CreateAssetMenu(fileName = "UnitSet", menuName = "Unit/UnitSet", order = 11)]

    public class UnitSet : ScriptableObject
    {
        public int missionId;
//        public int waveId;

        public List<UnitWave> unitSetList;
        
//                    public List<List<UnitClass>> unitSetList;

    }
}
