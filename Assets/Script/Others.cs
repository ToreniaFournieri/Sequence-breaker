using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class ObjectExtensions { public static string WithComma(this object self) { return string.Format("{0:#,##0}", self); } } //With comma override Object


//Function, dhisplay battle condition text
public class FuncBattleConditionsText
{
    public FuncBattleConditionsText(int currentTurn, int currentBattleWaves, List<BattleUnit> characters)
    { this.CurrentTurn = currentTurn; this.Characters = characters; this.CurrentBattleWaves = currentBattleWaves; }

    public string Text()
    {
        string text = null;
        text += "Turn " + CurrentTurn + " of " + "wave " + "" + CurrentBattleWaves + "\n";

        // get each affiliation unit list.
        List<BattleUnit> allys = Characters.FindAll(x => x.Affiliation == Affiliation.ally);
        allys.Sort((x, y) => x.UniqueID - y.UniqueID);
        List<BattleUnit> enemys = Characters.FindAll(x => x.Affiliation == Affiliation.enemy);
        enemys.Sort((x, y) => x.UniqueID - y.UniqueID);

        //Display allys info
        for (int j = 0; j < 2; j++)
        {
            List<BattleUnit> affiliationCharacters;
            if (j == 0) { text += "Ally: \n"; affiliationCharacters = allys; } //Display allys info
            else { text += "Enemy: \n"; affiliationCharacters = enemys; } //Display enemys info

            for (int i = 0; i < affiliationCharacters.Count; i++)
            {
                int shiledSpace = (9 - affiliationCharacters[i].Combat.ShiledCurrent.WithComma().Length);
                if (shiledSpace <= 0) { shiledSpace = 1; }
                int hPSpace = (9 - affiliationCharacters[i].Combat.HitPointCurrent.WithComma().Length);
                if (hPSpace <= 0) { hPSpace = 1; }
                int shiledPercentSpace = (3 - Math.Round(((double)affiliationCharacters[i].Combat.ShiledCurrent / (double)affiliationCharacters[i].Combat.ShiledMax * 100), 0).WithComma().Length);
                int hPPercentSpace = (3 - Math.Round(((double)affiliationCharacters[i].Combat.HitPointCurrent / (double)affiliationCharacters[i].Combat.HitPointMax * 100), 0).WithComma().Length);

                string barrierText = null;
                if (affiliationCharacters[i].Buff.BarrierRemaining > 0) { barrierText = "[Barrier:" + affiliationCharacters[i].Buff.BarrierRemaining + "] "; }
                string deteriorationText = null;
                if ((int)(affiliationCharacters[i].Deterioration * 100) >= 1) { deteriorationText = "(Deterioration:" + (int)(affiliationCharacters[i].Deterioration * 100) + "%) "; }
                string defenseText = null;
                if (affiliationCharacters[i].Buff.DefenseMagnification > 1.01) { defenseText = "[Defense: x" + affiliationCharacters[i].Buff.DefenseMagnification + "]"; }

                text += new string(' ', 1) + "#" + i + " " + affiliationCharacters[i].Name
                    + " Shiled:" + new string(' ', shiledSpace) + affiliationCharacters[i].Combat.ShiledCurrent.WithComma() + " ("
                    + new string(' ', shiledPercentSpace) + Math.Round(((double)affiliationCharacters[i].Combat.ShiledCurrent / (double)affiliationCharacters[i].Combat.ShiledMax * 100), 0) + "%)"
                    + " HP:" + new string(' ', hPSpace) + affiliationCharacters[i].Combat.HitPointCurrent.WithComma() + " ("
                    + new string(' ', hPPercentSpace) + Math.Round(((double)affiliationCharacters[i].Combat.HitPointCurrent / (double)affiliationCharacters[i].Combat.HitPointMax * 100), 0) + "%) "
                    + barrierText + deteriorationText + defenseText
                    + "\n";
            }
        }

        //text += "------------------------------------\n";
        return text;
    }
    private int CurrentTurn { get; }
    private int CurrentBattleWaves { get; }
    private List<BattleUnit> Characters { get; }
}


// SkillsMaster should be more Global class..
public struct SkillsMasterStruct
{
    public SkillsMasterStruct(SkillName name, ActionType actionType, CallSkillLogicName callSkillLogicName, bool isHeal, int usageCount, int veiledTurn, Ability ability, TriggerBaseClass triggerBase,
      SkillMagnificationClass magnification, TriggerTargetClass triggerTarget, BuffTargetParameterClass buffTarget, SkillName callingBuffName, DebuffTargetParameterClass debuffTarget)
    {
        this.Name = name; this.ActionType = actionType; this.CallSkillLogicName = callSkillLogicName; this.IsHeal = isHeal; this.UsageCount = usageCount; this.VeiledTurn = veiledTurn;
        this.Ability = ability; this.TriggerBase = triggerBase; this.Magnification = magnification; this.TriggerTarget = triggerTarget;
        this.BuffTarget = buffTarget; this.CallingBuffName = callingBuffName; this.DebuffTarget = debuffTarget;
    }

    public SkillName Name { get; }
    public ActionType ActionType { get; }
    public CallSkillLogicName CallSkillLogicName { get; }
    public bool IsHeal { get; }
    public int UsageCount { get; }
    public int VeiledTurn { get; }
    public Ability Ability { get; }
    public TriggerBaseClass TriggerBase { get; }
    public SkillMagnificationClass Magnification { get; }
    public TriggerTargetClass TriggerTarget { get; }
    public BuffTargetParameterClass BuffTarget { get; }
    public SkillName CallingBuffName { get; }
    public DebuffTargetParameterClass DebuffTarget { get; }
}

