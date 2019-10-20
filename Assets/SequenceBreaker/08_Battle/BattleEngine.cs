using System;
using System.Collections.Generic;
using System.Linq;

namespace SequenceBreaker._08_Battle
{
    public sealed class BattleEngine
    {
        //For output result
        public List<BattleLogClass> LogList;
        public WhichWin WhichWin = WhichWin.none; // get only last one
//    private string _winRatio = "[0%]"; //2019.9.22 to get win ratio to show the possibility.

        // output data for sequence battle.
        public List<BattleUnit> AllyBattleUnitsList;
//    public List<EffectClass> allySkillsList;

//    public List<BattleUnit> enemyBattleUnitsList;
//    public List<EffectClass> enemySkillsList;

        private static double _allyAttackMagnification = 1.0;
        private static double _allyDefenseMagnification = 1.0;

        private int _count;


        private readonly List<BattleUnit> _characters = new List<BattleUnit>();
        private AbilityClass[] _abilities;

        //Skill make logic , test: one character per one skill
        //SkillsMasterClass[] skillsMasters ;
        private SkillsMasterClass _normalAttackSkillMaster;
        private List<SkillsMasterClass> _buffMasters;
        //= new List<SkillsMasterClass>();

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
                allyBattleUnit.Affiliation = Affiliation.ally;
                allyBattleUnit.UniqueID = _count;

                if (isFirstWave)
                {
                    allyBattleUnit.Combat.ShieldCurrent = allyBattleUnit.Combat.ShieldMax;
                    allyBattleUnit.Combat.HitPointCurrent = allyBattleUnit.Combat.HitPointMax;
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
                enemyBattleUnit.Affiliation = Affiliation.enemy;
                enemyBattleUnit.UniqueID = _count;
                enemyBattleUnit.Combat.ShieldCurrent = enemyBattleUnit.Combat.ShieldMax;
                enemyBattleUnit.Combat.HitPointCurrent = enemyBattleUnit.Combat.HitPointMax;


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
            //string log = null;
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
                        //characters[i].Combat.ShieldCurrent = characters[i].Combat.ShieldMax;
                        //characters[i].Combat.HitPointCurrent = characters[i].Combat.HitPointMax;
                        t.Deterioration = 0.0; //Deterioration initialize to 0.0
                        t.Buff.InitializeBuff(); //Buff initialize
                        t.Buff.BarrierRemaining = 0; //Barrier initialize
                        t.Feature.InitializeFeature(); //Feature initialize
                        t.Statistics.Initialize(); // Initialise
                    }
                    //foreach (EffectClass effect in effects) { effect.InitializeAccumulation(); }


                    //effects will be set by outside of this battle engine, so effect initialize make different.
                    foreach (var effect in _effects)
                    {
                        effect.InitializeEffect();
                    }
                    //EffectInitialize(effects: effects, skillsMasters: skillsMasters, characters: characters); //Effect/Buff initialize

                    if (battleWave == battleWaves && battleWavesSet == battleWavesSets) // only last battle display inside.
                    {
                        foreach (var effect in _effects)
                        {
                            Console.WriteLine(effect.Character.Name + " has a skill: " + effect.Skill.Name + ". [possibility:" + (int)(effect.TriggeredPossibility * 1000) / 10.0 + "%]"
                                              + " Left:" + effect.UsageCount + " Offense Magnification:" + effect.OffenseEffectMagnification + " " + effect.Character.Buff.DefenseMagnification);
                        }
                    }
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
                                t.Buff.InitializeBuff();
                            }
                            foreach (var effect in _effects) { effect.BuffToCharacter(turn); }


                            for (var actionPhase = 0; actionPhase <= 3; actionPhase++)
                            {
                                //------------------------Action order routine------------------------
                                //Only alive character should be action.
                                var aliveCharacters = _characters.FindAll(character1 => character1.Combat.HitPointCurrent > 0);

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
                                            text = new FuncBattleConditionsText(turn, currentBattleWaves, _characters);
                                            //firstLine = text.FirstLine();
                                            //log += text.Text();
                                            log += null;

                                            orderCondition = new OrderConditionClass(environmentInfo.Wave,
                                                environmentInfo.Turn, environmentInfo.Phase, 0, 0, 0);
                                            battleLog = new BattleLogClass(orderCondition, false, null, firstLine, log, 1,
                                                Affiliation.none)
                                            {
                                                IsHeaderInfo = true,
                                                HeaderInfoText = text.FirstLine()
                                            };
                                            var copiedBattleUnit = (from BattleUnit character in _characters
                                                select character.Copy()).ToList();
                                            battleLog.Characters = copiedBattleUnit;
                                            battleLogList.Add(battleLog);
                                            break;
                                        }
                                        case 1:
                                        {
                                            environmentInfo.Phase = 1;
//                                            orderNumber = 0;
//                                            firstLine = "[At begging phase] \n";
//                                            orderCondition = new OrderConditionClass(environmentInfo.Wave,
//                                                environmentInfo.Turn, environmentInfo.Phase, orderNumber, 0, 0);
//                                            battleLog = new BattleLogClass(orderCondition, false, null, firstLine, null,
//                                                1,
//                                                Affiliation.none) {HeaderInfoText = "[At begging phase] \n"};
//                                            battleLogList.Add(battleLog);


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
                                            battleLog = new BattleLogClass(orderCondition, false, null, firstLine, null, 1,
                                                Affiliation.none);
                                            battleLog.HeaderInfoText = "[Main action phase] \n";
                                            //battleLogList.Add(battleLog);

                                            for (var i = 0; i <= aliveCharacters.Count - 1; i++)
                                            {
                                                var effectList = _effects.FindAll(obj =>
                                                    obj.Character == aliveCharacters[i] && obj.Skill.ActionType ==
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
                                                    (aliveCharacters[i].Ability.Responsiveness *
                                                     r.Next(40 + aliveCharacters[i].Ability.Luck, 100)), null, false));
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
                                            battleLog = new BattleLogClass(orderCondition, false, null, null, log, 1,
                                                Affiliation.none);
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
                                    if (order.Actor.Combat.HitPointCurrent <= 0) { continue; }

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
                                        order.SkillEffectChosen.NextAccumulationCount += (int)(order.SkillEffectChosen.Skill.TriggerBase.AccumulationBaseRate * order.SkillEffectChosen.Skill.TriggerBase.AccumulationWeight);
                                    }


