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

    public string FirstLine()
    {
        return "Turn " + CurrentTurn + " of " + "wave " + "" + CurrentBattleWaves + "\n";
    }

    public string Text()
    {
        string text = null;
        //text += "Turn " + CurrentTurn + " of " + "wave " + "" + CurrentBattleWaves + "\n";

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
                int shieldSpace = (9 - affiliationCharacters[i].Combat.ShieldCurrent.WithComma().Length);
                if (shieldSpace <= 0) { shieldSpace = 1; }
                int hPSpace = (9 - affiliationCharacters[i].Combat.HitPointCurrent.WithComma().Length);
                if (hPSpace <= 0) { hPSpace = 1; }
                int shieldPercentSpace = (3 - Math.Round(((double)affiliationCharacters[i].Combat.ShieldCurrent / (double)affiliationCharacters[i].Combat.ShieldMax * 100), 0).WithComma().Length);
                int hPPercentSpace = (3 - Math.Round(((double)affiliationCharacters[i].Combat.HitPointCurrent / (double)affiliationCharacters[i].Combat.HitPointMax * 100), 0).WithComma().Length);

                string barrierText = null;
                if (affiliationCharacters[i].Buff.BarrierRemaining > 0) { barrierText = "[Barrier:" + affiliationCharacters[i].Buff.BarrierRemaining + "] "; }
                string deteriorationText = null;
                if ((int)(affiliationCharacters[i].Deterioration * 100) >= 1) { deteriorationText = "(Deterioration:" + (int)(affiliationCharacters[i].Deterioration * 100) + "%) "; }
                string defenseText = null;
                if (affiliationCharacters[i].Buff.DefenseMagnification > 1.01) { defenseText = "[Defense: x" + affiliationCharacters[i].Buff.DefenseMagnification + "]"; }

                text += new string(' ', 1) + "#" + i + " " + affiliationCharacters[i].Name
                    + " (" + affiliationCharacters[i].Combat.ShieldCurrent.WithComma() + "+"
                    + affiliationCharacters[i].Combat.HitPointCurrent.WithComma() + ")" +
                     " (" + new string(' ', shieldPercentSpace) + Math.Round(((double)affiliationCharacters[i].Combat.ShieldCurrent / (double)affiliationCharacters[i].Combat.ShieldMax * 100), 0) + "%+"
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




//[[Skill logic ]]
public class SkillLogicShieldHealClass
{
    // heal shiled all actor's affiliation characters.
    public SkillLogicShieldHealClass(OrderClass order, List<BattleUnit> characters, bool isMulti, EnvironmentInfoClass environmentInfo)
    {
        string damageControlAssistText = null;
        if (order.IsDamageControlAssist) { damageControlAssistText = "[damage control assist] "; }
        firstLine = damageControlAssistText + order.SkillEffectChosen.Skill.Name + " (Left:" + order.SkillEffectChosen.UsageCount + ") \n";
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
                healingCharacter = characters.FindLast(obj => obj == order.IndividualTarget);
                if (healingCharacter != null) { healingCharacters.Add(healingCharacter); }
            }
            else
            {
                healingCharactersLaw = characters.Where((arg) => arg.Affiliation == order.Actor.Affiliation && arg.Combat.HitPointCurrent > 0 && arg.Combat.ShieldCurrent == 0).ToList();
                if (healingCharactersLaw.Count > 0) //No shield , lowest ratio HP
                {
                    healingCharactersLaw.Sort((x, y) => (x.Combat.HitPointCurrent / x.Combat.HitPointMax) - (y.Combat.HitPointCurrent / y.Combat.HitPointMax));
                    healingCharacter = healingCharactersLaw.First();
                    if (healingCharacter != null) { healingCharacters.Add(healingCharacter); }
                }
                else //All at leaset more than 1 shield, then lowest ratio shield.
                {
                    healingCharactersLaw = characters.Where((arg) => arg.Affiliation == order.Actor.Affiliation && arg.Combat.HitPointCurrent > 0).ToList();
                    healingCharactersLaw.Sort((x, y) => (x.Combat.ShieldCurrent / x.Combat.ShieldMax) - (y.Combat.ShieldCurrent / y.Combat.ShieldMax));
                    healingCharacter = healingCharactersLaw.First();
                    if (healingCharacter != null) { healingCharacters.Add(healingCharacter); }
                }
            }
        }
        foreach (BattleUnit character in healingCharacters)
        {
            double healValue = healBase * character.Ability.Generation * environmentInfo.R.Next(40 + order.Actor.Ability.Luck, 100) / 100;
            character.Combat.ShieldCurrent += (int)healValue;
            if (order.IsDamageControlAssist && character.Combat.HitPointCurrent == 0) //Damage controled, then heal armor only 1%
            { character.Combat.HitPointCurrent = (int)(character.Combat.HitPointMax * 0.01); }

            int shieldPercentSpace = (3 - Math.Round(((double)character.Combat.ShieldCurrent / (double)character.Combat.ShieldMax * 100), 0).WithComma().Length);
            if (shieldPercentSpace < 0) { shieldPercentSpace = 0; }
            int hPPercentSpace = (3 - Math.Round(((double)character.Combat.HitPointCurrent / (double)character.Combat.HitPointMax * 100), 0).WithComma().Length);
            if (hPPercentSpace < 0) { hPPercentSpace = 0; }
            int healSpace = (6 - healValue.WithComma().Length);
            if (healSpace < 0) { healSpace = 0; }


            // check overflow of shield current.
            if (character.Combat.ShieldCurrent > character.Combat.ShieldMax) { character.Combat.ShieldCurrent = character.Combat.ShieldMax; }
            Log += new string(' ', 4) + character.Name + " (Sh" + new string(' ', shieldPercentSpace)
                    + Math.Round(((double)character.Combat.ShieldCurrent / (double)character.Combat.ShieldMax * 100), 0).WithComma()
                    + "% HP" + new string(' ', hPPercentSpace)
                    + Math.Round(((double)character.Combat.HitPointCurrent / (double)character.Combat.HitPointMax * 100), 0).WithComma()
                    + "%)" + " is healed its shield by " + (int)healValue + "\n";
        }
        foreach (BattleUnit character in characters) { if (character.IsCrushedJustNow) { character.IsCrushedJustNow = false; } } // reset IsCrushedJustNow flag

        // hate control
        int hateAdd = 30; if (isMulti) { hateAdd = 50; }
        order.Actor.Feature.HateCurrent += hateAdd;
    }
    public string firstLine { get; }
    public string Log { get; }
}



