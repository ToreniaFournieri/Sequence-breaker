using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatusDisplay : MonoBehaviour
{

    public ItemDataBase itemDataBase;
    public List<UnitClass> UnitList;

    public CharacterTreeViewDataSourceMgr characterTreeViewDataSourceMgr;
    public CharacterTreeViewWithStickyHeadScript characterTreeViewWithStickyHeadScript;



    //Don't change these following values.
    public Text characterNameText;
    public Text itemAmountText;
    public Text AbilityText;
    public List<Item> itemList;
    private int selectedUnitNo;


    public void Init()
    {
        SetCharacterStatus(0);
    }


    public void SetCharacterStatus(int _selectedUnitNo)
    {

        if (UnitList != null && _selectedUnitNo < UnitList.Count)
        {
            // updating
            selectedUnitNo = _selectedUnitNo;

            characterNameText.text = UnitList[selectedUnitNo].Name + " (Level:" + UnitList[selectedUnitNo].Level + ")";
            itemAmountText.text = UnitList[selectedUnitNo].itemList.Count + "/" + UnitList[selectedUnitNo].ItemCapacity;

            CalculateUnitStatus _calculateUnitStatus = new CalculateUnitStatus(UnitList[selectedUnitNo]);
            AbilityText.text = _calculateUnitStatus.detailAbilityString;

            // load from saved data
            itemList = itemDataBase.LoadItemList("item-" + UnitList[selectedUnitNo].Affiliation + "-" + UnitList[selectedUnitNo].UniqueID);

            characterTreeViewDataSourceMgr.Show();
            characterTreeViewWithStickyHeadScript.Initialization();

        }

    }


}
