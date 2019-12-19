using SequenceBreaker.Master.Units;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    public static class UnitCreate 
    {
      
        [MenuItem("Assets/Create/UnitClass")]
        public static UnitClass Create(string path, UnitClass unit)
        {
            string pathWithName = path + "/" + unit.uniqueId + "-" + unit.shortName + ".asset";

            UnitClass asset = AssetDatabase.LoadAssetAtPath(pathWithName, typeof(UnitClass)) as UnitClass;
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<UnitClass>();
                AssetDatabase.CreateAsset(asset, pathWithName);
                AssetDatabase.SaveAssets();

            }

            asset.Copy(unit);
            //UnitClass asset = ScriptableObject.CreateInstance<UnitClass>();
            //AssetDatabase.CreateAsset(asset, pathWithName);
            //AssetDatabase.SaveAssets();
            return asset;
        }
    }
}
