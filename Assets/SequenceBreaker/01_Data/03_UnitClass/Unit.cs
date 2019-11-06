using System.Collections.Generic;
using SequenceBreaker._00_System;
using SequenceBreaker._01_Data._02_Items.Item;
using SequenceBreaker._01_Data._08_BattleUnitSub;

namespace SequenceBreaker._01_Data._03_UnitClass
{
    public class Unit 
    {
        public int UniqueId;
        public string Name;
        public Affiliation Affiliation;
        public UnitType UnitType;
        public int ItemCapacity;
        public List<Item> ItemList;

        public CoreFrame CoreFrame;
        public Pilot.Pilot Pilot;
        public int Experience;
    }
}
