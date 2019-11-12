using System.Collections.Generic;
using SequenceBreaker._01_Data._02_Items._01_ItemMaster;
using SequenceBreaker._01_Data._02_Items.Item;
using SequenceBreaker._01_Data._03_UnitClass;
using SequenceBreaker._03_Controller._00_Global;
using UnityEngine;
using UnityEngine.TestTools;

namespace SequenceBreaker._03_Controller._01_Home.Inventory
{
    public sealed class InventoryItemList : MonoBehaviour
    {
        public bool isInfinityInventoryMode;

        public UnitClass inventory;
//        public List<Item> itemList;
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
//            Debug.Log("Inventory Add item: " + addItem.ItemName + " itemList Count:" + inventory.itemList.Count);

            // no item exist
            if (inventory.itemList.Count == 0)
            {
                Item item = addItem.Copy();
                item.amount = 1;
                inventory.itemList.Add(item);
            }
            else
            {

                bool onceHasBeenAdded = false;
                for (int i = inventory.itemList.Count - 1; i >= 0; i--)
                {
                    if (inventory.itemList[i].GetId() == addItem.GetId())
                    {
                        onceHasBeenAdded = true;

                        if (inventory.itemList[i].amount >= 99)
                        {
                            //nothing to do.
                            continue;
                        }
                        else
                        {
                            inventory.itemList[i].amount += 1;
                            continue;
                        }
                    }
                }

                if (onceHasBeenAdded == false)
                {
                    Item item = addItem.Copy();
                    item.amount = 1;
                    inventory.itemList.Add(item);
                }
            }

            SaveFile();
        }


        public void RemoveItemAndSave(Item removedItem)
        {
//            Debug.Log("inventoryItem been removed: " + removedItem.ItemName + " itemList count:" + inventory.itemList.Count);
            for (int i = inventory.itemList.Count - 1; i >= 0; i--)
            {
                
                if (inventory.itemList[i].GetId() == removedItem.GetId())
                {
//                    Debug.Log("itemList amount: " + inventory.itemList[i].amount);
                    if (inventory.itemList[i].amount > 1)
                    {
                        inventory.itemList[i].amount -= 1;
                    }
                    else
                    {
                        inventory.itemList.RemoveAt(i);
                    }
                    continue;
                }
            }

            SaveFile();
        }


        private void SaveFile()
        {
            itemDataBase.SaveUnitInfo(inventory);
            
        }

        private void LoadFile()
        {
            if (isInfinityInventoryMode)
            {
//                itemList = itemDataBase.LoadUnitInfo("item-" + "Debug-" + "inventory");

                inventory.itemList.Clear();

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
                    inventory.itemList.Add(item);
                }
                



            }
            else
            {
                inventory = itemDataBase.LoadUnitInfo(inventory);
            }
        }


    }
}
