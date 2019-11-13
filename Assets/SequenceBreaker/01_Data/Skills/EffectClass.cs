using SequenceBreaker._00_System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._01_Data.Skills
{
    [CreateAssetMenu(fileName = "Skill-", menuName = "Skill/EffectClass", order = 10)]
    public sealed class EffectClass : ScriptableObject
    {
        [FormerlySerializedAs("Character")] [SerializeField] public BattleUnit.BattleUnit character;
        [FormerlySerializedAs("Skill")] [SerializeField] public SkillsMasterClass skill;
        [FormerlySerializedAs("OffenseEffectMagnification")] [SerializeField] public double offenseEffectMagnification;
        [FormerlySerializedAs("TriggeredPossibility")] [SerializeField] public double triggeredPossibility;
        [FormerlySerializedAs("AccumulationBaseRate")] [SerializeField] public int accumulationBaseRate;
        
        // for normal skill
        public EffectClass(BattleUnit.BattleUnit character, SkillsMasterClass skill, ActionType actionType, double offenseEffectMagnification, double triggeredPossibility, bool isRescueAble, int usageCount,
            int veiledFromTurn, int veiledToTurn)
        {

            this.character = character; this.skill = skill; this.ActionType = actionType; this.offenseEffectMagnification = offenseEffectMagnification;
            this.triggeredPossibility = triggeredPossibility;
            this.IsRescueAble = isRescueAble; this.UsageCount = usageCount;
            this.VeiledFromTurn = veiledFromTurn; this.VeiledToTurn = veiledToTurn;
            this.SpentCount = 0;


            this.accumulationBaseRate = (int)skill.triggerBase.accumulationBaseRate;
            this.NextAccumulationCount = accumulationBaseRate;
            this.IsntTriggeredBecause = new IsntTriggeredBecauseClass();
        }

        public void InitializeEffect()
        {
            this.SpentCount = 0;
            this.NextAccumulationCount = accumulationBaseRate;

            if (this.IsntTriggeredBecause == null) { this.IsntTriggeredBecause = new IsntTriggeredBecauseClass(); }
            else { this.IsntTriggeredBecause.Initialize(); }

            SkillsMasterClass copyedSkillsMaster = this.skill.DeepCopy(); //NEED TEST!! This may not works, i want copy the value, not reference.
            this.IsRescueAble = copyedSkillsMaster.isHeal; // sample implemented.
            this.ActionType = copyedSkillsMaster.actionType;
            this.BuffToCharacter(currentTurn: 1);

            this.UsageCount = copyedSkillsMaster.usageCount;
            this.VeiledFromTurn = 1;
            this.VeiledToTurn = 20;

        }

        public void BuffToCharacter(int currentTurn)
        {
            if (currentTurn <= VeiledToTurn && currentTurn >= VeiledFromTurn)
            {
                character.buff.DefenseMagnification *= skill.buffTarget.defenseMagnification;
                character.buff.MobilityMagnification *= skill.buffTarget.mobilityMagnification;
                character.buff.AttackMagnification *= skill.buffTarget.attackMagnification;
                character.buff.AccuracyMagnification *= skill.buffTarget.accuracyMagnification;
                character.buff.CriticalHitRateMagnification *= skill.buffTarget.criticalHitRateMagnification;
                character.buff.NumberOfAttackMagnification *= skill.buffTarget.criticalHitRateMagnification;
                character.buff.RangeMinCorrection += skill.buffTarget.rangeMinCorrection;
                character.buff.RangeMaxCorrection += skill.buffTarget.rangeMaxCorrection;
            }
        }

        public sealed class IsntTriggeredBecauseClass
        {
            public IsntTriggeredBecauseClass() { Initialize(); }
            public void Initialize()
            {
                this.IsItCalled = false; this.TriggerCondition = false; this.AfterAllMoved = false; this.TriggerTargetCounter = false; this.TriggerTargetChain = false;
                this.TriggerTargetReAttack = false; this.TriggerTargetMove = false; this.Critical = false; this.NonCritical = false;
                this.OnlyWhenBeenHitMoreThanOnce = false; this.OnlyWhenAvoidMoreThanOnce = false; this.AccumulationAvoid = false; this.AccumulationAllHitCount = false;
                this.AccumulationAllTotalBeenHit = false; this.AccumulationCriticalBeenHit = false; this.AccumulationCriticalHit = false; this.AccumulationSkillBeenHit = false;
                this.AccumulationSkillHit = false; this.TriggeredPossibility = false;
            }

            public bool IsItCalled { get; set; }
            public bool TriggerCondition { get; set; }
            public bool AfterAllMoved { get; set; }
            public bool TriggerTargetCounter { get; set; }
            public bool TriggerTargetChain { get; set; }
            public bool TriggerTargetReAttack { get; set; }
            public bool TriggerTargetMove { get; set; }
            public bool Critical { get; set; }
            public bool NonCritical { get; set; }
            public bool OnlyWhenBeenHitMoreThanOnce { get; set; }
            public bool OnlyWhenAvoidMoreThanOnce { get; set; }
            public bool AccumulationAvoid { get; set; }
            public bool AccumulationAllHitCount { get; set; }
            public bool AccumulationAllTotalBeenHit { get; set; }
            public bool AccumulationCriticalBeenHit { get; set; }
            public bool AccumulationCriticalHit { get; set; }
            public bool AccumulationSkillBeenHit { get; set; }
            public bool AccumulationSkillHit { get; set; }
            public bool TriggeredPossibility { get; set; }
        }

        public IsntTriggeredBecauseClass IsntTriggeredBecause { get; set; }
        public ActionType ActionType { get; set; }
        public bool IsRescueAble { get; set; }
        public int UsageCount { get; set; }
        public int SpentCount { get; set; }
        public int NextAccumulationCount { get; set; }
        public int VeiledFromTurn { get; set; }
        public int VeiledToTurn { get; set; }


    }
}
