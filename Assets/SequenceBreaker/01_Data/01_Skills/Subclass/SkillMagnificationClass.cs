using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "magnification", menuName = "Skill/SubClass/SkillMagnificationClass", order = 7)]
sealed public class SkillMagnificationClass : ScriptableObject
{

    //public SkillMagnificationClass(TargetType attackTarget, double damage, double kinetic,
    //double chemical, double thermal, double heal, double numberOfAttacks, double critical, double accuracy,
    //     double optimumRangeMin, double optimumRangeMax)
    //{
    //    this.AttackTarget = attackTarget; this.Damage = damage; this.Kinetic = kinetic; this.Chemical = chemical; this.Thermal = thermal; this.Heal = heal;
    //    this.NumberOfAttacks = numberOfAttacks; this.Critical = critical; this.Accuracy = accuracy; this.OptimumRangeMin = optimumRangeMin; this.OptimumRangeMax = optimumRangeMax;
    //}

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
