using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SkillMagnificationClass-", menuName = "BattleUnit/SkillMagnificationClass", order = 24)]
sealed public class UnitSkillMagnificationClass : ScriptableObject
{
    [FormerlySerializedAs("OffenseEffectPower")] [SerializeField] public ActionSkillClass offenseEffectPower;
    [FormerlySerializedAs("TriggerPossibility")] [SerializeField] public ActionSkillClass triggerPossibility;

    public UnitSkillMagnificationClass(ActionSkillClass offenseEffectPower, ActionSkillClass triggerPossibility)
    { this.offenseEffectPower = offenseEffectPower; this.triggerPossibility = triggerPossibility; }

}