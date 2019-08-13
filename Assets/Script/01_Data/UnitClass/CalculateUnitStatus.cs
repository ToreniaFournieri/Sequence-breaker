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

    //middle data
    private AbilityClass _frameTypeAbility;
    private AbilityClass _tuningStypeAbility;
    private AbilityClass _summedItemsAddAbility;

    private CombatClass _combatRaw;

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
        // (2-1) First Combat caluculation -> CombatRaw
        // Formula:
        //    CombatRaw.Shield = CoreFrame.Shield (fixed, independent on Level)
        //    CombatRaw.HitPoint = CoreFrame.Hitpoint (fixed, independent on Level)
        //    CombatRaw.Others values: Level * Ability * coefficient(depend on others values)


        _combatRaw = new CombatClass();

        _combatRaw.ShiledMax = Unit.CoreFrame.Shield;
        _combatRaw.HitPointMax = Unit.CoreFrame.HP;
        _combatRaw.Attack = Unit.Level * Ability.Power + (int)(Mathf.Pow(Unit.Level, (float)1.3) * Ability.Power * 0.4) ;


        // (2-3)Core Skill consideration, coreFrame skill and Pilot skill ->CombatBaseSkillConsidered
        // Formula:
        //    CombatCoreSkillConsidered.someValue = (CombatRaw.someValue + (CoreFrame and Pilot).skills.addCombat.someValue) * (CoreFrame and Pilot).skills.amplifyCombat.someValue

        // (2-4)Skill consideration of items equiped -> CombatItemSkillConsidered
        // Formula:
        //    CombatItemSkillConsidered.someValue = (CombatCoreSkillConsidered.someValue + itemList.skills.addCombat.someValue) * itemList.skills.amplifyCombat.someValue

        // (2-5)Add combat value of items equiped -> CombatItemEquiped
        // Formula:
        //    CombatItemEquiped.someValue = CombatItemSkillConsidered.someValue + itemList.addCombat.someValue * AmplifyEquipmentRate.someParts

        // (2-6)Finalize
        // Formula:
        //    CombatCaluculated = CombatItemEquiped

        Combat = _combatRaw;

        //(3) Feature
        //   double absorbShieldInitial, bool damageControlAssist, double hateInitial, double hateMagnificationPerTurn)
        // Formula:
        //   absorbShieldInitial = CoreFrame.addFeature.absorbShield + Pilot.addFeature.absorbShield
        //   damageControlAssist = CoreFrame.TuningStype.damageControlAssist
        //   hateInitial = (default value) + Pilot.addFeature.hate
        //   hateMagnificationPerTurn = (defalut value) * Pilot.addFeature.hateMagnificationPerTurn

        //(4)Offense/Defense/UnitSkill Magnification


        //BattleUnit(int uniqueID, string name, Affiliation affiliation, UnitType unitType, AbilityClass ability, CombatClass combat, FeatureClass feature,
        //OffenseMagnificationClass offenseMagnification, DefenseMagnificationClass defenseMagnification, UnitSkillMagnificationClass skillMagnification)

        //BattleUnit battleUnit = new BattleUnit();

    }

}
