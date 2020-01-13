using System.Collections.Generic;
using SequenceBreaker.Master.Items;
using UnityEngine;

namespace SequenceBreaker.Home.EquipView
{
    public sealed class InventoryTreeViewItemData
    {
        public string MName;
        public string MIcon;
        List<Item> _mChildItemDataList = new List<Item>();

        public int ChildCount
        {
            get { return _mChildItemDataList.Count; }
        }

        public void AddChild(Item data)
        {
            _mChildItemDataList.Add(data);
        }

        public void SortItemList()
        {
            try
            {
                //_mChildItemDataList.Sort((x, y) => (x.baseItem.itemId - y.baseItem.itemId));

                _mChildItemDataList.Sort((x, y) => y.CompareTo(x));


            }
            catch
            {
                Debug.LogError("Failed Sort item list: " + MName + " target list count:" + _mChildItemDataList.Count);

            }
        }

        public Item GetChild(int index)
        {
            if (index < 0 || index >= _mChildItemDataList.Count)
            {
                return null;
            }
            return _mChildItemDataList[index];
        }
    }

    public sealed class InventoryTreeViewDataSourceMgr : MonoBehaviour
    {
        // Item data base 
        //public ItemDataBase itemDataBase;

        // inventory
        public InventoryItemList allyInventoryItemList;

        //debug mode 
        public InventoryItemList infinityInventoryItemList;
        public bool isInfinityInventoryMode;



        // Other Inventory, which means character 
        public CharacterTreeViewDataSourceMgr otherCharacterTreeViewDataSourceMgr;


        //For update graphics
        public InventoryTreeViewWithStickyHeadScript inventoryTreeViewWithStickyHeadScript;


        List<InventoryTreeViewItemData> _mItemDataList = new List<InventoryTreeViewItemData>();

        static InventoryTreeViewDataSourceMgr _instance = null;
        int _mTreeViewItemCount = 1;

        public static InventoryTreeViewDataSourceMgr Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<InventoryTreeViewDataSourceMgr>();
                }
                return _instance;
            }

        }

        //void Awake()
        //{
        //    Init();
        //}

        private void Start()
        {
            Init();
        }


        public void Init()
        {
            DoRefreshDataSource();
        }

        public void AddItemAndSave(Item item)
        {
            if (isInfinityInventoryMode)
            {
                infinityInventoryItemList.AddItemAndSave(item);
            }
            else
            {
                allyInventoryItemList.AddItemAndSave(item);
            }
        }


        public InventoryTreeViewItemData GetItemDataByIndex(int index)
        {
            if (index < 0 || index >= _mItemDataList.Count)
            {
                return null;
            }
            return _mItemDataList[index];
        }

        public Item GetItemChildDataByIndex(int itemIndex, int childIndex)
        {
            InventoryTreeViewItemData data = GetItemDataByIndex(itemIndex);
            if (data == null)
            {
                return null;
            }
            return data.GetChild(childIndex);
        }

        public int TreeViewItemCount => _mItemDataList.Count;

        public int TotalTreeViewItemAndChildCount
        {
            get
            {
                var count = _mItemDataList.Count;
                var totalCount = 0;
                for (int i = 0; i < count; ++i)
                {
                    totalCount = totalCount + _mItemDataList[i].ChildCount;
                }
                return totalCount;
            }
        }


        public void TryTransferItemToOtherInventory(Item item)
        {
            if (otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.selectedCharacter.itemCapacity > otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.GetItemList().Count)
            {

                //add item
                bool isFailed = otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.AddAndSaveItem(item);

                if (isFailed == false)
                {

                    //remove from other inventory
                    if (isInfinityInventoryMode)
                    {
                        infinityInventoryItemList.RemoveItemAndSave(item);
                    }
                    else
                    {
                        allyInventoryItemList.RemoveItemAndSave(item);

                    }
                }


                DoRefreshDataSource();
                otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.RefreshCharacterStatusAndItemList();
            }
        }



        public void DoRefreshDataSource()
        {
            _mItemDataList.Clear();

            List<Item> itemList = new List<Item>();
            if (isInfinityInventoryMode)
            {
                infinityInventoryItemList.Init();
                itemList = infinityInventoryItemList.inventory.itemList;
            }
            else
            {
                allyInventoryItemList.Init();
                itemList = allyInventoryItemList.inventory.itemList;
            }

            itemList.Sort((x, y) => (x.baseItem.itemCategory - y.baseItem.itemCategory));

            int categoryCount = 0;
            int ppreviousCategory = 0;
            List<int> categorySorted = new List<int>();
            foreach (Item item in itemList)
            {
                if (item.baseItem.itemCategory != ppreviousCategory)
                {
                    categoryCount++;
                    ppreviousCategory = item.baseItem.itemCategory;
                    categorySorted.Add(item.baseItem.itemCategory);
                }
            }
            //for (int i = 0; i < _mTreeViewItemCount; ++i)

            foreach (int category in categorySorted)
            {
                InventoryTreeViewItemData tData = new InventoryTreeViewItemData
                {
                    MName = ItemDataBase.instance.GetItemCategoryName(category),
                    MIcon = "1"
                };
                _mItemDataList.Add(tData);

                //List<Item> _categorizedItemList = itemList.FindAll(x => x.baseItem.itemCategory == category);
                //_categorizedItemList.Sort((x, y) => (y.enhancedValue - x.enhancedValue));
                foreach (Item item in itemList.FindAll(x => x.baseItem.itemCategory == category))
                {
                    tData.AddChild(item);
                }

                tData.SortItemList();

            }

            inventoryTreeViewWithStickyHeadScript.Initialization();



        }



    }
}