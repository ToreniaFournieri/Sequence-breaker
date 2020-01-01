using System;
using System.Collections.Generic;
using System.Linq;
using SequenceBreaker.Environment;
using SequenceBreaker.Master.BattleUnit;
using SequenceBreaker.Master.Skills;
using SequenceBreaker.Translate;
using UnityEngine;

namespace SequenceBreaker.Play.Battle
{
    public static class ObjectExtensions { public static string WithComma(this object self) { return $"{self:#,##0}"; } } //With comma override Object


    ////Function, display battle condition text
    //public static class FuncBattleConditionsText
    //{
    //    public static string Get(int currentTurn, List<BattleUnit> characters)
    //    {
    //        return Word.Get("Turn") + " " + currentTurn + "\n";
    //    }

    //    //public FuncBattleConditionsText(int currentTurn, List<BattleUnit> characters)
    //    //{
    //    //    CurrentTurn = currentTurn; Characters = characters;
    //    //}

    //    //public string FirstLine()
    //    //{
    //    //    return Word.Get("Turn") + " " + CurrentTurn + "\n";
    //    //}

    //    //private int CurrentTurn { get; }
    //    //private List<BattleUnit> Characters { get; }
    //}




    //[[Skill logic ]]
    public sealed class SkillLogicShieldHealClass
    {
        // heal shield all actor's affiliation characters.
        public SkillLogicShieldHealClass(OrderClass order, List<BattleUnit> characters, bool isMulti, EnvironmentInfoClass environmentInfo)
        {
            string rescueString = null;
            if (order.IsRescue) { rescueString = "[" + Word.Get("Rescue") + "] "; }
            FirstLine = rescueString + order.SkillEffectChosen.skill.skillName + " (" + Word.Get("Left") + ":" + order.SkillEffectChosen.UsageCount + ")";
            var healBase = order.Actor.ability.generation * order.SkillEffectChosen.skill.magnification.heal * 10.0;

            // heal only same affiliation
            var healingCharacters = new List<BattleUnit>();
            if (isMulti)
            {
                healingCharacters = order.IsRescue ? characters.Where(arg => arg.affiliation == order.Actor.affiliation).ToList()
                    : characters.Where(arg => arg.affiliation == order.Actor.affiliation && arg.combat.hitPointCurrent > 0).ToList();
            }
            else
            { // heal single find who should heal.
                BattleUnit healingCharacter;
                if (order.IsRescue)//set crushed character.
                {
                    healingCharacter = characters.FindLast(obj => obj == order.IndividualTarget);
                    if (healingCharacter != null) { healingCharacters.Add(healingCharacter); }
                }
                else
                {
                    var healingCharactersLaw = characters.Where(arg => arg.affiliation == order.Actor.affiliation && arg.combat.hitPointCurrent > 0 && arg.combat.shieldCurrent == 0).ToList();
                    if (healingCharactersLaw.Count > 0) //No shield , lowest ratio HP
                    {
                        healingCharactersLaw.Sort((x, y) => (x.combat.hitPointCurrent / x.combat.hitPointMax) - (y.combat.hitPointCurrent / y.combat.hitPointMax));
                        healingCharacter = healingCharactersLaw.First();
                        if (healingCharacter != null) { healingCharacters.Add(healingCharacter); }
                    }
                    else //All at least more than 1 shield, then lowest ratio shield.
                    {
                        healingCharactersLaw = characters.Where(arg => arg.affiliation == order.Actor.affiliation && arg.combat.hitPointCurrent > 0).ToList();
                        healingCharactersLaw.Sort((x, y) => (x.combat.shieldCurrent / x.combat.shieldMax) - (y.combat.shieldCurrent / y.combat.shieldMax));
                        healingCharacter = healingCharactersLaw.First();
                        if (healingCharacter != null) { healingCharacters.Add(healingCharacter); }
                    }
                }
            }
            foreach (var character in healingCharacters)
            {
                var healValue = healBase * character.ability.generation * environmentInfo.R.Next(40 + order.Actor.ability.luck, 100) / 100;
                character.combat.shieldCurrent += (int)healValue;
                if (order.IsRescue && character.combat.hitPointCurrent == 0) //Rescued, then heal armor only 1%
                { character.combat.hitPointCurrent = (int)(character.combat.hitPointMax * 0.01); }

                var shieldPercentSpace = (3 - Math.Round((character.combat.shieldCurrent / (double)character.combat.shieldMax * 100), 0).WithComma().Length);
                if (shieldPercentSpace < 0) { shieldPercentSpace = 0; }
                var hPPercentSpace = (3 - Math.Round((character.combat.hitPointCurrent / (double)character.combat.hitPointMax * 100), 0).WithComma().Length);
                if (hPPercentSpace < 0) { hPPercentSpace = 0; }


                // check overflow of shield current.
                if (character.combat.shieldCurrent > character.combat.shieldMax) { character.combat.shieldCurrent = character.combat.shieldMax; }
                Log += new string(' ', 4) + character.longName + Word.Get("heals X shields.", ((int)healValue).ToString()) + "\n"
                    + character.GetShieldHp() + "\n";
            }
            foreach (var character in characters) { if (character.IsCrushedJustNow) { character.IsCrushedJustNow = false; } } // reset IsCrushedJustNow flag

            // hate control
            var hateAdd = 30; if (isMulti) { hateAdd = 50; }
            order.Actor.feature.hateCurrent += hateAdd;
        }
        public string FirstLine { get; }
        public string Log { get; }
    }



