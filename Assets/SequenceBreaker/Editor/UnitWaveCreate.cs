using SequenceBreaker.Master.Mission;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor
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
