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
public enum WhichWin { none, allyWin, enemyWin, Draw }
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
    //none, MagnificationFixedRatio, MagnificationRatio, AdditionalPercent,
    none, MagnificationRatio, AdditionalPercent,

}

public enum OffenseOrDefense
{
    none, Offense, Defense
}

//for Item Master, tell the Magnification Id
// none should not be selected.
//[CAUTION] Change this values or order will cause serious trouble in CalculateUnitStatus. because it use (int)MagnificationTarget.
// 0:none, 1:Critical, 2:Kinetic, 3:Chemical, 4:Thermal, 5:VsBeast, 6:VsCyborg, 7:VsDrone, 8:VsRobot, 9:VsTitan, 10:OptimumRangeBonus
// after 11, should only affect status, so use OffenseOrDefense.none
// 11:Shield, 12:HitPoint, 13: NumberOfAttacks, 14: MinRange, 15: MaxRange, 16:Accuracy, 17:Mobility, 18:Attack, 19:Defence

public enum MagnificationTarget
{
    none, Critical, Kinetic, Chemical, Thermal, VsBeast, VsCyborg, VsDrone, VsRobot, VsTitan, OptimumRangeBonus,
    Shield, HitPoint, NumberOfAttacks, MiniRange, MaxRange, Accuracy, Mobility, Attack, Defence
}

public enum MagnificationPercent { zero, one, two, three, four, five, six, seven, eight, nine, ten, eleven, twelve, thirteen }


