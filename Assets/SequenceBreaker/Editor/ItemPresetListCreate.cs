using SequenceBreaker.Master.Items;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    public class ItemPresetListCreate
    {
        [MenuItem("Assets/Create/ItemPreset")]
        public static ItemPresetList Create(string path)
        {
            ItemPresetList asset = AssetDatabase.LoadAssetAtPath(path, typeof(ItemPresetList)) as ItemPresetList;
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<ItemPresetList>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
            }

            return asset;
        }
    }

}