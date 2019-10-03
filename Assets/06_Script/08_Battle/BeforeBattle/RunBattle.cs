using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RunBattle : MonoBehaviour
{
    // Mission infomation
    public string missionText;
    public string location;
    public int missionLevel;
    public WhichWin whichWin;
    public string winRatio;

    //Output information
    //public List<KohmaiWorks.Scroller.Data> Data;
    public List<List<KohmaiWorks.Scroller.Data>> DataList;

    // environment setting
    public BattleEnvironment battleEnvironment;

    // Input data, Units list
    public List<UnitClass> allyUnitList;
    public List<UnitClass> enemyUnitList;

    public List<EnemyUnitSet> enemyUnitSetList;


    //Internal use
    private CalculateUnitStatus calculateUnitStatus;

    private List<EffectClass> allySkillsList;
    private List<EffectClass> enemySkillsList;
    private List<BattleUnit> allyBattleUnits;
    private List<BattleUnit> enemyBattleUnits;

    private List<BattleEngine> _battleList;
    private List<KohmaiWorks.Scroller.Data> _data;

    public RunBattle Copy()
    {
        return (RunBattle)this.MemberwiseClone();

    }

    public void Set(RunBattle runBattle)
    {
        RunBattle _runbattle = runBattle.Copy();

        this.missionText = _runbattle.missionText;
        this.location = _runbattle.location;
        this.missionLevel = _runbattle.missionLevel;
        this.whichWin = _runbattle.whichWin;
        this.DataList = _runbattle.DataList;
        this.battleEnvironment = _runbattle.battleEnvironment;
        this.allyUnitList = _runbattle.allyUnitList;
        this.enemyUnitList = _runbattle.enemyUnitList;
        this.enemyUnitSetList = _runbattle.enemyUnitSetList;
        this.winRatio = _runbattle.winRatio;

    }


    public RunBattle(List<UnitClass> _allyUnitList, List<EnemyUnitSet> _enemyUnitSetList, BattleEnvironment _battleEnvironment)
    {
        this.allyUnitList = _allyUnitList;
        //this.enemyUnitList = _enemyUnitList; // this should not be used 
        this.enemyUnitSetList = _enemyUnitSetList;
        this.battleEnvironment = _battleEnvironment;
    }

    public void Run(int enemyLevel)
    {
        allyBattleUnits = new List<BattleUnit>();
        _battleList = new List<BattleEngine>();
        DataList = new List<List<KohmaiWorks.Scroller.Data>>();

        (allyBattleUnits, allySkillsList) = SetUpBattleUnitFromUnit(allyUnitList);

        int _wave = 0;
        foreach (EnemyUnitSet _enemyUnitSet in enemyUnitSetList)
        {
            _battleList.Add(new BattleEngine());
            _battleList[_wave].SetUpEnvironment(normalAttackSkillMaster: battleEnvironment.normalAttackSkillsMaster, buffMasters: battleEnvironment.buffMasters);
            _battleList[_wave].SetAllyBattleUnits(allyBattleUnits, allySkillsList);

            //adjust enemy level
            foreach (UnitClass unit in _enemyUnitSet.enemyUnitList)
            {
                unit.Level = enemyLevel;

            }
            enemyBattleUnits = new List<BattleUnit>();
            (enemyBattleUnits, enemySkillsList) = SetUpBattleUnitFromUnit(_enemyUnitSet.enemyUnitList);

            //set name with levels
            foreach (BattleUnit battleUnit in enemyBattleUnits)
            {
                battleUnit.Name = battleUnit.Name + " (Lv:" + enemyLevel + ")";
                missionLevel = enemyLevel;
            }

            _battleList[_wave].SetEnemyBattleUnits(enemyBattleUnits, enemySkillsList);

            _battleList[_wave].Battle();

            whichWin = _battleList[_wave].whichWin;
            winRatio = _battleList[_wave].winRatio;

            DataList.Add(new List<KohmaiWorks.Scroller.Data>());
            SetBattleLogToData(_wave);

            _wave += 1;
        }

    }


    private void SetBattleLogToData(int _wave)
    {
        BattleEngine _battle = _battleList[_wave];

        _data = new List<KohmaiWorks.Scroller.Data>();

        //populate the scroller with some text
        for (var i = 0; i < _battle.logList.Count; i++)
        {
            // _data set
            string unitNameText = null;
            string unitHealthText = null;
            string reactText = null;
            float shieldRatio = 0f;
            float hPRatio = 0f;
            int barrierRemains = 0;
            bool isDead = true;
            if (_battle.logList[i].Order != null)
            {
                unitNameText = _battle.logList[i].Order.Actor.Name;
                unitHealthText = "[" + _battle.logList[i].Order.Actor.Combat.ShieldCurrent +
                    "(" + Mathf.Ceil((float)_battle.logList[i].Order.Actor.Combat.ShieldCurrent * 100 / (float)_battle.logList[i].Order.Actor.Combat.ShieldMax) + "%)+"
                + _battle.logList[i].Order.Actor.Combat.HitPointCurrent + "("
                + Mathf.Ceil((float)_battle.logList[i].Order.Actor.Combat.HitPointCurrent * 100 / (float)_battle.logList[i].Order.Actor.Combat.HitPointMax)
                 + "%)]";

                if (_battle.logList[i].Order.IndividualTarget != null)
                {
                    string preposition = " to ";
                    if (_battle.logList[i].Order.ActionType == ActionType.ReAttack) { preposition = " of "; }
                    //else if (_battle.logList[i].Order.ActionType == ActionType.) { preposition = " for "; }

                    reactText = _battle.logList[i].Order.ActionType.ToString() + preposition + _battle.logList[i].Order.IndividualTarget.Name;
                }

                shieldRatio = (float)_battle.logList[i].Order.Actor.Combat.ShieldCurrent / (float)_battle.logList[i].Order.Actor.Combat.ShieldMax;
                hPRatio = (float)_battle.logList[i].Order.Actor.Combat.HitPointCurrent / (float)_battle.logList[i].Order.Actor.Combat.HitPointMax;

                barrierRemains = _battle.logList[i].Order.Actor.Buff.BarrierRemaining;

                if (_battle.logList[i].Order.Actor.Combat.HitPointCurrent > 0)
                {
                    isDead = false;
                }

            }

            List<BattleUnit> characters = null;
            if (_battle.logList[i].Characters != null)
            {
                characters = _battle.logList[i].Characters;
            }



            _data.Add(new KohmaiWorks.Scroller.Data()
            {
                index = i,
                turn = _battle.logList[i].OrderCondition.Turn,
                cellSize = 1300, // this is dummy, need to calculate later
                reactText = reactText,
                unitInfo = "<b>" + unitNameText + "</b>  " + unitHealthText,
                firstLine = _battle.logList[i].FirstLine,
                mainText = _battle.logList[i].Log,
                affiliation = _battle.logList[i].WhichAffiliationAct,
                nestLevel = _battle.logList[i].OrderCondition.Nest,
                isDead = isDead,
                barrierRemains = barrierRemains,
                shieldRatio = shieldRatio,
                hPRatio = hPRatio,
                isHeaderInfo = _battle.logList[i].IsHeaderInfo,
                headerText = _battle.logList[i].HeaderInfoText,
                characters = characters
            });

            // set all of data to data (activate)
            DataList[_wave]= _data;
        }
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
