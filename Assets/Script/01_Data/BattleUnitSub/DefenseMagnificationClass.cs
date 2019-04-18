using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Defense magnification
[CreateAssetMenu(fileName = "DefenseMagnificationClass-", menuName = "BattleUnit/DefenseMagnificationClass", order = 22)]
public class DefenseMagnificationClass : MagnificationClass
{
    public DefenseMagnificationClass(double critical, double kinetic, double chemical, double thermal, double vsBeast, double vsCyborg, double vsDrone, double vsRobot, double vsTitan)
     : base(critical, kinetic, chemical, thermal, vsBeast, vsCyborg, vsDrone, vsRobot, vsTitan) { }
}
