//Enum: Affiliation (enemy or ally) definition 

namespace SequenceBreaker.Environment
{
    public enum Affiliation { Ally, Enemy, None }
//Enum: Unit type definition (for vs check, deal additional bonus or dealt additional reduction)
    public enum UnitType { Beast, Cyborg, Drone, Robot, Titan }
    public enum Ability { None, Power, Generation, Stability, Responsiveness, Precision, Intelligence, Luck }
    public enum ActionType { None, Any, NormalAttack, Move, Counter, Chain, ReAttack, Interrupt, Buff, AtBeginning, AtEnding }
    public enum AttackType { Any, Kinetic, Chemical, Thermal }
    public enum TargetType { None, Self, Single, Multi }
    public enum CriticalOrNot { Any, Critical, NonCritical }
    public enum ActorOrTargetUnit { No, ActorUnit, TargetUnit }
    public enum Range { Any, Within, Without }
    public enum WhichWin { None, AllyWin, EnemyWin, Draw }
    public enum SkillName
    {
        None, NormalAttack, BarrierCounterAvoidManyTimes, CounterNonCriticalAttack, ChainAllysCounter, FutureSightShot, ReAttackAfterCritical,
        InterruptTargetCounterReduceAccuracy, BarrierAll, Buffdefense12, Buffbarrier10,
        ShieldHealSingle, ShieldHealplusSingle, ShieldHealAll, ShieldHealLittle
    }
    public enum ReferenceStatistics { None, AllHitCount, CriticalHitCount, SkillHitCount, AllTotalBeenHitCount, CriticalBeenHitCount, SkillBeenHitCount, AvoidCount }
    public enum CallSkillLogicName { None, ShieldHealSingle, ShieldHealMulti, ReduceAccuracy }


//for Item Master, tell which type of magnification is selected,
//Magnification Fixed Ratio: ex) 2/3, 3/5
//Magnification Ratio: ex) x1.05
//Additional Percent: ex) +12%
// none should not be selected.
    public enum MagnificationType
    {
        //none, MagnificationFixedRatio, MagnificationRatio, AdditionalPercent,
        None, MagnificationRatio, AdditionalPercent,

    }

    public enum OffenseOrDefense
    {
        None, Offense, Defense
    }

//for Item Master, tell the Magnification Id
// none should not be selected.
//[CAUTION] Change this values or order will cause serious trouble in CalculateUnitStatus. because it use (int)MagnificationTarget.
// 0:none, 1:Critical, 2:Kinetic, 3:Chemical, 4:Thermal, 5:VsBeast, 6:VsCyborg, 7:VsDrone, 8:VsRobot, 9:VsTitan, 10:OptimumRangeBonus
// after 11, should only affect status, so use OffenseOrDefense.none
// 11:BaseShield, 12:BaseHitPoint, 13:BaseNumberOfAttacks, 14:MiniRange, 15:MaxRange, 16:BaseAccuracy, 17:BaseMobility, 18:BaseAttack, 19:BaseDefense

    public enum MagnificationTarget
    {
        None, Critical, Kinetic, Chemical, Thermal, VsBeast, VsCyborg, VsDrone, VsRobot, VsTitan, OptimumRangeBonus,
        BaseShield, BaseHitPoint, BaseNumberOfAttacks, MiniRange, MaxRange, BaseAccuracy, BaseMobility, BaseAttack, BaseDefense
    }

    public enum MagnificationPercent { Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Eleven, Twelve, Thirteen, Fourteen }
}