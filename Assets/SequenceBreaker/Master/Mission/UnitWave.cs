using System.Collections.Generic;
using SequenceBreaker.Master.Units;
using UnityEngine;

namespace SequenceBreaker.Master.Mission
{

    [CreateAssetMenu(fileName = "UnitWave-", menuName = "Unit/UnitWave", order = 11)]
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

        //level depend on the wave
        public int level;

    }
}
