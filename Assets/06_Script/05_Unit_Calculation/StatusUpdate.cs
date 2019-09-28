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
    //private List<Item> items = new List<Item>(); // this may not needed

    private CalculateUnitStatus calculateUnitStatus;

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
            //items = content.itemList;
            //Debug.Log("in inventory, contents is exist: " + items.Count);
            unit.itemList = content.itemList;
        }
        UnitNameUpdate();
        CalculateUnitStatus();
        AbilityTextUpdate();
        CapacityTextUpdate();
        refreshController.NeedToRefresh = false;
    }

    void CalculateUnitStatus()
    {
        if (unit != null)
        {
            calculateUnitStatus = new CalculateUnitStatus(unit);
            unit.Ability = calculateUnitStatus.BattleUnit.Ability;

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
        // 2019.9.23 MagnificationTarget
        // 0:none, 1:Critical, 2:Kinetic, 3:Chemical, 4:Thermal, 5:VsBeast, 6:VsCyborg, 7:VsDrone, 8:VsRobot, 9:VsTitan, 10:OptimumRangeBonus




        abilityText.text =
            calculateUnitStatus.BattleUnit.Combat.ShieldMax + " Shield \n"
            + calculateUnitStatus.BattleUnit.Combat.HitPointMax + " HP\n"
            + calculateUnitStatus.BattleUnit.Combat.Attack + " Attack \n"
            + calculateUnitStatus.BattleUnit.Combat.Accuracy + " Accuracy \n"
            + calculateUnitStatus.BattleUnit.Combat.Mobility + " Mobility \n"
            + calculateUnitStatus.BattleUnit.Combat.Defense + " Defense\n"

            + "(P:" + unit.Ability.Power + " G:" + unit.Ability.Generation
            + " S:" + unit.Ability.Stability + " R:" + unit.Ability.Responsiveness
            + " P:" + unit.Ability.Precision + " I:" + unit.Ability.Intelligence
            + " L:" + unit.Ability.Luck + ")\n"
            + " \n"

            + "<Offense> \n"
            + "[Critical: x" + Math.Round(calculateUnitStatus.BattleUnit.OffenseMagnification.Critical * 100) / 100 + "] "
            + " ("
            + " x" + Math.Round(calculateUnitStatus.summedOffenseList[1].ratioValue, 3)
            + " & {" + calculateUnitStatus.summedOffenseList[1].percents + "}) \n"
            + "[Kinetic: x" + Math.Round(calculateUnitStatus.BattleUnit.OffenseMagnification.Kinetic * 100) / 100 + "] "
            + " ("
            + " x" + Math.Round(calculateUnitStatus.summedOffenseList[2].ratioValue, 3)
            + " & {" + calculateUnitStatus.summedOffenseList[2].percents + "}) \n"

            + "[Chemical: x" + Math.Round(calculateUnitStatus.BattleUnit.OffenseMagnification.Chemical * 100) / 100 + "] "
            + " ("
            + " x" + Math.Round(calculateUnitStatus.summedOffenseList[3].ratioValue, 3)
            + " & {" + calculateUnitStatus.summedOffenseList[3].percents + "}) \n"

            + "[Thermal: x" + Math.Round(calculateUnitStatus.BattleUnit.OffenseMagnification.Thermal * 100) / 100 + "] "
            + " ("
            + " x" + Math.Round(calculateUnitStatus.summedOffenseList[4].ratioValue, 3)
            + " & {" + calculateUnitStatus.summedOffenseList[4].percents + "}) \n"

            + "[OptimumRangeBonus: x" + Math.Round(calculateUnitStatus.BattleUnit.OffenseMagnification.OptimumRangeBonus * 100) / 100 + "] "
                        + " ("
            + " x" + Math.Round(calculateUnitStatus.summedOffenseList[10].ratioValue, 3)
            + " & {" + calculateUnitStatus.summedOffenseList[10].percents + "}) \n"

            + " \n"

            + "<Deffense> \n"
            + "[Critical: x" + Math.Round(calculateUnitStatus.BattleUnit.DefenseMagnification.Critical * 100) / 100 + "] "
            + " ("
            + " x" + Math.Round(calculateUnitStatus.summedDefenseList[1].ratioValue, 3)
            + " & {" + calculateUnitStatus.summedDefenseList[1].percents + "}) \n"

            + "[Kinetic: x" + Math.Round(calculateUnitStatus.BattleUnit.DefenseMagnification.Kinetic * 100) / 100 + "] "
            + " ("
            + " x" + Math.Round(calculateUnitStatus.summedDefenseList[2].ratioValue, 3)
            + " & {" + calculateUnitStatus.summedDefenseList[2].percents + "}) \n"

            + "[Chemical: x" + Math.Round(calculateUnitStatus.BattleUnit.DefenseMagnification.Chemical * 100) / 100 + "] "
                        + " ("
            + " x" + Math.Round(calculateUnitStatus.summedDefenseList[3].ratioValue, 3)
            + " & {" + calculateUnitStatus.summedDefenseList[3].percents + "}) \n"

            + "[Thermal: x" + Math.Round(calculateUnitStatus.BattleUnit.DefenseMagnification.Thermal * 100) / 100 + "]"
                        + " ("
            + " x" + Math.Round(calculateUnitStatus.summedDefenseList[4].ratioValue, 3)
            + " & {" + calculateUnitStatus.summedDefenseList[4].percents + "}) \n"


            ;




    }


}
