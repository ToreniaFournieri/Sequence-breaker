using System;
using System.Collections.Generic;
using SequenceBreaker._00_System;
using SequenceBreaker._01_Data.UnitClass;
using SequenceBreaker._01_Data.UnitClass.Pilot;
using UnityEngine;

namespace _06_Excel.UnitMasterExcel
{
    [Serializable]
    public class UnitMasterExcel
    {
        public string unitName;
        public int uniqueId;
        public Affiliation affiliation;
        public UnitType unitType;
        public int itemCapacity;
        public string coreFrameString;
        public string pilotString;
        public int level;
        
        
        public UnitMaster GetUnitMaster()
        {
         UnitMaster unitMaster = new UnitMaster();
         unitMaster.unitName = unitName;
         unitMaster.uniqueId = uniqueId;
         unitMaster.affiliation = affiliation;
         unitMaster.unitType = unitType;
         unitMaster.itemCapacity = itemCapacity;
         string coreFramePath = "11_Unit-Base-Master/01_CoreFrame/" +coreFrameString;
         unitMaster.coreFrame = Resources.Load<CoreFrame>(coreFramePath);
         string pilotPath = "11_Unit-Base-Master/02_Pilot/" + pilotString ;
         unitMaster.pilot = Resources.Load<Pilot>(pilotPath);
         unitMaster.autoGenerationMode = false;

         return unitMaster;
         // level not used...

         //        public int uniqueId;
//        public Affiliation affiliation;
//        public UnitType unitType;
//        public int itemCapacity;
//        public List<Item> itemList;
//
//        public CoreFrame coreFrame;
//        public Pilot.Pilot pilot;
//        public int experience;
//        public bool autoGenerationMode;
//



        }
    }

 
    
}
