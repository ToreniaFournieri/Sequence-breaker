using System.Collections.Generic;
using _00_Asset._07_ExcelImporter.Example.Scripts.Entity;
using UnityEngine;

namespace _00_Asset._07_ExcelImporter.Example.Scripts.ExcelAsset
{
	[ExcelAsset]
	public class MstItems : ScriptableObject
	{
		public List<MstItemEntity> Entities; 
	}
}
