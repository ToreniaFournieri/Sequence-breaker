using System;
using System.Collections.Generic;
using KohmaiWorks.Scroller;
using SequenceBreaker._08_Battle;
using UnityEngine;

sealed public class RunBattle : MonoBehaviour
{
    // Mission infomation
    public string missionText;
    public string location;
    public int missionLevel;
    public WhichWin whichWin;
    public List<WhichWin> whichWinEachWaves;

    //Output information
    public List<List<Data>> DataList;

    // environment setting
    public BattleEnvironment battleEnvironment;

    // Input data, Units list
    public List<EnemyUnitSet> enemyUnitSetList;

    // to get current Ally Battle unit
    public List<BattleUnit> currentAllyUnitList;


    //Internal use
    private CalculateUnitStatus calculateUnitStatus;

    private List<EffectClass> allySkillsList;
    private List<EffectClass> enemySkillsList;
    private List<BattleUnit> allyBattleUnits;
    private List<BattleUnit> enemyBattleUnits;

    private List<BattleEngine> _battleList;
    private List<Data> _data;

    public RunBattle Copy()
    {
        var _deepCopyRunbattle = new RunBattle();
        _deepCopyRunbattle.enemyUnitSetList = enemyUnitSetList;
        _deepCopyRunbattle.battleEnvironment = battleEnvironment;
        _deepCopyRunbattle.missionText = missionText;
        _deepCopyRunbattle.location = location;
        _deepCopyRunbattle.whichWin = whichWin;
        _deepCopyRunbattle.DataList = DataList;
        _deepCopyRunbattle.currentAllyUnitList = currentAllyUnitList;
        _deepCopyRunbattle.whichWinEachWaves = whichWinEachWaves;

        return _deepCopyRunbattle;

    }

    public RunBattle Copy(int wave)
    {
        var _deepCopyRunbattle = new RunBattle();
        _deepCopyRunbattle.enemyUnitSetList = enemyUnitSetList;
        _deepCopyRunbattle.battleEnvironment = battleEnvironment;
        _deepCopyRunbattle.missionText = missionText;
        _deepCopyRunbattle.location = location;
        _deepCopyRunbattle.whichWin = whichWinEachWaves[wave];
        _deepCopyRunbattle.DataList = new List<List<Data>>();
        _deepCopyRunbattle.DataList.Add(DataList[wave]);
        _deepCopyRunbattle.currentAllyUnitList = currentAllyUnitList;
        _deepCopyRunbattle.whichWinEachWaves = whichWinEachWaves;


        return _deepCopyRunbattle;

    }




    public void Set(RunBattle runBattle)
    {
        var _runbattle = runBattle.Copy();

        missionText = _runbattle.missionText;
        location = _runbattle.location;
        missionLevel = _runbattle.missionLevel;
        whichWin = _runbattle.whichWin;
        DataList = _runbattle.DataList;
        battleEnvironment = _runbattle.battleEnvironment;
        enemyUnitSetList = _runbattle.enemyUnitSetList;
        whichWinEachWaves = _runbattle.whichWinEachWaves;


    }

