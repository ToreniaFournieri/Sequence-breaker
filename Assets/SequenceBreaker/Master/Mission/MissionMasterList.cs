using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SequenceBreaker.Master.Mission
{
    [CreateAssetMenu(fileName = "MissionMasterList", menuName = "Mission/MissionMasterList", order = 1)]
    public class MissionMasterList : ScriptableObject
    {
        public List<MissionMaster> missionMasterList;



    
    }
}

