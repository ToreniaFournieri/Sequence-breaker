using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperScrollView;


public class CharacterTreeViewItemData
{
    public string mName;
    public string mIcon;

    List<Item> mChildItemDataList = new List<Item>();

    //public CharacterStatusDisplay characterStatusDisplay;

    //List<Item> mChildItemDataList = new List<Item>();

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

public class CharacterTreeViewDataSourceMgr : MonoBehaviour
{
    // Item data base 
    public ItemDataBase itemDataBase;
    //public List<Item> itemList;

    public CharacterStatusDisplay characterStatusDisplay;

    // other inventory
    public InventoryTreeViewDataSourceMgr otherInventoryTreeViewDataSourceMgr;

    List<CharacterTreeViewItemData> mItemDataList = new List<CharacterTreeViewItemData>();

    static CharacterTreeViewDataSourceMgr instance = null;
    int mTreeViewItemCount = 1;
    //int mTreeViewChildItemCount = 10;


    //int mTreeViewItemCount = 20;
    //int mTreeViewChildItemCount = 30;

    public static CharacterTreeViewDataSourceMgr Get
    {
        get
        {
            if (instance == null)
            {
                instance = Object.FindObjectOfType<CharacterTreeViewDataSourceMgr>();
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

    public void Show()
    {
        DoRefreshDataSource();

    }


    public CharacterTreeViewItemData GetItemDataByIndex(int index)
    {
        if (index < 0 || index >= mItemDataList.Count)
        {
            return null;
        }
        return mItemDataList[index];
    }

    public Item GetItemChildDataByIndex(int itemIndex, int childIndex)
    {
        CharacterTreeViewItemData data = GetItemDataByIndex(itemIndex);
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
        //inventory is infinity

        //add item
        otherInventoryTreeViewDataSourceMgr.itemList.Add(item);
        itemDataBase.SaveItemList("item-" + "inventory", otherInventoryTreeViewDataSourceMgr.itemList);

        //remove from other inventory
        for (int i = characterStatusDisplay.itemList.Count - 1; i >= 0; i--)
        {
            if (characterStatusDisplay.itemList[i] == item)
            {
                characterStatusDisplay.itemList.RemoveAt(i);
            }
        }
        itemDataBase.SaveItemList("item-" + characterStatusDisplay.affiliation
            + "-" + characterStatusDisplay.uniqueID, characterStatusDisplay.itemList);


        DoRefreshDataSource();
        otherInventoryTreeViewDataSourceMgr.DoRefreshDataSource();

    }


    public void DoRefreshDataSource()
    {
        mItemDataList.Clear();
        for (int i = 0; i < mTreeViewItemCount; ++i)
        {
            CharacterTreeViewItemData tData = new CharacterTreeViewItemData();
            tData.mName = "Character";
            //tData.mIcon = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 24));
            tData.mIcon = "1";
            mItemDataList.Add(tData);
            //int childCount = mTreeViewChildItemCount;


            foreach (Item _item in characterStatusDisplay.itemList)
            {
                if (_item != null)
                {
                    //Debug.Log("characterStatusDisplay.itemList :" + _item.itemName);
                    tData.AddChild(_item);
                }
            }

            //int childCount = itemDataBase.itemBaseMasterList.Count;
            //for (int j = 0; j < childCount; ++j)
            //{
            //    Item _item = new Item();
            //    _item.baseItem = itemDataBase.itemBaseMasterList[j];
            //    //ItemData childItemData = new ItemData();
            //    //childItemData.mName = "Item" + i + ":Child" + j;
            //    //childItemData.mDesc = "Item Desc For " + childItemData.mName;
            //    ////childItemData.mIcon = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 24));
            //    //childItemData.mIcon = "1";
            //    //childItemData.mStarCount = Random.Range(0, 6);
            //    //childItemData.mFileSize = Random.Range(20, 999);
            //    tData.AddChild(_item);
            //}
            //}

        }
    }



}

