using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
//using _00_Asset.I2.Localization.Scripts.Manager;
using SequenceBreaker.Environment;
using SequenceBreaker.Master.BattleUnit;
using SequenceBreaker.Translate;
using UnityEngine;

// FUNCTION SEGMENT
namespace SequenceBreaker.Play.Battle
{
    public sealed class AttackFunction
    {
        public AttackFunction(OrderClass order, List<BattleUnit> characters, EnvironmentInfoClass environmentInfo)
        {
            string log = null;
            BattleResult = new BattleResultClass();
            // Target control
            var toTargetAffiliation = order.Actor.affiliation == Affiliation.Ally ? Affiliation.Enemy : Affiliation.Ally;

            // Initialize battle environment: Hit, Failed Hit, Damage for each opponent.
            var totalDealtDamageSum = 0;
            var totalDealtToShieldDamageSum = 0;
            var healedByAbsorbShield = 0;
            var numberOfHitTotal = 0;
            var numberOfSuccessAttacks = 0;
            var opponents = characters.FindAll(character1 => character1.affiliation == toTargetAffiliation && character1.combat.hitPointCurrent > 0);
            foreach (var character in characters.Where(character => character.IsCrushedJustNow))
            {
                character.IsCrushedJustNow = false;
            }
            // reset IsCrushedJustNow flag

            // no enemy anymore.
            var invalidAction = opponents.Count == 0;

            //Ally alive list
            var aliveAttackerSide = characters.FindAll(character1 => character1.affiliation == order.Actor.affiliation && character1.combat.hitPointCurrent > 0);
            var aliveAttackerIndex = aliveAttackerSide.IndexOf(order.Actor);
            if (aliveAttackerIndex == -1) { invalidAction = true; }// in case attacker is dead.

            if (invalidAction == false)
            {
                var totalDealtDamages = new int[characters.Count];
                var totalIndividualHits = new int[characters.Count];
                var isNeedCreateTargetPossibilityBox = true;
                var targetPossibilityBox = new List<int>();
                var totalTickets = 0;

                // Skill effect magnification set in case order.SkillEffectChosen is null
                var skillOffenseEffectMagnification = 1.0;
                var skillMagnificationDamage = 1.0;
                var skillMagnificationKinetic = 1.0;
                var skillMagnificationChemical = 1.0;
                var skillMagnificationThermal = 1.0;
                var skillMagnificationAccuracy = 1.0;
                var skillMagnificationCritical = 1.0;
                var skillMagnificationOptimumRangeMin = 1.0;
                var skillMagnificationOptimumRangeMax = 1.0;
                var skillMagnificationNumberOfAttacks = 1.0;

                if (order.SkillEffectChosen != null)
                {
                    skillOffenseEffectMagnification = order.SkillEffectChosen.offenseEffectMagnification;
                    skillMagnificationDamage = order.SkillEffectChosen.skill.magnification.damage;
                    skillMagnificationKinetic = order.SkillEffectChosen.skill.magnification.kinetic;
                    skillMagnificationChemical = order.SkillEffectChosen.skill.magnification.chemical;
                    skillMagnificationThermal = order.SkillEffectChosen.skill.magnification.thermal;
                    skillMagnificationAccuracy = order.SkillEffectChosen.skill.magnification.accuracy;
                    skillMagnificationCritical = order.SkillEffectChosen.skill.magnification.critical;
                    skillMagnificationNumberOfAttacks = order.SkillEffectChosen.skill.magnification.numberOfAttacks;
                    skillMagnificationOptimumRangeMin = order.SkillEffectChosen.skill.magnification.optimumRangeMin;
                    skillMagnificationOptimumRangeMax = order.SkillEffectChosen.skill.magnification.optimumRangeMax;
                }

                //Critical control
                var criticalReduction = 0; if (order.Actor.combat.criticalHit >= environmentInfo.R.Next(0, 100))
                { criticalReduction = 50; BattleResult.CriticalOrNot = CriticalOrNot.Critical; }
                else { BattleResult.CriticalOrNot = CriticalOrNot.NonCritical; } //Critical hit!

                //Decay definition & Decay cap control
                var decayAccuracy = 0.55 + 0.01 * order.Actor.ability.precision; if (decayAccuracy > 0.99) { decayAccuracy = 0.99; }
                var decayDamage = 0.55 + 0.01 * order.Actor.ability.power; if (decayDamage > 0.99) { decayDamage = 0.99; }

                // Minimum range - ally's column , Max range - ally's column
                var minTargetOptimumRange = (int)(order.Actor.combat.minRange * skillMagnificationOptimumRangeMin) - aliveAttackerIndex;
                var maxTargetOptimumRange = (int)(order.Actor.combat.maxRange * skillMagnificationOptimumRangeMax) - aliveAttackerIndex;

                //Initialize Is Barrier broken just now flag and is Avoid flag.
                foreach (var t in characters)
                {
                    if (t.IsBarrierBrokenJustNow) { t.IsBarrierBrokenJustNow = false; }
                    if (t.buff.BarrierRemaining > 0) { if (t.IsBarrierExistJustBefore == false) { t.IsBarrierExistJustBefore = true; } }
                    else { if (t.IsBarrierExistJustBefore) { t.IsBarrierExistJustBefore = false; } }
                    if (t.IsAvoidMoreThanOnce) { t.IsAvoidMoreThanOnce = false; }
                }

                // Individual attack routine
                var survivedOpponents = opponents.FindAll(character1 => character1.combat.hitPointCurrent > 0);

                for (var numberOfAttacks = 1; numberOfAttacks <= (int)(order.Actor.combat.numberOfAttacks * skillMagnificationNumberOfAttacks); numberOfAttacks++)
                {
                    //Target individual opponent per each number of attack.
                    var tickets = 0;
                    if (survivedOpponents.Count == 0) { continue; }//enemy all wipe out
                    survivedOpponents.Sort((x, y) => x.uniqueId - y.uniqueId);
                    BattleUnit toTarget = null;
                    switch (order.SkillEffectChosen.skill.magnification.attackTarget)
                    {
                        case TargetType.Multi:
                            {
                                if (isNeedCreateTargetPossibilityBox)
                                {
                                    targetPossibilityBox.Clear(); // initialize targetPossibilityBox
                                    const double basicTargetRatio = 2.0 / 3.0; //Tier 1: Basic Target ratio
                                    const double optimumTargetRatio = 1.0 / 3.0; //Tier 2: Optimum Target ratio
                                    double optimumTargetBonus = 0;
                                    for (var opponent = 0; opponent < survivedOpponents.Count; opponent++)
                                    {
                                        if (opponent >= minTargetOptimumRange && opponent <= maxTargetOptimumRange)
                                        {
                                            optimumTargetBonus = optimumTargetRatio / (1 + maxTargetOptimumRange - minTargetOptimumRange);
                                            survivedOpponents[opponent].IsOptimumTarget = true;
                                        }
                                        else { survivedOpponents[opponent].IsOptimumTarget = false; }
                                        var targetPossibilityTicket = (int)((basicTargetRatio / Math.Pow(2.0, opponent) + optimumTargetBonus) * 50);
                                        targetPossibilityTicket += (int)(survivedOpponents[opponent].feature.hateCurrent);// add Hate value 
                                        if (targetPossibilityTicket == 0) { targetPossibilityTicket = 1; }//at Least one chance to hit.
                                        for (var ticket = tickets; ticket <= targetPossibilityTicket + tickets; ticket++) { targetPossibilityBox.Add(opponent); } //Put tickets into Box with opponent column number (expected: column recalculated when they crushed)
                                        tickets += targetPossibilityTicket;
                                    }
                                }
                                else { tickets = totalTickets; }// get previous total tickets
                                var index = environmentInfo.R.Next(0, tickets);
                                var targetColumn = targetPossibilityBox[index];
                                totalTickets = tickets;
                                toTarget = survivedOpponents[targetColumn];
                                break;
                            }
                        // if individual target exist, choose he/she.
                        case TargetType.Single when survivedOpponents.Count == 0:
                            continue; //enemy all wipe out
                        case TargetType.Single:
                            {
                                for (var opponent = 0; opponent < survivedOpponents.Count; opponent++)
                                {
                                    if (opponent >= minTargetOptimumRange && opponent <= maxTargetOptimumRange) { survivedOpponents[opponent].IsOptimumTarget = true; }
                                    else { survivedOpponents[opponent].IsOptimumTarget = false; }
                                }
                                toTarget = opponents.Find(character1 => character1 == order.IndividualTarget);
                                break;
                            }
                        default:
                            Debug.LogError("unexpected. basic attack function, targetType is not single nor multi. info:" + environmentInfo.Info() + " " + order.OrderCondition);
                            break;
                    }

                    isNeedCreateTargetPossibilityBox = false;

                    if (toTarget != null && toTarget.combat.hitPointCurrent > 0)
                    {
                        //Hit control
                        //Logic of Hit: accuracy / mobility * (1- fatigue) * Decay(0.8) %
                        var hitPossibility = order.Actor.combat.accuracy * skillMagnificationAccuracy / toTarget.combat.mobility / (1.00 - toTarget.Deterioration)
                                             * (Math.Pow(decayAccuracy, numberOfSuccessAttacks) / decayAccuracy);

                        //judge
                        if (hitPossibility <= environmentInfo.R.NextDouble())
                        {
                            if (toTarget.IsAvoidMoreThanOnce == false)
                            { toTarget.IsAvoidMoreThanOnce = true; }
                            toTarget.Statistics.AvoidCount++;
                        } //Failed!
                        else //success!
                        {
                            numberOfSuccessAttacks++;
                            totalIndividualHits[toTarget.uniqueId]++;

                            var criticalMagnification = 1.0;
                            // critical magnification calculation
                            if (criticalReduction > 0)
                            {
                                criticalMagnification = order.Actor.offenseMagnification.critical
                                                        * toTarget.defenseMagnification.critical * skillMagnificationCritical;
                            } //critical

                            //Physical Attack damage calculation
                            var attackDamage = (double)order.Actor.combat.attack * environmentInfo.R.Next(40 + order.Actor.ability.luck, 100) / 100
                                               - toTarget.combat.defense * (1.00 - toTarget.Deterioration)
                                                                         * environmentInfo.R.Next(40 + toTarget.ability.luck - criticalReduction, 100 - criticalReduction) / 100;
                            if (attackDamage < 0) { attackDamage = 1; }

                            //vs Magnification offense
                            double vsOffenseMagnification;
                            switch (toTarget.unitType)
                            {
                                case (UnitType.Beast):
                                    vsOffenseMagnification = order.Actor.offenseMagnification.vsBeast;
                                    break;
                                case (UnitType.Cyborg):
                                    vsOffenseMagnification = order.Actor.offenseMagnification.vsCyborg;
                                    break;
                                case (UnitType.Drone):
                                    vsOffenseMagnification = order.Actor.offenseMagnification.vsDrone;
                                    break;
                                case (UnitType.Robot):
                                    vsOffenseMagnification = order.Actor.offenseMagnification.vsRobot;
                                    break;
                                case (UnitType.Titan):
                                    vsOffenseMagnification = order.Actor.offenseMagnification.vsTitan;
                                    break;
                                default:
                                    vsOffenseMagnification = 1.0;
                                    break;
                            }

                            //vs Magnification offense
                            double vsDefenseMagnification;
                            switch (order.Actor.unitType)
                            {
                                case (UnitType.Beast): vsDefenseMagnification = toTarget.defenseMagnification.vsBeast; break;
                                case (UnitType.Cyborg): vsDefenseMagnification = toTarget.defenseMagnification.vsCyborg; break;
                                case (UnitType.Drone): vsDefenseMagnification = toTarget.defenseMagnification.vsDrone; break;
                                case (UnitType.Robot): vsDefenseMagnification = toTarget.defenseMagnification.vsRobot; break;
                                case (UnitType.Titan): vsDefenseMagnification = toTarget.defenseMagnification.vsTitan; break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            //Damage type Magnification 
                            var damageTypeMagnification =
                                order.Actor.combat.kineticAttackRatio * order.Actor.offenseMagnification.kinetic * toTarget.defenseMagnification.kinetic * skillMagnificationKinetic
                                + order.Actor.combat.chemicalAttackRatio * order.Actor.offenseMagnification.chemical * toTarget.defenseMagnification.chemical * skillMagnificationChemical
                                + order.Actor.combat.thermalAttackRatio * order.Actor.offenseMagnification.thermal * toTarget.defenseMagnification.thermal * skillMagnificationThermal;

                            var optimumRangeBonus = 1.0; if (toTarget.IsOptimumTarget) { optimumRangeBonus = order.Actor.offenseMagnification.optimumRangeBonus; } //Consider optimum range bonus.
                            var barrierReduction = 1.0; if (toTarget.buff.RemoveBarrier()) // Barrier check, true: barrier has, false no barrier.
                            { barrierReduction = 1.0 / 3.0; if (toTarget.buff.BarrierRemaining <= 0) { toTarget.IsBarrierBrokenJustNow = true; } }
                            else { if (toTarget.IsBarrierExistJustBefore && toTarget.IsBarrierBrokenJustNow == false) { toTarget.IsBarrierBrokenJustNow = true; } } // if barrier is broken within this action, broken check.

                            var buffDamageMagnification = order.Actor.buff.AttackMagnification / toTarget.buff.DefenseMagnification; // Buff damage reduction
                            var dealtDamage = (int)((attackDamage) * damageTypeMagnification * vsOffenseMagnification * vsDefenseMagnification
                                                    * criticalMagnification * optimumRangeBonus * skillOffenseEffectMagnification * skillMagnificationDamage * buffDamageMagnification
                                                    * (Math.Pow(decayDamage, numberOfSuccessAttacks) / decayDamage) * barrierReduction);
                            if (dealtDamage <= 0) { dealtDamage = 1; }
                            totalDealtDamageSum += dealtDamage;

                            //Damage calculations 1st shield, and 2nd hitPoint.
                            if (toTarget.combat.shieldCurrent >= dealtDamage) { toTarget.combat.shieldCurrent -= dealtDamage; totalDealtToShieldDamageSum += dealtDamage; }// Only shield damaged
                            else // shield break
                            {
                                var remainsDamage = dealtDamage - toTarget.combat.shieldCurrent;
                                totalDealtToShieldDamageSum += toTarget.combat.shieldCurrent;
                                toTarget.combat.shieldCurrent = 0;
                                if (toTarget.combat.hitPointCurrent > remainsDamage) // hitPoint damage
                                {
                                    toTarget.combat.hitPointCurrent -= remainsDamage;
                                    var deteriorationStabilityReduction = 1.0 - 0.01 * toTarget.ability.stability;
                                    toTarget.Deterioration += (1 - toTarget.Deterioration) * deteriorationStabilityReduction * 0.01;
                                }
                                else //Crushed!
                                {
                                    toTarget.combat.hitPointCurrent = 0;
                                    toTarget.IsCrushedJustNow = true;
                                    isNeedCreateTargetPossibilityBox = true;
                                    BattleResult.NumberOfCrushed++;
                                    toTarget.Statistics.NumberOfCrushed++;
                                }
                            }
                            totalDealtDamages[toTarget.uniqueId] += dealtDamage;
                        }
                    }
                    numberOfHitTotal++;
                }
                //Absorb calculation HERE!!
                if (totalDealtToShieldDamageSum > 0 && order.Actor.feature.absorbShieldRatioCurrent > 0) //only when to shield damage exist and absorb exist.
                {
                    if (order.Actor.combat.shieldCurrent < order.Actor.combat.shieldMax) // when actor is not full shield 
                    {
                        healedByAbsorbShield = (int)(totalDealtToShieldDamageSum * order.Actor.feature.absorbShieldRatioCurrent);
                        if (healedByAbsorbShield > order.Actor.combat.shieldMax * order.Actor.feature.absorbShieldMaxRatioCurrent) // reached max absorb ratio
                        { healedByAbsorbShield = (int)(order.Actor.combat.shieldMax * order.Actor.feature.absorbShieldMaxRatioCurrent); }
                        order.Actor.combat.shieldCurrent += healedByAbsorbShield;
                        if (order.Actor.combat.shieldCurrent > order.Actor.combat.shieldMax) { order.Actor.combat.shieldCurrent = order.Actor.combat.shieldMax; }
                        // log should show expected total amount of healed value, not actual healed amount.
                    }
                }


                // Hate Management
                double criticalHateAdd = 0; if (criticalReduction > 0) { criticalHateAdd = 30; }
                double skillHateAdd = 0; if (order.SkillEffectChosen != null)
                {
                    if (!order.SkillEffectChosen.skill.isNormalAttack)
                    {
                        skillHateAdd = 50;
                    }
                }
                double crushedHateAdd = 0;
                for (var fTargetColumn = 0; fTargetColumn <= opponents.Count - 1; fTargetColumn++)
                { if (opponents[fTargetColumn].combat.hitPointCurrent == 0) { crushedHateAdd += 100; } }
                order.Actor.feature.hateCurrent = (numberOfHitTotal / 3.0) + criticalHateAdd + skillHateAdd + crushedHateAdd;

                //Statistics Collection
                for (var toTargetUniqueId = 0; toTargetUniqueId < totalDealtDamages.Length; toTargetUniqueId++)
                {
                    characters[toTargetUniqueId].Statistics.AllTotalBeTakenDamage += totalDealtDamages[toTargetUniqueId];
                    if (totalDealtDamages[toTargetUniqueId] > 0)
                    {
                        if (criticalReduction > 0)
                        {
                            characters[toTargetUniqueId].Statistics.CriticalTotalBeTakenDamage += totalDealtDamages[toTargetUniqueId];
                            characters[toTargetUniqueId].Statistics.CriticalBeenHitCount++;
                        }
                        if (order.SkillEffectChosen != null && !order.SkillEffectChosen.skill.isNormalAttack)
                        {
                            characters[toTargetUniqueId].Statistics.SkillTotalBeTakenDamage += totalDealtDamages[toTargetUniqueId];
                            characters[toTargetUniqueId].Statistics.SkillBeenHitCount++;
                        }
                        BattleResult.HitMoreThanOnceCharacters.Add(characters[toTargetUniqueId]);// Get information of character hit by attack.
                        characters[toTargetUniqueId].Statistics.AllHitCount++;
                    }
                    if (characters[toTargetUniqueId].IsAvoidMoreThanOnce) { BattleResult.AvoidMoreThanOnceCharacters.Add(characters[toTargetUniqueId]); } // Set avoidMoreThanOnce characters

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
                    if (!order.SkillEffectChosen.skill.isNormalAttack)
                    {
                        order.Actor.Statistics.SkillActivatedCount++;
                        order.Actor.Statistics.SkillHitCount += numberOfSuccessAttacks;
                        order.Actor.Statistics.SkillTotalDealtDamage += totalDealtDamageSum;
                    }
                }

                string criticalWords = null; if (criticalReduction > 0) { criticalWords = " " + Word.Get("Critical"); }//critical word.
                string skillTriggerPossibility = null; //if moveSkill, show possibility
                if (order.SkillEffectChosen != null)
                {
                    if (!order.SkillEffectChosen.skill.isNormalAttack)
                    {
                        skillTriggerPossibility = " (" + (int)(order.SkillEffectChosen.triggeredPossibility * 1000.0) / 10.0 + "% "
                              + Word.Get("Left") + ":" + order.SkillEffectChosen.UsageCount + ")";
                    }
                }
                //string sNumberOfAttacks = null; if (order.Actor.combat.numberOfAttacks != 1)
                //{
                //    sNumberOfAttacks = Word.Get("Plural-s");
                //}
                //string sNumberOfSuccessAttacks = null; if (numberOfSuccessAttacks != 1) { sNumberOfSuccessAttacks = "s"; }
                var skillName = "unknown skill"; if (order.SkillEffectChosen != null)
                {
                    skillName = Word.Get(order.SkillEffectChosen.skill.skillName);
                }
                //if (order.Actor.combat.kineticAttackRatio > 0.5) { majorityElement = " [Kinetic "+ order.Actor.combat.kineticAttackRatio + "]"; }
                //if (order.Actor.combat.chemicalAttackRatio > 0.5) { majorityElement = " [Chemical " + order.Actor.combat.chemicalAttackRatio + "]"; }
                //if (order.Actor.combat.thermalAttackRatio > 0.5) { majorityElement = " [Thermal "+ order.Actor.combat.thermalAttackRatio + "]"; }
                string majorityElement = null;
                if (order.Actor.combat.kineticAttackRatio > 0.001)
                {
                    majorityElement +=
                                    " [" + Word.Get("Kinetic") + " x"
                                    + Math.Round(order.Actor.combat.kineticAttackRatio * order.Actor.offenseMagnification.kinetic
                                    * order.Actor.offenseMagnification.critical, 2) + "]";
                }
                if (order.Actor.combat.chemicalAttackRatio > 0.001)
                {
                    majorityElement +=
                                    " [" + Word.Get("Chemical") + "x"
                                    + Math.Round(order.Actor.combat.chemicalAttackRatio * order.Actor.offenseMagnification.chemical
                                    * order.Actor.offenseMagnification.critical, 2) + "]";
                }
                if (order.Actor.combat.thermalAttackRatio > 0.001)
                {
                    majorityElement +=
                                    " [" + Word.Get("Thermal") + "x"
                                    + Math.Round(order.Actor.combat.thermalAttackRatio * order.Actor.offenseMagnification.thermal
                                    * order.Actor.offenseMagnification.critical, 2) + "]";
                }

                if (majorityElement == null)
                {
                    majorityElement = " [" + Word.Get("Element-None") + " x"
                        + Math.Round(order.Actor.offenseMagnification.critical, 2) + "]";

                }


                //string speedText = null; if (order.ActionSpeed > 0) { speedText = " Speed:" + order.ActionSpeed; }

                //string hitString = Word.Get("Hit-Active");

                var firstLine = skillName + " [" + Word.Get("X shots", order.Actor.combat.numberOfAttacks.ToString()) +"] " + skillTriggerPossibility + " "                               
                                + " " + Word.Get("X hits.-Active", numberOfSuccessAttacks.ToString()) + criticalWords + majorityElement;

                for (var fTargetColumn = 0; fTargetColumn <= opponents.Count - 1; fTargetColumn++)
                {
                    string crushed = null;
                    if (opponents[fTargetColumn].combat.hitPointCurrent == 0) { crushed = " " + Word.Get("crushed!"); }

                    if (totalIndividualHits[opponents[fTargetColumn].uniqueId] >= 1)
                    {
                        string optimumRangeWords = null;
                        if (opponents[fTargetColumn].IsOptimumTarget) { optimumRangeWords = " *"; }

                        string barrierWords = null;
                        if (opponents[fTargetColumn].IsBarrierBrokenJustNow) { barrierWords = " [" + Word.Get("Barrier Broken") + "]"; }
                        else if (opponents[fTargetColumn].buff.BarrierRemaining > 0)
                        {
                            barrierWords = " [" + Word.Get("Barriered") + "(" + opponents[fTargetColumn].buff.BarrierRemaining + ")]";
                        }

                        var damageRatio = Math.Round((totalDealtDamages[opponents[fTargetColumn].uniqueId]
                                          / ((double)opponents[fTargetColumn].combat.shieldMax + (double)opponents[fTargetColumn].combat.hitPointMax) * 100), 0);
                        var sign = " "; if (damageRatio > 0) { sign = "-"; }
                        var damageRateSpace = (4 - sign.Length - damageRatio.ToString(CultureInfo.InvariantCulture).Length);
                        if (damageRateSpace < 0) { damageRateSpace = 0; }
                        string damageRatioString = " (" + new string(' ', damageRateSpace) + sign + damageRatio + "%)";


                        log += opponents[fTargetColumn].shortName + Word.Get("takes X damages,", totalDealtDamages[opponents[fTargetColumn].uniqueId].WithComma())
                            + damageRatioString + " "
                            + Word.Get("X hits.", totalIndividualHits[opponents[fTargetColumn].uniqueId].WithComma()) + "\n"
                            + "   " + "["+ opponents[fTargetColumn].GetShieldHp() +"]" + crushed + barrierWords + optimumRangeWords + " \n";
                        if (opponents[fTargetColumn].IsOptimumTarget) { opponents[fTargetColumn].IsOptimumTarget = false; } //clear IsOptimumTarget to false
                    }

                    //Check wipe out and should continue the battle
                    WipeOutCheck = new WipeOutCheck(characters);
                    BattleResult.BattleEnd = WipeOutCheck.BattleEnd;
                    BattleResult.IsAllyWin = WipeOutCheck.IsAllyWin;
                    BattleResult.IsEnemyWin = WipeOutCheck.IsEnemyWin;
                    BattleResult.IsDraw = WipeOutCheck.IsDraw;
                    BattleResult.TotalDealtDamage = totalDealtDamageSum;
                } //fTargetColumn

                if (numberOfSuccessAttacks == 0) { log += new string(' ', 4) + Word.Get("All attacks missed") + "\n"; }

                //Absorb log
                if (healedByAbsorbShield > 0)
                {
                    log += new string(' ', 3) + order.Actor.shortName + Word.Get("absorbs X shields", healedByAbsorbShield.ToString());


                }


                //if (healedByAbsorbShield > 0) { log += new string(' ', 3) + order.Actor.shortName + " " + Word.Get("absorbs") + " "
                //        + healedByAbsorbShield + " " + Word.Get("shield")+ Word.Get("Period") +  " \n"; }

                var orderCondition = order.OrderCondition;
                BattleLog = new BattleLogClass(orderCondition: orderCondition, order: order, firstLine: firstLine, log: log, whichAffiliationAct: order.Actor.affiliation);
            }
        }

        public BattleResultClass BattleResult { get; }
        private WipeOutCheck WipeOutCheck { get; }
        public BattleLogClass BattleLog { get; }
    }
}
