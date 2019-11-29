using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker.Master.Items
{


    [Serializable]
    public class ItemPreset : ScriptableObject
    {
        public int characterUniqueId;

        public List<ItemIdSet> itemIdList;




    }

    public class ItemPresetList : ScriptableObject
    {
        public List<ItemPreset> itemPresetList;



        static ItemPreset _instance;
        public static ItemPreset Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ItemPreset>();
                }
                return _instance;
            }

        }

        //public List<ItemIdSet> ItemFromId(int uniqueId)
        //{
        //    foreach (var itemPreset in itemPresetList)
        //    {
        //        if (uniqueId == itemPreset.characterUniqueId)
        //        {
        //            return itemPreset.itemIdList;
        //        }

        //    }

        //    List<ItemIdSet> none = new List<ItemIdSet>();

        //    return none;
        //}

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
