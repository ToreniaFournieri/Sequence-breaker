using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Offense magnification
[CreateAssetMenu(fileName = "OffenseMagnificationClass-", menuName = "BattleUnit/OffenseMagnificationClass", order = 20)]
sealed public class OffenseMagnificationClass : MagnificationClass
{
    public OffenseMagnificationClass(double optimumRangeBonus, double critical, double kinetic, double chemical, double thermal, double vsBeast, double vsCyborg, double vsDrone, double vsRobot, double vsTitan)
    : base(critical, kinetic, chemical, thermal, vsBeast, vsCyborg, vsDrone, vsRobot, vsTitan)
    { this.OptimumRangeBonus = optimumRangeBonus; }
    [SerializeField] public double OptimumRangeBonus;
}