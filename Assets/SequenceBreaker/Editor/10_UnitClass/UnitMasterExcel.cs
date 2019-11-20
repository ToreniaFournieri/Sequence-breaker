using System;
using SequenceBreaker.Environment;
using SequenceBreaker.Master.UnitClass;
using UnityEngine;

namespace SequenceBreaker.Editor._10_UnitClass
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

		public UnitClass GetUnitClass()
		{
			UnitClass unit = ScriptableObject.CreateInstance<UnitClass>();
			unit.name = unitName;
			unit.uniqueId = uniqueId;
			unit.affiliation = affiliation;
			unit.unitType = unitType;
			unit.itemCapacity = itemCapacity;
			string coreFramePath = "11_Unit-Base-Master/01_CoreFrame/" + coreFrameString;
			unit.coreFrame = Resources.Load<CoreFrame>(coreFramePath);
			string pilotPath = "11_Unit-Base-Master/02_Pilot/" + pilotString;
			unit.pilot = Resources.Load<Pilot>(pilotPath);
			unit.level = 1;

			return unit;

		}

	}



 
    
}
