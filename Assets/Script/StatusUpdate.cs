using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class StatusUpdate : MonoBehaviour
{
    public BattleUnit battleUnit;
    public Text AbilityText { get; set; }
    public RefreshController refreshController;
    private Text[] NewText;
    private List<Text> TextList = new List<Text>();

    private void Awake()
    {
        NewText = GetComponentsInChildren<Text>();

        for (int i = 0; i < NewText.Length; i++)
        {
            TextList.Add(NewText[i]);
        }

        AbilityText = TextList.Find((Text obj) => obj.name == "AbilityText");

        Refresh();

    }

    // Update is called once per frame
    void Update()
    {
        if (refreshController) { Refresh(); }
    }


    void Refresh()
    {
        AbilityTextUpdate();
        refreshController.NeedToRefresh = false;
    }

    void AbilityTextUpdate()
    {
        //AbilityText.text = "Mew Power " + battleUnit.Ability.Power + "\nGeneration " + battleUnit.Ability.Generation + "\nStability "
            //+ battleUnit.Ability.Stability + "\nResponsiveness " + battleUnit.Ability.Responsiveness + "\nPrecision "
            //+ battleUnit.Ability.Precision + "\nIntelligence " + battleUnit.Ability.Intelligence + "\nLuck " + battleUnit.Ability.Luck;

    }


}