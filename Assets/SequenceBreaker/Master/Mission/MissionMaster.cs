using System.Collections;
using System.Collections.Generic;
using SequenceBreaker.Master.UnitClass;
using UnityEngine;

namespace SequenceBreaker.Master.Mission
{
    [CreateAssetMenu(fileName = "Mission-", menuName = "Mission/MissionMaster", order = 1)]
    public class MissionMaster : ScriptableObject
    {

        // Mission information
        public string missionText;
        public string location;
        public int missionLevelInitial;


        //unit List
        public UnitSet unitSet;


        public CalculateUnitStatus calculateUnitStatus;



    }

}