using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public TargetType TargetType;
    public int BarrierRemaining;
    public double DefenseMagnification;
    public double MobilityMagnification;
    public double AttackMagnification;
    public double AccuracyMagnification;
    public double CriticalHitRateMagnification;
    public double NumberOfAttackMagnification;
    public int RangeMinCorrection;
    public int RangeMaxCorrection;

}
