using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SequenceBreaker.Master.Units;
using SequenceBreaker.Translate;
using UnityEngine;

namespace SequenceBreaker.Master.Items
{
    public sealed class ItemDataBase : MonoBehaviour
    {
        public ItemBaseMasterList prefixBaseList;
        public ItemBaseMasterList itemBaseList;
        public ItemBaseMasterList suffixBaseList;


        public ItemPresetList itemPresetList;

        public static ItemDataBase instance;

        private void Awake()
        {
            //Debug.Log("ItemDataBase.Awake() GetInstanceID=" + this.GetInstanceID().ToString());

            if (instance == null)
            {
                instance = this;  //This is the first Singleton instance. Retain a handle to it.
            }
            else
            {
                if (instance != this)
                {
                    Destroy(this); //This is a duplicate Singleton. Destroy this instance.
                }
                else
                {
                    //Existing Singleton instance found. All is good. No change.
                }
            }

            DontDestroyOnLoad(gameObject);
        }

        //Warning should only use for a game reset.
        public void DeleteAllSavedFiles(bool reallyDeleteAlFiles)
        {
            if (reallyDeleteAlFiles)
            {
                DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath);
                FileInfo[] fileInfo = info.GetFiles();
                foreach (FileInfo file in fileInfo)
                {
                    // sample implement
                    if (file.Name.Contains("Ally"))
                    {
                        //Debug.Log("file name: " + file.Name);
                        //if(file.Name == "Ally-100001-item.save")
                        //{
                        file.Delete();

                        //}
                    }

                }

            }
        }

        // save flag

        public UnitClass LoadUnitInfo(UnitClass loadCharacterUnit)
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
                    itemForSaveList = (List<ItemForSave>)bf.Deserialize(file);
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
                    item.baseItem = itemBaseList.itemBaseMasterList.Find(obj => obj.itemId == itemForSave.bI);
                    item.prefixItem = prefixBaseList.itemBaseMasterList.Find(obj => obj.itemId == itemForSave.pI);
                    item.suffixItem = suffixBaseList.itemBaseMasterList.Find(obj => obj.itemId == itemForSave.sI);


