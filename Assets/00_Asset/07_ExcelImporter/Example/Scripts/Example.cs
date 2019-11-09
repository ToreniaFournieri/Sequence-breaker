using _00_Asset._07_ExcelImporter.Example.Scripts.Entity;
using _00_Asset._07_ExcelImporter.Example.Scripts.ExcelAsset;
using UnityEngine;
using UnityEngine.UI;

namespace _00_Asset._07_ExcelImporter.Example.Scripts
{
	public class Example : MonoBehaviour
	{
		[SerializeField] MstItems mstItems;
		[SerializeField] Text text;

		void Start()
		{
			ShowItems();
		}

		void ShowItems()
		{
			string str = "";

			mstItems.Entities
				.ForEach(entity => str += DescribeMstItemEntity(entity) + "\n");

			text.text = str;
		}

		string DescribeMstItemEntity(MstItemEntity entity)
		{
			return string.Format(
				"{0} : {1}, {2}, {3}, {4}, {5}",
				entity.id,
				entity.name,
				entity.price,
				entity.isNotForSale,
				entity.rate,
				entity.category
			);
		}
	}
}

