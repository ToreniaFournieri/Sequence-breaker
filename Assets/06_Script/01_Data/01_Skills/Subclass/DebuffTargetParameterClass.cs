using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "debuffTarget-", menuName = "Skill/SubClass/DebuffTargetParameterClass", order = 16)]
public class DebuffTargetParameterClass : ScriptableObject
{

    //public DebuffTargetParameterClass(TargetType targetType, double barrierRemaining, double defenseMagnification, double mobilityMagnification, double attackMagnification,
    // double accuracyMagnification, double criticalHitRateMagnification, double numberOfAttackMagnification)
    //{
    //    this.TargetType = targetType; this.BarrierRemaining = barrierRemaining; this.DefenseMagnification = defenseMagnification; this.MobilityMagnification = mobilityMagnification;
    //    this.AttackMagnification = attackMagnification; this.AccuracyMagnification = accuracyMagnification; this.CriticalHitRateMagnification = criticalHitRateMagnification;
    //    this.NumberOfAttackMagnification = numberOfAttackMagnification;
    //}

    public TargetType TargetType;
    public double BarrierRemaining;
    public double DefenseMagnification;
    public double MobilityMagnification;
    public double AttackMagnification;
    public double AccuracyMagnification;
    public double CriticalHitRateMagnification;
    public double NumberOfAttackMagnification;

}
