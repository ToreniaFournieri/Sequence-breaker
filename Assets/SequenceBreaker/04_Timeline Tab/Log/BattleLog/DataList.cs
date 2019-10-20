using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KohmaiWorks.Scroller
{
    [CreateAssetMenu(fileName = "DataList-", menuName = "Data/data", order = 5)]
    sealed public class DataList : ScriptableObject
    {
        public List<Data> Data;

    }

}
