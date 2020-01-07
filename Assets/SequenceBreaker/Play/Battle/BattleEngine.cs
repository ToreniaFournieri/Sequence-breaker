using System;
using System.Collections.Generic;
using System.Linq;
using SequenceBreaker.Environment;
using SequenceBreaker.Master.BattleUnit;
using SequenceBreaker.Master.Skills;
using SequenceBreaker.Translate;
using UnityEngine;
using Random = System.Random;

namespace SequenceBreaker.Play.Battle
{
    public sealed class BattleEngine
    {
        //For output result
        public List<BattleLogClass> LogList;
        public WhichWin WhichWin = WhichWin.None; // get only last one
        // output data for sequence battle.
        public List<BattleUnit> AllyBattleUnitsList;

        private int _count;
        private readonly List<BattleUnit> _characters = new List<BattleUnit>();

        //Skill make logic , test: one character per one skill
        private SkillsMasterClass _normalAttackSkillMaster;
        private List<SkillsMasterClass> _buffMasters;

        //Effect (permanent skill) make logic, the effect has two meanings, one is permanent skill, the other is temporary skill (may call buff).
        private readonly List<EffectClass> _effects = new List<EffectClass>();

        ///<summary>
        ///Set up a battle environment.
        ///</summary>
        public void SetUpEnvironment(SkillsMasterClass normalAttackSkillMaster, List<SkillsMasterClass> buffMasters)
        {
            _normalAttackSkillMaster = normalAttackSkillMaster;
            _buffMasters = buffMasters;
        }

        /// <summary>
        /// Set up Ally's BattleUnit and EffectClass.
        /// </summary>
        /// <param name="allyBattleUnits"> ally battle units, who enter the battle.</param>
        /// <param name="allyEffectClasses"> Effect class of ally battle units</param>
        /// <param name="isFirstWave"> isFirstWave = true, heal all units. Otherwise,they keep shield and HP.</param>
        //
        public void SetAllyBattleUnits(List<BattleUnit> allyBattleUnits, List<EffectClass> allyEffectClasses, bool isFirstWave)
        {

            foreach (var allyBattleUnit in allyBattleUnits)
            {
                // Forced update Affiliation to ally(not worked?)
                allyBattleUnit.affiliation = Affiliation.Ally;
                allyBattleUnit.uniqueId = _count;

                if (isFirstWave)
                {
                    allyBattleUnit.combat.shieldCurrent = allyBattleUnit.combat.shieldMax;
                    allyBattleUnit.combat.hitPointCurrent = allyBattleUnit.combat.hitPointMax;
                }

                _characters.Add(allyBattleUnit);
                _count++;
            }

            foreach (var allyEffectClass in allyEffectClasses)
            {
                _effects.Add(allyEffectClass);
            }

        }

        public void SetEnemyBattleUnits(List<BattleUnit> enemyBattleUnits, List<EffectClass> enemyEffectClasses)
        {

            foreach (var enemyBattleUnit in enemyBattleUnits)
            {
                // Forced update Affiliation to enemy (not worked?)
                enemyBattleUnit.affiliation = Affiliation.Enemy;
                enemyBattleUnit.uniqueId = _count;
                enemyBattleUnit.combat.shieldCurrent = enemyBattleUnit.combat.shieldMax;
                enemyBattleUnit.combat.hitPointCurrent = enemyBattleUnit.combat.hitPointMax;

                _characters.Add(enemyBattleUnit);
                _count++;
            }

            foreach (var enemyEffectClass in enemyEffectClasses)
            {
                _effects.Add(enemyEffectClass);
            }
        }



