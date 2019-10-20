using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

sealed public class ItemDataBase :MonoBehaviour
{

    public List<ItemBaseMaster> itemBaseMasterList;
    public List<ItemBaseMaster> prefixItemBaseMasterList;
    public List<ItemBaseMaster> suffixItemBaseMasterList;

    // save flag

    public List<Item> LoadItemList(string savedFileName)
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file;
        List<ItemForSave> itemForSaveList = new List<ItemForSave>();
        try
        {
            file = File.Open(Application.persistentDataPath + "/" + savedFileName + ".save", FileMode.Open);
            itemForSaveList = (List<ItemForSave>)bf.Deserialize(file);
            file.Close();

        }
        catch (IOException)
        {
            // first time to load file 
            Debug.Log("unable to load: " + savedFileName);
        }

        List<Item> itemList = new List<Item>();
        foreach (ItemForSave itemForSave in itemForSaveList)
        {
            Item item = new Item();
            item.baseItem = itemBaseMasterList.Find(obj => obj.itemId == itemForSave.bI);
            item.prefixItem = prefixItemBaseMasterList.Find(obj => obj.itemId == itemForSave.pI);
            item.suffixItem = suffixItemBaseMasterList.Find(obj => obj.itemId == itemForSave.sI);
            item.enhancedValue = itemForSave.eV;

            itemList.Add(item);

        }
        return itemList;
    }


    public void SaveItemList(string savedFileName, List<Item> itemList)
    {
        // Data save test implement.
        List<ItemForSave> itemForSaveList = new List<ItemForSave>();

        foreach (Item item in itemList)
        {
            if (item != null)
            {

                ItemForSave itemForSave = new ItemForSave();
                // 0 means no ItemID
                if (item.baseItem != null) { itemForSave.bI = item.baseItem.itemId; }
                if (item.prefixItem != null) { itemForSave.pI = item.prefixItem.itemId; }
                if (item.suffixItem != null) { itemForSave.sI = item.suffixItem.itemId; }
                itemForSave.eV = item.enhancedValue;

                itemForSaveList.Add(itemForSave);
            }
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + savedFileName +".save");

        bf.Serialize(file, itemForSaveList);
        file.Close();

        //Debug.Log("Saved: " + savedFileName);

    }


}