//[[Skill logic ]]
public class SkillLogicShieldHealClass
{
    // heal shiled all actor's affiliation characters.
    public SkillLogicShieldHealClass(OrderClass order, List<BattleUnit> characters, bool isMulti, EnvironmentInfoClass environmentInfo)
    {
        string damageControlAssistText = null;
        if (order.IsDamageControlAssist) { damageControlAssistText = "[damage control assist] "; }
        Log += new string(' ', 0) + order.Actor.Name + " " + damageControlAssistText + order.SkillEffectChosen.Skill.Name + " (Left:" + order.SkillEffectChosen.UsageCount + ") \n";
        double healBase = order.Actor.Ability.Generation * order.SkillEffectChosen.Skill.Magnification.Heal * 10.0;

        // heal only same affiliation
        List<BattleUnit> healingCharacters = new List<BattleUnit>();
        if (isMulti)
        {
            if (order.IsDamageControlAssist) // include dead character
            { healingCharacters = characters.Where((arg) => arg.Affiliation == order.Actor.Affiliation).ToList(); }
            else { healingCharacters = characters.Where((arg) => arg.Affiliation == order.Actor.Affiliation && arg.Combat.HitPointCurrent > 0).ToList(); }
        }
        else
        { // heal single find who should heal.
            List<BattleUnit> healingCharactersLaw;
            BattleUnit healingCharacter;
            if (order.IsDamageControlAssist)//set crushed character.
            {
                healingCharacter = characters.FindLast(obj => obj.UniqueID == order.IndividualTargetID);
                if (healingCharacter != null) { healingCharacters.Add(healingCharacter); }
            }
            else
            {
                healingCharactersLaw = characters.Where((arg) => arg.Affiliation == order.Actor.Affiliation && arg.Combat.HitPointCurrent > 0 && arg.Combat.ShiledCurrent == 0).ToList();
                if (healingCharactersLaw.Count > 0) //No shiled , lowest ratio HP
                {
                    healingCharactersLaw.Sort((x, y) => (x.Combat.HitPointCurrent / x.Combat.HitPointMax) - (y.Combat.HitPointCurrent / y.Combat.HitPointMax));
                    healingCharacter = healingCharactersLaw.First();
                    if (healingCharacter != null) { healingCharacters.Add(healingCharacter); }
                }
                else //All at leaset more than 1 shiled, then lowest ratio shiled.
                {
                    healingCharactersLaw = characters.Where((arg) => arg.Affiliation == order.Actor.Affiliation && arg.Combat.HitPointCurrent > 0).ToList();
                    healingCharactersLaw.Sort((x, y) => (x.Combat.ShiledCurrent / x.Combat.ShiledMax) - (y.Combat.ShiledCurrent / y.Combat.ShiledMax));
                    healingCharacter = healingCharactersLaw.First();
                    if (healingCharacter != null) { healingCharacters.Add(healingCharacter); }
                }
            }
        }
        foreach (BattleUnit character in healingCharacters)
        {
            double healValue = healBase * character.Ability.Generation * environmentInfo.R.Next(40 + order.Actor.Ability.Luck, 100) / 100;
            character.Combat.ShiledCurrent += (int)healValue;
            if (order.IsDamageControlAssist && character.Combat.HitPointCurrent == 0) //Damage controled, then heal armor only 1%
            { character.Combat.HitPointCurrent = (int)(character.Combat.HitPointMax * 0.01); }

            int shiledPercentSpace = (3 - Math.Round(((double)character.Combat.ShiledCurrent / (double)character.Combat.ShiledMax * 100), 0).WithComma().Length);
            int hPPercentSpace = (3 - Math.Round(((double)character.Combat.HitPointCurrent / (double)character.Combat.HitPointMax * 100), 0).WithComma().Length);
            int healSpace = (6 - healValue.WithComma().Length);

            // check overflow of shiled current.
            if (character.Combat.ShiledCurrent > character.Combat.ShiledMax) { character.Combat.ShiledCurrent = character.Combat.ShiledMax; }
            Log += new string(' ', 4) + character.Name + " (Sh" + new string(' ', shiledPercentSpace) + Math.Round(((double)character.Combat.ShiledCurrent
                    / (double)character.Combat.ShiledMax * 100), 0).WithComma() + "% HP" + new string(' ', hPPercentSpace) + Math.Round(((double)character.Combat.HitPointCurrent
                    / (double)character.Combat.HitPointMax * 100), 0).WithComma() + "%)" + " is healed its shiled by " + (int)healValue + "\n";
        }
        foreach (BattleUnit character in characters) { if (character.IsCrushedJustNow) { character.IsCrushedJustNow = false; } } // reset IsCrushedJustNow flag

        // hate control
        int hateAdd = 30; if (isMulti) { hateAdd = 50; }
        order.Actor.Feature.HateCurrent += hateAdd;
    }
    public string Log { get; }
}


public class EffectClass
{
    public EffectClass(BattleUnit character, SkillsMasterStruct skill, ActionType actionType, double offenseEffectMagnification, double triggeredPossibility, bool isDamageControlAssistAble, int usageCount,
    int veiledFromTurn, int veiledToTurn)
    {
        this.Character = character; this.Skill = skill; this.ActionType = actionType; this.OffenseEffectMagnification = offenseEffectMagnification; this.TriggeredPossibility = triggeredPossibility;
        this.IsDamageControlAssistAble = isDamageControlAssistAble; this.UsageCount = usageCount; this.VeiledFromTurn = veiledFromTurn; this.VeiledToTurn = veiledToTurn;
        this.SpentCount = 0; this.AccumulationBaseRate = (int)skill.TriggerBase.AccumulationBaseRate; this.NextAccumulationCount = AccumulationBaseRate;
        this.IsntTriggeredBecause = new IsntTriggeredBecauseClass();
    }

    //public void InitializeAccumulation()
    //{
    //    this.NextAccumulationCount = AccumulationBaseRate;
    //}

    public void BuffToCharacter(int currentTurn)
    {
        if (currentTurn <= VeiledToTurn && currentTurn >= VeiledFromTurn)
        {
            Character.Buff.DefenseMagnification *= Skill.BuffTarget.DefenseMagnification;
            Character.Buff.MobilityMagnification *= Skill.BuffTarget.MobilityMagnification;
            Character.Buff.AttackMagnification *= Skill.BuffTarget.AttackMagnification;
            Character.Buff.AccuracyMagnification *= Skill.BuffTarget.AccuracyMagnification;
            Character.Buff.CriticalHitRateMagnification *= Skill.BuffTarget.CriticalHitRateMagnification;
            Character.Buff.NumberOfAttackMagnification *= Skill.BuffTarget.CriticalHitRateMagnification;
            Character.Buff.RangeMinCorrection += Skill.BuffTarget.RangeMinCorrection;
            Character.Buff.RangeMaxCorrection += Skill.BuffTarget.RangeMaxCorrection;
        }
    }

    public class IsntTriggeredBecauseClass
    {
        public IsntTriggeredBecauseClass() { Initialize(); }
        public void Initialize()
        {
            this.IsItCalled = false; this.TriggerCondition = false; this.AfterAllMoved = false; this.TriggerTargetCounter = false; this.TriggerTargetChain = false;
            this.TriggerTargetReAttack = false; this.TriggerTargetMove = false; this.Critical = false; this.NonCritical = false;
            this.OnlyWhenBeenHitMoreThanOnce = false; this.OnlyWhenAvoidMoreThanOnce = false; this.AccumulationAvoid = false; this.AccumulationAllHitCount = false;
            this.AccumulationAllTotalBeenHit = false; this.AccumulationCriticalBeenHit = false; this.AccumulationCriticalHit = false; this.AccumulationSkillBeenHit = false;
            this.AccumulationSkillHit = false; this.TriggeredPossibility = false;
        }

        public bool IsItCalled { get; set; }
        public bool TriggerCondition { get; set; }
        public bool AfterAllMoved { get; set; }
        public bool TriggerTargetCounter { get; set; }
        public bool TriggerTargetChain { get; set; }
        public bool TriggerTargetReAttack { get; set; }
        public bool TriggerTargetMove { get; set; }
        public bool Critical { get; set; }
        public bool NonCritical { get; set; }
        public bool OnlyWhenBeenHitMoreThanOnce { get; set; }
        public bool OnlyWhenAvoidMoreThanOnce { get; set; }
        public bool AccumulationAvoid { get; set; }
        public bool AccumulationAllHitCount { get; set; }
        public bool AccumulationAllTotalBeenHit { get; set; }
        public bool AccumulationCriticalBeenHit { get; set; }
        public bool AccumulationCriticalHit { get; set; }
        public bool AccumulationSkillBeenHit { get; set; }
        public bool AccumulationSkillHit { get; set; }
        public bool TriggeredPossibility { get; set; }
    }

    public IsntTriggeredBecauseClass IsntTriggeredBecause { get; set; }
    public BattleUnit Character { get; }
    public SkillsMasterStruct Skill { get; }
    public ActionType ActionType { get; }
    public double OffenseEffectMagnification { get; }
    public double TriggeredPossibility { get; }
    public bool IsDamageControlAssistAble { get; }
    public int UsageCount { get; set; }
    public int SpentCount { get; set; }
    public int AccumulationBaseRate { get; set; }
    public int NextAccumulationCount { get; set; }
    public int VeiledFromTurn { get; }
    public int VeiledToTurn { get; }
}



//Report for struct
public class StatisticsReporterFirstBloodClass
{
    public StatisticsReporterFirstBloodClass(int battleWave)
    {
        this.BattleWave = battleWave; this.AllyCharacterName = "none"; this.AllyActionType = ActionType.none; this.AllyHappenedTurn = 0; this.AllyCrushedCount = 0;
        this.AllyTotalDealtDamage = 0; this.AllyContentText = "No first Blood."; this.EnemyCharacterName = "none"; this.EnemyActionType = ActionType.none;
        this.EnemyHappenedTurn = 0; this.EnemyCrushedCount = 0; this.EnemyTotalDealtDamage = 0; this.EnemyContentText = "No first Blood."; this.WhichWin = WhichWin.Draw;
    }

