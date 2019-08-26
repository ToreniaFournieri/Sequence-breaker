using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill-", menuName = "Skill/EffectClass", order = 10)]
public class EffectClass : ScriptableObject
{
    [SerializeField] public BattleUnit Character;
    [SerializeField] public SkillsMasterClass Skill;
    [SerializeField] public double OffenseEffectMagnification;
    [SerializeField] public double TriggeredPossibility;
    [SerializeField] public int AccumulationBaseRate;

    //private SkillsMasterClass _skill;

    // for normal skill
    public EffectClass(BattleUnit character, SkillsMasterClass skill, ActionType actionType, double offenseEffectMagnification, double triggeredPossibility, bool isDamageControlAssistAble, int usageCount,
    int veiledFromTurn, int veiledToTurn)
    {

        this.Character = character; this.Skill = skill; this.ActionType = actionType; this.OffenseEffectMagnification = offenseEffectMagnification;
        this.TriggeredPossibility = triggeredPossibility;
        this.IsDamageControlAssistAble = isDamageControlAssistAble; this.UsageCount = usageCount;
        this.VeiledFromTurn = veiledFromTurn; this.VeiledToTurn = veiledToTurn;
        this.SpentCount = 0;


        this.AccumulationBaseRate = (int)skill.TriggerBase.AccumulationBaseRate;
        this.NextAccumulationCount = AccumulationBaseRate;
        this.IsntTriggeredBecause = new IsntTriggeredBecauseClass();
    }

    public void InitializeEffect()
    {
        this.SpentCount = 0;
        this.NextAccumulationCount = AccumulationBaseRate;

        if (this.IsntTriggeredBecause == null) { this.IsntTriggeredBecause = new IsntTriggeredBecauseClass(); }
        else { this.IsntTriggeredBecause.Initialize(); }

        SkillsMasterClass copyedSkillsMaster = this.Skill.DeepCopy(); //NEED TEST!! This may not works, i want copy the value, not reference.
        this.IsDamageControlAssistAble = copyedSkillsMaster.IsHeal; // sample implemented.
        this.ActionType = copyedSkillsMaster.ActionType;
        this.BuffToCharacter(currentTurn: 1);

        this.UsageCount = copyedSkillsMaster.UsageCount;
        this.VeiledFromTurn = 1;
        this.VeiledToTurn = 20;

    }

    public void BuffToCharacter(int currentTurn)
    {
        if (currentTurn <= VeiledToTurn && currentTurn >= VeiledFromTurn)
        {
            Character.Buff.DefenseMagnification *= Skill.BuffTarget.DefenseMagnification;
            Character.Buff.MobilityMagnification *= Skill.BuffTarget.MobilityMagnification;
            Character.Buff.AttackMagnification *= Skill.BuffTarget.AttackMagnification;
            Character.Buff.AccuracyMagnification *= Skill.BuffTarget.AccuracyMagnification;
            Character.Buff.CriticalHitRateMagnification *= Skill.BuffTarget.CriticalHitRateMagnification;
            Character.Buff.NumberOfAttackMagnification *= Skill.BuffTarget.CriticalHitRateMagnification;
            Character.Buff.RangeMinCorrection += Skill.BuffTarget.RangeMinCorrection;
            Character.Buff.RangeMaxCorrection += Skill.BuffTarget.RangeMaxCorrection;
        }
    }

    public class IsntTriggeredBecauseClass
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
    public bool IsDamageControlAssistAble { get; set; }
    public int UsageCount { get; set; }
    public int SpentCount { get; set; }
    public int NextAccumulationCount { get; set; }
    public int VeiledFromTurn { get; set; }
    public int VeiledToTurn { get; set; }


}
