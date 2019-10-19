using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyUnitSet-", menuName = "Unit/EnemyUnitSet", order = 3)]
sealed public class EnemyUnitSet : ScriptableObject
{
    public List<UnitClass> enemyUnitList;

}
