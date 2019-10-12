using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ItemDataBase :MonoBehaviour
{

    public List<ItemBaseMaster> itemBaseMasterList;
    public List<ItemBaseMaster> prefixItemBaseMasterList;
    public List<ItemBaseMaster> suffixItemBaseMasterList;

    // save flag

    public List<Item> LoadItemList(string savedFileName)
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file;
        List<ItemForSave> _itemForSaveList = new List<ItemForSave>();
        try
        {
            file = File.Open(Application.persistentDataPath + "/" + savedFileName + ".save", FileMode.Open);
            _itemForSaveList = (List<ItemForSave>)bf.Deserialize(file);
            file.Close();

        }
        catch (IOException)
        {
            // first time to load file 
            Debug.Log("unable to load: " + savedFileName);
        }

        List<Item> _itemList = new List<Item>();
        foreach (ItemForSave _itemForSave in _itemForSaveList)
        {
            Item _item = new Item();
            _item.baseItem = itemBaseMasterList.Find(obj => obj.itemID == _itemForSave.bI);
            _item.prefixItem = prefixItemBaseMasterList.Find(obj => obj.itemID == _itemForSave.pI);
            _item.suffixItem = suffixItemBaseMasterList.Find(obj => obj.itemID == _itemForSave.sI);
            _item.enhancedValue = _itemForSave.eV;

            _itemList.Add(_item);

        }
        return _itemList;
    }


    public void SaveItemList(string savedFileName, List<Item> itemList)
    {
        // Data save test implement.
        List<ItemForSave> _itemForSaveList = new List<ItemForSave>();

        foreach (Item _item in itemList)
        {
            ItemForSave _itemForSave = new ItemForSave();
            // 0 means no ItemID
            if (_item.baseItem != null) { _itemForSave.bI = _item.baseItem.itemID; }
            if (_item.prefixItem != null) { _itemForSave.pI = _item.prefixItem.itemID; }
            if (_item.suffixItem != null) { _itemForSave.sI = _item.suffixItem.itemID; }
            _itemForSave.eV = _item.enhancedValue;

            _itemForSaveList.Add(_itemForSave);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + savedFileName +".save");

        bf.Serialize(file, _itemForSaveList);
        file.Close();

        //Debug.Log("Saved: " + savedFileName);

    }


}