        public void Battle()
        {
            var startDateTime = DateTime.Now;
            // Battle environment setting
            const int battleWavesSets = 1;
            const int battleWaves = 1; // one set of battle 
            //const string navigatorName = "Navigator";

            //FuncBattleConditionsText text;
            int seed = (int)DateTime.Now.Ticks; // when you find something wrong, use seed value to Reproduction the situation
            //seed = 1086664070; // Reproduction.
            var r = new Random(seed);

            // System initialize : Do not change them.
            var battleLogList = new List<BattleLogClass>();
            List<string> finalLogList = new List<string>();
            var totalTurn = 0; // Static info of total turn
            var currentBattleWaves = 1;


            //------------------------Battle Main Engine------------------------
            //Logic: Number of Battle
            for (int battleWavesSet = 1; battleWavesSet <= battleWavesSets; battleWavesSet++)
            {
                //set up set info
                var statisticsReporterFirstBlood = new List<StatisticsReporterFirstBloodClass>();
                var statisticsReporterWhichWins = new WhichWin[battleWaves];

                var allyWinCount = 0; var enemyWinCount = 0; var drawCount = 0;

                for (int battleWave = 1; battleWave <= battleWaves; battleWave++)
                {
                    var allyFirstBlood = false;
                    var enemyFirstBlood = false; // Set up Phase
                    statisticsReporterFirstBlood.Add(new StatisticsReporterFirstBloodClass(battleWave));

                    foreach (BattleUnit battleUnit in _characters)
                    {
                        //Stop healing
                        battleUnit.Deterioration = 0.0; //Deterioration initialize to 0.0
                        battleUnit.buff.InitializeBuff(); //Buff initialize
                        battleUnit.buff.BarrierRemaining = 0; //Barrier initialize
                        battleUnit.feature.InitializeFeature(); //Feature initialize
                        battleUnit.Statistics.Initialize(); // Initialise
                        battleUnit.previousTurnShield = battleUnit.combat.shieldCurrent;
                        battleUnit.previousTurnHp = battleUnit.combat.hitPointCurrent;
                    }
                    //effects will be set by outside of this battle engine, so effect initialize is needed.
                    foreach (var effect in _effects)
                    {
                        effect.InitializeEffect();
                    }

                    var battleEnd = false;
                    currentBattleWaves = battleWave;

                    while (battleEnd == false)
                    {
                        var environmentInfo = new EnvironmentInfoClass(battleWave, 0, 0, seed, r);

                        for (var turn = 1; turn <= 20; turn++)
                        {
                            environmentInfo.Turn = turn;
                            //------------------------Header Phase------------------------
                            //Battle End check
                            if (battleEnd) { continue; } //continue turn.

                            //Effect/Buff initialize and Set again
                            foreach (var t in _characters)
                            {
                                t.buff.InitializeBuff();
                            }
                            foreach (var effect in _effects) { effect.BuffToCharacter(turn); }


                            for (var actionPhase = 0; actionPhase <= 3; actionPhase++)
                            {
                                //------------------------Action order routine------------------------
                                //Only alive character should be action.
                                var aliveCharacters = _characters.FindAll(character1 => character1.combat.hitPointCurrent > 0);

                                //Action order decision for each turn
                                var orders = new Stack<OrderClass>();
                                var orderForSort = new List<OrderClass>();

                                {
                                    string firstLine = null;
                                    List<string> logList = new List<string>();
                                    BattleLogClass battleLog;
                                    OrderConditionClass orderCondition;

                                    // ReSharper disable once SwitchStatementMissingSomeCases
                                    ///<summary>
                                    /// actionPhase 0: Show Turns, ally and enemy infomation.
                                    /// actionPhase 1: At biginning Skill phase.
                                    /// actionPhase 2: Main phase.
                                    /// actionPhase 3: At End phase.
                                    ///</summary>
                                    switch (actionPhase)
                                    {
                                        case 0:
                                            {
                                                environmentInfo.Phase = 0;

                                                orderCondition = new OrderConditionClass(environmentInfo.Wave,
                                                    environmentInfo.Turn, environmentInfo.Phase, 0, 0, 0, 0);
                                                battleLog = new BattleLogClass(orderCondition, null, firstLine, logList,
                                                    Affiliation.None)
                                                {
                                                    isHeaderInfo = true,
                                                    headerInfoText = Word.Get("Turn") + " " + turn + "\n"
                                                };
                                                var copiedBattleUnit = (from BattleUnit character in _characters
                                                                        select character.Copy()).ToList();
                                                battleLog.characters = copiedBattleUnit;
                                                battleLogList.Add(battleLog);
                                                break;
                                            }
                                        case 1:
                                            {
                                                environmentInfo.Phase = 1;

                                                // _/_/_/_/_/_/_/_/ At Beginning Skill _/_/_/_/_/_/_/_/_/_/

                                                var skillTriggerPossibilityCheck = SkillTriggerPossibilityCheck.Get(null, _effects,
                                                    _characters, null,
                                                    orders, ActionType.AtBeginning, false, false, null, null, 0,
                                                    environmentInfo);
                                                while (skillTriggerPossibilityCheck != null &&
                                                       skillTriggerPossibilityCheck.Count > 0)
                                                {
                                                    orders.Push(skillTriggerPossibilityCheck.Pop());
                                                }

                                                break;
                                            }
                                        case 2:
                                            {
                                                environmentInfo.Phase = 2;

                                                // save HP and shield infomation
                                                foreach (var character in _characters)
                                                {
                                                    character.previousTurnShield = character.combat.shieldCurrent;
                                                    character.previousTurnHp = character.combat.hitPointCurrent;

                                                }

                                                for (var i = 0; i <= aliveCharacters.Count - 1; i++)
                                                {

                                                    var effectList = _effects.FindAll(obj =>
                                                        obj.character == aliveCharacters[i] && obj.skill.actionType ==
                                                                                            ActionType.Move
                                                                                            && obj.UsageCount > 0 &&
                                                                                            obj.VeiledFromTurn <= turn &&
                                                                                            obj.VeiledToTurn >= turn);
                                                    int speed = (aliveCharacters[i].ability.responsiveness *
                                                         r.Next(40 + aliveCharacters[i].ability.luck, 100) / 10);

                                                    orderCondition = new OrderConditionClass(environmentInfo.Wave,
                                                        environmentInfo.Turn, environmentInfo.Phase, 0, 0, 0, speed);

                                                    var normalAttackEffect = ScriptableObject.CreateInstance<EffectClass>();
                                                    normalAttackEffect.Set(aliveCharacters[i],
                                                        _normalAttackSkillMaster, ActionType.NormalAttack, 1.0,
                                                        1.0, false, 1000, 1, 20);

                                                    effectList.Add(normalAttackEffect);
                                                    orderForSort.Add(new OrderClass(orderCondition, aliveCharacters[i],
                                                        ActionType.Move, ref effectList, speed, null, false));
                                                }


                                                orderForSort.Sort((x, y) => x.ActionSpeed - y.ActionSpeed);
                                                var orderNumber = orderForSort.Count;
                                                foreach (var order in orderForSort)
                                                {
                                                    order.OrderCondition.OrderNumber = orderNumber;
                                                    orderNumber--;
                                                }

                                                foreach (var t in orderForSort)
                                                {
                                                    orders.Push(t);
                                                }

                                                break;
                                            }
                                        case 3:
                                            {
                                                environmentInfo.Phase = 3;
                                                logList.Add(new string(' ', 1) + "[At ending phase] ");
                                                //Initialize 

                                                //Heal Shield by generation %

                                                foreach (var character in _characters)
                                                {
                                                    List<string> phaseEndLogList = PhaseEndFunction.Get(character);
                                                    foreach (string log in phaseEndLogList)
                                                    {
                                                        logList.Add(log);
                                                    }

                                                }

                                                orderCondition = new OrderConditionClass(environmentInfo.Wave,
                                                    environmentInfo.Turn, environmentInfo.Phase, 0, 0, 0, 0);
                                                battleLog = new BattleLogClass(orderCondition, null, null, logList,
                                                    Affiliation.None);
                                                battleLogList.Add(battleLog);
                                                break;
                                            }
                                    }
                                }

                                ///<summary>
                                /// ------------------------Action phase------------------------
                                /// Action for each character by action order.
                                /// Loop until order is null.
                                ///</summary>
                                while (orders.Any())
                                {
                                    //[1]. ------------------------Start up------------------------
                                    //[1-1]. Difinition.
                                    string firstLine;
                                    List<string> logList;
                                    var order = orders.Pop();
                                    BattleLogClass battleLog;
                                    if (order.Actor == null) { continue; } // attacker alive check, if crushed, continue.
                                    if (order.Actor.combat.hitPointCurrent <= 0) { continue; }

                                    //[1-2]. [SKILLS CHECK] Interrupt skills trigger.

                                    //[1-3]. Clear previous infomation of characters.
                                    foreach (BattleUnit character in _characters)
                                    {
                                        character.previousBarrier = 0;
                                        character.previousShield = character.combat.shieldCurrent;
                                        character.previousHp = character.combat.hitPointCurrent;
                                    }

                                    //[2]. ------------------------Individual attacker's move phase------------------------

                                    //[3]. ------------------------Attacker's action decision phase------------------------

                                    //[3-1]. [ SKILLS CHECK ] Move skills trigger.
                                    order.SkillDecision(_characters, environmentInfo);

                                    //[3-2]. Effect spend.
                                    if (order.SkillEffectChosen != null)
                                    {
                                        order.SkillEffectChosen.UsageCount -= 1;
                                        order.SkillEffectChosen.SpentCount += 1;
                                        order.SkillEffectChosen.NextAccumulationCount += (int)(order.SkillEffectChosen.skill.triggerBase.accumulationBaseRate
                                            * order.SkillEffectChosen.skill.triggerBase.accumulationWeight);
                                    }

                                    //[3-3]. Cast Skill, Mainly heal logic.
                                    bool willSkip = false;
                                    (firstLine, logList) = SkillLogicDispatcher.Get(order, _characters, environmentInfo); // SkillLogic action include rescue.
                                    if (firstLine != null)
                                    {
                                        battleLog = new BattleLogClass(order.OrderCondition, order, firstLine, logList, order.Actor.affiliation);
                                        battleLogList.Add(battleLog);
                                        willSkip = true;
                                    }
                                    if (willSkip != true)
                                    {
                                        (firstLine, logList) = BuffDebuffFunction.Get(order, _characters, _effects, _buffMasters, turn); //Buff, debuff action


                                        // For debug
                                        if (order.SkillEffectChosen.skill.skillName != "Normal Attack")
                                        {
                                            Debug.Log(turn + " buff action: " + order.Actor.shortName + " skill:" + order.SkillEffectChosen.skill.skillName);
                                        }
                                        if (firstLine != null)
                                        {
                                            battleLog = new BattleLogClass(order.OrderCondition, order, firstLine, logList, order.Actor.affiliation);
                                            battleLogList.Add(battleLog);
                                        }
                                    }

                                    //[3-4]. Cast skill, mainly action skill, strong attack.
                                    var (battleLogClass, battleResult) = SkillMoveFunction.Get(order, _characters, environmentInfo);
                                    if (battleLogClass != null) { logList = battleLogClass.LogList; firstLine = battleLogClass.FirstLine; }
                                    if (logList != null && firstLine != null)
                                    {
                                        battleLog = new BattleLogClass(order.OrderCondition, order, firstLine, logList, order.Actor.affiliation);
                                        battleLogList.Add(battleLog);
                                    }

                                    //[3-5]. Rescue action.
                                    if (order.IsRescue) //only when rescue
                                    {
                                        var deleteOneActionOrderIfHave = orders.ToList();
                                        var deleteOneActionOrderRaw = deleteOneActionOrderIfHave.FindLast(obj => obj.Actor == order.Actor && obj.ActionType == ActionType.Move);
                                        OrderClass deleteOneActionOrder = null;
                                        foreach (var effect in deleteOneActionOrderRaw.SkillEffectProposed)
                                        { if (effect.skill.isNormalAttack) { deleteOneActionOrder = deleteOneActionOrderRaw; } }
                                        deleteOneActionOrderIfHave.Remove(deleteOneActionOrder);
                                        deleteOneActionOrderIfHave.Reverse();
                                        if (deleteOneActionOrder != null) //clear stack and input again
                                        { orders.Clear(); foreach (var data in deleteOneActionOrderIfHave) { orders.Push(data); } }
                                    }

                                    //[4]. ------------------------Statistis : First blood ------------------------
                                    //Only when first kill happened, insert statistics reporter for first blood per side.
                                    if (order.Actor.affiliation == Affiliation.Ally && allyFirstBlood == false && battleResult.NumberOfCrushed >= 1) //When ally first blood 
                                    {
                                        string es = null;
                                        if (battleResult.NumberOfCrushed != 1) { es = "es"; }
                                        var t = order.Actor.name + "'s " + order.SkillEffectChosen.skill.skillName + ". first blood! total dealt damage:" + battleResult.TotalDealtDamage.WithComma() + " " + battleResult.NumberOfCrushed.WithComma() + " crush" + es + ".";
                                        var setStatisticsReporterFirstBlood = statisticsReporterFirstBlood.FindLast(obj => obj.BattleWave == battleWave);
                                        setStatisticsReporterFirstBlood.Set(Affiliation.Ally, order.Actor.name, battleResult.TotalDealtDamage, t);
                                        allyFirstBlood = true;
                                    }
                                    else if (order.Actor.affiliation == Affiliation.Enemy && enemyFirstBlood == false && battleResult.NumberOfCrushed >= 1) //When enemy first blood 
                                    {
                                        string es = null;
                                        if (battleResult.NumberOfCrushed != 1) { es = "es"; }
                                        var t = order.Actor.name + "'s " + order.SkillEffectChosen.skill.skillName + ". first blood! total dealt damage:" + battleResult.TotalDealtDamage.WithComma() + " " + battleResult.NumberOfCrushed.WithComma() + " crush" + es + ".";
                                        var setStatisticsReporterFirstBlood = statisticsReporterFirstBlood.FindLast(obj => obj.BattleWave == battleWave);
                                        setStatisticsReporterFirstBlood.Set(Affiliation.Enemy, order.Actor.name, battleResult.TotalDealtDamage, t);
                                        if (battleLogClass != null) { setStatisticsReporterFirstBlood.BattleLogEnemy = battleLogClass; }
                                        enemyFirstBlood = true;
                                    }


                                    //[5]. ------------------------Battle end check.------------------------
                                    //  "battleResult.BattleEnd = true" means battle is ended.
                                    if (battleEnd == false)
                                    {
                                        battleEnd = battleResult.BattleEnd;
                                        if (battleResult.IsAllyWin)
                                        {
                                            allyWinCount++; statisticsReporterWhichWins[battleWave - 1] = WhichWin.AllyWin;
                                            WhichWin = WhichWin.AllyWin;
                                        }
                                        if (battleResult.IsEnemyWin)
                                        {
                                            enemyWinCount++; statisticsReporterWhichWins[battleWave - 1] = WhichWin.EnemyWin;
                                            WhichWin = WhichWin.EnemyWin;

                                        }
                                        if (battleResult.IsDraw)
                                        {
                                            drawCount++; statisticsReporterWhichWins[battleWave - 1] = WhichWin.Draw;
                                            WhichWin = WhichWin.Draw;
                                        }
                                    }

                                    //[6]. ------------------------ReAction Skill - check.------------------------

                                    var nestNumber = 0;


                                    //[6-1]. [SKILLS CHECK] Rescue trigger. Note: ActionType independent so ActionType.any!
                                    var damageControlAssistStack = SkillTriggerPossibilityCheck.Get(null, _effects, _characters,
                                        order, orders, ActionType.Any, true, true,
                                        battleResult, order.Actor, nestNumber, environmentInfo);

                                    //[6-2]. [SKILLS CHECK] ReAttack skills trigger.
                                    var reAttackStack = SkillTriggerPossibilityCheck.Get(null, _effects, _characters,
                                        order, orders, ActionType.ReAttack, false, false,
                                        battleResult, order.Actor, nestNumber, environmentInfo);

                                    //[6-3]. [SKILLS CHECK] Chain skills trigger.
                                    var chainStack = SkillTriggerPossibilityCheck.Get(null, _effects, _characters,
                                        order, orders, ActionType.Chain, false, false,
                                        battleResult, order.Actor, nestNumber, environmentInfo);

                                    //[6-4]. [SKILLS CHECK] Counter skills trigger.
                                    var counterStack = SkillTriggerPossibilityCheck.Get(null, _effects, _characters,
                                        order, orders, ActionType.Counter, false, false,
                                        battleResult, order.Actor, nestNumber, environmentInfo);

                                    //[7]. ------------------------ReAction Skill - push to orders.------------------------
                                    //Push order in reverse. counter -> chain -> reAttack -> rescue
                                    //[7-1]. Push Counter
                                    while (counterStack != null && counterStack.Count > 0) { orders.Push(counterStack.Pop()); nestNumber++; }
                                    //[7-2]. Push Chain
                                    while (chainStack != null && chainStack.Count > 0) { orders.Push(chainStack.Pop()); nestNumber++; }

                                    //[7-3]. Push ReAttack
                                    while (reAttackStack != null && reAttackStack.Count > 0) { orders.Push(reAttackStack.Pop()); nestNumber++; }

                                    //[7-4]. Push rescue
                                    while (damageControlAssistStack != null && damageControlAssistStack.Count > 0) { orders.Push(damageControlAssistStack.Pop()); nestNumber++; }

                                }  // Until all Characters act.

                            } // action Phase.
                            //[10]. ------------------------Footer phase------------------------
                            //[10-1]. Clear crushed flag. crushed flag is for rescure skill.
                            foreach (var character in _characters.Where(character => character.IsCrushedJustNow))
                            {
                                character.IsCrushedJustNow = false;
                            }

                            //[10-2]. Check wipe out and should continue the battle
                            if (battleEnd == false)
                            {
                                var wipeOutCheck = new WipeOutCheck(_characters);
                                if (wipeOutCheck.IsAllyWin) { allyWinCount++; WhichWin = WhichWin.AllyWin; }
                                if (wipeOutCheck.IsEnemyWin) { enemyWinCount++; WhichWin = WhichWin.EnemyWin; }
                                if (wipeOutCheck.IsDraw) { drawCount++; WhichWin = WhichWin.Draw; }
                                battleEnd = wipeOutCheck.BattleEnd;
                            }
                            totalTurn = turn; // If battle end, set total turn for show end log.

                        } //turn

                        //[11]. ------------------------Time Over------------------------
                        //[11-1]. Check wipe out and should continue the battle
                        if (battleEnd == false)
                        {
                            drawCount++; battleEnd = true;
                            WhichWin = WhichWin.Draw;

                            string log = Word.Get("Time over.");
                            var orderCondition = new OrderConditionClass(environmentInfo.Wave, environmentInfo.Turn, 4, 0, 0, 0, 0);
                            var battleLog = new BattleLogClass(orderCondition, null, log, null, Affiliation.None);
                            battleLogList.Add(battleLog);
                        }
                        //[11-2]. time over Statistics.


                    } //battleEnd 


                    //[12]. ------------------------Statistics to characters------------------------
                    foreach (BattleUnit character in _characters)
                    {
                        character.SetPermanentStatistics(character.Statistics);
                    }
                } //Battle waves

            } // Battle waves set

            //Battle is over.

            //[13]. ------------------------Battle is Over. clean up------------------------
            //[13-1]. 
            var orderConditionFinal = new OrderConditionClass(currentBattleWaves, totalTurn + 1, 0, 0, 0, 0, 0);
            var battleLogFinal = new BattleLogClass(orderConditionFinal, null, null, null, Affiliation.None)
            {
                isHeaderInfo = true,
                headerInfoText = Word.Get("Turn") + " " + (totalTurn + 1) + "\n"
            };
            var copiedBattleUnitLast = (from BattleUnit character in _characters
                                        select character.Copy()).ToList();
            battleLogFinal.characters = copiedBattleUnitLast;
            battleLogList.Add(battleLogFinal);


            //[13-2]. Set ally units for next battle
            AllyBattleUnitsList = battleLogFinal.characters.FindAll(unit => unit.affiliation == Affiliation.Ally);


            //[13-3]. This is the last cell for debug.
            finalLogList.Add("Battle is over. " + Word.Get(WhichWin.ToString()));

            var finishDateTime = DateTime.Now;
            var processedTimeSpan = finishDateTime - startDateTime;
            finalLogList.Add("finished:" + finishDateTime);
            finalLogList.Add(" processed time:" + processedTimeSpan);
            finalLogList.Add(" seed:" + seed);

            var battleLogDisplayList = battleLogList.FindAll(obj => obj.OrderCondition.Wave == battleWaves); // only last battle Log displayed.
            var finalOrderCondition = new OrderConditionClass(battleWaves, totalTurn + 1, 0, 0, 0, 0, 0);
            var finalLogListToSet = new BattleLogClass(finalOrderCondition, null, null, finalLogList, Affiliation.None);
            battleLogDisplayList.Add(finalLogListToSet);
            LogList = battleLogDisplayList;

        }

    }
} //End of MainClass


