using System.Collections.Generic;
using SequenceBreaker._01_Data.Items.Item;
using SequenceBreaker._04_Home.EquipView.Character;
using UnityEngine;

namespace SequenceBreaker._04_Home.EquipView.Inventory
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
        public ItemDataBase itemDataBase;

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
                    _instance = Object.FindObjectOfType<InventoryTreeViewDataSourceMgr>();
                }
                return _instance;
            }

        }

        void Awake()
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
            if (otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.itemCapacity > otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.GetItemList().Count)
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
            
            for (int i = 0; i < _mTreeViewItemCount; ++i)
            {
                InventoryTreeViewItemData tData = new InventoryTreeViewItemData
                {
                    MName = "Main Item Category: " + i, MIcon = "1"
                };
                _mItemDataList.Add(tData);
                

                if (isInfinityInventoryMode)
                {
                    infinityInventoryItemList.Init();
                    infinityInventoryItemList.inventory.itemList.Sort((x, y) => (x.baseItem.itemId - y.baseItem.itemId) )   ;

                    foreach (Item item in infinityInventoryItemList.inventory.itemList)
                    {
                        tData.AddChild(item);
                    }
                }
                else
                {
                    allyInventoryItemList.Init();
                    
                    if (allyInventoryItemList.inventory.itemList != null)
                    {
                        allyInventoryItemList.inventory.itemList.Sort((x, y) =>
                            (x.baseItem.itemId - y.baseItem.itemId));
                        foreach (Item item in allyInventoryItemList.inventory.itemList)
                        {
                            tData.AddChild(item);
                        }
                    }
                }


                inventoryTreeViewWithStickyHeadScript.Initialization();
            }


        }



    }
}