using UnityEditor;
using UnityEngine;

namespace SequenceBreaker._90_Test
{
    public class CreateInventoryItemList {
        [MenuItem("Assets/Create/Inventory Item List")]
        public static SInventoryItemList  Create()
        {
            SInventoryItemList asset = ScriptableObject.CreateInstance<SInventoryItemList>();
            

            AssetDatabase.CreateAsset(asset, "Assets/InventoryItemList.asset");
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}