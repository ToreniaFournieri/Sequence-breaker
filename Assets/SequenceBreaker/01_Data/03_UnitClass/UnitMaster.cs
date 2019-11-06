using System.Collections.Generic;
using System.Diagnostics;
using SequenceBreaker._00_System;
using SequenceBreaker._01_Data._02_Items.Item;
using SequenceBreaker._01_Data._08_BattleUnitSub;
using UnityEngine;

namespace SequenceBreaker._01_Data._03_UnitClass
{
    [System.Serializable]
    public class UnitMaster
    {
        public string unitName;
        public int uniqueId;
        public Affiliation affiliation;
        public UnitType unitType;
        public int itemCapacity;
        public List<Item> itemList;

        public CoreFrame coreFrame;
        public Pilot.Pilot pilot;
        public int experience;
        public bool autoGenerationMode;
        
        
        public UnitClass GetUnitClass()
        {
            UnitClass unitClass = new UnitClass();

            unitClass.name = unitName;
            unitClass.uniqueId = uniqueId;
            unitClass.affiliation = affiliation;
            unitClass.unitType = unitType;
            unitClass.itemCapacity = itemCapacity;
//            unitClass.itemList = itemList;
            unitClass.coreFrame = coreFrame;
            unitClass.pilot = pilot;
            unitClass.level = 1;
            unitClass.experience = experience;
            return unitClass;
            
        }

    }
    
    
}
