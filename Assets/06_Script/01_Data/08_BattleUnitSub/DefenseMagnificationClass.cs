using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Defense magnification
[CreateAssetMenu(fileName = "DefenseMagnificationClass-", menuName = "BattleUnit/DefenseMagnificationClass", order = 22)]
public class DefenseMagnificationClass : MagnificationClass
{
    public DefenseMagnificationClass(double critical, double kinetic, double chemical, double thermal, double vsBeast,
        double vsCyborg, double vsDrone, double vsRobot, double vsTitan)
     : base(critical, kinetic, chemical, thermal, vsBeast, vsCyborg, vsDrone, vsRobot, vsTitan) { }


    public DefenseMagnificationClass(double critical, double kinetic, double chemical, double thermal, double vsBeast,
        double vsCyborg, double vsDrone, double vsRobot, double vsTitan,
        double shield, double hitPoint, double numberOfAttacks, double minRange, double maxRange, double accuracy, double mobility,
        double attack, double defense)
     : base(critical, kinetic, chemical, thermal, vsBeast, vsCyborg, vsDrone, vsRobot, vsTitan)
    {
        this.shield = shield;
        this.hitPoint = hitPoint;
        this.numberOfAttacks = numberOfAttacks;
        this.minRange = minRange;
        this.maxRange = maxRange;
        this.accuracy = accuracy;
        this.mobility = mobility;
        this.attack = attack;
        this.defense = defense;
    }

    public double shield;
    public double hitPoint;
    public double numberOfAttacks;
    public double minRange;
    public double maxRange;
    public double accuracy;
    public double mobility;
    public double attack;
    public double defense;

}
