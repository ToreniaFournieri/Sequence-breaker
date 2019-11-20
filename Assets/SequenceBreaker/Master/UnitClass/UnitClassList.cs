using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker.Master.UnitClass
{
    [CreateAssetMenu(fileName = "UnitClassList-", menuName = "Unit/UnitClassList", order = 24)]
    public class UnitClassList : ScriptableObject
    {
        public List<UnitClass> unitList;
    }
}
