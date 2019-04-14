using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class StatusUpdate : MonoBehaviour
{
    public UnitClass unit;
    //public Text AbilityText { get; set; }
    public Text unitNameText;
    public Text abilityText;
    public Text itemAmountText;
    public RefreshController refreshController;
    public InventoryScrollList content;
    private Text[] newText;
    private List<Text> textList = new List<Text>();
    private List<Item> items = new List<Item>();


    private void Awake()
    {
        newText = GetComponentsInChildren<Text>();

        for (int i = 0; i < newText.Length; i++)
        {
            textList.Add(newText[i]);
        }

        Refresh();

    }

    // Update is called once per frame
    void Update()
    {
        if (refreshController.NeedToRefresh) { Refresh(); }
    }


    public void Refresh()
    {
        if (content != null)
        {
            items = content.itemList;
        }
        UnitNameUpdate();
        CalculateAbility();
        AbilityTextUpdate();
        CapacityTextUpdate();
        refreshController.NeedToRefresh = false;
    }

    void CalculateAbility()
    {
        if (unit != null)
        {
            unit.Ability = unit.InitialAbility.DeepCopy();
            if (items.Count > 0)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    Item item = items[i];
                    switch (item.ability)
                    {
                        case Ability.power:
                            unit.Ability.Power += item.addAbility;
                            break;
                        case Ability.stability:
                            unit.Ability.Stability += item.addAbility;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

    }

    void UnitNameUpdate()
    {
        unitNameText.text = unit.Name;
    }

    void CapacityTextUpdate()
    {
        itemAmountText.text = unit.itemList.Count + " / " + unit.ItemCapacity;
    }

    void AbilityTextUpdate()
    {
        abilityText.text = unit.Ability.Power + " Mew Power\n" 
            + unit.Ability.Generation+ " Generation\n"
            + unit.Ability.Stability + " Stability\n"
            + unit.Ability.Responsiveness + " Responsiveness\n"  
            + unit.Ability.Precision + " Precision\n"
            + unit.Ability.Intelligence + " Intelligence\n" 
            + unit.Ability.Luck + "Luck\n";

    }


}