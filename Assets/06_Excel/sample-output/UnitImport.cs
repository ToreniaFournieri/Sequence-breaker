using System.Collections.Generic;
using SequenceBreaker._01_Data._03_UnitClass;
using UnityEngine;

namespace _06_Excel
{
	[ExcelAsset]
	public class UnitImport : ScriptableObject
	{
		public List<UnitEntity> unit; // Replace 'EntityType' to an actual type that is serializable.
	}
}
