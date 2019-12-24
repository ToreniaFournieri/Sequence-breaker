using System;
using System.Collections.Generic;
using SequenceBreaker.Environment;
using SequenceBreaker.Master.BattleUnit;
using UnityEngine;

namespace SequenceBreaker.Master.Items
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
        [SerializeField] public int enhancedValue;

        // Item amount
        [SerializeField] public int amount;

        //Copy
        public Item Copy()
        {
            Item other = (Item)MemberwiseClone();
            return other;
        }

        public int CompareTo(Item item)
        {
            //1.item base id sort
            if (this.baseItem.itemId - item.baseItem.itemId != 0)
            {
                return this.baseItem.itemId - item.baseItem.itemId;
            }

            //2.same base Item id, prefix id sort
            if (this.prefixItem && item.prefixItem &&this.prefixItem.itemId - item.prefixItem.itemId != 0)
            {
                return this.prefixItem.itemId - item.prefixItem.itemId;
            }

            //3.same base and prefix id, suffix id sort
            if (this.suffixItem && item.suffixItem && this.suffixItem.itemId - item.suffixItem.itemId != 0)
            {
                return this.suffixItem.itemId - item.suffixItem.itemId;
            }

            //4.all same id, enhanced value sort
            if (this.enhancedValue - item.enhancedValue != 0)
            {
                return this.enhancedValue - item.enhancedValue;
            }


            //Unexpected value;
            Debug.LogError("unexpected Item.CompareTo: " + this.ItemDescription + " and " + item.ItemDescription);

            return 0;

        }





        //Calculated
        public CombatClass TotaledCombat(bool isConsiderAmount)
        {
            CombatClass combat = ScriptableObject.CreateInstance<CombatClass>();

            CombatClass itemBaseCombat = ScriptableObject.CreateInstance<CombatClass>();
            if (baseItem != null) { itemBaseCombat = baseItem.CalculatedCombatValue().Copy(); }

            CombatClass prefixCombat = ScriptableObject.CreateInstance<CombatClass>();
            if (prefixItem != null) { prefixCombat = prefixItem.CalculatedCombatValue().Copy(); }

            CombatClass suffixCombat = ScriptableObject.CreateInstance<CombatClass>();
            if (suffixItem != null) { suffixCombat = suffixItem.CalculatedCombatValue().Copy(); }

            if (isConsiderAmount)
            {
                for (int i = 0; i < amount; i++)
                {
                    combat.Add(itemBaseCombat);
                    combat.Add(prefixCombat);
                    combat.Add(suffixCombat);
                }
            }
            else
            {
                combat.Add(itemBaseCombat);
                combat.Add(prefixCombat);
                combat.Add(suffixCombat);
            }

            combat.Pow(1 + enhancedValue);

            return combat;
        }

        //Calculated
        public AbilityClass TotaledAbility()
        {
            AbilityClass ability = ScriptableObject.CreateInstance<AbilityClass>();

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
            string description = null;
            CombatClass totaledCombat = TotaledCombat(false);

            if (totaledCombat.shieldMax != 0) { description += "Shield +" + totaledCombat.shieldMax + "\n"; }
            if (totaledCombat.hitPointMax != 0) { description += "HP +" + totaledCombat.hitPointMax + "\n"; }
            if (totaledCombat.attack != 0) { description += "Attack +" + totaledCombat.attack + "\n"; }
            if (totaledCombat.accuracy != 0) { description += "Accuracy +" + totaledCombat.accuracy + "\n"; }
            if (totaledCombat.mobility != 0) { description += "Mobility +" + totaledCombat.mobility + "\n"; }
            if (totaledCombat.defense != 0) { description += "Defense +" + totaledCombat.defense + "\n"; }


            return description;
        }

        // ID but string type (this is temp)
        public string GetId()
        {
            string prefixId = null;
            if (prefixItem != null) { prefixId = "" + prefixItem.itemId; }
            string baseId = null;
            if (baseItem != null) { baseId = "" + baseItem.itemId; }
            string suffixId = null;
            if (suffixItem != null) { suffixId = "" + suffixItem.itemId; }


            return "" + enhancedValue + prefixId + baseId + suffixId;
        }

        private string GetName()
        {
            var amountString = "x" + amount + " ";


            string coefficient = null;
            if (enhancedValue >= 1)
            {
                coefficient = "+" + enhancedValue + " ";
            }

            string prefix = null;
            if (prefixItem != null) { prefix = prefixItem.itemName + " "; }

            string itemBase = null;
            if (baseItem != null) { itemBase = baseItem.itemName + " "; }

            string suffix = null;
            if (suffixItem != null) { suffix = "of " + suffixItem.itemName; }

            return coefficient + prefix + itemBase + suffix + amountString;
        }

        private string GetItemOneLineDescription()
        {
            string description = null;
            //if (baseItem != null) { description += baseItem.OneLineDescription(); }
            //if (prefixItem != null) { description += prefixItem.OneLineDescription(); }
            //if (suffixItem != null) { description += suffixItem.OneLineDescription(); }
            CombatClass totaledCombat = TotaledCombat(false);

            if (totaledCombat.shieldMax != 0) { description += "Shield +" + totaledCombat.shieldMax + " "; }
            if (totaledCombat.hitPointMax != 0) { description += "HP +" + totaledCombat.hitPointMax + " "; }
            if (totaledCombat.attack != 0) { description += "Attack +" + totaledCombat.attack + " "; }
            if (totaledCombat.accuracy != 0) { description += "Accuracy +" + totaledCombat.accuracy + " "; }
            if (totaledCombat.mobility != 0) { description += "Mobility +" + totaledCombat.mobility + " "; }
            if (totaledCombat.defense != 0) { description += "Defense +" + totaledCombat.defense + " "; }
            if (totaledCombat.numberOfAttacks != 0)
            {
                description += "Number of Attacks +" + totaledCombat.numberOfAttacks + " ";
            }


            if (baseItem != null) { description += baseItem.SkillAndAbilityDescription(); }
            if (prefixItem != null) { description += prefixItem.SkillAndAbilityDescription(); }
            if (suffixItem != null) { description += suffixItem.SkillAndAbilityDescription(); }


            return description;
        }

        public string GetItemDetailDescription()
        {
            string boldName = "<b>" + GetName() + "</b> \n";

            // Totaled combat status
            string totaledCombat = TotaledCombatDescription();


            //description detail
            string description = null;
            if (baseItem != null) { description += "<b>Item: </b>" + baseItem.DetailDescription() + "\n"; }
            if (prefixItem != null) { description += "<b>Prefix: </b> " + prefixItem.DetailDescription() + "\n"; }
            if (suffixItem != null) { description += "<b>Suffix: </b> " + suffixItem.DetailDescription() + "\n"; }


            var detailDescription = boldName + "\n" + totaledCombat + "\n" + description;


            return detailDescription;
        }

    }
}
