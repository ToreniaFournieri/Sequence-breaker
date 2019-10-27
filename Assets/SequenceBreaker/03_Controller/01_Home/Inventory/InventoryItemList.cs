using System.Collections.Generic;
using SequenceBreaker._01_Data._02_Items._01_ItemMaster;
using SequenceBreaker._01_Data._02_Items.Item;
using SequenceBreaker._03_Controller._00_Global;
using UnityEngine;
using UnityEngine.TestTools;

namespace SequenceBreaker._03_Controller._01_Home.Inventory
{
    public sealed class InventoryItemList : MonoBehaviour
    {
        public bool isInfinityInventoryMode;
        public List<Item> itemList;
        public ItemDataBase itemDataBase;

        private void Start()
        {
            //load from db
            Init();

        }


        //for initialize, can activate from outside
        public void Init()
        {
            LoadFile();
        }

        public void AddItemAndSave(Item addItem)
        {
            Debug.Log("Inventory Add item: " + addItem.ItemName + " itemList Count:" + itemList.Count);

            // no item exist
            if (itemList.Count == 0)
            {
                Item item = addItem.Copy();
                item.amount = 1;
                itemList.Add(item);
            }
            else
            {

                bool isExistSameItem = false;
                for (int i = itemList.Count - 1; i >= 0; i--)
                {
                    if (itemList[i].GetID() == addItem.GetID())
                    {
                        if (itemList[i].amount >= 99)
                        {
                            //nothing to do.
                            isExistSameItem = true;
                            continue;
                        }
                        else
                        {
                            isExistSameItem = true;
                            itemList[i].amount += 1;
                            continue;
                        }
                    }
                }

                if (isExistSameItem == false)
                {
                    Item item = addItem.Copy();
                    item.amount = 1;
                    itemList.Add(item);
                }
            }

            SaveFile();
        }


        public void RemoveItemAndSave(Item removedItem)
        {
            Debug.Log("inventoryItem been removed: " + removedItem.ItemName + " itemList count:" + itemList.Count);
            for (int i = itemList.Count - 1; i >= 0; i--)
            {
                
                if (itemList[i].GetID() == removedItem.GetID())
                {
                    Debug.Log("itemList amount: " + itemList[i].amount);
                    if (itemList[i].amount > 1)
                    {
                        itemList[i].amount -= 1;
                    }
                    else
                    {
                        itemList.RemoveAt(i);
                    }
                    continue;
                }
            }

            SaveFile();
        }


        private void SaveFile()
        {
            if (isInfinityInventoryMode)
            {
                itemDataBase.SaveItemList("item-" + "Debug-" + "inventory", itemList);
            }
            else
            {
                itemDataBase.SaveItemList("item-" + "Ally-"+ "inventory", itemList);
            }
        }

        private void LoadFile()
        {
            if (isInfinityInventoryMode)
            {
//                itemList = itemDataBase.LoadItemList("item-" + "Debug-" + "inventory");

                itemList.Clear();

                Item item;
                foreach (ItemBaseMaster itemBaseMaster in itemDataBase.itemBaseMasterList)
                {
                    item = new Item();
//                    Debug.Log("itemBaseMaster: " + itemBaseMaster.itemName);
                    item.prefixItem = null;
                    item.baseItem = itemBaseMaster;
                    item.suffixItem = null;
                    item.enhancedValue = 0;
                    item.amount = 99;
                    itemList.Add(item);
                }
                



            }
            else
            {
                itemList = itemDataBase.LoadItemList("item-" + "Ally-"+ "inventory");
            }
        }


    }
}
