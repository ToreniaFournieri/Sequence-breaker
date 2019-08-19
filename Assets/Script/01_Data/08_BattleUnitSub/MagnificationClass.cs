using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Basic Magnification, offense and defense is acturally same.
public class MagnificationClass : ScriptableObject
{
    public MagnificationClass(double critical, double kinetic, double chemical, double thermal, double vsBeast, double vsCyborg, double vsDrone, double vsRobot, double vsTitan)
    {
        this.Critical = critical; this.Kinetic = kinetic; this.Chemical = chemical; this.Thermal = thermal; this.VsBeast = vsBeast;
        this.VsCyborg = vsCyborg; this.VsDrone = vsDrone; this.VsRobot = vsRobot; this.VsTitan = vsTitan;
    }
    [SerializeField] public double Critical;
    [SerializeField] public double Kinetic;
    [SerializeField] public double Chemical;
    [SerializeField] public double Thermal;
    [SerializeField] public double VsBeast;
    [SerializeField] public double VsCyborg;
    [SerializeField] public double VsDrone;
    [SerializeField] public double VsRobot;
    [SerializeField] public double VsTitan;
}