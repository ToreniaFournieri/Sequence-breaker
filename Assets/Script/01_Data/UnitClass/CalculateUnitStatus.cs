using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateUnitStatus : MonoBehaviour
{

    //Input data
    public UnitClass Unit;

    //Output data
    public AbilityClass Ability;
    public CombatClass Combat;
    public OffenseMagnificationClass OffenseMagnification;
    public DefenseMagnificationClass DefenseMagnification;
    public UnitSkillMagnificationClass UnitSkillMagnification;
    public FeatureClass Feature;

    // Environment Parameter
    // for combat status calculation
    public float LevelCoefficient;
    public float LevelPowLittle;
    public float LevelPowBig;
    public float AbilityCoefficient;
    public float AbilityPowDenominator;

    //efficient block
    public float PowerCoefficient;
    public float CriticalHitCoefficient;
    public float NumberOfAttacksCoefficient;
    public float AccuracyCoefficient;
    public float MobilityCoefficient;
    public float DefenseCoefficient;


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
        Ability = new AbilityClass(3, 0, 0, 0, 0, 0, 0);

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
        Ability.AddUp(_frameTypeAbility);


        switch (Unit.CoreFrame.TuningStype)
        {
            case TuningStype.assaultMelee:
                _tuningStypeAbility = new AbilityClass(power: 2, generation: 0, stability: 0, responsiveness: 2, precision: 2, intelligence: 0, luck: 0);
                break;
            case TuningStype.heavyTank:
                _tuningStypeAbility = new AbilityClass(power: 0, generation: 2, stability: 4, responsiveness: 0, precision: 0, intelligence: 0, luck: 0);
                break;
            default:
                Debug.Log("CoreFrame.TuningStype unexpectet value, need TuningStype case update! :" + Unit.CoreFrame.TuningStype);
                break;
        }

        Ability.AddUp(_tuningStypeAbility);

        // (1-2) Ability caluculation -> SummedItemsAddAbility
        _summedItemsAddAbility = new AbilityClass(0, 0, 0, 0, 0, 1, 1); // This is dummy, need logic to collect data from UnitClass.ItemLists.
        // (1-3) Ability caluculation -> CaluculatedAbility
        // Formula:
        //  Ability = CoreFrame.Ability + Pilot.AddAbility + SummedItemsAddAbiliy
        Ability.AddUp(Unit.Pilot.AddAbility);
        Ability.AddUp(_summedItemsAddAbility);

        //(2) Combat
        // (2-1) Set environment values
        LevelPowLittle = (float)0.372;
        LevelPowBig = (float)1.475;
        AbilityCoefficient = (float)2.6;

        // each ability's coefficient
        {
            PowerCoefficient = (float)0.4;
            DefenseCoefficient = (float)0.4;
            MobilityCoefficient = (float)0.133;
            AccuracyCoefficient = (float)0.233;
            CriticalHitCoefficient = (float)0.01;
        }
        LevelCoefficient = (float)100.0;
        AbilityPowDenominator = (float)20.0;

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

        _combatRaw.ShiledMax = Unit.CoreFrame.Shield;
        _combatRaw.HitPointMax = Unit.CoreFrame.HP;

        _combatRaw.Attack = (int)((
                                    (LevelCoefficient * Mathf.Pow(Unit.Level, LevelPowLittle))
                                    + Mathf.Pow(Unit.Level, LevelPowBig)
                                    + Unit.Level * Ability.Power
                                    + Mathf.Pow(Unit.Level, (float)Ability.Power / AbilityPowDenominator) * AbilityCoefficient
                                  ) * PowerCoefficient);

        _combatRaw.Defense = (int)((
                                    (LevelCoefficient * Mathf.Pow(Unit.Level, LevelPowLittle))
                                    + Mathf.Pow(Unit.Level, LevelPowBig)
                                    + Unit.Level * Ability.Stability
                                    + Mathf.Pow(Unit.Level, (float)Ability.Stability / AbilityPowDenominator) * AbilityCoefficient
                                  ) * DefenseCoefficient);

        _combatRaw.Mobility = (int)((
                            (LevelCoefficient * Mathf.Pow(Unit.Level, LevelPowLittle))
                            + Mathf.Pow(Unit.Level, LevelPowBig)
                            + Unit.Level * Ability.Responsiveness
                            + Mathf.Pow(Unit.Level, (float)Ability.Responsiveness / AbilityPowDenominator) * AbilityCoefficient
                          ) * MobilityCoefficient);

        _combatRaw.Accuracy = (int)((
                            (LevelCoefficient * Mathf.Pow(Unit.Level, LevelPowLittle))
                            + Mathf.Pow(Unit.Level, LevelPowBig)
                            + Unit.Level * Ability.Precision
                            + Mathf.Pow(Unit.Level, (float)Ability.Precision / AbilityPowDenominator) * AbilityCoefficient
                          ) * AccuracyCoefficient);


        // critical hit = Level * Luck * CriticalHitCoefficient
        _combatRaw.CriticalHit = (int)(
                            (Unit.Level * Ability.Luck * CriticalHitCoefficient)
                          );

        // Number of attacks
        _combatRaw.NumberOfAttacks = (int)(1);

        // Min range and Max range, default are 1.
        _combatRaw.MinRange = (int)(1);
        _combatRaw.MaxRange = (int)(1);

        // (2-4)Core Skill consideration, coreFrame skill and Pilot skill ->CombatBaseSkillConsidered
        // Formula:
        //    CombatCoreSkillConsidered.someValue = (CombatRaw.someValue + (CoreFrame and Pilot).skills.addCombat.someValue) * (CoreFrame and Pilot).skills.amplifyCombat.someValue

        // some code here. omitted


        // (2-5)Skill consideration of items equiped -> CombatItemSkillConsidered
        // Formula:
        //    CombatItemSkillConsidered.someValue = (CombatCoreSkillConsidered.someValue + itemList.skills.addCombat.someValue) * itemList.skills.amplifyCombat.someValue

        // some code here. omitted

        // (2-6)Add combat value of items equiped -> CombatItemEquiped
        // Formula:
        //    CombatItemEquiped.someValue = CombatItemSkillConsidered.someValue + itemList.addCombat.someValue * AmplifyEquipmentRate.someParts

        // some code here. omitted

        // (2-7)Finalize
        // Formula:
        //    CombatCaluculated = CombatItemEquiped

        Combat = _combatRaw;


        //(3) Feature
        // (3-1) Set environment values
        _absorbShieldRatio = 0.0;
        _hateInitial = 10;
        _hateMagnificationPerTurn = 0.667;
        _optimumRangeBonusDefault = 1.2;
        _criticalMagnificationDefault = 1.5;
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

        Feature = new FeatureClass(absorbShieldInitial: _absorbShieldRatio, damageControlAssist: _isDamageControlAssist,
            hateInitial: _hateInitial, hateMagnificationPerTurn: _hateMagnificationPerTurn);


        //(4)Offense/Defense/UnitSkill Magnification
        OffenseMagnification = new OffenseMagnificationClass(optimumRangeBonus: _optimumRangeBonusDefault, critical: _criticalMagnificationDefault,
            kinetic: 1.0, chemical: 1.0, thermal: 1.0, vsBeast: 1.0, vsCyborg: 1.0, vsDrone: 1.0, vsRobot: 1.0, vsTitan: 1.0);
        DefenseMagnification = new DefenseMagnificationClass(critical: _criticalMagnificationDefault,
            kinetic: 1.0, chemical: 1.0, thermal: 1.0, vsBeast: 1.0, vsCyborg: 1.0, vsDrone: 1.0, vsRobot: 1.0, vsTitan: 1.0);

        UnitSkillMagnification = new UnitSkillMagnificationClass(offenseEffectPower: _offenseEffectPowerActionSkill, triggerPossibility: _triggerPossibilityActionSkill);

        //BattleUnit(int uniqueID, string name, Affiliation affiliation, UnitType unitType, AbilityClass ability, CombatClass combat, FeatureClass feature,

        //OffenseMagnificationClass offenseMagnification, DefenseMagnificationClass defenseMagnification, UnitSkillMagnificationClass skillMagnification)

        //BattleUnit battleUnit = new BattleUnit();

    }

}
