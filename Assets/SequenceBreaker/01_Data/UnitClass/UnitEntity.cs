using SequenceBreaker._00_System;
using UnityEngine;

namespace SequenceBreaker._01_Data.UnitClass
{
    [System.Serializable]
    public class UnitEntity
    {
        [SerializeField]public int uniqueId;
        public string name;
        public Affiliation affiliation;
        public UnitType unitType;
        public int itemCapacity;

        public CoreFrame coreFrame;

    }
}
