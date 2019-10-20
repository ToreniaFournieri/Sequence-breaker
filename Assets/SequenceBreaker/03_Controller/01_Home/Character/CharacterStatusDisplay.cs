using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

sealed public class CharacterStatusDisplay : MonoBehaviour
{

    public ItemDataBase itemDataBase;
    [FormerlySerializedAs("UnitList")] public List<UnitClass> unitList;

    public CharacterTreeViewDataSourceMgr characterTreeViewDataSourceMgr;
    public CharacterTreeViewWithStickyHeadScript characterTreeViewWithStickyHeadScript;



    //Don't change these following values.
    public Text characterNameText;
    public Text itemAmountText;
    [FormerlySerializedAs("AbilityText")] public Text abilityText;
    //public List<Item> itemList;

    //for data save
    public Affiliation affiliation;
    [FormerlySerializedAs("uniqueID")] public int uniqueId;
    public int itemCapacity;
    public int selectedUnitNo = 0;
    

    public void Init()
    {
        //GetItemList();
        RefleshCharacterStatusAndItemList();

    }

    // reflesh character status display
    public void RefleshCharacterStatusAndItemList()
    {
        //Debug.Log("show character: " + UnitList[selectedUnitNo].Name);
        SetCharacterStatus(selectedUnitNo);
    }


    public void SetCharacterStatus(int selectedUnitNo)
    {
        //Debug.Log("in SetCharacterStatus: " + UnitList[_selectedUnitNo].Name);

        if (unitList != null && selectedUnitNo < unitList.Count)
        {

            // updating
            this.selectedUnitNo = selectedUnitNo;

            characterNameText.text = unitList[this.selectedUnitNo].name + " (Level:" + unitList[this.selectedUnitNo].level + ")";
            itemAmountText.text = unitList[this.selectedUnitNo].itemList.Count + "/" + unitList[this.selectedUnitNo].itemCapacity;

            CalculateUnitStatus calculateUnitStatus = new CalculateUnitStatus(unitList[this.selectedUnitNo]);
            abilityText.text = calculateUnitStatus.detailAbilityString;

            //for data save
            itemCapacity = unitList[this.selectedUnitNo].itemCapacity;
            affiliation = unitList[this.selectedUnitNo].affiliation;
            uniqueId = unitList[this.selectedUnitNo].uniqueId;

            // load from saved data
            unitList[this.selectedUnitNo].itemList = itemDataBase.LoadItemList("item-" + unitList[this.selectedUnitNo].affiliation + "-" + unitList[this.selectedUnitNo].uniqueId);

            characterTreeViewDataSourceMgr.Show();
            characterTreeViewWithStickyHeadScript.Initialization();

        }

    }

    public void AddAndSaveItem(Item item)
    {
        //List<Item> _itemList = GetItemList();
        unitList[selectedUnitNo].itemList.Add(item);
        itemDataBase.SaveItemList("item-" + unitList[selectedUnitNo].affiliation + "-" + unitList[selectedUnitNo].uniqueId,
            unitList[selectedUnitNo].itemList);

    }

    public void RemoveAndSaveItem(Item item)
    {
        //List<Item> _itemList = GetItemList();

        for (int i = unitList[selectedUnitNo].itemList.Count - 1; i >= 0; i--)
        {
            if (unitList[selectedUnitNo].itemList[i] == item)
            {
                unitList[selectedUnitNo].itemList.RemoveAt(i);
                continue;
            }
        }
        itemDataBase.SaveItemList("item-" + unitList[selectedUnitNo].affiliation + "-" + unitList[selectedUnitNo].uniqueId,
            unitList[selectedUnitNo].itemList);
    }


    // for outernal use
    public List<Item> GetItemList()
    {
        return unitList[selectedUnitNo].itemList;

        //return itemDataBase.LoadItemList("item-" + UnitList[selectedUnitNo].Affiliation + "-" + UnitList[selectedUnitNo].UniqueID);

    }


}