    //Report for struct
    public sealed class StatisticsReporterFirstBloodClass
    {
        public StatisticsReporterFirstBloodClass(int battleWave)
        {
            BattleWave = battleWave; AllyCharacterName = "none";
            EnemyCharacterName = "none";
            EnemyTotalDealtDamage = 0; EnemyContentText = "No first Blood."; WhichWin = WhichWin.Draw;
        }

        public void Set(Affiliation whichAffiliation, string characterName, int totalDealtDamage, string contentText)
        {
            switch (whichAffiliation)
            {
                case Affiliation.Ally:
                    AllyCharacterName = characterName;
                    break;
                case Affiliation.Enemy:
                    EnemyCharacterName = characterName;
                    EnemyTotalDealtDamage = totalDealtDamage; EnemyContentText = contentText;
                    break;
                case Affiliation.None:
                    Debug.LogError("Affiliation.none is not expected in StatisticsReporterFirstBloodClass. characterName:" + characterName);
                    break;
            }

        }

        public int BattleWave { get; }
        public string AllyCharacterName { get; private set; }
        public BattleLogClass BattleLogEnemy { get; set; }
        public string EnemyCharacterName { get; private set; }
        public int EnemyTotalDealtDamage { get; private set; }
        public string EnemyContentText { get; private set; }
        public WhichWin WhichWin { get; set; }
    }

    //Action order class
    public sealed class OrderClass
    {
        public OrderClass(OrderConditionClass orderCondition, BattleUnit actor, ActionType actionType, ref List<EffectClass> skillEffectProposed, int actionSpeed, BattleUnit individualTarget, bool isRescue)
        {
            OrderCondition = orderCondition; Actor = actor; ActionType = actionType; SkillEffectProposed = skillEffectProposed;
            ActionSpeed = actionSpeed; IndividualTarget = individualTarget; IsRescue = isRescue;
            // By default, first list of SkillEffectProposed is selected if has.
            // You need override others if you want to change it.
            if (skillEffectProposed.Count >= 1) { SkillEffectChosen = skillEffectProposed[0]; }
            else { Debug.LogError(" skill Effect proposed is null!!"); }
        }

