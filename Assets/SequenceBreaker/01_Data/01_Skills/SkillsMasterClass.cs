using SequenceBreaker._00_System;
using SequenceBreaker._01_Data._01_Skills.Subclass;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._01_Data._01_Skills
{
    [CreateAssetMenu(fileName = "SkillMaster-", menuName = "Skill/SkillsMaster", order = 2)]
    public sealed class SkillsMasterClass : ScriptableObject
    {

        //public SkillsMasterStruct(SkillName name, ActionType actionType, CallSkillLogicName callSkillLogicName, bool isHeal, int usageCount, int veiledTurn, Ability ability, TriggerBaseClass triggerBase,
        //  SkillMagnificationClass magnification, TriggerTargetClass triggerTarget, BuffTargetParameterClass buffTarget, SkillName callingBuffName, DebuffTargetParameterClass debuffTarget)
        //{
        //    this.Name = name; this.ActionType = actionType; this.CallSkillLogicName = callSkillLogicName; this.IsHeal = isHeal; this.UsageCount = usageCount; this.VeiledTurn = veiledTurn;
        //    this.Ability = ability; this.TriggerBase = triggerBase; this.Magnification = magnification; this.TriggerTarget = triggerTarget;
        //    this.BuffTarget = buffTarget; this.CallingBuffName = callingBuffName; this.DebuffTarget = debuffTarget;
        //}

        [FormerlySerializedAs("Name")] [SerializeField] public SkillName name;
        [FormerlySerializedAs("ActionType")] [SerializeField] public ActionType actionType;
        [FormerlySerializedAs("CallSkillLogicName")] [SerializeField] public CallSkillLogicName callSkillLogicName;
        [FormerlySerializedAs("IsHeal")] [SerializeField] public bool isHeal;
        [FormerlySerializedAs("UsageCount")] [SerializeField] public int usageCount;
        [FormerlySerializedAs("VeiledTurn")] [SerializeField] public int veiledTurn;
        [FormerlySerializedAs("Ability")] [SerializeField] public Ability ability;
        [FormerlySerializedAs("TriggerBase")] [SerializeField] public TriggerBaseClass triggerBase;
        [FormerlySerializedAs("Magnification")] [SerializeField] public SkillMagnificationClass magnification;
        [FormerlySerializedAs("TriggerTarget")] [SerializeField] public TriggerTargetClass triggerTarget;
        [FormerlySerializedAs("BuffTarget")] [SerializeField] public BuffTargetParameterClass buffTarget;
        [FormerlySerializedAs("CallingBuffName")] [SerializeField] public SkillName callingBuffName;
        [FormerlySerializedAs("DebuffTarget")] [SerializeField] public DebuffTargetParameterClass debuffTarget;


        public SkillsMasterClass DeepCopy()
        {
            SkillsMasterClass other = (SkillsMasterClass)this.MemberwiseClone();
            return other;
        }
    }
}
