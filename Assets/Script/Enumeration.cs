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
    public enum ActorOrTargetUnit {no, actorUnit, targetUnit }
    public enum Range { any,within, without }
    public enum WhichWin { allyWin, enemyWin, Draw }
    public enum SkillName
    {
        none, normalAttack, BarrierCounterAvoidManyTimes, CounterNonCriticalAttack, ChainAllysCounter, FutureSightShot, ReAttackAfterCritical,
        InterruptTargetCounterReduceAccuracy, BarrierAll, Buffdefense12, Buffbarrier10,
        ShiledHealSingle, ShiledHealplusSingle, ShiledHealAll
    }
    public enum ReferenceStatistics { none, AllHitCount, CriticalHitCount, SkillHitCount, AllTotalBeenHitCount, CriticalBeenHitCount, SkillBeenHitCount, AvoidCount }
    public enum CallSkillLogicName { none, ShieldHealSingle, ShieldHealMulti, ReduceAccuracy }