    public void Set(Affiliation whichAffiliation, string characterName, ActionType actionType, int happenedTurn, int crushedCount, int totalDealtDamage, string contentText)
    {
        switch (whichAffiliation)
        {
            case Affiliation.ally:
                AllyActionType = actionType; AllyCharacterName = characterName; AllyHappenedTurn = happenedTurn; AllyCrushedCount = crushedCount; AllyTotalDealtDamage = totalDealtDamage; AllyContentText = contentText;
                break;
            case Affiliation.enemy:
                EnemyActionType = actionType; EnemyCharacterName = characterName; EnemyHappenedTurn = happenedTurn; EnemyCrushedCount = crushedCount; EnemyTotalDealtDamage = totalDealtDamage; EnemyContentText = contentText;
                break;
            case Affiliation.none:
                Console.WriteLine("Affiliation.none is not expected in StatisticsReporterFirstBloodClass.");
                break;
        }

    }

    public int BattleWave { get; set; }
    public BattleLogClass BattleLogAlly { get; set; }
    public string AllyCharacterName { get; set; }
    public ActionType AllyActionType { get; set; }
    public int AllyHappenedTurn { get; set; }
    public int AllyCrushedCount { get; set; }
    public int AllyTotalDealtDamage { get; set; }
    public string AllyContentText { get; set; }
    public BattleLogClass BattleLogEnemy { get; set; }
    public string EnemyCharacterName { get; set; }
    public ActionType EnemyActionType { get; set; }
    public int EnemyHappenedTurn { get; set; }
    public int EnemyCrushedCount { get; set; }
    public int EnemyTotalDealtDamage { get; set; }
    public string EnemyContentText { get; set; }
    public WhichWin WhichWin { get; set; }
}

//Action order class
public class OrderClass
{
    public OrderClass(OrderConditionClass orderCondition, BattleUnit actor, ActionType actionType, ref List<EffectClass> skillEffectProposed, int actionSpeed, int individualTargetID, bool isDamageControlAssist)
    {
        this.OrderCondition = orderCondition; this.Actor = actor; this.ActionType = actionType; this.SkillEffectProposed = skillEffectProposed;
        this.ActionSpeed = actionSpeed; this.IndividualTargetID = individualTargetID; this.IsDamageControlAssist = isDamageControlAssist;
        // By default, first list of SkillEffectProposed is selected if has.
        // You need override others if you want to change it.
        if (skillEffectProposed.Count >= 1) { this.SkillEffectChosen = skillEffectProposed[0]; }
        else { Console.WriteLine(" skill Effect proposed is null!!"); }
    }

    // Skill decision, decide best skill in this timming. healAll or healSingle or just do nothing, which move skill should use .
    public void SkillDecision(List<BattleUnit> characters, EnvironmentInfoClass environmentInfo)
    {
        if (SkillEffectProposed != null) // skill effect proposed valid check
        {
            List<EffectClass> validEffects = new List<EffectClass>();

            foreach (EffectClass effect in SkillEffectProposed) { if (effect.UsageCount > 0) { validEffects.Add(effect); } }

            if (validEffects.Count == 0) { Console.WriteLine(" no valid skill exist" + this.Actor.Name + " " + this.ActionType + " " + environmentInfo.Info()); }
            else if (validEffects.Count >= 1)// in case more than 2 skills proposed.
            {
                List<BattleUnit> healTargets;
                List<EffectClass> fillteredEffectList = null;
                if (IsDamageControlAssist) //(1)Damage control assist is requred?
                {
                    healTargets = characters.ToList().FindAll((obj) => obj.Affiliation == this.Actor.Affiliation && obj.Combat.HitPointCurrent == 0 && obj.IsCrushedJustNow == true);
                    healTargets.Sort((x, y) => y.Combat.HitPointCurrent - x.Combat.HitPointCurrent);
                }
                else // non Damage control assisted. //(2)heal expected?
                {
                    healTargets = characters.ToList().FindAll((obj) => obj.Combat.ShiledCurrent == 0 && obj.Affiliation == this.Actor.Affiliation && obj.Combat.HitPointCurrent > 0);
                    healTargets.Sort((x, y) => y.Combat.HitPointCurrent - x.Combat.HitPointCurrent);
                }

                //If (1)damage control assisted or (2)heal is expected, check skill proposed. 0 shiled and low HitPoint character should heal first
                if (healTargets.Count >= 2)//Multi heal reccomended
                {
                    fillteredEffectList = validEffects.FindAll(obj => obj.Skill.CallSkillLogicName == CallSkillLogicName.ShieldHealMulti);
                    //Something wise way to chose best skill...

                    // multi - ShieldHealMulti is not expected now..
                    if (fillteredEffectList.Count > 0)
                    {
                        this.SkillEffectChosen = fillteredEffectList.Last();
                        this.IndividualTargetID = healTargets.First().UniqueID;  // get heal target unique ID, 
                    }// multi heal exist
                    else
                    {
                        // find single 
                        fillteredEffectList.Clear();
                        fillteredEffectList = validEffects.FindAll(obj => obj.Skill.CallSkillLogicName == CallSkillLogicName.ShieldHealSingle);
                        //Something wise way to chose best skill.
                        if (fillteredEffectList.Count > 0)
                        {
                            this.SkillEffectChosen = fillteredEffectList.Last();
                            this.IndividualTargetID = healTargets.First().UniqueID;  // get heal target unique ID, 
                        }// single heal exist
                    }
                }
                else if (healTargets.Count == 1)
                { //Single heal reccomended
                    fillteredEffectList = validEffects.FindAll(obj => obj.Skill.CallSkillLogicName == CallSkillLogicName.ShieldHealSingle);
                    //Something wise way to chose best skill.
                    // multi - ShieldHealMulti is not expected now..
                    if (fillteredEffectList.Count > 0)
                    {
                        this.SkillEffectChosen = fillteredEffectList.Last();
                        this.IndividualTargetID = healTargets.First().UniqueID;  // get heal target unique ID, 
                    }// multi heal exist
                    else
                    {
                        // find single 
                        fillteredEffectList.Clear();
                        fillteredEffectList = validEffects.FindAll(obj => obj.Skill.CallSkillLogicName == CallSkillLogicName.ShieldHealMulti);
                        //Something wise way to chose best skill.
                        if (fillteredEffectList.Count > 0)
                        {
                            this.SkillEffectChosen = fillteredEffectList.Last();
                            this.IndividualTargetID = healTargets.First().UniqueID;  // get heal target unique ID, 
                        }// single heal exist
                    }
                }

                if (fillteredEffectList == null)
                {
                    // find non heal skill or valid heal target.
                    //(3)other skill ?
                    fillteredEffectList = validEffects;
                    //Something wise way to chose best skill.
                    if (fillteredEffectList != null) { this.SkillEffectChosen = fillteredEffectList.Last(); }// single heal exist
                    else { Console.WriteLine("unexpected message in SkillDecision."); }// no skill proposed. something wrong!
                }
            }
        }
    }

    public OrderConditionClass OrderCondition;
    public BattleUnit Actor;
    public ActionType ActionType;
    public List<EffectClass> SkillEffectProposed;
    public EffectClass SkillEffectChosen { get; set; }
    public int ActionSpeed;
    public int IndividualTargetID;
    public bool IsDamageControlAssist;
}

public class OrderStatusClass
{
    public OrderStatusClass() { Initialize(); }
    public void Initialize() { this.CounterSkillCount = 0; this.ChainSkillCount = 0; this.ReAttackSkillCount = 0; this.DamageControlAssistCount = 0; }

    public int CounterSkillCount { get; set; }
    public int ChainSkillCount { get; set; }
    public int ReAttackSkillCount { get; set; }
    public int DamageControlAssistCount { get; set; }
}

