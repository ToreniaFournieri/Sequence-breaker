using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit-", menuName = "Unit/Unit", order = 3)]
public class UnitClass : ScriptableObject
{
    [SerializeField] public int UniqueID;
    [SerializeField] public string Name;
    [SerializeField] public Affiliation Affiliation;
    [SerializeField] public UnitType UnitType;
    [SerializeField] public AbilityClass InitialAbility;
    [SerializeField] public int ItemCapacity;

    [SerializeField] public List<Item> itemList;

    public AbilityClass Ability { get; set; }
    [SerializeField] public CoreFrame CoreFrame;
    [SerializeField] public Pilot Pilot;
    [SerializeField] public int Level;

    private AbilityClass CoreFrameAddAbility; // = new AbilityClass(0, 0, 0, 0, 0, 0, 0);
    private AbilityClass PilotAddAbility; // = new AbilityClass(0, 0, 0, 0, 0, 0, 0);
    private AbilityClass SummedItemsAddAbility; // = new AbilityClass(0, 0, 0, 0, 0, 0, 0);
    AbilityClass FrameTypeAbility;
    AbilityClass TuningStypeAbility;

    // public AmplifyEquipmentRate AmplifyEquipmentRate

    //public BattleUnit CaluculatedBattleUnit()
    //{
    //    //(1) Ability
    //    // (1-1) Ability caluculation -> CoreFrameAddAbility
    //    // CoreFrame.FrameType.AddAbility + CoreFrame.TuningStype.AddAbility

    //    switch (CoreFrame.FrameType)
    //    {
    //        case FrameType.caterpillar:
    //            FrameTypeAbility = new AbilityClass(power: 6, generation: 3, stability: 6, responsiveness: 3, precision: 2, intelligence: 0, luck: 0);
    //            break;
    //        case FrameType.reverseJointLegs:
    //            FrameTypeAbility = new AbilityClass(power: 3, generation: 3, stability: 3, responsiveness: 8, precision: 2, intelligence: 0, luck: 2);
    //            break;
    //        case FrameType.spiderLegs:
    //            FrameTypeAbility = new AbilityClass(power: 4, generation: 3, stability: 5, responsiveness: 4, precision: 3, intelligence: 0, luck: 1);
    //            break;
    //        case FrameType.twoLegs:
    //            FrameTypeAbility = new AbilityClass(power: 5, generation: 3, stability: 4, responsiveness: 5, precision: 2, intelligence: 0, luck: 1);
    //            break;
    //        case FrameType.wheel:
    //            FrameTypeAbility = new AbilityClass(power: 2, generation: 6, stability: 3, responsiveness: 4, precision: 2, intelligence: 0, luck: 3);
    //            break; 
    //        default:
    //            Debug.Log("CoreFrame.FrameType unexpectet value, need FrameType case update! :" + CoreFrame.FrameType);
    //            break;
    //    }

    //    switch (CoreFrame.TuningStype)
    //    {
    //        case TuningStype.assaultMelee:
    //            TuningStypeAbility = new AbilityClass(power: 2, generation: 0, stability: 0, responsiveness: 2, precision: 2, intelligence: 0, luck: 0);
    //            break;
    //        case TuningStype.heavyTank:
    //            TuningStypeAbility = new AbilityClass(power: 0, generation: 2, stability: 4, responsiveness: 0, precision: 0, intelligence: 0, luck: 0);
    //            break;
    //        default:
    //            Debug.Log("CoreFrame.TuningStype unexpectet value, need TuningStype case update! :" + CoreFrame.TuningStype);
    //            break;
    //    }

    //    CoreFrameAddAbility = new AbilityClass(First: FrameTypeAbility,Second: TuningStypeAbility);


    //    // (1-2) Ability caluculation -> SummedItemsAddAbility
    //    SummedItemsAddAbility = new AbilityClass(0, 0, 0, 0, 0, 0, 0);
    //    // (1-3) Ability caluculation -> CaluculatedAbility
    //    // Formula:
    //    //  Ability = CoreFrame.Ability + Pilot.AddAbility + SummedItemsAddAbiliy

    //    Ability.AddUp(CoreFrameAddAbility, SummedItemsAddAbility);

    //    //(2) Combat
    //    // (2-1) First Combat caluculation -> CombatRaw
    //    // Formula:
    //    //    CombatRaw.Shield = CoreFrame.Hitpoint (fixed, independent on Level)
    //    //    CombatRaw.HitPoint = CoreFrame.Hitpoint (fixed, independent on Level)
    //    //    CombatRaw.Others values: Level * Ability * coefficient(depend on others values)

    //    // (2-3)Core Skill consideration, coreFrame skill and Pilot skill ->CombatBaseSkillConsidered
    //    // Formula:
    //    //    CombatCoreSkillConsidered.someValue = (CombatRaw.someValue + (CoreFrame and Pilot).skills.addCombat.someValue) * (CoreFrame and Pilot).skills.amplifyCombat.someValue

    //    // (2-4)Skill consideration of items equiped -> CombatItemSkillConsidered
    //    // Formula:
    //    //    CombatItemSkillConsidered.someValue = (CombatCoreSkillConsidered.someValue + itemList.skills.addCombat.someValue) * itemList.skills.amplifyCombat.someValue

    //    // (2-5)Add combat value of items equiped -> CombatItemEquiped
    //    // Formula:
    //    //    CombatItemEquiped.someValue = CombatItemSkillConsidered.someValue + itemList.addCombat.someValue * AmplifyEquipmentRate.someParts

    //    // (2-6)Finalize
    //    // Formula:
    //    //    CombatCaluculated = CombatItemEquiped

    //    //(3) Feature
    //    //   double absorbShieldInitial, bool damageControlAssist, double hateInitial, double hateMagnificationPerTurn)
    //    // Formula:
    //    //   absorbShieldInitial = CoreFrame.addFeature.absorbShield + Pilot.addFeature.absorbShield
    //    //   damageControlAssist = CoreFrame.TuningStype.damageControlAssist
    //    //   hateInitial = (default value) + Pilot.addFeature.hate
    //    //   hateMagnificationPerTurn = (defalut value) * Pilot.addFeature.hateMagnificationPerTurn

    //    //(4)Offense/Defense/UnitSkill Magnification


    //    //BattleUnit(int uniqueID, string name, Affiliation affiliation, UnitType unitType, AbilityClass ability, CombatClass combat, FeatureClass feature,
    //    //OffenseMagnificationClass offenseMagnification, DefenseMagnificationClass defenseMagnification, UnitSkillMagnificationClass skillMagnification)

    //    //BattleUnit battleUnit = new BattleUnit();

    //    return null;
    //}
    //public AbilityClass CaluculatedAbility;
    //public CombatClass CombatCaluculated;

}



