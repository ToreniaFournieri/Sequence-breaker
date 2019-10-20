using SequenceBreaker._00_System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._01_Data._01_Skills.Subclass
{
    [CreateAssetMenu(fileName = "debuffTarget-", menuName = "Skill/SubClass/DebuffTargetParameterClass", order = 16)]
    public sealed class DebuffTargetParameterClass : ScriptableObject
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
}
