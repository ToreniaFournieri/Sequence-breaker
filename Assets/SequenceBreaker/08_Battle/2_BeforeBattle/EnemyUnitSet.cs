using System.Collections.Generic;
using SequenceBreaker._01_Data._03_UnitClass;
using UnityEngine;

namespace SequenceBreaker._08_Battle._2_BeforeBattle
{
    [CreateAssetMenu(fileName = "EnemyUnitSet-", menuName = "Unit/EnemyUnitSet", order = 3)]
    public sealed class EnemyUnitSet : ScriptableObject
    {
        public List<UnitClass> enemyUnitList;

    }
}
