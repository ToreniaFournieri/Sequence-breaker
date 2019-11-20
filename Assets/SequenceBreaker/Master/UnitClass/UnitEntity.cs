using SequenceBreaker.Environment;
using UnityEngine;

namespace SequenceBreaker.Master.UnitClass
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
