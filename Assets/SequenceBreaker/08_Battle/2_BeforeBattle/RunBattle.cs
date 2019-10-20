using System;
using System.Collections.Generic;
using KohmaiWorks.Scroller;
using SequenceBreaker._08_Battle;
using UnityEngine;
using UnityEngine.Serialization;

sealed public class RunBattle : MonoBehaviour
{
    // Mission infomation
    public string missionText;
    public string location;
    public int missionLevel;
    public WhichWin whichWin;
    public List<WhichWin> whichWinEachWaves;

    //Output information
    [FormerlySerializedAs("DataList")] public List<List<Data>> dataList;

    // environment setting
    public BattleEnvironment battleEnvironment;

    // Input data, Units list
    public List<EnemyUnitSet> enemyUnitSetList;

    // to get current Ally Battle unit
    public List<BattleUnit> currentAllyUnitList;


    //Internal use
    private CalculateUnitStatus _calculateUnitStatus;

    private List<EffectClass> _allySkillsList;
    private List<EffectClass> _enemySkillsList;
    private List<BattleUnit> _allyBattleUnits;
    private List<BattleUnit> _enemyBattleUnits;

    private List<BattleEngine> _battleList;
    private List<Data> _data;

    public RunBattle Copy()
    {
        var deepCopyRunbattle = new RunBattle();
        deepCopyRunbattle.enemyUnitSetList = enemyUnitSetList;
        deepCopyRunbattle.battleEnvironment = battleEnvironment;
        deepCopyRunbattle.missionText = missionText;
        deepCopyRunbattle.location = location;
        deepCopyRunbattle.whichWin = whichWin;
        deepCopyRunbattle.dataList = dataList;
        deepCopyRunbattle.currentAllyUnitList = currentAllyUnitList;
        deepCopyRunbattle.whichWinEachWaves = whichWinEachWaves;

        return deepCopyRunbattle;

    }

    public RunBattle Copy(int wave)
    {
        var deepCopyRunbattle = new RunBattle();
        deepCopyRunbattle.enemyUnitSetList = enemyUnitSetList;
        deepCopyRunbattle.battleEnvironment = battleEnvironment;
        deepCopyRunbattle.missionText = missionText;
        deepCopyRunbattle.location = location;
        deepCopyRunbattle.whichWin = whichWinEachWaves[wave];
        deepCopyRunbattle.dataList = new List<List<Data>>();
        deepCopyRunbattle.dataList.Add(dataList[wave]);
        deepCopyRunbattle.currentAllyUnitList = currentAllyUnitList;
        deepCopyRunbattle.whichWinEachWaves = whichWinEachWaves;


        return deepCopyRunbattle;

    }




    public void Set(RunBattle runBattle)
    {
        var runbattle = runBattle.Copy();

        missionText = runbattle.missionText;
        location = runbattle.location;
        missionLevel = runbattle.missionLevel;
        whichWin = runbattle.whichWin;
        dataList = runbattle.dataList;
        battleEnvironment = runbattle.battleEnvironment;
        enemyUnitSetList = runbattle.enemyUnitSetList;
        whichWinEachWaves = runbattle.whichWinEachWaves;


    }

    public void Run(int enemyLevel, List<UnitClass> allyUnitList)
    {
        _allyBattleUnits = new List<BattleUnit>();
        _battleList = new List<BattleEngine>();
        whichWinEachWaves = new List<WhichWin>();
        dataList = new List<List<Data>>();

        (_allyBattleUnits, _allySkillsList) = SetUpBattleUnitFromUnit(allyUnitList);

        currentAllyUnitList = new List<BattleUnit>();
        currentAllyUnitList = _allyBattleUnits;

        var wave = 0;
        foreach (var enemyUnitSet in enemyUnitSetList )
        {


            _battleList.Add(new BattleEngine());
            _battleList[wave].SetUpEnvironment(normalAttackSkillMaster: battleEnvironment.normalAttackSkillsMaster, buffMasters: battleEnvironment.buffMasters);

            foreach (var unit in currentAllyUnitList)
            {
                var allyBattleUnit = _allyBattleUnits.Find(obj => obj.uniqueId == unit.uniqueId);
                allyBattleUnit.combat.shieldCurrent = unit.combat.shieldCurrent;
                allyBattleUnit.combat.hitPointCurrent = unit.combat.hitPointCurrent;

            }

            var isFirstWave = false;
            if(wave == 0) { isFirstWave = true; }
            _battleList[wave].SetAllyBattleUnits(_allyBattleUnits, _allySkillsList, isFirstWave);

            //adjust enemy level
            foreach (var unit in enemyUnitSet.enemyUnitList)
            {
                unit.level = enemyLevel;

            }
            _enemyBattleUnits = new List<BattleUnit>();
            (_enemyBattleUnits, _enemySkillsList) = SetUpBattleUnitFromUnit(enemyUnitSet.enemyUnitList);

            //set name with levels
            foreach (var battleUnit in _enemyBattleUnits)
            {
                battleUnit.name = battleUnit.name + " (Lv:" + enemyLevel + ")";
                missionLevel = enemyLevel;
            }

            _battleList[wave].SetEnemyBattleUnits(_enemyBattleUnits, _enemySkillsList);


            //[Battle start!!]
            _battleList[wave].Battle();

            whichWin = _battleList[wave].WhichWin;
            whichWinEachWaves.Add(_battleList[wave].WhichWin);

            currentAllyUnitList = _battleList[wave].AllyBattleUnitsList;

            dataList.Add(new List<Data>());
            SetBattleLogToData(wave);

            if (currentAllyUnitList.Find(unit => unit.combat.hitPointCurrent > 0) == null)
            {
                // all ally units has been slain.
                break;
            }

            wave += 1;
        }

    }


