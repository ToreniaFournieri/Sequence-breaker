using System;
using UnityEngine;
using UnityEngine.Serialization;

//Combat Status, the status whitch has been calculated by parts.
namespace SequenceBreaker._01_Data._08_BattleUnitSub
{
    [CreateAssetMenu(fileName = "CombatClass-", menuName = "BattleUnit/CombatClass", order = 14)]
    public class CombatClass : ScriptableObject
    {

        [FormerlySerializedAs("ShieldCurrent")] [SerializeField] public int shieldCurrent;
        [FormerlySerializedAs("ShieldMax")] [SerializeField] public int shieldMax;
        [FormerlySerializedAs("HitPointCurrent")] [SerializeField] public int hitPointCurrent;
        [FormerlySerializedAs("HitPointMax")] [SerializeField] public int hitPointMax;
        [FormerlySerializedAs("Attack")] [SerializeField] public int attack;
        [FormerlySerializedAs("KineticAttackRatio")] [SerializeField] public double kineticAttackRatio;
        [FormerlySerializedAs("ChemicalAttackRatio")] [SerializeField] public double chemicalAttackRatio;
        [FormerlySerializedAs("ThermalAttackRatio")] [SerializeField] public double thermalAttackRatio;
        [FormerlySerializedAs("CriticalHit")] [SerializeField] public int criticalHit;
        [FormerlySerializedAs("NumberOfAttacks")] [SerializeField] public int numberOfAttacks;
        [FormerlySerializedAs("MinRange")] [SerializeField] public int minRange;
        [FormerlySerializedAs("MaxRange")] [SerializeField] public int maxRange;
        [FormerlySerializedAs("Accuracy")] [SerializeField] public int accuracy;
        [FormerlySerializedAs("Mobility")] [SerializeField] public int mobility;
        [FormerlySerializedAs("Defense")] [SerializeField] public int defense;

        //New since 2019/08/16 use intelligence, generation
        [FormerlySerializedAs("Counterintelligence")] [SerializeField] public int counterintelligence;
        [FormerlySerializedAs("Repair")] [SerializeField] public int repair;

        public CombatClass Copy()
        {
            return (CombatClass)this.MemberwiseClone();
        }


        public CombatClass Add(CombatClass combatFrom)
        {
            this.shieldCurrent += combatFrom.shieldCurrent;
            this.shieldMax += combatFrom.shieldMax;
            this.hitPointCurrent += combatFrom.hitPointCurrent;
            this.hitPointMax += combatFrom.hitPointMax;
            this.attack += combatFrom.attack;
            this.kineticAttackRatio += combatFrom.kineticAttackRatio;
            this.chemicalAttackRatio += combatFrom.chemicalAttackRatio;
            this.thermalAttackRatio += combatFrom.thermalAttackRatio;
            this.criticalHit += combatFrom.criticalHit;
            this.numberOfAttacks += combatFrom.numberOfAttacks;

            //Note: MinRange and MaxRange is not be add up.
            //combatAdded.MinRange;
            //combatAdded.MaxRange;
            this.accuracy += combatFrom.accuracy;
            this.mobility += combatFrom.mobility;
            this.defense += combatFrom.defense;
            this.counterintelligence += combatFrom.counterintelligence;
            this.repair += combatFrom.repair;

            return this;
        }

        public CombatClass Pow(int specifies)
        {
            const double raised = 1.2; // need to be think later. this value is temp.

            this.shieldCurrent = (int)(this.shieldCurrent * Math.Pow(raised, specifies));
            this.shieldMax = (int)(this.shieldMax * Math.Pow(raised, specifies));
            this.hitPointCurrent = (int)(this.hitPointCurrent * Math.Pow(raised, specifies));
            this.hitPointMax = (int)(this.hitPointMax * Math.Pow(raised, specifies));
            this.attack = (int)(this.attack * Math.Pow(raised, specifies));
            this.kineticAttackRatio = (int)(this.kineticAttackRatio * Math.Pow(raised, specifies));
            this.chemicalAttackRatio = (int)(this.chemicalAttackRatio * Math.Pow(raised, specifies));
            this.thermalAttackRatio = (int)(this.thermalAttackRatio * Math.Pow(raised, specifies));
            this.criticalHit = (int)(this.criticalHit * Math.Pow(raised, specifies));
            this.numberOfAttacks = (int)(this.numberOfAttacks * Math.Pow(raised, specifies));

            //Note: MinRange and MaxRange is not be add up.
            //combatAdded.MinRange;
            //combatAdded.MaxRange;
            this.accuracy = (int)(this.accuracy * Math.Pow(raised, specifies));
            this.mobility = (int)(this.mobility * Math.Pow(raised, specifies));
            this.defense = (int)(this.defense * Math.Pow(raised, specifies));
            this.counterintelligence = (int)(this.counterintelligence * Math.Pow(raised, specifies));
            this.repair = (int)(this.repair * Math.Pow(raised, specifies));


            return this;
        }



        public CombatClass(int shieldCurrent, int shieldMax, int hitPointCurrent, int hitPointMax, int attack, double kineticAttackRatio, double chemicalAttackRatio,
            double thermalAttackRatio, int criticalHit, int numberOfAttacks, int minRange, int maxRange, int accuracy, int mobility, int defense)
        {
            this.shieldCurrent = shieldCurrent; this.shieldMax = shieldMax; this.hitPointCurrent = hitPointCurrent; this.hitPointMax = hitPointMax; this.attack = attack;
            this.kineticAttackRatio = kineticAttackRatio; this.chemicalAttackRatio = chemicalAttackRatio; this.thermalAttackRatio = thermalAttackRatio; this.criticalHit = criticalHit;
            this.numberOfAttacks = numberOfAttacks; this.minRange = minRange; this.maxRange = maxRange; this.accuracy = accuracy; this.mobility = mobility; this.defense = defense;
        }

        public CombatClass()
        {

        }

    }
}