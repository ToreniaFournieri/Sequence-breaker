using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

sealed public class CharacterStatusDisplay : MonoBehaviour
{

    public ItemDataBase itemDataBase;
    public List<UnitClass> UnitList;

    public CharacterTreeViewDataSourceMgr characterTreeViewDataSourceMgr;
    public CharacterTreeViewWithStickyHeadScript characterTreeViewWithStickyHeadScript;



    //Don't change these following values.
    public Text characterNameText;
    public Text itemAmountText;
    public Text AbilityText;
    //public List<Item> itemList;

    //for data save
    public Affiliation affiliation;
    public int uniqueID;
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


    public void SetCharacterStatus(int _selectedUnitNo)
    {
        //Debug.Log("in SetCharacterStatus: " + UnitList[_selectedUnitNo].Name);

        if (UnitList != null && _selectedUnitNo < UnitList.Count)
        {

            // updating
            selectedUnitNo = _selectedUnitNo;

            characterNameText.text = UnitList[selectedUnitNo].Name + " (Level:" + UnitList[selectedUnitNo].Level + ")";
            itemAmountText.text = UnitList[selectedUnitNo].itemList.Count + "/" + UnitList[selectedUnitNo].ItemCapacity;

            CalculateUnitStatus _calculateUnitStatus = new CalculateUnitStatus(UnitList[selectedUnitNo]);
            AbilityText.text = _calculateUnitStatus.detailAbilityString;

            //for data save
            itemCapacity = UnitList[selectedUnitNo].ItemCapacity;
            affiliation = UnitList[selectedUnitNo].Affiliation;
            uniqueID = UnitList[selectedUnitNo].UniqueID;

            // load from saved data
            UnitList[selectedUnitNo].itemList = itemDataBase.LoadItemList("item-" + UnitList[selectedUnitNo].Affiliation + "-" + UnitList[selectedUnitNo].UniqueID);

            characterTreeViewDataSourceMgr.Show();
            characterTreeViewWithStickyHeadScript.Initialization();

        }

    }

    public void AddAndSaveItem(Item _item)
    {
        //List<Item> _itemList = GetItemList();
        UnitList[selectedUnitNo].itemList.Add(_item);
        itemDataBase.SaveItemList("item-" + UnitList[selectedUnitNo].Affiliation + "-" + UnitList[selectedUnitNo].UniqueID,
            UnitList[selectedUnitNo].itemList);

    }

    public void RemoveAndSaveItem(Item _item)
    {
        //List<Item> _itemList = GetItemList();

        for (int i = UnitList[selectedUnitNo].itemList.Count - 1; i >= 0; i--)
        {
            if (UnitList[selectedUnitNo].itemList[i] == _item)
            {
                UnitList[selectedUnitNo].itemList.RemoveAt(i);
                continue;
            }
        }
        itemDataBase.SaveItemList("item-" + UnitList[selectedUnitNo].Affiliation + "-" + UnitList[selectedUnitNo].UniqueID,
            UnitList[selectedUnitNo].itemList);
    }


    // for outernal use
    public List<Item> GetItemList()
    {
        return UnitList[selectedUnitNo].itemList;

        //return itemDataBase.LoadItemList("item-" + UnitList[selectedUnitNo].Affiliation + "-" + UnitList[selectedUnitNo].UniqueID);

    }


}
