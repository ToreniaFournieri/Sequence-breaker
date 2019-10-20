using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._01_Data._08_BattleUnitSub
{
    [CreateAssetMenu(fileName = "SkillMagnificationClass-", menuName = "BattleUnit/SkillMagnificationClass", order = 24)]
    public sealed class UnitSkillMagnificationClass : ScriptableObject
    {
        [FormerlySerializedAs("OffenseEffectPower")] [SerializeField] public ActionSkillClass offenseEffectPower;
        [FormerlySerializedAs("TriggerPossibility")] [SerializeField] public ActionSkillClass triggerPossibility;

        public UnitSkillMagnificationClass(ActionSkillClass offenseEffectPower, ActionSkillClass triggerPossibility)
        { this.offenseEffectPower = offenseEffectPower; this.triggerPossibility = triggerPossibility; }

    }
}