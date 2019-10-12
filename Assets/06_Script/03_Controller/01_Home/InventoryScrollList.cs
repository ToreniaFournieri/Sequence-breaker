using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


//2019.10.12 TreeViewWithStickyHeadDemo is preferdred.
// 


public class InventoryScrollList : MonoBehaviour
{
    public List<Item> itemList;
    public float capacity = 200f;
    public UnitClass unit;

    // initial inventory master;
    public bool doesSetInitialInventory;
    public bool isLoadFromFile;
    public UnitClass initialInventoryUnit;

    // Global item master
    public ItemDataBase itemDataBase;

    public Transform contentPanel;
    public InventoryScrollList otherInventory;
    public RefreshController refreshController;
    public Text abilityText;
    public SimpleObjectPool itemObjectPool;
    public ItemDetailViewController itemDetailViewController;

    // 

    void Start()
    {
        if (unit != null)
        {
            unit.itemList = itemDataBase.LoadItemList("item-" + unit.Affiliation +"-" + unit.UniqueID);
        }

        RefreshDisplay();
    }

    public void SwitchUnit(UnitClass _unit)
    {
        this.unit = _unit;
        if (unit != null)
        {
            unit.itemList = itemDataBase.LoadItemList("item-" + unit.Affiliation + "-"+ unit.UniqueID);
        }
    }

    public void RefreshDisplay()
    {
        unit.itemList = itemDataBase.LoadItemList("item-" + unit.Affiliation + "-" + unit.UniqueID);


        if (unit != null)
        {
            capacity = unit.ItemCapacity;
            itemList = unit.itemList;
        }
        refreshController.NeedToRefresh = true;
        RemovePanels();
        AddPanels();


    }

    private void RemovePanels()
    {
        while (contentPanel.childCount > 0)
        {
            GameObject toRemove = transform.GetChild(0).gameObject;
            itemObjectPool.ReturnObject(toRemove);
        }
    }

    public void TryTransferItemToOtherInventory(Item item)
    {
        if (otherInventory.capacity > otherInventory.itemList.Count)
        {


            AddItemTo(item, otherInventory);
            RemoveItemFrom(item, this);



            RefreshDisplay();
            otherInventory.RefreshDisplay();
        }
    }


    private void AddPanels()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] != null)
            {
                Item item = itemList[i];
                GameObject newPanel = itemObjectPool.GetObject();
                newPanel.transform.SetParent(contentPanel);

                ItemPanel itemPanel = newPanel.GetComponent<ItemPanel>();
                itemPanel.Setup(item, this, itemDetailViewController);
            }
        }
    }

    //Add item of public object
    public void AddItem(Item item)
    {

        AddItemTo(item, this);

    }

    //Public save order
    public void Save(InventoryScrollList inventoryList)
    {
        inventoryList.itemDataBase.SaveItemList("item-" + inventoryList.unit.Affiliation + "-" + inventoryList.unit.UniqueID, inventoryList.unit.itemList);

    }

    // private object
    private void AddItemTo(Item itemToAdd, InventoryScrollList inventoryList)
    {
        //Debug.Log("in inventoryScroll before: " + itemToAdd.itemName + " Number of List: "+ inventoryList.itemList.Count);

        inventoryList.unit.itemList.Add(itemToAdd);
        inventoryList.itemDataBase.SaveItemList("item-" + inventoryList.unit.Affiliation + "-" + inventoryList.unit.UniqueID, inventoryList.unit.itemList);

        //Debug.Log("in inventoryScroll after: " + itemToAdd.itemName + " Number of List: " + inventoryList.itemList.Count);

    }

    private void RemoveItemFrom(Item itemToRemove, InventoryScrollList inventoryList)
    {
        for (int i = inventoryList.itemList.Count - 1; i >= 0; i--)
        {
            if (inventoryList.itemList[i] == itemToRemove)
            {
                inventoryList.unit.itemList.RemoveAt(i);
            }
        }
        inventoryList.itemDataBase.SaveItemList("item-" + inventoryList.unit.Affiliation + "-" + inventoryList.unit.UniqueID, inventoryList.unit.itemList);

    }

}