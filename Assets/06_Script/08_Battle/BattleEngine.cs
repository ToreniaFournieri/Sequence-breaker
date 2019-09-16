using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BattleEngine
{
    //For output result
    public List<BattleLogClass> logList;
    public WhichWin whichWin = WhichWin.none; // get only last one

    public List<BattleUnit> allyBattleUnitsList;
    public List<EffectClass> allySkillsList;

    public List<BattleUnit> enemyBattleUnitsList;
    public List<EffectClass> enemySkillsList;
    private static int _numberOfCharacters = 14;
    private static double _allyAttackMagnification = 1.0;
    private static double _allyDefenseMagnification = 1.0;

    private int _count = 0;


    private List<BattleUnit> characters = new List<BattleUnit>();
    AbilityClass[] abilities;

    //Skill make logic , test: one character per one skill
    //SkillsMasterClass[] skillsMasters ;
    private SkillsMasterClass _normalAttackSkillMaster;
    private List<SkillsMasterClass> _buffMasters;
    //= new List<SkillsMasterClass>();

    //Effect (permament skill) make logic, the effect has two meanings, one is permanent skill, the other is temporary skill (may call buff).
    List<EffectClass> effects = new List<EffectClass>();

    //Set up a battle environment.
    public void SetUpEnvironment(SkillsMasterClass normalAttackSkillMaster, List<SkillsMasterClass> buffMasters)
    {
        _normalAttackSkillMaster = normalAttackSkillMaster;
        _buffMasters = buffMasters;
    }


    //Set up Ally's BattleUnit and EffectClass.
    public void SetAllyBattleUnits(List<BattleUnit> allyBattleUnits, List<EffectClass> allyEffectClasses)
    {
        foreach (BattleUnit allyBattleUnit in allyBattleUnits)
        {
            // Forced update Affiliation to ally(not worked?)
            allyBattleUnit.Affiliation = Affiliation.ally;
            allyBattleUnit.UniqueID = _count;

            characters.Add(allyBattleUnit);
            _count++;
        }

        foreach (EffectClass allyEffectClass in allyEffectClasses)
        {
            effects.Add(allyEffectClass);
        }

    }

    public void SetEnemyBattleUnits(List<BattleUnit> enemyBattleUnits, List<EffectClass> enemyEffectClasses)
    {
        foreach (BattleUnit enemyBattleUnit in enemyBattleUnits)
        {
            // Forced update Affiliation to enemy (not worked?)
            enemyBattleUnit.Affiliation = Affiliation.enemy;
            enemyBattleUnit.UniqueID = _count;

            characters.Add(enemyBattleUnit);
            _count++;
        }

        foreach (EffectClass enemyEffectClass in enemyEffectClasses)
        {
            effects.Add(enemyEffectClass);
        }
    }



    public void Battle()
    {
        DateTime startDateTime = DateTime.Now;
        Console.WriteLine("start:" + startDateTime);
        // Battle environment setting
        int battleWavesSets = 1;
        int battleWaves = 1; // one set of battle 
        string navigatorName = "Navigator";

        //BattleWaveSet variables
        double allyAttackMagnificationPerWavesSet = 0.0;
        double allyDefenseMagnificationPerWavesSet = 0.0;

        bool battleEnd = false;
        FuncBattleConditionsText text = null;
        WipeOutCheck wipeOutCheck = null;
        int seed = (int)DateTime.Now.Ticks; // when you find something wrong, use seed value to Reproduction the situation
                                            //seed = 1086664070; // Reproduction.
        System.Random r = new System.Random(seed);

        // System initialize : Do not change them.
        List<BattleLogClass> battleLogList = new List<BattleLogClass>();
        //string log = null;
        string finalLog = null;
        string[] logPerWavesSets = new string[battleWavesSets];
        string[] subLogPerWavesSets = new string[battleWavesSets];
        int totalturn = 0; // Static info of total turn
        int currentBattleWaves = 1;
        int allyWinCount = 0;
        int enemyWinCount = 0;
        int drawCount = 0;



        //------------------------Battle Main Engine------------------------
        //Logic: Number of Battle
        for (int battleWavesSet = 1; battleWavesSet <= battleWavesSets; battleWavesSet++)
        {
            //set up set info
            List<StatisticsReporterFirstBloodClass> statisticsReporterFirstBlood = new List<StatisticsReporterFirstBloodClass>();
            WhichWin[] statisticsReporterWhichWins = new WhichWin[battleWaves];

            if (battleWavesSet > 1) //  magnification per wave set
            { _allyAttackMagnification *= (1 + allyAttackMagnificationPerWavesSet); _allyDefenseMagnification *= (1 + allyDefenseMagnificationPerWavesSet); }



            allyWinCount = 0; enemyWinCount = 0; drawCount = 0; // Initialize

            for (int battleWave = 1; battleWave <= battleWaves; battleWave++)
            {
                bool allyFirstBlood = false;
                bool enemyFirstBlood = false; // Set up Phase
                statisticsReporterFirstBlood.Add(new StatisticsReporterFirstBloodClass(battleWave: battleWave));

                for (int i = 0; i < characters.Count; i++) //Shield, HitPoint initialize
                {
                    characters[i].Combat.ShieldCurrent = characters[i].Combat.ShieldMax;
                    characters[i].Combat.HitPointCurrent = characters[i].Combat.HitPointMax;
                    characters[i].Deterioration = 0.0; //Deterioration initialize to 0.0
                    characters[i].Buff.InitializeBuff(); //Buff initialize
                    characters[i].Buff.BarrierRemaining = 0; //Barrier initialize
                    characters[i].Feature.InitializeFeature(); //Feature initialize
                    characters[i].Statistics.Initialize(); // Initialise
                }
                //foreach (EffectClass effect in effects) { effect.InitializeAccumulation(); }


                //effects will be set by outside of this battle engine, so effect initialize make defferent.
                foreach (EffectClass effect in effects)
                {
                    effect.InitializeEffect();
                }
                //EffectInitialize(effects: effects, skillsMasters: skillsMasters, characters: characters); //Effect/Buff initialize

                if (battleWave == battleWaves && battleWavesSet == battleWavesSets) // only last battle display inside.
                {
                    foreach (EffectClass effect in effects)
                    {
                        Console.WriteLine(effect.Character.Name + " has a skill: " + effect.Skill.Name + ". [possibility:" + (double)((int)(effect.TriggeredPossibility * 1000) / 10.0) + "%]"
                        + " Left:" + effect.UsageCount + " Offense Magnification:" + effect.OffenseEffectMagnification + " " + effect.Character.Buff.DefenseMagnification);
                    }
                }
                // _/_/_/_/_/_/ Effect setting end _/_/_/_/_/_/

                battleEnd = false;
                currentBattleWaves = battleWave;

                while (battleEnd == false)
                {
                    EnvironmentInfoClass environmentInfo = new EnvironmentInfoClass(wave: battleWave, turn: 0, phase: 0, randomSeed: seed, r: r);

                    for (int turn = 1; turn <= 20; turn++)
                    {
                        environmentInfo.Turn = turn;
                        //------------------------Header Phase------------------------
                        //Battle End check
                        if (battleEnd == true) { continue; } //continue turn.

                        //Effect/Buff initialize and Set again
                        for (int i = 0; i < characters.Count; i++) { characters[i].Buff.InitializeBuff(); }
                        foreach (EffectClass effect in effects) { effect.BuffToCharacter(currentTurn: turn); }


                        for (int actionPhase = 0; actionPhase <= 3; actionPhase++)
                        {
                            Stack<OrderClass> skillTriggerPossibilityCheck;
                            //------------------------Action order routine------------------------
                            //Only alive character should be action.
                            List<BattleUnit> aliveCharacters = characters.FindAll(character1 => character1.Combat.HitPointCurrent > 0);

                            //Action order decision for each turn
                            Stack<OrderClass> orders = new Stack<OrderClass>();
                            List<OrderClass> orderForSort = new List<OrderClass>();

                            {

                                string firstLine = null;
                                string log = null; int orderNumber = 0;
                                BattleLogClass battleLog;
                                OrderConditionClass orderCondition;

                                switch (actionPhase)
                                {
                                    case 0: //Battle conditions output
                                        environmentInfo.Phase = 0;
                                        //log += "------------------------------------\n";
                                        text = new FuncBattleConditionsText(currentTurn: turn, currentBattleWaves: currentBattleWaves, characters: characters);
                                        //firstLine = text.FirstLine();
                                        //log += text.Text();
                                        log += null;

                                        orderCondition = new OrderConditionClass(wave: environmentInfo.Wave, turn: environmentInfo.Turn, phase: environmentInfo.Phase, orderNumber: 0, nest: 0, nestOrderNumber: 0);
                                        battleLog = new BattleLogClass(orderCondition: orderCondition, isNavigation: false, order: null, firstLine: firstLine, log: log, importance: 1, whichAffiliationAct: Affiliation.none)
                                        {
                                            IsHeaderInfo = true,
                                            HeaderInfoText = text.FirstLine()
                                        };
                                        List<BattleUnit> copyedBattleUnit = (from BattleUnit character in characters
                                                                             select character.Copy()).ToList();
                                        battleLog.Characters = copyedBattleUnit;
                                        battleLogList.Add(battleLog);

                                        break;
                                    case 1: // Action phase:1 at beginning
                                        environmentInfo.Phase = 1; orderNumber = 0;
                                        firstLine = "[At beging phase] \n";
                                        orderCondition = new OrderConditionClass(wave: environmentInfo.Wave, turn: environmentInfo.Turn, phase: environmentInfo.Phase, orderNumber: orderNumber, nest: 0, nestOrderNumber: 0);
                                        battleLog = new BattleLogClass(orderCondition: orderCondition, isNavigation: false, order: null, firstLine: firstLine, log: null, importance: 1, whichAffiliationAct: Affiliation.none);
                                        battleLog.HeaderInfoText = "[At beging phase] \n";
                                        //battleLogList.Add(battleLog);


                                        // _/_/_/_/_/_/_/_/ At Beginning Skill _/_/_/_/_/_/_/_/_/_/

                                        skillTriggerPossibilityCheck = SkillTriggerPossibilityCheck(actor: null, effects: effects, characters: characters, attackerOrder: null,
                                        orders: orders, actionType: ActionType.AtBeginning, shouldHeal: false, isDamageControlAssist: false, battleResult: null, individualTarget: null, nestNumber: 0, environmentInfo: environmentInfo);
                                        while (skillTriggerPossibilityCheck != null && skillTriggerPossibilityCheck.Count > 0) { orders.Push(skillTriggerPossibilityCheck.Pop()); }
                                        break;
                                    case 2:
                                        environmentInfo.Phase = 2;
                                        firstLine = "[Main action phase] \n";
                                        orderCondition = new OrderConditionClass(wave: environmentInfo.Wave, turn: environmentInfo.Turn, phase: environmentInfo.Phase, orderNumber: 0, nest: 0, nestOrderNumber: 0);
                                        battleLog = new BattleLogClass(orderCondition: orderCondition, isNavigation: false, order: null, firstLine: firstLine, log: null, importance: 1, whichAffiliationAct: Affiliation.none);
                                        battleLog.HeaderInfoText = "[Main action phase] \n";
                                        //battleLogList.Add(battleLog);

                                        for (int i = 0; i <= aliveCharacters.Count - 1; i++)
                                        {
                                            List<EffectClass> effectList = effects.FindAll((obj) => obj.Character == aliveCharacters[i] && obj.Skill.ActionType == ActionType.Move
                                            && obj.UsageCount > 0 && obj.VeiledFromTurn <= turn && obj.VeiledToTurn >= turn);
                                            orderCondition = new OrderConditionClass(wave: environmentInfo.Wave, turn: environmentInfo.Turn, phase: environmentInfo.Phase, orderNumber: 0, nest: 0, nestOrderNumber: 0);

                                            // Add normal attack skills
                                            EffectClass normalAttackEffect = new EffectClass(character: aliveCharacters[i], skill: _normalAttackSkillMaster, actionType: ActionType.NormalAttack, offenseEffectMagnification: 1.0,
                                            triggeredPossibility: 1.0, isDamageControlAssistAble: false, usageCount: 1000, veiledFromTurn: 1, veiledToTurn: 20);
                                            effectList.Add(normalAttackEffect);
                                            orderForSort.Add(new OrderClass(orderCondition: orderCondition, actor: aliveCharacters[i], actionType: ActionType.Move, skillEffectProposed: ref effectList,
                                            actionSpeed: (aliveCharacters[i].Ability.Responsiveness * r.Next(40 + aliveCharacters[i].Ability.Luck, 100)), individualTarget: null, isDamageControlAssist: false));
                                        }
                                        orderForSort.Sort((OrderClass x, OrderClass y) => x.ActionSpeed - y.ActionSpeed);
                                        orderNumber = orderForSort.Count(); foreach (OrderClass order in orderForSort) { order.OrderCondition.OrderNumber = orderNumber; orderNumber--; }

                                        for (int i = 0; i < orderForSort.Count; i++) { orders.Push(orderForSort[i]); }
                                        break;
                                    case 3:
                                        environmentInfo.Phase = 3;
                                        log += new string(' ', 1) + "[At ending phase] \n";
                                        //Initializee 

                                        //Heal Shiled by generation %
                                        ShieldHealFunction shieldHeal = new ShieldHealFunction(characters: characters);
                                        log += shieldHeal.Log;
                                        CalculationHateMagnificationPerTurnFunction camlHate = new CalculationHateMagnificationPerTurnFunction(characters: characters);
                                        log += camlHate.Log;

                                        orderCondition = new OrderConditionClass(wave: environmentInfo.Wave, turn: environmentInfo.Turn, phase: environmentInfo.Phase, orderNumber: 0, nest: 0, nestOrderNumber: 0);
                                        battleLog = new BattleLogClass(orderCondition: orderCondition, isNavigation: false, order: null, firstLine: null, log: log, importance: 1, whichAffiliationAct: Affiliation.none);
                                        battleLogList.Add(battleLog);

                                        break;
                                }


                            }
                            //------------------------Action phase------------------------
                            //Action for each character by action order.

                            while (orders.Any()) // loop until order is null.
                            {
                                string firstLine = null;
                                string log = null;
                                OrderClass order = orders.Pop();
                                BattleLogClass battleLog;
                                if (order.Actor == null) { continue; } // attacker alive check, if crushed, continue.
                                if (order.Actor.Combat.HitPointCurrent <= 0) { continue; }

                                //[[ SKILLS CHECK ]] Interrupt skills triger.

                                //------------------------Indivisual attacker's move phase------------------------

                                //------------------------Attacker's action dicision phase------------------------
                                // BasicAttackFunction basicAttack;
                                BattleResultClass battleResult = new BattleResultClass();
                                (BattleLogClass BattleLog, BattleResultClass battleResult) result; //BasicAttackFunction basicAttack;

                                //[[ SKILLS CHECK ]] Move skills triger.
                                order.SkillDecision(characters: characters, environmentInfo: environmentInfo);

                                //effect spend.
                                if (order.SkillEffectChosen != null)
                                {
                                    order.SkillEffectChosen.UsageCount -= 1;
                                    order.SkillEffectChosen.SpentCount += 1;
                                    order.SkillEffectChosen.NextAccumulationCount += (int)(order.SkillEffectChosen.Skill.TriggerBase.AccumulationBaseRate * order.SkillEffectChosen.Skill.TriggerBase.AccumulationWeight);
                                }


                                bool willSkip = false;
                                (firstLine, log) = SkillLogicDispatcher(order: order, characters: characters, environmentInfo: environmentInfo); // SkillLogic action include damage control assist.
                                if (firstLine != null)
                                {
                                    battleLog = new BattleLogClass(orderCondition: order.OrderCondition, isNavigation: false, order: order, firstLine: firstLine, log: log, importance: 1, whichAffiliationAct: order.Actor.Affiliation);
                                    battleLogList.Add(battleLog);
                                    willSkip = true;
                                }
                                if (willSkip != true)
                                {
                                    (firstLine, log) = BuffDebuffFunction(order: order, characters: characters, effects: effects, buffMasters: _buffMasters, turn: turn); //Buff, debuff action
                                    if (firstLine != null)
                                    {
                                        battleLog = new BattleLogClass(orderCondition: order.OrderCondition, isNavigation: false, order: order, firstLine: firstLine, log: log, importance: 1, whichAffiliationAct: order.Actor.Affiliation);
                                        battleLogList.Add(battleLog);
                                        willSkip = true;
                                    }
                                }

                                result = SkillMoveFunction(order: order, characters: characters, environmentInfo: environmentInfo); // offense action                            
                                if (result.BattleLog != null) { log = result.BattleLog.Log; firstLine = result.BattleLog.FirstLine; }

                                if (log != null)
                                {
                                    battleLog = new BattleLogClass(orderCondition: order.OrderCondition, isNavigation: false, order: order, firstLine: firstLine, log: log, importance: 1, whichAffiliationAct: order.Actor.Affiliation);
                                    battleLogList.Add(battleLog);
                                }
                                battleResult = result.battleResult;




                                if (order.IsDamageControlAssist) //only when Damage Control Assist
                                {
                                    List<OrderClass> deleteOneActionOrderrIfHave = orders.ToList();
                                    OrderClass deleteOneActionOrderRaw = deleteOneActionOrderrIfHave.FindLast(obj => obj.Actor == order.Actor && obj.ActionType == ActionType.Move);
                                    OrderClass deleteOneActionOrder = null;
                                    foreach (EffectClass effect in deleteOneActionOrderRaw.SkillEffectProposed)
                                    { if (effect.Skill.Name == SkillName.normalAttack) { deleteOneActionOrder = deleteOneActionOrderRaw; } }
                                    deleteOneActionOrderrIfHave.Remove(deleteOneActionOrder);
                                    deleteOneActionOrderrIfHave.Reverse();
                                    if (deleteOneActionOrder != null) //clear stack and input again
                                    { orders.Clear(); foreach (OrderClass data in deleteOneActionOrderrIfHave) { orders.Push(data); } }
                                }
                                //Only when first kill happend, insert statistics reporter for first blood per side.
                                if (order.Actor.Affiliation == Affiliation.ally && allyFirstBlood == false && battleResult.NumberOfCrushed >= 1) //When ally first blood 
                                {
                                    string es = null;
                                    if (battleResult.NumberOfCrushed != 1) { es = "es"; }
                                    string t = order.Actor.Name + "'s " + order.SkillEffectChosen.Skill.Name + ". first blood! total dealt damage:" + battleResult.TotalDeltDamage.WithComma() + " " + battleResult.NumberOfCrushed.WithComma() + " crush" + es + ".";
                                    StatisticsReporterFirstBloodClass setStatisticsReporterFirstBlood = statisticsReporterFirstBlood.FindLast((obj) => obj.BattleWave == battleWave);
                                    setStatisticsReporterFirstBlood.Set(whichAffiliation: Affiliation.ally, characterName: order.Actor.Name, actionType: order.ActionType, happenedTurn: turn,
                                        crushedCount: battleResult.NumberOfCrushed, totalDealtDamage: battleResult.TotalDeltDamage, contentText: t);
                                    if (result.BattleLog != null) { setStatisticsReporterFirstBlood.BattleLogAlly = result.BattleLog; }
                                    allyFirstBlood = true;

                                }
                                else if (order.Actor.Affiliation == Affiliation.enemy && enemyFirstBlood == false && battleResult.NumberOfCrushed >= 1) //When enemy first blood 
                                {
                                    string es = null;
                                    if (battleResult.NumberOfCrushed != 1) { es = "es"; }
                                    string t = order.Actor.Name + "'s " + order.SkillEffectChosen.Skill.Name + ". first blood! total dealt damage:" + battleResult.TotalDeltDamage.WithComma() + " " + battleResult.NumberOfCrushed.WithComma() + " crush" + es + ".";
                                    StatisticsReporterFirstBloodClass setStatisticsReporterFirstBlood = statisticsReporterFirstBlood.FindLast((obj) => obj.BattleWave == battleWave);
                                    setStatisticsReporterFirstBlood.Set(whichAffiliation: Affiliation.enemy, characterName: order.Actor.Name, actionType: order.ActionType, happenedTurn: turn,
                                    crushedCount: battleResult.NumberOfCrushed, totalDealtDamage: battleResult.TotalDeltDamage, contentText: t);
                                    if (result.BattleLog != null) { setStatisticsReporterFirstBlood.BattleLogEnemy = result.BattleLog; }
                                    enemyFirstBlood = true;
                                }

                                if (battleEnd == false)
                                {
                                    battleEnd = battleResult.BattleEnd;
                                    if (battleResult.IsAllyWin == true)
                                    {
                                        allyWinCount++; statisticsReporterWhichWins[battleWave - 1] = WhichWin.allyWin;
                                        whichWin = WhichWin.allyWin;
                                    }
                                    if (battleResult.IsEnemyWin) { enemyWinCount++; statisticsReporterWhichWins[battleWave - 1] = WhichWin.enemyWin;
                                        whichWin = WhichWin.enemyWin;

                                    }
                                    if (battleResult.IsDraw) { drawCount++; statisticsReporterWhichWins[battleWave - 1] = WhichWin.Draw;
                                        whichWin = WhichWin.Draw;
                                    }
                                }

                                OrderStatusClass orderStatus = new OrderStatusClass(); orderStatus.Initialize();

                                int nestNumber = 0;
                                //[[ SKILLS CHECK ]] Damage Control Assist trigger. Note: ActionType independent so ActionType.any!
                                Stack<OrderClass> damageControlAssistStack = SkillTriggerPossibilityCheck(actor: null, effects: effects, characters: characters,
                                    attackerOrder: order, orders: orders, actionType: ActionType.Any, shouldHeal: true, isDamageControlAssist: true,
                                        battleResult: battleResult, individualTarget: order.Actor, nestNumber: nestNumber, environmentInfo: environmentInfo);

                                //[[ SKILLS CHECK ]] ReAttack skills trigger.
                                Stack<OrderClass> reAttackStack = SkillTriggerPossibilityCheck(actor: null, effects: effects, characters: characters,
                                    attackerOrder: order, orders: orders, actionType: ActionType.ReAttack, shouldHeal: false, isDamageControlAssist: false,
                                        battleResult: battleResult, individualTarget: order.Actor, nestNumber: nestNumber, environmentInfo: environmentInfo);

                                //[[ SKILLS CHECK ]] Chain skills trigger.
                                Stack<OrderClass> chainStack = SkillTriggerPossibilityCheck(actor: null, effects: effects, characters: characters,
                                     attackerOrder: order, orders: orders, actionType: ActionType.Chain, shouldHeal: false, isDamageControlAssist: false,
                                      battleResult: battleResult, individualTarget: order.Actor, nestNumber: nestNumber, environmentInfo: environmentInfo);

                                //[[ SKILLS CHECK ]] Counter skills triger.
                                Stack<OrderClass> counterStack = SkillTriggerPossibilityCheck(actor: null, effects: effects, characters: characters,
                                 attackerOrder: order, orders: orders, actionType: ActionType.Counter, shouldHeal: false, isDamageControlAssist: false,
                                  battleResult: battleResult, individualTarget: order.Actor, nestNumber: nestNumber, environmentInfo: environmentInfo);


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
                                string navigationLog = null;
                                NavigatorSpeechAfterMoveClass navigatorSpeechAfterMove = new NavigatorSpeechAfterMoveClass(navigatorName: navigatorName, order: order,
                                    characters: characters, effects: effects, orderStatus: orderStatus, environmentInfo: environmentInfo);

                                navigationLog = navigatorSpeechAfterMove.Log;
                                if (navigationLog != null)
                                {
                                    //navigationLog += new string(' ', 2) + "-------------\n";
                                    battleLog = new BattleLogClass(orderCondition: order.OrderCondition, isNavigation: true, order: null, firstLine: null, log: navigationLog, importance: 1, whichAffiliationAct: order.Actor.Affiliation);
                                    battleLogList.Add(battleLog);
                                }
                            }  // Until all Characters act.

                        } // action Phase.
                          //------------------------Footer phase------------------------
                        foreach (BattleUnit character in characters) { if (character.IsCrushedJustNow) { character.IsCrushedJustNow = false; } } // reset IsCrushedJustNow flag

                        //Check wipe out and should continue the battle
                        if (battleEnd == false)
                        {
                            wipeOutCheck = new WipeOutCheck(characters);
                            if (wipeOutCheck.IsAllyWin) { allyWinCount++; whichWin = WhichWin.allyWin; }
                            if (wipeOutCheck.IsEnemyWin) { enemyWinCount++; whichWin = WhichWin.enemyWin; }
                            if (wipeOutCheck.IsDraw) { drawCount++; whichWin = WhichWin.Draw; }
                            battleEnd = wipeOutCheck.BatleEnd;
                        }
                        totalturn = turn; // If battle end, set total turn for show end log.

                    } //turn

                    //------------------------Statistics phase------------------------

                    //time over NEED statistics reporter and FIX BUG "no count"!
                    //Check wipe out and should continue the battle
                    if (battleEnd == false)
                    {
                        drawCount++; battleEnd = true;
                        whichWin = WhichWin.Draw;

                        string log = "Time over. \n";
                        OrderConditionClass orderCondition = new OrderConditionClass(wave: environmentInfo.Wave, turn: environmentInfo.Turn, phase: 4, orderNumber: 0, nest: 0, nestOrderNumber: 0);
                        BattleLogClass battleLog = new BattleLogClass(orderCondition: orderCondition, isNavigation: false, order: null, firstLine: log, log: null, importance: 1, whichAffiliationAct: Affiliation.none);
                        battleLogList.Add(battleLog);
                    }

                } //battleEnd 

                for (int i = 0; i < characters.Count; i++) //Shield, HitPoint initialize
                { characters[i].SetPermanentStatistics(statistics: characters[i].Statistics); }// set permanent statistics before initialize statistics.
            } //Battle waves

            subLogPerWavesSets[battleWavesSet - 1] += "[Set:" + battleWavesSet + "] Battle count:" + (allyWinCount + enemyWinCount + drawCount) + " Win:" + (allyWinCount) + " lost:" + (enemyWinCount)
                + " Win Ratio:" + (int)((double)allyWinCount / (double)(allyWinCount + enemyWinCount + drawCount) * 100)
                + "% Ally:[Attack x" + Math.Round(_allyAttackMagnification, 2) + "] [Defense x" + Math.Round(_allyDefenseMagnification, 2) + "] \n";
            //statistics reporter open
            for (int battleWave = 1; battleWave <= battleWaves; battleWave++)
            {
                StatisticsReporterFirstBloodClass setStatisticsReporterFirstBlood = statisticsReporterFirstBlood.FindLast((obj) => obj.BattleWave == battleWave);
                setStatisticsReporterFirstBlood.WhichWin = statisticsReporterWhichWins[battleWave - 1];
            }
            var statisticsQueryAlly = statisticsReporterFirstBlood.Where(x => x.WhichWin == WhichWin.allyWin)
                .GroupBy(x => x.AllyCharacterName).Select(x => new { Subj = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count);
            var statisticsQueryEnemy = statisticsReporterFirstBlood.Where(x => x.WhichWin == WhichWin.enemyWin)
                .GroupBy(x => x.EnemyCharacterName).Select(x => new { Subj = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count);

            logPerWavesSets[battleWavesSet - 1] += "Ally info: MVP(times)\n";
            foreach (var group in statisticsQueryAlly) { logPerWavesSets[battleWavesSet - 1] += new string(' ', 2) + group.Subj + " (" + group.Count + ")."; }
            logPerWavesSets[battleWavesSet - 1] += "\n";
            if (statisticsReporterFirstBlood.FindAll((obj) => obj.WhichWin == WhichWin.allyWin).Any())
            {
                StatisticsReporterFirstBloodClass bestFirstBloodAlly = statisticsReporterFirstBlood.FindAll((obj) => obj.WhichWin == WhichWin.allyWin).OrderByDescending(obj => obj.AllyTotalDealtDamage).First();
                //logPerWavesSets[battleWavesSet - 1] += "[Best shot]" + bestFirstBloodAlly.AllyContentText + " " + bestFirstBloodAlly.BattleLogAlly.OrderCondition + "\n";
            }
            logPerWavesSets[battleWavesSet - 1] += "Enemy info: MVP(times) \n";
            foreach (var group in statisticsQueryEnemy) { logPerWavesSets[battleWavesSet - 1] += new string(' ', 2) + group.Subj + " (" + group.Count + ")."; }
            logPerWavesSets[battleWavesSet - 1] += "\n";
            if (statisticsReporterFirstBlood.FindAll((obj) => obj.WhichWin == WhichWin.enemyWin).Any())
            {
                StatisticsReporterFirstBloodClass bestFirstBloodEnemy = statisticsReporterFirstBlood.FindAll((obj) => obj.WhichWin == WhichWin.enemyWin).OrderByDescending(obj => obj.EnemyTotalDealtDamage).First();
                logPerWavesSets[battleWavesSet - 1] += "[Best shot]" + bestFirstBloodEnemy.EnemyContentText + " " + bestFirstBloodEnemy.BattleLogEnemy.OrderCondition + "\n";
            }
            //Characters permanent Statistics Collection
            foreach (BattleUnit character in characters) { character.PermanentStatistics.Avarage(battleWaves: battleWaves); } // Avarage Calculation
            logPerWavesSets[battleWavesSet - 1] += "Avarage (critical):\n";
            foreach (BattleUnit character in characters) { logPerWavesSets[battleWavesSet - 1] += new string(' ', 1) + character.Name + " " + character.PermanentStatistics.AllCriticalRatio() + "\n"; }
            logPerWavesSets[battleWavesSet - 1] += "Avarage Skill:\n";
            foreach (BattleUnit character in characters) { logPerWavesSets[battleWavesSet - 1] += character.Name + " " + character.PermanentStatistics.Skill() + "\n"; }
            //logPerWavesSets[battleWavesSet - 1] += "------------------------------------\n";

        } // Battle waves set

        //Battle is over.

        // last turn battle log 
        //environmentInfo.Phase = 0;
        //log += "------------------------------------\n";
        text = new FuncBattleConditionsText(currentTurn: totalturn + 1, currentBattleWaves: currentBattleWaves, characters: characters);
        //firstLine = text.FirstLine();
        //log += text.Text();
        //string log = null;

        OrderConditionClass orderConditionFinal;
        orderConditionFinal = new OrderConditionClass(wave: currentBattleWaves, turn: totalturn + 1, phase: 0, orderNumber: 0, nest: 0, nestOrderNumber: 0);
        BattleLogClass battleLogFinal;
        battleLogFinal = new BattleLogClass(orderCondition: orderConditionFinal, isNavigation: false, order: null, firstLine: null, log: null, importance: 1, whichAffiliationAct: Affiliation.none)
        {
            IsHeaderInfo = true,
            HeaderInfoText = text.FirstLine()
        };
        List<BattleUnit> copyedBattleUnitLast = (from BattleUnit character in characters
                                                 select character.Copy()).ToList();
        battleLogFinal.Characters = copyedBattleUnitLast;
        battleLogList.Add(battleLogFinal);



        List<BattleLogClass> battleLogDisplayList = battleLogList.FindAll(obj => obj.OrderCondition.Wave == battleWaves); // only last battlelog displayed.
        //foreach (BattleLogClass battleLog in battleLogDisplayList)
        //{

        //    if (battleLog.IsNavigation == false)
        //    { finalLog += "(" + battleLog.OrderCondition.Phase + "-" + battleLog.OrderCondition.OrderNumber + "-" + battleLog.OrderCondition.Nest + "-" + battleLog.OrderCondition.NestOrderNumber + ")" + battleLog.Log; }
        //    else { finalLog += new string(' ', 5) + battleLog.Log; }
        //}
        //// delete battle display list to final log, because now meaningless.

        //finalLog += "------------------------------------\n";
        finalLog += "Battle is over. " + whichWin + "\n";
        text = new FuncBattleConditionsText(currentTurn: totalturn, currentBattleWaves: currentBattleWaves, characters: characters);
        finalLog += text.Text();

        for (int i = 0; i < battleWavesSets; i++) { finalLog += logPerWavesSets[i]; }
        finalLog += " Ally attack magnification per waves set: x" + (1 + allyAttackMagnificationPerWavesSet) + "\n";
        finalLog += " Ally defense magnification per waves set: x" + (1 + allyDefenseMagnificationPerWavesSet) + "\n";
        for (int i = 0; i < battleWavesSets; i++) { finalLog += subLogPerWavesSets[i]; }

        DateTime finishDateTime = DateTime.Now;
        TimeSpan processedTimeSpan = finishDateTime - startDateTime;
        finalLog += "finished:" + finishDateTime + " processed time:" + processedTimeSpan + " seed:" + seed + "\n";

        Console.WriteLine(finalLog);
        OrderConditionClass finalorderCondition = new OrderConditionClass(wave: battleWaves, turn: totalturn, phase: 0, orderNumber: 0, nest: 0, nestOrderNumber: 0);
        BattleLogClass finalLoglist = new BattleLogClass(orderCondition: finalorderCondition, isNavigation: true, order: null, firstLine: null, log: finalLog,
            importance: 1, whichAffiliationAct: Affiliation.none);
        battleLogDisplayList.Add(finalLoglist);
        logList = battleLogDisplayList;
    }

    // Skill check method
    public static Stack<OrderClass> SkillTriggerPossibilityCheck(BattleUnit actor, List<EffectClass> effects, List<BattleUnit> characters, OrderClass attackerOrder,
    Stack<OrderClass> orders, ActionType actionType, bool shouldHeal, bool isDamageControlAssist,
        BattleResultClass battleResult, BattleUnit individualTarget, int nestNumber, EnvironmentInfoClass environmentInfo)
    {
        if (attackerOrder != null && attackerOrder.IsDamageControlAssist) { return null; } //If previous move is Damage Control Assist, no counter, reattack, chain and Damage control assist is triggered.
        List<EffectClass> rawActionTypeEffects;
        if (isDamageControlAssist) // Damage control assist is ActionType independent
        {
            rawActionTypeEffects = effects.FindAll((obj) => obj.UsageCount > 0 && obj.Character.Combat.HitPointCurrent > 0 &&
            obj.Skill.IsHeal == shouldHeal && obj.VeiledFromTurn <= environmentInfo.Turn && obj.VeiledToTurn >= environmentInfo.Turn);
        }
        else
        {
            if (shouldHeal) //if heal has, be selected.
            {
                rawActionTypeEffects = effects.FindAll((obj) => obj.ActionType == actionType && obj.UsageCount > 0 && obj.Character.Combat.HitPointCurrent > 0 &&
                obj.Skill.IsHeal == shouldHeal && obj.VeiledFromTurn <= environmentInfo.Turn && obj.VeiledToTurn >= environmentInfo.Turn);
                if (!rawActionTypeEffects.Any()) //if no heal skill left, other move skill should be selected.
                {
                    rawActionTypeEffects = effects.FindAll((obj) => obj.ActionType == actionType && obj.UsageCount > 0 && obj.Character.Combat.HitPointCurrent > 0 &&
                    obj.Skill.IsHeal == !shouldHeal && obj.VeiledFromTurn <= environmentInfo.Turn && obj.VeiledToTurn >= environmentInfo.Turn);
                }
            }
            else // should not heal, so find other move skill.
            {
                rawActionTypeEffects = effects.FindAll((obj) => obj.ActionType == actionType && obj.UsageCount > 0 && obj.Character.Combat.HitPointCurrent > 0 &&
                obj.Skill.IsHeal == shouldHeal && obj.VeiledFromTurn <= environmentInfo.Turn && obj.VeiledToTurn >= environmentInfo.Turn);
            }
        }
        List<EffectClass> matchedActionTypeEffects = new List<EffectClass>();
        Affiliation counterAffiliation = Affiliation.ally;
        if (attackerOrder != null) //Memo: at Beginning and move skills, attackOrder is null.
        { if (attackerOrder.Actor.Affiliation == Affiliation.ally) { counterAffiliation = Affiliation.enemy; } else { counterAffiliation = Affiliation.ally; } }

        Affiliation affiliationWhoWillAct = Affiliation.none;
        switch (actionType) // Get actionType dependent condition before calculation.
        {
            case ActionType.Move: //Normal moveskill logic: only actor should trigger moveskill.
                matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => obj.Character == actor);
                break;
            case ActionType.Counter:
                if (attackerOrder.Actor.Affiliation == Affiliation.ally) { affiliationWhoWillAct = Affiliation.enemy; }
                else if (attackerOrder.Actor.Affiliation == Affiliation.enemy) { affiliationWhoWillAct = Affiliation.ally; }
                matchedActionTypeEffects = rawActionTypeEffects.FindAll((obj) => obj.Character.Affiliation == affiliationWhoWillAct);
                break;
            case ActionType.Chain:
                matchedActionTypeEffects = rawActionTypeEffects.FindAll((obj) => obj.Character.Affiliation == attackerOrder.Actor.Affiliation && obj.Character != attackerOrder.Actor);
                break;
            case ActionType.ReAttack:
                matchedActionTypeEffects = rawActionTypeEffects.FindAll((obj) => obj.Character == attackerOrder.Actor);
                break;
            case ActionType.AtBeginning:
                matchedActionTypeEffects = rawActionTypeEffects;
                break;
            case ActionType.Any: //[Damage Control Assist skill logic]. ActionType independent so DCA is in ActionType.any.
                if (isDamageControlAssist) // Damage Control Assist skill logic
                {
                    // Actor's affiliation character is dead just now?
                    List<BattleUnit> crushedJustNowCounterAffiliationCharacter = characters.FindAll(obj => obj.IsCrushedJustNow == true && obj.Affiliation == counterAffiliation);
                    if (crushedJustNowCounterAffiliationCharacter.Count > 0) // Damage Control Assist required!
                    { matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => obj.Character.Affiliation == counterAffiliation && obj.Character.Feature.DamageControlAssist == true); }
                    // incase of friendly fired.
                    List<BattleUnit> crushedJustNowbyFriendlyFiredCharacter = characters.FindAll(obj => obj.IsCrushedJustNow == true && obj.Affiliation == attackerOrder.Actor.Affiliation);
                    if (crushedJustNowbyFriendlyFiredCharacter.Count > 0)
                    { matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => obj.Character.Affiliation == attackerOrder.Actor.Affiliation && obj.Character.Feature.DamageControlAssist == true); }
                }
                break;
            default: matchedActionTypeEffects = new List<EffectClass>(); break;
        }

        //push order from slow character's effect to fast character's effect. It means pop from fast character's effect to slow character's effect.
        matchedActionTypeEffects.Sort((EffectClass x, EffectClass y) => y.Character.Ability.Responsiveness - x.Character.Ability.Responsiveness);

        List<EffectClass> validEffects = new List<EffectClass>();
        foreach (EffectClass effect in matchedActionTypeEffects)
        {
            effect.IsntTriggeredBecause.Initialize();
            if (effect.Skill.TriggerTarget.ActionType != ActionType.Any)
            {
                if ((effect.Skill.TriggerTarget.ActionType == attackerOrder.ActionType) == false)
                { effect.IsntTriggeredBecause.TriggerCondition = true; continue; }
            } // Trigger condition check
            if (effect.Skill.ActionType != ActionType.Move && effect.Skill.TriggerTarget.AfterAllMoved == false) // check normal Attack left, except move skill. (move skill is itself so check this logic make no sense )
            {
                List<OrderClass> checkOrders = orders.ToList();
                if (checkOrders.FindLast((obj) => obj.Actor == effect.Character && obj.SkillEffectChosen.Skill.Name == SkillName.normalAttack) == null)
                { effect.IsntTriggeredBecause.AfterAllMoved = true; continue; }// no normalAttack left, whitch means no action.   
            }


            if (effect.Skill.ActionType == ActionType.Move && effect.IsDamageControlAssistAble) //Damage Control Assist Special Logic....
            {
                // check normal Attack left, except move skill. (move skill is itself so check this logic make no sense )
                if (effect.Skill.TriggerTarget.AfterAllMoved == false)
                {
                    List<OrderClass> checkActionOrders = orders.ToList();
                    if (checkActionOrders.FindLast((obj) => obj.Actor == effect.Character && obj.ActionType == ActionType.Move) == null)
                    { effect.IsntTriggeredBecause.AfterAllMoved = true; continue; }// no normalAttack left, whitch means no action.   
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
                List<BattleUnit> aliveActorSide;
                List<BattleUnit> survivaledOpponents;
                aliveActorSide = characters.FindAll(character1 => character1.Affiliation == effect.Character.Affiliation && character1.Combat.HitPointCurrent > 0);
                int aliveAttackerIndex = aliveActorSide.IndexOf(effect.Character);
                int minTargetOptimumRange = (int)(effect.Character.Combat.MinRange * effect.Skill.Magnification.OptimumRangeMin) - aliveAttackerIndex;
                int maxTargetOptimumRange = (int)(effect.Character.Combat.MaxRange * effect.Skill.Magnification.OptimumRangeMax) - aliveAttackerIndex;
                if (effect.Character.Affiliation == Affiliation.ally) { counterAffiliation = Affiliation.enemy; } else { counterAffiliation = Affiliation.ally; }
                survivaledOpponents = characters.FindAll(character1 => character1.Combat.HitPointCurrent > 0 && character1.Affiliation == counterAffiliation);
                survivaledOpponents.Sort((x, y) => x.UniqueID - y.UniqueID);
                int attackerIndex = survivaledOpponents.IndexOf(attackerOrder.Actor);

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

            if (effect.Skill.TriggerTarget.OnlyWhenBeenHitMoreThanOnce && (battleResult.HitMoreThanOnceCharacters.Find((obj) => obj == effect.Character) == null))
            { effect.IsntTriggeredBecause.OnlyWhenBeenHitMoreThanOnce = true; continue; } //Being hit .this means not hit, so skill should not be triggered.
            if (effect.Skill.TriggerTarget.OnlyWhenAvoidMoreThanOnce && ((battleResult.AvoidMoreThanOnceCharacters.Find((obj) => obj == effect.Character)) == null))
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
                default: break;
            }

            double possibility = (double)(environmentInfo.R.Next(0, 1000)) / 1000.0; //TriggerPossibility Check
            if (effect.TriggeredPossibility >= possibility) { validEffects.Add(effect); } else { effect.IsntTriggeredBecause.TriggeredPossibility = true; continue; }
        }

        //set order  grouped by actors
        List<EffectClass> validEffectsPerActor;
        OrderClass skillsByOrder;
        Stack<OrderClass> skillsByOrderStack = new Stack<OrderClass>();
        foreach (BattleUnit character in characters)
        {
            validEffectsPerActor = validEffects.FindAll(obj => obj.Character == character);
            if (validEffectsPerActor.Count >= 1)
            {
                int orderNumber = 0; int nest = 0; if (attackerOrder != null) { orderNumber = attackerOrder.OrderCondition.OrderNumber; nest = attackerOrder.OrderCondition.Nest; }
                int addCount = 0;

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
                OrderConditionClass orderCondition = new OrderConditionClass(wave: environmentInfo.Wave, turn: environmentInfo.Turn, phase: environmentInfo.Phase, orderNumber: orderNumber,
                    nest: nest + addCount, nestOrderNumber: nestNumber);

                skillsByOrder = new OrderClass(orderCondition: orderCondition, actor: character, actionType: actionType, skillEffectProposed: ref validEffectsPerActor, actionSpeed: 0,
                 individualTarget: individualTarget, isDamageControlAssist: isDamageControlAssist);
                skillsByOrderStack.Push(skillsByOrder); nestNumber++;
            }
        }
        if (skillsByOrderStack.Count > 0) { return skillsByOrderStack; }
        return null;
    }

    // Buff logic
    public static (string firstLine, string log) BuffDebuffFunction(OrderClass order, List<BattleUnit> characters, List<EffectClass> effects, List<SkillsMasterClass> buffMasters, int turn)
    {
        SkillsMasterClass addingBuff;
        List<EffectClass> addingEffect = new List<EffectClass>();
        string firstline = null;
        string log = null;
        if (order.SkillEffectChosen == null) { return (null, null); } // no effect exist, so no buff/ debuff happened
        foreach (BattleUnit character in characters) { if (character.IsCrushedJustNow) { character.IsCrushedJustNow = false; } } // reset IsCrushedJustNow flag
        switch (order.SkillEffectChosen.Skill.BuffTarget.TargetType)
        {
            case TargetType.self: //Buff self
                addingBuff = buffMasters.FindLast((obj) => obj.Name == order.SkillEffectChosen.Skill.CallingBuffName);
                addingEffect.Add(new EffectClass(character: order.Actor, skill: addingBuff, actionType: ActionType.None,
                offenseEffectMagnification: 1.0, triggeredPossibility: 0.0, isDamageControlAssistAble: false, usageCount: addingBuff.UsageCount,
                   veiledFromTurn: turn, veiledToTurn: (turn + addingBuff.VeiledTurn)));
                effects.Add(addingEffect[0]);
                addingEffect[0].BuffToCharacter(currentTurn: turn);
                order.Actor.Buff.AddBarrier(addingEffect[0].Skill.BuffTarget.BarrierRemaining);

                string triggerPossibilityText = null;
                if (order.SkillEffectChosen.TriggeredPossibility < 1.0) { triggerPossibilityText = "(" + (double)((int)(order.SkillEffectChosen.TriggeredPossibility * 1000) / 10.0) + "%) "; }
                string accumulationText = null;
                if (order.SkillEffectChosen.NextAccumulationCount > 0)
                {
                    double count = 0.0;
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
                        default: break;
                    }
                    string nextText = null;
                    if (order.SkillEffectChosen.UsageCount > 0) { nextText = " next trigger: " + order.SkillEffectChosen.NextAccumulationCount; } else { nextText = " no usuage count left."; }
                    accumulationText = "(Accumulation " + order.SkillEffectChosen.Skill.TriggerBase.AccumulationReference.ToString() + ": " + count + nextText + ")";
                }

                firstline = order.SkillEffectChosen.Skill.Name + "! " + triggerPossibilityText + accumulationText + "\n"
          + new string(' ', 3) + order.Actor.Name + " gets " + addingBuff.Name + " which will last " + addingBuff.VeiledTurn + " turns. " + "\n";

                //firstline = new string(' ', 0) + order.Actor.Name + "'s " + order.SkillEffectChosen.Skill.Name + "! " + triggerPossibilityText + accumulationText + "\n"
                //+ new string(' ', 3) + order.Actor.Name + " gets " + addingBuff.Name + " which will last " + addingBuff.VeiledTurn + " turns. " + "\n";
                // Belows: It's a temporary message, only for deffense magnification.
                if (addingBuff.BuffTarget.DefenseMagnification > 1.0) { log += new string(' ', 4) + "[Defense: " + order.Actor.Buff.DefenseMagnification + " (x" + addingBuff.BuffTarget.DefenseMagnification + ")] "; }
                if (addingEffect[0].Skill.BuffTarget.BarrierRemaining > 0) { log += "[Barrier:" + order.Actor.Buff.BarrierRemaining + " (+" + addingEffect[0].Skill.BuffTarget.BarrierRemaining + ")] "; }
                log += "\n";
                break;
            case TargetType.multi: //Buff attacker's side all
                addingBuff = buffMasters.FindLast((obj) => obj.Name == order.SkillEffectChosen.Skill.CallingBuffName);
                List<BattleUnit> buffTargetCharacters = characters.FindAll(character1 => character1.Affiliation == order.Actor.Affiliation && character1.Combat.HitPointCurrent > 0);
                firstline = new string(' ', 0) + order.Actor.Name + "'s " + order.SkillEffectChosen.Skill.Name + "! (Trigger Possibility:" + (double)((int)(order.SkillEffectChosen.TriggeredPossibility * 1000) / 10.0) + "%) \n";

                for (int i = 0; i < buffTargetCharacters.Count; i++)
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
            default: break;
        }

        if (order.SkillEffectChosen.Skill.DebuffTarget.TargetType != TargetType.none)
        {
            //Debuff exist
        }
        return (firstline, log);
    }

    public static (string firstline, string log) SkillLogicDispatcher(OrderClass order, List<BattleUnit> characters, EnvironmentInfoClass environmentInfo)
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
            default:
                break;
        }
        return (firstLine, log);
    }

    public static (BattleLogClass, BattleResultClass) SkillMoveFunction(OrderClass order, List<BattleUnit> characters, EnvironmentInfoClass environmentInfo)
    {
        AttackFunction attack;
        BattleResultClass battleResult = new BattleResultClass();
        BattleLogClass battleLog = new BattleLogClass();

        switch (order.SkillEffectChosen.Skill.Magnification.AttackTarget)
        {
            case TargetType.self: break;
            case TargetType.none: break;
            default:
                attack = new AttackFunction(order: order, characters: characters, environmentInfo: environmentInfo);
                battleResult = attack.BattleResult;
                battleLog = attack.BattleLog;
                break;
        }
        return (battleLog, battleResult);
    }

} //End of MainClass


