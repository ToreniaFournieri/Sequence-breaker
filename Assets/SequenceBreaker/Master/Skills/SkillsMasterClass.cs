using SequenceBreaker.Environment;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker.Master.Skills
{
    [CreateAssetMenu(fileName = "SkillMaster-", menuName = "Skill/SkillsMaster", order = 2)]
    public sealed class SkillsMasterClass : ScriptableObject
    {
        [SerializeField] public string skillName;
        [SerializeField] public int skillId;
        [SerializeField] public bool isNormalAttack;
 
        //[FormerlySerializedAs("Name")] [SerializeField] public SkillName skillName;
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
        [SerializeField] public SkillsMasterClass callingBuffName;

        //[FormerlySerializedAs("CallingBuffName")] [SerializeField] public SkillName callingBuffName;
        [FormerlySerializedAs("DebuffTarget")] [SerializeField] public DebuffTargetParameterClass debuffTarget;


        public SkillsMasterClass DeepCopy()
        {
            SkillsMasterClass other = (SkillsMasterClass)MemberwiseClone();
            return other;
        }
    }
}
