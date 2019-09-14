using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item-", menuName = "Item/Item", order = 10)]
public class Item : ScriptableObject
{
    public string itemName;
    public string itemDescription;

    public Sprite icon;
    public Ability ability;
    public int addAbility;



    // base of this item
    public ItemBaseMaster baseItem;

    // prefix
    public ItemBaseMaster prefixItem;

    // suffix, super-rare
    public ItemBaseMaster suffixItem;

    // Item enhanced
    public int enhancedValue;

    //Calculated
    public CombatClass TotaledCombat()
    {
        CombatClass _combat = new CombatClass();

        CombatClass _itemBaseCombat = new CombatClass();
        if (baseItem != null) { _itemBaseCombat = baseItem.CalculatedCombatValue().Copy(); }

        CombatClass _prefixCombat = new CombatClass();
        if (prefixItem != null) { _prefixCombat = prefixItem.CalculatedCombatValue().Copy(); }

        CombatClass _suffixCombat = new CombatClass();
        if (suffixItem != null) { _suffixCombat = suffixItem.CalculatedCombatValue().Copy(); }

        _combat.Add(_itemBaseCombat);
        _combat.Add(_prefixCombat);
        _combat.Add(_suffixCombat);

        _combat.Pow(enhancedValue);

        return _combat;
    }

    //Calculated
    public AbilityClass TotaledAbility()
    {
        AbilityClass _ability = new AbilityClass(0, 0, 0, 0, 0, 0, 0);

        List<ItemBaseMaster> _itemBasesList = new List<ItemBaseMaster>();

        if (baseItem != null) { _itemBasesList.Add(baseItem); }
        if (prefixItem != null) { _itemBasesList.Add(prefixItem); }
        if (suffixItem != null) { _itemBasesList.Add(suffixItem); }

        foreach (ItemBaseMaster _itemBase in _itemBasesList)
        {
            //Set itemBase, prefix, suffix ability
            foreach (AddAbilityClass _addAbility in _itemBase.AddAbilityList)
            {


                switch (_addAbility.Ability)
                {
                    case Ability.generation:
                        _ability.Generation += _addAbility.ValueOfAbility;
                        break;
                    case Ability.intelligence:
                        _ability.Intelligence += _addAbility.ValueOfAbility;
                        break;
                    case Ability.luck:
                        _ability.Luck += _addAbility.ValueOfAbility;
                        break;
                    case Ability.none:
                        break;
                    case Ability.power:
                        _ability.Power += _addAbility.ValueOfAbility;
                        break;
                    case Ability.precision:
                        _ability.Precision += _addAbility.ValueOfAbility;
                        break;
                    case Ability.responsiveness:
                        _ability.Precision += _addAbility.ValueOfAbility;
                        break;
                    case Ability.stability:
                        _ability.Stability += _addAbility.ValueOfAbility;
                        break;
                    default:
                        Debug.LogError( "unexpected Ability: " + _addAbility.Ability );
                        break;


                }
            }
        }

        return _ability;
    }

    public string TotaledCombatDescription()
    {
        string _descrption = null;
        CombatClass _totaledCombat = this.TotaledCombat();

        if (_totaledCombat.ShieldMax != 0) { _descrption += "Shield +" + _totaledCombat.ShieldMax + "\n"; }
        if (_totaledCombat.HitPointMax != 0) { _descrption += "HP +" + _totaledCombat.HitPointMax + "\n"; }
        if (_totaledCombat.Attack != 0) { _descrption += "Attack +" + _totaledCombat.Attack + "\n"; }
        if (_totaledCombat.Accuracy != 0) { _descrption += "Accuracy +" + _totaledCombat.Accuracy + "\n"; }
        if (_totaledCombat.Mobility != 0) { _descrption += "Mobility +" + _totaledCombat.Mobility + "\n"; }
        if (_totaledCombat.Defense != 0) { _descrption += "Defense +" + _totaledCombat.Defense + "\n"; }


        return _descrption;
    }


    public string GetItemDetailDescription()
    {
        string _detailDescription = null;
        // Set Name
        string _nameOfTrueItem;

        string _coefficient = null;
        if (enhancedValue > 1)
        {
            _coefficient = "+" + enhancedValue;
        }

        string _prefix = null;
        if (prefixItem != null) { _prefix = prefixItem.itemName; }

        string _itemBase = null;
        if (baseItem != null) { _itemBase = baseItem.itemName; }

        string _suffix = null;
        if (suffixItem != null) { _suffix = "of " + suffixItem.itemName; }

        _nameOfTrueItem = "<b>" + _coefficient + " " + _prefix + " " + _itemBase + " " + _suffix + "</b> \n";

        // Totaled combat status
        string _totaledCombat = this.TotaledCombatDescription();


        //description detail
        string _description = null;
        if (baseItem != null) { _description += "<b>Item: </b>" + baseItem.DetailDescription() + "\n"; }
        if (prefixItem != null) { _description += "<b>Prefix: </b> " + prefixItem.DetailDescription() + "\n"; }
        if (suffixItem != null) { _description += "<b>Suffix: </b> " + suffixItem.DetailDescription() + "\n"; }


        _detailDescription = _nameOfTrueItem + "\n" + _totaledCombat + "\n" + _description;
        return _detailDescription;
    }

}
