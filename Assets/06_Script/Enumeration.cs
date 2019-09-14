using System;
using UnityEngine;
using System.Collections;

//Enum: Affiliation (enemy or ally) difinition 
public enum Affiliation { ally, enemy, none }
//Enum: Unit type difinition (for vs check, deal additional bonus or delt additional reduction)
public enum UnitType { beast, cyborg, drone, robot, titan }
public enum Ability { none, power, generation, stability, responsiveness, precision, intelligence, luck }
public enum ActionType { None, Any, NormalAttack, Move, Counter, Chain, ReAttack, Interrupt, Buff, AtBeginning, AtEnding }
public enum AttackType { any, kinetic, chemical, thermal }
public enum TargetType { none, self, single, multi }
public enum CriticalOrNot { any, critical, nonCritical }
public enum ActorOrTargetUnit { no, actorUnit, targetUnit }
public enum Range { any, within, without }
public enum WhichWin { allyWin, enemyWin, Draw }
public enum SkillName
{
    none, normalAttack, BarrierCounterAvoidManyTimes, CounterNonCriticalAttack, ChainAllysCounter, FutureSightShot, ReAttackAfterCritical,
    InterruptTargetCounterReduceAccuracy, BarrierAll, Buffdefense12, Buffbarrier10,
    ShieldHealSingle, ShieldHealplusSingle, ShieldHealAll
}
public enum ReferenceStatistics { none, AllHitCount, CriticalHitCount, SkillHitCount, AllTotalBeenHitCount, CriticalBeenHitCount, SkillBeenHitCount, AvoidCount }
public enum CallSkillLogicName { none, ShieldHealSingle, ShieldHealMulti, ReduceAccuracy }


//for Item Master, tell which type of magnification is selected,
//Magnification Fixed Ratio: ex) 2/3, 3/5
//Magnification Ratio: ex) x1.05
//Additional Percent: ex) +12%
// none should not be selected.
public enum MagnificationType
{
    none, OffenseMagnificationFixedRatio, OffenseMagnificationRatio, OffenseAdditionalPercent,
    DefenseMagnificationFixedRatio, DefenseMagnificationRatio, DefenseAdditionalPercent
}

//for Item Master, tell the Magnification Id
// none should not be selected.
public enum MagnificationTarget
{
    none, Critical, Kinetic, Chemical, Thermal, VsBeast, VsCyborg, VsDrone, VsRobot, VsTitan, OptimumRangeBonus
}

public enum MagnificationFixedRatio
{
    oneOverOne, twoOverThree, threeOverFour, fourOverFive, fiveOverSix,
    sixOverFive, fiveOverFour, fourOverThree, threeOverTwo, twoOverOne, oneOverTen, oneOverHundred
}
public enum MagnificationPercent { zero, one, two, three, four, five, six, seven, eight, nine, ten, eleven, twelve, thirteen }


