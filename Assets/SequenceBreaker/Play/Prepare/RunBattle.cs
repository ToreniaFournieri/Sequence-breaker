using System;
using System.Collections.Generic;
using SequenceBreaker.Environment;
using SequenceBreaker.Master.BattleUnit;
using SequenceBreaker.Master.Mission;
using SequenceBreaker.Master.Skills;
using SequenceBreaker.Master.Units;
using SequenceBreaker.Play.Battle;
using SequenceBreaker.Timeline.BattleLogView;
using SequenceBreaker.Translate;
using UnityEngine;

namespace SequenceBreaker.Play.Prepare
{
    public sealed class RunBattle : MonoBehaviour
    {


        public MissionMaster mission;
        public string currentMissionName;
        public int currentLevel;


        // depend on the slider value
        //        public int missionLevelCurrent;
        public WhichWin whichWin;
        public List<WhichWin> whichWinEachWaves;

        //Output information
        //public List<List<Data>> dataList;
        public List<Data> dataList;


        // to get current Ally Battle unit
        public List<BattleUnit> currentAllyUnitList;
        public int wavebyCopyed;


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

            deepCopyRunBattle.whichWin = whichWin;
            deepCopyRunBattle.dataList = dataList;
            deepCopyRunBattle.currentAllyUnitList = currentAllyUnitList;
            deepCopyRunBattle.whichWinEachWaves = whichWinEachWaves;
            deepCopyRunBattle.mission = mission;

            return deepCopyRunBattle;

        }

        public RunBattle Copy(int wave)
        {
            var deepCopyRunBattle = new RunBattle();
            deepCopyRunBattle.whichWin = whichWinEachWaves[wave];
            deepCopyRunBattle.dataList = new List<Data>();

            List<Data> _dataOfWave = dataList.FindAll(x => x.Wave == wave);
            //deepCopyRunBattle.dataList.Add(dataList[wave]);
            deepCopyRunBattle.dataList = _dataOfWave;
            deepCopyRunBattle.currentAllyUnitList = currentAllyUnitList;
            deepCopyRunBattle.whichWinEachWaves = whichWinEachWaves;
            deepCopyRunBattle.mission = mission;


            wavebyCopyed = wave;

            return deepCopyRunBattle;

        }




        public void Set(RunBattle runBattle)
        {
            var runbattle = runBattle.Copy();

            whichWin = runbattle.whichWin;
            dataList = runbattle.dataList;
            whichWinEachWaves = runbattle.whichWinEachWaves;
            mission = runbattle.mission;


        }

