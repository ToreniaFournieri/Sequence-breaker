using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KohmaiWorks.Scroller
{
    [CreateAssetMenu(fileName = "DataList-", menuName = "Data/data", order = 5)]
    sealed public class DataList : ScriptableObject
    {
        [FormerlySerializedAs("Data")] public List<Data> data;

    }

}
