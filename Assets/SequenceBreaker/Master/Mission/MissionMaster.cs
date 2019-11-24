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
        public string missionName;

        public string category; //Identify the missions, mainstory or some sub story.
        public int Id;
        public string locationString;
        public int levelInitial;


        //unit List
        public UnitSet unitSet;


        public CalculateUnitStatus calculateUnitStatus;



    }

}