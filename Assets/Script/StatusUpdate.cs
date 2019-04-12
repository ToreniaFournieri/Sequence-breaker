using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class StatusUpdate : MonoBehaviour
{
    public BattleUnit battleUnit;
    //public Text AbilityText { get; set; }
    public Text AbilityText;
    public RefreshController refreshController;
    public InventoryScrollList content;
    private Text[] NewText;
    private List<Text> TextList = new List<Text>();
    private List<Item> items = new List<Item>();


    private void Awake()
    {
        NewText = GetComponentsInChildren<Text>();

        for (int i = 0; i < NewText.Length; i++)
        {
            TextList.Add(NewText[i]);
        }

        //AbilityText = TextList.Find((Text obj) => obj.name == "AbilityText");

        Refresh();

    }

    // Update is called once per frame
    void Update()
    {
        if (refreshController) { Refresh(); }
    }


    void Refresh()
    {
        if (content != null)
        {
            items = content.itemList;
        }
        CalculateAbility();
        AbilityTextUpdate();
        refreshController.NeedToRefresh = false;
    }

    void CalculateAbility()
    {
        battleUnit.Ability = battleUnit.InitialAbility;

        if (items.Count > 0)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];

                switch (item.ability)
                {
                    case Ability.power:
                        battleUnit.Ability.Power += item.addAbility;
                        break;
                    case Ability.stability:
                        battleUnit.Ability.Stability += item.addAbility;
                        break;
                    default:
                        break;
                }


            }
        }
    }


    void AbilityTextUpdate()
    {
        AbilityText.text = "Mew Power " + battleUnit.Ability.Power + "\nGeneration " + battleUnit.Ability.Generation + "\nStability "
            + battleUnit.Ability.Stability + "\nResponsiveness " + battleUnit.Ability.Responsiveness + "\nPrecision "
            + battleUnit.Ability.Precision + "\nIntelligence " + battleUnit.Ability.Intelligence + "\nLuck " + battleUnit.Ability.Luck;

    }


}