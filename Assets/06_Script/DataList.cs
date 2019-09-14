using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KohmaiWorks.Scroller
{
    [CreateAssetMenu(fileName = "DataList-", menuName = "Data/data", order = 5)]
    public class DataList : ScriptableObject
    {
        public List<Data> Data;

    }

}
