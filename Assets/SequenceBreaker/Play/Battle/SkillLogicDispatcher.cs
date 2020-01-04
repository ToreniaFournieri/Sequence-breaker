using System.Collections;
using System.Collections.Generic;
using SequenceBreaker.Environment;
using SequenceBreaker.Master.BattleUnit;
using UnityEngine;

namespace SequenceBreaker.Play.Battle
{
    public class SkillLogicDispatcher
    {
        public static (string firstline, List<string> logList) Get(OrderClass order, List<BattleUnit> characters, EnvironmentInfoClass environmentInfo)
        {
            string firstLine = null;
            List<string> logList = null;
            if (order.SkillEffectChosen.skill.callSkillLogicName == CallSkillLogicName.None) { return (null, null); } // check call skill 
            SkillLogicShieldHealClass healMulti;
            switch (order.SkillEffectChosen.skill.callSkillLogicName)
            {
                case CallSkillLogicName.ShieldHealMulti:
                    healMulti = new SkillLogicShieldHealClass(order, characters, true, environmentInfo);
                    logList = healMulti.LogList;
                    firstLine = healMulti.FirstLine;
                    break;
                case CallSkillLogicName.ShieldHealSingle:
                    healMulti = new SkillLogicShieldHealClass(order, characters, false, environmentInfo);
                    logList = healMulti.LogList;
                    firstLine = healMulti.FirstLine;
                    break;
            }
            return (firstLine, logList);
        }
    }
}