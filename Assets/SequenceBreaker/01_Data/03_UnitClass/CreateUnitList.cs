using UnityEditor;
using UnityEngine;

namespace SequenceBreaker._01_Data._03_UnitClass
{
    public class CreateUnitList : MonoBehaviour
    {
        [MenuItem("Assets/Create/Unit List")]
        public static UnitListScriptable  Create()
        {
            UnitListScriptable asset = ScriptableObject.CreateInstance<UnitListScriptable>();
            

            AssetDatabase.CreateAsset(asset, "Assets/UnitListScriptable.asset");
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}
