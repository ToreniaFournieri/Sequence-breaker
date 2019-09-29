using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class InventoryScrollList : MonoBehaviour
{
    public List<Item> itemList;
    public float capacity = 200f;
    public UnitClass unit;

    // initial inventory master;
    public bool doesSetInitialInventory;
    public bool isLoadFromFile;
    public UnitClass initialInventoryUnit;

    public Transform contentPanel;
    public InventoryScrollList otherInventory;
    public RefreshController refreshController;
    public Text abilityText;
    public SimpleObjectPool itemObjectPool;
    public ItemDetailViewController itemDetailViewController;

    void Start()
    {
        if (isLoadFromFile)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);

            List<ItemForSave> _itemForSaveList = (List<ItemForSave>)bf.Deserialize(file);
            file.Close();

            List<Item> _itemList = new List<Item>();
            foreach (ItemForSave _itemForSave in _itemForSaveList)
            {
                Item _item = new Item();
                ItemBaseMaster _BaseItem = new ItemBaseMaster();
                ItemBaseMaster _PrefixItem = new ItemBaseMaster();
                ItemBaseMaster _SuffixItem = new ItemBaseMaster();

                //test implement
                _BaseItem = initialInventoryUnit.itemList.Find(obj => obj.baseItem.itemID == _itemForSave.bI).baseItem;

                _item.baseItem = _BaseItem;

                unit.itemList.Add(_item);

            }


        }
        else
        {

            if (doesSetInitialInventory && initialInventoryUnit != null)
            {
                // inventory 
                unit.ItemCapacity = initialInventoryUnit.ItemCapacity;

                unit.itemList.Clear();

                foreach (Item item in initialInventoryUnit.itemList)
                {
                    Item copyedItem = Instantiate(item.Copy());
                    unit.itemList.Add(copyedItem);
                }

            }
        }


       

        RefreshDisplay();
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