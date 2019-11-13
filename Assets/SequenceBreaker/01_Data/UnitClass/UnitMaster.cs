using System.Collections.Generic;
using SequenceBreaker._00_System;
using SequenceBreaker._01_Data.Items.Item;

namespace SequenceBreaker._01_Data.UnitClass
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
            unitClass.coreFrame = coreFrame;
            unitClass.pilot = pilot;
            unitClass.level = 1;
            unitClass.experience = experience;
            return unitClass;
            
        }

    }
    
    
}
