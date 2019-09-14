using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "magnification", menuName = "Skill/SubClass/SkillMagnificationClass", order = 7)]
public class SkillMagnificationClass : ScriptableObject
{

    //public SkillMagnificationClass(TargetType attackTarget, double damage, double kinetic,
    //double chemical, double thermal, double heal, double numberOfAttacks, double critical, double accuracy,
    //     double optimumRangeMin, double optimumRangeMax)
    //{
    //    this.AttackTarget = attackTarget; this.Damage = damage; this.Kinetic = kinetic; this.Chemical = chemical; this.Thermal = thermal; this.Heal = heal;
    //    this.NumberOfAttacks = numberOfAttacks; this.Critical = critical; this.Accuracy = accuracy; this.OptimumRangeMin = optimumRangeMin; this.OptimumRangeMax = optimumRangeMax;
    //}

    public TargetType AttackTarget;
    public double Damage;
    public double Kinetic;
    public double Chemical;
    public double Thermal;
    public double Heal;
    public double NumberOfAttacks;
    public double Critical;
    public double Accuracy;
    public double OptimumRangeMin;
    public double OptimumRangeMax;
 

}
