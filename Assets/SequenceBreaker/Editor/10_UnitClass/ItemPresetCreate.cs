using System.Collections;
using System.Collections.Generic;
using SequenceBreaker.Master.Items;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor._10_UnitClass
{
    public class ItemPresetCreate
    {
        [MenuItem("Assets/Create/ItemPreset")]
        public static ItemPreset Create(string path)
        {
            ItemPreset asset = ScriptableObject.CreateInstance<ItemPreset>();
            //asset.Copy(unitClassList);
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            return asset;
        }
    }

}