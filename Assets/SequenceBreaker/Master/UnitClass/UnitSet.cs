using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker.Master.UnitClass
{
    [CreateAssetMenu(fileName = "UnitSet", menuName = "Unit/UnitSet", order = 11)]
    public class UnitSet : ScriptableObject
    {
        public int missionId;
//        public int waveId;

        public List<Runbattle> unitSetList;
        
//                    public List<List<UnitClass>> unitSetList;

    }
}
