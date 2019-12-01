using SequenceBreaker.Master.Items;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    public class ItemPresetCreate
    {
        [MenuItem("Assets/Create/ItemPreset")]
        public static ItemPreset Create(string path)
        {
            ItemPreset asset = AssetDatabase.LoadAssetAtPath(path, typeof(ItemPreset)) as ItemPreset;
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<ItemPreset>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();

            }

            return asset;
        }
    }

}