using SequenceBreaker._01_Data.UnitClass;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    public static class CreateUnitList
    {
        [MenuItem("Assets/Create/UnitMaster")]
        public static UnitMasterList  Create(string pathAndName)
        {
            UnitMasterList asset = ScriptableObject.CreateInstance<UnitMasterList>();
            
Debug.Log("path: " + pathAndName);
            AssetDatabase.CreateAsset(asset, pathAndName);
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}
