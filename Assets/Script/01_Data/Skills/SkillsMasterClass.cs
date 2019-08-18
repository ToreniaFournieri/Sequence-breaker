using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill-", menuName = "Skill/SkillsMaster", order = 2)]
public class SkillsMasterClass : ScriptableObject
{

    //public SkillsMasterStruct(SkillName name, ActionType actionType, CallSkillLogicName callSkillLogicName, bool isHeal, int usageCount, int veiledTurn, Ability ability, TriggerBaseClass triggerBase,
    //  SkillMagnificationClass magnification, TriggerTargetClass triggerTarget, BuffTargetParameterClass buffTarget, SkillName callingBuffName, DebuffTargetParameterClass debuffTarget)
    //{
    //    this.Name = name; this.ActionType = actionType; this.CallSkillLogicName = callSkillLogicName; this.IsHeal = isHeal; this.UsageCount = usageCount; this.VeiledTurn = veiledTurn;
    //    this.Ability = ability; this.TriggerBase = triggerBase; this.Magnification = magnification; this.TriggerTarget = triggerTarget;
    //    this.BuffTarget = buffTarget; this.CallingBuffName = callingBuffName; this.DebuffTarget = debuffTarget;
    //}

    [SerializeField] public SkillName Name;
    [SerializeField] public ActionType ActionType { get; }
    [SerializeField] public CallSkillLogicName CallSkillLogicName { get; }
    [SerializeField] public bool IsHeal { get; }
    [SerializeField] public int UsageCount { get; }
    [SerializeField] public int VeiledTurn { get; }
    [SerializeField] public Ability Ability { get; }
    [SerializeField] public TriggerBaseClass TriggerBase { get; }
    [SerializeField] public SkillMagnificationClass Magnification { get; }
    [SerializeField] public TriggerTargetClass TriggerTarget { get; }
    [SerializeField] public BuffTargetParameterClass BuffTarget { get; }
    [SerializeField] public SkillName CallingBuffName { get; }
    [SerializeField] public DebuffTargetParameterClass DebuffTarget { get; }


    public SkillsMasterClass DeepCopy()
    {
        SkillsMasterClass other = (SkillsMasterClass)this.MemberwiseClone();
        return other;
    }
}
