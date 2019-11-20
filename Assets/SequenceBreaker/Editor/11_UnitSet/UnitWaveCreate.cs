using SequenceBreaker.Master.UnitClass;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor._11_UnitSet
{
    public class UnitWaveCreate
    {
        [MenuItem("Assets/Create/UnitWave")]
        public static Runbattle Create(string pathAndName)
        {
            Runbattle asset = ScriptableObject.CreateInstance<Runbattle>();
            
            
            AssetDatabase.CreateAsset(asset, pathAndName);
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}
