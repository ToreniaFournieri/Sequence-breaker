using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SequenceBreaker.Environment;
using SequenceBreaker.Master.BattleUnit;
using SequenceBreaker.Master.Skills;
using UnityEngine;

namespace SequenceBreaker.Play.Battle
{ 
public class SkillTriggerPossibilityCheck 
{
    // Skill check method
    public static Stack<OrderClass> Get(BattleUnit actor, List<EffectClass> effects, List<BattleUnit> characters, OrderClass attackerOrder,
        Stack<OrderClass> orders, ActionType actionType, bool shouldHeal, bool isRescue,
        BattleResultClass battleResult, BattleUnit individualTarget, int nestNumber, EnvironmentInfoClass environmentInfo)
    {
        if (attackerOrder != null && attackerOrder.IsRescue) { return null; } //If previous move is Rescue no counter, re-attack, chain and Rescue is triggered.
        List<EffectClass> rawActionTypeEffects;
        if (isRescue) // Rescue is ActionType independent
        {
            rawActionTypeEffects = effects.FindAll(obj => obj.UsageCount > 0 && obj.character.combat.hitPointCurrent > 0 &&
                                                            obj.skill.isHeal == shouldHeal && obj.VeiledFromTurn <= environmentInfo.Turn && obj.VeiledToTurn >= environmentInfo.Turn);
        }
        else
        {
            if (shouldHeal) //if heal has, be selected.
            {
                rawActionTypeEffects = effects.FindAll(obj => obj.ActionType == actionType && obj.UsageCount > 0 && obj.character.combat.hitPointCurrent > 0 &&
                                                                obj.skill.isHeal && obj.VeiledFromTurn <= environmentInfo.Turn && obj.VeiledToTurn >= environmentInfo.Turn);
                if (!rawActionTypeEffects.Any()) //if no heal skill left, other move skill should be selected.
                {
                    rawActionTypeEffects = effects.FindAll(obj => obj.ActionType == actionType && obj.UsageCount > 0 && obj.character.combat.hitPointCurrent > 0 &&
                                                                    obj.skill.isHeal == false && obj.VeiledFromTurn <= environmentInfo.Turn && obj.VeiledToTurn >= environmentInfo.Turn);
                }
            }
            else // should not heal, so find other move skill.
            {
                rawActionTypeEffects = effects.FindAll(obj => obj.ActionType == actionType && obj.UsageCount > 0 && obj.character.combat.hitPointCurrent > 0 &&
                                                                obj.skill.isHeal == false && obj.VeiledFromTurn <= environmentInfo.Turn && obj.VeiledToTurn >= environmentInfo.Turn);
            }
        }
        var matchedActionTypeEffects = new List<EffectClass>();
        var counterAffiliation = Affiliation.Ally;
        if (attackerOrder != null) //Memo: at Beginning and move skills, attackOrder is null.
        {
            counterAffiliation = attackerOrder.Actor.affiliation == Affiliation.Ally ? Affiliation.Enemy : Affiliation.Ally;
        }

        var affiliationWhoWillAct = Affiliation.None;
        switch (actionType) // Get actionType dependent condition before calculation.
        {
            case ActionType.Move: //Normal moveSkill logic: only actor should trigger moveSkill.
                matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => obj.character == actor);
                break;
            case ActionType.Counter:
                if (attackerOrder != null && attackerOrder.Actor.affiliation == Affiliation.Ally) { affiliationWhoWillAct = Affiliation.Enemy; }
                else if (attackerOrder != null && attackerOrder.Actor.affiliation == Affiliation.Enemy) { affiliationWhoWillAct = Affiliation.Ally; }
                matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => obj.character.affiliation == affiliationWhoWillAct);
                break;
            case ActionType.Chain:
                matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => attackerOrder != null && (obj.character.affiliation == attackerOrder.Actor.affiliation && obj.character != attackerOrder.Actor));
                break;
            case ActionType.ReAttack:
                matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => attackerOrder != null && obj.character == attackerOrder.Actor);
                break;
            case ActionType.AtBeginning:
                matchedActionTypeEffects = rawActionTypeEffects;
                break;
            case ActionType.Any: //[Rescue skill logic]. ActionType independent so DCA is in ActionType.any.
                if (isRescue) // Rescue skill logic
                {
                    // Actor's affiliation character is dead just now?
                    var crushedJustNowCounterAffiliationCharacter = characters.FindAll(obj => obj.IsCrushedJustNow && obj.affiliation == counterAffiliation);
                    if (crushedJustNowCounterAffiliationCharacter.Count > 0) // Rescue required!
                    { matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => obj.character.affiliation == counterAffiliation && obj.character.feature.damageControlAssist); }
                    // in case of friendly fired.
                    var crushedJustNowByFriendlyFiredCharacter = characters.FindAll(obj => attackerOrder != null && (obj.IsCrushedJustNow && obj.affiliation == attackerOrder.Actor.affiliation));
                    if (crushedJustNowByFriendlyFiredCharacter.Count > 0)
                    { matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => attackerOrder != null && (obj.character.affiliation == attackerOrder.Actor.affiliation && obj.character.feature.damageControlAssist)); }
                }
                break;
            default: matchedActionTypeEffects = new List<EffectClass>(); break;
        }

        //push order from slow character's effect to fast character's effect. It means pop from fast character's effect to slow character's effect.
        matchedActionTypeEffects.Sort((x, y) => y.character.ability.responsiveness - x.character.ability.responsiveness);

        var validEffects = new List<EffectClass>();
        foreach (var effect in matchedActionTypeEffects)
        {
            effect.IsntTriggeredBecause.Initialize();
            if (effect.skill.triggerTarget.actionType != ActionType.Any)
            {
                if ((effect.skill.triggerTarget.actionType == attackerOrder.ActionType) == false)
                { effect.IsntTriggeredBecause.TriggerCondition = true; continue; }
            } // Trigger condition check
            if (effect.skill.actionType != ActionType.Move && effect.skill.triggerTarget.afterAllMoved == false) // check normal Attack left, except move skill. (move skill is itself so check this logic make no sense )
            {
                var checkOrders = orders.ToList();
                if (checkOrders.FindLast(obj => obj.Actor == effect.character && obj.SkillEffectChosen.skill.isNormalAttack) == null)
                { effect.IsntTriggeredBecause.AfterAllMoved = true; continue; }// no normalAttack left, which means no action.   
            }


            if (effect.skill.actionType == ActionType.Move && effect.IsRescueAble) //Rescue Special Logic....
            {
                // check normal Attack left, except move skill. (move skill is itself so check this logic make no sense )
                if (effect.skill.triggerTarget.afterAllMoved == false)
                {
                    var checkActionOrders = orders.ToList();
                    if (checkActionOrders.FindLast(obj => obj.Actor == effect.character && obj.ActionType == ActionType.Move) == null)
                    { effect.IsntTriggeredBecause.AfterAllMoved = true; continue; }// no normalAttack left, which means no action.   
                }
            }

            if (attackerOrder != null) // only attackOrder exist, check
            {
                if (effect.skill.triggerTarget.counter == false && attackerOrder.ActionType == ActionType.Counter)
                { effect.IsntTriggeredBecause.TriggerTargetCounter = true; continue; } // counter reaction
                if (effect.skill.triggerTarget.chain == false && attackerOrder.ActionType == ActionType.Chain)
                { effect.IsntTriggeredBecause.TriggerTargetChain = true; continue; } // chain reaction
                if (effect.skill.triggerTarget.reAttack == false && attackerOrder.ActionType == ActionType.ReAttack)
                { effect.IsntTriggeredBecause.TriggerTargetReAttack = true; continue; } // reAttack reaction
                if (effect.skill.triggerTarget.move == false && attackerOrder.ActionType == ActionType.Move)
                { effect.IsntTriggeredBecause.TriggerTargetMove = true; continue; } // move skill reaction
            }


            if (effect.skill.triggerTarget.optimumRange != Range.Any && attackerOrder != null) // within OptimumRange check
            {
                var aliveActorSide = characters.FindAll(character1 => character1.affiliation == effect.character.affiliation && character1.combat.hitPointCurrent > 0);
                var aliveAttackerIndex = aliveActorSide.IndexOf(effect.character);
                var minTargetOptimumRange = (int)(effect.character.combat.minRange * effect.skill.magnification.optimumRangeMin) - aliveAttackerIndex;
                var maxTargetOptimumRange = (int)(effect.character.combat.maxRange * effect.skill.magnification.optimumRangeMax) - aliveAttackerIndex;
                counterAffiliation = effect.character.affiliation == Affiliation.Ally ? Affiliation.Enemy : Affiliation.Ally;
                var survivedOpponents = characters.FindAll(character1 => character1.combat.hitPointCurrent > 0 && character1.affiliation == counterAffiliation);
                survivedOpponents.Sort((x, y) => x.uniqueId - y.uniqueId);
                var attackerIndex = survivedOpponents.IndexOf(attackerOrder.Actor);

                switch (effect.skill.triggerTarget.optimumRange) //Optimum Range check.
                {
                    case Range.Any: break;
                    case Range.Within: if (attackerIndex >= minTargetOptimumRange && attackerIndex <= maxTargetOptimumRange) { break; } else { continue; }
                    case Range.Without: if (attackerIndex >= minTargetOptimumRange && attackerIndex <= maxTargetOptimumRange) { continue; } else { break; }
                }
            }


            // AttackType MajestyAttackType NO IMPLEMENTATION.

            if (effect.skill.triggerTarget.critical != CriticalOrNot.Any)
            {
                if (effect.skill.triggerTarget.critical == CriticalOrNot.Critical && battleResult.CriticalOrNot == CriticalOrNot.NonCritical)
                { effect.IsntTriggeredBecause.Critical = true; continue; } // non critical but only when critical triggers
                if (effect.skill.triggerTarget.critical == CriticalOrNot.NonCritical && battleResult.CriticalOrNot == CriticalOrNot.Critical)
                { effect.IsntTriggeredBecause.NonCritical = true; continue; } // critical but only when non critical triggers
            }


            //ActorOrTargetUnit WhoCrushed   NO IMPLEMENTATION.

            if (effect.skill.triggerTarget.onlyWhenBeenHitMoreThanOnce && (battleResult.HitMoreThanOnceCharacters.Find(obj => obj == effect.character) == null))
            { effect.IsntTriggeredBecause.OnlyWhenBeenHitMoreThanOnce = true; continue; } //Being hit .this means not hit, so skill should not be triggered.
            if (effect.skill.triggerTarget.onlyWhenAvoidMoreThanOnce && ((battleResult.AvoidMoreThanOnceCharacters.Find(obj => obj == effect.character)) == null))
            { effect.IsntTriggeredBecause.OnlyWhenAvoidMoreThanOnce = true; continue; } //being avoid. this means not hit, so skill should not be triggered.

            switch (effect.skill.triggerBase.accumulationReference) //Trigger Accumulation check
            {
                case ReferenceStatistics.None: break;
                case ReferenceStatistics.AvoidCount:
                    if (effect.character.Statistics.AvoidCount < effect.NextAccumulationCount)
                    { effect.IsntTriggeredBecause.AccumulationAvoid = true; continue; }
                    break;
                case ReferenceStatistics.AllHitCount:
                    if (effect.character.Statistics.AllHitCount < effect.NextAccumulationCount)
                    { effect.IsntTriggeredBecause.AccumulationAllHitCount = true; continue; }
                    break;
                case ReferenceStatistics.AllTotalBeenHitCount:
                    if (effect.character.Statistics.AllTotalBeenHitCount < effect.NextAccumulationCount)
                    { effect.IsntTriggeredBecause.AccumulationAllTotalBeenHit = true; continue; }
                    break;
                case ReferenceStatistics.CriticalBeenHitCount:
                    if (effect.character.Statistics.CriticalBeenHitCount < effect.NextAccumulationCount)
                    { effect.IsntTriggeredBecause.AccumulationCriticalBeenHit = true; continue; }
                    break;
                case ReferenceStatistics.CriticalHitCount:
                    if (effect.character.Statistics.CriticalHitCount < effect.NextAccumulationCount)
                    { effect.IsntTriggeredBecause.AccumulationCriticalHit = true; continue; }
                    break;
                case ReferenceStatistics.SkillBeenHitCount:
                    if (effect.character.Statistics.SkillBeenHitCount < effect.NextAccumulationCount)
                    { effect.IsntTriggeredBecause.AccumulationSkillBeenHit = true; continue; }
                    break;
                case ReferenceStatistics.SkillHitCount:
                    if (effect.character.Statistics.SkillHitCount < effect.NextAccumulationCount)
                    { effect.IsntTriggeredBecause.AccumulationSkillHit = true; continue; }
                    break;
            }

            var possibility = environmentInfo.R.Next(0, 1000) / 1000.0; //TriggerPossibility Check
            if (effect.triggeredPossibility >= possibility) { validEffects.Add(effect); }
            else
            {
                effect.IsntTriggeredBecause.TriggeredPossibility = true;
            }
        }

        //set order  grouped by actors
        var skillsByOrderStack = new Stack<OrderClass>();
        foreach (var character in characters)
        {
            var validEffectsPerActor = validEffects.FindAll(obj => obj.character == character);
            if (validEffectsPerActor.Count >= 1)
            {
                var orderNumber = 0; var nest = 0; if (attackerOrder != null) { orderNumber = attackerOrder.OrderCondition.OrderNumber; nest = attackerOrder.OrderCondition.Nest; }
                var addCount = 0;

                if (attackerOrder != null)
                {
                    if (attackerOrder.ActionType == ActionType.Counter)
                    { addCount = 1; }
                    if (attackerOrder.ActionType == ActionType.Chain)
                    { addCount = 1; }
                    if (attackerOrder.ActionType == ActionType.ReAttack)
                    { addCount = 1; }

                    if (attackerOrder.ActionType == ActionType.Move)
                    {
                        //Sample dummy implement
                        addCount = 1;
                    }
                }
                var orderCondition = new OrderConditionClass(environmentInfo.Wave, environmentInfo.Turn, environmentInfo.Phase, orderNumber,
                    nest + addCount, nestNumber, 0);

                var skillsByOrder = new OrderClass(orderCondition, character, actionType, ref validEffectsPerActor, 0,
                    individualTarget, isRescue);
                skillsByOrderStack.Push(skillsByOrder); nestNumber++;
            }
        }
        if (skillsByOrderStack.Count > 0) { return skillsByOrderStack; }
        return null;
    }
}

}