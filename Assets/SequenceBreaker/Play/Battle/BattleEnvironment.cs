using System.Collections.Generic;
using SequenceBreaker.Master.Skills;
using UnityEngine;

namespace SequenceBreaker.Play.Battle
{
    [CreateAssetMenu(fileName = "battleEnvironment", menuName = "Battle/Environment", order = 2)]
    public sealed class BattleEnvironment : ScriptableObject
    {
        // Environment values
        public SkillsMasterClass normalAttackSkillsMaster;
        public List<SkillsMasterClass> buffMasters;

    }
}