public class ShiledHealFunction
{
    public ShiledHealFunction(List<BattleUnit> characters)
    {
        for (int i = 0; i <= characters.Count - 1; i++)
        {
            if (characters[i].Combat.HitPointCurrent > 0)
            { // Only when character is not crushed.
                int shiledHealAmount = (int)((double)characters[i].Combat.ShiledMax * (double)characters[i].Ability.Generation / 100.0);
                if ((characters[i].Combat.ShiledMax - characters[i].Combat.ShiledCurrent) <= shiledHealAmount)
                { // can heal max
                    characters[i].Combat.ShiledCurrent = characters[i].Combat.ShiledMax;
                    Log += new string(' ', 5) + characters[i].Name + " heals all shiled." +
                     " Shiled:" + characters[i].Combat.ShiledCurrent + " (" + (int)((double)characters[i].Combat.ShiledCurrent / (double)characters[i].Combat.ShiledMax * 100) + "%) \n";
                }
                else
                {
                    characters[i].Combat.ShiledCurrent += shiledHealAmount;
                    Log += new string(' ', 5) + characters[i].Name + " heals " + shiledHealAmount +
                     " Shiled:" + characters[i].Combat.ShiledCurrent + " (" + (int)((double)characters[i].Combat.ShiledCurrent / (double)characters[i].Combat.ShiledMax * 100) + "%) \n";
                }
            }
        }
    }
    public string Log { get; }
}

public class CalculationHateMagnificationPerTurnFunction
{
    public CalculationHateMagnificationPerTurnFunction(List<BattleUnit> characters)
    { foreach (BattleUnit character in characters) { character.Feature.HateCurrent *= character.Feature.HateMagnificationPerTurn; } }
    public string Log { get; }
}

public class TriggerBaseClass
{
    public TriggerBaseClass(double possibilityBaseRate, double possibilityWeight, ReferenceStatistics accumulationReference, double accumulationBaseRate, double accumulationWeight)
    {
        this.PossibilityBaseRate = possibilityBaseRate; this.PossibilityWeight = possibilityWeight; this.AccumulationReference = accumulationReference;
        this.AccumulationBaseRate = accumulationBaseRate; this.AccumulationWeight = accumulationWeight;
    }

    public double PossibilityBaseRate { get; }
    public double PossibilityWeight { get; }
    public ReferenceStatistics AccumulationReference { get; }
    public double AccumulationBaseRate { get; }
    public double AccumulationWeight { get; }
}

public class SkillMagnificationClass
{
    public SkillMagnificationClass(TargetType attackTarget, double damage, double kinetic,
    double chemical, double thermal, double heal, double numberOfAttacks, double critical, double accuracy,
         double optimumRangeMin, double optimumRangeMax)
    {
        this.AttackTarget = attackTarget; this.Damage = damage; this.Kinetic = kinetic; this.Chemical = chemical; this.Thermal = thermal; this.Heal = heal;
        this.NumberOfAttacks = numberOfAttacks; this.Critical = critical; this.Accuracy = accuracy; this.OptimumRangeMin = optimumRangeMin; this.OptimumRangeMax = optimumRangeMax;
    }

    public TargetType AttackTarget { get; }
    public double Damage { get; }
    public double Kinetic { get; }
    public double Chemical { get; }
    public double Thermal { get; }
    public double Heal { get; }
    public double NumberOfAttacks { get; }
    public double Critical { get; }
    public double Accuracy { get; }
    public double OptimumRangeMin { get; }
    public double OptimumRangeMax { get; }
}

public class TriggerTargetClass
{
    public TriggerTargetClass(ActionType actionType, bool afterAllMoved, bool counter, bool chain,
     bool reAttack, bool heal, bool move, Range optimumRange, AttackType majestyAttackType, CriticalOrNot critical, ActorOrTargetUnit whoCrushed, bool onlyWhenBeenHitMoreThanOnce, bool onlyWhenAvoidMoreThanOnce)
    {
        this.ActionType = actionType; this.AfterAllMoved = afterAllMoved; this.Counter = counter; this.Chain = chain; this.ReAttack = reAttack; this.Heal = Heal;
        this.Move = move; this.OptimumRange = optimumRange; this.MajestyAttackType = majestyAttackType; this.Critical = critical; this.WhoCrushed = whoCrushed;
        this.OnlyWhenBeenHitMoreThanOnce = onlyWhenBeenHitMoreThanOnce; this.OnlyWhenAvoidMoreThanOnce = onlyWhenAvoidMoreThanOnce;
    }

    public ActionType ActionType { get; }
    public bool AfterAllMoved { get; }
    public bool Counter { get; }
    public bool Chain { get; }
    public bool ReAttack { get; }
    public bool Heal { get; }
    public bool Move { get; }
    public Range OptimumRange { get; }
    public AttackType MajestyAttackType { get; }
    public CriticalOrNot Critical { get; }
    public ActorOrTargetUnit WhoCrushed { get; }
    public bool OnlyWhenBeenHitMoreThanOnce { get; }
    public bool OnlyWhenAvoidMoreThanOnce { get; }
}

public class BuffTargetParameterClass
{
    public BuffTargetParameterClass(TargetType targetType, int barrierRemaining, double defenseMagnification, double mobilityMagnification, double attackMagnification,
     double accuracyMagnification, double criticalHitRateMagnification, double numberOfAttackMagnification, int rangeMinCorrection, int rangeMaxCorrection)
    {
        this.TargetType = targetType; this.BarrierRemaining = barrierRemaining; this.DefenseMagnification = defenseMagnification; this.MobilityMagnification = mobilityMagnification;
        this.AttackMagnification = attackMagnification; this.AccuracyMagnification = accuracyMagnification; this.CriticalHitRateMagnification = criticalHitRateMagnification;
        this.NumberOfAttackMagnification = numberOfAttackMagnification; this.RangeMinCorrection = rangeMinCorrection; this.RangeMaxCorrection = rangeMaxCorrection;
    }

    public TargetType TargetType { get; }
    public int BarrierRemaining { get; }
    public double DefenseMagnification { get; }
    public double MobilityMagnification { get; }
    public double AttackMagnification { get; }
    public double AccuracyMagnification { get; }
    public double CriticalHitRateMagnification { get; }
    public double NumberOfAttackMagnification { get; }
    public int RangeMinCorrection { get; }
    public int RangeMaxCorrection { get; }
}

public class DebuffTargetParameterClass
{
    public DebuffTargetParameterClass(TargetType targetType, double barrierRemaining, double defenseMagnification, double mobilityMagnification, double attackMagnification,
     double accuracyMagnification, double criticalHitRateMagnification, double numberOfAttackMagnification)
    {
        this.TargetType = targetType; this.BarrierRemaining = barrierRemaining; this.DefenseMagnification = defenseMagnification; this.MobilityMagnification = mobilityMagnification;
        this.AttackMagnification = attackMagnification; this.AccuracyMagnification = accuracyMagnification; this.CriticalHitRateMagnification = criticalHitRateMagnification;
        this.NumberOfAttackMagnification = numberOfAttackMagnification;
    }

    public TargetType TargetType { get; }
    public double BarrierRemaining { get; }
    public double DefenseMagnification { get; }
    public double MobilityMagnification { get; }
    public double AttackMagnification { get; }
    public double AccuracyMagnification { get; }
    public double CriticalHitRateMagnification { get; }
    public double NumberOfAttackMagnification { get; }
}

//Check wipe out and should continue the battle
public class WipeOutCheck
{
    public WipeOutCheck(List<BattleUnit> characters)
    {
        List<BattleUnit> totalAllys = characters.FindAll(character => character.Affiliation == Affiliation.ally);
        List<BattleUnit> totalEnemys = characters.FindAll(character => character.Affiliation == Affiliation.enemy);
        List<BattleUnit> crushedAllys = characters.FindAll(character => character.Affiliation == Affiliation.ally && character.Combat.HitPointCurrent == 0);
        List<BattleUnit> crushedEnemys = characters.FindAll(character => character.Affiliation == Affiliation.enemy && character.Combat.HitPointCurrent == 0);
        IsDraw = false; IsEnemyWin = false; IsAllyWin = false; BatleEnd = false;
        if (totalAllys.Count == crushedAllys.Count && totalEnemys.Count == crushedEnemys.Count) { IsDraw = true; BatleEnd = true; }//Draw
        else if (totalAllys.Count == crushedAllys.Count) { IsEnemyWin = true; BatleEnd = true; }  //Ally is wiped out
        else if (totalEnemys.Count == crushedEnemys.Count) { IsAllyWin = true; BatleEnd = true; }//Enemy is wiped out
    }
    public bool BatleEnd { get; set; }
    public bool IsDraw { get; set; }
    public bool IsAllyWin { get; set; }
    public bool IsEnemyWin { get; set; }
}

