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

        CombatClass _calculated = CombatBaseValue.Copy();

        _calculated.Pow(Level);

        //_calculated.ShiledCurrent = (int)(CombatBaseValue.ShiledCurrent * Math.Pow(1.2, CombatMagnificationCoefficient));
        //_calculated.ShiledMax = (int)(CombatBaseValue.ShiledMax * Math.Pow(1.2, CombatMagnificationCoefficient));
        //_calculated.HitPointCurrent = (int)(CombatBaseValue.HitPointCurrent * Math.Pow(1.2, CombatMagnificationCoefficient));
        //_calculated.HitPointMax = (int)(CombatBaseValue.HitPointMax * Math.Pow(1.2, CombatMagnificationCoefficient));
        //_calculated.Attack = (int)(CombatBaseValue.Attack * Math.Pow(1.2, CombatMagnificationCoefficient));
        //_calculated.KineticAttackRatio = (int)(CombatBaseValue.KineticAttackRatio * Math.Pow(1.2, CombatMagnificationCoefficient));
        //_calculated.ChemicalAttackRatio = (int)(CombatBaseValue.ChemicalAttackRatio * Math.Pow(1.2, CombatMagnificationCoefficient));
        //_calculated.ThermalAttackRatio = (int)(CombatBaseValue.ThermalAttackRatio * Math.Pow(1.2, CombatMagnificationCoefficient));
        //_calculated.CriticalHit = (int)(CombatBaseValue.ShiledMax * Math.Pow(1.2, CombatMagnificationCoefficient));
        //_calculated.NumberOfAttacks = (int)(CombatBaseValue.ShiledMax * Math.Pow(1.2, CombatMagnificationCoefficient));

        ////Note: MinRange and MaxRange is not be add up.
        ////combatAdded.MinRange;
        ////combatAdded.MaxRange;
        //_calculated.Accuracy = (int)(CombatBaseValue.Accuracy * Math.Pow(1.2, Level));
        //_calculated.Mobility = (int)(CombatBaseValue.Mobility * Math.Pow(1.2, Level));
        //_calculated.Defense = (int)(CombatBaseValue.Defense * Math.Pow(1.2, Level));
        //_calculated.Counterintelligence = (int)(CombatBaseValue.Counterintelligence * Math.Pow(1.2, Level));
        //_calculated.Repair = (int)(CombatBaseValue.Repair * Math.Pow(1.2, Level));

        return _calculated;
    }



    public string DetailDescription()
    {
        string _descrption = null;

        _descrption += itemName + "\n";

        //1. CombatBase Value
        if (CombatBaseValue.ShieldMax != 0) { _descrption += "(Shield +" + CalculatedCombatValue().ShieldMax + ")\n"; }
        if (CombatBaseValue.HitPointMax != 0) { _descrption += "(HP +" + CalculatedCombatValue().HitPointMax + ")\n"; }
        if (CombatBaseValue.Attack != 0) { _descrption += "(Attack +" + CalculatedCombatValue().Attack + ")\n"; }
        if (CombatBaseValue.Accuracy != 0) { _descrption += "(Accuracy +" + CalculatedCombatValue().Accuracy + ")\n"; }
        if (CombatBaseValue.Mobility != 0) { _descrption += "(Mobility +" + CalculatedCombatValue().Mobility + ")\n"; }
        if (CombatBaseValue.Defense != 0) { _descrption += "(Defense +" + CalculatedCombatValue().Defense + ")\n"; }


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
                case MagnificationType.MagnificationFixedRatio:
                    switch (magnification.MagnificationFixedRatio)
                    {
                        case MagnificationFixedRatio.fiveOverFour: _magnificationDetail += "5/4"; break;
                        case MagnificationFixedRatio.fiveOverSix: _magnificationDetail += "5/6"; break;
                        case MagnificationFixedRatio.fourOverFive: _magnificationDetail += "4/5"; break;
                        case MagnificationFixedRatio.fourOverThree: _magnificationDetail += "4/3"; break;
                        case MagnificationFixedRatio.oneOverHundred: _magnificationDetail += "1/100"; break;
                        case MagnificationFixedRatio.oneOverOne: _magnificationDetail += "1/1"; break;
                        case MagnificationFixedRatio.oneOverTen: _magnificationDetail += "1/10"; break;
                        case MagnificationFixedRatio.sixOverFive: _magnificationDetail += "6/5"; break;
                        case MagnificationFixedRatio.threeOverFour: _magnificationDetail += "3/4"; break;
                        case MagnificationFixedRatio.threeOverTwo: _magnificationDetail += "3/2"; break;
                        case MagnificationFixedRatio.twoOverOne: _magnificationDetail += "2/1"; break;
                        case MagnificationFixedRatio.twoOverThree: _magnificationDetail += "2/3"; break;
                        default:
                            Debug.Log("ItemBaseMaster, in MagnificationFixedRatio, unexpected :" + magnification.MagnificationFixedRatio);
                            break;
                    }
                    break;
                case MagnificationType.AdditionalPercent:
                    switch (magnification.MagnificationPercent)
                    {
                        case MagnificationPercent.one: _magnificationDetail += "+1%"; break;
                        case MagnificationPercent.two: _magnificationDetail += "+2%"; break;
                        case MagnificationPercent.three: _magnificationDetail += "+3%"; break;
                        case MagnificationPercent.four: _magnificationDetail += "+4%"; break;
                        case MagnificationPercent.five: _magnificationDetail += "+5%"; break;
                        case MagnificationPercent.six: _magnificationDetail += "+6%"; break;
                        case MagnificationPercent.seven: _magnificationDetail += "+7%"; break;
                        case MagnificationPercent.eight: _magnificationDetail += "+8%"; break;
                        case MagnificationPercent.nine: _magnificationDetail += "+9%"; break;
                        case MagnificationPercent.ten: _magnificationDetail += "+10%"; break;
                        case MagnificationPercent.eleven: _magnificationDetail += "+11%"; break;
                        case MagnificationPercent.twelve: _magnificationDetail += "+12%"; break;
                        case MagnificationPercent.thirteen: _magnificationDetail += "+13%"; break;
                        default:
                            Debug.Log("ItemBaseMaster, in MagnificationPercent, unexpected: " + magnification.MagnificationPercent);
                            break;
                    }
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