        public void Run(int enemyLevel, List<UnitClass> allyUnitList)
        {
            _allyBattleUnits = new List<BattleUnit>();
            _battleList = new List<BattleEngine>();
            whichWinEachWaves = new List<WhichWin>();
            dataList = new List<Data>();

            (_allyBattleUnits, _allySkillsList) = SetUpBattleUnitFromUnit(allyUnitList);

            currentAllyUnitList = new List<BattleUnit>();
            currentAllyUnitList = _allyBattleUnits;

            var wave = 0;

            foreach (var unitWave in mission.unitSet.unitSetList)
            {


                _battleList.Add(new BattleEngine());


                _battleList[wave].SetUpEnvironment(normalAttackSkillMaster: CalculateUnitStatus.Get.master.battleEnvironment.normalAttackSkillsMaster,
                        buffMasters: CalculateUnitStatus.Get.master.battleEnvironment.buffMasters);

                foreach (var unit in currentAllyUnitList)
                {
                    var allyBattleUnit = _allyBattleUnits.Find(obj => obj.uniqueId == unit.uniqueId);
                    allyBattleUnit.combat.shieldCurrent = unit.combat.shieldCurrent;
                    allyBattleUnit.combat.hitPointCurrent = unit.combat.hitPointCurrent;

                }

                var isFirstWave = false;
                if (wave == 0) { isFirstWave = true; }
                _battleList[wave].SetAllyBattleUnits(_allyBattleUnits, _allySkillsList, isFirstWave);

                //adjust enemy level
                foreach (var unit in unitWave.unitWave)
                {
                    unit.level = enemyLevel;
                }

                unitWave.level = enemyLevel;

                _enemyBattleUnits = new List<BattleUnit>();
                (_enemyBattleUnits, _enemySkillsList) = SetUpBattleUnitFromUnit(unitWave.unitWave);

                //set unitName with levels
                foreach (var battleUnit in _enemyBattleUnits)
                {
                    currentLevel = enemyLevel;
                }

                _battleList[wave].SetEnemyBattleUnits(_enemyBattleUnits, _enemySkillsList);


                //[Battle start!!]
                _battleList[wave].Battle();


                whichWin = _battleList[wave].WhichWin;
                whichWinEachWaves.Add(_battleList[wave].WhichWin);

                currentAllyUnitList = _battleList[wave].AllyBattleUnitsList;

                //dataList.Add(new List<Data>());
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
                //string unitHealthText = null;
                string reactText = null;
                float previousShieldRatio = 0f;
                var shieldRatio = 0f;
                float previousHpRatio = 0f;
                var hPRatio = 0f;
                var barrierRemains = 0;
                var isDead = true;
                if (battle.LogList[i].Order != null)
                {
                    unitNameText = battle.LogList[i].Order.Actor.longName;

                    if (battle.LogList[i].Order.IndividualTarget != null)
                    {
                        var preposition = " to ";
                        if (battle.LogList[i].Order.ActionType == ActionType.ReAttack) { preposition = " of "; }

                        reactText = battle.LogList[i].Order.ActionType + preposition + battle.LogList[i].Order.IndividualTarget.longName;
                    }

                    previousShieldRatio = battle.LogList[i].Order.Actor.previousShield / (float)battle.LogList[i].Order.Actor.combat.shieldMax;
                    shieldRatio = battle.LogList[i].Order.Actor.combat.shieldCurrent / (float)battle.LogList[i].Order.Actor.combat.shieldMax;
                    previousHpRatio = battle.LogList[i].Order.Actor.previousHp / (float)battle.LogList[i].Order.Actor.combat.hitPointMax;
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

                string speedString = null;
                if (battle.LogList[i].OrderCondition.Speed != 0)
                {
                    speedString = " " + Word.Get("Speed") + ":" + battle.LogList[i].OrderCondition.Speed;
                }

                List<string> _mainText = null;
                string _bigText = null;
                if (battle.LogList[i].FirstLine != null)
                {
                    _mainText = battle.LogList[i].LogList;
                }
                else
                {
                    if (battle.LogList[i].LogList != null && (battle.LogList[i].LogList.Count >= 1))
                    {
                        foreach (string words in battle.LogList[i].LogList)
                        {
                            _bigText += words + "\n";
                        }

                        //_bigText = battle.LogList[i].LogList[0];
                    }
                }




                //_data.Add(new Data
                //{
                //    Index = i,
                //    Turn = battle.LogList[i].OrderCondition.Turn,
                //    CellSize = 1300, // this is dummy, need to calculate later
                //    ReactText = reactText,
                //    Wave = wave,

                //    UnitInfo = unitNameText + " " + speedString,
                //    FirstLine = battle.LogList[i].FirstLine,
                //    MainTextList = _mainText,
                //    BigText = _bigText,
                //    Affiliation = battle.LogList[i].WhichAffiliationAct,
                //    NestLevel = battle.LogList[i].OrderCondition.Nest,
                //    IsDead = isDead,
                //    BarrierRemains = barrierRemains,
                //    PreviousShieldRatio = previousShieldRatio,
                //    ShieldRatio = shieldRatio,
                //    PreviousHpRatio = previousHpRatio,
                //    HpRatio = hPRatio,
                //    IsHeaderInfo = battle.LogList[i].isHeaderInfo,
                //    HeaderText = battle.LogList[i].headerInfoText,
                //    Characters = characters
                //});
                dataList.Add(new Data
                {
                    Index = i,
                    Turn = battle.LogList[i].OrderCondition.Turn,
                    CellSize = 1300, // this is dummy, need to calculate later
                    ReactText = reactText,
                    Wave = wave,

                    UnitInfo = unitNameText + " " + speedString,
                    FirstLine = battle.LogList[i].FirstLine,
                    MainTextList = _mainText,
                    BigText = _bigText,
                    Affiliation = battle.LogList[i].WhichAffiliationAct,
                    NestLevel = battle.LogList[i].OrderCondition.Nest,
                    IsDead = isDead,
                    BarrierRemains = barrierRemains,
                    PreviousShieldRatio = previousShieldRatio,
                    ShieldRatio = shieldRatio,
                    PreviousHpRatio = previousHpRatio,
                    HpRatio = hPRatio,
                    IsHeaderInfo = battle.LogList[i].isHeaderInfo,
                    HeaderText = battle.LogList[i].headerInfoText,
                    Characters = characters
                });
                // set all of data to data (activate)
                //dataList[wave] = _data;
                //dataList.Add(_data);
            }
        }


        public (List<BattleUnit> BattleUnits, List<EffectClass> SkillsList) SetUpBattleUnitFromUnit(List<UnitClass> unitList)
        {
            var battleUnits = new List<BattleUnit>();
            var skillsList = new List<EffectClass>();
            foreach (var unit in unitList)
            {
                CalculateCombatStatusFromUnit(unit);
                battleUnits.Add(CalculateUnitStatus.Get.battleUnit);

                List<SkillsMasterClass> skillList = CalculateUnitStatus.Get.summedSkillList;
                EffectClass effectClass;

                foreach (var skillMaster in skillList)
                {

                    // Skills offenseEffectMagnification calculation
                    var magnificationRate = 1.0;
                    switch (skillMaster.actionType)
                    {
                        case ActionType.Counter:
                            magnificationRate = CalculateUnitStatus.Get.battleUnit.skillMagnification.offenseEffectPower
                                .counter;
                            break;
                        case ActionType.Chain:
                            magnificationRate = CalculateUnitStatus.Get.battleUnit.skillMagnification.offenseEffectPower
                                .chain;
                            break;
                        case ActionType.ReAttack:
                            magnificationRate = CalculateUnitStatus.Get.battleUnit.skillMagnification.offenseEffectPower
                                .reAttack;
                            break;
                        case ActionType.Move:
                            magnificationRate = CalculateUnitStatus.Get.battleUnit.skillMagnification.offenseEffectPower
                                .move;
                            break;
                        case ActionType.Interrupt:
                            magnificationRate = CalculateUnitStatus.Get.battleUnit.skillMagnification.offenseEffectPower
                                .interrupt;
                            break;
                        case ActionType.AtBeginning:
                            magnificationRate = CalculateUnitStatus.Get.battleUnit.skillMagnification.offenseEffectPower
                                .atBeginning;
                            break;
                        case ActionType.AtEnding:
                            magnificationRate = CalculateUnitStatus.Get.battleUnit.skillMagnification.offenseEffectPower
                                .atEnding;
                            break;
                    }

                    // Skills Possibility Rate calculation
                    var abilityValue = 0.0;
                    var possibilityMagnificationRate = 1.0;
                    switch (skillMaster.ability)
                    {
                        case Ability.Power:
                            abilityValue = CalculateUnitStatus.Get.battleUnit.ability.power;
                            break;
                        case Ability.Generation:
                            abilityValue = CalculateUnitStatus.Get.battleUnit.ability.generation;
                            break;
                        case Ability.Responsiveness:
                            abilityValue = CalculateUnitStatus.Get.battleUnit.ability.responsiveness;
                            break;
                        case Ability.Intelligence:
                            abilityValue = CalculateUnitStatus.Get.battleUnit.ability.intelligence;
                            break;
                        case Ability.Precision:
                            abilityValue = CalculateUnitStatus.Get.battleUnit.ability.precision;
                            break;
                        case Ability.Luck:
                            abilityValue = CalculateUnitStatus.Get.battleUnit.ability.luck;
                            break;
                        case Ability.None:
                            abilityValue = 0.0;
                            break;
                    }

                    switch (skillMaster.actionType)
                    {
                        case ActionType.Counter:
                            possibilityMagnificationRate = CalculateUnitStatus.Get.battleUnit.skillMagnification
                                .triggerPossibility.counter;
                            break;
                        case ActionType.Chain:
                            possibilityMagnificationRate = CalculateUnitStatus.Get.battleUnit.skillMagnification
                                .triggerPossibility.chain;
                            break;
                        case ActionType.ReAttack:
                            possibilityMagnificationRate = CalculateUnitStatus.Get.battleUnit.skillMagnification
                                .triggerPossibility.reAttack;
                            break;
                        case ActionType.Move:
                            possibilityMagnificationRate = CalculateUnitStatus.Get.battleUnit.skillMagnification
                                .triggerPossibility.move;
                            break;
                        case ActionType.Interrupt:
                            possibilityMagnificationRate = CalculateUnitStatus.Get.battleUnit.skillMagnification
                                .triggerPossibility.interrupt;
                            break;
                        case ActionType.AtBeginning:
                            possibilityMagnificationRate = CalculateUnitStatus.Get.battleUnit.skillMagnification
                                .triggerPossibility.atBeginning;
                            break;
                        case ActionType.AtEnding:
                            possibilityMagnificationRate = CalculateUnitStatus.Get.battleUnit.skillMagnification
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

                    effectClass = new EffectClass(character: CalculateUnitStatus.Get.battleUnit, skill: skillMaster,
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

            CalculateUnitStatus.Get.Init(inputUnit);
        }


    }
}
