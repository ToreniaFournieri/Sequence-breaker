using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CalculateUnitStatus : MonoBehaviour
{

    //Input data
    public UnitClass Unit;

    //Output data
    public BattleUnit BattleUnit;

    // Output data to show magnification data
    public List<(int magnificationTargetID, int percentValue, double fixedRatioValue, double ratioValue, double totalValue, string percents)> summedOffenseList;
    public List<(int magnificationTargetID, int percentValue, double fixedRatioValue, double ratioValue, double totalValue, string percents)> summedDefenseList;


    //output middle data
    private AbilityClass _ability;
    private CombatClass _combat;
    private OffenseMagnificationClass _offenseMagnification;
    private DefenseMagnificationClass _defenseMagnification;
    private UnitSkillMagnificationClass _unitSkillMagnification;
    private FeatureClass _feature;

    // Environment Parameter
    // for combat status calculation
    public float T5LevelCoefficient;
    public float T5LevelPowLittle;
    public float T5LevelPowBig;
    public float T5AbilityCoefficient;
    public float T5AbilityPowDenominator;

    public float T4LevelCoefficient;
    public float T4LevelPowLittle;
    public float T4LevelPowBig;
    public float T4AbilityCoefficient;
    public float T4AbilityPowDenominator;

    public float T3LevelCoefficient;
    public float T3LevelPowLittle;
    public float T3LevelPowBig;
    public float T3AbilityCoefficient;
    public float T3AbilityPowDenominator;
    public float T3AbilityBasedCoefficient;

    public float T2LevelCoefficient;
    public float T2LevelPowLittle;
    public float T2LevelPowBig;
    public float T2AbilityCoefficient;
    public float T2AbilityPowDenominator;
    public float T2AbilityBasedCoefficient;


    //efficient block
    public float PowerCoefficient;
    public float CriticalHitCoefficient;
    public float NumberOfAttacksCoefficient;
    public float AccuracyCoefficient;
    public float MobilityCoefficient;
    public float DefenseCoefficient;
    public float CounterintelligenceCoefficient;
    public float RepairCoefficient;
    public float ShieldCoefficient;
    public float HPCoefficient;


    //middle data
    private AbilityClass _frameTypeAbility;
    private AbilityClass _tuningStypeAbility;
    private AbilityClass _summedItemsAddAbility;

    private CombatClass _combatRaw;
    private CombatClass _combatItems;

    //private double _optimumRangeBonusDefault;
    //private double _criticalMagnificationDefault;
    private ActionSkillClass _offenseEffectPowerActionSkill;
    private ActionSkillClass _triggerPossibilityActionSkill;
    private double _hateInitial;
    private double _hateMagnificationPerTurn;
    private double _absorbShieldRatio;
    private bool _isDamageControlAssist;


    //public AmplifyEquipmentRate AmplifyEquipmentRate
    public CalculateUnitStatus(UnitClass unitClass)
    {
        _ability = new AbilityClass(0, 0, 0, 0, 0, 0, 0);

        Unit = unitClass;

        //(1) Ability
        // (1-1) Ability caluculation -> CoreFrameAddAbility
        // CoreFrame.FrameType.AddAbility + CoreFrame.TuningStype.AddAbility

        switch (Unit.CoreFrame.FrameType)
        {
            case FrameType.caterpillar:
                _frameTypeAbility = new AbilityClass(power: 6, generation: 3, stability: 6, responsiveness: 3, precision: 2, intelligence: 0, luck: 0);
                break;
            case FrameType.reverseJointLegs:
                _frameTypeAbility = new AbilityClass(power: 3, generation: 3, stability: 3, responsiveness: 8, precision: 2, intelligence: 0, luck: 2);
                break;
            case FrameType.spiderLegs:
                _frameTypeAbility = new AbilityClass(power: 4, generation: 3, stability: 5, responsiveness: 4, precision: 3, intelligence: 0, luck: 1);
                break;
            case FrameType.twoLegs:
                _frameTypeAbility = new AbilityClass(power: 5, generation: 3, stability: 4, responsiveness: 5, precision: 2, intelligence: 0, luck: 1);
                break;
            case FrameType.wheel:
                _frameTypeAbility = new AbilityClass(power: 2, generation: 6, stability: 3, responsiveness: 4, precision: 2, intelligence: 0, luck: 3);
                break;
            default:
                Debug.LogError("CoreFrame.FrameType unexpectet value, need FrameType case update! :" + Unit.CoreFrame.FrameType);
                break;
        }
        _ability.AddUp(_frameTypeAbility);


        switch (Unit.CoreFrame.TuningStype)
        {
            case TuningStype.assaultMelee:
                _tuningStypeAbility = new AbilityClass(power: 2, generation: 0, stability: 0, responsiveness: 2, precision: 2, intelligence: 0, luck: 0);
                break;
            case TuningStype.heavyTank:
                _tuningStypeAbility = new AbilityClass(power: 0, generation: 2, stability: 4, responsiveness: 0, precision: 0, intelligence: 0, luck: 0);
                break;

            case TuningStype.chargeMelee:
                _tuningStypeAbility = new AbilityClass(power: 3, generation: 0, stability: 0, responsiveness: 0, precision: 2, intelligence: 0, luck: 0);
                break;
            case TuningStype.medic:
                _tuningStypeAbility = new AbilityClass(power: 0, generation: 5, stability: 2, responsiveness: 0, precision: 0, intelligence: 1, luck: 0);
                break;

            default:
                Debug.LogError("CoreFrame.TuningStype unexpectet value, need TuningStype case update! :" + Unit.CoreFrame.TuningStype);
                break;
        }

        _ability.AddUp(_tuningStypeAbility);

        // (1-2) Ability caluculation -> SummedItemsAddAbility
        _summedItemsAddAbility = new AbilityClass(0, 0, 0, 0, 0, 0, 0);

        foreach (Item _item in Unit.itemList)
        {
            if (_item != null)
            {
                _summedItemsAddAbility.AddUp(_item.TotaledAbility());
            }

        }
        // (1-3) Ability caluculation -> CaluculatedAbility
        // Formula:
        //  Ability = CoreFrame.Ability + Pilot.AddAbility + SummedItemsAddAbiliy
        _ability.AddUp(Unit.Pilot.AddAbility);
        _ability.AddUp(_summedItemsAddAbility);

        //(2) Combat
        // (2-1) Set environment values


        // each ability's coefficient
        {

        }

        // (2-3) First Combat caluculation -> CombatRaw
        // Formula:
        //    CombatRaw.Shield = CoreFrame.Shield (fixed, independent on Level)
        //    CombatRaw.HitPoint = CoreFrame.Hitpoint (fixed, independent on Level)
        //    CombatRaw.Others values: (
        //                                  LevelCoefficient *(Level^LevelPowLittle)
        //                                  +Level^LevelPowBig
        //                                  +Level * Ability
        //                                  +Level^(Ability / PowDenominator)*AbilityCoefficient
        //                             )*XxxxxCoefficient
        //     + Unit.CoreFrame.Shield or HitPoint;
        _combatRaw = new CombatClass();


        //Tier 5
        T5LevelPowLittle = (float)0.372;
        T5LevelPowBig = (float)1.475;
        T5AbilityCoefficient = (float)2.6;
        T5LevelCoefficient = (float)100.0;
        T5AbilityPowDenominator = (float)20.0;
        ShieldCoefficient = (float)0.82;
        HPCoefficient = (float)0.67;

        _combatRaw.ShieldMax =
                                    //Unit.CoreFrame.Shield +
                                    (int)((
                                    (T5LevelCoefficient * Mathf.Pow(Unit.Level, T5LevelPowLittle))
                                    + Mathf.Pow(Unit.Level, T5LevelPowBig)
                                    + Unit.Level * _ability.Generation
                                    + Mathf.Pow(Unit.Level, (float)_ability.Generation / T5AbilityPowDenominator) * T5AbilityCoefficient
                                  ) * ShieldCoefficient
                                    + Unit.CoreFrame.Shield);

        _combatRaw.HitPointMax =
                                //Unit.CoreFrame.HP +
                                (int)((
                                    (T5LevelCoefficient * Mathf.Pow(Unit.Level, T5LevelPowLittle))
                                    + Mathf.Pow(Unit.Level, T5LevelPowBig)
                                    + Unit.Level * _ability.Stability
                                    + Mathf.Pow(Unit.Level, (float)_ability.Stability / T5AbilityPowDenominator) * T5AbilityCoefficient
                                  ) * HPCoefficient
                                   + Unit.CoreFrame.HP
                                  );


        //Tier 4
        T4LevelPowLittle = (float)0.372;
        T4LevelPowBig = (float)1.475;
        T4AbilityCoefficient = (float)2.6;
        T4LevelCoefficient = (float)100.0;
        T4AbilityPowDenominator = (float)20.0;
        PowerCoefficient = (float)0.4;
        DefenseCoefficient = (float)0.4;

        _combatRaw.Attack = (int)((
                                    (T4LevelCoefficient * Mathf.Pow(Unit.Level, T4LevelPowLittle))
                                    + Mathf.Pow(Unit.Level, T4LevelPowBig)
                                    + Unit.Level * _ability.Power
                                    + Mathf.Pow(Unit.Level, (float)_ability.Power / T4AbilityPowDenominator) * T4AbilityCoefficient
                                  ) * PowerCoefficient);

        _combatRaw.Defense = (int)((
                                    (T4LevelCoefficient * Mathf.Pow(Unit.Level, T4LevelPowLittle))
                                    + Mathf.Pow(Unit.Level, T4LevelPowBig)
                                    + Unit.Level * _ability.Stability
                                    + Mathf.Pow(Unit.Level, (float)_ability.Stability / T4AbilityPowDenominator) * T4AbilityCoefficient
                                  ) * DefenseCoefficient);

        // Tier 3
        T3LevelPowLittle = (float)0.34;
        T3LevelPowBig = (float)1.00;
        T3AbilityCoefficient = (float)2.1;
        T3LevelCoefficient = (float)100.0;
        T3AbilityPowDenominator = (float)20.0;
        T3AbilityBasedCoefficient = (float)0.3;
        MobilityCoefficient = (float)0.233;
        AccuracyCoefficient = (float)0.233;

        _combatRaw.Mobility = (int)((
                            (T3LevelCoefficient * Mathf.Pow(Unit.Level, T3LevelPowLittle))
                            + Mathf.Pow(Unit.Level, T3LevelPowBig)
                            + Unit.Level * _ability.Responsiveness * T3AbilityBasedCoefficient
                            + Mathf.Pow(Unit.Level, (float)_ability.Responsiveness / T3AbilityPowDenominator) * T3AbilityCoefficient
                          ) * MobilityCoefficient);

        _combatRaw.Accuracy = (int)((
                            (T3LevelCoefficient * Mathf.Pow(Unit.Level, T3LevelPowLittle))
                            + Mathf.Pow(Unit.Level, T3LevelPowBig)
                            + Unit.Level * _ability.Precision * T3AbilityBasedCoefficient
                            + Mathf.Pow(Unit.Level, (float)_ability.Precision / T3AbilityPowDenominator) * T3AbilityCoefficient
                          ) * AccuracyCoefficient);

        //Tier 2
        T2LevelPowLittle = (float)0.1;
        T2LevelPowBig = (float)0.7;
        T2AbilityCoefficient = (float)2.1;
        T2LevelCoefficient = (float)100.0;
        T2AbilityPowDenominator = (float)20.0;
        T2AbilityBasedCoefficient = (float)0.0;
        CounterintelligenceCoefficient = (float)1.2;
        RepairCoefficient = (float)1.2;

        _combatRaw.Counterintelligence = (int)((
                    (T2LevelCoefficient * Mathf.Pow(Unit.Level, T2LevelPowLittle))
                    + Mathf.Pow(Unit.Level, T2LevelPowBig)
                    + Unit.Level * _ability.Intelligence * T2AbilityBasedCoefficient
                    + Mathf.Pow(Unit.Level, (float)_ability.Intelligence / T2AbilityPowDenominator) * T2AbilityCoefficient
                  ) * CounterintelligenceCoefficient);

        _combatRaw.Repair = (int)((
            (T2LevelCoefficient * Mathf.Pow(Unit.Level, T2LevelPowLittle))
            + Mathf.Pow(Unit.Level, T2LevelPowBig)
            + Unit.Level * _ability.Generation * T2AbilityBasedCoefficient
            + Mathf.Pow(Unit.Level, (float)_ability.Generation / T2AbilityPowDenominator) * T2AbilityCoefficient
          ) * RepairCoefficient);

        // Tier 1
        // critical hit = Level * Luck * CriticalHitCoefficient
        CriticalHitCoefficient = (float)0.01;

        _combatRaw.CriticalHit = (int)(
                            (Unit.Level * _ability.Luck * CriticalHitCoefficient)
                          );

        // Number of attacks
        _combatRaw.NumberOfAttacks = (int)(8);

        // Min range and Max range, default are 1.
        _combatRaw.MinRange = (int)(1);
        _combatRaw.MaxRange = (int)(6);

        // Sample set kinetic Attack ratio
        _combatRaw.KineticAttackRatio = 1.0;

        // (2-4)Core Skill consideration, coreFrame skill and Pilot skill ->CombatBaseSkillConsidered
        // Formula:
        //    CombatCoreSkillConsidered.someValue = (CombatRaw.someValue + (CoreFrame and Pilot).skills.addCombat.someValue) * (CoreFrame and Pilot).skills.amplifyCombat.someValue

        // some code here. omitted


        // (2-5)Skill consideration of items equiped -> CombatItemSkillConsidered
        // Formula:
        //    CombatItemSkillConsidered.someValue = (CombatCoreSkillConsidered.someValue + itemList.skills.addCombat.someValue) * itemList.skills.amplifyCombat.someValue

        _combatRaw.CriticalHit = _combatRaw.CriticalHit + 18;
        // some code here. omitted

        // (2-6)Add combat value of items equiped -> CombatItemEquiped
        // Formula:
        //    CombatItemEquiped.someValue = CombatItemSkillConsidered.someValue + itemList.addCombat.someValue * AmplifyEquipmentRate.someParts

        _combatItems = new CombatClass();

        foreach (Item _item in Unit.itemList)
        {
            if (_item != null)
            {
                _combatItems.Add(_item.TotaledCombat());
            }
        }

        // some code here. omitted

        // (2-7)Finalize
        // Formula:
        //    CombatCaluculated = CombatItemEquiped

        _combat = new CombatClass();
        _combat.Add(_combatRaw);
        if (_combatItems != null) { _combat.Add(_combatItems); }


        //(3) Feature
        // (3-1) Set environment values
        _absorbShieldRatio = 0.1;
        _hateInitial = 10;
        _hateMagnificationPerTurn = 0.667;
        //_optimumRangeBonusDefault = 1.2;
        //_criticalMagnificationDefault = 1.4; this will obsolate
        _offenseEffectPowerActionSkill = new ActionSkillClass(1, 1, 1, 1, 1, 1, 1, 1);
        _triggerPossibilityActionSkill = new ActionSkillClass(1, 1, 1, 1, 1, 1, 1, 1);
        // (3-2) 
        //   double absorbShieldInitial, bool damageControlAssist, double hateInitial, double hateMagnificationPerTurn)
        // Formula:
        //   absorbShieldInitial = CoreFrame.addFeature.absorbShield + Pilot.addFeature.absorbShield
        //   damageControlAssist = CoreFrame.TuningStype.damageControlAssist
        //   hateInitial = (default value) + Pilot.addFeature.hate
        //   hateMagnificationPerTurn = (defalut value) * Pilot.addFeature.hateMagnificationPerTurn

        _isDamageControlAssist = false;

        switch (Unit.CoreFrame.TuningStype)
        {
            case TuningStype.medic:
                _isDamageControlAssist = true;
                break;
            default:
                break;
        }

        _feature = new FeatureClass(absorbShieldInitial: _absorbShieldRatio, damageControlAssist: _isDamageControlAssist,
            hateInitial: _hateInitial, hateMagnificationPerTurn: _hateMagnificationPerTurn);


        //(4)Offense/Defense/UnitSkill Magnification

        List<MagnificationMasterClass> _itemMagnificationMasterList = new List<MagnificationMasterClass>();
        foreach (Item _item in Unit.itemList)
        {
            if (_item != null)
            {
                //item has prefix, base and suffix. each one has magnification master calss list. collect them all
                if (_item.prefixItem != null)
                {
                    foreach (MagnificationMasterClass _magnificationMaster in _item.prefixItem.MagnificationMasterList)
                    {
                        _itemMagnificationMasterList.Add(_magnificationMaster);
                    }
                }

                if (_item.baseItem != null)
                {
                    foreach (MagnificationMasterClass _magnificationMaster in _item.baseItem.MagnificationMasterList)
                    {
                        _itemMagnificationMasterList.Add(_magnificationMaster);
                    }
                }

                if (_item.suffixItem != null)
                {
                    foreach (MagnificationMasterClass _magnificationMaster in _item.suffixItem.MagnificationMasterList)
                    {
                        _itemMagnificationMasterList.Add(_magnificationMaster);
                    }
                }
            }
        }

        // Deduplication
        MagnificationComparer Comparer = new MagnificationComparer();
        IEnumerable<MagnificationMasterClass> _deduplicationedMagnification = _itemMagnificationMasterList.Distinct(Comparer);


        // this List is magnificationTarget
        // magnificationTargetID is stand for enum magnificationTarget.

        List<(int magnificationTargetID, int _percent, double _fixedRatio, double _ratio, List<int> _percentList)> magnificationOffenseList;
        List<(int magnificationTargetID, int _percent, double _fixedRatio, double _ratio, List<int> _percentList)> magnificationDefenseList;

        magnificationOffenseList = new List<(int magnificationTargetID, int _percentSummed, double _fixedRatioSummed, double _ratioSummed, List<int> _percentList)>();
        magnificationDefenseList = new List<(int magnificationTargetID, int _percentSummed, double _fixedRatioSummed, double _ratioSummed, List<int> _percentList)>();

        // calculated values
        List<(int magnificationTargetID, int percentValue, double fixedValue, double ratioValue)> calculatedMagOffenseList
            = new List<(int magnificationTargetID, int percentValue, double fixedValue, double magnificationValue)>();
        List<(int magnificationTargetID, int percentValue, double fixedValue, double ratioValue)> calculatedMagDefenseList
            = new List<(int magnificationTargetID, int percentValue, double fixedValue, double magnificationValue)>();

        // final value
        summedOffenseList = new List<(int magnificationTargetID, int percentValue, double fixedRatioValue, double ratioValue, double totalValue, string )>();
        summedDefenseList = new List<(int magnificationTargetID, int percentValue, double fixedRatioValue, double ratioValue, double totalValue, string)>();


        // get length of enum magnification Target
        int totalMagnificationTargetLength = Enum.GetNames(typeof(MagnificationTarget)).Length;

        // initialize
        for (int i = 0; i < totalMagnificationTargetLength; i++)
        {
            magnificationOffenseList.Add((i, 0, 1.0, 1.0, null));
            magnificationDefenseList.Add((i, 0, 1.0, 1.0, null));
            //calculatedMagOffenseList.Add((i, 0, 1.0, 1.0));
            //calculatedMagDefenseList.Add((i, 0, 1.0, 1.0));
            summedOffenseList.Add((i, 0, 1.0, 1.0, 1.0, null));
            summedDefenseList.Add((i, 0, 1.0, 1.0, 1.0, null));
        }

        //string _debuMagnificationText = null;
        foreach (MagnificationMasterClass magnificationClass in _deduplicationedMagnification)
        {
            //fortest
            (int magnificationTargetID, int _percent, double _fixedRatio, double _ratio, List<int> _percentList) magnification;

            switch (magnificationClass.OffenseOrDefense)
            {
                case OffenseOrDefense.none:
                    break;
                case OffenseOrDefense.Offense:
                    magnification = MagnificationTypeModule(magnificationClass);

                    if (magnificationOffenseList[magnification.magnificationTargetID]._percentList != null)
                    {
                        foreach (int i in magnificationOffenseList[magnification.magnificationTargetID]._percentList)
                        {
                            magnification._percentList.Add(i);
                        }
                    }

                    magnification = (magnification.magnificationTargetID,
                        magnificationOffenseList[magnification.magnificationTargetID]._percent + magnification._percent,
                        magnificationOffenseList[magnification.magnificationTargetID]._fixedRatio * magnification._fixedRatio,
                        magnificationOffenseList[magnification.magnificationTargetID]._ratio * magnification._ratio,
                        magnification._percentList
                        );

                    magnificationOffenseList[magnification.magnificationTargetID] = magnification;

                    break;
                case OffenseOrDefense.Defense:
                    magnification = MagnificationTypeModule(magnificationClass);
                    if (magnificationDefenseList[magnification.magnificationTargetID]._percentList != null)
                    {
                        foreach (int i in magnificationDefenseList[magnification.magnificationTargetID]._percentList)
                        {
                            magnification._percentList.Add(i);
                        }
                    }
                    magnification = (magnification.magnificationTargetID,
                        magnificationDefenseList[magnification.magnificationTargetID]._percent + magnification._percent,
                        magnificationDefenseList[magnification.magnificationTargetID]._fixedRatio * magnification._fixedRatio,
                        magnificationDefenseList[magnification.magnificationTargetID]._ratio * magnification._ratio,
                        magnification._percentList
                        );

                    magnificationDefenseList[magnification.magnificationTargetID] = magnification;


                    break;
                default:
                    Debug.LogError("unexpected OffenseOrDefense Value:" + magnificationClass.OffenseOrDefense);
                    break;
            }

            //_debuMagnificationText += "[" + magnificationDefenseList[(int)magnificationClass.MagnificationTarget] + " percent: "
            //   + magnificationDefenseList[(int)magnificationClass.MagnificationTarget]._percent + "\n";

        }




        foreach ((int magnificationTargetID, int percentValue, double fixedValue, double ratioValue, List<int> percentList) calculated in magnificationOffenseList)
        {
            double total = 1.0 / (1.0 - calculated.percentValue * 0.01) * calculated.fixedValue * calculated.ratioValue;

            string _percents = null;
            if (calculated.percentList != null)
            {
                calculated.percentList.Sort();

                foreach (int i in calculated.percentList)
                {
                    _percents += i + " ";
                }

            }

            summedOffenseList[calculated.magnificationTargetID] =
                (
                calculated.magnificationTargetID,
                calculated.percentValue,
                calculated.fixedValue,
                calculated.ratioValue,
                total,
                _percents
                );
        }

        foreach ((int magnificationTargetID, int percentValue, double fixedValue, double ratioValue, List<int> percentList) calculated in magnificationDefenseList)
        {
            double total = (1.0 - calculated.percentValue * 0.01) * calculated.fixedValue * calculated.ratioValue;

            string _percents = null;
            if (calculated.percentList != null)
            {
                calculated.percentList.Sort();
                foreach (int i in calculated.percentList)
                {
                    _percents += i + " ";
                }
            }

            summedDefenseList[calculated.magnificationTargetID] =
                (
                calculated.magnificationTargetID,
                calculated.percentValue,
                calculated.fixedValue,
                calculated.ratioValue,
                total,
                _percents
                );
        }

        // 2019.9.23 MagnificationTarget
        // 0:none, 1:Critical, 2:Kinetic, 3:Chemical, 4:Thermal, 5:VsBeast, 6:VsCyborg, 7:VsDrone, 8:VsRobot, 9:VsTitan, 10:OptimumRangeBonus
        //  

        _offenseMagnification = new OffenseMagnificationClass(
            optimumRangeBonus: summedOffenseList[10].totalValue,
            critical: summedOffenseList[1].totalValue,
            kinetic: summedOffenseList[2].totalValue,
            chemical: summedOffenseList[3].totalValue,
            thermal: summedOffenseList[4].totalValue,
            vsBeast: summedOffenseList[5].totalValue,
            vsCyborg: summedOffenseList[6].totalValue,
            vsDrone: summedOffenseList[7].totalValue,
            vsRobot: summedOffenseList[8].totalValue,
            vsTitan: summedOffenseList[9].totalValue);
        _defenseMagnification = new DefenseMagnificationClass(
            critical: summedDefenseList[1].totalValue,
            kinetic: summedDefenseList[2].totalValue,
            chemical: summedDefenseList[3].totalValue,
            thermal: summedDefenseList[4].totalValue,
            vsBeast: summedDefenseList[5].totalValue,
            vsCyborg: summedDefenseList[6].totalValue,
            vsDrone: summedDefenseList[7].totalValue,
            vsRobot: summedDefenseList[8].totalValue,
            vsTitan: summedDefenseList[9].totalValue);

        _unitSkillMagnification = new UnitSkillMagnificationClass(offenseEffectPower: _offenseEffectPowerActionSkill, triggerPossibility: _triggerPossibilityActionSkill);

        //BattleUnit(int uniqueID, string name, Affiliation affiliation, UnitType unitType, AbilityClass ability, CombatClass combat, FeatureClass feature,

        BattleUnit = new BattleUnit(uniqueID: 1, name: unitClass.Name, affiliation: Affiliation.none, unitType: unitClass.UnitType, ability: _ability,
            combat: _combat, feature: _feature, offenseMagnification: _offenseMagnification,
            defenseMagnification: _defenseMagnification, skillMagnification: _unitSkillMagnification);


    }

    private (int magnificationTargetID, int _percent, double _fixedRatio, double _ratio, List<int> _percentList) MagnificationTypeModule(MagnificationMasterClass magnificationClass)
    {
        int _magnificationTargetID = (int)magnificationClass.MagnificationTarget;
        int _percentSummed = 0;
        double _fixedRatioSummed = 1.0;
        double _ratioSummed = 1.0;
        List<int> _percentList = new List<int>();
        switch (magnificationClass.MagnificationType)
        {
            case MagnificationType.none:
                break;
            case MagnificationType.AdditionalPercent:
                // values and Enum ID is equal
                _percentSummed += (int)magnificationClass.MagnificationPercent;
                _percentList.Add((int)magnificationClass.MagnificationPercent);
                //Debug.Log(magnificationClass.MagnificationPercent + " is "+ (int)magnificationClass.MagnificationPercent);
                break;
            case MagnificationType.MagnificationFixedRatio:
                FixedRatioCalculator _fix = new FixedRatioCalculator(magnificationClass.MagnificationFixedRatio);
                //Debug.Log( magnificationClass.MagnificationTarget + " magnificationFixedRatio: " + magnificationClass.MagnificationFixedRatio + " value:" + _fix.value);
                _fixedRatioSummed = _fix.value;
                break;
            case MagnificationType.MagnificationRatio:
                _ratioSummed *= magnificationClass.MagnificationRatio;
                break;
            default:
                Debug.Log("unexpected value:" + magnificationClass.MagnificationType);
                break;
        }

        return (_magnificationTargetID, _percentSummed, _fixedRatioSummed, _ratioSummed, _percentList);
    }


    private class MagnificationComparer : IEqualityComparer<MagnificationMasterClass>
    {
        public bool Equals(MagnificationMasterClass i_lhs, MagnificationMasterClass i_rhs)
        {
            if (
                i_lhs.OffenseOrDefense == i_rhs.OffenseOrDefense &&
                i_lhs.MagnificationFixedRatio == i_rhs.MagnificationFixedRatio &&
                i_lhs.MagnificationPercent == i_rhs.MagnificationPercent &&
                System.Math.Abs(i_lhs.MagnificationRatio - i_rhs.MagnificationRatio) < System.Double.Epsilon &&
                i_lhs.MagnificationTarget == i_rhs.MagnificationTarget &&
                i_lhs.MagnificationType == i_rhs.MagnificationType
                )
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(MagnificationMasterClass i_obj)
        {
            return i_obj.OffenseOrDefense.GetHashCode()
                ^ i_obj.MagnificationFixedRatio.GetHashCode()
                ^ i_obj.MagnificationPercent.GetHashCode()
                ^ i_obj.MagnificationRatio.GetHashCode()
                ^ i_obj.MagnificationTarget.GetHashCode()
                ^ i_obj.MagnificationType.GetHashCode();
        }
    }

}
