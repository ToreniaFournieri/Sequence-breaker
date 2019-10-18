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
    public List<Item> itemList;

    // Other Inventory, whitch means character 
    public CharacterTreeViewDataSourceMgr otherCharacterTreeViewDataSourceMgr;



    List<InventoryTreeViewItemData> mItemDataList = new List<InventoryTreeViewItemData>();

    static InventoryTreeViewDataSourceMgr instance = null;
    int mTreeViewItemCount = 10;
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
        if (otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.itemCapacity > otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.itemList.Count)
        {

            //add item
            otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.itemList.Add(item);
            itemDataBase.SaveItemList("item-" + otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.affiliation
                + "-" + otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.uniqueID,
                otherCharacterTreeViewDataSourceMgr.characterStatusDisplay.itemList);

            //remove from other inventory
            for (int i = itemList.Count - 1; i >= 0; i--)
            {
                if (itemList[i] == item)
                {
                    itemList.RemoveAt(i);
                }
            }
            itemDataBase.SaveItemList("item-" + "inventory", itemList);

            //DoRefreshDataSource();
            //otherCharacterTreeViewDataSourceMgr.DoRefreshDataSource();
        }
    }



    public void DoRefreshDataSource()
    {
        mItemDataList.Clear();
        for (int i = 0; i < mTreeViewItemCount; ++i)
        {
            InventoryTreeViewItemData tData = new InventoryTreeViewItemData();
            tData.mName = "Item" + i;
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
                foreach (Item _item in itemList)
                {
                    tData.AddChild(_item);
                }
            }
        }
    }



}

