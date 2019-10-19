using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillMagnificationClass-", menuName = "BattleUnit/SkillMagnificationClass", order = 24)]
sealed public class UnitSkillMagnificationClass : ScriptableObject
{
    [SerializeField] public ActionSkillClass OffenseEffectPower;
    [SerializeField] public ActionSkillClass TriggerPossibility;

    public UnitSkillMagnificationClass(ActionSkillClass offenseEffectPower, ActionSkillClass triggerPossibility)
    { this.OffenseEffectPower = offenseEffectPower; this.TriggerPossibility = triggerPossibility; }

}