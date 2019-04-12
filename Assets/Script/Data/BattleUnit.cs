using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "BattleUnit-", menuName = "BattleUnit/BattleUnit", order = 3)]
public class BattleUnit : ScriptableObject
{
    [SerializeField] public int UniqueID;
    [SerializeField] public string Name;
    [SerializeField] public Affiliation Affiliation;
    [SerializeField] public UnitType UnitType;
    [SerializeField] public AbilityClass Ability;
    [SerializeField] public BuffClass Buff;
    [SerializeField] public CombatClass Combat;
    [SerializeField] public FeatureClass Feature;
    [SerializeField] public OffenseMagnificationClass OffenseMagnification;
    [SerializeField] public DefenseMagnificationClass DefenseMagnification;
    [SerializeField] public UnitSkillMagnificationClass SkillMagnification;
    public bool IsOptimumTarget { get; set; }
    public bool IsBarrierExistJustBefore { get; set; }
    public bool IsBarrierBrokenJustNow { get; set; }
    public bool IsCrushedJustNow { get; set; }
    public bool IsAvoidMoreThanOnce { get; set; }
    public double Deterioration { get; set; }
    public StatisticsCollectionClass Statistics { get; set; }
    public StatisticsCollectionClass PermanentStatistics { get; set; }


    public AbilityClass InitialAbility;
    public enum Conditions { current, max }


    public BattleUnit(int uniqueID, string name, Affiliation affiliation, UnitType unitType, AbilityClass ability, CombatClass combat, FeatureClass feature,
     OffenseMagnificationClass offenseMagnification, DefenseMagnificationClass defenseMagnification, UnitSkillMagnificationClass skillMagnification)
    {
        this.UniqueID = uniqueID; this.Name = name; this.Affiliation = affiliation; this.UnitType = unitType; this.Ability = ability; this.Combat = combat;
        this.Feature = feature; this.OffenseMagnification = offenseMagnification; this.DefenseMagnification = defenseMagnification; this.SkillMagnification = skillMagnification;
        //Initialize 
        this.Deterioration = 0.0; this.Buff = new BuffClass(); this.IsOptimumTarget = false; this.IsBarrierBrokenJustNow = false;
        this.IsBarrierExistJustBefore = false; this.IsCrushedJustNow = false; this.IsAvoidMoreThanOnce = false;
        this.Statistics = new StatisticsCollectionClass(); this.PermanentStatistics = new StatisticsCollectionClass();
    }


    public class BuffClass
    {
        public int BarrierRemaining { get; set; }
        public double DefenseMagnification { get; set; }
        public double MobilityMagnification { get; set; }
        public double AttackMagnification { get; set; }
        public double AccuracyMagnification { get; set; }
        public double CriticalHitRateMagnification { get; set; }
        public double NumberOfAttackMagnification { get; set; }
        public int RangeMinCorrection { get; set; }
        public int RangeMaxCorrection { get; set; }

        public BuffClass()
        {
            this.BarrierRemaining = 0; this.DefenseMagnification = 1.0; this.MobilityMagnification = 1.0; this.AttackMagnification = 1.0; this.AccuracyMagnification = 1.0;
            this.CriticalHitRateMagnification = 1.0; this.NumberOfAttackMagnification = 1.0; this.RangeMinCorrection = 0; this.RangeMaxCorrection = 0;
        }

        public void InitializeBuff()
        {
            this.DefenseMagnification = 1.0; this.MobilityMagnification = 1.0; this.AttackMagnification = 1.0; this.AccuracyMagnification = 1.0;
            this.CriticalHitRateMagnification = 1.0; this.NumberOfAttackMagnification = 1.0; this.RangeMinCorrection = 0; this.RangeMaxCorrection = 0;
        }
        public void AddBarrier(int addBarrierCount) { this.BarrierRemaining += addBarrierCount; }
        // take 1 barrier. true=barrier has, false= no barrier anymore
        public bool RemoveBarrier() { if (this.BarrierRemaining > 0) { this.BarrierRemaining--; return true; } else { this.BarrierRemaining = 0; return false; } }
    }