    private void SetBattleLogToData(int wave)
    {
        var battle = _battleList[wave];

        _data = new List<Data>();

        //populate the scroller with some text
        for (var i = 0; i < battle.LogList.Count; i++)
        {
            // _data set
            string unitNameText = null;
            string unitHealthText = null;
            string reactText = null;
            var shieldRatio = 0f;
            var hPRatio = 0f;
            var barrierRemains = 0;
            var isDead = true;
            if (battle.LogList[i].Order != null)
            {
                unitNameText = battle.LogList[i].Order.Actor.name;
                unitHealthText = "[" + battle.LogList[i].Order.Actor.combat.shieldCurrent +
                    "(" + Mathf.Ceil((float)battle.LogList[i].Order.Actor.combat.shieldCurrent * 100 / battle.LogList[i].Order.Actor.combat.shieldMax) + "%)+"
                + battle.LogList[i].Order.Actor.combat.hitPointCurrent + "("
                + Mathf.Ceil((float)battle.LogList[i].Order.Actor.combat.hitPointCurrent * 100 / battle.LogList[i].Order.Actor.combat.hitPointMax)
                 + "%)]";

                if (battle.LogList[i].Order.IndividualTarget != null)
                {
                    var preposition = " to ";
                    if (battle.LogList[i].Order.ActionType == ActionType.ReAttack) { preposition = " of "; }

                    reactText = battle.LogList[i].Order.ActionType + preposition + battle.LogList[i].Order.IndividualTarget.name;
                }

                shieldRatio = battle.LogList[i].Order.Actor.combat.shieldCurrent / (float)battle.LogList[i].Order.Actor.combat.shieldMax;
                hPRatio = battle.LogList[i].Order.Actor.combat.hitPointCurrent / (float)battle.LogList[i].Order.Actor.combat.hitPointMax;

                barrierRemains = battle.LogList[i].Order.Actor.buff.BarrierRemaining;

                if (battle.LogList[i].Order.Actor.combat.hitPointCurrent > 0)
                {
                    isDead = false;
                }

            }

            List<BattleUnit> characters = null;
            if (battle.LogList[i].characters != null)
            {
                characters = battle.LogList[i].characters;
            }



            _data.Add(new Data
            {
                index = i,
                turn = battle.LogList[i].OrderCondition.Turn,
                cellSize = 1300, // this is dummy, need to calculate later
                reactText = reactText,
                unitInfo = "<b>" + unitNameText + "</b>  " + unitHealthText,
                firstLine = battle.LogList[i].FirstLine,
                mainText = battle.LogList[i].Log,
                affiliation = battle.LogList[i].WhichAffiliationAct,
                nestLevel = battle.LogList[i].OrderCondition.Nest,
                isDead = isDead,
                barrierRemains = barrierRemains,
                shieldRatio = shieldRatio,
                hPRatio = hPRatio,
                isHeaderInfo = battle.LogList[i].isHeaderInfo,
                headerText = battle.LogList[i].headerInfoText,
                characters = characters
            });

            // set all of data to data (activate)
            dataList[wave]= _data;
        }
    }


