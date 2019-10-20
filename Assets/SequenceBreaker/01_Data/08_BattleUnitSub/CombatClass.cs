using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Combat Status, the status whitch has been calculated by parts.
[CreateAssetMenu(fileName = "CombatClass-", menuName = "BattleUnit/CombatClass", order = 14)]
public class CombatClass : ScriptableObject
{

    [SerializeField] public int ShieldCurrent;
    [SerializeField] public int ShieldMax;
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

    //New since 2019/08/16 use intelligence, generation
    [SerializeField] public int Counterintelligence;
    [SerializeField] public int Repair;

    public CombatClass Copy()
    {
        return (CombatClass)this.MemberwiseClone();
    }


    public CombatClass Add(CombatClass combatFrom)
    {
        this.ShieldCurrent += combatFrom.ShieldCurrent;
        this.ShieldMax += combatFrom.ShieldMax;
        this.HitPointCurrent += combatFrom.HitPointCurrent;
        this.HitPointMax += combatFrom.HitPointMax;
        this.Attack += combatFrom.Attack;
        this.KineticAttackRatio += combatFrom.KineticAttackRatio;
        this.ChemicalAttackRatio += combatFrom.ChemicalAttackRatio;
        this.ThermalAttackRatio += combatFrom.ThermalAttackRatio;
        this.CriticalHit += combatFrom.CriticalHit;
        this.NumberOfAttacks += combatFrom.NumberOfAttacks;

        //Note: MinRange and MaxRange is not be add up.
        //combatAdded.MinRange;
        //combatAdded.MaxRange;
        this.Accuracy += combatFrom.Accuracy;
        this.Mobility += combatFrom.Mobility;
        this.Defense += combatFrom.Defense;
        this.Counterintelligence += combatFrom.Counterintelligence;
        this.Repair += combatFrom.Repair;

        return this;
    }

    public CombatClass Pow(int specifies)
    {
        double raised = 1.2; // need to be think later. this value is temp.

        this.ShieldCurrent = (int)(this.ShieldCurrent * Math.Pow(raised, specifies));
        this.ShieldMax = (int)(this.ShieldMax * Math.Pow(raised, specifies));
        this.HitPointCurrent = (int)(this.HitPointCurrent * Math.Pow(raised, specifies));
        this.HitPointMax = (int)(this.HitPointMax * Math.Pow(raised, specifies));
        this.Attack = (int)(this.Attack * Math.Pow(raised, specifies));
        this.KineticAttackRatio = (int)(this.KineticAttackRatio * Math.Pow(raised, specifies));
        this.ChemicalAttackRatio = (int)(this.ChemicalAttackRatio * Math.Pow(raised, specifies));
        this.ThermalAttackRatio = (int)(this.ThermalAttackRatio * Math.Pow(raised, specifies));
        this.CriticalHit = (int)(this.CriticalHit * Math.Pow(raised, specifies));
        this.NumberOfAttacks = (int)(this.NumberOfAttacks * Math.Pow(raised, specifies));

        //Note: MinRange and MaxRange is not be add up.
        //combatAdded.MinRange;
        //combatAdded.MaxRange;
        this.Accuracy = (int)(this.Accuracy * Math.Pow(raised, specifies));
        this.Mobility = (int)(this.Mobility * Math.Pow(raised, specifies));
        this.Defense = (int)(this.Defense * Math.Pow(raised, specifies));
        this.Counterintelligence = (int)(this.Counterintelligence * Math.Pow(raised, specifies));
        this.Repair = (int)(this.Repair * Math.Pow(raised, specifies));


        return this;
    }



    public CombatClass(int shieldCurrent, int shieldMax, int hitPointCurrent, int hitPointMax, int attack, double kineticAttackRatio, double chemicalAttackRatio,
double thermalAttackRatio, int criticalHit, int numberOfAttacks, int minRange, int maxRange, int accuracy, int mobility, int defense)
    {
        this.ShieldCurrent = shieldCurrent; this.ShieldMax = shieldMax; this.HitPointCurrent = hitPointCurrent; this.HitPointMax = hitPointMax; this.Attack = attack;
        this.KineticAttackRatio = kineticAttackRatio; this.ChemicalAttackRatio = chemicalAttackRatio; this.ThermalAttackRatio = thermalAttackRatio; this.CriticalHit = criticalHit;
        this.NumberOfAttacks = numberOfAttacks; this.MinRange = minRange; this.MaxRange = maxRange; this.Accuracy = accuracy; this.Mobility = mobility; this.Defense = defense;
    }

    public CombatClass()
    {

    }

}