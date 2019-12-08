using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker.Master.Items
{


    [Serializable]
    [CreateAssetMenu(fileName = "ItemPreset", menuName = "Item/ItemPreset", order = 7)]
    public class ItemPreset : ScriptableObject
    {
        public int characterUniqueId;

        public List<ItemIdSet> itemIdList;




    }

    [Serializable]
    public class ItemIdSet
    {
        public int prefixId;
        public int baseId;
        public int suffixId;
        public int enhancedValue;

        public ItemIdSet Add(int prefix, int id, int suffix, int enhance)
        {
            prefixId = prefix;
            baseId = id;
            suffixId = suffix;
            enhancedValue = enhance;

            return Copy();
        }

        public ItemIdSet Copy()
        {
            return (ItemIdSet)this.MemberwiseClone();
        }

        
    }

}
