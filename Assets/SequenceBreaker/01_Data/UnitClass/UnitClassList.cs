using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker._01_Data.UnitClass
{
    [CreateAssetMenu(fileName = "UnitClassList-", menuName = "Unit/UnitClassList", order = 24)]
    public class UnitClassList : ScriptableObject
    {
        public List<UnitClass> unitList;
    }
}
