using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RunBattle : MonoBehaviour
{


    public BattleEngine Battle;

    // environment setting
    public SkillsMasterClass normalAttackSkillsMaster;
    public List<SkillsMasterClass> buffMasters;

    // Input data, Units list
    public List<UnitClass> allyUnitList;
    public List<UnitClass> enemyUnitList;

    private CalculateUnitStatus calculateUnitStatus;

    private List<EffectClass> allySkillsList;
    private List<EffectClass> enemySkillsList;
    private List<BattleUnit> allyBattleUnits;
    private List<BattleUnit> enemyBattleUnits;


    public RunBattle(List<UnitClass> allyUnitList, List<UnitClass> enemyUnitList, SkillsMasterClass normalAttackSkillsMaster, List<SkillsMasterClass> buffMasters)
    {
        this.allyUnitList = allyUnitList;
        this.enemyUnitList = enemyUnitList;
        this.normalAttackSkillsMaster = normalAttackSkillsMaster;
        this.buffMasters = buffMasters;

    }


    public void Run()
    {
        Debug.Log("RunBattle.Run() has been activated!");
        allyBattleUnits = new List<BattleUnit>();
        Battle = new BattleEngine();

        Battle.SetUpEnvironment(normalAttackSkillMaster: normalAttackSkillsMaster, buffMasters: buffMasters);

        (allyBattleUnits, allySkillsList) = SetUpBattleUnitFromUnit(allyUnitList);
        Battle.SetAllyBattleUnits(allyBattleUnits, allySkillsList);

        enemyBattleUnits = new List<BattleUnit>();
        (enemyBattleUnits, enemySkillsList) = SetUpBattleUnitFromUnit(enemyUnitList);
        Battle.SetEnemyBattleUnits(enemyBattleUnits, enemySkillsList);

        Battle.Battle();

    }

    public (List<BattleUnit> BattleUnits, List<EffectClass> SkillsList) SetUpBattleUnitFromUnit(List<UnitClass> UnitList)
    {
        List<BattleUnit> BattleUnits = new List<BattleUnit>();
        List<EffectClass> SkillsList = new List<EffectClass>();
        foreach (UnitClass unit in UnitList)
        {
            CalculateCombatStatusFromUnit(unit);
            BattleUnits.Add(calculateUnitStatus.BattleUnit);


            EffectClass effectClass;
            foreach (SkillsMasterClass skillMaster in unit.skillsMaster)
            {
                // Skills offenseEffectMagnification calculation
                double magnificationRate = 1.0;
                switch (skillMaster.ActionType)
                {
                    case ActionType.Counter: magnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.OffenseEffectPower.Counter; break;
                    case ActionType.Chain: magnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.OffenseEffectPower.Chain; break;
                    case ActionType.ReAttack: magnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.OffenseEffectPower.ReAttack; break;
                    case ActionType.Move: magnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.OffenseEffectPower.Move; break;
                    case ActionType.Interrupt: magnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.OffenseEffectPower.Interrupt; break;
                    case ActionType.AtBeginning: magnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.OffenseEffectPower.AtBeginning; break;
                    case ActionType.AtEnding: magnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.OffenseEffectPower.AtEnding; break;
                    default: break;
                }

                // Skills Possibility Rate calculation
                double abilityValue = 0.0;
                double possibilityMagnificationRate = 1.0;
                switch (skillMaster.Ability)
                {
                    case Ability.power: abilityValue = (double)calculateUnitStatus.BattleUnit.Ability.Power; break;
                    case Ability.generation: abilityValue = (double)calculateUnitStatus.BattleUnit.Ability.Generation; break;
                    case Ability.responsiveness: abilityValue = (double)calculateUnitStatus.BattleUnit.Ability.Responsiveness; break;
                    case Ability.intelligence: abilityValue = (double)calculateUnitStatus.BattleUnit.Ability.Intelligence; break;
                    case Ability.precision: abilityValue = (double)calculateUnitStatus.BattleUnit.Ability.Precision; break;
                    case Ability.luck: abilityValue = (double)calculateUnitStatus.BattleUnit.Ability.Luck; break;
                    case Ability.none: abilityValue = 0.0; break;
                    default: break;
                }

                switch (skillMaster.ActionType)
                {
                    case ActionType.Counter: possibilityMagnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.TriggerPossibility.Counter; break;
                    case ActionType.Chain: possibilityMagnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.TriggerPossibility.Chain; break;
                    case ActionType.ReAttack: possibilityMagnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.TriggerPossibility.ReAttack; break;
                    case ActionType.Move: possibilityMagnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.TriggerPossibility.Move; break;
                    case ActionType.Interrupt: possibilityMagnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.TriggerPossibility.Interrupt; break;
                    case ActionType.AtBeginning: possibilityMagnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.TriggerPossibility.AtBeginning; break;
                    case ActionType.AtEnding: possibilityMagnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.TriggerPossibility.AtEnding; break;
                    default: break;
                }

                double triggeredPossibilityValue = Math.Pow(((skillMaster.TriggerBase.PossibilityBaseRate + abilityValue /
                         (100.0 + abilityValue * 2 * skillMaster.TriggerBase.PossibilityWeight)) * 100), 3.0) / 40000 * magnificationRate;
                if (triggeredPossibilityValue > 1.0) { triggeredPossibilityValue = 1.0; }

                // damage control assist able check
                bool isDamageControlAssitAble = false;
                switch (unit.CoreFrame.TuningStype)
                {
                    case TuningStype.medic: isDamageControlAssitAble = true; break;
                    default: break;
                }

                effectClass = new EffectClass(character: calculateUnitStatus.BattleUnit, skill: skillMaster, actionType: skillMaster.ActionType,
                     offenseEffectMagnification: magnificationRate, triggeredPossibility: triggeredPossibilityValue, isDamageControlAssistAble: isDamageControlAssitAble,
                     usageCount: skillMaster.UsageCount, veiledFromTurn: 1, veiledToTurn: skillMaster.VeiledTurn);

                SkillsList.Add(effectClass);

            }
        }

        return (BattleUnits, SkillsList);
    }

    private void CalculateCombatStatusFromUnit(UnitClass inputUnit)
    {
        //sample implement 2019.8.6 should be deleted after this test.
        {
            calculateUnitStatus = new CalculateUnitStatus(inputUnit);
        }
        //end sample implement 2019.8.6

    }


}
