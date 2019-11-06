using UnityEditor;
using UnityEngine;

namespace SequenceBreaker._01_Data._03_UnitClass
{
    public class CreateUnitList : MonoBehaviour
    {
        [MenuItem("Assets/Create/Inventory Item List")]
        public static UnitClassList  Create()
        {
            UnitClassList asset = ScriptableObject.CreateInstance<UnitClassList>();
            

            AssetDatabase.CreateAsset(asset, "Assets/InventoryItemList.asset");
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}
