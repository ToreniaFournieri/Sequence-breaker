using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker.Master.UnitClass
{
    public class UnitWave : ScriptableObject
    {

        // Header
        // only first missionCategory& missionId information is read. others are redundant.
        //public string missionCategory; 
        //public int missionId;
        //public string missionString;
        //public string locationString;
        //public int missionLevelInitial;

        //Item
        public List<UnitClass> unitWave;
    }
}
