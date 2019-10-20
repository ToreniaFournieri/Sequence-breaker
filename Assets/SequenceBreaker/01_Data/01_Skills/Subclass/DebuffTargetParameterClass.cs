using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "debuffTarget-", menuName = "Skill/SubClass/DebuffTargetParameterClass", order = 16)]
sealed public class DebuffTargetParameterClass : ScriptableObject
{

    //public DebuffTargetParameterClass(TargetType targetType, double barrierRemaining, double defenseMagnification, double mobilityMagnification, double attackMagnification,
    // double accuracyMagnification, double criticalHitRateMagnification, double numberOfAttackMagnification)
    //{
    //    this.TargetType = targetType; this.BarrierRemaining = barrierRemaining; this.DefenseMagnification = defenseMagnification; this.MobilityMagnification = mobilityMagnification;
    //    this.AttackMagnification = attackMagnification; this.AccuracyMagnification = accuracyMagnification; this.CriticalHitRateMagnification = criticalHitRateMagnification;
    //    this.NumberOfAttackMagnification = numberOfAttackMagnification;
    //}

    [FormerlySerializedAs("TargetType")] public TargetType targetType;
    [FormerlySerializedAs("BarrierRemaining")] public double barrierRemaining;
    [FormerlySerializedAs("DefenseMagnification")] public double defenseMagnification;
    [FormerlySerializedAs("MobilityMagnification")] public double mobilityMagnification;
    [FormerlySerializedAs("AttackMagnification")] public double attackMagnification;
    [FormerlySerializedAs("AccuracyMagnification")] public double accuracyMagnification;
    [FormerlySerializedAs("CriticalHitRateMagnification")] public double criticalHitRateMagnification;
    [FormerlySerializedAs("NumberOfAttackMagnification")] public double numberOfAttackMagnification;

}