    public void Run(int enemyLevel, List<UnitClass> allyUnitList)
    {
        allyBattleUnits = new List<BattleUnit>();
        _battleList = new List<BattleEngine>();
        whichWinEachWaves = new List<WhichWin>();
        DataList = new List<List<Data>>();

        (allyBattleUnits, allySkillsList) = SetUpBattleUnitFromUnit(allyUnitList);

        currentAllyUnitList = new List<BattleUnit>();
        currentAllyUnitList = allyBattleUnits;

        var _wave = 0;
        foreach (var _enemyUnitSet in enemyUnitSetList )
        {


            _battleList.Add(new BattleEngine());
            _battleList[_wave].SetUpEnvironment(normalAttackSkillMaster: battleEnvironment.normalAttackSkillsMaster, buffMasters: battleEnvironment.buffMasters);

            foreach (var _unit in currentAllyUnitList)
            {
                var allyBattleUnit = allyBattleUnits.Find(obj => obj.UniqueID == _unit.UniqueID);
                allyBattleUnit.Combat.ShieldCurrent = _unit.Combat.ShieldCurrent;
                allyBattleUnit.Combat.HitPointCurrent = _unit.Combat.HitPointCurrent;

            }

            var isFirstWave = false;
            if(_wave == 0) { isFirstWave = true; }
            _battleList[_wave].SetAllyBattleUnits(allyBattleUnits, allySkillsList, isFirstWave);

            //adjust enemy level
            foreach (var unit in _enemyUnitSet.enemyUnitList)
            {
                unit.Level = enemyLevel;

            }
            enemyBattleUnits = new List<BattleUnit>();
            (enemyBattleUnits, enemySkillsList) = SetUpBattleUnitFromUnit(_enemyUnitSet.enemyUnitList);

            //set name with levels
            foreach (var battleUnit in enemyBattleUnits)
            {
                battleUnit.Name = battleUnit.Name + " (Lv:" + enemyLevel + ")";
                missionLevel = enemyLevel;
            }

            _battleList[_wave].SetEnemyBattleUnits(enemyBattleUnits, enemySkillsList);


            //[Battle start!!]
            _battleList[_wave].Battle();

            whichWin = _battleList[_wave].WhichWin;
            whichWinEachWaves.Add(_battleList[_wave].WhichWin);

            currentAllyUnitList = _battleList[_wave].AllyBattleUnitsList;

            DataList.Add(new List<Data>());
            SetBattleLogToData(_wave);

            if (currentAllyUnitList.Find(unit => unit.Combat.HitPointCurrent > 0) == null)
            {
                // all ally units has been slain.
                break;
            }

            _wave += 1;
        }

    }


    private void SetBattleLogToData(int _wave)
    {
        var _battle = _battleList[_wave];

        _data = new List<Data>();

        //populate the scroller with some text
        for (var i = 0; i < _battle.LogList.Count; i++)
        {
            // _data set
            string unitNameText = null;
            string unitHealthText = null;
            string reactText = null;
            var shieldRatio = 0f;
            var hPRatio = 0f;
            var barrierRemains = 0;
            var isDead = true;
            if (_battle.LogList[i].Order != null)
            {
                unitNameText = _battle.LogList[i].Order.Actor.Name;
                unitHealthText = "[" + _battle.LogList[i].Order.Actor.Combat.ShieldCurrent +
                    "(" + Mathf.Ceil((float)_battle.LogList[i].Order.Actor.Combat.ShieldCurrent * 100 / _battle.LogList[i].Order.Actor.Combat.ShieldMax) + "%)+"
                + _battle.LogList[i].Order.Actor.Combat.HitPointCurrent + "("
                + Mathf.Ceil((float)_battle.LogList[i].Order.Actor.Combat.HitPointCurrent * 100 / _battle.LogList[i].Order.Actor.Combat.HitPointMax)
                 + "%)]";

                if (_battle.LogList[i].Order.IndividualTarget != null)
                {
                    var preposition = " to ";
                    if (_battle.LogList[i].Order.ActionType == ActionType.ReAttack) { preposition = " of "; }

                    reactText = _battle.LogList[i].Order.ActionType + preposition + _battle.LogList[i].Order.IndividualTarget.Name;
                }

                shieldRatio = _battle.LogList[i].Order.Actor.Combat.ShieldCurrent / (float)_battle.LogList[i].Order.Actor.Combat.ShieldMax;
                hPRatio = _battle.LogList[i].Order.Actor.Combat.HitPointCurrent / (float)_battle.LogList[i].Order.Actor.Combat.HitPointMax;

                barrierRemains = _battle.LogList[i].Order.Actor.Buff.BarrierRemaining;

                if (_battle.LogList[i].Order.Actor.Combat.HitPointCurrent > 0)
                {
                    isDead = false;
                }

            }

            List<BattleUnit> characters = null;
            if (_battle.LogList[i].characters != null)
            {
                characters = _battle.LogList[i].characters;
            }



            _data.Add(new Data
            {
                index = i,
                turn = _battle.LogList[i].OrderCondition.Turn,
                cellSize = 1300, // this is dummy, need to calculate later
                reactText = reactText,
                unitInfo = "<b>" + unitNameText + "</b>  " + unitHealthText,
                firstLine = _battle.LogList[i].FirstLine,
                mainText = _battle.LogList[i].Log,
                affiliation = _battle.LogList[i].WhichAffiliationAct,
                nestLevel = _battle.LogList[i].OrderCondition.Nest,
                isDead = isDead,
                barrierRemains = barrierRemains,
                shieldRatio = shieldRatio,
                hPRatio = hPRatio,
                isHeaderInfo = _battle.LogList[i].isHeaderInfo,
                headerText = _battle.LogList[i].headerInfoText,
                characters = characters
            });

            // set all of data to data (activate)
            DataList[_wave]= _data;
        }
    }


