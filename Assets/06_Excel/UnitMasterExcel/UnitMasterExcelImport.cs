using System.Collections.Generic;
using UnityEngine;

namespace _06_Excel.UnitMasterExcel
{
	[ExcelAsset]
	public class UnitMasterExcelImport : ScriptableObject
	{
		public List<UnitMasterExcel> unitMasterExcel; // Replace 'EntityType' to an actual type that is serializable.
	}
}
