using System;

namespace _00_Asset._07_ExcelImporter.Example.Scripts.Entity
{
	[Serializable]
	public class MstItemEntity
	{
		public int id;
		public string name;
		public int price;
		public bool isNotForSale;
		public float rate;
		public MstItemCategory category;
	}

	public enum MstItemCategory
	{
		Red,
		Green,
		Blue,
	}
}