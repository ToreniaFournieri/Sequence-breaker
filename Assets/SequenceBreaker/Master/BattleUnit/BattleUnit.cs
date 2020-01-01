using System;
using System.Globalization;
using SequenceBreaker.Environment;
using SequenceBreaker.Play.Battle;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker.Master.BattleUnit
{
    [CreateAssetMenu(fileName = "BattleUnit-", menuName = "BattleUnit/BattleUnit", order = 3)]
    public sealed class BattleUnit : ScriptableObject
    {
        [FormerlySerializedAs("UniqueID")] [SerializeField] public int uniqueId;
        [SerializeField] public string longName;
        [SerializeField] public string shortName;
        [FormerlySerializedAs("Affiliation")] [SerializeField] public Affiliation affiliation;
        [FormerlySerializedAs("UnitType")] [SerializeField] public UnitType unitType;
        [FormerlySerializedAs("Buff")] [SerializeField] public BuffClass buff;
        [FormerlySerializedAs("Combat")] [SerializeField] public CombatClass combat;
        [FormerlySerializedAs("Feature")] [SerializeField] public FeatureClass feature;
        [FormerlySerializedAs("OffenseMagnification")] [SerializeField] public OffenseMagnificationClass offenseMagnification;
        [FormerlySerializedAs("DefenseMagnification")] [SerializeField] public DefenseMagnificationClass defenseMagnification;
        [FormerlySerializedAs("SkillMagnification")] [SerializeField] public UnitSkillMagnificationClass skillMagnification;
        public bool IsOptimumTarget { get; set; }
        public bool IsBarrierExistJustBefore { get; set; }
        public bool IsBarrierBrokenJustNow { get; set; }
        public bool IsCrushedJustNow { get; set; }
        public bool IsAvoidMoreThanOnce { get; set; }
        public double Deterioration { get; set; }
        public StatisticsCollectionClass Statistics { get; set; }
        public StatisticsCollectionClass PermanentStatistics { get; set; }

        // One time Values
        public double effectiveDefense;

        [FormerlySerializedAs("Ability")] [HideInInspector] public AbilityClass ability;
        [FormerlySerializedAs("InitialAbility")] [SerializeField] public AbilityClass initialAbility;
        public enum Conditions { Current, Max }

        public BattleUnit Copy()
        {
            BattleUnit other = (BattleUnit)MemberwiseClone();
            other.combat = combat.Copy();
            other.buff = buff.Copy();
            return other;
        }


        public BattleUnit(int uniqueId, string longName, string shortName, Affiliation affiliation, UnitType unitType, AbilityClass ability, CombatClass combat, FeatureClass feature,
            OffenseMagnificationClass offenseMagnification, DefenseMagnificationClass defenseMagnification, UnitSkillMagnificationClass skillMagnification)
        {
            this.uniqueId = uniqueId; this.longName = longName; this.shortName = shortName ; this.affiliation = affiliation; this.unitType = unitType; this.ability = ability; this.combat = combat;
            this.feature = feature; this.offenseMagnification = offenseMagnification; this.defenseMagnification = defenseMagnification; this.skillMagnification = skillMagnification;
            //Initialize 
            Deterioration = 0.0; buff = new BuffClass(); IsOptimumTarget = false; IsBarrierBrokenJustNow = false;
            IsBarrierExistJustBefore = false; IsCrushedJustNow = false; IsAvoidMoreThanOnce = false;
            Statistics = new StatisticsCollectionClass(); PermanentStatistics = new StatisticsCollectionClass();
        }

        public void Set(int iUniqueId, string ilongName, string iShortName, Affiliation iAffiliation, UnitType iUnitType, AbilityClass iAbility, CombatClass iCombat, FeatureClass iFeature,
            OffenseMagnificationClass iOffenseMagnification, DefenseMagnificationClass iDefenseMagnification, UnitSkillMagnificationClass iSkillMagnification)
        {

            uniqueId = iUniqueId; longName = ilongName; shortName = iShortName ; affiliation = iAffiliation; unitType = iUnitType; ability = iAbility; combat = iCombat;
            feature = iFeature; offenseMagnification = iOffenseMagnification; defenseMagnification = iDefenseMagnification; skillMagnification = iSkillMagnification;
            //Initialize 
            Deterioration = 0.0; buff = new BuffClass(); IsOptimumTarget = false; IsBarrierBrokenJustNow = false;
            IsBarrierExistJustBefore = false; IsCrushedJustNow = false; IsAvoidMoreThanOnce = false;
            Statistics = new StatisticsCollectionClass(); PermanentStatistics = new StatisticsCollectionClass();
        }

        public string GetShieldHp()
        {
            return "Sh: " + combat.shieldCurrent + "/" + combat.shieldMax + " HP: " + combat.hitPointCurrent + "/" + combat.hitPointMax;
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
                BarrierRemaining = 0; DefenseMagnification = 1.0; MobilityMagnification = 1.0; AttackMagnification = 1.0; AccuracyMagnification = 1.0;
                CriticalHitRateMagnification = 1.0; NumberOfAttackMagnification = 1.0; RangeMinCorrection = 0; RangeMaxCorrection = 0;
            }

            public BuffClass Copy()
            {
                return (BuffClass)MemberwiseClone();
            }


            public void InitializeBuff()
            {
                DefenseMagnification = 1.0; MobilityMagnification = 1.0; AttackMagnification = 1.0; AccuracyMagnification = 1.0;
                CriticalHitRateMagnification = 1.0; NumberOfAttackMagnification = 1.0; RangeMinCorrection = 0; RangeMaxCorrection = 0;
            }
            public void AddBarrier(int addBarrierCount) { BarrierRemaining += addBarrierCount; }
            // take 1 barrier. true=barrier has, false= no barrier anymore
            public bool RemoveBarrier()
            {
                if (BarrierRemaining > 0) { BarrierRemaining--; return true; }

                BarrierRemaining = 0; return false;
            }
        }


        public class StatisticsCollectionClass
        {
            public StatisticsCollectionClass() { Initialize(); }

            public void Initialize()
            {
                NumberOfCrushed = 0; AllActivatedCount = 0; AllHitCount = 0; AllTotalDealtDamage = 0; AllTotalBeenHitCount = 0; AllTotalBeTakenDamage = 0;
                CriticalActivatedCount = 0; CriticalHitCount = 0; CriticalTotalDealtDamage = 0; CriticalBeenHitCount = 0; CriticalTotalBeTakenDamage = 0;
                SkillActivatedCount = 0; SkillHitCount = 0; SkillTotalDealtDamage = 0; SkillBeenHitCount = 0; SkillTotalBeTakenDamage = 0; AvoidCount = 0;
            }

            public void Avarage(int battleWaves)
            {
                NumberOfCrushed /= battleWaves;
                AllActivatedCount /= battleWaves;
                AllHitCount /= battleWaves;
                AllTotalDealtDamage /= battleWaves;
                AllTotalBeTakenDamage /= battleWaves;
                CriticalActivatedCount /= battleWaves;
                CriticalHitCount /= battleWaves;
                CriticalTotalDealtDamage /= battleWaves;
                CriticalTotalBeTakenDamage /= battleWaves;
                SkillActivatedCount /= battleWaves;
                SkillHitCount /= battleWaves;
                SkillTotalDealtDamage /= battleWaves;
                SkillTotalBeTakenDamage /= battleWaves;
            }
            public string AllCriticalRatio()
            {
                int countSpace = (3 - Math.Round(AllActivatedCount, 0).ToString(CultureInfo.InvariantCulture).Length); if (countSpace <= 0) { countSpace = 0; }
                int criticalCountRateSpace = (3 - Math.Round((CriticalActivatedCount / AllActivatedCount * 100), 1).WithComma().Length); if (criticalCountRateSpace <= 0) { criticalCountRateSpace = 0; }
                int hitSpace = (5 - Math.Round(AllHitCount, 0).WithComma().Length); if (hitSpace <= 0) { hitSpace = 0; }
                int criticalHitRateSpace = (3 - Math.Round((CriticalHitCount / AllHitCount * 100), 1).WithComma().Length); if (criticalHitRateSpace <= 0) { criticalHitRateSpace = 0; }
                int totalDamageSpace = (8 - Math.Round(AllTotalDealtDamage, 1).WithComma().Length); if (totalDamageSpace <= 0) { totalDamageSpace = 0; }
                int criticalTotalDamageRateSpace = (3 - Math.Round((CriticalTotalDealtDamage / AllTotalDealtDamage * 100), 1).WithComma().Length); if (criticalTotalDamageRateSpace <= 0) { criticalTotalDamageRateSpace = 0; }
                int beTakenDamageSpace = (8 - Math.Round(AllTotalBeTakenDamage, 1).WithComma().Length); if (beTakenDamageSpace <= 0) { beTakenDamageSpace = 0; }
                int criticalBeTakenDamageRateSpace = (3 - Math.Round((CriticalTotalBeTakenDamage / AllTotalBeTakenDamage * 100), 1).WithComma().Length); if (criticalBeTakenDamageRateSpace <= 0) { criticalBeTakenDamageRateSpace = 0; }
                int crushedRateSpace = (3 - Math.Round((NumberOfCrushed) * 100, 0).WithComma().Length); if (crushedRateSpace <= 0) { crushedRateSpace = 0; }

                return "Attacks:" + new string(' ', countSpace) + Math.Round(AllActivatedCount, 0) + " (" + new string(' ',
                           criticalCountRateSpace) + Math.Round((CriticalActivatedCount / AllActivatedCount * 100), 1).WithComma() + "%) Hit:"
                       + new string(' ', hitSpace) + Math.Round(AllHitCount, 0) + " (" + new string(' ', criticalHitRateSpace) + Math.Round((CriticalHitCount / AllHitCount * 100), 1).WithComma() + "%) Damage:"
                       + new string(' ', totalDamageSpace) + Math.Round(AllTotalDealtDamage, 1).WithComma() + " ("
                       + new string(' ', criticalTotalDamageRateSpace) + Math.Round((CriticalTotalDealtDamage / AllTotalDealtDamage * 100), 1).WithComma()
                       + "%) BeTaken: "
                       + new string(' ', beTakenDamageSpace) + Math.Round(AllTotalBeTakenDamage, 1).WithComma() + " ("
                       + new string(' ', criticalBeTakenDamageRateSpace) + Math.Round((CriticalTotalBeTakenDamage / AllTotalBeTakenDamage * 100), 1).WithComma() + "%)"
                       + " Crushed:" + new string(' ', crushedRateSpace) + Math.Round((NumberOfCrushed) * 100, 0).WithComma() + "%)";
            }

            public string Skill()
            { return "[Skill] Count: " + SkillActivatedCount + " Hit: " + SkillHitCount + " Damage: " + SkillTotalDealtDamage + " (BeTakenDamage: " + SkillTotalBeTakenDamage + ")"; }

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
}
