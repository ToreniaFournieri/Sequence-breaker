using System.Collections;
using System.Collections.Generic;
using SequenceBreaker.Environment;
using SequenceBreaker.Master.BattleUnit;
using UnityEngine;

namespace SequenceBreaker.Play.Battle
{
    public class SkillMoveFunction
    {
        public static (BattleLogClass, BattleResultClass) Get(OrderClass order, List<BattleUnit> characters, EnvironmentInfoClass environmentInfo)
        {
            var battleResult = new BattleResultClass();
            var battleLog = new BattleLogClass();

            switch (order.SkillEffectChosen.skill.magnification.attackTarget)
            {
                case TargetType.Self: break;
                case TargetType.None: break;
                default:
                    var attack = new AttackFunction(order, characters, environmentInfo);
                    battleResult = attack.BattleResult;
                    battleLog = attack.BattleLog;
                    break;
            }
            return (battleLog, battleResult);
        }
    }
}