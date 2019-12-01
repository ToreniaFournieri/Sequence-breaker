using SequenceBreaker.Master.UnitClass;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    public class UnitClassListCreate
    {
        [MenuItem("Assets/Create/UnitClassList")]
        public static UnitClassList Create(string path)
        {
            UnitClassList asset = AssetDatabase.LoadAssetAtPath(path, typeof(UnitClassList)) as UnitClassList;
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<UnitClassList>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();

            }

            //UnitClassList asset = ScriptableObject.CreateInstance<UnitClassList>();
            ////asset.Copy(unitClassList);
            //AssetDatabase.CreateAsset(asset, path);
            //AssetDatabase.SaveAssets();
            return asset;
        }
    }
}
