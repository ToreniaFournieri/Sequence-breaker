using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


//Basic Magnification, offense and defense is acturally same.
public class MagnificationClass : ScriptableObject
{
    public MagnificationClass(double critical, double kinetic, double chemical, double thermal, double vsBeast, double vsCyborg, double vsDrone, double vsRobot, double vsTitan)
    {
        this.critical = critical; this.kinetic = kinetic; this.chemical = chemical; this.thermal = thermal; this.vsBeast = vsBeast;
        this.vsCyborg = vsCyborg; this.vsDrone = vsDrone; this.vsRobot = vsRobot; this.vsTitan = vsTitan;
    }
    [FormerlySerializedAs("Critical")] [SerializeField] public double critical;
    [FormerlySerializedAs("Kinetic")] [SerializeField] public double kinetic;
    [FormerlySerializedAs("Chemical")] [SerializeField] public double chemical;
    [FormerlySerializedAs("Thermal")] [SerializeField] public double thermal;
    [FormerlySerializedAs("VsBeast")] [SerializeField] public double vsBeast;
    [FormerlySerializedAs("VsCyborg")] [SerializeField] public double vsCyborg;
    [FormerlySerializedAs("VsDrone")] [SerializeField] public double vsDrone;
    [FormerlySerializedAs("VsRobot")] [SerializeField] public double vsRobot;
    [FormerlySerializedAs("VsTitan")] [SerializeField] public double vsTitan;
}