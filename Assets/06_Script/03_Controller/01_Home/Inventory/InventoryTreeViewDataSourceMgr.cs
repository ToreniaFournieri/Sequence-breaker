using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperScrollView;


public class InventoryTreeViewItemData
{
    public string mName;
    public string mIcon;
    List<Item> mChildItemDataList = new List<Item>();

    public int ChildCount
    {
        get { return mChildItemDataList.Count; }
    }

    public void AddChild(Item data)
    {
        mChildItemDataList.Add(data);
    }
    public Item GetChild(int index)
    {
        if (index < 0 || index >= mChildItemDataList.Count)
        {
            return null;
        }
        return mChildItemDataList[index];
    }
}

public class InventoryTreeViewDataSourceMgr : MonoBehaviour
{
    // Item data base 
    public ItemDataBase itemDataBase;
    public bool isDebugModeForInventory;
    //public List<Item> itemList;

    // inventory
    public InventoryItemList inventoryItemList;



    // Other Inventory, whitch means character 
    public CharacterTreeViewDataSourceMgr otherCharacterTreeViewDataSourceMgr;


    //For update graphics
    public InventoryTreeViewWithStickyHeadScript InventoryTreeViewWithStickyHeadScript;


    List<InventoryTreeViewItemData> mItemDataList = new List<InventoryTreeViewItemData>();

    static InventoryTreeViewDataSourceMgr instance = null;
    int mTreeViewItemCount = 1;
    //int mTreeViewChildItemCount = 10;


    //int mTreeViewItemCount = 20;
    //int mTreeViewChildItemCount = 30;

    public static InventoryTreeViewDataSourceMgr Get
    {
        get
        {
            if (instance == null)
            {
                instance = Object.FindObjectOfType<InventoryTreeViewDataSourceMgr>();
            }
            return instance;
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

    public InventoryTreeViewItemData GetItemDataByIndex(int index)
    {
        if (index < 0 || index >= mItemDataList.Count)
        {
            return null;
        }
        return mItemDataList[index];
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

    public int TreeViewItemCount
    {
        get
        {
            return mItemDataList.Count;
        }
    }

    public int TotalTreeViewItemAndChildCount
    {
        get
        {
            int count = mItemDataList.Count;
            int totalCount = 0;
            for (int i = 0; i < count; ++i)
            {
                totalCount = totalCount + mItemDataList[i].ChildCount;
            }
            return totalCount;
        }
    }


    public void TryTransferItemToOtherInventory(Item item)
    {
        if (otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.itemCapacity > otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.GetItemList().Count)
        {

            //add item
            otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.AddAndSaveItem(item);

            //otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.GetItemList().Add(item);
            //itemDataBase.SaveItemList("item-" + otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.affiliation
            //    + "-" + otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.uniqueID,
            //    otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.GetItemList());

            //remove from other inventory
            inventoryItemList.removeItemAndSave(item);

            //for (int i = inventoryItemList.itemList.Count - 1; i >= 0; i--)
            //{
            //    if (inventoryItemList.itemList[i] == item)
            //    {
            //        inventoryItemList.itemList.RemoveAt(i);
            //        continue;
            //    }
            //}
            //itemDataBase.SaveItemList("item-" + "inventory", inventoryItemList.itemList);

            DoRefreshDataSource();
            otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.RefleshCharacterStatusAndItemList();
        }
    }



    public void DoRefreshDataSource()
    {
        mItemDataList.Clear();
        for (int i = 0; i < mTreeViewItemCount; ++i)
        {
            InventoryTreeViewItemData tData = new InventoryTreeViewItemData();
            tData.mName = "Main Item Cateory: " + i;
            //tData.mIcon = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 24));
            tData.mIcon = "1";
            mItemDataList.Add(tData);
            //int childCount = mTreeViewChildItemCount;


            if (isDebugModeForInventory)
            {
                int childCount = itemDataBase.itemBaseMasterList.Count;
                for (int j = 0; j < childCount; ++j)
                {
                    Item _item = new Item();
                    _item.baseItem = itemDataBase.itemBaseMasterList[j];
                    //ItemData childItemData = new ItemData();
                    //childItemData.mName = "Item" + i + ":Child" + j;
                    //childItemData.mDesc = "Item Desc For " + childItemData.mName;
                    ////childItemData.mIcon = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 24));
                    //childItemData.mIcon = "1";
                    //childItemData.mStarCount = Random.Range(0, 6);
                    //childItemData.mFileSize = Random.Range(20, 999);
                    tData.AddChild(_item);
                }
            }
            else
            {

                inventoryItemList.init();
                foreach (Item _item in inventoryItemList.itemList)
                {
                    tData.AddChild(_item);
                }

                Debug.Log("tData count:" + tData.ChildCount);

            }

            InventoryTreeViewWithStickyHeadScript.Initialization();
        }


    }



}

