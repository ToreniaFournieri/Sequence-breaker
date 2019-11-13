using System;
using System.Collections.Generic;
using System.Linq;
using SequenceBreaker._00_System;
using SequenceBreaker._01_Data.BattleUnit;
using SequenceBreaker._01_Data.Skills;

namespace SequenceBreaker._05_Play.Battle
{
    public sealed class BattleEngine
    {
        //For output result
        public List<BattleLogClass> LogList;
        public WhichWin WhichWin = WhichWin.None; // get only last one
        // output data for sequence battle.
        public List<BattleUnit> AllyBattleUnitsList;
        private static double _allyAttackMagnification = 1.0;
        private static double _allyDefenseMagnification = 1.0;

        private int _count;
        private readonly List<BattleUnit> _characters = new List<BattleUnit>();
        private AbilityClass[] _abilities;

        //Skill make logic , test: one character per one skill
        private SkillsMasterClass _normalAttackSkillMaster;
        private List<SkillsMasterClass> _buffMasters;

        //Effect (permanent skill) make logic, the effect has two meanings, one is permanent skill, the other is temporary skill (may call buff).
        private readonly List<EffectClass> _effects = new List<EffectClass>();

        //Set up a battle environment.
        public void SetUpEnvironment(SkillsMasterClass normalAttackSkillMaster, List<SkillsMasterClass> buffMasters)
        {
            _normalAttackSkillMaster = normalAttackSkillMaster;
            _buffMasters = buffMasters;
        }
        
        //Set up Ally's BattleUnit and EffectClass.
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
            Console.WriteLine("start:" + startDateTime);
            // Battle environment setting
            const int battleWavesSets = 1;
            const int battleWaves = 1; // one set of battle 
            const string navigatorName = "Navigator";

            //BattleWaveSet variables
            const double allyAttackMagnificationPerWavesSet = 0.0;
            const double allyDefenseMagnificationPerWavesSet = 0.0;

            FuncBattleConditionsText text;
            var seed = (int)DateTime.Now.Ticks; // when you find something wrong, use seed value to Reproduction the situation
            //seed = 1086664070; // Reproduction.
            var r = new Random(seed);

            // System initialize : Do not change them.
            var battleLogList = new List<BattleLogClass>();
            string finalLog = null;
            var logPerWavesSets = new string[battleWavesSets];
            var subLogPerWavesSets = new string[battleWavesSets];
            var totalTurn = 0; // Static info of total turn
            var currentBattleWaves = 1;


