using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker.Master.Items
{

    [CreateAssetMenu(fileName = "ItemBaseMasterList", menuName = "Item/ItemBaseMasterList", order = 0)]
    public class ItemBaseMasterList : ScriptableObject
    {
        public List<ItemBaseMaster> itemBaseMasterList;

    }
}