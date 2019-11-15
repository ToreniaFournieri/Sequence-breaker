using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker._01_Data.UnitClass
{
    [CreateAssetMenu(fileName = "EnemyUnitSet-", menuName = "Unit/EnemyUnitSet", order = 3)]
    public sealed class EnemyUnitSet : ScriptableObject
    {
        public List<UnitClass> enemyUnitList;

    }
}
