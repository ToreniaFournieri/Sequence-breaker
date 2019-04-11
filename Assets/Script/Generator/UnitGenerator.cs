using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UnitGenerator : MonoBehaviour
{
    public int numberOfCharactersAlly = 7;
    public int numberOfCharactersEnemy = 7;
    List<BattleUnit> characters = new List<BattleUnit>();
    AbilityClass[] abilities;


    //void Awake()
    //{

    //    abilities = new AbilityClass[numberOfCharactersAlly + numberOfCharactersEnemy];

    //    //Skill make logic , test: one character per one skill
    //    SkillsMasterStruct[] skillsMasters = new SkillsMasterStruct[numberOfCharactersAlly + numberOfCharactersEnemy + 1];
    //    List<SkillsMasterStruct> buffMasters = new List<SkillsMasterStruct>();

    //    //Effect (permament skill) make logic, the effect has two meanings, one is permanent skill, the other is temporary skill (may call buff).
    //    List<EffectClass> effects = new List<EffectClass>();

    //    //ally info
    //    abilities[0] = new AbilityClass(power: 27, generation: 11, stability: 22, responsiveness: 32, precision: 22, intelligence: 21, luck: 34); //PIG1-
    //    abilities[1] = new AbilityClass(power: 29, generation: 13, stability: 22, responsiveness: 24, precision: 22, intelligence: 14, luck: 15); //PIG2-
    //    abilities[2] = new AbilityClass(power: 27, generation: 10, stability: 22, responsiveness: 21, precision: 25, intelligence: 21, luck: 34); //PIG3-
    //    abilities[3] = new AbilityClass(power: 29, generation: 10, stability: 22, responsiveness: 24, precision: 22, intelligence: 14, luck: 15); //PIG4-
    //    abilities[4] = new AbilityClass(power: 27, generation: 10, stability: 22, responsiveness: 32, precision: 22, intelligence: 21, luck: 34); //PIG5-
    //    abilities[5] = new AbilityClass(power: 29, generation: 12, stability: 22, responsiveness: 24, precision: 22, intelligence: 30, luck: 15); //PIG6-
    //    abilities[6] = new AbilityClass(power: 27, generation: 11, stability: 22, responsiveness: 20, precision: 22, intelligence: 21, luck: 34); //PIG7-
    //                                                                                                                                                         //enemy info
    //    abilities[7] = new AbilityClass(power: 29, generation: 8, stability: 22, responsiveness: 23, precision: 22, intelligence: 14, luck: 15); //ELD1- 
    //    abilities[8] = new AbilityClass(power: 27, generation: 13, stability: 22, responsiveness: 31, precision: 22, intelligence: 21, luck: 34); //ELD2-
    //    abilities[9] = new AbilityClass(power: 29, generation: 8, stability: 22, responsiveness: 22, precision: 22, intelligence: 14, luck: 15); //ELD3-
    //    abilities[10] = new AbilityClass(power: 27, generation: 9, stability: 22, responsiveness: 30, precision: 22, intelligence: 21, luck: 34); //ELD4-
    //    abilities[11] = new AbilityClass(power: 29, generation: 8, stability: 22, responsiveness: 25, precision: 22, intelligence: 14, luck: 15); //ELD5-
    //    abilities[12] = new AbilityClass(power: 27, generation: 11, stability: 22, responsiveness: 36, precision: 22, intelligence: 21, luck: 34); //ELD6-
    //    abilities[13] = new AbilityClass(power: 29, generation: 12, stability: 22, responsiveness: 20, precision: 22, intelligence: 14, luck: 15); //ELD7-

    //    BattleUnit.OffenseMagnificationClass offenseMagnification =
    //     new BattleUnit.OffenseMagnificationClass(optimumRangeBonus: 1.2, critical: 1.4, kinetic: 1.0, chemical: 1.0, thermal: 1.4, vsBeast: 1.0, vsCyborg: 2.44, vsDrone: 1.0, vsRobot: 2.2, vsTitan: 1.0);

    //    BattleUnit.DefenseMagnificationClass defenseMagnification =
    //        new BattleUnit.DefenseMagnificationClass(critical: 1.0, kinetic: 1.0, chemical: 1.0, thermal: 1.0, vsBeast: 1.0, vsCyborg: 1.0, vsDrone: 1.0, vsRobot: 1.2, vsTitan: 1.0);

    //    BattleUnit.SkillMagnificationClass.ActionSkillClass skillActionSkillInitial = new BattleUnit.SkillMagnificationClass.ActionSkillClass(move: 1.0, heal: 1.0, counter: 1.0, chain: 1.0, reAttack: 1.0,
    //        interrupt: 1.0, atBeginning: 1.0, atEnding: 1.0);
    //    BattleUnit.SkillMagnificationClass.ActionSkillClass skillActionSkillAllDouble = new BattleUnit.SkillMagnificationClass.ActionSkillClass(move: 2.0, heal: 1.0, counter: 2.0, chain: 2.0, reAttack: 2.0,
    //        interrupt: 2.0, atBeginning: 2.0, atEnding: 2.0);
    //    BattleUnit.SkillMagnificationClass.ActionSkillClass skillActionSkillAllTriple = new BattleUnit.SkillMagnificationClass.ActionSkillClass(move: 3.0, heal: 1.0, counter: 3.0, chain: 3.0, reAttack: 3.0,
    //        interrupt: 3.0, atBeginning: 3.0, atEnding: 3.0);

    //    BattleUnit.SkillMagnificationClass skillMagnificationAllInitial = new BattleUnit.SkillMagnificationClass(offenseEffectPower: skillActionSkillInitial, triggerPossibility: skillActionSkillInitial);
    //    BattleUnit.SkillMagnificationClass skillMagnificationAllDouble = new BattleUnit.SkillMagnificationClass(offenseEffectPower: skillActionSkillAllDouble, triggerPossibility: skillActionSkillAllDouble);
    //    BattleUnit.SkillMagnificationClass skillMagnificationOffenseDoubleTriggerTriple = new BattleUnit.SkillMagnificationClass(offenseEffectPower: skillActionSkillAllDouble, triggerPossibility: skillActionSkillAllTriple);

    //    //Skills
    //    TriggerBaseClass triggerPossibilityNone = new TriggerBaseClass(possibilityBaseRate: 0.0, possibilityWeight: 0, accumulationReference: ReferenceStatistics.none, accumulationBaseRate: 0.0, accumulationWeight: 0.0);
    //    TriggerBaseClass triggerPossibilityBasic = new TriggerBaseClass(possibilityBaseRate: 0.149, possibilityWeight: 15, accumulationReference: ReferenceStatistics.none, accumulationBaseRate: 0, accumulationWeight: 0.0);
    //    TriggerBaseClass triggerPossibilityNormal = new TriggerBaseClass(possibilityBaseRate: 0.122, possibilityWeight: 6, accumulationReference: ReferenceStatistics.none, accumulationBaseRate: 0.0, accumulationWeight: 0.0);
    //    TriggerBaseClass triggerPossibilityExpert = new TriggerBaseClass(possibilityBaseRate: 0.060, possibilityWeight: 2, accumulationReference: ReferenceStatistics.none, accumulationBaseRate: 0.0, accumulationWeight: 0.0);
    //    TriggerBaseClass triggerPossibilityMaster = new TriggerBaseClass(possibilityBaseRate: 0.002, possibilityWeight: 1, accumulationReference: ReferenceStatistics.none, accumulationBaseRate: 0.0, accumulationWeight: 0.0);
    //    TriggerBaseClass triggerPossibility100 = new TriggerBaseClass(possibilityBaseRate: 1.0, possibilityWeight: 0, accumulationReference: ReferenceStatistics.none, accumulationBaseRate: 0.0, accumulationWeight: 0.0);
    //    TriggerBaseClass triggerAccumulationMiddle = new TriggerBaseClass(possibilityBaseRate: 1.0, possibilityWeight: 0, accumulationReference: ReferenceStatistics.AvoidCount, accumulationBaseRate: 10.0, accumulationWeight: 1.5);
    //    TriggerBaseClass triggerAccumulationHit = new TriggerBaseClass(possibilityBaseRate: 1.0, possibilityWeight: 0, accumulationReference: ReferenceStatistics.AllTotalBeenHitCount, accumulationBaseRate: 10.0, accumulationWeight: 1.5);

    //    SkillMagnificationClass magnificationNone = new SkillMagnificationClass(attackTarget: TargetType.none, damage: 1.0, kinetic: 1.0, chemical: 1.0,
    //    thermal: 1.0, heal: 1.0, numberOfAttacks: 1.0, critical: 1.0, accuracy: 1.0, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
    //    SkillMagnificationClass magnificationNormal = new SkillMagnificationClass(attackTarget: TargetType.multi, damage: 1.0, kinetic: 1.0, chemical: 1.0,
    //    thermal: 1.0, heal: 1.0, numberOfAttacks: 1.0, critical: 1.0, accuracy: 1.0, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
    //    SkillMagnificationClass magnificationHeal20 = new SkillMagnificationClass(attackTarget: TargetType.none, damage: 1.0, kinetic: 1.0, chemical: 1.0,
    //    thermal: 1.0, heal: 2.0, numberOfAttacks: 1.0, critical: 1.0, accuracy: 1.0, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
    //    SkillMagnificationClass magnificationHeal40 = new SkillMagnificationClass(attackTarget: TargetType.none, damage: 1.0, kinetic: 1.0, chemical: 1.0,
    //    thermal: 1.0, heal: 4.0, numberOfAttacks: 1.0, critical: 1.0, accuracy: 1.0, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
    //    SkillMagnificationClass magnificationSingleD05N05CR05AC05 = new SkillMagnificationClass(attackTarget: TargetType.single, damage: 0.5, kinetic: 1.0, chemical: 1.0,
    //    thermal: 1.0, heal: 3.0, numberOfAttacks: 0.5, critical: 0.5, accuracy: 0.5, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
    //    SkillMagnificationClass magnificationMultiD075N05CR05AC05 = new SkillMagnificationClass(attackTarget: TargetType.multi, damage: 0.75, kinetic: 1.0, chemical: 1.0,
    //    thermal: 1.0, heal: 3.0, numberOfAttacks: 0.5, critical: 0.5, accuracy: 0.5, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
    //    SkillMagnificationClass magnificationMultiD10N05CR05AC075 = new SkillMagnificationClass(attackTarget: TargetType.multi, damage: 0.75, kinetic: 1.0, chemical: 1.0,
    //    thermal: 1.0, heal: 3.0, numberOfAttacks: 0.5, critical: 0.5, accuracy: 0.75, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
    //    SkillMagnificationClass magnificationMultiD10N10CR15AC20 = new SkillMagnificationClass(attackTarget: TargetType.multi, damage: 1.00, kinetic: 1.0, chemical: 1.0,
    //    thermal: 1.0, heal: 3.0, numberOfAttacks: 1.0, critical: 1.5, accuracy: 2.0, optimumRangeMin: 0.5, optimumRangeMax: 2.0);

    //    TriggerTargetClass triggerTargetNone = new TriggerTargetClass(actionType: ActionType.none, afterAllMoved: false, counter: false, chain: false, reAttack: false, heal: false, move: false,
    //     optimumRange: Range.any, majestyAttackType: AttackType.any, critical: CriticalOrNot.any, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: false, onlyWhenAvoidMoreThanOnce: false);
    //    TriggerTargetClass triggerTargetDamageControl = new TriggerTargetClass(actionType: ActionType.any, afterAllMoved: false, counter: true, chain: true, reAttack: true, heal: false, move: true,
    //     optimumRange: Range.any, majestyAttackType: AttackType.any, critical: CriticalOrNot.any, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: false, onlyWhenAvoidMoreThanOnce: false);
    //    TriggerTargetClass triggerTargetIndependent = new TriggerTargetClass(actionType: ActionType.any, afterAllMoved: true, counter: false, chain: false, reAttack: false, heal: false, move: false,
    //     optimumRange: Range.any, majestyAttackType: AttackType.any, critical: CriticalOrNot.any, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: false, onlyWhenAvoidMoreThanOnce: false);
    //    TriggerTargetClass triggerTargetCounter = new TriggerTargetClass(actionType: ActionType.any, afterAllMoved: true, counter: true, chain: true, reAttack: true, heal: false, move: true,
    //     optimumRange: Range.within, majestyAttackType: AttackType.any, critical: CriticalOrNot.nonCritical, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: true, onlyWhenAvoidMoreThanOnce: false);
    //    TriggerTargetClass triggerTargetChainCounter = new TriggerTargetClass(actionType: ActionType.counter, afterAllMoved: false, counter: true, chain: false, reAttack: false, heal: false, move: true,
    //     optimumRange: Range.any, majestyAttackType: AttackType.any, critical: CriticalOrNot.any, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: false, onlyWhenAvoidMoreThanOnce: false);
    //    TriggerTargetClass triggerTargetCriticalReAttack = new TriggerTargetClass(actionType: ActionType.any, afterAllMoved: true, counter: false, chain: true, reAttack: false, heal: false, move: true,
    //     optimumRange: Range.any, majestyAttackType: AttackType.any, critical: CriticalOrNot.critical, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: false, onlyWhenAvoidMoreThanOnce: false);
    //    TriggerTargetClass triggerTargetInterrupt = new TriggerTargetClass(actionType: ActionType.interrupt, afterAllMoved: false, counter: true, chain: false, reAttack: false, heal: false, move: false,
    //     optimumRange: Range.any, majestyAttackType: AttackType.any, critical: CriticalOrNot.any, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: false, onlyWhenAvoidMoreThanOnce: false);

    //    BuffTargetParameterClass buffTargetNone = new BuffTargetParameterClass(targetType: TargetType.none, barrierRemaining: 0, defenseMagnification: 1.0, mobilityMagnification: 1.0,
    //    attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0, rangeMinCorrection: 0, rangeMaxCorrection: 0);
    //    BuffTargetParameterClass buffTargetSelf = new BuffTargetParameterClass(targetType: TargetType.self, barrierRemaining: 0, defenseMagnification: 1.0, mobilityMagnification: 1.0,
    //   attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0, rangeMinCorrection: 0, rangeMaxCorrection: 0);
    //    BuffTargetParameterClass buffTargetMulti = new BuffTargetParameterClass(targetType: TargetType.multi, barrierRemaining: 0, defenseMagnification: 1.0, mobilityMagnification: 1.0,
    //   attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0, rangeMinCorrection: 0, rangeMaxCorrection: 0);
    //    BuffTargetParameterClass buffTargetMultiBarrier = new BuffTargetParameterClass(targetType: TargetType.multi, barrierRemaining: 0, defenseMagnification: 1.0, mobilityMagnification: 1.0,
    //   attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0, rangeMinCorrection: 0, rangeMaxCorrection: 0);
    //    BuffTargetParameterClass buffBarrierDefense12 = new BuffTargetParameterClass(targetType: TargetType.none, barrierRemaining: 20, defenseMagnification: 1.1, mobilityMagnification: 1.0,
    //   attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0, rangeMinCorrection: 0, rangeMaxCorrection: 0);
    //    BuffTargetParameterClass buffBarrier10 = new BuffTargetParameterClass(targetType: TargetType.none, barrierRemaining: 6, defenseMagnification: 1.05, mobilityMagnification: 1.0,
    //   attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0, rangeMinCorrection: 0, rangeMaxCorrection: 0);

    //    DebuffTargetParameterClass debuffTargetNone = new DebuffTargetParameterClass(targetType: TargetType.none, barrierRemaining: 0, defenseMagnification: 1.0, mobilityMagnification: 1.0,
    //    attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0);

    //    skillsMasters[0] = new SkillsMasterStruct(name: SkillName.BarrierCounterAvoidManyTimes, actionType: ActionType.counter, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 3, veiledTurn: 20, ability: Ability.generation,
    //     triggerBase: triggerAccumulationMiddle, magnification: magnificationNone, triggerTarget: triggerTargetCounter, buffTarget: buffTargetSelf, callingBuffName: SkillName.Buffdefense12,
    //         debuffTarget: debuffTargetNone);
    //    skillsMasters[1] = new SkillsMasterStruct(name: SkillName.CounterNonCriticalAttack, actionType: ActionType.counter, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 6, veiledTurn: 20, ability: Ability.responsiveness,
    //     triggerBase: triggerPossibilityBasic, magnification: magnificationSingleD05N05CR05AC05,
    //         triggerTarget: triggerTargetCounter, buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);
    //    skillsMasters[2] = new SkillsMasterStruct(name: SkillName.ChainAllysCounter, actionType: ActionType.chain, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 6, veiledTurn: 20, ability: Ability.responsiveness,
    //     triggerBase: triggerPossibilityNormal, magnification: magnificationMultiD075N05CR05AC05, triggerTarget: triggerTargetChainCounter, buffTarget: buffTargetNone, callingBuffName: SkillName.none,
    //     debuffTarget: debuffTargetNone);
    //    skillsMasters[3] = new SkillsMasterStruct(name: SkillName.FutureSightShot, actionType: ActionType.move, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 6, veiledTurn: 20, ability: Ability.power,
    //     triggerBase: triggerPossibilityNormal, magnification: magnificationMultiD10N10CR15AC20, triggerTarget: triggerTargetIndependent, buffTarget: buffTargetNone, callingBuffName: SkillName.none,
    //     debuffTarget: debuffTargetNone);
    //    skillsMasters[4] = new SkillsMasterStruct(name: SkillName.ReAttackAfterCritical, actionType: ActionType.reAttack, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 6, veiledTurn: 20, ability: Ability.power,
    //     triggerBase: triggerPossibilityExpert, magnification: magnificationMultiD10N05CR05AC075, triggerTarget: triggerTargetCriticalReAttack,
    //     buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);
    //    skillsMasters[5] = new SkillsMasterStruct(name: SkillName.InterruptTargetCounterReduceAccuracy, actionType: ActionType.interrupt, callSkillLogicName: CallSkillLogicName.ReduceAccuracy, isHeal: false, usageCount: 4, veiledTurn: 20, ability: Ability.intelligence,
    //    triggerBase: triggerPossibilityMaster, magnification: magnificationNone, triggerTarget: triggerTargetInterrupt, buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);
    //    // interrupt skill needs coding..
    //    skillsMasters[6] = new SkillsMasterStruct(name: SkillName.ShiledHealAll, actionType: ActionType.move, callSkillLogicName: CallSkillLogicName.ShieldHealMulti, isHeal: false, usageCount: 1, veiledTurn: 20, ability: Ability.generation,
    //     triggerBase: triggerPossibility100, magnification: magnificationNone, triggerTarget: triggerTargetDamageControl, buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);

    //    skillsMasters[11] = new SkillsMasterStruct(name: SkillName.ShiledHealplusSingle, actionType: ActionType.move, callSkillLogicName: CallSkillLogicName.ShieldHealSingle, isHeal: true, usageCount: 2, veiledTurn: 20, ability: Ability.generation,
    //     triggerBase: triggerPossibility100, magnification: magnificationHeal40, triggerTarget: triggerTargetDamageControl, buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);
    //    skillsMasters[12] = new SkillsMasterStruct(name: SkillName.ShiledHealSingle, actionType: ActionType.move, callSkillLogicName: CallSkillLogicName.ShieldHealSingle, isHeal: true, usageCount: 3, veiledTurn: 20, ability: Ability.generation,
    //     triggerBase: triggerPossibility100, magnification: magnificationHeal20, triggerTarget: triggerTargetDamageControl, buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);

    //    skillsMasters[13] = new SkillsMasterStruct(name: SkillName.BarrierAll, actionType: ActionType.atBeginning, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 2, veiledTurn: 20, ability: Ability.none,
    //    triggerBase: triggerPossibilityNormal, magnification: magnificationNone, triggerTarget: triggerTargetIndependent, buffTarget: buffTargetMultiBarrier, callingBuffName: SkillName.Buffbarrier10, debuffTarget: debuffTargetNone);

    //    // Special Normal attack skill
    //    skillsMasters[14] = new SkillsMasterStruct(name: SkillName.normalAttack, actionType: ActionType.none, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 1000, veiledTurn: 20, ability: Ability.none,
    //     triggerBase: triggerPossibility100, magnification: magnificationNormal, triggerTarget: triggerTargetNone, buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);

    //    //Buff
    //    buffMasters.Add(new SkillsMasterStruct(name: SkillName.Buffdefense12, actionType: ActionType.none, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 0, veiledTurn: 5, ability: Ability.none,
    //    triggerBase: triggerPossibilityNone, magnification: magnificationNone, triggerTarget: triggerTargetNone, buffTarget: buffBarrierDefense12, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone));
    //    buffMasters.Add(new SkillsMasterStruct(name: SkillName.Buffbarrier10, actionType: ActionType.none, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 0, veiledTurn: 5, ability: Ability.none,
    //    triggerBase: triggerPossibilityNone, magnification: magnificationNone, triggerTarget: triggerTargetNone, buffTarget: buffBarrier10, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone));


    //    //------------------------Battle Main
    //}


}
