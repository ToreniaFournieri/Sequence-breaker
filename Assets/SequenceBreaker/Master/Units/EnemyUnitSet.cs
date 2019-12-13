using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker.Master.Units
{
    [CreateAssetMenu(fileName = "EnemyUnitSet-", menuName = "Unit/EnemyUnitSet", order = 3)]
    public sealed class EnemyUnitSet : ScriptableObject
    {
        public List<UnitClass> enemyUnitList;

    }
}
