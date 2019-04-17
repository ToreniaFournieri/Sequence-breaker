using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BattleEngine
{
    public List<BattleLogClass> logList;

    public void Battle()
    {
        DateTime startDateTime = DateTime.Now;
        Console.WriteLine("start:" + startDateTime);
        // Battle environment setting
        int numberOfCharacters = 14;
        int battleWavesSets = 1;
        int battleWaves = 1; // one set of battle 
        string navigatorName = "Navigator";

        //BattleWaveSet variables
        double allyAttackMagnificationPerWavesSet = 0.20;
        double allyDefenseMagnificationPerWavesSet = 0.20;

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

        double allyAttackMagnification = 1.0;
        double allyDefenseMagnification = 1.0;

        List<BattleUnit> characters = new List<BattleUnit>();
        AbilityClass[] abilities = new AbilityClass[numberOfCharacters];

        //Skill make logic , test: one character per one skill
        SkillsMasterStruct[] skillsMasters = new SkillsMasterStruct[numberOfCharacters + 1];
        List<SkillsMasterStruct> buffMasters = new List<SkillsMasterStruct>();

        //Effect (permament skill) make logic, the effect has two meanings, one is permanent skill, the other is temporary skill (may call buff).
        List<EffectClass> effects = new List<EffectClass>();

        //ally info
        abilities[0] = new AbilityClass(power: 27, generation: 11, stability: 22, responsiveness: 32, precision: 22, intelligence: 21, luck: 34); //PIG1-
        abilities[1] = new AbilityClass(power: 29, generation: 13, stability: 22, responsiveness: 24, precision: 22, intelligence: 14, luck: 15); //PIG2-
        abilities[2] = new AbilityClass(power: 27, generation: 10, stability: 22, responsiveness: 21, precision: 25, intelligence: 21, luck: 34); //PIG3-
        abilities[3] = new AbilityClass(power: 29, generation: 10, stability: 22, responsiveness: 24, precision: 22, intelligence: 14, luck: 15); //PIG4-
        abilities[4] = new AbilityClass(power: 27, generation: 10, stability: 22, responsiveness: 32, precision: 22, intelligence: 21, luck: 34); //PIG5-
        abilities[5] = new AbilityClass(power: 29, generation: 12, stability: 22, responsiveness: 24, precision: 22, intelligence: 30, luck: 15); //PIG6-
        abilities[6] = new AbilityClass(power: 27, generation: 11, stability: 22, responsiveness: 20, precision: 22, intelligence: 21, luck: 34); //PIG7-
                                                                                                                                                  //enemy info
        abilities[7] = new AbilityClass(power: 29, generation: 8, stability: 22, responsiveness: 23, precision: 22, intelligence: 14, luck: 15); //ELD1- 
        abilities[8] = new AbilityClass(power: 27, generation: 13, stability: 22, responsiveness: 31, precision: 22, intelligence: 21, luck: 34); //ELD2-
        abilities[9] = new AbilityClass(power: 29, generation: 8, stability: 22, responsiveness: 22, precision: 22, intelligence: 14, luck: 15); //ELD3-
        abilities[10] = new AbilityClass(power: 27, generation: 9, stability: 22, responsiveness: 30, precision: 22, intelligence: 21, luck: 34); //ELD4-
        abilities[11] = new AbilityClass(power: 29, generation: 8, stability: 22, responsiveness: 25, precision: 22, intelligence: 14, luck: 15); //ELD5-
        abilities[12] = new AbilityClass(power: 27, generation: 11, stability: 22, responsiveness: 36, precision: 22, intelligence: 21, luck: 34); //ELD6-
        abilities[13] = new AbilityClass(power: 29, generation: 12, stability: 22, responsiveness: 20, precision: 22, intelligence: 14, luck: 15); //ELD7-

        OffenseMagnificationClass offenseMagnification =
         new OffenseMagnificationClass(optimumRangeBonus: 1.2, critical: 1.4, kinetic: 1.0, chemical: 1.0, thermal: 1.4, vsBeast: 1.0, vsCyborg: 2.44, vsDrone: 1.0, vsRobot: 2.2, vsTitan: 1.0);

        DefenseMagnificationClass defenseMagnification =
            new DefenseMagnificationClass(critical: 1.0, kinetic: 1.0, chemical: 1.0, thermal: 1.0, vsBeast: 1.0, vsCyborg: 1.0, vsDrone: 1.0, vsRobot: 1.2, vsTitan: 1.0);

        ActionSkillClass skillActionSkillInitial = new ActionSkillClass(move: 1.0, heal: 1.0, counter: 1.0, chain: 1.0, reAttack: 1.0,
            interrupt: 1.0, atBeginning: 1.0, atEnding: 1.0);
        ActionSkillClass skillActionSkillAllDouble = new ActionSkillClass(move: 2.0, heal: 1.0, counter: 2.0, chain: 2.0, reAttack: 2.0,
            interrupt: 2.0, atBeginning: 2.0, atEnding: 2.0);
        ActionSkillClass skillActionSkillAllTriple = new ActionSkillClass(move: 3.0, heal: 1.0, counter: 3.0, chain: 3.0, reAttack: 3.0,
            interrupt: 3.0, atBeginning: 3.0, atEnding: 3.0);

        UnitSkillMagnificationClass skillMagnificationAllInitial = new UnitSkillMagnificationClass(offenseEffectPower: skillActionSkillInitial, triggerPossibility: skillActionSkillInitial);
        UnitSkillMagnificationClass skillMagnificationAllDouble = new UnitSkillMagnificationClass(offenseEffectPower: skillActionSkillAllDouble, triggerPossibility: skillActionSkillAllDouble);
        UnitSkillMagnificationClass skillMagnificationOffenseDoubleTriggerTriple = new UnitSkillMagnificationClass(offenseEffectPower: skillActionSkillAllDouble, triggerPossibility: skillActionSkillAllTriple);

        //Skills
        TriggerBaseClass triggerPossibilityNone = new TriggerBaseClass(possibilityBaseRate: 0.0, possibilityWeight: 0, accumulationReference: ReferenceStatistics.none, accumulationBaseRate: 0.0, accumulationWeight: 0.0);
        TriggerBaseClass triggerPossibilityBasic = new TriggerBaseClass(possibilityBaseRate: 0.149, possibilityWeight: 15, accumulationReference: ReferenceStatistics.none, accumulationBaseRate: 0, accumulationWeight: 0.0);
        TriggerBaseClass triggerPossibilityNormal = new TriggerBaseClass(possibilityBaseRate: 0.122, possibilityWeight: 6, accumulationReference: ReferenceStatistics.none, accumulationBaseRate: 0.0, accumulationWeight: 0.0);
        TriggerBaseClass triggerPossibilityExpert = new TriggerBaseClass(possibilityBaseRate: 0.060, possibilityWeight: 2, accumulationReference: ReferenceStatistics.none, accumulationBaseRate: 0.0, accumulationWeight: 0.0);
        TriggerBaseClass triggerPossibilityMaster = new TriggerBaseClass(possibilityBaseRate: 0.002, possibilityWeight: 1, accumulationReference: ReferenceStatistics.none, accumulationBaseRate: 0.0, accumulationWeight: 0.0);
        TriggerBaseClass triggerPossibility100 = new TriggerBaseClass(possibilityBaseRate: 1.0, possibilityWeight: 0, accumulationReference: ReferenceStatistics.none, accumulationBaseRate: 0.0, accumulationWeight: 0.0);
        TriggerBaseClass triggerAccumulationMiddle = new TriggerBaseClass(possibilityBaseRate: 1.0, possibilityWeight: 0, accumulationReference: ReferenceStatistics.AvoidCount, accumulationBaseRate: 10.0, accumulationWeight: 1.5);
        TriggerBaseClass triggerAccumulationHit = new TriggerBaseClass(possibilityBaseRate: 1.0, possibilityWeight: 0, accumulationReference: ReferenceStatistics.AllTotalBeenHitCount, accumulationBaseRate: 10.0, accumulationWeight: 1.5);

        SkillMagnificationClass magnificationNone = new SkillMagnificationClass(attackTarget: TargetType.none, damage: 1.0, kinetic: 1.0, chemical: 1.0,
        thermal: 1.0, heal: 1.0, numberOfAttacks: 1.0, critical: 1.0, accuracy: 1.0, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
        SkillMagnificationClass magnificationNormal = new SkillMagnificationClass(attackTarget: TargetType.multi, damage: 1.0, kinetic: 1.0, chemical: 1.0,
        thermal: 1.0, heal: 1.0, numberOfAttacks: 1.0, critical: 1.0, accuracy: 1.0, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
        SkillMagnificationClass magnificationHeal20 = new SkillMagnificationClass(attackTarget: TargetType.none, damage: 1.0, kinetic: 1.0, chemical: 1.0,
        thermal: 1.0, heal: 2.0, numberOfAttacks: 1.0, critical: 1.0, accuracy: 1.0, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
        SkillMagnificationClass magnificationHeal40 = new SkillMagnificationClass(attackTarget: TargetType.none, damage: 1.0, kinetic: 1.0, chemical: 1.0,
        thermal: 1.0, heal: 4.0, numberOfAttacks: 1.0, critical: 1.0, accuracy: 1.0, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
        SkillMagnificationClass magnificationSingleD05N05CR05AC05 = new SkillMagnificationClass(attackTarget: TargetType.single, damage: 0.5, kinetic: 1.0, chemical: 1.0,
        thermal: 1.0, heal: 3.0, numberOfAttacks: 0.5, critical: 0.5, accuracy: 0.5, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
        SkillMagnificationClass magnificationMultiD075N05CR05AC05 = new SkillMagnificationClass(attackTarget: TargetType.multi, damage: 0.75, kinetic: 1.0, chemical: 1.0,
        thermal: 1.0, heal: 3.0, numberOfAttacks: 0.5, critical: 0.5, accuracy: 0.5, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
        SkillMagnificationClass magnificationMultiD10N05CR05AC075 = new SkillMagnificationClass(attackTarget: TargetType.multi, damage: 0.75, kinetic: 1.0, chemical: 1.0,
        thermal: 1.0, heal: 3.0, numberOfAttacks: 0.5, critical: 0.5, accuracy: 0.75, optimumRangeMin: 1.0, optimumRangeMax: 1.0);
        SkillMagnificationClass magnificationMultiD10N10CR15AC20 = new SkillMagnificationClass(attackTarget: TargetType.multi, damage: 1.00, kinetic: 1.0, chemical: 1.0,
        thermal: 1.0, heal: 3.0, numberOfAttacks: 1.0, critical: 1.5, accuracy: 2.0, optimumRangeMin: 0.5, optimumRangeMax: 2.0);

        TriggerTargetClass triggerTargetNone = new TriggerTargetClass(actionType: ActionType.none, afterAllMoved: false, counter: false, chain: false, reAttack: false, heal: false, move: false,
         optimumRange: Range.any, majestyAttackType: AttackType.any, critical: CriticalOrNot.any, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: false, onlyWhenAvoidMoreThanOnce: false);
        TriggerTargetClass triggerTargetDamageControl = new TriggerTargetClass(actionType: ActionType.any, afterAllMoved: false, counter: true, chain: true, reAttack: true, heal: false, move: true,
         optimumRange: Range.any, majestyAttackType: AttackType.any, critical: CriticalOrNot.any, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: false, onlyWhenAvoidMoreThanOnce: false);
        TriggerTargetClass triggerTargetIndependent = new TriggerTargetClass(actionType: ActionType.any, afterAllMoved: true, counter: false, chain: false, reAttack: false, heal: false, move: false,
         optimumRange: Range.any, majestyAttackType: AttackType.any, critical: CriticalOrNot.any, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: false, onlyWhenAvoidMoreThanOnce: false);
        TriggerTargetClass triggerTargetCounter = new TriggerTargetClass(actionType: ActionType.any, afterAllMoved: true, counter: true, chain: true, reAttack: true, heal: false, move: true,
         optimumRange: Range.within, majestyAttackType: AttackType.any, critical: CriticalOrNot.nonCritical, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: true, onlyWhenAvoidMoreThanOnce: false);
        TriggerTargetClass triggerTargetChainCounter = new TriggerTargetClass(actionType: ActionType.counter, afterAllMoved: false, counter: true, chain: false, reAttack: false, heal: false, move: true,
         optimumRange: Range.any, majestyAttackType: AttackType.any, critical: CriticalOrNot.any, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: false, onlyWhenAvoidMoreThanOnce: false);
        TriggerTargetClass triggerTargetCriticalReAttack = new TriggerTargetClass(actionType: ActionType.any, afterAllMoved: true, counter: false, chain: true, reAttack: false, heal: false, move: true,
         optimumRange: Range.any, majestyAttackType: AttackType.any, critical: CriticalOrNot.critical, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: false, onlyWhenAvoidMoreThanOnce: false);
        TriggerTargetClass triggerTargetInterrupt = new TriggerTargetClass(actionType: ActionType.interrupt, afterAllMoved: false, counter: true, chain: false, reAttack: false, heal: false, move: false,
         optimumRange: Range.any, majestyAttackType: AttackType.any, critical: CriticalOrNot.any, whoCrushed: ActorOrTargetUnit.no, onlyWhenBeenHitMoreThanOnce: false, onlyWhenAvoidMoreThanOnce: false);

        BuffTargetParameterClass buffTargetNone = new BuffTargetParameterClass(targetType: TargetType.none, barrierRemaining: 0, defenseMagnification: 1.0, mobilityMagnification: 1.0,
        attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0, rangeMinCorrection: 0, rangeMaxCorrection: 0);
        BuffTargetParameterClass buffTargetSelf = new BuffTargetParameterClass(targetType: TargetType.self, barrierRemaining: 0, defenseMagnification: 1.0, mobilityMagnification: 1.0,
       attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0, rangeMinCorrection: 0, rangeMaxCorrection: 0);
        BuffTargetParameterClass buffTargetMulti = new BuffTargetParameterClass(targetType: TargetType.multi, barrierRemaining: 0, defenseMagnification: 1.0, mobilityMagnification: 1.0,
       attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0, rangeMinCorrection: 0, rangeMaxCorrection: 0);
        BuffTargetParameterClass buffTargetMultiBarrier = new BuffTargetParameterClass(targetType: TargetType.multi, barrierRemaining: 0, defenseMagnification: 1.0, mobilityMagnification: 1.0,
       attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0, rangeMinCorrection: 0, rangeMaxCorrection: 0);
        BuffTargetParameterClass buffBarrierDefense12 = new BuffTargetParameterClass(targetType: TargetType.none, barrierRemaining: 20, defenseMagnification: 1.1, mobilityMagnification: 1.0,
       attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0, rangeMinCorrection: 0, rangeMaxCorrection: 0);
        BuffTargetParameterClass buffBarrier10 = new BuffTargetParameterClass(targetType: TargetType.none, barrierRemaining: 6, defenseMagnification: 1.05, mobilityMagnification: 1.0,
       attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0, rangeMinCorrection: 0, rangeMaxCorrection: 0);

        DebuffTargetParameterClass debuffTargetNone = new DebuffTargetParameterClass(targetType: TargetType.none, barrierRemaining: 0, defenseMagnification: 1.0, mobilityMagnification: 1.0,
        attackMagnification: 1.0, accuracyMagnification: 1.0, criticalHitRateMagnification: 1.0, numberOfAttackMagnification: 1.0);

        skillsMasters[0] = new SkillsMasterStruct(name: SkillName.BarrierCounterAvoidManyTimes, actionType: ActionType.counter, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 3, veiledTurn: 20, ability: Ability.generation,
         triggerBase: triggerAccumulationMiddle, magnification: magnificationNone, triggerTarget: triggerTargetCounter, buffTarget: buffTargetSelf, callingBuffName: SkillName.Buffdefense12,
             debuffTarget: debuffTargetNone);
        skillsMasters[1] = new SkillsMasterStruct(name: SkillName.CounterNonCriticalAttack, actionType: ActionType.counter, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 6, veiledTurn: 20, ability: Ability.responsiveness,
         triggerBase: triggerPossibilityBasic, magnification: magnificationSingleD05N05CR05AC05,
             triggerTarget: triggerTargetCounter, buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);
        skillsMasters[2] = new SkillsMasterStruct(name: SkillName.ChainAllysCounter, actionType: ActionType.chain, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 6, veiledTurn: 20, ability: Ability.responsiveness,
         triggerBase: triggerPossibilityNormal, magnification: magnificationMultiD075N05CR05AC05, triggerTarget: triggerTargetChainCounter, buffTarget: buffTargetNone, callingBuffName: SkillName.none,
         debuffTarget: debuffTargetNone);
        skillsMasters[3] = new SkillsMasterStruct(name: SkillName.FutureSightShot, actionType: ActionType.move, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 6, veiledTurn: 20, ability: Ability.power,
         triggerBase: triggerPossibilityNormal, magnification: magnificationMultiD10N10CR15AC20, triggerTarget: triggerTargetIndependent, buffTarget: buffTargetNone, callingBuffName: SkillName.none,
         debuffTarget: debuffTargetNone);
        skillsMasters[4] = new SkillsMasterStruct(name: SkillName.ReAttackAfterCritical, actionType: ActionType.reAttack, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 6, veiledTurn: 20, ability: Ability.power,
         triggerBase: triggerPossibilityExpert, magnification: magnificationMultiD10N05CR05AC075, triggerTarget: triggerTargetCriticalReAttack,
         buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);
        skillsMasters[5] = new SkillsMasterStruct(name: SkillName.InterruptTargetCounterReduceAccuracy, actionType: ActionType.interrupt, callSkillLogicName: CallSkillLogicName.ReduceAccuracy, isHeal: false, usageCount: 4, veiledTurn: 20, ability: Ability.intelligence,
        triggerBase: triggerPossibilityMaster, magnification: magnificationNone, triggerTarget: triggerTargetInterrupt, buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);
        // interrupt skill needs coding..
        skillsMasters[6] = new SkillsMasterStruct(name: SkillName.ShiledHealAll, actionType: ActionType.move, callSkillLogicName: CallSkillLogicName.ShieldHealMulti, isHeal: false, usageCount: 1, veiledTurn: 20, ability: Ability.generation,
         triggerBase: triggerPossibility100, magnification: magnificationNone, triggerTarget: triggerTargetDamageControl, buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);

        skillsMasters[11] = new SkillsMasterStruct(name: SkillName.ShiledHealplusSingle, actionType: ActionType.move, callSkillLogicName: CallSkillLogicName.ShieldHealSingle, isHeal: true, usageCount: 2, veiledTurn: 20, ability: Ability.generation,
         triggerBase: triggerPossibility100, magnification: magnificationHeal40, triggerTarget: triggerTargetDamageControl, buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);
        skillsMasters[12] = new SkillsMasterStruct(name: SkillName.ShiledHealSingle, actionType: ActionType.move, callSkillLogicName: CallSkillLogicName.ShieldHealSingle, isHeal: true, usageCount: 3, veiledTurn: 20, ability: Ability.generation,
         triggerBase: triggerPossibility100, magnification: magnificationHeal20, triggerTarget: triggerTargetDamageControl, buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);

        skillsMasters[13] = new SkillsMasterStruct(name: SkillName.BarrierAll, actionType: ActionType.atBeginning, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 2, veiledTurn: 20, ability: Ability.none,
        triggerBase: triggerPossibilityNormal, magnification: magnificationNone, triggerTarget: triggerTargetIndependent, buffTarget: buffTargetMultiBarrier, callingBuffName: SkillName.Buffbarrier10, debuffTarget: debuffTargetNone);

        // Special Normal attack skill
        skillsMasters[14] = new SkillsMasterStruct(name: SkillName.normalAttack, actionType: ActionType.none, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 1000, veiledTurn: 20, ability: Ability.none,
         triggerBase: triggerPossibility100, magnification: magnificationNormal, triggerTarget: triggerTargetNone, buffTarget: buffTargetNone, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone);

        //Buff
        buffMasters.Add(new SkillsMasterStruct(name: SkillName.Buffdefense12, actionType: ActionType.none, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 0, veiledTurn: 5, ability: Ability.none,
        triggerBase: triggerPossibilityNone, magnification: magnificationNone, triggerTarget: triggerTargetNone, buffTarget: buffBarrierDefense12, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone));
        buffMasters.Add(new SkillsMasterStruct(name: SkillName.Buffbarrier10, actionType: ActionType.none, callSkillLogicName: CallSkillLogicName.none, isHeal: false, usageCount: 0, veiledTurn: 5, ability: Ability.none,
        triggerBase: triggerPossibilityNone, magnification: magnificationNone, triggerTarget: triggerTargetNone, buffTarget: buffBarrier10, callingBuffName: SkillName.none, debuffTarget: debuffTargetNone));


        //------------------------Battle Main Engine------------------------
        //Logic: Number of Battle
        for (int battleWavesSet = 1; battleWavesSet <= battleWavesSets; battleWavesSet++)
        {
            //set up set info
            List<StatisticsReporterFirstBloodClass> statisticsReporterFirstBlood = new List<StatisticsReporterFirstBloodClass>();
            WhichWin[] statisticsReporterWhichWins = new WhichWin[battleWaves];

            if (battleWavesSet > 1) //  magnification per wave set
            { allyAttackMagnification *= (1 + allyAttackMagnificationPerWavesSet); allyDefenseMagnification *= (1 + allyDefenseMagnificationPerWavesSet); }


            CombatClass[] combats = new CombatClass[numberOfCharacters];
            //Ally info
            combats[0] = new CombatClass(shiledCurrent: 5000, shiledMax: 5000, hitPointCurrent: 10000, hitPointMax: 10000,
             attack: (int)(542 * allyAttackMagnification), kineticAttackRatio: 0.5, chemicalAttackRatio: 0.1, thermalAttackRatio: 0.4, criticalHit: 30,
            numberOfAttacks: 20, minRange: 5, maxRange: 6, accuracy: 28344, mobility: 1321, deffense: (int)(700 * allyDefenseMagnification));
            combats[1] = new CombatClass(shiledCurrent: 5000, shiledMax: 5000, hitPointCurrent: 13000, hitPointMax: 13000,
             attack: (int)(682 * allyAttackMagnification), kineticAttackRatio: 0.7, chemicalAttackRatio: 0.0, thermalAttackRatio: 0.3, criticalHit: 30,
            numberOfAttacks: 14, minRange: 3, maxRange: 3, accuracy: 1344, mobility: 1321, deffense: (int)(600 * allyDefenseMagnification));
            combats[2] = new CombatClass(shiledCurrent: 5000, shiledMax: 5000, hitPointCurrent: 9800, hitPointMax: 9800,
             attack: (int)(699 * allyAttackMagnification), kineticAttackRatio: 0.8, chemicalAttackRatio: 0.1, thermalAttackRatio: 0.1, criticalHit: 30,
            numberOfAttacks: 20, minRange: 3, maxRange: 4, accuracy: 1344, mobility: 1321, deffense: (int)(700 * allyDefenseMagnification));
            combats[3] = new CombatClass(shiledCurrent: 5000, shiledMax: 5000, hitPointCurrent: 11200, hitPointMax: 11200,
             attack: (int)(832 * allyAttackMagnification), kineticAttackRatio: 1.0, chemicalAttackRatio: 0.0, thermalAttackRatio: 0.0, criticalHit: 30,
            numberOfAttacks: 9, minRange: 3, maxRange: 4, accuracy: 1344, mobility: 1300, deffense: (int)(600 * allyDefenseMagnification));
            combats[4] = new CombatClass(shiledCurrent: 5000, shiledMax: 5000, hitPointCurrent: 6000, hitPointMax: 6000,
             attack: (int)(592 * allyAttackMagnification), kineticAttackRatio: 0.6, chemicalAttackRatio: 0.4, thermalAttackRatio: 0.0, criticalHit: 30,
            numberOfAttacks: 10, minRange: 3, maxRange: 4, accuracy: 1344, mobility: 1300, deffense: (int)(700 * allyDefenseMagnification));
            combats[5] = new CombatClass(shiledCurrent: 4000, shiledMax: 4000, hitPointCurrent: 7800, hitPointMax: 7800,
             attack: (int)(688 * allyAttackMagnification), kineticAttackRatio: 0.8, chemicalAttackRatio: 0.1, thermalAttackRatio: 0.1, criticalHit: 30,
            numberOfAttacks: 8, minRange: 3, maxRange: 4, accuracy: 1344, mobility: 1321, deffense: (int)(700 * allyDefenseMagnification));
            combats[6] = new CombatClass(shiledCurrent: 7000, shiledMax: 7000, hitPointCurrent: 6190, hitPointMax: 6190,
             attack: (int)(642 * allyAttackMagnification), kineticAttackRatio: 0.3, chemicalAttackRatio: 0.7, thermalAttackRatio: 0.0, criticalHit: 30,
            numberOfAttacks: 6, minRange: 3, maxRange: 4, accuracy: 1344, mobility: 1300, deffense: (int)(700 * allyDefenseMagnification));
            //Enemy info
            combats[7] = new CombatClass(shiledCurrent: 7000, shiledMax: 7000, hitPointCurrent: 12300, hitPointMax: 12300,
             attack: 800, kineticAttackRatio: 0.0, chemicalAttackRatio: 0.1, thermalAttackRatio: 0.9, criticalHit: 30,
             numberOfAttacks: 12, minRange: 3, maxRange: 7, accuracy: 1344, mobility: 1200, deffense: 700);
            combats[8] = new CombatClass(shiledCurrent: 6000, shiledMax: 6000, hitPointCurrent: 13400, hitPointMax: 13400,
             attack: 880, kineticAttackRatio: 0.0, chemicalAttackRatio: 0.0, thermalAttackRatio: 1.0, criticalHit: 30,
            numberOfAttacks: 11, minRange: 3, maxRange: 7, accuracy: 1344, mobility: 1321, deffense: 700);
            combats[9] = new CombatClass(shiledCurrent: 6000, shiledMax: 6000, hitPointCurrent: 13100, hitPointMax: 13100,
             attack: 482, kineticAttackRatio: 1.0, chemicalAttackRatio: 0.0, thermalAttackRatio: 0.0, criticalHit: 30,
            numberOfAttacks: 20, minRange: 3, maxRange: 7, accuracy: 4344, mobility: 1300, deffense: 700);
            combats[10] = new CombatClass(shiledCurrent: 5000, shiledMax: 5000, hitPointCurrent: 9840, hitPointMax: 9840,
             attack: 742, kineticAttackRatio: 1.0, chemicalAttackRatio: 0.0, thermalAttackRatio: 0.0, criticalHit: 30,
            numberOfAttacks: 5, minRange: 3, maxRange: 7, accuracy: 1344, mobility: 1300, deffense: 700);
            combats[11] = new CombatClass(shiledCurrent: 5000, shiledMax: 5000, hitPointCurrent: 7640, hitPointMax: 7640,
             attack: 732, kineticAttackRatio: 1.0, chemicalAttackRatio: 0.0, thermalAttackRatio: 0.0, criticalHit: 30,
            numberOfAttacks: 3, minRange: 3, maxRange: 7, accuracy: 1344, mobility: 1321, deffense: 700);
            combats[12] = new CombatClass(shiledCurrent: 4500, shiledMax: 4500, hitPointCurrent: 5600, hitPointMax: 5600,
             attack: 712, kineticAttackRatio: 1.0, chemicalAttackRatio: 0.0, thermalAttackRatio: 0.0, criticalHit: 30,
            numberOfAttacks: 7, minRange: 3, maxRange: 7, accuracy: 1344, mobility: 1300, deffense: 700);
            combats[13] = new CombatClass(shiledCurrent: 7500, shiledMax: 7500, hitPointCurrent: 6210, hitPointMax: 6210,
             attack: 682, kineticAttackRatio: 0.8, chemicalAttackRatio: 0.0, thermalAttackRatio: 0.2, criticalHit: 30,
            numberOfAttacks: 6, minRange: 3, maxRange: 7, accuracy: 1344, mobility: 1210, deffense: 700);

            FeatureClass featureNormal;
            FeatureClass featureMedic;

            for (int i = 0; i <= 5; i++)
            {
                featureNormal = new FeatureClass(absorbShieldInitial: 0.07, damageControlAssist: false, hateInitial: 10, hateMagnificationPerTurn: 0.666);
                characters.Add(new BattleUnit(uniqueID: i, name: "PIG" + (i + 1).ToString() + "-" + skillsMasters[i].Name.ToString().Substring(0, 7), affiliation: Affiliation.ally, unitType: UnitType.robot,
                 ability: abilities[i], combat: combats[i], feature: featureNormal,
                     offenseMagnification: offenseMagnification, defenseMagnification: defenseMagnification, skillMagnification: skillMagnificationOffenseDoubleTriggerTriple));
            }
            //Medic only  number 6
            for (int i = 6; i <= 6; i++)
            {
                featureMedic = new FeatureClass(absorbShieldInitial: 0.0, damageControlAssist: true, hateInitial: 0, hateMagnificationPerTurn: 0.500);
                characters.Add(new BattleUnit(uniqueID: i, name: "PIG" + (i + 1).ToString() + "-MedicHe", affiliation: Affiliation.ally, unitType: UnitType.robot,
                 ability: abilities[i], combat: combats[i], feature: featureMedic,
                     offenseMagnification: offenseMagnification, defenseMagnification: defenseMagnification, skillMagnification: skillMagnificationOffenseDoubleTriggerTriple));
            }
            for (int i = 7; i <= 12; i++)
            {
                featureNormal = new FeatureClass(absorbShieldInitial: 0.07, damageControlAssist: false, hateInitial: 10, hateMagnificationPerTurn: 0.666);
                // pigs skill has 8 so  skillsMasters [i - 6 -1 ] collect
                characters.Add(new BattleUnit(uniqueID: i, name: "ELD" + (i - 6).ToString() + "-" + skillsMasters[i - 7].Name.ToString().Substring(0, 7), affiliation: Affiliation.enemy, unitType: UnitType.cyborg,
                 ability: abilities[i], combat: combats[i], feature: featureNormal,
                offenseMagnification: offenseMagnification, defenseMagnification: defenseMagnification, skillMagnification: skillMagnificationOffenseDoubleTriggerTriple));
            }
            //Medic only  number 13
            for (int i = 13; i <= 13; i++)
            {
                featureMedic = new FeatureClass(absorbShieldInitial: 0.0, damageControlAssist: true, hateInitial: 0, hateMagnificationPerTurn: 0.500);
                characters.Add(new BattleUnit(uniqueID: i, name: "ELD" + (i - 6).ToString() + "-MedicHe", affiliation: Affiliation.enemy, unitType: UnitType.cyborg,
                 ability: abilities[i], combat: combats[i], feature: featureMedic,
                offenseMagnification: offenseMagnification, defenseMagnification: defenseMagnification, skillMagnification: skillMagnificationOffenseDoubleTriggerTriple));
            }

            allyWinCount = 0; enemyWinCount = 0; drawCount = 0; // Initialize

            for (int battleWave = 1; battleWave <= battleWaves; battleWave++)
            {
                bool allyFirstBlood = false;
                bool enemyFirstBlood = false; // Set up Phase
                statisticsReporterFirstBlood.Add(new StatisticsReporterFirstBloodClass(battleWave: battleWave));

                for (int i = 0; i < numberOfCharacters; i++) //Shiled, HitPoint initialize
                {
                    characters[i].Combat.ShiledCurrent = characters[i].Combat.ShiledMax;
                    characters[i].Combat.HitPointCurrent = characters[i].Combat.HitPointMax;
                    characters[i].Deterioration = 0.0; //Deterioration initialize to 0.0
                    characters[i].Buff.InitializeBuff(); //Buff initialize
                    characters[i].Buff.BarrierRemaining = 0; //Barrier initialize
                    characters[i].Feature.InitializeFeature(); //Feature initialize
                    characters[i].Statistics.Initialize(); // Initialise
                }
                //foreach (EffectClass effect in effects) { effect.InitializeAccumulation(); }

                EffectInitialize(effects: effects, skillsMasters: skillsMasters, characters: characters); //Effect/Buff initialize

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
                        for (int i = 0; i < numberOfCharacters; i++) { characters[i].Buff.InitializeBuff(); }
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
                                string log = null; int orderNumber = 0;
                                BattleLogClass battleLog;
                                OrderConditionClass orderCondition;

                                switch (actionPhase)
                                {
                                    case 0: //Battle conditions output
                                        environmentInfo.Phase = 0;
                                        log += "\n";
                                        //log += "------------------------------------\n";
                                        text = new FuncBattleConditionsText(currentTurn: turn, currentBattleWaves: currentBattleWaves, characters: characters);
                                        log += text.Text();
                                        orderCondition = new OrderConditionClass(wave: environmentInfo.Wave, turn: environmentInfo.Turn, phase: environmentInfo.Phase, orderNumber: 0, nest: 0, nestOrderNumber: 0);
                                        battleLog = new BattleLogClass(orderCondition: orderCondition, isNavigation: false, log: log, importance: 1);
                                        battleLogList.Add(battleLog);

                                        break;
                                    case 1: // Action phase:1 at beginning
                                        environmentInfo.Phase = 1; orderNumber = 0;
                                        log += new string(' ', 1) + "[At beging phase] \n";
                                        orderCondition = new OrderConditionClass(wave: environmentInfo.Wave, turn: environmentInfo.Turn, phase: environmentInfo.Phase, orderNumber: orderNumber, nest: 0, nestOrderNumber: 0);
                                        battleLog = new BattleLogClass(orderCondition: orderCondition, isNavigation: false, log: log, importance: 1);
                                        battleLogList.Add(battleLog);

                                        // _/_/_/_/_/_/_/_/ At Beginning Skill _/_/_/_/_/_/_/_/_/_/

                                        skillTriggerPossibilityCheck = SkillTriggerPossibilityCheck(actor: null, effects: effects, characters: characters, attackerOrder: null,
                                        orders: orders, actionType: ActionType.atBeginning, shouldHeal: false, isDamageControlAssist: false, battleResult: null, individualTargetID: 0, nestNumber: 0, environmentInfo: environmentInfo);
                                        while (skillTriggerPossibilityCheck != null && skillTriggerPossibilityCheck.Count > 0) { orders.Push(skillTriggerPossibilityCheck.Pop()); }
                                        break;
                                    case 2:
                                        environmentInfo.Phase = 2;
                                        log += new string(' ', 1) + "[Main action phase] \n";
                                        orderCondition = new OrderConditionClass(wave: environmentInfo.Wave, turn: environmentInfo.Turn, phase: environmentInfo.Phase, orderNumber: 0, nest: 0, nestOrderNumber: 0);
                                        battleLog = new BattleLogClass(orderCondition: orderCondition, isNavigation: false, log: log, importance: 1);
                                        battleLogList.Add(battleLog);

                                        for (int i = 0; i <= aliveCharacters.Count - 1; i++)
                                        {
                                            List<EffectClass> effectList = effects.FindAll((obj) => obj.Character == aliveCharacters[i] && obj.Skill.ActionType == ActionType.move
                                            && obj.UsageCount > 0 && obj.VeiledFromTurn <= turn && obj.VeiledToTurn >= turn);
                                            orderCondition = new OrderConditionClass(wave: environmentInfo.Wave, turn: environmentInfo.Turn, phase: environmentInfo.Phase, orderNumber: 0, nest: 0, nestOrderNumber: 0);

                                            // Add normal attack skills
                                            EffectClass normalAttackEffect = new EffectClass(character: aliveCharacters[i], skill: skillsMasters[14], actionType: ActionType.normalAttack, offenseEffectMagnification: 1.0,
                                            triggeredPossibility: 1.0, isDamageControlAssistAble: false, usageCount: 1000, veiledFromTurn: 1, veiledToTurn: 20);
                                            effectList.Add(normalAttackEffect);
                                            orderForSort.Add(new OrderClass(orderCondition: orderCondition, actor: aliveCharacters[i], actionType: ActionType.move, skillEffectProposed: ref effectList,
                                            actionSpeed: (aliveCharacters[i].Ability.Responsiveness * r.Next(40 + aliveCharacters[i].Ability.Luck, 100)), individualTargetID: -1, isDamageControlAssist: false));
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
                                        ShiledHealFunction shiledHeal = new ShiledHealFunction(characters: characters);
                                        log += shiledHeal.Log;
                                        CalculationHateMagnificationPerTurnFunction camlHate = new CalculationHateMagnificationPerTurnFunction(characters: characters);
                                        log += camlHate.Log;

                                        orderCondition = new OrderConditionClass(wave: environmentInfo.Wave, turn: environmentInfo.Turn, phase: environmentInfo.Phase, orderNumber: 0, nest: 0, nestOrderNumber: 0);
                                        battleLog = new BattleLogClass(orderCondition: orderCondition, isNavigation: false, log: log, importance: 1);
                                        battleLogList.Add(battleLog);

                                        break;
                                }
                            }
                            //------------------------Action phase------------------------
                            //Action for each character by action order.

                            while (orders.Any()) // loop until order is null.
                            {
                                string log = null;
                                OrderClass order = orders.Pop();

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

                                log += SkillLogicDispatcher(order: order, characters: characters, environmentInfo: environmentInfo); // SkillLogic action include damage control assist.
                                log += BuffDebuffFunction(order: order, characters: characters, effects: effects, buffMasters: buffMasters, turn: turn); //Buff, debuff action
                                result = SkillMoveFunction(order: order, characters: characters, environmentInfo: environmentInfo); // offense action
                                if (result.BattleLog != null) { log += result.BattleLog.Log; }

                                battleResult = result.battleResult;

                                if (order.IsDamageControlAssist) //only when Damage Control Assist
                                {
                                    List<OrderClass> deleteOneActionOrderrIfHave = orders.ToList();
                                    OrderClass deleteOneActionOrderRaw = deleteOneActionOrderrIfHave.FindLast(obj => obj.Actor == order.Actor && obj.ActionType == ActionType.move);
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
                                    if (battleResult.IsAllyWin == true) { allyWinCount++; statisticsReporterWhichWins[battleWave - 1] = WhichWin.allyWin; }
                                    if (battleResult.IsEnemyWin) { enemyWinCount++; statisticsReporterWhichWins[battleWave - 1] = WhichWin.enemyWin; }
                                    if (battleResult.IsDraw) { drawCount++; statisticsReporterWhichWins[battleWave - 1] = WhichWin.Draw; }
                                }

                                OrderStatusClass orderStatus = new OrderStatusClass(); orderStatus.Initialize();

                                int nestNumber = 0;
                                //[[ SKILLS CHECK ]] Damage Control Assist trigger. Note: ActionType independent so ActionType.any!
                                Stack<OrderClass> damageControlAssistStack = SkillTriggerPossibilityCheck(actor: null, effects: effects, characters: characters,
                                    attackerOrder: order, orders: orders, actionType: ActionType.any, shouldHeal: true, isDamageControlAssist: true,
                                        battleResult: battleResult, individualTargetID: order.Actor.UniqueID, nestNumber: nestNumber, environmentInfo: environmentInfo);

                                //[[ SKILLS CHECK ]] ReAttack skills trigger.
                                Stack<OrderClass> reAttackStack = SkillTriggerPossibilityCheck(actor: null, effects: effects, characters: characters,
                                    attackerOrder: order, orders: orders, actionType: ActionType.reAttack, shouldHeal: false, isDamageControlAssist: false,
                                        battleResult: battleResult, individualTargetID: order.Actor.UniqueID, nestNumber: nestNumber, environmentInfo: environmentInfo);

                                //[[ SKILLS CHECK ]] Chain skills trigger.
                                Stack<OrderClass> chainStack = SkillTriggerPossibilityCheck(actor: null, effects: effects, characters: characters,
                                     attackerOrder: order, orders: orders, actionType: ActionType.chain, shouldHeal: false, isDamageControlAssist: false,
                                      battleResult: battleResult, individualTargetID: order.Actor.UniqueID, nestNumber: nestNumber, environmentInfo: environmentInfo);

                                //[[ SKILLS CHECK ]] Counter skills triger.
                                Stack<OrderClass> counterStack = SkillTriggerPossibilityCheck(actor: null, effects: effects, characters: characters,
                                 attackerOrder: order, orders: orders, actionType: ActionType.counter, shouldHeal: false, isDamageControlAssist: false,
                                  battleResult: battleResult, individualTargetID: order.Actor.UniqueID, nestNumber: nestNumber, environmentInfo: environmentInfo);


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

                                BattleLogClass battleLog = new BattleLogClass(orderCondition: order.OrderCondition, isNavigation: false, log: log, importance: 1);
                                battleLogList.Add(battleLog);

                                //Navigation Logic
                                string navigationLog = null;
                                NavigatorSpeechAfterMoveClass navigatorSpeechAfterMove = new NavigatorSpeechAfterMoveClass(navigatorName: navigatorName, order: order,
                                    characters: characters, effects: effects, orderStatus: orderStatus, environmentInfo: environmentInfo);

                                navigationLog = navigatorSpeechAfterMove.Log;
                                if (navigationLog != null)
                                {
                                    //navigationLog += new string(' ', 2) + "-------------\n";
                                    battleLog = new BattleLogClass(orderCondition: order.OrderCondition, isNavigation: true, log: navigationLog, importance: 1);
                                    Debug.Log("in navigationlog is not null :" + battleLog.Log);
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
                            if (wipeOutCheck.IsAllyWin) { allyWinCount++; }
                            if (wipeOutCheck.IsEnemyWin) { enemyWinCount++; }
                            if (wipeOutCheck.IsDraw) { drawCount++; }
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

                        string log = "Time over. \n";
                        OrderConditionClass orderCondition = new OrderConditionClass(wave: environmentInfo.Wave, turn: environmentInfo.Turn, phase: 4, orderNumber: 0, nest: 0, nestOrderNumber: 0);
                        BattleLogClass battleLog = new BattleLogClass(orderCondition: orderCondition, isNavigation: false, log: log, importance: 1);
                        battleLogList.Add(battleLog);
                    }

                } //battleEnd 

                for (int i = 0; i < numberOfCharacters; i++) //Shiled, HitPoint initialize
                { characters[i].SetPermanentStatistics(statistics: characters[i].Statistics); }// set permanent statistics before initialize statistics.
            } //Battle waves

            subLogPerWavesSets[battleWavesSet - 1] += "[Set:" + battleWavesSet + "] Battle count:" + (allyWinCount + enemyWinCount + drawCount) + " Win:" + (allyWinCount) + " lost:" + (enemyWinCount)
                + " Win Ratio:" + (int)((double)allyWinCount / (double)(allyWinCount + enemyWinCount + drawCount) * 100)
                + "% Ally:[Attack x" + Math.Round(allyAttackMagnification, 2) + "] [Defense x" + Math.Round(allyDefenseMagnification, 2) + "] \n";
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
                logPerWavesSets[battleWavesSet - 1] += "[Best shot]" + bestFirstBloodAlly.AllyContentText + " " + bestFirstBloodAlly.BattleLogAlly.OrderCondition + "\n";
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
        List<BattleLogClass> battleLogDisplayList = battleLogList.FindAll(obj => obj.OrderCondition.Wave == battleWaves); // only last battlelog displayed.
        foreach (BattleLogClass battleLog in battleLogDisplayList)
        {

            if (battleLog.IsNavigation == false)
            { finalLog += "(" + battleLog.OrderCondition.Phase + "-" + battleLog.OrderCondition.OrderNumber + "-" + battleLog.OrderCondition.Nest + "-" + battleLog.OrderCondition.NestOrderNumber + ")" + battleLog.Log; }
            else { finalLog += new string(' ', 5) + battleLog.Log; }
        }

        //finalLog += "------------------------------------\n";
        finalLog += "Battle is over. ";
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
        logList = battleLogDisplayList;
    }

    // Skill check method
    public static Stack<OrderClass> SkillTriggerPossibilityCheck(BattleUnit actor, List<EffectClass> effects, List<BattleUnit> characters, OrderClass attackerOrder,
    Stack<OrderClass> orders, ActionType actionType, bool shouldHeal, bool isDamageControlAssist,
        BattleResultClass battleResult, int individualTargetID, int nestNumber, EnvironmentInfoClass environmentInfo)
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
            case ActionType.move: //Normal moveskill logic: only actor should trigger moveskill.
                matchedActionTypeEffects = rawActionTypeEffects.FindAll(obj => obj.Character == actor);
                break;
            case ActionType.counter:
                if (attackerOrder.Actor.Affiliation == Affiliation.ally) { affiliationWhoWillAct = Affiliation.enemy; }
                else if (attackerOrder.Actor.Affiliation == Affiliation.enemy) { affiliationWhoWillAct = Affiliation.ally; }
                matchedActionTypeEffects = rawActionTypeEffects.FindAll((obj) => obj.Character.Affiliation == affiliationWhoWillAct);
                break;
            case ActionType.chain:
                matchedActionTypeEffects = rawActionTypeEffects.FindAll((obj) => obj.Character.Affiliation == attackerOrder.Actor.Affiliation && obj.Character != attackerOrder.Actor);
                break;
            case ActionType.reAttack:
                matchedActionTypeEffects = rawActionTypeEffects.FindAll((obj) => obj.Character == attackerOrder.Actor);
                break;
            case ActionType.atBeginning:
                matchedActionTypeEffects = rawActionTypeEffects;
                break;
            case ActionType.any: //[Damage Control Assist skill logic]. ActionType independent so DCA is in ActionType.any.
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
            if (effect.Skill.TriggerTarget.ActionType != ActionType.any)
            {
                if ((effect.Skill.TriggerTarget.ActionType == attackerOrder.ActionType) == false)
                { effect.IsntTriggeredBecause.TriggerCondition = true; continue; }
            } // Trigger condition check
            if (effect.Skill.ActionType != ActionType.move && effect.Skill.TriggerTarget.AfterAllMoved == false) // check normal Attack left, except move skill. (move skill is itself so check this logic make no sense )
            {
                List<OrderClass> checkOrders = orders.ToList();
                if (checkOrders.FindLast((obj) => obj.Actor == effect.Character && obj.SkillEffectChosen.Skill.Name == SkillName.normalAttack) == null)
                { effect.IsntTriggeredBecause.AfterAllMoved = true; continue; }// no normalAttack left, whitch means no action.   
            }


            if (effect.Skill.ActionType == ActionType.move && effect.IsDamageControlAssistAble) //Damage Control Assist Special Logic....
            {
                // check normal Attack left, except move skill. (move skill is itself so check this logic make no sense )
                if (effect.Skill.TriggerTarget.AfterAllMoved == false)
                {
                    List<OrderClass> checkActionOrders = orders.ToList();
                    if (checkActionOrders.FindLast((obj) => obj.Actor == effect.Character && obj.ActionType == ActionType.move) == null)
                    { effect.IsntTriggeredBecause.AfterAllMoved = true; continue; }// no normalAttack left, whitch means no action.   
                }
            }

            if (attackerOrder != null) // only attackOrder exist, check
            {
                if (effect.Skill.TriggerTarget.Counter == false && attackerOrder.ActionType == ActionType.counter)
                { effect.IsntTriggeredBecause.TriggerTargetCounter = true; continue; } // counter reaction
                if (effect.Skill.TriggerTarget.Chain == false && attackerOrder.ActionType == ActionType.chain)
                { effect.IsntTriggeredBecause.TriggerTargetChain = true; continue; } // chain reaction
                if (effect.Skill.TriggerTarget.ReAttack == false && attackerOrder.ActionType == ActionType.reAttack)
                { effect.IsntTriggeredBecause.TriggerTargetReAttack = true; continue; } // reAttack reaction
                if (effect.Skill.TriggerTarget.Move == false && attackerOrder.ActionType == ActionType.move)
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

                OrderConditionClass orderCondition = new OrderConditionClass(wave: environmentInfo.Wave, turn: environmentInfo.Turn, phase: environmentInfo.Phase, orderNumber: orderNumber,
                    nest: nest + 1, nestOrderNumber: nestNumber);

                skillsByOrder = new OrderClass(orderCondition: orderCondition, actor: character, actionType: actionType, skillEffectProposed: ref validEffectsPerActor, actionSpeed: 0,
                 individualTargetID: individualTargetID, isDamageControlAssist: isDamageControlAssist);
                skillsByOrderStack.Push(skillsByOrder); nestNumber++;
            }
        }
        if (skillsByOrderStack.Count > 0) { return skillsByOrderStack; }
        return null;
    }

    // Skills Possibility Rate calculation
    public static double TriggerPossibilityRate(SkillsMasterStruct skillsMaster, BattleUnit character)
    {
        double abilityValue = 0.0;
        double magnificationRate = 1.0;
        switch (skillsMaster.Ability)
        {
            case Ability.power: abilityValue = (double)character.Ability.Power; break;
            case Ability.generation: abilityValue = (double)character.Ability.Generation; break;
            case Ability.responsiveness: abilityValue = (double)character.Ability.Responsiveness; break;
            case Ability.intelligence: abilityValue = (double)character.Ability.Intelligence; break;
            case Ability.precision: abilityValue = (double)character.Ability.Precision; break;
            case Ability.luck: abilityValue = (double)character.Ability.Luck; break;
            case Ability.none: abilityValue = 0.0; break;
            default: break;
        }

        switch (skillsMaster.ActionType)
        {
            case ActionType.counter: magnificationRate = character.SkillMagnification.TriggerPossibility.Counter; break;
            case ActionType.chain: magnificationRate = character.SkillMagnification.TriggerPossibility.Chain; break;
            case ActionType.reAttack: magnificationRate = character.SkillMagnification.TriggerPossibility.ReAttack; break;
            case ActionType.move: magnificationRate = character.SkillMagnification.TriggerPossibility.Move; break;
            case ActionType.interrupt: magnificationRate = character.SkillMagnification.TriggerPossibility.Interrupt; break;
            case ActionType.atBeginning: magnificationRate = character.SkillMagnification.TriggerPossibility.AtBeginning; break;
            case ActionType.atEnding: magnificationRate = character.SkillMagnification.TriggerPossibility.AtEnding; break;
            default: break;
        }

        double value = Math.Pow(((skillsMaster.TriggerBase.PossibilityBaseRate + abilityValue /
                 (100.0 + abilityValue * 2 * skillsMaster.TriggerBase.PossibilityWeight)) * 100), 3.0) / 40000 * magnificationRate;
        if (value > 1.0) { value = 1.0; }
        return value;
    }

    // Skills offenseEffectMagnification calculation
    public static double EffectPower(SkillsMasterStruct skillsMaster, BattleUnit character)
    {
        double magnificationRate = 1.0;
        switch (skillsMaster.ActionType)
        {
            case ActionType.counter: magnificationRate = character.SkillMagnification.OffenseEffectPower.Counter; break;
            case ActionType.chain: magnificationRate = character.SkillMagnification.OffenseEffectPower.Chain; break;
            case ActionType.reAttack: magnificationRate = character.SkillMagnification.OffenseEffectPower.ReAttack; break;
            case ActionType.move: magnificationRate = character.SkillMagnification.OffenseEffectPower.Move; break;
            case ActionType.interrupt: magnificationRate = character.SkillMagnification.OffenseEffectPower.Interrupt; break;
            case ActionType.atBeginning: magnificationRate = character.SkillMagnification.OffenseEffectPower.AtBeginning; break;
            case ActionType.atEnding: magnificationRate = character.SkillMagnification.OffenseEffectPower.AtEnding; break;
            default: break;
        }
        return 1.0 * magnificationRate;
    }

    public static void EffectInitialize(List<EffectClass> effects, SkillsMasterStruct[] skillsMasters, List<BattleUnit> characters)
    {
        double[] TriggerPossibility = new double[characters.Count];
        double[] OffenseEffectMagnification = new double[characters.Count];
        //Effect/Buff initialize
        effects.Clear();
        //for (int i = effects.Count - 1; i >= 0; i--) { effects.RemoveAt(i); }

        for (int i = 0; i < 7; i++)
        {
            TriggerPossibility[i] = TriggerPossibilityRate(skillsMaster: skillsMasters[i], character: characters[i]);
            OffenseEffectMagnification[i] = EffectPower(skillsMaster: skillsMasters[i], character: characters[i]);
            EffectClass effect = new EffectClass(character: characters[i], skill: skillsMasters[i], actionType: skillsMasters[i].ActionType,
             offenseEffectMagnification: OffenseEffectMagnification[i],
            triggeredPossibility: TriggerPossibility[i], isDamageControlAssistAble: false, usageCount: skillsMasters[i].UsageCount, veiledFromTurn: 1, veiledToTurn: 20);
            effects.Add(effect);
        }
        for (int i = 7; i < 14; i++)
        {
            TriggerPossibility[i] = TriggerPossibilityRate(skillsMaster: skillsMasters[i - 7], character: characters[i]);
            OffenseEffectMagnification[i] = EffectPower(skillsMaster: skillsMasters[i - 7], character: characters[i]);
            EffectClass effect = new EffectClass(character: characters[i], skill: skillsMasters[i - 7], actionType: skillsMasters[i - 7].ActionType,
             offenseEffectMagnification: OffenseEffectMagnification[i],
            triggeredPossibility: TriggerPossibility[i], isDamageControlAssistAble: false, usageCount: skillsMasters[i - 7].UsageCount, veiledFromTurn: 1, veiledToTurn: 20);
            effects.Add(effect);
        }

        //Special add Eld4 to counter skill
        EffectClass secondEffect = new EffectClass(character: characters[10], skill: skillsMasters[1], actionType: skillsMasters[1].ActionType,
offenseEffectMagnification: OffenseEffectMagnification[10],
triggeredPossibility: TriggerPossibilityRate(skillsMaster: skillsMasters[1], character: characters[10]), isDamageControlAssistAble: false, usageCount: skillsMasters[1].UsageCount, veiledFromTurn: 1, veiledToTurn: 20);
        effects.Add(secondEffect);

        //Special add to PIG7 and ELD7 to ShiledHealAll, ShiledHealSingle and ShiledHealPlusSingle
        //ShiledHealAll
        secondEffect = new EffectClass(character: characters[6], skill: skillsMasters[13], actionType: skillsMasters[13].ActionType,
offenseEffectMagnification: OffenseEffectMagnification[6],
triggeredPossibility: TriggerPossibilityRate(skillsMaster: skillsMasters[13], character: characters[6]), isDamageControlAssistAble: true, usageCount: skillsMasters[13].UsageCount, veiledFromTurn: 1, veiledToTurn: 20);
        effects.Add(secondEffect);
        secondEffect = new EffectClass(character: characters[13], skill: skillsMasters[13], actionType: skillsMasters[13].ActionType,
offenseEffectMagnification: OffenseEffectMagnification[13],
triggeredPossibility: TriggerPossibilityRate(skillsMaster: skillsMasters[13], character: characters[13]), isDamageControlAssistAble: true, usageCount: skillsMasters[13].UsageCount, veiledFromTurn: 1, veiledToTurn: 20);
        effects.Add(secondEffect);
        //ShiledHealSingle
        secondEffect = new EffectClass(character: characters[6], skill: skillsMasters[12], actionType: skillsMasters[12].ActionType,
offenseEffectMagnification: OffenseEffectMagnification[6],
triggeredPossibility: TriggerPossibilityRate(skillsMaster: skillsMasters[12], character: characters[6]), isDamageControlAssistAble: true, usageCount: skillsMasters[12].UsageCount, veiledFromTurn: 1, veiledToTurn: 20);
        effects.Add(secondEffect);
        secondEffect = new EffectClass(character: characters[13], skill: skillsMasters[12], actionType: skillsMasters[12].ActionType,
offenseEffectMagnification: OffenseEffectMagnification[12],
triggeredPossibility: TriggerPossibilityRate(skillsMaster: skillsMasters[12], character: characters[13]), isDamageControlAssistAble: true, usageCount: skillsMasters[12].UsageCount, veiledFromTurn: 1, veiledToTurn: 20);
        effects.Add(secondEffect);
        //ShiledHealPlusSingle
        secondEffect = new EffectClass(character: characters[6], skill: skillsMasters[11], actionType: skillsMasters[11].ActionType,
offenseEffectMagnification: OffenseEffectMagnification[6],
triggeredPossibility: TriggerPossibilityRate(skillsMaster: skillsMasters[11], character: characters[6]), isDamageControlAssistAble: true, usageCount: skillsMasters[11].UsageCount, veiledFromTurn: 1, veiledToTurn: 20);
        effects.Add(secondEffect);
        secondEffect = new EffectClass(character: characters[13], skill: skillsMasters[11], actionType: skillsMasters[12].ActionType,
offenseEffectMagnification: OffenseEffectMagnification[12],
triggeredPossibility: TriggerPossibilityRate(skillsMaster: skillsMasters[11], character: characters[13]), isDamageControlAssistAble: true, usageCount: skillsMasters[11].UsageCount, veiledFromTurn: 1, veiledToTurn: 20);
        effects.Add(secondEffect);
        foreach (EffectClass effect in effects) { effect.BuffToCharacter(currentTurn: 1); }
    }

    // Buff logic
    public static string BuffDebuffFunction(OrderClass order, List<BattleUnit> characters, List<EffectClass> effects, List<SkillsMasterStruct> buffMasters, int turn)
    {
        SkillsMasterStruct addingBuff;
        List<EffectClass> addingEffect = new List<EffectClass>();
        string log = null;
        if (order.SkillEffectChosen == null) { return log; } // no effect exist, so no buff/ debuff happened
        foreach (BattleUnit character in characters) { if (character.IsCrushedJustNow) { character.IsCrushedJustNow = false; } } // reset IsCrushedJustNow flag
        switch (order.SkillEffectChosen.Skill.BuffTarget.TargetType)
        {
            case TargetType.self: //Buff self
                addingBuff = buffMasters.FindLast((obj) => obj.Name == order.SkillEffectChosen.Skill.CallingBuffName);
                addingEffect.Add(new EffectClass(character: order.Actor, skill: addingBuff, actionType: ActionType.none,
                offenseEffectMagnification: 1.0, triggeredPossibility: 0.0, isDamageControlAssistAble: false, usageCount: addingBuff.UsageCount,
                   veiledFromTurn: turn, veiledToTurn: (turn + addingBuff.VeiledTurn)));
                effects.Add(addingEffect[0]);
                addingEffect[0].BuffToCharacter(currentTurn: turn);
                order.Actor.Buff.AddBarrier(addingEffect[0].Skill.BuffTarget.BarrierRemaining);

                string triggerPossibilityText = null;
                if (order.SkillEffectChosen.TriggeredPossibility < 1.0) { triggerPossibilityText = "(Trigger Possibility: " + (double)((int)(order.SkillEffectChosen.TriggeredPossibility * 1000) / 10.0) + "%) "; }
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
                log += new string(' ', 0) + order.Actor.Name + "'s " + order.SkillEffectChosen.Skill.Name + "! " + triggerPossibilityText + accumulationText + "\n"
                          + new string(' ', 3) + order.Actor.Name + " gets " + addingBuff.Name + " which will last " + addingBuff.VeiledTurn + " turns. " + "\n";
                // Belows: It's a temporary message, only for deffense magnification.
                if (addingBuff.BuffTarget.DefenseMagnification > 1.0) { log += new string(' ', 4) + "[Defense: " + order.Actor.Buff.DefenseMagnification + " (x" + addingBuff.BuffTarget.DefenseMagnification + ")] "; }
                if (addingEffect[0].Skill.BuffTarget.BarrierRemaining > 0) { log += "[Barrier:" + order.Actor.Buff.BarrierRemaining + " (+" + addingEffect[0].Skill.BuffTarget.BarrierRemaining + ")] "; }
                log += "\n";
                break;
            case TargetType.multi: //Buff attacker's side all
                addingBuff = buffMasters.FindLast((obj) => obj.Name == order.SkillEffectChosen.Skill.CallingBuffName);
                List<BattleUnit> buffTargetCharacters = characters.FindAll(character1 => character1.Affiliation == order.Actor.Affiliation && character1.Combat.HitPointCurrent > 0);
                log += new string(' ', 0) + order.Actor.Name + "'s " + order.SkillEffectChosen.Skill.Name + "! (Trigger Possibility:" + (double)((int)(order.SkillEffectChosen.TriggeredPossibility * 1000) / 10.0) + "%) \n";

                for (int i = 0; i < buffTargetCharacters.Count; i++)
                {
                    addingEffect.Add(new EffectClass(character: buffTargetCharacters[i], skill: addingBuff, actionType: ActionType.none,
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
        return log;
    }

    public static string SkillLogicDispatcher(OrderClass order, List<BattleUnit> characters, EnvironmentInfoClass environmentInfo)
    {
        string log = null;
        if (order.SkillEffectChosen.Skill.CallSkillLogicName == CallSkillLogicName.none) { return null; } // check call skill 
        SkillLogicShieldHealClass healMulti;
        switch (order.SkillEffectChosen.Skill.CallSkillLogicName)
        {
            case CallSkillLogicName.ShieldHealMulti:
                healMulti = new SkillLogicShieldHealClass(order: order, characters: characters, isMulti: true, environmentInfo: environmentInfo);
                log += healMulti.Log;
                break;
            case CallSkillLogicName.ShieldHealSingle:
                healMulti = new SkillLogicShieldHealClass(order: order, characters: characters, isMulti: false, environmentInfo: environmentInfo);
                log += healMulti.Log;
                break;
            default:
                break;
        }
        return log;
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


