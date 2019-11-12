using System;
using System.Collections.Generic;
using SequenceBreaker._00_System;
using SequenceBreaker._01_Data._02_Items.Item;
using SequenceBreaker._01_Data._08_BattleUnitSub;
using UnityEngine;

namespace SequenceBreaker._01_Data._03_UnitClass
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
