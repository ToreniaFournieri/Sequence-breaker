using System.Collections.Generic;
using UnityEngine;


sealed public class CharacterTreeViewItemData
{
    public string MName;
    public string MIcon;

    List<Item> _mChildItemDataList = new List<Item>();

    //public CharacterStatusDisplay characterStatusDisplay;

    //List<Item> mChildItemDataList = new List<Item>();

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

sealed public class CharacterTreeViewDataSourceMgr : MonoBehaviour
{
    // Item data base 
    public ItemDataBase itemDataBase;
    //public List<Item> itemList;

    public CharacterStatusDisplay characterStatusDisplay;

    // other inventory
    public InventoryTreeViewDataSourceMgr otherInventoryTreeViewDataSourceMgr;

    List<CharacterTreeViewItemData> _mItemDataList = new List<CharacterTreeViewItemData>();

    static CharacterTreeViewDataSourceMgr _instance = null;
    int _mTreeViewItemCount = 1;
    //int mTreeViewChildItemCount = 10;


    //int mTreeViewItemCount = 20;
    //int mTreeViewChildItemCount = 30;

    public static CharacterTreeViewDataSourceMgr Get
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindObjectOfType<CharacterTreeViewDataSourceMgr>();
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

    public void Show()
    {
        DoRefreshDataSource();

    }


    public CharacterTreeViewItemData GetItemDataByIndex(int index)
    {
        if (index < 0 || index >= _mItemDataList.Count)
        {
            return null;
        }
        return _mItemDataList[index];
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
            return _mItemDataList.Count;
        }
    }

    public int TotalTreeViewItemAndChildCount
    {
        get
        {
            int count = _mItemDataList.Count;
            int totalCount = 0;
            for (int i = 0; i < count; ++i)
            {
                totalCount = totalCount + _mItemDataList[i].ChildCount;
            }
            return totalCount;
        }
    }

    public void TryTransferItemToOtherInventory(Item item)
    {
        //inventory is infinity

        //add item
        otherInventoryTreeViewDataSourceMgr.inventoryItemList.AddItemAndSave(item);
        //itemDataBase.SaveItemList("item-" + "inventory", otherInventoryTreeViewDataSourceMgr.itemList);

        //remove from other inventory
        characterStatusDisplay.RemoveAndSaveItem(item);
        //for (int i = characterStatusDisplay.GetItemList().Count - 1; i >= 0; i--)
        //{
        //    if (characterStatusDisplay.GetItemList()[i] == item)
        //    {
        //        characterStatusDisplay.GetItemList().RemoveAt(i);
        //        continue;
        //    }
        //}
        //itemDataBase.SaveItemList("item-" + characterStatusDisplay.affiliation
        //    + "-" + characterStatusDisplay.uniqueID, characterStatusDisplay.GetItemList());


        characterStatusDisplay.RefleshCharacterStatusAndItemList();
        otherInventoryTreeViewDataSourceMgr.DoRefreshDataSource();

    }


    // this method is called from CharacterStatus Display . SetCharacterStatus
    private void DoRefreshDataSource()
    {


        _mItemDataList.Clear();
        for (int i = 0; i < _mTreeViewItemCount; ++i)
        {
            CharacterTreeViewItemData tData = new CharacterTreeViewItemData();
            tData.MName = "Character";
            //tData.mIcon = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 24));
            tData.MIcon = "1";
            _mItemDataList.Add(tData);
            //int childCount = mTreeViewChildItemCount;


            foreach (Item item in characterStatusDisplay.GetItemList())
            {
                if (item != null)
                {
                    //Debug.Log("characterStatusDisplay.itemList :" + _item.itemName);
                    tData.AddChild(item);
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

        //if (characterStatusDisplay.UnitList != null)
        //{
        //    characterStatusDisplay.RefleshCharacterStatus();
        //}


    }



}

