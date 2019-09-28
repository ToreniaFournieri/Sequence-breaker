using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemBase-", menuName = "Item/ItemBase", order = 1)]
public class ItemBaseMaster : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite icon;

    //1. CombatBaseValues
    [SerializeField] public CombatClass CombatBaseValue;

    // item level like 2, 3...
    [SerializeField] public int Level = 1;

    //2. Skill add
    [SerializeField] public List<SkillsMasterClass> SkillsMasterList;

    //3. Ability add value
    [SerializeField] public List<AddAbilityClass> AddAbilityList;


    //4. Offense or defense magnification set
    [SerializeField] public List<MagnificationMasterClass> MagnificationMasterList;


    // be calculated by coefficient
    public CombatClass CalculatedCombatValue()
    {
        CombatClass _calculated = new CombatClass();
        if (CombatBaseValue != null)
        {
            _calculated = CombatBaseValue.Copy();
            _calculated.Pow(Level);

        }
        return _calculated;
    }


    public string OneLineDescription()
    {
        //1. CombatBase Value

        string _descrption = null;
        if (CombatBaseValue != null)
        {
            if (CombatBaseValue.ShieldMax != 0) { _descrption += "Shield +" + CalculatedCombatValue().ShieldMax + " "; }
            if (CombatBaseValue.HitPointMax != 0) { _descrption += "HP +" + CalculatedCombatValue().HitPointMax + " "; }
            if (CombatBaseValue.Attack != 0) { _descrption += "Attack +" + CalculatedCombatValue().Attack + " "; }
            if (CombatBaseValue.Accuracy != 0) { _descrption += "Accuracy +" + CalculatedCombatValue().Accuracy + " "; }
            if (CombatBaseValue.Mobility != 0) { _descrption += "Mobility +" + CalculatedCombatValue().Mobility + " "; }
            if (CombatBaseValue.Defense != 0) { _descrption += "Defense +" + CalculatedCombatValue().Defense + " "; }
        }
        //2. Skill content
        foreach (SkillsMasterClass skill in SkillsMasterList)
        {
            _descrption += "[Skill: " + skill.name + "] ";
        }

        //3. Ability
        foreach (AddAbilityClass addAbility in AddAbilityList)
        {
            _descrption += addAbility.Ability + " +" + addAbility.ValueOfAbility + " ";
        }

        return _descrption;

    }

    public string DetailDescription()
    {
        string _descrption = null;

        _descrption += itemName + "\n";

        //1. CombatBase Value
        if (CombatBaseValue != null)
        {
            if (CombatBaseValue.ShieldMax != 0) { _descrption += "(Shield +" + CalculatedCombatValue().ShieldMax + ")\n"; }
            if (CombatBaseValue.HitPointMax != 0) { _descrption += "(HP +" + CalculatedCombatValue().HitPointMax + ")\n"; }
            if (CombatBaseValue.Attack != 0) { _descrption += "(Attack +" + CalculatedCombatValue().Attack + ")\n"; }
            if (CombatBaseValue.Accuracy != 0) { _descrption += "(Accuracy +" + CalculatedCombatValue().Accuracy + ")\n"; }
            if (CombatBaseValue.Mobility != 0) { _descrption += "(Mobility +" + CalculatedCombatValue().Mobility + ")\n"; }
            if (CombatBaseValue.Defense != 0) { _descrption += "(Defense +" + CalculatedCombatValue().Defense + ")\n"; }
        }

        //2. Skill content
        foreach (SkillsMasterClass skill in SkillsMasterList)
        {
            _descrption += "[Skill acquisition] " + skill.name + "\n";
        }

        //3. Ability
        foreach (AddAbilityClass addAbility in AddAbilityList)
        {
            _descrption += addAbility.Ability + " +" + addAbility.ValueOfAbility + "\n";
        }

        //4. offense or defense magnification
        foreach (MagnificationMasterClass magnification in MagnificationMasterList)
        {
            string _offenseOrDefense = null;
            string _magnificationDetail = null;

            switch (magnification.OffenseOrDefense)
            {
                case OffenseOrDefense.none:
                    _offenseOrDefense = null;
                    break;
                case OffenseOrDefense.Offense:
                    _offenseOrDefense = "Offense";
                    break;
                case OffenseOrDefense.Defense:
                    _offenseOrDefense = "Defense";
                    break;
                default:
                    Debug.LogError("unexpected OffenseOrDefense value:" + magnification.OffenseOrDefense);
                    break;
            }

            switch (magnification.MagnificationType)
            {
                case MagnificationType.MagnificationRatio:
                    _magnificationDetail += "x" + magnification.MagnificationRatio.ToString("F2");
                    break;
                case MagnificationType.AdditionalPercent:
                    _magnificationDetail += "+" + (int)magnification.MagnificationPercent + "%";
                    break;
                case MagnificationType.none:
                    break;
                default:
                    Debug.Log("ItemBaseMaster, set text of MagnificationMasterClass, unexpected MagnificationType :" + magnification.MagnificationType.ToString());
                    break;

            }

            _descrption += _offenseOrDefense + " " + magnification.MagnificationTarget + " [" + _magnificationDetail + "] \n";
        }


        return _descrption;
    }

}
