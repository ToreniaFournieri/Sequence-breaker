using System.Collections;
using System.Collections.Generic;
using SequenceBreaker.Master.Mission;
using SequenceBreaker.Play.Prepare;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor._11_UnitSet
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