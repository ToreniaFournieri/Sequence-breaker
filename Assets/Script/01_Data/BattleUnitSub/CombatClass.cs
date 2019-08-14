using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Combat Status, the status whitch has been calculated by parts.
[CreateAssetMenu(fileName = "CombatClass-", menuName = "BattleUnit/CombatClass", order = 14)]
public class CombatClass : ScriptableObject
{

    [SerializeField] public int ShiledCurrent;
    [SerializeField] public int ShiledMax;
    [SerializeField] public int HitPointCurrent;
    [SerializeField] public int HitPointMax;
    [SerializeField] public int Attack;
    [SerializeField] public double KineticAttackRatio;
    [SerializeField] public double ChemicalAttackRatio;
    [SerializeField] public double ThermalAttackRatio;
    [SerializeField] public int CriticalHit;
    [SerializeField] public int NumberOfAttacks;
    [SerializeField] public int MinRange;
    [SerializeField] public int MaxRange;
    [SerializeField] public int Accuracy;
    [SerializeField] public int Mobility;
    [SerializeField] public int Defense;

    public CombatClass Copy()
    {
        return (CombatClass)this.MemberwiseClone();
    }

    public CombatClass(int shiledCurrent, int shiledMax, int hitPointCurrent, int hitPointMax, int attack, double kineticAttackRatio, double chemicalAttackRatio,
double thermalAttackRatio, int criticalHit, int numberOfAttacks, int minRange, int maxRange, int accuracy, int mobility, int defense)
    {
        this.ShiledCurrent = shiledCurrent; this.ShiledMax = shiledMax; this.HitPointCurrent = hitPointCurrent; this.HitPointMax = hitPointMax; this.Attack = attack;
        this.KineticAttackRatio = kineticAttackRatio; this.ChemicalAttackRatio = chemicalAttackRatio; this.ThermalAttackRatio = thermalAttackRatio; this.CriticalHit = criticalHit;
        this.NumberOfAttacks = numberOfAttacks; this.MinRange = minRange; this.MaxRange = maxRange; this.Accuracy = accuracy; this.Mobility = mobility; this.Defense = defense;
    }

    public CombatClass()
    {

    }

}