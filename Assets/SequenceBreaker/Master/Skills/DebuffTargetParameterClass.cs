using SequenceBreaker.Environment;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker.Master.Skills
{
    [CreateAssetMenu(fileName = "debuffTarget-", menuName = "Skill/SubClass/DebuffTargetParameterClass", order = 16)]
    public sealed class DebuffTargetParameterClass : ScriptableObject
    {
        
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
