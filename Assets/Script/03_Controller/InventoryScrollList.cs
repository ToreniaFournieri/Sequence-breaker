using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;



public class InventoryScrollList : MonoBehaviour
{
    public List<Item> itemList;
    public float capacity = 200f;
    public UnitClass unit;

    public Transform contentPanel;
    public InventoryScrollList otherInventory;
    public RefreshController refreshController;
    public Text abilityText;
    public SimpleObjectPool itemObjectPool;


    void Start()
    {
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
            //currentCapacity += 1;
            //otherInventory.currentCapacity -= 1;

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
            itemPanel.Setup(item, this);
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