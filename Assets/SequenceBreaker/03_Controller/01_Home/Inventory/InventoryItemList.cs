using System.Collections.Generic;
using SequenceBreaker._01_Data._02_Items._01_ItemMaster;
using SequenceBreaker._01_Data._02_Items.Item;
using SequenceBreaker._03_Controller._00_Global;
using UnityEngine;

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

        public void AddItemAndSave(Item item)
        {
            itemList.Add(item);
            SaveFile();
        }

        //reccomend to use this method
        public void AddItemListAndSave(List<Item> itemList)
        {
            foreach (Item item in itemList)
            {
                this.itemList.Add(item);

            }

            SaveFile();

        }

        public void RemoveItemAndSave(Item removedItem)
        {
            for (int i = itemList.Count - 1; i >= 0; i--)
            {
                if (itemList[i] == removedItem)
                {
                    itemList.RemoveAt(i);
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
                    Debug.Log("itemBaseMaster: " + itemBaseMaster.itemName);
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
