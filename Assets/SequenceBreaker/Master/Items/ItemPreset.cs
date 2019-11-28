using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker.Master.Items
{
    public class ItemPreset : ScriptableObject
    {
        public List<ItemIdSet> itemIdList;

    }

    public class ItemIdSet
    {
        public int PrifixId;
        public int BaseId;
        public int SuffixId;
        public int enhancedValue;

    }

}