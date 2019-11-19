using SequenceBreaker._01_Data.UnitClass;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor._11_UnitSet
{
    public class UnitWaveCreate
    {
        [MenuItem("Assets/Create/UnitWave")]
        public static UnitWave Create(string pathAndName)
        {
            UnitWave asset = ScriptableObject.CreateInstance<UnitWave>();
            
            
            AssetDatabase.CreateAsset(asset, pathAndName);
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}
