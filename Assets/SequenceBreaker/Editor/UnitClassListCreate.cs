using SequenceBreaker._01_Data.UnitClass;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    public class UnitClassListCreate
    {
        [MenuItem("Assets/Create/UnitClassList")]
        public static UnitClassList Create(string path)
        {
            UnitClassList asset = ScriptableObject.CreateInstance<UnitClassList>();
            
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}
