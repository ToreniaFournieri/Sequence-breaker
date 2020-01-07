using System;
using UnityEngine;

namespace SequenceBreaker.Master.Items
{
    [Serializable]
    public sealed class ItemForSave
    {

        // base of this item: baseItemId
        [SerializeField] public int bI;
        // prefix itemID
        [SerializeField] public int pI;

        // suffix, super-rare, itemID
        [SerializeField] public int sI;

        // Item enhanced Value: +1 or + 3 etc
        [SerializeField] public int eV;
        
        // Item amount:  x3 or x99 (max will be 99?)
        [SerializeField] public int am;
    }
}
