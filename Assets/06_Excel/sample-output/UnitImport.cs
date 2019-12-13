using System.Collections.Generic;
using _00_Asset._07_ExcelImporter;
using SequenceBreaker.Master.Units;
using UnityEngine;

namespace _06_Excel
{
	[ExcelAsset]
	public class UnitImport : ScriptableObject
	{
		public List<UnitEntity> unit; // Replace 'EntityType' to an actual type that is serializable.
	}
}
