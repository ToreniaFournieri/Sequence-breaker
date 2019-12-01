using SequenceBreaker.Master.Mission;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    public class MissionCreate
    {
        [MenuItem("Assets/Create/Mission")]
        public static MissionMaster Create(string path)
        {
            MissionMaster asset = AssetDatabase.LoadAssetAtPath(path, typeof(MissionMaster)) as MissionMaster;
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<MissionMaster>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();

            }

            //MissionMaster asset = ScriptableObject.CreateInstance<MissionMaster>();

            //AssetDatabase.CreateAsset(asset, pathAndName);
            //AssetDatabase.SaveAssets();
            return asset;
        }
    }

}