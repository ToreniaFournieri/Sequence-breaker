using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SequenceBreaker._01_Data.Items.Master;
using SequenceBreaker._01_Data.UnitClass;
using UnityEngine;

namespace SequenceBreaker._01_Data.Items.Item
{
    public sealed class ItemDataBase :MonoBehaviour
    {
        public List<ItemBaseMaster> itemBaseMasterList;
        public List<ItemBaseMaster> prefixItemBaseMasterList;
        public List<ItemBaseMaster> suffixItemBaseMasterList;

        // save flag

        public UnitClass.UnitClass LoadUnitInfo(UnitClass.UnitClass loadCharacterUnit)
        {
            BinaryFormatter bf = new BinaryFormatter();

            List<Item> itemList = new List<Item>();
            if (loadCharacterUnit != null)
            {
                List<ItemForSave> itemForSaveList = new List<ItemForSave>();
                try
                {
                    var file = File.Open(Application.persistentDataPath + "/" + loadCharacterUnit.affiliation + "-"
                                         + loadCharacterUnit.uniqueId + "-item" + ".save", FileMode.Open);
                    itemForSaveList = (List<ItemForSave>) bf.Deserialize(file);
                    file.Close();
                }
                catch (IOException)
                {
                    // first time to load file 
                    itemList.Clear();
                }

                foreach (ItemForSave itemForSave in itemForSaveList)
                {
                    Item item = ScriptableObject.CreateInstance<Item>();
                    item.baseItem = itemBaseMasterList.Find(obj => obj.itemId == itemForSave.bI);
                    item.prefixItem = prefixItemBaseMasterList.Find(obj => obj.itemId == itemForSave.pI);
                    item.suffixItem = suffixItemBaseMasterList.Find(obj => obj.itemId == itemForSave.sI);
                    item.enhancedValue = itemForSave.eV;
                    item.amount = itemForSave.am;

                    itemList.Add(item);

                }
            }
            loadCharacterUnit.itemList = itemList;

            if (loadCharacterUnit != null)
            {
                    UnitClassForSave unitForLoad = new UnitClassForSave();
                    
                    try
                    {
                        var file = File.Open(Application.persistentDataPath + "/" + loadCharacterUnit.affiliation + "-"
                                             + loadCharacterUnit.uniqueId +"-unitInfo" + ".save",
                            FileMode.Open);
                        unitForLoad = (UnitClassForSave) bf.Deserialize(file);
                        file.Close();
                    }
                    catch (IOException)
                    {
                        // first time to load file 
//                        Debug.Log("unable to load unitInfo: " + loadCharacterUnit.affiliation + "-" + loadCharacterUnit.uniqueId);
                    }
                    
                    loadCharacterUnit.experience = unitForLoad.experience;

            }
            
            return loadCharacterUnit;
        }


        public void SaveUnitInfo(UnitClass.UnitClass saveCharacterUnit)
        {
            BinaryFormatter bf = new BinaryFormatter();

            // Data save test implement.
            if (saveCharacterUnit != null)
            {
                List<ItemForSave> itemForSaveList = new List<ItemForSave>();

                foreach (Item item in saveCharacterUnit.itemList)
                {
                    if (item != null)
                    {

                        ItemForSave itemForSave = new ItemForSave();
                        // 0 means no ItemID
                        if (item.baseItem != null)
                        {
                            itemForSave.bI = item.baseItem.itemId;
                        }

                        if (item.prefixItem != null)
                        {
                            itemForSave.pI = item.prefixItem.itemId;
                        }

                        if (item.suffixItem != null)
                        {
                            itemForSave.sI = item.suffixItem.itemId;
                        }

                        itemForSave.eV = item.enhancedValue;
                        itemForSave.am = item.amount;
                        itemForSaveList.Add(itemForSave);
                    }
                }

                FileStream file = File.Create(Application.persistentDataPath + "/" + saveCharacterUnit.affiliation +"-"
                                              + saveCharacterUnit.uniqueId + "-item" + ".save");

                bf.Serialize(file, itemForSaveList);
                file.Close();
            }

            // save experience point
            if (saveCharacterUnit != null)
            {
                UnitClassForSave unitForSave = new UnitClassForSave {experience = saveCharacterUnit.experience};

                var file = File.Create(Application.persistentDataPath + "/" +saveCharacterUnit.affiliation + "-"
                                                                            + saveCharacterUnit.uniqueId +"-unitInfo" + ".save");
                        bf.Serialize(file, unitForSave);
                        file.Close();
                        
            }
            

            
        }


    }
}
