using SequenceBreaker._01_Data.UnitClass;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    public static class UnitCreate 
    {
      
        [MenuItem("Assets/Create/UnitClass")]
        public static UnitClass Create(string path, UnitClass unit)
        {
            UnitClass asset = ScriptableObject.CreateInstance<UnitClass>();
            asset.Copy(unit);
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}
