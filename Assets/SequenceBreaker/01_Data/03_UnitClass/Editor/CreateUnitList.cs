using UnityEditor;
using UnityEngine;

namespace SequenceBreaker._01_Data._03_UnitClass.Editor
{
    public static class CreateUnitList
    {
        [MenuItem("Assets/Create/UnitMaster")]
        public static UnitMasterList  Create()
        {
            UnitMasterList asset = ScriptableObject.CreateInstance<UnitMasterList>();
            

            AssetDatabase.CreateAsset(asset, "Assets/UnitMaster.asset");
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}