        // Skill decision, decide best skill in this timing. healAll or healSingle or just do nothing, which move skill should use .
        public void SkillDecision(IEnumerable<BattleUnit> characters, EnvironmentInfoClass environmentInfo)
        {
            if (SkillEffectProposed != null) // skill effect proposed valid check
            {
                var validEffects = new List<EffectClass>();

                foreach (var effect in SkillEffectProposed) { if (effect.UsageCount > 0) { validEffects.Add(effect); } }

                if (validEffects.Count == 0) { Debug.LogError(" no valid skill exist" + Actor.longName + " " + ActionType + " " + environmentInfo.Info()); }
                else if (validEffects.Count >= 1)// in case more than 2 skills proposed.
                {
                    List<BattleUnit> healTargets;
                    List<EffectClass> filteredEffectList = null;
                    if (IsRescue) //(1)Rescue is required?
                    {
                        healTargets = characters.ToList().FindAll(obj => obj.affiliation == Actor.affiliation && obj.combat.hitPointCurrent == 0 && obj.IsCrushedJustNow);
                        healTargets.Sort((x, y) => y.combat.hitPointCurrent - x.combat.hitPointCurrent);
                    }
                    else // non rescued. //(2)heal expected?
                    {
                        healTargets = characters.ToList().FindAll(obj => obj.combat.shieldCurrent == 0 && obj.affiliation == Actor.affiliation && obj.combat.hitPointCurrent > 0);
                        healTargets.Sort((x, y) => y.combat.hitPointCurrent - x.combat.hitPointCurrent);
                    }

                    //If (1)Rescue or (2)heal is expected, check skill proposed. 0 shield and low HitPoint character should heal first
                    if (healTargets.Count >= 2)//Multi heal recommended
                    {
                        filteredEffectList = validEffects.FindAll(obj => obj.skill.callSkillLogicName == CallSkillLogicName.ShieldHealMulti);
                        //Something wise way to chose best skill...

                        // multi - ShieldHealMulti is not expected now..
                        if (filteredEffectList.Count > 0)
                        {
                            SkillEffectChosen = filteredEffectList.Last();
                            IndividualTarget = healTargets.First();  // get heal target unique ID, 
                        }// multi heal exist
                        else
                        {
                            // find single 
                            filteredEffectList.Clear();
                            filteredEffectList = validEffects.FindAll(obj => obj.skill.callSkillLogicName == CallSkillLogicName.ShieldHealSingle);
                            //Something wise way to chose best skill.
                            if (filteredEffectList.Count > 0)
                            {
                                SkillEffectChosen = filteredEffectList.Last();
                                IndividualTarget = healTargets.First();  // get heal target unique ID, 
                            }// single heal exist
                        }
                    }
                    else if (healTargets.Count == 1)
                    { //Single heal recommended
                        filteredEffectList = validEffects.FindAll(obj => obj.skill.callSkillLogicName == CallSkillLogicName.ShieldHealSingle);
                        //Something wise way to chose best skill.
                        // multi - ShieldHealMulti is not expected now..
                        if (filteredEffectList.Count > 0)
                        {
                            SkillEffectChosen = filteredEffectList.Last();
                            IndividualTarget = healTargets.First();  // get heal target unique ID, 
                        }// multi heal exist
                        else
                        {
                            // find single 
                            filteredEffectList.Clear();
                            filteredEffectList = validEffects.FindAll(obj => obj.skill.callSkillLogicName == CallSkillLogicName.ShieldHealMulti);
                            //Something wise way to chose best skill.
                            if (filteredEffectList.Count > 0)
                            {
                                SkillEffectChosen = filteredEffectList.Last();
                                IndividualTarget = healTargets.First();  // get heal target unique ID, 
                            }// single heal exist
                        }
                    }

                    if (filteredEffectList == null)
                    {
                        // find non heal skill or valid heal target.
                        //(3)other skill ?
                        filteredEffectList = validEffects;
                        //Something wise way to chose best skill.
                        SkillEffectChosen = filteredEffectList.Last();
                    }
                }
            }
        }

        public OrderClass Copy()
        {
            var other = (OrderClass)MemberwiseClone();
            other.Actor = Actor.Copy();

            return other;
        }

        public readonly OrderConditionClass OrderCondition;
        public BattleUnit Actor;
        public readonly ActionType ActionType;
        public readonly List<EffectClass> SkillEffectProposed;
        public EffectClass SkillEffectChosen { get; private set; }
        public readonly int ActionSpeed;
        public BattleUnit IndividualTarget;
        public readonly bool IsRescue;
    }

    public sealed class OrderStatusClass
    {
        public OrderStatusClass() { Initialize(); }
        public void Initialize()
        {
            DamageControlAssistCount = 0;
        }

        public int DamageControlAssistCount { get; set; }
    }

    public static class ShieldHealFunction
    {
        public static string Get(BattleUnit character)
        {
            string Log = null;

            if (character.combat.hitPointCurrent > 0)
            {
                // Only when character is not crushed.
                var shieldHealAmount = (int)(character.combat.shieldMax * (double)character.ability.generation / 100.0);
                if ((character.combat.shieldMax - character.combat.shieldCurrent) <= shieldHealAmount)
                {
                    character.combat.shieldCurrent = character.combat.shieldMax;
                    Log += character.longName 
                        +  Word.Get("heals all shields.") + " (" + shieldHealAmount + ")\n"
                        + new string(' ', 3) + " [" + character.GetShieldHp() + "]\n";
                    //+ " (" + (int)(character.combat.shieldCurrent / (double)character.combat.shieldMax * 100) + "%) \n";
                }
                else
                {
                    character.combat.shieldCurrent += shieldHealAmount;
                    Log += character.longName 
                        + Word.Get("heals X shields.", shieldHealAmount.ToString()) + "\n"
                        + new string(' ', 3) + "[" + character.GetShieldHp() + "]\n";

                    //+ " (" + (int)(character.combat.shieldCurrent / (double)character.combat.shieldMax * 100) + "%) \n";
                }
            }

            return Log;

        }