    public (List<BattleUnit> BattleUnits, List<EffectClass> SkillsList) SetUpBattleUnitFromUnit(List<UnitClass> unitList)
    {
        var battleUnits = new List<BattleUnit>();
        var skillsList = new List<EffectClass>();
        foreach (var unit in unitList)
        {
            CalculateCombatStatusFromUnit(unit);
            battleUnits.Add(_calculateUnitStatus.battleUnit);


            EffectClass effectClass;
            foreach (var skillMaster in unit.skillsMaster)
            {
                // Skills offenseEffectMagnification calculation
                var magnificationRate = 1.0;
                switch (skillMaster.actionType)
                {
                    case ActionType.Counter: magnificationRate = _calculateUnitStatus.battleUnit.skillMagnification.offenseEffectPower.counter; break;
                    case ActionType.Chain: magnificationRate = _calculateUnitStatus.battleUnit.skillMagnification.offenseEffectPower.chain; break;
                    case ActionType.ReAttack: magnificationRate = _calculateUnitStatus.battleUnit.skillMagnification.offenseEffectPower.reAttack; break;
                    case ActionType.Move: magnificationRate = _calculateUnitStatus.battleUnit.skillMagnification.offenseEffectPower.move; break;
                    case ActionType.Interrupt: magnificationRate = _calculateUnitStatus.battleUnit.skillMagnification.offenseEffectPower.interrupt; break;
                    case ActionType.AtBeginning: magnificationRate = _calculateUnitStatus.battleUnit.skillMagnification.offenseEffectPower.atBeginning; break;
                    case ActionType.AtEnding: magnificationRate = _calculateUnitStatus.battleUnit.skillMagnification.offenseEffectPower.atEnding; break;
                }

                // Skills Possibility Rate calculation
                var abilityValue = 0.0;
                var possibilityMagnificationRate = 1.0;
                switch (skillMaster.ability)
                {
                    case Ability.Power: abilityValue = _calculateUnitStatus.battleUnit.ability.power; break;
                    case Ability.Generation: abilityValue = _calculateUnitStatus.battleUnit.ability.generation; break;
                    case Ability.Responsiveness: abilityValue = _calculateUnitStatus.battleUnit.ability.responsiveness; break;
                    case Ability.Intelligence: abilityValue = _calculateUnitStatus.battleUnit.ability.intelligence; break;
                    case Ability.Precision: abilityValue = _calculateUnitStatus.battleUnit.ability.precision; break;
                    case Ability.Luck: abilityValue = _calculateUnitStatus.battleUnit.ability.luck; break;
                    case Ability.None: abilityValue = 0.0; break;
                }

                switch (skillMaster.actionType)
                {
                    case ActionType.Counter: possibilityMagnificationRate = _calculateUnitStatus.battleUnit.skillMagnification.triggerPossibility.counter; break;
                    case ActionType.Chain: possibilityMagnificationRate = _calculateUnitStatus.battleUnit.skillMagnification.triggerPossibility.chain; break;
                    case ActionType.ReAttack: possibilityMagnificationRate = _calculateUnitStatus.battleUnit.skillMagnification.triggerPossibility.reAttack; break;
                    case ActionType.Move: possibilityMagnificationRate = _calculateUnitStatus.battleUnit.skillMagnification.triggerPossibility.move; break;
                    case ActionType.Interrupt: possibilityMagnificationRate = _calculateUnitStatus.battleUnit.skillMagnification.triggerPossibility.interrupt; break;
                    case ActionType.AtBeginning: possibilityMagnificationRate = _calculateUnitStatus.battleUnit.skillMagnification.triggerPossibility.atBeginning; break;
                    case ActionType.AtEnding: possibilityMagnificationRate = _calculateUnitStatus.battleUnit.skillMagnification.triggerPossibility.atEnding; break;
                }

                var triggeredPossibilityValue = Math.Pow(((skillMaster.triggerBase.possibilityBaseRate + abilityValue /
                         (100.0 + abilityValue * 2 * skillMaster.triggerBase.possibilityWeight)) * 100), 3.0) / 40000 * magnificationRate;
                if (triggeredPossibilityValue > 1.0) { triggeredPossibilityValue = 1.0; }

                // damage control assist able check
                var isDamageControlAssitAble = false;
                switch (unit.coreFrame.tuningStype)
                {
                    case TuningStype.Medic: isDamageControlAssitAble = true; break;
                }

                effectClass = new EffectClass(character: _calculateUnitStatus.battleUnit, skill: skillMaster, actionType: skillMaster.actionType,
                     offenseEffectMagnification: magnificationRate, triggeredPossibility: triggeredPossibilityValue, isDamageControlAssistAble: isDamageControlAssitAble,
                     usageCount: skillMaster.usageCount, veiledFromTurn: 1, veiledToTurn: skillMaster.veiledTurn);

                skillsList.Add(effectClass);

            }
        }

        return (battleUnits, skillsList);
    }

    private void CalculateCombatStatusFromUnit(UnitClass inputUnit)
    {
        //sample implement 2019.8.6 should be deleted after this test.
        {
            _calculateUnitStatus = new CalculateUnitStatus(inputUnit);
        }
        //end sample implement 2019.8.6

    }


}