                                    var willSkip = false;
                                    (firstLine, log) = SkillLogicDispatcher(order, _characters, environmentInfo); // SkillLogic action include damage control assist.
                                    if (firstLine != null)
                                    {
                                        battleLog = new BattleLogClass(order.OrderCondition, false, order, firstLine, log, 1, order.Actor.Affiliation);
                                        battleLogList.Add(battleLog);
                                        willSkip = true;
                                    }
                                    if (willSkip != true)
                                    {
                                        (firstLine, log) = BuffDebuffFunction(order, _characters, _effects, _buffMasters, turn); //Buff, debuff action
                                        if (firstLine != null)
                                        {
                                            battleLog = new BattleLogClass(order.OrderCondition, false, order, firstLine, log, 1, order.Actor.Affiliation);
                                            battleLogList.Add(battleLog);
                                        }
                                    }

                                    var (battleLogClass, battleResult) = SkillMoveFunction(order, _characters, environmentInfo);
                                    if (battleLogClass != null) { log = battleLogClass.Log; firstLine = battleLogClass.FirstLine; }

                                    if (log != null)
                                    {
                                        battleLog = new BattleLogClass(order.OrderCondition, false, order, firstLine, log, 1, order.Actor.Affiliation);
                                        battleLogList.Add(battleLog);
                                    }


                                    if (order.IsDamageControlAssist) //only when Damage Control Assist
                                    {
                                        var deleteOneActionOrderIfHave = orders.ToList();
                                        var deleteOneActionOrderRaw = deleteOneActionOrderIfHave.FindLast(obj => obj.Actor == order.Actor && obj.ActionType == ActionType.Move);
                                        OrderClass deleteOneActionOrder = null;
                                        foreach (var effect in deleteOneActionOrderRaw.SkillEffectProposed)
                                        { if (effect.Skill.Name == SkillName.normalAttack) { deleteOneActionOrder = deleteOneActionOrderRaw; } }
                                        deleteOneActionOrderIfHave.Remove(deleteOneActionOrder);
                                        deleteOneActionOrderIfHave.Reverse();
                                        if (deleteOneActionOrder != null) //clear stack and input again
                                        { orders.Clear(); foreach (var data in deleteOneActionOrderIfHave) { orders.Push(data); } }
                                    }
                                    //Only when first kill happened, insert statistics reporter for first blood per side.
                                    if (order.Actor.Affiliation == Affiliation.ally && allyFirstBlood == false && battleResult.NumberOfCrushed >= 1) //When ally first blood 
                                    {
                                        string es = null;
                                        if (battleResult.NumberOfCrushed != 1) { es = "es"; }
                                        var t = order.Actor.Name + "'s " + order.SkillEffectChosen.Skill.Name + ". first blood! total dealt damage:" + battleResult.TotalDeltDamage.WithComma() + " " + battleResult.NumberOfCrushed.WithComma() + " crush" + es + ".";
                                        var setStatisticsReporterFirstBlood = statisticsReporterFirstBlood.FindLast(obj => obj.BattleWave == battleWave);
                                        setStatisticsReporterFirstBlood.Set(Affiliation.ally, order.Actor.Name, order.ActionType, turn,
                                            battleResult.NumberOfCrushed, battleResult.TotalDeltDamage, t);
                                        if (battleLogClass != null) { setStatisticsReporterFirstBlood.BattleLogAlly = battleLogClass; }
                                        allyFirstBlood = true;

                                    }
                                    else if (order.Actor.Affiliation == Affiliation.enemy && enemyFirstBlood == false && battleResult.NumberOfCrushed >= 1) //When enemy first blood 
                                    {
                                        string es = null;
                                        if (battleResult.NumberOfCrushed != 1) { es = "es"; }
                                        var t = order.Actor.Name + "'s " + order.SkillEffectChosen.Skill.Name + ". first blood! total dealt damage:" + battleResult.TotalDeltDamage.WithComma() + " " + battleResult.NumberOfCrushed.WithComma() + " crush" + es + ".";
                                        var setStatisticsReporterFirstBlood = statisticsReporterFirstBlood.FindLast(obj => obj.BattleWave == battleWave);
                                        setStatisticsReporterFirstBlood.Set(Affiliation.enemy, order.Actor.Name, order.ActionType, turn,
                                            battleResult.NumberOfCrushed, battleResult.TotalDeltDamage, t);
                                        if (battleLogClass != null) { setStatisticsReporterFirstBlood.BattleLogEnemy = battleLogClass; }
                                        enemyFirstBlood = true;
                                    }