        //public ShieldHealFunction(BattleUnit character)
        //{
        //        if (character.combat.hitPointCurrent > 0)
        //        { // Only when character is not crushed.
        //            var shieldHealAmount = (int)(character.combat.shieldMax * (double)character.ability.generation / 100.0);
        //            if ((character.combat.shieldMax - character.combat.shieldCurrent) <= shieldHealAmount)
        //            { // can heal max
        //                character.combat.shieldCurrent = character.combat.shieldMax;
        //                Log += character.longName + Word.Get("heals all shields.")
        //                      + " (" + (int)(character.combat.shieldCurrent / (double)character.combat.shieldMax * 100) + "%) \n";
        //            }
        //            else
        //            {
        //                character.combat.shieldCurrent += shieldHealAmount;
        //                Log += character.longName + Word.Get("heals X shields.", shieldHealAmount.ToString()) 
        //                     + " (" + (int)(character.combat.shieldCurrent / (double)character.combat.shieldMax * 100) + "%) \n";
        //            }
        //        }

        //}
        //public string Log { get; }
    }

    public sealed class CalculationHateMagnificationPerTurnFunction
    {
        public CalculationHateMagnificationPerTurnFunction(BattleUnit character)
        {
            character.feature.hateCurrent *= character.feature.hateMagnificationPerTurn;
            Log = "";
        }
        public string Log { get; }
    }







    //Check wipe out and should continue the battle
    public sealed class WipeOutCheck
    {
        public WipeOutCheck(List<BattleUnit> characters)
        {
            var totalAllys = characters.FindAll(character => character.affiliation == Affiliation.Ally);
            var totalEnemies = characters.FindAll(character => character.affiliation == Affiliation.Enemy);
            var crushedAllys = characters.FindAll(character => character.affiliation == Affiliation.Ally && character.combat.hitPointCurrent == 0);
            var crushedEnemies = characters.FindAll(character => character.affiliation == Affiliation.Enemy && character.combat.hitPointCurrent == 0);
            IsDraw = false; IsEnemyWin = false; IsAllyWin = false; BattleEnd = false;
            if (totalAllys.Count == crushedAllys.Count && totalEnemies.Count == crushedEnemies.Count) { IsDraw = true; BattleEnd = true; }//Draw
            else if (totalAllys.Count == crushedAllys.Count) { IsEnemyWin = true; BattleEnd = true; }  //Ally is wiped out
            else if (totalEnemies.Count == crushedEnemies.Count) { IsAllyWin = true; BattleEnd = true; }//Enemy is wiped out
        }
        public bool BattleEnd { get; }
        public bool IsDraw { get; }
        public bool IsAllyWin { get; }
        public bool IsEnemyWin { get; }
    }

    ////NavigatorReaction
    //public sealed class NavigatorSpeechAfterMoveClass
    //{
    //    public NavigatorSpeechAfterMoveClass(string navigatorName, OrderClass order, List<BattleUnit> characters, List<EffectClass> effects, OrderStatusClass orderStatus)
    //    {
    //        Log = null;
    //        // Status check

    //        // Ally [Rescue]
    //        var justCrushedAlly = characters.FindAll(obj => obj.affiliation == Affiliation.Ally && obj.IsCrushedJustNow);
    //        if (justCrushedAlly.Count > 0)
    //        {
    //            var damageControlAssistCharacterHave = effects.FindAll(obj => obj.character.affiliation == Affiliation.Ally && obj.character.combat.hitPointCurrent > 0
    //                                                                                                                        && obj.character.feature.damageControlAssist && obj.skill.isHeal); // get character who has rescue (doesnt matter can or cannot)
    //            if (damageControlAssistCharacterHave.Count > 0 && orderStatus.DamageControlAssistCount == 0) // Rescue should be triggered but cannot..
    //            {
    //                var crushedCountText = "Help " + justCrushedAlly[0].name + " soon,";
    //                if (justCrushedAlly.Count >= 2) { crushedCountText = justCrushedAlly.Count + " allys are being crushed." + " Help them soon!"; }