    public class StatisticsCollectionClass
    {
        public StatisticsCollectionClass() { this.Initialize(); }

        public void Initialize()
        {
            this.NumberOfCrushed = 0; this.AllActivatedCount = 0; this.AllHitCount = 0; this.AllTotalDealtDamage = 0; this.AllTotalBeenHitCount = 0; this.AllTotalBeTakenDamage = 0;
            this.CriticalActivatedCount = 0; this.CriticalHitCount = 0; this.CriticalTotalDealtDamage = 0; this.CriticalBeenHitCount = 0; this.CriticalTotalBeTakenDamage = 0;
            this.SkillActivatedCount = 0; this.SkillHitCount = 0; this.SkillTotalDealtDamage = 0; this.SkillBeenHitCount = 0; this.SkillTotalBeTakenDamage = 0; this.AvoidCount = 0;
        }

        public void Avarage(int battleWaves)
        {
            this.NumberOfCrushed /= battleWaves;
            this.AllActivatedCount /= battleWaves;
            this.AllHitCount /= battleWaves;
            this.AllTotalDealtDamage /= battleWaves;
            this.AllTotalBeTakenDamage /= battleWaves;
            this.CriticalActivatedCount /= battleWaves;
            this.CriticalHitCount /= battleWaves;
            this.CriticalTotalDealtDamage /= battleWaves;
            this.CriticalTotalBeTakenDamage /= battleWaves;
            this.SkillActivatedCount /= battleWaves;
            this.SkillHitCount /= battleWaves;
            this.SkillTotalDealtDamage /= battleWaves;
            this.SkillTotalBeTakenDamage /= battleWaves;
        }
        public string AllCriticalRatio()
        {
            int countSpace = (3 - Math.Round(this.AllActivatedCount, 0).ToString().Length); if (countSpace <= 0) { countSpace = 0; }
            int criticalCountRateSpace = (3 - Math.Round((this.CriticalActivatedCount / this.AllActivatedCount * 100), 1).WithComma().Length); if (criticalCountRateSpace <= 0) { criticalCountRateSpace = 0; }
            int hitSpace = (5 - Math.Round(this.AllHitCount, 0).WithComma().Length); if (hitSpace <= 0) { hitSpace = 0; }
            int criticalHitRateSpace = (3 - Math.Round((this.CriticalHitCount / this.AllHitCount * 100), 1).WithComma().Length); if (criticalHitRateSpace <= 0) { criticalHitRateSpace = 0; }
            int totalDamageSpace = (8 - Math.Round(this.AllTotalDealtDamage, 1).WithComma().Length); if (totalDamageSpace <= 0) { totalDamageSpace = 0; }
            int criticalTotalDamageRateSpace = (3 - Math.Round((this.CriticalTotalDealtDamage / this.AllTotalDealtDamage * 100), 1).WithComma().Length); if (criticalTotalDamageRateSpace <= 0) { criticalTotalDamageRateSpace = 0; }
            int beTakenDamageSpace = (8 - Math.Round(this.AllTotalBeTakenDamage, 1).WithComma().Length); if (beTakenDamageSpace <= 0) { beTakenDamageSpace = 0; }
            int criticalBeTakenDamageRateSpace = (3 - Math.Round((this.CriticalTotalBeTakenDamage / this.AllTotalBeTakenDamage * 100), 1).WithComma().Length); if (criticalBeTakenDamageRateSpace <= 0) { criticalBeTakenDamageRateSpace = 0; }
            int crushedRateSpace = (3 - Math.Round((this.NumberOfCrushed) * 100, 0).WithComma().Length); if (crushedRateSpace <= 0) { crushedRateSpace = 0; }

            return "Attacks:" + new string(' ', countSpace) + Math.Round(this.AllActivatedCount, 0) + " (" + new string(' ',
             criticalCountRateSpace) + Math.Round((this.CriticalActivatedCount / this.AllActivatedCount * 100), 1).WithComma() + "%) Hit:"
            + new string(' ', hitSpace) + Math.Round(this.AllHitCount, 0) + " (" + new string(' ', criticalHitRateSpace) + Math.Round((this.CriticalHitCount / this.AllHitCount * 100), 1).WithComma() + "%) Damage:"
               + new string(' ', totalDamageSpace) + Math.Round(this.AllTotalDealtDamage, 1).WithComma() + " ("
            + new string(' ', criticalTotalDamageRateSpace) + Math.Round((this.CriticalTotalDealtDamage / this.AllTotalDealtDamage * 100), 1).WithComma()
             + "%) BeTaken: "
            + new string(' ', beTakenDamageSpace) + Math.Round(this.AllTotalBeTakenDamage, 1).WithComma() + " ("
            + new string(' ', criticalBeTakenDamageRateSpace) + Math.Round((this.CriticalTotalBeTakenDamage / this.AllTotalBeTakenDamage * 100), 1).WithComma() + "%)"
            + " Crushed:" + new string(' ', crushedRateSpace) + Math.Round((this.NumberOfCrushed) * 100, 0).WithComma() + "%)";
        }