    public (List<BattleUnit> BattleUnits, List<EffectClass> SkillsList) SetUpBattleUnitFromUnit(List<UnitClass> UnitList)
    {
        var BattleUnits = new List<BattleUnit>();
        var SkillsList = new List<EffectClass>();
        foreach (var unit in UnitList)
        {
            CalculateCombatStatusFromUnit(unit);
            BattleUnits.Add(calculateUnitStatus.BattleUnit);


            EffectClass effectClass;
            foreach (var skillMaster in unit.skillsMaster)
            {
                // Skills offenseEffectMagnification calculation
                var magnificationRate = 1.0;
                switch (skillMaster.ActionType)
                {
                    case ActionType.Counter: magnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.OffenseEffectPower.Counter; break;
                    case ActionType.Chain: magnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.OffenseEffectPower.Chain; break;
                    case ActionType.ReAttack: magnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.OffenseEffectPower.ReAttack; break;
                    case ActionType.Move: magnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.OffenseEffectPower.Move; break;
                    case ActionType.Interrupt: magnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.OffenseEffectPower.Interrupt; break;
                    case ActionType.AtBeginning: magnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.OffenseEffectPower.AtBeginning; break;
                    case ActionType.AtEnding: magnificationRate = calculateUnitStatus.BattleUnit.SkillMagnification.OffenseEffectPower.AtEnding; break;
                }

                // Skills Possibility Rate calculation
                var abilityValue = 0.0;
                var possibilityMagnificationRate = 1.0;
                switch (skillMaster.Ability)
                {
                    case Ability.power: abilityValue = calculateUnitStatus.BattleUnit.Ability.Power; break;
                    case Ability.generation: abilityValue = calculateUnitStatus.BattleUnit.Ability.Generation; break;
                    case Ability.responsiveness: abilityValue = calculateUnitStatus.BattleUnit.Ability.Responsiveness; break;
                    case Ability.intelligence: abilityValue = calculateUnitStatus.BattleUnit.Ability.Intelligence; break;
                    case Ability.precision: abilityValue = calculateUnitStatus.BattleUnit.Ability.Precision; break;
                    case Ability.luck: abilityValue = calculateUnitStatus.BattleUnit.Ability.Luck; break;
                    case Ability.none: abilityValue = 0.0; break;
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
                }

                var triggeredPossibilityValue = Math.Pow(((skillMaster.TriggerBase.PossibilityBaseRate + abilityValue /
                         (100.0 + abilityValue * 2 * skillMaster.TriggerBase.PossibilityWeight)) * 100), 3.0) / 40000 * magnificationRate;
                if (triggeredPossibilityValue > 1.0) { triggeredPossibilityValue = 1.0; }

                // damage control assist able check
                var isDamageControlAssitAble = false;
                switch (unit.CoreFrame.TuningStype)
                {
                    case TuningStype.medic: isDamageControlAssitAble = true; break;
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