    //                Log += navigatorName + ": " + crushedCountText + " " + damageControlAssistCharacterHave[0].character.name + "! ";
    //                if (damageControlAssistCharacterHave[0].IsntTriggeredBecause.AfterAllMoved) // moved already
    //                { Log += "You already moved and cannot? \n"; }
    //                else { Log += "Wait, you said no medic kit left? \n"; }
    //            }
    //            else if (damageControlAssistCharacterHave.Count == 0 && orderStatus.DamageControlAssistCount == 0)
    //            {
    //                var speechText = justCrushedAlly.Count == 1 ? "ally is being crushed." : "allys are being crushed.";
    //                string crushedMedicText;
    //                var isMedicCrushed = effects.FindAll(obj => obj.character.affiliation == Affiliation.Ally && obj.character.combat.hitPointCurrent == 0
    //                                                                                                          && obj.character.feature.damageControlAssist && obj.skill.isHeal); // dead

    //                if (isMedicCrushed.Count == 0) { crushedMedicText = " We need a medic."; }
    //                else
    //                { crushedMedicText = " Now we lost a medic, I wish " + isMedicCrushed[0].character.name + " survived."; }

    //                Log += navigatorName + ": " + speechText + crushedMedicText + " \n";
    //            }
    //            else if (orderStatus.DamageControlAssistCount > 0)
    //            {
    //                //this.Log += "(Enemy may be triggered rescue.)  \n";
    //            }
    //            else // when happened
    //            { Log += "(unexpected..) \n"; }
    //        }


    //        // Enemy [Damage Control check]
    //        var justCrushedEnemy = characters.FindAll(obj => obj.affiliation == Affiliation.Enemy && obj.IsCrushedJustNow);
    //        if (justCrushedEnemy.Count > 0 && orderStatus.DamageControlAssistCount == 0)
    //        {
    //            string speechText;
    //            switch (justCrushedEnemy.Count)
    //            {
    //                case 1:
    //                    speechText = justCrushedEnemy[0].name + " is crushed, well done " + order.Actor.name + "."; break;
    //                case 2:
    //                    speechText = "Double crushed! good job " + order.Actor.name + "."; break;
    //                case 3:
    //                    speechText = "Triple crushed! " + order.Actor.name + ", Keep going."; break;
    //                default:
    //                    speechText = "Wow, " + justCrushedEnemy.Count + " enemies are down. " + order.Actor.name + ", you are amazing!"; break;
    //            }
    //            Log += navigatorName + ": " + speechText + "\n";
    //        }
    //    }

    //    public string Log { get; }

    //}


    public sealed class OrderConditionClass
    {
        public OrderConditionClass(int wave, int turn, int phase, int orderNumber, int nest, int nestOrderNumber, int speed)
        { Wave = wave; Turn = turn; Phase = phase; OrderNumber = orderNumber; Nest = nest; NestOrderNumber = nestOrderNumber; Speed = speed; }
        public int Wave { get; }
        public int Turn { get; }
        private int Phase { get; }
        public int OrderNumber { get; set; }
        public int Nest { get; }
        public int NestOrderNumber { get; }
        public int Speed { get; }
        public override string ToString()
        { return "Wave:" + Wave + " Turn:" + Turn + " Phase:" + Phase + " OrderNumber:" + OrderNumber + " Nest:" + Nest + " NestOrderNumber:" + NestOrderNumber; }

    }

    public sealed class BattleResultClass
    {
        public BattleResultClass()
        {
            BattleEnd = false; IsAllyWin = false; IsEnemyWin = false; IsDraw = false; NumberOfCrushed = 0; TotalDealtDamage = 0;
            CriticalOrNot = CriticalOrNot.Any; HitMoreThanOnceCharacters = new List<BattleUnit>(); AvoidMoreThanOnceCharacters = new List<BattleUnit>();
        }
        public bool BattleEnd;
        public bool IsAllyWin;
        public bool IsEnemyWin;
        public bool IsDraw;
        public int NumberOfCrushed;
        public int TotalDealtDamage;
        public CriticalOrNot CriticalOrNot;
        public List<BattleUnit> HitMoreThanOnceCharacters;
        public List<BattleUnit> AvoidMoreThanOnceCharacters;
    }

    public sealed class EnvironmentInfoClass
    {
        public EnvironmentInfoClass(int wave, int turn, int phase, int randomSeed, System.Random r)
        { Wave = wave; Turn = turn; Phase = phase; _randomSeed = randomSeed; R = r; }
        public readonly int Wave;
        public int Turn;
        public int Phase;
        private readonly int _randomSeed;
        public readonly System.Random R;

        public string Info() { return "Wave:" + Wave + " Turn:" + Turn + " Phase:" + Phase + " RandomSeed:" + _randomSeed; }

    }
}