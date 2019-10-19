using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed public class InventoryItemList : MonoBehaviour
{
    public List<Item> itemList;
    public ItemDataBase itemDataBase;

    private void Start()
    {
        //load from db
        init();

    }


    //for initialize, can activate from outside
    public void init()
    {
        LoadFile();
    }

    public void AddItemAndSave(Item item)
    {
        itemList.Add(item);
        SaveFile();
    }

    //reccomend to use this method
    public void AddItemListAndSave(List<Item> _itemList)
    {
        foreach (Item item in _itemList)
        {
            itemList.Add(item);

        }

        SaveFile();

    }

    public void removeItemAndSave(Item _removedItem)
    {
        for (int i = itemList.Count - 1; i >= 0; i--)
        {
            if (itemList[i] == _removedItem)
            {
                itemList.RemoveAt(i);
                continue;
            }
        }

        SaveFile();
    }


    private void SaveFile()
    {
        itemDataBase.SaveItemList("item-" + "inventory", itemList);

    }

    private void LoadFile()
    {
        itemList = itemDataBase.LoadItemList("item-" + "inventory");

    }


}
