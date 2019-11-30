using SequenceBreaker.Master.Mission;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    public class MissionCreate
    {
        [MenuItem("Assets/Create/Mission")]
        public static MissionMaster Create(string pathAndName)
        {
            MissionMaster asset = ScriptableObject.CreateInstance<MissionMaster>();

            //RunBattle asset = ScriptableObject.CreateInstance<RunBattle>();


            AssetDatabase.CreateAsset(asset, pathAndName);
            AssetDatabase.SaveAssets();
            return asset;
        }
    }

}