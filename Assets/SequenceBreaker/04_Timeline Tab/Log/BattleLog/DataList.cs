using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._04_Timeline_Tab.Log.BattleLog
{
    [CreateAssetMenu(fileName = "DataList-", menuName = "Data/data", order = 5)]
    public sealed class DataList : ScriptableObject
    {
        [FormerlySerializedAs("Data")] public List<Data> data;

    }

}
