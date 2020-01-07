using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker.Master.Mission
{    
    [CreateAssetMenu(fileName = "Mission-", menuName = "Mission/MissionMaster", order = 1)]
    public class MissionMaster : ScriptableObject
    {

        // Mission information
        [SerializeField]
        public string missionName;

        [SerializeField]
        public string category; //Identify the missions, mainstory or some sub story.
        [SerializeField]
        public int Id;
        [SerializeField]
        public string locationString;
        [SerializeField]
        public int levelInitial;


        //unit List
        [SerializeField]
        public UnitSet unitSet;

        //[SerializeField]
        //public CalculateUnitStatus calculateUnitStatus;
        //public CalculateUnitStatusMaster calculateUnitStatusMaster;


        //public void Copy(MissionMaster _mission)
        //{
        //    missionName = _mission.missionName;
        //    category = _mission.category;
        //    Id = _mission.Id;
        //    locationString = _mission.locationString;
        //    levelInitial = _mission.levelInitial;

        //    if (_mission.unitSet != null)
        //    {
        //        unitSet = _mission.unitSet;
        //    }


        //}


    }

}