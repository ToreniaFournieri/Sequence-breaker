using System.Collections.Generic;
using SequenceBreaker._01_Data._02_Items.Item;
using SequenceBreaker._03_Controller._00_Global;
using SequenceBreaker._03_Controller._01_Home.Inventory;
using UnityEngine;

namespace SequenceBreaker._03_Controller._01_Home.Character
{
    public sealed class CharacterTreeViewItemData
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

    public sealed class CharacterTreeViewDataSourceMgr : MonoBehaviour
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
//            otherInventoryTreeViewDataSourceMgr.allyInventoryItemList.AddItemAndSave(item, 1);
            otherInventoryTreeViewDataSourceMgr.AddItemAndSave(item);

            //remove from other inventory
            characterStatusDisplay.RemoveAndSaveItem(item);
            

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
                tData.MIcon = "1";
                _mItemDataList.Add(tData);


                foreach (Item item in characterStatusDisplay.GetItemList())
                {
                    if (item != null)
                    {
                        tData.AddChild(item);
                    }
                }


            }


        }



    }
}