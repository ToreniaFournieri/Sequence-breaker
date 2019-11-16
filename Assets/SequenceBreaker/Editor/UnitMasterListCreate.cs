using SequenceBreaker._01_Data.UnitClass;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    public static class UnitMasterListCreate
    {
        [MenuItem("Assets/Create/UnitMaster")]
        public static UnitMasterList  Create(string pathAndName)
        {
            UnitMasterList asset = ScriptableObject.CreateInstance<UnitMasterList>();
            
Debug.Log("Created. path: " + pathAndName);
            AssetDatabase.CreateAsset(asset, pathAndName);
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}
