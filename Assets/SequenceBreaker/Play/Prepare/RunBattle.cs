using System;
using System.Collections.Generic;
using SequenceBreaker.Environment;
using SequenceBreaker.Master.BattleUnit;
using SequenceBreaker.Master.Skills;
using SequenceBreaker.Master.UnitClass;
using SequenceBreaker.Play.Battle;
using SequenceBreaker.Timeline.BattleLogView;
using UnityEngine;

namespace SequenceBreaker.Play.Prepare
{
    public sealed class RunBattle : MonoBehaviour
    {
        // Mission information
        public string missionText;
        public string location;
        public int missionLevelInitial;
        
        
        // depend on the slider value
//        public int missionLevelCurrent;
        public WhichWin whichWin;
        public List<WhichWin> whichWinEachWaves;

        //Output information
        public List<List<Data>> dataList;

        // environment setting
        public BattleEnvironment battleEnvironment;

        // Input data, Units list
        public List<EnemyUnitSet> enemyUnitSetList;

        //unit List
        public UnitSet unitSet;


        // to get current Ally Battle unit
        public List<BattleUnit> currentAllyUnitList;

        
        public  CalculateUnitStatus calculateUnitStatus;

        //for SuperScroll use
        public int mId;
        public bool mChecked;
        public bool mIsExpand;
        
        //Internal use

        private List<EffectClass> _allySkillsList;
        private List<EffectClass> _enemySkillsList;
        private List<BattleUnit> _allyBattleUnits;
        private List<BattleUnit> _enemyBattleUnits;

        private List<BattleEngine> _battleList;
        private List<Data> _data;

        public RunBattle Copy()
        {
            var deepCopyRunBattle = new RunBattle();
            deepCopyRunBattle.enemyUnitSetList = enemyUnitSetList;
            deepCopyRunBattle.battleEnvironment = battleEnvironment;
            deepCopyRunBattle.missionText = missionText;
            deepCopyRunBattle.location = location;
            deepCopyRunBattle.whichWin = whichWin;
            deepCopyRunBattle.dataList = dataList;
            deepCopyRunBattle.currentAllyUnitList = currentAllyUnitList;
            deepCopyRunBattle.whichWinEachWaves = whichWinEachWaves;

            return deepCopyRunBattle;

        }

        public RunBattle Copy(int wave)
        {
            var deepCopyRunBattle = new RunBattle();
            deepCopyRunBattle.enemyUnitSetList = enemyUnitSetList;
            deepCopyRunBattle.battleEnvironment = battleEnvironment;
            deepCopyRunBattle.missionText = missionText;
            deepCopyRunBattle.location = location;
            deepCopyRunBattle.whichWin = whichWinEachWaves[wave];
            deepCopyRunBattle.dataList = new List<List<Data>>();
            deepCopyRunBattle.dataList.Add(dataList[wave]);
            deepCopyRunBattle.currentAllyUnitList = currentAllyUnitList;
            deepCopyRunBattle.whichWinEachWaves = whichWinEachWaves;


            return deepCopyRunBattle;

        }




