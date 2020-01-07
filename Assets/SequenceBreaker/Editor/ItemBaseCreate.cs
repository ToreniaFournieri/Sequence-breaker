using System.Collections;
using System.Collections.Generic;
using SequenceBreaker.Master.Items;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    public class ItemBaseCreate
    {
        [MenuItem("Assets/Create/Item")]
        public static ItemBaseMaster Create(string path)
        {
            ItemBaseMaster asset = AssetDatabase.LoadAssetAtPath(path, typeof(ItemBaseMaster)) as ItemBaseMaster;
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<ItemBaseMaster>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();

            }

            //asset = Item.CreateInstance<Item>();
            return asset;
        }


    }

}