using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "battleEnvironment", menuName = "Battle/Environment", order = 2)]
public class BattleEnvironment : ScriptableObject
{
    // Environment values
    public SkillsMasterClass normalAttackSkillsMaster;
    public List<SkillsMasterClass> buffMasters;

}