                                    if (battleEnd == false)
                                    {
                                        battleEnd = battleResult.BattleEnd;
                                        if (battleResult.IsAllyWin)
                                        {
                                            allyWinCount++; statisticsReporterWhichWins[battleWave - 1] = WhichWin.allyWin;
                                            WhichWin = WhichWin.allyWin;
                                        }
                                        if (battleResult.IsEnemyWin) { enemyWinCount++; statisticsReporterWhichWins[battleWave - 1] = WhichWin.enemyWin;
                                            WhichWin = WhichWin.enemyWin;

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


                                    //Push order in reverse. counter -> chain -> reAttack -> Damage control assist
                                    //Push Counter
                                    if (counterStack != null) { orderStatus.CounterSkillCount = counterStack.Count; }
                                    while (counterStack != null && counterStack.Count > 0) { orders.Push(counterStack.Pop()); nestNumber++; }

                                    //Push Chain
                                    if (chainStack != null) { orderStatus.ChainSkillCount = chainStack.Count; }
                                    while (chainStack != null && chainStack.Count > 0) { orders.Push(chainStack.Pop()); nestNumber++; }

                                    //Push ReAttack
                                    if (reAttackStack != null) { orderStatus.ReAttackSkillCount = reAttackStack.Count; }
                                    while (reAttackStack != null && reAttackStack.Count > 0) { orders.Push(reAttackStack.Pop()); nestNumber++; }

                                    //Push Damage Control Assist
                                    if (damageControlAssistStack != null) { orderStatus.DamageControlAssistCount = damageControlAssistStack.Count; }
                                    while (damageControlAssistStack != null && damageControlAssistStack.Count > 0) { orders.Push(damageControlAssistStack.Pop()); nestNumber++; }


                                    //Navigation Logic
                                    var navigatorSpeechAfterMove = new NavigatorSpeechAfterMoveClass(navigatorName, order,
                                        _characters, _effects, orderStatus, environmentInfo);

                                    var navigationLog = navigatorSpeechAfterMove.Log;
                                    if (navigationLog == null) continue;
                                    //navigationLog += new string(' ', 2) + "-------------\n";
                                    battleLog = new BattleLogClass(order.OrderCondition, true, null, null, navigationLog, 1, order.Actor.Affiliation);
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
                                if (wipeOutCheck.IsAllyWin) { allyWinCount++; WhichWin = WhichWin.allyWin; }
                                if (wipeOutCheck.IsEnemyWin) { enemyWinCount++; WhichWin = WhichWin.enemyWin; }
                                if (wipeOutCheck.IsDraw) { drawCount++; WhichWin = WhichWin.Draw; }
                                battleEnd = wipeOutCheck.BatleEnd;
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
                            var battleLog = new BattleLogClass(orderCondition, false, null, log, null, 1, Affiliation.none);
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
                var statisticsQueryAlly = statisticsReporterFirstBlood.Where(x => x.WhichWin == WhichWin.allyWin)
                    .GroupBy(x => x.AllyCharacterName).Select(x => new { Subj = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count);
                var statisticsQueryEnemy = statisticsReporterFirstBlood.Where(x => x.WhichWin == WhichWin.enemyWin)
                    .GroupBy(x => x.EnemyCharacterName).Select(x => new { Subj = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count);

                logPerWavesSets[battleWavesSet - 1] += "Ally info: MVP(times)\n";
                foreach (var group in statisticsQueryAlly) { logPerWavesSets[battleWavesSet - 1] += new string(' ', 2) + group.Subj + " (" + group.Count + ")."; }
                logPerWavesSets[battleWavesSet - 1] += "\n";
                if (statisticsReporterFirstBlood.FindAll(obj => obj.WhichWin == WhichWin.allyWin).Any())
                {
//                    var bestFirstBloodAlly = statisticsReporterFirstBlood.FindAll(obj => obj.WhichWin == WhichWin.allyWin).OrderByDescending(obj => obj.AllyTotalDealtDamage).First();
                    //logPerWavesSets[battleWavesSet - 1] += "[Best shot]" + bestFirstBloodAlly.AllyContentText + " " + bestFirstBloodAlly.BattleLogAlly.OrderCondition + "\n";
                }
                logPerWavesSets[battleWavesSet - 1] += "Enemy info: MVP(times) \n";
                foreach (var group in statisticsQueryEnemy) { logPerWavesSets[battleWavesSet - 1] += new string(' ', 2) + group.Subj + " (" + group.Count + ")."; }
                logPerWavesSets[battleWavesSet - 1] += "\n";
                if (statisticsReporterFirstBlood.FindAll(obj => obj.WhichWin == WhichWin.enemyWin).Any())
                {
                    var bestFirstBloodEnemy = statisticsReporterFirstBlood.FindAll(obj => obj.WhichWin == WhichWin.enemyWin).OrderByDescending(obj => obj.EnemyTotalDealtDamage).First();
                    logPerWavesSets[battleWavesSet - 1] += "[Best shot]" + bestFirstBloodEnemy.EnemyContentText + " " + bestFirstBloodEnemy.BattleLogEnemy.OrderCondition + "\n";
                }
                //Characters permanent Statistics Collection
                foreach (var character in _characters) { character.PermanentStatistics.Avarage(battleWaves); } // Average Calculation
                logPerWavesSets[battleWavesSet - 1] += "Average (critical):\n";
                foreach (var character in _characters) { logPerWavesSets[battleWavesSet - 1] += new string(' ', 1) + character.Name + " " + character.PermanentStatistics.AllCriticalRatio() + "\n"; }
                logPerWavesSets[battleWavesSet - 1] += "Average Skill:\n";
                foreach (var character in _characters) { logPerWavesSets[battleWavesSet - 1] += character.Name + " " + character.PermanentStatistics.Skill() + "\n"; }
                //logPerWavesSets[battleWavesSet - 1] += "------------------------------------\n";

            } // Battle waves set

            //Battle is over.

            // last turn battle log 
            //environmentInfo.Phase = 0;
            //log += "------------------------------------\n";
            text = new FuncBattleConditionsText(totalTurn + 1, currentBattleWaves, _characters);
            //firstLine = text.FirstLine();
            //log += text.Text();
            //string log = null;

            var orderConditionFinal = new OrderConditionClass(currentBattleWaves, totalTurn + 1, 0, 0, 0, 0);
            var battleLogFinal = new BattleLogClass(orderConditionFinal, false, null, null, null, 1, Affiliation.none)
            {
                IsHeaderInfo = true,
                HeaderInfoText = text.FirstLine()
            };
            var copiedBattleUnitLast = (from BattleUnit character in _characters
                select character.Copy()).ToList();
            battleLogFinal.Characters = copiedBattleUnitLast;
            battleLogList.Add(battleLogFinal);


            //Set ally units for next battle
            AllyBattleUnitsList = battleLogFinal.Characters.FindAll(unit => unit.Affiliation == Affiliation.ally);



            var battleLogDisplayList = battleLogList.FindAll(obj => obj.OrderCondition.Wave == battleWaves); // only last battle Log displayed.
            //foreach (BattleLogClass battleLog in battleLogDisplayList)
            //{

            //    if (battleLog.IsNavigation == false)
            //    { finalLog += "(" + battleLog.OrderCondition.Phase + "-" + battleLog.OrderCondition.OrderNumber + "-" + battleLog.OrderCondition.Nest + "-" + battleLog.OrderCondition.NestOrderNumber + ")" + battleLog.Log; }
            //    else { finalLog += new string(' ', 5) + battleLog.Log; }
            //}
            //// delete battle display list to final log, because now meaningless.

            //finalLog += "------------------------------------\n";
            finalLog += "Battle is over. " + WhichWin + "\n";
            text = new FuncBattleConditionsText(totalTurn, currentBattleWaves, _characters);
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
            var finalLogList = new BattleLogClass(finalOrderCondition, true, null, null, finalLog,
                1, Affiliation.none);
            battleLogDisplayList.Add(finalLogList);
            LogList = battleLogDisplayList;

            //Set win ratio
//        _winRatio = "[" + (int)(100 * allyWinCount / battleWaves) + "%]";
 
   

        }

        // Skill check method
        private static Stack<OrderClass> SkillTriggerPossibilityCheck(BattleUnit actor, List<EffectClass> effects, List<BattleUnit> characters, OrderClass attackerOrder,
            Stack<OrderClass> orders, ActionType actionType, bool shouldHeal, bool isDamageControlAssist,
            BattleResultClass battleResult, BattleUnit individualTarget, int nestNumber, EnvironmentInfoClass environmentInfo)
        {
            if (attackerOrder != null && attackerOrder.IsDamageControlAssist) { return null; } //If previous move is Damage Control Assist, no counter, re-attack, chain and Damage control assist is triggered.
            List<EffectClass> rawActionTypeEffects;
            if (isDamageControlAssist) // Damage control assist is ActionType independent
            {
                rawActionTypeEffects = effects.FindAll(obj => obj.UsageCount > 0 && obj.Character.Combat.HitPointCurrent > 0 &&
                                                                obj.Skill.IsHeal == shouldHeal && obj.VeiledFromTurn <= environmentInfo.Turn && obj.VeiledToTurn >= environmentInfo.Turn);
            }
            else
            {
                if (shouldHeal) //if heal has, be selected.
                {
                    rawActionTypeEffects = effects.FindAll(obj => obj.ActionType == actionType && obj.UsageCount > 0 && obj.Character.Combat.HitPointCurrent > 0 &&
                                                                    obj.Skill.IsHeal && obj.VeiledFromTurn <= environmentInfo.Turn && obj.VeiledToTurn >= environmentInfo.Turn);
                    if (!rawActionTypeEffects.Any()) //if no heal skill left, other move skill should be selected.
                    {
                        rawActionTypeEffects = effects.FindAll(obj => obj.ActionType == actionType && obj.UsageCount > 0 && obj.Character.Combat.HitPointCurrent > 0 &&
                                                                        obj.Skill.IsHeal == false && obj.VeiledFromTurn <= environmentInfo.Turn && obj.VeiledToTurn >= environmentInfo.Turn);
                    }
                }
                else // should not heal, so find other move skill.
                {
                    rawActionTypeEffects = effects.FindAll(obj => obj.ActionType == actionType && obj.UsageCount > 0 && obj.Character.Combat.HitPointCurrent > 0 &&
                                                                    obj.Skill.IsHeal == false && obj.VeiledFromTurn <= environmentInfo.Turn && obj.VeiledToTurn >= environmentInfo.Turn);
                }
            }
            var matchedActionTypeEffects = new List<EffectClass>();
            var counterAffiliation = Affiliation.ally;
            if (attackerOrder != null) //Memo: at Beginning and move skills, attackOrder is null.
            {
                counterAffiliation = attackerOrder.Actor.Affiliation == Affiliation.ally ? Affiliation.enemy : Affiliation.ally;
            }

            var affiliationWhoWillAct = Affiliation.none;
            switch (actionType) // Get actionType dependent condition before calculation.
            {
                case ActionType.Move: //Normal moveSkill logic: only actor should trigger moveSkill.
                    matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => obj.Character == actor);
                    break;
                case ActionType.Counter:
                    if (attackerOrder != null && attackerOrder.Actor.Affiliation == Affiliation.ally) { affiliationWhoWillAct = Affiliation.enemy; }
                    else if (attackerOrder != null && attackerOrder.Actor.Affiliation == Affiliation.enemy) { affiliationWhoWillAct = Affiliation.ally; }
                    matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => obj.Character.Affiliation == affiliationWhoWillAct);
                    break;
                case ActionType.Chain:
                    matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => attackerOrder != null && (obj.Character.Affiliation == attackerOrder.Actor.Affiliation && obj.Character != attackerOrder.Actor));
                    break;
                case ActionType.ReAttack:
                    matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => attackerOrder != null && obj.Character == attackerOrder.Actor);
                    break;
                case ActionType.AtBeginning:
                    matchedActionTypeEffects = rawActionTypeEffects;
                    break;
                case ActionType.Any: //[Damage Control Assist skill logic]. ActionType independent so DCA is in ActionType.any.
                    if (isDamageControlAssist) // Damage Control Assist skill logic
                    {
                        // Actor's affiliation character is dead just now?
                        var crushedJustNowCounterAffiliationCharacter = characters.FindAll(obj => obj.IsCrushedJustNow && obj.Affiliation == counterAffiliation);
                        if (crushedJustNowCounterAffiliationCharacter.Count > 0) // Damage Control Assist required!
                        { matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => obj.Character.Affiliation == counterAffiliation && obj.Character.Feature.DamageControlAssist); }
                        // in case of friendly fired.
                        var crushedJustNowByFriendlyFiredCharacter = characters.FindAll(obj => attackerOrder != null && (obj.IsCrushedJustNow && obj.Affiliation == attackerOrder.Actor.Affiliation));
                        if (crushedJustNowByFriendlyFiredCharacter.Count > 0)
                        { matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => attackerOrder != null && (obj.Character.Affiliation == attackerOrder.Actor.Affiliation && obj.Character.Feature.DamageControlAssist)); }
                    }
                    break;
                default: matchedActionTypeEffects = new List<EffectClass>(); break;
            }

            //push order from slow character's effect to fast character's effect. It means pop from fast character's effect to slow character's effect.
            matchedActionTypeEffects.Sort((x, y) => y.Character.Ability.Responsiveness - x.Character.Ability.Responsiveness);

            var validEffects = new List<EffectClass>();
            foreach (var effect in matchedActionTypeEffects)
            {
                effect.IsntTriggeredBecause.Initialize();
                if (effect.Skill.TriggerTarget.ActionType != ActionType.Any)
                {
                    if ((effect.Skill.TriggerTarget.ActionType == attackerOrder.ActionType) == false)
                    { effect.IsntTriggeredBecause.TriggerCondition = true; continue; }
                } // Trigger condition check
                if (effect.Skill.ActionType != ActionType.Move && effect.Skill.TriggerTarget.AfterAllMoved == false) // check normal Attack left, except move skill. (move skill is itself so check this logic make no sense )
                {
                    var checkOrders = orders.ToList();
                    if (checkOrders.FindLast(obj => obj.Actor == effect.Character && obj.SkillEffectChosen.Skill.Name == SkillName.normalAttack) == null)
                    { effect.IsntTriggeredBecause.AfterAllMoved = true; continue; }// no normalAttack left, which means no action.   
                }


                if (effect.Skill.ActionType == ActionType.Move && effect.IsDamageControlAssistAble) //Damage Control Assist Special Logic....
                {
                    // check normal Attack left, except move skill. (move skill is itself so check this logic make no sense )
                    if (effect.Skill.TriggerTarget.AfterAllMoved == false)
                    {
                        var checkActionOrders = orders.ToList();
                        if (checkActionOrders.FindLast(obj => obj.Actor == effect.Character && obj.ActionType == ActionType.Move) == null)
                        { effect.IsntTriggeredBecause.AfterAllMoved = true; continue; }// no normalAttack left, which means no action.   
                    }
                }

                if (attackerOrder != null) // only attackOrder exist, check
                {
                    if (effect.Skill.TriggerTarget.Counter == false && attackerOrder.ActionType == ActionType.Counter)
                    { effect.IsntTriggeredBecause.TriggerTargetCounter = true; continue; } // counter reaction
                    if (effect.Skill.TriggerTarget.Chain == false && attackerOrder.ActionType == ActionType.Chain)
                    { effect.IsntTriggeredBecause.TriggerTargetChain = true; continue; } // chain reaction
                    if (effect.Skill.TriggerTarget.ReAttack == false && attackerOrder.ActionType == ActionType.ReAttack)
                    { effect.IsntTriggeredBecause.TriggerTargetReAttack = true; continue; } // reAttack reaction
                    if (effect.Skill.TriggerTarget.Move == false && attackerOrder.ActionType == ActionType.Move)
                    { effect.IsntTriggeredBecause.TriggerTargetMove = true; continue; } // move skill reaction
                }


                if (effect.Skill.TriggerTarget.OptimumRange != Range.any && attackerOrder != null) // within OptimumRange check
                {
                    var aliveActorSide = characters.FindAll(character1 => character1.Affiliation == effect.Character.Affiliation && character1.Combat.HitPointCurrent > 0);
                    var aliveAttackerIndex = aliveActorSide.IndexOf(effect.Character);
                    var minTargetOptimumRange = (int)(effect.Character.Combat.MinRange * effect.Skill.Magnification.OptimumRangeMin) - aliveAttackerIndex;
                    var maxTargetOptimumRange = (int)(effect.Character.Combat.MaxRange * effect.Skill.Magnification.OptimumRangeMax) - aliveAttackerIndex;
                    counterAffiliation = effect.Character.Affiliation == Affiliation.ally ? Affiliation.enemy : Affiliation.ally;
                    var survivedOpponents = characters.FindAll(character1 => character1.Combat.HitPointCurrent > 0 && character1.Affiliation == counterAffiliation);
                    survivedOpponents.Sort((x, y) => x.UniqueID - y.UniqueID);
                    var attackerIndex = survivedOpponents.IndexOf(attackerOrder.Actor);

                    switch (effect.Skill.TriggerTarget.OptimumRange) //Optimum Range check.
                    {
                        case Range.any: break;
                        case Range.within: if (attackerIndex >= minTargetOptimumRange && attackerIndex <= maxTargetOptimumRange) { break; } else { continue; }
                        case Range.without: if (attackerIndex >= minTargetOptimumRange && attackerIndex <= maxTargetOptimumRange) { continue; } else { break; }
                    }
                }


                // AttackType MajestyAttackType NO IMPLEMENTATION.

                if (effect.Skill.TriggerTarget.Critical != CriticalOrNot.any)
                {
                    if (effect.Skill.TriggerTarget.Critical == CriticalOrNot.critical && battleResult.CriticalOrNot == CriticalOrNot.nonCritical)
                    { effect.IsntTriggeredBecause.Critical = true; continue; } // non critical but only when critical triggers
                    if (effect.Skill.TriggerTarget.Critical == CriticalOrNot.nonCritical && battleResult.CriticalOrNot == CriticalOrNot.critical)
                    { effect.IsntTriggeredBecause.NonCritical = true; continue; } // critical but only when non critical triggers
                }


                //ActorOrTargetUnit WhoCrushed   NO IMPLEMENTATION.

                if (effect.Skill.TriggerTarget.OnlyWhenBeenHitMoreThanOnce && (battleResult.HitMoreThanOnceCharacters.Find(obj => obj == effect.Character) == null))
                { effect.IsntTriggeredBecause.OnlyWhenBeenHitMoreThanOnce = true; continue; } //Being hit .this means not hit, so skill should not be triggered.
                if (effect.Skill.TriggerTarget.OnlyWhenAvoidMoreThanOnce && ((battleResult.AvoidMoreThanOnceCharacters.Find(obj => obj == effect.Character)) == null))
                { effect.IsntTriggeredBecause.OnlyWhenAvoidMoreThanOnce = true; continue; } //being avoid. this means not hit, so skill should not be triggered.

                switch (effect.Skill.TriggerBase.AccumulationReference) //Trigger Accumulation check
                {
                    case ReferenceStatistics.none: break;
                    case ReferenceStatistics.AvoidCount:
                        if (effect.Character.Statistics.AvoidCount < effect.NextAccumulationCount)
                        { effect.IsntTriggeredBecause.AccumulationAvoid = true; continue; }
                        break;
                    case ReferenceStatistics.AllHitCount:
                        if (effect.Character.Statistics.AllHitCount < effect.NextAccumulationCount)
                        { effect.IsntTriggeredBecause.AccumulationAllHitCount = true; continue; }
                        break;
                    case ReferenceStatistics.AllTotalBeenHitCount:
                        if (effect.Character.Statistics.AllTotalBeenHitCount < effect.NextAccumulationCount)
                        { effect.IsntTriggeredBecause.AccumulationAllTotalBeenHit = true; continue; }
                        break;
                    case ReferenceStatistics.CriticalBeenHitCount:
                        if (effect.Character.Statistics.CriticalBeenHitCount < effect.NextAccumulationCount)
                        { effect.IsntTriggeredBecause.AccumulationCriticalBeenHit = true; continue; }
                        break;
                    case ReferenceStatistics.CriticalHitCount:
                        if (effect.Character.Statistics.CriticalHitCount < effect.NextAccumulationCount)
                        { effect.IsntTriggeredBecause.AccumulationCriticalHit = true; continue; }
                        break;
                    case ReferenceStatistics.SkillBeenHitCount:
                        if (effect.Character.Statistics.SkillBeenHitCount < effect.NextAccumulationCount)
                        { effect.IsntTriggeredBecause.AccumulationSkillBeenHit = true; continue; }
                        break;
                    case ReferenceStatistics.SkillHitCount:
                        if (effect.Character.Statistics.SkillHitCount < effect.NextAccumulationCount)
                        { effect.IsntTriggeredBecause.AccumulationSkillHit = true; continue; }
                        break;
                }

                var possibility = environmentInfo.R.Next(0, 1000) / 1000.0; //TriggerPossibility Check
                if (effect.TriggeredPossibility >= possibility) { validEffects.Add(effect); } else { effect.IsntTriggeredBecause.TriggeredPossibility = true;
                }
            }

            //set order  grouped by actors
            var skillsByOrderStack = new Stack<OrderClass>();
            foreach (var character in characters)
            {
                var validEffectsPerActor = validEffects.FindAll(obj => obj.Character == character);
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
                        individualTarget: individualTarget, isDamageControlAssist: isDamageControlAssist);
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
            switch (order.SkillEffectChosen.Skill.BuffTarget.TargetType)
            {
                case TargetType.self: //Buff self
                    addingBuff = buffMasters.FindLast(obj => obj.Name == order.SkillEffectChosen.Skill.CallingBuffName);
                    addingEffect.Add(new EffectClass(character: order.Actor, skill: addingBuff, actionType: ActionType.None,
                        offenseEffectMagnification: 1.0, triggeredPossibility: 0.0, isDamageControlAssistAble: false, usageCount: addingBuff.UsageCount,
                        veiledFromTurn: turn, veiledToTurn: (turn + addingBuff.VeiledTurn)));
                    effects.Add(addingEffect[0]);
                    addingEffect[0].BuffToCharacter(currentTurn: turn);
                    order.Actor.Buff.AddBarrier(addingEffect[0].Skill.BuffTarget.BarrierRemaining);

                    string triggerPossibilityText = null;
                    if (order.SkillEffectChosen.TriggeredPossibility < 1.0) { triggerPossibilityText = "(" + (int)(order.SkillEffectChosen.TriggeredPossibility * 1000) / 10.0 + "%) "; }
                    string accumulationText = null;
                    if (order.SkillEffectChosen.NextAccumulationCount > 0)
                    {
                        var count = 0.0;
                        switch (order.SkillEffectChosen.Skill.TriggerBase.AccumulationReference)
                        {
                            case ReferenceStatistics.none: break;
                            case ReferenceStatistics.AvoidCount: count = order.Actor.Statistics.AvoidCount; break;
                            case ReferenceStatistics.AllHitCount: count = order.Actor.Statistics.AllHitCount; break;
                            case ReferenceStatistics.AllTotalBeenHitCount: count = order.Actor.Statistics.AllTotalBeenHitCount; break;
                            case ReferenceStatistics.CriticalBeenHitCount: count = order.Actor.Statistics.CriticalBeenHitCount; break;
                            case ReferenceStatistics.CriticalHitCount: count = order.Actor.Statistics.CriticalHitCount; break;
                            case ReferenceStatistics.SkillBeenHitCount: count = order.Actor.Statistics.SkillBeenHitCount; break;
                            case ReferenceStatistics.SkillHitCount: count = order.Actor.Statistics.SkillHitCount; break;
                        }
                        string nextText = null;
                        if (order.SkillEffectChosen.UsageCount > 0) { nextText = " next trigger: " + order.SkillEffectChosen.NextAccumulationCount; } else { nextText = " no usage count left."; }
                        accumulationText = "(Accumulation " + order.SkillEffectChosen.Skill.TriggerBase.AccumulationReference + ": " + count + nextText + ")";
                    }

                    firstLine = order.SkillEffectChosen.Skill.Name + "! " + triggerPossibilityText + accumulationText + "\n"
                                + new string(' ', 3) + order.Actor.Name + " gets " + addingBuff.Name + " which will last " + addingBuff.VeiledTurn + " turns. " + "\n";

                    //firstLine = new string(' ', 0) + order.Actor.Name + "'s " + order.SkillEffectChosen.Skill.Name + "! " + triggerPossibilityText + accumulationText + "\n"
                    //+ new string(' ', 3) + order.Actor.Name + " gets " + addingBuff.Name + " which will last " + addingBuff.VeiledTurn + " turns. " + "\n";
                    // Belows: It's a temporary message, only for defense magnification.
                    if (addingBuff.BuffTarget.DefenseMagnification > 1.0) { log += new string(' ', 4) + "[Defense: " + order.Actor.Buff.DefenseMagnification + " (x" + addingBuff.BuffTarget.DefenseMagnification + ")] "; }
                    if (addingEffect[0].Skill.BuffTarget.BarrierRemaining > 0) { log += "[Barrier:" + order.Actor.Buff.BarrierRemaining + " (+" + addingEffect[0].Skill.BuffTarget.BarrierRemaining + ")] "; }
                    log += "\n";
                    break;
                case TargetType.multi: //Buff attacker's side all
                    addingBuff = buffMasters.FindLast(obj => obj.Name == order.SkillEffectChosen.Skill.CallingBuffName);
                    var buffTargetCharacters = characters.FindAll(character1 => character1.Affiliation == order.Actor.Affiliation && character1.Combat.HitPointCurrent > 0);
                    firstLine = new string(' ', 0) + order.Actor.Name + "'s " + order.SkillEffectChosen.Skill.Name + "! (Trigger Possibility:" + (int)(order.SkillEffectChosen.TriggeredPossibility * 1000) / 10.0 + "%) \n";

                    for (var i = 0; i < buffTargetCharacters.Count; i++)
                    {
                        addingEffect.Add(new EffectClass(character: buffTargetCharacters[i], skill: addingBuff, actionType: ActionType.None,
                            offenseEffectMagnification: 1.0, triggeredPossibility: 0.0, isDamageControlAssistAble: false, usageCount: addingBuff.UsageCount,
                            veiledFromTurn: turn, veiledToTurn: (turn + addingBuff.VeiledTurn)));
                        effects.Add(addingEffect[i]);

                        addingEffect[i].BuffToCharacter(currentTurn: turn);
                        buffTargetCharacters[i].Buff.AddBarrier(addingEffect[i].Skill.BuffTarget.BarrierRemaining);

                        log += new string(' ', 3) + buffTargetCharacters[i].Name + " gets " + addingBuff.Name + " which will last " + addingBuff.VeiledTurn + " turns.";
                        if (addingBuff.BuffTarget.DefenseMagnification > 1.0)
                        { log += new string(' ', 4) + "[Defense: " + buffTargetCharacters[i].Buff.DefenseMagnification + " (x" + addingBuff.BuffTarget.DefenseMagnification + ")] "; }
                        if (addingEffect[i].Skill.BuffTarget.BarrierRemaining > 0)
                        { log += " [Barrier: " + buffTargetCharacters[i].Buff.BarrierRemaining + " (+" + addingEffect[i].Skill.BuffTarget.BarrierRemaining + ")] \n"; }
                    }
                    //log += "\n";
                    break;
                case TargetType.none: break;
            }

            if (order.SkillEffectChosen.Skill.DebuffTarget.TargetType != TargetType.none)
            {
                //Debuff exist
            }
            return (firstLine, log);
        }

        private static (string firstline, string log) SkillLogicDispatcher(OrderClass order, List<BattleUnit> characters, EnvironmentInfoClass environmentInfo)
        {
            string firstLine = null;
            string log = null;
            if (order.SkillEffectChosen.Skill.CallSkillLogicName == CallSkillLogicName.none) { return (null, null); } // check call skill 
            SkillLogicShieldHealClass healMulti;
            switch (order.SkillEffectChosen.Skill.CallSkillLogicName)
            {
                case CallSkillLogicName.ShieldHealMulti:
                    healMulti = new SkillLogicShieldHealClass(order: order, characters: characters, isMulti: true, environmentInfo: environmentInfo);
                    log += healMulti.Log;
                    firstLine = healMulti.firstLine;
                    break;
                case CallSkillLogicName.ShieldHealSingle:
                    healMulti = new SkillLogicShieldHealClass(order: order, characters: characters, isMulti: false, environmentInfo: environmentInfo);
                    log += healMulti.Log;
                    firstLine = healMulti.firstLine;
                    break;
            }
            return (firstLine, log);
        }

        private static (BattleLogClass, BattleResultClass) SkillMoveFunction(OrderClass order, List<BattleUnit> characters, EnvironmentInfoClass environmentInfo)
        {
            var battleResult = new BattleResultClass();
            var battleLog = new BattleLogClass();

            switch (order.SkillEffectChosen.Skill.Magnification.AttackTarget)
            {
                case TargetType.self: break;
                case TargetType.none: break;
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


