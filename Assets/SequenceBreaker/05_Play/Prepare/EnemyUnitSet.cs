using System.Collections.Generic;
using SequenceBreaker._01_Data.UnitClass;
using UnityEngine;

namespace SequenceBreaker._05_Play.Prepare
{
    [CreateAssetMenu(fileName = "EnemyUnitSet-", menuName = "Unit/EnemyUnitSet", order = 3)]
    public sealed class EnemyUnitSet : ScriptableObject
    {
        public List<UnitClass> enemyUnitList;

    }
}
