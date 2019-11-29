using System.Collections;
using System.Collections.Generic;
using SequenceBreaker.Master.Items;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor._10_UnitClass
{
    public class ItemPresetListCreate
    {
        [MenuItem("Assets/Create/ItemPreset")]
        public static ItemPresetList Create(string path)
        {
            ItemPresetList asset = ScriptableObject.CreateInstance<ItemPresetList>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            return asset;
        }
    }

}