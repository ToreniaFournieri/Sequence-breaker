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

    // Coefficient is like x1.05
    [SerializeField] public double CombatMagnificationCoefficient = 1.0f;

    //2. Skill add
    [SerializeField] public List<SkillsMasterClass> SkillsMasterList;

    //3. Ability add value
    [SerializeField] public List<AddAbilityClass> AddAbilityList;


    //4. Offense or defense magnification set
    [SerializeField] public List<MagnificationMasterClass> MagnificationMasterList;

    public string DetailDescription()
    {
        string _detailDescrption = null;

        //1. CombatBase Value
        _detailDescrption += itemName + "\n";
        if (CombatBaseValue.ShiledMax != 0) { _detailDescrption += "Shiled +" + CombatBaseValue.ShiledMax + "\n"; }
        if (CombatBaseValue.HitPointMax != 0) { _detailDescrption += "HP +" + CombatBaseValue.HitPointMax + "\n"; }
        if (CombatBaseValue.Attack != 0) { _detailDescrption += "Attack +" + CombatBaseValue.Attack + "\n"; }
        if (CombatBaseValue.Accuracy != 0) { _detailDescrption += "Accuracy +" + CombatBaseValue.Accuracy + "\n"; }
        if (CombatBaseValue.Mobility != 0) { _detailDescrption += "Mobility +" + CombatBaseValue.Mobility + "\n"; }
        if (CombatBaseValue.Defense != 0) { _detailDescrption += "Defense +" + CombatBaseValue.Defense + "\n"; }

        //2. Skill content
        foreach (SkillsMasterClass skill in SkillsMasterList)
        {
            _detailDescrption += "[Skill acuisition] " + skill.name + "\n";
        }

        //3. Ability
        foreach (AddAbilityClass addAbility in AddAbilityList)
        {
            _detailDescrption += addAbility.Ability + " +" + addAbility.ValueOfAbility + "\n";
        }

        //4. offense or defense magnification
        foreach (MagnificationMasterClass magnification in MagnificationMasterList)
        {
            string _offenseOrDefense = null;
            string _magnificationDetail = null;

            switch (magnification.MagnificationType)
            {
                case MagnificationType.OffenseMagnificationRatio:
                    _offenseOrDefense += "Offense ";
                    _magnificationDetail += "x" + magnification.MagnificationRatio.ToString("F2");
                    break;
                case MagnificationType.OffenseMagnificationFixedRatio:
                    _offenseOrDefense += "Offense ";
                    switch (magnification.MagnificationFixedRatio)
                    {
                        case MagnificationFixedRatio.fiveOverFour:
                            _magnificationDetail += "5/4";
                            break;
                        case MagnificationFixedRatio.fiveOverSix:
                            _magnificationDetail += "5/6";
                            break;
                        case MagnificationFixedRatio.fourOverFive:
                            _magnificationDetail += "4/5";
                            break;
                        case MagnificationFixedRatio.fourOverThree:
                            _magnificationDetail += "4/3";
                            break;
                        case MagnificationFixedRatio.oneOverHundred:
                            _magnificationDetail += "1/100";
                            break;
                        case MagnificationFixedRatio.oneOverOne:
                            _magnificationDetail += "1/1";
                            break;
                        case MagnificationFixedRatio.oneOverTen:
                            _magnificationDetail += "1/10";
                            break;
                        case MagnificationFixedRatio.sixOverFive:
                            _magnificationDetail += "6/5";
                            break;
                        case MagnificationFixedRatio.threeOverFour:
                            _magnificationDetail += "3/4";
                            break;
                        case MagnificationFixedRatio.threeOverTwo:
                            _magnificationDetail += "3/2";
                            break;
                        case MagnificationFixedRatio.twoOverOne:
                            _magnificationDetail += "2/1";
                            break;
                        case MagnificationFixedRatio.twoOverThree:
                            _magnificationDetail += "2/3";
                            break;
                        default:
                            Debug.Log("ItemBaseMaster, in MagnificationFixedRatio, unexpected :" + magnification.MagnificationFixedRatio);
                            break;
                    }
                    break;
                case MagnificationType.OffenseAdditionalPercent:
                    _offenseOrDefense += "Offense ";
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
                case MagnificationType.DefenseMagnificationRatio:
                    _offenseOrDefense += "Defense ";

                    _magnificationDetail += "x" + magnification.MagnificationRatio.ToString("F2");
                    break;
                case MagnificationType.DefenseMagnificationFixedRatio:
                    _offenseOrDefense += "Defense ";
                    switch (magnification.MagnificationFixedRatio)
                    {
                        case MagnificationFixedRatio.fiveOverFour:
                            _magnificationDetail += "5/4";
                            break;
                        case MagnificationFixedRatio.fiveOverSix:
                            _magnificationDetail += "5/6";
                            break;
                        case MagnificationFixedRatio.fourOverFive:
                            _magnificationDetail += "4/5";
                            break;
                        case MagnificationFixedRatio.fourOverThree:
                            _magnificationDetail += "4/3";
                            break;
                        case MagnificationFixedRatio.oneOverHundred:
                            _magnificationDetail += "1/100";
                            break;
                        case MagnificationFixedRatio.oneOverOne:
                            _magnificationDetail += "1/1";
                            break;
                        case MagnificationFixedRatio.oneOverTen:
                            _magnificationDetail += "1/10";
                            break;
                        case MagnificationFixedRatio.sixOverFive:
                            _magnificationDetail += "6/5";
                            break;
                        case MagnificationFixedRatio.threeOverFour:
                            _magnificationDetail += "3/4";
                            break;
                        case MagnificationFixedRatio.threeOverTwo:
                            _magnificationDetail += "3/2";
                            break;
                        case MagnificationFixedRatio.twoOverOne:
                            _magnificationDetail += "2/1";
                            break;
                        case MagnificationFixedRatio.twoOverThree:
                            _magnificationDetail += "2/3";
                            break;
                        default:
                            Debug.Log("ItemBaseMaster, in MagnificationFixedRatio, unexpected :" + magnification.MagnificationFixedRatio);
                            break;
                    }
                    break;
                case MagnificationType.DefenseAdditionalPercent:
                    _offenseOrDefense += "Defense ";
                    switch (magnification.MagnificationPercent)
                    {
                        case MagnificationPercent.one: _magnificationDetail += "-1%"; break;
                        case MagnificationPercent.two: _magnificationDetail += "-2%"; break;
                        case MagnificationPercent.three: _magnificationDetail += "-3%"; break;
                        case MagnificationPercent.four: _magnificationDetail += "-4%"; break;
                        case MagnificationPercent.five: _magnificationDetail += "-5%"; break;
                        case MagnificationPercent.six: _magnificationDetail += "-6%"; break;
                        case MagnificationPercent.seven: _magnificationDetail += "-7%"; break;
                        case MagnificationPercent.eight: _magnificationDetail += "-8%"; break;
                        case MagnificationPercent.nine: _magnificationDetail += "-9%"; break;
                        case MagnificationPercent.ten: _magnificationDetail += "-10%"; break;
                        case MagnificationPercent.eleven: _magnificationDetail += "-11%"; break;
                        case MagnificationPercent.twelve: _magnificationDetail += "-12%"; break;
                        case MagnificationPercent.thirteen: _magnificationDetail += "-13%"; break;
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

            _detailDescrption += _offenseOrDefense + " " + magnification.MagnificationTarget + " [" + _magnificationDetail + "] \n";
        }


        return _detailDescrption;
    }

}
