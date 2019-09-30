using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


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
            unit.itemList = itemDataBase.LoadItemList("item-" + unit.UniqueID);
        }
        //if (isLoadFromFile)
        //{
        //}
        //else
        //{

        //    if (doesSetInitialInventory && initialInventoryUnit != null)
        //    {
        //        // inventory 
        //        unit.ItemCapacity = initialInventoryUnit.ItemCapacity;

        //        unit.itemList.Clear();

        //        foreach (Item item in initialInventoryUnit.itemList)
        //        {
        //            Item copyedItem = Instantiate(item.Copy());
        //            unit.itemList.Add(copyedItem);
        //        }

        //    }
        //}


       

        RefreshDisplay();
    }

    public void SwitchUnit(UnitClass _unit)
    {
        this.unit = _unit;
        if (unit != null)
        {
            unit.itemList = itemDataBase.LoadItemList("item-" + unit.UniqueID);
        }
    }



    public void RefreshDisplay()
    {

        // to Delete empty itemList
        for (int i = unit.itemList.Count - 1 ; i >= 0; i--)
        {
            //Debug.Log(unit.Name + " " + i + " item count:" + unit.itemList.Count);

            if (i >= 0)
            {
                if (unit.itemList[i] == null)
                {
                    unit.itemList.RemoveAt(i);
                }
            }
        }


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


            AddItem(item, otherInventory);
            RemoveItem(item, this);



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
    public void AddItemAndSave(Item item)
    {
        AddItem(item, this);

        //itemList.Add(item);
        //itemDataBase.SaveItemList("item-" + unit.UniqueID, itemList);
    }

    // private object
    private void AddItem(Item itemToAdd, InventoryScrollList inventoryList)
    {
        inventoryList.itemList.Add(itemToAdd);
        inventoryList.itemDataBase.SaveItemList("item-" + inventoryList.unit.UniqueID, inventoryList.itemList);

    }

    private void RemoveItem(Item itemToRemove, InventoryScrollList inventoryList)
    {
        for (int i = inventoryList.itemList.Count - 1; i >= 0; i--)
        {
            if (inventoryList.itemList[i] == itemToRemove)
            {
                inventoryList.itemList.RemoveAt(i);
            }
        }
        inventoryList.itemDataBase.SaveItemList("item-" + inventoryList.unit.UniqueID, inventoryList.itemList);

    }

}