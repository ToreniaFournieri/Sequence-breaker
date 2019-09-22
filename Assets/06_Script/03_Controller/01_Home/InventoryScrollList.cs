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
    public UnitClass initialInventoryUnit;

    public Transform contentPanel;
    public InventoryScrollList otherInventory;
    public RefreshController refreshController;
    public Text abilityText;
    public SimpleObjectPool itemObjectPool;
    public ItemDetailViewController itemDetailViewController;

    void Start()
    {
        if (initialInventoryUnit != null)
        {
            unit.ItemCapacity = initialInventoryUnit.ItemCapacity;

            unit.itemList.Clear();

            foreach (Item item in initialInventoryUnit.itemList)
            {
                Item copyedItem = Instantiate(item.Copy());
                unit.itemList.Add(copyedItem);
            }

        }

        RefreshDisplay();
    }


    public void RefreshDisplay()
    {
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
            Item item = itemList[i];
            GameObject newPanel = itemObjectPool.GetObject();
            newPanel.transform.SetParent(contentPanel);

            ItemPanel itemPanel = newPanel.GetComponent<ItemPanel>();
            itemPanel.Setup(item, this, itemDetailViewController);
        }
    }

    private void AddItem(Item itemToAdd, InventoryScrollList inventoryList)
    {
        inventoryList.itemList.Add(itemToAdd);
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
    }

}