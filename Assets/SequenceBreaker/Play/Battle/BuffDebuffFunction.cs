using System.Collections;
using System.Collections.Generic;
using SequenceBreaker.Environment;
using SequenceBreaker.Master.BattleUnit;
using SequenceBreaker.Master.Skills;
using SequenceBreaker.Play.Battle;
using SequenceBreaker.Translate;
using UnityEngine;

namespace SequenceBreaker.Play.Battle
{

    public static class BuffDebuffFunction
    {
        public static (string firstLine, List<string> logList) Get(OrderClass order, List<BattleUnit> characters, List<EffectClass> effects, List<SkillsMasterClass> buffMasters, int turn)
        {
            SkillsMasterClass addingBuff;
            var addingEffect = new List<EffectClass>();
            string firstLine = null;
            List<string> logList = new List<string>();
            if (order.SkillEffectChosen == null) { return (null, null); } // no effect exist, so no buff/ debuff happened
            foreach (var character in characters) { if (character.IsCrushedJustNow) { character.IsCrushedJustNow = false; } } // reset IsCrushedJustNow flag
            switch (order.SkillEffectChosen.skill.buffTarget.targetType)
            {
                case TargetType.Self: //Buff self
                    addingBuff = buffMasters.FindLast(obj => obj.skillId == order.SkillEffectChosen.skill.callingBuffName.skillId);
                    //                    addingEffect.Add(new EffectClass(order.Actor, addingBuff, ActionType.None,
                    //                        1.0, 0.0, false, addingBuff.usageCount,
                    //                        turn, (turn + addingBuff.veiledTurn)));
                    EffectClass effectClass = ScriptableObject.CreateInstance<EffectClass>();
                    effectClass.Set(order.Actor, addingBuff, ActionType.None,
                        1.0, 0.0, false, addingBuff.usageCount,
                        turn, (turn + addingBuff.vailedTurn));

                    addingEffect.Add(effectClass);
                    effects.Add(addingEffect[0]);
                    addingEffect[0].BuffToCharacter(turn);
                    order.Actor.buff.AddBarrier(addingEffect[0].skill.buffTarget.barrierRemaining);

                    string triggerPossibilityText = null;
                    if (order.SkillEffectChosen.triggeredPossibility < 1.0) { triggerPossibilityText = "(" + (int)(order.SkillEffectChosen.triggeredPossibility * 1000) / 10.0 + "%) "; }
                    string accumulationText = null;
                    if (order.SkillEffectChosen.NextAccumulationCount > 0)
                    {
                        var count = 0.0;
                        switch (order.SkillEffectChosen.skill.triggerBase.accumulationReference)
                        {
                            case ReferenceStatistics.None: break;
                            case ReferenceStatistics.AvoidCount: count = order.Actor.Statistics.AvoidCount; break;
                            case ReferenceStatistics.AllHitCount: count = order.Actor.Statistics.AllHitCount; break;
                            case ReferenceStatistics.AllTotalBeenHitCount: count = order.Actor.Statistics.AllTotalBeenHitCount; break;
                            case ReferenceStatistics.CriticalBeenHitCount: count = order.Actor.Statistics.CriticalBeenHitCount; break;
                            case ReferenceStatistics.CriticalHitCount: count = order.Actor.Statistics.CriticalHitCount; break;
                            case ReferenceStatistics.SkillBeenHitCount: count = order.Actor.Statistics.SkillBeenHitCount; break;
                            case ReferenceStatistics.SkillHitCount: count = order.Actor.Statistics.SkillHitCount; break;
                        }
                        string nextText = " "
                                + Word.Get("Next: A (X)", order.SkillEffectChosen.UsageCount.ToString(), order.SkillEffectChosen.NextAccumulationCount.ToString());
                        accumulationText = "(" + order.SkillEffectChosen.skill.triggerBase.accumulationReference + ": " + count + nextText + ")";
                    }

                    firstLine = order.SkillEffectChosen.skill.skillName + "! " + triggerPossibilityText + accumulationText;

                    //log += order.Actor.shortName + " gets " + addingBuff.skillName + " which will last " + addingBuff.veiledTurn + " turns." ;
                    //logList += order.Actor.shortName + Word.Get("gets A (valid for X turns).", addingBuff.veiledTurn.ToString(), addingBuff.skillName);
                    logList.Add(order.Actor.shortName + Word.Get("gets A (valid for X turns).", addingBuff.vailedTurn.ToString(), addingBuff.skillName));

                    if (addingBuff.buffTarget.defenseMagnification > 1.0)
                    {
                        //logList += "\n" + new string(' ', 4) + "[" + Word.Get("Defense") + ": " + order.Actor.buff.DefenseMagnification
                        //    + " (x" + addingBuff.buffTarget.defenseMagnification + ")] ";
                        logList.Add(new string(' ', 4) + "[" + Word.Get("Defense") + ": " + order.Actor.buff.DefenseMagnification
                            + " (x" + addingBuff.buffTarget.defenseMagnification + ")] ");
                    }
                    if (addingEffect[0].skill.buffTarget.barrierRemaining > 0)
                    {
                        //logList += "[" + Word.Get("Barrier") + ":" + order.Actor.buff.BarrierRemaining + " (+" + addingEffect[0].skill.buffTarget.barrierRemaining + ")] ";
                        logList.Add("[" + Word.Get("Barrier") + ":" + order.Actor.buff.BarrierRemaining + " (+" + addingEffect[0].skill.buffTarget.barrierRemaining + ")] ");
                    }
                    //logList += "\n";
                    break;
                case TargetType.Multi: //Buff attacker's side all
                    addingBuff = buffMasters.FindLast(obj => obj.skillId == order.SkillEffectChosen.skill.callingBuffName.skillId);
                    var buffTargetCharacters = characters.FindAll(character1 => character1.affiliation == order.Actor.affiliation && character1.combat.hitPointCurrent > 0);
                    firstLine = order.SkillEffectChosen.skill.skillName
                                + "! (" + Word.Get("Trigger Possibility") + ":" + (int)(order.SkillEffectChosen.triggeredPossibility * 1000) / 10.0 + "%) ";

                    for (var i = 0; i < buffTargetCharacters.Count; i++)
                    {
                        EffectClass effectClass2 = ScriptableObject.CreateInstance<EffectClass>();
                        effectClass2.Set(buffTargetCharacters[i], addingBuff, ActionType.None,
                            1.0, 0.0, false, addingBuff.usageCount,
                            turn, (turn + addingBuff.vailedTurn));

                        addingEffect.Add(effectClass2);
                        effects.Add(addingEffect[i]);

                        addingEffect[i].BuffToCharacter(turn);
                        buffTargetCharacters[i].buff.AddBarrier(addingEffect[i].skill.buffTarget.barrierRemaining);

                        //log += new string(' ', 3) + buffTargetCharacters[i].shortName + " gets " + addingBuff.skillName + " which will last " + addingBuff.veiledTurn + " turns." + "\n";
                        //logList += new string(' ', 3) + buffTargetCharacters[i].shortName + Word.Get("gets A (valid for X turns).", addingBuff.veiledTurn.ToString(), addingBuff.skillName);
                        logList.Add(new string(' ', 3) + buffTargetCharacters[i].shortName + Word.Get("gets A (valid for X turns).", addingBuff.vailedTurn.ToString()
                            , addingBuff.skillName));
                        if (addingBuff.buffTarget.defenseMagnification > 1.0)
                        //{ logList += "\n" + new string(' ', 4) + "[" + Word.Get("Defense") + ": " + buffTargetCharacters[i].buff.DefenseMagnification + " (x" + addingBuff.buffTarget.defenseMagnification + ")] "; }
                        {
                            logList.Add(new string(' ', 4) + "[" + Word.Get("Defense") + ": " + buffTargetCharacters[i].buff.DefenseMagnification
                                + " (x" + addingBuff.buffTarget.defenseMagnification + ")] ");
                        }
                        if (addingEffect[i].skill.buffTarget.barrierRemaining > 0)
                        {
                            //logList += " [" + Word.Get("Barrier") + ": " + buffTargetCharacters[i].buff.BarrierRemaining + " (+" + addingEffect[i].skill.buffTarget.barrierRemaining + ")] \n";
                            logList.Add(" [" + Word.Get("Barrier") + ": " + buffTargetCharacters[i].buff.BarrierRemaining + " (+"
                                + addingEffect[i].skill.buffTarget.barrierRemaining + ")] ");
                        }
                    }
                    //log += "\n";
                    break;
                case TargetType.None: break;
            }

            if (order.SkillEffectChosen.skill.debuffTarget.targetType != TargetType.None)
            {
                //Debuff exist
            }
            return (firstLine, logList);
        }
    }

}