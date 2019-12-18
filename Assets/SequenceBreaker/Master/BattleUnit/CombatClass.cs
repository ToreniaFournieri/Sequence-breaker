using System;
using UnityEngine;
using UnityEngine.Serialization;

//Combat Status, the status whitch has been calculated by parts.
namespace SequenceBreaker.Master.BattleUnit
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
            return (CombatClass)MemberwiseClone();
        }


        public CombatClass Add(CombatClass combatFrom)
        {
            shieldCurrent += combatFrom.shieldCurrent;
            shieldMax += combatFrom.shieldMax;
            hitPointCurrent += combatFrom.hitPointCurrent;
            hitPointMax += combatFrom.hitPointMax;
            attack += combatFrom.attack;
            kineticAttackRatio += combatFrom.kineticAttackRatio;
            chemicalAttackRatio += combatFrom.chemicalAttackRatio;
            thermalAttackRatio += combatFrom.thermalAttackRatio;
            criticalHit += combatFrom.criticalHit;
            numberOfAttacks += combatFrom.numberOfAttacks;

            //Note: MinRange and MaxRange is not be add up.
            //combatAdded.MinRange;
            //combatAdded.MaxRange;
            accuracy += combatFrom.accuracy;
            mobility += combatFrom.mobility;
            defense += combatFrom.defense;
            counterintelligence += combatFrom.counterintelligence;
            repair += combatFrom.repair;

            return this;
        }

        public void AdjustElementAttackRatio()
        {
            double _kinetic = kineticAttackRatio;
            double _chemical = chemicalAttackRatio;
            double _thermal = thermalAttackRatio;

            kineticAttackRatio = _kinetic / (_kinetic + _chemical + _thermal);
            chemicalAttackRatio = _chemical / (_kinetic + _chemical + _thermal);
            thermalAttackRatio = _thermal / (_kinetic + _chemical + _thermal);

        }

        public CombatClass Pow(int specifies)
        {
            const double raised = 1.2; // need to be think later. this value is temp.

            shieldCurrent = (int)(shieldCurrent * Math.Pow(raised, specifies));
            shieldMax = (int)(shieldMax * Math.Pow(raised, specifies));
            hitPointCurrent = (int)(hitPointCurrent * Math.Pow(raised, specifies));
            hitPointMax = (int)(hitPointMax * Math.Pow(raised, specifies));
            attack = (int)(attack * Math.Pow(raised, specifies));
            //kineticAttackRatio = (kineticAttackRatio * Math.Pow(raised, specifies));
            //chemicalAttackRatio = (chemicalAttackRatio * Math.Pow(raised, specifies));
            //thermalAttackRatio = (thermalAttackRatio * Math.Pow(raised, specifies));
            criticalHit = (int)(criticalHit * Math.Pow(raised, specifies));
            numberOfAttacks = (int)(numberOfAttacks * Math.Pow(raised, specifies));

            //Note: MinRange and MaxRange is not be add up.
            //combatAdded.MinRange;
            //combatAdded.MaxRange;
            accuracy = (int)(accuracy * Math.Pow(raised, specifies));
            mobility = (int)(mobility * Math.Pow(raised, specifies));
            defense = (int)(defense * Math.Pow(raised, specifies));
            counterintelligence = (int)(counterintelligence * Math.Pow(raised, specifies));
            repair = (int)(repair * Math.Pow(raised, specifies));


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