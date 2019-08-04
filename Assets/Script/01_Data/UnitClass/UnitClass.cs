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



    // public AmplifyEquipmentRate AmplifyEquipmentRate

    public BattleUnit CaluculatedBattleUnit()
    {
        //(1) Ability
        // (1-1) Ability caluculation -> CaluculatedAbility
        // Formula:
        //  Ability = CoreFrame.Ability + Pilot.AddAbility + SummedItems.AddAbiliy

        //(2) Combat
        // (2-1) First Combat caluculation -> CombatRaw
        // Formula:
        //    CombatRaw.Shield = CoreFrame.Hitpoint (fixed, independent on Level)
        //    CombatRaw.HitPoint = CoreFrame.Hitpoint (fixed, independent on Level)
        //    CombatRaw.Others values: Level * Ability * coefficient(depend on others values)

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

        return null;
    }
    public AbilityClass CaluculatedAbility;
    public CombatClass CombatCaluculated;

}