//Report for struct
public class StatisticsReporterFirstBloodClass
{
    public StatisticsReporterFirstBloodClass(int battleWave)
    {
        this.BattleWave = battleWave; this.AllyCharacterName = "none"; this.AllyActionType = ActionType.None; this.AllyHappenedTurn = 0; this.AllyCrushedCount = 0;
        this.AllyTotalDealtDamage = 0; this.AllyContentText = "No first Blood."; this.EnemyCharacterName = "none"; this.EnemyActionType = ActionType.None;
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
    public OrderClass(OrderConditionClass orderCondition, BattleUnit actor, ActionType actionType, ref List<EffectClass> skillEffectProposed, int actionSpeed, BattleUnit individualTarget, bool isDamageControlAssist)
    {
        this.OrderCondition = orderCondition; this.Actor = actor; this.ActionType = actionType; this.SkillEffectProposed = skillEffectProposed;
        this.ActionSpeed = actionSpeed; this.IndividualTarget = individualTarget; this.IsDamageControlAssist = isDamageControlAssist;
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
                    healTargets = characters.ToList().FindAll((obj) => obj.Combat.ShieldCurrent == 0 && obj.Affiliation == this.Actor.Affiliation && obj.Combat.HitPointCurrent > 0);
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
                        this.IndividualTarget = healTargets.First();  // get heal target unique ID, 
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
                            this.IndividualTarget = healTargets.First();  // get heal target unique ID, 
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
                        this.IndividualTarget = healTargets.First();  // get heal target unique ID, 
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
                            this.IndividualTarget = healTargets.First();  // get heal target unique ID, 
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

    public OrderClass Copy()
    {
        OrderClass other = (OrderClass)this.MemberwiseClone();
        other.Actor = this.Actor.Copy();

        return other;
    }

    public OrderConditionClass OrderCondition;
    public BattleUnit Actor;
    public ActionType ActionType;
    public List<EffectClass> SkillEffectProposed;
    public EffectClass SkillEffectChosen { get; set; }
    public int ActionSpeed;
    public BattleUnit IndividualTarget;
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

public class ShieldHealFunction
{
    public ShieldHealFunction(List<BattleUnit> characters)
    {
        for (int i = 0; i <= characters.Count - 1; i++)
        {
            if (characters[i].Combat.HitPointCurrent > 0)
            { // Only when character is not crushed.
                int shieldHealAmount = (int)((double)characters[i].Combat.ShieldMax * (double)characters[i].Ability.Generation / 100.0);
                if ((characters[i].Combat.ShieldMax - characters[i].Combat.ShieldCurrent) <= shieldHealAmount)
                { // can heal max
                    characters[i].Combat.ShieldCurrent = characters[i].Combat.ShieldMax;
                    Log += characters[i].Name + " heals all shield." +
                     " Shield:" + characters[i].Combat.ShieldCurrent + " (" + (int)((double)characters[i].Combat.ShieldCurrent / (double)characters[i].Combat.ShieldMax * 100) + "%) \n";
                }
                else
                {
                    characters[i].Combat.ShieldCurrent += shieldHealAmount;
                    Log += characters[i].Name + " heals " + shieldHealAmount +
                     " Shield:" + characters[i].Combat.ShieldCurrent + " (" + (int)((double)characters[i].Combat.ShieldCurrent / (double)characters[i].Combat.ShieldMax * 100) + "%) \n";
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
            {
                //this.Log += "(Enemy may be triggered damage control assist.)  \n";
            }
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



