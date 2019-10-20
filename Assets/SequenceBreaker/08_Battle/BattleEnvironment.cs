using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker._08_Battle
{
    [CreateAssetMenu(fileName = "battleEnvironment", menuName = "Battle/Environment", order = 2)]
    public sealed class BattleEnvironment : ScriptableObject
    {
        // Environment values
        public SkillsMasterClass normalAttackSkillsMaster;
        public List<SkillsMasterClass> buffMasters;

    }
}
