using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateUnitStatus : MonoBehaviour
{

    //Input data
    public UnitClass Unit;

    //Output data
    public BattleUnit BattleUnit;

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

    private double _optimumRangeBonusDefault;
    private double _criticalMagnificationDefault;
    private ActionSkillClass _offenseEffectPowerActionSkill;
    private ActionSkillClass _triggerPossibilityActionSkill;
    private double _hateInitial;
    private double _hateMagnificationPerTurn;
    private double _absorbShieldRatio;
    private bool _isDamageControlAssist;


    //public AmplifyEquipmentRate AmplifyEquipmentRate
    public CalculateUnitStatus(UnitClass unitClass)
    {
        _ability = new AbilityClass(3, 0, 0, 0, 0, 0, 0);

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
                Debug.Log("CoreFrame.FrameType unexpectet value, need FrameType case update! :" + Unit.CoreFrame.FrameType);
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
                Debug.Log("CoreFrame.TuningStype unexpectet value, need TuningStype case update! :" + Unit.CoreFrame.TuningStype);
                break;
        }

        _ability.AddUp(_tuningStypeAbility);

        // (1-2) Ability caluculation -> SummedItemsAddAbility
        _summedItemsAddAbility = new AbilityClass(0, 0, 0, 0, 0, 1, 1); // This is dummy, need logic to collect data from UnitClass.ItemLists.
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
        _combatRaw = new CombatClass();


        //Tier 5
        T5LevelPowLittle = (float)0.372;
        T5LevelPowBig = (float)1.475;
        T5AbilityCoefficient = (float)2.6;
        T5LevelCoefficient = (float)100.0;
        T5AbilityPowDenominator = (float)20.0;
        ShieldCoefficient = (float)0.82;
        HPCoefficient = (float)0.67;

        _combatRaw.ShiledMax =
                                //Unit.CoreFrame.Shield +
                                    (int)((
                                    (T5LevelCoefficient * Mathf.Pow(Unit.Level, T5LevelPowLittle))
                                    + Mathf.Pow(Unit.Level, T5LevelPowBig)
                                    + Unit.Level * _ability.Generation
                                    + Mathf.Pow(Unit.Level, (float)_ability.Generation / T5AbilityPowDenominator) * T5AbilityCoefficient
                                  ) * ShieldCoefficient);

        _combatRaw.HitPointMax =
                                //Unit.CoreFrame.HP +
                                (int)((
                                    (T5LevelCoefficient * Mathf.Pow(Unit.Level, T5LevelPowLittle))
                                    + Mathf.Pow(Unit.Level, T5LevelPowBig)
                                    + Unit.Level * _ability.Stability
                                    + Mathf.Pow(Unit.Level, (float)_ability.Stability / T5AbilityPowDenominator) * T5AbilityCoefficient
                                  ) *HPCoefficient);


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

        // some code here. omitted

        // (2-7)Finalize
        // Formula:
        //    CombatCaluculated = CombatItemEquiped

        _combat = _combatRaw;


        //(3) Feature
        // (3-1) Set environment values
        _absorbShieldRatio = 0.1;
        _hateInitial = 10;
        _hateMagnificationPerTurn = 0.667;
        _optimumRangeBonusDefault = 1.2;
        _criticalMagnificationDefault = 1.4;
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
        _offenseMagnification = new OffenseMagnificationClass(optimumRangeBonus: _optimumRangeBonusDefault, critical: _criticalMagnificationDefault,
            kinetic: 1.0, chemical: 1.0, thermal: 1.0, vsBeast: 1.0, vsCyborg: 1.0, vsDrone: 1.0, vsRobot: 1.0, vsTitan: 1.0);
        _defenseMagnification = new DefenseMagnificationClass(critical: _criticalMagnificationDefault,
            kinetic: 1.0, chemical: 1.0, thermal: 1.0, vsBeast: 1.0, vsCyborg: 1.0, vsDrone: 1.0, vsRobot: 1.0, vsTitan: 1.0);

        _unitSkillMagnification = new UnitSkillMagnificationClass(offenseEffectPower: _offenseEffectPowerActionSkill, triggerPossibility: _triggerPossibilityActionSkill);

        //BattleUnit(int uniqueID, string name, Affiliation affiliation, UnitType unitType, AbilityClass ability, CombatClass combat, FeatureClass feature,

        BattleUnit = new BattleUnit(uniqueID: 1, name: unitClass.Name, affiliation: Affiliation.none, unitType: unitClass.UnitType, ability: _ability,
            combat: _combat, feature: _feature, offenseMagnification: _offenseMagnification,
            defenseMagnification: _defenseMagnification, skillMagnification: _unitSkillMagnification ) ;


    }

}
