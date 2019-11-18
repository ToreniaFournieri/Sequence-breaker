using SequenceBreaker._01_Data.UnitClass;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor._11_UnitSet
{
    public class UnitSetCreate 
    {
        [MenuItem("Assets/Create/UnitSet")]
        public static UnitSet Create(string pathAndName)
        {
            UnitSet asset = ScriptableObject.CreateInstance<UnitSet>();
            
            AssetDatabase.CreateAsset(asset, pathAndName);
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}
