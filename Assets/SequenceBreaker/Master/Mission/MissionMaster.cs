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

        [SerializeField]
        public CalculateUnitStatus calculateUnitStatus;



    }

}