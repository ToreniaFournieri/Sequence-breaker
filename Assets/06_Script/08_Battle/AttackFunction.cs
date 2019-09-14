using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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
        int totalDealtToShieldDamageSum = 0;
        int healedByAbsorbShield = 0;
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
                    toTarget = opponents.Find(character1 => character1 == order.IndividualTarget);
                }
                else { Debug.Log("unexpected. basic attack function, targetType is not single nor multi. info:" + environmentInfo.Info() + " " + order.OrderCondition); }

                isNeedCreateTargetPossibilityBox = false;

                if (toTarget != null && toTarget.Combat.HitPointCurrent > 0)
                {
                    //Hit control
                    //Logic of Hit: accuracy / mobility * (1- fatigue) * Decay(0.8) %
                    double hitPossibility = (double)order.Actor.Combat.Accuracy * skillMagnificationAccuracy / (double)toTarget.Combat.Mobility / (1.00 - toTarget.Deterioration)
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
                        totalIndivisualHits[toTarget.UniqueID]++;

                        double criticalMagnification = 1.0;
                        // critical magnification calculation
                        if (criticalReduction > 0)
                        {
                            criticalMagnification = order.Actor.OffenseMagnification.Critical
                            * toTarget.DefenseMagnification.Critical * skillMagnificationCritical;
                        } //critical

                        //Physical Attack damage calculation
                        double attackDamage = (double)order.Actor.Combat.Attack * environmentInfo.R.Next(40 + order.Actor.Ability.Luck, 100) / 100
                        - (double)toTarget.Combat.Defense * (1.00 - toTarget.Deterioration)
                        * environmentInfo.R.Next(40 + toTarget.Ability.Luck - criticalReduction, 100 - criticalReduction) / 100;
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
                        * criticalMagnification * optimumRangeBonus * skillOffenseEffectMagnification * skillMagnificationDamage * buffDamageMagnification
                        * (Math.Pow(decayDamage, numberOfSuccessAttacks) / decayDamage) * barrierReduction);
                        if (dealtDamage <= 0) { dealtDamage = 1; }
                        totalDealtDamageSum += dealtDamage;

                        //Damage calclations 1st shield, and 2nd hitPoint.
                        if (toTarget.Combat.ShieldCurrent >= dealtDamage) { toTarget.Combat.ShieldCurrent -= dealtDamage; totalDealtToShieldDamageSum += dealtDamage; }// Only shield damaged
                        else // shield break
                        {
                            int remainsDamage = dealtDamage - toTarget.Combat.ShieldCurrent;
                            totalDealtToShieldDamageSum += toTarget.Combat.ShieldCurrent;
                            toTarget.Combat.ShieldCurrent = 0;
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
            if (totalDealtToShieldDamageSum > 0 && order.Actor.Feature.AbsorbShieldRatioCurrent > 0) //only when to shield damage exist and absorb exist.
            {
                if (order.Actor.Combat.ShieldCurrent < order.Actor.Combat.ShieldMax) // when actor is not full shield 
                {
                    healedByAbsorbShield = (int)(totalDealtToShieldDamageSum * order.Actor.Feature.AbsorbShieldRatioCurrent);
                    if (healedByAbsorbShield > order.Actor.Combat.ShieldMax * order.Actor.Feature.AbsorbShieldMaxRatioCurrent) // reached max absorb ratio
                    { healedByAbsorbShield = (int)(order.Actor.Combat.ShieldMax * order.Actor.Feature.AbsorbShieldMaxRatioCurrent); }
                    order.Actor.Combat.ShieldCurrent += healedByAbsorbShield;
                    if (order.Actor.Combat.ShieldCurrent > order.Actor.Combat.ShieldMax) { order.Actor.Combat.ShieldCurrent = order.Actor.Combat.ShieldMax; }
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
                { skillTriggerPossibility = " (" + (int)(order.SkillEffectChosen.TriggeredPossibility * 1000.0) / 10.0 + "% left:" + order.SkillEffectChosen.UsageCount + ")"; }
            }
            string sNumberofAttacks = null; if (order.Actor.Combat.NumberOfAttacks != 1) { sNumberofAttacks = "s"; }
            string snumberOfSuccessAttacks = null; if (numberOfSuccessAttacks != 1) { snumberOfSuccessAttacks = "s"; }
            string skillName = "unknown skill"; if (order.SkillEffectChosen != null) { skillName = order.SkillEffectChosen.Skill.Name.ToString(); }
            string majorityElement = " [mixed]";
            if (order.Actor.Combat.KineticAttackRatio > 0.5) { majorityElement = " [Kinetic]"; }
            if (order.Actor.Combat.ChemicalAttackRatio > 0.5) { majorityElement = " [Chemical]"; }
            if (order.Actor.Combat.ThermalAttackRatio > 0.5) { majorityElement = " [Thermal]"; }

            string speedText = null; if (order.ActionSpeed > 0) { speedText = " Speed:" + order.ActionSpeed; }

            string firstLine = criticalWords + skillName + skillTriggerPossibility + " "
+ order.Actor.Combat.NumberOfAttacks + "time" + sNumberofAttacks +
 " total hit" + snumberOfSuccessAttacks + ":" + numberOfSuccessAttacks + majorityElement + speedText + "\n";

            for (int fTargetColumn = 0; fTargetColumn <= opponents.Count - 1; fTargetColumn++)
            {
                string crushed = null;
                if (opponents[fTargetColumn].Combat.HitPointCurrent == 0) { crushed = " crushed!"; }
                if (totalIndivisualHits[opponents[fTargetColumn].UniqueID] >= 1)
                {
                    string optimumRangeWords = null;
                    if (opponents[fTargetColumn].IsOptimumTarget) { optimumRangeWords = " *"; }

                    string barrierWords = null;
                    if (opponents[fTargetColumn].IsBarrierBrokenJustNow) { barrierWords = " [Barrier Broken]"; }
                    else if (opponents[fTargetColumn].Buff.BarrierRemaining > 0) { barrierWords = " [Barriered(" + opponents[fTargetColumn].Buff.BarrierRemaining + ")]"; }

                    double shieldRatio = Math.Round(((double)opponents[fTargetColumn].Combat.ShieldCurrent / (double)opponents[fTargetColumn].Combat.ShieldMax * 100), 0);
                    double hpRatio = Math.Round(((double)opponents[fTargetColumn].Combat.HitPointCurrent / (double)opponents[fTargetColumn].Combat.HitPointMax * 100), 0);
                    double damageRatio = Math.Round(((double)totalDealtDamages[opponents[fTargetColumn].UniqueID]
                                        / ((double)opponents[fTargetColumn].Combat.ShieldMax + (double)opponents[fTargetColumn].Combat.HitPointMax) * 100), 0);
                    string sign = " "; if (damageRatio > 0) { sign = "-"; }

                    string s = null;
                    if (totalIndivisualHits[opponents[fTargetColumn].UniqueID] != 1) { s = "s"; }
                    int shieldPercentSpace = (3 - shieldRatio.WithComma().Length);
                    int hPPercentSpace = (3 - hpRatio.WithComma().Length);
                    int damageSpace = (6 - totalDealtDamages[opponents[fTargetColumn].UniqueID].WithComma().Length);
                    int damageRateSpace = (4 - sign.Length - damageRatio.ToString().Length);

                    log += opponents[fTargetColumn].Name + " gets " + new string(' ', damageSpace) + totalDealtDamages[opponents[fTargetColumn].UniqueID].WithComma() + " damage ("
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
            if (healedByAbsorbShield > 0) { log += new string(' ', 3) + order.Actor.Name + " absorbs shield by " + healedByAbsorbShield + ". \n"; }

            OrderConditionClass orderCondition = order.OrderCondition;
            BattleLog = new BattleLogClass(orderCondition: orderCondition, isNavigation: false, order: order, firstLine: firstLine, log: log, importance: 1, whichAffiliationAct: order.Actor.Affiliation);
        }
    }

    public BattleResultClass BattleResult { get; }
    public WipeOutCheck WipeOutCheck { get; set; }
    public BattleLogClass BattleLog { get; }
}
