using System;
using UnityEngine;
using System.Collections;

//Enum: Affiliation (enemy or ally) difinition 
public enum Affiliation { ally, enemy, none }
    //Enum: Unit type difinition (for vs check, deal additional bonus or delt additional reduction)
    public enum UnitType { beast, cyborg, drone, robot, titan }
    public enum Ability { power, generation, stability, responsiveness, precision, intelligence, luck, none }
    public enum ActionType { normalAttack, move, counter, chain, reAttack, interrupt, buff, atBeginning, atEnding, any, none }
    public enum AttackType { kinetic, chemical, thermal, any }
    public enum TargetType { self, single, multi, none }
    public enum CriticalOrNot { critical, nonCritical, any }
    public enum ActorOrTargetUnit { actorUnit, targetUnit, no }
    public enum Range { within, without, any }
    public enum WhichWin { allyWin, enemyWin, Draw }
    public enum SkillName
    {
        none, normalAttack, BarrierCounterAvoidManyTimes, CounterNonCriticalAttack, ChainAllysCounter, FutureSightShot, ReAttackAfterCritical,
        InterruptTargetCounterReduceAccuracy, BarrierAll, Buffdefense12, Buffbarrier10,
        ShiledHealSingle, ShiledHealplusSingle, ShiledHealAll
    }
    public enum ReferenceStatistics { none, AllHitCount, CriticalHitCount, SkillHitCount, AllTotalBeenHitCount, CriticalBeenHitCount, SkillBeenHitCount, AvoidCount }
    public enum CallSkillLogicName { none, ShieldHealSingle, ShieldHealMulti, ReduceAccuracy }