                    //item.baseItem = itemBaseMasterList.Find(obj => obj.itemId == itemForSave.bI);
                    //item.prefixItem = prefixItemBaseMasterList.Find(obj => obj.itemId == itemForSave.pI);
                    //item.suffixItem = suffixItemBaseMasterList.Find(obj => obj.itemId == itemForSave.sI);
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
                                         + loadCharacterUnit.uniqueId + "-unitInfo" + ".save",
                        FileMode.Open);
                    unitForLoad = (UnitClassForSave)bf.Deserialize(file);
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


        public void SaveUnitInfo(UnitClass saveCharacterUnit)
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

                FileStream file = File.Create(Application.persistentDataPath + "/" + saveCharacterUnit.affiliation + "-"
                                              + saveCharacterUnit.uniqueId + "-item" + ".save");

                bf.Serialize(file, itemForSaveList);
                file.Close();
            }

            // save experience point
            if (saveCharacterUnit != null)
            {
                UnitClassForSave unitForSave = new UnitClassForSave { experience = saveCharacterUnit.experience };

                var file = File.Create(Application.persistentDataPath + "/" + saveCharacterUnit.affiliation + "-"
                                                                            + saveCharacterUnit.uniqueId + "-unitInfo" + ".save");
                bf.Serialize(file, unitForSave);
                file.Close();

            }



        }

        public List<Item> GetItemsFromUniqueId(int uniqueId)
        {
            List<Item> itemList = new List<Item>();

            foreach (var set in itemPresetList.itemPresetList)
            {
                if (uniqueId == set.characterUniqueId)
                {
                    foreach (var id in set.itemIdList)
                    {
                        itemList.Add(this.GetItemFromId(id.prefixId, id.baseId, id.suffixId, id.enhancedValue));
                    }
                }

            }
            return itemList;


        }

        public Item GetItemFromItemIdSet(ItemIdSet itemIdSet)
        {
            return GetItemFromId(itemIdSet.prefixId, itemIdSet.baseId, itemIdSet.suffixId, itemIdSet.enhancedValue);
        }




        public Item GetItemFromId(int prefixId, int baseId, int suffixId, int enhancedValue)
        {
            //Item item = new Item();
            Item item = ScriptableObject.CreateInstance<Item>();

            if (prefixBaseList != null && prefixBaseList.itemBaseMasterList.Count > 0)
            {
                foreach (var prefix in prefixBaseList.itemBaseMasterList)
                {
                    if (prefixId == prefix.itemId)
                    {
                        item.prefixItem = prefix;
                    }
                }
            }
            if (itemBaseList != null && itemBaseList.itemBaseMasterList.Count > 0)
            {


                foreach (var itemBase in itemBaseList.itemBaseMasterList)
                {
                    if (baseId == itemBase.itemId)
                    {
                        item.baseItem = itemBase;
                    }
                }
            }

            if (suffixBaseList != null && suffixBaseList.itemBaseMasterList.Count > 0)
            {
                foreach (var suffix in suffixBaseList.itemBaseMasterList)
                {
                    if (suffixId == suffix.itemId)
                    {
                        item.suffixItem = suffix;
                    }
                }
            }
            item.enhancedValue = enhancedValue;

            // it fix
            item.amount = 1;
            return item;

        }

        public string GetItemCategoryName(int itemCategory)
        {
            switch (itemCategory)
            {

                case 0:
                    return Word.Get("ItemCategory-nothing");
                case 1:
                    return Word.Get("ItemCategory-Weapon Kinetic");
                case 2:
                    return Word.Get("ItemCategory-Weapon Chemical");
                case 3:
                    return Word.Get("ItemCategory-Weapon Thermal");
                case 10:
                    return Word.Get("ItemCategory-Head (+Accuracy)");
                case 11:
                    return Word.Get("ItemCategory-Body (+Base HP)");
                case 12:
                    return Word.Get("ItemCategory-Structure (+Base Defense)");
                case 13:
                    return Word.Get("ItemCategory-Breastplate (+Defense Critical)");
                case 14:
                    return Word.Get("ItemCategory-Arm (+Base Attack)");
                case 15:
                    return Word.Get("ItemCategory-Leg (+Base Mobility)");
                case 16:
                    return Word.Get("ItemCategory-Shield (+Base Shield)");
                case 20:
                    return Word.Get("ItemCategory-Magazine (+Number of attacks)");
                case 21:
                    return Word.Get("ItemCategory-(+Critical damage)");
                case 30:
                    return Word.Get("ItemCategory-Armor (+Defense Kinetic)");
                case 31:
                    return Word.Get("ItemCategory-Armor (+Defense Chemical)");
                case 32:
                    return Word.Get("ItemCategory-Armor (+Defense Thermal)");
                case 40:
                    return Word.Get("ItemCategory-Repair (+Base HP)");
                case 60:
                    return Word.Get("ItemCategory-Ability (+ability)");



                default:
                    return itemCategory + " unknown";
            }
        }

        public string GetItemIndexShortName(int itemCategory)
        {
            switch (itemCategory)
            {

                case 0:
                    return Word.Get("ItemCategoryShort-No");
                case 1:
                    return Word.Get("ItemCategoryShort-WKin");
                case 2:
                    return Word.Get("ItemCategoryShort-WChe");
                case 3:
                    return Word.Get("ItemCategoryShort-WThe");
                case 10:
                    return Word.Get("ItemCategoryShort-Head");
                case 11:
                    return Word.Get("ItemCategoryShort-Body");
                case 12:
                    return Word.Get("ItemCategoryShort-Strc");
                case 13:
                    return Word.Get("ItemCategoryShort-Bres");
                case 14:
                    return Word.Get("ItemCategoryShort-Arm");
                case 15:
                    return Word.Get("ItemCategoryShort-Leg");
                case 16:
                    return Word.Get("ItemCategoryShort-Shie");
                case 20:
                    return Word.Get("ItemCategoryShort-Mgzn");
                case 21:
                    return Word.Get("ItemCategoryShort-DCri");
                case 30:
                    return Word.Get("ItemCategoryShort-DKin");
                case 31:
                    return Word.Get("ItemCategoryShort-DChe");
                case 32:
                    return Word.Get("ItemCategoryShort-DThe");
                case 40:
                    return Word.Get("ItemCategoryShort-Repa");
                case 60:
                    return Word.Get("ItemCategoryShort-Abli");
                default:
                    return itemCategory + "?";
            }
        }


    }
}
