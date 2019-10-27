using System;
using System.Collections.Generic;
using SequenceBreaker._00_System;
using SequenceBreaker._01_Data._02_Items._01_ItemMaster;
using SequenceBreaker._01_Data._08_BattleUnitSub;
using UnityEngine;

namespace SequenceBreaker._01_Data._02_Items.Item
{
    [Serializable]
    [CreateAssetMenu(fileName = "Item-", menuName = "Item/Item", order = 10)]
    public sealed class Item : ScriptableObject
    {

        public string ItemName => GetName();

        public string ItemDescription => GetItemOneLineDescription();

        // base of this item
        [SerializeField] public ItemBaseMaster baseItem;

        // prefix
        [SerializeField] public ItemBaseMaster prefixItem;

        // suffix, super-rare
        [SerializeField] public ItemBaseMaster suffixItem;

        // Item enhanced
        [SerializeField]  public int enhancedValue;
        
        // Item amount
        [SerializeField] public int amount;

        //Copy
        public Item Copy()
        {
            Item other = (Item)this.MemberwiseClone();
            return other;
        }


        //Calculated
        public CombatClass TotaledCombat()
        {
            CombatClass combat = new CombatClass();

            CombatClass itemBaseCombat = new CombatClass();
            if (baseItem != null) { itemBaseCombat = baseItem.CalculatedCombatValue().Copy(); }

            CombatClass prefixCombat = new CombatClass();
            if (prefixItem != null) { prefixCombat = prefixItem.CalculatedCombatValue().Copy(); }

            CombatClass suffixCombat = new CombatClass();
            if (suffixItem != null) { suffixCombat = suffixItem.CalculatedCombatValue().Copy(); }

            combat.Add(itemBaseCombat);
            combat.Add(prefixCombat);
            combat.Add(suffixCombat);

            combat.Pow(enhancedValue);

            return combat;
        }

        //Calculated
        public AbilityClass TotaledAbility()
        {
            AbilityClass ability = new AbilityClass(0, 0, 0, 0, 0, 0, 0);

            List<ItemBaseMaster> itemBasesList = new List<ItemBaseMaster>();

            if (baseItem != null) { itemBasesList.Add(baseItem); }
            if (prefixItem != null) { itemBasesList.Add(prefixItem); }
            if (suffixItem != null) { itemBasesList.Add(suffixItem); }

            foreach (ItemBaseMaster itemBase in itemBasesList)
            {
                //Set itemBase, prefix, suffix ability
                foreach (AddAbilityClass addAbility in itemBase.addAbilityList)
                {


                    switch (addAbility.ability)
                    {
                        case Ability.Generation:
                            ability.generation += addAbility.valueOfAbility;
                            break;
                        case Ability.Intelligence:
                            ability.intelligence += addAbility.valueOfAbility;
                            break;
                        case Ability.Luck:
                            ability.luck += addAbility.valueOfAbility;
                            break;
                        case Ability.None:
                            break;
                        case Ability.Power:
                            ability.power += addAbility.valueOfAbility;
                            break;
                        case Ability.Precision:
                            ability.precision += addAbility.valueOfAbility;
                            break;
                        case Ability.Responsiveness:
                            ability.precision += addAbility.valueOfAbility;
                            break;
                        case Ability.Stability:
                            ability.stability += addAbility.valueOfAbility;
                            break;
                        default:
                            Debug.LogError("unexpected Ability: " + addAbility.ability);
                            break;


                    }
                }
            }

            return ability;
        }

        public string TotaledCombatDescription()
        {
            string descrption = null;
            CombatClass totaledCombat = this.TotaledCombat();

            if (totaledCombat.shieldMax != 0) { descrption += "Shield +" + totaledCombat.shieldMax + "\n"; }
            if (totaledCombat.hitPointMax != 0) { descrption += "HP +" + totaledCombat.hitPointMax + "\n"; }
            if (totaledCombat.attack != 0) { descrption += "Attack +" + totaledCombat.attack + "\n"; }
            if (totaledCombat.accuracy != 0) { descrption += "Accuracy +" + totaledCombat.accuracy + "\n"; }
            if (totaledCombat.mobility != 0) { descrption += "Mobility +" + totaledCombat.mobility + "\n"; }
            if (totaledCombat.defense != 0) { descrption += "Defense +" + totaledCombat.defense + "\n"; }


            return descrption;
        }

        private string GetName()
        {

            string amountString = null;
//            if (amount >= 2)
//            {
                amountString = "x" + amount + " ";
//            }

            string coefficient = null;
            if (enhancedValue > 1)
            {
                coefficient = "+" + enhancedValue +" ";
            }

            string prefix = null;
            if (prefixItem != null) { prefix = prefixItem.itemName + " "; }

            string itemBase = null;
            if (baseItem != null) { itemBase = baseItem.itemName + " "; }

            string suffix = null;
            if (suffixItem != null) { suffix = "of " + suffixItem.itemName; }

            return amountString + coefficient + prefix + itemBase + suffix;
        }

        private string GetItemOneLineDescription()
        {
            string description = null;
            if (baseItem != null) { description += baseItem.OneLineDescription(); }
            if (prefixItem != null) { description += prefixItem.OneLineDescription(); }
            if (suffixItem != null) { description += suffixItem.OneLineDescription(); }


            return description;
        }

        public string GetItemDetailDescription()
        {
            string detailDescription = null;

            string name = "<b>" + GetName() + "</b> \n";

            // Totaled combat status
            string totaledCombat = this.TotaledCombatDescription();


            //description detail
            string description = null;
            if (baseItem != null) { description += "<b>Item: </b>" + baseItem.DetailDescription() + "\n"; }
            if (prefixItem != null) { description += "<b>Prefix: </b> " + prefixItem.DetailDescription() + "\n"; }
            if (suffixItem != null) { description += "<b>Suffix: </b> " + suffixItem.DetailDescription() + "\n"; }


            detailDescription = name + "\n" + totaledCombat + "\n" + description;


            return detailDescription;
        }

    }
}