//NavigatorReaction
public class NavigatorSpeechAfterMoveClass
{
    public NavigatorSpeechAfterMoveClass(string navigatorName, OrderClass order, List<BattleUnit> characters, List<EffectClass> effects, OrderStatusClass orderStatus,
     EnvironmentInfoClass environmentInfo)
    {
        this.Log = null;
        // Status check

        // Ally [Damage Control check]
        List<BattleUnit> justCrushedAlly = characters.FindAll(obj => obj.Affiliation == Affiliation.ally && obj.IsCrushedJustNow == true);
        if (justCrushedAlly.Count > 0)
        {
            List<EffectClass> damageControlAssistCharacterHave = effects.FindAll(obj => obj.Character.Affiliation == Affiliation.ally && obj.Character.Combat.HitPointCurrent > 0
            && obj.Character.Feature.DamageControlAssist == true && obj.Skill.IsHeal == true); // get character who has damage control assist (doesnt matter can or cannot)
            if (damageControlAssistCharacterHave.Count > 0 && orderStatus.DamageControlAssistCount == 0) // Damage control assist should be triggered but cannot..
            {
                string crushedCountText = "Help " + justCrushedAlly[0].Name + " soon,";
                if (justCrushedAlly.Count >= 2) { crushedCountText = justCrushedAlly.Count + " allys are being crushed." + " Help them soon!"; }

                this.Log += navigatorName + ": " + crushedCountText + " " + damageControlAssistCharacterHave[0].Character.Name + "! ";
                if (damageControlAssistCharacterHave[0].IsntTriggeredBecause.AfterAllMoved == true) // moved already
                { this.Log += "You already moved and cannot? \n"; }
                else { this.Log += "Wait, you said no medic kit left? \n"; }
            }
            else if (damageControlAssistCharacterHave.Count == 0 && orderStatus.DamageControlAssistCount == 0)
            {
                string speechText = null;
                if (justCrushedAlly.Count == 1) { speechText = "ally is being crushed."; } else { speechText = "allys are being crushed."; }
                string crushedMedicText = null;
                List<EffectClass> isMedicCrushed = effects.FindAll(obj => obj.Character.Affiliation == Affiliation.ally && obj.Character.Combat.HitPointCurrent == 0
                    && obj.Character.Feature.DamageControlAssist == true && obj.Skill.IsHeal == true); // dead

                if (isMedicCrushed.Count == 0) { crushedMedicText = " We need a medic."; }
                else
                { crushedMedicText = " Now we lost a medic, I wish " + isMedicCrushed[0].Character.Name + " survived."; }

                this.Log += navigatorName + ": " + speechText + crushedMedicText + " \n";
            }
            else if (orderStatus.DamageControlAssistCount > 0)
            { this.Log += "(should triggered damage control assist.)  \n"; }
            else // when happened
            { this.Log += "(unexpected..) \n"; }
        }


        // Enemy [Damage Control check]
        List<BattleUnit> justCrushedEnemy = characters.FindAll(obj => obj.Affiliation == Affiliation.enemy && obj.IsCrushedJustNow == true);
        if (justCrushedEnemy.Count > 0 && orderStatus.DamageControlAssistCount == 0)
        {
            string speechText = null;
            switch (justCrushedEnemy.Count)
            {
                case 1:
                    speechText = justCrushedEnemy[0].Name + " is crushed, well done " + order.Actor.Name + "."; break;
                case 2:
                    speechText = "Double crushed! good job " + order.Actor.Name + "."; break;
                case 3:
                    speechText = "Triple crushed! " + order.Actor.Name + ", Keep going."; break;
                default:
                    speechText = "Wow, " + justCrushedEnemy.Count + " enemys are down. " + order.Actor.Name + ", you are amazing!"; break;
            }
            this.Log += navigatorName + ": " + speechText + "\n";
        }
    }

    public string Log { get; }

}

public class OrderConditionClass
{
    public OrderConditionClass(int wave, int turn, int phase, int orderNumber, int nest, int nestOrderNumber)
    { Wave = wave; Turn = turn; Phase = phase; OrderNumber = orderNumber; Nest = nest; NestOrderNumber = nestOrderNumber; }
    public int Wave { get; }
    public int Turn { get; }
    public int Phase { get; }
    public int OrderNumber { get; set; }
    public int Nest { get; set; }
    public int NestOrderNumber { get; set; }

    public override string ToString()
    { return "Wave:" + Wave + " Turn:" + Turn + " Phase:" + Phase + " OrderNumber:" + OrderNumber + " Nest:" + Nest + " NestOrderNumber:" + NestOrderNumber; }

}

public class BattleResultClass
{
    public BattleResultClass()
    {
        this.BattleEnd = false; this.IsAllyWin = false; this.IsEnemyWin = false; this.IsDraw = false; this.NumberOfCrushed = 0; this.TotalDeltDamage = 0;
        this.CriticalOrNot = CriticalOrNot.any; this.HitMoreThanOnceCharacters = new List<BattleUnit>(); this.AvoidMoreThanOnceCharacters = new List<BattleUnit>();
    }
    public bool BattleEnd;
    public bool IsAllyWin;
    public bool IsEnemyWin;
    public bool IsDraw;
    public int NumberOfCrushed;
    public int TotalDeltDamage;
    public CriticalOrNot CriticalOrNot;
    public List<BattleUnit> HitMoreThanOnceCharacters;
    public List<BattleUnit> AvoidMoreThanOnceCharacters;
}

public class EnvironmentInfoClass
{
    public EnvironmentInfoClass(int wave, int turn, int phase, int randomSeed, System.Random r)
    { Wave = wave; Turn = turn; Phase = phase; RandomSeed = randomSeed; R = r; }
    public int Wave;
    public int Turn;
    public int Phase;
    public int RandomSeed;
    public System.Random R;

    public string Info() { return "Wave:" + Wave + " Turn:" + Turn + " Phase:" + Phase + " RandomSeed:" + RandomSeed; }

}