            //------------------------Battle Main Engine------------------------
            //Logic: Number of Battle
            for (var battleWavesSet = 1; battleWavesSet <= battleWavesSets; battleWavesSet++)
            {
                //set up set info
                var statisticsReporterFirstBlood = new List<StatisticsReporterFirstBloodClass>();
                var statisticsReporterWhichWins = new WhichWin[battleWaves];

                if (battleWavesSet > 1) //  magnification per wave set
                { _allyAttackMagnification *= (1 + allyAttackMagnificationPerWavesSet); _allyDefenseMagnification *= (1 + allyDefenseMagnificationPerWavesSet); }
                
                var allyWinCount = 0; var enemyWinCount = 0; var drawCount = 0;

                for (var battleWave = 1; battleWave <= battleWaves; battleWave++)
                {
                    var allyFirstBlood = false;
                    var enemyFirstBlood = false; // Set up Phase
                    statisticsReporterFirstBlood.Add(new StatisticsReporterFirstBloodClass(battleWave));

                    foreach (var t in _characters)
                    {
                        //Stop healing
                        t.Deterioration = 0.0; //Deterioration initialize to 0.0
                        t.buff.InitializeBuff(); //Buff initialize
                        t.buff.BarrierRemaining = 0; //Barrier initialize
                        t.feature.InitializeFeature(); //Feature initialize
                        t.Statistics.Initialize(); // Initialise
                    }
                    //effects will be set by outside of this battle engine, so effect initialize make different.
                    foreach (var effect in _effects)
                    {
                        effect.InitializeEffect();
                    }

//                    if (battleWave == battleWaves && battleWavesSet == battleWavesSets) // only last battle display inside.
//                    {
//                        foreach (var effect in _effects)
//                        {
//                            Console.WriteLine(effect.character.unitName + " has a skill: " + effect.skill.unitName + ". [possibility:" + (int)(effect.triggeredPossibility * 1000) / 10.0 + "%]"
//                                              + " Left:" + effect.UsageCount + " Offense Magnification:" + effect.offenseEffectMagnification + " " + effect.character.buff.DefenseMagnification);
//                        }
//                    }
                    // _/_/_/_/_/_/ Effect setting end _/_/_/_/_/_/

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
                                    string log = null;
                                    BattleLogClass battleLog;
                                    OrderConditionClass orderCondition;

                                    // ReSharper disable once SwitchStatementMissingSomeCases
                                    switch (actionPhase)
                                    {
                                        case 0:
                                        {
                                            environmentInfo.Phase = 0;
                                            //log += "------------------------------------\n";
                                            text = new FuncBattleConditionsText(turn, _characters);
                                            //firstLine = text.FirstLine();
                                            //log += text.Text();
                                            log += null;

                                            orderCondition = new OrderConditionClass(environmentInfo.Wave,
                                                environmentInfo.Turn, environmentInfo.Phase, 0, 0, 0);
                                            battleLog = new BattleLogClass(orderCondition, null, firstLine, log,
                                                Affiliation.None)
                                            {
                                                isHeaderInfo = true,
                                                headerInfoText = text.FirstLine()
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

                                            var skillTriggerPossibilityCheck = SkillTriggerPossibilityCheck(null, _effects,
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
                                            firstLine = "[Main action phase] \n";
                                            orderCondition = new OrderConditionClass(environmentInfo.Wave,
                                                environmentInfo.Turn, environmentInfo.Phase, 0, 0, 0);
                                            battleLog = new BattleLogClass(orderCondition, null, firstLine, null,
                                                Affiliation.None) {headerInfoText = "[Main action phase] \n"};
                                            //battleLogList.Add(battleLog);

                                            for (var i = 0; i <= aliveCharacters.Count - 1; i++)
                                            {
                                                var effectList = _effects.FindAll(obj =>
                                                    obj.character == aliveCharacters[i] && obj.skill.actionType ==
                                                                                        ActionType.Move
                                                                                        && obj.UsageCount > 0 &&
                                                                                        obj.VeiledFromTurn <= turn &&
                                                                                        obj.VeiledToTurn >= turn);
                                                orderCondition = new OrderConditionClass(environmentInfo.Wave,
                                                    environmentInfo.Turn, environmentInfo.Phase, 0, 0, 0);

                                                // Add normal attack skills
                                                var normalAttackEffect = new EffectClass(aliveCharacters[i],
                                                    _normalAttackSkillMaster, ActionType.NormalAttack, 1.0,
                                                    1.0, false, 1000, 1, 20);
                                                effectList.Add(normalAttackEffect);
                                                orderForSort.Add(new OrderClass(orderCondition, aliveCharacters[i],
                                                    ActionType.Move, ref effectList,
                                                    (aliveCharacters[i].ability.responsiveness *
                                                     r.Next(40 + aliveCharacters[i].ability.luck, 100)), null, false));
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
                                            log += new string(' ', 1) + "[At ending phase] \n";
                                            //Initialize 

                                            //Heal Shield by generation %
                                            var shieldHeal = new ShieldHealFunction(_characters);
                                            log += shieldHeal.Log;
                                            var calculateHate = new CalculationHateMagnificationPerTurnFunction(_characters);
                                            log += calculateHate.Log;

                                            orderCondition = new OrderConditionClass(environmentInfo.Wave,
                                                environmentInfo.Turn, environmentInfo.Phase, 0, 0, 0);
                                            battleLog = new BattleLogClass(orderCondition, null, null, log,
                                                Affiliation.None);
                                            battleLogList.Add(battleLog);
                                            break;
                                        }
                                    }
                                }
                                //------------------------Action phase------------------------
                                //Action for each character by action order.

                                while (orders.Any()) // loop until order is null.
                                {
                                    string firstLine;
                                    string log;
                                    var order = orders.Pop();
                                    BattleLogClass battleLog;
                                    if (order.Actor == null) { continue; } // attacker alive check, if crushed, continue.
                                    if (order.Actor.combat.hitPointCurrent <= 0) { continue; }

                                    //[[ SKILLS CHECK ]] Interrupt skills trigger.

                                    //------------------------Individual attacker's move phase------------------------

                                    //------------------------Attacker's action decision phase------------------------
                                    // BasicAttackFunction basicAttack;

                                    //[[ SKILLS CHECK ]] Move skills trigger.
                                    order.SkillDecision(_characters, environmentInfo);

                                    //effect spend.
                                    if (order.SkillEffectChosen != null)
                                    {
                                        order.SkillEffectChosen.UsageCount -= 1;
                                        order.SkillEffectChosen.SpentCount += 1;
                                        order.SkillEffectChosen.NextAccumulationCount += (int)(order.SkillEffectChosen.skill.triggerBase.accumulationBaseRate * order.SkillEffectChosen.skill.triggerBase.accumulationWeight);
                                    }


                                    var willSkip = false;
                                    (firstLine, log) = SkillLogicDispatcher(order, _characters, environmentInfo); // SkillLogic action include rescue.
                                    if (firstLine != null)
                                    {
                                        battleLog = new BattleLogClass(order.OrderCondition, order, firstLine, log, order.Actor.affiliation);
                                        battleLogList.Add(battleLog);
                                        willSkip = true;
                                    }
                                    if (willSkip != true)
                                    {
                                        (firstLine, log) = BuffDebuffFunction(order, _characters, _effects, _buffMasters, turn); //Buff, debuff action
                                        if (firstLine != null)
                                        {
                                            battleLog = new BattleLogClass(order.OrderCondition, order, firstLine, log, order.Actor.affiliation);
                                            battleLogList.Add(battleLog);
                                        }
                                    }

                                    var (battleLogClass, battleResult) = SkillMoveFunction(order, _characters, environmentInfo);
                                    if (battleLogClass != null) { log = battleLogClass.Log; firstLine = battleLogClass.FirstLine; }

                                    if (log != null)
                                    {
                                        battleLog = new BattleLogClass(order.OrderCondition, order, firstLine, log, order.Actor.affiliation);
                                        battleLogList.Add(battleLog);
                                    }


                                    if (order.IsRescue) //only when rescure
                                    {
                                        var deleteOneActionOrderIfHave = orders.ToList();
                                        var deleteOneActionOrderRaw = deleteOneActionOrderIfHave.FindLast(obj => obj.Actor == order.Actor && obj.ActionType == ActionType.Move);
                                        OrderClass deleteOneActionOrder = null;
                                        foreach (var effect in deleteOneActionOrderRaw.SkillEffectProposed)
                                        { if (effect.skill.name == SkillName.NormalAttack) { deleteOneActionOrder = deleteOneActionOrderRaw; } }
                                        deleteOneActionOrderIfHave.Remove(deleteOneActionOrder);
                                        deleteOneActionOrderIfHave.Reverse();
                                        if (deleteOneActionOrder != null) //clear stack and input again
                                        { orders.Clear(); foreach (var data in deleteOneActionOrderIfHave) { orders.Push(data); } }
                                    }
                                    //Only when first kill happened, insert statistics reporter for first blood per side.
                                    if (order.Actor.affiliation == Affiliation.Ally && allyFirstBlood == false && battleResult.NumberOfCrushed >= 1) //When ally first blood 
                                    {
                                        string es = null;
                                        if (battleResult.NumberOfCrushed != 1) { es = "es"; }
                                        var t = order.Actor.name + "'s " + order.SkillEffectChosen.skill.name + ". first blood! total dealt damage:" + battleResult.TotalDealtDamage.WithComma() + " " + battleResult.NumberOfCrushed.WithComma() + " crush" + es + ".";
                                        var setStatisticsReporterFirstBlood = statisticsReporterFirstBlood.FindLast(obj => obj.BattleWave == battleWave);
                                        setStatisticsReporterFirstBlood.Set(Affiliation.Ally, order.Actor.name, battleResult.TotalDealtDamage, t);
                                        if (battleLogClass != null) {
                                        }
                                        allyFirstBlood = true;

                                    }
                                    else if (order.Actor.affiliation == Affiliation.Enemy && enemyFirstBlood == false && battleResult.NumberOfCrushed >= 1) //When enemy first blood 
                                    {
                                        string es = null;
                                        if (battleResult.NumberOfCrushed != 1) { es = "es"; }
                                        var t = order.Actor.name + "'s " + order.SkillEffectChosen.skill.name + ". first blood! total dealt damage:" + battleResult.TotalDealtDamage.WithComma() + " " + battleResult.NumberOfCrushed.WithComma() + " crush" + es + ".";
                                        var setStatisticsReporterFirstBlood = statisticsReporterFirstBlood.FindLast(obj => obj.BattleWave == battleWave);
                                        setStatisticsReporterFirstBlood.Set(Affiliation.Enemy, order.Actor.name, battleResult.TotalDealtDamage, t);
                                        if (battleLogClass != null) { setStatisticsReporterFirstBlood.BattleLogEnemy = battleLogClass; }
                                        enemyFirstBlood = true;
                                    }

                                    if (battleEnd == false)
                                    {
                                        battleEnd = battleResult.BattleEnd;
                                        if (battleResult.IsAllyWin)
                                        {
                                            allyWinCount++; statisticsReporterWhichWins[battleWave - 1] = WhichWin.AllyWin;
                                            WhichWin = WhichWin.AllyWin;
                                        }
                                        if (battleResult.IsEnemyWin) { enemyWinCount++; statisticsReporterWhichWins[battleWave - 1] = WhichWin.EnemyWin;
                                            WhichWin = WhichWin.EnemyWin;

                                        }
                                        if (battleResult.IsDraw) { drawCount++; statisticsReporterWhichWins[battleWave - 1] = WhichWin.Draw;
                                            WhichWin = WhichWin.Draw;
                                        }
                                    }

                                    var orderStatus = new OrderStatusClass(); orderStatus.Initialize();

                                    var nestNumber = 0;
                                    //[[ SKILLS CHECK ]] Damage Control Assist trigger. Note: ActionType independent so ActionType.any!
                                    var damageControlAssistStack = SkillTriggerPossibilityCheck(null, _effects, _characters,
                                        order, orders, ActionType.Any, true, true,
                                        battleResult, order.Actor, nestNumber, environmentInfo);

                                    //[[ SKILLS CHECK ]] ReAttack skills trigger.
                                    var reAttackStack = SkillTriggerPossibilityCheck(null, _effects, _characters,
                                        order, orders, ActionType.ReAttack, false, false,
                                        battleResult, order.Actor, nestNumber, environmentInfo);

                                    //[[ SKILLS CHECK ]] Chain skills trigger.
                                    var chainStack = SkillTriggerPossibilityCheck(null, _effects, _characters,
                                        order, orders, ActionType.Chain, false, false,
                                        battleResult, order.Actor, nestNumber, environmentInfo);

                                    //[[ SKILLS CHECK ]] Counter skills trigger.
                                    var counterStack = SkillTriggerPossibilityCheck(null, _effects, _characters,
                                        order, orders, ActionType.Counter, false, false,
                                        battleResult, order.Actor, nestNumber, environmentInfo);


                                    //Push order in reverse. counter -> chain -> reAttack -> rescue
                                    //Push Counter
                                    if (counterStack != null) {
                                    }
                                    while (counterStack != null && counterStack.Count > 0) { orders.Push(counterStack.Pop()); nestNumber++; }

                                    //Push Chain
                                    if (chainStack != null) {
                                    }
                                    while (chainStack != null && chainStack.Count > 0) { orders.Push(chainStack.Pop()); nestNumber++; }

                                    //Push ReAttack
                                    if (reAttackStack != null) {
                                    }
                                    while (reAttackStack != null && reAttackStack.Count > 0) { orders.Push(reAttackStack.Pop()); nestNumber++; }

                                    //Push rescue
                                    if (damageControlAssistStack != null) { orderStatus.DamageControlAssistCount = damageControlAssistStack.Count; }
                                    while (damageControlAssistStack != null && damageControlAssistStack.Count > 0) { orders.Push(damageControlAssistStack.Pop()); nestNumber++; }


                                    //Navigation Logic
                                    var navigatorSpeechAfterMove = new NavigatorSpeechAfterMoveClass(navigatorName, order,
                                        _characters, _effects, orderStatus);

                                    var navigationLog = navigatorSpeechAfterMove.Log;
                                    if (navigationLog == null) continue;
                                    //navigationLog += new string(' ', 2) + "-------------\n";
                                    battleLog = new BattleLogClass(order.OrderCondition, null, null, navigationLog, order.Actor.affiliation);
                                    battleLogList.Add(battleLog);
                                }  // Until all Characters act.

                            } // action Phase.
                            //------------------------Footer phase------------------------
                            foreach (var character in _characters.Where(character => character.IsCrushedJustNow))
                            {
                                character.IsCrushedJustNow = false;
                            }

                            //Check wipe out and should continue the battle
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

                        //------------------------Statistics phase------------------------

                        //time over NEED statistics reporter and FIX BUG "no count"!
                        //Check wipe out and should continue the battle
                        if (battleEnd == false)
                        {
                            drawCount++; battleEnd = true;
                            WhichWin = WhichWin.Draw;

                            const string log = "Time over. \n";
                            var orderCondition = new OrderConditionClass(environmentInfo.Wave, environmentInfo.Turn, 4, 0, 0, 0);
                            var battleLog = new BattleLogClass(orderCondition, null, log, null, Affiliation.None);
                            battleLogList.Add(battleLog);
                        }

                    } //battleEnd 

                    foreach (var t in _characters)
                    {
                        t.SetPermanentStatistics(t.Statistics);
                    }
                } //Battle waves

                subLogPerWavesSets[battleWavesSet - 1] += "[Set:" + battleWavesSet + "] Battle count:" + (allyWinCount + enemyWinCount + drawCount) + " Win:" + (allyWinCount) + " lost:" + (enemyWinCount)
                                                          + " Win Ratio:" + (int)(allyWinCount / (double)(allyWinCount + enemyWinCount + drawCount) * 100)
                                                          + "% Ally:[Attack x" + Math.Round(_allyAttackMagnification, 2) + "] [Defense x" + Math.Round(_allyDefenseMagnification, 2) + "] \n";
                //statistics reporter open
                for (var battleWave = 1; battleWave <= battleWaves; battleWave++)
                {
                    var setStatisticsReporterFirstBlood = statisticsReporterFirstBlood.FindLast(obj => obj.BattleWave == battleWave);
                    setStatisticsReporterFirstBlood.WhichWin = statisticsReporterWhichWins[battleWave - 1];
                }
                var statisticsQueryAlly = statisticsReporterFirstBlood.Where(x => x.WhichWin == WhichWin.AllyWin)
                    .GroupBy(x => x.AllyCharacterName).Select(x => new { Subj = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count);
                var statisticsQueryEnemy = statisticsReporterFirstBlood.Where(x => x.WhichWin == WhichWin.EnemyWin)
                    .GroupBy(x => x.EnemyCharacterName).Select(x => new { Subj = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count);

                logPerWavesSets[battleWavesSet - 1] += "Ally info: MVP(times)\n";
                foreach (var group in statisticsQueryAlly) { logPerWavesSets[battleWavesSet - 1] += new string(' ', 2) + group.Subj + " (" + group.Count + ")."; }
                logPerWavesSets[battleWavesSet - 1] += "\n";
                if (statisticsReporterFirstBlood.FindAll(obj => obj.WhichWin == WhichWin.AllyWin).Any())
                {
//                    var bestFirstBloodAlly = statisticsReporterFirstBlood.FindAll(obj => obj.WhichWin == WhichWin.allyWin).OrderByDescending(obj => obj.AllyTotalDealtDamage).First();
                    //logPerWavesSets[battleWavesSet - 1] += "[Best shot]" + bestFirstBloodAlly.AllyContentText + " " + bestFirstBloodAlly.BattleLogAlly.OrderCondition + "\n";
                }
                logPerWavesSets[battleWavesSet - 1] += "Enemy info: MVP(times) \n";
                foreach (var group in statisticsQueryEnemy) { logPerWavesSets[battleWavesSet - 1] += new string(' ', 2) + group.Subj + " (" + group.Count + ")."; }
                logPerWavesSets[battleWavesSet - 1] += "\n";
                if (statisticsReporterFirstBlood.FindAll(obj => obj.WhichWin == WhichWin.EnemyWin).Any())
                {
                    var bestFirstBloodEnemy = statisticsReporterFirstBlood.FindAll(obj => obj.WhichWin == WhichWin.EnemyWin).OrderByDescending(obj => obj.EnemyTotalDealtDamage).First();
                    logPerWavesSets[battleWavesSet - 1] += "[Best shot]" + bestFirstBloodEnemy.EnemyContentText + " " + bestFirstBloodEnemy.BattleLogEnemy.OrderCondition + "\n";
                }
                //Characters permanent Statistics Collection
                foreach (var character in _characters) { character.PermanentStatistics.Avarage(battleWaves); } // Average Calculation
                logPerWavesSets[battleWavesSet - 1] += "Average (critical):\n";
                foreach (var character in _characters) { logPerWavesSets[battleWavesSet - 1] += new string(' ', 1) + character.name + " " + character.PermanentStatistics.AllCriticalRatio() + "\n"; }
                logPerWavesSets[battleWavesSet - 1] += "Average Skill:\n";
                foreach (var character in _characters) { logPerWavesSets[battleWavesSet - 1] += character.name + " " + character.PermanentStatistics.Skill() + "\n"; }
            } // Battle waves set

            //Battle is over.

            // last turn battle log 
            text = new FuncBattleConditionsText(totalTurn + 1, _characters);

            var orderConditionFinal = new OrderConditionClass(currentBattleWaves, totalTurn + 1, 0, 0, 0, 0);
            var battleLogFinal = new BattleLogClass(orderConditionFinal, null, null, null, Affiliation.None)
            {
                isHeaderInfo = true,
                headerInfoText = text.FirstLine()
            };
            var copiedBattleUnitLast = (from BattleUnit character in _characters
                select character.Copy()).ToList();
            battleLogFinal.characters = copiedBattleUnitLast;
            battleLogList.Add(battleLogFinal);


            //Set ally units for next battle
            AllyBattleUnitsList = battleLogFinal.characters.FindAll(unit => unit.affiliation == Affiliation.Ally);



            var battleLogDisplayList = battleLogList.FindAll(obj => obj.OrderCondition.Wave == battleWaves); // only last battle Log displayed.

            finalLog += "Battle is over. " + WhichWin + "\n";
            text = new FuncBattleConditionsText(totalTurn, _characters);
            finalLog += text.Text();

            for (var i = 0; i < battleWavesSets; i++) { finalLog += logPerWavesSets[i]; }
            finalLog += " Ally attack magnification per waves set: x" + (1 + allyAttackMagnificationPerWavesSet) + "\n";
            finalLog += " Ally defense magnification per waves set: x" + (1 + allyDefenseMagnificationPerWavesSet) + "\n";
            for (var i = 0; i < battleWavesSets; i++) { finalLog += subLogPerWavesSets[i]; }

            var finishDateTime = DateTime.Now;
            var processedTimeSpan = finishDateTime - startDateTime;
            finalLog += "finished:" + finishDateTime + " processed time:" + processedTimeSpan + " seed:" + seed + "\n";

            Console.WriteLine(finalLog);
            var finalOrderCondition = new OrderConditionClass(battleWaves, totalTurn, 0, 0, 0, 0);
            var finalLogList = new BattleLogClass(finalOrderCondition, null, null, finalLog, Affiliation.None);
            battleLogDisplayList.Add(finalLogList);
            LogList = battleLogDisplayList;

        }

        // Skill check method
        private static Stack<OrderClass> SkillTriggerPossibilityCheck(BattleUnit actor, List<EffectClass> effects, List<BattleUnit> characters, OrderClass attackerOrder,
            Stack<OrderClass> orders, ActionType actionType, bool shouldHeal, bool isRescue,
            BattleResultClass battleResult, BattleUnit individualTarget, int nestNumber, EnvironmentInfoClass environmentInfo)
        {
            if (attackerOrder != null && attackerOrder.IsRescue) { return null; } //If previous move is Rescue no counter, re-attack, chain and Rescue is triggered.
            List<EffectClass> rawActionTypeEffects;
            if (isRescue) // Rescue is ActionType independent
            {
                rawActionTypeEffects = effects.FindAll(obj => obj.UsageCount > 0 && obj.character.combat.hitPointCurrent > 0 &&
                                                                obj.skill.isHeal == shouldHeal && obj.VeiledFromTurn <= environmentInfo.Turn && obj.VeiledToTurn >= environmentInfo.Turn);
            }
            else
            {
                if (shouldHeal) //if heal has, be selected.
                {
                    rawActionTypeEffects = effects.FindAll(obj => obj.ActionType == actionType && obj.UsageCount > 0 && obj.character.combat.hitPointCurrent > 0 &&
                                                                    obj.skill.isHeal && obj.VeiledFromTurn <= environmentInfo.Turn && obj.VeiledToTurn >= environmentInfo.Turn);
                    if (!rawActionTypeEffects.Any()) //if no heal skill left, other move skill should be selected.
                    {
                        rawActionTypeEffects = effects.FindAll(obj => obj.ActionType == actionType && obj.UsageCount > 0 && obj.character.combat.hitPointCurrent > 0 &&
                                                                        obj.skill.isHeal == false && obj.VeiledFromTurn <= environmentInfo.Turn && obj.VeiledToTurn >= environmentInfo.Turn);
                    }
                }
                else // should not heal, so find other move skill.
                {
                    rawActionTypeEffects = effects.FindAll(obj => obj.ActionType == actionType && obj.UsageCount > 0 && obj.character.combat.hitPointCurrent > 0 &&
                                                                    obj.skill.isHeal == false && obj.VeiledFromTurn <= environmentInfo.Turn && obj.VeiledToTurn >= environmentInfo.Turn);
                }
            }
            var matchedActionTypeEffects = new List<EffectClass>();
            var counterAffiliation = Affiliation.Ally;
            if (attackerOrder != null) //Memo: at Beginning and move skills, attackOrder is null.
            {
                counterAffiliation = attackerOrder.Actor.affiliation == Affiliation.Ally ? Affiliation.Enemy : Affiliation.Ally;
            }

            var affiliationWhoWillAct = Affiliation.None;
            switch (actionType) // Get actionType dependent condition before calculation.
            {
                case ActionType.Move: //Normal moveSkill logic: only actor should trigger moveSkill.
                    matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => obj.character == actor);
                    break;
                case ActionType.Counter:
                    if (attackerOrder != null && attackerOrder.Actor.affiliation == Affiliation.Ally) { affiliationWhoWillAct = Affiliation.Enemy; }
                    else if (attackerOrder != null && attackerOrder.Actor.affiliation == Affiliation.Enemy) { affiliationWhoWillAct = Affiliation.Ally; }
                    matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => obj.character.affiliation == affiliationWhoWillAct);
                    break;
                case ActionType.Chain:
                    matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => attackerOrder != null && (obj.character.affiliation == attackerOrder.Actor.affiliation && obj.character != attackerOrder.Actor));
                    break;
                case ActionType.ReAttack:
                    matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => attackerOrder != null && obj.character == attackerOrder.Actor);
                    break;
                case ActionType.AtBeginning:
                    matchedActionTypeEffects = rawActionTypeEffects;
                    break;
                case ActionType.Any: //[Rescue skill logic]. ActionType independent so DCA is in ActionType.any.
                    if (isRescue) // Rescue skill logic
                    {
                        // Actor's affiliation character is dead just now?
                        var crushedJustNowCounterAffiliationCharacter = characters.FindAll(obj => obj.IsCrushedJustNow && obj.affiliation == counterAffiliation);
                        if (crushedJustNowCounterAffiliationCharacter.Count > 0) // Rescue required!
                        { matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => obj.character.affiliation == counterAffiliation && obj.character.feature.damageControlAssist); }
                        // in case of friendly fired.
                        var crushedJustNowByFriendlyFiredCharacter = characters.FindAll(obj => attackerOrder != null && (obj.IsCrushedJustNow && obj.affiliation == attackerOrder.Actor.affiliation));
                        if (crushedJustNowByFriendlyFiredCharacter.Count > 0)
                        { matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => attackerOrder != null && (obj.character.affiliation == attackerOrder.Actor.affiliation && obj.character.feature.damageControlAssist)); }
                    }
                    break;
                default: matchedActionTypeEffects = new List<EffectClass>(); break;
            }

            //push order from slow character's effect to fast character's effect. It means pop from fast character's effect to slow character's effect.
            matchedActionTypeEffects.Sort((x, y) => y.character.ability.responsiveness - x.character.ability.responsiveness);

            var validEffects = new List<EffectClass>();
            foreach (var effect in matchedActionTypeEffects)
            {
                effect.IsntTriggeredBecause.Initialize();
                if (effect.skill.triggerTarget.actionType != ActionType.Any)
                {
                    if ((effect.skill.triggerTarget.actionType == attackerOrder.ActionType) == false)
                    { effect.IsntTriggeredBecause.TriggerCondition = true; continue; }
                } // Trigger condition check
                if (effect.skill.actionType != ActionType.Move && effect.skill.triggerTarget.afterAllMoved == false) // check normal Attack left, except move skill. (move skill is itself so check this logic make no sense )
                {
                    var checkOrders = orders.ToList();
                    if (checkOrders.FindLast(obj => obj.Actor == effect.character && obj.SkillEffectChosen.skill.name == SkillName.NormalAttack) == null)
                    { effect.IsntTriggeredBecause.AfterAllMoved = true; continue; }// no normalAttack left, which means no action.   
                }


                if (effect.skill.actionType == ActionType.Move && effect.IsRescueAble) //Rescue Special Logic....
                {
                    // check normal Attack left, except move skill. (move skill is itself so check this logic make no sense )
                    if (effect.skill.triggerTarget.afterAllMoved == false)
                    {
                        var checkActionOrders = orders.ToList();
                        if (checkActionOrders.FindLast(obj => obj.Actor == effect.character && obj.ActionType == ActionType.Move) == null)
                        { effect.IsntTriggeredBecause.AfterAllMoved = true; continue; }// no normalAttack left, which means no action.   
                    }
                }

                if (attackerOrder != null) // only attackOrder exist, check
                {
                    if (effect.skill.triggerTarget.counter == false && attackerOrder.ActionType == ActionType.Counter)
                    { effect.IsntTriggeredBecause.TriggerTargetCounter = true; continue; } // counter reaction
                    if (effect.skill.triggerTarget.chain == false && attackerOrder.ActionType == ActionType.Chain)
                    { effect.IsntTriggeredBecause.TriggerTargetChain = true; continue; } // chain reaction
                    if (effect.skill.triggerTarget.reAttack == false && attackerOrder.ActionType == ActionType.ReAttack)
                    { effect.IsntTriggeredBecause.TriggerTargetReAttack = true; continue; } // reAttack reaction
                    if (effect.skill.triggerTarget.move == false && attackerOrder.ActionType == ActionType.Move)
                    { effect.IsntTriggeredBecause.TriggerTargetMove = true; continue; } // move skill reaction
                }


                if (effect.skill.triggerTarget.optimumRange != Range.Any && attackerOrder != null) // within OptimumRange check
                {
                    var aliveActorSide = characters.FindAll(character1 => character1.affiliation == effect.character.affiliation && character1.combat.hitPointCurrent > 0);
                    var aliveAttackerIndex = aliveActorSide.IndexOf(effect.character);
                    var minTargetOptimumRange = (int)(effect.character.combat.minRange * effect.skill.magnification.optimumRangeMin) - aliveAttackerIndex;
                    var maxTargetOptimumRange = (int)(effect.character.combat.maxRange * effect.skill.magnification.optimumRangeMax) - aliveAttackerIndex;
                    counterAffiliation = effect.character.affiliation == Affiliation.Ally ? Affiliation.Enemy : Affiliation.Ally;
                    var survivedOpponents = characters.FindAll(character1 => character1.combat.hitPointCurrent > 0 && character1.affiliation == counterAffiliation);
                    survivedOpponents.Sort((x, y) => x.uniqueId - y.uniqueId);
                    var attackerIndex = survivedOpponents.IndexOf(attackerOrder.Actor);

                    switch (effect.skill.triggerTarget.optimumRange) //Optimum Range check.
                    {
                        case Range.Any: break;
                        case Range.Within: if (attackerIndex >= minTargetOptimumRange && attackerIndex <= maxTargetOptimumRange) { break; } else { continue; }
                        case Range.Without: if (attackerIndex >= minTargetOptimumRange && attackerIndex <= maxTargetOptimumRange) { continue; } else { break; }
                    }
                }


                // AttackType MajestyAttackType NO IMPLEMENTATION.

                if (effect.skill.triggerTarget.critical != CriticalOrNot.Any)
                {
                    if (effect.skill.triggerTarget.critical == CriticalOrNot.Critical && battleResult.CriticalOrNot == CriticalOrNot.NonCritical)
                    { effect.IsntTriggeredBecause.Critical = true; continue; } // non critical but only when critical triggers
                    if (effect.skill.triggerTarget.critical == CriticalOrNot.NonCritical && battleResult.CriticalOrNot == CriticalOrNot.Critical)
                    { effect.IsntTriggeredBecause.NonCritical = true; continue; } // critical but only when non critical triggers
                }


                //ActorOrTargetUnit WhoCrushed   NO IMPLEMENTATION.

                if (effect.skill.triggerTarget.onlyWhenBeenHitMoreThanOnce && (battleResult.HitMoreThanOnceCharacters.Find(obj => obj == effect.character) == null))
                { effect.IsntTriggeredBecause.OnlyWhenBeenHitMoreThanOnce = true; continue; } //Being hit .this means not hit, so skill should not be triggered.
                if (effect.skill.triggerTarget.onlyWhenAvoidMoreThanOnce && ((battleResult.AvoidMoreThanOnceCharacters.Find(obj => obj == effect.character)) == null))
                { effect.IsntTriggeredBecause.OnlyWhenAvoidMoreThanOnce = true; continue; } //being avoid. this means not hit, so skill should not be triggered.

                switch (effect.skill.triggerBase.accumulationReference) //Trigger Accumulation check
                {
                    case ReferenceStatistics.None: break;
                    case ReferenceStatistics.AvoidCount:
                        if (effect.character.Statistics.AvoidCount < effect.NextAccumulationCount)
                        { effect.IsntTriggeredBecause.AccumulationAvoid = true; continue; }
                        break;
                    case ReferenceStatistics.AllHitCount:
                        if (effect.character.Statistics.AllHitCount < effect.NextAccumulationCount)
                        { effect.IsntTriggeredBecause.AccumulationAllHitCount = true; continue; }
                        break;
                    case ReferenceStatistics.AllTotalBeenHitCount:
                        if (effect.character.Statistics.AllTotalBeenHitCount < effect.NextAccumulationCount)
                        { effect.IsntTriggeredBecause.AccumulationAllTotalBeenHit = true; continue; }
                        break;
                    case ReferenceStatistics.CriticalBeenHitCount:
                        if (effect.character.Statistics.CriticalBeenHitCount < effect.NextAccumulationCount)
                        { effect.IsntTriggeredBecause.AccumulationCriticalBeenHit = true; continue; }
                        break;
                    case ReferenceStatistics.CriticalHitCount:
                        if (effect.character.Statistics.CriticalHitCount < effect.NextAccumulationCount)
                        { effect.IsntTriggeredBecause.AccumulationCriticalHit = true; continue; }
                        break;
                    case ReferenceStatistics.SkillBeenHitCount:
                        if (effect.character.Statistics.SkillBeenHitCount < effect.NextAccumulationCount)
                        { effect.IsntTriggeredBecause.AccumulationSkillBeenHit = true; continue; }
                        break;
                    case ReferenceStatistics.SkillHitCount:
                        if (effect.character.Statistics.SkillHitCount < effect.NextAccumulationCount)
                        { effect.IsntTriggeredBecause.AccumulationSkillHit = true; continue; }
                        break;
                }

                var possibility = environmentInfo.R.Next(0, 1000) / 1000.0; //TriggerPossibility Check
                if (effect.triggeredPossibility >= possibility) { validEffects.Add(effect); } else { effect.IsntTriggeredBecause.TriggeredPossibility = true;
                }
            }

            //set order  grouped by actors
            var skillsByOrderStack = new Stack<OrderClass>();
            foreach (var character in characters)
            {
                var validEffectsPerActor = validEffects.FindAll(obj => obj.character == character);
                if (validEffectsPerActor.Count >= 1)
                {
                    var orderNumber = 0; var nest = 0; if (attackerOrder != null) { orderNumber = attackerOrder.OrderCondition.OrderNumber; nest = attackerOrder.OrderCondition.Nest; }
                    var addCount = 0;

                    if (attackerOrder != null)
                    {
                        if (attackerOrder.ActionType == ActionType.Counter)
                        { addCount = 1; }
                        if (attackerOrder.ActionType == ActionType.Chain)
                        { addCount = 1; }
                        if (attackerOrder.ActionType == ActionType.ReAttack)
                        { addCount = 1; }

                        if (attackerOrder.ActionType == ActionType.Move)
                        {
                            //Sample dummy implement
                            addCount = 1;
                        }
                    }
                    var orderCondition = new OrderConditionClass(wave: environmentInfo.Wave, turn: environmentInfo.Turn, phase: environmentInfo.Phase, orderNumber: orderNumber,
                        nest: nest + addCount, nestOrderNumber: nestNumber);

                    var skillsByOrder = new OrderClass(orderCondition: orderCondition, actor: character, actionType: actionType, skillEffectProposed: ref validEffectsPerActor, actionSpeed: 0,
                        individualTarget: individualTarget, isRescue: isRescue);
                    skillsByOrderStack.Push(skillsByOrder); nestNumber++;
                }
            }
            if (skillsByOrderStack.Count > 0) { return skillsByOrderStack; }
            return null;
        }

        // Buff logic
        private static (string firstLine, string log) BuffDebuffFunction(OrderClass order, List<BattleUnit> characters, List<EffectClass> effects, List<SkillsMasterClass> buffMasters, int turn)
        {
            SkillsMasterClass addingBuff;
            var addingEffect = new List<EffectClass>();
            string firstLine = null;
            string log = null;
            if (order.SkillEffectChosen == null) { return (null, null); } // no effect exist, so no buff/ debuff happened
            foreach (var character in characters) { if (character.IsCrushedJustNow) { character.IsCrushedJustNow = false; } } // reset IsCrushedJustNow flag
            switch (order.SkillEffectChosen.skill.buffTarget.targetType)
            {
                case TargetType.Self: //Buff self
                    addingBuff = buffMasters.FindLast(obj => obj.name == order.SkillEffectChosen.skill.callingBuffName);
                    addingEffect.Add(new EffectClass(character: order.Actor, skill: addingBuff, actionType: ActionType.None,
                        offenseEffectMagnification: 1.0, triggeredPossibility: 0.0, isRescueAble: false, usageCount: addingBuff.usageCount,
                        veiledFromTurn: turn, veiledToTurn: (turn + addingBuff.veiledTurn)));
                    effects.Add(addingEffect[0]);
                    addingEffect[0].BuffToCharacter(currentTurn: turn);
                    order.Actor.buff.AddBarrier(addingEffect[0].skill.buffTarget.barrierRemaining);

                    string triggerPossibilityText = null;
                    if (order.SkillEffectChosen.triggeredPossibility < 1.0) { triggerPossibilityText = "(" + (int)(order.SkillEffectChosen.triggeredPossibility * 1000) / 10.0 + "%) "; }
                    string accumulationText = null;
                    if (order.SkillEffectChosen.NextAccumulationCount > 0)
                    {
                        var count = 0.0;
                        switch (order.SkillEffectChosen.skill.triggerBase.accumulationReference)
                        {
                            case ReferenceStatistics.None: break;
                            case ReferenceStatistics.AvoidCount: count = order.Actor.Statistics.AvoidCount; break;
                            case ReferenceStatistics.AllHitCount: count = order.Actor.Statistics.AllHitCount; break;
                            case ReferenceStatistics.AllTotalBeenHitCount: count = order.Actor.Statistics.AllTotalBeenHitCount; break;
                            case ReferenceStatistics.CriticalBeenHitCount: count = order.Actor.Statistics.CriticalBeenHitCount; break;
                            case ReferenceStatistics.CriticalHitCount: count = order.Actor.Statistics.CriticalHitCount; break;
                            case ReferenceStatistics.SkillBeenHitCount: count = order.Actor.Statistics.SkillBeenHitCount; break;
                            case ReferenceStatistics.SkillHitCount: count = order.Actor.Statistics.SkillHitCount; break;
                        }
                        string nextText = null;
                        if (order.SkillEffectChosen.UsageCount > 0) { nextText = " next: " + order.SkillEffectChosen.NextAccumulationCount; } else { nextText = " no usage count left."; }
                        accumulationText = "(" + order.SkillEffectChosen.skill.triggerBase.accumulationReference + ": " + count + nextText + ")";
                    }

                    firstLine = order.SkillEffectChosen.skill.name + "! " + triggerPossibilityText + accumulationText;
                    log += order.Actor.name + " gets " + addingBuff.name + " which will last " + addingBuff.veiledTurn + " turns.";
                    if (addingBuff.buffTarget.defenseMagnification > 1.0) { log += new string(' ', 4) + "[Defense: " + order.Actor.buff.DefenseMagnification + " (x" + addingBuff.buffTarget.defenseMagnification + ")] "; }
                    if (addingEffect[0].skill.buffTarget.barrierRemaining > 0) { log += "[Barrier:" + order.Actor.buff.BarrierRemaining + " (+" + addingEffect[0].skill.buffTarget.barrierRemaining + ")] "; }
                    log += "\n";
                    break;
                case TargetType.Multi: //Buff attacker's side all
                    addingBuff = buffMasters.FindLast(obj => obj.name == order.SkillEffectChosen.skill.callingBuffName);
                    var buffTargetCharacters = characters.FindAll(character1 => character1.affiliation == order.Actor.affiliation && character1.combat.hitPointCurrent > 0);
                    firstLine = new string(' ', 0) + order.Actor.name + "'s " + order.SkillEffectChosen.skill.name 
                                + "! (Trigger Possibility:" + (int)(order.SkillEffectChosen.triggeredPossibility * 1000) / 10.0 + "%) ";

                    for (var i = 0; i < buffTargetCharacters.Count; i++)
                    {
                        addingEffect.Add(new EffectClass(character: buffTargetCharacters[i], skill: addingBuff, actionType: ActionType.None,
                            offenseEffectMagnification: 1.0, triggeredPossibility: 0.0, isRescueAble: false, usageCount: addingBuff.usageCount,
                            veiledFromTurn: turn, veiledToTurn: (turn + addingBuff.veiledTurn)));
                        effects.Add(addingEffect[i]);

                        addingEffect[i].BuffToCharacter(currentTurn: turn);
                        buffTargetCharacters[i].buff.AddBarrier(addingEffect[i].skill.buffTarget.barrierRemaining);

                        log += new string(' ', 3) + buffTargetCharacters[i].name + " gets " + addingBuff.name + " which will last " + addingBuff.veiledTurn + " turns.";
                        if (addingBuff.buffTarget.defenseMagnification > 1.0)
                        { log += new string(' ', 4) + "[Defense: " + buffTargetCharacters[i].buff.DefenseMagnification + " (x" + addingBuff.buffTarget.defenseMagnification + ")] "; }
                        if (addingEffect[i].skill.buffTarget.barrierRemaining > 0)
                        { log += " [Barrier: " + buffTargetCharacters[i].buff.BarrierRemaining + " (+" + addingEffect[i].skill.buffTarget.barrierRemaining + ")] \n"; }
                    }
                    //log += "\n";
                    break;
                case TargetType.None: break;
            }

            if (order.SkillEffectChosen.skill.debuffTarget.targetType != TargetType.None)
            {
                //Debuff exist
            }
            return (firstLine, log);
        }

        private static (string firstline, string log) SkillLogicDispatcher(OrderClass order, List<BattleUnit> characters, EnvironmentInfoClass environmentInfo)
        {
            string firstLine = null;
            string log = null;
            if (order.SkillEffectChosen.skill.callSkillLogicName == CallSkillLogicName.None) { return (null, null); } // check call skill 
            SkillLogicShieldHealClass healMulti;
            switch (order.SkillEffectChosen.skill.callSkillLogicName)
            {
                case CallSkillLogicName.ShieldHealMulti:
                    healMulti = new SkillLogicShieldHealClass(order: order, characters: characters, isMulti: true, environmentInfo: environmentInfo);
                    log += healMulti.Log;
                    firstLine = healMulti.FirstLine;
                    break;
                case CallSkillLogicName.ShieldHealSingle:
                    healMulti = new SkillLogicShieldHealClass(order: order, characters: characters, isMulti: false, environmentInfo: environmentInfo);
                    log += healMulti.Log;
                    firstLine = healMulti.FirstLine;
                    break;
            }
            return (firstLine, log);
        }

        private static (BattleLogClass, BattleResultClass) SkillMoveFunction(OrderClass order, List<BattleUnit> characters, EnvironmentInfoClass environmentInfo)
        {
            var battleResult = new BattleResultClass();
            var battleLog = new BattleLogClass();

            switch (order.SkillEffectChosen.skill.magnification.attackTarget)
            {
                case TargetType.Self: break;
                case TargetType.None: break;
                default:
                    var attack = new AttackFunction(order: order, characters: characters, environmentInfo: environmentInfo);
                    battleResult = attack.BattleResult;
                    battleLog = attack.BattleLog;
                    break;
            }
            return (battleLog, battleResult);
        }

    }
} //End of MainClass


