using System.Collections;
using System.Collections.Generic;
using SequenceBreaker.Master.Mission;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    public class MissionListCreate
    {

        [MenuItem("Assets/Create/Mission")]
        public static MissionMasterList Create(string path)
        {
            MissionMasterList asset = AssetDatabase.LoadAssetAtPath(path, typeof(MissionMasterList)) as MissionMasterList;
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<MissionMasterList>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();

            }

            return asset;
        }
    }

}