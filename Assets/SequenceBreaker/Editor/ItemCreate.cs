using System.Collections;
using System.Collections.Generic;
using SequenceBreaker.Master.Items;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    public class ItemCreate
    {
        [MenuItem("Assets/Create/Item")]
        public static Item Create(string path)
        {
            Item asset = AssetDatabase.LoadAssetAtPath(path, typeof(Item)) as Item;
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<Item>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();

            }

            //asset = Item.CreateInstance<Item>();
            return asset;
        }


    }

}