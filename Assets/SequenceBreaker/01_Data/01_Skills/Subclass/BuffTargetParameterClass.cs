using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "buffTarget", menuName = "Skill/SubClass/BuffTargetParameterClass", order = 10)]
sealed public class BuffTargetParameterClass : ScriptableObject
{

    //public BuffTargetParameterClass(TargetType targetType, int barrierRemaining, double defenseMagnification, double mobilityMagnification, double attackMagnification,
    // double accuracyMagnification, double criticalHitRateMagnification, double numberOfAttackMagnification, int rangeMinCorrection, int rangeMaxCorrection)
    //{
    //    this.TargetType = targetType; this.BarrierRemaining = barrierRemaining; this.DefenseMagnification = defenseMagnification; this.MobilityMagnification = mobilityMagnification;
    //    this.AttackMagnification = attackMagnification; this.AccuracyMagnification = accuracyMagnification; this.CriticalHitRateMagnification = criticalHitRateMagnification;
    //    this.NumberOfAttackMagnification = numberOfAttackMagnification; this.RangeMinCorrection = rangeMinCorrection; this.RangeMaxCorrection = rangeMaxCorrection;
    //}

    [FormerlySerializedAs("TargetType")] public TargetType targetType;
    [FormerlySerializedAs("BarrierRemaining")] public int barrierRemaining;
    [FormerlySerializedAs("DefenseMagnification")] public double defenseMagnification;
    [FormerlySerializedAs("MobilityMagnification")] public double mobilityMagnification;
    [FormerlySerializedAs("AttackMagnification")] public double attackMagnification;
    [FormerlySerializedAs("AccuracyMagnification")] public double accuracyMagnification;
    [FormerlySerializedAs("CriticalHitRateMagnification")] public double criticalHitRateMagnification;
    [FormerlySerializedAs("NumberOfAttackMagnification")] public double numberOfAttackMagnification;
    [FormerlySerializedAs("RangeMinCorrection")] public int rangeMinCorrection;
    [FormerlySerializedAs("RangeMaxCorrection")] public int rangeMaxCorrection;

}
