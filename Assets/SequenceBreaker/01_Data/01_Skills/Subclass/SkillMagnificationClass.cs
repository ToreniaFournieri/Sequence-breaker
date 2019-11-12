using SequenceBreaker._00_System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._01_Data._01_Skills.Subclass
{
    [CreateAssetMenu(fileName = "magnification", menuName = "Skill/SubClass/SkillMagnificationClass", order = 7)]
    public sealed class SkillMagnificationClass : ScriptableObject
    {
        [FormerlySerializedAs("AttackTarget")] public TargetType attackTarget;
        [FormerlySerializedAs("Damage")] public double damage;
        [FormerlySerializedAs("Kinetic")] public double kinetic;
        [FormerlySerializedAs("Chemical")] public double chemical;
        [FormerlySerializedAs("Thermal")] public double thermal;
        [FormerlySerializedAs("Heal")] public double heal;
        [FormerlySerializedAs("NumberOfAttacks")] public double numberOfAttacks;
        [FormerlySerializedAs("Critical")] public double critical;
        [FormerlySerializedAs("Accuracy")] public double accuracy;
        [FormerlySerializedAs("OptimumRangeMin")] public double optimumRangeMin;
        [FormerlySerializedAs("OptimumRangeMax")] public double optimumRangeMax;


    }
}
