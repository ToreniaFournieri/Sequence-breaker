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

            this.character = character; this.skill = skill; ActionType = actionType; this.offenseEffectMagnification = offenseEffectMagnification;
            this.triggeredPossibility = triggeredPossibility;
            IsRescueAble = isRescueAble; UsageCount = usageCount;
            VeiledFromTurn = veiledFromTurn; VeiledToTurn = veiledToTurn;
            SpentCount = 0;


            accumulationBaseRate = (int)skill.triggerBase.accumulationBaseRate;
            NextAccumulationCount = accumulationBaseRate;
            IsntTriggeredBecause = new IsntTriggeredBecauseClass();
        }
        
        public void Set(BattleUnit.BattleUnit iCharacter, SkillsMasterClass iSkill, ActionType iActionType, double iOffenseEffectMagnification,
            double iTriggeredPossibility, bool iIsRescueAble, int iUsageCount,
            int veiledFromTurn, int veiledToTurn)
        {

            character = iCharacter; skill = iSkill; ActionType = iActionType; offenseEffectMagnification = iOffenseEffectMagnification;
            triggeredPossibility = iTriggeredPossibility;
            IsRescueAble = iIsRescueAble; UsageCount = iUsageCount;
            VeiledFromTurn = veiledFromTurn; VeiledToTurn = veiledToTurn;
            SpentCount = 0;


            accumulationBaseRate = (int)iSkill.triggerBase.accumulationBaseRate;
            NextAccumulationCount = accumulationBaseRate;
            IsntTriggeredBecause = new IsntTriggeredBecauseClass();
        }

        public void InitializeEffect()
        {
            SpentCount = 0;
            NextAccumulationCount = accumulationBaseRate;

            if (IsntTriggeredBecause == null) { IsntTriggeredBecause = new IsntTriggeredBecauseClass(); }
            else { IsntTriggeredBecause.Initialize(); }

            SkillsMasterClass copyedSkillsMaster = skill.DeepCopy(); //NEED TEST!! This may not works, i want copy the value, not reference.
            IsRescueAble = copyedSkillsMaster.isHeal; // sample implemented.
            ActionType = copyedSkillsMaster.actionType;
            BuffToCharacter(currentTurn: 1);

            UsageCount = copyedSkillsMaster.usageCount;
            VeiledFromTurn = 1;
            VeiledToTurn = 20;

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
                IsItCalled = false; TriggerCondition = false; AfterAllMoved = false; TriggerTargetCounter = false; TriggerTargetChain = false;
                TriggerTargetReAttack = false; TriggerTargetMove = false; Critical = false; NonCritical = false;
                OnlyWhenBeenHitMoreThanOnce = false; OnlyWhenAvoidMoreThanOnce = false; AccumulationAvoid = false; AccumulationAllHitCount = false;
                AccumulationAllTotalBeenHit = false; AccumulationCriticalBeenHit = false; AccumulationCriticalHit = false; AccumulationSkillBeenHit = false;
                AccumulationSkillHit = false; TriggeredPossibility = false;
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
