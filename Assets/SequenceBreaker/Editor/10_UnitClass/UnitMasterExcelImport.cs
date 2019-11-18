using System.Collections.Generic;
using _00_Asset._07_ExcelImporter;
using UnityEngine;

namespace SequenceBreaker.Editor._10_UnitClass
{
	[ExcelAsset]
	public class UnitMasterExcelImport : ScriptableObject
	{
		public List<UnitMasterExcel> unitMasterExcel; // Replace 'EntityType' to an actual type that is serializable.
	}
}
