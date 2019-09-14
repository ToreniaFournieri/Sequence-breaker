using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillMaster-", menuName = "Skill/SkillsMaster", order = 2)]
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
    [SerializeField] public ActionType ActionType;
    [SerializeField] public CallSkillLogicName CallSkillLogicName;
    [SerializeField] public bool IsHeal;
    [SerializeField] public int UsageCount;
    [SerializeField] public int VeiledTurn;
    [SerializeField] public Ability Ability;
    [SerializeField] public TriggerBaseClass TriggerBase;
    [SerializeField] public SkillMagnificationClass Magnification;
    [SerializeField] public TriggerTargetClass TriggerTarget;
    [SerializeField] public BuffTargetParameterClass BuffTarget;
    [SerializeField] public SkillName CallingBuffName;
    [SerializeField] public DebuffTargetParameterClass DebuffTarget;


    public SkillsMasterClass DeepCopy()
    {
        SkillsMasterClass other = (SkillsMasterClass)this.MemberwiseClone();
        return other;
    }
}