// FUNCTION SEGUMENT
public class AttackFunction
{
    public AttackFunction(OrderClass order, List<BattleUnit> characters, EnvironmentInfoClass environmentInfo)
    {
        string log = null;
        this.BattleResult = new BattleResultClass();
        // Target control
        Affiliation toTargetAffiliation; if (order.Actor.Affiliation == Affiliation.ally) { toTargetAffiliation = Affiliation.enemy; } //ally's move, so target should be enemy, without confusion.
        else { toTargetAffiliation = Affiliation.ally; } //enemy's move, so target should be ally, without confusuion.

        // Initialize battle environment: Hit, Failed Hit, Damage for each opponent.
        int totalDealtDamageSum = 0;
        int totalDealtToShiledDamageSum = 0;
        int healedByAbsorbShiled = 0;
        int numberOfHitTotal = 0;
        int numberOfSuccessAttacks = 0;
        List<BattleUnit> opponents = characters.FindAll(character1 => character1.Affiliation == toTargetAffiliation && character1.Combat.HitPointCurrent > 0);
        foreach (BattleUnit character in characters) { if (character.IsCrushedJustNow) { character.IsCrushedJustNow = false; } } // reset IsCrushedJustNow flag

        // no enemy anymore.
        bool invalidAction = false || opponents.Count == 0;

        //Ally alive list
        List<BattleUnit> aliveAttackerSide = characters.FindAll(character1 => character1.Affiliation == order.Actor.Affiliation && character1.Combat.HitPointCurrent > 0);
        int aliveAttackerIndex = aliveAttackerSide.IndexOf(order.Actor);
        if (aliveAttackerIndex == -1) { invalidAction = true; }// in case attacker is dead.

        if (invalidAction == false)
        {
            int[] totalDealtDamages = new int[characters.Count];
            int[] totalIndivisualHits = new int[characters.Count];
            bool isNeedCreateTargetPossibilityBox = true;
            List<int> targetPossibilityBox = new List<int>();
            int totalTickets = 0;

            // Skill effect magnification set in case order.SkillEffectChosen is null
            double skillOffenseEffectMagnification = 1.0;
            double skillMagnificationDamage = 1.0;
            double skillMagnificationKinetic = 1.0;
            double skillMagnificationChemical = 1.0;
            double skillMagnificationThermal = 1.0;
            double skillMagnificationAccuracy = 1.0;
            double skillMagnificationCritical = 1.0;
            double skillMagnificationOptimumRangeMin = 1.0;
            double skillMagnificationOptimumRangeMax = 1.0;
            double skillMagnificationNumberOfAttacks = 1.0;

            if (order.SkillEffectChosen != null)
            {
                skillOffenseEffectMagnification = order.SkillEffectChosen.OffenseEffectMagnification;
                skillMagnificationDamage = order.SkillEffectChosen.Skill.Magnification.Damage;
                skillMagnificationKinetic = order.SkillEffectChosen.Skill.Magnification.Kinetic;
                skillMagnificationChemical = order.SkillEffectChosen.Skill.Magnification.Chemical;
                skillMagnificationThermal = order.SkillEffectChosen.Skill.Magnification.Thermal;
                skillMagnificationAccuracy = order.SkillEffectChosen.Skill.Magnification.Accuracy;
                skillMagnificationCritical = order.SkillEffectChosen.Skill.Magnification.Critical;
                skillMagnificationNumberOfAttacks = order.SkillEffectChosen.Skill.Magnification.NumberOfAttacks;
                skillMagnificationOptimumRangeMin = order.SkillEffectChosen.Skill.Magnification.OptimumRangeMin;
                skillMagnificationOptimumRangeMax = order.SkillEffectChosen.Skill.Magnification.OptimumRangeMax;
            }

            //Critical control
            int criticalReduction = 0; if (order.Actor.Combat.CriticalHit >= environmentInfo.R.Next(0, 100))
            { criticalReduction = 50; this.BattleResult.CriticalOrNot = CriticalOrNot.critical; }
            else { this.BattleResult.CriticalOrNot = CriticalOrNot.nonCritical; } //Critical hit!

            //Dacay difinition & Decay cap control
            double decayAccuracy = 0.55 + 0.01 * order.Actor.Ability.Precision; if (decayAccuracy > 0.99) { decayAccuracy = 0.99; }
            double decayDamage = 0.55 + 0.01 * order.Actor.Ability.Power; if (decayDamage > 0.99) { decayDamage = 0.99; }

            // Minimum range - ally's column , Max range - ally's column
            int minTargetOptimumRange = (int)(order.Actor.Combat.MinRange * skillMagnificationOptimumRangeMin) - aliveAttackerIndex;
            int maxTargetOptimumRange = (int)(order.Actor.Combat.MaxRange * skillMagnificationOptimumRangeMax) - aliveAttackerIndex;

            //Initialize Is Barrier broken just now flag and is Avoid flag.
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i].IsBarrierBrokenJustNow) { characters[i].IsBarrierBrokenJustNow = false; }
                if (characters[i].Buff.BarrierRemaining > 0) { if (characters[i].IsBarrierExistJustBefore == false) { characters[i].IsBarrierExistJustBefore = true; } }
                else { if (characters[i].IsBarrierExistJustBefore) { characters[i].IsBarrierExistJustBefore = false; } }
                if (characters[i].IsAvoidMoreThanOnce) { characters[i].IsAvoidMoreThanOnce = false; }
            }

            // Indivisual attack routine
            List<BattleUnit> survivaledOpponents = opponents.FindAll(character1 => character1.Combat.HitPointCurrent > 0);

            for (int numberOfAttacks = 1; numberOfAttacks <= (int)(order.Actor.Combat.NumberOfAttacks * skillMagnificationNumberOfAttacks); numberOfAttacks++)
            {
                //Target individual oppornet per each number of attack.
                int tickets = 0;
                if (survivaledOpponents.Count == 0) { continue; }//enemy all wipe out
                survivaledOpponents.Sort((x, y) => x.UniqueID - y.UniqueID);
                BattleUnit toTarget = null;
                if (order.SkillEffectChosen.Skill.Magnification.AttackTarget == TargetType.multi)
                {
                    int targetColumn = 0;
                    if (isNeedCreateTargetPossibilityBox)
                    {
                        targetPossibilityBox.Clear(); // initialize targetPossibilityBox
                        double basicTargetRatio = 2.0 / 3.0;     //Tiar 1: Basic Target ratio
                        double optimumTargetRatio = 1.0 / 3.0;     //Tiar 2: Optinum Target ratio
                        double optimumTargetBonus = 0;
                        for (int opponent = 0; opponent < survivaledOpponents.Count; opponent++)
                        {
                            if (opponent >= minTargetOptimumRange && opponent <= maxTargetOptimumRange)
                            {
                                optimumTargetBonus = optimumTargetRatio / (1 + maxTargetOptimumRange - minTargetOptimumRange);
                                survivaledOpponents[opponent].IsOptimumTarget = true;
                            }
                            else { survivaledOpponents[opponent].IsOptimumTarget = false; }
                            int targetPossibilityTicket = (int)((basicTargetRatio / Math.Pow(2.0, opponent) + optimumTargetBonus) * 50);
                            targetPossibilityTicket += (int)(survivaledOpponents[opponent].Feature.HateCurrent);// add Hate value 
                            if (targetPossibilityTicket == 0) { targetPossibilityTicket = 1; }//at leaset one chance to hit.
                            for (int ticket = tickets; ticket <= targetPossibilityTicket + tickets; ticket++) { targetPossibilityBox.Add(opponent); } //Put tickets into Box with opponent column number (expected: column recalculated when they crushed)
                            tickets += targetPossibilityTicket;
                        }
                    }
                    else { tickets = totalTickets; }// get previous total tickets
                    int index = environmentInfo.R.Next(0, tickets);
                    targetColumn = targetPossibilityBox[index];
                    totalTickets = tickets;
                    toTarget = survivaledOpponents[targetColumn];
                }
                else if (order.SkillEffectChosen.Skill.Magnification.AttackTarget == TargetType.single)// if individual target exist, choose he/she.
                {
                    if (survivaledOpponents.Count == 0) { continue; }//enemy all wipe out
                    for (int opponent = 0; opponent < survivaledOpponents.Count; opponent++)
                    {
                        if (opponent >= minTargetOptimumRange && opponent <= maxTargetOptimumRange) { survivaledOpponents[opponent].IsOptimumTarget = true; }
                        else { survivaledOpponents[opponent].IsOptimumTarget = false; }
                    }
                    toTarget = opponents.Find(character1 => character1.UniqueID == order.IndividualTargetID);
                }
                else { Console.WriteLine("unexpected. basic attack function, targetType is not single nor multi. info:" + environmentInfo.Info() + " " + order.OrderCondition); }

                isNeedCreateTargetPossibilityBox = false;

                if (toTarget != null && toTarget.Combat.HitPointCurrent > 0)
                {
                    //Hit control
                    //Logic of Hit: accuracy / mobility * (1- fatigue) * Decay(0.8) %
                    double hitPossibility = (double)order.Actor.Combat.Accuracy * skillMagnificationAccuracy / (double)toTarget.Combat.Mobility / (1.00 - toTarget.Deterioration)
                     * (Math.Pow(decayAccuracy, numberOfSuccessAttacks) / decayAccuracy);

                    //judge
                    if (hitPossibility <= environmentInfo.R.NextDouble()) { if (toTarget.IsAvoidMoreThanOnce == false) { toTarget.IsAvoidMoreThanOnce = true; } toTarget.Statistics.AvoidCount++; } //Failed!
                    else //success!
                    {
                        numberOfSuccessAttacks++;
                        totalIndivisualHits[toTarget.UniqueID]++;

                        double criticalMagnification = 1.0;
                        // critical magnification calculation
                        if (criticalReduction > 0) { criticalMagnification = order.Actor.OffenseMagnification.Critical * toTarget.DefenseMagnification.Critical * skillMagnificationCritical; } //critical

                        //Physical Attack damage calculation
                        double attackDamage = (double)order.Actor.Combat.Attack * environmentInfo.R.Next(40 + order.Actor.Ability.Luck, 100) / 100
                        - (double)toTarget.Combat.Deffense * (1.00 - toTarget.Deterioration) * environmentInfo.R.Next(40 + toTarget.Ability.Luck - criticalReduction, 100 - criticalReduction) / 100;
                        if (attackDamage < 0) { attackDamage = 1; }

                        //vs Magnification offense
                        double vsOffenseMagnification = 1.0;
                        switch (toTarget.UnitType)
                        {
                            case (UnitType.beast): vsOffenseMagnification = order.Actor.OffenseMagnification.VsBeast; break;
                            case (UnitType.cyborg): vsOffenseMagnification = order.Actor.OffenseMagnification.VsCyborg; break;
                            case (UnitType.drone): vsOffenseMagnification = order.Actor.OffenseMagnification.VsDrone; break;
                            case (UnitType.robot): vsOffenseMagnification = order.Actor.OffenseMagnification.VsRobot; break;
                            case (UnitType.titan): vsOffenseMagnification = order.Actor.OffenseMagnification.VsTitan; break;
                            default: break;
                        }

                        //vs Magnification offense
                        double vsDeffenseMagnification = 1.0;
                        switch (order.Actor.UnitType)
                        {
                            case (UnitType.beast): vsDeffenseMagnification = toTarget.DefenseMagnification.VsBeast; break;
                            case (UnitType.cyborg): vsDeffenseMagnification = toTarget.DefenseMagnification.VsCyborg; break;
                            case (UnitType.drone): vsDeffenseMagnification = toTarget.DefenseMagnification.VsDrone; break;
                            case (UnitType.robot): vsDeffenseMagnification = toTarget.DefenseMagnification.VsRobot; break;
                            case (UnitType.titan): vsDeffenseMagnification = toTarget.DefenseMagnification.VsTitan; break;
                            default: break;
                        }

                        //Damage type Magnification 
                        double damageTypeMagnification =
                         order.Actor.Combat.KineticAttackRatio * order.Actor.OffenseMagnification.Kinetic * toTarget.DefenseMagnification.Kinetic * skillMagnificationKinetic
                         + order.Actor.Combat.ChemicalAttackRatio * order.Actor.OffenseMagnification.Chemical * toTarget.DefenseMagnification.Chemical * skillMagnificationChemical
                         + order.Actor.Combat.ThermalAttackRatio * order.Actor.OffenseMagnification.Thermal * toTarget.DefenseMagnification.Thermal * skillMagnificationThermal;

                        double optimumRangeBonus = 1.0; if (toTarget.IsOptimumTarget) { optimumRangeBonus = order.Actor.OffenseMagnification.OptimumRangeBonus; } //Consider optimum range bonus.
                        double barrierReduction = 1.0; if (toTarget.Buff.RemoveBarrier()) // Barrier check, true: barrier has, false no barrier.
                        { barrierReduction = 1.0 / 3.0; if (toTarget.Buff.BarrierRemaining <= 0) { toTarget.IsBarrierBrokenJustNow = true; } }
                        else { if (toTarget.IsBarrierExistJustBefore && toTarget.IsBarrierBrokenJustNow == false) { toTarget.IsBarrierBrokenJustNow = true; } } // if barrier is broken within this action, broken check.

                        double buffDamageMagnification = order.Actor.Buff.AttackMagnification / toTarget.Buff.DefenseMagnification; // Buff damage reduction
                        int dealtDamage = (int)((attackDamage) * damageTypeMagnification * vsOffenseMagnification * vsDeffenseMagnification
                        * criticalMagnification * optimumRangeBonus * skillOffenseEffectMagnification * skillMagnificationDamage * buffDamageMagnification * (Math.Pow(decayDamage, numberOfSuccessAttacks) / decayDamage) * barrierReduction);
                        if (dealtDamage <= 0) { dealtDamage = 1; }
                        totalDealtDamageSum += dealtDamage;

                        //Damage calclations 1st shiled, and 2nd hitPoint.
                        if (toTarget.Combat.ShiledCurrent >= dealtDamage) { toTarget.Combat.ShiledCurrent -= dealtDamage; totalDealtToShiledDamageSum += dealtDamage; }// Only shiled damaged
                        else // shiled break
                        {
                            int remainsDamage = dealtDamage - toTarget.Combat.ShiledCurrent;
                            totalDealtToShiledDamageSum += toTarget.Combat.ShiledCurrent;
                            toTarget.Combat.ShiledCurrent = 0;
                            if (toTarget.Combat.HitPointCurrent > remainsDamage) // hitPoint damage
                            {
                                toTarget.Combat.HitPointCurrent -= remainsDamage;
                                double deteriorationStabilityReduction = 1.0 - 0.01 * toTarget.Ability.Stability;
                                toTarget.Deterioration += (1 - toTarget.Deterioration) * deteriorationStabilityReduction * 0.01;
                            }
                            else //Crushed!
                            {
                                toTarget.Combat.HitPointCurrent = 0;
                                toTarget.IsCrushedJustNow = true;
                                isNeedCreateTargetPossibilityBox = true;
                                BattleResult.NumberOfCrushed++;
                                toTarget.Statistics.NumberOfCrushed++;
                            }
                        }
                        totalDealtDamages[toTarget.UniqueID] += dealtDamage;
                    }
                }
                numberOfHitTotal++;
            }
            //Absorb calculation HERE!!
            if (totalDealtToShiledDamageSum > 0 && order.Actor.Feature.AbsorbShieldRatioCurrent > 0) //only when to shiled damage exist and absorb exist.
            {
                if (order.Actor.Combat.ShiledCurrent < order.Actor.Combat.ShiledMax) // when actor is not full shiled 
                {
                    healedByAbsorbShiled = (int)(totalDealtToShiledDamageSum * order.Actor.Feature.AbsorbShieldRatioCurrent);
                    if (healedByAbsorbShiled > order.Actor.Combat.ShiledMax * order.Actor.Feature.AbsorbShieldMaxRatioCurrent) // reached max absorb ratio
                    { healedByAbsorbShiled = (int)(order.Actor.Combat.ShiledMax * order.Actor.Feature.AbsorbShieldMaxRatioCurrent); }
                    order.Actor.Combat.ShiledCurrent += healedByAbsorbShiled;
                    if (order.Actor.Combat.ShiledCurrent > order.Actor.Combat.ShiledMax) { order.Actor.Combat.ShiledCurrent = order.Actor.Combat.ShiledMax; }
                    // log should show expected total amount of healed value, not actural healed amount.
                }
            }


            // Hate Managiment
            double criticalHateAdd = 0; if (criticalReduction > 0) { criticalHateAdd = 30; }
            double skillHateAdd = 0; if (order.SkillEffectChosen != null) { if (order.SkillEffectChosen.Skill.Name != SkillName.normalAttack) { skillHateAdd = 50; } }
            double crushedHateAdd = 0;
            for (int fTargetColumn = 0; fTargetColumn <= opponents.Count - 1; fTargetColumn++)
            { if (opponents[fTargetColumn].Combat.HitPointCurrent == 0) { crushedHateAdd += 100; } }
            order.Actor.Feature.HateCurrent = (numberOfHitTotal / 3.0) + criticalHateAdd + skillHateAdd + crushedHateAdd;

            //Statistics Collection
            for (int toTargetUniqueID = 0; toTargetUniqueID < totalDealtDamages.Length; toTargetUniqueID++)
            {
                characters[toTargetUniqueID].Statistics.AllTotalBeTakenDamage += totalDealtDamages[toTargetUniqueID];
                if (totalDealtDamages[toTargetUniqueID] > 0)
                {
                    if (criticalReduction > 0)
                    {
                        characters[toTargetUniqueID].Statistics.CriticalTotalBeTakenDamage += totalDealtDamages[toTargetUniqueID];
                        characters[toTargetUniqueID].Statistics.CriticalBeenHitCount++;
                    }
                    if (order.SkillEffectChosen != null && order.SkillEffectChosen.Skill.Name != SkillName.normalAttack)
                    {
                        characters[toTargetUniqueID].Statistics.SkillTotalBeTakenDamage += totalDealtDamages[toTargetUniqueID];
                        characters[toTargetUniqueID].Statistics.SkillBeenHitCount++;
                    }
                    BattleResult.HitMoreThanOnceCharacters.Add(characters[toTargetUniqueID]);// Get information of character hit by attack.
                    characters[toTargetUniqueID].Statistics.AllHitCount++;
                }
                if (characters[toTargetUniqueID].IsAvoidMoreThanOnce) { BattleResult.AvoidMoreThanOnceCharacters.Add(characters[toTargetUniqueID]); } // Set avoidMoreThanOnce characters

            }
            order.Actor.Statistics.AllActivatedCount++;
            order.Actor.Statistics.AllTotalDealtDamage += totalDealtDamageSum;
            order.Actor.Statistics.AllHitCount += numberOfSuccessAttacks;
            if (criticalReduction > 0)
            {
                order.Actor.Statistics.CriticalActivatedCount++;
                order.Actor.Statistics.CriticalHitCount += numberOfSuccessAttacks;
                order.Actor.Statistics.CriticalTotalDealtDamage += totalDealtDamageSum;
            }

            if (order.SkillEffectChosen != null)
            {
                if (order.SkillEffectChosen.Skill.Name != SkillName.normalAttack)
                {
                    order.Actor.Statistics.SkillActivatedCount++;
                    order.Actor.Statistics.SkillHitCount += numberOfSuccessAttacks;
                    order.Actor.Statistics.SkillTotalDealtDamage += totalDealtDamageSum;
                }
            }

            string criticalWords = null; if (criticalReduction > 0) { criticalWords = "critical "; }//critical word.
            string skillTriggerPossibility = null; //if moveskill, show possibility
            if (order.SkillEffectChosen != null)
            {
                if (order.SkillEffectChosen.Skill.Name != SkillName.normalAttack)
                { skillTriggerPossibility = " (Trigger Possibility: " + (int)(order.SkillEffectChosen.TriggeredPossibility * 1000.0) / 10.0 + "% left:" + order.SkillEffectChosen.UsageCount + ")"; }
            }
            string sNumberofAttacks = null; if (order.Actor.Combat.NumberOfAttacks != 1) { sNumberofAttacks = "s"; }
            string snumberOfSuccessAttacks = null; if (numberOfSuccessAttacks != 1) { snumberOfSuccessAttacks = "s"; }
            string skillName = "unknown skill"; if (order.SkillEffectChosen != null) { skillName = order.SkillEffectChosen.Skill.Name.ToString(); }
            string majorityElement = " [mixed]";
            if (order.Actor.Combat.KineticAttackRatio > 0.5) { majorityElement = " [Kinetic]"; }
            if (order.Actor.Combat.ChemicalAttackRatio > 0.5) { majorityElement = " [Chemical]"; }
            if (order.Actor.Combat.ThermalAttackRatio > 0.5) { majorityElement = " [Thermal]"; }

            string speedText = null; if (order.ActionSpeed > 0) { speedText = " Speed:" + order.ActionSpeed; }

            log += new string(' ', 0) + order.Actor.Name + "'s " + criticalWords + skillName + skillTriggerPossibility + " "
            + order.Actor.Combat.NumberOfAttacks + "time" + sNumberofAttacks +
             " total hit" + snumberOfSuccessAttacks + ":" + numberOfSuccessAttacks + majorityElement + speedText + "\n";

            for (int fTargetColumn = 0; fTargetColumn <= opponents.Count - 1; fTargetColumn++)
            {
                string crushed = null;
                if (opponents[fTargetColumn].Combat.HitPointCurrent == 0) { crushed = " crushed!"; }
                if (totalIndivisualHits[opponents[fTargetColumn].UniqueID] >= 1)
                {
                    string optimumRangeWords = null;
                    if (opponents[fTargetColumn].IsOptimumTarget) { optimumRangeWords = " [optimum range]"; }

                    string barrierWords = null;
                    if (opponents[fTargetColumn].IsBarrierBrokenJustNow) { barrierWords = " [Barrier Broken]"; }
                    else if (opponents[fTargetColumn].Buff.BarrierRemaining > 0) { barrierWords = " [Barriered(" + opponents[fTargetColumn].Buff.BarrierRemaining + ")]"; }

                    double shiledRatio = Math.Round(((double)opponents[fTargetColumn].Combat.ShiledCurrent / (double)opponents[fTargetColumn].Combat.ShiledMax * 100), 0);
                    double hpRatio = Math.Round(((double)opponents[fTargetColumn].Combat.HitPointCurrent / (double)opponents[fTargetColumn].Combat.HitPointMax * 100), 0);
                    double damageRatio = Math.Round(((double)totalDealtDamages[opponents[fTargetColumn].UniqueID]
                                        / ((double)opponents[fTargetColumn].Combat.ShiledMax + (double)opponents[fTargetColumn].Combat.HitPointMax) * 100), 0);
                    string sign = " "; if (damageRatio > 0) { sign = "-"; }

                    string s = null;
                    if (totalIndivisualHits[opponents[fTargetColumn].UniqueID] != 1) { s = "s"; }
                    int shiledPercentSpace = (3 - shiledRatio.WithComma().Length);
                    int hPPercentSpace = (3 - hpRatio.WithComma().Length);
                    int damageSpace = (6 - totalDealtDamages[opponents[fTargetColumn].UniqueID].WithComma().Length);
                    int damageRateSpace = (4 - sign.Length - damageRatio.ToString().Length);

                    log += new string(' ', 4) + opponents[fTargetColumn].Name + " (Sh" + new string(' ', shiledPercentSpace) + shiledRatio.WithComma() + "% HP" + new string(' ', hPPercentSpace)
                    + hpRatio.WithComma() + "%)" + " gets " + new string(' ', damageSpace) + totalDealtDamages[opponents[fTargetColumn].UniqueID].WithComma() + " damage ("
                    + new string(' ', damageRateSpace) + sign + damageRatio + "%)" + crushed + " Hit" + s + ":"
                    + totalIndivisualHits[opponents[fTargetColumn].UniqueID] + barrierWords + optimumRangeWords + " \n";

                    if (opponents[fTargetColumn].IsOptimumTarget) { opponents[fTargetColumn].IsOptimumTarget = false; } //clear IsOptimumTarget to false
                }

                //Check wipe out and should continue the battle
                WipeOutCheck = new WipeOutCheck(characters);
                BattleResult.BattleEnd = WipeOutCheck.BatleEnd;
                BattleResult.IsAllyWin = WipeOutCheck.IsAllyWin;
                BattleResult.IsEnemyWin = WipeOutCheck.IsEnemyWin;
                BattleResult.IsDraw = WipeOutCheck.IsDraw;
                BattleResult.TotalDeltDamage = totalDealtDamageSum;
            } //fTargetColumn

            if (numberOfSuccessAttacks == 0) { log += new string(' ', 4) + "All attacks missed...  \n"; }

            //Absorb log
            if (healedByAbsorbShiled > 0) { log += new string(' ', 3) + order.Actor.Name + " absorbs shield by " + healedByAbsorbShiled + ". \n"; }

            OrderConditionClass orderCondition = order.OrderCondition;
            BattleLog = new BattleLogClass(orderCondition: orderCondition, isNavigation: false, log: log, importance: 1);
        }
    }

    public BattleResultClass BattleResult { get; }
    public WipeOutCheck WipeOutCheck { get; set; }
    public BattleLogClass BattleLog { get; }
}