        public string Skill()
        { return "[Skill] Count: " + this.SkillActivatedCount + " Hit: " + this.SkillHitCount + " Damage: " + this.SkillTotalDealtDamage + " (BeTakenDamage: " + this.SkillTotalBeTakenDamage + ")"; }

        public double NumberOfCrushed { get; set; }
        public double AllActivatedCount { get; set; }
        public double AllHitCount { get; set; }
        public double AllTotalDealtDamage { get; set; }
        public double AllTotalBeenHitCount { get; set; }
        public double AllTotalBeTakenDamage { get; set; }
        public double CriticalActivatedCount { get; set; }
        public double CriticalHitCount { get; set; }
        public double CriticalTotalDealtDamage { get; set; }
        public double CriticalBeenHitCount { get; set; }
        public double CriticalTotalBeTakenDamage { get; set; }
        public double SkillHitCount { get; set; }
        public double SkillTotalDealtDamage { get; set; }
        public double SkillBeenHitCount { get; set; }
        public double SkillTotalBeTakenDamage { get; set; }
        public double SkillActivatedCount { get; set; }
        public double AvoidCount { get; set; }
    }

    public void SetPermanentStatistics(StatisticsCollectionClass statistics)
    {
        PermanentStatistics.NumberOfCrushed += Statistics.NumberOfCrushed; PermanentStatistics.AllActivatedCount += Statistics.AllActivatedCount;
        PermanentStatistics.AllHitCount += Statistics.AllHitCount; PermanentStatistics.AllTotalDealtDamage += Statistics.AllTotalDealtDamage;
        PermanentStatistics.AllTotalBeenHitCount += Statistics.AllTotalBeenHitCount; PermanentStatistics.AllTotalBeTakenDamage += Statistics.AllTotalBeTakenDamage;
        PermanentStatistics.CriticalActivatedCount += Statistics.CriticalActivatedCount; PermanentStatistics.CriticalHitCount += Statistics.CriticalHitCount;
        PermanentStatistics.CriticalTotalDealtDamage += Statistics.CriticalTotalDealtDamage;
        PermanentStatistics.CriticalBeenHitCount += Statistics.CriticalBeenHitCount; PermanentStatistics.CriticalTotalBeTakenDamage += Statistics.CriticalTotalBeTakenDamage;
        PermanentStatistics.SkillActivatedCount += Statistics.SkillActivatedCount; PermanentStatistics.SkillHitCount += Statistics.SkillHitCount;
        PermanentStatistics.SkillTotalDealtDamage += Statistics.SkillTotalDealtDamage; PermanentStatistics.SkillBeenHitCount += Statistics.SkillBeenHitCount;
        PermanentStatistics.SkillTotalBeTakenDamage += Statistics.SkillTotalBeTakenDamage; PermanentStatistics.AvoidCount += Statistics.AvoidCount;

    }

}
