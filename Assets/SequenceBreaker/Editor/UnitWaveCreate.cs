using SequenceBreaker.Master.Mission;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    public class UnitWaveCreate
    {
        [MenuItem("Assets/Create/UnitWave")]
        public static UnitWave Create(string path)
        {
            UnitWave asset = AssetDatabase.LoadAssetAtPath(path, typeof(UnitWave)) as UnitWave;

            //UnitWave asset = ScriptableObject.CreateInstance<UnitWave>();
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<UnitWave>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                return asset;
            }

            return asset;
        }
    }
}
