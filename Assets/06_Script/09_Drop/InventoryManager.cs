using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    //public UnitClass inventory;
    //public InventoryScrollList inventoryScrollList;

    public void AddtoInventory(List<Item> itemList)
    {
        foreach (Item item in itemList)
        {
            //inventory.itemList.Add(item);
            //inventoryScrollList.itemList.Add(item);
        }

        //inventoryScrollList.RefreshDisplay();
        //inventoryScrollList.otherInventory.RefreshDisplay();
    }

}
