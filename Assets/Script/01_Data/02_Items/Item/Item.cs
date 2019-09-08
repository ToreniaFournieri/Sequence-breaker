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
    public ItemBaseMaster itembaseMaster;

    // prefix
    public ItemBaseMaster prefixItem;

    // suffix, super-rare
    public ItemBaseMaster suffixItem;

    public string GetItemDetailDescription()
    {
        string _detailDescription = null;

        // Set Name
        string _nameOfTrueItem;

        string _coefficient = null;
        if (itembaseMaster.CombatMagnificationCoefficient > 1.0f)
        {
            _coefficient = "+" + (int)(itembaseMaster.CombatMagnificationCoefficient);
        }

        string _prefix = null;
        if (prefixItem != null) { _prefix = prefixItem.itemName; }

        string _itemBase = null;
        if (itembaseMaster != null) { _itemBase = itembaseMaster.itemName; }

        string _suffix = null;
        if (suffixItem != null) { _suffix = "of " +  suffixItem.itemName; }

        _nameOfTrueItem = "<b>" + _coefficient + " " + _prefix + " " + _itemBase + " " + _suffix + "</b> \n";

        //description detail
        string _description = null;
        if (itembaseMaster != null) { _description += itembaseMaster.DetailDescription(); }
        if (prefixItem != null) { _description += "\n <b>Prefix: </b> " + prefixItem.DetailDescription(); }
        if (suffixItem != null) { _description += "\n <b>Suffix: </b> " + suffixItem.DetailDescription(); }


        _detailDescription = _nameOfTrueItem + "\n" + _description;
        return _detailDescription;
    }

}