        public void Set(RunBattle runBattle)
        {
            var runbattle = runBattle.Copy();

            missionText = runbattle.missionText;
            location = runbattle.location;
            missionLevelInitial = runbattle.missionLevelInitial;
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
            
            // set current enemy level
//            missionLevelCurrent = enemyLevel;

            

            (_allyBattleUnits, _allySkillsList) = SetUpBattleUnitFromUnit(allyUnitList);

            currentAllyUnitList = new List<BattleUnit>();
            currentAllyUnitList = _allyBattleUnits;

            var wave = 0;

            if (unitSet != null)
            {
                // new method 
                foreach (var unitWave in unitSet.unitSetList )
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
                        foreach (var unit in unitWave.unitWave)
                        {
                            unit.level = enemyLevel;
                        }

                    _enemyBattleUnits = new List<BattleUnit>();
                    (_enemyBattleUnits, _enemySkillsList) = SetUpBattleUnitFromUnit(unitWave.unitWave);

                    //set unitName with levels
                    foreach (var battleUnit in _enemyBattleUnits)
                    {
                        battleUnit.name = battleUnit.name ;
                        missionLevelInitial = enemyLevel;
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
            else
            {
                
                // previous system ; will be obsolete
                
            
            
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

                    //set unitName with levels
                    foreach (var battleUnit in _enemyBattleUnits)
                    {
                        battleUnit.name = battleUnit.name ;
                        missionLevelInitial = enemyLevel;
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
                    Index = i,
                    Turn = battle.LogList[i].OrderCondition.Turn,
                    CellSize = 1300, // this is dummy, need to calculate later
                    ReactText = reactText,
                    UnitInfo = "<b>" + unitNameText + "</b>  " + unitHealthText,
                    FirstLine = battle.LogList[i].FirstLine,
                    MainText = battle.LogList[i].Log,
                    Affiliation = battle.LogList[i].WhichAffiliationAct,
                    NestLevel = battle.LogList[i].OrderCondition.Nest,
                    IsDead = isDead,
                    BarrierRemains = barrierRemains,
                    ShieldRatio = shieldRatio,
                    HpRatio = hPRatio,
                    IsHeaderInfo = battle.LogList[i].isHeaderInfo,
                    HeaderText = battle.LogList[i].headerInfoText,
                    Characters = characters
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
                battleUnits.Add(calculateUnitStatus.battleUnit);

//                List<SkillsMasterClass> skillList = calculateUnitStatus.tuningStyleClass.GetSkills(unit.coreFrame.tuningStyle);
                List<SkillsMasterClass> skillList = calculateUnitStatus.summedSkillList;
                EffectClass effectClass;

                    foreach (var skillMaster in skillList)
                    {

                        // Skills offenseEffectMagnification calculation
                        var magnificationRate = 1.0;
                        switch (skillMaster.actionType)
                        {
                            case ActionType.Counter:
                                magnificationRate = calculateUnitStatus.battleUnit.skillMagnification.offenseEffectPower
                                    .counter;
                                break;
                            case ActionType.Chain:
                                magnificationRate = calculateUnitStatus.battleUnit.skillMagnification.offenseEffectPower
                                    .chain;
                                break;
                            case ActionType.ReAttack:
                                magnificationRate = calculateUnitStatus.battleUnit.skillMagnification.offenseEffectPower
                                    .reAttack;
                                break;
                            case ActionType.Move:
                                magnificationRate = calculateUnitStatus.battleUnit.skillMagnification.offenseEffectPower
                                    .move;
                                break;
                            case ActionType.Interrupt:
                                magnificationRate = calculateUnitStatus.battleUnit.skillMagnification.offenseEffectPower
                                    .interrupt;
                                break;
                            case ActionType.AtBeginning:
                                magnificationRate = calculateUnitStatus.battleUnit.skillMagnification.offenseEffectPower
                                    .atBeginning;
                                break;
                            case ActionType.AtEnding:
                                magnificationRate = calculateUnitStatus.battleUnit.skillMagnification.offenseEffectPower
                                    .atEnding;
                                break;
                        }

                        // Skills Possibility Rate calculation
                        var abilityValue = 0.0;
                        var possibilityMagnificationRate = 1.0;
                        switch (skillMaster.ability)
                        {
                            case Ability.Power:
                                abilityValue = calculateUnitStatus.battleUnit.ability.power;
                                break;
                            case Ability.Generation:
                                abilityValue = calculateUnitStatus.battleUnit.ability.generation;
                                break;
                            case Ability.Responsiveness:
                                abilityValue = calculateUnitStatus.battleUnit.ability.responsiveness;
                                break;
                            case Ability.Intelligence:
                                abilityValue = calculateUnitStatus.battleUnit.ability.intelligence;
                                break;
                            case Ability.Precision:
                                abilityValue = calculateUnitStatus.battleUnit.ability.precision;
                                break;
                            case Ability.Luck:
                                abilityValue = calculateUnitStatus.battleUnit.ability.luck;
                                break;
                            case Ability.None:
                                abilityValue = 0.0;
                                break;
                        }

                        switch (skillMaster.actionType)
                        {
                            case ActionType.Counter:
                                possibilityMagnificationRate = calculateUnitStatus.battleUnit.skillMagnification
                                    .triggerPossibility.counter;
                                break;
                            case ActionType.Chain:
                                possibilityMagnificationRate = calculateUnitStatus.battleUnit.skillMagnification
                                    .triggerPossibility.chain;
                                break;
                            case ActionType.ReAttack:
                                possibilityMagnificationRate = calculateUnitStatus.battleUnit.skillMagnification
                                    .triggerPossibility.reAttack;
                                break;
                            case ActionType.Move:
                                possibilityMagnificationRate = calculateUnitStatus.battleUnit.skillMagnification
                                    .triggerPossibility.move;
                                break;
                            case ActionType.Interrupt:
                                possibilityMagnificationRate = calculateUnitStatus.battleUnit.skillMagnification
                                    .triggerPossibility.interrupt;
                                break;
                            case ActionType.AtBeginning:
                                possibilityMagnificationRate = calculateUnitStatus.battleUnit.skillMagnification
                                    .triggerPossibility.atBeginning;
                                break;
                            case ActionType.AtEnding:
                                possibilityMagnificationRate = calculateUnitStatus.battleUnit.skillMagnification
                                    .triggerPossibility.atEnding;
                                break;
                        }

                        var triggeredPossibilityValue = Math.Pow(((skillMaster.triggerBase.possibilityBaseRate +
                                                                   abilityValue /
                                                                   (100.0 + abilityValue * 2 * skillMaster.triggerBase
                                                                        .possibilityWeight)) * 100), 3.0) / 40000 *
                                                        magnificationRate;
                        if (triggeredPossibilityValue > 1.0)
                        {
                            triggeredPossibilityValue = 1.0;
                        }

                        // damage control assist able check
                        var isDamageControlAssitAble = false;
                        switch (unit.coreFrame.tuningStyle)
                        {
                            case TuningStyle.Medic:
                                isDamageControlAssitAble = true;
                                break;
                        }

                        effectClass = new EffectClass(character: calculateUnitStatus.battleUnit, skill: skillMaster,
                            actionType: skillMaster.actionType,
                            offenseEffectMagnification: magnificationRate,
                            triggeredPossibility: triggeredPossibilityValue,
                            isRescueAble: isDamageControlAssitAble,
                            usageCount: skillMaster.usageCount, veiledFromTurn: 1,
                            veiledToTurn: skillMaster.veiledTurn);

                        skillsList.Add(effectClass);

                    }
                
            }

            return (battleUnits, skillsList);
        }

        private void CalculateCombatStatusFromUnit(UnitClass inputUnit)
        {
                calculateUnitStatus.Init(inputUnit);
//                _calculateUnitStatus = new CalculateUnitStatus(inputUnit);
        }


    }
}
