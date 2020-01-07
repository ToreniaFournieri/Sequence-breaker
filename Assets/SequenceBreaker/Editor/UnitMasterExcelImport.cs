using System.Collections.Generic;
using _00_Asset._07_ExcelImporter;
using UnityEngine;

namespace SequenceBreaker.Editor
{
	[ExcelAsset]
	public class UnitMasterExcelImport : ScriptableObject
	{
		public List<UnitMasterExcel> unitMaster; // Replace 'EntityType' to an actual type that is serializable.
        public List<UnitEquipmentExcel> unitEquipment;
        public List<ItemBaseExcel> itemBase;
	}

}
